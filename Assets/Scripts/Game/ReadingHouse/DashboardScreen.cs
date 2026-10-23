using System;
using UnityEngine;
using UnityEngine.UIElements;
using SReader.Game.Trek;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The dashboard's big display as a game screen. While the student reads at the dashboard it carries a
    /// small toolbar (Read · Finish · ‹ ›) over the picture; when a word activity or the passage reader opens,
    /// it fills the display — the same activities (GatePanelHost) and reader (ReadingViewController) the
    /// Classroom uses, recoloured from their light mobile-card look into the dashboard's game palette: deep
    /// navy glass, a gold rim, gold titles, cream words, pieces that look like word stones, and orange /
    /// violet / teal action buttons. Taps on the display reach the page (see <see cref="WorldUIScreen"/>).
    /// Presentation only.
    /// </summary>
    public sealed class DashboardScreen : WorldUIScreen
    {
        protected override bool MarkAsBoard => false;

        /// <summary>Where the activities and the reader open (above the toolbar).</summary>
        public VisualElement Host { get; private set; }

        public event Action ReadPressed, FinishPressed, PreviousPressed, NextPressed;

        static readonly Color Navy = new Color32(16, 30, 72, 255);
        static readonly Color NavyDeep = new Color32(9, 18, 46, 255);
        static readonly Color Gold = new Color32(255, 196, 64, 255);
        static readonly Color GoldText = new Color32(255, 214, 102, 255);
        static readonly Color Cream = new Color32(255, 244, 214, 255);
        static readonly Color Orange = new Color32(255, 138, 31, 255);
        static readonly Color Violet = new Color32(123, 92, 255, 255);
        static readonly Color Teal = new Color32(37, 166, 154, 255);
        static readonly Color Stone = new Color32(243, 226, 184, 255);
        static readonly Color StoneEdge = new Color32(201, 167, 94, 255);

        VisualElement toolbar;
        Button read, finish, prev, next;
        float nextStyle;

        public override void Open()
        {
            base.Open();
            var root = page.rootVisualElement;
            root.Clear();
            root.pickingMode = PickingMode.Ignore;
            root.style.position = Position.Absolute;
            root.style.left = 0; root.style.right = 0; root.style.top = 0; root.style.bottom = 0;

            Host = new VisualElement { name = "dashboard-host", pickingMode = PickingMode.Ignore };
            Host.style.position = Position.Absolute;
            Host.style.left = 0; Host.style.right = 0; Host.style.top = 0; Host.style.bottom = 0;
            root.Add(Host);

            toolbar = new VisualElement { pickingMode = PickingMode.Ignore };
            toolbar.style.position = Position.Absolute;
            toolbar.style.right = 18; toolbar.style.bottom = 14;
            toolbar.style.flexDirection = FlexDirection.Row;
            root.Add(toolbar);
            prev = Pill("‹", () => PreviousPressed?.Invoke(), Navy);
            next = Pill("›", () => NextPressed?.Invoke(), Navy);
            read = Pill("Read the passage", () => ReadPressed?.Invoke(), Navy);
            finish = Pill("Finish & submit", () => FinishPressed?.Invoke(), new Color32(34, 150, 86, 255));
            SetToolbar(false, false, false, false, false);
        }

        public override void Close()
        {
            Host = null;
            base.Close();
        }

        Button Pill(string text, Action click, Color fill)
        {
            var b = new Button(click) { text = text };
            b.style.backgroundColor = fill; b.style.color = Cream;
            b.style.fontSize = 22; b.style.unityFontStyleAndWeight = FontStyle.Bold;
            b.style.paddingLeft = 20; b.style.paddingRight = 20; b.style.paddingTop = 8; b.style.paddingBottom = 8;
            b.style.marginLeft = 10;
            Round(b, 22f); Border(b, Gold, 3f);
            toolbar.Add(b);
            return b;
        }

        /// <summary>Toolbar over the picture while reading (hidden when an activity or the reader fills the display).</summary>
        public void SetToolbar(bool showRead, bool showFinish, bool pages, bool canPrev, bool canNext)
        {
            if (toolbar == null) return;
            bool busy = Host != null && Host.childCount > 0;
            toolbar.style.display = !busy && (showRead || showFinish || pages) ? DisplayStyle.Flex : DisplayStyle.None;
            read.style.display = showRead ? DisplayStyle.Flex : DisplayStyle.None;
            finish.style.display = showFinish ? DisplayStyle.Flex : DisplayStyle.None;
            prev.style.display = pages ? DisplayStyle.Flex : DisplayStyle.None;
            next.style.display = pages ? DisplayStyle.Flex : DisplayStyle.None;
            prev.SetEnabled(canPrev); next.SetEnabled(canNext);
        }

        /// <summary>Keep the action row fixed; only the challenge body scrolls.</summary>
        public void PresentActivity(VisualElement sheet, string sentence)
        {
            sheet.AddToClassList("rh-activity");
            sheet.style.height = Length.Percent(94);
            sheet.style.minHeight = 0;
            sheet.style.flexShrink = 0;
            sheet.style.paddingTop = sheet.style.paddingBottom = 12;
            sheet.style.paddingLeft = sheet.style.paddingRight = 18;

            var footer = sheet.Q<VisualElement>("gate-actions");
            footer?.RemoveFromHierarchy();
            var body = new ScrollView(ScrollViewMode.Vertical) { name = "rh-activity-body" };
            body.style.flexGrow = 1;
            body.style.flexShrink = 1;
            body.style.minHeight = 0;
            body.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            body.verticalScrollerVisibility = ScrollerVisibility.Auto;
            var children = new System.Collections.Generic.List<VisualElement>(sheet.Children());
            foreach (var child in children) { child.style.flexShrink = 0; body.Add(child); }
            if (!string.IsNullOrWhiteSpace(sentence))
            {
                var context = new Label(sentence) { name = "rh-activity-context" };
                context.style.whiteSpace = WhiteSpace.Normal;
                context.style.fontSize = 15;
                context.style.color = Cream;
                context.style.marginBottom = 8;
                context.style.flexShrink = 0;
                body.Insert(0, context);
            }
            // A single compact heading replaces three stacked header rows.
            if (children.Count >= 3 && footer != null)
            {
                children[0].RemoveFromHierarchy();
                var heading = new VisualElement { name = "rh-challenge-heading" };
                heading.style.flexDirection = FlexDirection.Row;
                heading.style.alignItems = Align.Center;
                heading.style.flexWrap = Wrap.Wrap;
                heading.style.flexShrink = 0;
                heading.style.marginBottom = 6;
                children[1].style.fontSize = 20;
                children[2].style.fontSize = 14;
                children[2].style.marginLeft = 18;
                heading.Add(children[1]); heading.Add(children[2]);
                body.Insert(string.IsNullOrWhiteSpace(sentence) ? 0 : 1, heading);
            }
            body.Query<Label>().ForEach(l =>
            {
                l.style.marginTop = 0; l.style.marginBottom = 4;
                l.style.paddingTop = l.style.paddingBottom = 0;
                l.style.paddingLeft = l.style.paddingRight = 0;
            });
            sheet.Add(body);
            if (footer != null)
            {
                footer.style.flexShrink = 0;
                footer.style.marginTop = 8;
                footer.style.paddingTop = 6;
                footer.style.borderTopWidth = 1;
                footer.style.borderTopColor = new Color(Gold.r, Gold.g, Gold.b, 0.35f);
                sheet.Add(footer);
            }
            sheet.Query<Button>().ForEach(b =>
            {
                b.style.minHeight = 48;
                b.style.minWidth = 48;
                b.style.fontSize = 19;
                b.style.whiteSpace = WhiteSpace.Normal;
                b.style.flexShrink = 0;
            });
            var bar = body.verticalScroller;
            bar.style.width = 8;
            bar.lowButton.style.display = bar.highButton.style.display = DisplayStyle.None;
            var track = bar.slider.Q("unity-tracker");
            var thumb = bar.slider.Q("unity-dragger");
            if (track != null) { track.style.backgroundColor = NavyDeep; Round(track, 4); }
            if (thumb != null) { thumb.style.backgroundColor = StoneEdge; Round(thumb, 4); }
        }

        static void RefreshActivityLayout(VisualElement overlay)
        {
            var sheet = overlay.Q(className: "rh-activity");
            if (sheet == null) return;
            // The preview and the answer chips used to repeat the same letters,
            // consuming another row. Keep the tappable answer once it has pieces.
            var preview = sheet.Q<Label>("gate-preview");
            var answer = sheet.Q("gate-answer");
            if (preview != null && answer != null)
            {
                bool hasAnswer = answer.childCount > 0;
                preview.style.display = hasAnswer ? DisplayStyle.None : DisplayStyle.Flex;
                preview.style.fontSize = 24;
                answer.style.marginTop = hasAnswer ? 6 : 0;
            }
        }

        void LateUpdate()
        {
            // The activities rebuild their pieces as the student plays: keep recolouring what appears.
            if (Host == null || Host.childCount == 0 || Time.unscaledTime < nextStyle) return;
            nextStyle = Time.unscaledTime + 0.1f;
            foreach (var overlay in Host.Children()) { Restyle(overlay, top: true); RefreshActivityLayout(overlay); }
        }

        /// <summary>The light Trek card look → the dashboard's game palette (only TrekTheme's own colours are changed).</summary>
        static void Restyle(VisualElement e, bool top = false)
        {
            if (top)
            {
                e.style.backgroundColor = new Color(NavyDeep.r, NavyDeep.g, NavyDeep.b, 0.82f);
                e.style.justifyContent = Justify.Center;
                e.style.alignItems = Align.Center;
            }
            var bg = e.style.backgroundColor.value;
            if (e is Button b)
            {
                string t = b.text ?? "";
                if (Same(bg, TrekTheme.Sun)) Fill(b, Orange, Color.white, Gold);
                else if (Same(bg, TrekTheme.Paper)) Fill(b, Stone, Navy, StoneEdge);
                else if (Same(bg, TrekTheme.Line)) Fill(b, new Color(1f, 1f, 1f, 0.08f), new Color(Cream.r, Cream.g, Cream.b, 0.5f), new Color(1f, 1f, 1f, 0.15f));
                else if (bg.a < 0.01f && !b.ClassListContains("rh-styled"))
                {
                    if (t.StartsWith("Hint")) Fill(b, Violet, Color.white, new Color32(190, 175, 255, 255));
                    else if (t.Contains("Later")) Fill(b, Teal, Color.white, new Color32(150, 230, 220, 255));
                    else Fill(b, new Color(1f, 1f, 1f, 0.1f), Cream, new Color(1f, 1f, 1f, 0.25f));
                }
            }
            else
            {
                if (Same(bg, TrekTheme.Card))
                {
                    e.style.backgroundColor = new Color(Navy.r, Navy.g, Navy.b, 0.97f);
                    Border(e, Gold, 3f); Round(e, 24f);
                    e.style.width = Length.Percent(94); e.style.maxHeight = Length.Percent(94);
                    e.style.marginLeft = 0; e.style.marginRight = 0; e.style.marginBottom = 0;
                }
                else if (Same(bg, TrekTheme.Paper) || Same(bg, TrekTheme.Line)) e.style.backgroundColor = new Color(1f, 1f, 1f, 0.07f);
                if (e is Label l)
                {
                    var c = l.style.color.value;
                    if (Same(c, TrekTheme.Ink) && l.style.fontSize.value.value >= 18f) l.style.color = GoldText;
                    else if (Same(c, TrekTheme.Ink) || l.style.color.keyword == StyleKeyword.Null) l.style.color = Cream;   // unset = inherited dark ink
                    else if (Same(c, TrekTheme.Muted)) l.style.color = new Color(Cream.r, Cream.g, Cream.b, 0.72f);
                }
            }
            foreach (var child in e.Children()) Restyle(child);
        }

        static void Fill(Button b, Color fill, Color text, Color rim)
        {
            b.style.backgroundColor = fill; b.style.color = text;
            b.style.unityFontStyleAndWeight = FontStyle.Bold;
            Round(b, 16f); Border(b, rim, 2f);
            b.AddToClassList("rh-styled");
        }

        static bool Same(Color a, Color b) => Mathf.Abs(a.r - b.r) < 0.004f && Mathf.Abs(a.g - b.g) < 0.004f && Mathf.Abs(a.b - b.b) < 0.004f && Mathf.Abs(a.a - b.a) < 0.004f;

        static void Round(VisualElement e, float r)
        {
            e.style.borderTopLeftRadius = r; e.style.borderTopRightRadius = r;
            e.style.borderBottomLeftRadius = r; e.style.borderBottomRightRadius = r;
        }

        static void Border(VisualElement e, Color c, float w)
        {
            e.style.borderTopColor = c; e.style.borderBottomColor = c; e.style.borderLeftColor = c; e.style.borderRightColor = c;
            e.style.borderTopWidth = w; e.style.borderBottomWidth = w; e.style.borderLeftWidth = w; e.style.borderRightWidth = w;
        }
    }
}
