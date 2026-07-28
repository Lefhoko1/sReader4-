// ===========================================================================
//  StoryBook.cs — the candy storybook: pages, clickable sentences, and the
//  sentence-to-stones flight with sparkle FX
// ===========================================================================
//  DROP-IN: put this on the SM_StoryBook prefab in your scene. Press Play.
//
//  WHAT IT DOES
//   • Lays the title + paragraph on the book's pages, one TextMeshPro per
//     sentence, sized and oriented from the MODEL (not guessed constants).
//   • Hovering a sentence lifts and warms it. Clicking it:
//        1. the sentence flashes and bursts candy sparkles along its length
//        2. every word detaches as a glowing 3D chip
//        3. the chips arc through the air on a curved path, trailing sparkles
//        4. each lands on its stone with a pop + ring flash
//   • Page turn: < > buttons at the FBX sockets, with the FlipPage child
//     rotating around the spine.
//
//  TARGET STONES
//   Assign `stonePath` = the WordPath_River object (its children are stones,
//   in order). Leave empty and it finds the first WordPathBuilder in the scene.
//
//  Requires TextMeshPro. Works with both input backends.
// ===========================================================================
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class StoryBook : MonoBehaviour
{
    // ------------------------------------------------------------ CONTENT
    [Header("Content")]
    public string title = "The Island of the Great Library";
    [TextArea(4, 10)]
    public string paragraph =
        "The fox lived near a quiet river. " +
        "Every morning he went out to explore. " +
        "He loved to find new things. " +
        "His friends would join his adventures. " +
        "Together they had so much fun.";

    [Tooltip("ONE sentence across the whole spread, paged with the < > buttons. " +
             "The sentence gets the entire book, so the text is large and readable — " +
             "this is the recommended mode for a small on-screen book.")]
    public bool oneSentencePerSpread = true;

    [Tooltip("Sentences per page when One Sentence Per Spread is off. " +
             "FEWER = bigger text.")]
    public int linesPerPage = 3;

    [Header("Flight target")]
    [Tooltip("Object whose children are the word stones, in order. " +
             "Leave empty to auto-find WordPath_River / WordPathBuilder.")]
    public Transform stonePath;

    [Header("Page layout (auto-fitted to the model)")]
    [Tooltip("How much of each page's depth the rows spread over.")]
    [Range(0.4f, 1f)] public float pageFill = 0.82f;
    [Tooltip("Row width as a fraction of a page's half-width.")]
    [Range(0.5f, 1f)] public float columnFill = 0.90f;
    [Tooltip("Row height as a fraction of the row spacing (higher = bigger text).")]
    [Range(0.5f, 1.5f)] public float lineHeight = 0.92f;
    [Tooltip("Lift the text above the page, in the book's LOCAL units. 0.05 suits SM_StoryBook.")]
    public float textLift = 0.05f;
    [Tooltip("Extra world-space nudge off the page (z-fighting insurance).")]
    public float pageLift = 0f;
    [Tooltip("Tick if the text reads mirrored/upside-down on your book.")]
    public bool flipText;

    [Header("Look — candy palette")]
    public Color inkColor      = new Color(0.30f, 0.22f, 0.35f);
    public Color hoverColor    = new Color(0.85f, 0.35f, 0.60f);
    public Color doneColor     = new Color(0.62f, 0.60f, 0.70f);
    public Color titleColor    = new Color(0.55f, 0.35f, 0.85f);
    public Color chipColor     = new Color(1.00f, 0.98f, 0.92f);
    public Color[] sparkleColors = {
        new Color(1.00f, 0.45f, 0.70f),   // bubblegum
        new Color(0.55f, 0.85f, 1.00f),   // sky candy
        new Color(1.00f, 0.82f, 0.35f),   // gold
        new Color(0.50f, 0.95f, 0.80f),   // mint
        new Color(0.75f, 0.55f, 1.00f),   // grape
    };

    [Header("Timing")]
    public float chipStagger = 0.09f;
    public float chipFlightTime = 0.95f;
    public float arcHeight = 0.9f;

    [System.Serializable] public class SentenceEvent : UnityEvent<string, int> { }
    public SentenceEvent onSentenceSent = new SentenceEvent();

    // ------------------------------------------------------------ INTERNAL
    class Line { public TextMeshPro tmp; public string text; public bool sent;
                 public Vector3 home; public BoxCollider col; }
    readonly List<Line> _lines = new List<Line>();
    List<string> _sentences = new List<string>();
    int _page;
    Transform _flip;
    Line _hover;
    bool _busy;

    static Texture2D _star, _dot, _ring;
    static Material _fxMat;

    // measured from the model so text always fits, whatever the book's scale
    Transform _textRoot;
    float _bookWidth = 1f, _rowGap = 0.1f;
    // the page rectangle, in the book's own local space
    Vector3 _pageLocalSize = Vector3.one;   // mesh local size
    float _colLocalX = 0.2f;                // |x| of a page column's centre
    float _pageLocalY = 0.02f;              // the page surface height

    // ==================================================================== //
    void Start()
    {
        EnsureFX();
        _flip = FindChild("SM_StoryBook_FlipPage");
        if (_flip != null) _flip.gameObject.SetActive(false);
        if (stonePath == null) AutoFindStones();
        MeasureBook();
        SplitSentences();
        BuildTitle();
        BuildPageButtons();
        ShowPage(0);
    }

    Transform FindChild(string n) =>
        GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == n);

    void AutoFindStones()
    {
        var go = GameObject.Find("WordPath_River");
        if (go != null) { stonePath = go.transform; return; }
        var b = FindAnyObjectByType<WordPathBuilder>();
        if (b != null) stonePath = b.transform;
    }

    // Work out the book's real size and the gap between line sockets, so text is
    // sized in WORLD units from the model itself instead of guessed constants.
    void MeasureBook()
    {
        // a clean, unrotated parent for all page text
        var root = FindChild("PageText");
        if (root == null)
        {
            var go = new GameObject("PageText");
            go.transform.SetParent(transform, false);
            root = go.transform;
        }
        _textRoot = root;
        _textRoot.localPosition = Vector3.zero;
        _textRoot.localRotation = Quaternion.identity;

        var rends = GetComponentsInChildren<Renderer>(true);
        if (rends.Length > 0)
        {
            var b = rends[0].bounds;
            foreach (var r in rends) b.Encapsulate(r.bounds);
            // width across the spread = extent along the book's own "right"
            _bookWidth = Vector3.Scale(b.size, Abs(transform.right)).magnitude;
            if (_bookWidth < 0.01f) _bookWidth = b.size.magnitude * 0.5f;
        }

        var l1 = FindChild("SOCKET_LineL_1");
        var l2 = FindChild("SOCKET_LineL_2");
        _rowGap = (l1 != null && l2 != null)
            ? Vector3.Distance(l1.position, l2.position)
            : _bookWidth * 0.12f;
        if (_rowGap < 0.001f) _rowGap = _bookWidth * 0.12f;

        // the page rectangle in local space: the mesh gives the true page size,
        // the sockets give where a column sits and how high the page surface is
        var mfl = GetComponentInChildren<MeshFilter>();
        _pageLocalSize = (mfl != null && mfl.sharedMesh != null)
            ? mfl.sharedMesh.bounds.size : Vector3.one;
        if (l1 != null)
        {
            var lp = transform.InverseTransformPoint(l1.position);
            _colLocalX = Mathf.Abs(lp.x);
            _pageLocalY = lp.y;
        }
        else
        {
            _colLocalX = _pageLocalSize.x * 0.25f;
            _pageLocalY = _pageLocalSize.y * 0.5f;
        }
    }

    static Vector3 Abs(Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

    /// <summary>World position of row i of n on the left/right page.</summary>
    Vector3 RowPosition(int i, int n, bool rightPage)
    {
        float halfDepth = _pageLocalSize.z * 0.5f * pageFill;
        float z = n <= 1 ? 0f : Mathf.Lerp(halfDepth, -halfDepth, i / (float)(n - 1));
        float x = rightPage ? -_colLocalX : _colLocalX;
        return transform.TransformPoint(new Vector3(x, _pageLocalY + textLift, z));
    }

    /// <summary>Centre of the whole spread — the single-sentence layout.</summary>
    Vector3 SpreadCentre() =>
        transform.TransformPoint(new Vector3(0f, _pageLocalY + textLift, 0f));

    /// <summary>World size of the full spread text area.</summary>
    Vector2 SpreadSize()
    {
        float s = transform.lossyScale.x;
        return new Vector2(_pageLocalSize.x * columnFill * s,
                           _pageLocalSize.z * pageFill * s);
    }

    /// <summary>World-space row size (width, height) for the current layout.</summary>
    Vector2 RowSize(int n)
    {
        float s = transform.lossyScale.x;
        float w = _colLocalX * 2f * columnFill * s;                       // the column
        float gap = (n <= 1 ? _pageLocalSize.z * pageFill
                            : _pageLocalSize.z * pageFill / (n - 1)) * s; // row spacing
        return new Vector2(w, gap * lineHeight);
    }

    // The page plane's normal is the book's up; text reads "up the page" along
    // the book's forward. Derived from the model, so any book rotation works.
    Quaternion TextRotation()
    {
        Vector3 normal = transform.up;          // the page plane's normal
        Vector3 up = transform.forward;         // "up the page"

        // TMP is single-sided: it is only visible when its forward points AWAY
        // from the viewer. Pick the page side the camera is actually on, and the
        // in-plane direction that reads upright on screen.
        var cam = Camera.main;
        if (cam != null)
        {
            if (Vector3.Dot(normal, cam.transform.position - transform.position) < 0f) normal = -normal;
            if (Vector3.Dot(up, cam.transform.up) < 0f) up = -up;
        }
        if (flipText) up = -up;
        return Quaternion.LookRotation(-normal, up);
    }

    // Build a TMP that AUTO-FITS a rect measured in world units — no font-size
    // guesswork, so it can never spill off the page.
    TextMeshPro MakeText(string name, Vector3 worldPos, float width, float height,
                         Color color, TextAlignmentOptions align)
    {
        var go = new GameObject(name);
        go.transform.SetParent(_textRoot, true);
        go.transform.position = worldPos + transform.up * pageLift;
        go.transform.rotation = TextRotation();
        go.transform.localScale = Vector3.one;

        var t = go.AddComponent<TextMeshPro>();
        t.color = color;
        t.alignment = align;
        t.textWrappingMode = TextWrappingModes.Normal;
        t.enableAutoSizing = true;                 // fit the rect, whatever the scale
        t.fontSizeMin = 0.02f;
        t.fontSizeMax = 12f;
        t.rectTransform.sizeDelta = new Vector2(width, height);
        t.margin = Vector4.zero;
        return t;
    }

    void SplitSentences()
    {
        _sentences = paragraph
            .Replace("\n", " ")
            .Split('.')
            .Select(s => s.Trim())
            .Where(s => s.Length > 0)
            .Select(s => s + ".")
            .ToList();
    }

    // ------------------------------------------------------------- LAYOUT
    void BuildTitle()
    {
        var socket = FindChild("SOCKET_Title");
        Vector3 pos = socket != null ? socket.position : transform.position;
        float s = transform.lossyScale.x;
        var t = MakeText("BookTitle", pos, _colLocalX * 2f * s, _pageLocalSize.z * 0.13f * s,
                         titleColor, TextAlignmentOptions.Center);
        t.text = title;
        t.fontStyle = FontStyles.Bold;
    }

    void ShowPage(int page)
    {
        foreach (var l in _lines) if (l.tmp) Destroy(l.tmp.gameObject);
        _lines.Clear();
        _page = Mathf.Max(0, page);

        if (oneSentencePerSpread)
        {
            if (_page < _sentences.Count)
            {
                var size = SpreadSize();
                var t = MakeText("Sentence", SpreadCentre(), size.x, size.y,
                                 inkColor, TextAlignmentOptions.Center);
                t.text = _sentences[_page];
                var col = t.gameObject.AddComponent<BoxCollider>();
                col.size = new Vector3(size.x, size.y, 0.01f);
                col.isTrigger = true;
                _lines.Add(new Line { tmp = t, text = _sentences[_page], col = col,
                                      home = t.transform.localPosition });
            }
            return;
        }

        int perSpread = linesPerPage * 2;
        int first = _page * perSpread;
        for (int i = 0; i < perSpread; i++)
        {
            int idx = first + i;
            if (idx >= _sentences.Count) break;
            bool right = i >= linesPerPage;
            int slot = i % linesPerPage;
            MakeLine(_sentences[idx], RowPosition(slot, linesPerPage, right), linesPerPage);
        }
    }

    void MakeLine(string text, Vector3 worldPos, int rows)
    {
        var size = RowSize(rows);
        float w = size.x, h = size.y;

        var t = MakeText("Sentence", worldPos, w, h,
                         inkColor, TextAlignmentOptions.Left);
        t.text = text;

        // collider in the text's own space, so it matches the rect exactly
        var col = t.gameObject.AddComponent<BoxCollider>();
        col.size = new Vector3(w, h, 0.01f);
        col.isTrigger = true;

        _lines.Add(new Line { tmp = t, text = text, col = col,
                              home = t.transform.localPosition });
    }

    void BuildPageButtons()
    {
        // plain ASCII — the arrow glyphs aren't in LiberationSans SDF
        MakeButton("SOCKET_NextPage", ">", () => TurnPage(+1));
        MakeButton("SOCKET_PrevPage", "<", () => TurnPage(-1));
    }

    void MakeButton(string socketName, string glyph, System.Action act)
    {
        var s = FindChild(socketName); if (s == null) return;
        var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = "PageBtn_" + socketName;
        go.transform.SetParent(s, false);
        go.transform.localRotation = Quaternion.Euler(90, 0, 0);
        go.transform.localScale = Vector3.one * 0.09f;
        var r = go.GetComponent<Renderer>();
        var m = new Material(Shader.Find("Sprites/Default"));
        m.mainTexture = _ring;
        m.color = new Color(1f, 0.82f, 0.35f, 0.95f);
        r.material = m;
        var lbl = new GameObject("g").AddComponent<TextMeshPro>();
        lbl.transform.SetParent(go.transform, false);
        lbl.transform.localPosition = new Vector3(0, 0, -0.01f);
        lbl.transform.localScale = Vector3.one * 0.9f;
        lbl.text = glyph; lbl.fontSize = 3.4f;
        lbl.color = new Color(0.5f, 0.3f, 0.1f);
        lbl.alignment = TextAlignmentOptions.Center;
        var btn = go.AddComponent<BookButton>();
        btn.action = act;
    }

    class BookButton : MonoBehaviour { public System.Action action; }

    // -------------------------------------------------------------- INPUT
    void Update()
    {
        var cam = Camera.main; if (cam == null) return;
        Vector2 p = Pointer();
        Ray ray = cam.ScreenPointToRay(p);

        // hover
        Line found = null;
        if (Physics.Raycast(ray, out var hit, 100f))
        {
            found = _lines.FirstOrDefault(l => l.col == hit.collider);
            var btn = hit.collider.GetComponent<BookButton>();
            if (btn != null && Pressed()) { btn.action(); return; }
        }
        if (found != _hover)
        {
            if (_hover != null && !_hover.sent) Style(_hover, false);
            _hover = found;
            if (_hover != null && !_hover.sent) Style(_hover, true);
        }
        if (_hover != null && !_hover.sent && !_busy && Pressed())
            StartCoroutine(SendSentence(_hover));
    }

    void Style(Line l, bool hot)
    {
        if (l.tmp == null) return;
        l.tmp.color = hot ? hoverColor : inkColor;
        l.tmp.fontStyle = hot ? FontStyles.Bold : FontStyles.Normal;
        // lift OFF the page (along the book's normal), not sideways in book space
        l.tmp.transform.localPosition = l.home +
            (hot ? transform.InverseTransformVector(transform.up * _rowGap * 0.12f) : Vector3.zero);
    }

    static bool Pressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetMouseButtonDown(0)) return true;
        if (Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Began) return true;
