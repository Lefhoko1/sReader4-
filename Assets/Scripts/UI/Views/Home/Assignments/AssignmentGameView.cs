using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using SReader.Domains.Assignments.Models;

namespace SReader.UI.Views.Home
{
    /// <summary>
    /// The attempt experience as a game. The assignment content renders as
    /// readable sentences with the interactive words underlined; tapping one
    /// opens its mini-game:
    ///   • Define     — arrange the shuffled definition words into order.
    ///   • Fill blank — arrange the shuffled letters to spell the word.
    ///   • Illustrate — pick the correct image among distractors.
    /// Each correct answer scores points and shows a success splash; when every
    /// activity is done the game finishes and reports the score (which the caller
    /// submits). Tap-to-arrange keeps it touch-friendly.
    /// </summary>
    internal sealed class AssignmentGameView
    {
        VisualElement root;
        AssignmentContent content;
        Action onBack;
        Action<int, int> onFinished;

        readonly HashSet<ContentToken> done = new HashSet<ContentToken>();
        int score;

        // ── Understanding & mastery (per-token, this session) ────────────────
        // How well each word was answered: first try unaided = Mastered; solved
        // with hints = Learned; revealed / too many tries = NeedsReview. The solo
        // review pass at the end resurfaces the NeedsReview words for a second go,
        // so the game rewards understanding, not just finishing.
        enum SolveQuality { Mastered = 0, Learned = 1, NeedsReview = 2 }
        readonly Dictionary<ContentToken, SolveQuality> quality = new Dictionary<ContentToken, SolveQuality>();
        readonly Dictionary<ContentToken, int> awarded = new Dictionary<ContentToken, int>();   // points banked per word
        readonly HashSet<ContentToken> reviewQueue = new HashSet<ContentToken>();
        bool reviewing;   // solo end-of-round review pass in progress

        // The board's activity tokens in deterministic order — the SHARED index
        // every player agrees on (both parse the same content), so "token #3 was
        // solved" means the same word on every screen.
        readonly List<ContentToken> activities = new List<ContentToken>();

        // Multiplayer (null = solo). A shared, turn-based board: only the player
        // whose turn it is can act; their taps (select) and solves are broadcast
        // through the event stream and applied on every screen.
        MultiplayerGame mp;
        int selectedIndex = -1;     // token the active player is currently playing
        int currentActivityIndex = -1;
        string boardSignature = "";
        bool loopStarted;
        bool active;                // false once we've left the game (stops the loop touching other screens)
        bool turnConsumed;          // I just acted; lock my board until the turn-pass propagates
        IVisualElementScheduledItem pollItem, liveItem;

        // Spectator mirror — the read-only copy of the active player's open mini-game.
        VisualElement mirrorOverlay, mirrorCard;
        bool mirrorCelebrating;
        MirrorPayload pendingState;
        bool stateScheduled;
        int mirrorTokenIndex = -1;   // which board word the mirror is showing (for spectator "play along")

        // Fixed (no-scroll) layout + fullscreen.
        VisualElement host;            // the game's own container (re-parented when maximized)
        VisualElement savedParent;
        bool maximized;

        // Per-turn countdown — the turn auto-passes after a few seconds to keep it lively.
        const int TurnSeconds = 20;
        int turnSeconds;
        string countdownTurnId = "__none__";
        Label countdownLabel, turnBadge;
        IVisualElementScheduledItem countdownItem;

        /// <summary>The hooks that make this a shared, turn-based game.</summary>
        internal sealed class MultiplayerGame
        {
            public Func<Task> SyncRoom;                              // refresh roster + turn state (+ auto-skip a dropped player)
            public Func<Task<IReadOnlyList<GameEvent>>> PullEvents;  // new shared events since our cursor
            public Func<Task> Heartbeat;                             // tell the room I'm still here
            public Func<bool> IsMyTurn;
            public Func<string> TurnName;
            public Func<string> CurrentTurnId;                       // whose turn (drives the countdown + highlight)
            public Func<string> MyId;                                // my user id (skip mirroring my own events)
            public Func<string> MyName;                              // my display name (shown on my reactions / tips)
            public Func<IReadOnlyList<GameParticipant>> Scoreboard;  // current roster + live scores
            public Func<string, int, string, Task> ReportEvent;      // type, token #i, json payload — open/state/cancel
            public Func<int, int, int, bool, Task> ReportSolve;      // i, points, my total score, finished?
            public Func<Task> PassTurn;                              // hand the turn to the next player
            public Func<IReadOnlyList<GameEvent>> DrainLiveEvents;   // events pushed over the live (WebSocket) connection
        }

        public void SetMultiplayerGame(MultiplayerGame game) => mp = game;

        // Resume: the steps already solved on this device + the score to restore.
        IReadOnlyList<int> resumeSolved;
        int resumeScore;

        // Sink called after every solved step so the caller can persist progress
        // (board indices solved so far + the running score) for resume-later.
        Action<IReadOnlyList<int>, int> onProgress;

        /// <summary>Restore a saved board: re-mark these solved indices and score.</summary>
        public void SetResume(IReadOnlyList<int> solvedIndices, int score)
        {
            resumeSolved = solvedIndices;
            resumeScore = score;
        }

        /// <summary>Record every solved step (board indices + score) for resume-later.</summary>
        public void SetProgressSink(Action<IReadOnlyList<int>, int> sink) => onProgress = sink;

        static readonly Color Bg        = Rgb(18, 20, 25);
        static readonly Color Panel     = Rgb(35, 39, 51);
        static readonly Color Parchment = Rgb(233, 223, 200);
        static readonly Color Text      = Rgb(242, 239, 230);
        static readonly Color Muted     = Rgb(152, 160, 174);
        static readonly Color Success   = Rgb(140, 196, 140);
        static readonly Color Danger    = Rgb(224, 122, 113);
        static readonly Color Hairline  = Rgb(46, 51, 64);
        static readonly Color Review    = Rgb(232, 196, 104);   // amber: a word to revisit

        public void Show(VisualElement contentArea, AssignmentContent content, Action onBack, Action<int, int> onFinished)
        {
            this.root = contentArea;
            this.content = content;
            this.onBack = onBack;
            this.onFinished = onFinished;
            score = 0;
            done.Clear();
            quality.Clear();
            awarded.Clear();
            reviewQueue.Clear();
            reviewing = false;
            active = true;
            turnSeconds = TurnSeconds;
            BuildActivityIndex();

            // Resume: re-mark the steps already solved on this device and restore
            // the score, so the student continues exactly where they left off.
            if (resumeSolved != null && resumeSolved.Count > 0)
            {
                foreach (var i in resumeSolved)
                    if (i >= 0 && i < activities.Count)
                    {
                        var t = activities[i];
                        done.Add(t);
                        quality[t] = SolveQuality.Learned;       // already solved earlier this device
                        awarded[t] = Math.Max(0, t.points);
                    }
                score = resumeScore;
            }

            // The game lives in its own container so it can fill the area without
            // scrolling and be popped out to fullscreen.
            root.Clear();
            host = new VisualElement();
            host.style.flexGrow = 1;
            host.style.flexShrink = 1;
            host.style.minHeight = 0;
            root.Add(host);

            RenderReader();

            // In multiplayer, keep the shared board in sync, and run the per-turn
            // countdown. Started once.
            if (mp != null && !loopStarted)
            {
                loopStarted = true;
                pollItem = root.schedule.Execute(() => _ = PollTick()).Every(1500);   // DB reconcile / fallback
                liveItem = root.schedule.Execute(LiveTick).Every(150);                 // instant: drain pushed events
                countdownItem = root.schedule.Execute(CountdownTick).Every(1000);
                _ = PollTick();
            }
        }

        // Stop the loops when the player leaves the board, so they can never
        // redraw over whatever screen they navigate to next.
        void StopLoop()
        {
            active = false;
            pollItem?.Pause();
            liveItem?.Pause();
            countdownItem?.Pause();
        }

        void BuildActivityIndex()
        {
            activities.Clear();
            if (content == null) return;
            foreach (var page in content.pages)
                foreach (var sentence in page.sentences)
                    foreach (var token in sentence.tokens)
                        if (token.IsActivity) activities.Add(token);
        }

        int IndexOf(ContentToken token) => activities.IndexOf(token);

        // Record every solved step (board indices solved so far + score) so the
        // caller can persist it for resume-later — fires on every solve, in both
        // solo and multiplayer.
        void ReportProgress()
        {
            if (onProgress == null) return;
            var solved = new List<int>();
            foreach (var t in done)
            {
                var i = IndexOf(t);
                if (i >= 0) solved.Add(i);
            }
            onProgress(solved, score);
        }

        int Total => activities.Count;

        bool CanAct => mp == null || (!turnConsumed && (mp.IsMyTurn?.Invoke() ?? true));

        // ── The reader (fixed, no-scroll: everything lives in the window) ─────
        void RenderReader()
        {
            if (host == null) return;
            host.Clear();
            countdownLabel = null;
            turnBadge = null;

            host.Add(BuildTopBar());
            host.Add(BuildProgress());
            host.Add(BuildLearnStatus());
            if (mp != null) host.Add(BuildPlayersStrip());
            if (mp != null) host.Add(BuildReactionsBar());

            // The reading "page" fills the rest and clips (no scrolling); fullscreen
            // gives long passages room to breathe.
            var surface = ReadingSurface();
            surface.style.flexGrow = 1;
            surface.style.flexShrink = 1;
            surface.style.minHeight = 0;
            surface.style.overflow = Overflow.Hidden;
            host.Add(surface);

            if (content == null || content.pages.Count == 0)
            {
                surface.Add(Caption("This assignment has no interactive content yet."));
            }
            else
            {
                foreach (var page in content.pages)
                    foreach (var paragraph in Paragraphs(page, wordBudget: 60))
                        surface.Add(RenderParagraph(paragraph));
            }

            host.Add(BuildFooter());
        }

