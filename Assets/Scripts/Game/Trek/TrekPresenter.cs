using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SReader.Domains.Assignments.Models;
using SReader.UI.ViewModels;

namespace SReader.Game.Trek
{
    /// <summary>
    /// Binds one trek run together: the TrailData (from the EXISTING assignment
    /// content), the TrekSession rules engine, the procedural 2D world, and the
    /// UI overlays (HUD/ribbon, Reading View, gate panels, Cleanup Camp, Trail
    /// Complete). Persistence and submission go through the EXISTING
    /// StudentAssignmentsViewModel — this class renders state and forwards input
    /// only (MVVM); every completion rule lives in TrekSession.
    ///
    /// With no ViewModel (placeholder mode, GEN-4) it runs a practice trek on the
    /// bundled sample passage, gates included, and submits nothing.
    /// </summary>
    internal sealed class TrekPresenter : MonoBehaviour
    {
        // ── run context (set by Begin) ──
        AssignmentView assignmentView;
        StudentAssignmentsViewModel vm;
        Action onExit;
        VisualElement uiRoot;

        // ── run state ──
        TrailData trail;
        TrekSession session;
        TrekWorldBuilder world;
        GameObject worldRoot;
        Camera cam;
        Vector3 camSavedPos;
        float camSavedSize;
        bool camSavedOrtho;
        Color camSavedBg;
        CameraClearFlags camSavedFlags;

        readonly TrekHud hud = new TrekHud();
        readonly GatePanelHost gatePanel = new GatePanelHost();
        readonly ReadingViewController readingView = new ReadingViewController();
        readonly CleanupCampController camp = new CleanupCampController();
        readonly TrailCompleteView complete = new TrailCompleteView();

        bool running;
        bool campSheetShown;   // the camp banner shows once per camp entry
        int totalBasePoints;

        Assignment Assignment => assignmentView?.Assignment;
        string AssignmentId => Assignment?.Id ?? "sample";
        string UserId => vm?.CurrentUserId;

        /// <summary>Start a trek. Null view/vm = placeholder practice run (GEN-4).</summary>
        public async void Begin(VisualElement root, AssignmentView view, StudentAssignmentsViewModel viewModel, Action exit)
        {
            uiRoot = root;
            assignmentView = view;
            vm = viewModel;
            onExit = exit;

            trail = BuildTrail();
            if (trail.IsTextOnlyFallback && !string.IsNullOrEmpty(trail.BuildWarning))
                Debug.LogWarning($"[Trek] {trail.BuildWarning}", this);
            totalBasePoints = 0;
            foreach (var g in trail.Gates) totalBasePoints += Mathf.Max(0, g.BasePoints);

            // R-3/FR-SHOME-5.2: entering the trek begins (or continues) the attempt.
            if (vm != null && Assignment != null && !assignmentView.Submitted)
                await vm.StartAttemptAsync(Assignment);

            // Restore the existing per-device resume point + the trek side-car.
            var progress = vm?.LoadProgress(AssignmentId);
            var checkpoint = TrekCheckpointStore.Load(UserId, AssignmentId);
            session = progress != null || checkpoint != null
                ? TrekSession.Restore(trail, new TrekConfig(), progress?.SolvedIndices, progress?.Score ?? checkpoint?.PointsSoFar ?? 0, checkpoint)
                : new TrekSession(trail, new TrekConfig());

            BuildWorld();
            BuildOverlay();
            running = true;

            // A finished-but-unsubmitted run (offline last time): retry the paperwork.
            if (progress != null && progress.Finished && session.CanComplete) { FinishTrail(); return; }

            hud.Toast(session.SolvedGates > 0 ? "Welcome back — your flag is planted." : "Walk the trail. Read as you go!");
            StartNextLeg();
        }

        /// <summary>Save and leave (system back / HUD exit). The attempt stays in progress (R-9).</summary>
        public void RequestExit()
        {
            if (session != null && session.Phase != TrekPhase.Complete) SaveResume(finished: false);
            TearDown();
            onExit?.Invoke();
        }

