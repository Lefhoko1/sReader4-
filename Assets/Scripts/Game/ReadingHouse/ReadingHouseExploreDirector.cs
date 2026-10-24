using System.Collections;
using UnityEngine;
using SReader.UI.Navigation;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The walk-in flow when the Reading House runs on its own:
    ///   1. Outside — a wide shot of the whole front of the house, roof included,
    ///      the student standing beside the path.
    ///   2. Exploring — move the stick and the camera eases into a follow shot;
    ///      walk up to the door and tap "Log in" / "Register". The app's existing
    ///      page opens as a frosted card on the left while the camera glides to a
    ///      shot with the house on the right. After sign-in the doors open; inside,
    ///      the front wall fades away so he can explore and reach the bookshelves.
    ///   3. Reading — tap the dashboard and the existing reading views take over;
    ///      Back returns to walking.
    /// When the app opens the house for an assignment this director stays out of
    /// the way. Presentation only: sign-in, content and rules live in existing code.
    /// </summary>
    public sealed class ReadingHouseExploreDirector : MonoBehaviour
    {
        public enum State { Outside, Exploring, Reading }

        [Header("Wiring")]
        public ReadingHousePresenter presenter;
        public ReadingHouseCameraRig cameraRig;
        public ReadingHouseFollowCamera follow;
        public StudentWalker walker;
        public TouchJoystick joystick;
        public HouseDoor door;
        public ReadingHouseHud hud;
        public Transform studentStart;
        public Transform exteriorCameraPos, exteriorCameraAim;
        public Transform signInCameraPos, signInCameraAim;
        [Tooltip("Kept for older scenes; the explore camera's own field of view is used.")]
        public float exteriorFov = 40f;

        [Header("Reused app sign-in (optional — absent when opened from the app)")]
        public NavigationManager signInNavigation;
        public GameObject loginPage, registerPage;

        [Tooltip("How close (m) the student must be to the doors to use the sign-in panels.")]
        public float panelReach = 4.5f;

        [Header("Chalkboard (the app's real pages, inside the house)")]
        public ChalkboardScreen chalkboardScreen;
        [Tooltip("The chalkboard: walk up to it and tap it to use the app on it.")]
        public Transform chalkboard;
        [Tooltip("Kept for older scenes: the board can be focused from anywhere in the room.")]
        public float boardReach = 3.5f;
        [Tooltip("Seconds for the room to turn all the way to the board (a little less to turn back).")]
        public float turnSeconds = 1.4f;
        [Tooltip("Drag distance (UI px) on the knob for a full turn.")]
        public float knobDragFull = 300f;
        [Tooltip("Hidden while the camera faces the board, so he never stands in front of it.")]
        public StudentSpriteAnimator studentSprite;
        [Range(0.6f, 1f), Tooltip("How much of the screen the board fills when the camera turns to it.")]
        public float boardFraming = 0.92f;

        public State Current { get; private set; } = State.Outside;
        public bool BoardOpen => chalkboardScreen != null && chalkboardScreen.IsOpen;
        /// <summary>0 = walking in the room ... 1 = the room has turned and the board faces the camera.</summary>
        public float BoardFocus => focus;
        public bool OverlayOpen => (loginPage != null && loginPage.activeInHierarchy) || (registerPage != null && registerPage.activeInHierarchy) || BoardOpen;
        public bool Active { get; private set; }

        string flash;
        float flashUntil;
        bool overlayWasOpen, backdropCaptured;

        [Header("Device")]
        [Range(0.5f, 1f), Tooltip("Render scale on Android phones (the UI stays sharp; the 3D room renders a little smaller).")]
        public float mobileRenderScale = 0.85f;

        void Awake()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline is UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset urp)
                urp.renderScale = mobileRenderScale;
            Application.targetFrameRate = 60;