        // Compact top bar: exit · fullscreen · score · countdown — small icon
        // buttons (with tooltips/toasts) rather than big banners.
        VisualElement BuildTopBar()
        {
            var row = Row();
            row.style.justifyContent = Justify.SpaceBetween;
            row.style.alignItems = Align.Center;
            row.style.marginBottom = 6;

            var left = Row();
            left.Add(IconButton("‹ Exit", "Leave the game", () => { StopLoop(); onBack?.Invoke(); }));
            left.Add(IconButton(maximized ? "⤡ Minimize" : "⤢ Fullscreen", maximized ? "Exit fullscreen" : "Go fullscreen", ToggleMaximize));
            row.Add(left);

            var right = Row();
            var s = new Label($"⭐ {score}");
            s.style.color = Parchment; s.style.unityFontStyleAndWeight = FontStyle.Bold; s.style.fontSize = 14;
            s.style.marginRight = 10;
            right.Add(s);

            if (mp != null)
            {
                countdownLabel = new Label("");
                countdownLabel.style.color = Muted; countdownLabel.style.fontSize = 13;
                countdownLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                right.Add(countdownLabel);
                UpdateCountdownLabel();
            }
            row.Add(right);
            return row;
        }

        VisualElement BuildProgress()
        {
            var track = new VisualElement();
            track.style.height = 5;
            track.style.marginBottom = 8;
            track.style.backgroundColor = Hairline;
            Radius(track, 3);
            var fill = new VisualElement();
            fill.style.height = 5;
            Radius(fill, 3);
            fill.style.backgroundColor = Parchment;
            fill.style.width = Length.Percent(Total > 0 ? (done.Count * 100f / Total) : 0f);
            track.Add(fill);
            return track;
        }

        // A tiny line that turns "completion" into "understanding": how many words
        // are mastered (⭐, first try unaided) vs. flagged to review (✎), and a
        // gentle prompt during the review pass.
        VisualElement BuildLearnStatus()
        {
            var wrap = new VisualElement();
            int mastered = quality.Values.Count(q => q == SolveQuality.Mastered);
            int toReview = reviewQueue.Count;

            if (reviewing)
            {
                wrap.Add(MiniNote("✎ Review — tap the highlighted words to try them once more.", Review));
                return wrap;
            }
            if (done.Count == 0) return wrap;

            var bits = new List<string> { "⭐ " + mastered + " mastered" };
            if (toReview > 0) bits.Add("✎ " + toReview + " to review");
            wrap.Add(MiniNote(string.Join("   ·   ", bits), Muted));
            return wrap;
        }

        static Label MiniNote(string text, Color color)
        {
            var l = new Label(text);
            l.style.color = color;
            l.style.fontSize = 12;
            l.style.unityFontStyleAndWeight = FontStyle.Bold;
            l.style.marginBottom = 6;
            l.style.whiteSpace = WhiteSpace.Normal;
            return l;
        }

        // The row of player avatars; the one whose turn it is is highlighted with a
        // ring and shows the countdown badge. Replaces the old scoreboard list.
        VisualElement BuildPlayersStrip()
        {
            var strip = new VisualElement();
            strip.style.flexDirection = FlexDirection.Row;
            strip.style.flexWrap = Wrap.Wrap;
            strip.style.justifyContent = Justify.Center;   // centered horizontal lineage
            strip.style.alignItems = Align.Center;
            strip.style.marginBottom = 8;

            var people = mp.Scoreboard?.Invoke();
            var turnId = mp.CurrentTurnId?.Invoke();
            var now = DateTime.UtcNow;
            var ordered = (people ?? new List<GameParticipant>()).OrderBy(x => x.TurnOrder).ToList();

            // Place the current player in the MIDDLE of the row (others split to the sides).
            var current = ordered.FirstOrDefault(p => p.StudentId == turnId);
            List<GameParticipant> arranged;
            if (current != null)
            {
                var others = ordered.Where(p => p.StudentId != turnId).ToList();
                var half = others.Count / 2;
                arranged = new List<GameParticipant>();
                arranged.AddRange(others.Take(half));
                arranged.Add(current);
                arranged.AddRange(others.Skip(half));
            }
            else arranged = ordered;

            foreach (var p in arranged)
            {
                var isTurn = p.StudentId == turnId;
                var present = p.IsActiveAt(now, AssignmentGameServicePresenceGrace);
                strip.Add(PlayerAvatar(p, isTurn, present));
            }
            return strip;
        }

        VisualElement PlayerAvatar(GameParticipant p, bool isTurn, bool present)
        {
            var cell = new VisualElement();
            cell.style.alignItems = Align.Center;
            cell.style.width = isTurn ? 76 : 54;
            cell.style.marginLeft = cell.style.marginRight = 4;
            if (!present) cell.style.opacity = 0.4f;

            var ring = new VisualElement();
            var size = isTurn ? 60 : 38;   // current player's avatar is clearly bigger
            ring.style.width = ring.style.height = size;
            ring.style.borderTopLeftRadius = ring.style.borderTopRightRadius = size / 2f;
            ring.style.borderBottomLeftRadius = ring.style.borderBottomRightRadius = size / 2f;
            ring.style.backgroundColor = Hairline;
            ring.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Cover);
            ring.style.overflow = Overflow.Hidden;
            ring.style.alignItems = Align.Center;
            ring.style.justifyContent = Justify.Center;
            Border(ring, isTurn ? Success : Hairline, isTurn ? 3 : 1);

            // The player's profile picture (so you can see who it is); initial as fallback.
            if (!string.IsNullOrEmpty(p.AvatarUrl)) LoadImageInto(ring, p.AvatarUrl);
            else
            {
                var init = new Label(InitialOf(p.StudentName));
                init.style.color = Parchment; init.style.unityFontStyleAndWeight = FontStyle.Bold;
                init.style.fontSize = isTurn ? 22 : 15;
                ring.Add(init);
            }
            cell.Add(ring);

            // The live countdown badge sits on the current player (seen on every device).
            if (isTurn && mp != null)
            {
                turnBadge = new Label("");
                turnBadge.style.color = Success; turnBadge.style.fontSize = 13;
                turnBadge.style.unityFontStyleAndWeight = FontStyle.Bold;
                turnBadge.style.unityTextAlign = TextAnchor.MiddleCenter;
                turnBadge.style.marginTop = 2;
                cell.Add(turnBadge);
                UpdateCountdownLabel();
            }

            // Just the points beneath the avatar.
            var pts = new Label(p.Score + " pts");
            pts.style.color = isTurn ? Parchment : Muted;
            pts.style.fontSize = isTurn ? 12 : 11;
            pts.style.unityFontStyleAndWeight = isTurn ? FontStyle.Bold : FontStyle.Normal;
            pts.style.unityTextAlign = TextAnchor.MiddleCenter;
            cell.Add(pts);
            return cell;
        }

        static string InitialOf(string name)
            => string.IsNullOrWhiteSpace(name) ? "?" : name.Trim().Substring(0, 1).ToUpperInvariant();

        // The bottom action row: finish, or a small Pass button on your turn.
        VisualElement BuildFooter()
        {
            var row = WrapRow();
            row.style.justifyContent = Justify.Center;
            row.style.marginTop = 8;

            if (Total > 0 && done.Count >= Total)
            {
                // Solo: before finishing, give the student a focused retrieval pass
                // over the words they struggled with (spaced practice within the
                // session). Multiplayer just finishes (the board is shared/turn-based).
                if (mp == null && !reviewing && reviewQueue.Count > 0)
                {
                    var n = reviewQueue.Count;
                    row.Add(PrimaryButton($"Review {n} tricky word{(n > 1 ? "s" : "")} ✎", () => { reviewing = true; RenderReader(); }));
                    var skip = GameGhost("Finish anyway", () => ShowFinishSplash());
                    skip.style.marginTop = 0;
                    skip.style.marginLeft = 8;
                    row.Add(skip);
                    return row;
                }
                row.Add(PrimaryButton("Finish ✓ — submit", () => ShowFinishSplash()));
                return row;
            }

            if (mp != null && CanAct && Total > 0)
            {
                var pass = IconButton("⏭ Pass", "Skip your turn", () =>
                {
                    turnConsumed = true;
                    selectedIndex = -1;
                    Toast("Turn passed");
                    if (mp?.PassTurn != null) _ = mp.PassTurn();
                    RenderReader();
                });
                row.Add(pass);
            }
            return row;
        }

        // The dark "page" the text sits on, for a book/game reading feel.
        VisualElement ReadingSurface()
        {
            var c = new VisualElement();
            c.style.backgroundColor = Rgb(24, 27, 34);
            Radius(c, 14);
            Border(c, Hairline, 1);
            c.style.paddingTop = c.style.paddingBottom = 12;
            c.style.paddingLeft = c.style.paddingRight = 14;
            return c;
        }

        // ── Fullscreen ───────────────────────────────────────────────────────
        void ToggleMaximize()
        {
            if (host == null) return;
            maximized = !maximized;
            var top = host; while (top.parent != null) top = top.parent;

            if (maximized)
            {
                savedParent = host.parent;
                host.RemoveFromHierarchy();
                host.style.position = Position.Absolute;
                host.style.left = host.style.right = host.style.top = host.style.bottom = 0;
                host.style.backgroundColor = Bg;
                host.style.paddingLeft = host.style.paddingRight = 12;
                host.style.paddingTop = host.style.paddingBottom = 12;
                top.Add(host);
            }
            else
            {
                host.RemoveFromHierarchy();
                host.style.position = Position.Relative;
                host.style.left = host.style.right = host.style.top = host.style.bottom = StyleKeyword.Auto;
                host.style.backgroundColor = StyleKeyword.Null;
                host.style.paddingLeft = host.style.paddingRight = 0;
                host.style.paddingTop = host.style.paddingBottom = 0;
                savedParent?.Add(host);
            }
            RenderReader();
        }