        void OnDisable() => TearDown();

        // ── build ─────────────────────────────────────────────────────────────

        TrailData BuildTrail()
        {
            if (vm != null && Assignment != null)
            {
                var content = vm.ContentFor(Assignment);
                return TrailBuilder.Build(Assignment.Id, Assignment.Title, content);
            }
            return SampleTrail();
        }

        // Placeholder-mode practice trek (GEN-4): the bundled passage with a few
        // gates authored in code so the full loop is playable with no services.
        static TrailData SampleTrail()
        {
            var asset = Resources.Load<TextAsset>("Trek/SamplePassage");
            var text = asset != null ? asset.text : "The reader walked the quiet trail. Every word was a step.";
            var content = new AssignmentContent();
            content.pages.Add(AssignmentContentBuilder.PageFromText(1, text));

            var seeds = new (string word, ActivityType type, string definition)[]
            {
                ("lighthouse", ActivityType.Define, "a tall coastal tower with a guiding light"),
                ("winding",    ActivityType.FillBlank, null),
                ("reagent",    ActivityType.FillBlank, null),
                ("steady",     ActivityType.FillBlank, null),
                ("fog",        ActivityType.FillBlank, null),
            };
            foreach (var page in content.pages)
                foreach (var s in page.sentences)
                    foreach (var t in s.tokens)
                        foreach (var seed in seeds)
                            if (t.isWord && t.activity == ActivityType.None &&
                                string.Equals(t.text, seed.word, StringComparison.OrdinalIgnoreCase))
                            {
                                t.activity = seed.type;
                                t.definition = seed.definition;
                                t.points = 10;
                            }

            return TrailBuilder.Build("sample", "Sample Passage", content);
        }

        void BuildWorld()
        {
            cam = Camera.main;
            if (cam != null)
            {
                camSavedPos = cam.transform.position;
                camSavedSize = cam.orthographicSize;
                camSavedOrtho = cam.orthographic;
                camSavedBg = cam.backgroundColor;
                camSavedFlags = cam.clearFlags;
                cam.orthographic = true;
                cam.orthographicSize = 5f;
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = TrekTheme.SkyTop;
            }

            worldRoot = new GameObject("TrekWorld");
            worldRoot.transform.SetParent(transform, false);
            worldRoot.transform.position = new Vector3(5000f, 5000f, 0f);   // far from the scene's other content

            world = new TrekWorldBuilder();
            world.Build(worldRoot.transform, trail, cam);

            // Paint restored gate states + plant the flag at the resume point.
            for (int i = 0; i < trail.Gates.Count; i++)
                world.PaintGate(i, trail.Gates[i].State);
            int last = LastSolvedGateIndex();
            world.PlantFlag(last);
            world.PlaceKai(last >= 0 ? world.GateX(last) + 1.4f : 0f);
            SnapCameraToKai();
        }

        void BuildOverlay()
        {
            uiRoot.Clear();
            uiRoot.Add(hud.Build(trail, OpenReadingView, RequestExit));
            hud.SetPoints(session.Points);
            hud.SetGateCounter(session.SolvedGates, session.TotalGates);
            hud.HideTimer();
            hud.Ribbon.SetSentence(session.CurrentSentenceIndex, force: true);
        }

        // ── the trek loop ─────────────────────────────────────────────────────

        void StartNextLeg()
        {
            if (!running) return;
            hud.SetGateCounter(session.SolvedGates, session.TotalGates);

            if (session.Phase == TrekPhase.Complete) { WalkToFinishThenComplete(); return; }

            var next = session.PeekNextGate();
            if (next == null) { WalkToFinishThenComplete(); return; }

            // Cleanup Camp (R-7): show the campfire sheet once, then present the
            // deferred gates in order — Kai has reached the camp; no walking back.
            if (session.Phase == TrekPhase.CleanupCamp)
            {
                if (!campSheetShown)
                {
                    campSheetShown = true;
                    camp.Open(hud.Root, DeferredGates(), OpenGateHere);
                    return;
                }
                OpenGateHere();
                return;
            }

            int gateIndex = trail.Gates.IndexOf(next);
            int fromSentence = session.CurrentSentenceIndex;
            float targetX = world.WorldX(world.GateX(gateIndex) - 1.1f);

            // Walk to the gate; the ribbon reads along with Kai (R-1), landing on
            // the gate's sentence BEFORE the panel opens (R-5).
            world.Kai.WalkTo(targetX,
                onProgress: t => hud.Ribbon.SetSentence(
                    Mathf.RoundToInt(Mathf.Lerp(fromSentence, next.SentenceIndex, t))),
                onArrive: OpenGateHere);
        }

