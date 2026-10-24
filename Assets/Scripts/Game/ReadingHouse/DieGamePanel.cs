using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The die game's screen, on the dashboard's display: one dark panel with a single gold rim laid over
    /// the display's picture so every word reads clearly. It holds
    ///   • the sentence being read (key words styled as in the app's reading game: fill-in words a blank,
    ///     define words gold, picture words pink, restored words green; the words already picked bright);
    ///   • the key word's activity while one is open: the definition / word slots filling in, the pieces
    ///     still to place, or the pictures to choose from, large and numbered (the die shows the same numbers);
    ///   • the streak counter, a clue line and the buttons (Flip the die, Hint, Guide me, Later, Read, Finish).
    /// Built in code into the dashboard screen's page; presentation only.
    /// </summary>
    public sealed class DieGamePanel
    {
        public event Action FlipPressed, HintPressed, GuidePressed, LaterPressed, ReadPressed, FinishPressed;

        /// <summary>One slot / piece of an activity line.</summary>
        public enum Piece { Word, Key, KeyBlank, Later, Slot }

        static readonly Color Back = new Color32(10, 18, 44, 238);
        static readonly Color Gold = new Color32(255, 196, 64, 255);
        static readonly Color GoldText = new Color32(255, 214, 102, 255);
        static readonly Color Cream = new Color32(255, 244, 214, 255);
        static readonly Color Mint = new Color32(140, 228, 196, 255);
        static readonly Color Pink = new Color32(255, 150, 190, 255);

        readonly VisualElement root;
        readonly Label title, streak, sentence, clue;
        readonly VisualElement activity, line, pictures;
        readonly Button flip, hint, guide, later, read, finish;
        int shownStreak;

        public DieGamePanel(VisualElement parent)
        {
            root = new VisualElement { name = "die-game" };
            root.style.position = Position.Absolute;
            root.style.left = 10; root.style.right = 10; root.style.top = 8; root.style.bottom = 8;
            root.style.backgroundColor = Back;
            Round(root, 22); Border(root, Gold, 3);
            root.style.paddingLeft = root.style.paddingRight = 26;
            root.style.paddingTop = 10; root.style.paddingBottom = 10;
            parent.Add(root);

            var head = Row(); head.style.justifyContent = Justify.SpaceBetween; root.Add(head);
            title = new Label(); title.style.color = GoldText; title.style.fontSize = 26; title.style.unityFontStyleAndWeight = FontStyle.Bold;
            head.Add(title);
            streak = new Label(); streak.style.color = Back; streak.style.backgroundColor = Gold;
            streak.style.fontSize = 24; streak.style.unityFontStyleAndWeight = FontStyle.Bold;
            streak.style.paddingLeft = streak.style.paddingRight = 14; streak.style.paddingTop = streak.style.paddingBottom = 2;
            Round(streak, 16);
            Springy(streak, 0.22f);
            head.Add(streak);

            sentence = new Label { enableRichText = true };
            sentence.style.color = Cream; sentence.style.fontSize = 40; sentence.style.unityFontStyleAndWeight = FontStyle.Bold;
            sentence.style.whiteSpace = WhiteSpace.Normal; sentence.style.marginTop = 4;
            root.Add(sentence);

            activity = new VisualElement(); activity.style.marginTop = 4; activity.style.flexGrow = 1;
            root.Add(activity);
            line = Row(); line.style.flexWrap = Wrap.Wrap; activity.Add(line);
            pictures = Row(); pictures.style.marginTop = 2; activity.Add(pictures);

            clue = new Label { enableRichText = true };
            clue.style.color = new Color(Cream.r, Cream.g, Cream.b, 0.9f); clue.style.fontSize = 24; clue.style.whiteSpace = WhiteSpace.Normal;
            root.Add(clue);

            var buttons = Row(); buttons.style.justifyContent = Justify.FlexEnd; buttons.style.marginTop = 4; root.Add(buttons);
            flip = Pill(buttons, "Flip the die", () => FlipPressed?.Invoke(), new Color32(255, 138, 31, 255));
            hint = Pill(buttons, "Hint", () => HintPressed?.Invoke(), new Color32(123, 92, 255, 255));
            guide = Pill(buttons, "Guide me", () => GuidePressed?.Invoke(), new Color32(123, 92, 255, 255));
            later = Pill(buttons, "Later", () => LaterPressed?.Invoke(), new Color32(37, 166, 154, 255));
            read = Pill(buttons, "Read the passage", () => ReadPressed?.Invoke(), new Color32(44, 70, 150, 255));
            finish = Pill(buttons, "Finish & submit", () => FinishPressed?.Invoke(), new Color32(34, 150, 86, 255));
            SetStreak(0);
            SetButtons(false, false, false, false, true);
            SetFinish(false);
        }

        public void Remove() => root.RemoveFromHierarchy();
        public void SetVisible(bool on) => root.style.display = on ? DisplayStyle.Flex : DisplayStyle.None;

        static VisualElement Row()
        {
            var r = new VisualElement();
            r.style.flexDirection = FlexDirection.Row; r.style.alignItems = Align.Center;
            return r;
        }

        static void Springy(VisualElement e, float seconds)
        {
            e.style.transitionProperty = new List<StylePropertyName> { new StylePropertyName("scale") };
            e.style.transitionDuration = new List<TimeValue> { new TimeValue(seconds) };
            e.style.transitionTimingFunction = new List<EasingFunction> { new EasingFunction(EasingMode.EaseOutBack) };
        }

        Button Pill(VisualElement row, string text, Action click, Color fill)
        {
            var b = new Button(click) { text = text };
            b.style.backgroundColor = fill; b.style.color = Color.white;
            b.style.fontSize = 26; b.style.unityFontStyleAndWeight = FontStyle.Bold;
            b.style.paddingLeft = b.style.paddingRight = 18; b.style.paddingTop = b.style.paddingBottom = 6;
            b.style.marginLeft = 10;
            Round(b, 20); Border(b, Color.clear, 0);
            Springy(b, 0.16f);
            b.RegisterCallback<PointerDownEvent>(_ => b.style.scale = new Scale(new Vector2(0.9f, 0.9f)), TrickleDown.TrickleDown);
            b.RegisterCallback<PointerUpEvent>(_ => b.style.scale = new Scale(Vector2.one), TrickleDown.TrickleDown);
            b.RegisterCallback<PointerLeaveEvent>(_ => b.style.scale = new Scale(Vector2.one));
            row.Add(b);
            return b;
        }

        public void SetTitle(string text) => title.text = text ?? "";

        /// <summary>The sentence (rich text: colours / underline for the key words, dim for words still to pick).</summary>
        public void SetSentence(string rich) => sentence.text = rich ?? "";

        public void SetClue(string text)
        {
            clue.text = text ?? "";
            clue.style.display = string.IsNullOrEmpty(text) ? DisplayStyle.None : DisplayStyle.Flex;
        }

        public void SetButtons(bool showFlip, bool showHint, bool showGuide, bool showLater, bool showRead)
        {
            flip.style.display = showFlip ? DisplayStyle.Flex : DisplayStyle.None;
            hint.style.display = showHint ? DisplayStyle.Flex : DisplayStyle.None;
            guide.style.display = showGuide ? DisplayStyle.Flex : DisplayStyle.None;
            later.style.display = showLater ? DisplayStyle.Flex : DisplayStyle.None;
            read.style.display = showRead ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetFinish(bool show) => finish.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        public void SetHintLabel(string text) => hint.text = text;

        /// <summary>The streak counter; it pops each time it climbs.</summary>
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

        /// <summary>An activity's line (definition / word slots); empty hides it. The newest piece pops in.</summary>
        public void SetLine(IReadOnlyList<(string text, Piece kind)> pieces, bool popLast)
        {
            line.Clear();
            if (pieces == null) return;
            for (int i = 0; i < pieces.Count; i++)
            {
                var (text, kind) = pieces[i];
                var l = new Label(text);
                l.style.fontSize = 34; l.style.unityFontStyleAndWeight = FontStyle.Bold;
                l.style.marginRight = 10; l.style.marginBottom = 2;
                l.style.paddingLeft = l.style.paddingRight = 8; l.style.paddingTop = l.style.paddingBottom = 1;
                Round(l, 10);
                switch (kind)
                {
                    case Piece.Word: l.style.color = Back; l.style.backgroundColor = Cream; break;
                    case Piece.Key: l.style.color = Back; l.style.backgroundColor = Gold; break;
                    case Piece.KeyBlank: l.style.color = Gold; break;
                    case Piece.Later: l.style.color = Mint; break;
                    case Piece.Slot: l.style.color = new Color(Cream.r, Cream.g, Cream.b, 0.4f); l.style.backgroundColor = new Color(1f, 1f, 1f, 0.08f); break;
                }
                if (popLast && i == pieces.Count - 1 && kind != Piece.Slot)
                {
                    l.style.scale = new Scale(new Vector2(0.4f, 0.4f));
                    Springy(l, 0.25f);
                    l.schedule.Execute(() => l.style.scale = new Scale(Vector2.one)).StartingIn(16);
                }
                line.Add(l);
            }
        }

        /// <summary>The pictures of an Illustrate word, numbered like the die's faces (null / empty hides them).</summary>
        public void SetPictures(IReadOnlyList<Texture2D> textures, int glow = -1)
        {
            pictures.Clear();
            if (textures == null) return;
            for (int i = 0; i < textures.Count; i++)
            {
                var box = new VisualElement();
                box.style.width = 120; box.style.height = 96; box.style.marginRight = 14;
                box.style.backgroundColor = new Color(1f, 1f, 1f, 0.92f);
                Round(box, 12);
                if (i == glow) Border(box, Gold, 4);
                if (textures[i] != null)
                {
                    box.style.backgroundImage = Background.FromTexture2D(textures[i]);
                    box.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
                }
                var n = new Label((i + 1).ToString());
                n.style.position = Position.Absolute; n.style.left = 6; n.style.top = 4;
                n.style.color = Back; n.style.backgroundColor = Gold; n.style.fontSize = 18; n.style.unityFontStyleAndWeight = FontStyle.Bold;
                n.style.paddingLeft = n.style.paddingRight = 7;
                Round(n, 10);
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
