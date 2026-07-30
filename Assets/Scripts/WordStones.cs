// ===========================================================================
//  WordStones.cs — the Word Path: stones that carry words and keyword bursts
// ===========================================================================
//  WordStone — put on a stone prefab (SM_WordStone_A/B/C/Key).
//              Shows its word above the stone (TextMeshPro, billboarded so it
//              stays readable from any angle). Click/tap:
//                keyword  -> gold ring burst + star sparkles + card
//                normal   -> soft puff + little hop
//              Raises onWordClicked(word, isKeyword) for game logic.
//
//  KeywordCard and WordPathBuilder live in their OWN files — Unity cannot load
//  a component whose class name does not match its file name.
//
//  Uses TextMeshPro. New & old input supported.
// ===========================================================================
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class WordStone : MonoBehaviour
{
    [Header("Word")]
    public string word = "word";
    public bool isKeyword = false;
    [Tooltip("Add a full stop after the word (last stone of a sentence).")]
    public bool endsSentence = false;

    [Header("Look")]
    public float textSize = 1.6f;
    [Tooltip("Word height above the stone (metres).")]
    public float wordHeight = 0.3f;
    public Color textColor = new Color(0.25f, 0.20f, 0.15f);
    public Color keywordTextColor = new Color(0.55f, 0.35f, 0.05f);

    [System.Serializable] public class WordClickEvent :
        UnityEvent<string, bool> { }
    public WordClickEvent onWordClicked = new WordClickEvent();

    /// <summary>
    /// The tutor's content for this word (Define / Fill / Illustrate + its
    /// definition / image options). Set by WordPathBuilder when the path is built
    /// from real assignment content; null for plain decorative words.
    /// </summary>
    [System.NonSerialized] public SReader.Domains.Assignments.Models.ContentToken token;

    /// <summary>True once its challenge has been solved (stone stays lit).</summary>
    [System.NonSerialized] public bool solved;

    /// <summary>Set while a challenge/board owns input, so stone taps don't leak through.</summary>
    public static bool InputLocked;

    /// <summary>
    /// When the game drives this stone, the tap opens the real challenge instead of
    /// the "key word" callout card — so the two never pile on top of each other.
    /// </summary>
    [System.NonSerialized] public bool suppressCard;

    TextMeshPro _tmp;
    static Material _burstMat;
    static Texture2D _starTex, _dotTex;

    void Start()
    {
        BuildText();
        EnsureCollider();
    }

    void BuildText()
    {
        var socket = transform.Find("SOCKET_Text");
        var go = new GameObject("WordText");
        go.transform.SetParent(socket != null ? socket : transform, false);
        if (socket == null)
            go.transform.localPosition = new Vector3(0, 0.20f, 0);
        // rotation is handled each frame by LateUpdate (billboard toward camera)
        _tmp = go.AddComponent<TextMeshPro>();
        _tmp.fontSize = textSize;
        _tmp.color = isKeyword ? keywordTextColor : textColor;
        _tmp.alignment = TextAlignmentOptions.Center;
        _tmp.fontStyle = FontStyles.Bold;
        _tmp.textWrappingMode = TextWrappingModes.NoWrap;   // never break a word over two lines
        _tmp.overflowMode = TextOverflowModes.Overflow;
        _tmp.outlineWidth = 0.12f;
        _tmp.outlineColor = new Color32(255, 250, 235, 160);
        Relabel();
    }

    /// <summary>
    /// Put the current word on the stone and fit the label's rect to it, so the
    /// text is never clipped or wrapped by a fixed-size box.
    /// </summary>
    public void Relabel()
    {
        if (_tmp == null) return;
        _tmp.text = word + (endsSentence ? "." : "");
        _tmp.color = isKeyword ? keywordTextColor : textColor;
        Fit();
    }

    /// <summary>Grow the label's rect to whatever it currently says.</summary>
    void Fit()
    {
        if (_tmp == null) return;
        var pref = _tmp.GetPreferredValues();
        _tmp.rectTransform.sizeDelta = new Vector2(Mathf.Max(pref.x, 0.4f),
                                                   Mathf.Max(pref.y, 0.3f));
    }

    /// <summary>Set the word this stone carries (used when a flying chip lands).</summary>
    public void SetWord(string w, bool last)
    {
        word = w;
        endsSentence = last;
        Relabel();
    }

    /// <summary>Light the stone permanently — its challenge was solved.</summary>
    public void MarkSolved()
    {
        solved = true;
        if (_tmp != null)
        {
            _tmp.color = new Color(0.45f, 0.30f, 0.05f);
            _tmp.fontStyle = FontStyles.Bold;
            _tmp.text = word + (endsSentence ? "." : "") + " <color=#E0A21Fff>*</color>";
            Fit();
        }
        PlayKeywordBurst(transform.position + Vector3.up * 0.15f, transform);
    }

    void EnsureCollider()
    {
        if (GetComponent<Collider>() == null &&
            GetComponentInChildren<Collider>() == null)
        {
            var bc = gameObject.AddComponent<BoxCollider>();
            var r = GetComponentInChildren<Renderer>();
            if (r != null)
            {
                bc.center = transform.InverseTransformPoint(r.bounds.center);
                bc.size = transform.InverseTransformVector(
                              r.bounds.size).Abs() + Vector3.up * 0.3f;
            }
        }
    }

    // ---- click detection (works for mouse + touch, both input systems) ----
    void Update()
    {
        if (InputLocked) return;                 // a challenge/board is capturing taps
        Vector2 pos; bool pressed = PointerDown(out pos);
        if (!pressed) return;
        var cam = Camera.main; if (cam == null) return;
        var ray = cam.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out var hit, 200f) &&
            (hit.collider.transform == transform ||
             hit.collider.transform.IsChildOf(transform)))
            Clicked();
    }

    // Keep the word hovering above the stone and turned toward the camera, so it
    // stays readable from any angle (not flat/edge-on).
    void LateUpdate()
    {
        if (_tmp == null) return;
        var cam = Camera.main;
        if (cam == null) return;
        // above the stone itself — scaled with it, so the label clears a far stone
        // that perspective compensation has grown, and doesn't float off a near one
        _tmp.transform.position = transform.position +
                                  Vector3.up * wordHeight * Mathf.Max(0.01f, transform.lossyScale.y);
        _tmp.transform.rotation = Quaternion.LookRotation(
            _tmp.transform.position - cam.transform.position, Vector3.up);
    }

    static bool PointerDown(out Vector2 pos)
    {
        pos = default;
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        { pos = Touchscreen.current.primaryTouch.position.ReadValue(); return true; }
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        { pos = Mouse.current.position.ReadValue(); return true; }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetMouseButtonDown(0)) { pos = Input.mousePosition; return true; }
        if (Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Began)
        { pos = Input.GetTouch(0).position; return true; }