        void OpenGateHere()
        {
            if (!running) return;
            var gate = session.ArriveAtGate();
            if (gate == null) { StartNextLeg(); return; }

            hud.Ribbon.SetSentence(gate.SentenceIndex);   // R-5: context before puzzle
            hud.SetGateCounter(session.SolvedGates, session.TotalGates);

            gatePanel.Open(hud.Root, gate, new GatePanelHost.Hooks
            {
                UseHint = () => session.UseHint(),
                RequestGuided = () => session.RequestGuidedSolve(),
                Solved = OnGateSolved,
                Later = OnGateLater
            });
        }

        void OnGateSolved()
        {
            int index = session.ActiveGateIndex;
            int earned = session.SolveActiveGate();
            gatePanel.Close();
            hud.HideTimer();

            world.PaintGate(index, trail.Gates[index].State);
            world.PlantFlag(LastSolvedGateIndex());
            world.Kai.Celebrate();
            hud.SetPoints(session.Points);
            hud.Ribbon.Refresh();                          // the word unseals (R-1)
            hud.Toast(earned > 0 ? $"+{earned} ★  “{trail.Gates[index].Word}” unlocked!" : $"“{trail.Gates[index].Word}” unlocked!");

            SaveResume(finished: false);
            StartNextLeg();
        }

        void OnGateLater()
        {
            int index = session.ActiveGateIndex;
            session.DeferActiveGate();                     // R-7: defers, never skips
            gatePanel.Close();
            hud.HideTimer();

            world.PaintGate(index, GateState.Deferred);
            hud.Toast("Saved for Cleanup Camp ⛺");
            SaveResume(finished: false);
            StartNextLeg();
        }

        void WalkToFinishThenComplete()
        {
            float targetX = world.WorldX(world.TrailEndX(trail.Gates.Count));
            int lastSentence = Mathf.Max(0, trail.Sentences.Count - 1);
            int fromSentence = session.CurrentSentenceIndex;
            world.Kai.WalkTo(targetX,
                onProgress: t => hud.Ribbon.SetSentence(
                    Mathf.RoundToInt(Mathf.Lerp(fromSentence, lastSentence, t))),
                onArrive: FinishTrail);
        }

        // ── completion (R-6/R-9/R-10) ─────────────────────────────────────────

        async void FinishTrail()
        {
            if (!running || !session.CanComplete) return;   // R-6 guard — never celebrate an uncleared trail
            var result = session.CompleteTrail(Assignment?.MaxScore ?? 0);
            world.Kai.Celebrate();

            bool submitted = false;
            string note;
            if (vm != null && Assignment != null)
            {
                // R-9: submission fires only on full clearance — through the
                // EXISTING submission path, with hint usage reported (R-10).
                var r = await vm.RecordGameResultAsync(Assignment,
                    result.MaxScore > 0 ? result.Marks : result.Points,
                    result.MaxScore > 0 ? result.MaxScore : result.TotalBasePoints,
                    "hints " + string.Join(",", result.HintReport));
                submitted = r.IsSuccess;
                if (submitted)
                {
                    vm.ClearProgress(AssignmentId);
                    TrekCheckpointStore.Clear(UserId, AssignmentId);
                    if (assignmentView != null) assignmentView.Status = AttemptStatus.Submitted;
                }
                else
                {
                    // Offline / transient failure: keep the finished run saved and
                    // say so honestly — it retries next time this trek opens.
                    SaveResume(finished: true);
                }
                note = submitted ? "" : "Couldn't reach your tutor — your result is saved and will be submitted next time you open this trek.";
            }
            else
            {
                note = "Practice trek — nothing was submitted (GEN-4 placeholder mode).";
            }

            complete.Open(hud.Root, result, submitted, note,
                onExit: () => { TearDown(); onExit?.Invoke(); },
                onRead: () => readingView.Open(hud.Root, trail, trail.Sentences.Count - 1, null));
        }