        // ── Turn countdown (auto-pass keeps the game fast and fun) ───────────
        void CountdownTick()
        {
            if (mp == null || !active) return;

            // Freeze while a mini-game / mirror is open — you've claimed your turn;
            // the clock is for choosing, not for solving.
            if (HasOpenOverlay()) { UpdateCountdownLabel(); return; }

            var turn = mp.CurrentTurnId?.Invoke();
            if (turn != countdownTurnId) { countdownTurnId = turn; turnSeconds = TurnSeconds; }
            else if (turnSeconds > 0) turnSeconds--;

            UpdateCountdownLabel();

            // My time's up. Pass to the next player if there is one; either way restart
            // the clock immediately so the timer keeps visibly ticking (never freezes at 0).
            if (turnSeconds <= 0 && (mp.IsMyTurn?.Invoke() ?? false) && Total > 0 && done.Count < Total)
            {
                var people = mp.Scoreboard?.Invoke();
                var others = people == null ? 0
                    : people.Count(x => x.StudentId != turn && x.IsActiveAt(DateTime.UtcNow, AssignmentGameServicePresenceGrace));
                if (others > 0)
                {
                    turnConsumed = true;
                    Toast("⏱ Time! Turn passed");
                    if (mp.PassTurn != null) _ = mp.PassTurn();
                }
                turnSeconds = TurnSeconds;
            }
        }

        void UpdateCountdownLabel()
        {
            if (mp == null) return;
            var turn = mp.CurrentTurnId?.Invoke();
            var mine = mp.IsMyTurn?.Invoke() ?? false;
            var text = string.IsNullOrEmpty(turn) ? "" : "⏱ " + Math.Max(0, turnSeconds) + "s";
            if (countdownLabel != null) { countdownLabel.text = text; countdownLabel.style.color = mine ? Success : Muted; }
            if (turnBadge != null) turnBadge.text = Math.Max(0, turnSeconds) + "s";
        }

        // ── Toast (transient info messages) ──────────────────────────────────
        void Toast(string message)
        {
            var top = host; while (top != null && top.parent != null) top = top.parent;
            if (top == null) return;

            var rowWrap = new VisualElement();
            rowWrap.style.position = Position.Absolute;
            rowWrap.style.left = rowWrap.style.right = 0;
            rowWrap.style.top = 10;
            rowWrap.style.flexDirection = FlexDirection.Row;
            rowWrap.style.justifyContent = Justify.Center;
            rowWrap.pickingMode = PickingMode.Ignore;

            var chip = new Label(message);
            chip.style.backgroundColor = Panel;
            chip.style.color = Text;
            chip.style.fontSize = 13;
            chip.style.unityFontStyleAndWeight = FontStyle.Bold;
            chip.style.paddingLeft = chip.style.paddingRight = 14;
            chip.style.paddingTop = chip.style.paddingBottom = 8;
            Radius(chip, 12);
            Border(chip, Parchment, 1);
            rowWrap.Add(chip);
            top.Add(rowWrap);
            rowWrap.schedule.Execute(() => rowWrap.RemoveFromHierarchy()).StartingIn(1800);
        }

        // A longer-lived, wrapping banner near the top — used for peer tips, which
        // are worth reading (the toast is too brief for a sentence).
        void Banner(string message, Color accent, int ms)
        {
            var top = host; while (top != null && top.parent != null) top = top.parent;
            if (top == null) return;

            var wrap = new VisualElement();
            wrap.style.position = Position.Absolute;
            wrap.style.left = wrap.style.right = 0;
            wrap.style.top = 10;
            wrap.style.alignItems = Align.Center;
            wrap.pickingMode = PickingMode.Ignore;

            var chip = new Label(message);
            chip.style.maxWidth = Length.Percent(88);
            chip.style.whiteSpace = WhiteSpace.Normal;
            chip.style.backgroundColor = Panel;
            chip.style.color = Text;
            chip.style.fontSize = 13;
            chip.style.unityFontStyleAndWeight = FontStyle.Bold;
            chip.style.unityTextAlign = TextAnchor.MiddleCenter;
            chip.style.paddingLeft = chip.style.paddingRight = 14;
            chip.style.paddingTop = chip.style.paddingBottom = 8;
            Radius(chip, 12);
            Border(chip, accent, 1);
            wrap.Add(chip);
            top.Add(wrap);
            wrap.schedule.Execute(() => wrap.RemoveFromHierarchy()).StartingIn(ms);
        }

        // A small icon/text button with a tooltip describing what it does.
        static Button IconButton(string text, string tip, Action onClick)
        {
            var b = new Button(onClick) { text = text };
            b.tooltip = tip;
            b.style.height = 32;
            b.style.minWidth = 32;
            b.style.backgroundColor = Panel;
            b.style.color = Text;
            b.style.fontSize = 14;
            b.style.unityFontStyleAndWeight = FontStyle.Bold;
            b.style.marginRight = 6;
            b.style.paddingLeft = b.style.paddingRight = 10;
            Radius(b, 10);
            Border(b, Hairline, 1);
            return b;
        }

        // Group a page's sentences into paragraphs by a rough word budget.
        static List<List<ContentSentence>> Paragraphs(ContentPage page, int wordBudget)
        {
            var paras = new List<List<ContentSentence>>();
            var cur = new List<ContentSentence>();
            int words = 0;
            foreach (var s in page.sentences)
            {
                cur.Add(s);
                words += s.tokens.Count(t => t.isWord);
                if (words >= wordBudget) { paras.Add(cur); cur = new List<ContentSentence>(); words = 0; }
            }
            if (cur.Count > 0) paras.Add(cur);
            return paras;
        }

        VisualElement RenderParagraph(List<ContentSentence> sentences)
        {
            var para = new VisualElement();
            para.style.flexDirection = FlexDirection.Row;
            para.style.flexWrap = Wrap.Wrap;
            para.style.alignItems = Align.FlexEnd;   // words sit on a common baseline, not floating-centred
            para.style.marginBottom = 14;            // paragraph spacing

            var run = "";
            void FlushRun()
            {
                if (run.Length == 0) return;
                // Flow text within the paragraph: line breaks → spaces, collapse runs.
                var clean = run.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');
                while (clean.Contains("  ")) clean = clean.Replace("  ", " ");

                // Emit ONE label per word so the text and the activity words flow and
                // wrap together inline — an activity word is never isolated on its own
                // line (a single wrapping label would behave like a full-width block).
                foreach (var word in clean.Split(' '))
                {
                    if (word.Length == 0) continue;
                    var l = new Label(word);
                    l.style.color = Text;
                    l.style.fontSize = 17;            // match the activity words on the same line
                    l.style.whiteSpace = WhiteSpace.NoWrap;
                    l.style.marginRight = 5;          // word spacing
                    l.style.marginBottom = 7;         // line leading for wrapped lines
                    para.Add(l);
                }
                run = "";
            }

            foreach (var sentence in sentences)
                foreach (var token in sentence.tokens)
                {
                    if (token.IsActivity) { FlushRun(); para.Add(ActivityWord(token)); }
                    else run += token.text;
                }
            FlushRun();
            return para;
        }

        VisualElement ActivityWord(ContentToken token)
        {
            var completed = done.Contains(token);
            var inReview = reviewing && reviewQueue.Contains(token);     // re-attemptable in the review pass
            var open = !completed || inReview;                           // treat a review word like an unsolved one
            var tappable = CanAct && open;
            var isSelected = mp != null && open && IndexOf(token) == selectedIndex;

            // A fill-in word is NEVER shown in full — it reads as a boxed blank slot
            // until spelled (and is re-blanked when revisited in review); define /
            // illustrate words read as highlighted terms.
            if (token.activity == ActivityType.FillBlank && open)
            {
                var slot = new Label(Blanked(token.text) + (isSelected ? " ▶" : ""));
                slot.style.color = inReview ? Review : Parchment;
                slot.style.fontSize = 16;
                slot.style.unityFontStyleAndWeight = FontStyle.Bold;
                slot.style.backgroundColor = isSelected ? Rgb(40, 44, 56) : Bg;
                Border(slot, inReview ? Review : (isSelected ? Success : Parchment), inReview ? 2 : 1);
                Radius(slot, 6);
                slot.style.paddingLeft = slot.style.paddingRight = 6;
                slot.style.paddingTop = slot.style.paddingBottom = 1;
                slot.style.marginLeft = slot.style.marginRight = 2;
                slot.style.marginBottom = 7;   // match body line leading
                if (tappable) slot.RegisterCallback<ClickEvent>(_ => OnTap(token));
                return slot;
            }

            string display;
            if (inReview)                                       display = "✎ " + token.text;
            else if (completed)                                 display = token.text + " " + BadgeFor(token);
            else if (token.activity == ActivityType.Illustrate) display = "🖼 " + token.text;
            else                                                display = token.text;
            if (isSelected) display += " ▶";

            // Colour by mastery: amber = revisit, green = solved (⭐ mastered / ✓ learned).
            Color tone;
            if (inReview)        tone = Review;
            else if (completed)  tone = quality.TryGetValue(token, out var q) && q == SolveQuality.NeedsReview ? Review : Success;
            else                 tone = isSelected ? Success : Parchment;

            var l = new Label(display);
            l.style.fontSize = 17;
            l.style.unityFontStyleAndWeight = FontStyle.Bold;
            l.style.color = tone;
            l.style.borderBottomWidth = (completed && !inReview) ? 0 : 2;
            l.style.borderBottomColor = inReview ? Review : (isSelected ? Success : Parchment);
            l.style.marginLeft = 2;
            l.style.marginRight = 2;
            l.style.marginBottom = 7;   // match body line leading
            if (tappable) l.RegisterCallback<ClickEvent>(_ => OnTap(token));
            return l;
        }

