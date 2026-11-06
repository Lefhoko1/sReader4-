// ===========================================================================
//  WordStones.cs — the Word Path: stones that carry words, keyword bursts,
//  and the "key word" card (matches the reference mock-up)
// ===========================================================================
//  THREE components in this file:
//
//  • WordStone       — put on a stone prefab (SM_WordStone_A/B/C/Key).
//                      Shows its word on top (TextMeshPro, auto-created at
//                      SOCKET_Text). Click/tap:
//                        keyword  -> gold ring burst + star sparkles + card
//                        normal   -> soft puff + little hop
//                      Raises onWordClicked(word, isKeyword) for game logic.
//
//  • WordPathBuilder — give it a sentence + the keyword + two end points;
//                      it lays the stones along a gentle S-curve, one word
//                      per stone, keyword on the glowing Key stone.
//                      Works in Edit mode: right-click component header ->
//                      "Build Path". Great for placing across your island.
//
//  • KeywordCard     — internal: the gold "key word" callout card (star,
//                      word, label) that pops in when a keyword is clicked.
//
//  SETUP (once):
//   1. Import the WordStones FBX files anywhere OUTSIDE Models_Kit
//      (e.g. Assets/Art/Models_WordPath) so their colours import as-is.
//      Extract/remap materials or just let Unity create them.
//   2. Make prefabs: drag each SM_WordStone_* into the scene, Add Component
//      -> WordStone, save as prefab. Tick "Is Keyword" on the Key variant.
//   3. For a full sentence: empty GameObject -> WordPathBuilder, assign the
//      four stone prefabs, type the sentence + keyword, set Start/End
//      transforms (two empties on your island), context-menu "Build Path".
//
//  Uses TextMeshPro (in every Unity 6 project). New & old input supported.
// ===========================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// ===========================================================================
//  WORD STONE
// ===========================================================================
public class WordStone : MonoBehaviour
{
    [Header("Word")]
    public string word = "word";
    public bool isKeyword = false;
    [Tooltip("Add a full stop after the word (last stone of a sentence).")]
    public bool endsSentence = false;

    [Header("Look")]
    public float textSize = 1.6f;
    public Color textColor = new Color(0.25f, 0.20f, 0.15f);
    public Color keywordTextColor = new Color(0.55f, 0.35f, 0.05f);

    [System.Serializable] public class WordClickEvent :
        UnityEvent<string, bool> { }
    public WordClickEvent onWordClicked = new WordClickEvent();

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
        go.transform.localRotation = Quaternion.Euler(90, 0, 0); // lie flat
        _tmp = go.AddComponent<TextMeshPro>();
        _tmp.text = word + (endsSentence ? "." : "");
        _tmp.fontSize = textSize;
        _tmp.color = isKeyword ? keywordTextColor : textColor;
        _tmp.alignment = TextAlignmentOptions.Center;
        _tmp.fontStyle = FontStyles.Bold;
        _tmp.rectTransform.sizeDelta = new Vector2(1.2f, 0.5f);
        _tmp.outlineWidth = 0.12f;
        _tmp.outlineColor = new Color32(255, 250, 235, 160);
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
        Vector2 pos; bool pressed = PointerDown(out pos);
        if (!pressed) return;
        var cam = Camera.main; if (cam == null) return;
        var ray = cam.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out var hit, 200f) &&
            (hit.collider.transform == transform ||
             hit.collider.transform.IsChildOf(transform)))
            Clicked();
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
        StartCoroutine(Hop());
        if (isKeyword)
        {
            PlayKeywordBurst(transform.position + Vector3.up * 0.15f,
                             transform);
            KeywordCard.Show(word, transform);
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
        var stars = MakePS(pos, _starTex ?? MakeStarTexture(64),
                           new Color(1f, 0.85f, 0.35f), 0.16f, 1.3f);
        EnsureFXAssets();
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
        if (rm.HasProperty("_Color"))
            rm.SetColor("_Color", new Color(1f, 0.8f, 0.3f, 1f));
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
    static Texture2D MakeStarTexture(int s)
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
            for (float t = 0; t < 0.7f; t += Time.deltaTime)
            {
                float k = t / 0.7f;
                tr.localScale = Vector3.one * Mathf.Lerp(0.3f, 2.2f,
                                                         1 - (1-k)*(1-k));
                var c = r.material.color; c.a = 1f - k;
                if (r.material.HasProperty("_Color")) r.material.color = c;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}

// ===========================================================================
//  KEYWORD CARD — the gold "key word" callout
// ===========================================================================
public class KeywordCard : MonoBehaviour
{
    static KeywordCard _current;

    public static void Show(string word, Transform stone)
    {
        if (_current != null) Destroy(_current.gameObject);
        var go = new GameObject("KeywordCard");
        _current = go.AddComponent<KeywordCard>();
        _current.Build(word, stone);
    }

    void Build(string word, Transform stone)
    {
        // world-space card floating above the stone, always facing camera
        transform.position = stone.position + Vector3.up * 0.9f
                             + Vector3.right * 0.25f;
        // gold rounded panel (a scaled quad with generated rounded-rect tex)
        var panel = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(panel.GetComponent<Collider>());
        panel.transform.SetParent(transform, false);
        panel.transform.localScale = new Vector3(1.1f, 0.55f, 1f);
        var pr = panel.GetComponent<Renderer>();
        var sh = Shader.Find("Sprites/Default");
        var pm = new Material(sh) { mainTexture = RoundedRect(256, 128, 28,
            new Color(1f, 0.93f, 0.72f), new Color(0.85f, 0.62f, 0.20f)) };
        pr.material = pm;

        var star = new GameObject("Star").AddComponent<TextMeshPro>();
        star.transform.SetParent(transform, false);
        star.transform.localPosition = new Vector3(0, 0.30f, -0.01f);
        star.text = "\u2605";
        star.fontSize = 3; star.color = new Color(1f, 0.75f, 0.15f);
        star.alignment = TextAlignmentOptions.Center;

        var txt = new GameObject("Word").AddComponent<TextMeshPro>();
        txt.transform.SetParent(transform, false);
        txt.transform.localPosition = new Vector3(0, 0.02f, -0.01f);
        txt.text = word;
        txt.fontSize = 4.2f; txt.fontStyle = FontStyles.Bold;
        txt.color = new Color(0.35f, 0.22f, 0.05f);
        txt.alignment = TextAlignmentOptions.Center;

        var tag = new GameObject("Tag").AddComponent<TextMeshPro>();
        tag.transform.SetParent(transform, false);
        tag.transform.localPosition = new Vector3(0, -0.42f, -0.01f);
        tag.text = "key word";
        tag.fontSize = 1.6f; tag.color = new Color(0.45f, 0.30f, 0.10f);
        tag.alignment = TextAlignmentOptions.Center;

        StartCoroutine(Life());
    }

    IEnumerator Life()
    {
        // pop in
        Vector3 target = Vector3.one;
        transform.localScale = Vector3.zero;
        for (float t = 0; t < 0.25f; t += Time.deltaTime)
        {
            float k = t / 0.25f;
            transform.localScale = target * (1.1f - 0.1f * (1-k)) *
                                   Mathf.SmoothStep(0, 1, k);
            Face(); yield return null;
        }
        transform.localScale = target;
        float life = 0;
        while (life < 3.2f)                      // hold ~3 s, gentle float
        {
            life += Time.deltaTime;
            transform.position += Vector3.up * Mathf.Sin(life * 2f)
                                  * 0.0004f;
            Face(); yield return null;
        }
        for (float t = 0; t < 0.3f; t += Time.deltaTime)   // fade-shrink out
        {
            transform.localScale = target * (1f - t / 0.3f);
            Face(); yield return null;
        }
        Destroy(gameObject);
    }

    void Face()
    {
        var cam = Camera.main;
        if (cam != null)
            transform.rotation = Quaternion.LookRotation(
                transform.position - cam.transform.position);
    }

    static Texture2D RoundedRect(int w, int h, int rad, Color fill, Color edge)
    {
        var t = new Texture2D(w, h, TextureFormat.RGBA32, false);
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            int cx = Mathf.Clamp(x, rad, w - rad), cy = Mathf.Clamp(y, rad, h - rad);
            float d = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
            float a = Mathf.Clamp01(rad - d + 1);
            bool border = d > rad - 7 && d <= rad;
            t.SetPixel(x, y, border ? new Color(edge.r, edge.g, edge.b, a)
                                    : new Color(fill.r, fill.g, fill.b, a));
        }
        t.Apply(); return t;
    }
}

