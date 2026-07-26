using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using SReader.Domains.Assignments.Models;
using SReader.Game.Trek;
using SReader.UI.Navigation;
using SReader.UI.ViewModels;

namespace SReader.UI.Views.Home
{
    /// <summary>
    /// The student Assignments module. One concern per screen, navigation-first:
    ///   • home mini-nav: My assignments · Search · Schedules
    ///   • My assignments: filter pills (All / Due soon / Overdue / Submitted /
    ///     Completed / Scheduled) + a class filter; cards open the profile.
    ///   • Assignment PROFILE (the heart of this module): details, status, and
    ///     actions — Attempt, Submit, Schedule, Download — plus "who's planning
    ///     to do it" (classmates / friends, unless they hid their schedule).
    /// Content rendering arrives when the JSON schema lands; for now the profile
    /// shows the metadata and can still be downloaded for offline use.
    /// </summary>
    internal sealed class StudentAssignmentsView
    {
        readonly StudentAssignmentsViewModel vm;
        readonly Action onFindClass;   // jump the student to "Discover" to enrol (optional)
        readonly Action<AssignmentView> onOpenTrek;   // launch the Story Trek for an assignment (optional)
        VisualElement content;

        // Shared back stack for the everyday flow (list ↔ profile ↔ submit ↔
        // schedule). "‹ Back" and the Android system back pop this, returning the
        // student to exactly where they were — same mini-nav tab, same data.
        readonly NavStack nav = new NavStack();
        int homeTab;        // remembered mini-nav tab (My assignments / Search / Schedules)
        bool playMode;      // true while in the play flow (mode-select / lobby / room / game)
        string pendingOpenId;   // a home-dashboard deep-link to open once data has loaded

        AssignmentFilter filter = AssignmentFilter.All;
        string classFilter;   // null = all classes

        public StudentAssignmentsView(StudentAssignmentsViewModel vm, Action onFindClass = null, Action<AssignmentView> onOpenTrek = null)
        {
            this.vm = vm;
            this.onFindClass = onFindClass;
            this.onOpenTrek = onOpenTrek;
        }

        public void Render(VisualElement contentArea)
        {
            content = contentArea;
            GoHome();
        }

        // ── Back-stack entry points ──────────────────────────────────────────
        // Root of the trail: the assignments home with its last-used tab.
        void GoHome()
        {
            playMode = false;
            nav.Root(() => ShowHome(homeTab));
        }

        // Returns to an assignment's profile with a clean two-deep trail
        // (home → profile), used when coming back out of the play/game flow.
        void BackToProfile(AssignmentView v)
        {
            playMode = false;
            nav.RootSilent(() => ShowHome(homeTab));   // seed the list as back target, don't render it
            nav.Push(() => ShowProfile(v));
        }

        /// <summary>
        /// Handle an Android/system back press. While in the play flow the game
        /// has its own on-screen Exit (and may need to leave a live session), so
        /// we swallow the press there; otherwise we pop one screen. Returns false
        /// only when already at the assignments home, letting the shell decide
        /// (switch to the home section, or confirm-to-exit).
        /// </summary>
        public bool HandleBack()
        {
            if (playMode) return true;   // use the game's on-screen Exit
            return nav.Back();
        }

        /// <summary>
        /// Open straight to an assignment's profile (used by the home dashboard's
        /// "Resume" / "Open" deep-links). Deferred until the list has loaded, then
        /// pushed on top of the home so "‹ Back" still returns to the list.
        /// </summary>
        public void OpenAssignment(string assignmentId) => pendingOpenId = assignmentId;

        /// <summary>
        /// Open the assignments list pre-filtered to one class (used by the home
        /// dashboard's "My classes" tiles). Applied before the first render.
        /// </summary>
        public void OpenClass(string classId)
        {
            classFilter = classId;
            filter = AssignmentFilter.All;
            homeTab = 0;   // land on "My assignments"
        }

