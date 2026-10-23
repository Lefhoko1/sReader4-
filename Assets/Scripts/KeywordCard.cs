// ===========================================================================
//  KeywordCard — the gold "key word" callout card, with a pointer line down
//  to the stone it belongs to.
//  (Own file so Unity can load it — one MonoBehaviour per file.)
// ===========================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeywordCard : MonoBehaviour
{
    static KeywordCard _current;

    // palette shared by the card, the leader line and the anchor dot
    static readonly Color GoldFill = new Color(1f, 0.93f, 0.72f);
    static readonly Color GoldEdge = new Color(0.85f, 0.62f, 0.20f);
    static readonly Color GoldStar = new Color(1f, 0.75f, 0.15f);

    // The card is grown to fit the word — never the other way round, so a long
    // word can't spill over the panel edges.
    const float WordFont = 4.2f;    // before any shrink-to-fit
    const float TagFont  = 1.6f;
    const float MaxWidth = 2.6f;    // widest the card may grow before the word shrinks
    const float PadX     = 0.22f;   // breathing room either side of the word
    const float PadY     = 0.10f;   // …and above / below
    const float TagGap   = 0.04f;   // between the word and its "key word" tag

    LineRenderer _leader;
    Transform _stone;
    Renderer _dot;
    Action _action;
    Camera _camera;
    Rect _buttonRect, _cardRect;
    readonly List<UnityEngine.Object> _owned = new List<UnityEngine.Object>();
    public static bool ActionOpen => _current != null && _current._action != null;

    public static void Dismiss()
    {
        if (_current == null) return;
        _current.gameObject.SetActive(false);
        Destroy(_current.gameObject);
        _current = null;
    }

    // The earlier Word Path callout remains unchanged. The house adds an explicit
    // action instead of opening a puzzle before the student has read the card.
    public static void ShowAction(string word, Transform stone, string action, Action onAction, Camera camera)
    {
        Dismiss();
        var go = new GameObject("KeywordCard");
        _current = go.AddComponent<KeywordCard>();
        _current._action = onAction;
        _current._camera = camera;
        _current.Build(word, stone, action);
    }

    /// <summary>Route a house tap before the rug; outside taps dismiss the card.</summary>
    public static bool HandleTap(Vector2 point, Camera camera)
    {
        if (!ActionOpen) return false;
        var card = _current;
        var ray = camera.ScreenPointToRay(point);
        var plane = new Plane(card.transform.forward, card.transform.position);
        if (!plane.Raycast(ray, out float distance)) { Dismiss(); return true; }
        Vector3 local = card.transform.InverseTransformPoint(ray.GetPoint(distance));
        var xy = new Vector2(local.x, local.y);
        if (card._buttonRect.Contains(xy))
        {
            var action = card._action;
            Dismiss();
            action?.Invoke();
        }
        else if (!card._cardRect.Contains(xy)) Dismiss();
        return true;
    }

    T Own<T>(T resource) where T : UnityEngine.Object { _owned.Add(resource); return resource; }
    void OnDestroy() { foreach (var resource in _owned) if (resource != null) Destroy(resource); }

    float _halfHeight = 0.3f;       // measured, so the leader meets the real edge

    public static void Show(string word, Transform stone)
    {
        Dismiss();
        var go = new GameObject("KeywordCard");
        _current = go.AddComponent<KeywordCard>();
        _current.Build(word, stone);
    }

    void Build(string word, Transform stone, string action = null)
    {
        // world-space card floating above the stone, always facing camera.
        // Sits well clear of the stone's own word label so the two never crowd.
        var cam = _camera != null ? _camera : Camera.main;
        transform.position = stone.position + Vector3.up * (action != null ? 1.05f : 1.5f)
                             + (cam != null ? cam.transform.right : Vector3.right) * 0.18f;

        // 1. the word FIRST — everything else is measured from it
        var txt = new GameObject("Word").AddComponent<TextMeshPro>();
        txt.transform.SetParent(transform, false);
        txt.text = string.IsNullOrEmpty(word) ? " " : word;
        txt.fontSize = action != null ? 3.2f : WordFont;
        txt.fontStyle = FontStyles.Bold;
        txt.color = new Color(0.35f, 0.22f, 0.05f);
        txt.alignment = TextAlignmentOptions.Center;
        txt.textWrappingMode = TextWrappingModes.NoWrap;   // a word is never broken in two
        txt.overflowMode = TextOverflowModes.Overflow;

        Vector2 wordSize = txt.GetPreferredValues();
        if (wordSize.x > MaxWidth)                    // very long word: shrink it to fit
        {
            txt.fontSize *= MaxWidth / wordSize.x;
            wordSize = txt.GetPreferredValues();
        }
        txt.rectTransform.sizeDelta = wordSize;
        txt.renderer.sortingOrder = 102;

        var tag = new GameObject("Tag").AddComponent<TextMeshPro>();
        tag.transform.SetParent(transform, false);
        tag.text = action != null ? "KEY WORD" : "key word";
        tag.fontSize = action != null ? 1.3f : TagFont;
        tag.color = new Color(0.45f, 0.30f, 0.10f);
        tag.alignment = TextAlignmentOptions.Center;
        tag.textWrappingMode = TextWrappingModes.NoWrap;
        Vector2 tagSize = tag.GetPreferredValues();
        tag.rectTransform.sizeDelta = tagSize;
        tag.renderer.sortingOrder = 102;

        // 2. the card, grown around them
        float w = Mathf.Max(wordSize.x, tagSize.x) + PadX * 2f;
        float buttonHeight = action != null ? 0.38f : 0f;
        float actionSpace = action != null ? buttonHeight + 0.10f : 0f;
        if (action != null) w = Mathf.Max(w, 1.9f);
        float h = wordSize.y + TagGap + tagSize.y + PadY * 2f + actionSpace;
        _halfHeight = h * 0.5f;
        _cardRect = new Rect(-w * 0.5f, -h * 0.5f, w, h);

        // gold rounded panel (a scaled quad with generated rounded-rect tex)
        var panel = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(panel.GetComponent<Collider>());
        panel.name = "Panel";
        panel.transform.SetParent(transform, false);
        panel.transform.localScale = new Vector3(w, h, 1f);
        var pr = panel.GetComponent<Renderer>();
        var sh = Shader.Find("Sprites/Default");
        // texture drawn at the card's own aspect so the corners stay round
        // however wide the word made it
        int th = 128;
        int tw = Mathf.Clamp(Mathf.RoundToInt(th * (w / h)), 48, 1024);
        var pm = Own(new Material(sh));
        pm.mainTexture = Own(RoundedRect(tw, th, Mathf.RoundToInt(Mathf.Min(tw, th) * 0.22f), GoldFill, GoldEdge));
        pr.sharedMaterial = pm;
        pr.sortingOrder = 100;

        // 3. place the contents inside the measured card
        txt.transform.localPosition =
            new Vector3(0f, h * 0.5f - PadY - wordSize.y * 0.5f, -0.01f);
        tag.transform.localPosition =
            new Vector3(0f, -h * 0.5f + PadY + actionSpace + tagSize.y * 0.5f, -0.01f);

        if (action != null)
        {
            float by = -h * 0.5f + PadY + buttonHeight * 0.5f;
            _buttonRect = new Rect(-w * 0.5f + 0.12f, by - buttonHeight * 0.5f, w - 0.24f, buttonHeight);
            var button = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(button.GetComponent<Collider>());
            button.name = "Keyword Action";
            button.transform.SetParent(transform, false);
            button.transform.localPosition = new Vector3(0f, by, -0.02f);
            button.transform.localScale = new Vector3(_buttonRect.width, buttonHeight, 1f);
            var material = Own(new Material(sh));
            material.mainTexture = Own(RoundedRect(512, 96, 22, new Color32(27, 46, 89, 255), GoldEdge));
            button.GetComponent<Renderer>().sharedMaterial = material;
            button.GetComponent<Renderer>().sortingOrder = 101;
            var label = new GameObject("Action Label").AddComponent<TextMeshPro>();
            label.transform.SetParent(transform, false);
            label.transform.localPosition = new Vector3(0f, by, -0.03f);
            label.text = action;
            label.fontSize = 2.4f;
            label.renderer.sortingOrder = 102;
            label.fontStyle = FontStyles.Bold;
            label.color = new Color32(255, 235, 173, 255);
            label.alignment = TextAlignmentOptions.Center;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.rectTransform.sizeDelta = new Vector2(_buttonRect.width, buttonHeight);
        }

        var star = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(star.GetComponent<Collider>());
        star.name = "Star";
        star.transform.SetParent(transform, false);
        star.transform.localPosition = new Vector3(0f, h * 0.5f + 0.05f, -0.012f);
        star.transform.localScale = Vector3.one * 0.26f;
        var sr2 = star.GetComponent<Renderer>();
        var sm2 = Own(new Material(Shader.Find("Sprites/Default")));
        sm2.mainTexture = Own(WordStone.MakeStarTexture(64));
        sm2.color = GoldStar;
        sr2.sharedMaterial = sm2;
        sr2.sortingOrder = 101;

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

        var mat = Own(new Material(Shader.Find("Sprites/Default")));
        _leader.sharedMaterial = mat;
        _leader.sortingOrder = 99;
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
        _dot.sharedMaterial = Own(new Material(Shader.Find("Sprites/Default"))
        {
            mainTexture = Own(WordStone.MakeStarTexture(32)),
            color = GoldStar
        });

        UpdateLeader(1f);
    }

    // Re-pin the line every frame: the card floats and billboards, the stone may hop.
    void UpdateLeader(float alpha)
    {
        if (_leader == null || _stone == null) return;
        Vector3 from = _stone.position + Vector3.up * 0.30f;    // just above the stone's own word
        Vector3 to   = transform.position - transform.up * _halfHeight; // the card's real lower edge
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
            _dot.sharedMaterial.color = c;
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
        while ((_action != null || life < 3.2f) && _stone != null && _stone.gameObject.activeInHierarchy)                      // hold ~3 s, gentle float
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
        var cam = _camera != null ? _camera : Camera.main;
        if (cam != null)
            transform.rotation = _action != null ? cam.transform.rotation
                : Quaternion.LookRotation(transform.position - cam.transform.position);
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
