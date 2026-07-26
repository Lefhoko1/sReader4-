// ===========================================================================
//  WordPathAuto — one-click river word path (Editor)
// ===========================================================================
//  Tools > Great Library > Build River Word Path
//    • finds (or creates) WordPath_River, removes any stray WordStone on it,
//      attaches WordPathBuilder in code (bypasses the Add Component menu)
//    • finds your SM_WordStone_A/B/C/Key prefabs automatically
//    • uses your existing PathStart/PathEnd if present; otherwise creates
//      them between the dock and the library steps automatically
//    • sets "The fox lived near a quiet river" / keyword "river" and builds
//
//  Tools > Great Library > Clear Word Path — removes the stones.
//
//  Put this file in Assets/Editor/.
// ===========================================================================
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class WordPathAuto
{
    [MenuItem("Tools/Great Library/Build River Word Path")]
    public static void Build()
    {
        // ---- prefabs ----------------------------------------------------
        GameObject Find(string name)
        {
            var guid = AssetDatabase.FindAssets(name + " t:Prefab")
                                    .FirstOrDefault();
            if (guid == null)
            {
                Debug.LogError($"[WordPath] Prefab '{name}' not found. " +
                               "Make the four stone prefabs first.");
                return null;
            }
            return AssetDatabase.LoadAssetAtPath<GameObject>(
                AssetDatabase.GUIDToAssetPath(guid));
        }
        var a = Find("SM_WordStone_A"); var b = Find("SM_WordStone_B");
        var c = Find("SM_WordStone_C"); var k = Find("SM_WordStone_Key");
        if (a == null || b == null || c == null || k == null) return;

        // ---- path endpoints --------------------------------------------
        Transform PointNamed(string n, System.Func<Vector3> fallback)
        {
            var go = GameObject.Find(n);
            if (go == null)
            {
                go = new GameObject(n);
                go.transform.position = fallback();
                Undo.RegisterCreatedObjectUndo(go, "Create " + n);
            }
            return go.transform;
        }
        Vector3 NearObject(string contains, Vector3 def)
        {
            var t = Object.FindObjectsByType<Transform>(
                        FindObjectsSortMode.None)
                    .FirstOrDefault(x => x.name.Contains(contains));
            return t != null ? t.position : def;
        }
        // Placed from the scene dump: sea top = y0, island radius ~11, library at
        // origin. Keep the whole path on OPEN WATER (radius > 11) at y0.2 so no stone
        // sinks into the hill, running from the front toward the shore/library.
        var start = PointNamed("PathStart", () => Vector3.zero);
        var end   = PointNamed("PathEnd",   () => Vector3.zero);
        start.position = new Vector3(3.0f, 0.2f, -18.0f);   // open water, front
        end.position   = new Vector3(0.8f, 0.2f, -10.5f);   // waterline in front of the library

        // ---- builder object --------------------------------------------
        var host = GameObject.Find("WordPath_River");
        if (host == null)
        {
            host = new GameObject("WordPath_River");
            Undo.RegisterCreatedObjectUndo(host, "Create WordPath_River");
        }
        host.transform.position = Vector3.zero;          // fix stray offsets
        host.transform.rotation = Quaternion.identity;

        // remove a mistakenly-added WordStone on the builder object
        var stray = host.GetComponent<WordStone>();
        if (stray != null) Object.DestroyImmediate(stray);

        var builder = host.GetComponent<WordPathBuilder>()
                      ?? Undo.AddComponent<WordPathBuilder>(host);
        builder.sentence  = "The fox lived near a quiet river";
        builder.keyword   = "river";
        builder.stoneA = a; builder.stoneB = b;
        builder.stoneC = c; builder.stoneKey = k;
        builder.startPoint = start; builder.endPoint = end;
        builder.curve = 0.8f; builder.stoneScale = 1f;

        builder.Build();
        Selection.activeGameObject = host;
        EditorGUIUtility.PingObject(host);
        UnityEditor.SceneManagement.EditorSceneManager
            .MarkSceneDirty(host.scene);
        Debug.Log("[WordPath] River word path built — press Play and tap " +
                  "the glowing 'river' stone.");
    }

    [MenuItem("Tools/Great Library/Clear Word Path")]
    public static void Clear()
    {
        var host = GameObject.Find("WordPath_River");
        if (host == null) { Debug.Log("[WordPath] Nothing to clear."); return; }
        for (int i = host.transform.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(host.transform.GetChild(i).gameObject);
        Debug.Log("[WordPath] Cleared.");
    }
}
