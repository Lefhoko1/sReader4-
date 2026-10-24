using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// Touch/mouse taps on the 3D dashboard: a tap (press and release without a
    /// drag) raycasts from the active camera and forwards a stone or a dashboard
    /// control to the presenter. Taps are ignored while the camera is moving.
    /// A tap on the chalkboard opens the app's pages on it (walking inside).
    /// Back / Escape leaves the chalkboard when it is open; otherwise it
    /// leaves through the presenter (the app's navigation when launched from it).
    /// Development keys (editor / dev builds): 1-3 camera views, N/P sentences,
    /// T layout test passage, B reload the current source.
    /// </summary>
    public sealed class ReadingHouseInput : MonoBehaviour
    {
        public Camera viewCamera;
        public ReadingHousePresenter presenter;
        public ReadingHouseCameraRig cameraRig;
        [Tooltip("Optional: the walk-in flow (door panels, starting to read).")]
        public ReadingHouseExploreDirector director;
        public TouchJoystick joystick;
        public float maxTapDragPixels = 24f;
        [Tooltip("Dragging empty space swings the view: degrees per screen pixel, and metres of height per pixel.")]
        public float orbitDegreesPerPixel = 0.25f;
        public float orbitHeightPerPixel = 0.006f;

        Vector2 pressPosition, lastDrag;
        bool pressing, orbiting, pressOnDie;

        bool OnDie(Vector2 screen) => viewCamera != null &&
            Physics.Raycast(viewCamera.ScreenPointToRay(screen), out var hit, 80f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide) &&
            hit.collider.GetComponentInParent<ReadingDie>() != null;

#if ENABLE_INPUT_SYSTEM
        void Update()
        {
            var pointer = Pointer.current;
            if (pointer != null)
            {
                if (pointer.press.wasPressedThisFrame)
                {
                    pressing = true; pressPosition = lastDrag = pointer.position.ReadValue();
                    orbiting = CanOrbitFrom(pressPosition);
                    pressOnDie = OnDie(pressPosition);
                }
                if (pressing && orbiting && pointer.press.isPressed)
                {
                    var p = pointer.position.ReadValue();
                    if ((p - pressPosition).magnitude > maxTapDragPixels)
                        director.follow.Orbit((p - lastDrag).x * orbitDegreesPerPixel, -(p - lastDrag).y * orbitHeightPerPixel);
                    lastDrag = p;
                }
                if (pressing && pointer.press.wasReleasedThisFrame)
                {
                    pressing = false;
                    var p = pointer.position.ReadValue();
                    if ((p - pressPosition).magnitude <= maxTapDragPixels) Tap(p);
                    else if (pressOnDie && Mathf.Abs((p - pressPosition).x) > maxTapDragPixels) presenter.OnDieFlip(p.x > pressPosition.x ? 1 : -1);   // a swipe flips the die
                }
            }
            // Android's system Back arrives as the Escape key.
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && !TextInputFocus.Typing)
            {
                if (director != null && director.BoardOpen) director.CloseChalkboard();   // leave the chalkboard first
                else presenter.RequestExit();
            }
            if (Debug.isDebugBuild && !TextInputFocus.Typing) DevelopmentKeys();
        }

        /// <summary>A press on empty scene (not the stick, a HUD control, the knob or an open page) may swing the view round.</summary>
        bool CanOrbitFrom(Vector2 p)
        {
            if (director == null || director.follow == null || !director.Active || !director.follow.Engaged) return false;
            if (director.OverlayOpen || director.BoardFocus > 0.001f || director.Current == ReadingHouseExploreDirector.State.Reading) return false;
            if (joystick != null && joystick.Claims(p)) return false;
            if (director.hud != null && (director.hud.KnobBusy || director.hud.IsOverUI(p))) return false;
            return true;
        }

        void DevelopmentKeys()
        {
            var k = Keyboard.current;
            if (k == null) return;
            if (k.digit1Key.wasPressedThisFrame) presenter.EnterView(ReadingHouseView.Overview);
            if (k.digit2Key.wasPressedThisFrame) presenter.EnterView(ReadingHouseView.Gameplay);
            if (k.digit3Key.wasPressedThisFrame) presenter.EnterView(ReadingHouseView.TrayFocus);
            if (k.nKey.wasPressedThisFrame) presenter.NextSentence();
            if (k.pKey.wasPressedThisFrame) presenter.PreviousSentence();
            if (k.tKey.wasPressedThisFrame) presenter.LoadTestPassage();
            if (k.bKey.wasPressedThisFrame) presenter.LoadFromBestSource();
        }
#endif

        public void Tap(Vector2 screenPosition)
        {
            if (viewCamera == null || presenter == null) return;
            if (joystick != null && joystick.OwnsPointer) return;          // that was a walk, not a tap
            if (cameraRig != null && cameraRig.IsMoving) return;
            if (director != null && director.OverlayOpen) return;            // the sign-in card / the board has the input
            if (director != null && director.hud != null && director.hud.KnobBusy) return;   // that was the board knob
            if (KeywordCard.HandleTap(screenPosition, viewCamera)) return;
            if (presenter.AttemptUIOpen) return;                                           // a word activity / the reader has the input
            // The reading die: a tap on one of its faces is a pick.
            if (Physics.Raycast(viewCamera.ScreenPointToRay(screenPosition), out var dieHit, 80f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
            {
                var die = dieHit.collider.GetComponentInParent<ReadingDie>();
                if (die != null) { presenter.OnDieTapped(die.ChoiceAt(dieHit.point)); return; }
            }
            // The Word Path stones on the rug have trigger colliders (the student walks over them): look for one first.
            var ray = viewCamera.ScreenPointToRay(screenPosition);
            foreach (var h in Physics.RaycastAll(ray, 80f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
            {
                var ws = h.collider.GetComponentInParent<WordStone>();
                if (ws != null && presenter.matStones != null && presenter.matStones.TryGetWord(ws, out _)) { presenter.OnMatStoneTapped(ws); return; }
            }
            // Invisible walls live on Ignore Raycast, so they block walking but never a tap.
            if (!Physics.Raycast(viewCamera.ScreenPointToRay(screenPosition), out var hit, 80f,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) return;

            var sign = hit.collider.GetComponentInParent<SignpostButton>();
            if (sign != null) { sign.Press(); director?.OnSignpost(sign.kind); return; }
            var panel = hit.collider.GetComponentInParent<DoorPanelTarget>();
            if (panel != null) { director?.OnDoorPanelTapped(panel.kind); return; }
            if (hit.collider.GetComponentInParent<AssignmentsBoardTarget>() != null) { director?.OnChalkboardTapped(); return; }

            // Walking inside: a tap on the dashboard starts reading.
            if (director != null && director.Active && director.Current != ReadingHouseExploreDirector.State.Reading)
            {
                if (hit.collider.transform.root.name == "Dashboard" || hit.collider.GetComponentInParent<WordStoneView>() != null)
                    director.TryStartReading();
                return;
            }

            var stone = hit.collider.GetComponentInParent<WordStoneView>();
            if (stone != null) { presenter.OnStoneTapped(stone); return; }

            var control = hit.collider.GetComponent<DashboardControlTarget>();
            if (control != null) presenter.OnControlTapped(control.control);
        }
    }
}