        // ── Home mini-nav ────────────────────────────────────────────────────
        async void ShowHome(int activeTab)
        {
            content.Clear();
            content.Add(HomeUI.Heading("Assignments"));

            var menu = HomeUI.Row();
            menu.style.marginTop = 4;
            menu.style.marginBottom = 6;
            content.Add(menu);
            var loading = HomeUI.Caption("Loading…");
            content.Add(loading);
            var tabBox = new VisualElement();
            content.Add(tabBox);

            var result = await vm.LoadAsync();
            loading.RemoveFromHierarchy();
            if (result.IsFailure) { tabBox.Add(HomeUI.Status(vm.ErrorMessage, true)); }

            var tabs = new[] { "My assignments", "Search", "Schedules" };
            var buttons = new Button[tabs.Length];
            Action<int> select = i =>
            {
                homeTab = i;   // remember the tab so a later "‹ Back" restores it
                for (int j = 0; j < buttons.Length; j++) HomeUI.StyleTab(buttons[j], j == i);
                tabBox.Clear();
                if (tabs[i] == "Search")         RenderSearch(tabBox);
                else if (tabs[i] == "Schedules") RenderMySchedules(tabBox);
                else                             RenderMyAssignments(tabBox);
            };
            for (int i = 0; i < tabs.Length; i++)
            {
                int idx = i;
                buttons[i] = new Button(() => select(idx)) { text = tabs[i] };
                menu.Add(buttons[i]);
            }
            select(activeTab);

            // Honour a pending home-dashboard deep-link now that the list is loaded.
            if (pendingOpenId != null)
            {
                var open = MyAssignmentFor(pendingOpenId);
                pendingOpenId = null;
                if (open != null) nav.Push(() => ShowProfile(open));
            }
        }

        // ── My assignments + filters ─────────────────────────────────────────
        void RenderMyAssignments(VisualElement box)
        {
            box.Clear();

            // Filter pills laid out as an even 3-column grid: equal width and
            // height, evenly spaced — a tidy segmented filter rather than a ragged
            // wrap of different-sized chips.
            var pills = new VisualElement();
            pills.style.flexDirection = FlexDirection.Row;
            pills.style.flexWrap = Wrap.Wrap;
            pills.style.justifyContent = Justify.SpaceBetween;
            pills.style.marginBottom = 10;
            void AddFilter(string label, AssignmentFilter f)
            {
                var p = HomeUI.Chip(label, null, false);
                p.style.flexGrow = 0;
                p.style.flexShrink = 0;
                p.style.flexBasis = Length.Percent(31.5f);   // 3 per row
                p.style.height = 36;
                p.style.fontSize = 13;
                p.style.paddingLeft = p.style.paddingRight = 2;
                p.style.marginLeft = p.style.marginRight = 0;
                p.style.marginBottom = 9;
                if (filter == f) { p.style.backgroundColor = HomeUI.Parchment; p.style.color = HomeUI.Bg; }
                p.clicked += () => { filter = f; RenderMyAssignments(box); };
                pills.Add(p);
            }
            AddFilter("All", AssignmentFilter.All);
            AddFilter("Due soon", AssignmentFilter.DueSoon);
            AddFilter("Overdue", AssignmentFilter.Overdue);
            AddFilter("Submitted", AssignmentFilter.Submitted);
            AddFilter("Completed", AssignmentFilter.Completed);
            AddFilter("Scheduled", AssignmentFilter.Scheduled);
            box.Add(pills);

            // Class filter (only when enrolled in more than one class).
            if (vm.ClassNames.Count > 1)
            {
                var choices = new List<string> { "All classes" };
                choices.AddRange(vm.ClassNames.Values);
                var dd = HomeUI.Dropdown(choices.ToArray());
                dd.value = classFilter == null ? "All classes" : vm.ClassNameFor(classFilter);
                dd.OnChanged = picked =>
                {
                    classFilter = picked == "All classes"
                        ? null
                        : vm.ClassNames.FirstOrDefault(kv => kv.Value == picked).Key;
                    RenderMyAssignments(box);
                };
                box.Add(dd);
            }

            var list = vm.Filtered(filter, null, classFilter).ToList();
            RenderAssignmentCards(box, list);
        }

        void RenderSearch(VisualElement box)
        {
            box.Add(HomeUI.FieldLabel("Search assignments by title"));
            var field = HomeUI.Field(null);
            box.Add(field);
            var results = new VisualElement();
            var search = HomeUI.Primary("Search", () =>
            {
                results.Clear();
                RenderAssignmentCards(results, vm.Filtered(AssignmentFilter.All, field.value, null).ToList());
            });
            search.style.marginTop = 4;
            search.style.marginBottom = 8;
            box.Add(search);
            box.Add(results);
        }