#endif
        return false;
    }

    static Vector2 Pointer()
    {
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.isPressed)
            return Touchscreen.current.primaryTouch.position.ReadValue();
        if (Mouse.current != null) return Mouse.current.position.ReadValue();
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.mousePosition;
#else
        return new Vector2(Screen.width / 2f, Screen.height / 2f);
#endif
    }

    // ======================================================== THE MOMENT ==
    IEnumerator SendSentence(Line line)
    {
        _busy = true;
        line.sent = true;

        // 1. flash + sparkle sweep along the sentence
        Vector3 lineCentre = line.tmp.transform.position;
        Burst(lineCentre, 18, 0.22f, 0.9f);
        yield return StartCoroutine(FlashLine(line));

        // 2. gather targets
        var stones = new List<Transform>();
        if (stonePath != null)
            for (int i = 0; i < stonePath.childCount; i++)
                stones.Add(stonePath.GetChild(i));

        var words = line.text.TrimEnd('.').Split(' ')
                        .Where(w => w.Length > 0).ToArray();

        // 3. launch chips
        for (int i = 0; i < words.Length; i++)
        {
            Transform target = i < stones.Count ? stones[i] : null;
            Vector3 from = line.tmp.transform.position
                           + Vector3.up * 0.02f
                           + line.tmp.transform.right * (i * 0.01f);
            StartCoroutine(FlyChip(words[i], from, target,
                                   i == words.Length - 1));
            yield return new WaitForSeconds(chipStagger);
        }

        yield return new WaitForSeconds(chipFlightTime + 0.3f);
        line.tmp.color = doneColor;
        line.tmp.fontStyle = FontStyles.Italic;
        onSentenceSent.Invoke(line.text, _lines.IndexOf(line));
        _busy = false;
    }

    IEnumerator FlashLine(Line l)
    {
        for (float t = 0; t < 0.35f; t += Time.deltaTime)
        {
            float k = Mathf.Sin(t / 0.35f * Mathf.PI);
            l.tmp.color = Color.Lerp(hoverColor, Color.white, k);
            l.tmp.transform.localPosition = l.home +
                transform.InverseTransformVector(transform.up * _rowGap * (0.12f + 0.10f * k));
            yield return null;
        }
    }

    IEnumerator FlyChip(string word, Vector3 from, Transform target, bool isLast)
    {
        // the flying word chip
        var go = new GameObject("Chip_" + word);
        go.transform.position = from;
        var tmp = go.AddComponent<TextMeshPro>();
        tmp.text = word;
        tmp.fontSize = 1.1f;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = chipColor;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.outlineWidth = 0.25f;
        tmp.outlineColor = new Color32(255, 150, 210, 220);
        tmp.rectTransform.sizeDelta = new Vector2(1.2f, 0.4f);

        // sparkle trail attached to the chip
        var trail = MakeTrail(go.transform);

        Vector3 to = target != null
            ? target.position + Vector3.up * 0.22f
            : from + Camera.main.transform.up * 1.5f;
        Vector3 mid = (from + to) * 0.5f + Vector3.up * arcHeight;

        float dur = chipFlightTime;
        for (float t = 0; t < dur; t += Time.deltaTime)
        {
            float k = t / dur;
            float e = 1f - Mathf.Pow(1f - k, 2.2f);          // ease out
            // quadratic bezier arc
            Vector3 a = Vector3.Lerp(from, mid, e);
            Vector3 b = Vector3.Lerp(mid, to, e);
            go.transform.position = Vector3.Lerp(a, b, e);
            // face camera, spin a little, shrink into the stone
            if (Camera.main != null)
                go.transform.rotation = Quaternion.LookRotation(
                    go.transform.position - Camera.main.transform.position);
            go.transform.localScale = Vector3.one * Mathf.Lerp(1.15f, 0.55f, e);
            tmp.color = Color.Lerp(chipColor, new Color(1f, 0.85f, 0.5f), e);
            yield return null;
        }

        // 4. landing
        if (trail != null) trail.Stop();
        Burst(to, 14, 0.26f, 1.0f);
        RingFlash(to, target);
        if (target != null)
        {
            var ws = target.GetComponent<WordStone>();
            if (ws != null)
            {
                ws.word = word;
                var t2 = target.GetComponentInChildren<TextMeshPro>();
                if (t2 != null) t2.text = word + (isLast ? "." : "");
                StartCoroutine(StonePop(target));
            }
        }
        Destroy(go);
    }

    IEnumerator StonePop(Transform stone)
    {
        Vector3 s0 = stone.localScale;
        for (float t = 0; t < 0.32f; t += Time.deltaTime)
        {
            float k = Mathf.Sin(t / 0.32f * Mathf.PI);
            stone.localScale = s0 * (1f + 0.18f * k);
            yield return null;
        }
        stone.localScale = s0;
    }

    // ============================================================ FX =====
    void EnsureFX()
    {
        if (_star != null && _fxMat != null) return;

        // Five-point star with a hot core. The falloff is squared (not a hard
        // clamp) so the edges dissolve instead of stepping.
        _star = Tex(128, (dx, dy) => {
            float ang = Mathf.Atan2(dy, dx), r = Mathf.Sqrt(dx*dx+dy*dy);
            float st = 0.55f + 0.45f * Mathf.Cos(5f*(ang - Mathf.PI/2));
            float body = Mathf.Clamp01(1f - r / Mathf.Max(st * 0.85f, 0.05f));
            float core = Mathf.Clamp01(1f - r * 3.4f);
            return Mathf.Clamp01(body * body * 0.9f + core * core * 0.6f); });

        // Soft glow dot — the same read as the keyword burst's motes.
        _dot = Tex(128, (dx, dy) => {
            float r = Mathf.Sqrt(dx*dx+dy*dy);
            float g = Mathf.Clamp01(1f - r);
            return Mathf.Clamp01(g * g * 0.85f + Mathf.Clamp01(1f - r*2.6f) * 0.5f); });

        // Gaussian band instead of a clamped triangle: no visible ring edges.
        _ring = Tex(256, (dx, dy) => {
            float r = Mathf.Sqrt(dx*dx+dy*dy);
            float d = (r - 0.78f) / 0.085f;
            return Mathf.Exp(-0.5f * d * d); });

        var sh = FXShader();
        _fxMat = sh != null ? new Material(sh) : null;
        MakeAdditive(_fxMat);
    }

    // Prefer the project's URP additive shaders. The built-in particle shaders
    // are the last resort: under URP a freshly-created "Particles/Standard
    // Unlit" material is OPAQUE, which is what drew the hard squares.
    static Shader FXShader()
    {
        // explicit null checks, not ??: Unity objects have an overloaded ==
        string[] names = {
            "GreatLibrary/SparkleAdditive",
            "GreatLibrary/MoteAdditive",
            "Universal Render Pipeline/Particles/Unlit",
            "Particles/Standard Unlit",
            "Sprites/Default",
        };
        foreach (var n in names)
        {
            var sh = Shader.Find(n);
            if (sh != null) return sh;
        }
        Debug.LogWarning("[StoryBook] No sparkle shader found — FX will be flat.");
        return null;
    }

    /// <summary>Force soft additive blending, whichever shader we landed on.</summary>
    static void MakeAdditive(Material m)
    {
        if (m == null) return;
        if (m.HasProperty("_Surface"))  m.SetFloat("_Surface", 1f);   // URP: transparent
        if (m.HasProperty("_Blend"))    m.SetFloat("_Blend", 2f);     // URP: additive
        if (m.HasProperty("_Mode"))     m.SetFloat("_Mode", 4f);      // built-in: additive
        if (m.HasProperty("_SrcBlend"))
            m.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        if (m.HasProperty("_DstBlend"))
            m.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.One);
        if (m.HasProperty("_ZWrite"))   m.SetFloat("_ZWrite", 0f);
        if (m.HasProperty("_Cull"))     m.SetFloat("_Cull", 0f);
        m.DisableKeyword("_ALPHATEST_ON");
        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        m.EnableKeyword("_ALPHABLEND_ON");
        m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    /// <summary>A per-effect instance of the FX material, on one sprite.</summary>
    static Material FXMaterial(Texture2D tex, float intensity = 1.35f)
    {
        if (_fxMat == null) return null;
        var m = new Material(_fxMat);
        m.mainTexture = tex;
        if (m.HasProperty("_BaseMap"))   m.SetTexture("_BaseMap", tex);
        if (m.HasProperty("_Intensity")) m.SetFloat("_Intensity", intensity);
        MakeAdditive(m);
        return m;
    }

    // Supersampled + mip-mapped + clamped: the three things that stop a small
    // billboard from shimmering or showing its own edges.
    static Texture2D Tex(int s, System.Func<float,float,float> f)
    {
        const int SS = 3;                               // 3x3 samples per texel
        var t = new Texture2D(s, s, TextureFormat.RGBA32, true)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Trilinear,
            anisoLevel = 1,
        };
        var px = new Color[s * s];
        for (int y = 0; y < s; y++) for (int x = 0; x < s; x++)
        {
            float a = 0f;
            for (int sy = 0; sy < SS; sy++) for (int sx = 0; sx < SS; sx++)
            {
                float fx = x + (sx + 0.5f) / SS, fy = y + (sy + 0.5f) / SS;
                float dx = (fx - s/2f)/(s/2f), dy = (fy - s/2f)/(s/2f);
                a += Mathf.Clamp01(f(dx, dy));
            }
            px[y * s + x] = new Color(1, 1, 1, a / (SS * SS));
        }
        t.SetPixels(px);
        t.Apply(true, false);
        return t;
    }

    Color RandCandy() => sparkleColors[Random.Range(0, sparkleColors.Length)];

    ParticleSystem MakeTrail(Transform parent)
    {
        EnsureFX();
        var go = new GameObject("Trail");
        go.transform.SetParent(parent, false);
        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.40f, 0.65f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.02f, 0.10f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.030f, 0.075f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            RandCandy(), RandCandy());
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = -0.03f;
        main.maxParticles = 80;
        var em = ps.emission; em.rateOverTime = 48f;
        var shape = ps.shape; shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere; shape.radius = 0.045f;
        var col = ps.colorOverLifetime; col.enabled = true;
        col.color = FadeGradient(0.85f, 0.06f);
        var sol = ps.sizeOverLifetime; sol.enabled = true;
        sol.size = new ParticleSystem.MinMaxCurve(1f, SoftSizeCurve());
        SetupRenderer(go, _dot, 1.15f);
        return ps;
    }

    void Burst(Vector3 pos, int count, float size, float life)
    {
        EnsureFX();

        // Two layers, exactly like the keyword burst: crisp stars over a soft
        // glow. Both additive, so overlaps bloom instead of stacking squares.
        Layer(_star, size * 0.62f, life, count, 1.35f, 0.45f, 1.25f);
        Layer(_dot, size * 1.05f, life * 0.72f, Mathf.Max(4, count / 2),
              0.95f, 0.25f, 0.70f);

        void Layer(Texture2D tex, float sz, float lf, int n,
                   float intensity, float spdMin, float spdMax)
        {
            var go = new GameObject("Sparkle");
            go.transform.position = pos;
            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startLifetime = new ParticleSystem.MinMaxCurve(lf * 0.7f, lf);
            main.startSpeed = new ParticleSystem.MinMaxCurve(spdMin, spdMax);
            main.startSize = new ParticleSystem.MinMaxCurve(sz * 0.55f, sz);
            main.startColor = new ParticleSystem.MinMaxGradient(
                RandCandy(), RandCandy());
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.gravityModifier = -0.04f;               // drift up, like motes
            main.playOnAwake = false;
            main.maxParticles = 64;
            var em = ps.emission; em.enabled = false;
            var shape = ps.shape; shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = size * 0.9f;                  // spread along the line
            shape.radiusThickness = 1f;
            // ease out of the launch instead of flying off at constant speed
            var vel = ps.limitVelocityOverLifetime;
            vel.enabled = true; vel.dampen = 0.35f;
            vel.limit = new ParticleSystem.MinMaxCurve(spdMax * 0.5f);
            var col = ps.colorOverLifetime; col.enabled = true;
            col.color = FadeGradient(1f, 0.12f);
            var sol = ps.sizeOverLifetime; sol.enabled = true;
            sol.size = new ParticleSystem.MinMaxCurve(1f, SoftSizeCurve());
            var rot = ps.rotationOverLifetime; rot.enabled = true;
            rot.z = new ParticleSystem.MinMaxCurve(-1.8f, 1.8f);
            SetupRenderer(go, tex, intensity);
            ps.Emit(n);
            Destroy(go, lf + 1.2f);
        }
    }

    /// <summary>Fade in fast, fade out slowly — never pop on or off.</summary>
    static Gradient FadeGradient(float peak, float peakAt)
    {
        var g = new Gradient();
        g.SetKeys(new[]{ new GradientColorKey(Color.white,0),
                         new GradientColorKey(Color.white,1)},
                  new[]{ new GradientAlphaKey(0f, 0f),
                         new GradientAlphaKey(peak, peakAt),
                         new GradientAlphaKey(peak * 0.45f, 0.55f),
                         new GradientAlphaKey(0f, 1f)});
        return g;
    }

    /// <summary>Small pop, long smooth shrink to nothing.</summary>
    static AnimationCurve SoftSizeCurve()
    {
        var c = new AnimationCurve(
            new Keyframe(0f, 0.45f),
            new Keyframe(0.18f, 1f),
            new Keyframe(1f, 0f));
        for (int i = 0; i < c.length; i++)
            c.SmoothTangents(i, 0.5f);
        return c;
    }

    static void SetupRenderer(GameObject go, Texture2D tex, float intensity)
    {
        var r = go.GetComponent<ParticleSystemRenderer>();
        var m = FXMaterial(tex, intensity);
        if (m != null) r.material = m;
        r.renderMode = ParticleSystemRenderMode.Billboard;
        r.alignment = ParticleSystemRenderSpace.View;
        r.sortMode = ParticleSystemSortMode.None;      // additive: order is free
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        r.receiveShadows = false;
        r.minParticleSize = 0f;
        r.maxParticleSize = 0.35f;                     // no screen-filling quads
    }

    void RingFlash(Vector3 pos, Transform face)
    {
        EnsureFX();
        var q = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(q.GetComponent<Collider>());
        q.transform.position = pos;
        q.transform.rotation = Quaternion.Euler(90, 0, 0);
        var r = q.GetComponent<Renderer>();
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        r.receiveShadows = false;
        var rm = FXMaterial(_ring, 1.1f);
        if (rm != null) r.material = rm;
        StartCoroutine(RingGrow(q.transform, r));
    }

    IEnumerator RingGrow(Transform tr, Renderer r)
    {
        var m = r.material;
        string prop = m.HasProperty("_BaseColor") ? "_BaseColor"
                    : m.HasProperty("_Color")     ? "_Color" : null;
        var gold = new Color(1f, 0.85f, 0.45f, 1f);
        if (prop != null) m.SetColor(prop, gold);
        for (float t = 0; t < 0.55f; t += Time.deltaTime)
        {
            float k = t / 0.55f;
            tr.localScale = Vector3.one * Mathf.Lerp(0.15f, 1.3f,
                                                     1 - (1-k)*(1-k));
            gold.a = (1f - k) * (1f - k);              // smooth, not linear
            if (prop != null) m.SetColor(prop, gold);
            else { var c = m.color; c.a = gold.a; m.color = c; }
            yield return null;
        }
        Destroy(tr.gameObject);
    }

    // ==================================================== PAGE TURNING ====
    public void TurnPage(int dir)
    {
        if (_busy) return;
        int next = _page + dir;
        if (next < 0) return;
        int limit = oneSentencePerSpread
            ? _sentences.Count
            : Mathf.CeilToInt(_sentences.Count / (float)(linesPerPage * 2));
        if (next >= limit && dir > 0) return;
        StartCoroutine(TurnRoutine(next, dir));
    }

    IEnumerator TurnRoutine(int next, int dir)
    {
        _busy = true;
        if (_flip != null)
        {
            _flip.gameObject.SetActive(true);
            float a0 = dir > 0 ? 0f : 180f, a1 = dir > 0 ? 180f : 0f;
            for (float t = 0; t < 0.55f; t += Time.deltaTime)
            {
                float k = Mathf.SmoothStep(0, 1, t / 0.55f);
                _flip.localRotation = Quaternion.Euler(
                    0, Mathf.Lerp(a0, a1, k), 0);
                yield return null;
            }
            _flip.gameObject.SetActive(false);
            _flip.localRotation = Quaternion.identity;
        }
        Burst(transform.position + Vector3.up * 0.15f, 10, 0.16f, 0.7f);
        ShowPage(next);
        _busy = false;
    }
}