#endif
        return false;
    }

    public void Clicked()
    {
        if (solved) { PlayPuff(transform.position + Vector3.up * 0.15f); return; }
        StartCoroutine(Hop());
        if (isKeyword)
        {
            PlayKeywordBurst(transform.position + Vector3.up * 0.15f,
                             transform);
            if (!suppressCard) KeywordCard.Show(word, transform);   // the game opens a challenge instead
        }
        else
            PlayPuff(transform.position + Vector3.up * 0.15f);
        onWordClicked.Invoke(word, isKeyword);
    }

    IEnumerator Hop()
    {
        Vector3 home = transform.localPosition;
        for (float t = 0; t < 0.28f; t += Time.deltaTime)
        {
            float k = Mathf.Sin(Mathf.Clamp01(t / 0.28f) * Mathf.PI);
            transform.localPosition = home + Vector3.up * 0.06f * k;
            yield return null;
        }
        transform.localPosition = home;
    }

    // ================================================== FX (built in code) =
    static void EnsureFXAssets()
    {
        if (_starTex != null) return;
        _starTex = MakeStarTexture(64);
        _dotTex = MakeDotTexture(64);
        var sh = Shader.Find("GreatLibrary/MoteAdditive");
        if (sh == null) sh = Shader.Find("Particles/Standard Unlit");
        if (sh == null) sh = Shader.Find("Sprites/Default");
        _burstMat = new Material(sh);
        if (_burstMat.HasProperty("_BaseMap"))
            _burstMat.SetTexture("_BaseMap", _starTex);
        _burstMat.mainTexture = _starTex;
    }

    static ParticleSystem MakePS(Vector3 pos, Texture2D tex, Color col,
                                 float size, float life)
    {
        EnsureFXAssets();
        var go = new GameObject("WordFX");
        go.transform.position = pos;
        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = life;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.4f, 1.2f);
        main.startSize = new ParticleSystem.MinMaxCurve(size * 0.6f, size);
        main.startColor = col;
        main.gravityModifier = -0.05f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = false;
        var em = ps.emission; em.enabled = false;
        var shp = ps.shape; shp.enabled = true;
        shp.shapeType = ParticleSystemShapeType.Circle; shp.radius = 0.28f;
        var colL = ps.colorOverLifetime; colL.enabled = true;
        var g = new Gradient();
        g.SetKeys(new[]{ new GradientColorKey(Color.white,0),
                         new GradientColorKey(Color.white,1)},
                  new[]{ new GradientAlphaKey(0,0), new GradientAlphaKey(1,0.12f),
                         new GradientAlphaKey(0,1)});
        colL.color = g;
        var rot = ps.rotationOverLifetime; rot.enabled = true;
        rot.z = new ParticleSystem.MinMaxCurve(-2f, 2f);
        var psr = go.GetComponent<ParticleSystemRenderer>();
        var m = new Material(_burstMat); m.mainTexture = tex;
        if (m.HasProperty("_BaseMap")) m.SetTexture("_BaseMap", tex);
        psr.material = m;
        Destroy(go, life + 1f);
        return ps;
    }

    public static void PlayKeywordBurst(Vector3 pos, Transform follow = null)
    {
        EnsureFXAssets();
        var stars = MakePS(pos, _starTex, new Color(1f, 0.85f, 0.35f), 0.16f, 1.3f);
        stars.Emit(14);
        var glow = MakePS(pos, _dotTex, new Color(1f, 0.75f, 0.30f, 0.8f),
                          0.30f, 0.9f);
        glow.Emit(10);
        // expanding golden ring (a flat quad that scales + fades)
        var ring = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Object.Destroy(ring.GetComponent<Collider>());
        ring.transform.position = pos + Vector3.up * 0.02f;
        ring.transform.rotation = Quaternion.Euler(90, 0, 0);
        var rr = ring.GetComponent<Renderer>();
        var rm = new Material(_burstMat);
        var ringTex = MakeRingTexture(128);
        rm.mainTexture = ringTex;
        if (rm.HasProperty("_BaseMap")) rm.SetTexture("_BaseMap", ringTex);
        var goldC = new Color(1f, 0.8f, 0.3f, 1f);
        if (rm.HasProperty("_BaseColor")) rm.SetColor("_BaseColor", goldC);
        else if (rm.HasProperty("_Color")) rm.SetColor("_Color", goldC);
        rr.material = rm;
        var runner = ring.AddComponent<FXRunner>();
        runner.StartCoroutine(runner.RingExpand(ring.transform, rr));
    }

    public static void PlayPuff(Vector3 pos)
    {
        EnsureFXAssets();
        var p = MakePS(pos, _dotTex, new Color(0.95f, 0.93f, 0.85f, 0.7f),
                       0.14f, 0.7f);
        p.Emit(6);
    }

    // ---- tiny runtime textures --------------------------------------------
    public static Texture2D MakeStarTexture(int s)
    {
        var t = new Texture2D(s, s, TextureFormat.RGBA32, false);
        for (int y = 0; y < s; y++) for (int x = 0; x < s; x++)
        {
            float dx = (x - s/2f)/(s/2f), dy = (y - s/2f)/(s/2f);
            float ang = Mathf.Atan2(dy, dx);
            float r = Mathf.Sqrt(dx*dx + dy*dy);
            float star = 0.55f + 0.45f * Mathf.Cos(5f * (ang - Mathf.PI/2));
            float a = Mathf.Clamp01((star*0.9f - r) * 6f);
            t.SetPixel(x, y, new Color(1, 1, 1, a));
        }
        t.Apply(); return t;
    }
    static Texture2D MakeDotTexture(int s)
    {
        var t = new Texture2D(s, s, TextureFormat.RGBA32, false);
        for (int y = 0; y < s; y++) for (int x = 0; x < s; x++)
        {
            float dx=(x-s/2f)/(s/2f), dy=(y-s/2f)/(s/2f);
            float r=Mathf.Sqrt(dx*dx+dy*dy);
            t.SetPixel(x,y,new Color(1,1,1,Mathf.Clamp01((1f-r)*1.6f)));
        }
        t.Apply(); return t;
    }
    static Texture2D MakeRingTexture(int s)
    {
        var t = new Texture2D(s, s, TextureFormat.RGBA32, false);
        for (int y = 0; y < s; y++) for (int x = 0; x < s; x++)
        {
            float dx=(x-s/2f)/(s/2f), dy=(y-s/2f)/(s/2f);
            float r=Mathf.Sqrt(dx*dx+dy*dy);
            float band = Mathf.Clamp01(1f - Mathf.Abs(r-0.8f)*8f);
            t.SetPixel(x,y,new Color(1,1,1,band));
        }
        t.Apply(); return t;
    }

    class FXRunner : MonoBehaviour
    {
        public IEnumerator RingExpand(Transform tr, Renderer r)
        {
            var m = r.material;
            string prop = m.HasProperty("_BaseColor") ? "_BaseColor"
                        : m.HasProperty("_Color")     ? "_Color" : null;
            Color baseC = prop != null ? m.GetColor(prop)
                                       : new Color(1f, 0.8f, 0.3f, 1f);
            for (float t = 0; t < 0.7f; t += Time.deltaTime)
            {
                float k = t / 0.7f;
                tr.localScale = Vector3.one * Mathf.Lerp(0.3f, 2.2f,
                                                         1 - (1-k)*(1-k));
                if (prop != null)
                {
                    baseC.a = 1f - k;
                    m.SetColor(prop, baseC);
                }
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}

static class VecExt
{
    public static Vector3 Abs(this Vector3 v) =>
        new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
}
