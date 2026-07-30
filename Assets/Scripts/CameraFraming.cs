// ===========================================================================
//  CameraFraming — frame the whole scene so near and far read the SAME SIZE
// ===========================================================================
//  The stones look wrong for one reason only: the camera is close to them, so
//  the near stone is ~6 units away and the far one ~14. Apparent size goes as
//  1/distance, so the far stone renders at half the size — and no amount of
//  per-stone fiddling downstream is really fixing that, it is only papering
//  over it.
//
//  The honest fix is the photographer's one: STEP BACK AND ZOOM IN. Move the
//  camera far away along the same viewing angle and narrow the field of view to
//  keep the same framing. The distances become (say) 50 and 58 instead of 6 and
//  14, so their ratio falls from 2.1 to 1.16 and everything on the water reads
//  at nearly the same size. A long lens flattens depth; that is exactly what is
//  wanted here.
//
//  Trade-off: the flatter it gets, the less the scene feels three-dimensional.
//  `depthEvenness` is that dial — 1.0 would be perfectly flat (orthographic),
//  2.0 is roughly what you have now.
//
//  USE: put this on the Main Camera, right-click the component ▸ Frame Now.
//  It only ever writes the camera's position, FOV and clip planes — the viewing
//  ANGLE is yours and is never touched.
// ===========================================================================
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFraming : MonoBehaviour
{
    [Header("What has to be on screen")]
    [Tooltip("Leave empty to find them automatically: the storybook, the word path " +
             "(its start/end points and stones) and the island.")]
    public Transform book;
    public Transform stones;
    public Transform island;

    [Header("Framing")]
    [Tooltip("How much bigger the NEAREST STONE may look than the farthest one. " +
             "1.1 = almost perfectly even (a long lens, flat). " +
             "2.0 = strong perspective (a wide lens, deep). " +
             "Lower values push the camera further back and narrow the lens.\n\n" +
             "Measured over the WORD PATH only — the book sits right under the lens " +
             "and the island is far behind it, and forcing those two to match would " +
             "demand an absurd lens. They only have to stay in frame.")]
    [Range(1.05f, 3f)] public float depthEvenness = 1.25f;

    [Tooltip("Empty space kept around the framed content, as a fraction of the view.")]
    [Range(0f, 0.5f)] public float margin = 0.08f;

    [Tooltip("The lens is clamped to this range. A very low value is a very long lens.")]
    public float minFov = 6f;
    public float maxFov = 60f;

    [Header("When")]
    [Tooltip("Frame once on Start. Leave OFF if IslandFlow is flying the camera in — " +
             "it would fight the fly-in. Frame in the editor instead and save the scene.")]
    public bool frameOnStart = false;

    void Start() { if (frameOnStart) Frame(); }

    /// <summary>
    /// Pull the camera back along its current viewing direction until the near/far
    /// distance ratio is <see cref="depthEvenness"/>, then narrow the lens until
    /// everything is back in frame.
    /// </summary>
    [ContextMenu("Frame Now")]
    public void Frame()
    {
        var cam = GetComponent<Camera>();
        var points = CollectPoints();
        if (points.Count == 0)
        { Debug.LogWarning("[CameraFraming] Nothing found to frame."); return; }

        // The viewing ANGLE is art direction — keep it exactly as it is.
        Vector3 fwd = transform.forward, up = transform.up, right = transform.right;

        // Evenness is judged over the word path alone (see depthEvenness), but the
        // camera must still end up in front of EVERYTHING, so take the nearest of
        // both sets as the near plane of the shot.
        var evenPts = EvennessPoints();
        if (evenPts.Count == 0) evenPts = points;

        Span(evenPts, fwd, out float evenNear, out float evenFar);
        Span(points, fwd, out float allNear, out _);
        float span = evenFar - evenNear;

        // (near + span) / near = evenness  ->  near = span / (evenness - 1)
        float near = span / Mathf.Max(0.01f, depthEvenness - 1f);

        // Put the eye that far in front of the nearest stone, on the same axis, and
        // never behind anything else that has to be in shot.
        float eyeS = Mathf.Min(evenNear - near, allNear - 1f);

        // Only the along-view component moves; the sideways placement is unchanged.
        Vector3 eye = transform.position + fwd * (eyeS - Vector3.Dot(transform.position, fwd));

        // Now open the lens just enough to hold everything, plus the margin
        float halfV = 0f, halfH = 0f;
        foreach (var p in points)
        {
            Vector3 v = p - eye;
            float z = Vector3.Dot(v, fwd);
            if (z <= 0.01f) continue;
            halfV = Mathf.Max(halfV, Mathf.Abs(Vector3.Dot(v, up)) / z);
            halfH = Mathf.Max(halfH, Mathf.Abs(Vector3.Dot(v, right)) / z);
        }

        float aspect = cam.aspect > 0.01f ? cam.aspect : 9f / 16f;
        float needed = Mathf.Max(halfV, halfH / aspect) * (1f + margin);
        float fov = Mathf.Clamp(2f * Mathf.Atan(needed) * Mathf.Rad2Deg, minFov, maxFov);

        transform.position = eye;
        cam.fieldOfView = fov;
        cam.farClipPlane = Mathf.Max(cam.farClipPlane, (evenFar - eyeS) + 200f);
        cam.nearClipPlane = Mathf.Clamp((allNear - eyeS) * 0.5f, 0.03f, 5f);

        float dNear = evenNear - eyeS, dFar = evenFar - eyeS;
        Debug.Log($"[CameraFraming] eye={eye} fov={fov:0.#}° — nearest stone {dNear:0.#}u, " +
                  $"farthest {dFar:0.#}u, so the far stone renders at " +
                  $"{dNear / Mathf.Max(0.01f, dFar) * 100f:0}% of the near one " +
                  $"(was ~48% at fov 53). Framed {points.Count} points.");
    }

    /// <summary>Extent of a point set along the view direction.</summary>
    static void Span(List<Vector3> pts, Vector3 fwd, out float near, out float far)
    {
        near = float.MaxValue; far = float.MinValue;
        foreach (var p in pts)
        {
            float s = Vector3.Dot(p, fwd);
            near = Mathf.Min(near, s); far = Mathf.Max(far, s);
        }
    }

    /// <summary>The things that must read at the same size: the word path.</summary>
    List<Vector3> EvennessPoints()
    {
        var pts = new List<Vector3>();
        var path = FindAnyObjectByType<WordPathBuilder>();
        if (path != null)
        {
            if (path.startPoint != null) pts.Add(path.startPoint.position);
            if (path.endPoint != null) pts.Add(path.endPoint.position);
        }
        if (pts.Count == 0) AddBounds(pts, stones);
        return pts;
    }

    /// <summary>Every corner that has to stay inside the frame.</summary>
    List<Vector3> CollectPoints()
    {
        var pts = new List<Vector3>();

        if (book == null)
        {
            var sb = FindAnyObjectByType<StoryBook>();
            if (sb != null) book = sb.transform;
        }

        var path = FindAnyObjectByType<WordPathBuilder>();
        if (stones == null && path != null) stones = path.transform;

        if (island == null)
            foreach (var t in FindObjectsByType<Transform>(FindObjectsSortMode.None))
            {
                string n = t.name.ToLowerInvariant();
                if ((n.Contains("island") || n.Contains("vista")) &&
                    t.GetComponentInChildren<Renderer>() != null)
                { island = t; break; }
            }

        AddBounds(pts, book);
        AddBounds(pts, island);

        // the path matters even before any stones exist
        if (path != null)
        {
            if (path.startPoint != null) pts.Add(path.startPoint.position);
            if (path.endPoint != null) pts.Add(path.endPoint.position);
        }
        AddBounds(pts, stones);

        return pts;
    }

    static void AddBounds(List<Vector3> pts, Transform t)
    {
        if (t == null) return;
        var rends = t.GetComponentsInChildren<Renderer>();
        if (rends.Length == 0) { pts.Add(t.position); return; }

        var b = rends[0].bounds;
        foreach (var r in rends) b.Encapsulate(r.bounds);
        for (int i = 0; i < 8; i++)
            pts.Add(new Vector3((i & 1) == 0 ? b.min.x : b.max.x,
                                (i & 2) == 0 ? b.min.y : b.max.y,
                                (i & 4) == 0 ? b.min.z : b.max.z));
    }
}
