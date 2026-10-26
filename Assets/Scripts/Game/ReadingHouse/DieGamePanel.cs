using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The game screen on the dashboard's display, two boards that wipe from one to the other:
    ///   • the PASSAGE: the whole assignment, its key words marked as in the app's reading game (a fill-in word
    ///     shows only some of its letters, boxed; a word to define is gold and underlined; a word to illustrate is
    ///     pink and underlined with a little picture mark; restored words turn green; the next key word glows).
    ///     Key words are buttons: a tap opens the play board for that word.
    ///   • the PLAY board for one key word: its sentence (the key word marked), the answer slots that fill in as
    ///     the student picks on the die, the pieces still to place (shuffled) or the pictures (numbered), a clue,
    ///     and Hint / Guide me / Later / Back to the passage. A right pick flies from the pieces into its slot.
    /// One dark panel with a single gold rim over the display's picture, so every word reads clearly.
    /// Built in code into the dashboard screen's page; presentation only.
    /// </summary>
    public sealed class DieGamePanel
    {
        public event Action<int> KeyWordPressed;                    // the key word's index (the presenter's id)
        public event Action HintPressed, GuidePressed, LaterPressed, BackPressed, FinishPressed;
        /// <summary>Leave this assignment for the chalkboard's list / start it again (after the student confirms).</summary>
        public event Action AssignmentsPressed, RetryConfirmed;

        /// <summary>How a word of the passage is drawn.</summary>
        public enum Mark { Plain, Define, Illustrate, Fill, Restored }

        /// <summary>One word of the passage. <see cref="key"/> ≥ 0 makes it a button.</summary>
        public struct Word { public string text; public Mark mark; public int key; public bool next; }

        /// <summary>One slot / piece of an activity line.</summary>
        public enum Piece { Word, Key, KeyBlank, Later, Slot }

        static readonly Color Back = new Color32(10, 18, 44, 240);
        static readonly Color Gold = new Color32(255, 196, 64, 255);
        static readonly Color GoldText = new Color32(255, 214, 102, 255);
        static readonly Color Cream = new Color32(255, 244, 214, 255);
        static readonly Color Mint = new Color32(140, 228, 196, 255);
        static readonly Color Pink = new Color32(255, 150, 190, 255);
        static readonly Color Green = new Color32(120, 230, 140, 255);

        readonly VisualElement root, passageView, playView, passageWords;
        readonly ScrollView passageScroll;
        readonly Label passageTitle, passageStatus, playBadge, playTitle, playSentence, clue, streak, timeLeft;
        readonly VisualElement slots, pool, pictures;
        readonly Button hint, guide, later, back, finish;
        readonly VisualElement confirm;
        readonly List<Label> poolChips = new List<Label>();
        readonly List<Label> slotChips = new List<Label>();
        readonly Dictionary<int, VisualElement> keyButtons = new Dictionary<int, VisualElement>();
        int shownStreak;
        bool playing;
        IVisualElementScheduledItem pulse, scrollToNext;

        public bool Playing => playing;

        public DieGamePanel(VisualElement parent)
        {
            root = new VisualElement { name = "die-game" };
            root.style.position = Position.Absolute;
            root.style.left = 6; root.style.right = 6; root.style.top = 6; root.style.bottom = 6;
            root.style.backgroundColor = Back;
            root.style.overflow = Overflow.Hidden;
            Round(root, 22); Border(root, Gold, 3);
            parent.Add(root);

            // ── the passage ──
            passageView = Page();
            var head = Row(); head.style.justifyContent = Justify.SpaceBetween; passageView.Add(head);
            passageTitle = Text(32, GoldText, bold: true); head.Add(passageTitle);
            var right = Row(); head.Add(right);
            passageStatus = Text(24, Cream); passageStatus.style.marginRight = 12; right.Add(passageStatus);
            finish = Pill(right, "Finish & submit", () => FinishPressed?.Invoke(), new Color32(34, 150, 86, 255));
            Pill(right, "Start again", () => ShowConfirm(true), new Color32(123, 92, 255, 255));
            Pill(right, "Other assignments", () => AssignmentsPressed?.Invoke(), new Color32(44, 70, 150, 255));
            passageScroll = new ScrollView(ScrollViewMode.Vertical);
            passageScroll.style.flexGrow = 1; passageScroll.style.marginTop = 6;
            passageScroll.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            passageScroll.AddManipulator(SlideDrag.ForScrollView(passageScroll));
            passageView.Add(passageScroll);
            passageWords = new VisualElement();
            passageWords.style.flexDirection = FlexDirection.Row; passageWords.style.flexWrap = Wrap.Wrap;
            passageWords.style.alignItems = Align.Center;
            passageScroll.Add(passageWords);
            var legend = Text(21, new Color(Cream.r, Cream.g, Cream.b, 0.8f)); legend.enableRichText = true;
            legend.text = "Tap a key word to play it:  <color=#FFD666>define</color>   ·   <color=#FF9BC0>illustrate</color>   ·   <color=#FFD666>f_ll in</color>   ·   <color=#78E68C>restored</color>";
            legend.style.marginTop = 4;
            passageView.Add(legend);
            root.Add(passageView);

            // "Start again?" asks before clearing anything.
            confirm = new VisualElement();
            confirm.style.position = Position.Absolute;
            confirm.style.left = 0; confirm.style.right = 0; confirm.style.top = 0; confirm.style.bottom = 0;
            confirm.style.backgroundColor = new Color(0.02f, 0.04f, 0.12f, 0.88f);
            confirm.style.alignItems = Align.Center; confirm.style.justifyContent = Justify.Center;
            var ask = Text(34, Cream, bold: true); ask.text = "Start this assignment again?";
            var note = Text(24, new Color(Cream.r, Cream.g, Cream.b, 0.85f)); note.text = "Your restored words will be cleared.";
            note.style.marginTop = 6;
            var answers = Row(); answers.style.marginTop = 18;
            Pill(answers, "Yes, start again", () => { ShowConfirm(false); RetryConfirmed?.Invoke(); }, new Color32(123, 92, 255, 255));
            Pill(answers, "Cancel", () => ShowConfirm(false), new Color32(60, 70, 100, 255));
            confirm.Add(ask); confirm.Add(note); confirm.Add(answers);
            passageView.Add(confirm);
            ShowConfirm(false);

            // ── the play board ──
            playView = Page();
            var phead = Row(); phead.style.justifyContent = Justify.SpaceBetween; playView.Add(phead);
            var left = Row(); phead.Add(left);
            playBadge = Text(24, Back, bold: true);
            playBadge.style.paddingLeft = playBadge.style.paddingRight = 12; playBadge.style.paddingTop = playBadge.style.paddingBottom = 2;
            Round(playBadge, 12); left.Add(playBadge);
            playTitle = Text(38, GoldText, bold: true); playTitle.style.marginLeft = 12; left.Add(playTitle);
            streak = Text(24, Back, bold: true);
            streak.style.backgroundColor = Gold; streak.style.paddingLeft = streak.style.paddingRight = 14;
            streak.style.paddingTop = streak.style.paddingBottom = 2; Round(streak, 14); Springy(streak, 0.22f);
            var pright = Row(); phead.Add(pright);
            timeLeft = Text(22, Cream, bold: true); timeLeft.style.marginRight = 12; pright.Add(timeLeft);
            pright.Add(streak);
            playSentence = Text(32, Cream); playSentence.enableRichText = true; playSentence.style.whiteSpace = WhiteSpace.Normal;
            playSentence.style.marginTop = 6;
            playView.Add(playSentence);
            slots = Row(); slots.style.flexWrap = Wrap.Wrap; slots.style.marginTop = 10; playView.Add(slots);
            pool = Row(); pool.style.flexWrap = Wrap.Wrap; pool.style.marginTop = 8; playView.Add(pool);
            pictures = Row(); pictures.style.marginTop = 8; playView.Add(pictures);
            var spacer = new VisualElement(); spacer.style.flexGrow = 1; playView.Add(spacer);
            clue = Text(25, Cream); clue.enableRichText = true; clue.style.whiteSpace = WhiteSpace.Normal; playView.Add(clue);
            var buttons = Row(); buttons.style.justifyContent = Justify.FlexEnd; buttons.style.marginTop = 4; playView.Add(buttons);
            hint = Pill(buttons, "Hint", () => HintPressed?.Invoke(), new Color32(123, 92, 255, 255));
            guide = Pill(buttons, "Guide me", () => GuidePressed?.Invoke(), new Color32(123, 92, 255, 255));
            later = Pill(buttons, "Later", () => LaterPressed?.Invoke(), new Color32(37, 166, 154, 255));
            back = Pill(buttons, "Back to the passage", () => BackPressed?.Invoke(), new Color32(44, 70, 150, 255));
            root.Add(playView);

            playView.style.translate = new Translate(Length.Percent(105), 0);
            SetFinish(false);
            SetStreak(0);
        }

        public void Remove() { pulse?.Pause(); root.RemoveFromHierarchy(); }
        public void SetVisible(bool on) => root.style.display = on ? DisplayStyle.Flex : DisplayStyle.None;

        // ── building blocks ──

        static VisualElement Page()
        {
            var p = new VisualElement();
            p.style.position = Position.Absolute;
            p.style.left = 0; p.style.right = 0; p.style.top = 0; p.style.bottom = 0;
            p.style.paddingLeft = p.style.paddingRight = 24; p.style.paddingTop = 10; p.style.paddingBottom = 10;
            p.style.transitionProperty = new List<StylePropertyName> { new StylePropertyName("translate") };
            p.style.transitionDuration = new List<TimeValue> { new TimeValue(0.45f) };
            p.style.transitionTimingFunction = new List<EasingFunction> { new EasingFunction(EasingMode.EaseInOutCubic) };
            return p;
        }

        static VisualElement Row()
        {
            var r = new VisualElement();
            r.style.flexDirection = FlexDirection.Row; r.style.alignItems = Align.Center;
            return r;
        }

        static Label Text(int size, Color colour, bool bold = false)
        {
            var l = new Label();
            l.style.fontSize = size; l.style.color = colour;
            if (bold) l.style.unityFontStyleAndWeight = FontStyle.Bold;
            l.style.marginTop = l.style.marginBottom = 0;
            return l;
        }

        static void Springy(VisualElement e, float seconds)
        {
            e.style.transitionProperty = new List<StylePropertyName> { new StylePropertyName("scale"), new StylePropertyName("translate") };
            e.style.transitionDuration = new List<TimeValue> { new TimeValue(seconds), new TimeValue(seconds) };
            e.style.transitionTimingFunction = new List<EasingFunction> { new EasingFunction(EasingMode.EaseOutBack), new EasingFunction(EasingMode.EaseInOutCubic) };
        }

        Button Pill(VisualElement row, string text, Action click, Color fill)
        {
            var b = new Button(click) { text = text };
            b.style.backgroundColor = fill; b.style.color = Color.white;
            b.style.fontSize = 26; b.style.unityFontStyleAndWeight = FontStyle.Bold;
            b.style.paddingLeft = b.style.paddingRight = 16; b.style.paddingTop = b.style.paddingBottom = 5;
            b.style.marginLeft = 10;
            Round(b, 18); Border(b, Color.clear, 0);
            Springy(b, 0.16f);
            b.RegisterCallback<PointerDownEvent>(_ => b.style.scale = new Scale(new Vector2(0.9f, 0.9f)), TrickleDown.TrickleDown);
            b.RegisterCallback<PointerUpEvent>(_ => b.style.scale = new Scale(Vector2.one), TrickleDown.TrickleDown);
            b.RegisterCallback<PointerLeaveEvent>(_ => b.style.scale = new Scale(Vector2.one));
            row.Add(b);
            return b;
        }

        // ── the passage ──

        public void SetPassageTitle(string title, string status)
        {
            passageTitle.text = title ?? "";
            passageStatus.text = status ?? "";
        }

        /// <summary>Lay out the whole passage, word by word; key words are buttons, the next one glows.</summary>
        public void SetPassage(IReadOnlyList<Word> words)
        {
            // A redraw cancels what the last one scheduled (its words are gone now).
            pulse?.Pause(); pulse = null;
            scrollToNext?.Pause(); scrollToNext = null;
            passageWords.Clear(); keyButtons.Clear();
            VisualElement glowing = null;
            foreach (var w in words)
            {
                VisualElement el;
                if (w.key < 0)
                {
                    var l = Text(40, Cream); l.text = w.text;
                    l.style.marginRight = 11; l.style.marginBottom = 6;
                    el = l;
                }
                else el = KeyWord(w);
                passageWords.Add(el);
                if (w.key >= 0) keyButtons[w.key] = el;
                if (w.next) glowing = el;
            }
            if (glowing != null)
            {
                // the next key word breathes, so the eye finds it
                bool big = false;
                pulse = glowing.schedule.Execute(() => { big = !big; glowing.style.scale = new Scale(big ? new Vector2(1.08f, 1.08f) : Vector2.one); }).Every(520);
                var target = glowing;
                scrollToNext = passageScroll.schedule.Execute(() =>
                {
                    scrollToNext = null;
                    // only while that word is still on the board (a later redraw replaces it)
                    if (target.panel != null && passageScroll.contentContainer.Contains(target)) passageScroll.ScrollTo(target);
                }).StartingIn(60);
            }
        }

        VisualElement KeyWord(Word w)
        {
            var box = Row();
            box.style.marginRight = 8; box.style.marginBottom = 4;
            box.style.paddingLeft = box.style.paddingRight = 6; box.style.paddingTop = box.style.paddingBottom = 0;
            Round(box, 10);
            Springy(box, 0.5f);
            var colour = w.mark == Mark.Illustrate ? Pink : w.mark == Mark.Restored ? Green : GoldText;
            if (w.mark == Mark.Illustrate)
            {
                // a little picture mark: a framed sun over a hill
                var pic = new VisualElement();
                pic.style.width = 30; pic.style.height = 24; pic.style.marginRight = 6;
                Border(pic, Pink, 2); Round(pic, 4);
                var sun = new VisualElement(); sun.style.position = Position.Absolute; sun.style.width = 6; sun.style.height = 6;
                sun.style.left = 3; sun.style.top = 2; sun.style.backgroundColor = Pink; Round(sun, 3); pic.Add(sun);
                var hill = new VisualElement(); hill.style.position = Position.Absolute; hill.style.left = 2; hill.style.right = 2; hill.style.bottom = 2;
                hill.style.height = 5; hill.style.backgroundColor = Pink; Round(hill, 3); pic.Add(hill);
                box.Add(pic);
            }
            var l = Text(40, colour, bold: true);
            l.text = w.text;
            if (w.mark == Mark.Fill) { Border(box, GoldText, 2); }
            else if (w.mark != Mark.Restored) { l.style.borderBottomWidth = 3; l.style.borderBottomColor = colour; }
            if (w.mark != Mark.Restored) box.style.backgroundColor = new Color(colour.r, colour.g, colour.b, w.next ? 0.22f : 0.1f);
            box.Add(l);
            int key = w.key;
            box.RegisterCallback<PointerDownEvent>(_ => box.style.scale = new Scale(new Vector2(0.9f, 0.9f)), TrickleDown.TrickleDown);
            box.RegisterCallback<PointerUpEvent>(_ => { box.style.scale = new Scale(Vector2.one); KeyWordPressed?.Invoke(key); });
            box.RegisterCallback<PointerLeaveEvent>(_ => box.style.scale = new Scale(Vector2.one));
            return box;
        }

        /// <summary>Where a key word sits (for an effect at it), or null.</summary>
        public VisualElement KeyElement(int key) => keyButtons.TryGetValue(key, out var e) ? e : null;

        public void SetFinish(bool show) => finish.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;

        void ShowConfirm(bool on) => confirm.style.display = on ? DisplayStyle.Flex : DisplayStyle.None;

        // ── switching boards (a wipe) ──

        public void ShowPlay()
        {
            playing = true;
            passageView.style.translate = new Translate(Length.Percent(-105), 0);
            playView.style.translate = new Translate(0, 0);
        }

        public void ShowPassage()
        {
            playing = false;
            passageView.style.translate = new Translate(0, 0);
            playView.style.translate = new Translate(Length.Percent(105), 0);
        }

        // ── the play board ──

        public void SetPlay(string badge, Color badgeColour, string title, string sentenceRich)
        {
            playBadge.text = badge; playBadge.style.backgroundColor = badgeColour;
            playTitle.text = title ?? "";
            playSentence.text = sentenceRich ?? "";
        }

        public void SetClue(string text)
        {
            clue.text = text ?? "";
            clue.style.display = string.IsNullOrEmpty(text) ? DisplayStyle.None : DisplayStyle.Flex;
        }

        public void SetButtons(bool showHint, bool showGuide, bool showLater, string backText = "Back to the passage")
        {
            hint.style.display = showHint ? DisplayStyle.Flex : DisplayStyle.None;
            guide.style.display = showGuide ? DisplayStyle.Flex : DisplayStyle.None;
            later.style.display = showLater ? DisplayStyle.Flex : DisplayStyle.None;
            back.text = backText;
        }

        public void SetHintLabel(string text) => hint.text = text;

        /// <summary>The word's time left (the trek timer); hidden when negative / untimed.</summary>
        public void SetTimeLeft(float seconds)
        {
            if (seconds < 0f) { timeLeft.style.display = DisplayStyle.None; return; }
            timeLeft.style.display = DisplayStyle.Flex;
            int s = Mathf.CeilToInt(seconds);
            timeLeft.text = $"Time  {s / 60}:{s % 60:00}";
            timeLeft.style.color = s <= 10 ? Pink : Cream;
        }

        public void SetStreak(int n)
        {
            streak.text = n > 0 ? $"★ {n} in a row" : "★ 0";
            streak.style.opacity = n > 0 ? 1f : 0.55f;
            if (n > shownStreak)
            {
                streak.style.scale = new Scale(new Vector2(1.25f, 1.25f));
                streak.schedule.Execute(() => streak.style.scale = new Scale(Vector2.one)).StartingIn(120);
            }
            shownStreak = n;
        }

        /// <summary>The answer slots: filled ones show their piece.</summary>
        public void SetSlots(IReadOnlyList<string> filled, int total, bool letters)
        {
            slots.Clear(); slotChips.Clear();
            for (int i = 0; i < total; i++)
            {
                bool has = i < filled.Count;
                var l = Text(letters ? 46 : 34, has ? Back : new Color(Cream.r, Cream.g, Cream.b, 0.4f), bold: true);
                l.text = has ? filled[i] : (letters ? "_" : "____");
                l.style.marginRight = 8; l.style.marginBottom = 6;
                l.style.paddingLeft = l.style.paddingRight = 10; l.style.paddingTop = l.style.paddingBottom = 2;
                l.style.backgroundColor = has ? Cream : new Color(1f, 1f, 1f, 0.08f);
                Round(l, 10);
                slots.Add(l); slotChips.Add(l);
            }
        }

        /// <summary>The pieces still to place, shuffled (as the die shows them).</summary>
        public void SetPool(IReadOnlyList<string> pieces)
        {
            pool.Clear(); poolChips.Clear();
            if (pieces == null) return;
            foreach (var p in pieces)
            {
                var l = Text(30, Cream, bold: true);
                l.text = p;
                l.style.marginRight = 8; l.style.marginBottom = 4;
                l.style.paddingLeft = l.style.paddingRight = 8; l.style.paddingTop = l.style.paddingBottom = 1;
                Border(l, new Color(Cream.r, Cream.g, Cream.b, 0.45f), 2); Round(l, 9);
                Springy(l, 0.4f);
                pool.Add(l); poolChips.Add(l);
            }
        }

        /// <summary>A right pick: the piece flies from the pool into its slot, then the board shows it placed.</summary>
        public void FlyToSlot(int poolIndex, int slotIndex, Action landed)
        {
            if (poolIndex < 0 || poolIndex >= poolChips.Count || slotIndex < 0 || slotIndex >= slotChips.Count) { landed?.Invoke(); return; }
            var from = poolChips[poolIndex];
            var to = slotChips[slotIndex];
            var delta = to.worldBound.center - from.worldBound.center;
            from.style.backgroundColor = Gold; from.style.color = Back;
            from.style.translate = new Translate(delta.x, delta.y);
            from.style.scale = new Scale(new Vector2(1.2f, 1.2f));
            root.schedule.Execute(() => landed?.Invoke()).StartingIn(420);   // on the panel, not the chip: a redraw can't lose it
        }

        /// <summary>The pictures of an Illustrate word, numbered like the die's faces (null hides them).</summary>
        public void SetPictures(IReadOnlyList<Texture2D> textures, int glow = -1, int wrong = -1)
        {
            pictures.Clear();
            if (textures == null) return;
            for (int i = 0; i < textures.Count; i++)
            {
                var box = new VisualElement();
                box.style.width = 210; box.style.height = 160; box.style.marginRight = 18;
                box.style.backgroundColor = new Color(1f, 1f, 1f, 0.94f);
                Round(box, 14);
                if (i == glow) Border(box, Gold, 5); else if (i == wrong) Border(box, Pink, 4);
                if (textures[i] != null)
                {
                    box.style.backgroundImage = Background.FromTexture2D(textures[i]);
                    box.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
                }
                var n = Text(22, Back, bold: true); n.text = (i + 1).ToString();
                n.style.position = Position.Absolute; n.style.left = 6; n.style.top = 4;
                n.style.backgroundColor = Gold; n.style.paddingLeft = n.style.paddingRight = 8; Round(n, 11);
                box.Add(n);
                pictures.Add(box);
            }
        }

        /// <summary>A quick glow of the rim: mint for right, pink for wrong.</summary>
        public void Flash(bool right)
        {
            Border(root, right ? Mint : Pink, 5);
            root.schedule.Execute(() => Border(root, Gold, 3)).StartingIn(240);
        }

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
