// ===========================================================================
//  WordPathGameSetup — wire the whole reading loop into the open scene
// ===========================================================================
//  Tools > Great Library > Setup Word Path Game
//    • finds/creates WordPath_River + its WordPathBuilder (stone prefabs, path
//      points on open water)
//    • puts a SentenceBoard on SM_SkyBoard (or the open book if no board)
//    • adds the WordPathGame orchestrator with the photosynthesis sample
//
//  Press Play: the passage lists on the board -> tap a sentence -> it lays out
//  on the river -> tap a glowing stone -> Define / Fill / Illustrate.
// ===========================================================================
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class WordPathGameSetup
{
    [MenuItem("Tools/Great Library/Setup Word Path Game")]
    public static void Setup()
    {
        GameObject Prefab(string name)
        {
            var guid = AssetDatabase.FindAssets(name + " t:Prefab").FirstOrDefault();
            return guid == null ? null
                : AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
        }

        // ---- the path -----------------------------------------------------
        var host = GameObject.Find("WordPath_River") ?? new GameObject("WordPath_River");
        host.transform.position = Vector3.zero;
        host.transform.rotation = Quaternion.identity;
        var stray = host.GetComponent<WordStone>();
        if (stray != null) Object.DestroyImmediate(stray);

        var builder = host.GetComponent<WordPathBuilder>() ?? host.AddComponent<WordPathBuilder>();
        builder.stoneA   = builder.stoneA   ?? Prefab("SM_WordStone_A");
        builder.stoneB   = builder.stoneB   ?? Prefab("SM_WordStone_B");
        builder.stoneC   = builder.stoneC   ?? Prefab("SM_WordStone_C");
        builder.stoneKey = builder.stoneKey ?? Prefab("SM_WordStone_Key");

        Transform Point(string n, Vector3 p)
        {
            var go = GameObject.Find(n) ?? new GameObject(n);
            go.transform.position = p;
            return go.transform;
        }
        builder.startPoint = Point("PathStart", new Vector3(3.0f, 0.2f, -18.0f));
        builder.endPoint   = Point("PathEnd",   new Vector3(0.8f, 0.2f, -10.5f));
        builder.curve = 0.8f; builder.stoneScale = 1f;

        // clear any preview stones — the game builds them from content
        for (int i = host.transform.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(host.transform.GetChild(i).gameObject);

        // ---- the board ----------------------------------------------------
        var boardGO = GameObject.Find("SM_SkyBoard") ?? GameObject.Find("SM_Book_Hero_Open");
        SentenceBoard board = null;
        if (boardGO != null)
        {
            board = boardGO.GetComponent<SentenceBoard>() ?? boardGO.AddComponent<SentenceBoard>();
        }
        else Debug.LogWarning("[WordPathGame] No SM_SkyBoard or SM_Book_Hero_Open in the scene — " +
                              "drag one in, then re-run this menu.");

        // ---- the orchestrator ---------------------------------------------
        var gameGO = GameObject.Find("WordPathGame") ?? new GameObject("WordPathGame");
        var game = gameGO.GetComponent<WordPathGame>() ?? gameGO.AddComponent<WordPathGame>();
        game.path = builder;
        game.board = board;
        game.title = "Photosynthesis";
        game.useSampleContent = true;

        // the old prototype must not fight this one
        var old = GameObject.Find("StoneReading (Sample)");
        if (old != null) old.SetActive(false);

        Selection.activeGameObject = gameGO;
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameGO.scene);
        Debug.Log("[WordPathGame] Ready. Press Play: tap a sentence on the board, " +
                  "then tap the glowing key stone on the river.");
    }
}