        void RenderAssignmentCards(VisualElement box, List<AssignmentView> list)
        {
            if (list.Count == 0)
            {
                box.Add(HomeUI.Caption("No assignments here. Enrol in a class (by paying for one of its subjects) to receive assignments."));
                // Don't strand a student with no assignments — give them a direct
                // jump to Discover where they can find a class and enrol.
                if (onFindClass != null)
                {
                    var find = HomeUI.Primary("Find a class to enrol", () => onFindClass());
                    find.style.marginTop = 8;
                    box.Add(find);
                }
                return;
            }

            // The list is the Quest Map: islands along a due-date-ordered trail.
            QuestMapView.Render(box, list, vm, v => nav.Push(() => ShowProfile(v)));
        }

        // ── Assignment profile ───────────────────────────────────────────────
        async void ShowProfile(AssignmentView v)
        {
            var a = v.Assignment;
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => nav.Back()));

            var head = HomeUI.Row();
            var title = HomeUI.Heading(a.Title);
            title.style.flexGrow = 1;
            head.Add(title);
            head.Add(Badge(v));
            content.Add(head);

            var info = HomeUI.Card();
            info.Add(HomeUI.FieldLabel("Class"));
            info.Add(HomeUI.Sub("🏫 " + vm.ClassNameFor(a.ClassId)));
            info.Add(HomeUI.FieldLabel("Due"));
            // Storm-cloud urgency with the printed date (spec Phase 2).
            string dueGlyph = (a.IsOverdue && !v.Submitted) ? "⛈" : (a.IsDueSoon && !v.Submitted) ? "🌥" : "📅";
            var dueLine = HomeUI.Sub(dueGlyph + " " + a.DueDate.ToLocalTime().ToString("dddd dd MMM yyyy, HH:mm")
                + (a.IsOverdue && !v.Submitted ? "  ·  overdue" : a.IsDueSoon && !v.Submitted ? "  ·  due soon" : ""));
            if (!v.Submitted && (a.IsOverdue || a.IsDueSoon)) dueLine.style.color = a.IsOverdue ? HomeUI.Danger : HomeUI.Parchment;
            info.Add(dueLine);
            info.Add(HomeUI.FieldLabel("Marks"));
            info.Add(HomeUI.Sub(a.MaxScore > 0 ? "🏆 Out of " + a.MaxScore : "Ungraded"));
            // The actual instructions / reading content are deliberately NOT shown
            // here — a student could read or copy the answers without attempting.
            // They appear only on the attempt screen (see ShowModeSelect) and in the game.
            bool hasBrief = !string.IsNullOrWhiteSpace(a.Instructions) || a.HasContent;
            info.Add(HomeUI.FieldLabel("Instructions"));
            info.Add(HomeUI.Sub(hasBrief ? "🔒 Available when you start the attempt." : "No instructions yet."));
            content.Add(info);

