// ===========================================================================
//  LibraryTourRoute — carry the walk past the door and around the hall
// ===========================================================================
//  Tools > Great Library > Island > 12. Extend Walk Into The Library
//
//  The route was: river stones -> dock -> stair -> the reading mat at the door,
//  and it simply ENDED there. So the compass walked you up to the entrance and
//  stopped, with the whole library — shelves, desk, globe, lanterns — on the
//  other side of a threshold nothing could cross.
//
//  Blender now authors a loop inside the hall (SOCKET_Tour_00…07): in through
//  the door, up the left aisle past the book stack, along the front of the
//  reading desk, down the right aisle and back. Inset from the shelves and
//  routed in front of the desk rather than through it. This appends those
//  waypoints to the road between the entrance and the mat, so the walk becomes
//
//      river -> dock -> stair -> door -> AROUND THE HALL -> back to the mat
//
//  and the compass drives all of it, because it is still one road and one
//  distance along it. Nothing about how the reader moves changes.
// ===========================================================================
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class LibraryTourRoute
{
    const string TOUR_PREFIX = "SOCKET_Tour_";

    [MenuItem("Tools/Great Library/Island/12. Extend Walk Into The Library", true)]
    static bool ExtendValidate() => !Application.isPlaying;

    [MenuItem("Tools/Great Library/Island/12. Extend Walk Into The Library")]
    public static void Extend()
    {
        var road = Object.FindAnyObjectByType<RiversideRoad>(FindObjectsInactive.Include);
        if (road == null)
        {
            Debug.LogError("[Tour] No RiversideRoad in the scene. Run " +
                           "'6. Build Riverside Walk' first.");
            return;
        }

        var tour = FindAll(TOUR_PREFIX);
        if (tour.Count == 0)
        {
            Debug.LogError(
                $"[Tour] No '{TOUR_PREFIX}*' objects in the scene. They come in with " +
                "SM_Island_World.fbx — delete IslandWorld and run " +
                "'0. Rebuild World From Blender' to pick up the re-exported one.");
            return;
        }

        // Keep whatever gets the reader ashore (the dock and the entrance) and drop
        // any tour points from a previous run, so this is safe to re-run.
        var kept = road.climbWaypoints
            .Where(t => t != null && !t.name.StartsWith(TOUR_PREFIX))
            .ToList();

        Undo.RecordObject(road, "Extend Walk Into The Library");
        road.climbWaypoints = kept.Concat(tour).ToList();
        road.Rebuild();
        EditorUtility.SetDirty(road);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = road.gameObject;

        Debug.Log(
            $"[Tour] The walk now goes inside. {road.climbWaypoints.Count} waypoint(s) " +
            $"after the last word: {string.Join(" → ", road.climbWaypoints.Select(t => t.name))}" +
            (road.seat != null ? $" → {road.seat.name}" : "") + "\n" +
            $"  • road is {road.Length:0.0} m end to end, {road.StopCount} word stop(s).\n" +
            $"  • hold UP on the compass past the door and the reader keeps going " +
            $"round the hall; DOWN brings them back out the same way.\n" +
            $"  • the route is authored in island_library.blend — move the " +
            $"SOCKET_Tour_ empties there and re-export to change it.");
    }

    /// <summary>The tour waypoints, in the order Blender numbered them.</summary>
    static List<Transform> FindAll(string prefix)
    {
        var found = new List<Transform>();
        foreach (var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
                if (t.name.StartsWith(prefix)) found.Add(t);

        found.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
        return found;
    }
}
