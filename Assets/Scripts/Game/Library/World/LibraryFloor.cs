// ===========================================================================
//  LibraryFloor — where the library's floor is, measured once for everybody
// ===========================================================================
//  Two things have to know whether a point is inside the library: the CAMERA,
//  so the lens never ends up outside the building looking back at it, and the
//  WALKER, so a free walk stays on the floor instead of strolling out through a
//  wall. Both could work it out for themselves, and that is exactly the kind of
//  duplicate that drifts — one gets a fix, the other does not, and the reader
//  ends up indoors for the walk and outdoors for the camera in the same frame.
//
//  WHAT IS MEASURED, AND WHY IT IS THE TILES. The imported building carries no
//  colliders on its scenery, so a probe upward for a roof finds nothing and
//  reports "outdoors" while the reader stands in the middle of the hall. The
//  floor tiles are right there in the scene with real bounds, and they describe
//  the room exactly: their combined footprint IS the plan of the room, and their
//  top IS the height the reader's feet go at.
//
//  Measured once and kept, because it does not change: a building is baked into
//  the scene. Forget() is there for the editor, where it does.
// ===========================================================================
using UnityEngine;

public static class LibraryFloor
{
    /// <summary>Name prefix of the floor tiles. The Blender library uses Floor_00…</summary>
    public static string TilePrefix = "Floor_";

    static Bounds _plan;
    static bool _known;

    /// <summary>The room's plan and floor height. Meaningless unless Known.</summary>
    public static Bounds Plan => _plan;

    /// <summary>True once a library has been found in the scene.</summary>
    public static bool Known => _known || Measure();

    /// <summary>The height the reader's feet stand at, indoors.</summary>
    public static float FloorY => _plan.max.y;

    /// <summary>
    /// Find the tiles and take their combined bounds. False when the scene has no
    /// library in it — which is most scenes, and is not an error.
    /// </summary>
    public static bool Measure()
    {
        var rends = Object.FindObjectsByType<MeshRenderer>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        bool any = false;
        foreach (var r in rends)
        {
            if (r == null || !r.name.StartsWith(TilePrefix)) continue;
            if (!any) { _plan = r.bounds; any = true; }
            else _plan.Encapsulate(r.bounds);
        }
        _known = any;
        return any;
    }

    /// <summary>Forget the measurement — the editor rebuilds the world under us.</summary>
    public static void Forget() { _known = false; }

    // Statics outlive a play session when the editor is set to skip the domain
    // reload, and a room measured in the last session is a room that may not be
    // in this one.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reset() { _known = false; }

    /// <summary>
    /// 0 outside the room, 1 well inside, eased over <paramref name="blend"/> metres
    /// from the nearest wall. <paramref name="ceilingY"/> caps it in height, so the
    /// river passing under the island's edge does not read as indoors.
    /// </summary>
    public static float Insideness(Vector3 p, float blend, float ceilingY)
    {
        if (!Known) return 0f;
        if (p.y < _plan.min.y - 1f || p.y > ceilingY + 1f) return 0f;
        return Mathf.Clamp01(-Outside(p) / Mathf.Max(0.1f, blend));
    }

    /// <summary>
    /// How far the point is beyond the floor's edge, in metres — negative once
    /// inside. The furthest of the two axes, so a corner reads honestly.
    /// </summary>
    public static float Outside(Vector3 p)
    {
        if (!Known) return float.MaxValue;
        float dx = Mathf.Abs(p.x - _plan.center.x) - _plan.extents.x;
        float dz = Mathf.Abs(p.z - _plan.center.z) - _plan.extents.z;
        return Mathf.Max(dx, dz);
    }

    /// <summary>True when the point stands on the floor, allowing an inset.</summary>
    public static bool Contains(Vector3 p, float inset = 0f) =>
        Known && Outside(p) <= -inset;

    /// <summary>
    /// Put a point back on the floor, no nearer the walls than
    /// <paramref name="inset"/>. Height is left alone — the caller owns that.
    /// </summary>
    public static Vector3 KeepOn(Vector3 p, float inset)
    {
        if (!Known) return p;
        float hx = Mathf.Max(0.15f, _plan.extents.x - inset);
        float hz = Mathf.Max(0.15f, _plan.extents.z - inset);
        return new Vector3(
            Mathf.Clamp(p.x, _plan.center.x - hx, _plan.center.x + hx),
            p.y,
            Mathf.Clamp(p.z, _plan.center.z - hz, _plan.center.z + hz));
    }
}
