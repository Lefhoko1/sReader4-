using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Domains.Assignments.Models;
using SReader.Domains.Assignments.Services;
using SReader.Domains.Education.Services;
using SReader.Domains.Files.Services;
using SReader.Domains.Identity.Services;
using SReader.UI.Bindings;

namespace SReader.UI.ViewModels
{
    /// <summary>How the My-assignments list is filtered.</summary>
    public enum AssignmentFilter { All, DueSoon, Overdue, Submitted, Completed, Scheduled }

    /// <summary>
    /// Backs the student Assignments module: it aggregates the assignments from
    /// every class the student is enrolled in, tags each with the student's own
    /// attempt status and schedule, and forwards attempt / submit / schedule /
    /// download actions. A null service = placeholder mode.
    /// </summary>
    public sealed class StudentAssignmentsViewModel : ViewModelBase
    {
        readonly IAssignmentService assignments;
        readonly IEducationService education;
        readonly IUserService users;
        readonly IFileStorage files;
        readonly CurrentSessionHolder session;
        readonly IAssignmentGameService games;
        readonly IGameRealtime realtime;   // optional instant fast-path
        readonly MultiplayerBackendSetting backend;   // which networking stack (Supabase / Photon)
        readonly IGameProgressStore progress;   // per-device resume points (solo + multiplayer)

        // Events pushed over the live connection, drained by the game view.
        readonly System.Collections.Concurrent.ConcurrentQueue<GameEvent> liveEvents
            = new System.Collections.Concurrent.ConcurrentQueue<GameEvent>();

        public IReadOnlyList<AssignmentView> MyAssignments { get; private set; } = new List<AssignmentView>();
        public IReadOnlyList<AssignmentSchedule> MySchedules { get; private set; } = new List<AssignmentSchedule>();
        public IReadOnlyList<AssignmentSchedule> AssignmentSchedules { get; private set; } = new List<AssignmentSchedule>();

        // Multiplayer.
        public IReadOnlyList<AssignmentGameSession> OpenSessions { get; private set; } = new List<AssignmentGameSession>();
        public IReadOnlyList<GameParticipant> Participants { get; private set; } = new List<GameParticipant>();
        public AssignmentGameSession CurrentSession { get; private set; }
        public string MyAvatarUrl { get; private set; } = "";

        /// <summary>Cursor into the shared event stream — last seq we've applied.</summary>
        public long EventCursor { get; private set; }

        /// <summary>Players whose heartbeat is fresh (present in the room right now).</summary>
        public IReadOnlyList<GameParticipant> ActivePlayers =>
            Participants.Where(p => p.IsActiveAt(DateTime.UtcNow, AssignmentGameService.PresenceGraceSeconds))
                        .OrderBy(p => p.TurnOrder).ToList();

        /// <summary>Class id → class name, for labelling and the class filter.</summary>
        public IReadOnlyDictionary<string, string> ClassNames { get; private set; } = new Dictionary<string, string>();

        public string StudentName { get; private set; } = "A student";
        public string CurrentUserId => session?.CurrentUserId;

        public StudentAssignmentsViewModel(IAssignmentService assignments = null, IEducationService education = null,
            IUserService users = null, IFileStorage files = null, CurrentSessionHolder session = null,
            IAssignmentGameService games = null, IGameRealtime realtime = null, MultiplayerBackendSetting backend = null,
            IGameProgressStore progress = null)
        {
            this.assignments = assignments;
            this.education = education;
            this.users = users;
            this.files = files;
            this.session = session;
            this.games = games;
            this.realtime = realtime;
            this.backend = backend;
            this.progress = progress;
        }

        // ── Networking backend selection (Supabase Realtime vs Photon PUN) ────
        /// <summary>True when Photon PUN is compiled in and can be chosen.</summary>
        public bool PhotonAvailable => backend != null && backend.PhotonAvailable;
        /// <summary>True while Photon is the active backend.</summary>
        public bool UsingPhoton => backend != null && backend.UsingPhoton;
        public string BackendLabel => backend != null ? backend.Label : "Supabase";

