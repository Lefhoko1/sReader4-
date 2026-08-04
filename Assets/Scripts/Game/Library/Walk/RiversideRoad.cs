// ===========================================================================
//  RiversideRoad — the towpath: one continuous walk from open water to the mat
// ===========================================================================
//  The stones are the RIVER; this is the BANK beside them. It is a single
//  smooth curve, built from things already in the scene, in this order:
//
//      lead-in  →  one point beside every stone  →  the dock / the entrance
//                                                →  the reading mat
//
//  Everything downstream (the walker, the camera) asks this class two things
//  only: "where am I at N metres along?" and "how far along is stone i?".
//  That is the whole point of a road object — the walk is one 1-D problem
//  instead of a pile of world-space special cases.
//
//  WHY A ROAD AT ALL. The old river was composed for a FIXED camera: the
//  stones were shrunk, spaced and slid about on screen so a far stone read as
//  big as a near one (WordPathBuilder's perspective compensation). A camera
//  that walks with the reader does that job for free — every stone is a near
//  stone when you are standing next to it. So this road expects the river laid
//  out in plain WORLD space; the setup menu turns the compensation off.
//
//  Runs in the editor too ([ExecuteAlways]) so you can see the road without
//  pressing Play. The mesh is generated, never saved — it is rebuilt on load.
// ===========================================================================
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RiversideRoad : MonoBehaviour
{
    [Header("What the road follows")]
    [Tooltip("The river of word stones. The road runs alongside its stones, in " +
             "reading order. Leave empty to find the one in the scene.")]
    public WordPathBuilder path;

    [Tooltip("Points the walk passes through AFTER the last stone, in order — " +
             "the dock, then the library entrance. This is the climb.")]
    public List<Transform> climbWaypoints = new List<Transform>();

    [Tooltip("Where the walk ends: the reading mat. The walker sits here facing " +
             "the mat's forward (blue) axis.")]
    public Transform seat;

    [Header("Shape")]
    [Tooltip("How far to the side of the stones the road runs, in metres.")]
    public float sideOffset = 2.0f;
    [Tooltip("Which bank. 0 = pick automatically (the far side from the camera, " +
             "so the stones stay in the foreground and the walker is never " +
             "standing on top of a word). +1 / -1 to force it.")]
    [Range(-1, 1)] public int side = 0;
    [Tooltip("Road laid before the first stone, so the walk starts by walking IN " +
             "rather than popping into place.")]
    public float leadIn = 3.5f;
    [Tooltip("Road width in metres.")]
    public float width = 1.7f;
    [Tooltip("Lifts the road off the water so it never z-fights the sea.")]
    public float surfaceRise = 0.03f;
    [Tooltip("Walk on TOP of the stones rather than at their pivots. A stone's " +
             "pivot is its centre, so a reader placed at pivot height wades " +
             "through it at waist level — the top of its bounds is the surface a " +
             "person would actually stand on. Only makes sense while the road " +
             "runs over the stones (side offset near 0); step off them and this " +
             "leaves the reader hovering at stone height over open water.")]
    public bool standOnStones = true;
    [Tooltip("Metres between sample points. Smaller = smoother curve and mesh.")]
    [Range(0.1f, 1f)] public float resolution = 0.35f;

    [Header("Look")]
    [Tooltip("Draw the road as a sand ribbon. Turn it OFF wherever the world " +
             "already has a surface to walk on — the stair modelled into the " +
             "island, the stones themselves — so the road stays a ROUTE and " +
             "nothing gets extruded over the top of the authored art.")]
    public bool drawMesh = true;
    public Material roadMaterial;
    [Tooltip("Used only when no material is assigned (a plain lit sand colour).")]
    public Color roadColor = new Color(0.78f, 0.70f, 0.53f);
    [Tooltip("Metres of road per texture tile, lengthways.")]
    public float uvTile = 4f;

    // ── what the walker and the camera read ─────────────────────────────────

    /// <summary>Total walkable length, in metres.</summary>
    public float Length => _cum.Count > 0 ? _cum[_cum.Count - 1] : 0f;

    /// <summary>How many stones the walk stops at.</summary>
    public int StopCount => _stopDist.Count;

    /// <summary>Distance along the road of the stop beside stone <paramref name="i"/>.</summary>
    public float StopDistance(int i) =>
        i >= 0 && i < _stopDist.Count ? _stopDist[i] : 0f;

    /// <summary>The stone the walk stops at, for facing and for framing.</summary>
    public Transform Stone(int i) => i >= 0 && i < _stones.Count ? _stones[i] : null;

    /// <summary>Which bank the road ended up on. The camera sits on the other one.</summary>
    public int SideSign { get; private set; } = 1;

    readonly List<Vector3> _pts = new List<Vector3>();     // sampled centre line
    readonly List<float> _cum = new List<float>();         // arc length at each point
    readonly List<float> _stopDist = new List<float>();    // arc length of each stop
    readonly List<Transform> _stones = new List<Transform>();
    Mesh _mesh;
    Material _runtimeMat;

    void OnEnable() { Rebuild(); }

    /// <summary>
    /// Re-read the scene and lay the road out again. Cheap — call it whenever the
    /// sentence changes and the stones move.
    /// </summary>
    [ContextMenu("Rebuild")]
    public void Rebuild()
    {
        _pts.Clear(); _cum.Clear(); _stopDist.Clear();
        CollectStones();

        var knots = new List<Vector3>();
        var stoneKnot = new List<int>();
        if (!BuildKnots(knots, stoneKnot)) { ApplyMesh(null); return; }

        // ── sample a Catmull-Rom through the knots, keeping arc length ───────
        // The knots are corners; the walk has to be a curve, or the walker snaps
        // its heading at every stone. Arc length is accumulated as we go, which is
        // what turns "stone 4" into "11.8 metres along".
        var knotDist = new float[knots.Count];
        _pts.Add(knots[0]); _cum.Add(0f); knotDist[0] = 0f;

        for (int j = 0; j < knots.Count - 1; j++)
        {
            Vector3 p0 = knots[Mathf.Max(0, j - 1)];
            Vector3 p1 = knots[j];
            Vector3 p2 = knots[j + 1];
            Vector3 p3 = knots[Mathf.Min(knots.Count - 1, j + 2)];

            int steps = Mathf.Max(1, Mathf.CeilToInt(
                Vector3.Distance(p1, p2) / Mathf.Max(0.05f, resolution)));
            for (int s = 1; s <= steps; s++)
            {
                Vector3 p = CatmullRom(p0, p1, p2, p3, s / (float)steps);
                _cum.Add(_cum[_cum.Count - 1] + Vector3.Distance(_pts[_pts.Count - 1], p));
                _pts.Add(p);
            }
            knotDist[j + 1] = _cum[_cum.Count - 1];
        }

        foreach (int k in stoneKnot) _stopDist.Add(knotDist[Mathf.Clamp(k, 0, knots.Count - 1)]);

        ApplyMesh(drawMesh ? BuildRibbon() : null);
    }

    /// <summary>The corners of the walk, and which of them a stone sits beside.</summary>
    bool BuildKnots(List<Vector3> knots, List<int> stoneKnot)
    {
        if (_stones.Count == 0)
        {
            // Nothing to walk past yet — the sentence has not been laid out. Not an
            // error: the demo builds the sentence first and then calls Rebuild().
            return false;
        }

        // Which bank? The one further from the camera, so the reader walks BEHIND
        // the words instead of in front of them.
        Vector3 dir = Direction();
        Vector3 lateral = Vector3.Cross(Vector3.up, dir).normalized;
        SideSign = side != 0 ? side : PickSide(lateral);

        for (int i = 0; i < _stones.Count; i++)
        {
            Vector3 p = _stones[i].position + lateral * (SideSign * sideOffset);
            p.y = StandHeight(_stones[i]) + surfaceRise;
            stoneKnot.Add(knots.Count);
            knots.Add(p);
        }

        // lead-in, so the walk starts moving rather than starting stopped
        Vector3 first = knots[0];
        Vector3 back = knots.Count > 1 ? (knots[0] - knots[1]).normalized : -dir;
        knots.Insert(0, first + back * Mathf.Max(0f, leadIn));
        for (int i = 0; i < stoneKnot.Count; i++) stoneKnot[i] += 1;

        // the climb: dock, entrance, … then the mat
        foreach (var t in climbWaypoints)
            if (t != null) knots.Add(t.position + Vector3.up * surfaceRise);
        if (seat != null) knots.Add(seat.position + Vector3.up * surfaceRise);

        return knots.Count >= 2;
    }

    /// <summary>
    /// The height the reader's feet go at this stone: the top of the stone when
    /// walking over it, its pivot otherwise. Read from the RENDERER's bounds, so
    /// it follows however the builder scaled that particular stone — key stones
    /// are 25% bigger than plain ones and would otherwise be waded through.
    /// </summary>
    float StandHeight(Transform stone)
    {
        if (!standOnStones) return stone.position.y;
        var r = stone.GetComponentInChildren<Renderer>();
        return r != null ? r.bounds.max.y : stone.position.y;
    }

    /// <summary>Stones in reading order — from the live list, or the built children.</summary>
    void CollectStones()
    {
        _stones.Clear();
        if (path == null) path = FindAnyObjectByType<WordPathBuilder>();
        if (path == null) return;

        // In play mode the builder hands us its list. In the editor (and after a
        // domain reload) that list is gone but the stones are still there as
        // children, in build order — which is reading order.
        if (path.Stones != null && path.Stones.Count > 0)
        {
            foreach (var s in path.Stones) if (s != null) _stones.Add(s.transform);
            if (_stones.Count > 0) return;
        }
        foreach (Transform c in path.transform)
            if (c.GetComponent<WordStone>() != null) _stones.Add(c);
        if (_stones.Count == 0) CollectAuthored();
    }

    /// <summary>
    /// The stones authored in Blender, which live under the WORLD, not under the
    /// builder — so neither the live list nor the child scan above can find them.
    ///
    /// This is the path that matters after a script recompile or a fresh Play: the
    /// builder's list is a plain List and does not survive a domain reload, and
    /// without this the road silently rebuilds to ZERO stops. The stones are still
    /// standing there in the river; nothing could see them.
    ///
    /// Read from the SCENE, not from runtime state: a slot's `Raised` flag is also
    /// lost on reload, but whether its GameObject is switched on is saved with the
    /// scene, and a sunk slot is a switched-off one.
    /// </summary>
    void CollectAuthored()
    {
        var slots = new List<StoneSlot>(
            FindObjectsByType<StoneSlot>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        slots.RemoveAll(s => s == null || !s.name.StartsWith("WORDSLOT_") ||
                             !s.gameObject.activeSelf);
        slots.Sort((a, b) => string.CompareOrdinal(a.name, b.name));   // name order = reading order

        foreach (var s in slots)
        {
            Transform t = null;
            if (s.normal != null && s.normal.gameObject.activeSelf) t = s.normal.transform;
            else if (s.key != null && s.key.gameObject.activeSelf) t = s.key.transform;
            if (t != null) _stones.Add(t);
        }
    }

    Vector3 Direction()
    {
        if (_stones.Count >= 2)
        {
            Vector3 d = _stones[_stones.Count - 1].position - _stones[0].position;
            d.y = 0f;
            if (d.sqrMagnitude > 0.001f) return d.normalized;
        }
        if (path != null && path.startPoint != null && path.endPoint != null)
        {
            Vector3 d = path.endPoint.position - path.startPoint.position;
            d.y = 0f;
            if (d.sqrMagnitude > 0.001f) return d.normalized;
        }
        return Vector3.forward;
    }

    int PickSide(Vector3 lateral)
    {
        var cam = Camera.main;
        if (cam == null || _stones.Count == 0) return 1;
        Vector3 mid = _stones[_stones.Count / 2].position;
        float plus = Vector3.Distance(cam.transform.position, mid + lateral * sideOffset);
        float minus = Vector3.Distance(cam.transform.position, mid - lateral * sideOffset);
        return plus >= minus ? 1 : -1;
    }

    // ── sampling the finished road ──────────────────────────────────────────

    /// <summary>Where the walker stands, <paramref name="d"/> metres along.</summary>
    public Vector3 PositionAt(float d)
    {
        if (_pts.Count == 0) return transform.position;
        if (_pts.Count == 1) return _pts[0];
        d = Mathf.Clamp(d, 0f, Length);
        int i = IndexAt(d);
        float seg = _cum[i + 1] - _cum[i];
        float t = seg > 1e-5f ? (d - _cum[i]) / seg : 0f;
        return Vector3.Lerp(_pts[i], _pts[i + 1], t);
    }

    /// <summary>
    /// How far along the road the point nearest <paramref name="world"/> is.
    /// Used to steer the walk by pointer: the road stays a 1-D problem, and
    /// "where the finger is" becomes just another distance along it.
    /// </summary>
    public float NearestDistance(Vector3 world)
    {
        if (_pts.Count == 0) return 0f;
        if (_pts.Count == 1) return 0f;

        float best = 0f, bestSqr = float.MaxValue;
        for (int i = 1; i < _pts.Count; i++)
        {
            Vector3 a = _pts[i - 1], b = _pts[i];
            Vector3 ab = b - a;
            float len2 = ab.sqrMagnitude;
            if (len2 < 1e-6f) continue;

            // clamped projection onto this segment, flat: height must not decide
            // which part of the road a tap on the water is nearest to
            float t = Mathf.Clamp01(Vector3.Dot(world - a, ab) / len2);
            Vector3 p = a + ab * t;
            Vector3 gap = p - world; gap.y = 0f;

            float sqr = gap.sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = Mathf.Lerp(_cum[i - 1], _cum[i], t);
            }
        }
        return best;
    }

    /// <summary>Which way the road is heading, <paramref name="d"/> metres along.</summary>
    public Vector3 TangentAt(float d)
    {
        if (_pts.Count < 2) return Vector3.forward;
        float a = Mathf.Clamp(d - 0.25f, 0f, Length);
        float b = Mathf.Clamp(d + 0.25f, 0f, Length);
        Vector3 t = PositionAt(b) - PositionAt(a);
        return t.sqrMagnitude > 1e-6f ? t.normalized
                                      : (_pts[_pts.Count - 1] - _pts[0]).normalized;
    }

    int IndexAt(float d)
    {
        int lo = 0, hi = _cum.Count - 1;
        while (lo < hi - 1)
        {
            int mid = (lo + hi) / 2;
            if (_cum[mid] <= d) lo = mid; else hi = mid;
        }
        return Mathf.Clamp(lo, 0, _pts.Count - 2);
    }

    // ── the mesh ────────────────────────────────────────────────────────────

    /// <summary>A flat ribbon along the centre line, tapered in at both ends.</summary>
    Mesh BuildRibbon()
    {
        int n = _pts.Count;
        if (n < 2) return null;

        var verts = new Vector3[n * 2];
        var norms = new Vector3[n * 2];
        var uvs = new Vector2[n * 2];
        var tris = new int[(n - 1) * 6];

        for (int i = 0; i < n; i++)
        {
            Vector3 fwd = i == 0 ? _pts[1] - _pts[0]
                        : i == n - 1 ? _pts[n - 1] - _pts[n - 2]
                        : _pts[i + 1] - _pts[i - 1];
            fwd.y = 0f;
            Vector3 perp = Vector3.Cross(Vector3.up, fwd.sqrMagnitude > 1e-6f
                                                     ? fwd.normalized : Vector3.forward);
            float w = width * 0.5f * EndTaper(_cum[i]);
            verts[i * 2 + 0] = transform.InverseTransformPoint(_pts[i] - perp * w);
            verts[i * 2 + 1] = transform.InverseTransformPoint(_pts[i] + perp * w);
            norms[i * 2 + 0] = norms[i * 2 + 1] = Vector3.up;
            float v = _cum[i] / Mathf.Max(0.01f, uvTile);
            uvs[i * 2 + 0] = new Vector2(0f, v);
            uvs[i * 2 + 1] = new Vector2(1f, v);
        }

        for (int i = 0; i < n - 1; i++)
        {
            int t = i * 6, a = i * 2;
            tris[t + 0] = a; tris[t + 1] = a + 2; tris[t + 2] = a + 1;
            tris[t + 3] = a + 1; tris[t + 4] = a + 2; tris[t + 5] = a + 3;
        }

        var m = new Mesh { name = "RiversideRoad" };
        m.indexFormat = n * 2 > 65000
            ? UnityEngine.Rendering.IndexFormat.UInt32
            : UnityEngine.Rendering.IndexFormat.UInt16;
        m.vertices = verts; m.normals = norms; m.uv = uvs; m.triangles = tris;
        m.RecalculateBounds();
        return m;
    }

    /// <summary>Narrows the very ends so the road doesn't stop in a hard rectangle.</summary>
    float EndTaper(float d)
    {
        const float fade = 1.4f;
        float a = Mathf.Clamp01(d / fade);
        float b = Mathf.Clamp01((Length - d) / fade);
        return Mathf.Lerp(0.35f, 1f, Mathf.Min(a, b));
    }

    void ApplyMesh(Mesh m)
    {
        var mf = GetComponent<MeshFilter>();
        var mr = GetComponent<MeshRenderer>();

        if (_mesh != null) DestroyMesh(_mesh);
        _mesh = m;
        if (_mesh != null) _mesh.hideFlags = HideFlags.DontSave;   // regenerated on load
        mf.sharedMesh = _mesh;

        if (roadMaterial != null) mr.sharedMaterial = roadMaterial;
        else
        {
            if (_runtimeMat == null)
            {
                var sh = Shader.Find("Universal Render Pipeline/Lit") ??
                         Shader.Find("Standard");
                _runtimeMat = new Material(sh) { name = "M_Road (runtime)",
                                                 hideFlags = HideFlags.DontSave };
            }
            if (_runtimeMat.HasProperty("_BaseColor")) _runtimeMat.SetColor("_BaseColor", roadColor);
            if (_runtimeMat.HasProperty("_Color")) _runtimeMat.SetColor("_Color", roadColor);
            if (_runtimeMat.HasProperty("_Smoothness")) _runtimeMat.SetFloat("_Smoothness", 0.12f);
            mr.sharedMaterial = _runtimeMat;
        }
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    static void DestroyMesh(Object o)
    {
        if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
    }

    static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t, t3 = t2 * t;
        return 0.5f * ((2f * p1) +
                       (-p0 + p2) * t +
                       (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                       (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.85f, 0.4f, 0.9f);
        for (int i = 1; i < _pts.Count; i++) Gizmos.DrawLine(_pts[i - 1], _pts[i]);
        Gizmos.color = new Color(0.3f, 0.9f, 1f, 0.9f);
        for (int i = 0; i < _stopDist.Count; i++)
            Gizmos.DrawWireSphere(PositionAt(_stopDist[i]), 0.35f);
    }
}