            // Primary actions as compact pills.
            var actions = HomeUI.WrapRow();
            actions.style.marginTop = 4;
            if (!v.Submitted)
                actions.Add(Pill(v.Started ? "Continue attempt" : "Start attempt", async () =>
                {
                    var r = await vm.StartAttemptAsync(a);
                    if (r.IsFailure) { content.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
                    v.Status = AttemptStatus.InProgress;
                    ShowModeSelect(v);   // always navigate; the screen explains if there's no content yet
                }));
            if (v.Started && !v.Submitted)
                actions.Add(Pill("Submit", () => nav.Push(() => ShowSubmit(v))));
            // Once finished, let the student reset and play it again for practice.
            if (v.Completed)
                actions.Add(Pill("Play again", async () =>
                {
                    var reset = await vm.ResetAttemptAsync(a);
                    TrekCheckpointStore.Clear(vm.CurrentUserId, a.Id);   // also wipe the trek side-car
                    if (reset.IsFailure) { content.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
                    var started = await vm.StartAttemptAsync(a);
                    if (started.IsFailure) { content.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
                    v.Status = AttemptStatus.InProgress;
                    ShowModeSelect(v);
                }));
            // Story Trek (the 2D reading game) — shown only when a launcher is wired.
            // Opens the Trek scene page for this assignment; Back returns here.
            if (onOpenTrek != null && !v.Submitted)
                actions.Add(Pill(v.Started ? "▶ Resume trek" : "▶ Story Trek", () => onOpenTrek(v)));
            // Read first (R-3): read the whole passage, untimed, before the trek starts.
            if (!v.Submitted && a.HasContent)
                actions.Add(Pill("📖 Read first", () => ReadFirst(v)));
            actions.Add(Pill(v.IsScheduled ? "Edit schedule" : "Schedule", () => nav.Push(() => ShowSchedule(v))));
            actions.Add(Pill("Download", async () =>
            {
                var r = await vm.DownloadAsync(a);
                content.Add(HomeUI.Status(r.IsSuccess ? "Downloaded to this device ✓" : vm.ErrorMessage, r.IsFailure));
            }));
            // Reset trail: wipe the attempt AND the trek checkpoint (solved gates +
            // the deferred-queue side-car), so the trek starts fresh (spec Phase 2).
            if ((v.Started || v.Completed) && !v.Submitted)
                actions.Add(Pill("♻ Reset trail", async () =>
                {
                    var r = await vm.ResetAttemptAsync(a);
                    TrekCheckpointStore.Clear(vm.CurrentUserId, a.Id);
                    if (r.IsFailure) { content.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
                    v.Status = AttemptStatus.NotStarted;
                    ShowProfile(v);   // re-render the sheet in its reset state (same stack entry)
                }, danger: true));
            content.Add(actions);

            // Who's planning to do it (classmates / friends, unless hidden).
            var planBox = new VisualElement();
            planBox.style.marginTop = 12;
            content.Add(planBox);
            await RenderPlanWithOthers(planBox, v);
        }

        // Read first (R-3): the whole passage as a scrollable overlay, untimed,
        // before any attempt begins. Not-yet-won key words render sealed. From
        // here the student can start the trek, which begins the attempt.
        readonly ReadingViewController readFirst = new ReadingViewController();
        void ReadFirst(AssignmentView v)
        {
            var trail = TrailBuilder.Build(v.Assignment.Id, v.Assignment.Title, vm.ContentFor(v.Assignment));
            (string label, Action onStart)? start = null;
            if (onOpenTrek != null)
                start = ("▶ Start the trek", () => { readFirst.Close(); onOpenTrek(v); });
            readFirst.Open(content, trail, currentSentence: 0, onClose: null, startAction: start);
        }

        async System.Threading.Tasks.Task RenderPlanWithOthers(VisualElement box, AssignmentView v)
        {
            box.Clear();
            box.Add(HomeUI.Title("Plan with others"));
            box.Add(HomeUI.Caption("Classmates and friends who scheduled this — unless they hid it."));

            var loading = HomeUI.Caption("Loading…");
            box.Add(loading);
            var r = await vm.LoadSchedulesForAsync(v.Assignment.Id);
            loading.RemoveFromHierarchy();
            if (r.IsFailure) { box.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }

            var others = vm.AssignmentSchedules.Where(s => s.StudentId != vm.CurrentUserId).ToList();
            if (others.Count == 0) { box.Add(HomeUI.Caption("No one else has scheduled this yet.")); return; }

            foreach (var s in others)
            {
                var card = HomeUI.Card();
                card.Add(HomeUI.Title(string.IsNullOrWhiteSpace(s.StudentName) ? "A student" : s.StudentName, 14));
                card.Add(HomeUI.Sub("🗓 " + s.ScheduledFor.ToLocalTime().ToString("ddd dd MMM, HH:mm")));
                if (!string.IsNullOrWhiteSpace(s.Note)) card.Add(HomeUI.Sub(s.Note));
                box.Add(card);
            }
        }

        // ── Submit ───────────────────────────────────────────────────────────
        void ShowSubmit(AssignmentView v)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => nav.Back()));
            content.Add(HomeUI.Heading("Submit assignment"));
            content.Add(HomeUI.Caption(v.Assignment.Title));

            content.Add(HomeUI.FieldLabel("Note / reference (a link or message for your tutor)"));
            var reference = HomeUI.Field(null, multiline: true);
            content.Add(reference);
            content.Add(HomeUI.Sub("File attachments arrive with the content update."));

            var status = HomeUI.Status("", true);
            status.style.display = DisplayStyle.None;
            content.Add(status);

            var submit = HomeUI.Primary("Submit", async () =>
            {
                var r = await vm.SubmitAsync(v.Assignment, reference.value);
                if (r.IsSuccess) GoHome();
                else { status.text = vm.ErrorMessage; status.style.display = DisplayStyle.Flex; }
            });
            submit.style.marginTop = 8;
            content.Add(submit);
        }

