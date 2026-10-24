using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// On-screen movement stick for phones, plus WASD / arrow keys for testing on
    /// a PC. Press anywhere in the bottom-left quarter of the screen and drag;
    /// <see cref="Value"/> is the resulting direction (length 0..1). Drawn with
    /// IMGUI so it needs no Canvas or UI assets. Touches that start inside the
    /// stick zone are reported via <see cref="OwnsPointer"/> so they are not also
    /// treated as taps on the 3D scene. Keys are ignored while the player is typing into a text
    /// field (a password, a search box) on any page. The stick is always drawn; while walking is
    /// not possible (reading, a page open) it is drawn faded and gives no input.
    /// </summary>
    public sealed class TouchJoystick : MonoBehaviour
    {
        [Tooltip("Fraction of screen height used for the stick's radius.")]
        public float radiusFraction = 0.11f;
        public bool showStick = true;
        [Tooltip("Walking is possible right now (otherwise the stick is drawn faded and reports nothing).")]
        public bool interactive = true;
        [Tooltip("Only a press on the stick itself takes it (not the whole corner) — while reading, so taps on the stones stay taps.")]
        public bool compactZone;
        [Tooltip("The scene camera (to tell a press on a sign from a press for the stick); Camera.main when empty.")]
        public Camera viewCamera;
        static Camera sceneCamera;

        public Vector2 Value { get; private set; }
        public bool OwnsPointer { get; private set; }

        /// <summary>For automated play-mode tests: when set, overrides touch/keyboard.</summary>
        public Vector2? Simulated { get; set; }

        Vector2 origin, current;
        Texture2D ring, knob;

        void OnDisable() { Value = Vector2.zero; OwnsPointer = false; }

        void Update()
        {
            if (viewCamera != null) sceneCamera = viewCamera;
            Vector2 stick = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
            var pointer = interactive ? Pointer.current : null;
            float radius = Screen.height * radiusFraction;
            if (pointer != null)
            {
                var p = pointer.position.ReadValue();
                if (pointer.press.wasPressedThisFrame && (compactZone ? Vector2.Distance(p, RestCentre) <= radius * 1.4f : InZone(p) && !OverSign(p))) { OwnsPointer = true; origin = compactZone ? RestCentre : p; }
                if (OwnsPointer && pointer.press.isPressed)
                {
                    current = p;
                    stick = Vector2.ClampMagnitude((current - origin) / radius, 1f);
                }
                if (pointer.press.wasReleasedThisFrame) OwnsPointer = false;
            }
            else OwnsPointer = false;
            var k = Keyboard.current;
            if (k != null && interactive && !TextInputFocus.Typing)
            {
                var keys = new Vector2(
                    (k.dKey.isPressed || k.rightArrowKey.isPressed ? 1 : 0) - (k.aKey.isPressed || k.leftArrowKey.isPressed ? 1 : 0),
                    (k.wKey.isPressed || k.upArrowKey.isPressed ? 1 : 0) - (k.sKey.isPressed || k.downArrowKey.isPressed ? 1 : 0));
                if (keys.sqrMagnitude > 0f) stick = keys.normalized;
            }
#endif
            if (Simulated.HasValue) stick = Simulated.Value;   // automated tests drive the stick
            Value = stick;
        }

        /// <summary>Bottom-left quarter of the screen, in Input System coordinates (origin bottom-left).</summary>
        /// <summary>Would a press here take the stick (rather than being a tap / drag on the scene)?</summary>
        public bool Claims(Vector2 p) => isActiveAndEnabled && interactive &&
            (compactZone ? Vector2.Distance(p, RestCentre) <= Screen.height * radiusFraction * 1.4f : InZone(p) && !OverSign(p));

        /// <summary>A press on a signpost sign (outside the house) is a tap on it, even inside the stick's corner.</summary>
        static bool OverSign(Vector2 p)
        {
            var cam = sceneCamera != null ? sceneCamera : Camera.main;
            if (cam == null) return false;
            if (Vector2.Distance(p, RestCentre) <= Screen.height * 0.11f) return false;   // the stick itself always wins
            return Physics.Raycast(cam.ScreenPointToRay(p), out var hit, 120f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)
                   && hit.collider.GetComponentInParent<SignpostButton>() != null;
        }

        static Vector2 RestCentre => new Vector2(Screen.width * 0.12f, Screen.height * 0.2f);

        static bool InZone(Vector2 p) => p.x < Screen.width * 0.32f && p.y < Screen.height * 0.55f;

        void OnGUI()
        {
            if (!showStick) return;
            if (ring == null) { ring = Disc(128, 0.42f, false); knob = Disc(64, 0.85f, true); }
            var tint = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, interactive ? 1f : 0.35f);
            float radius = Screen.height * radiusFraction;
            Vector2 centre = OwnsPointer ? origin : RestCentre;
            Vector2 knobPos = OwnsPointer ? centre + Vector2.ClampMagnitude(current - centre, radius) : centre;
            // IMGUI's y axis points down.
            GUI.DrawTexture(new Rect(centre.x - radius, Screen.height - centre.y - radius, radius * 2, radius * 2), ring);
            float kr = radius * 0.45f;
            GUI.DrawTexture(new Rect(knobPos.x - kr, Screen.height - knobPos.y - kr, kr * 2, kr * 2), knob);
            GUI.color = tint;
        }

        static Texture2D Disc(int size, float alpha, bool solid)
        {
            var t = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            float r = size / 2f;
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(r, r)) / r;
                    float a = solid ? (d <= 1f ? alpha : 0f) : (d <= 1f && d >= 0.86f ? alpha : d < 0.86f ? alpha * 0.25f : 0f);
                    t.SetPixel(x, y, new Color(1f, 0.97f, 0.9f, a));
                }
            t.Apply();
            return t;
        }
    }

    /// <summary>
    /// True while a text field has the keyboard on any UI Toolkit page (sign-in, the chalkboard's pages,
    /// the HUD) or in IMGUI, or the phone's on-screen keyboard is up — so typed letters and digits never
    /// walk the student or trigger shortcut keys.
    /// </summary>
    public static class TextInputFocus
    {
        static UIDocument[] docs = new UIDocument[0];
        static float nextScan;
        static int frame = -1;
        static bool typing;

        public static bool Typing
        {
            get
            {
                if (frame == Time.frameCount) return typing;
                frame = Time.frameCount;
                typing = Compute();
                return typing;
            }
        }

        static bool Compute()
        {
            if (GUIUtility.keyboardControl != 0) return true;
            if (TouchScreenKeyboard.isSupported && TouchScreenKeyboard.visible) return true;
            if (Time.unscaledTime >= nextScan)
            {
                docs = Object.FindObjectsByType<UIDocument>(FindObjectsSortMode.None);
                nextScan = Time.unscaledTime + 1f;
            }
            foreach (var d in docs)
            {
                if (d == null || !d.isActiveAndEnabled) continue;
                var focused = d.rootVisualElement?.panel?.focusController?.focusedElement as VisualElement;
                if (focused == null) continue;
                if (IsTextInput(focused)) return true;
            }
            return false;
        }

        /// <summary>Any text / number field (TextField, IntegerField, …): they all derive from TextInputBaseField.</summary>
        static bool IsTextInput(VisualElement e)
        {
            for (var v = e; v != null; v = v.parent)
                for (var t = v.GetType(); t != null && t != typeof(VisualElement); t = t.BaseType)
                    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(TextInputBaseField<>)) return true;
            return false;
        }
    }
}