        // The badge a solved word wears: ⭐ first-try unaided, ✓ solved with help,
        // ✎ still flagged for review.
        string BadgeFor(ContentToken token)
        {
            if (!quality.TryGetValue(token, out var q)) return "✓";
            return q == SolveQuality.Mastered ? "⭐" : q == SolveQuality.Learned ? "✓" : "✎";
        }

        // The active player taps a token: open the mini-game AND broadcast it so
        // the same modal opens on every screen.
        void OnTap(ContentToken token)
        {
            if (!CanAct) return;
            var i = IndexOf(token);
            selectedIndex = i;
            currentActivityIndex = i;
            if (mp?.ReportEvent != null && i >= 0)
                _ = mp.ReportEvent(GameEventType.Open, i, ToJson(InitialPayload(token)));
            OpenActivity(token);
        }

        // ── Broadcasting the active player's mini-game (so spectators mirror it) ─

        // The first snapshot when a mini-game opens (title/kind + starting state).
        MirrorPayload InitialPayload(ContentToken token)
        {
            var p = new MirrorPayload { status = "playing" };
            switch (token.activity)
            {
                case ActivityType.Define:
                    p.kind = "define"; p.title = "Define: " + token.text;
                    p.caption = "Arranging the definition…"; break;
                case ActivityType.FillBlank:
                    p.kind = "fill"; p.title = "Fill in the word";
                    p.caption = "Spelling the hidden word…"; p.display = Blanked(token.text); break;
                case ActivityType.Illustrate:
                    p.kind = "illustrate"; p.title = "Illustrate: " + token.text;
                    p.caption = "Choosing the matching image…";
                    p.images = new List<string>(AssignmentContentBuilder.ShuffledCopy(token.imageOptions)); break;
            }
            return p;
        }

        // Live state is throttled (rapid taps coalesce into the latest snapshot).
        void BroadcastState(MirrorPayload p)
        {
            if (mp?.ReportEvent == null) return;
            pendingState = p;
            if (stateScheduled) return;
            stateScheduled = true;
            root.schedule.Execute(() =>
            {
                stateScheduled = false;
                var snap = pendingState; pendingState = null;
                if (snap != null && mp?.ReportEvent != null)
                    _ = mp.ReportEvent(GameEventType.State, currentActivityIndex, ToJson(snap));
            }).StartingIn(250);
        }

        void BroadcastCancel()
        {
            if (mp?.ReportEvent != null) _ = mp.ReportEvent(GameEventType.Cancel, currentActivityIndex, null);
            currentActivityIndex = -1;
            pendingState = null;   // don't let a queued snapshot re-open the mirror after cancel
        }

        // ── Spectator mirror: render the active player's modal read-only ─────
        void ApplyMirrorEvent(GameEvent e, bool mine)
        {
            switch (e.Type)
            {
                case GameEventType.Open:   if (!mine) { mirrorTokenIndex = e.TokenIndex; ShowMirror(FromJson(e.Payload)); } break;
                case GameEventType.State:  if (!mine) UpdateMirror(FromJson(e.Payload)); break;
                case GameEventType.Cancel: if (!mine) CloseMirror();                     break;
                case GameEventType.Solve:
                    if (e.TokenIndex >= 0 && e.TokenIndex < activities.Count)
                    {
                        done.Add(activities[e.TokenIndex]);
                        if (selectedIndex == e.TokenIndex) selectedIndex = -1;
                        ReportProgress();   // a teammate solved a step — record the shared board
                    }
                    if (!mine) MirrorSuccess();
                    break;
                case GameEventType.Turn:   if (!mine && !mirrorCelebrating) CloseMirror(); break;

                // ── Collaborative / social (transient overlays; no board change) ──
                // (each is shown locally by the sender already, so only mirror others')
                case GameEventType.Cheer:      if (!mine) ShowCheer(e.Payload);      break;
                case GameEventType.Tip:        if (!mine) ShowTip(e.Payload);        break;
                case GameEventType.AlsoSolved: if (!mine) ShowAlsoSolved(e.Payload); break;
            }
        }

        // ── Collaborative: reactions, peer tips, "I solved it too" ───────────

        // The emoji reaction bar — everyone (the active player AND spectators) can
        // cheer each other on every turn, so nobody is just sitting idle watching.
        static readonly string[] Cheers = { "👏", "💪", "🔥", "🎉", "🤔" };
        VisualElement BuildReactionsBar()
        {
            var row = WrapRow();
            row.style.justifyContent = Justify.Center;
            row.style.marginBottom = 6;
            foreach (var emoji in Cheers)
            {
                var captured = emoji;
                var b = new Button(() => SendCheer(captured)) { text = emoji };
                b.tooltip = "Cheer your team";
                b.style.fontSize = 18;
                b.style.height = 34; b.style.minWidth = 40;
                b.style.backgroundColor = Panel;
                b.style.marginLeft = b.style.marginRight = 3;
                b.style.paddingLeft = b.style.paddingRight = 6;
                Radius(b, 10); Border(b, Hairline, 1);
                row.Add(b);
            }
            return row;
        }

        void SendCheer(string emoji)
        {
            FloatEmoji(emoji);   // show it on my own screen immediately (events only reach others)
            if (mp?.ReportEvent != null)
                _ = mp.ReportEvent(GameEventType.Cheer, -1, emoji + "|" + MyNameSafe());
        }

        void ShowCheer(string payload)
        {
            var (emoji, name) = Split2(payload);
            if (string.IsNullOrEmpty(emoji)) return;
            FloatEmoji(emoji);
            if (!string.IsNullOrEmpty(name)) Toast(name + " " + emoji);
        }

        // A teammate's tip on how they cracked the word — peer explanation, the
        // heart of social learning. Shown as a short amber banner.
        void ShowTip(string payload)
        {
            var (name, text) = Split2(payload);
            if (string.IsNullOrWhiteSpace(text)) return;
            Banner("💡 " + (string.IsNullOrWhiteSpace(name) ? "A teammate" : name) + ": " + text, Review, 4200);
        }

        void ShowAlsoSolved(string payload)
        {
            var name = (payload ?? "").Replace("|", "").Trim();
            Toast("✓ " + (string.IsNullOrWhiteSpace(name) ? "A teammate" : name) + " also got it!");
        }

        // Float a few copies of an emoji up the screen (like the success burst, but
        // for a reaction). Drawn on the top-most layer so it's seen over everything.
        void FloatEmoji(string emoji)
        {
            var layer = host; while (layer != null && layer.parent != null) layer = layer.parent;
            if (layer == null) return;
            var rng = new System.Random();
            for (int i = 0; i < 4; i++)
            {
                var g = new Label(emoji);
                g.style.position = Position.Absolute;
                g.style.left = Length.Percent(rng.Next(20, 80));
                g.style.bottom = 40;
                g.style.fontSize = rng.Next(22, 34);
                g.pickingMode = PickingMode.Ignore;
                layer.Add(g);
                float driftX = rng.Next(-40, 40);
                float rise = rng.Next(160, 300);
                g.experimental.animation.Start(0f, 1f, 1100, (el, t) =>
                {
                    el.style.opacity = 1f - t;
                    el.style.translate = new Translate(driftX * t, -rise * t);
                }).OnCompleted(() => g.RemoveFromHierarchy());
            }
        }

        string MyNameSafe()
        {
            var n = mp?.MyName?.Invoke();
            return string.IsNullOrWhiteSpace(n) ? "A teammate" : n.Replace("|", " ").Trim();
        }

        static (string, string) Split2(string payload)
        {
            if (string.IsNullOrEmpty(payload)) return ("", "");
            var i = payload.IndexOf('|');
            return i < 0 ? (payload, "") : (payload.Substring(0, i), payload.Substring(i + 1));
        }

        void ShowMirror(MirrorPayload p)
        {
            if (p == null) return;
            if (mirrorOverlay == null) mirrorOverlay = Overlay(out mirrorCard);
            RenderMirror(p);
        }

        void UpdateMirror(MirrorPayload p)
        {
            if (p == null || mirrorCelebrating) return;   // a late snapshot must not overwrite the success splash
            if (mirrorOverlay == null) { ShowMirror(p); return; }
            RenderMirror(p);
        }

