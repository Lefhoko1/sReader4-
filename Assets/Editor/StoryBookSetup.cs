// ===========================================================================
//  StoryBookSetup — one-click setup for SM_StoryBook (Editor)
// ===========================================================================
//  MENU:  Tools > Great Library > Book >
//     1. Setup Book Materials & Import   — creates the 8 candy materials
//        (URP/Lit) with the painted textures + emission on gold/mint/gem,
//        configures the FBX import, and remaps its slots by name
//     2. Place Book In Scene             — drops the book in front of the
//        Main Camera, facing it like an open lectern, adds the StoryBook
//        component, and wires stonePath to WordPath_River automatically
//     3. Frame Camera On Book            — nudges the Main Camera to a nice
//        reading angle (use in a test scene, not your island flow scene)
//     4. Save Book As Prefab             — writes Assets/Prefabs/P_StoryBook
//
//  BEFORE RUNNING:
//    • copy SM_StoryBook.fbx into  Assets/Art/Models_Book/
//    • copy T_Book_Page.png, T_Book_CoverL.png, T_Book_CoverR.png into
//      Assets/Art/Textures_Book/
//    • copy StoryBook.cs into Assets/Scripts/
//  (folders are created for you if missing — just put the files there)
//
//  Place this file in Assets/Editor/.
// ===========================================================================
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class StoryBookSetup
{
    const string MODEL_DIR = "Assets/Art/Models_Book";
    const string TEX_DIR   = "Assets/Art/Textures_Book";
    const string MAT_DIR   = "Assets/Art/Materials_Book";
    const string FBX       = MODEL_DIR + "/SM_StoryBook.fbx";
    const string PREFAB    = "Assets/Prefabs/P_StoryBook.prefab";

    // material name -> (colour, smoothness, emissionColour|null, strength, textureFile|null)
    static readonly (string n, Color c, float sm, Color? e, float s, string tex)[] DEF =
    {
        ("M_Book_CoverL",   new Color(0.49f,0.35f,0.78f), 0.35f, null, 0, "T_Book_CoverL.png"),
        ("M_Book_CoverR",   new Color(1.00f,0.50f,0.66f), 0.35f, null, 0, "T_Book_CoverR.png"),
        ("M_Book_Page",     new Color(1.00f,0.98f,0.93f), 0.06f, null, 0, "T_Book_Page.png"),
        ("M_Book_PageEdge", new Color(0.99f,0.94f,0.84f), 0.05f, null, 0, null),
        ("M_Book_Gold",     new Color(1.00f,0.80f,0.35f), 0.72f,
                            new Color(1.00f,0.75f,0.30f), 0.8f, null),
        ("M_Book_Mint",     new Color(0.45f,0.90f,0.78f), 0.55f,
                            new Color(0.40f,0.90f,0.78f), 0.6f, null),
        ("M_Book_Gem",      new Color(0.55f,0.45f,1.00f), 0.90f,
                            new Color(0.60f,0.45f,1.00f), 2.5f, null),
        ("M_Book_Ribbon",   new Color(1.00f,0.42f,0.52f), 0.30f, null, 0, null),
    };

    // ======================================================================
    [MenuItem("Tools/Great Library/Book/1. Setup Book Materials && Import")]
    public static void SetupMaterials()
    {
        var lit = Shader.Find("Universal Render Pipeline/Lit");
        if (lit == null) { Debug.LogError("[Book] URP/Lit not found."); return; }
        Directory.CreateDirectory(MAT_DIR);
        Directory.CreateDirectory(MODEL_DIR);
        Directory.CreateDirectory(TEX_DIR);
        AssetDatabase.Refresh();

        // --- textures: correct import settings -------------------------
        foreach (var f in new[] { "T_Book_Page", "T_Book_CoverL", "T_Book_CoverR" })
        {
            string p = $"{TEX_DIR}/{f}.png";
            var ti = AssetImporter.GetAtPath(p) as TextureImporter;
            if (ti == null) continue;
            ti.sRGBTexture = true;
            ti.maxTextureSize = 1024;
            ti.mipmapEnabled = true;
            var astc = new TextureImporterPlatformSettings {
                overridden = true, maxTextureSize = 1024,
                format = TextureImporterFormat.ASTC_6x6 };
            astc.name = "Android"; ti.SetPlatformTextureSettings(astc);
            astc.name = "iPhone";  ti.SetPlatformTextureSettings(astc);
            ti.SaveAndReimport();
        }

        // --- materials --------------------------------------------------
        foreach (var d in DEF)
        {
            string path = $"{MAT_DIR}/{d.n}.mat";
            var m = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (m == null)
            { m = new Material(lit) { name = d.n }; AssetDatabase.CreateAsset(m, path); }
            m.SetColor("_BaseColor", d.c);
            m.SetFloat("_Smoothness", d.sm);
            if (d.tex != null)
            {
                var t = AssetDatabase.LoadAssetAtPath<Texture2D>(
                            $"{TEX_DIR}/{d.tex}");
                if (t != null) m.SetTexture("_BaseMap", t);
                else Debug.LogWarning($"[Book] texture {d.tex} not found in {TEX_DIR}");
            }
            if (d.e.HasValue)
            {
                m.EnableKeyword("_EMISSION");
                m.globalIlluminationFlags =
                    MaterialGlobalIlluminationFlags.RealtimeEmissive;
                m.SetColor("_EmissionColor", d.e.Value * d.s);
            }
            EditorUtility.SetDirty(m);
        }
        AssetDatabase.SaveAssets();

        // --- model import + remap by name -------------------------------
        var mi = AssetImporter.GetAtPath(FBX) as ModelImporter;
        if (mi == null)
        { Debug.LogError($"[Book] {FBX} not found — copy the FBX there first."); return; }
        mi.globalScale = 1f; mi.useFileScale = true;
        mi.importAnimation = false;
        mi.animationType = ModelImporterAnimationType.None;
        mi.importCameras = mi.importLights = false;
        mi.isReadable = false;
        mi.generateSecondaryUV = true;
        mi.materialImportMode =
            ModelImporterMaterialImportMode.ImportViaMaterialDescription;

        foreach (var sub in AssetDatabase.LoadAllAssetsAtPath(FBX)
                                         .OfType<Material>())
        {
            var hit = DEF.FirstOrDefault(x => sub.name == x.n ||
                                              sub.name.StartsWith(x.n));
            if (hit.n == null) continue;
            var target = AssetDatabase.LoadAssetAtPath<Material>(
                            $"{MAT_DIR}/{hit.n}.mat");
            if (target != null)
                mi.AddRemap(new AssetImporter.SourceAssetIdentifier(
                    typeof(Material), sub.name), target);
        }
        mi.SaveAndReimport();
        Debug.Log("[Book] Materials created & FBX remapped. Run step 2 next.");
    }

    // ======================================================================
    [MenuItem("Tools/Great Library/Book/2. Place Book In Scene")]
    public static void PlaceBook()
    {
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(FBX);
        if (model == null) { Debug.LogError("[Book] Run step 1 first."); return; }

        var existing = GameObject.Find("StoryBook");
        if (existing != null) Object.DestroyImmediate(existing);

        var go = (GameObject)PrefabUtility.InstantiatePrefab(model);
        go.name = "StoryBook";
        Undo.RegisterCreatedObjectUndo(go, "Place StoryBook");

        // position: in front of the main camera, tilted to face it
        var cam = Camera.main;
        if (cam != null)
        {
            Vector3 pos = cam.transform.position
                        + cam.transform.forward * 1.15f
                        - cam.transform.up * 0.28f;
            go.transform.position = pos;
            Vector3 toCam = (cam.transform.position - pos).normalized;
            // page plane faces the camera; title edge points "up" on screen
            go.transform.rotation = Quaternion.LookRotation(
                Vector3.Slerp(cam.transform.up, toCam, 0.25f), toCam);
        }
        else
        {
            go.transform.position = new Vector3(0, 1.2f, 0);
            go.transform.rotation = Quaternion.Euler(-35, 0, 0);
        }

        // component + wiring
        var type = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => { try { return a.GetTypes(); }
                               catch { return new System.Type[0]; } })
            .FirstOrDefault(t => t.Name == "StoryBook");
        if (type == null)
        {
            Debug.LogWarning("[Book] StoryBook.cs not in project — book placed " +
                             "without the script. Add it to Assets/Scripts and " +
                             "re-run this step.");
        }
        else
        {
            var comp = go.GetComponent(type) ?? go.AddComponent(type);
            var path = GameObject.Find("WordPath_River");
            if (path != null)
            {
                var f = type.GetField("stonePath");
                if (f != null) f.SetValue(comp, path.transform);
                Debug.Log("[Book] Wired stonePath -> WordPath_River.");
            }
            else Debug.Log("[Book] No WordPath_River found — the book will " +
                           "auto-search at runtime.");
        }

        Selection.activeGameObject = go;
        EditorGUIUtility.PingObject(go);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(go.scene);
        Debug.Log("[Book] Placed. Press Play, then click a sentence.");
    }

    // ======================================================================
    [MenuItem("Tools/Great Library/Book/3. Frame Camera On Book")]
    public static void FrameCamera()
    {
        var book = GameObject.Find("StoryBook");
        var cam = Camera.main;
        if (book == null || cam == null)
        { Debug.LogError("[Book] Need a StoryBook and a Main Camera."); return; }
        Undo.RecordObject(cam.transform, "Frame Book");
        cam.transform.position = book.transform.position
                               + book.transform.up * 0.95f
                               - book.transform.forward * 0.15f;
        cam.transform.rotation = Quaternion.LookRotation(
            book.transform.position - cam.transform.position,
            book.transform.forward);
        cam.fieldOfView = 45f;
        Debug.Log("[Book] Camera framed on the book.");
    }

    // ======================================================================
    [MenuItem("Tools/Great Library/Book/4. Save Book As Prefab")]
    public static void SavePrefab()
    {
        var book = GameObject.Find("StoryBook");
        if (book == null) { Debug.LogError("[Book] Place the book first."); return; }
        Directory.CreateDirectory("Assets/Prefabs");
        PrefabUtility.SaveAsPrefabAssetAndConnect(
            book, PREFAB, InteractionMode.UserAction);
        Debug.Log("[Book] Saved " + PREFAB);
    }
}
