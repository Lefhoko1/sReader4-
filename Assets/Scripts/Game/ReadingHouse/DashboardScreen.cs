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
    /// Classroom uses, recoloured from their light mobile-card look into the dashboard's game palette:
    /// no panel behind the words (the display's illustration shows through), gold titles and cream words
    /// with a navy outline, pieces that look like word stones, and orange / violet / teal action buttons
    /// that squash and bounce with a soft pop when pressed. A tall activity slides as one piece under the
    /// finger (<see cref="SlideDrag"/>). Taps on the display reach the page (see <see cref="WorldUIScreen"/>).
    /// Presentation only.
    /// </summary>
    public sealed class DashboardScreen : WorldUIScreen
    {
        protected override bool MarkAsBoard => false;

        [Tooltip("Extra height (m) the screen grows upwards beyond the display's picture, so the passage has room.")]
        public float extraHeightTop = 0.3f;
        [Tooltip("Extra height (m) the screen grows downwards.")]
        public float extraHeightBottom = 0.08f;

        protected override void Measure(out Vector3 normal, out Vector2 size, out Vector3 centre)
        {
            base.Measure(out normal, out size, out centre);
            size.y += extraHeightTop + extraHeightBottom;
            centre += Vector3.up * ((extraHeightTop - extraHeightBottom) * 0.5f);
        }

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
            Touchable(b);
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

        /// <summary>
        /// Dress an activity for the display: its sentence above it, one compact heading, big buttons — and the
        /// whole sheet (actions included) slides as one piece when it is taller than the display.
        /// </summary>
        public void PresentActivity(VisualElement sheet, string sentence)
        {
            sheet.AddToClassList("rh-activity");
            sheet.style.minHeight = 0;
            sheet.style.flexShrink = 0;
            sheet.style.maxHeight = StyleKeyword.None;
            sheet.style.paddingTop = sheet.style.paddingBottom = 12;
            sheet.style.paddingLeft = sheet.style.paddingRight = 18;

            var children = new System.Collections.Generic.List<VisualElement>(sheet.Children());
            var footer = sheet.Q<VisualElement>("gate-actions");
            foreach (var child in children) child.style.flexShrink = 0;
            int insertAt = 0;
            if (!string.IsNullOrWhiteSpace(sentence))
            {
                var context = new Label(sentence) { name = "rh-activity-context" };
                context.style.whiteSpace = WhiteSpace.Normal;
                context.style.fontSize = 17;
                context.style.color = Cream;
                context.style.marginBottom = 8;
                context.style.flexShrink = 0;
                sheet.Insert(0, context);
                insertAt = 1;
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
                children[1].style.fontSize = 22;
                children[2].style.fontSize = 15;
                children[2].style.marginLeft = 18;
                heading.Add(children[1]); heading.Add(children[2]);
                sheet.Insert(insertAt, heading);
            }
            sheet.Query<Label>().ForEach(l =>
            {
                l.style.marginTop = 0; l.style.marginBottom = 4;
                l.style.paddingTop = l.style.paddingBottom = 0;
                l.style.paddingLeft = l.style.paddingRight = 0;
            });
            if (footer != null)
            {
                footer.BringToFront();                                           // actions close the sheet
                footer.style.flexShrink = 0;
                footer.style.marginTop = 10;
            }
            sheet.Query<Button>().ForEach(b =>
            {
                b.style.minHeight = 52;
                b.style.minWidth = 52;
                b.style.fontSize = 20;
                b.style.whiteSpace = WhiteSpace.Normal;
                b.style.flexShrink = 0;
            });
            var frame = sheet.parent;
            if (frame != null)
            {
                frame.style.overflow = Overflow.Hidden;
                sheet.AddManipulator(SlideDrag.ForSheet(sheet, frame));
            }
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
            nextStyle = Time.unscaledTime + 0.25f;
            foreach (var overlay in Host.Children()) { Restyle(overlay, top: true); RefreshActivityLayout(overlay); }
        }

        /// <summary>The light Trek card look → the dashboard's game palette (only TrekTheme's own colours are changed).</summary>
        static void Restyle(VisualElement e, bool top = false)
        {
            if (top)
            {
                e.style.backgroundColor = Color.clear;                        // the display's picture shows through
                e.style.justifyContent = Justify.Center;
                e.style.alignItems = Align.Center;
            }
            if (e is ScrollView sv && !sv.ClassListContains("rh-drag"))
            {
                sv.AddToClassList("rh-drag");
                sv.AddManipulator(SlideDrag.ForScrollView(sv));
                sv.verticalScrollerVisibility = ScrollerVisibility.Hidden;
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
                    else Fill(b, new Color(NavyDeep.r, NavyDeep.g, NavyDeep.b, 0.55f), Cream, new Color(Gold.r, Gold.g, Gold.b, 0.6f));
                }
                Touchable(b);
            }
            else
            {
                if (Same(bg, TrekTheme.Card))
                {
                    // No card: the words stand straight on the display's picture.
                    e.style.backgroundColor = Color.clear;
                    Border(e, Color.clear, 0f);
                    e.style.width = Length.Percent(94);
                    if (!e.ClassListContains("rh-activity")) e.style.maxHeight = Length.Percent(94);
                    e.style.marginLeft = 0; e.style.marginRight = 0; e.style.marginBottom = 0;
                }
                else if (Same(bg, TrekTheme.Paper) || Same(bg, TrekTheme.Line)) e.style.backgroundColor = Color.clear;
                if (e is Label l)
                {
                    l.style.unityTextOutlineColor = NavyDeep;
                    l.style.unityTextOutlineWidth = 0.18f;
                    l.style.textShadow = new TextShadow { offset = new Vector2(0f, 2f), blurRadius = 3f, color = new Color(NavyDeep.r, NavyDeep.g, NavyDeep.b, 0.85f) };
                    var c = l.style.color.value;
                    if (Same(c, TrekTheme.Ink) && l.style.fontSize.value.value >= 18f) l.style.color = GoldText;
                    else if (Same(c, TrekTheme.Ink) || l.style.color.keyword == StyleKeyword.Null) l.style.color = Cream;   // unset = inherited dark ink
                    else if (Same(c, TrekTheme.Muted)) l.style.color = new Color(Cream.r, Cream.g, Cream.b, 0.72f);
                }
            }
            foreach (var child in e.Children()) Restyle(child);
        }

        /// <summary>A press squashes the button a little and lets it spring back with a soft pop.</summary>
        static void Touchable(Button b)
        {
            if (b.ClassListContains("rh-touch")) return;
            b.AddToClassList("rh-touch");
            b.style.transitionProperty = new System.Collections.Generic.List<StylePropertyName> { new StylePropertyName("scale") };
            b.style.transitionDuration = new System.Collections.Generic.List<TimeValue> { new TimeValue(0.18f) };
            b.style.transitionTimingFunction = new System.Collections.Generic.List<EasingFunction> { new EasingFunction(EasingMode.EaseOutBack) };
            b.RegisterCallback<PointerDownEvent>(_ => { b.style.scale = new Scale(new Vector2(0.9f, 0.9f)); Pop(); }, TrickleDown.TrickleDown);
            b.RegisterCallback<PointerUpEvent>(_ => b.style.scale = new Scale(Vector2.one), TrickleDown.TrickleDown);
            b.RegisterCallback<PointerLeaveEvent>(_ => b.style.scale = new Scale(Vector2.one));
            b.RegisterCallback<PointerCaptureOutEvent>(_ => b.style.scale = new Scale(Vector2.one));
        }

        static AudioSource popSource;
        static AudioClip popClip;

        /// <summary>A short soft "pop" (synthesised once, like the stones' chime).</summary>
        static void Pop()
        {
            if (popClip == null)
            {
                const int rate = 22050; int n = rate / 12;
                var data = new float[n];
                for (int i = 0; i < n; i++)
                {
                    float t = i / (float)rate;
                    float f = Mathf.Lerp(900f, 420f, t * 12f);
                    data[i] = Mathf.Exp(-t * 45f) * 0.45f * Mathf.Sin(2f * Mathf.PI * f * t);
                }
                popClip = AudioClip.Create("DashboardPop", n, 1, rate, false);
                popClip.SetData(data, 0);
            }
            if (popSource == null)
            {
                var go = new GameObject("Dashboard Pop") { hideFlags = HideFlags.DontSave };
                popSource = go.AddComponent<AudioSource>();
                popSource.playOnAwake = false; popSource.spatialBlend = 0f; popSource.volume = 0.5f;
            }
            popSource.PlayOneShot(popClip);
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
