using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.Game.Trek
{
    /// <summary>
    /// The Story Trek's storybook palette and small element builders, matching the
    /// design mockup's tokens (paper/ink/leaf/sun/sky/coral/violet + night/panel).
    /// Deliberately distinct from the app shell's dark theme: the game world is
    /// warm and bright, the HUD sits on translucent "glass". Mirrors
    /// Assets/UI/Game/TrekTokens.uss — keep the two in sync.
    /// </summary>
    internal static class TrekTheme
    {
        // ── Mockup tokens ──
        public static readonly Color Paper  = Hex(0xF7F8F4);
        public static readonly Color Card   = Hex(0xFFFFFF);
        public static readonly Color Ink    = Hex(0x1D2417);
        public static readonly Color Line   = Hex(0xE0E5D8);
        public static readonly Color Leaf   = Hex(0x5FCE7E);
        public static readonly Color Sun    = Hex(0xFFC93C);
        public static readonly Color Sky    = Hex(0x57C8F2);
        public static readonly Color Coral  = Hex(0xFF6B5D);
        public static readonly Color Violet = Hex(0x9A86F2);
        public static readonly Color Night  = Hex(0x122217);
        public static readonly Color Panel  = Hex(0x1D3524);

        // ── Derived world shades (Netville Meadows, day) ──
        public static readonly Color SkyTop     = Hex(0x9ADCF7);
        public static readonly Color HillFar    = Hex(0x8FDCA8);
        public static readonly Color HillNear   = Hex(0x6ECF8D);
        public static readonly Color Ground     = Hex(0x54B872);
        public static readonly Color PathTan    = Hex(0xE8D5A8);
        public static readonly Color GateWood   = Hex(0x8A6A4A);
        public static readonly Color Muted      = new Color(0.11f, 0.14f, 0.09f, 0.55f);   // ink @ 55%

        /// <summary>HUD "glass": dark translucent chip that reads over any biome.</summary>
        public static readonly Color Glass = new Color(0.07f, 0.13f, 0.09f, 0.78f);

        public static Color Hex(int rgb) => new Color(
            ((rgb >> 16) & 0xFF) / 255f, ((rgb >> 8) & 0xFF) / 255f, (rgb & 0xFF) / 255f);

        // ── 1×1 solid sprite (shared by every generated world quad) ──
        static Sprite solid;
        public static Sprite SolidSprite
        {
            get
            {
                if (solid == null)
                {
                    var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                    tex.SetPixel(0, 0, Color.white);
                    tex.Apply();
                    tex.hideFlags = HideFlags.HideAndDontSave;
                    solid = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
                    solid.hideFlags = HideFlags.HideAndDontSave;
                }
                return solid;
            }
        }

        // ── UI Toolkit builders (storybook style) ──

        public static void Round(VisualElement e, float r)
        {
            e.style.borderTopLeftRadius = e.style.borderTopRightRadius = r;
            e.style.borderBottomLeftRadius = e.style.borderBottomRightRadius = r;
        }

        public static void Border(VisualElement e, Color c, float w)
        {
            e.style.borderLeftWidth = e.style.borderRightWidth = w;
            e.style.borderTopWidth = e.style.borderBottomWidth = w;
            e.style.borderLeftColor = e.style.borderRightColor = c;
            e.style.borderTopColor = e.style.borderBottomColor = c;
        }

        /// <summary>A white storybook card with the soft line border.</summary>
        public static VisualElement CardBox()
        {
            var c = new VisualElement();
            c.style.backgroundColor = Card;
            Round(c, 16);
            Border(c, Line, 1);
            c.style.paddingLeft = c.style.paddingRight = 16;
            c.style.paddingTop = c.style.paddingBottom = 14;
            return c;
        }

        public static Label Title(string text, int size = 18)
        {
            var l = new Label(text);
            l.style.fontSize = size;
            l.style.unityFontStyleAndWeight = FontStyle.Bold;
            l.style.color = Ink;
            l.style.whiteSpace = WhiteSpace.Normal;
            return l;
        }

        public static Label Sub(string text, int size = 13)
        {
            var l = new Label(text);
            l.style.fontSize = size;
            l.style.color = Muted;
            l.style.whiteSpace = WhiteSpace.Normal;
            return l;
        }

        /// <summary>Primary action — sun-yellow, ink text (the mockup's CTA).</summary>
        public static Button Primary(string text, System.Action onClick)
        {
            var b = new Button(onClick) { text = text };
            b.style.height = 44;
            b.style.paddingLeft = b.style.paddingRight = 20;
            b.style.backgroundColor = Sun;
            b.style.color = Ink;
            b.style.fontSize = 15;
            b.style.unityFontStyleAndWeight = FontStyle.Bold;
            Round(b, 12);
            Border(b, Color.clear, 0);
            return b;
        }

        /// <summary>Ghost action — outlined on white.</summary>
        public static Button Ghost(string text, System.Action onClick)
        {
            var b = new Button(onClick) { text = text };
            b.style.height = 40;
            b.style.paddingLeft = b.style.paddingRight = 14;
            b.style.backgroundColor = Color.clear;
            b.style.color = Ink;
            b.style.fontSize = 14;
            Round(b, 12);
            Border(b, Line, 1);
            return b;
        }

        /// <summary>Tappable piece chip for the arrange/spell games.</summary>
        public static Button Chip(string text, System.Action onClick, bool filled, bool locked = false)
        {
            var b = new Button(onClick) { text = text };
            b.style.height = 40;
            b.style.minWidth = 40;
            b.style.paddingLeft = b.style.paddingRight = 12;
            b.style.marginRight = 6;
            b.style.marginBottom = 6;
            b.style.fontSize = 16;
            b.style.unityFontStyleAndWeight = FontStyle.Bold;
            Round(b, 10);
            if (locked)
            {
                b.style.backgroundColor = Line;
                b.style.color = Muted;
                Border(b, Line, 1);
                b.SetEnabled(false);
            }
            else if (filled)
            {
                b.style.backgroundColor = Sun;
                b.style.color = Ink;
                Border(b, Color.clear, 0);
            }
            else
            {
                b.style.backgroundColor = Paper;
                b.style.color = Ink;
                Border(b, Line, 1);
            }
            return b;
        }

        /// <summary>Translucent HUD chip ("glass") with light text.</summary>
        public static Label GlassChip(string text)
        {
            var l = new Label(text);
            l.style.backgroundColor = Glass;
            l.style.color = Paper;
            l.style.fontSize = 13;
            l.style.unityFontStyleAndWeight = FontStyle.Bold;
            l.style.paddingLeft = l.style.paddingRight = 12;
            l.style.paddingTop = l.style.paddingBottom = 6;
            l.style.marginRight = 8;
            Round(l, 12);
            return l;
        }
    }
}
