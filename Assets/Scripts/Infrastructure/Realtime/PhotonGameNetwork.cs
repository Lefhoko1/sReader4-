#if PHOTON_UNITY_NETWORKING
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Domains.Assignments.Models;
using SReader.Domains.Assignments.Services;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using AppSettings = SReader.Core.Configuration.AppSettings;

namespace SReader.Infrastructure.Realtime
{
    /// <summary>
    /// Photon PUN 2 implementation of the WHOLE multiplayer layer — both
    /// <see cref="IAssignmentGameService"/> (host/join/turns/scores) AND
    /// <see cref="IGameRealtime"/> (the instant move stream) on ONE object that owns
    /// the Photon connection and room. NO database writes: a game session is a Photon
    /// Room, live state lives in Room/Player Custom Properties, and moves are
    /// delivered with <c>RaiseEvent</c>. This is why it's instant — nothing waits on
    /// an HTTP round-trip the way the Supabase path does.
    ///
    /// Mapping:
    ///   session            → Photon Room (room name == join code)
    ///   session state      → Room custom properties (status / turn / turnIndex…)
    ///   participant + score → Player custom properties (name / avatar / score / seat)
    ///   presence           → native (a player in the room is present; gone = removed)
    ///   the event stream    → RaiseEvent (open/state/solve/turn/finish…)
    ///
    /// Callbacks fire on Unity's main thread (PUN's PhotonHandler), so handlers touch
    /// the UI directly. Plain C# object — registered via AddCallbackTarget; needs no
    /// MonoBehaviour. The assignment CONTENT still comes from the DB (that's where
    /// tutors author it); only the live game session avoids the database.
    /// </summary>
    public sealed class PhotonGameNetwork : IAssignmentGameService, IGameRealtime,
        IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks, IInRoomCallbacks, IOnEventCallback
    {
        const byte GameEventCode = 1;

        // Room property keys (short to keep the wire small).
        const string RCode = "cd", RAssignment = "aid", RTitle = "atitle", RClass = "cid";
        const string RHost = "host", RHostName = "hn", RStatus = "st";
        const string RTurn = "tn", RTurnIndex = "ti", RStateVersion = "sv";
        // Player property keys.
        const string PUid = "uid", PName = "nm", PAvatar = "av", PScore = "sc", PFinished = "fn", PSeat = "se";

        readonly string appId;
        readonly CurrentSessionHolder session;

        Action<GameEvent> handler;     // live event sink (the ViewModel's OnRealtimeEvent)
        long localSeq;                  // local ordering for pushed events (no DB seq exists)

        readonly List<RoomInfo> roomList = new List<RoomInfo>();   // lobby cache for ListOpenSessions

        bool registered, connecting, wantLobby;

        // One in-flight matchmaking op (host/join) at a time.
        enum Op { None, Create, Join }
        Op pendingOp;
        string pendingRoom;
        Hashtable pendingRoomProps, pendingMyProps;
        bool opIssued;
        TaskCompletionSource<bool> opTcs;
        TaskCompletionSource<bool> lobbyTcs;

        public PhotonGameNetwork(AppSettings settings, CurrentSessionHolder session)
        {
            this.session = session;
            appId = settings != null ? settings.PhotonAppId : "";
        }

        string MyUid => session != null ? session.CurrentUserId : null;
        bool SignedIn => session != null && session.IsSignedIn;

        // ── IGameRealtime ────────────────────────────────────────────────────
        public bool Connected => PhotonNetwork.InRoom;

        // The room is already joined by Host/JoinByCode; here we just wire the sink.
        public Task<bool> JoinAsync(string sessionId, Action<GameEvent> onEvent)
        {
            EnsureRegistered();
            handler = onEvent;
            return Task.FromResult(PhotonNetwork.InRoom);
        }

        public void Leave(string sessionId)
        {
            handler = null;
            if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
        }

        public Task BroadcastAsync(string sessionId, GameEvent gameEvent)
        {
            RaiseGameEvent(gameEvent);
            return Task.CompletedTask;
        }

        // ── Host / join ──────────────────────────────────────────────────────
        public async Task<Result<AssignmentGameSession>> HostAsync(Assignment assignment, string hostName, string avatarUrl, CancellationToken ct = default)
        {
            if (!SignedIn) return Result.Fail<AssignmentGameSession>("Not signed in.");
            if (assignment == null || string.IsNullOrEmpty(assignment.Id))
                return Result.Fail<AssignmentGameSession>("An assignment is required.");

            var name = Clean(hostName);
            var code = NewCode();
            var roomProps = new Hashtable
            {
                { RCode, code }, { RAssignment, assignment.Id }, { RTitle, assignment.Title ?? "" },
                { RClass, assignment.ClassId ?? "" }, { RHost, MyUid }, { RHostName, name },
                { RStatus, "open" }, { RTurn, "" }, { RTurnIndex, 0 }, { RStateVersion, 0 }
            };

            var ok = await BeginOpAsync(Op.Create, code, roomProps, MyProps(name, avatarUrl, seat: 0));
            if (!ok || !PhotonNetwork.InRoom)
                return Result.Fail<AssignmentGameSession>("Couldn't create the game room (check your Photon App ID / connection).");
            return Result.Ok(SessionFromRoom(PhotonNetwork.CurrentRoom));
        }

        public async Task<Result<AssignmentGameSession>> JoinByCodeAsync(string code, string studentName, string avatarUrl, CancellationToken ct = default)
        {
            if (!SignedIn) return Result.Fail<AssignmentGameSession>("Not signed in.");
            if (string.IsNullOrWhiteSpace(code)) return Result.Fail<AssignmentGameSession>("Enter a join code.");

            var room = code.Trim().ToUpperInvariant();
            // seat -1 → assigned from the room once we're in (see OnJoinedRoom).
            var ok = await BeginOpAsync(Op.Join, room, null, MyProps(Clean(studentName), avatarUrl, seat: -1));
            if (!ok || !PhotonNetwork.InRoom)
                return Result.Fail<AssignmentGameSession>("No game with that code (or it's already closed).");
            return Result.Ok(SessionFromRoom(PhotonNetwork.CurrentRoom));
        }

        public async Task<Result> JoinSessionAsync(AssignmentGameSession gameSession, string studentName, string avatarUrl, CancellationToken ct = default)
        {
            if (gameSession == null) return Result.Fail("A session is required.");
            var code = !string.IsNullOrEmpty(gameSession.Code) ? gameSession.Code : gameSession.Id;
            var r = await JoinByCodeAsync(code, studentName, avatarUrl, ct);
            return r.IsSuccess ? Result.Ok() : Result.Fail(r.Error);
        }

        public Task<Result> LeaveSessionAsync(string sessionId, CancellationToken ct = default)
        {
            handler = null;
            if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
            return Task.FromResult(Result.Ok());
        }

        public async Task<Result<IReadOnlyList<AssignmentGameSession>>> ListOpenSessionsAsync(string assignmentId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(assignmentId))
                return Result.Fail<IReadOnlyList<AssignmentGameSession>>("Assignment id is required.");

            // Discovery needs the lobby; if we can't reach it, return an empty list
            // (the user can still host or join by code) rather than failing.
            if (!await EnsureInLobbyAsync())
                return Result.Ok<IReadOnlyList<AssignmentGameSession>>(Array.Empty<AssignmentGameSession>());

            var open = roomList
                .Where(r => r != null && !r.RemovedFromList && r.IsOpen && r.PlayerCount > 0)
                .Where(r => Str(r.CustomProperties, RAssignment) == assignmentId)
                .Where(r => (Str(r.CustomProperties, RStatus) ?? "open") == "open")
                .Select(SessionFromRoomInfo)
                .ToList();
            return Result.Ok<IReadOnlyList<AssignmentGameSession>>(open);
        }