// ===========================================================================
//  WORD PATH BUILDER — sentence -> stones along a curve
// ===========================================================================
public class WordPathBuilder : MonoBehaviour
{
    [Header("Content")]
    [TextArea] public string sentence = "The fox lived near a quiet river";
    public string keyword = "river";

    [Header("Stone prefabs (A/B/C + Key)")]
    public GameObject stoneA, stoneB, stoneC, stoneKey;

    [Header("Path")]
    public Transform startPoint, endPoint;
    [Tooltip("Sideways S-curve amount")] public float curve = 0.8f;
    public float stoneScale = 1f;

    [ContextMenu("Build Path")]
    public void Build()
    {
        // clear previous
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var c = transform.GetChild(i).gameObject;
            if (Application.isPlaying) Destroy(c);
            else DestroyImmediate(c);
        }
        if (startPoint == null || endPoint == null)
        { Debug.LogWarning("[WordPath] Assign start & end points."); return; }

        var words = sentence.Trim().TrimEnd('.').Split(' ');
        var variants = new[] { stoneA, stoneB, stoneC };
        Vector3 a = startPoint.position, b = endPoint.position;
        Vector3 side = Vector3.Cross((b - a).normalized, Vector3.up);

        for (int i = 0; i < words.Length; i++)
        {
            float t = words.Length == 1 ? 0.5f : i / (float)(words.Length - 1);
            Vector3 pos = Vector3.Lerp(a, b, t)
                + side * Mathf.Sin(t * Mathf.PI * 2f) * curve;
            bool isKey = words[i].ToLower().Trim() == keyword.ToLower().Trim();
            var prefab = isKey && stoneKey != null
                         ? stoneKey : variants[i % 3];
            if (prefab == null) continue;
            var go = Instantiate(prefab, pos, 
                Quaternion.Euler(0, Random.Range(-14f, 14f), 0), transform);
            go.transform.localScale = Vector3.one * stoneScale *
                                      (isKey ? 1.25f : 1f);
            var ws = go.GetComponent<WordStone>() ??
                     go.AddComponent<WordStone>();
            ws.word = words[i];
            ws.isKeyword = isKey;
            ws.endsSentence = (i == words.Length - 1);
        }
        Debug.Log($"[WordPath] Built {words.Length} stones " +
                  $"(keyword: {keyword}).");
    }
}

static class VecExt
{
    public static Vector3 Abs(this Vector3 v) =>
        new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
}