        // ── Schedule ─────────────────────────────────────────────────────────
        void ShowSchedule(AssignmentView v)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => nav.Back()));
            content.Add(HomeUI.Heading(v.IsScheduled ? "Edit schedule" : "Schedule assignment"));
            content.Add(HomeUI.Caption(v.Assignment.Title));

            var existing = v.MySchedule;
            var initialWhen = existing?.ScheduledFor.ToLocalTime()
                              ?? (v.Assignment.DueDate != default ? v.Assignment.DueDate.ToLocalTime().AddDays(-1) : DateTime.Now.AddDays(1));

            content.Add(HomeUI.FieldLabel("Date (yyyy-MM-dd)"));
            var date = HomeUI.Field(initialWhen.ToString("yyyy-MM-dd"));
            content.Add(date);
            content.Add(HomeUI.FieldLabel("Time (HH:mm)"));
            var time = HomeUI.Field(initialWhen.ToString("HH:mm"));
            content.Add(time);
            content.Add(HomeUI.FieldLabel("Note (optional)"));
            var note = HomeUI.Field(existing?.Note, multiline: true);
            content.Add(note);

            var hide = new Toggle("Hide from classmates and friends") { value = existing?.Hidden ?? false };
            hide.style.marginTop = 6;
            hide.style.color = HomeUI.Text;
            content.Add(hide);

            var status = HomeUI.Status("", true);
            status.style.display = DisplayStyle.None;
            content.Add(status);

            var save = HomeUI.Primary(v.IsScheduled ? "Update schedule" : "Save schedule", async () =>
            {
                if (!DateTime.TryParse(date.value + " " + time.value, out var when))
                {
                    status.text = "Enter a valid date (yyyy-MM-dd) and time (HH:mm).";
                    status.style.display = DisplayStyle.Flex;
                    return;
                }
                var r = await vm.ScheduleAsync(v.Assignment, when.ToUniversalTime(), note.value, hide.value);
                if (r.IsSuccess) GoHome();
                else { status.text = vm.ErrorMessage; status.style.display = DisplayStyle.Flex; }
            });
            save.style.marginTop = 8;
            content.Add(save);

            if (v.IsScheduled)
            {
                var remove = HomeUI.Chip("Remove schedule", async () =>
                {
                    var r = await vm.UnscheduleAsync(existing.Id);
                    if (r.IsSuccess) GoHome();
                    else content.Add(HomeUI.Status(vm.ErrorMessage, true));
                }, danger: true);
                remove.style.marginTop = 10;
                content.Add(remove);
            }
        }

        // ── My schedules tab ─────────────────────────────────────────────────
        void RenderMySchedules(VisualElement box)
        {
            box.Add(HomeUI.Caption("Your assignment plans. Tap one to open its assignment."));

            if (vm.MySchedules.Count == 0)
            {
                box.Add(HomeUI.Caption("You haven't scheduled any assignments yet."));
                return;
            }

            foreach (var s in vm.MySchedules.OrderBy(x => x.ScheduledFor))
            {
                var captured = s;
                var card = HomeUI.Card();
                card.Add(HomeUI.Title(string.IsNullOrWhiteSpace(s.AssignmentTitle) ? "Assignment" : s.AssignmentTitle, 15));
                card.Add(HomeUI.Sub("🗓 " + s.ScheduledFor.ToLocalTime().ToString("ddd dd MMM yyyy, HH:mm")));
                card.Add(HomeUI.Sub(s.Hidden ? "🔒 Hidden from others" : "👁 Visible to classmates & friends"));
                if (!string.IsNullOrWhiteSpace(s.Note)) card.Add(HomeUI.Sub(s.Note));

                var view = MyAssignmentFor(captured.AssignmentId);
                if (view != null)
                {
                    var open = HomeUI.Chip("Open assignment", () => nav.Push(() => ShowProfile(view)));
                    open.style.marginTop = 8;
                    card.Add(open);
                }
                box.Add(card);
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        // ── Solo vs multiplayer ──────────────────────────────────────────────
        void ShowModeSelect(AssignmentView v)
        {
            playMode = true;   // the play flow owns navigation until we return to the profile
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => BackToProfile(v)));
            content.Add(HomeUI.Heading("Play"));
            content.Add(HomeUI.Caption(v.Assignment.Title));

            // NOTE: the assignment instructions / reading content are deliberately
            // NOT shown here — a student could read or copy them before playing.
            // The content appears only inside the game board once it launches.

            // No game content authored for this assignment yet — say so clearly
            // instead of doing nothing.
            var parsed = vm.ContentFor(v.Assignment);
            if (parsed == null || !parsed.HasActivities)
            {
                var card = HomeUI.Card();
                card.Add(HomeUI.FieldLabel("Nothing to play yet"));
                card.Add(HomeUI.Sub("This assignment has no interactive content. Your tutor adds words to define, fill in or illustrate — once they do, the game appears here."));
                content.Add(card);
                return;
            }

            var solo = HomeUI.Primary("Play solo", () => LaunchGame(v, multiplayer: false));
            solo.style.marginBottom = 8;
            content.Add(solo);
            content.Add(HomeUI.Outline("Play with others", () => ShowLobby(v)));
        }

        // Launch the gamified attempt. In multiplayer it's a SHARED, turn-based
        // board: presence, live scores and every move sync through the game hooks.
        void LaunchGame(AssignmentView v, bool multiplayer)
        {
            var parsed = vm.ContentFor(v.Assignment);
            if (parsed == null || !parsed.HasActivities) { BackToProfile(v); return; }

            var game = new AssignmentGameView();
            if (multiplayer)
                game.SetMultiplayerGame(new AssignmentGameView.MultiplayerGame
                {
                    SyncRoom     = () => vm.RefreshRoomAsync(),
                    PullEvents   = () => vm.PullEventsAsync(),
                    Heartbeat    = () => vm.HeartbeatAsync(),
                    IsMyTurn      = () => vm.IsMyTurn,
                    TurnName      = () => vm.CurrentTurnName,
                    CurrentTurnId = () => vm.CurrentTurnId,
                    MyId          = () => vm.CurrentUserId,
                    MyName        = () => vm.StudentName,
                    Scoreboard   = () => vm.Participants,
                    ReportEvent     = (type, i, payload) => vm.ReportEventAsync(type, i, payload),
                    ReportSolve     = (i, pts, total, fin) => vm.ReportSolveAsync(i, pts, total, fin),
                    PassTurn        = () => vm.PassTurnAsync(),
                    DrainLiveEvents = () => vm.DrainLiveEvents()
                });

            // Resume any saved progress for this assignment on this device (solo
            // restores the solved steps directly; multiplayer rebuilds the shared
            // board from the live event stream when re-joining the session).
            var saved = vm.LoadProgress(v.Assignment.Id);
            if (!multiplayer && saved != null && saved.HasProgress)
                game.SetResume(saved.SolvedIndices, saved.Score);

            // Record every solved step so the student can leave and continue later.
            game.SetProgressSink((solved, score) =>
                vm.SaveProgress(v.Assignment.Id, multiplayer, solved, score, parsed.TotalPoints, finished: false));

            game.Show(content, parsed,
                onBack: async () =>
                {
                    // Exit means exit. In multiplayer, leave the session entirely so
                    // we're not pulled back into the board and the turn doesn't stall
                    // on us — the host's room (Photon) advances the turn once we're
                    // gone. Then return to the assignment profile.
                    if (multiplayer) await vm.LeaveSessionAsync();
                    BackToProfile(v);
                },
                onFinished: async (score, total) =>
                {
                    vm.ClearProgress(v.Assignment.Id);   // game submitted — forget the resume point
                    if (multiplayer) await vm.ReportFinishAsync(score);
                    await vm.RecordGameResultAsync(v.Assignment, score, total);
                    GoHome();
                });
        }

        // ── Multiplayer lobby ────────────────────────────────────────────────
        async void ShowLobby(AssignmentView v)
        {
            if (vm.CurrentSession != null) { ShowSession(v); return; }

            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => ShowModeSelect(v)));
            content.Add(HomeUI.Heading("Play with others"));
            content.Add(HomeUI.Caption(v.Assignment.Title));

            // Pick the networking backend (only when Photon PUN is available). The
            // active one is the filled button; the choice is remembered per device.
            if (vm.PhotonAvailable)
            {
                content.Add(HomeUI.FieldLabel("Network"));
                var netRow = new VisualElement();
                netRow.style.flexDirection = FlexDirection.Row;
                netRow.style.marginBottom = 4;
                void RenderNet()
                {
                    netRow.Clear();
                    var photon = vm.UsingPhoton
                        ? HomeUI.Primary("Photon", () => { })
                        : HomeUI.Outline("Photon", () => { vm.SelectBackend(true); RenderNet(); });
                    var supa = vm.UsingPhoton
                        ? HomeUI.Outline("Supabase", () => { vm.SelectBackend(false); RenderNet(); })
                        : HomeUI.Primary("Supabase", () => { });
                    photon.style.flexGrow = 1; supa.style.flexGrow = 1; photon.style.marginRight = 6;
                    netRow.Add(photon); netRow.Add(supa);
                }
                RenderNet();
                content.Add(netRow);
                content.Add(HomeUI.Caption("Photon = instant (PUN, no database). Supabase = database-backed."));
            }

            content.Add(HomeUI.Primary("Host a new game", async () =>
            {
                var r = await vm.HostSessionAsync(v.Assignment);
                if (r.IsSuccess) ShowSession(v);
                else content.Add(HomeUI.Status(vm.ErrorMessage, true));
            }));

            content.Add(HomeUI.FieldLabel("Join with a code"));
            var code = HomeUI.Field(null);
            content.Add(code);
            var join = HomeUI.Outline("Join", async () =>
            {
                var r = await vm.JoinByCodeAsync(code.value);
                if (r.IsSuccess) ShowSession(v);
                else content.Add(HomeUI.Status(vm.ErrorMessage, true));
            });
            join.style.marginBottom = 8;
            content.Add(join);

            content.Add(HomeUI.Title("Open games", 15));
            var loading = HomeUI.Caption("Loading…");
            content.Add(loading);
            await vm.LoadOpenSessionsAsync(v.Assignment.Id);
            loading.RemoveFromHierarchy();

            if (vm.OpenSessions.Count == 0) { content.Add(HomeUI.Caption("No open games. Host one above.")); return; }
            foreach (var s in vm.OpenSessions)
            {
                var captured = s;
                var card = HomeUI.Card();
                card.Add(HomeUI.Title(string.IsNullOrWhiteSpace(s.HostName) ? "A student's game" : s.HostName + "'s game", 15));
                card.Add(HomeUI.Sub("Code: " + s.Code));
                var enter = HomeUI.Chip("Join", async () =>
                {
                    var r = await vm.JoinSessionAsync(captured);
                    if (r.IsSuccess) ShowSession(v);
                    else card.Add(HomeUI.Status(vm.ErrorMessage, true));
                });
                enter.style.marginTop = 8;
                card.Add(enter);
                content.Add(card);
            }
        }

        // The game room (the "lobby"): the shareable code, the live avatars of who's
        // here (spawning in as they join, greying out when they drop), and Start.
        // When the host starts, every screen jumps into the shared board together.
        void ShowSession(AssignmentView v)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Leave", async () => { await vm.LeaveSessionAsync(); ShowLobby(v); }));
            content.Add(HomeUI.Heading("Game room"));
            content.Add(HomeUI.Caption(v.Assignment.Title));

            if (vm.CurrentSession != null)
            {
                var codeCard = HomeUI.Card();
                codeCard.Add(HomeUI.FieldLabel("Share this code"));
                var code = HomeUI.Title(vm.CurrentSession.Code, 24);
                codeCard.Add(code);
                content.Add(codeCard);
            }

            content.Add(HomeUI.Title("In the room", 15));
            var players = new VisualElement();
            players.style.flexDirection = FlexDirection.Row;
            players.style.flexWrap = Wrap.Wrap;
            content.Add(players);

            var statusLine = HomeUI.Caption("");
            content.Add(statusLine);

            var actions = new VisualElement();
            content.Add(actions);

            var entered = false;   // navigate into the board exactly once
            async void Refresh()
            {
                await vm.HeartbeatAsync();      // I'm still here
                await vm.RefreshRoomAsync();    // roster + turn state (+ auto-skip a dropped turn-holder)

                // Host pressed Start → everyone follows into the shared board.
                if (!entered && vm.CurrentSession != null && vm.CurrentSession.IsPlaying)
                {
                    entered = true;
                    LaunchGame(v, multiplayer: true);
                    return;
                }

                RenderRoom(players, statusLine, actions, v, () => entered = true);
            }
            Refresh();
            players.schedule.Execute(Refresh).Every(2000);
        }

        void RenderRoom(VisualElement players, Label statusLine, VisualElement actions,
            AssignmentView v, Action markEntered)
        {
            var present = vm.ActivePlayers;
            players.Clear();
            if (present.Count == 0) { players.Add(HomeUI.Caption("Waiting for players…")); }
            foreach (var p in present) players.Add(PlayerAvatar(p));

            // Show anyone who has dropped (stale heartbeat) so it's visible they left.
            foreach (var p in vm.Participants)
                if (!present.Any(a => a.StudentId == p.StudentId))
                    players.Add(PlayerAvatar(p, away: true));

            statusLine.text = present.Count <= 1
                ? "Share the code so classmates can join."
                : present.Count + " players ready.";

            actions.Clear();
            if (vm.IsHost)
            {
                var start = HomeUI.Primary("Start game", async () =>
                {
                    markEntered();
                    var r = await vm.StartGameAsync();
                    if (r.IsSuccess) LaunchGame(v, multiplayer: true);
                    else { actions.Add(HomeUI.Status(vm.ErrorMessage, true)); }
                });
                start.style.marginTop = 12;
                actions.Add(start);
            }
            else
            {
                actions.Add(HomeUI.Caption("Waiting for the host to start…"));
            }
        }

        // A circular avatar with the player's photo (or initial), name, score and
        // a presence dot — the "player object" that spawns in for each student.
        VisualElement PlayerAvatar(GameParticipant p, bool away = false)
        {
            var cell = new VisualElement();
            cell.style.alignItems = Align.Center;
            cell.style.width = 84;
            cell.style.marginRight = 6;
            cell.style.marginBottom = 10;
            if (away) cell.style.opacity = 0.45f;

            var ring = new VisualElement();
            ring.style.width = ring.style.height = 52;
            ring.style.borderTopLeftRadius = ring.style.borderTopRightRadius = 26;
            ring.style.borderBottomLeftRadius = ring.style.borderBottomRightRadius = 26;
            ring.style.backgroundColor = new Color(46 / 255f, 51 / 255f, 64 / 255f);
            HomeUI.CoverBackground(ring);
            ring.style.justifyContent = Justify.Center;
            ring.style.alignItems = Align.Center;
            ring.style.overflow = Overflow.Hidden;

            if (!string.IsNullOrEmpty(p.AvatarUrl))
                HomeUI.LoadImageInto(ring, p.AvatarUrl);
            else
            {
                var initial = new Label(InitialOf(p.StudentName));
                initial.style.color = new Color(233 / 255f, 223 / 255f, 200 / 255f);
                initial.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;
                initial.style.fontSize = 20;
                ring.Add(initial);
            }
            cell.Add(ring);

            var name = new Label((string.IsNullOrWhiteSpace(p.StudentName) ? "A student" : p.StudentName)
                + (p.Finished ? " ✓" : ""));
            name.style.color = away ? HomeUI.Muted : new Color(242 / 255f, 239 / 255f, 230 / 255f);
            name.style.fontSize = 12;
            name.style.unityTextAlign = UnityEngine.TextAnchor.MiddleCenter;
            name.style.whiteSpace = WhiteSpace.Normal;
            cell.Add(name);

            var sub = new Label(away ? "away" : p.Score + " pts");
            sub.style.color = away ? HomeUI.Danger : HomeUI.Muted;
            sub.style.fontSize = 11;
            sub.style.unityTextAlign = UnityEngine.TextAnchor.MiddleCenter;
            cell.Add(sub);
            return cell;
        }

        static string InitialOf(string name)
            => string.IsNullOrWhiteSpace(name) ? "?" : name.Trim().Substring(0, 1).ToUpperInvariant();

        AssignmentView MyAssignmentFor(string assignmentId)
            => vm.MyAssignments.FirstOrDefault(x => x.Assignment.Id == assignmentId);

        static Label Badge(AssignmentView v)
        {
            var overdue = v.Assignment != null && v.Assignment.IsOverdue && !v.Submitted;
            var l = new Label(v.StatusLabel);
            l.style.fontSize = 11;
            l.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;
            l.style.color = overdue ? HomeUI.Danger : (v.Submitted ? HomeUI.Success : HomeUI.Muted);
            l.style.flexShrink = 0;
            return l;
        }

        static Button Pill(string text, Action onClick, bool danger = false)
        {
            var b = HomeUI.Chip(text, onClick, danger);
            b.style.marginRight = 8;
            b.style.marginBottom = 6;
            return b;
        }
    }
}