        void RenderMirror(MirrorPayload p)
        {
            if (mirrorCard == null) return;
            mirrorCard.Clear();

            var who = mp?.TurnName?.Invoke();
            var banner = new Label("👀 " + (string.IsNullOrWhiteSpace(who) ? "A player" : who) + " is playing");
            banner.style.color = Muted; banner.style.fontSize = 12;
            banner.style.unityTextAlign = TextAnchor.MiddleCenter; banner.style.marginBottom = 6;
            mirrorCard.Add(banner);

            mirrorCard.Add(Title(p.title ?? "Mini-game"));
            if (!string.IsNullOrEmpty(p.caption)) mirrorCard.Add(Caption(p.caption));

            if (!string.IsNullOrEmpty(p.display))
            {
                var disp = new Label(p.display);
                disp.style.fontSize = 26; disp.style.unityFontStyleAndWeight = FontStyle.Bold;
                disp.style.color = Parchment; disp.style.unityTextAlign = TextAnchor.MiddleCenter;
                disp.style.marginTop = 6; disp.style.marginBottom = 6; disp.style.whiteSpace = WhiteSpace.Normal;
                mirrorCard.Add(disp);
            }

            if (p.answer != null && p.answer.Count > 0)
            {
                var row = WrapRow(); row.style.justifyContent = Justify.Center; row.style.minHeight = 40;
                foreach (var a in p.answer) row.Add(MirrorChip(a, filled: true));
                mirrorCard.Add(row);
            }
            if (p.pool != null && p.pool.Count > 0)
            {
                var div = new Label("Pieces"); div.style.color = Muted; div.style.fontSize = 12; div.style.marginTop = 10;
                mirrorCard.Add(div);
                var row = WrapRow();
                foreach (var a in p.pool) row.Add(MirrorChip(a, filled: false));
                mirrorCard.Add(row);
            }
            if (p.images != null && p.images.Count > 0)
            {
                // Same compact 2-column grid as the player's modal (no scrolling).
                var grid = new VisualElement();
                grid.style.flexDirection = FlexDirection.Row;
                grid.style.flexWrap = Wrap.Wrap;
                grid.style.justifyContent = Justify.SpaceBetween;
                grid.style.marginTop = 6;
                mirrorCard.Add(grid);
                var tileHeight = p.images.Count > 4 ? 80 : 104;
                foreach (var url in p.images)
                {
                    var box = new VisualElement();
                    box.style.width = Length.Percent(48);
                    box.style.height = tileHeight;
                    box.style.marginBottom = 8;
                    box.style.backgroundColor = Bg;
                    Radius(box, 10);
                    Border(box, Hairline, 1);
                    box.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
                    LoadImageInto(box, url);
                    grid.Add(box);
                }
            }

            if (!string.IsNullOrEmpty(p.note))
            {
                var note = new Label(p.note);
                note.style.fontSize = 13; note.style.marginTop = 8; note.style.whiteSpace = WhiteSpace.Normal;
                note.style.color = p.status == "wrong" ? Danger : Muted;
                note.style.unityTextAlign = TextAnchor.MiddleCenter;
                mirrorCard.Add(note);
            }

            // Play along: a spectator can attempt the SAME word on their own copy —
            // so everyone learns each turn instead of only watching. It never touches
            // the shared board or score; solving just cheers the team.
            if (!mirrorCelebrating && mirrorTokenIndex >= 0 && mirrorTokenIndex < activities.Count)
            {
                var tryBtn = GameGhost("✏️ Try it yourself", () => OpenActivity(activities[mirrorTokenIndex], practice: true));
                tryBtn.style.marginTop = 10;
                mirrorCard.Add(tryBtn);
            }

            if (p.status == "wrong") { ShakeError(mirrorCard); Burst(mirrorOverlay, success: false); }
        }

        void MirrorSuccess()
        {
            if (mirrorOverlay == null) return;
            mirrorCelebrating = true;
            Burst(mirrorOverlay, success: true);
            var ok = new Label("✓ Solved!");
            ok.style.color = Success; ok.style.fontSize = 20; ok.style.unityFontStyleAndWeight = FontStyle.Bold;
            ok.style.unityTextAlign = TextAnchor.MiddleCenter; ok.style.marginTop = 10;
            mirrorCard?.Add(ok);
            root.schedule.Execute(() => { mirrorCelebrating = false; CloseMirror(); }).StartingIn(1200);
        }

        void CloseMirror()
        {
            mirrorOverlay?.RemoveFromHierarchy();
            mirrorOverlay = null; mirrorCard = null; mirrorCelebrating = false;
            mirrorTokenIndex = -1;
        }

        static Label MirrorChip(string text, bool filled)
        {
            var l = new Label(text);
            l.style.height = 36; l.style.fontSize = 15; l.style.unityFontStyleAndWeight = FontStyle.Bold;
            l.style.backgroundColor = filled ? Parchment : Bg;
            l.style.color = filled ? Bg : Text;
            l.style.paddingLeft = l.style.paddingRight = 12;
            l.style.paddingTop = l.style.paddingBottom = 7;
            l.style.marginRight = 6; l.style.marginBottom = 6;
            Radius(l, 10); Border(l, filled ? Parchment : Hairline, 1);
            return l;
        }

        // ── Particle bursts + shake (success / failure feedback, sync'd) ─────
        void Burst(VisualElement host, bool success)
        {
            if (host == null) return;
            var layer = host; while (layer.parent != null) layer = layer.parent;
            var glyphs = success ? new[] { "✨", "🎉", "⭐", "💫", "🌟" } : new[] { "💥", "⚡", "✖" };
            var rng = new System.Random();
            for (int i = 0; i < 12; i++)
            {
                var g = new Label(glyphs[rng.Next(glyphs.Length)]);
                g.style.position = Position.Absolute;
                g.style.left = Length.Percent(rng.Next(12, 88));
                g.style.top = Length.Percent(success ? 32 : 40);
                g.style.fontSize = rng.Next(16, 30);
                g.pickingMode = PickingMode.Ignore;
                layer.Add(g);

                float driftX = rng.Next(-50, 50);
                float fall = success ? rng.Next(120, 280) : rng.Next(40, 90);
                g.experimental.animation.Start(0f, 1f, 850, (e, t) =>
                {
                    e.style.opacity = 1f - t;
                    e.style.translate = new Translate(driftX * t, fall * t);
                }).OnCompleted(() => g.RemoveFromHierarchy());
            }
        }

        static void ShakeError(VisualElement card)
        {
            if (card == null) return;
            card.experimental.animation.Start(0f, 1f, 350, (e, t) =>
            {
                var dx = Mathf.Sin(t * Mathf.PI * 6f) * 8f * (1f - t);
                e.style.translate = new Translate(dx, 0);
            });
        }

        // ── Mirror payload (small JSON over the event stream) ────────────────
        [Serializable]
        class MirrorPayload
        {
            public string kind;       // define | fill | illustrate
            public string title;
            public string caption;
            public string display;    // building answer text / "C A _ _" preview
            public List<string> answer = new List<string>();
            public List<string> pool = new List<string>();
            public List<string> images = new List<string>();
            public string status;     // playing | wrong | hint
            public string note;
        }

        static string ToJson(MirrorPayload p) => JsonUtility.ToJson(p);
        static MirrorPayload FromJson(string s)
        {
            if (string.IsNullOrEmpty(s)) return null;
            try { return JsonUtility.FromJson<MirrorPayload>(s); } catch { return null; }
        }

        // "▢ ▢ ▢ ▢" for the letters in a word (used in the reader and the game).
        static string Blanked(string word)
        {
            var n = (word ?? "").Count(char.IsLetterOrDigit);
            return n == 0 ? "▢" : string.Join(" ", Enumerable.Repeat("▢", n));
        }

        // ── Activity overlay ─────────────────────────────────────────────────
        // practice = a spectator playing along on their own copy (the "play along"
        // feature). It never broadcasts board state and never touches the shared
        // score/turn — solving just cheers the team that you got it too.
        void OpenActivity(ContentToken token, bool practice = false)
        {
            var overlay = Overlay(out var card);
            switch (token.activity)
            {
                case ActivityType.Define:    BuildDefine(card, token, overlay, practice); break;
                case ActivityType.FillBlank: BuildFill(card, token, overlay, practice);   break;
                case ActivityType.Illustrate:BuildIllustrate(card, token, overlay, practice); break;
                default: overlay.RemoveFromHierarchy(); break;
            }
        }

        void BuildDefine(VisualElement card, ContentToken token, VisualElement overlay, bool practice = false)
        {
            card.Add(Title("Define: " + token.text));
            card.Add(Caption(practice ? "Play along — arrange the definition yourself." : "Arrange the definition in the right order."));

            var correct = (token.definition ?? "")
                .Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var pool = AssignmentContentBuilder.ShuffleWords(token.definition);
            ArrangeGame(card, token, overlay, pool, correct, joinWith: " ", practice: practice);
        }

