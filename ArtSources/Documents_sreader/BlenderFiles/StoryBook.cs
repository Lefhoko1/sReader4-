// ===========================================================================
//  StoryBook.cs — the candy storybook: pages, clickable sentences, and the
//  sentence-to-stones flight with sparkle FX
// ===========================================================================
//  DROP-IN: put this on the SM_StoryBook prefab in your scene. Press Play.
//
//  WHAT IT DOES
//   • Lays the title + paragraph across the book's two pages, one TextMeshPro
//     per sentence, using the SOCKET_LineL_/SOCKET_LineR_ anchors in the FBX.
//   • Hovering a sentence lifts and warms it. Clicking it:
//        1. the sentence flashes and bursts candy sparkles along its length
//        2. every word detaches as a glowing 3D chip
//        3. the chips arc through the air on a curved path, trailing sparkles
//        4. each lands on its stone with a pop + ring flash, and writes its
//           word onto that WordStone (keyword stone gets the gold treatment)
//   • Page turn: Next/Prev buttons at the FBX sockets, with the FlipPage child
//     rotating around the spine.
//
//  TARGET STONES
//   Assign `stonePath` = the WordPath_River object (its children are stones,
//   in order). Leave empty and it finds the first WordPathBuilder in the scene.
//   Words land on stones 1..N in order; extra words fade out mid-air.
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
    [Tooltip("Sentences per page (matches the 6 line sockets per page).")]
    public int linesPerPage = 5;

    [Header("Flight target")]
    [Tooltip("Object whose children are the word stones, in order. " +
             "Leave empty to auto-find WordPath_River / WordPathBuilder.")]
    public Transform stonePath;

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

    // ==================================================================== //
    void Start()
    {
        EnsureFX();
        _flip = FindChild("SM_StoryBook_FlipPage");
        if (_flip != null) _flip.gameObject.SetActive(false);
        if (stonePath == null) AutoFindStones();
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
        var go = new GameObject("BookTitle");
        go.transform.SetParent(socket != null ? socket : transform, false);
        go.transform.localRotation = Quaternion.Euler(90, 0, 0);
        var t = go.AddComponent<TextMeshPro>();
        t.text = title;
        t.fontSize = 1.5f; t.fontStyle = FontStyles.Bold;
        t.color = titleColor;
        t.alignment = TextAlignmentOptions.Center;
        t.rectTransform.sizeDelta = new Vector2(0.9f, 0.12f);
    }

    void ShowPage(int page)
    {
        foreach (var l in _lines) if (l.tmp) Destroy(l.tmp.gameObject);
        _lines.Clear();
        _page = Mathf.Max(0, page);

        int perSpread = linesPerPage * 2;
        int first = _page * perSpread;
        for (int i = 0; i < perSpread; i++)
        {
            int idx = first + i;
            if (idx >= _sentences.Count) break;
            bool right = i >= linesPerPage;
            int slot = (i % linesPerPage) + 1;
            var socket = FindChild($"SOCKET_Line{(right ? "R" : "L")}_{slot}");
            if (socket == null) continue;
            MakeLine(_sentences[idx], socket);
        }
    }

    void MakeLine(string text, Transform socket)
    {
        var go = new GameObject("Sentence");
        go.transform.SetParent(socket, false);
        go.transform.localRotation = Quaternion.Euler(90, 0, 0);
        var t = go.AddComponent<TextMeshPro>();
        t.text = text;
        t.fontSize = 0.62f;
        t.color = inkColor;
        t.alignment = TextAlignmentOptions.Left;
        t.enableWordWrapping = true;
        t.rectTransform.sizeDelta = new Vector2(0.42f, 0.10f);
        t.margin = new Vector4(0.01f, 0, 0.01f, 0);

        var col = go.AddComponent<BoxCollider>();
        col.size = new Vector3(0.42f, 0.075f, 0.01f);
        col.isTrigger = true;

        _lines.Add(new Line { tmp = t, text = text, col = col,
                              home = go.transform.localPosition });
    }

    void BuildPageButtons()
    {
        MakeButton("SOCKET_NextPage", "▶", () => TurnPage(+1));
        MakeButton("SOCKET_PrevPage", "◀", () => TurnPage(-1));
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
        l.tmp.transform.localPosition = l.home +
            (hot ? new Vector3(0, 0, -0.012f) : Vector3.zero);
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
        var launch = FindChild("SOCKET_LaunchPoint");
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
                new Vector3(0, 0, -0.012f - 0.01f * k);
            yield return null;
        }
    }

    IEnumerator FlyChip(string word, Vector3 from, Transform target,
                        bool isLast)
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
            go.transform.localScale = Vector3.one *
                Mathf.Lerp(1.15f, 0.55f, e);
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
        if (_star != null) return;
        _star = Tex(64, (dx, dy) => {
            float ang = Mathf.Atan2(dy, dx), r = Mathf.Sqrt(dx*dx+dy*dy);
            float st = 0.55f + 0.45f * Mathf.Cos(5f*(ang - Mathf.PI/2));
            return Mathf.Clamp01((st*0.9f - r) * 6f); });
        _dot = Tex(64, (dx, dy) =>
            Mathf.Clamp01((1f - Mathf.Sqrt(dx*dx+dy*dy)) * 1.7f));
        _ring = Tex(128, (dx, dy) => {
            float r = Mathf.Sqrt(dx*dx+dy*dy);
            return Mathf.Clamp01(1f - Mathf.Abs(r - 0.78f) * 7f); });
        var sh = Shader.Find("Particles/Standard Unlit")
              ?? Shader.Find("Sprites/Default");
        _fxMat = new Material(sh);
    }

    static Texture2D Tex(int s, System.Func<float,float,float> f)
    {
        var t = new Texture2D(s, s, TextureFormat.RGBA32, false);
        for (int y = 0; y < s; y++) for (int x = 0; x < s; x++)
        {
            float dx = (x - s/2f)/(s/2f), dy = (y - s/2f)/(s/2f);
            t.SetPixel(x, y, new Color(1,1,1, Mathf.Clamp01(f(dx,dy))));
        }
        t.Apply(); return t;
    }

    Color RandCandy() => sparkleColors[Random.Range(0, sparkleColors.Length)];

    ParticleSystem MakeTrail(Transform parent)
    {
        EnsureFX();
        var go = new GameObject("Trail");
        go.transform.SetParent(parent, false);
        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 0.55f;
        main.startSpeed = 0.06f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.045f, 0.10f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            RandCandy(), RandCandy());
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = -0.03f;
        main.maxParticles = 80;
        var em = ps.emission; em.rateOverTime = 55f;
        var shape = ps.shape; shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere; shape.radius = 0.05f;
        var col = ps.colorOverLifetime; col.enabled = true;
        var g = new Gradient();
        g.SetKeys(new[]{ new GradientColorKey(Color.white,0),
                         new GradientColorKey(Color.white,1)},
                  new[]{ new GradientAlphaKey(0.9f,0),
                         new GradientAlphaKey(0f,1)});
        col.color = g;
        var sol = ps.sizeOverLifetime; sol.enabled = true;
        sol.size = new ParticleSystem.MinMaxCurve(1f,
            new AnimationCurve(new Keyframe(0,1), new Keyframe(1,0.1f)));
        var r = go.GetComponent<ParticleSystemRenderer>();
        var m = new Material(_fxMat); m.mainTexture = _star;
        if (m.HasProperty("_BaseMap")) m.SetTexture("_BaseMap", _star);
        r.material = m;
        r.renderMode = ParticleSystemRenderMode.Billboard;
        return ps;
    }

    void Burst(Vector3 pos, int count, float size, float life)
    {
        EnsureFX();
        var go = new GameObject("Sparkle");
        go.transform.position = pos;
        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = life;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1.8f);
        main.startSize = new ParticleSystem.MinMaxCurve(size*0.5f, size);
        main.startColor = new ParticleSystem.MinMaxGradient(
            RandCandy(), RandCandy());
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 0.15f;
        main.playOnAwake = false;
        main.maxParticles = 60;
        var em = ps.emission; em.enabled = false;
        var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.12f;
        var col = ps.colorOverLifetime; col.enabled = true;
        var g = new Gradient();
        g.SetKeys(new[]{ new GradientColorKey(Color.white,0),
                         new GradientColorKey(Color.white,1)},
                  new[]{ new GradientAlphaKey(0,0), new GradientAlphaKey(1,0.1f),
                         new GradientAlphaKey(0,1)});
        col.color = g;
        var rot = ps.rotationOverLifetime; rot.enabled = true;
        rot.z = new ParticleSystem.MinMaxCurve(-3f, 3f);
        var r = go.GetComponent<ParticleSystemRenderer>();
        var m = new Material(_fxMat); m.mainTexture = _star;
        if (m.HasProperty("_BaseMap")) m.SetTexture("_BaseMap", _star);
        r.material = m;
        ps.Emit(count);
        Destroy(go, life + 1.2f);
    }

    void RingFlash(Vector3 pos, Transform face)
    {
        EnsureFX();
        var q = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(q.GetComponent<Collider>());
        q.transform.position = pos;
        q.transform.rotation = Quaternion.Euler(90, 0, 0);
        var r = q.GetComponent<Renderer>();
        var m = new Material(Shader.Find("Sprites/Default"));
        m.mainTexture = _ring;
        m.color = new Color(1f, 0.85f, 0.45f, 1f);
        r.material = m;
        StartCoroutine(RingGrow(q.transform, r));
    }

    IEnumerator RingGrow(Transform tr, Renderer r)
    {
        for (float t = 0; t < 0.55f; t += Time.deltaTime)
        {
            float k = t / 0.55f;
            tr.localScale = Vector3.one * Mathf.Lerp(0.15f, 1.3f,
                                                     1 - (1-k)*(1-k));
            var c = r.material.color; c.a = 1f - k;
            r.material.color = c;
            yield return null;
        }
        Destroy(tr.gameObject);
    }

    // ==================================================== PAGE TURNING ====
    public void TurnPage(int dir)
    {
        if (_busy) return;
        int perSpread = linesPerPage * 2;
        int next = _page + dir;
        if (next < 0) return;
        if (next * perSpread >= _sentences.Count && dir > 0) return;
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
