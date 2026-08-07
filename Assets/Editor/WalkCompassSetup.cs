// ===========================================================================
//  WalkCompassSetup — put the compass in the scene, and slow the walk down
// ===========================================================================
//  Tools > Great Library > Island > 11. Add Walk Compass
//
//  Makes one Canvas object holding WalkCompass, points it at the reader, and
//  drops the pace so the reader ambles rather than marches — the whole point of
//  driving them by hand is to look at things on the way past.
//
//  It changes nothing about HOW the reader moves: the compass sets the same two
//  numbers the arrow keys already set, and PathWalker runs the walk exactly as
//  before. Remove the object and the walk is untouched.
// ===========================================================================
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class WalkCompassSetup
{
    const string GO = "WalkCompass";

    /// <summary>
    /// Metres per second while touring. The default 1.15 is a purposeful walk to a
    /// word; this is a stroll, which is what looking around wants.
    /// </summary>
    const float TOUR_SPEED = 0.75f;

    [MenuItem("Tools/Great Library/Island/11. Add Walk Compass", true)]
    static bool AddValidate() => !Application.isPlaying;

    [MenuItem("Tools/Great Library/Island/11. Add Walk Compass")]
    public static void Add()
    {
        var walker = Object.FindAnyObjectByType<PathWalker>(FindObjectsInactive.Include);
        if (walker == null)
        {
            Debug.LogError("[Compass] No PathWalker in the scene — there is nothing to " +
                           "drive. Run '6. Build Riverside Walk' first.");
            return;
        }

        var go = FindAnywhere(GO);
        if (go == null)
        {
            go = new GameObject(GO, typeof(Canvas), typeof(CanvasScaler),
                                typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(go, "Add Walk Compass");
        }
        go.SetActive(true);

        var compass = go.GetComponent<WalkCompass>();
        if (compass == null) compass = Undo.AddComponent<WalkCompass>(go);
        Undo.RecordObject(compass, "Add Walk Compass");
        compass.walker = walker;
        EditorUtility.SetDirty(compass);

        Undo.RecordObject(walker, "Add Walk Compass");
        walker.walkSpeed = TOUR_SPEED;
        walker.autoPlay = false;      // the player starts it, by holding an arrow
        // 1.0, not the default 1.5: the destination must not run ahead of the
        // reader, or letting go of an arrow leaves them still walking for a step.
        walker.keyNudgeRate = 1.0f;
        EditorUtility.SetDirty(walker);

        // BookWalkFlow is left ALONE. Switching it off here stopped it hijacking the
        // camera, but it also took the book's clickable sentences with it — and the
        // book is the start of the loop, not a rival to it. Whether the book
        // sequence runs is one checkbox on that component, which is the player's
        // call to make, not this tool's.
        var book = Object.FindAnyObjectByType<BookWalkFlow>(FindObjectsInactive.Include);
        string bookNote = book != null
            ? $"  • '{book.name}' (BookWalkFlow) left running — the book's sentences " +
              $"stay clickable. Untick the component if you want to skip straight to " +
              $"the river.\n"
            : "";

        // hand the camera back to the walk, in case the book left it holding one
        var wc = Object.FindAnyObjectByType<WalkCamera>(FindObjectsInactive.Include);
        if (wc != null && wc.focus != null)
        {
            Undo.RecordObject(wc, "Add Walk Compass");
            wc.focus = null;
            EditorUtility.SetDirty(wc);
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = go;

        Debug.Log(
            $"[Compass] Added '{GO}', bottom-right, driving '{walker.name}'.\n" +
            $"  • UP / DOWN walk onward and back along the path.\n" +
            $"  • LEFT / RIGHT turn on the spot to look around — they do not steer, " +
            $"so the reader can never leave the authored route.\n" +
            $"  • hold to move, release to stop. The arrow keys still work the same.\n" +
            $"  • pace dropped to {TOUR_SPEED:0.00} m/s so there is time to look; " +
            $"Reader ▸ PathWalker ▸ Walk Speed to change it.\n" +
            $"  • auto-play off — the reader waits for you.\n" +
            bookNote);
    }

    [MenuItem("Tools/Great Library/Island/Remove Walk Compass")]
    public static void Remove()
    {
        var go = FindAnywhere(GO);
        if (go == null) { Debug.Log("[Compass] Nothing to remove."); return; }
        Undo.DestroyObjectImmediate(go);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("[Compass] Removed. The walk itself is unchanged — it never " +
                  "depended on the pad.");
    }

    static GameObject FindAnywhere(string name)
    {
        foreach (var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
                if (t.name == name) return t.gameObject;
        return null;
    }
}
