// ===========================================================================
//  WordRiverPath — the river's SHAPE, authored in Blender.
// ===========================================================================
//  A world-space polyline that the word path follows instead of the straight
//  PathStart -> PathEnd line. Art owns the bend (round the boat, past the dock,
//  up to the library); code owns how the words are spread along it.
//
//  Filled by Tools > Great Library > Island > 7. Import River Path From Blender,
//  which reads Assets/Art/Models_Island/WordRiver_Path.json and bakes the points
//  into `points` below. Nothing here loads at runtime — the polyline is plain
//  serialised scene data, so a build carries no JSON and no importer.
//
//  Sampling is by ARC LENGTH, not by segment index: SampleByT(0.5) is the point
//  half way ALONG the river, not the middle vertex. Even word spacing depends on
//  that, and a hand-drawn curve never has even vertex spacing.
// ===========================================================================
using UnityEngine;

public class WordRiverPath : MonoBehaviour
{
    [Tooltip("The river, in world space, start (open water) -> end (the shore). " +
             "Baked from Blender by the importer — you can nudge points by hand " +
             "afterwards, but a re-import overwrites them.")]
    public Vector3[] points = new Vector3[0];

    [Header("Gizmo")]
    public bool drawGizmo = true;
    public Color gizmoColor = new Color(0.95f, 0.80f, 0.40f, 1f);

    float[] _cum;          // cumulative arc length, _cum[i] = distance to points[i]
    int _builtFor = -1;    // points.Length the table was built for

    /// <summary>Total length of the river in world units. 0 if unusable.</summary>
    public float Length { get { EnsureTable(); return Valid ? _cum[_cum.Length - 1] : 0f; } }

    /// <summary>True once there are enough points to sample.</summary>
    public bool Valid => points != null && points.Length >= 2;

    void EnsureTable()
    {
        if (!Valid) { _cum = null; _builtFor = -1; return; }
        if (_cum != null && _builtFor == points.Length) return;
        _cum = new float[points.Length];
        _cum[0] = 0f;
        for (int i = 1; i < points.Length; i++)
            _cum[i] = _cum[i - 1] + Vector3.Distance(points[i - 1], points[i]);
        _builtFor = points.Length;
    }

    /// <summary>Rebuild the arc-length table — call after editing `points`.</summary>
    public void Invalidate() { _cum = null; _builtFor = -1; }

    /// <summary>The point <paramref name="s"/> metres along the river.</summary>
    public Vector3 SampleByDistance(float s)
    {
        EnsureTable();
        if (!Valid) return transform.position;
        float total = _cum[_cum.Length - 1];
        if (total <= 0.0001f) return points[0];
        s = Mathf.Clamp(s, 0f, total);

        // binary search for the segment containing s
        int lo = 0, hi = _cum.Length - 1;
        while (lo + 1 < hi)
        {
            int mid = (lo + hi) >> 1;
            if (_cum[mid] <= s) lo = mid; else hi = mid;
        }
        float seg = _cum[hi] - _cum[lo];
        float f = seg > 0.0001f ? (s - _cum[lo]) / seg : 0f;
        return Vector3.Lerp(points[lo], points[hi], f);
    }

    /// <summary>The point a fraction <paramref name="t"/> (0..1) along the river.</summary>
    public Vector3 SampleByT(float t) => SampleByDistance(Mathf.Clamp01(t) * Length);

    /// <summary>
    /// Which way the river runs at <paramref name="t"/>, flat on the water.
    /// Used to face a stone along the path rather than at a fixed angle.
    /// </summary>
    public Vector3 DirectionAtT(float t)
    {
        float len = Length;
        if (len <= 0.0001f) return Vector3.forward;
        float s = Mathf.Clamp01(t) * len;
        float e = Mathf.Max(0.01f, len * 0.01f);
        Vector3 d = SampleByDistance(Mathf.Min(len, s + e)) -
                    SampleByDistance(Mathf.Max(0f, s - e));
        d.y = 0f;
        return d.sqrMagnitude > 0.000001f ? d.normalized : Vector3.forward;
    }

    void OnDrawGizmos()
    {
        if (!drawGizmo || !Valid) return;
        Gizmos.color = gizmoColor;
        for (int i = 1; i < points.Length; i++)
            Gizmos.DrawLine(points[i - 1], points[i]);
        // ends: open water (start) and the shore (end)
        Gizmos.DrawWireSphere(points[0], 0.25f);
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.6f);
        Gizmos.DrawWireSphere(points[points.Length - 1], 0.35f);
    }
}
