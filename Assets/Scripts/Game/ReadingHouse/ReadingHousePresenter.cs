using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using SReader.Domains.Assignments.Models;
using SReader.Game.Trek;
using SReader.UI.ViewModels;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// Binds one assignment to the Reading House: the EXISTING content pipeline
    /// (AssignmentContent → TrailBuilder → TrailData) and the EXISTING rules engine
    /// (TrekSession) drive the dashboard, the word stones and the cameras. This
    /// class renders state and forwards input only (MVVM): it builds no second
    /// reading engine, scores nothing itself and never talks to Supabase/Photon.
    ///
    /// Content source, in order:
    ///  1. An assignment handed over with <see cref="SetPendingAssignment"/> (the
    ///     app's StudentAssignmentsViewModel supplies its content, as for Story Trek).
    ///  2. Placeholder mode (GEN-4): the bundled sample book.
    ///  3. The stone-layout test passage (5, 11 and 20-word sentences), on request.
    /// </summary>
    public sealed class ReadingHousePresenter : MonoBehaviour
    {
        [Header("Scene wiring")]
        public ReadingHouseDashboard dashboard;
        public WordStoneTray tray;
        public ReadingHouseCameraRig cameraRig;
        public ReadingHouseStudentMover studentMover;

        [Header("Placeholder content (GEN-4)")]
        public TextAsset sampleBook;
        public Texture sampleIllustration;

        [Header("Control labels")]
        public string pointsLabel = "Your Points";
        public string menuLabel = "Menu";
        public string playWithOthersLabel = "Play with Others";

        [Header("Character lines (placeholder — see note)")]
        [Tooltip("No runtime dialogue source exists in the assignment system yet. These are " +
                 "placeholders until one is chosen; they are not assignment content.")]
        public string encouragerLine = "Great job! You can do it!";
        public string zombieLine = "This is hard... you'll never finish.";

        [Header("Stone layout test passage (5, 11 and 20 words)")]
        [TextArea] public string testPassage =
            "The sun warms green leaves. " +
            "Lefa packed his bag and set off early in the morning. " +
            "Photosynthesis allows green plants to convert sunlight into chemical energy " +
            "that helps them grow strong roots, stems and green leaves.";

        [Header("Start-up")]
        public bool enterGameplayOnStart = true;
        public float overviewHoldSeconds = 1.5f;

        [Header("Events (hook the app's existing navigation / multiplayer here)")]
        public UnityEvent onMenu = new UnityEvent();
        public UnityEvent onPlayWithOthers = new UnityEvent();

        // Launch payload, set by ReadingHousePage before the scene opens (mirrors TrekScenePage).
        static AssignmentView pendingView;
        static StudentAssignmentsViewModel pendingVm;
        static System.Action pendingExit;

        TrailData trail;
        TrekSession session;
        int sentenceIndex;
        readonly List<(string text, int start, int end)> words = new List<(string, int, int)>();
        WordStoneView selected;

        // Live mode: the assignment came from the app's StudentAssignmentsViewModel.
        AssignmentView liveView;
        StudentAssignmentsViewModel liveVm;
        System.Action onExit;

        public TrailData Trail => trail;
        public TrekSession Session => session;
        public int SentenceIndex => sentenceIndex;
        public IReadOnlyList<(string text, int start, int end)> Words => words;
        /// <summary>True when showing a real assignment from the app (not the editor sample).</summary>
        public bool IsLive => liveVm != null;

        public static void SetPendingAssignment(AssignmentView view, StudentAssignmentsViewModel vm, System.Action exit = null)
        {
            pendingView = view;
            pendingVm = vm;
            pendingExit = exit;
        }

        public static void ClearPendingAssignment() => SetPendingAssignment(null, null, null);

        /// <summary>True while the app is opening the house for an assignment (set before the scene loads).</summary>
        public static bool IsLaunchPending => pendingView?.Assignment != null && pendingVm != null;

        /// <summary>Where Back goes when the house runs on its own (the walk-in director sets this).</summary>
        public System.Action standaloneExit;

        /// <summary>Where the dashboard's Menu goes when the house runs on its own (the walk-in director: the board).</summary>
        public System.Action standaloneMenu;

        [Header("Chalkboard (optional)")]
        public ChalkboardView chalkboard;

        [Header("Encourager (optional)")]
        public EncouragerBehaviour encourager;

        [Header("Boredom Zombie (optional)")]
        public ActorSpriteAnimator zombie;

        /// <summary>A word's challenge was solved: the encourager cheers, the bored zombie slumps.</summary>
        public void OnWordSolved()
        {
            if (encourager != null) encourager.Praise();
            if (zombie != null) zombie.Play("slump");          // the discouraging one loses this round
        }

        IEnumerator GreetStudent()
        {
            yield return new WaitForSeconds(0.6f);
            if (encourager != null) encourager.Greet();
        }

        // ── the attempt, at the dashboard (the same flow as the Classroom: activities, hints,
        //    Later, timer, reading, finishing and submitting through the existing ViewModel) ────────

        [Header("Attempt")]
        [Tooltip("Fallback host for the word activities and the reader (the HUD's overlay) when there is no dashboard screen.")]
        public VisualElement attemptHost;
        [Tooltip("The dashboard's display as a game screen: activities, reader and the reading toolbar open there.")]
        public DashboardScreen dashboardScreen;
        [Tooltip("The sentence as Word Path stones on the rug (replaces the dashboard tray's stones).")]
        public MatStoneBoard matStones;

        /// <summary>Where activities / the reader open right now (the dashboard display while reading).</summary>
        VisualElement Host => dashboardScreen != null && dashboardScreen.IsOpen ? dashboardScreen.Host : attemptHost;
        /// <summary>Short status messages for the student (the walk-in director shows them in its hint pill).</summary>
        public System.Action<string> showMessage;

        readonly GatePanelHost activity = new GatePanelHost();
        readonly ReadingViewController reader = new ReadingViewController();
        readonly List<int> pageStarts = new List<int>();
        int page;
        bool submitting, submitted;

        /// <summary>True while a word activity or the reader is open (the 3D scene ignores taps then).</summary>
        public bool AttemptUIOpen => activity.IsOpen || reader.IsOpen;
        public bool CanRead => trail != null && Host != null && !AttemptUIOpen;
        public bool CanFinish => liveVm != null && session != null && session.CanComplete && !submitted && !submitting;
        public int PageCount => pageStarts.Count;
        public int Page => page;

        void Update()
        {
            if (session == null) return;
            if (activity.IsOpen && session.TickTimer(Time.deltaTime))
            {
                activity.Close(); Save(); RefreshTray(); FocusDisplay(false);
                Say("No rush — that word will wait in Later.");
            }
        }

        /// <summary>Activities and the reader fill the dashboard's display: the camera comes in close while they are open.</summary>
        void FocusDisplay(bool on)
        {
            if (dashboardScreen == null || !Application.isPlaying) return;
            if (dashboard.sentenceText != null) dashboard.sentenceText.enabled = !on;   // the panel covers the display; no sentence showing through
            cameraRig.GoTo(on ? ReadingHouseView.TrayFocus : ReadingHouseView.Gameplay);
        }

        void Say(string text)
        {
            showMessage?.Invoke(text);
        }

        void Save(bool finished = false)
        {
            if (liveVm == null || session == null || trail == null || submitted) return;
            int total = 0;
            foreach (var g in trail.Gates) total += Mathf.Max(0, g.BasePoints);
            liveVm.SaveProgress(trail.AssignmentId, false, session.SolvedBoardIndices(), session.Points, total, finished);
            TrekCheckpointStore.Save(liveVm.CurrentUserId, session.ToCheckpoint());
        }

        /// <summary>The gate (key word) inside a word of the current sentence, if any.</summary>
        GateData GateInWord(int wordIndex)
        {
            if (trail == null || wordIndex < 0 || wordIndex >= words.Count) return null;
            var (_, start, end) = words[wordIndex];
            foreach (var gate in trail.Gates)
                if (gate.PassageCharIndex >= start && gate.PassageCharIndex < end) return gate;
            return null;
        }

        static bool Solved(GateData g) => g.State == GateState.SolvedGold || g.State == GateState.SolvedSilver || g.State == GateState.SolvedBronze;

        /// <summary>Open a key word's activity (only the word the session is at, as in the Classroom).</summary>
        void TryOpenGate(GateData gate, WordStone source = null)
        {
            if (session == null || Host == null || AttemptUIOpen || submitting) return;
            if (source != null)
            {
                source.token = gate.ChallengePayload as ContentToken;
                string action = gate.Type == GateType.Define ? "Define"
                    : gate.Type == GateType.Illustrate ? "Illustrate" : "Fill the word";
                KeywordCard.ShowAction(gate.Word, source.transform, action, () =>
                {
                    // Revalidate the real session when the student chooses the action.
                    if (this != null && source != null && source.gameObject.activeInHierarchy)
                        TryOpenGate(gate);
                }, matStones.viewCamera);
                return;
            }
            if (Solved(gate)) { Say($"“{gate.Word}” is already restored in gold!"); return; }
            var current = session.ActiveGate ?? session.ArriveAtGate();
            if (current != gate)
            {
                if (encourager != null) encourager.Nudge();
                RefreshTray();
                return;
            }
            KeywordCard.Dismiss();
            if (encourager != null) encourager.GoodPick();
            activity.Open(Host, gate, new GatePanelHost.Hooks
            {
                UseHint = () => { int level = session.UseHint(); Save(); return level; },
                RequestGuided = () => { session.RequestGuidedSolve(); Save(); },
                Solved = () =>
                {
                    session.SolveActiveGate(); activity.Close(); Save(); FocusDisplay(false);
                    ShowSentence(session.CurrentSentenceIndex, announce: false);
                    OnWordSolved();
                    if (session.CanComplete) Say("Every word restored — tap Finish to hand it in!");
                },
                Later = () => { session.DeferActiveGate(); activity.Close(); Save(); RefreshTray(); FocusDisplay(false); Say("Saved for Later — on to the next word."); },
            });
            DressActivity(gate);
            FocusDisplay(true);
        }

        /// <summary>The Classroom's touches on the activity sheet: the word's own sentence above it, big buttons, scrolling.</summary>
        void DressActivity(GateData gate)
        {
            var host = Host;
            var overlay = host[host.childCount - 1];
            if (overlay.childCount == 0) return;
            var sheet = overlay[0];
            if (dashboardScreen != null && dashboardScreen.IsOpen)
            {
                string context = "";
                if (gate.SentenceIndex >= 0 && gate.SentenceIndex < trail.Sentences.Count)
                {
                    var span = trail.Sentences[gate.SentenceIndex];
                    context = trail.PassageText.Substring(span.Start, span.Length);
                    if (gate.Type == GateType.Fill)
                    {
                        int offset = gate.PassageCharIndex - span.Start;
                        context = context.Substring(0, offset) + "____" + context.Substring(offset + gate.Word.Length);
                    }
                }
                dashboardScreen.PresentActivity(sheet, context);
                return;
            }
            sheet.style.maxHeight = Length.Percent(80);
            var scroll = new ScrollView(ScrollViewMode.Vertical); scroll.style.flexShrink = 1;
            var children = new List<VisualElement>(sheet.Children());
            foreach (var child in children) scroll.Add(child);
            sheet.Add(scroll);
            if (gate.SentenceIndex >= 0 && gate.SentenceIndex < trail.Sentences.Count)
            {
                var span = trail.Sentences[gate.SentenceIndex];
                string sentence = trail.PassageText.Substring(span.Start, span.Length);
                if (gate.Type == GateType.Fill)
                {
                    int offset = gate.PassageCharIndex - span.Start;
                    sentence = sentence.Substring(0, offset) + "____" + sentence.Substring(offset + gate.Word.Length);
                }
                var context = new Label(sentence);
                context.style.whiteSpace = WhiteSpace.Normal; context.style.fontSize = 16; context.style.marginBottom = 10;
                scroll.Insert(0, context);
            }
            host.Query<Button>().ForEach(b => { b.style.minHeight = 48; b.style.minWidth = 48; b.style.fontSize = 18; b.style.whiteSpace = WhiteSpace.Normal; });
        }

        /// <summary>Read the passage (the existing reader, full passage).</summary>
        public void OpenReader()
        {
            if (!CanRead) return;
            KeywordCard.Dismiss();
            session.PauseTimer();
            reader.Open(Host, trail, session.CurrentSentenceIndex, () => { session.ResumeTimer(); FocusDisplay(false); }, classroomMode: true);
            FocusDisplay(true);
        }

        /// <summary>Every word restored: complete the trail and submit through the existing ViewModel.</summary>
        public async void FinishAttempt()
        {
            if (!CanFinish) return;
            submitting = true; session.PauseTimer();
            try
            {
                var result = session.CompleteTrail(liveView.Assignment.MaxScore);
                Save(true);
                var response = await liveVm.RecordGameResultAsync(liveView.Assignment,
                    result.MaxScore > 0 ? result.Marks : result.Points, result.MaxScore > 0 ? result.MaxScore : result.TotalBasePoints,
                    "hints " + string.Join(",", result.HintReport));
                if (!this) return;
                submitted = response.IsSuccess;
                if (submitted)
                {
                    liveVm.ClearProgress(trail.AssignmentId);
                    TrekCheckpointStore.Clear(liveVm.CurrentUserId, trail.AssignmentId);
                    liveView.Status = AttemptStatus.Submitted;
                    if (encourager != null) encourager.Finished();
                    if (zombie != null) zombie.Play("slump");
                    Say("Reading complete — your result has been submitted!");
                }
                else Say("Result saved on this device. Tap Finish again when you're connected.");
            }
            catch (System.Exception e) { Debug.LogException(e, this); if (this) Say("Result saved. Tap Finish again when you're connected."); }
            finally { submitting = false; }
        }

        /// <summary>Back while an activity or the reader is open closes it first (true = handled).</summary>
        bool CloseAttemptUI()
        {
            if (KeywordCard.ActionOpen) { KeywordCard.Dismiss(); return true; }
            if (reader.IsOpen) { reader.Close(() => session?.ResumeTimer()); FocusDisplay(false); return true; }
            if (activity.IsOpen) { activity.Close(); Save(); RefreshTray(); FocusDisplay(false); return true; }
            return false;
        }

        /// <summary>Keep the dashboard's toolbar in step (called every frame by the walk-in director / while reading).</summary>
        public void UpdateToolbar(bool reading)
        {
            if (dashboardScreen == null || !dashboardScreen.IsOpen) return;
            bool show = reading && IsLive && !AttemptUIOpen;
            dashboardScreen.SetToolbar(show && CanRead, show && CanFinish, show && PageCount > 1, Page > 0, Page < PageCount - 1);
        }

        void OpenDashboardScreen()
        {
            if (dashboardScreen == null || dashboardScreen.IsOpen) return;
            dashboardScreen.Open();
            dashboardScreen.ReadPressed -= OpenReader; dashboardScreen.ReadPressed += OpenReader;
            dashboardScreen.FinishPressed -= FinishAttempt; dashboardScreen.FinishPressed += FinishAttempt;
            dashboardScreen.PreviousPressed -= PreviousPage; dashboardScreen.PreviousPressed += PreviousPage;
            dashboardScreen.NextPressed -= NextPage; dashboardScreen.NextPressed += NextPage;
        }

        /// <summary>The student walked away from the dashboard (the walk-in house): its screen closes.</summary>
        public void LeaveReading() => CloseDashboardScreen();

        void CloseDashboardScreen()
        {
            KeywordCard.Dismiss();
            if (dashboardScreen == null || !dashboardScreen.IsOpen) return;
            CloseAttemptUI();
            dashboardScreen.Close();
        }

        /// <summary>A Word Path stone on the rug was tapped (forwarded by ReadingHouseInput).</summary>
        public void OnMatStoneTapped(WordStone stone)
        {
            if (matStones == null || !matStones.TryGetWord(stone, out int wordIndex)) return;
            stone.Clicked();                                   // its own feedback: gold burst for a key word, a puff otherwise
            if (dashboardScreen != null && !dashboardScreen.IsOpen) { Say("Tap the dashboard to start reading — then tap the glowing stones."); return; }
            var gate = GateInWord(wordIndex);
            if (gate != null && liveVm != null) TryOpenGate(gate, stone);
        }

        public void NextPage() { if (page < pageStarts.Count - 1) { page++; RefreshTray(); } }
        public void PreviousPage() { if (page > 0) { page--; RefreshTray(); } }

        void OnApplicationPause(bool pause)
        {
            if (session == null) return;
            if (pause) { session.PauseTimer(); Save(); }
            else if (!reader.IsOpen) session.ResumeTimer();
        }

        void OnDestroy()
        {
            KeywordCard.Dismiss();
            if (session != null) Save();
            activity.Close(); reader.Close();
        }

        void Start()
        {
            bool launchedFromApp = pendingView?.Assignment != null && pendingVm != null;
            if (!launchedFromApp) Screen.orientation = ScreenOrientation.LandscapeLeft;   // the page does this when launched from the app
            dashboard.SetControlLabels(pointsLabel, menuLabel, playWithOthersLabel);
            dashboard.SetIllustration(sampleIllustration);

            LoadFromBestSource();

            // There is no runtime dialogue source yet: the placeholder lines only show in editor preview.
            dashboard.SetDialogue(encouragerLine, zombieLine);
            dashboard.SetDialogueVisible(!IsLive);
            studentMover.ToOverview(immediate: true);
            dashboard.SetDialogueLayout(false);
            cameraRig.GoTo(ReadingHouseView.Overview, immediate: true);
            // Opened from the app: go straight to reading. On its own, the walk-in director leads instead.
            if (enterGameplayOnStart && IsLive && Application.isPlaying) StartCoroutine(EnterGameplayAfterDelay());
        }

        IEnumerator EnterGameplayAfterDelay()
        {
            yield return new WaitForSeconds(overviewHoldSeconds);
            EnterView(ReadingHouseView.Gameplay);
        }

        // ── content ────────────────────────────────────────────────────────────

        /// <summary>
        /// Off in the walk-in house: content only ever comes from the database — the student picks a
        /// real assignment on the chalkboard. On only for an editor preview of this scene on its own.
        /// </summary>
        [Tooltip("Editor preview only: show the bundled sample book when no assignment is given.")]
        public bool allowSampleContent = true;

        public void LoadFromBestSource()
        {
            if (pendingView?.Assignment != null && pendingVm != null)
            {
                BindLive(pendingView, pendingVm, pendingExit);
                return;
            }
            if (!allowSampleContent) { ShowNoAssignment(); return; }
            // Editor preview only (scene opened directly, no app services): the bundled sample book.
            if (sampleBook != null)
            {
                Bind(TrailBuilder.Build("sample", "Meadow at Dawn", AssignmentContentCodec.FromJson(sampleBook.text)));
                return;
            }
            LoadTestPassage();
        }

        /// <summary>
        /// Live mode — the same binding TrekPresenter.Begin does for Story Trek:
        /// content from the view model, the existing per-device resume point
        /// (GameProgress) and the trek side-car (TrekCheckpointStore) restored into
        /// TrekSession. Read-only: the attempt is started when gameplay begins.
        /// </summary>
        void BindLive(AssignmentView view, StudentAssignmentsViewModel vm, System.Action exit)
        {
            activity.Close(); reader.Close();
            submitted = view.Submitted; submitting = false;
            liveView = view;
            liveVm = vm;
            onExit = exit;
            var a = view.Assignment;
            trail = TrailBuilder.Build(a.Id, a.Title, vm.ContentFor(a));
            if (trail.IsTextOnlyFallback && !string.IsNullOrEmpty(trail.BuildWarning))
                Debug.LogWarning($"[ReadingHouse] {trail.BuildWarning}", this);

            var progress = vm.LoadProgress(a.Id);
            var checkpoint = TrekCheckpointStore.Load(vm.CurrentUserId, a.Id);
            session = progress != null || checkpoint != null
                ? TrekSession.Restore(trail, new TrekConfig(), progress?.SolvedIndices, progress?.Score ?? checkpoint?.PointsSoFar ?? 0, checkpoint)
                : new TrekSession(trail, new TrekConfig());
            Debug.Log($"[ReadingHouse] Live assignment '{a.Title}' ({a.Id}): {trail.Sentences.Count} sentences, " +
                      $"{trail.Gates.Count} key words, {session.SolvedGates} solved, {session.Points} points" +
                      (progress != null ? " (resumed)" : ""), this);
            ShowSentence(session.CurrentSentenceIndex);
        }

        /// <summary>No assignment chosen yet: an empty board that sends the student to the chalkboard.</summary>
        public void ShowNoAssignment()
        {
            liveView = null; liveVm = null; onExit = null;
            trail = null; session = null; selected = null; words.Clear();
            dashboard.SetHeader("Choose an assignment", 0, 0);
            dashboard.SetSentence("Walk to the chalkboard and pick one of your assignments.");
            dashboard.SetPoints(0);
            tray.HideAll();
            if (matStones != null) matStones.HideAll();
            if (chalkboard != null) chalkboard.ShowPrompt();
        }

        /// <summary>The assignments ViewModel the house is reading from (opened from the app), if any.</summary>
        public StudentAssignmentsViewModel LiveViewModel => liveVm ?? pendingVm;
        public AssignmentView LiveAssignment => liveView;

        /// <summary>
        /// Inside the house: read an assignment chosen on the assignments board. Same live binding
        /// as opening it from the app, but Back returns to walking round the house.
        /// </summary>
        public void BindAssignment(AssignmentView view, StudentAssignmentsViewModel vm)
        {
            if (view?.Assignment == null || vm == null) return;
            BindLive(view, vm, null);
        }

        public void LoadTestPassage() => Bind(TrailBuilder.BuildFromPlainText("reading-house-layout-test", "Photosynthesis", testPassage));

        /// <summary>Editor preview / layout tests: a fresh session on local content.</summary>
        public void Bind(TrailData data)
        {
            liveView = null;
            liveVm = null;
            trail = data;
            if (trail.IsTextOnlyFallback && !string.IsNullOrEmpty(trail.BuildWarning))
                Debug.LogWarning($"[ReadingHouse] {trail.BuildWarning}", this);
            session = new TrekSession(trail);
            ShowSentence(0);
        }

        /// <summary>Leave the Reading House (Back / Menu). In editor preview there is nowhere to go, so return to the overview.</summary>
        public void RequestExit()
        {
            if (CloseAttemptUI()) return;
            Save();
            if (onExit != null) onExit();
            else if (standaloneExit != null) standaloneExit();
            else EnterView(ReadingHouseView.Overview);
        }

        public void NextSentence() => ShowSentence(sentenceIndex + 1);
        public void PreviousSentence() => ShowSentence(sentenceIndex - 1);

        public void ShowSentence(int index) => ShowSentence(index, announce: true);

        public void ShowSentence(int index, bool announce)
        {
            if (trail == null || trail.Sentences.Count == 0)
            {
                dashboard.SetHeader(trail?.Title, 0, 0);
                dashboard.SetSentence("");
                tray.HideAll();
                return;
            }
            sentenceIndex = Mathf.Clamp(index, 0, trail.Sentences.Count - 1);
            selected = null;

            var span = trail.Sentences[sentenceIndex];
            string sentence = trail.PassageText.Substring(span.Start, span.Length);
            SplitWords(sentence, span.Start);

            dashboard.SetHeader(trail.Title, sentenceIndex + 1, trail.Sentences.Count);
            if (chalkboard != null) chalkboard.Show(trail);
            dashboard.SetSentence(sentence.Trim());
            dashboard.SetPoints(session.Points);

            // Long sentences are paged: as many stones as fit on the two rows, the page with the
            // word the student is working on first.
            var labels = new List<string>(words.Count);
            foreach (var w in words) labels.Add(w.text);
            pageStarts.Clear();
            for (int start = 0; start < labels.Count;) { pageStarts.Add(start); start += matStones != null ? matStones.FitCount(labels, start) : tray.FitCount(labels, start); }
            page = 0;
            var focus = session.ActiveGate;
            if (focus == null) foreach (var g in trail.Gates) if (g.SentenceIndex == sentenceIndex && !Solved(g) && g.State != GateState.Deferred) { focus = g; break; }
            if (focus != null)
                for (int w = 0; w < words.Count; w++)
                    if (GateInWord(w) == focus) { page = PageOf(w); break; }
            RefreshTray();
            if (announce && encourager != null && Application.isPlaying && liveVm != null) encourager.Encourage();
        }

        int PageOf(int wordIndex)
        {
            int p = 0;
            for (int i = 0; i < pageStarts.Count; i++) if (pageStarts[i] <= wordIndex) p = i;
            return p;
        }

        void RefreshTray()
        {
            KeywordCard.Dismiss();
            if (trail == null || pageStarts.Count == 0) { tray.HideAll(); return; }
            int start = pageStarts[page];
            int end = page + 1 < pageStarts.Count ? pageStarts[page + 1] : words.Count;
            if (matStones != null)
            {
                tray.HideAll();
                var all = new List<string>(words.Count);
                foreach (var w in words) all.Add(w.text);
                matStones.Show(all, start, end,
                    i => GateInWord(i) != null,
                    i => GateInWord(i) is GateData g && Solved(g),
                    i => GateInWord(i) is GateData g2 && session.ActiveGate == g2);
            }
            else
            {
                var labels = new List<string>(end - start);
                for (int i = start; i < end; i++) labels.Add(words[i].text);
                tray.Show(labels, i => StateOfWord(start + i));
            }
            dashboard.SetPoints(session.Points);
            if (chalkboard != null) chalkboard.Show(trail);
        }

        void SplitWords(string sentence, int offset)
        {
            words.Clear();
            int i = 0;
            while (i < sentence.Length)
            {
                while (i < sentence.Length && char.IsWhiteSpace(sentence[i])) i++;
                int start = i;
                while (i < sentence.Length && !char.IsWhiteSpace(sentence[i])) i++;
                if (i > start) words.Add((sentence.Substring(start, i - start), offset + start, offset + i));
            }
        }

        /// <summary>Stone state comes straight from the gate state TrekSession keeps.</summary>
        StoneState StateOfWord(int wordIndex)
        {
            var (_, start, end) = words[wordIndex];
            foreach (var gate in trail.Gates)
            {
                if (gate.PassageCharIndex < start || gate.PassageCharIndex >= end) continue;
                switch (gate.State)
                {
                    case GateState.Deferred: return StoneState.Deferred;
                    case GateState.SolvedGold:
                    case GateState.SolvedSilver:
                    case GateState.SolvedBronze: return StoneState.Completed;
                    default: return session != null && session.ActiveGate == gate ? StoneState.Selected : StoneState.KeyWord;
                }
            }
            return StoneState.Normal;
        }

        // ── views ──────────────────────────────────────────────────────────────

        /// <summary>Gameplay views always move the student to his gameplay spot first; he never stands in front of the tray.</summary>
        public void EnterView(ReadingHouseView view)
        {
            if (view == ReadingHouseView.Overview) { studentMover.ToOverview(); CloseDashboardScreen(); }
            else
            {
                studentMover.ToGameplay();
                if (Application.isPlaying) OpenDashboardScreen();
                if (Application.isPlaying) StartCoroutine(GreetStudent());
            }
            dashboard.SetDialogueLayout(view != ReadingHouseView.Overview);
            cameraRig.GoTo(view);
        }

        // ── input (forwarded by ReadingHouseInput) ─────────────────────────────

        public void OnStoneTapped(WordStoneView stone)
        {
            int wordIndex = (pageStarts.Count > 0 ? pageStarts[page] : 0) + stone.WordIndex;
            var gate = GateInWord(wordIndex);
            if (gate != null && liveVm != null) { TryOpenGate(gate); return; }
            // An ordinary word (or no live assignment): just highlight it.
            if (selected != null && selected != stone) selected.SetState(StateOfWord((pageStarts.Count > 0 ? pageStarts[page] : 0) + selected.WordIndex));
            selected = stone;
            stone.SetState(StoneState.Selected);
        }

        public void OnControlTapped(DashboardControl control)
        {
            switch (control)
            {
                case DashboardControl.Menu:
                    // Milestone A: Menu leaves the Reading House through the app's own
                    // navigation. A fuller game menu is a later milestone.
                    onMenu.Invoke();
                    if (onExit == null && standaloneMenu != null) standaloneMenu();   // in the house: the app's menu, on the chalkboard
                    else RequestExit();
                    break;
                case DashboardControl.PlayWithOthers: onPlayWithOthers.Invoke(); break;
                case DashboardControl.Assignment: EnterView(ReadingHouseView.Overview); break;
            }
            Debug.Log($"[ReadingHouse] {control} tapped.", this);
        }
    }
}