        /// <summary>Pick the multiplayer backend. Do this in the lobby, before a game
        /// is live; the choice is remembered on the device.</summary>
        public void SelectBackend(bool photon)
            => backend?.Set(photon ? MultiplayerBackend.Photon : MultiplayerBackend.Supabase);

        // ── Load + aggregate ─────────────────────────────────────────────────

        public async Task<Result> LoadAsync()
        {
            if (assignments == null || education == null) return PlaceholderOk();
            IsBusy = true;
            try
            {
                if (users != null)
                {
                    var me = await users.GetCurrentUserAsync();
                    if (me.IsSuccess && me.Value != null)
                    {
                        if (!string.IsNullOrWhiteSpace(me.Value.DisplayName))
                            StudentName = me.Value.DisplayName;

                        // My avatar — shown to others in the game room.
                        var profile = await users.GetProfileAsync(me.Value.Id);
                        if (profile.IsSuccess && profile.Value != null)
                            MyAvatarUrl = profile.Value.ProfileImageUrl ?? "";
                    }
                }

                var classesResult = await education.ListMyClassesAsync();
                if (classesResult.IsFailure) return Report(classesResult);

                var classNames = new Dictionary<string, string>();
                var all = new List<Assignment>();
                foreach (var enr in classesResult.Value)
                {
                    if (string.IsNullOrEmpty(enr.ClassId)) continue;
                    if (!classNames.ContainsKey(enr.ClassId)) classNames[enr.ClassId] = enr.ClassName;
                    var listed = await assignments.ListForClassAsync(enr.ClassId);
                    if (listed.IsSuccess) all.AddRange(listed.Value);
                }
                ClassNames = classNames;

                // My attempt status + schedule per assignment.
                var statusById = new Dictionary<string, AttemptStatus>();
                var attempts = await assignments.ListMyAttemptsAsync();
                if (attempts.IsSuccess)
                    foreach (var a in attempts.Value)
                        if (!string.IsNullOrEmpty(a.AssignmentId)) statusById[a.AssignmentId] = a.Status;

                var scheduleById = new Dictionary<string, AssignmentSchedule>();
                var mySchedules = await assignments.ListMySchedulesAsync();
                if (mySchedules.IsSuccess)
                {
                    MySchedules = mySchedules.Value;
                    foreach (var s in mySchedules.Value)
                        if (!string.IsNullOrEmpty(s.AssignmentId)) scheduleById[s.AssignmentId] = s;
                }

                var views = all
                    .GroupBy(a => a.Id).Select(g => g.First())   // de-dupe
                    .Select(a => new AssignmentView
                    {
                        Assignment = a,
                        Status = statusById.TryGetValue(a.Id, out var st) ? st : AttemptStatus.NotStarted,
                        MySchedule = scheduleById.TryGetValue(a.Id, out var sc) ? sc : null
                    })
                    .OrderBy(v => v.Assignment.DueDate)
                    .ToList();
                MyAssignments = views;
                return Report(Result.Ok());
            }
            finally { IsBusy = false; }
        }

