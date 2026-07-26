// ===========================================================================
//  StoneReadingSetup — drop the "Whispering Stones" reading sample into a scene
// ===========================================================================
//  Tools > Great Library > Reading > Setup Stone Reading Sample
//
//  Places a StoneReadingPresenter on the grass in front of the library and
//  (for an isolated test) mutes the IslandFlow arrival + its canvases so you
//  can Play straight into the stone-reading mechanic. Re-run to reset.
//
//  Tools > Great Library > Reading > Remove Stone Reading Sample
//  puts IslandFlow back and deletes the rig.
// ===========================================================================
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using SReader.Game.Library.Reading;

public static class StoneReadingSetup
{
    const string RIG = "StoneReading (Sample)";

    [MenuItem("Tools/Great Library/Reading/Setup Stone Reading Sample")]
    public static void Setup()
    {
        var scene = EditorSceneManager.GetActiveScene();

        // isolate the sample: silence the arrival flow + its demo canvases
        Toggle("IslandFlow", false);
        Toggle("FlowCanvas", false);
        Toggle("ReadingCanvas", false);

        var rig = GameObject.Find(RIG) ?? new GameObject(RIG);
        rig.transform.position = new Vector3(0f, 1.0f, -16f);   // out on the open water; the path winds toward the island, staying on the flat sea
        rig.transform.rotation = Quaternion.identity;
        if (rig.GetComponent<StoneReadingPresenter>() == null)
            rig.AddComponent<StoneReadingPresenter>();

        Selection.activeGameObject = rig;
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[StoneReading] Sample ready. Press Play — the sentence rises as " +
                  "stones; the glowing (forgotten) words invite a tap. Arrival flow " +
                  "muted for this test; run 'Remove Stone Reading Sample' to restore it.");
    }

    [MenuItem("Tools/Great Library/Reading/Remove Stone Reading Sample")]
    public static void Remove()
    {
        var rig = GameObject.Find(RIG);
        if (rig != null) Object.DestroyImmediate(rig);
        Toggle("IslandFlow", true);
        Toggle("FlowCanvas", true);
        Toggle("ReadingCanvas", true);
        var scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[StoneReading] Sample removed; arrival flow restored.");
    }

    static void Toggle(string name, bool on)
    {
        var go = GameObject.Find(name);
        if (go != null) go.SetActive(on);
    }
}