        void BuildFill(VisualElement card, ContentToken token, VisualElement overlay, bool practice = false)
        {
            Action back = () => { if (!practice) BroadcastCancel(); overlay.RemoveFromHierarchy(); };
            void broadcast(MirrorPayload p) { if (!practice) BroadcastState(p); }
            void Solve(SolveQuality q) { if (practice) PracticeWin(token, overlay); else Win(token, overlay, q); }

            card.Add(Title("Fill in the word"));
            card.Add(Caption(practice ? "Play along — spell the word yourself." : "Tap the letters to spell the hidden word."));

            var correct = (token.text ?? "").Where(char.IsLetterOrDigit).Select(c => c.ToString()).ToList();
            var pool = AssignmentContentBuilder.ShuffleLetters(token.text);
            var answer = new List<string>();

            var preview = new Label();
            preview.style.fontSize = 26;
            preview.style.unityFontStyleAndWeight = FontStyle.Bold;
            preview.style.color = Parchment;
            preview.style.unityTextAlign = TextAnchor.MiddleCenter;
            preview.style.marginTop = 6;
            preview.style.marginBottom = 6;
            card.Add(preview);

            var clue = StatusLabel();
            clue.style.unityTextAlign = TextAnchor.MiddleCenter;
            card.Add(clue);

            var answerRow = WrapRow();
            answerRow.style.justifyContent = Justify.Center;
            answerRow.style.minHeight = 44;
            var divider = new Label("Letters"); divider.style.color = Muted; divider.style.fontSize = 12; divider.style.marginTop = 10;
            var poolRow = WrapRow();
            poolRow.style.marginTop = 6;
            var status = StatusLabel();
            card.Add(answerRow);
            card.Add(divider);
            card.Add(poolRow);
            card.Add(status);

            void UpdatePreview()
            {
                var slots = new List<string>();
                for (int i = 0; i < correct.Count; i++)
                    slots.Add(i < answer.Count ? answer[i].ToUpperInvariant() : "▢");
                preview.text = string.Join(" ", slots);
            }

            MirrorPayload Snap(string st = "playing", string note = null) => new MirrorPayload
            {
                kind = "fill", title = "Fill in the word", caption = "Spelling the hidden word…",
                display = preview.text, answer = new List<string>(answer), pool = new List<string>(pool),
                status = st, note = note
            };

            void Rebuild()
            {
                answerRow.Clear();
                poolRow.Clear();
                for (int i = 0; i < answer.Count; i++)
                {
                    int idx = i;
                    answerRow.Add(GameChip(answer[i], () => { pool.Add(answer[idx]); answer.RemoveAt(idx); status.style.display = DisplayStyle.None; Rebuild(); }, filled: true));
                }
                for (int i = 0; i < pool.Count; i++)
                {
                    int idx = i;
                    poolRow.Add(GameChip(pool[i], () => { answer.Add(pool[idx]); pool.RemoveAt(idx); status.style.display = DisplayStyle.None; Rebuild(); }, filled: false));
                }
                UpdatePreview();
                broadcast(Snap());   // mirror the spelling-in-progress to everyone
            }
            Rebuild();

            int hintLevel = 0, wrongTries = 0;
            bool revealed = false;
            Button reveal = null;

            var actions = WrapRow();
            actions.style.marginTop = 12;
            actions.Add(GamePrimary("Check", () =>
            {
                var ok = string.Equals(string.Join("", answer), string.Join("", correct), StringComparison.OrdinalIgnoreCase);
                if (ok) { Solve(hintLevel > 0 ? SolveQuality.Learned : SolveQuality.Mastered); return; }

                // Specific, encouraging feedback instead of a generic "wrong".
                wrongTries++;
                string fb;
                if (answer.Count == correct.Count)
                {
                    int k = 0;
                    for (int i = 0; i < correct.Count; i++)
                        if (string.Equals(answer[i], correct[i], StringComparison.OrdinalIgnoreCase)) k++;
                    fb = k > 0 ? $"So close — {k} of {correct.Count} letters are in the right place."
                               : "Right letters — check the order.";
                }
                else fb = "Not quite — keep spelling.";
                status.text = fb; status.style.color = Danger; status.style.display = DisplayStyle.Flex;
                ShakeError(card); Burst(overlay, success: false);
                broadcast(Snap("wrong", fb));
                if (wrongTries >= 2 && reveal != null) reveal.style.display = DisplayStyle.Flex;   // offer a way out
            }));
            // Tiered, fading hints — each one helps a little more, and costs points.
            actions.Add(GameGhost("Hint (-40%)", () =>
            {
                if (correct.Count == 0) return;
                hintLevel++;
                var word = string.Join("", correct).ToUpperInvariant();
                string msg;
                switch (hintLevel)
                {
                    case 1:  msg = "💡 It has " + correct.Count + " letters."; break;
                    case 2:  msg = "💡 Starts with “" + correct[0].ToUpperInvariant() + "”."; break;
                    case 3:  msg = "💡 Begins “" + word.Substring(0, Math.Min(2, word.Length)) + "…”."; break;
                    default: hintLevel = 3; msg = "💡 No more hints — give it a try!"; break;
                }
                clue.text = msg; clue.style.color = Muted; clue.style.display = DisplayStyle.Flex;
                broadcast(Snap("hint", msg));
            }));
            // A way out after two tries: see the answer, then it returns in review.
            reveal = GameGhost("Show me", () =>
            {
                if (!revealed)
                {
                    revealed = true;
                    clue.text = "💡 The word is “" + string.Join("", correct).ToUpperInvariant() + "”.";
                    clue.style.color = Review; clue.style.display = DisplayStyle.Flex;
                    status.text = "Read it, then continue — it comes back in the review round.";
                    status.style.color = Review; status.style.display = DisplayStyle.Flex;
                    broadcast(Snap("hint", clue.text));
                    reveal.text = "Got it — continue";
                }
                else Solve(SolveQuality.NeedsReview);
            });
            reveal.style.display = DisplayStyle.None;
            actions.Add(reveal);
            actions.Add(GameGhost("← Back", back));
            card.Add(actions);
        }

        // Shared tap-to-arrange game for Define (words) and Fill (letters).
        void ArrangeGame(VisualElement card, ContentToken token, VisualElement overlay,
            List<string> pool, List<string> correct, string joinWith, bool caseInsensitive = false, bool practice = false)
        {
            Action back = () => { if (!practice) BroadcastCancel(); overlay.RemoveFromHierarchy(); };
            void broadcast(MirrorPayload p) { if (!practice) BroadcastState(p); }
            void Solve(SolveQuality q) { if (practice) PracticeWin(token, overlay); else Win(token, overlay, q); }

            var answer = new List<string>();
            var answerRow = WrapRow();
            answerRow.style.minHeight = 44;
            answerRow.style.marginTop = 8;
            var poolRow = WrapRow();
            poolRow.style.marginTop = 8;
            var status = StatusLabel();
            card.Add(answerRow);
            var div = new Label("Pieces"); div.style.color = Muted; div.style.fontSize = 12; div.style.marginTop = 10;
            card.Add(div);
            card.Add(poolRow);
            card.Add(status);

            MirrorPayload Snap(string st = "playing", string note = null) => new MirrorPayload
            {
                kind = "define", title = "Define: " + token.text, caption = "Arranging the definition…",
                display = string.Join(" ", answer), answer = new List<string>(answer), pool = new List<string>(pool),
                status = st, note = note
            };

            void Rebuild()
            {
                answerRow.Clear();
                poolRow.Clear();
                for (int i = 0; i < answer.Count; i++)
                {
                    int idx = i;
                    answerRow.Add(GameChip(answer[i], () => { pool.Add(answer[idx]); answer.RemoveAt(idx); Rebuild(); }, filled: true));
                }
                for (int i = 0; i < pool.Count; i++)
                {
                    int idx = i;
                    poolRow.Add(GameChip(pool[i], () => { answer.Add(pool[idx]); pool.RemoveAt(idx); Rebuild(); }, filled: false));
                }
                broadcast(Snap());   // mirror the arrangement-in-progress to everyone
            }
            Rebuild();

            var clue = StatusLabel();
            clue.style.unityTextAlign = TextAnchor.MiddleCenter;
            card.Add(clue);

            int hintLevel = 0, wrongTries = 0;
            bool revealed = false;
            Button reveal = null;

            // True when the right words are present but in the wrong order — lets us
            // give a useful nudge rather than a flat "wrong".
            bool SameWords()
            {
                if (answer.Count != correct.Count) return false;
                var cmp = caseInsensitive ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
                return answer.OrderBy(x => x, cmp).SequenceEqual(correct.OrderBy(x => x, cmp), cmp);
            }

            var actions = WrapRow();
            actions.style.marginTop = 12;
            actions.Add(GamePrimary("Check", () =>
            {
                var a = string.Join("|", answer);
                var b = string.Join("|", correct);
                var ok = caseInsensitive ? string.Equals(a, b, StringComparison.OrdinalIgnoreCase) : a == b;
                if (ok) { Solve(hintLevel > 0 ? SolveQuality.Learned : SolveQuality.Mastered); return; }

                wrongTries++;
                var fb = SameWords() ? "Right words — check the order." : "Not quite — keep arranging.";
                status.text = fb; status.style.color = Danger; status.style.display = DisplayStyle.Flex;
                ShakeError(card); Burst(overlay, success: false);
                broadcast(Snap("wrong", fb));
                if (wrongTries >= 2 && reveal != null) reveal.style.display = DisplayStyle.Flex;
            }));
            actions.Add(GameGhost("Hint (-40%)", () =>
            {
                if (correct.Count == 0) return;
                hintLevel++;
                string msg;
                switch (hintLevel)
                {
                    case 1:  msg = "💡 It has " + correct.Count + " words."; break;
                    case 2:  msg = "💡 It starts with “" + correct[0] + "”."; break;
                    case 3:  msg = "💡 First words: “" + string.Join(" ", correct.Take(Math.Min(2, correct.Count))) + " …”."; break;
                    default: hintLevel = 3; msg = "💡 No more hints — arrange what you can!"; break;
                }
                clue.text = msg; clue.style.color = Muted; clue.style.display = DisplayStyle.Flex;
                broadcast(Snap("hint", msg));
            }));
            reveal = GameGhost("Show me", () =>
            {
                if (!revealed)
                {
                    revealed = true;
                    clue.text = "💡 " + string.Join(" ", correct);
                    clue.style.color = Review; clue.style.display = DisplayStyle.Flex;
                    status.text = "Read it, then continue — it comes back in the review round.";
                    status.style.color = Review; status.style.display = DisplayStyle.Flex;
                    broadcast(Snap("hint", clue.text));
                    reveal.text = "Got it — continue";
                }
                else Solve(SolveQuality.NeedsReview);
            });
            reveal.style.display = DisplayStyle.None;
            actions.Add(reveal);
            actions.Add(GameGhost("← Back", back));
            card.Add(actions);
        }

