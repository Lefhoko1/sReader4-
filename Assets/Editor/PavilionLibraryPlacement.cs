// ===========================================================================
//  PavilionLibraryPlacement — put the sReader library in the living room
// ===========================================================================
//  Tools > Great Library > Pavilion > Move sReader Into The Living Room
//
//  The sReader probe was dropped into SC_Pavilion at (-32.7, 2.0, -0.3), which
//  is Room 1 — the pier at the ocean end. The living room is Room 3, about
//  ninety-six metres away at the other end of the building. This carries the
//  whole probe across.
//
//  THE FLOOR HEIGHT IS FOUND, NOT TYPED. Room 3's reflection probes and volumes
//  give an idea of where the room is but not where its floor is — a volume's
//  centre sits in mid-air, and guessing from it would bury the library in the
//  slab or hang it in space. A downward probe from above the target answers
//  exactly, and the Pavilion is a full sample scene, so its floors do have
//  colliders to find.
//
//  Nothing is created or deleted; one object is moved, and Undo puts it back.
// ===========================================================================
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PavilionLibraryPlacement
{
    const string PROBE = "sReaderTest";

    // The lounge, from the scene's own Room 3 markers. XZ only — Y is probed.
    static readonly string[] ANCHORS =
    {
        "Volume Room 3 Sitting",        // the seating area: where a library belongs
        "ReflectionProbe Room 3 Main",
        "Volume Room 3",
    };

    [MenuItem("Tools/Great Library/Pavilion/Move sReader Into The Living Room", true)]
    static bool MoveValidate() => !Application.isPlaying;

    [MenuItem("Tools/Great Library/Pavilion/Move sReader Into The Living Room")]
    public static void Move()
    {
        var probe = Find(PROBE);
        if (probe == null)
        {
            Debug.LogError($"[Pavilion] No '{PROBE}' in the open scene. Open " +
                           "Assets/PavilionPort/SC_Pavilion.unity first.");
            return;
        }

        Transform anchor = null;
        foreach (var n in ANCHORS) { anchor = Find(n); if (anchor != null) break; }
        if (anchor == null)
        {
            Debug.LogError("[Pavilion] Could not find any Room 3 marker (" +
                           string.Join(", ", ANCHORS) + "). Is this the Pavilion scene?");
            return;
        }

        Vector3 from = probe.position;
        Vector3 target = new Vector3(anchor.position.x, anchor.position.y, anchor.position.z);

        // Drop it onto whatever the floor actually is.
        float floor;
        bool found = Floor(target, out floor, probe);
        if (found) target.y = floor;

        Undo.RecordObject(probe, "Move sReader Into The Living Room");
        probe.position = target;
        EditorUtility.SetDirty(probe);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = probe.gameObject;
        SceneView.lastActiveSceneView?.FrameSelected();

        Debug.Log(
            $"[Pavilion] '{probe.name}' moved into the living room.\n" +
            $"  • from {from} (Room 1, the pier) → {probe.position}\n" +
            $"  • {Vector3.Distance(from, probe.position):0.0} m, anchored on " +
            $"'{anchor.name}'\n" +
            (found
                ? $"  • floor found by probe at y {floor:0.00}.\n"
                : $"  • NO FLOOR FOUND under that point — it kept the anchor's own " +
                  $"height ({target.y:0.00}), which is a volume centre and probably " +
                  $"mid-air. Nudge it down by hand, or tell me and I will anchor it " +
                  $"to a different marker.\n") +
            $"  • nothing was created or deleted; Undo puts it back.");
    }

    // ── standing furniture against a wall ───────────────────────────────────
    //  Works on the SELECTION rather than on a name. What is actually in that
    //  room, and what each piece is called, is not something this can know from
    //  outside Unity — but you can see it and click it, and the geometry answers
    //  the rest: which wall is nearest, which way it faces, and how deep the
    //  piece is. Select the bookshelf, run it.

    const float WALL_GAP = 0.02f;      // a hair off the wall, so faces don't z-fight

    [MenuItem("Tools/Great Library/Pavilion/Stand Selected Against The Nearest Wall", true)]
    static bool SnapValidate() => !Application.isPlaying && Selection.activeTransform != null;

    [MenuItem("Tools/Great Library/Pavilion/Stand Selected Against The Nearest Wall")]
    public static void SnapToWall()
    {
        var t = Selection.activeTransform;
        var bounds = WorldBounds(t);
        if (bounds.size == Vector3.zero)
        {
            Debug.LogError($"[Pavilion] '{t.name}' has no renderers, so there is no " +
                           "shape to stand against anything.");
            return;
        }

        // Which wall is nearest? Probe outward around the piece at chest height.
        Vector3 eye = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z);
        Vector3 wallPoint = Vector3.zero, wallNormal = Vector3.zero;
        float nearest = float.MaxValue;

        for (int i = 0; i < 24; i++)
        {
            float a = i * (360f / 24f) * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Sin(a), 0f, Mathf.Cos(a));
            foreach (var h in Physics.RaycastAll(eye, dir, 12f, ~0,
                                                 QueryTriggerInteraction.Ignore))
            {
                if (h.collider.transform == t || h.collider.transform.IsChildOf(t)) continue;
                // walls only: a floor or a table top has an upward normal
                if (Mathf.Abs(h.normal.y) > 0.5f) continue;
                if (h.distance < nearest)
                {
                    nearest = h.distance;
                    wallPoint = h.point;
                    wallNormal = new Vector3(h.normal.x, 0f, h.normal.z).normalized;
                }
            }
        }

        if (nearest == float.MaxValue)
        {
            Debug.LogError($"[Pavilion] Found no wall within 12 m of '{t.name}'. Either " +
                           "it is out in the open, or the walls here have no colliders.");
            return;
        }

        Undo.RecordObject(t, "Stand Against Wall");
        Vector3 was = t.position;

        // Face into the room. +Z is the front for most furniture; if this piece
        // ends up back-to-front, 'Turn Selected Around' flips it.
        t.rotation = Quaternion.LookRotation(wallNormal, Vector3.up);

        // Flush: push it back until its rear face touches the wall. Re-measured
        // AFTER rotating, because turning the piece changes how deep it reads
        // along the wall normal.
        bounds = WorldBounds(t);
        float half = Mathf.Abs(bounds.extents.x * wallNormal.x) +
                     Mathf.Abs(bounds.extents.z * wallNormal.z);
        Vector3 wantCentre = wallPoint + wallNormal * (half + WALL_GAP);
        t.position += Vector3.Project(wantCentre - bounds.center, wallNormal);

        // and sit it on the floor
        bounds = WorldBounds(t);
        if (Floor(new Vector3(bounds.center.x, bounds.max.y, bounds.center.z), out float y, t))
            t.position += Vector3.up * (y - bounds.min.y);

        EditorUtility.SetDirty(t);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log(
            $"[Pavilion] '{t.name}' stood against the wall.\n" +
            $"  • {was} → {t.position}, facing {wallNormal}\n" +
            $"  • wall found {nearest:0.00} m away; piece is {half * 2f:0.00} m deep\n" +
            $"  • if it is back-to-front, run 'Turn Selected Around'.");
    }

    [MenuItem("Tools/Great Library/Pavilion/Turn Selected Around", true)]
    static bool FlipValidate() => !Application.isPlaying && Selection.activeTransform != null;

    /// <summary>
    /// Spin a piece 180° on the spot, keeping it flush. Which local face is a
    /// model's "front" is a modelling decision nothing here can read, so rather
    /// than guess twice this offers the other option in one click.
    /// </summary>
    [MenuItem("Tools/Great Library/Pavilion/Turn Selected Around")]
    public static void Flip()
    {
        var t = Selection.activeTransform;
        var before = WorldBounds(t);
        Undo.RecordObject(t, "Turn Around");

        t.rotation = Quaternion.AngleAxis(180f, Vector3.up) * t.rotation;

        // keep the same footprint: rotating about the pivot can walk the piece
        // sideways if the pivot is not at its centre
        var after = WorldBounds(t);
        t.position += before.center - after.center;
        t.position = new Vector3(t.position.x, t.position.y, t.position.z);

        EditorUtility.SetDirty(t);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"[Pavilion] '{t.name}' turned around, same spot.");
    }

    const float NUDGE = 0.1f;

    [MenuItem("Tools/Great Library/Pavilion/Push Selected Back &LEFT", true)]
    static bool PushValidate() => !Application.isPlaying && Selection.activeTransform != null;

    /// <summary>
    /// Shuffle a piece back toward the wall by a hand's width, and again each time
    /// it is run. "A little further back" is a judgement made by eye, not a number
    /// anyone can name from outside the editor — so this is the eye's instrument:
    /// nudge, look, nudge again. Back is along -forward, because
    /// StandAgainstTheNearestWall left the piece facing into the room.
    /// </summary>
    [MenuItem("Tools/Great Library/Pavilion/Push Selected Back &LEFT")]
    public static void PushBack()
    {
        var t = Selection.activeTransform;
        Undo.RecordObject(t, "Push Back");
        t.position -= t.forward * NUDGE;
        EditorUtility.SetDirty(t);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"[Pavilion] '{t.name}' pushed back {NUDGE:0.00} m → {t.position}. " +
                  $"Run again for more (Alt+Left), or 'Pull Selected Forward' to undo a step.");
    }

    [MenuItem("Tools/Great Library/Pavilion/Pull Selected Forward &RIGHT", true)]
    static bool PullValidate() => !Application.isPlaying && Selection.activeTransform != null;

    [MenuItem("Tools/Great Library/Pavilion/Pull Selected Forward &RIGHT")]
    public static void PullForward()
    {
        var t = Selection.activeTransform;
        Undo.RecordObject(t, "Pull Forward");
        t.position += t.forward * NUDGE;
        EditorUtility.SetDirty(t);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"[Pavilion] '{t.name}' pulled forward {NUDGE:0.00} m → {t.position}.");
    }

    [MenuItem("Tools/Great Library/Pavilion/Make Selected Solid", true)]
    static bool SolidValidate() => !Application.isPlaying && Selection.activeTransform != null;

    /// <summary>
    /// Give a piece collision so the player is stopped by it instead of walking
    /// through.
    ///
    /// A BOX per mesh, not a mesh collider. Furniture only has to be solid, and a
    /// box is exactly that — cheap, reliable, and it will not let the player wedge
    /// themselves into the gap between two shelves and get stuck, which an
    /// accurate collider on a bookcase absolutely would.
    ///
    /// Sized from the mesh's LOCAL bounds, so the object's own scale and rotation
    /// carry it into place with no arithmetic here to get wrong.
    /// </summary>
    [MenuItem("Tools/Great Library/Pavilion/Make Selected Solid")]
    public static void MakeSolid()
    {
        var t = Selection.activeTransform;
        int added = 0, already = 0, skipped = 0;

        foreach (var mf in t.GetComponentsInChildren<MeshFilter>(false))
        {
            if (mf.sharedMesh == null) { skipped++; continue; }
            if (mf.GetComponent<Collider>() != null) { already++; continue; }

            var box = Undo.AddComponent<BoxCollider>(mf.gameObject);
            box.center = mf.sharedMesh.bounds.center;
            box.size = mf.sharedMesh.bounds.size;
            EditorUtility.SetDirty(box);
            added++;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        if (added == 0 && already == 0)
        {
            Debug.LogWarning($"[Pavilion] '{t.name}' has no meshes to make solid.");
            return;
        }
        Debug.Log(
            $"[Pavilion] '{t.name}' is solid: {added} collider(s) added" +
            (already > 0 ? $", {already} already had one" : "") +
            (skipped > 0 ? $", {skipped} skipped (no mesh)" : "") + ".\n" +
            $"  • the player will now be stopped by it. Undo removes them again.\n" +
            $"  • boxes, not exact shapes — you cannot get wedged inside a bookcase.");
    }

    /// <summary>Everything the piece renders, in world space.</summary>
    static Bounds WorldBounds(Transform t)
    {
        var rs = t.GetComponentsInChildren<Renderer>(false);
        if (rs.Length == 0) return new Bounds(t.position, Vector3.zero);
        var b = rs[0].bounds;
        for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
        return b;
    }

    /// <summary>
    /// The floor under a point, by probing downward from well above it.
    /// <paramref name="ignore"/> is the thing being placed — it must not be
    /// allowed to land on itself.
    /// </summary>
    static bool Floor(Vector3 at, out float y, Transform ignore = null)
    {
        y = at.y;
        var hits = Physics.RaycastAll(at + Vector3.up * 3f, Vector3.down, 12f, ~0,
                                      QueryTriggerInteraction.Ignore);
        float best = float.NegativeInfinity;
        foreach (var h in hits)
        {
            if (ignore != null &&
                (h.collider.transform == ignore || h.collider.transform.IsChildOf(ignore)))
                continue;
            if (Mathf.Abs(h.normal.y) < 0.5f) continue;   // a wall is not a floor
            if (h.point.y > best) best = h.point.y;
        }
        if (best <= float.NegativeInfinity) return false;
        y = best;
        return true;
    }

    static Transform Find(string name)
    {
        foreach (var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
                if (t.name == name) return t;
        return null;
    }
}