        // ── reading view (R-2/R-11) ───────────────────────────────────────────

        void OpenReadingView()
        {
            if (readingView.IsOpen) return;
            session.PauseTimer();                          // Reading View pauses the timer
            readingView.Open(hud.Root, trail, session.CurrentSentenceIndex, onClose: () => session.ResumeTimer());
        }

        // ── timer (R-11) ──────────────────────────────────────────────────────

        void Update()
        {
            if (!running || session == null) return;

            bool gateOpen = gatePanel.IsOpen && session.ActiveGate != null;
            int openIndex = session.ActiveGateIndex;   // captured before the tick: expiry clears it
            bool expired = session.TickTimer(Time.deltaTime);

            if (gateOpen)
                hud.SetTimer(session.GateSecondsLeft, session.TimerPaused);

            if (expired)
            {
                // Timer never skips: the gate auto-defers to Cleanup Camp with a
                // small points penalty and the trek continues (R-11).
                int index = openIndex;
                gatePanel.Close();
                hud.HideTimer();
                hud.Toast("⏱ Time! That word moved to Cleanup Camp.");
                if (index >= 0) world.PaintGate(index, GateState.Deferred);
                SaveResume(finished: false);
                StartNextLeg();
            }

            FollowKai();
        }

        // ── camera ────────────────────────────────────────────────────────────

        void FollowKai()
        {
            if (cam == null || world?.Kai == null) return;
            var p = cam.transform.position;
            p.x = Mathf.Lerp(p.x, world.Kai.transform.position.x + 1.6f, Time.deltaTime * 4f);
            p.y = world.WorldGroundY + 2.6f;
            p.z = -10f;
            cam.transform.position = p;
        }

        void SnapCameraToKai()
        {
            if (cam == null || world?.Kai == null) return;
            cam.transform.position = new Vector3(world.Kai.transform.position.x + 1.6f, world.WorldGroundY + 2.6f, -10f);
        }

        // ── persistence (existing resume point + trek side-car) ──────────────

        void SaveResume(bool finished)
        {
            if (vm == null || Assignment == null) return;   // placeholder: nothing persists
            vm.SaveProgress(AssignmentId, multiplayer: false,
                session.SolvedBoardIndices(), session.Points, totalBasePoints, finished);
            TrekCheckpointStore.Save(UserId, session.ToCheckpoint());
        }

        // ── teardown ──────────────────────────────────────────────────────────

        void TearDown()
        {
            running = false;
            campSheetShown = false;
            gatePanel.Close();
            camp.Close();
            complete.Close();
            readingView.Close();
            if (worldRoot != null) { Destroy(worldRoot); worldRoot = null; }
            if (cam != null)
            {
                cam.transform.position = camSavedPos;
                cam.orthographic = camSavedOrtho;
                cam.orthographicSize = camSavedSize;
                cam.backgroundColor = camSavedBg;
                cam.clearFlags = camSavedFlags;
                cam = null;
            }
        }

        // ── small helpers ─────────────────────────────────────────────────────

        int LastSolvedGateIndex()
        {
            int last = -1;
            for (int i = 0; i < trail.Gates.Count; i++)
            {
                var s = trail.Gates[i].State;
                if (s == GateState.SolvedGold || s == GateState.SolvedSilver || s == GateState.SolvedBronze) last = i;
            }
            return last;
        }

        List<GateData> DeferredGates()
        {
            var list = new List<GateData>();
            foreach (var g in trail.Gates)
                if (g.State == GateState.Deferred) list.Add(g);
            return list;
        }
    }
}
