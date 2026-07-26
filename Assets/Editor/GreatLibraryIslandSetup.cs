// ===========================================================================
//  GreatLibraryIslandSetup — one-click island world setup (Editor)
// ===========================================================================
//  Automates everything from the manual walkthrough:
//
//  Tools > Great Library > Island > 1. Setup Island Materials & Import
//      • creates all island materials (URP/Lit) with the generator's exact
//        palette — including EMISSION already configured for the library
//        windows, crystals, lantern glow and sea shimmer
//      • configures every FBX in Assets/Art/Models_Island (scale, no anim,
//        lightmap UVs) and remaps its material slots BY NAME to the shared
//        assets — no per-file extraction, no "M_Grass 1" duplicates
//
//  Tools > Great Library > Island > 2. Build Vista Scene
//      • creates Assets/Scenes/SC_IslandVista.unity
//      • places SM_Island_Vista at origin, aims the camera across the sea,
//        angles the sun, and adds a Global Volume with Bloom (0.6 / 1.1)
//        so the windows and crystals actually glow
//
//  Tools > Great Library > Island > 3. Add Reading Adventure To Open Scene
//      • drops a ReadingAdventure object into whatever scene is open
//
//  Tools > Great Library > Fix Hero Book Pages Material
//      • gives P_Book_Hero_Open's page spread a clean parchment material
//        and its gem a violet glow (they otherwise inherit the trim atlas)
//
//  Requires: island FBX files in Assets/Art/Models_Island (see chat for the
//  read-only/meta fix if that folder failed to import).
// ===========================================================================
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class GreatLibraryIslandSetup
{
    const string ISLAND_DIR   = "Assets/Art/Models_Island";
    const string MAT_DIR      = "Assets/Art/Materials_Island";
    const string SCENE_PATH   = "Assets/Scenes/SC_IslandVista.unity";
    const string PROFILE_PATH = "Assets/Settings/IslandPostProfile.asset";

    // name -> (baseColor, roughness->smoothness handled, emissionColor, strength)
    static readonly (string n, Color c, Color? e, float s)[] PALETTE =
    {
        ("M_Grass",        new Color(0.32f,0.55f,0.24f), null, 0),
        ("M_Cliff",        new Color(0.45f,0.40f,0.36f), null, 0),
        ("M_Beach",        new Color(0.87f,0.78f,0.55f), null, 0),
        ("M_Sea",          new Color(0.16f,0.42f,0.52f),
                           new Color(0.10f,0.30f,0.38f), 0.25f),
        ("M_StoneWarm",    new Color(0.78f,0.71f,0.58f), null, 0),
        ("M_StoneDeep",    new Color(0.60f,0.53f,0.44f), null, 0),
        ("M_RoofTeal",     new Color(0.22f,0.45f,0.44f), null, 0),
        ("M_GoldTrim",     new Color(0.85f,0.63f,0.25f), null, 0),
        ("M_WindowGlow",   new Color(1.0f,0.78f,0.35f),
                           new Color(1.0f,0.72f,0.30f), 3.0f),
        ("M_WoodDark",     new Color(0.33f,0.22f,0.14f), null, 0),
        ("M_Trunk",        new Color(0.36f,0.25f,0.16f), null, 0),
        ("M_Canopy_A",     new Color(0.30f,0.52f,0.26f), null, 0),
        ("M_Canopy_B",     new Color(0.40f,0.60f,0.28f), null, 0),
        ("M_CrystalViolet",new Color(0.42f,0.30f,0.75f),
                           new Color(0.55f,0.40f,1.0f), 2.2f),
        ("M_SailCream",    new Color(0.94f,0.90f,0.80f), null, 0),
        ("M_PathSand",     new Color(0.80f,0.70f,0.52f), null, 0),
    };

    // ======================================================================
    //  STEP 1 — MATERIALS + IMPORT SETTINGS + REMAP
    // ======================================================================
    [MenuItem("Tools/Great Library/Island/1. Setup Island Materials && Import")]
    public static void SetupIsland()
    {
        var lit = Shader.Find("Universal Render Pipeline/Lit");
        if (lit == null)
        {
            Debug.LogError("[Island] URP/Lit shader not found — is URP installed?");
            return;
        }
        Directory.CreateDirectory(MAT_DIR);

        // ---- shared materials with glow pre-configured -----------------
        var mats = new Dictionary<string, Material>();
        foreach (var (n, c, e, s) in PALETTE)
        {
            string p = $"{MAT_DIR}/{n}.mat";
            var m = AssetDatabase.LoadAssetAtPath<Material>(p);
            if (m == null)
            {
                m = new Material(lit) { name = n };
                AssetDatabase.CreateAsset(m, p);
            }
            m.SetColor("_BaseColor", c);
            m.SetFloat("_Smoothness", n == "M_Sea" ? 0.7f :
                                      n == "M_CrystalViolet" ? 0.6f : 0.15f);
            if (e.HasValue)
            {
                m.EnableKeyword("_EMISSION");
                m.globalIlluminationFlags =
                    MaterialGlobalIlluminationFlags.RealtimeEmissive;
                m.SetColor("_EmissionColor", e.Value * s);
            }
            EditorUtility.SetDirty(m);
            mats[n] = m;
        }
        AssetDatabase.SaveAssets();

        // ---- import settings + name-based remap on every island FBX ----
        if (!Directory.Exists(ISLAND_DIR))
        {
            Debug.LogError($"[Island] {ISLAND_DIR} not found — copy the FBX " +
                           "files there first (fix the read-only flag if the " +
                           "meta error appeared).");
            return;
        }
        var guids = AssetDatabase.FindAssets("t:Model", new[] { ISLAND_DIR });
        int done = 0;
        foreach (var g in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            var mi = AssetImporter.GetAtPath(path) as ModelImporter;
            if (mi == null) continue;

            mi.globalScale = 1f;
            mi.useFileScale = true;
            mi.importAnimation = false;
            mi.animationType = ModelImporterAnimationType.None;
            mi.importCameras = mi.importLights = false;
            mi.isReadable = false;
            mi.meshCompression = ModelImporterMeshCompression.Medium;
            mi.generateSecondaryUV = true;
            mi.materialImportMode =
                ModelImporterMaterialImportMode.ImportViaMaterialDescription;

            // remap each embedded material slot to our shared asset by name
            foreach (var sub in AssetDatabase.LoadAllAssetsAtPath(path)
                                             .OfType<Material>())
            {
                string key = mats.Keys.FirstOrDefault(k =>
                                 sub.name == k || sub.name.StartsWith(k));
                if (key == null) continue;
                mi.AddRemap(new AssetImporter.SourceAssetIdentifier(
                                typeof(Material), sub.name), mats[key]);
            }
            mi.SaveAndReimport();
            done++;
        }
        Debug.Log($"[Island] Materials ready; {done} FBX configured & remapped.");
    }

    // ======================================================================
    //  STEP 2 — BUILD THE VISTA SCENE
    // ======================================================================
    [MenuItem("Tools/Great Library/Island/2. Build Vista Scene")]
    public static void BuildVistaScene()
    {
        var vistaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            $"{ISLAND_DIR}/SM_Island_Vista.fbx");
        if (vistaPrefab == null)
        {
            Debug.LogError("[Island] SM_Island_Vista.fbx not found in " +
                           ISLAND_DIR + " — run step 1 first.");
            return;
        }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        var scene = EditorSceneManager.NewScene(
            NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var vista = (GameObject)PrefabUtility.InstantiatePrefab(vistaPrefab);
        vista.name = "IslandVista";
        vista.transform.position = Vector3.zero;

        var cam = GameObject.Find("Main Camera");
        if (cam != null)
        {
            cam.transform.position = new Vector3(2.5f, 4f, -19f);
            cam.transform.rotation = Quaternion.Euler(8f, 0f, 0f);
            var c = cam.GetComponent<Camera>();
            if (c != null) { c.farClipPlane = 300f; }
        }
        var sun = GameObject.Find("Directional Light");
        if (sun != null)
        {
            sun.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
            var l = sun.GetComponent<Light>();
            if (l != null) l.intensity = 1.2f;
        }

        // post-processing: bloom so windows & crystals glow
        Directory.CreateDirectory("Assets/Settings");
        var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(PROFILE_PATH);
        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, PROFILE_PATH);
        }
        var bloom = profile.components.OfType<Bloom>().FirstOrDefault()
                    ?? profile.Add<Bloom>(true);
        bloom.intensity.overrideState = true;  bloom.intensity.value = 0.6f;
        bloom.threshold.overrideState = true;  bloom.threshold.value = 1.1f;
        EditorUtility.SetDirty(profile);

        var volGO = new GameObject("Global Volume");
        var vol = volGO.AddComponent<Volume>();
        vol.isGlobal = true;
        vol.profile = profile;

        Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(scene, SCENE_PATH);
        AssetDatabase.SaveAssets();
        Debug.Log("[Island] Vista scene built & saved: " + SCENE_PATH +
                  " — press Play (or just look at the Game view).");
    }

    // ======================================================================
    //  STEP 3 — ADD READING ADVENTURE TO WHATEVER SCENE IS OPEN
    // ======================================================================
    [MenuItem("Tools/Great Library/Island/3. Add Reading Adventure To Open Scene")]
    public static void AddReadingAdventure()
    {
        var t = System.AppDomain.CurrentDomain.GetAssemblies()
                 .SelectMany(a => { try { return a.GetTypes(); }
                                    catch { return new System.Type[0]; } })
                 .FirstOrDefault(x => x.Name == "ReadingAdventure");
        if (t == null)
        {
            Debug.LogError("[Island] ReadingAdventure.cs not found in the " +
                           "project — copy it to Assets/Scripts/Reading first.");
            return;
        }
        if (Object.FindAnyObjectByType(t) != null)
        {
            Debug.Log("[Island] ReadingAdventure already present in this scene.");
            return;
        }
        var go = new GameObject("ReadingAdventure");
        go.AddComponent(t);
        Undo.RegisterCreatedObjectUndo(go, "Add ReadingAdventure");
        EditorSceneManager.MarkSceneDirty(go.scene);
        Debug.Log("[Island] ReadingAdventure added — press Play to read.");
    }

    // ======================================================================
    //  BONUS — fix the hero Book's page spread & gem materials
    // ======================================================================
    [MenuItem("Tools/Great Library/Fix Hero Book Pages Material")]
    public static void FixBookPages()
    {
        var lit = Shader.Find("Universal Render Pipeline/Lit");
        Directory.CreateDirectory(MAT_DIR);

        Material Make(string n, Color c, Color? e = null, float s = 0)
        {
            string p = $"{MAT_DIR}/{n}.mat";
            var m = AssetDatabase.LoadAssetAtPath<Material>(p);
            if (m == null)
            { m = new Material(lit) { name = n };
              AssetDatabase.CreateAsset(m, p); }
            m.SetColor("_BaseColor", c);
            m.SetFloat("_Smoothness", 0.12f);
            if (e.HasValue)
            {
                m.EnableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", e.Value * s);
            }
            EditorUtility.SetDirty(m);
            return m;
        }
        var pages = Make("M_BookPages", new Color(0.957f, 0.914f, 0.827f));
        var gem   = Make("M_SpineGem",  new Color(0.357f, 0.247f, 0.659f),
                         new Color(0.55f, 0.40f, 1.0f), 1.6f);

        string prefabPath = "Assets/Prefabs/Kit/P_Book_Hero_Open.prefab";
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("[Island] " + prefabPath + " not found — did the " +
                           "pipeline build the hero prefabs?");
            return;
        }
        var root = PrefabUtility.LoadPrefabContents(prefabPath);
        int fixedCount = 0;
        foreach (var r in root.GetComponentsInChildren<Renderer>(true))
        {
            if (r.gameObject.name.Contains("_Pages"))
            { r.sharedMaterial = pages; fixedCount++; }
            else if (r.gameObject.name.Contains("_Gem"))
            { r.sharedMaterial = gem; fixedCount++; }
        }
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        PrefabUtility.UnloadPrefabContents(root);
        AssetDatabase.SaveAssets();
        Debug.Log($"[Island] Hero Book fixed ({fixedCount} renderers): clean " +
                  "parchment pages + glowing spine gem.");
    }
}
