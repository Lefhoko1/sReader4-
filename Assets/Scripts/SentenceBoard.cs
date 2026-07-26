// ===========================================================================
//  SentenceBoard — the paragraph board (SM_SkyBoard / the open book)
// ===========================================================================
//  Shows the passage as a list of sentences on the board. Tap a sentence and
//  it lays itself out along the river as the word path; solved sentences are
//  ticked off and stay lit, so the board doubles as the progress record —
//  "the Library is the scoreboard" (Production Bible).
//
//  The rows live on a BoardFace child that BILLBOARDS toward the camera and is
//  kept upright with world-up. That way the text is never upside-down or
//  mirrored no matter how the board model's own axes are rotated, and the
//  layout auto-fits the board's real renderer bounds.
//
//  Put this on SM_SkyBoard (or the open book). WordPathGame drives it.
// ===========================================================================
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SReader.Domains.Assignments.Models;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class SentenceBoard : MonoBehaviour
{
    [Header("Fit")]
    [Tooltip("Size the rows from the board's own renderer bounds.")]
    public bool fitToBoard = true;
    [Tooltip("How much of the board face the text may use (0-1).")]
    [Range(0.4f, 1f)] public float fillAmount = 0.86f;
    [Tooltip("Push the text this far off the board face, toward the camera.")]
    public float faceOffset = 0.12f;
    [Tooltip("Manual size when Fit To Board is off.")]
    public Vector2 manualSize = new Vector2(3.4f, 2.2f);
    [Tooltip("Extra text scale (1 = auto).")]
    [Range(0.4f, 2f)] public float textScale = 1f;

    [Header("Colours")]
    public Color titleColor = new Color(0.32f, 0.20f, 0.05f);
    public Color rowColor = new Color(0.25f, 0.18f, 0.08f);
    public Color doneColor = new Color(0.30f, 0.48f, 0.20f);
    public Color activeColor = new Color(0.72f, 0.45f, 0.05f);

    public System.Action<int> onSentenceChosen;

    readonly List<Row> _rows = new List<Row>();
    class Row { public GameObject hit; public TextMeshPro label; public int index; public bool done; public string text; }

    Transform _face;
    Bounds _bounds;
    bool _hasBounds;

    // the board's own face plane, resolved from its mesh (not world axes)
    Vector3 _faceCentre, _faceNormal, _faceUp, _faceRight;
    float _faceW = 2f, _faceH = 1.4f, _faceHalfThick = 0.05f;

    /// <summary>Render the passage: a title plus one tappable line per sentence.</summary>
    public void Show(string title, AssignmentContent content)
    {
        Clear();
        if (content == null) return;

        MeasureBoard();
        EnsureFace();

        // gather the sentences first so rows can be sized to fit them all
        var texts = new List<string>();
        foreach (var page in content.pages)
            foreach (var s in page.sentences)
                texts.Add(Flatten(s));

        float w = (fitToBoard && _hasBounds ? _faceW : manualSize.x) * fillAmount;
        float h = (fitToBoard && _hasBounds ? _faceH : manualSize.y) * fillAmount;

        int lines = texts.Count + 1;                       // + the title
        float rowH = h / lines;
        float font = rowH * 0.62f * textScale;
        float top = h * 0.5f - rowH * 0.5f;

        var head = NewText("Title", new Vector3(0f, top, 0f), titleColor, font * 1.15f, w);
        head.text = $"<b>{title}</b>";
        head.alignment = TextAlignmentOptions.Center;

        for (int i = 0; i < texts.Count; i++)
        {
            float y = top - rowH * (i + 1);
            var pos = new Vector3(0f, y, 0f);

            var hit = GameObject.CreatePrimitive(PrimitiveType.Quad);   // tap target
            hit.name = "Row_" + i;
            hit.transform.SetParent(_face, false);
            hit.transform.localPosition = pos + new Vector3(0f, 0f, 0.01f);
            hit.transform.localScale = new Vector3(w, rowH * 0.92f, 1f);
            hit.GetComponent<Renderer>().material =
                new Material(Shader.Find("Sprites/Default")) { color = new Color(1f, 1f, 1f, 0.04f) };

            var lbl = NewText("Row" + i, pos, rowColor, font, w);
            lbl.text = texts[i];

            _rows.Add(new Row { hit = hit, label = lbl, index = i, text = texts[i] });
        }
        Highlight(0);
    }

    /// <summary>Tick a finished sentence — it stays on the board as progress.</summary>
    public void MarkDone(int index)
    {
        var row = _rows.Find(r => r.index == index);
        if (row == null) return;
        row.done = true;
        row.label.text = $"<color=#4C7A33ff>✓</color> {row.text}";
        row.label.color = doneColor;
    }

    /// <summary>Show which sentence is currently laid out on the river.</summary>
    public void Highlight(int index)
    {
        foreach (var r in _rows)
        {
            if (r.done) continue;
            bool on = r.index == index;
            r.label.color = on ? activeColor : rowColor;
            r.label.text = on ? $"<b>{r.text}</b>" : r.text;
        }
    }

    public bool AllDone() { foreach (var r in _rows) if (!r.done) return false; return _rows.Count > 0; }

    // ── the billboarding face ───────────────────────────────────────────────

    // Resolve the board's flat face from its MESH: the axis with the smallest
    // extent is the panel's normal; of the remaining two, the one closest to world
    // up becomes "up". This keeps text ON the board, upright, whatever the model's
    // own axes do — world-axis guessing is what put the text off the panel before.
    void MeasureBoard()
    {
        _hasBounds = false;
        var mf = GetComponentInChildren<MeshFilter>();
        var rend = GetComponentInChildren<Renderer>();

        if (mf != null && mf.sharedMesh != null)
        {
            var mt = mf.transform;
            var lb = mf.sharedMesh.bounds;
            var ls = mt.lossyScale;
            var size = new Vector3(lb.size.x * Mathf.Abs(ls.x),
                                   lb.size.y * Mathf.Abs(ls.y),
                                   lb.size.z * Mathf.Abs(ls.z));
            var dirs = new[] { mt.right, mt.up, mt.forward };

            int thin = size.x <= size.y && size.x <= size.z ? 0 : (size.y <= size.z ? 1 : 2);
            int a = (thin + 1) % 3, b = (thin + 2) % 3;
            // of the two in-plane axes, the more vertical one is "up"
            int upIdx = Mathf.Abs(Vector3.Dot(dirs[a], Vector3.up)) >=
                        Mathf.Abs(Vector3.Dot(dirs[b], Vector3.up)) ? a : b;
            int rightIdx = upIdx == a ? b : a;

            _faceCentre = mt.TransformPoint(lb.center);
            _faceNormal = dirs[thin].normalized;
            _faceUp = dirs[upIdx].normalized;
            if (Vector3.Dot(_faceUp, Vector3.up) < 0f) _faceUp = -_faceUp;
            _faceRight = dirs[rightIdx].normalized;
            _faceH = size[upIdx];
            _faceW = size[rightIdx];
            _faceHalfThick = size[thin] * 0.5f;
            _hasBounds = true;
        }
        else if (rend != null)
        {
            _bounds = rend.bounds;
            _faceCentre = _bounds.center;
            _faceNormal = Vector3.forward; _faceUp = Vector3.up; _faceRight = Vector3.right;
            _faceW = Mathf.Max(_bounds.size.x, _bounds.size.z);
            _faceH = _bounds.size.y;
            _faceHalfThick = 0.05f;
            _hasBounds = true;
        }
        else
        {
            _faceCentre = transform.position;
            _faceNormal = Vector3.forward; _faceUp = Vector3.up; _faceRight = Vector3.right;
            _faceW = manualSize.x; _faceH = manualSize.y; _faceHalfThick = 0.05f;
        }
    }

    float BoardWidth() => _faceW;

    void EnsureFace()
    {
        if (_face != null) return;
        var go = new GameObject("BoardFace");
        go.transform.SetParent(transform, true);
        go.transform.localScale = Vector3.one;          // immune to the board's own scaling
        _face = go.transform;
        PlaceFace();
    }

    // Keep the text plate in front of the board and square to the camera, upright.
    void LateUpdate() => PlaceFace();

    void PlaceFace()
    {
        if (_face == null) return;
        var cam = Camera.main; if (cam == null) return;
        if (!_hasBounds) MeasureBoard();

        // use whichever side of the panel the camera is on
        Vector3 n = _faceNormal;
        if (Vector3.Dot(n, cam.transform.position - _faceCentre) < 0f) n = -n;

        _face.position = _faceCentre + n * (_faceHalfThick + faceOffset);
        // forward points AWAY from the viewer => TMP reads correctly; _faceUp keeps it upright
        _face.rotation = Quaternion.LookRotation(-n, _faceUp);
        _face.localScale = Vector3.one;
    }

    // ── input ───────────────────────────────────────────────────────────────

    void Update()
    {
        if (WordStone.InputLocked) return;              // a challenge owns input
        if (!PointerDown(out var sp)) return;
        var cam = Camera.main; if (cam == null) return;
        if (!Physics.Raycast(cam.ScreenPointToRay(sp), out var hit, 300f)) return;
        foreach (var r in _rows)
            if (r.hit == hit.collider.gameObject && !r.done)
            { onSentenceChosen?.Invoke(r.index); return; }
    }

    TextMeshPro NewText(string name, Vector3 localPos, Color color, float size, float width)
    {
        var go = new GameObject(name);
        go.transform.SetParent(_face, false);
        go.transform.localPosition = localPos;
        go.transform.localRotation = Quaternion.identity;
        var t = go.AddComponent<TextMeshPro>();
        t.color = color;
        t.alignment = TextAlignmentOptions.Center;
        t.enableAutoSizing = true;                      // shrink to fit long sentences
        t.fontSizeMin = size * 0.35f;
        t.fontSizeMax = size;
        t.rectTransform.sizeDelta = new Vector2(width, Mathf.Max(0.2f, size * 1.6f));
        return t;
    }

    public void Clear()
    {
        foreach (var r in _rows)
        {
            if (r.hit) Destroy(r.hit);
            if (r.label) Destroy(r.label.gameObject);
        }
        _rows.Clear();
        if (_face != null)
            for (int i = _face.childCount - 1; i >= 0; i--)
                Destroy(_face.GetChild(i).gameObject);
    }

    public static string Flatten(ContentSentence s)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var t in s.tokens)
        {
            if (!t.isWord) { sb.Append(t.text); continue; }
            sb.Append(sb.Length > 0 && sb[sb.Length - 1] != ' ' ? " " : "").Append(t.text);
        }
        var text = sb.ToString().Trim();
        return text.EndsWith(".") ? text : text + ".";
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
}
