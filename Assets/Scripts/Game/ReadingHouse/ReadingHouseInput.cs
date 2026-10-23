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

        Vector2 pressPosition;
        bool pressing;

#if ENABLE_INPUT_SYSTEM
        void Update()
        {
            var pointer = Pointer.current;
            if (pointer != null)
            {
                if (pointer.press.wasPressedThisFrame) { pressing = true; pressPosition = pointer.position.ReadValue(); }
                if (pressing && pointer.press.wasReleasedThisFrame)
                {
                    pressing = false;
                    var p = pointer.position.ReadValue();
                    if ((p - pressPosition).magnitude <= maxTapDragPixels) Tap(p);
                }
            }
            // Android's system Back arrives as the Escape key.
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (director != null && director.BoardOpen) director.CloseChalkboard();   // leave the chalkboard first
                else presenter.RequestExit();
            }
            if (Debug.isDebugBuild) DevelopmentKeys();
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
