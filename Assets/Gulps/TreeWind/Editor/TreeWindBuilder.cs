using System.IO;
using UnityEditor;
using UnityEngine;

// One-click setup: creates trunk/leaf wind materials and builds a reusable
// prefab variant of the exported tree FBX with those materials assigned.
// Run from the menu:  Tools > Tree Wind > Build Tree Prefab
public static class TreeWindBuilder
{
    const string ShaderName = "Custom/TreeWind";
    const string OutDir     = "Assets/Gulps/TreeWind";

    [MenuItem("Tools/Tree Wind/Build Tree Prefab")]
    static void Build()
    {
        var shader = Shader.Find(ShaderName);
        if (shader == null)
        {
            Debug.LogError("[TreeWind] Shader 'Custom/TreeWind' not found. " +
                           "Let Unity finish compiling, then run again.");
            return;
        }

        string fbxPath = FindTreeFbx();
        if (fbxPath == null)
        {
            Debug.LogError("[TreeWind] Could not find a tree FBX " +
                           "(no imported mesh named 'Tree_Leaves'). " +
                           "Make sure the exported FBX is imported in the project.");
            return;
        }
        Debug.Log("[TreeWind] Using FBX: " + fbxPath);

        var model = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
        if (model == null) { Debug.LogError("[TreeWind] Failed to load model at " + fbxPath); return; }

        if (!Directory.Exists(OutDir)) Directory.CreateDirectory(OutDir);

        // Two materials, same wind shader, different colors / culling.
        var barkMat = MakeMaterial("TreeWind_Bark",   shader, new Color(0.27f, 0.17f, 0.09f), cullOff: false);
        var leafMat = MakeMaterial("TreeWind_Leaves", shader, new Color(0.18f, 0.35f, 0.12f), cullOff: true);

        // Instantiate the model, assign materials per renderer, save as a prefab variant.
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(model);
        foreach (var r in instance.GetComponentsInChildren<MeshRenderer>(true))
        {
            string n = r.gameObject.name.ToLowerInvariant();
            var mat = (n.Contains("leaf") || n.Contains("leaves")) ? leafMat : barkMat;

            var mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++) mats[i] = mat;
            r.sharedMaterials = mats;
        }

        string prefabPath = OutDir + "/Tree_Wind.prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        Object.DestroyImmediate(instance);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (prefab != null)
        {
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
            Debug.Log("[TreeWind] Done. Prefab created at " + prefabPath +
                      "  (materials in " + OutDir + ").");
        }
    }

    static Material MakeMaterial(string name, Shader shader, Color baseColor, bool cullOff)
    {
        string path = OutDir + "/" + name + ".mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }
        mat.shader = shader;
        mat.SetColor("_BaseColor", baseColor);
        mat.SetFloat("_Cull", cullOff ? 0f : 2f);            // Off for leaf cards, Back for trunk
        mat.SetVector("_WindDir", new Vector4(1f, 0f, 0.3f, 0f));
        mat.SetFloat("_WindStrength", 0.3f);
        mat.SetFloat("_WindSpeed", 1.2f);
        mat.SetFloat("_FlutterStrength", 0.08f);
        mat.SetFloat("_FlutterSpeed", 6f);
        EditorUtility.SetDirty(mat);
        return mat;
    }

    // Locate the FBX that owns a mesh named "Tree_Leaves".
    static string FindTreeFbx()
    {
        foreach (var guid in AssetDatabase.FindAssets("t:Mesh"))
        {
            string p = AssetDatabase.GUIDToAssetPath(guid);
            if (!p.ToLowerInvariant().EndsWith(".fbx")) continue;
            foreach (var o in AssetDatabase.LoadAllAssetsAtPath(p))
                if (o is Mesh m && m.name == "Tree_Leaves")
                    return p;
        }
        return null;
    }
}
