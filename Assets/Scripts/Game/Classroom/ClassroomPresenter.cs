using System;
using System.Collections.Generic;
using SReader.Domains.Assignments.Models;
using SReader.Game.Trek;
using SReader.UI.ViewModels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SReader.Game.Classroom
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class ClassroomPresenter : MonoBehaviour
    {
        public ClassroomCameraRig rig;
        public ClassroomBoard board;
        public AudioClip successChime;
        public int secondsPerWord = 45;
        public TrekSession Session { get; private set; }
        public bool ReadingOpen => reader.IsOpen;
        public bool ActivityOpen => activity.IsOpen;
        public VisualElement Overlay => host;
        readonly GatePanelHost activity = new GatePanelHost();
        readonly ReadingViewController reader = new ReadingViewController();
        readonly List<Renderer> stones = new List<Renderer>();
        MaterialPropertyBlock feedback;
        VisualElement host, safeRoot;
        Label instruction;
        Button back;
        StudentAssignmentsViewModel vm;
        AssignmentView assignment;
        TrailData trail;
        AudioSource audioSource;
        Rect safeArea;
        Vector2 pressedPosition;
        bool pressed, ready, submitting, submitted, finishing;
        int pendingWord = -1;
        bool pendingReading;
        int total;
        float nextTimerLabel;

        void Start()
        {
            feedback = new MaterialPropertyBlock();
            var context = ClassroomLaunch.Current;
            vm = context?.ViewModel; assignment = context?.View;
            var content = vm != null ? vm.ContentFor(assignment.Assignment) : AssignmentContentCodec.FromJson(board.previewContent.text);
            trail = TrailBuilder.Build(assignment?.Assignment.Id ?? "classroom-practice", assignment?.Assignment.Title ?? "Meadow at Dawn", content);
            foreach (var gate in trail.Gates) total += Mathf.Max(0, gate.BasePoints);
            var progress = vm?.LoadProgress(trail.AssignmentId);
            var checkpoint = vm != null ? TrekCheckpointStore.Load(vm.CurrentUserId, trail.AssignmentId) : null;
            // Hint levels also determine restored word colours; seed before calling the shared restore method.
            if (checkpoint != null)
                foreach (var gate in trail.Gates)
                    if (checkpoint.HintsUsedByGate.TryGetValue(gate.GateId, out var level)) gate.HintsUsed = level;
            Session = TrekSession.Restore(trail, new TrekConfig { SecondsPerGate = secondsPerWord }, progress?.SolvedIndices,
                progress?.Score ?? checkpoint?.PointsSoFar ?? 0, checkpoint);
            BuildOverlay();
            if(vm == null) activity.TryLoadLocalImage = ClassroomPracticePictures.TryDraw;
            board.WordSelected += SelectWord; board.FinishRequested += Finish; board.PageChanged += RefreshStones;
            rig.Arrived += CameraArrived;
            board.Bind(trail, Session);
            var source = GameObject.Find("Blender Classroom Geometry");
            if (source)
                foreach (var renderer in source.GetComponentsInChildren<Renderer>())
                    if (renderer.name.StartsWith("SR_WORD_STONE_GOLD_")) stones.Add(renderer);
            audioSource = gameObject.AddComponent<AudioSource>(); audioSource.playOnAwake = false; audioSource.spatialBlend = 0;
            Refresh(); ready = true;
            instruction.text = vm == null ? "Practice reading  ·  Tap the book or teaching board" : "Tap the book to read, or the board to restore words";
            if (progress != null && progress.Finished && Session.CanComplete) Finish();
        }

        void BuildOverlay()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            root.pickingMode = PickingMode.Ignore;
            safeRoot = new VisualElement { pickingMode = PickingMode.Ignore };
            safeRoot.style.position = Position.Absolute;
            root.Add(safeRoot);
            host = new VisualElement { pickingMode = PickingMode.Ignore };
            host.style.position = Position.Absolute;
            host.style.left = host.style.right = host.style.top = host.style.bottom = 0;
            safeRoot.Add(host);
            back = new Button(Back) { text = "Leave classroom", name = "classroom-back" };
            back.style.position = Position.Absolute; back.style.left = 10; back.style.top = 6;
            back.style.height = 48; back.style.fontSize = 15;
            back.style.color = new Color(.97f,.94f,.83f); back.style.backgroundColor = new Color(.12f,.17f,.17f,.85f);
            safeRoot.Add(back);
            instruction = new Label { pickingMode = PickingMode.Ignore };
            instruction.style.position = Position.Absolute; instruction.style.left = 16; instruction.style.right = 16; instruction.style.bottom = 10;
            instruction.style.whiteSpace = WhiteSpace.Normal; instruction.style.fontSize = 14;
            instruction.style.color = new Color(.98f,.95f,.83f); instruction.style.unityTextAlign = TextAnchor.MiddleCenter;
            instruction.style.backgroundColor = new Color(.12f,.17f,.17f,.8f); instruction.style.paddingTop = instruction.style.paddingBottom = 8;
            safeRoot.Add(instruction);
            UpdateSafeArea();
        }

        void UpdateSafeArea()
        {
            safeArea = Screen.safeArea;
            // Percentages follow the panel's reference resolution and work across portrait aspect ratios.
            safeRoot.style.left = Length.Percent(safeArea.xMin / Screen.width * 100);
            safeRoot.style.right = Length.Percent((Screen.width - safeArea.xMax) / Screen.width * 100);
            safeRoot.style.top = Length.Percent((Screen.height - safeArea.yMax) / Screen.height * 100);
            safeRoot.style.bottom = Length.Percent(safeArea.yMin / Screen.height * 100);
        }

        void Update()
        {
            if (!ready) return;
            if (safeArea != Screen.safeArea) UpdateSafeArea();
            if (Session.TickTimer(Time.deltaTime)) { activity.Close(); Save(); Refresh(); instruction.text = "Time to move on. That word is waiting in Later."; }
            if (activity.IsOpen && Time.unscaledTime >= nextTimerLabel)
            { nextTimerLabel = Time.unscaledTime + .25f; instruction.text = secondsPerWord > 0 ? $"{Mathf.CeilToInt(Session.GateSecondsLeft)}s  ·  Later keeps this word for the end" : "Take your time. Later keeps this word for the end."; }
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) { Back(); return; }
            var touch = Touchscreen.current?.primaryTouch;
            if (touch != null && touch.press.wasPressedThisFrame) Press(touch.position.ReadValue());
            if (touch != null && touch.press.wasReleasedThisFrame) Release(touch.position.ReadValue(), touch.touchId.ReadValue());
            if (Touchscreen.current == null || !Touchscreen.current.primaryTouch.press.isPressed)
            {
                var mouse = Mouse.current;
                if (mouse != null && mouse.leftButton.wasPressedThisFrame) Press(mouse.position.ReadValue());
                if (mouse != null && mouse.leftButton.wasReleasedThisFrame) Release(mouse.position.ReadValue(), -1);
            }
