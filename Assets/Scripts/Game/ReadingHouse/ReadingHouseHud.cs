using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The walk-in HUD, in UI Toolkit with the app's design language (dark slate
    /// glass, parchment text, 18px radius): a hint pill at the top that fades and
    /// slides between messages, and a soft left-side veil that eases in behind the
    /// sign-in card so the card always reads cleanly against the bright scene.
    /// Built in code, so there is no UXML to maintain; ignores pointer input.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public sealed class ReadingHouseHud : MonoBehaviour
    {
        static readonly Color Slate = new Color(18f / 255f, 20f / 255f, 25f / 255f, 0.78f);
        static readonly Color Parchment = new Color32(242, 239, 230, 255);
        static readonly Color Hairline = new Color(233f / 255f, 223f / 255f, 200f / 255f, 0.35f);

        VisualElement veil;
        Label hint;
        string shown = "";
        IVisualElementScheduledItem pendingSwap;

        void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            root.Clear();
            root.pickingMode = PickingMode.Ignore;
            root.style.position = Position.Absolute;
            root.style.left = 0; root.style.right = 0; root.style.top = 0; root.style.bottom = 0;

            veil = new VisualElement { name = "veil", pickingMode = PickingMode.Ignore };
            veil.style.position = Position.Absolute;
            veil.style.left = 0; veil.style.top = 0; veil.style.bottom = 0;
            veil.style.width = Length.Percent(58);
            veil.style.backgroundImage = Background.FromTexture2D(LeftFade());
            veil.style.opacity = 0f;
            AuthCardLayout.Transition(veil, "opacity", 0.45f);
            root.Add(veil);

            hint = new Label { name = "hint", pickingMode = PickingMode.Ignore };
            hint.style.position = Position.Absolute;
            hint.style.top = Length.Percent(4);
            hint.style.alignSelf = Align.Center;
            hint.style.left = Length.Percent(50);
            hint.style.translate = new Translate(Length.Percent(-50), 0);
            hint.style.backgroundColor = Slate;
            hint.style.color = Parchment;
            hint.style.fontSize = 17;
            hint.style.unityFontStyleAndWeight = FontStyle.Bold;
            hint.style.paddingLeft = 20; hint.style.paddingRight = 20;
            hint.style.paddingTop = 9; hint.style.paddingBottom = 9;
            hint.style.borderTopLeftRadius = 18; hint.style.borderTopRightRadius = 18;
            hint.style.borderBottomLeftRadius = 18; hint.style.borderBottomRightRadius = 18;
            hint.style.borderTopColor = Hairline; hint.style.borderBottomColor = Hairline;
            hint.style.borderLeftColor = Hairline; hint.style.borderRightColor = Hairline;
            hint.style.borderTopWidth = 1; hint.style.borderBottomWidth = 1; hint.style.borderLeftWidth = 1; hint.style.borderRightWidth = 1;
            hint.style.opacity = 0f;
            AuthCardLayout.TransitionMany(hint, 0.28f, "opacity");
            root.Add(hint);
            shown = "";

            back = new Button(() => onBack?.Invoke()) { name = "back-to-room", text = "‹  Back to the room" };
            back.style.position = Position.Absolute;
            back.style.left = Length.Percent(2); back.style.top = Length.Percent(4);
            back.style.backgroundColor = Slate;
            back.style.color = Parchment;
            back.style.fontSize = 16;
            back.style.unityFontStyleAndWeight = FontStyle.Bold;
            back.style.paddingLeft = 18; back.style.paddingRight = 18; back.style.paddingTop = 9; back.style.paddingBottom = 9;
            back.style.borderTopLeftRadius = 18; back.style.borderTopRightRadius = 18;
            back.style.borderBottomLeftRadius = 18; back.style.borderBottomRightRadius = 18;
            back.style.borderTopColor = Hairline; back.style.borderBottomColor = Hairline;
            back.style.borderLeftColor = Hairline; back.style.borderRightColor = Hairline;
            back.style.borderTopWidth = 1; back.style.borderBottomWidth = 1; back.style.borderLeftWidth = 1; back.style.borderRightWidth = 1;
            back.style.display = DisplayStyle.None;
            root.Add(back);

            BuildKnob(root);
            BuildReadingControls(root);
        }

        // ── the chalkboard knob: drag it towards the middle (or tap it) to turn the room to the board ──
        VisualElement knob, knobRing;
        Label knobIcon, knobLabel;
        bool knobDown;
        Vector2 knobStart;
        float knobTravel;

        /// <summary>Pressed (panel position).</summary>
        public event System.Action<Vector2> KnobPressed;
        /// <summary>Dragged: total offset from where it was pressed (panel units).</summary>
        public event System.Action<Vector2> KnobDragged;
        /// <summary>Released: true when it was a tap (hardly moved).</summary>
        public event System.Action<bool> KnobReleased;

        /// <summary>True while the knob has the pointer (world taps and walking ignore it).</summary>
        /// <summary>Is a HUD control (Back, the knob, the reading buttons) under this screen point (bottom-left origin)?</summary>
        public bool IsOverUI(Vector2 screen)
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            var panel = root?.panel;
            if (panel == null) return false;
            var picked = panel.Pick(RuntimePanelUtils.ScreenToPanel(panel, new Vector2(screen.x, Screen.height - screen.y)));
            return picked != null && picked != root;
        }

        public bool KnobBusy => knobDown || Time.frameCount - knobReleasedFrame <= 1;
        int knobReleasedFrame = -10;

        void BuildKnob(VisualElement root)
        {
            knob = new VisualElement { name = "board-knob", pickingMode = PickingMode.Ignore };   // only the ring takes the pointer
            knob.style.position = Position.Absolute;
            knob.style.width = 132; knob.style.height = 112;
            knob.style.alignItems = Align.Center;
            knob.style.translate = new Translate(Length.Percent(-50), Length.Percent(-50));
            knob.style.opacity = 0f;
            AuthCardLayout.TransitionMany(knob, 0.35f, "opacity", "scale");
            knob.style.display = DisplayStyle.None;

            knobRing = new VisualElement { name = "board-knob-ring" };
            knobRing.style.width = 74; knobRing.style.height = 74;
            knobRing.style.alignItems = Align.Center; knobRing.style.justifyContent = Justify.Center;
            knobRing.style.backgroundColor = Slate;
            knobRing.style.borderTopLeftRadius = 37; knobRing.style.borderTopRightRadius = 37;
            knobRing.style.borderBottomLeftRadius = 37; knobRing.style.borderBottomRightRadius = 37;
            Color gold = new Color32(242, 193, 78, 255);
            knobRing.style.borderTopColor = gold; knobRing.style.borderBottomColor = gold;
            knobRing.style.borderLeftColor = gold; knobRing.style.borderRightColor = gold;
            knobRing.style.borderTopWidth = 2; knobRing.style.borderBottomWidth = 2; knobRing.style.borderLeftWidth = 2; knobRing.style.borderRightWidth = 2;
            AuthCardLayout.TransitionMany(knobRing, 0.18f, "scale", "background-color");
            knob.Add(knobRing);

            knobIcon = new Label("▣") { pickingMode = PickingMode.Ignore };
            knobIcon.style.fontSize = 30; knobIcon.style.color = Parchment;
            knobIcon.style.unityTextAlign = TextAnchor.MiddleCenter;
            knobRing.Add(knobIcon);

            knobLabel = new Label("Drag to the board") { pickingMode = PickingMode.Ignore };
            knobLabel.style.marginTop = 6;
            knobLabel.style.fontSize = 13; knobLabel.style.color = Parchment;
            knobLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            knobLabel.style.backgroundColor = Slate;
            knobLabel.style.paddingLeft = 10; knobLabel.style.paddingRight = 10; knobLabel.style.paddingTop = 3; knobLabel.style.paddingBottom = 3;
            knobLabel.style.borderTopLeftRadius = 10; knobLabel.style.borderTopRightRadius = 10;
            knobLabel.style.borderBottomLeftRadius = 10; knobLabel.style.borderBottomRightRadius = 10;
            knobLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            knob.Add(knobLabel);
            root.Add(knob);

            knobRing.RegisterCallback<PointerDownEvent>(e =>
            {
                knobDown = true; knobStart = e.position; knobTravel = 0f;
                knobRing.CapturePointer(e.pointerId);
                knobRing.style.scale = new Scale(new Vector3(0.92f, 0.92f, 1f));
                KnobPressed?.Invoke(e.position);
                e.StopPropagation();
            });
            knobRing.RegisterCallback<PointerMoveEvent>(e =>
            {
                if (!knobDown || !knobRing.HasPointerCapture(e.pointerId)) return;
                var d = (Vector2)e.position - knobStart;
                knobTravel = Mathf.Max(knobTravel, d.magnitude);
                KnobDragged?.Invoke(d);
            });
            System.Action<int> release = id =>
            {
                if (!knobDown) return;
                knobDown = false;
                knobReleasedFrame = Time.frameCount;
                if (knobRing.HasPointerCapture(id)) knobRing.ReleasePointer(id);
                knobRing.style.scale = new Scale(Vector3.one);
                KnobReleased?.Invoke(knobTravel < 12f);
            };
            knobRing.RegisterCallback<PointerUpEvent>(e => { release(e.pointerId); e.StopPropagation(); });
            knobRing.RegisterCallback<PointerCancelEvent>(e => release(e.pointerId));
        }

        /// <summary>
        /// Place the knob beside the chalkboard (screen position, bottom-left origin) or hide it (null).
        /// <paramref name="progress"/> 0 = in the room … 1 = facing the board (label and glow follow it).
        /// </summary>
        public void SetBoardKnob(Vector2? screenPosition, float progress)
        {
            if (knob == null) return;
            if (screenPosition == null)
            {
                knob.style.opacity = 0f;
                if (!knobDown) knob.style.display = DisplayStyle.None;
                return;
            }
            var root = knob.parent;
            if (root?.panel == null) return;
            var p = screenPosition.Value;
            var panelPos = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(p.x, Screen.height - p.y));
            var size = root.layout.size;
            if (size.x > 1f)
            {
                panelPos.x = Mathf.Clamp(panelPos.x, 100f, size.x - 100f);   // room for the label under the ring
                panelPos.y = Mathf.Clamp(panelPos.y, 80f, size.y - 90f);
            }
            knob.style.left = panelPos.x; knob.style.top = panelPos.y;
            knob.style.display = DisplayStyle.Flex;
            knob.style.opacity = 1f;
            knobIcon.text = progress > 0.5f ? "⌂" : "▣";
            knobLabel.text = progress > 0.5f ? "Drag back to the room" : "Drag to the board";
            var glow = Color.Lerp(Slate, new Color(0.95f, 0.76f, 0.31f, 0.9f), Mathf.Clamp01(progress));
            if (!knobDown) knobRing.style.backgroundColor = glow;
        }

        // ── reading controls (at the dashboard) and the overlay the word activities / reader open in ──
        VisualElement controls;
        Button readButton, finishButton, prevPage, nextPage;

        /// <summary>Full-screen layer the attempt's activities and the passage reader open in.</summary>
        public VisualElement OverlayHost { get; private set; }

        public event System.Action ReadPressed, FinishPressed, PreviousPagePressed, NextPagePressed;

        void BuildReadingControls(VisualElement root)
        {
            controls = new VisualElement { pickingMode = PickingMode.Ignore };
            controls.style.position = Position.Absolute;
            controls.style.right = Length.Percent(2); controls.style.top = Length.Percent(4);
            controls.style.flexDirection = FlexDirection.Row;
            controls.style.display = DisplayStyle.None;
            root.Add(controls);
            Button Pill(string text, System.Action click, Color tint)
            {
                var b = new Button(click) { text = text };
                b.style.backgroundColor = tint; b.style.color = Parchment;
                b.style.fontSize = 16; b.style.unityFontStyleAndWeight = FontStyle.Bold;
                b.style.paddingLeft = 16; b.style.paddingRight = 16; b.style.paddingTop = 9; b.style.paddingBottom = 9;
                b.style.marginLeft = 8;
                b.style.borderTopLeftRadius = 18; b.style.borderTopRightRadius = 18;
                b.style.borderBottomLeftRadius = 18; b.style.borderBottomRightRadius = 18;
                b.style.borderTopColor = Hairline; b.style.borderBottomColor = Hairline; b.style.borderLeftColor = Hairline; b.style.borderRightColor = Hairline;
                b.style.borderTopWidth = 1; b.style.borderBottomWidth = 1; b.style.borderLeftWidth = 1; b.style.borderRightWidth = 1;
                controls.Add(b);
                return b;
            }
            prevPage = Pill("‹", () => PreviousPagePressed?.Invoke(), Slate);
            nextPage = Pill("›", () => NextPagePressed?.Invoke(), Slate);
            readButton = Pill("Read the passage", () => ReadPressed?.Invoke(), Slate);
            finishButton = Pill("Finish & submit", () => FinishPressed?.Invoke(), new Color(0.18f, 0.55f, 0.32f, 0.92f));

            OverlayHost = new VisualElement { name = "attempt-overlay", pickingMode = PickingMode.Ignore };
            OverlayHost.style.position = Position.Absolute;
            OverlayHost.style.left = 0; OverlayHost.style.right = 0; OverlayHost.style.top = 0; OverlayHost.style.bottom = 0;
            root.Add(OverlayHost);
        }

        /// <summary>Show the reading controls at the dashboard (all false hides them).</summary>
        public void SetReadingControls(bool read, bool finish, bool pages, bool canPrev, bool canNext)
        {
            if (controls == null) return;
            controls.style.display = read || finish || pages ? DisplayStyle.Flex : DisplayStyle.None;
            readButton.style.display = read ? DisplayStyle.Flex : DisplayStyle.None;
            finishButton.style.display = finish ? DisplayStyle.Flex : DisplayStyle.None;
            prevPage.style.display = pages ? DisplayStyle.Flex : DisplayStyle.None;
            nextPage.style.display = pages ? DisplayStyle.Flex : DisplayStyle.None;
            prevPage.SetEnabled(canPrev); nextPage.SetEnabled(canNext);
        }

        Button back;
        System.Action onBack;

        /// <summary>A "Back to the room" button (top left) while the chalkboard is in use; null hides it.</summary>
        public void SetBackButton(System.Action action)
        {
            onBack = action;
            if (back != null) back.style.display = action != null ? DisplayStyle.Flex : DisplayStyle.None;
        }

        /// <summary>Show a message (null/empty hides it). Changes cross-fade.</summary>
        public void SetHint(string text)
        {
            text ??= "";
            if (hint == null || text == shown) return;
            shown = text;
            pendingSwap?.Pause();
            if (string.IsNullOrEmpty(text)) { hint.style.opacity = 0f; return; }
            if (hint.resolvedStyle.opacity < 0.05f)
            {
                hint.text = text;
                hint.style.opacity = 1f;
                return;
            }
            hint.style.opacity = 0f;
            pendingSwap = hint.schedule.Execute(() => { hint.text = text; hint.style.opacity = 1f; }).StartingIn(260);
        }

        public void SetVeil(bool on)
        {
            if (veil != null) veil.style.opacity = on ? 1f : 0f;
        }

        /// <summary>Dark on the left, fading to clear at the right edge (eased).</summary>
        static Texture2D LeftFade()
        {
            const int w = 128;
            var t = new Texture2D(w, 1, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
            for (int x = 0; x < w; x++)
            {
                float k = 1f - x / (w - 1f);
                t.SetPixel(x, 0, new Color(18f / 255f, 20f / 255f, 25f / 255f, 0.55f * k * k * (3f - 2f * k)));
            }
            t.Apply();
            return t;
        }
    }
}
