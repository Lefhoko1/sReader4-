// ===========================================================================
//  GreatLibraryVista3D — make SC_IslandVista actually render as a lit 3D world
// ===========================================================================
//  Tools > Great Library > Island > 5. Make Vista 3D-Lit & Beautify
//
//  THE ROOT-CAUSE FIX. The whole project renders through the URP *2D* renderer
//  (Assets/Settings/UniversalRP.asset -> Renderer2D), which cannot light 3D
//  meshes — so the island looks flat no matter how good the models are. This
//  tool, run once, does the surgical thing:
//
//    1. Ensures a 3D "Universal Renderer" exists (Assets/Settings/Renderer3D)
//       and is registered on UniversalRP.asset as an ADDITIONAL renderer. The
//       app's default renderer stays Renderer2D (index 0) — nothing else is
//       touched or at risk. The island camera alone is overridden to the 3D
//       renderer, so it gets real directional lighting, shadows and post.
//
//    2. Beautifies the OPEN SC_IslandVista scene:
//         • Main Camera -> PERSPECTIVE (the Polish script left it orthographic,
//           which flattens the whole vista), framed across the sea, renderer
//           overridden to the 3D renderer.
//         • A warm "Sun" directional light (created if the scene has none — it
//           doesn't) with soft shadows.
//         • Linear fog + trilight ambient so sea melts into sky and shapes
//           shade properly.
//         • The Global Volume gets the IslandPostProfile wired (bloom,
//           tonemapping, a touch of colour/vignette) — created if missing.
//
//  Judge the result in the GAME VIEW (and builds): per-camera renderer
//  overrides apply there. The Scene view uses the default renderer and may
//  still preview flat — that's expected, not a failure.
//
//  Safe to re-run. Reversible: delete Renderer3D.asset's entry from the URP
//  asset's renderer list to go back.
// ===========================================================================
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class GreatLibraryVista3D
{
    const string URP_PATH      = "Assets/Settings/UniversalRP.asset";
    const string RENDERER3D    = "Assets/Settings/Renderer3D.asset";
    const string PROFILE_PATH  = "Assets/Settings/IslandPostProfile.asset";
    const string SCENE_PATH    = "Assets/Scenes/SC_IslandVista.unity";

    [MenuItem("Tools/Great Library/Island/5. Make Vista 3D-Lit && Beautify")]
    public static void Beautify()
    {
        // ── 1. a 3D renderer, registered on the URP asset ──────────────────
        int rendererIndex = EnsureThreeDRenderer(out string rendererErr);
        if (rendererIndex < 0)
        {
            Debug.LogError("[Vista3D] " + rendererErr);
            return;
        }

        // ── make sure SC_IslandVista is the open, active scene ─────────────
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.path != SCENE_PATH)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (!File.Exists(SCENE_PATH))
            {
                Debug.LogError("[Vista3D] " + SCENE_PATH + " not found — run " +
                               "'2. Build Vista Scene' first.");
                return;
            }
            scene = EditorSceneManager.OpenScene(SCENE_PATH, OpenSceneMode.Single);
        }

        // ── 2a. camera: perspective + 3D renderer override ─────────────────
        var camGO = GameObject.Find("Main Camera");
        if (camGO == null)
        {
            Debug.LogError("[Vista3D] No 'Main Camera' in SC_IslandVista.");
            return;
        }
        var cam = camGO.GetComponent<Camera>();
        camGO.tag = "MainCamera";
        cam.orthographic     = false;                       // THE flatness fix
        cam.fieldOfView      = 40f;
        cam.nearClipPlane    = 0.3f;
        cam.farClipPlane     = 400f;
        cam.clearFlags       = CameraClearFlags.SolidColor; // blends into fog
        cam.backgroundColor  = new Color(0.62f, 0.78f, 0.94f);
        camGO.transform.position = new Vector3(3.5f, 6.0f, -27f);
        camGO.transform.rotation = Quaternion.Euler(6.5f, -6f, 0f);

        var camData = cam.GetUniversalAdditionalCameraData();
        camData.SetRenderer(rendererIndex);                 // island uses the 3D path
        camData.renderPostProcessing = true;
        camData.renderShadows        = true;

        // ── 2b. a real sun (the scene ships with none) ─────────────────────
        var sunGO = GameObject.Find("Sun") ?? GameObject.Find("Directional Light");
        if (sunGO == null || sunGO.GetComponent<Light>() == null)
        {
            sunGO = new GameObject("Sun");
            sunGO.AddComponent<Light>();
        }
        sunGO.name = "Sun";
        var sun = sunGO.GetComponent<Light>();
        sun.type            = LightType.Directional;
        sun.color           = new Color(1.0f, 0.93f, 0.80f);
        sun.intensity       = 1.35f;
        sun.shadows         = LightShadows.Soft;
        sun.shadowStrength  = 0.65f;
        sunGO.transform.rotation = Quaternion.Euler(42f, -48f, 0f);
        RenderSettings.sun = sun;

        // ── 2c. fog + trilight ambient ─────────────────────────────────────
        RenderSettings.fog              = true;
        RenderSettings.fogMode          = FogMode.Linear;
        RenderSettings.fogColor         = new Color(0.66f, 0.80f, 0.92f);
        RenderSettings.fogStartDistance = 22f;
        RenderSettings.fogEndDistance   = 110f;
        RenderSettings.ambientMode         = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor     = new Color(0.72f, 0.82f, 0.94f);
        RenderSettings.ambientEquatorColor = new Color(0.52f, 0.62f, 0.70f);
        RenderSettings.ambientGroundColor  = new Color(0.30f, 0.40f, 0.34f);

        // ── 2d. post: bloom + tonemapping + a little grade ─────────────────
        WirePostProfile();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[Vista3D] Done. Island renders on the 3D renderer (index " +
                  rendererIndex + "): perspective camera, warm sun + soft " +
                  "shadows, fog, bloom. LOOK AT THE GAME VIEW — the Scene view " +
                  "still uses the default 2D renderer and may preview flat.");
    }

    // Create Renderer3D.asset if missing and make sure UniversalRP.asset lists
    // it. Returns its index in the renderer list, or -1 on failure.
    static int EnsureThreeDRenderer(out string error)
    {
        error = null;
        var urp = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(URP_PATH);
        if (urp == null)
        {
            error = URP_PATH + " not found (is this the active URP asset?).";
            return -1;
        }

        var data = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(RENDERER3D);
        if (data == null)
        {
            data = ScriptableObject.CreateInstance<UniversalRendererData>();
            data.name = "Renderer3D";
            AssetDatabase.CreateAsset(data, RENDERER3D);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            Debug.Log("[Vista3D] Created 3D Universal Renderer at " + RENDERER3D);
        }

        // Register on the URP asset's renderer list via SerializedObject (the
        // list is private; this is the reflection-free, version-safe way).
        var so   = new SerializedObject(urp);
        var list = so.FindProperty("m_RendererDataList");
        if (list == null) { error = "URP asset has no m_RendererDataList."; return -1; }

        for (int i = 0; i < list.arraySize; i++)
            if (list.GetArrayElementAtIndex(i).objectReferenceValue == data)
                return i;                                   // already registered

        int idx = list.arraySize;
        list.arraySize = idx + 1;
        list.GetArrayElementAtIndex(idx).objectReferenceValue = data;
        so.ApplyModifiedProperties();                       // default index left at 0 (2D) on purpose
        EditorUtility.SetDirty(urp);
        AssetDatabase.SaveAssets();
        Debug.Log("[Vista3D] Registered Renderer3D on UniversalRP at index " + idx +
                  " (default renderer unchanged — the app still uses Renderer2D).");
        return idx;
    }

    static void WirePostProfile()
    {
        Directory.CreateDirectory("Assets/Settings");
        var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(PROFILE_PATH);
        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, PROFILE_PATH);
        }

        var bloom = profile.components.OfType<Bloom>().FirstOrDefault() ?? profile.Add<Bloom>(true);
        bloom.active = true;
        bloom.intensity.overrideState = true; bloom.intensity.value = 0.75f;
        bloom.threshold.overrideState = true; bloom.threshold.value = 0.95f;
        bloom.scatter.overrideState   = true; bloom.scatter.value   = 0.7f;

        var tone = profile.components.OfType<Tonemapping>().FirstOrDefault() ?? profile.Add<Tonemapping>(true);
        tone.active = true;
        tone.mode.overrideState = true; tone.mode.value = TonemappingMode.Neutral;

        var grade = profile.components.OfType<ColorAdjustments>().FirstOrDefault() ?? profile.Add<ColorAdjustments>(true);
        grade.active = true;
        grade.postExposure.overrideState = true; grade.postExposure.value = 0.15f;
        grade.saturation.overrideState   = true; grade.saturation.value   = 12f;
        grade.contrast.overrideState     = true; grade.contrast.value     = 8f;

        var vig = profile.components.OfType<Vignette>().FirstOrDefault() ?? profile.Add<Vignette>(true);
        vig.active = true;
        vig.intensity.overrideState = true; vig.intensity.value = 0.22f;
        vig.smoothness.overrideState = true; vig.smoothness.value = 0.8f;

        EditorUtility.SetDirty(profile);

        var volGO = GameObject.Find("Global Volume");
        if (volGO == null) volGO = new GameObject("Global Volume");
        var vol = volGO.GetComponent<Volume>() ?? volGO.AddComponent<Volume>();
        vol.isGlobal = true;
        vol.priority = 0;
        vol.sharedProfile = profile;
    }
}