        public IEnumerable<AssignmentView> Filtered(AssignmentFilter filter, string query, string classId)
        {
            IEnumerable<AssignmentView> q = MyAssignments;
            if (!string.IsNullOrEmpty(classId))
                q = q.Where(v => v.Assignment.ClassId == classId);

            switch (filter)
            {
                case AssignmentFilter.DueSoon:   q = q.Where(v => v.Assignment.IsDueSoon && !v.Submitted); break;
                case AssignmentFilter.Overdue:   q = q.Where(v => v.Assignment.IsOverdue && !v.Submitted); break;
                case AssignmentFilter.Submitted: q = q.Where(v => v.Submitted); break;
                case AssignmentFilter.Completed: q = q.Where(v => v.Completed); break;
                case AssignmentFilter.Scheduled: q = q.Where(v => v.IsScheduled); break;
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                var needle = query.Trim();
                q = q.Where(v => (v.Assignment.Title ?? "").IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            return q;
        }

        public string ClassNameFor(string classId)
            => classId != null && ClassNames.TryGetValue(classId, out var n) && !string.IsNullOrWhiteSpace(n) ? n : "Class";

        // ── Attempt / submit ─────────────────────────────────────────────────

        public async Task<Result> StartAttemptAsync(Assignment assignment)
        {
            if (assignments == null) return PlaceholderOk();
            IsBusy = true;
            var r = await assignments.StartAttemptAsync(assignment.Id, session.CurrentUserId);
            IsBusy = false;
            return Report(r);
        }

        /// <summary>Reset an assignment so the student can play it again, even after
        /// submitting. Also clears any saved resume point so it starts fresh.</summary>
        public async Task<Result> ResetAttemptAsync(Assignment assignment)
        {
            if (assignments == null) return PlaceholderOk();
            IsBusy = true;
            var r = await assignments.ResetAttemptAsync(assignment.Id, session.CurrentUserId);
            if (r.IsSuccess) ClearProgress(assignment.Id);
            IsBusy = false;
            return Report(r);
        }

        public async Task<Result> SubmitAsync(Assignment assignment, string reference)
        {
            if (assignments == null) return PlaceholderOk();
            IsBusy = true;
            var r = await assignments.SubmitAsync(assignment.Id, session.CurrentUserId, reference);
            IsBusy = false;
            return Report(r);
        }

        // ── Game content + result ────────────────────────────────────────────

        public AssignmentContent ContentFor(Assignment assignment)
            => AssignmentContentCodec.FromJson(assignment?.ContentJson);

        /// <param name="note">Optional extra detail appended to the submission reference
        /// (e.g. the Trek's per-word hint usage, R-10) — additive, same path.</param>
        public async Task<Result> RecordGameResultAsync(Assignment assignment, int score, int total, string note = null)
        {
            if (assignments == null) return PlaceholderOk();
            IsBusy = true;
            var reference = $"Game score: {score}/{total}" + (string.IsNullOrWhiteSpace(note) ? "" : " · " + note);
            var r = await assignments.SubmitAsync(assignment.Id, session.CurrentUserId, reference);
            IsBusy = false;
            return Report(r);
        }

        // ── Resume: record every step so a student can leave and continue ─────

        /// <summary>The saved resume point for this assignment on this device, or null.</summary>
        public GameProgress LoadProgress(string assignmentId)
            => progress?.Load(session?.CurrentUserId, assignmentId);

        /// <summary>Record the current board state (every solved step) so the game can be resumed.</summary>
        public void SaveProgress(string assignmentId, bool multiplayer, IReadOnlyList<int> solvedIndices,
            int score, int total, bool finished)
        {
            if (progress == null || string.IsNullOrEmpty(assignmentId)) return;
            progress.Save(new GameProgress
            {
                UserId        = session?.CurrentUserId,
                AssignmentId  = assignmentId,
                Mode          = multiplayer ? "multiplayer" : "solo",
                SessionId     = multiplayer ? CurrentSession?.Id : null,
                SolvedIndices = solvedIndices != null ? solvedIndices.ToList() : new List<int>(),
                Score         = score,
                Total         = total,
                Finished      = finished
            });
        }

        /// <summary>Forget the resume point (the game was submitted / finished).</summary>
        public void ClearProgress(string assignmentId)
            => progress?.Clear(session?.CurrentUserId, assignmentId);

        // ── Multiplayer: host / join / leave ─────────────────────────────────

        // Make sure my profile picture is loaded before I appear in a room — the
        // avatar is stored on my participant row at host/join time, so it must be
        // ready by then (LoadAsync may not have finished, or ran before login).
        async Task EnsureMyAvatarAsync()
        {
            if (!string.IsNullOrEmpty(MyAvatarUrl) || users == null) return;
            var id = session?.CurrentUserId;
            if (string.IsNullOrEmpty(id)) return;
            var profile = await users.GetProfileAsync(id);
            if (profile.IsSuccess && profile.Value != null)
                MyAvatarUrl = profile.Value.ProfileImageUrl ?? "";
        }

        public async Task<Result> HostSessionAsync(Assignment assignment)
        {
            if (games == null) return Fail("Multiplayer isn't available here.");
            await EnsureMyAvatarAsync();
            IsBusy = true;
            var r = await games.HostAsync(assignment, StudentName, MyAvatarUrl);
            IsBusy = false;
            if (r.IsSuccess) { CurrentSession = r.Value; EventCursor = 0; JoinRealtime(); }
            return Report(r);
        }

        // ── Realtime (instant) fast-path ─────────────────────────────────────
        void JoinRealtime()
        {
            if (realtime == null || CurrentSession == null) return;
            _ = realtime.JoinAsync(CurrentSession.Id, OnRealtimeEvent);
        }

        // A live event from another player (fires on the main thread). Apply the
        // bits that change shared state, then queue it for the game view to render.
        void OnRealtimeEvent(GameEvent e)
        {
            if (e == null || CurrentSession == null) return;
            if (e.Seq > EventCursor) EventCursor = e.Seq;   // keep the poll from re-delivering it

            if (e.Type == GameEventType.Turn && !string.IsNullOrEmpty(e.Payload))
                CurrentSession.CurrentTurnId = e.Payload;
            else if (e.Type == GameEventType.Solve || e.Type == GameEventType.Finish)
            {
                var p = Participants.FirstOrDefault(x => x.StudentId == e.ActorId);
                if (p != null)
                {
                    if (e.ActorScore > 0) p.Score = e.ActorScore;
                    if (e.Type == GameEventType.Finish) p.Finished = true;
                }
            }
            liveEvents.Enqueue(e);
        }

        /// <summary>Pull (and clear) the events pushed over the live connection.</summary>
        public IReadOnlyList<GameEvent> DrainLiveEvents()
        {
            if (liveEvents.IsEmpty) return System.Array.Empty<GameEvent>();
            var list = new List<GameEvent>();
            while (liveEvents.TryDequeue(out var e)) list.Add(e);
            return list;
        }

        public bool RealtimeConnected => realtime != null && realtime.Connected;

        public async Task<Result> LoadOpenSessionsAsync(string assignmentId)
        {
            if (games == null) return PlaceholderOk();
            IsBusy = true;
            var r = await games.ListOpenSessionsAsync(assignmentId);
            IsBusy = false;
            if (r.IsSuccess) OpenSessions = r.Value;
            return Report(r);
        }

        public async Task<Result> JoinByCodeAsync(string code)
        {
            if (games == null) return Fail("Multiplayer isn't available here.");
            await EnsureMyAvatarAsync();
            IsBusy = true;
            var r = await games.JoinByCodeAsync(code, StudentName, MyAvatarUrl);
            IsBusy = false;
            if (r.IsSuccess) { CurrentSession = r.Value; EventCursor = 0; JoinRealtime(); }
            return Report(r);
        }

        public async Task<Result> JoinSessionAsync(AssignmentGameSession gameSession)
        {
            if (games == null) return Fail("Multiplayer isn't available here.");
            await EnsureMyAvatarAsync();
            IsBusy = true;
            var r = await games.JoinSessionAsync(gameSession, StudentName, MyAvatarUrl);
            IsBusy = false;
            if (r.IsSuccess) { CurrentSession = gameSession; EventCursor = 0; JoinRealtime(); }
            return Report(r);
        }

        public async Task<Result> LeaveSessionAsync()
        {
            if (realtime != null && CurrentSession != null) realtime.Leave(CurrentSession.Id);
            if (games == null || CurrentSession == null) { CurrentSession = null; return Result.Ok(); }
            IsBusy = true;
            var r = await games.LeaveSessionAsync(CurrentSession.Id);
            IsBusy = false;
            CurrentSession = null;
            return Report(r);
        }

        // ── Multiplayer: presence + room refresh ─────────────────────────────

        public Task<Result> HeartbeatAsync()
        {
            if (games == null || CurrentSession == null) return Task.FromResult(Result.Ok());
            return games.HeartbeatAsync(CurrentSession.Id);
        }

        /// <summary>Refresh the roster + the shared turn state, and (if I'm the
        /// designated leader) skip a turn that's stuck on a disconnected player.</summary>
        public async Task<Result> RefreshRoomAsync()
        {
            if (games == null || CurrentSession == null) return PlaceholderOk();

            var players = await games.ListParticipantsAsync(CurrentSession.Id);
            if (players.IsSuccess) Participants = players.Value;

            // Self-heal: if my row is missing its avatar (e.g. I joined before the
            // profile picture loaded), push it now so my face shows on every screen.
            await HealMyAvatarAsync();

            var sess = await games.RefreshSessionAsync(CurrentSession.Id);
            if (sess.IsSuccess && sess.Value != null) CurrentSession = sess.Value;

            if (games.ShouldDriveAutoAdvance(CurrentSession, Participants))
                await games.PassTurnAsync(CurrentSession, Participants);

            return Result.Ok();
        }

        bool avatarHealed;
        async Task HealMyAvatarAsync()
        {
            if (avatarHealed || CurrentSession == null) return;
            await EnsureMyAvatarAsync();
            if (string.IsNullOrEmpty(MyAvatarUrl)) return;

            var mine = Participants.FirstOrDefault(p => p.StudentId == CurrentUserId);
            if (mine == null || !string.IsNullOrEmpty(mine.AvatarUrl)) { avatarHealed = true; return; }

            // Avatar-only PATCH — does not touch my score, turn order or cursor.
            await games.UpdateMyAvatarAsync(CurrentSession.Id, MyAvatarUrl);
            avatarHealed = true;
        }

        // Kept for the lobby's simple player list refresh.
        public async Task<Result> LoadParticipantsAsync()
        {
            if (games == null || CurrentSession == null) return PlaceholderOk();
            var r = await games.ListParticipantsAsync(CurrentSession.Id);
            if (r.IsSuccess) Participants = r.Value;
            return Report(r);
        }

        // ── Multiplayer: turn-based board ────────────────────────────────────

        public async Task<Result> StartGameAsync()
        {
            if (games == null || CurrentSession == null) return Fail("No session.");
            await LoadParticipantsAsync();
            IsBusy = true;
            var r = await games.StartGameAsync(CurrentSession, Participants);
            IsBusy = false;
            if (r.IsSuccess) await RefreshRoomAsync();
            return Report(r);
        }

        public Task<Result> PassTurnAsync()
        {
            if (games == null || CurrentSession == null) return Task.FromResult(Result.Ok());
            return games.PassTurnAsync(CurrentSession, Participants);
        }

        public Task ReportEventAsync(string type, int tokenIndex, string payload)
        {
            if (games == null || CurrentSession == null) return Task.CompletedTask;
            return games.ReportEventAsync(CurrentSession.Id, type, tokenIndex, payload);
        }

        public Task ReportSolveAsync(int tokenIndex, int points, int totalScore, bool finished)
        {
            if (games == null || CurrentSession == null) return Task.CompletedTask;
            return games.ReportSolveAsync(CurrentSession.Id, tokenIndex, points, totalScore, finished);
        }

        public Task ReportFinishAsync(int totalScore)
        {
            if (games == null || CurrentSession == null) return Task.CompletedTask;
            return games.ReportFinishAsync(CurrentSession.Id, totalScore);
        }

        /// <summary>Pull events newer than our cursor and advance it.</summary>
        public async Task<IReadOnlyList<GameEvent>> PullEventsAsync()
        {
            if (games == null || CurrentSession == null) return Array.Empty<GameEvent>();
            var r = await games.PullEventsAsync(CurrentSession.Id, EventCursor);
            if (r.IsFailure || r.Value == null || r.Value.Count == 0) return Array.Empty<GameEvent>();
            EventCursor = r.Value[r.Value.Count - 1].Seq;
            return r.Value;
        }

        public bool IsMyTurn => games != null && games.IsMyTurn(CurrentSession);
        public bool IsHost => CurrentSession != null && CurrentSession.HostId == session?.CurrentUserId;
        public string CurrentTurnId => CurrentSession?.CurrentTurnId;

        public string CurrentTurnName
        {
            get
            {
                var id = CurrentSession?.CurrentTurnId;
                if (string.IsNullOrEmpty(id)) return "";
                var p = Participants.FirstOrDefault(x => x.StudentId == id);
                return p != null ? (string.IsNullOrWhiteSpace(p.StudentName) ? "A student" : p.StudentName) : "";
            }
        }

        // ── Scheduling ───────────────────────────────────────────────────────

        public async Task<Result> ScheduleAsync(Assignment assignment, DateTime when, string note, bool hidden)
        {
            if (assignments == null) return PlaceholderOk();
            IsBusy = true;
            var r = await assignments.ScheduleAsync(assignment, when, note, StudentName, hidden);
            IsBusy = false;
            return Report(r);
        }

        public async Task<Result> ToggleScheduleHiddenAsync(Assignment assignment, AssignmentSchedule schedule)
        {
            if (assignments == null) return PlaceholderOk();
            IsBusy = true;
            var r = await assignments.ScheduleAsync(assignment, schedule.ScheduledFor, schedule.Note, StudentName, !schedule.Hidden);
            IsBusy = false;
            return Report(r);
        }

        public async Task<Result> UnscheduleAsync(string scheduleId)
        {
            if (assignments == null) return PlaceholderOk();
            IsBusy = true;
            var r = await assignments.UnscheduleAsync(scheduleId);
            IsBusy = false;
            return Report(r);
        }

        public async Task<Result> LoadSchedulesForAsync(string assignmentId)
        {
            if (assignments == null) return PlaceholderOk();
            IsBusy = true;
            var r = await assignments.ListSchedulesForAssignmentAsync(assignmentId);
            IsBusy = false;
            if (r.IsSuccess) AssignmentSchedules = r.Value;
            return Report(r);
        }

        // ── Download to local device ─────────────────────────────────────────

        public async Task<bool> IsDownloadedAsync(string assignmentId)
            => files != null && await files.ExistsAsync(PathFor(assignmentId));

        public async Task<Result> DownloadAsync(Assignment assignment)
        {
            if (files == null) return Fail("Download isn't available here.");

            // Until the structured content schema lands we save what we have, so
            // the assignment is still available offline on this device.
            var payload = assignment.HasContent
                ? assignment.ContentJson
                : "{\"title\":" + Json(assignment.Title) + ",\"instructions\":" + Json(assignment.Instructions)
                    + ",\"due\":" + Json(assignment.DueDate.ToString("o")) + ",\"maxScore\":" + assignment.MaxScore + "}";

            IsBusy = true;
            var r = await files.SaveAsync(PathFor(assignment.Id), Encoding.UTF8.GetBytes(payload ?? ""));
            IsBusy = false;
            return Report(r);
        }

        static string PathFor(string assignmentId) => "assignments/" + assignmentId + ".json";
        static string Json(string s) => "\"" + (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";

        Result Report(Result result)
        {
            ErrorMessage = result.IsFailure ? result.Error : "";
            return result;
        }

        Result Report<T>(Result<T> result)
        {
            ErrorMessage = result.IsFailure ? result.Error : "";
            return result.IsSuccess ? Result.Ok() : Result.Fail(result.Error);
        }

        Result Fail(string message) { ErrorMessage = message; return Result.Fail(message); }
    }
}