        void BuildIllustrate(VisualElement card, ContentToken token, VisualElement overlay, bool practice = false)
        {
            Action back = () => { if (!practice) BroadcastCancel(); overlay.RemoveFromHierarchy(); };
            void broadcast(MirrorPayload p) { if (!practice) BroadcastState(p); }

            card.Add(Title("Illustrate: " + token.text));
            card.Add(Caption(practice ? "Play along — pick the matching image yourself." : "Pick the image that matches the word."));

            var status = StatusLabel();
            var options = AssignmentContentBuilder.ShuffledCopy(token.imageOptions);
            var imageList = new List<string>(options);

            MirrorPayload Snap(string st, string note) => new MirrorPayload
            {
                kind = "illustrate", title = "Illustrate: " + token.text, caption = "Choosing the matching image…",
                images = imageList, status = st, note = note
            };

            int wrongTries = 0;
            bool hintUsed = false, revealed = false;
            Button reveal = null;
            var boxes = new Dictionary<string, VisualElement>();

            // A compact 2-column grid so all the options fit on screen without scrolling.
            var grid = new VisualElement();
            grid.style.flexDirection = FlexDirection.Row;
            grid.style.flexWrap = Wrap.Wrap;
            grid.style.justifyContent = Justify.SpaceBetween;
            grid.style.marginTop = 6;
            card.Add(grid);

            // Two rows max keeps it on-screen; shrink the tiles a touch if there are many.
            var tileHeight = options.Count > 4 ? 80 : 104;
            foreach (var url in options)
            {
                var captured = url;
                var box = new VisualElement();
                box.style.width = Length.Percent(48);
                box.style.height = tileHeight;
                box.style.marginBottom = 8;
                box.style.backgroundColor = Bg;
                Radius(box, 10);
                Border(box, Hairline, 1);
                box.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
                LoadImageInto(box, captured);
                boxes[captured] = box;
                box.RegisterCallback<ClickEvent>(_ =>
                {
                    if (captured == token.correctImage)
                    {
                        if (practice) { PracticeWin(token, overlay); return; }
                        Win(token, overlay, revealed ? SolveQuality.NeedsReview
                                          : hintUsed ? SolveQuality.Learned
                                          : SolveQuality.Mastered);
                        return;
                    }
                    wrongTries++;
                    box.style.opacity = 0.25f;   // fade a wrong pick so the field narrows
                    box.pickingMode = PickingMode.Ignore;
                    status.text = "Not that one — look for the clue in the word."; status.style.color = Danger; status.style.display = DisplayStyle.Flex;
                    ShakeError(card); Burst(overlay, success: false);
                    broadcast(Snap("wrong", "Not that one — try again."));
                    if (wrongTries >= 2 && reveal != null) reveal.style.display = DisplayStyle.Flex;
                });
                grid.Add(box);
            }
            card.Add(status);

            var actions = WrapRow();
            actions.style.marginTop = 8;
            actions.Add(GameGhost("Hint (-40%)", () =>
            {
                hintUsed = true;
                // Remove one wrong option to narrow the choice.
                foreach (var kv in boxes)
                    if (kv.Key != token.correctImage && kv.Value.style.opacity != 0.25f)
                    { kv.Value.style.opacity = 0.25f; kv.Value.pickingMode = PickingMode.Ignore; break; }
                status.text = "💡 One wrong option removed."; status.style.color = Muted; status.style.display = DisplayStyle.Flex;
                broadcast(Snap("hint", status.text));
            }));
            reveal = GameGhost("Show me", () =>
            {
                if (revealed) return;
                revealed = true;
                if (boxes.TryGetValue(token.correctImage, out var correctBox)) Border(correctBox, Review, 3);
                status.text = "This is the one — tap it to continue. It comes back in review.";
                status.style.color = Review; status.style.display = DisplayStyle.Flex;
                broadcast(Snap("hint", "Revealed"));
            });
            reveal.style.display = DisplayStyle.None;
            actions.Add(reveal);
            actions.Add(GameGhost("← Back", back));
            card.Add(actions);
        }

        // A word is solved. Points scale with HOW it was solved so the game rewards
        // understanding: full marks for a first-try unaided solve, 60% when hints
        // were used, none for a revealed answer (which is flagged for review).
        void Win(ContentToken token, VisualElement overlay, SolveQuality q = SolveQuality.Mastered)
        {
            var basePoints = Math.Max(0, token.points);
            var earn = q == SolveQuality.Mastered ? basePoints
                     : q == SolveQuality.Learned  ? Mathf.CeilToInt(basePoints * 0.6f)
                     : 0;

            // Bank only the DELTA over what this word already earned, so re-solving
            // it in the review pass tops up the score without double-counting.
            var prev = awarded.TryGetValue(token, out var ap) ? ap : 0;
            var delta = Math.Max(0, earn - prev);
            score += delta;
            awarded[token] = Math.Max(prev, earn);
            done.Add(token);

            // Keep the BEST quality seen (a review re-solve can upgrade NeedsReview → Learned).
            if (!quality.TryGetValue(token, out var pq) || (int)q < (int)pq) quality[token] = q;
            if (q == SolveQuality.NeedsReview) reviewQueue.Add(token);
            else                               reviewQueue.Remove(token);

            overlay.RemoveFromHierarchy();
            selectedIndex = -1;

            if (mp != null)
            {
                // Broadcast the solve (marks the token done on every screen + my
                // score), then hand the turn to the next player.
                var i = IndexOf(token);
                var finished = done.Count >= Total;
                turnConsumed = true;   // lock my board until the pass propagates
                currentActivityIndex = -1;
                pendingState = null;   // cancel any queued snapshot for the solved token
                if (mp.ReportSolve != null && i >= 0) _ = mp.ReportSolve(i, delta, score, finished);
                if (mp.PassTurn != null) _ = mp.PassTurn();
            }
            ReportProgress();   // record this step so the attempt can be resumed

            // In the solo review pass, drop straight back to the board to work the
            // remaining tricky words (and auto-finish when none are left).
            if (reviewing) { RenderReader(); MaybeEndReview(); return; }
            if (mp != null) ShowSolveSplashWithTip(SplashFor(q, delta), token);
            else            ShowSplash(SplashFor(q, delta));
        }

        static string SplashFor(SolveQuality q, int points)
            => q == SolveQuality.Mastered   ? $"Mastered! +{points} ⭐"
             : q == SolveQuality.Learned    ? $"+{points} points"
             : "Saved for review ✎";

        // End the review pass once every flagged word has been revisited.
        void MaybeEndReview()
        {
            if (reviewQueue.Count > 0) return;
            reviewing = false;
            ShowFinishSplash();
        }

        // A spectator solved the word on their own copy ("play along"). It changes
        // NOTHING on the shared board — it just celebrates locally and tells the
        // team you got it too, so everyone stays engaged on every turn.
        void PracticeWin(ContentToken token, VisualElement overlay)
        {
            overlay.RemoveFromHierarchy();
            var i = IndexOf(token);
            if (mp?.ReportEvent != null && i >= 0)
                _ = mp.ReportEvent(GameEventType.AlsoSolved, i, MyNameSafe());
            PracticeSplash();
        }

        void PracticeSplash()
        {
            var overlay = Overlay(out var card);
            Burst(overlay, success: true);
            card.style.alignItems = Align.Center;
            var tick = new Label("👏");
            tick.style.fontSize = 52;
            card.Add(tick);
            var msg = Title("Nice — you got it too!");
            msg.style.unityTextAlign = TextAnchor.MiddleCenter;
            card.Add(msg);
            card.Add(Caption("Practice doesn't change the score — it keeps you in the game."));
            var cont = GamePrimary("Back to watching", () => overlay.RemoveFromHierarchy());
            cont.style.marginTop = 12;
            card.Add(cont);
        }

        const double AssignmentGameServicePresenceGrace = 15;

        // ── Instant path: drain events pushed over the live connection ───────
        // Cheap, in-memory, every 150ms → the mirror/turn/score update immediately
        // (the DB poll below is just reconcile/fallback when the socket is down).
        void LiveTick()
        {
            if (mp == null || !active || mp.DrainLiveEvents == null) return;
            var live = mp.DrainLiveEvents();
            if (live == null || live.Count == 0) return;

            var myId = mp.MyId?.Invoke();
            foreach (var e in live)
                ApplyMirrorEvent(e, mine: myId != null && e.ActorId == myId);

            var sig = Signature();
            if (sig != boardSignature && !HasOpenOverlay()) { boardSignature = sig; RenderReader(); }
        }

        // ── The shared-board poll loop (DB reconcile / fallback) ─────────────
        bool ticking;
        async Task PollTick()
        {
            if (mp == null || !active || ticking) return;
            ticking = true;
            try
            {
                if (mp.Heartbeat != null) await mp.Heartbeat();
                if (mp.SyncRoom != null)  await mp.SyncRoom();
                turnConsumed = false;   // the pass has had a chance to propagate

                // Apply everyone else's moves to my board: open/update/close the
                // mirrored modal, mark solved tokens done, fire success effects.
                if (mp.PullEvents != null)
                {
                    var events = await mp.PullEvents();
                    var myId = mp.MyId?.Invoke();
                    if (events != null)
                        foreach (var e in events)
                            ApplyMirrorEvent(e, mine: myId != null && e.ActorId == myId);
                }

                // Re-render only when something visible changed (board / turn / scores).
                var sig = Signature();
                if (sig != boardSignature)
                {
                    boardSignature = sig;
                    // Don't rip an open activity overlay out from under the player.
                    if (!HasOpenOverlay()) RenderReader();
                }
            }
            catch { /* a dropped poll is fine; the next tick recovers */ }
            finally { ticking = false; }
        }

        string Signature()
        {
            var turn = mp?.TurnName?.Invoke() ?? "";
            var mine = CanAct ? "1" : "0";
            var scores = "";
            var people = mp?.Scoreboard?.Invoke();
            if (people != null)
                foreach (var p in people.OrderBy(x => x.TurnOrder))
                    scores += p.StudentId + ":" + p.Score + ":" + (p.Finished ? "f" : "") + ":"
                              + (p.IsActiveAt(DateTime.UtcNow, AssignmentGameServicePresenceGrace) ? "a" : "x") + "|";
            return done.Count + "/" + selectedIndex + "/" + mine + "/" + turn + "/" + scores;
        }

        bool HasOpenOverlay()
        {
            var r = root;
            while (r.parent != null) r = r.parent;
            return r.Q<ScrollView>(className: GameOverlayClass) != null;
        }

        const string GameOverlayClass = "sr-game-overlay";

        // ── Splashes ─────────────────────────────────────────────────────────
        void ShowSplash(string message)
        {
            var overlay = Overlay(out var card);
            Burst(overlay, success: true);
            card.style.alignItems = Align.Center;
            var tick = new Label("✓");
            tick.style.fontSize = 56;
            tick.style.color = Success;
            tick.style.unityFontStyleAndWeight = FontStyle.Bold;
            card.Add(tick);
            var msg = Title(message);
            msg.style.unityTextAlign = TextAnchor.MiddleCenter;
            card.Add(msg);
            var cont = GamePrimary("Continue", () => { overlay.RemoveFromHierarchy(); RenderReader(); });
            cont.style.marginTop = 12;
            card.Add(cont);
        }

