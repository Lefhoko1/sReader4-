// ===========================================================================
//  PavilionPortSetup — finish the HDRP→URP port of SC_Pavilion
// ===========================================================================
//  Tools > Pavilion Port > ...
//
//  Does the editor-side steps the on-disk conversion could not do: activate a
//  camera, point it at Renderer3D, add the Decal Renderer Feature, set the
//  reflection probes to Baked, and register the scene for building.
//
//  NOTHING HERE RUNS BY ITSELF. There is no [InitializeOnLoad], no callback,
//  no asset postprocessor. Every action happens only when you click a menu
//  item, every action is safe to run twice, and deleting this file leaves no
//  trace in the project.
//
//  Run "0. Report Status" first — it only reads, and tells you what is left.
// ===========================================================================
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class PavilionPortSetup
{
    const string ScenePath = "Assets/PavilionPort/SC_Pavilion.unity";
    const string Renderer3DPath = "Assets/Settings/Renderer3D.asset";

    /// <summary>Camera preferred when activating one; any camera works.</summary>
    const string PreferredCamera = "Screenshot Camera 1";

    const string Menu = "Tools/Pavilion Port/";

    // =======================================================================
    //  0. Report — read-only. Changes nothing.
    // =======================================================================
    [MenuItem(Menu + "0. Report Status", priority = 0)]
    public static void Report()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== Pavilion Port status ===\n");

        var urp = GetUrpAsset();
        sb.AppendLine(urp == null
            ? "URP asset            : NOT FOUND (is a URP quality level active?)"
            : $"URP asset            : {urp.name}");

        int idx = FindRendererIndex(urp, "Renderer3D");
        sb.AppendLine($"Renderer3D index     : {(idx < 0 ? "not in renderer list" : idx.ToString())}");
        sb.AppendLine($"Default renderer idx : {GetDefaultRendererIndex(urp)}  (cameras without an override use this)");

        var rd = AssetDatabase.LoadAssetAtPath<ScriptableRendererData>(Renderer3DPath);
        sb.AppendLine($"Decal feature        : {(rd != null && HasDecalFeature(rd) ? "present" : "MISSING — run step 2")}");

        sb.AppendLine($"Light probe system   : {ReadEnumName(urp, "m_LightProbeSystem")}");

        if (SceneIsOpen())
        {
            sb.AppendLine($"\nSkybox material      : {(RenderSettings.skybox == null ? "NONE — run step 1b (no sky, no ambient light)" : RenderSettings.skybox.name)}");
            sb.AppendLine($"Ambient mode         : {RenderSettings.ambientMode}");

            var cams = FindAll<Camera>();
            var active = cams.Where(c => c.gameObject.activeInHierarchy).ToList();
            sb.AppendLine($"\nScene cameras        : {cams.Count} total, {active.Count} active");
            foreach (var c in active)
            {
                var data = c.GetComponent<UniversalAdditionalCameraData>();
                int r = data == null ? -1 : ReadInt(data, "m_RendererIndex");
                sb.AppendLine($"   {c.name,-28} tag={c.tag,-12} renderer={(r < 0 ? "default (!)" : r.ToString())}");
            }
            var probes = FindAll<ReflectionProbe>();
            sb.AppendLine($"Reflection probes    : {probes.Count} ({probes.Count(p => p.mode == ReflectionProbeMode.Baked)} baked)");
        }
        else
        {
            sb.AppendLine($"\nScene                : not open — open {ScenePath} for scene detail");
        }

        bool inBuild = EditorBuildSettings.scenes.Any(s => s.path == ScenePath);
        sb.AppendLine($"In Build Settings    : {(inBuild ? "yes" : "no")}");

        Debug.Log(sb.ToString());
    }

    // =======================================================================
    //  1. Scene setup — camera, renderer, probes, build settings
    // =======================================================================
    [MenuItem(Menu + "1. Set Up Scene (camera + renderer + probes)", priority = 20)]
    public static void SetUpScene()
    {
        if (!EnsureSceneOpen()) return;

        var urp = GetUrpAsset();
        int rendererIndex = FindRendererIndex(urp, "Renderer3D");
        if (rendererIndex < 0)
        {
            EditorUtility.DisplayDialog("Pavilion Port",
                "Renderer3D is not in the active URP asset's renderer list, so cameras " +
                "cannot be pointed at it.\n\nCheck that a URP quality level is active.", "OK");
            return;
        }

        var log = new StringBuilder("=== Set Up Scene ===\n");

        // --- camera -------------------------------------------------------
        var cams = FindAll<Camera>();
        if (cams.Count == 0)
        {
            log.AppendLine("No Camera in scene — nothing to activate.");
        }
        else
        {
            var cam = cams.FirstOrDefault(c => c.gameObject.activeInHierarchy && c.CompareTag("MainCamera"))
                   ?? cams.FirstOrDefault(c => c.name == PreferredCamera)
                   ?? cams[0];

            Undo.RecordObject(cam.gameObject, "Pavilion: activate camera");
            if (!cam.gameObject.activeSelf)
            {
                cam.gameObject.SetActive(true);
                log.AppendLine($"Activated camera     : {cam.name}");
            }
            else log.AppendLine($"Camera already active: {cam.name}");

            if (!cam.CompareTag("MainCamera"))
            {
                cam.tag = "MainCamera";
                log.AppendLine("Tagged MainCamera.");
            }
            EditorUtility.SetDirty(cam.gameObject);

            // Only one AudioListener may be active; this scene ships 12.
            int muted = 0;
            foreach (var al in FindAll<AudioListener>())
            {
                if (al.gameObject == cam.gameObject || !al.gameObject.activeInHierarchy) continue;
                Undo.RecordObject(al, "Pavilion: disable extra listener");
                al.enabled = false; muted++; EditorUtility.SetDirty(al);
            }
            if (muted > 0) log.AppendLine($"Disabled extra AudioListeners: {muted}");

            // --- renderer index -------------------------------------------
            // This is the step that decides whether the scene looks right at
            // all: without it the camera falls back to the project default,
            // which in this project is Renderer2D.
            int fixedCams = 0;
            foreach (var c in cams)
            {
                if (!c.gameObject.activeInHierarchy) continue;
                var data = c.GetComponent<UniversalAdditionalCameraData>();
                if (data == null) data = Undo.AddComponent<UniversalAdditionalCameraData>(c.gameObject);
                if (ReadInt(data, "m_RendererIndex") == rendererIndex) continue;

                var so = new SerializedObject(data);
                so.FindProperty("m_RendererIndex").intValue = rendererIndex;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(data);
                fixedCams++;
            }
            log.AppendLine($"Cameras set to Renderer3D (index {rendererIndex}): {fixedCams}");
        }

        // --- reflection probes -------------------------------------------
        int probesFixed = 0;
        foreach (var p in FindAll<ReflectionProbe>())
        {
            if (p.mode == ReflectionProbeMode.Baked) continue;
            Undo.RecordObject(p, "Pavilion: bake mode");
            p.mode = ReflectionProbeMode.Baked;
            EditorUtility.SetDirty(p);
            probesFixed++;
        }
        log.AppendLine($"Reflection probes set to Baked: {probesFixed}");

        // --- build settings ----------------------------------------------
        if (!EditorBuildSettings.scenes.Any(s => s.path == ScenePath))
        {
            var list = EditorBuildSettings.scenes.ToList();
            list.Add(new EditorBuildSettingsScene(ScenePath, true));
            EditorBuildSettings.scenes = list.ToArray();
            log.AppendLine("Added to Build Settings.");
        }
        else log.AppendLine("Already in Build Settings.");

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        log.AppendLine("\nScene saved.");
        Debug.Log(log.ToString());
    }

    // =======================================================================
    //  1b. Environment — sky + ambient
    // =======================================================================
    //  The HDRP original got its sky from PhysicallyBasedSky / VisualEnvironment
    //  volume overrides, which have no URP equivalent and were dropped in the
    //  port. URP takes its sky from RenderSettings.skybox instead, and this
    //  scene arrived with that empty (m_SkyboxMaterial: {fileID: 0}) while
    //  ambient mode was still set to Skybox — so there was no sky AND no
    //  environment light, leaving only the single directional light.
    // =======================================================================
    const string SkyMaterialPath = "Assets/PavilionPort/Sky_Pavilion.mat";

    [MenuItem(Menu + "1b. Fix Environment (sky + ambient)", priority = 22)]
    public static void FixEnvironment()
    {
        if (!EnsureSceneOpen()) return;

        var sky = AssetDatabase.LoadAssetAtPath<Material>(SkyMaterialPath);
        if (sky == null)
        {
            var shader = Shader.Find("Skybox/Procedural");
            if (shader == null)
            {
                Debug.LogError("Skybox/Procedural shader not found — cannot build a sky material.");
                return;
            }
            sky = new Material(shader) { name = "Sky_Pavilion" };
            // Overcast-ish daylight: the pavilion is a bright, open building and
            // its baked look assumed a soft sky rather than a hard blue one.
            sky.SetFloat("_SunSize", 0.04f);
            sky.SetFloat("_AtmosphereThickness", 1.0f);
            sky.SetFloat("_Exposure", 1.15f);
            sky.SetColor("_SkyTint", new Color(0.62f, 0.68f, 0.78f));
            sky.SetColor("_GroundColor", new Color(0.30f, 0.29f, 0.27f));
            AssetDatabase.CreateAsset(sky, SkyMaterialPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Created {SkyMaterialPath}");
        }

        // RenderSettings is static scene state; it has no Undo target. The scene
        // is saved below, and the change is reversible through git.
        RenderSettings.skybox = sky;
        RenderSettings.ambientMode = AmbientMode.Skybox;
        RenderSettings.ambientIntensity = 1f;
        RenderSettings.reflectionIntensity = 1f;
        DynamicGI.UpdateEnvironment();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Environment set: skybox assigned, ambient = Skybox. " +
                  "The scene now has sky light; bake (step 4) to get indirect bounce.");
    }

    // =======================================================================
    //  1c. Light intensities — HDRP physical units -> URP arbitrary units
    // =======================================================================
    //  HDRP stores light intensity in PHYSICAL units: lux for directional,
    //  candela for punctual. URP has no physical light units (there is no
    //  LightUnit anywhere in URP 17.4), and its scale is arbitrary — 1 to 3 is
    //  an ordinary light.
    //
    //  The port carried the physical numbers straight across on the native
    //  Light component, so the scene arrived with a 100,000 "lux" sun and
    //  ~900 candela spots. That is roughly two orders of magnitude too bright
    //  and renders as a pure white screen.
    //
    //  Both the prefab assets and the scene instances are converted, so the
    //  fix survives re-instantiating a light prefab later.
    // =======================================================================

    /// <summary>100,000 lux is roughly midday sun, which is URP intensity ~1.</summary>
    const float LuxPerUrpUnit = 100000f;

    /// <summary>Candela per URP unit. 100 cd ≈ a 1 unit domestic lamp.</summary>
    const float CandelaPerUrpUnit = 100f;

    /// <summary>
    /// Anything at or below this is already in URP's range, so conversion is
    /// skipped. That is what makes this menu item safe to run twice.
    /// </summary>
    const float AlreadyConvertedBelow = 25f;

    [MenuItem(Menu + "1c. Fix Light Intensities (HDRP units → URP)", priority = 23)]
    public static void FixLightIntensities()
    {
        if (!EnsureSceneOpen()) return;

        int prefabLights = 0, sceneLights = 0, skipped = 0;
        var log = new StringBuilder("=== Light intensity conversion ===\n");

        // --- prefab assets first, so future instances are correct -----------
        foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/PavilionPort" }))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var root = PrefabUtility.LoadPrefabContents(path);
            bool changed = false;
            foreach (var l in root.GetComponentsInChildren<Light>(true))
            {
                if (Convert(l, out float from, out float to)) { changed = true; prefabLights++; log.AppendLine($"  {System.IO.Path.GetFileNameWithoutExtension(path),-28} {l.type,-11} {from,10:F1} -> {to:F2}"); }
                else skipped++;
            }
            if (changed) PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
        }

        // --- then anything living directly in the scene ---------------------
        foreach (var l in FindAll<Light>())
        {
            // Instances of the prefabs fixed above already inherit the new
            // value, so they fall under AlreadyConvertedBelow and are skipped.
            // Only scene-level lights and per-instance overrides are touched.
            Undo.RecordObject(l, "Pavilion: convert light intensity");
            if (Convert(l, out float from, out float to))
            {
                sceneLights++;
                EditorUtility.SetDirty(l);
                log.AppendLine($"  [scene] {l.name,-22} {l.type,-11} {from,10:F1} -> {to:F2}");
            }
            else skipped++;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();

        log.AppendLine($"\nConverted: {prefabLights} in prefabs, {sceneLights} in scene. " +
                       $"Already in URP range: {skipped}.");
        Debug.Log(log.ToString());
    }

    /// <summary>
    /// Rescales one light. Returns false if it was already within URP's range,
    /// which keeps this idempotent.
    /// </summary>
    static bool Convert(Light l, out float from, out float to)
    {
        from = l.intensity; to = from;
        if (from <= AlreadyConvertedBelow) return false;

        to = l.type == LightType.Directional
            ? Mathf.Clamp(from / LuxPerUrpUnit, 0.4f, 3f)
            : Mathf.Clamp(from / CandelaPerUrpUnit, 0.5f, 12f);

        l.intensity = to;
        return true;
    }

    // =======================================================================
    //  1d. Discard the HDRP-baked lighting data
    // =======================================================================
    //  The port copied the original scene's baked lighting wholesale: the
    //  lightmap EXRs, the 19 reflection probe captures, and the Adaptive Probe
    //  Volume cell data. All of it was baked by HDRP in PHYSICAL units — the
    //  same scale that produced a 100,000 lux sun.
    //
    //  URP reads those values at face value, so as soon as APV is switched on
    //  the scene blows out to white. The data is not convertible; it has to be
    //  thrown away and rebaked by URP.
    // =======================================================================
    const string BakeFolder = "Assets/PavilionPort/SC_Pavilion";

    [MenuItem(Menu + "1d. Discard HDRP-Baked Lighting Data", priority = 24)]
    public static void ClearStaleBake()
    {
        if (!EnsureSceneOpen()) return;

        var files = AssetDatabase.FindAssets("", new[] { BakeFolder })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => !AssetDatabase.IsValidFolder(p))
            .Distinct()
            .ToList();

        if (files.Count == 0)
        {
            Debug.Log("No baked lighting data found — already clear. Run step 4 to bake.");
            return;
        }

        long bytes = files.Sum(p => { var fi = new System.IO.FileInfo(p); return fi.Exists ? fi.Length : 0; });

        if (!EditorUtility.DisplayDialog("Pavilion Port",
                $"Discard {files.Count} baked lighting file(s) ({bytes / 1048576} MB)?\n\n" +
                "These were baked by HDRP in physical units. URP reads them at face " +
                "value, which is why the scene turns white with APV enabled. They " +
                "cannot be converted — the scene must be rebaked.\n\n" +
                "Recoverable from git.",
                "Discard and rebake", "Cancel"))
            return;

        // Detach first so the scene stops pointing at data that is going away.
        Lightmapping.Clear();
        Lightmapping.ClearLightingDataAsset();

        int deleted = 0;
        foreach (var p in files) if (AssetDatabase.DeleteAsset(p)) deleted++;

        AssetDatabase.Refresh();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        Debug.Log($"Discarded {deleted} HDRP-baked lighting file(s).\n" +
                  "The scene is now unlit-but-correct: direct light only, no indirect. " +
                  "Run step 4 to bake it properly in URP.");
    }

    // =======================================================================
    //  2. Decal Renderer Feature — edits Assets/Settings/Renderer3D.asset
    // =======================================================================
    [MenuItem(Menu + "2. Add Decal Renderer Feature", priority = 21)]
    public static void AddDecalFeature()
    {
        var rd = AssetDatabase.LoadAssetAtPath<ScriptableRendererData>(Renderer3DPath);
        if (rd == null)
        {
            EditorUtility.DisplayDialog("Pavilion Port", $"Could not load {Renderer3DPath}.", "OK");
            return;
        }

        if (HasDecalFeature(rd))
        {
            Debug.Log("Decal Renderer Feature already present on Renderer3D — nothing to do.");
            return;
        }

        // Renderer3D is used by the whole game, so make the change explicit.
        if (!EditorUtility.DisplayDialog("Pavilion Port",
                "Add the Decal Renderer Feature to Assets/Settings/Renderer3D.asset?\n\n" +
                "Renderer3D is used by this project's 3D scenes, not just the pavilion. " +
                "Decals cost some fill rate on mobile; the feature will be configured as " +
                "Screen Space (the cheaper technique) rather than DBuffer.",
                "Add it", "Cancel"))
            return;

        // Mirrors what URP's own ScriptableRendererDataEditor.AddComponent does:
        // the feature is a sub-asset, and BOTH the feature list and the feature
        // map (keyed on the sub-asset's local file id) have to be grown.
        var feature = ScriptableObject.CreateInstance<DecalRendererFeature>();
        feature.name = "DecalRendererFeature";
        Undo.RegisterCreatedObjectUndo(feature, "Add Decal Renderer Feature");

        AssetDatabase.AddObjectToAsset(feature, rd);
        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(feature, out _, out long localId);

        var so = new SerializedObject(rd);
        var features = so.FindProperty("m_RendererFeatures");
        var map = so.FindProperty("m_RendererFeatureMap");
        features.arraySize++;
        features.GetArrayElementAtIndex(features.arraySize - 1).objectReferenceValue = feature;
        map.arraySize++;
        map.GetArrayElementAtIndex(map.arraySize - 1).longValue = localId;
        so.ApplyModifiedProperties();

        // Configure for mobile. DBufferSettings / DecalScreenSpaceSettings are
        // internal types, so drive them by serialized path and match the enum
        // by NAME — that survives the enum being reordered in a URP update.
        var fso = new SerializedObject(feature);
        SetEnumByName(fso, "m_Settings.technique", "ScreenSpace");
        SetEnumByName(fso, "m_Settings.screenSpaceSettings.normalBlend", "Low");
        fso.ApplyModifiedProperties();

        EditorUtility.SetDirty(rd);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Added Decal Renderer Feature to Renderer3D (Screen Space, normal blend Low). " +
                  "The scene's 80 decals will now render.");
    }

    // =======================================================================
    //  3a. Route A — Adaptive Probe Volumes. PROJECT-WIDE.
    // =======================================================================
    [MenuItem(Menu + "3a. Enable Adaptive Probe Volumes (PROJECT-WIDE)", priority = 40)]
    public static void EnableApv()
    {
        var urp = GetUrpAsset();
        if (urp == null) { Debug.LogError("No active URP asset."); return; }

        if (ReadEnumName(urp, "m_LightProbeSystem") == "ProbeVolumes")
        {
            Debug.Log("Adaptive Probe Volumes already enabled — nothing to do.");
            return;
        }

        if (!EditorUtility.DisplayDialog("Pavilion Port — project-wide change",
                $"Switch {urp.name} from Light Probe Groups to Adaptive Probe Volumes?\n\n" +
                "This URP asset is used by ALL quality levels, so it also changes how " +
                "SC_IslandVista and every other 3D scene is lit.\n\n" +
                "The pavilion was authored for APV and its ProbeVolume components do " +
                "nothing until this is on — but check your other scenes afterwards.",
                "Enable APV", "Cancel"))
            return;

        var so = new SerializedObject(urp);
        SetEnumByName(so, "m_LightProbeSystem", "ProbeVolumes");
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(urp);
        AssetDatabase.SaveAssets();
        Debug.Log($"{urp.name}: Light Probe System → Adaptive Probe Volumes. Now bake (step 4).");
    }

    // =======================================================================
    //  3b. Contribute GI — REQUIRED before any bake, APV or lightmaps
    // =======================================================================
    //  Not an alternative to 3a. The scene arrived with 261 objects flagged
    //  ReflectionProbeStatic only and almost nothing marked Contribute GI,
    //  because HDRP drove its indirect light from APV without needing it.
    //
    //  Unity places adaptive probes around GI-CONTRIBUTING geometry, so with
    //  nothing contributing, an APV bake produces an empty baking set and the
    //  scene goes black in play mode. Lightmapping has the same requirement.
    //  Run this before step 4 whichever lighting route you took.
    // =======================================================================
    [MenuItem(Menu + "3b. Mark Contribute GI (REQUIRED before baking)", priority = 41)]
    public static void MarkStatic()
    {
        if (!EnsureSceneOpen()) return;

        if (!EditorUtility.DisplayDialog("Pavilion Port",
                "Mark every MeshRenderer in the scene as Contribute GI + Batching Static?\n\n" +
                "Required before baking, for Adaptive Probe Volumes as much as for " +
                "lightmaps — neither can bake anything without GI-contributing geometry.\n\n" +
                "Writes static flags as scene overrides only; the prefab assets are " +
                "not modified.",
                "Mark static", "Cancel"))
            return;

        int n = 0;
        foreach (var mr in FindAll<MeshRenderer>())
        {
            var go = mr.gameObject;
            var flags = GameObjectUtility.GetStaticEditorFlags(go);
            var wanted = flags | StaticEditorFlags.ContributeGI | StaticEditorFlags.BatchingStatic;
            if (flags == wanted) continue;
            Undo.RecordObject(go, "Pavilion: mark static");
            GameObjectUtility.SetStaticEditorFlags(go, wanted);
            EditorUtility.SetDirty(go);
            n++;
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log($"Marked {n} renderers Contribute GI + Batching Static. Now bake (step 4).");
    }

    // =======================================================================
    //  4. Bake
    // =======================================================================
    [MenuItem(Menu + "4. Bake Lighting", priority = 60)]
    public static void Bake()
    {
        if (!EnsureSceneOpen()) return;
        if (Lightmapping.isRunning) { Debug.LogWarning("A bake is already running."); return; }

        // A bake with no GI-contributing geometry "succeeds" and writes an empty
        // baking set, which then reads as pitch black in play mode. Refuse it.
        var rends = FindAll<MeshRenderer>().Where(r => r.enabled && r.gameObject.activeInHierarchy).ToList();
        int contributing = rends.Count(r =>
            (GameObjectUtility.GetStaticEditorFlags(r.gameObject) & StaticEditorFlags.ContributeGI) != 0);

        if (contributing == 0)
        {
            EditorUtility.DisplayDialog("Pavilion Port — nothing to bake",
                $"None of the {rends.Count} active renderers is marked Contribute GI, so the " +
                "bake would produce an empty result and the scene would go black in play mode.\n\n" +
                "Run \"3b. Mark Contribute GI\" first. It is required for Adaptive Probe " +
                "Volumes as well as for lightmaps.", "OK");
            return;
        }

        if (contributing < rends.Count / 4)
            Debug.LogWarning($"Only {contributing} of {rends.Count} renderers contribute GI. " +
                             "The bake will be sparse — consider running 3b first.");

        Debug.Log($"Baking lighting ({contributing} of {rends.Count} renderers contribute GI). " +
                  "This takes a while; progress is in the Lighting window.");
        Lightmapping.BakeAsync();
    }

    // =======================================================================
    //  1e. Missing scripts — left behind by packages this project lacks
    // =======================================================================
    [MenuItem(Menu + "1e. Remove Missing Scripts", priority = 25)]
    public static void RemoveMissingScripts()
    {
        if (!EnsureSceneOpen()) return;

        int objs = 0, comps = 0;
        foreach (var go in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            foreach (var t in go.GetComponentsInChildren<Transform>(true))
            {
                int n = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);
                if (n == 0) continue;
                Undo.RegisterCompleteObjectUndo(t.gameObject, "Remove missing scripts");
                comps += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
                objs++;
            }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log(comps == 0
            ? "No missing scripts in the scene."
            : $"Removed {comps} missing script(s) from {objs} object(s).");
    }

    // =======================================================================
    //  6. sReader integration probe — bookshelf, book, and a word path
    // =======================================================================
    //  A deliberately small test: does sReader's own library kit and reading
    //  code drop into the ported pavilion and behave?
    //
    //  Uses the project's REAL assets, not stand-ins — P_ShelfBay_A,
    //  P_Book_Hero_Open, the SM_WordStone prefabs, BookStation and
    //  WordPathBuilder — so what you see is what integration actually looks
    //  like. Everything lands under one "sReaderTest" object; delete that and
    //  the pavilion is untouched.
    //
    //  Placed in front of the active camera and dropped onto the floor by
    //  raycast, so it appears where you are looking rather than at some
    //  authored coordinate that may be inside a wall.
    // =======================================================================
    const string TestRoot = "sReaderTest";

    [MenuItem(Menu + "6. Place sReader Test (shelf + book + sentence)", priority = 70)]
    public static void PlaceSReaderTest()
    {
        if (!EnsureSceneOpen()) return;

        GameObject Load(string path)
        {
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null) Debug.LogWarning($"sReader test: missing {path}");
            return go;
        }

        var shelfPf = Load("Assets/Prefabs/Kit/P_ShelfBay_A.prefab");
        var bookPf  = Load("Assets/Prefabs/Kit/P_Book_Hero_Open.prefab");
        var deskPf  = Load("Assets/Prefabs/Kit/P_Desk_Reading.prefab");
        var stoneA  = Load("Assets/Prefabs/SM_WordStone_A.prefab");
        var stoneB  = Load("Assets/Prefabs/SM_WordStone_B.prefab");
        var stoneC  = Load("Assets/Prefabs/SM_WordStone_C.prefab");
        var stoneK  = Load("Assets/Prefabs/SM_WordStone_Key.prefab");

        if (shelfPf == null || bookPf == null || stoneA == null)
        {
            EditorUtility.DisplayDialog("sReader test",
                "Could not find the library kit or word stone prefabs. Nothing placed.", "OK");
            return;
        }

        // --- where: in front of the camera, dropped to the floor -----------
        var cam = Camera.main ?? FindAll<Camera>().FirstOrDefault(c => c.isActiveAndEnabled);
        Vector3 origin = cam != null
            ? cam.transform.position + Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1)).normalized * 6f
            : Vector3.zero;
        if (Physics.Raycast(origin + Vector3.up * 5f, Vector3.down, out var floor, 50f))
            origin = floor.point;
        else
            origin.y = 0f;

        // Face back toward the camera so the shelf and book are readable.
        var faceCam = cam != null
            ? Quaternion.LookRotation(Vector3.Scale(cam.transform.position - origin, new Vector3(1, 0, 1)).normalized)
            : Quaternion.identity;

        // --- clear any previous run so this is repeatable ------------------
        var existing = EditorSceneManager.GetActiveScene().GetRootGameObjects()
            .FirstOrDefault(g => g.name == TestRoot);
        if (existing != null) Undo.DestroyObjectImmediate(existing);

        var root = new GameObject(TestRoot);
        Undo.RegisterCreatedObjectUndo(root, "Place sReader test");
        root.transform.SetPositionAndRotation(origin, faceCam);

        GameObject Place(GameObject pf, string name, Vector3 local, Transform parent)
        {
            if (pf == null) return null;
            var go = (GameObject)PrefabUtility.InstantiatePrefab(pf, parent);
            go.name = name;
            go.transform.localPosition = local;
            go.transform.localRotation = Quaternion.identity;
            Undo.RegisterCreatedObjectUndo(go, "Place sReader test");
            return go;
        }

        // --- the furniture -------------------------------------------------
        Place(shelfPf, "Bookshelf", new Vector3(-2.2f, 0f, 0.6f), root.transform);
        var desk = Place(deskPf, "ReadingDesk", new Vector3(0f, 0f, 0f), root.transform);

        // The book sits on the desk if we have one, otherwise waist height.
        float bookY = 0.95f;
        if (desk != null)
        {
            var rends = desk.GetComponentsInChildren<Renderer>();
            if (rends.Length > 0)
            {
                var b = rends[0].bounds;
                foreach (var r in rends) b.Encapsulate(r.bounds);
                bookY = b.max.y - root.transform.position.y;
            }
        }
        var book = Place(bookPf, "StoryBook", new Vector3(0f, bookY, 0f), root.transform);

        // --- BookStation: sReader's lectern marker --------------------------
        var station = Undo.AddComponent<BookStation>(root);
        if (book != null) station.book = book.transform;

        // --- the word path: one stone per word ------------------------------
        var pathGo = new GameObject("WordPath");
        Undo.RegisterCreatedObjectUndo(pathGo, "Place sReader test");
        pathGo.transform.SetParent(root.transform, false);
        pathGo.transform.localPosition = new Vector3(0f, 0f, -2.5f);

        var a = new GameObject("Start").transform; a.SetParent(pathGo.transform, false);
        a.localPosition = new Vector3(-3f, 0.02f, 0f);
        var z = new GameObject("End").transform; z.SetParent(pathGo.transform, false);
        z.localPosition = new Vector3(3f, 0.02f, 0f);

        var builder = Undo.AddComponent<WordPathBuilder>(pathGo);
        builder.sentence = "The reader walked into the quiet library";
        builder.keywords = new List<string> { "library" };
        builder.stoneA = stoneA; builder.stoneB = stoneB ?? stoneA;
        builder.stoneC = stoneC ?? stoneA; builder.stoneKey = stoneK ?? stoneA;
        builder.startPoint = a; builder.endPoint = z;
        builder.useAuthoredSlots = false;   // no WORDSLOT_ markers in this scene
        builder.usePool = false;            // build directly, simpler at edit time
        builder.frameOnScreen = false;      // keep the authored placement
        builder.curve = 0.4f;
        builder.viewCamera = cam;

        try { builder.Build(); }
        catch (System.Exception e) { Debug.LogWarning($"WordPathBuilder.Build() threw: {e.Message}"); }

        Selection.activeGameObject = root;
        SceneView.lastActiveSceneView?.FrameSelected();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log($"sReader test placed at {origin}.\n" +
                  $"  Bookshelf   : P_ShelfBay_A\n" +
                  $"  Desk + book : P_Desk_Reading + P_Book_Hero_Open\n" +
                  $"  BookStation : book -> {(book == null ? "none" : book.name)}\n" +
                  $"  WordPath    : \"{builder.sentence}\" -> {builder.Stones.Count} stones\n" +
                  $"Delete the '{TestRoot}' object to remove all of it.");
    }

    // =======================================================================
    //  9. Diagnose — read-only. For working out why nothing renders.
    // =======================================================================
    [MenuItem(Menu + "9. Diagnose Camera / Render", priority = 80)]
    public static void Diagnose()
    {
        if (!SceneIsOpen()) { Debug.LogWarning($"Open {ScenePath} first."); return; }

        var sb = new StringBuilder("=== Pavilion render diagnosis ===\n");

        var cam = Camera.main ?? FindAll<Camera>().FirstOrDefault(c => c.isActiveAndEnabled);
        if (cam == null) { Debug.LogError("No active camera at all."); return; }

        var t = cam.transform;
        sb.AppendLine($"Camera            : {cam.name}");
        sb.AppendLine($"  position        : {t.position}");
        sb.AppendLine($"  forward         : {t.forward}");
        sb.AppendLine($"  clip near/far   : {cam.nearClipPlane} / {cam.farClipPlane}");
        sb.AppendLine($"  fov / ortho     : {cam.fieldOfView} / {cam.orthographic}");
        sb.AppendLine($"  clearFlags      : {cam.clearFlags}");
        sb.AppendLine($"  cullingMask     : {cam.cullingMask}  (-1 = everything)");
        sb.AppendLine($"  depth / target  : {cam.depth} / {(cam.targetTexture == null ? "screen" : cam.targetTexture.name)}");
        sb.AppendLine($"  rect            : {cam.rect}");

        var uacd = cam.GetComponent<UniversalAdditionalCameraData>();
        if (uacd != null)
        {
            sb.AppendLine($"  renderer index  : {ReadInt(uacd, "m_RendererIndex")}");
            sb.AppendLine($"  post processing : {uacd.renderPostProcessing}");
            sb.AppendLine($"  renderType      : {uacd.renderType}");
        }

        // --- what is actually in the scene, and where -----------------------
        var rends = FindAll<Renderer>().Where(r => r.enabled && r.gameObject.activeInHierarchy).ToList();
        sb.AppendLine($"\nActive renderers  : {rends.Count}");
        if (rends.Count > 0)
        {
            var b = rends[0].bounds;
            foreach (var r in rends) b.Encapsulate(r.bounds);
            sb.AppendLine($"  scene bounds    : center {b.center}  size {b.size}");
            sb.AppendLine($"  camera inside?  : {b.Contains(t.position)}");
            sb.AppendLine($"  dist to center  : {Vector3.Distance(t.position, b.center):F1} m");

            // How many renderers actually fall inside the camera frustum?
            var planes = GeometryUtility.CalculateFrustumPlanes(cam);
            int visible = rends.Count(r => GeometryUtility.TestPlanesAABB(planes, r.bounds));
            sb.AppendLine($"  in frustum      : {visible}");
        }

        // --- is anything straight ahead? -----------------------------------
        if (Physics.Raycast(t.position, t.forward, out var hit, 5000f))
            sb.AppendLine($"\nRaycast forward   : hit '{hit.collider.name}' at {hit.distance:F1} m");
        else
            sb.AppendLine("\nRaycast forward   : hits nothing (camera may be facing empty space/sky)");

        // --- environment ---------------------------------------------------
        sb.AppendLine($"\nSkybox            : {(RenderSettings.skybox == null ? "NONE" : RenderSettings.skybox.name)}");
        if (RenderSettings.skybox != null)
            sb.AppendLine($"  shader          : {(RenderSettings.skybox.shader == null ? "MISSING" : RenderSettings.skybox.shader.name)}");
        sb.AppendLine($"Ambient mode      : {RenderSettings.ambientMode}  intensity {RenderSettings.ambientIntensity}");
        sb.AppendLine($"Sun               : {(RenderSettings.sun == null ? "none assigned" : RenderSettings.sun.name)}");

        var lights = FindAll<Light>().Where(l => l.isActiveAndEnabled).ToList();
        sb.AppendLine($"Active lights     : {lights.Count}");
        foreach (var l in lights.Take(6))
            sb.AppendLine($"   {l.name,-24} {l.type,-12} intensity={l.intensity} mode={l.lightmapBakeType}");

        var urp = GetUrpAsset();
        if (urp != null)
        {
            var so = new SerializedObject(urp);
            sb.AppendLine($"\nURP renderScale   : {so.FindProperty("m_RenderScale")?.floatValue}");
            sb.AppendLine($"URP HDR           : {so.FindProperty("m_SupportsHDR")?.boolValue}");
            sb.AppendLine($"URP MSAA          : {so.FindProperty("m_MSAA")?.intValue}");
        }

        // --- volumes affecting the camera ----------------------------------
        var vols = FindAll<Volume>().Where(v => v.isActiveAndEnabled).ToList();
        sb.AppendLine($"\nActive volumes    : {vols.Count}");
        foreach (var v in vols)
            sb.AppendLine($"   {v.name,-24} global={v.isGlobal} weight={v.weight} profile={(v.sharedProfile == null ? "none" : v.sharedProfile.name)}");

        Debug.Log(sb.ToString());
    }

    // =======================================================================
    //  9b. Geometry outliers — read-only. Run before baking.
    // =======================================================================
    //  The scene's combined renderer bounds are ~4.4 km tall, which is not the
    //  pavilion. Lightmapping and APV size their working volume from those
    //  bounds, so one stray object kilometres away silently wrecks bake
    //  quality (and bake time) for everything else.
    //
    //  Reports by distance from the MEDIAN centre rather than the mean, so a
    //  handful of outliers cannot drag the reference point out with them.
    // =======================================================================
    [MenuItem(Menu + "9b. Find Geometry Outliers", priority = 81)]
    public static void FindOutliers()
    {
        if (!SceneIsOpen()) { Debug.LogWarning($"Open {ScenePath} first."); return; }

        var rends = FindAll<Renderer>().Where(r => r.enabled && r.gameObject.activeInHierarchy).ToList();
        if (rends.Count == 0) { Debug.Log("No active renderers."); return; }

        Vector3 Median(System.Func<Renderer, float> sel)
        {
            var v = rends.Select(sel).OrderBy(x => x).ToList();
            return Vector3.one * v[v.Count / 2];
        }
        var centre = new Vector3(
            Median(r => r.bounds.center.x).x,
            Median(r => r.bounds.center.y).y,
            Median(r => r.bounds.center.z).z);

        var ranked = rends
            .Select(r => new { r, d = Vector3.Distance(r.bounds.center, centre) })
            .OrderByDescending(x => x.d)
            .ToList();

        var sb = new StringBuilder("=== Geometry outliers ===\n");
        sb.AppendLine($"Median centre     : {centre}");
        sb.AppendLine($"Renderers         : {rends.Count}");

        var bAll = rends[0].bounds;
        foreach (var r in rends) bAll.Encapsulate(r.bounds);
        sb.AppendLine($"Bounds WITH all   : size {bAll.size}");

        // What the bounds would be if the worst offenders were dealt with.
        var keep = ranked.Where(x => x.d < 500f).Select(x => x.r).ToList();
        if (keep.Count > 0)
        {
            var bKeep = keep[0].bounds;
            foreach (var r in keep) bKeep.Encapsulate(r.bounds);
            sb.AppendLine($"Bounds WITHOUT    : size {bKeep.size}   ({rends.Count - keep.Count} excluded, >500 m out)");
        }

        sb.AppendLine("\nFurthest 12:");
        foreach (var x in ranked.Take(12))
            sb.AppendLine($"  {x.d,10:F1} m   {Path(x.r.transform)}");

        sb.AppendLine("\nSelect them in the Hierarchy with the selection below.");
        Debug.Log(sb.ToString());

        // Leave the far ones selected so they can be inspected or deleted.
        var far = ranked.Where(x => x.d > 500f).Select(x => (Object)x.r.gameObject).ToArray();
        if (far.Length > 0)
        {
            Selection.objects = far;
            Debug.Log($"Selected {far.Length} renderer(s) more than 500 m from the pavilion.");
        }
        else Debug.Log("Nothing beyond 500 m — the tall bounds come from something closer in.");
    }

    // =======================================================================
    //  9c. Remove stray geometry from the PORT'S PREFABS
    // =======================================================================
    //  Leaves.prefab ships with one leaf at y = -4422.7 — present in Unity's
    //  original HDRP sample, so this is an upstream authoring slip rather than
    //  anything the port did. It is invisible in play, but it stretches the
    //  scene's GI bounds to ~4.4 km, which ruins lightmap and APV bake density.
    //
    //  Fixes the prefab rather than the scene instance, so the strays cannot
    //  come back the next time the prefab is used.
    // =======================================================================
    [MenuItem(Menu + "9c. Remove Stray Prefab Geometry", priority = 82)]
    public static void RemoveStrays()
    {
        const float MaxFromRoot = 500f;

        var found = new List<(string prefab, string child, Vector3 pos)>();
        foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/PavilionPort" }))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var root = PrefabUtility.LoadPrefabContents(path);
            foreach (var tr in root.GetComponentsInChildren<Transform>(true))
                if (tr != root.transform && tr.position.magnitude > MaxFromRoot)
                    found.Add((path, tr.name, tr.position));
            PrefabUtility.UnloadPrefabContents(root);
        }

        if (found.Count == 0) { Debug.Log("No stray geometry beyond 500 m in any PavilionPort prefab."); return; }

        var list = string.Join("\n", found.Take(10).Select(f =>
            $"  {System.IO.Path.GetFileName(f.prefab)} → {f.child}  at {f.pos}"));

        if (!EditorUtility.DisplayDialog("Pavilion Port",
                $"Delete {found.Count} stray object(s) from the port's prefabs?\n\n{list}\n\n" +
                "These sit kilometres from the pavilion and stretch the GI bounds, " +
                "ruining bake density. Recoverable from git.",
                $"Delete {found.Count}", "Cancel"))
            return;

        int removed = 0;
        foreach (var g in found.GroupBy(f => f.prefab))
        {
            var root = PrefabUtility.LoadPrefabContents(g.Key);
            foreach (var tr in root.GetComponentsInChildren<Transform>(true).ToList())
            {
                if (tr == null || tr == root.transform) continue;
                if (tr.position.magnitude <= MaxFromRoot) continue;
                Object.DestroyImmediate(tr.gameObject);
                removed++;
            }
            PrefabUtility.SaveAsPrefabAsset(root, g.Key);
            PrefabUtility.UnloadPrefabContents(root);
        }
        AssetDatabase.SaveAssets();
        Debug.Log($"Removed {removed} stray object(s). Re-run 9b — the scene bounds should now be " +
                  "the pavilion's real size, and the bake will be worth running.");
    }

    static string Path(Transform t)
    {
        var s = t.name;
        while (t.parent != null) { t = t.parent; s = t.name + "/" + s; }
        return s;
    }

    // =======================================================================
    //  helpers
    // =======================================================================

    static bool SceneIsOpen() => EditorSceneManager.GetActiveScene().path == ScenePath;

    static bool EnsureSceneOpen()
    {
        if (SceneIsOpen()) return true;

        if (!EditorUtility.DisplayDialog("Pavilion Port",
                $"This works on {ScenePath}, which is not the active scene.\n\nOpen it now?",
                "Open scene", "Cancel"))
            return false;

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return false;
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        return SceneIsOpen();
    }

    static List<T> FindAll<T>() where T : Object =>
        Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();

    /// <summary>
    /// The active URP asset. GraphicsSettings.defaultRenderPipeline is null in
    /// this project — the pipeline is set per quality level — so ask
    /// QualitySettings first.
    /// </summary>
    static UniversalRenderPipelineAsset GetUrpAsset() =>
        (QualitySettings.renderPipeline ?? GraphicsSettings.defaultRenderPipeline) as UniversalRenderPipelineAsset;

    static int FindRendererIndex(UniversalRenderPipelineAsset urp, string rendererName)
    {
        if (urp == null) return -1;
        var so = new SerializedObject(urp);
        var list = so.FindProperty("m_RendererDataList");
        if (list == null) return -1;
        for (int i = 0; i < list.arraySize; i++)
        {
            var o = list.GetArrayElementAtIndex(i).objectReferenceValue;
            if (o != null && o.name == rendererName) return i;
        }
        return -1;
    }

    static int GetDefaultRendererIndex(UniversalRenderPipelineAsset urp)
    {
        if (urp == null) return -1;
        var p = new SerializedObject(urp).FindProperty("m_DefaultRendererIndex");
        return p == null ? -1 : p.intValue;
    }

    static bool HasDecalFeature(ScriptableRendererData rd)
    {
        var list = new SerializedObject(rd).FindProperty("m_RendererFeatures");
        if (list == null) return false;
        for (int i = 0; i < list.arraySize; i++)
        {
            var o = list.GetArrayElementAtIndex(i).objectReferenceValue;
            if (o is DecalRendererFeature) return true;
        }
        return false;
    }

    static int ReadInt(Object target, string path)
    {
        var p = new SerializedObject(target).FindProperty(path);
        return p == null ? -1 : p.intValue;
    }

    static string ReadEnumName(Object target, string path)
    {
        if (target == null) return "?";
        var p = new SerializedObject(target).FindProperty(path);
        if (p == null || p.enumNames == null || p.enumValueIndex < 0 || p.enumValueIndex >= p.enumNames.Length)
            return "?";
        return p.enumNames[p.enumValueIndex];
    }

    /// <summary>
    /// Sets an enum property by the NAME of the value rather than its ordinal,
    /// so a URP update that reorders the enum cannot silently select the wrong
    /// option. Logs and does nothing if the name is not found.
    /// </summary>
    static void SetEnumByName(SerializedObject so, string path, string valueName)
    {
        var p = so.FindProperty(path);
        if (p == null) { Debug.LogWarning($"Pavilion Port: property '{path}' not found — left at default."); return; }
        int i = System.Array.IndexOf(p.enumNames, valueName);
        if (i < 0) { Debug.LogWarning($"Pavilion Port: '{valueName}' not a value of '{path}' — left at default."); return; }
        p.enumValueIndex = i;
    }
}