        // ── Presence / roster ────────────────────────────────────────────────
        // Presence is native in Photon, so there's nothing to heartbeat.
        public Task<Result> HeartbeatAsync(string sessionId, CancellationToken ct = default)
            => Task.FromResult(Result.Ok());

        public Task<Result> UpdateMyAvatarAsync(string sessionId, string avatarUrl, CancellationToken ct = default)
        {
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { PAvatar, avatarUrl ?? "" } });
            return Task.FromResult(Result.Ok());
        }

        public Task<Result<IReadOnlyList<GameParticipant>>> ListParticipantsAsync(string sessionId, CancellationToken ct = default)
        {
            if (!PhotonNetwork.InRoom)
                return Task.FromResult(Result.Ok<IReadOnlyList<GameParticipant>>(Array.Empty<GameParticipant>()));
            var list = PhotonNetwork.PlayerList
                .Select(ParticipantFromPlayer)
                .OrderBy(p => p.TurnOrder)
                .ToList();
            return Task.FromResult(Result.Ok<IReadOnlyList<GameParticipant>>(list));
        }

        public Task<Result<AssignmentGameSession>> RefreshSessionAsync(string sessionId, CancellationToken ct = default)
        {
            if (!PhotonNetwork.InRoom) return Task.FromResult(Result.Fail<AssignmentGameSession>("Not in a game."));
            return Task.FromResult(Result.Ok(SessionFromRoom(PhotonNetwork.CurrentRoom)));
        }

        // ── Turn-based shared board ──────────────────────────────────────────
        public Task<Result> StartGameAsync(AssignmentGameSession gameSession, IReadOnlyList<GameParticipant> participants, CancellationToken ct = default)
        {
            if (!PhotonNetwork.InRoom) return Task.FromResult(Result.Fail("No session."));
            var first = ActiveOrdered(participants).FirstOrDefault();
            if (first == null) return Task.FromResult(Result.Fail("No active players to start."));

            SetRoom(new Hashtable
            {
                { RStatus, "playing" },
                { RTurn, first.StudentId ?? "" },
                { RTurnIndex, RoomInt(RTurnIndex) + 1 }
            });
            RaiseGameEvent(NewEvent(GameEventType.Turn, -1, 0, first.StudentId));
            return Task.FromResult(Result.Ok());
        }

        public Task<Result> PassTurnAsync(AssignmentGameSession gameSession, IReadOnlyList<GameParticipant> participants, CancellationToken ct = default)
        {
            if (!PhotonNetwork.InRoom) return Task.FromResult(Result.Fail("No session."));
            var current = Str(PhotonNetwork.CurrentRoom.CustomProperties, RTurn);
            var next = NextActiveAfter(participants, current);
            SetRoom(new Hashtable
            {
                { RTurn, next?.StudentId ?? "" },
                { RTurnIndex, RoomInt(RTurnIndex) + 1 }
            });
            RaiseGameEvent(NewEvent(GameEventType.Turn, -1, 0, next?.StudentId));
            return Task.FromResult(Result.Ok());
        }

        public Task<Result<GameEvent>> ReportEventAsync(string sessionId, string type, int tokenIndex, string payload, CancellationToken ct = default)
        {
            var e = NewEvent(type, tokenIndex, 0, payload);
            RaiseGameEvent(e);
            return Task.FromResult(Result.Ok(e));
        }

        public Task<Result<GameEvent>> ReportSolveAsync(string sessionId, int tokenIndex, int points, int totalScore, bool finished, CancellationToken ct = default)
        {
            // My score lives in MY player properties — the scoreboard reads it live.
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { PScore, totalScore }, { PFinished, finished } });
            var e = NewEvent(GameEventType.Solve, tokenIndex, points, null);
            e.ActorScore = totalScore;
            RaiseGameEvent(e);
            return Task.FromResult(Result.Ok(e));
        }

        public Task<Result<GameEvent>> ReportFinishAsync(string sessionId, int totalScore, CancellationToken ct = default)
        {
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { PScore, totalScore }, { PFinished, true } });
            var e = NewEvent(GameEventType.Finish, -1, totalScore, null);
            e.ActorScore = totalScore;
            RaiseGameEvent(e);
            return Task.FromResult(Result.Ok(e));
        }

        // Events are pushed live (RaiseEvent), so there is no log to pull from.
        public Task<Result<IReadOnlyList<GameEvent>>> PullEventsAsync(string sessionId, long afterSeq, CancellationToken ct = default)
            => Task.FromResult(Result.Ok<IReadOnlyList<GameEvent>>(Array.Empty<GameEvent>()));

        // ── Turn helpers ─────────────────────────────────────────────────────
        public bool ShouldDriveAutoAdvance(AssignmentGameSession gameSession, IReadOnlyList<GameParticipant> participants)
        {
            if (gameSession == null || string.IsNullOrEmpty(gameSession.CurrentTurnId)) return false;
            var ordered = ActiveOrdered(participants);
            if (ordered.Count == 0) return false;
            if (ordered.Any(p => p.StudentId == gameSession.CurrentTurnId)) return false;   // holder still present
            return ordered[0].StudentId == MyUid;                                            // I'm the leader → I advance
        }

        public bool IsMyTurn(AssignmentGameSession gameSession)
            => gameSession != null && !string.IsNullOrEmpty(MyUid) && gameSession.CurrentTurnId == MyUid;

        // ── Photon connection / matchmaking plumbing ─────────────────────────
        void EnsureRegistered()
        {
            if (registered) return;
            PhotonNetwork.AddCallbackTarget(this);
            registered = true;
        }

        void ApplyAppSettings()
        {
            EnsureRegistered();
            if (!string.IsNullOrEmpty(appId))
                PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = appId;
            PhotonNetwork.AutomaticallySyncScene = false;
            if (string.IsNullOrEmpty(PhotonNetwork.NickName))
                PhotonNetwork.NickName = MyUid ?? "player";
        }

        Task<bool> BeginOpAsync(Op op, string room, Hashtable roomProps, Hashtable myProps)
        {
            ApplyAppSettings();

            opTcs?.TrySetResult(false);   // supersede any previous in-flight op
            opTcs = new TaskCompletionSource<bool>();
            pendingOp = op; pendingRoom = room; pendingRoomProps = roomProps; pendingMyProps = myProps; opIssued = false;

            if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();   // OnLeftRoom → Pump
            else Pump();
            return opTcs.Task;
        }

        // Take the single next step needed to satisfy the pending op from wherever we are.
        void Pump()
        {
            if (pendingOp == Op.None || opTcs == null || opIssued || PhotonNetwork.InRoom) return;

            if (!PhotonNetwork.IsConnected)
            {
                if (!connecting)
                {
                    connecting = true;
                    if (!PhotonNetwork.ConnectUsingSettings()) { connecting = false; CompleteOp(false); }
                }
                return;   // → OnConnectedToMaster
            }

            var st = PhotonNetwork.NetworkClientState;
            if (st == ClientState.ConnectedToMasterServer || st == ClientState.JoinedLobby)
            {
                opIssued = true;
                if (pendingOp == Op.Create)
                {
                    var opts = new RoomOptions
                    {
                        CustomRoomProperties = pendingRoomProps,
                        CustomRoomPropertiesForLobby = new[] { RAssignment, RStatus, RHostName, RTitle },
                        MaxPlayers = 0,
                        CleanupCacheOnLeave = true
                    };
                    PhotonNetwork.CreateRoom(pendingRoom, opts, TypedLobby.Default);
                }
                else
                {
                    PhotonNetwork.JoinRoom(pendingRoom);
                }
            }
            // else transitional — wait for the next callback, then Pump again.
        }

        void CompleteOp(bool ok)
        {
            pendingOp = Op.None;
            opIssued = false;
            var tcs = opTcs;
            opTcs = null;
            tcs?.TrySetResult(ok);
        }

        async Task<bool> EnsureInLobbyAsync()
        {
            if (PhotonNetwork.InLobby) return true;
            if (PhotonNetwork.InRoom) return false;   // can't browse while in a room

            ApplyAppSettings();
            lobbyTcs?.TrySetResult(false);
            lobbyTcs = new TaskCompletionSource<bool>();

            if (!PhotonNetwork.IsConnected)
            {
                wantLobby = true;
                if (!PhotonNetwork.ConnectUsingSettings()) { wantLobby = false; return false; }
            }
            else
            {
                PhotonNetwork.JoinLobby();
            }
            return await lobbyTcs.Task;
        }

        // ── IConnectionCallbacks ─────────────────────────────────────────────
        public void OnConnectedToMaster()
        {
            connecting = false;
            if (pendingOp != Op.None) Pump();
            else if (wantLobby) { wantLobby = false; PhotonNetwork.JoinLobby(); }
        }

        public void OnDisconnected(DisconnectCause cause)
        {
            connecting = false; wantLobby = false;
            if (opTcs != null) { Debug.LogWarning("[Photon] disconnected: " + cause); CompleteOp(false); }
            lobbyTcs?.TrySetResult(false); lobbyTcs = null;
        }

        public void OnConnected() { }
        public void OnRegionListReceived(RegionHandler regionHandler) { }
        public void OnCustomAuthenticationResponse(Dictionary<string, object> data) { }
        public void OnCustomAuthenticationFailed(string debugMessage) => Debug.LogWarning("[Photon] auth failed: " + debugMessage);

        // ── IMatchmakingCallbacks ────────────────────────────────────────────
        public void OnJoinedRoom()
        {
            if (pendingMyProps != null)
            {
                // Late seat assignment for joiners (host already has seat 0).
                if (pendingMyProps.TryGetValue(PSeat, out var sv) && sv is int s && s < 0)
                {
                    int max = -1;
                    foreach (var pl in PhotonNetwork.PlayerListOthers) max = Math.Max(max, Int(pl.CustomProperties, PSeat));
                    pendingMyProps[PSeat] = max + 1;
                }
                PhotonNetwork.LocalPlayer.SetCustomProperties(pendingMyProps);
                pendingMyProps = null;
            }
            CompleteOp(true);
        }

        public void OnLeftRoom() { if (pendingOp != Op.None) Pump(); }
        public void OnJoinRoomFailed(short returnCode, string message)   { Debug.LogWarning("[Photon] join failed: " + message);   CompleteOp(false); }
        public void OnCreateRoomFailed(short returnCode, string message) { Debug.LogWarning("[Photon] create failed: " + message); CompleteOp(false); }
        public void OnCreatedRoom() { }
        public void OnJoinRandomFailed(short returnCode, string message) { }
        public void OnFriendListUpdate(List<FriendInfo> friendList) { }

        // ── ILobbyCallbacks ──────────────────────────────────────────────────
        public void OnJoinedLobby() { lobbyTcs?.TrySetResult(true); lobbyTcs = null; }
        public void OnLeftLobby() { }
        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics) { }
        public void OnRoomListUpdate(List<RoomInfo> roomListUpdate)
        {
            foreach (var info in roomListUpdate)
            {
                roomList.RemoveAll(r => r.Name == info.Name);
                if (!info.RemovedFromList) roomList.Add(info);
            }
        }

        // ── IInRoomCallbacks (presence: join / leave) ────────────────────────
        // Photon presence is native, so a player going offline or exiting the game
        // fires these directly — no heartbeat needed, and the roster/scores are
        // re-read live from Photon on the game view's poll. The master client is
        // authoritative for shared turn state, so it heals a turn left stranded by
        // a player who dropped, which is the move that keeps a game from freezing.

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            // If the player who left was holding the turn, the master hands it on
            // immediately (broadcast to every client) so the game never stalls
            // waiting on someone who's gone.
            if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient) return;

            var status = Str(PhotonNetwork.CurrentRoom.CustomProperties, RStatus);
            var turn   = Str(PhotonNetwork.CurrentRoom.CustomProperties, RTurn);
            var leftId = Str(otherPlayer.CustomProperties, PUid) ?? otherPlayer.UserId ?? otherPlayer.NickName;
            if (status != "playing" || string.IsNullOrEmpty(turn) || turn != leftId) return;

            var remaining = PhotonNetwork.PlayerList.Select(ParticipantFromPlayer).ToList();
            var next = NextActiveAfter(remaining, turn);
            SetRoom(new Hashtable { { RTurn, next?.StudentId ?? "" }, { RTurnIndex, RoomInt(RTurnIndex) + 1 } });
            RaiseGameEvent(NewEvent(GameEventType.Turn, -1, 0, next?.StudentId));
        }

        public void OnPlayerEnteredRoom(Player newPlayer) { }
        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) { }
        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }
        public void OnMasterClientSwitched(Player newMasterClient) { }

        // ── IOnEventCallback ─────────────────────────────────────────────────
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code != GameEventCode) return;
            var json = photonEvent.CustomData as string;
            if (string.IsNullOrEmpty(json)) return;
            GameEvent model;
            try { model = JsonUtility.FromJson<Wire>(json).ToModel(); }
            catch (Exception ex) { Debug.LogWarning("[Photon] bad event: " + ex.Message); return; }
            handler?.Invoke(model);
        }

        // ── Helpers ──────────────────────────────────────────────────────────
        void RaiseGameEvent(GameEvent e)
        {
            if (!PhotonNetwork.InRoom || e == null) return;
            if (e.Seq == 0) e.Seq = ++localSeq;
            var json = JsonUtility.ToJson(Wire.From(e));
            PhotonNetwork.RaiseEvent(GameEventCode, json,
                new RaiseEventOptions { Receivers = ReceiverGroup.Others },
                SendOptions.SendReliable);
        }

        GameEvent NewEvent(string type, int tokenIndex, int points, string payload) => new GameEvent
        {
            SessionId = PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : null,
            ActorId = MyUid,
            Type = type,
            TokenIndex = tokenIndex,
            Points = points,
            Payload = payload,
            CreatedAt = DateTime.UtcNow
        };

        Hashtable MyProps(string name, string avatarUrl, int seat) => new Hashtable
        {
            { PUid, MyUid ?? "" }, { PName, name ?? "" }, { PAvatar, avatarUrl ?? "" },
            { PScore, 0 }, { PFinished, false }, { PSeat, seat }
        };

        void SetRoom(Hashtable props)
        {
            if (PhotonNetwork.InRoom) PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

        int RoomInt(string key) => PhotonNetwork.InRoom ? Int(PhotonNetwork.CurrentRoom.CustomProperties, key) : 0;

        AssignmentGameSession SessionFromRoom(Room r)
        {
            var p = r.CustomProperties;
            var turn = Str(p, RTurn);
            return new AssignmentGameSession
            {
                Id = r.Name,
                Code = Str(p, RCode) ?? r.Name,
                AssignmentId = Str(p, RAssignment),
                AssignmentTitle = Str(p, RTitle),
                ClassId = Str(p, RClass),
                HostId = Str(p, RHost),
                HostName = Str(p, RHostName),
                Status = Str(p, RStatus) ?? "open",
                CurrentTurnId = string.IsNullOrEmpty(turn) ? null : turn,
                TurnIndex = Int(p, RTurnIndex),
                StateVersion = Int(p, RStateVersion),
                CreatedAt = DateTime.UtcNow
            };
        }

        AssignmentGameSession SessionFromRoomInfo(RoomInfo r)
        {
            var p = r.CustomProperties;
            return new AssignmentGameSession
            {
                Id = r.Name,
                Code = r.Name,
                AssignmentId = Str(p, RAssignment),
                AssignmentTitle = Str(p, RTitle),
                HostName = Str(p, RHostName),
                Status = Str(p, RStatus) ?? "open",
                CreatedAt = DateTime.UtcNow
            };
        }

        GameParticipant ParticipantFromPlayer(Player pl)
        {
            var p = pl.CustomProperties;
            return new GameParticipant
            {
                SessionId = PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : null,
                StudentId = Str(p, PUid) ?? pl.UserId ?? pl.NickName,
                StudentName = Str(p, PName) ?? pl.NickName,
                AvatarUrl = Str(p, PAvatar),
                Score = Int(p, PScore),
                Finished = Bool(p, PFinished),
                TurnOrder = Int(p, PSeat),
                LastSeen = DateTime.UtcNow,   // present in the room == active
                UpdatedAt = DateTime.UtcNow
            };
        }

        // In Photon everyone in the room is present, so "active" = ordered by seat.
        static List<GameParticipant> ActiveOrdered(IReadOnlyList<GameParticipant> participants)
            => (participants ?? Array.Empty<GameParticipant>()).OrderBy(p => p.TurnOrder).ToList();

        static GameParticipant NextActiveAfter(IReadOnlyList<GameParticipant> participants, string currentId)
        {
            var ordered = ActiveOrdered(participants);
            if (ordered.Count == 0) return null;
            int idx = -1;
            for (int i = 0; i < ordered.Count; i++)
                if (ordered[i].StudentId == currentId) { idx = i; break; }
            // Current holder already gone (left/dropped) → start from the first
            // remaining player rather than skipping one.
            if (idx < 0) return ordered[0];
            return ordered[(idx + 1) % ordered.Count];
        }

        static string Str(Hashtable h, string k) => h != null && h.TryGetValue(k, out var v) && v != null ? v.ToString() : null;
        static int Int(Hashtable h, string k) => h != null && h.TryGetValue(k, out var v) && v is int i ? i : 0;
        static bool Bool(Hashtable h, string k) => h != null && h.TryGetValue(k, out var v) && v is bool b && b;

        static string Clean(string name) => string.IsNullOrWhiteSpace(name) ? "A student" : name.Trim();

        static string NewCode()
        {
            const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var rng = new System.Random();
            var chars = new char[5];
            for (int i = 0; i < chars.Length; i++) chars[i] = alphabet[rng.Next(alphabet.Length)];
            return new string(chars);
        }

        // ── Wire DTO (same short keys as the Supabase client) ────────────────
        [Serializable]
        class Wire
        {
            public long seq;
            public string sid, t, pl, a, an;
            public int ti, p, sc;

            public static Wire From(GameEvent e) => new Wire
            {
                seq = e.Seq, sid = e.SessionId, t = e.Type, ti = e.TokenIndex,
                p = e.Points, pl = e.Payload, a = e.ActorId, an = e.ActorName, sc = e.ActorScore
            };

            public GameEvent ToModel() => new GameEvent
            {
                Seq = seq, SessionId = sid, Type = t, TokenIndex = ti, Points = p,
                Payload = string.IsNullOrEmpty(pl) ? null : pl, ActorId = a, ActorName = an,
                ActorScore = sc, CreatedAt = DateTime.UtcNow
            };
        }
    }
}
#endif
