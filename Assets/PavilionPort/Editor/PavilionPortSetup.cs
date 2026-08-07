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
    //  3b. Route B — traditional lightmaps, isolated to this scene.
    // =======================================================================
    [MenuItem(Menu + "3b. Mark Scene Static for Lightmaps (Route B)", priority = 41)]
    public static void MarkStatic()
    {
        if (!EnsureSceneOpen()) return;

        if (!EditorUtility.DisplayDialog("Pavilion Port",
                "Mark every MeshRenderer in the scene as Contribute GI + Batching Static?\n\n" +
                "This is the alternative to Adaptive Probe Volumes. It writes static flags " +
                "as scene overrides only — the prefab assets are not modified.",
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

        Debug.Log("Baking lighting — this takes a while. Progress is in the Lighting window.");
        Lightmapping.BakeAsync();
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
