// ===========================================================================
//  THE GREAT LIBRARY OF KNOWLEDGE — Kit Import Pipeline (Unity Editor)
// ===========================================================================
//  Automates Production Bible steps 2–6 for the generated Blender kit:
//    Step 2  Texture import settings (sRGB, 2048, ASTC 6x6 mobile)
//    Step 3  Creates M_TrimLibrary + M_LibraryGlass (URP/Lit)
//    Step 4  FBX import settings (scale 1, lightmap UVs, compression, no anim)
//    Step 5  Material remap by name (fully automatic, per-mesh)
//    Step 6  Generates P_ prefabs with lights on SOCKET_Flame*, fog anchor
//            on SOCKET_Fog, static flags on architecture
//
//  INSTALL:  put this file at  Assets/Editor/GreatLibraryImportPipeline.cs
//  USE:      copy the unzipped kit like this, then let it import:
//              Assets/Art/Models/Kit/   <- all SM_*.fbx
//              Assets/Art/Textures/     <- T_TrimLibrary_*.png
//            Everything in steps 2–5 happens automatically on import.
//            Then run:  Tools > Great Library > Build Kit Prefabs
//            (or it runs automatically if AUTO_BUILD_PREFABS = true)
//
//  Requires: Universal Render Pipeline package installed (Bible Ch. 7.1).
// ===========================================================================
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GreatLibraryImportPipeline : AssetPostprocessor
{
    // ------------------------------------------------------------ CONFIG --
    const string KIT_MODEL_DIR   = "Assets/Art/Models_Kit";
    const string TEXTURE_DIR     = "Assets/Art/Textures";
    const string MATERIAL_DIR    = "Assets/Art/Materials";
    const string PREFAB_DIR      = "Assets/Prefabs/Kit";
    const string TRIM_MAT_NAME   = "M_TrimLibrary";
    const string GLASS_MAT_NAME  = "M_LibraryGlass";
    static readonly bool AUTO_BUILD_PREFABS = true;

    // Amber candle light ≈ 2700K (explicit RGB so it works on every URP ver.)
    static readonly Color CandleColor = new Color(1.00f, 0.72f, 0.42f);

    // Architecture gets full static flags; these movable pieces do not.
    static readonly string[] NonStatic =
        { "SM_Book_Closed_A", "SM_Book_Closed_B", "SM_Book_Closed_C",
          "SM_Book_Stack", "SM_Scroll_Pile", "SM_Key", "SM_Quill",
          "SM_Inkpot", "SM_Globe_Desk" };

    // ======================================================================
    //  STEP 2 — TEXTURES  (automatic on import)
    // ======================================================================
    void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(TEXTURE_DIR) ||
            !Path.GetFileName(assetPath).StartsWith("T_Trim")) return;

        var ti = (TextureImporter)assetImporter;
        bool isData = assetPath.Contains("_Normal") || assetPath.Contains("_Roughness");

        ti.textureType   = assetPath.Contains("_Normal")
                           ? TextureImporterType.NormalMap
                           : TextureImporterType.Default;
        ti.sRGBTexture   = !isData;                    // albedo sRGB, maps linear
        ti.maxTextureSize = 2048;
        ti.mipmapEnabled = true;
        ti.streamingMipmaps = true;                    // mobile memory friendly
        ti.textureCompression = TextureImporterCompression.Compressed;

        var astc = new TextureImporterPlatformSettings {
            overridden = true, maxTextureSize = 2048,
            format = TextureImporterFormat.ASTC_6x6
        };
        astc.name = "Android"; ti.SetPlatformTextureSettings(astc);
        astc.name = "iPhone";  ti.SetPlatformTextureSettings(astc);
        Debug.Log($"[GreatLibrary] Texture configured: {assetPath}");
    }

    // ======================================================================
    //  STEP 4 — FBX IMPORT SETTINGS  (automatic on import)
    // ======================================================================
    void OnPreprocessModel()
    {
        if (!assetPath.StartsWith(KIT_MODEL_DIR)) return;

        var mi = (ModelImporter)assetImporter;
        mi.globalScale        = 1f;
        mi.useFileScale       = true;
        mi.importAnimation    = false;                 // SM_* are static
        mi.animationType      = ModelImporterAnimationType.None;
        mi.importBlendShapes  = false;
        mi.importCameras      = false;
        mi.importLights       = false;                 // lights come from prefabs
        mi.isReadable         = false;                 // saves runtime memory
        mi.meshCompression    = ModelImporterMeshCompression.Medium;
        mi.optimizeMeshVertices = true;
        mi.optimizeMeshPolygons = true;
        mi.generateSecondaryUV  = true;                // CRITICAL: lightmap UVs
        mi.secondaryUVHardAngle = 60;
        mi.secondaryUVPackMargin = 4;
        mi.materialImportMode =
            ModelImporterMaterialImportMode.ImportViaMaterialDescription;
    }

    // ======================================================================
    //  STEP 5 — MATERIAL REMAP BY NAME  (automatic on import)
    //  The Blender script authored every mesh with material slots already
    //  named M_TrimLibrary / M_LibraryGlass, so remap is exact.
    // ======================================================================
    Material OnAssignMaterialModel(Material material, Renderer renderer)
    {
        if (!assetPath.StartsWith(KIT_MODEL_DIR)) return null;
        // IMPORTANT: creating assets is forbidden during import — load only.
        // If materials don't exist yet, return null (Unity default) and the
        // prefab builder force-assigns the correct materials afterwards.
        string want = material.name.Contains("Glass") ? GLASS_MAT_NAME : TRIM_MAT_NAME;
        return AssetDatabase.LoadAssetAtPath<Material>(
            $"{MATERIAL_DIR}/{want}.mat");
    }

    // ======================================================================
    //  STEP 3 — MATERIAL CREATION  (runs on demand, idempotent)
    // ======================================================================
    [MenuItem("Tools/Great Library/Create Or Repair Materials")]
    public static void EnsureMaterials()
    {
        Directory.CreateDirectory(MATERIAL_DIR);

        Shader lit = Shader.Find("Universal Render Pipeline/Lit");
        if (lit == null)
        {
            Debug.LogError("[GreatLibrary] URP/Lit shader not found — is the " +
                           "Universal RP package installed? (Bible Ch. 7.1)");
            return;
        }

        // ---- M_TrimLibrary --------------------------------------------
        string trimPath = $"{MATERIAL_DIR}/{TRIM_MAT_NAME}.mat";
        var trim = AssetDatabase.LoadAssetAtPath<Material>(trimPath);
        if (trim == null)
        {
            trim = new Material(lit) { name = TRIM_MAT_NAME };
            AssetDatabase.CreateAsset(trim, trimPath);
        }
        trim.SetFloat("_Smoothness", 0.18f);
        var albedo = AssetDatabase.LoadAssetAtPath<Texture2D>(
            $"{TEXTURE_DIR}/T_TrimLibrary_Albedo.png");
        if (albedo) trim.SetTexture("_BaseMap", albedo);
        var normal = AssetDatabase.LoadAssetAtPath<Texture2D>(
            $"{TEXTURE_DIR}/T_TrimLibrary_Normal.png");
        if (normal) { trim.SetTexture("_BumpMap", normal);
                      trim.EnableKeyword("_NORMALMAP"); }

        // ---- M_LibraryGlass -------------------------------------------
        string glassPath = $"{MATERIAL_DIR}/{GLASS_MAT_NAME}.mat";
        var glass = AssetDatabase.LoadAssetAtPath<Material>(glassPath);
        if (glass == null)
        {
            glass = new Material(lit) { name = GLASS_MAT_NAME };
            AssetDatabase.CreateAsset(glass, glassPath);
        }
        glass.SetFloat("_Surface", 1f);                // Transparent
        glass.SetFloat("_Blend", 0f);                  // Alpha blend
        glass.SetColor("_BaseColor", new Color(0.75f, 0.85f, 0.90f, 0.25f));
        glass.SetFloat("_Smoothness", 0.92f);
        glass.SetOverrideTag("RenderType", "Transparent");
        glass.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        glass.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        glass.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        glass.SetInt("_ZWrite", 0);
        glass.DisableKeyword("_ALPHATEST_ON");
        glass.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

        EditorUtility.SetDirty(trim); EditorUtility.SetDirty(glass);
        AssetDatabase.SaveAssets();
        Debug.Log("[GreatLibrary] Materials ready.");
    }

    // ======================================================================
    //  STEP 6 — PREFAB GENERATION
    // ======================================================================
    static void OnPostprocessAllAssets(string[] imported, string[] deleted,
                                       string[] moved, string[] movedFrom)
    {
        if (!AUTO_BUILD_PREFABS) return;
        if (imported.Any(p => p.StartsWith(KIT_MODEL_DIR) && p.EndsWith(".fbx")))
            EditorApplication.delayCall += BuildKitPrefabs;   // after import
    }

    [MenuItem("Tools/Great Library/Build Kit Prefabs")]
    public static void BuildKitPrefabs()
    {
        EnsureMaterials();
        Directory.CreateDirectory(PREFAB_DIR);

        var fbxGuids = AssetDatabase.FindAssets("t:Model", new[] { KIT_MODEL_DIR });
        int built = 0;

        foreach (var guid in fbxGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (model == null || !model.name.StartsWith("SM_")) continue;

            string prefabName = "P_" + model.name.Substring(3);
            string prefabPath = $"{PREFAB_DIR}/{prefabName}.prefab";

            // Idempotent: regenerate cleanly each run
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (existing != null) AssetDatabase.DeleteAsset(prefabPath);

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(model);
            instance.name = prefabName;

            ForceAssignMaterials(instance);            // heals failed remaps
            DressSockets(instance);
            ApplyStaticFlags(instance, model.name);

            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            Object.DestroyImmediate(instance);
            built++;
        }
        AssetDatabase.SaveAssets();
        Debug.Log($"[GreatLibrary] Built {built} prefabs into {PREFAB_DIR}");
    }

    // ---- material healing: guarantees correct materials on prefabs -------
    static void ForceAssignMaterials(GameObject root)
    {
        var trim  = AssetDatabase.LoadAssetAtPath<Material>(
            $"{MATERIAL_DIR}/{TRIM_MAT_NAME}.mat");
        var glass = AssetDatabase.LoadAssetAtPath<Material>(
            $"{MATERIAL_DIR}/{GLASS_MAT_NAME}.mat");
        if (trim == null) return;                      // EnsureMaterials ran first

        foreach (var r in root.GetComponentsInChildren<Renderer>(true))
        {
            var mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                bool wantGlass = r.gameObject.name.Contains("_Glass") ||
                                 (mats[i] != null && mats[i].name.Contains("Glass"));
                mats[i] = wantGlass && glass != null ? glass : trim;
            }
            r.sharedMaterials = mats;
        }
    }

    // ---- socket dressing: lights, fog anchor, mount markers --------------
    static void DressSockets(GameObject root)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            if (!t.name.StartsWith("SOCKET_")) continue;

            if (t.name.StartsWith("SOCKET_Flame"))
            {
                // Amber point light per candle flame (Bible Ch. 8.6):
                // baked-indirect, low intensity, short range, no shadows.
                var lightGO = new GameObject("Light_Candle");
                lightGO.transform.SetParent(t, false);
                lightGO.transform.localPosition = new Vector3(0f, 0.03f, 0f);
                var l = lightGO.AddComponent<Light>();
                l.type = LightType.Point;
                l.color = CandleColor;
                l.intensity = 1.1f;
                l.range = 3.0f;
                l.shadows = LightShadows.None;         // mobile: never on candles
                l.lightmapBakeType = LightmapBakeType.Mixed;
            }
            else if (t.name == "SOCKET_Fog")
            {
                // Anchor object the P_FogVolume particle prefab parents to.
                if (t.Find("FogAnchor") == null)
                {
                    var fog = new GameObject("FogAnchor");
                    fog.transform.SetParent(t, false);
                }
            }
            // SOCKET_Book / SOCKET_Candles / SOCKET_Lantern remain pure
            // mount transforms — gameplay prefabs attach to them at runtime.
        }
    }

    static void ApplyStaticFlags(GameObject root, string modelName)
    {
        bool isStatic = !NonStatic.Contains(modelName);
        var flags = isStatic
            ? StaticEditorFlags.ContributeGI
              | StaticEditorFlags.BatchingStatic
              | StaticEditorFlags.OccluderStatic
              | StaticEditorFlags.OccludeeStatic
              | StaticEditorFlags.ReflectionProbeStatic
            : 0;

        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            // Glass: batched + occludee, but never an occluder, never GI-baked
            if (t.name.Contains("_Glass"))
                GameObjectUtility.SetStaticEditorFlags(t.gameObject,
                    StaticEditorFlags.BatchingStatic |
                    StaticEditorFlags.OccludeeStatic);
            else if (!t.name.StartsWith("SOCKET_") && t.name != "FogAnchor"
                     && !t.name.StartsWith("Light_"))
                GameObjectUtility.SetStaticEditorFlags(t.gameObject, flags);
        }
    }

    // ======================================================================
    //  VALIDATION REPORT
    // ======================================================================
    [MenuItem("Tools/Great Library/Validate Kit Import")]
    public static void Validate()
    {
        var report = new List<string>();

        var albedo = AssetDatabase.LoadAssetAtPath<Texture2D>(
            $"{TEXTURE_DIR}/T_TrimLibrary_Albedo.png");
        report.Add(albedo ? "OK   Trim albedo present"
                          : "MISS Trim albedo (placeholder or painted) not found");

        foreach (var m in new[] { TRIM_MAT_NAME, GLASS_MAT_NAME })
            report.Add(AssetDatabase.LoadAssetAtPath<Material>(
                $"{MATERIAL_DIR}/{m}.mat")
                ? $"OK   {m}" : $"MISS {m} — run Create Or Repair Materials");

        var fbx = AssetDatabase.FindAssets("t:Model", new[] { KIT_MODEL_DIR });
        report.Add($"{(fbx.Length >= 26 ? "OK  " : "WARN")} {fbx.Length}/26 kit FBX files in {KIT_MODEL_DIR}");

        int prefabs = Directory.Exists(PREFAB_DIR)
            ? AssetDatabase.FindAssets("t:Prefab", new[] { PREFAB_DIR }).Length : 0;
        report.Add($"{(prefabs >= 26 ? "OK  " : "WARN")} {prefabs} prefabs in {PREFAB_DIR}");

        foreach (var guid in fbx)
        {
            var mi = AssetImporter.GetAtPath(
                AssetDatabase.GUIDToAssetPath(guid)) as ModelImporter;
            if (mi != null && !mi.generateSecondaryUV)
                report.Add($"FIX  {Path.GetFileName(mi.assetPath)} missing lightmap UVs — reimport it");
        }

        Debug.Log("[GreatLibrary] Validation\n  " + string.Join("\n  ", report));
    }
}