        // After solving in multiplayer, the solver can leave a one-tap "how I knew
        // it" tip for teammates — peer explanation, the heart of social learning.
        static readonly string[] TipChoices =
        {
            "I read it in the sentence", "I knew the spelling", "I sounded it out", "I guessed from the picture"
        };
        void ShowSolveSplashWithTip(string message, ContentToken token)
        {
            var overlay = Overlay(out var card);
            Burst(overlay, success: true);
            card.style.alignItems = Align.Center;

            var tick = new Label("✓");
            tick.style.fontSize = 56; tick.style.color = Success; tick.style.unityFontStyleAndWeight = FontStyle.Bold;
            card.Add(tick);
            var msg = Title(message); msg.style.unityTextAlign = TextAnchor.MiddleCenter; card.Add(msg);

            card.Add(Caption("Share a tip with your team? (optional)"));
            var tips = WrapRow(); tips.style.justifyContent = Justify.Center;
            var i = IndexOf(token);
            void SendTip(string text)
            {
                if (mp?.ReportEvent != null) _ = mp.ReportEvent(GameEventType.Tip, i, MyNameSafe() + "|" + text);
                overlay.RemoveFromHierarchy(); RenderReader();
            }
            foreach (var t in TipChoices)
            {
                var captured = t;
                tips.Add(GameChip(captured, () => SendTip(captured), filled: false));
            }
            card.Add(tips);

            var cont = GamePrimary("Continue", () => { overlay.RemoveFromHierarchy(); RenderReader(); });
            cont.style.marginTop = 12;
            card.Add(cont);
        }

        void ShowFinishSplash()
        {
            var overlay = Overlay(out var card);
            card.style.alignItems = Align.Center;
            var star = new Label("🎉");
            star.style.fontSize = 56;
            card.Add(star);
            var msg = Title($"Done! {score} / {content.TotalPoints} points");
            msg.style.unityTextAlign = TextAnchor.MiddleCenter;
            card.Add(msg);

            // A learning summary, not just a score — what you mastered vs. should revisit.
            int mastered = quality.Values.Count(q => q == SolveQuality.Mastered);
            int learned  = quality.Values.Count(q => q == SolveQuality.Learned);
            int needs    = quality.Values.Count(q => q == SolveQuality.NeedsReview);
            var summary = new Label($"⭐ {mastered} mastered    ✓ {learned} learned" + (needs > 0 ? $"    ✎ {needs} to revisit" : ""));
            summary.style.color = Muted;
            summary.style.fontSize = 13;
            summary.style.unityFontStyleAndWeight = FontStyle.Bold;
            summary.style.unityTextAlign = TextAnchor.MiddleCenter;
            summary.style.whiteSpace = WhiteSpace.Normal;
            summary.style.marginTop = 4;
            summary.style.marginBottom = 4;
            card.Add(summary);

            var submit = GamePrimary("Submit my work", () =>
            {
                StopLoop();
                overlay.RemoveFromHierarchy();
                onFinished?.Invoke(score, content.TotalPoints);
            });
            submit.style.marginTop = 12;
            card.Add(submit);
            card.Add(GameGhost("Keep playing", () => overlay.RemoveFromHierarchy()));
        }

        // ── Element builders (self-contained palette) ────────────────────────

        VisualElement Overlay(out VisualElement card)
        {
            var r = root;
            while (r.parent != null) r = r.parent;

            // A full-screen scroller so a tall activity (e.g. several images, a
            // long definition) scrolls instead of clipping; the card centres when
            // it's short enough.
            var overlay = new ScrollView(ScrollViewMode.Vertical);
            overlay.AddToClassList(GameOverlayClass);
            overlay.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            overlay.style.position = Position.Absolute;
            overlay.style.left = overlay.style.right = overlay.style.top = overlay.style.bottom = 0;
            // A light scrim — dims the board just enough to focus the card without
            // turning the whole screen black.
            overlay.style.backgroundColor = new Color(0, 0, 0, 0.35f);
            overlay.contentContainer.style.minHeight = Length.Percent(100);
            overlay.contentContainer.style.alignItems = Align.Center;
            overlay.contentContainer.style.justifyContent = Justify.Center;
            overlay.contentContainer.style.paddingLeft = overlay.contentContainer.style.paddingRight = 18;

            card = new VisualElement();
            card.style.backgroundColor = Panel;
            card.style.width = Length.Percent(100);
            card.style.maxWidth = 460;
            card.style.marginTop = card.style.marginBottom = 24;
            Radius(card, 16);
            Border(card, Hairline, 1);
            card.style.paddingTop = card.style.paddingBottom = 16;
            card.style.paddingLeft = card.style.paddingRight = 16;
            overlay.Add(card);
            r.Add(overlay);
            return overlay;
        }


        static Button GameChip(string text, Action onClick, bool filled)
        {
            var b = new Button(onClick) { text = text };
            b.style.height = 40;
            b.style.backgroundColor = filled ? Parchment : Bg;
            b.style.color = filled ? Bg : Text;
            b.style.fontSize = 16;
            b.style.unityFontStyleAndWeight = FontStyle.Bold;
            b.style.marginRight = 6;
            b.style.marginBottom = 6;
            b.style.paddingLeft = b.style.paddingRight = 14;
            Radius(b, 10);
            Border(b, filled ? Parchment : Hairline, 1);
            return b;
        }

        static Button GamePrimary(string text, Action onClick)
        {
            var b = new Button(onClick) { text = text };
            b.style.height = 44;
            b.style.backgroundColor = Parchment;
            b.style.color = Bg;
            b.style.unityFontStyleAndWeight = FontStyle.Bold;
            b.style.fontSize = 15;
            b.style.marginRight = 8;
            Radius(b, 12);
            Border(b, Parchment, 0);
            return b;
        }

        static Button GameGhost(string text, Action onClick)
        {
            var b = new Button(onClick) { text = text };
            b.style.height = 44;
            b.style.backgroundColor = Color.clear;
            b.style.color = Text;
            b.style.fontSize = 14;
            b.style.marginTop = 8;
            Radius(b, 12);
            Border(b, Hairline, 1);
            return b;
        }

        static Button LinkButton(string text, Action onClick)
        {
            var b = new Button(onClick) { text = text };
            b.style.backgroundColor = Color.clear;
            b.style.color = Parchment;
            b.style.unityFontStyleAndWeight = FontStyle.Bold;
            b.style.fontSize = 14;
            Border(b, Color.clear, 0);
            return b;
        }

        static Button PrimaryButton(string text, Action onClick) => GamePrimary(text, onClick);

        // Downloaded avatars/images are cached by URL so they paint INSTANTLY on
        // re-render (the players strip rebuilds every poll) instead of re-fetching
        // and flickering each time.
        static readonly Dictionary<string, Texture2D> ImageCache = new Dictionary<string, Texture2D>();

        static async void LoadImageInto(VisualElement target, string url)
        {
            if (string.IsNullOrEmpty(url) || target == null) return;

            // Already have it — set synchronously, no flash, no network call.
            if (ImageCache.TryGetValue(url, out var cached) && cached != null)
            {
                target.style.backgroundImage = new StyleBackground(cached);
                return;
            }

            var tcs = new System.Threading.Tasks.TaskCompletionSource<Texture2D>();
            var request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url);
            var op = request.SendWebRequest();
            op.completed += _ =>
            {
                tcs.SetResult(request.result == UnityEngine.Networking.UnityWebRequest.Result.Success
                    ? UnityEngine.Networking.DownloadHandlerTexture.GetContent(request) : null);
                request.Dispose();
            };
            var tex = await tcs.Task;
            if (tex == null) return;
            ImageCache[url] = tex;
            if (target != null) target.style.backgroundImage = new StyleBackground(tex);
        }

        static Label Title(string text)
        {
            var l = new Label(text);
            l.style.fontSize = 18;
            l.style.unityFontStyleAndWeight = FontStyle.Bold;
            l.style.color = Text;
            l.style.whiteSpace = WhiteSpace.Normal;
            l.style.marginBottom = 4;
            return l;
        }

        static Label Caption(string text)
        {
            var l = new Label(text);
            l.style.fontSize = 13;
            l.style.color = Muted;
            l.style.whiteSpace = WhiteSpace.Normal;
            l.style.marginBottom = 6;
            return l;
        }

        static Label StatusLabel()
        {
            var l = new Label("");
            l.style.fontSize = 13;
            l.style.marginTop = 8;
            l.style.whiteSpace = WhiteSpace.Normal;
            l.style.display = DisplayStyle.None;
            return l;
        }

        static VisualElement Row()
        {
            var r = new VisualElement();
            r.style.flexDirection = FlexDirection.Row;
            r.style.alignItems = Align.Center;
            return r;
        }

        static VisualElement WrapRow()
        {
            var r = new VisualElement();
            r.style.flexDirection = FlexDirection.Row;
            r.style.flexWrap = Wrap.Wrap;
            r.style.alignItems = Align.Center;
            return r;
        }

        static void Radius(VisualElement el, float r)
        {
            el.style.borderTopLeftRadius = el.style.borderTopRightRadius = r;
            el.style.borderBottomLeftRadius = el.style.borderBottomRightRadius = r;
        }

        static void Border(VisualElement el, Color color, float w)
        {
            el.style.borderLeftWidth = el.style.borderRightWidth = w;
            el.style.borderTopWidth = el.style.borderBottomWidth = w;
            el.style.borderLeftColor = el.style.borderRightColor = color;
            el.style.borderTopColor = el.style.borderBottomColor = color;
        }

        static Color Rgb(int r, int g, int b) => new Color(r / 255f, g / 255f, b / 255f);
    }
}
