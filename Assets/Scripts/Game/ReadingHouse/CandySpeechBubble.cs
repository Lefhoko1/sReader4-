using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The Encourager's speech: a candy-coloured bubble (bubblegum pink → peach → lemon, white rim,
    /// bold white words with a candy-pink outline, sprinkles of confetti and a little tail) that pops
    /// in with a bounce above his head, floats gently while it shows, and fades away. Screen-space
    /// UI Toolkit, following his head every frame; kept on screen when he is near an edge.
    /// Presentation only.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public sealed class CandySpeechBubble : MonoBehaviour
    {
        public Camera viewCamera;
        [Tooltip("Who is speaking (the bubble sits above the top of this sprite).")]
        public SpriteRenderer speaker;
        public float aboveHead = 0.12f;
        public float maxWidth = 360f;

        /// <summary>Set by the house while the chalkboard is open, so the bubble never covers the board.</summary>
        public static bool Muted;

        static readonly Color Rim = Color.white;
        static readonly Color Ink = new Color32(214, 51, 108, 255);         // candy pink outline
        static readonly Color[] Sprinkles =
        {
            new Color32(255, 111, 168, 255), new Color32(120, 200, 255, 255), new Color32(255, 217, 90, 255),
            new Color32(150, 230, 160, 255), new Color32(190, 150, 255, 255),
        };

        VisualElement root, holder, bubble, tail;
        Label label;
        float hideAt = -1f, shownAt;
        bool visible;

        void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            root.Clear();
            root.pickingMode = PickingMode.Ignore;
            root.style.position = Position.Absolute;
            root.style.left = 0; root.style.right = 0; root.style.top = 0; root.style.bottom = 0;

            holder = new VisualElement { pickingMode = PickingMode.Ignore };
            holder.style.position = Position.Absolute;
            holder.style.alignItems = Align.Center;
            holder.style.translate = new Translate(Length.Percent(-50), Length.Percent(-100));
            holder.style.opacity = 0f;
            holder.style.scale = new Scale(new Vector3(0.6f, 0.6f, 1f));
            holder.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(100));
            AuthCardLayout.TransitionMany(holder, 0.22f, "opacity", "scale");
            root.Add(holder);

            bubble = new VisualElement { pickingMode = PickingMode.Ignore };
            bubble.style.maxWidth = maxWidth;
            bubble.style.paddingLeft = 22; bubble.style.paddingRight = 22; bubble.style.paddingTop = 12; bubble.style.paddingBottom = 14;
            bubble.style.backgroundImage = Background.FromTexture2D(CandyGradient());
            SetRadius(bubble, 26f);
            SetBorder(bubble, Rim, 3f);
            holder.Add(bubble);

            label = new Label { pickingMode = PickingMode.Ignore };
            label.style.fontSize = 21;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.color = Color.white;
            label.style.whiteSpace = WhiteSpace.Normal;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            label.style.unityTextOutlineColor = Ink;
            label.style.unityTextOutlineWidth = 1.2f;
            label.style.textShadow = new TextShadow { offset = new Vector2(0f, 2f), blurRadius = 2f, color = new Color(0.55f, 0.1f, 0.3f, 0.45f) };
            bubble.Add(label);

            // confetti sprinkles round the rim
            var spots = new[] { (-8f, 10f, 0), (96f, -6f, 1), (14f, 88f, 2), (-10f, 70f, 3), (102f, 70f, 4), (50f, -9f, 0) };
            foreach (var (x, y, c) in spots)
            {
                var s = new VisualElement { pickingMode = PickingMode.Ignore };
                s.style.position = Position.Absolute;
                s.style.left = Length.Percent(x); s.style.top = Length.Percent(y);
                s.style.width = 10; s.style.height = 10;
                s.style.backgroundColor = Sprinkles[c];
                SetRadius(s, 3f);
                SetBorder(s, Rim, 1.5f);
                s.style.rotate = new Rotate(new Angle(30f + 25f * c));
                bubble.Add(s);
            }

            tail = new VisualElement { pickingMode = PickingMode.Ignore };
            tail.style.width = 22; tail.style.height = 22;
            tail.style.marginTop = -13;
            tail.style.backgroundColor = (Color)new Color32(255, 170, 120, 255);
            tail.style.rotate = new Rotate(new Angle(45f));
            tail.style.borderRightWidth = 3; tail.style.borderBottomWidth = 3;
            tail.style.borderRightColor = Rim; tail.style.borderBottomColor = Rim;
            holder.Add(tail);
            visible = false;
        }

        /// <summary>Say something for a few seconds (pops in, floats, fades out).</summary>
        public void Show(string text, float seconds = 3.2f)
        {
            if (label == null || string.IsNullOrEmpty(text)) return;
            label.text = text;
            hideAt = Time.time + seconds;
            shownAt = Time.time;
            visible = true;
            holder.style.opacity = 1f;
            holder.style.scale = new Scale(new Vector3(1.1f, 1.1f, 1f));                    // overshoot ...
            holder.schedule.Execute(() => holder.style.scale = new Scale(Vector3.one)).StartingIn(220);   // ... and settle
        }

        public void Hide()
        {
            visible = false;
            if (holder == null) return;
            holder.style.opacity = 0f;
            holder.style.scale = new Scale(new Vector3(0.8f, 0.8f, 1f));
        }

        void LateUpdate()
        {
            if (holder == null || speaker == null || viewCamera == null) return;
            if (visible && (Time.time >= hideAt || Muted)) Hide();
            if (!visible) return;

            var b = speaker.bounds;
            var world = new Vector3(b.center.x, b.max.y + aboveHead, b.center.z);
            if (Vector3.Dot(world - viewCamera.transform.position, viewCamera.transform.forward) <= 0f) { holder.style.opacity = 0f; return; }
            var p = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, world, viewCamera);
            var size = root.layout.size;
            float halfW = Mathf.Max(80f, bubble.layout.width * 0.5f), h = Mathf.Max(60f, holder.layout.height);
            if (size.x > 1f)
            {
                p.x = Mathf.Clamp(p.x, halfW + 12f, size.x - halfW - 12f);
                p.y = Mathf.Clamp(p.y, h + 12f, size.y - 12f);
            }
            float bob = Mathf.Sin((Time.time - shownAt) * 2.6f) * 4f;                         // a gentle float
            holder.style.left = p.x; holder.style.top = p.y + bob;
            holder.style.opacity = 1f;
        }

        /// <summary>Bubblegum pink → peach → lemon, with a soft light band across the top.</summary>
        static Texture2D CandyGradient()
        {
            const int w = 128, h = 32;
            var t = new Texture2D(w, h, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
            Color a = new Color32(255, 111, 168, 255), m = new Color32(255, 163, 108, 255), z = new Color32(255, 217, 90, 255);
            for (int y = 0; y < h; y++)
            {
                float shine = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(h * 0.45f, h, y)) * 0.28f;   // texture rows run bottom-up
                for (int x = 0; x < w; x++)
                {
                    float k = x / (w - 1f);
                    var c = k < 0.5f ? Color.Lerp(a, m, k * 2f) : Color.Lerp(m, z, (k - 0.5f) * 2f);
                    t.SetPixel(x, y, Color.Lerp(c, Color.white, shine));
                }
            }
            t.Apply();
            return t;
        }

        static void SetRadius(VisualElement e, float r)
        {
            e.style.borderTopLeftRadius = r; e.style.borderTopRightRadius = r;
            e.style.borderBottomLeftRadius = r; e.style.borderBottomRightRadius = r;
        }

        static void SetBorder(VisualElement e, Color c, float w)
        {
            e.style.borderTopColor = c; e.style.borderBottomColor = c; e.style.borderLeftColor = c; e.style.borderRightColor = c;
            e.style.borderTopWidth = w; e.style.borderBottomWidth = w; e.style.borderLeftWidth = w; e.style.borderRightWidth = w;
        }
    }
}
