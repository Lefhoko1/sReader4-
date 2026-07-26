using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.Game.Trek
{
    /// <summary>
    /// The Trek HUD overlay: Read button + word-gate counter + timer + points on
    /// translucent glass chips (top), the story ribbon docked at the bottom, and
    /// transient toasts. Renders state only — every value is pushed by the
    /// presenter; no rule lives here.
    /// </summary>
    internal sealed class TrekHud
    {
        Label gateChip, timerChip, pointsChip;
        VisualElement bottomDock;
        VisualElement root;

        public RibbonBinder Ribbon { get; } = new RibbonBinder();

        public VisualElement Build(TrailData trail, Action onRead, Action onExit)
        {
            root = new VisualElement();
            root.style.position = Position.Absolute;
            root.style.left = 0; root.style.right = 0; root.style.top = 0; root.style.bottom = 0;
            root.pickingMode = PickingMode.Ignore;

            // ── top bar ──
            var top = new VisualElement();
            top.style.flexDirection = FlexDirection.Row;
            top.style.alignItems = Align.Center;
            top.style.marginTop = 10;
            top.style.marginLeft = 10;
            top.style.marginRight = 10;
            root.Add(top);

            var exit = GlassButton("‹", onExit);
            top.Add(exit);

            var read = GlassButton("📖 Read", onRead);   // R-2: opens Reading View
            top.Add(read);

            var spacer = new VisualElement();
            spacer.style.flexGrow = 1;
            spacer.pickingMode = PickingMode.Ignore;
            top.Add(spacer);

            gateChip = TrekTheme.GlassChip("");
            top.Add(gateChip);
            timerChip = TrekTheme.GlassChip("");
            top.Add(timerChip);
            pointsChip = TrekTheme.GlassChip("0");
            top.Add(pointsChip);

            // ── bottom dock: title + ribbon ──
            bottomDock = new VisualElement();
            bottomDock.style.position = Position.Absolute;
            bottomDock.style.left = 0; bottomDock.style.right = 0; bottomDock.style.bottom = 0;
            root.Add(bottomDock);
            bottomDock.Add(Ribbon.Build(trail, onRead));   // tapping the ribbon also opens Reading View (R-2)

            return root;
        }

        public void SetGateCounter(int solved, int total)
            => gateChip.text = total > 0 ? $"WORD GATE {Mathf.Min(solved + 1, total)}/{total}" : "READING";

        public void SetTimer(float secondsLeft, bool paused)
        {
            if (secondsLeft <= 0f) { timerChip.style.display = DisplayStyle.None; return; }
            timerChip.style.display = DisplayStyle.Flex;
            timerChip.text = paused ? "⏸" : Mathf.CeilToInt(secondsLeft) + "s";
            timerChip.style.color = secondsLeft <= 8f && !paused ? TrekTheme.Coral : TrekTheme.Paper;
        }

        public void HideTimer() => timerChip.style.display = DisplayStyle.None;

        public void SetPoints(int points) => pointsChip.text = "★ " + points;

        /// <summary>A transient centred toast on the glass (auto-removes ~2 s).</summary>
        public void Toast(string message)
        {
            var t = new Label(message);
            t.style.position = Position.Absolute;
            t.style.top = Length.Percent(30);
            t.style.left = 0; t.style.right = 0;
            t.style.unityTextAlign = TextAnchor.MiddleCenter;
            t.style.fontSize = 15;
            t.style.color = TrekTheme.Paper;
            t.style.backgroundColor = TrekTheme.Glass;
            t.style.paddingTop = t.style.paddingBottom = 10;
            t.style.marginLeft = t.style.marginRight = 40;
            TrekTheme.Round(t, 12);
            t.pickingMode = PickingMode.Ignore;
            root.Add(t);
            t.schedule.Execute(() => t.RemoveFromHierarchy()).StartingIn(2000);
        }

        /// <summary>Anchor container for slide-up panels (gate / camp / complete).</summary>
        public VisualElement Root => root;

        static Button GlassButton(string text, Action onClick)
        {
            var b = new Button(onClick) { text = text };
            b.style.height = 34;
            b.style.paddingLeft = b.style.paddingRight = 14;
            b.style.marginRight = 8;
            b.style.backgroundColor = TrekTheme.Glass;
            b.style.color = TrekTheme.Paper;
            b.style.fontSize = 13;
            b.style.unityFontStyleAndWeight = FontStyle.Bold;
            TrekTheme.Round(b, 12);
            TrekTheme.Border(b, Color.clear, 0);
            return b;
        }
    }
}