#endif
        }

        void Press(Vector2 position) { pressed = true; pressedPosition = position; }
        void Release(Vector2 position, int pointerId)
        {
            if (!pressed) return; pressed = false;
            if (Vector2.Distance(position, pressedPosition) > Screen.width * .035f || rig.IsMoving || activity.IsOpen || reader.IsOpen || submitting) return;
            // uGUI handles the world-space word buttons; UI Toolkit handles the small Back action.
            if (EventSystem.current && EventSystem.current.IsPointerOverGameObject(pointerId)) return;
            var panel = host.panel;
            var point = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(position.x, Screen.height - position.y));
            var picked = panel.Pick(point);
            if (picked != null && (picked == back || back.Contains(picked))) return;
            if (Physics.Raycast(rig.viewCamera.ScreenPointToRay(position), out var hit, 40))
            {
                var target = hit.collider.GetComponentInParent<ClassroomTapTarget>();
                if (target) Interact(target);
            }
        }

        public void Interact(ClassroomTapTarget target)
        {
            if (!ready || rig.IsMoving || activity.IsOpen || reader.IsOpen || submitting) return;
            switch (target.kind)
            {
                case ClassroomTapTarget.Kind.Board: rig.GoTo(1); break;
                case ClassroomTapTarget.Kind.Book: OpenBook(); break;
                case ClassroomTapTarget.Kind.Lexicon: rig.GoTo(1); instruction.text = "Choose the next word on the teaching board"; break;
                case ClassroomTapTarget.Kind.Word: rig.GoTo(1); board.ShowWord(target.wordIndex); break;
            }
        }

        public void OpenBook()
        {
            if (!ready || activity.IsOpen || reader.IsOpen) return;
            pendingReading = true; Session.PauseTimer(); rig.GoTo(2);
        }

        void CameraArrived(int index)
        {
            if (index == 2 && pendingReading)
            {
                pendingReading = false;
                reader.Open(host, trail, Session.CurrentSentenceIndex, () => { Session.ResumeTimer(); rig.GoTo(0); Refresh(); }, classroomMode:true);
                instruction.style.display = DisplayStyle.None; back.style.display = DisplayStyle.None;
                ImproveTargets(host);
            }
            if (index == 1 && pendingWord >= 0) { int next = pendingWord; pendingWord = -1; SelectWord(next); }
            if (index == 0 || index == 1) Refresh();
        }

        public void SelectWord(int index)
        {
            if (!ready || activity.IsOpen || reader.IsOpen || submitting || index < 0 || index >= trail.Gates.Count || !board.Available(index)) return;
            if (rig.currentView != 1 || rig.IsMoving) { pendingWord = index; rig.GoTo(1); return; }
            var gate = Session.ActiveGate ?? Session.ArriveAtGate();
            if (gate != trail.Gates[index]) return;
            board.Refresh(); RefreshStones();
            activity.Open(host, gate, new GatePanelHost.Hooks
            {
                UseHint = () => { int level = Session.UseHint(); Save(); return level; },
                RequestGuided = () => { Session.RequestGuidedSolve(); Save(); },
                Solved = () => { Session.SolveActiveGate(); activity.Close(); Save(); Refresh(); if(successChime) audioSource.PlayOneShot(successChime,.35f); instruction.text = "A word restored. Its gold ink belongs to the library."; },
                Later = () => { Session.DeferActiveGate(); activity.Close(); Save(); Refresh(); instruction.text = "Saved for Later. Restore the next word first."; }
            });
            var overlay = host[host.childCount - 1]; var sheet = overlay[0];
            sheet.style.marginBottom = 54; sheet.style.backgroundColor = new Color(.96f,.92f,.81f);
            sheet.style.borderTopLeftRadius = sheet.style.borderTopRightRadius = 3;
            sheet.style.borderBottomLeftRadius = sheet.style.borderBottomRightRadius = 3;
            sheet.style.maxHeight = Length.Percent(78);
            var scroll = new ScrollView(ScrollViewMode.Vertical); scroll.style.flexShrink = 1;
            var children = new List<VisualElement>(sheet.Children()); foreach (var child in children) scroll.Add(child); sheet.Add(scroll);
            // Show the word's original sentence before asking for its activity.
            if (gate.SentenceIndex >= 0 && gate.SentenceIndex < trail.Sentences.Count)
            { var span = trail.Sentences[gate.SentenceIndex]; string sentence = trail.PassageText.Substring(span.Start, span.Length);
                if(gate.Type == GateType.Fill){int offset=gate.PassageCharIndex-span.Start;sentence=sentence.Substring(0,offset)+"____"+sentence.Substring(offset+gate.Word.Length);}
                var context = new Label(sentence); context.style.whiteSpace = WhiteSpace.Normal; context.style.fontSize = 16; context.style.marginBottom = 10; context.style.color = new Color(.18f,.21f,.20f); scroll.Insert(0,context); }
            ImproveTargets(host); back.style.display = DisplayStyle.None;
        }

        static void ImproveTargets(VisualElement element)
        {
            element.Query<Button>().ForEach(button => { button.style.minHeight = 48; button.style.minWidth = 48; button.style.height = StyleKeyword.Auto; button.style.fontSize = 16; button.style.whiteSpace = WhiteSpace.Normal; });
        }

        void Refresh()
        {
            board.Refresh(); RefreshStones();
            if (instruction == null) return;
            instruction.style.display = reader.IsOpen ? DisplayStyle.None : DisplayStyle.Flex; back.style.display = activity.IsOpen || reader.IsOpen ? DisplayStyle.None : DisplayStyle.Flex;
            back.text = rig.currentView == 0 ? "Leave classroom" : "Back to classroom";
            instruction.text = submitted ? "Your reading has been submitted" : Session.CanComplete ? "Every word restored. Tap Words completed on the board to finish." : rig.currentView == 1 ? "Tap the next bright word  ·  Gold words are restored" : "Tap the hero book to read, or the teaching board for words";
        }

        void RefreshStones()
        {
            foreach (var stone in stones)
            {
                if (!int.TryParse(stone.name.Substring(stone.name.LastIndexOf('_') + 1), out int slot)) continue;
                // The five preserved stones reflect the visible chalkboard page for long assignments.
                int index = slot < board.wordLabels.Length ? board.PageStart + slot : trail.Gates.Count; bool solved = index < trail.Gates.Count && ClassroomBoard.IsSolved(trail.Gates[index].State);
                bool active = index >= 0 && index < trail.Gates.Count && Session.ActiveGate == trail.Gates[index];
                stone.GetPropertyBlock(feedback);
                feedback.SetColor("_BaseColor", solved ? new Color(.91f,.67f,.23f) : active ? new Color(.98f,.9f,.57f) : new Color(.37f,.39f,.31f));
                feedback.SetColor("_EmissionColor", solved ? new Color(.23f,.12f,.015f) : active ? new Color(.16f,.12f,.04f) : Color.black);
                stone.SetPropertyBlock(feedback);
            }
        }

        public async void Finish()
        {
            if (!ready || submitting || finishing || !Session.CanComplete || submitted) return;
            finishing = true; submitting = true; Session.PauseTimer();
            try
            {
                var result = Session.CompleteTrail(assignment?.Assignment.MaxScore ?? 0);
                Save(true);
                if (vm != null)
                {
                    var response = await vm.RecordGameResultAsync(assignment.Assignment,
                        result.MaxScore > 0 ? result.Marks : result.Points, result.MaxScore > 0 ? result.MaxScore : result.TotalBasePoints,
                        "hints " + string.Join(",", result.HintReport));
                    if (!this) return;
                    submitted = response.IsSuccess;
                    if (submitted) { vm.ClearProgress(trail.AssignmentId); TrekCheckpointStore.Clear(vm.CurrentUserId,trail.AssignmentId); assignment.Status = AttemptStatus.Submitted; }
                    instruction.text = submitted ? "Reading complete. Your result has been submitted." : "Result saved on this device. Tap completion to retry submission.";
                }
                else instruction.text = $"Practice complete  ·  {result.Points}/{result.TotalBasePoints}  ·  Nothing submitted";
            }
            catch (Exception exception) { Debug.LogException(exception); if(this) instruction.text = "Result saved. Tap completion to retry when connected."; }
            finally { submitting = false; finishing = false; }
        }

        public void Back()
        {
            if (submitting) return;
            if (reader.IsOpen) { reader.Close(() => Session.ResumeTimer()); rig.GoTo(0); Refresh(); return; }
            if (activity.IsOpen) { activity.Close(); Save(); rig.GoTo(0); Refresh(); return; }
            if (rig.currentView != 0) { rig.GoTo(0); return; }
            Save(Session.CanComplete && Session.Phase == TrekPhase.Complete);
            if (ClassroomLaunch.Current != null) ClassroomLaunch.Close();
            else { instruction.text = "Practice scene. Tap the book to read again."; }
        }

        void Save(bool finished = false)
        {
            if (vm == null || Session == null || submitted) return;
            vm.SaveProgress(trail.AssignmentId,false,Session.SolvedBoardIndices(),Session.Points,total,finished);
            TrekCheckpointStore.Save(vm.CurrentUserId,Session.ToCheckpoint());
        }
        void OnApplicationPause(bool pause) { if(!ready)return; if(pause){Session.PauseTimer();Save(Session.CanComplete && Session.Phase == TrekPhase.Complete);} else if(!reader.IsOpen)Session.ResumeTimer(); }
        void OnDestroy()
        {
            if (ready) Save(Session.CanComplete && Session.Phase == TrekPhase.Complete);
            if (board) { board.WordSelected -= SelectWord; board.FinishRequested -= Finish; board.PageChanged -= RefreshStones; }
            if (rig) rig.Arrived -= CameraArrived;
            activity.Close(); reader.Close();
        }
    }
}