#endif
            // The walk-in house reads only real assignments, chosen on the chalkboard: no sample book.
            if (!ReadingHousePresenter.IsLaunchPending) presenter.allowSampleContent = false;
        }

        void Start()
        {
            Active = !presenter.IsLive;
            // The attempt's activities, reader and controls live in the HUD — in the walk-in house and
            // when the app opens the house for an assignment alike.
            if (hud != null)
            {
                presenter.attemptHost = hud.OverlayHost;
                presenter.showMessage = Flash;
                hud.ReadPressed += presenter.OpenReader;
                hud.FinishPressed += presenter.FinishAttempt;
                hud.PreviousPagePressed += presenter.PreviousPage;
                hud.NextPagePressed += presenter.NextPage;
            }
            if (!Active)
            {
                if (joystick != null) joystick.enabled = false;
                if (walker != null) walker.enabled = false;
                return;
            }
            presenter.standaloneExit = ExitReading;
            if (presenter.matStones != null) presenter.matStones.walkingStudent = walker.transform;
            presenter.standaloneMenu = () => { ExitReading(); FocusBoard(); };   // the dashboard's Menu shows the app's menu on the board
            hud.KnobPressed += OnKnobPressed;
            hud.KnobDragged += OnKnobDragged;
            hud.KnobReleased += OnKnobReleased;

            // Outside: student beside the path, facing the house; wide shot of the whole front.
            walker.enabled = false;
            presenter.studentMover.student.SetPositionAndRotation(studentStart.position, studentStart.rotation);
            var intoHouse = Vector3.ProjectOnPlane(door.houseCentre.position - door.transform.position, Vector3.up).normalized;
            walker.SetFacingReference(intoHouse);
            walker.enabled = true;

            follow.SnapToPose(exteriorCameraPos.position, exteriorCameraAim.position);
            presenter.dashboard.SetDialogueVisible(false);
            Current = State.Outside;
        }

        void Update()
        {
            UpdateReadingControls();
            if (!Active) { if (hud != null) hud.SetHint(Time.time < flashUntil ? flash : null); return; }
            bool overlay = OverlayOpen;
            // The stick is always on screen. While reading it only takes a press on itself (taps on the
            // stones stay taps); pushing it walks him away from the dashboard — no menu in between.
            joystick.enabled = true;
            joystick.showStick = true;
            joystick.interactive = !overlay && !presenter.AttemptUIOpen && !walkingToDashboard;
            joystick.compactZone = Current == State.Reading;
            if (Current == State.Reading && joystick.Value.sqrMagnitude > 0.2f) ExitReading();
            bool canWalk = Current != State.Reading && !overlay;
            walker.enabled = canWalk;

            // Leaving the start spot by any means (stick, keys, or being moved) starts exploring.
            bool movedAway = Vector3.Distance(walker.transform.position, studentStart.position) > 0.6f;
            if (Current == State.Outside && canWalk && (joystick.Value.sqrMagnitude > 0.01f || movedAway))
            {
                Current = State.Exploring;
                follow.Follow();
            }

            // Sign-in card opened / closed: frame the house on the right, blur behind the card.
            // (The chalkboard has its own camera move: the room turns — see UpdateBoardFocus.)
            if (overlay && !overlayWasOpen && !BoardOpen)
            {
                follow.HoldPose(signInCameraPos.position, signInCameraAim.position);
                hud.SetVeil(true);
                backdropCaptured = false;
            }
            else if (!overlay && overlayWasOpen)
            {
                hud.SetVeil(false);
                if (Current == State.Outside) follow.HoldPose(exteriorCameraPos.position, exteriorCameraAim.position);
                else follow.Follow();
            }
            if (overlay && !BoardOpen && !backdropCaptured && follow.Settled)
            {
                FrostedBackdrop.Capture(cameraRig.viewCamera);
                backdropCaptured = true;
            }
            overlayWasOpen = overlay;

            UpdateBoardFocus();
            WelcomeOnEntering();
            hud.SetHint(Time.time < flashUntil ? flash : StandingHint());
        }

        bool wasInside, walkingToDashboard;

        /// <summary>The Encourager cheers whenever the (signed-in) student steps into the house.</summary>
        void WelcomeOnEntering()
        {
            bool inside = Current != State.Outside && door.Unlocked && IsInside();
            if (inside && !wasInside && presenter.encourager != null) presenter.encourager.Welcome();
            wasInside = inside;
        }

        /// <summary>Read / Finish / page controls while reading at the dashboard (hidden under an open activity).</summary>
        void UpdateReadingControls()
        {
            if (hud == null) return;
            bool reading = (!Active || Current == State.Reading) && presenter.IsLive && !presenter.AttemptUIOpen;
            if (presenter.dashboardScreen != null)
            {
                // The controls live on the dashboard's display.
                hud.SetReadingControls(false, false, false, false, false);
                presenter.UpdateToolbar(!Active || Current == State.Reading);
                return;
            }
            hud.SetReadingControls(reading && presenter.CanRead, reading && presenter.CanFinish,
                reading && presenter.PageCount > 1, presenter.Page > 0, presenter.Page < presenter.PageCount - 1);
        }

        /// <summary>
        /// "Play solo" on an assignment (the app's page, on the chalkboard — ClassroomLaunch sends it here inside
        /// the house): the room turns back, the student walks himself to the dashboard and the attempt opens there.
        /// </summary>
        public void OnAttemptStarted(SReader.Domains.Assignments.Models.AssignmentView view, SReader.UI.ViewModels.StudentAssignmentsViewModel vm)
        {
            if (!Active) return;
            presenter.BindAssignment(view, vm);
            StopAllCoroutines();
            walkingToDashboard = false; joystick.Simulated = null;
            StartCoroutine(WalkToDashboardAndRead());
        }

        IEnumerator WalkToDashboardAndRead()
        {
            CloseChalkboard();
            while (focus > 0.001f || focusTo >= 0f) yield return null;      // the room has turned back
            Flash("Off to the dashboard — let's read!");
            var target = presenter.studentMover.gameplaySpot.position;
            float giveUp = Time.time + 12f;
            walkingToDashboard = true;
            while (Time.time < giveUp)
            {
                var goal = presenter.matStones != null ? presenter.matStones.Waypoint(walker.transform.position, target) : target;
                var to = Vector3.ProjectOnPlane(target - walker.transform.position, Vector3.up);
                if (to.magnitude < 0.9f) break;
                to = Vector3.ProjectOnPlane(goal - walker.transform.position, Vector3.up);
                var cam = cameraRig.viewCamera.transform;
                var fwd = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
                var right = Vector3.Cross(Vector3.up, fwd);
                var d = to.normalized;
                joystick.Simulated = new Vector2(Vector3.Dot(d, right), Vector3.Dot(d, fwd)).normalized * 0.85f;
                yield return null;
            }
            joystick.Simulated = null;
            walkingToDashboard = false;
            yield return new WaitForSeconds(0.3f);
            TryStartReading();
        }

        // ── called by ReadingHouseInput / DoorUnlockPage ───────────────────────

        public void OnDoorPanelTapped(DoorPanelTarget.Kind kind)
        {
            if (!Active || OverlayOpen) return;
            if (door.Unlocked) { Flash("You're signed in — walk through the door."); return; }
            if (door.DistanceToStudent > panelReach) { Flash("Walk up to the door first."); return; }
            if (signInNavigation == null) { Flash("Sign-in isn't available here."); return; }
            if (kind == DoorPanelTarget.Kind.Login) signInNavigation.ShowLogin();
            else signInNavigation.ShowRegister();
        }

        /// <summary>A tap on the dashboard while walking inside starts reading.</summary>
        public bool TryStartReading()
        {
            if (!Active || Current == State.Reading || OverlayOpen) return false;
            if (!IsInside()) return false;
            if (presenter.LiveAssignment == null) { Flash("Choose an assignment on the chalkboard first."); return false; }
            Current = State.Reading;
            follow.Release();
            walker.enabled = false;
            presenter.EnterView(ReadingHouseView.Gameplay);
            return true;
        }

        public void ExitReading()
        {
            if (Current != State.Reading) return;
            Current = State.Exploring;
            presenter.dashboard.SetDialogueVisible(false);
            presenter.LeaveReading();
            follow.Follow();
        }

        public void OnSignedIn() => Flash("Welcome! The doors are open — go inside.");

        /// <summary>
        /// Signed out from the app's pages on the board: the board closes, the doors shut, the dashboard
        /// empties and the student is back outside beside the path.
        /// </summary>
        public void OnSignedOut()
        {
            if (!Active) return;
            dragging = false; focusTo = -1f; focus = 0f;
            if (chalkboardScreen != null) chalkboardScreen.Close();
            if (studentSprite != null) studentSprite.hidden = false;
            hud.SetBackButton(null);
            door.Lock();
            presenter.ShowNoAssignment();
            walker.enabled = false;
            presenter.studentMover.student.SetPositionAndRotation(studentStart.position, studentStart.rotation);
            walker.enabled = true;
            Current = State.Outside;
            follow.HoldPose(exteriorCameraPos.position, exteriorCameraAim.position);
            Flash("Signed out — see you soon!");
        }

        // ── the chalkboard: the app's real pages ──────────────────────────────
        // Focusing the board turns the room: the camera sweeps on an arc round the board (so the
        // house seems to rotate) until the board faces it squarely, easing in and out. The knob
        // beside the board scrubs that turn under the finger; a tap on it, or on the board, plays it.

        float focus, focusFrom, focusTo = -1f, focusClock, dragStartFocus;
        Vector3 roomPos, roomAim;
        Vector2 dragDirection;
        bool dragging;

        bool CanUseBoard => Active && chalkboardScreen != null && Current == State.Exploring && IsInside()
                            && !(loginPage != null && loginPage.activeInHierarchy) && !(registerPage != null && registerPage.activeInHierarchy);

        /// <summary>A tap on the chalkboard while walking inside: the room turns to it.</summary>
        public void OnChalkboardTapped()
        {
            if (focus < 0.01f && CanUseBoard) FocusBoard();
        }

        /// <summary>Turn the room to the board (from wherever the turn is now).</summary>
        public void FocusBoard()
        {
            if (chalkboardScreen == null || !Active) return;
            if (focus < 0.001f && !BoardOpen) BeginFocus();
            AnimateFocus(1f);
        }

        /// <summary>Turn back to the room.</summary>
        public void CloseChalkboard()
        {
            if (focus <= 0.001f && !BoardOpen) return;
            AnimateFocus(0f);
        }

        void BeginFocus()
        {
            var cam = cameraRig.viewCamera.transform;
            roomPos = cam.position;
            roomAim = cam.position + cam.forward * Vector3.Distance(cam.position, chalkboardScreen.Centre);
            chalkboardScreen.Open();                // the page is live while the room turns
            hud.SetVeil(false);
        }

        void AnimateFocus(float to)
        {
            focusFrom = focus; focusTo = to; focusClock = 0f;
        }

        void BoardPose(out Vector3 pos, out Vector3 aim)
        {
            // Face the board straight on, close enough that it fills the screen (both directions fit).
            float vfov = follow.fieldOfView * Mathf.Deg2Rad;
            float hfov = 2f * Mathf.Atan(Mathf.Tan(vfov * 0.5f) * cameraRig.viewCamera.aspect);
            var size = chalkboardScreen.Size;
            float d = Mathf.Max(size.x * 0.5f / Mathf.Tan(hfov * 0.5f), size.y * 0.5f / Mathf.Tan(vfov * 0.5f)) / boardFraming;
            aim = chalkboardScreen.Centre;
            pos = aim + chalkboardScreen.Normal * d;
        }

        /// <summary>The camera at a point of the turn: an arc round the board, eased (smootherstep).</summary>
        void ApplyFocusPose(float f)
        {
            float e = f * f * f * (f * (6f * f - 15f) + 10f);
            BoardPose(out var boardPos, out var boardAim);
            var c = chalkboardScreen.Centre;
            Vector3 o0 = roomPos - c, o1 = boardPos - c;
            Vector3 h0 = Vector3.ProjectOnPlane(o0, Vector3.up), h1 = Vector3.ProjectOnPlane(o1, Vector3.up);
            var dir = Vector3.Slerp(h0.normalized, h1.normalized, e);
            float radius = Mathf.Lerp(h0.magnitude, h1.magnitude, e);
            var pos = c + dir * radius + Vector3.up * Mathf.Lerp(o0.y, o1.y, e);
            follow.SnapToPose(pos, Vector3.Lerp(roomAim, boardAim, e));
        }

        void UpdateBoardFocus()
        {
            if (chalkboardScreen == null) return;

            if (!dragging && focusTo >= 0f)
            {
                float seconds = (focusTo > focusFrom ? turnSeconds : turnSeconds * 0.8f) * Mathf.Abs(focusTo - focusFrom);
                focusClock += Time.deltaTime;
                focus = Mathf.Lerp(focusFrom, focusTo, Mathf.Clamp01(focusClock / Mathf.Max(0.05f, seconds)));
                if (focusClock >= seconds) { focus = focusTo; focusTo = -1f; ArriveAt(focus); }
            }
            if (focus > 0.0005f || dragging) ApplyFocusPose(focus);

            if (studentSprite != null) studentSprite.hidden = focus > 0.55f;
            CandySpeechBubble.Muted = focus > 0.1f;
            hud.SetBackButton(focus > 0.999f ? CloseChalkboard : null);

            // The knob sits beside the board (on the side towards the camera), clamped onto the screen;
            // while facing the board it waits at the screen's right edge.
            Vector2? knobAt = null;
            if (focus > 0.001f || dragging || CanUseBoard)
            {
                var cam = cameraRig.viewCamera;
                var c = chalkboardScreen.Centre;
                var along = Vector3.Cross(Vector3.up, chalkboardScreen.Normal).normalized;
                if (Vector3.Dot(cam.transform.position - c, along) < 0f) along = -along;
                var sp = cam.WorldToScreenPoint(c + along * (chalkboardScreen.Size.x * 0.5f + 0.45f));
                var edge = new Vector3(Screen.width * 0.95f, Screen.height * 0.5f, 1f);
                float k = Mathf.Clamp01((focus - 0.35f) / 0.4f);
                if (sp.z > 0f || k > 0f) knobAt = Vector3.Lerp(sp.z > 0f ? sp : edge, edge, k);
            }
            hud.SetBoardKnob(knobAt, focus);
        }

        void ArriveAt(float f)
        {
            if (f >= 1f) return;                                    // facing the board: the page has the input
            chalkboardScreen.Close();
            if (studentSprite != null) studentSprite.hidden = false;
            if (Current != State.Reading) follow.Follow();
        }

        void OnKnobPressed(Vector2 _)
        {
            if (focus < 0.001f && !CanUseBoard) return;
            dragging = true;
            if (focus < 0.001f && !BoardOpen) BeginFocus();
            focusTo = -1f;
            dragStartFocus = focus;
            // Opening: drag towards the middle of the screen. Closing: drag back out to the right.
            var board = (Vector2)cameraRig.viewCamera.WorldToScreenPoint(chalkboardScreen.Centre);
            var toMiddle = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f) - board;
            dragDirection = focus > 0.5f ? Vector2.right : (toMiddle.sqrMagnitude > 100f ? toMiddle.normalized : Vector2.left);
            dragDirection.y = -dragDirection.y;                     // the knob reports panel offsets (y runs down)
        }

        void OnKnobDragged(Vector2 offset)
        {
            if (!dragging) return;
            float along = Vector2.Dot(offset, dragDirection) / knobDragFull;
            focus = Mathf.Clamp01(dragStartFocus > 0.5f ? dragStartFocus - along : dragStartFocus + along);
        }

        void OnKnobReleased(bool wasTap)
        {
            if (!dragging) return;
            dragging = false;
            if (wasTap) { AnimateFocus(dragStartFocus > 0.5f ? 0f : 1f); return; }
            // Past a third of the way it settles on the far side; otherwise it eases back.
            bool opening = dragStartFocus < 0.5f;
            AnimateFocus(opening ? (focus > 0.33f ? 1f : 0f) : (focus < 0.67f ? 0f : 1f));
        }

        /// <summary>
        /// The page's "🏠 Reading House" action, inside the house (ReadingHouseLauncher sends it here):
        /// read that real assignment on the dashboard.
        /// </summary>
        public void OnAssignmentChosen(SReader.Domains.Assignments.Models.AssignmentView view, SReader.UI.ViewModels.StudentAssignmentsViewModel vm)
        {
            CloseChalkboard();
            presenter.BindAssignment(view, vm);
            if (presenter.LiveAssignment == view && view?.Assignment != null)
                Flash($"“{view.Assignment.Title}” is on the board — tap the board to start reading.");
        }

        bool IsInside()
        {
            var outward = Vector3.ProjectOnPlane(door.transform.position - door.houseCentre.position, Vector3.up).normalized;
            return Vector3.Dot(walker.transform.position - door.transform.position, outward) < 0f;
        }

        void Flash(string text) { flash = text; flashUntil = Time.time + 3f; }

        string StandingHint()
        {
            if (Current == State.Reading || OverlayOpen) return null;
            if (Current == State.Outside) return "Use the stick (or WASD) to walk to the door";
            if (!door.Unlocked) return door.DistanceToStudent <= panelReach ? "Tap  Log in  or  Register  beside the door" : "Walk to the door to sign in";
            if (!IsInside()) return null;
            if (presenter.LiveAssignment?.Assignment != null)
                return $"Tap the board to read “{presenter.LiveAssignment.Assignment.Title}”";
            if (chalkboardScreen == null) return "Explore the room";
            return "Drag the knob by the chalkboard (or tap the board) to open your assignments";
        }
    }
}
