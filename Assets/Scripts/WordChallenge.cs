// ===========================================================================
//  WordChallenge — the in-world Define / Fill / Illustrate challenge
// ===========================================================================
//  Opened when a student taps a glowing key stone. Floats a parchment card
//  above the stone with the prompt, and lays the pieces out as tappable
//  tablets in front of it:
//
//    • FILL      "Spell the word"      -> tap the letter runes in order
//    • DEFINE    "What does it mean?"  -> tap the meaning shards in order
//    • ILLUSTRATE"Which picture shows" -> tap the true vision
//
//  ALL CHECKING IS THE EXISTING, TESTED GateSolving — this class only renders
//  and forwards input (MVVM). Wrong taps never say "wrong": the piece wavers
//  and returns, per the Production Bible's restoration grammar.
//
//  WordChallenge.Open(stone, onSolved) — that's the whole API.
// ===========================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SReader.Domains.Assignments.Models;
using SReader.Game.Trek;                     // GateSolving
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class WordChallenge : MonoBehaviour
{
    static WordChallenge _current;
    public static bool IsOpen => _current != null;

    // palette (matches KeywordCard / the gold key stone)
    static readonly Color Parchment = new Color(1f, 0.95f, 0.82f);
    static readonly Color GoldEdge  = new Color(0.85f, 0.62f, 0.20f);
    static readonly Color Ink       = new Color(0.30f, 0.20f, 0.06f);

    WordStone _stone;
    ContentToken _token;
    System.Action _onSolved;

    readonly List<Piece> _pieces = new List<Piece>();
    readonly List<string> _answer = new List<string>();
    List<string> _correct;
    ActivityType _kind;
    TextMeshPro _prompt, _progress;
    Transform _card;
    bool _busy;

    class Piece
    {
        public GameObject go;
        public TextMeshPro label;
        public Renderer rend;
        public string value;
        public Vector3 home;
    }

    public static void Open(WordStone stone, System.Action onSolved)
    {
        if (_current != null) Destroy(_current.gameObject);
        if (stone == null || stone.token == null) return;
        var go = new GameObject("WordChallenge");
        _current = go.AddComponent<WordChallenge>();
        _current.Build(stone, onSolved);
    }

    public static void CloseAny()
    {
        if (_current != null) Destroy(_current.gameObject);
        _current = null;
        WordStone.InputLocked = false;
    }

    void OnDestroy() { if (_current == this) { _current = null; WordStone.InputLocked = false; } }

    // ── build ──────────────────────────────────────────────────────────────

    void Build(WordStone stone, System.Action onSolved)
    {
        _stone = stone; _token = stone.token; _onSolved = onSolved;
        _kind = _token.activity;
        WordStone.InputLocked = true;                  // stones stop listening while we're up

        transform.position = stone.transform.position + Vector3.up * 1.55f;

        List<string> pool;
        string prompt;
        switch (_kind)
        {
            case ActivityType.Define:
                _correct = GateSolving.DefineCorrect(_token);
                pool     = GateSolving.DefinePool(_token);
                prompt   = $"What does <b>{_token.text}</b> mean?\n<size=60%>Tap the pieces in order</size>";
                break;
            case ActivityType.Illustrate:
                _correct = new List<string> { _token.correctImage };
                pool     = GateSolving.IllustrateOptions(_token);
                prompt   = $"Which picture shows <b>{_token.text}</b>?";
                break;
            default:   // FillBlank
                _correct = GateSolving.FillCorrect(_token);
                pool     = GateSolving.FillPool(_token);
                prompt   = $"Spell the word\n<size=60%>Tap the letters in order</size>";
                break;
        }

        BuildCard(prompt);
        LayOutPieces(pool);
        UpdateProgress();
        StartCoroutine(PopIn());
    }

    void BuildCard(string prompt)
    {
        var card = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(card.GetComponent<Collider>());
        card.name = "Card";
        card.transform.SetParent(transform, false);
        card.transform.localPosition = new Vector3(0f, 0.45f, 0.02f);
        card.transform.localScale = new Vector3(3.1f, 1.15f, 1f);
        var r = card.GetComponent<Renderer>();
        r.material = new Material(Shader.Find("Sprites/Default"))
        { mainTexture = Panel(320, 128, 26, Parchment, GoldEdge) };
        _card = card.transform;

        _prompt = new GameObject("Prompt").AddComponent<TextMeshPro>();
        _prompt.transform.SetParent(transform, false);
        _prompt.transform.localPosition = new Vector3(0f, 0.55f, -0.01f);
        _prompt.text = prompt;
        _prompt.color = Ink;
        _prompt.alignment = TextAlignmentOptions.Center;
        _prompt.rectTransform.sizeDelta = new Vector2(3.0f, 1.0f);
        _prompt.enableAutoSizing = true;             // a long key word must not spill
        _prompt.fontSizeMin = 0.9f;                  // off the parchment card
        _prompt.fontSizeMax = 2.2f;

        _progress = new GameObject("Progress").AddComponent<TextMeshPro>();
        _progress.transform.SetParent(transform, false);
        _progress.transform.localPosition = new Vector3(0f, 0.06f, -0.01f);
        _progress.fontSize = 1.9f;
        _progress.color = new Color(0.5f, 0.35f, 0.1f);
        _progress.alignment = TextAlignmentOptions.Center;
        _progress.rectTransform.sizeDelta = new Vector2(3.0f, 0.5f);
    }

    void LayOutPieces(List<string> pool)
    {
        bool wide = _kind != ActivityType.FillBlank;      // words/images need wider tiles
        float w = wide ? 1.15f : 0.44f;
        float gap = w + 0.10f;
        int perRow = wide ? 3 : 8;

        for (int i = 0; i < pool.Count; i++)
        {
            int row = i / perRow, col = i % perRow;
            int inRow = Mathf.Min(perRow, pool.Count - row * perRow);
            float x = (col - (inRow - 1) * 0.5f) * gap;
            float y = -0.35f - row * (wide ? 0.52f : 0.50f);
            _pieces.Add(MakePiece(pool[i], new Vector3(x, y, 0f), w, wide));
        }
    }

    Piece MakePiece(string value, Vector3 local, float width, bool wide)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = "Piece_" + value;
        go.transform.SetParent(transform, false);
        go.transform.localPosition = local;
        go.transform.localScale = new Vector3(width, wide ? 0.42f : 0.44f, 1f);
        var rend = go.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("Sprites/Default"))
        { mainTexture = Panel(160, 96, 20, new Color(0.99f, 0.97f, 0.90f), GoldEdge) };

        var label = new GameObject("L").AddComponent<TextMeshPro>();
        label.transform.SetParent(go.transform, false);
        label.transform.localPosition = new Vector3(0f, 0f, -0.01f);
        label.text = value;
        label.color = Ink;
        label.alignment = TextAlignmentOptions.Center;
        label.enableAutoSizing = true; label.fontSizeMin = 0.4f; label.fontSizeMax = wide ? 2.6f : 4.0f;
        // the label is parented to a non-uniformly scaled quad, so counter-scale it
        label.rectTransform.sizeDelta = new Vector2(0.92f, 0.8f);

        return new Piece { go = go, label = label, rend = rend, value = value, home = local };
    }

    // ── input ──────────────────────────────────────────────────────────────

    void Update()
    {
        Face();
        if (_busy) return;
        if (!PointerDown(out var sp)) return;
        var cam = Camera.main; if (cam == null) return;
        if (!Physics.Raycast(cam.ScreenPointToRay(sp), out var hit, 200f)) return;

        foreach (var p in _pieces)
            if (p.go == hit.collider.gameObject) { Tapped(p); return; }
    }

    void Tapped(Piece p)
    {
        if (_kind == ActivityType.Illustrate)
        {
            if (GateSolving.CheckIllustrate(p.value, _token)) StartCoroutine(Solved());
            else StartCoroutine(Waver(p));
            return;
        }

        // Fill / Define: pieces are tapped in reading order.
        int idx = _answer.Count;
        if (idx < _correct.Count &&
            string.Equals(p.value, _correct[idx], System.StringComparison.OrdinalIgnoreCase))
        {
            _answer.Add(p.value);
            Consume(p);
            UpdateProgress();
            bool done = _answer.Count == _correct.Count &&
                (_kind == ActivityType.FillBlank
                    ? GateSolving.CheckFill(_answer, _correct)
                    : GateSolving.CheckDefine(_answer, _correct));
            if (done) StartCoroutine(Solved());
        }
        else StartCoroutine(Waver(p));    // "not yet" — never a buzzer
    }

    void Consume(Piece p)
    {
        p.rend.material.color = new Color(0.85f, 0.78f, 0.60f, 0.55f);
        if (p.label != null) p.label.color = new Color(0.55f, 0.45f, 0.25f, 0.7f);
        var col = p.go.GetComponent<Collider>(); if (col) col.enabled = false;
        StartCoroutine(Sink(p.go.transform));
    }

    void UpdateProgress()
    {
        if (_progress == null) return;
        if (_kind == ActivityType.Illustrate) { _progress.text = ""; return; }
        _progress.text = _answer.Count == 0
            ? new string('·', _correct.Count)
            : string.Join(_kind == ActivityType.FillBlank ? "" : " ", _answer);
    }

    IEnumerator Solved()
    {
        _busy = true;
        WordStone.PlayKeywordBurst(_stone.transform.position + Vector3.up * 0.2f, _stone.transform);
        if (_prompt != null) { _prompt.text = "<b>Restored!</b>"; _prompt.color = new Color(0.2f, 0.45f, 0.15f); }
        yield return new WaitForSeconds(0.65f);
        _stone.MarkSolved();
        var cb = _onSolved;
        CloseAny();
        cb?.Invoke();
    }

    IEnumerator Waver(Piece p)
    {
        var t = p.go.transform;
        Vector3 home = p.home;
        for (float e = 0; e < 0.32f; e += Time.deltaTime)
        {
            t.localPosition = home + Vector3.right * Mathf.Sin(e * 55f) * 0.05f;
            yield return null;
        }
        t.localPosition = home;
    }

    IEnumerator Sink(Transform t)
    {
        Vector3 s = t.localScale;
        for (float e = 0; e < 0.2f; e += Time.deltaTime)
        { t.localScale = s * (1f - 0.35f * (e / 0.2f)); yield return null; }
        t.localScale = s * 0.65f;
    }

    IEnumerator PopIn()
    {
        Vector3 target = Vector3.one;
        transform.localScale = Vector3.zero;
        for (float e = 0; e < 0.22f; e += Time.deltaTime)
        { transform.localScale = target * Mathf.SmoothStep(0, 1, e / 0.22f); Face(); yield return null; }
        transform.localScale = target;
    }

    void Face()
    {
        var cam = Camera.main;
        if (cam != null)
            transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }

    static bool PointerDown(out Vector2 pos)
    {
        pos = default;
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        { pos = Touchscreen.current.primaryTouch.position.ReadValue(); return true; }
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        { pos = Mouse.current.position.ReadValue(); return true; }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetMouseButtonDown(0)) { pos = Input.mousePosition; return true; }
#endif
        return false;
    }

    // rounded parchment panel texture (shared look with KeywordCard)
    public static Texture2D Panel(int w, int h, int rad, Color fill, Color edge)
    {
        var t = new Texture2D(w, h, TextureFormat.RGBA32, false);
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            int cx = Mathf.Clamp(x, rad, w - rad), cy = Mathf.Clamp(y, rad, h - rad);
            float d = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
            float a = Mathf.Clamp01(rad - d + 1);
            bool border = d > rad - 6 && d <= rad;
            t.SetPixel(x, y, border ? new Color(edge.r, edge.g, edge.b, a)
                                    : new Color(fill.r, fill.g, fill.b, a));
        }
        t.Apply(); return t;
    }
}
