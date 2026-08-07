// ===========================================================================
//  WalkCompass — a four-arrow pad, bottom right, that drives the existing walk
// ===========================================================================
//  Hold an arrow and the reader moves; let go and they stop. That is the whole
//  contract.
//
//  IT DOES NOT MOVE ANYTHING ITSELF. The walk already exists — PathWalker runs
//  the reader along the road, and everything downstream (the stops at each word,
//  onArrive, the camera) is built on that one number. So this only ever sets
//  PathWalker.driveForward / driveTurn, exactly as the arrow keys already do. A
//  second movement system would have meant a second set of bugs, and the reader
//  could have walked off the path.
//
//     UP / DOWN     forward and back
//     LEFT / RIGHT  turn
//
//  WHAT THOSE MEAN IS THE WALKER'S BUSINESS, NOT THIS FILE'S, and it is not the
//  same in both places. Out on the river the reader is a distance along a line:
//  up and down move along it, and left and right turn ON THE SPOT, because
//  steering could only ever fight the path. On the library floor there is no path
//  to fight — up walks the way they are facing and left and right steer, which is
//  what walking around a room is. Same pad, same two numbers; see PathWalker.Roam.
//
//  The UI is built in code so it needs no prefab, no scene asset and no
//  PanelSettings — drop the component on anything and it works.
// ===========================================================================
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class WalkCompass : MonoBehaviour
{
    [Header("Wiring")]
    [Tooltip("The reader this drives. Left empty, it finds the one in the scene.")]
    public PathWalker walker;

    [Header("Placement")]
    [Tooltip("Distance from the bottom-right corner of the screen, in pixels.")]
    public Vector2 margin = new Vector2(90f, 110f);
    [Tooltip("Size of one arrow button, in pixels. Big enough for a thumb.")]
    public float buttonSize = 78f;
    [Tooltip("Gap between the arrows, in pixels.")]
    public float spread = 6f;

    [Header("Look")]
    public Color idleColor = new Color(1f, 1f, 1f, 0.30f);
    public Color heldColor = new Color(1f, 0.85f, 0.45f, 0.85f);
    public Color glyphColor = new Color(0.15f, 0.12f, 0.09f, 0.95f);

    /// <summary>What the pad is asking for right now. -1..1 each.</summary>
    public float Forward { get; private set; }
    public float Turn { get; private set; }

    Arrow _up, _down, _left, _right;

    void Awake()
    {
        if (walker == null) walker = FindAnyObjectByType<PathWalker>();
        Build();
    }

    void Update()
    {
        // Held, not clicked: the reader moves while a finger is down and stops the
        // moment it lifts.
        Forward = (_up != null && _up.Held ? 1f : 0f) - (_down != null && _down.Held ? 1f : 0f);
        Turn = (_right != null && _right.Held ? 1f : 0f) - (_left != null && _left.Held ? 1f : 0f);

        if (walker == null) return;
        walker.driveForward = Forward;
        walker.driveTurn = Turn;
    }

    void OnDisable()
    {
        // Never leave the reader walking because the pad went away mid-press.
        if (walker != null) { walker.driveForward = 0f; walker.driveTurn = 0f; }
        Forward = Turn = 0f;
    }

    // ── the pad ─────────────────────────────────────────────────────────────

    void Build()
    {
        var canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500;                 // over the game, under nothing

        var scaler = GetComponent<CanvasScaler>() ?? gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(720f, 1520f);   // the phone it is for
        scaler.matchWidthOrHeight = 0.5f;
        if (GetComponent<GraphicRaycaster>() == null) gameObject.AddComponent<GraphicRaycaster>();

        // Taps have to reach the UI at all.
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem", typeof(EventSystem));
            es.transform.SetParent(transform.parent, false);
#if ENABLE_INPUT_SYSTEM
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
            es.AddComponent<StandaloneInputModule>();
#endif
        }

        var root = new GameObject("Compass", typeof(RectTransform)).GetComponent<RectTransform>();
        root.SetParent(transform, false);
        root.anchorMin = root.anchorMax = new Vector2(1f, 0f);      // bottom right
        root.pivot = new Vector2(0.5f, 0.5f);
        root.anchoredPosition = new Vector2(-margin.x, margin.y);
        root.sizeDelta = new Vector2(buttonSize * 3f, buttonSize * 3f);

        float step = buttonSize + spread;
        _up = Arrow.Make(root, "Up", "▲", new Vector2(0f, step), this);
        _down = Arrow.Make(root, "Down", "▼", new Vector2(0f, -step), this);
        _left = Arrow.Make(root, "Left", "◀", new Vector2(-step, 0f), this);
        _right = Arrow.Make(root, "Right", "▶", new Vector2(step, 0f), this);
    }

    /// <summary>
    /// One arrow. Reports whether it is being HELD rather than firing a click —
    /// Button.onClick is a press-and-release, and this has to move the reader for
    /// as long as a finger stays down.
    /// </summary>
    class Arrow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public bool Held { get; private set; }
        Image _bg;
        WalkCompass _owner;

        public static Arrow Make(RectTransform parent, string name, string glyph,
                                 Vector2 offset, WalkCompass owner)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchoredPosition = offset;
            rt.sizeDelta = Vector2.one * owner.buttonSize;

            var bg = go.GetComponent<Image>();
            bg.color = owner.idleColor;
            bg.raycastTarget = true;

            var label = new GameObject("Glyph", typeof(RectTransform), typeof(Text));
            var lrt = label.GetComponent<RectTransform>();
            lrt.SetParent(rt, false);
            lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
            lrt.offsetMin = lrt.offsetMax = Vector2.zero;

            var text = label.GetComponent<Text>();
            text.text = glyph;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = owner.glyphColor;
            text.raycastTarget = false;                       // never eat the press
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = Mathf.RoundToInt(owner.buttonSize * 0.5f);

            var a = go.AddComponent<Arrow>();
            a._bg = bg;
            a._owner = owner;
            return a;
        }

        public void OnPointerDown(PointerEventData e) { Held = true; Tint(); }
        public void OnPointerUp(PointerEventData e) { Held = false; Tint(); }

        // A finger that slides off the button has stopped asking. Without this the
        // reader keeps walking after the press has visually ended.
        public void OnPointerExit(PointerEventData e) { Held = false; Tint(); }

        void OnDisable() { Held = false; }

        void Tint()
        {
            if (_bg != null && _owner != null)
                _bg.color = Held ? _owner.heldColor : _owner.idleColor;
        }
    }
}
