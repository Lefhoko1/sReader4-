// ===========================================================================
//  KeywordCard — the gold "key word" callout card, with a pointer line down
//  to the stone it belongs to.
//  (Own file so Unity can load it — one MonoBehaviour per file.)
// ===========================================================================
using System.Collections;
using UnityEngine;
using TMPro;

public class KeywordCard : MonoBehaviour
{
    static KeywordCard _current;

    // palette shared by the card, the leader line and the anchor dot
    static readonly Color GoldFill = new Color(1f, 0.93f, 0.72f);
    static readonly Color GoldEdge = new Color(0.85f, 0.62f, 0.20f);
    static readonly Color GoldStar = new Color(1f, 0.75f, 0.15f);

    LineRenderer _leader;
    Transform _stone;
    Renderer _dot;

    public static void Show(string word, Transform stone)
    {
        if (_current != null) Destroy(_current.gameObject);
        var go = new GameObject("KeywordCard");
        _current = go.AddComponent<KeywordCard>();
        _current.Build(word, stone);
    }

    void Build(string word, Transform stone)
    {
        // world-space card floating above the stone, always facing camera.
        // Sits well clear of the stone's own word label so the two never crowd.
        transform.position = stone.position + Vector3.up * 1.5f
                             + Vector3.right * 0.25f;
        // gold rounded panel (a scaled quad with generated rounded-rect tex)
        var panel = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(panel.GetComponent<Collider>());
        panel.transform.SetParent(transform, false);
        panel.transform.localScale = new Vector3(1.1f, 0.55f, 1f);
        var pr = panel.GetComponent<Renderer>();
        var sh = Shader.Find("Sprites/Default");
        var pm = new Material(sh) { mainTexture = RoundedRect(256, 128, 28,
            GoldFill, GoldEdge) };
        pr.material = pm;

        var star = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(star.GetComponent<Collider>());
        star.name = "Star";
        star.transform.SetParent(transform, false);
        star.transform.localPosition = new Vector3(0, 0.34f, -0.012f);
        star.transform.localScale = Vector3.one * 0.26f;
        var sr2 = star.GetComponent<Renderer>();
        var sm2 = new Material(Shader.Find("Sprites/Default"));
        sm2.mainTexture = WordStone.MakeStarTexture(64);
        sm2.color = GoldStar;
        sr2.material = sm2;

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

        BuildLeader(stone);
        StartCoroutine(Life());
    }

    // A tapered gold pointer from the stone up to the card, plus a small glowing
    // dot where it meets the stone. World-space so the card's pop-in scale never
    // distorts it.
    void BuildLeader(Transform stone)
    {
        _stone = stone;

        var go = new GameObject("Leader");
        go.transform.SetParent(transform, false);
        _leader = go.AddComponent<LineRenderer>();
        _leader.useWorldSpace = true;
        _leader.positionCount = 2;
        _leader.numCapVertices = 4;                 // rounded ends
        _leader.textureMode = LineTextureMode.Stretch;
        _leader.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _leader.receiveShadows = false;
        _leader.widthCurve = AnimationCurve.Linear(0f, 0.035f, 1f, 0.018f);  // thick at the stone, fine at the card

        var mat = new Material(Shader.Find("Sprites/Default"));
        _leader.material = mat;
        _leader.colorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(GoldEdge, 0f), new GradientColorKey(GoldFill, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(0.95f, 0f), new GradientAlphaKey(0.55f, 1f) }
        };

        var dot = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(dot.GetComponent<Collider>());
        dot.name = "LeaderDot";
        dot.transform.SetParent(transform, false);
        dot.transform.localScale = Vector3.one * 0.16f;
        _dot = dot.GetComponent<Renderer>();
        _dot.material = new Material(Shader.Find("Sprites/Default"))
        {
            mainTexture = WordStone.MakeStarTexture(32),
            color = GoldStar
        };

        UpdateLeader(1f);
    }

    // Re-pin the line every frame: the card floats and billboards, the stone may hop.
    void UpdateLeader(float alpha)
    {
        if (_leader == null || _stone == null) return;
        Vector3 from = _stone.position + Vector3.up * 0.30f;    // just above the stone's own word
        Vector3 to   = transform.position - transform.up * 0.30f; // the card's lower edge
        _leader.SetPosition(0, from);
        _leader.SetPosition(1, to);

        var g = _leader.colorGradient;
        g.alphaKeys = new[] { new GradientAlphaKey(0.95f * alpha, 0f), new GradientAlphaKey(0.55f * alpha, 1f) };
        _leader.colorGradient = g;

        if (_dot != null)
        {
            _dot.transform.position = from;
            _dot.transform.rotation = transform.rotation;       // face the camera with the card
            var c = GoldStar; c.a = alpha;
            _dot.material.color = c;
        }
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
            Face(); UpdateLeader(k); yield return null;      // line draws in with the card
        }
        transform.localScale = target;
        float life = 0;
        while (life < 3.2f)                      // hold ~3 s, gentle float
        {
            life += Time.deltaTime;
            transform.position += Vector3.up * Mathf.Sin(life * 2f)
                                  * 0.0004f;
            Face(); UpdateLeader(1f); yield return null;
        }
        for (float t = 0; t < 0.3f; t += Time.deltaTime)   // fade-shrink out
        {
            float k = 1f - t / 0.3f;
            transform.localScale = target * k;
            Face(); UpdateLeader(k); yield return null;
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
