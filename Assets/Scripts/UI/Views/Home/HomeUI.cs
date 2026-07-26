using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace SReader.UI.Views.Home
{
    /// <summary>
    /// Small factory of design-system-styled UI Toolkit elements, shared by the
    /// home sections that build their content in code (e.g. the Academies CRUD).
    /// Keeps the palette and inline styling in one place so every card, button
    /// and field matches the rest of the app.
    /// </summary>
    internal static class HomeUI
    {
        public static readonly Color Bg        = Rgb(18, 20, 25);
        public static readonly Color Panel     = Rgb(35, 39, 51);
        public static readonly Color Hairline  = Rgb(46, 51, 64);
        public static readonly Color Parchment = Rgb(233, 223, 200);
        public static readonly Color Text      = Rgb(242, 239, 230);
        public static readonly Color Muted      = Rgb(152, 160, 174);
        public static readonly Color Danger     = Rgb(224, 122, 113);
        public static readonly Color Success    = Rgb(140, 196, 140);

        public static Label Heading(string text)
        {
            var l = new Label(text);
            l.style.fontSize = 20;
            l.style.unityFontStyleAndWeight = FontStyle.Bold;
            l.style.color = Text;
            l.style.marginBottom = 4;
            l.style.whiteSpace = WhiteSpace.Normal;
            return l;
        }

        public static Label Caption(string text)
        {
            var l = new Label(text);
            l.style.fontSize = 14;
            l.style.color = Muted;
            l.style.marginBottom = 16;
            l.style.whiteSpace = WhiteSpace.Normal;
            return l;
        }

        public static Label Title(string text, float size = 16)
        {
            var l = new Label(text);
            l.style.fontSize = size;
            l.style.unityFontStyleAndWeight = FontStyle.Bold;
            l.style.color = Text;
            l.style.whiteSpace = WhiteSpace.Normal;
            return l;
        }

        public static Label Sub(string text)
        {
            var l = new Label(text);
            l.style.fontSize = 13;
            l.style.color = Muted;
            l.style.whiteSpace = WhiteSpace.Normal;
            l.style.marginTop = 2;
            return l;
        }

        public static Label FieldLabel(string text)
        {
            var l = new Label(text);
            l.style.fontSize = 12;
            l.style.color = Muted;
            l.style.marginBottom = 4;
            l.style.marginTop = 6;
            return l;
        }

        public static Label Status(string text, bool isError)
        {
            var l = new Label(text);
            l.style.fontSize = 13;
            l.style.color = isError ? Danger : Success;
            l.style.whiteSpace = WhiteSpace.Normal;
            l.style.marginTop = 6;
            l.style.marginBottom = 6;
            return l;
        }

        public static VisualElement Card()
        {
            var c = new VisualElement();
            c.style.backgroundColor = Panel;
            Radius(c, 12);
            Border(c, Hairline, 1);
            c.style.paddingTop = c.style.paddingBottom = 14;
            c.style.paddingLeft = c.style.paddingRight = 14;
            c.style.marginBottom = 10;
            return c;
        }

        public static VisualElement Row()
        {
            var r = new VisualElement();
            r.style.flexDirection = FlexDirection.Row;
            r.style.alignItems = Align.Center;
            return r;
        }

        public static TextField Field(string value, bool multiline = false)
        {
            var f = new TextField { value = value ?? "" };
            f.multiline = multiline;
            f.style.marginBottom = 6;
            f.style.color = Text;
            f.style.fontSize = 15;
            f.style.backgroundColor = Bg;
            Radius(f, 10);
            Border(f, Hairline, 1);
            f.style.paddingLeft = f.style.paddingRight = 8;
            f.style.minHeight = multiline ? 64 : 40;
            StripInputSkin(f);
            return f;
        }

        /// <summary>
        /// Unity's TextField ships an inner "#unity-text-input" child with the
        /// built-in (white) theme skin, so styling the outer field alone leaves
        /// a white box that hides our light text. Auth pages fix this on a whole
        /// UIDocument via AuthPageStyler; code-built fields appear after that runs,
        /// so each field strips its own inner skin here. The inner child is created
        /// in the TextField ctor, but we also re-apply on attach for safety.
        /// </summary>
        static void StripInputSkin(TextField f)
        {
            ApplyInputSkin(f);
            f.RegisterCallback<AttachToPanelEvent>(_ => ApplyInputSkin(f));
        }

        static void ApplyInputSkin(TextField f)
        {
            var input = f.Q("unity-text-input");
            if (input == null) return;
            input.style.backgroundColor   = Color.clear;
            input.style.borderTopWidth    = 0;
            input.style.borderBottomWidth = 0;
            input.style.borderLeftWidth   = 0;
            input.style.borderRightWidth  = 0;
            input.style.unityTextAlign    = TextAnchor.MiddleLeft;
            input.style.color             = Text;
        }

        /// <summary>
        /// A dark, design-system dropdown (see <see cref="HomeDropdown"/>) — no
        /// visible scrollbars and always-legible text, unlike the native control.
        /// </summary>
        public static HomeDropdown Dropdown(string[] choices) => new HomeDropdown(choices);

        public static Button Primary(string text, Action onClick)
        {
            var b = new Button(onClick) { text = text };
            b.style.height = 44;
            b.style.backgroundColor = Parchment;
            b.style.color = Bg;
            b.style.unityFontStyleAndWeight = FontStyle.Bold;
            b.style.fontSize = 15;
            Radius(b, 12);
            Border(b, Parchment, 0);
            return b;
        }

        public static Button Outline(string text, Action onClick)
        {
            var b = new Button(onClick) { text = text };
            b.style.height = 40;
            b.style.backgroundColor = Color.clear;
            b.style.color = Text;
            b.style.fontSize = 14;
            Radius(b, 12);
            Border(b, Hairline, 1);
            return b;
        }

        public static Button Danger_(string text, Action onClick)
        {
            var b = Outline(text, onClick);
            b.style.color = Danger;
            return b;
        }

        /// <summary>
        /// A compact pill button in the login Apple/Google style: transparent
        /// fill, hairline (or danger) border, bold label. Use instead of
        /// full-width Primary buttons for secondary / grouped actions.
        /// </summary>
        public static Button Chip(string text, Action onClick, bool danger = false)
        {
            var b = new Button(onClick) { text = text };
            b.style.height = 44;
            b.style.backgroundColor = Color.clear;
            b.style.color = danger ? Danger : Text;
            b.style.fontSize = 15;
            b.style.unityFontStyleAndWeight = FontStyle.Bold;
            Radius(b, 14);
            Border(b, danger ? Danger : Hairline, 1);
            b.style.paddingLeft = b.style.paddingRight = 16;
            b.style.marginLeft = b.style.marginRight = 0;
            return b;
        }

        /// <summary>Lay buttons side by side, equal width — the login Apple/Google row.</summary>
        public static VisualElement ChipRow(params VisualElement[] buttons)
        {
            var row = Row();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].style.flexGrow = 1;
                buttons[i].style.flexBasis = 0;
                if (i > 0) buttons[i].style.marginLeft = 8;
                row.Add(buttons[i]);
            }
            return row;
        }

        /// <summary>A row whose children keep their content width and wrap onto the next line.</summary>
        public static VisualElement WrapRow()
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexWrap = Wrap.Wrap;
            row.style.alignItems = Align.Center;
            return row;
        }

        /// <summary>Mini-navigation tab styling (parchment underline when selected).</summary>
        public static void StyleTab(Button tab, bool selected)
        {
            tab.style.backgroundColor = Color.clear;
            tab.style.color = selected ? Parchment : Muted;
            tab.style.unityFontStyleAndWeight = selected ? FontStyle.Bold : FontStyle.Normal;
            tab.style.fontSize = 14;
            tab.style.marginRight = 18;
            tab.style.paddingLeft = tab.style.paddingRight = 0;
            tab.style.paddingTop = tab.style.paddingBottom = 4;
            Border(tab, Color.clear, 0);
            tab.style.borderBottomWidth = selected ? 2 : 0;
            tab.style.borderBottomColor = Parchment;
        }

        public static Button Link(string text, Action onClick)
        {
            var b = new Button(onClick) { text = text };
            b.style.backgroundColor = Color.clear;
            b.style.color = Parchment;
            b.style.unityFontStyleAndWeight = FontStyle.Bold;
            b.style.fontSize = 14;
            Border(b, Color.clear, 0);
            b.style.paddingLeft = b.style.paddingRight = 4;
            return b;
        }

        /// <summary>A framed box that displays a downloaded image, scaled to fit.</summary>
        public static VisualElement ImageBox(float height = 240)
        {
            var box = new VisualElement();
            box.style.height = height;
            box.style.marginTop = 8;
            box.style.backgroundColor = Bg;
            Radius(box, 10);
            Border(box, Hairline, 1);
            ContainBackground(box);
            return box;
        }

        /// <summary>Downloads an image URL and sets it as the element's background.</summary>
        // Downloaded images (avatars especially) are cached by URL so they paint
        // INSTANTLY on re-render instead of re-fetching and flickering each time
        // the player list / room refreshes.
        static readonly Dictionary<string, Texture2D> ImageCache = new Dictionary<string, Texture2D>();

        public static async void LoadImageInto(VisualElement target, string url)
        {
            if (string.IsNullOrEmpty(url) || target == null) return;

            // Already downloaded — set synchronously, no flash, no network call.
            if (ImageCache.TryGetValue(url, out var cached) && cached != null)
            {
                target.style.backgroundImage = new StyleBackground(cached);
                return;
            }

            var tex = await LoadTextureAsync(url);
            if (tex == null) return;
            ImageCache[url] = tex;
            if (target != null)
                target.style.backgroundImage = new StyleBackground(tex);
        }

        static Task<Texture2D> LoadTextureAsync(string url)
        {
            var tcs = new TaskCompletionSource<Texture2D>();
            var request = UnityWebRequestTexture.GetTexture(url);
            var op = request.SendWebRequest();
            op.completed += _ =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                    tcs.SetResult(DownloadHandlerTexture.GetContent(request));
                else
                    tcs.SetResult(null);
                request.Dispose();
            };
            return tcs.Task;
        }

        /// <summary>
        /// Scale a background image to fill the element, cropping overflow — the
        /// replacement for the deprecated <c>ScaleMode.ScaleAndCrop</c>.
        /// </summary>
        public static void CoverBackground(VisualElement el)
            => el.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Cover);

        /// <summary>
        /// Scale a background image to fit inside the element — the replacement
        /// for the deprecated <c>ScaleMode.ScaleToFit</c>.
        /// </summary>
        public static void ContainBackground(VisualElement el)
            => el.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);

        public static void Radius(VisualElement el, float r)
        {
            el.style.borderTopLeftRadius = el.style.borderTopRightRadius = r;
            el.style.borderBottomLeftRadius = el.style.borderBottomRightRadius = r;
        }

        public static void Border(VisualElement el, Color color, float width)
        {
            el.style.borderLeftWidth = el.style.borderRightWidth = width;
            el.style.borderTopWidth = el.style.borderBottomWidth = width;
            el.style.borderLeftColor = el.style.borderRightColor = color;
            el.style.borderTopColor = el.style.borderBottomColor = color;
        }

        static Color Rgb(int r, int g, int b) => new Color(r / 255f, g / 255f, b / 255f);
    }
}
