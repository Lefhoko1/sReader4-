using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// Presents one of the app's EXISTING auth pages (LoginReal.uxml /
    /// RegisterReal.uxml, still driven by their unchanged controllers) as a
    /// frosted-glass card on the left of the landscape screen, while the camera
    /// frames the house on the right.
    ///
    /// Systematic layout: the page is laid out at its own phone design size
    /// (<see cref="designWidth"/> × <see cref="designHeight"/>, the shape it was
    /// built for) and the whole card is then scaled UNIFORMLY to fit the screen
    /// height. Its layout is therefore exactly the designed one on every screen —
    /// nothing squashes or overlaps — and nothing is ever stretched.
    ///
    /// Look: the page's own dark slate (rgb 18,20,25) made translucent over a
    /// blurred copy of the scene, with a thin parchment hairline. Motion: fades and
    /// slides in from the left every time it opens. Only placement, the page
    /// background and the phone status-bar padding are styled here — the UXML files
    /// and their logic are untouched.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public sealed class AuthCardLayout : MonoBehaviour
    {
        public float designWidth = 400f;
        public float designHeight = 780f;
        [Tooltip("Gap above and below the card, as a fraction of screen height.")]
        [Range(0f, 0.2f)] public float verticalMargin = 0.04f;
        [Tooltip("Gap from the left edge, as a fraction of screen width.")]
        [Range(0f, 0.2f)] public float leftMargin = 0.06f;
        [Range(0f, 1f)] public float glassOpacity = 0.66f;
        [Tooltip("Top padding of the page header inside the card (the page's own 52px is for a phone status bar).")]
        public float headerTopPadding = 18f;

        static readonly Color Slate = new Color32(18, 20, 25, 255);
        static readonly Color Hairline = new Color(233f / 255f, 223f / 255f, 200f / 255f, 0.30f);

        VisualElement root, frost;
        float scale = 1f;

        void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            if (root == null) return;

            // Card at the page's own design size; scaled uniformly from its top-left corner.
            root.style.position = Position.Absolute;
            root.style.width = designWidth;
            root.style.height = designHeight;
            root.style.right = StyleKeyword.Auto;
            root.style.bottom = StyleKeyword.Auto;
            root.style.transformOrigin = new TransformOrigin(0, 0);
            SetRadius(root, 24f);
            SetBorder(root, Hairline, 1f);
            root.style.overflow = Overflow.Hidden;
            root.style.backgroundColor = Color.clear;

            var page = root.Q("page-root");
            if (page != null)
            {
                var c = Slate; c.a = glassOpacity;
                page.style.backgroundColor = c;
            }
            var header = root.Q("header");
            if (header != null) header.style.paddingTop = headerTopPadding;

            // Frosted layer behind the page: the blurred scene, lined up with the screen.
            frost = new VisualElement { name = "frosted-backdrop", pickingMode = PickingMode.Ignore };
            frost.style.position = Position.Absolute;
            frost.style.opacity = 0f;
            Transition(frost, "opacity", 0.4f);
            root.Insert(0, frost);

            root.RegisterCallback<AttachToPanelEvent>(OnAttach);
            if (root.parent != null) Hook(root.parent);
            FrostedBackdrop.Changed += Refresh;
            Refresh();

            // Entrance: fade + slide from the left (eased).
            root.style.opacity = 0f;
            root.style.translate = new Translate(-40f, 0f);
            TransitionMany(root, 0.4f, "opacity", "translate");
            root.schedule.Execute(() =>
            {
                root.style.opacity = 1f;
                root.style.translate = new Translate(0f, 0f);
            }).StartingIn(30);
        }

        void OnDisable()
        {
            FrostedBackdrop.Changed -= Refresh;
            if (root?.parent != null) root.parent.UnregisterCallback<GeometryChangedEvent>(OnScreenGeometry);
        }

        void OnAttach(AttachToPanelEvent _) { if (root.parent != null) Hook(root.parent); }

        void Hook(VisualElement screen)
        {
            screen.UnregisterCallback<GeometryChangedEvent>(OnScreenGeometry);
            screen.RegisterCallback<GeometryChangedEvent>(OnScreenGeometry);
            Fit();
        }

        void OnScreenGeometry(GeometryChangedEvent _) => Fit();

        /// <summary>Uniform scale so the designed page fits the screen height; position on the left.</summary>
        void Fit()
        {
            var screen = root?.parent;
            if (screen == null || screen.layout.height <= 1f) return;
            float h = screen.layout.height, w = screen.layout.width;
            scale = Mathf.Min(h * (1f - 2f * verticalMargin) / designHeight, w * 0.45f / designWidth);
            root.style.scale = new Scale(new Vector3(scale, scale, 1f));
            root.style.left = w * leftMargin;
            root.style.top = (h - designHeight * scale) * 0.5f;
            PlaceFrost();
        }

        void Refresh()
        {
            if (frost == null) return;
            var tex = FrostedBackdrop.Texture;
            if (tex == null) { frost.style.opacity = 0f; return; }
            frost.style.backgroundImage = Background.FromRenderTexture(tex);
            PlaceFrost();
            frost.style.opacity = 1f;
        }

        /// <summary>The blurred image covers the whole screen, offset (in the card's scaled space) to line up behind the card.</summary>
        void PlaceFrost()
        {
            var screen = root?.parent;
            if (frost == null || screen == null || scale <= 0f) return;
            float left = root.resolvedStyle.left, top = root.resolvedStyle.top;
            frost.style.left = -left / scale;
            frost.style.top = -top / scale;
            frost.style.width = screen.layout.width / scale;
            frost.style.height = screen.layout.height / scale;
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

        internal static void Transition(VisualElement e, string property, float seconds) => TransitionMany(e, seconds, property);

        internal static void TransitionMany(VisualElement e, float seconds, params string[] properties)
        {
            var names = new System.Collections.Generic.List<StylePropertyName>();
            var durations = new System.Collections.Generic.List<TimeValue>();
            var easings = new System.Collections.Generic.List<EasingFunction>();
            foreach (var p in properties)
            {
                names.Add(new StylePropertyName(p));
                durations.Add(new TimeValue(seconds, TimeUnit.Second));
                easings.Add(new EasingFunction(EasingMode.EaseOutCubic));
            }
            e.style.transitionProperty = names;
            e.style.transitionDuration = durations;
            e.style.transitionTimingFunction = easings;
        }
    }
}
