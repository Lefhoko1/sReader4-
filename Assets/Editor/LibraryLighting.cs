// ===========================================================================
//  LibraryLighting — warm pools in a cool world (Editor)
// ===========================================================================
//  Tools > Great Library > Library >
//     7. Light The World        — sun, ambient, fog, emissives, interior lights
//     7b. Remove Library Lights
//
//  RUN ISLAND ▸ 5 FIRST if you have not. That is the tool that gets this
//  project rendering 3D at all — everything ships through the URP *2D*
//  renderer, which cannot light a mesh. This one assumes that fix is in and
//  spends its effort on the look.
//
//  THE RULE (Production Bible Ch. 3.2 / 3.4): the visual battle is WARMTH
//  against FOG, and lighting IS the progress bar. So the world outside is kept
//  cool and a little hazy, and every warm source — a window, a candle, the
//  reading lamp — is placed deliberately and reads as precious. A screenshot
//  should say "candlelit library" before you have read a word of UI.
//
//  WHAT WAS ACTUALLY MISSING
//    • The library is a closed box with a ceiling. Nothing lit the inside, so
//      the room the whole game happens in was black.
//    • The Blender lanterns and candles are MESHES. They looked like lights
//      and emitted nothing.
//    • M_WindowGlow, M_CrystalViolet and friends came in off the FBX with
//      emission disabled, so the bloom in the post stack had nothing to catch.
//    • URP will silently ignore extra lights if the pipeline asset's additional
//      light budget is off or too low — so that is checked too.
// ===========================================================================
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class LibraryLighting
{
    const string ROOT = "LibraryLights";
    const string MAT_DIR = "Assets/Art/Materials_Island";
    const string PROFILE = "Assets/Settings/IslandPostProfile.asset";

    // Warm is the reward, cool is the world. Two colours, used everywhere.
    static readonly Color WARM = new Color(1.00f, 0.82f, 0.55f);
    static readonly Color EMBER = new Color(1.00f, 0.66f, 0.32f);
    static readonly Color SKYLIGHT = new Color(0.78f, 0.87f, 1.00f);

    // material -> (emission colour, strength)
    static readonly (string mat, Color col, float mul)[] EMISSIVE =
    {
        ("M_WindowGlow",    new Color(1.00f, 0.72f, 0.30f), 3.0f),
        ("M_CrystalViolet", new Color(0.55f, 0.40f, 1.00f), 2.2f),
        ("M_GoldTrim",      new Color(1.00f, 0.78f, 0.35f), 0.25f),
        ("M_SpineGem",      new Color(0.60f, 0.45f, 1.00f), 2.0f),
    };

    // ======================================================================
    [MenuItem("Tools/Great Library/Library/7. Light The World")]
    public static void Light()
    {
        var log = new List<string>();

        log.Add(Emissives());
        log.Add(LightBudget());
        log.Add(SunAndSky());
        log.Add(InteriorLights());
        log.Add(Grade());

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("[Light] " + string.Join("\n  • ", new[] { "World lit." }.Concat(log)) +
                  "\n  → judge it in the GAME view: the per-camera 3D renderer " +
                  "override does not apply to the Scene view, which will still " +
                  "preview flat. That is expected, not a failure.");
    }

    [MenuItem("Tools/Great Library/Library/7. Light The World", true)]
    static bool LightValidate() => !Application.isPlaying;

    // ── 1. make the glowing things actually glow ────────────────────────────

    /// <summary>
    /// Blender's emission does not survive the FBX round trip — the materials
    /// arrive as plain lit surfaces with the right colour and no glow at all.
    /// Without this the bloom in the post stack has nothing to bloom.
    /// </summary>
    static string Emissives()
    {
        int n = 0;
        foreach (var (mat, col, mul) in EMISSIVE)
        {
            var m = AssetDatabase.LoadAssetAtPath<Material>($"{MAT_DIR}/{mat}.mat");
            if (m == null) continue;
            Undo.RecordObject(m, "Light The World");
            m.EnableKeyword("_EMISSION");
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            if (m.HasProperty("_EmissionColor")) m.SetColor("_EmissionColor", col * mul);
            EditorUtility.SetDirty(m);
            n++;
        }
        AssetDatabase.SaveAssets();
        return $"{n} material(s) now emit (windows, crystals, gold trim)";
    }

    // ── 2. URP has to be willing to draw the extra lights ───────────────────

    /// <summary>
    /// A pile of point lights does nothing if the pipeline asset is set to one
    /// light per object, which is a common mobile default. Most of these fields
    /// are private, hence SerializedObject rather than the public API.
    /// </summary>
    static string LightBudget()
    {
        var assets = AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset")
            .Select(g => AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(
                         AssetDatabase.GUIDToAssetPath(g)))
            .Where(a => a != null).ToList();
        if (assets.Count == 0) return "no URP asset found — light budget unchanged";

        int touched = 0;
        foreach (var a in assets)
        {
            var so = new SerializedObject(a);
            void Set(string prop, int v)
            {
                var p = so.FindProperty(prop);
                if (p != null && p.intValue < v) { p.intValue = v; touched++; }
            }
            Set("m_AdditionalLightsRenderingMode", 1);   // 0 disabled, 1 per-pixel
            Set("m_AdditionalLightsPerObjectLimit", 8);
            var sh = so.FindProperty("m_AdditionalLightShadowsSupported");
            if (sh != null && !sh.boolValue) { sh.boolValue = true; touched++; }
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(a);
        }
        AssetDatabase.SaveAssets();
        return $"URP light budget raised on {assets.Count} asset(s) ({touched} field(s))";
    }

    // ── 3. one warm key, a cool sky, and haze sized to the island ───────────

    static string SunAndSky()
    {
        var sunGO = GameObject.Find("Sun") ?? GameObject.Find("Directional Light");
        if (sunGO == null)
        {
            sunGO = new GameObject("Sun");
            Undo.RegisterCreatedObjectUndo(sunGO, "Light The World");
            sunGO.AddComponent<Light>();
        }
        var sun = sunGO.GetComponent<Light>();
        Undo.RecordObject(sun, "Light The World");
        Undo.RecordObject(sunGO.transform, "Light The World");

        sun.type = LightType.Directional;
        sun.color = new Color(1.00f, 0.94f, 0.82f);   // low, warm, late afternoon
        sun.intensity = 1.35f;
        sun.shadows = LightShadows.Soft;
        sun.shadowStrength = 0.62f;                   // soft, not black — storybook
        sun.shadowBias = 0.04f;
        sun.shadowNormalBias = 0.5f;
        // Raking across the island rather than straight down: the columns, the
        // dome and the stones only read as solid when they cast something.
        sunGO.transform.rotation = Quaternion.Euler(34f, 214f, 0f);
        RenderSettings.sun = sun;

        // Cool ambient so every warm source below is worth something.
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.60f, 0.72f, 0.90f);
        RenderSettings.ambientEquatorColor = new Color(0.42f, 0.52f, 0.62f);
        RenderSettings.ambientGroundColor = new Color(0.24f, 0.30f, 0.30f);
        RenderSettings.ambientIntensity = 1f;

        // Fog re-sized: the island is 17 m now, not 23. The old 22..110 m range
        // put the haze entirely behind the world, where nobody could see it.
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = new Color(0.62f, 0.76f, 0.88f);
        RenderSettings.fogStartDistance = 12f;
        RenderSettings.fogEndDistance = 62f;
        return "sun raking at 34°, cool trilight ambient, fog re-sized to the " +
               "17 m island (12–62 m)";
    }

    // ── 4. the room the game happens in ─────────────────────────────────────

    /// <summary>
    /// The library is a sealed box. Every lantern and candle in it is a mesh with
    /// no light attached, so without this the one space the player spends all
    /// their time in renders black.
    /// </summary>
    static string InteriorLights()
    {
        var lib = FindAny("SM_Library_Exterior");

        // Clear the old set BEFORE making the new one, or Find picks up the object
        // we just created and deletes that instead — leaving the previous run's
        // lamps behind and doubling them on every re-run.
        var old = GameObject.Find(ROOT);
        if (old != null) Undo.DestroyObjectImmediate(old);

        var root = new GameObject(ROOT);
        Undo.RegisterCreatedObjectUndo(root, "Light The World");
        if (lib != null) root.transform.SetParent(lib, true);

        int n = 0;
        // every hanging lantern and candle cluster that came from Blender
        foreach (var t in AllNamed("Lantern"))
        { Lamp(root.transform, t.position, WARM, 2.4f, 5.0f, "Lamp_Lantern"); n++; }
        foreach (var t in AllNamed("Candles"))
        { Lamp(root.transform, t.position + Vector3.up * 0.1f, EMBER, 1.6f, 3.0f, "Lamp_Candle"); n++; }
        foreach (var t in AllNamed("SM_LanternPost"))
        { Lamp(root.transform, t.position + Vector3.up * 1.0f, EMBER, 1.8f, 4.5f, "Lamp_Post"); n++; }

        // the reading lamp: the one that has to make the PAGE legible
        var desk = FindAny("SOCKET_ReadingDesk");
        if (desk != null)
        { Lamp(root.transform, desk.position + Vector3.up * 1.7f, WARM, 3.4f, 5.5f, "Lamp_Reading"); n++; }

        // daylight down the oculus — the dome is open over the hall, so this is
        // the shaft that keeps the room from being a cave
        var centre = FindAny("SOCKET_HallCentre");
        if (centre != null)
        {
            var go = new GameObject("Lamp_Oculus");
            Undo.RegisterCreatedObjectUndo(go, "Light The World");
            go.transform.SetParent(root.transform, false);
            go.transform.position = centre.position + Vector3.up * 3.6f;
            go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            var l = go.AddComponent<Light>();
            l.type = LightType.Spot;
            l.color = SKYLIGHT;
            l.intensity = 4.0f;
            l.range = 9f;
            l.spotAngle = 78f;
            l.innerSpotAngle = 30f;
            l.shadows = LightShadows.None;
            n++;
        }
        return $"{n} light(s) placed — lanterns, candles, path posts, the reading " +
               "lamp and a shaft down the oculus";
    }

    static void Lamp(Transform parent, Vector3 pos, Color c, float intensity,
                     float range, string name)
    {
        var go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Light The World");
        go.transform.SetParent(parent, false);
        go.transform.position = pos;
        var l = go.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = c;
        l.intensity = intensity;
        l.range = range;
        l.shadows = LightShadows.None;      // mobile: never shadow the small lamps
    }

    // ── 5. the grade ────────────────────────────────────────────────────────

    static string Grade()
    {
        var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(PROFILE);
        if (profile == null) return "no IslandPostProfile — run Island ▸ 5 first";

        // ACES over Neutral: it rolls the highlights instead of clipping them,
        // which is what stops the emissive windows from turning into white holes.
        var tone = Get<Tonemapping>(profile);
        tone.mode.overrideState = true; tone.mode.value = TonemappingMode.ACES;

        var bloom = Get<Bloom>(profile);
        bloom.threshold.overrideState = true; bloom.threshold.value = 0.85f;
        bloom.intensity.overrideState = true; bloom.intensity.value = 0.75f;
        bloom.scatter.overrideState = true; bloom.scatter.value = 0.72f;
        bloom.tint.overrideState = true; bloom.tint.value = new Color(1f, 0.92f, 0.80f);

        var grade = Get<ColorAdjustments>(profile);
        grade.postExposure.overrideState = true; grade.postExposure.value = 0.15f;
        grade.contrast.overrideState = true; grade.contrast.value = 14f;
        grade.saturation.overrideState = true; grade.saturation.value = 12f;
        grade.colorFilter.overrideState = true;
        grade.colorFilter.value = new Color(1.00f, 0.98f, 0.94f);

        // Warm the highlights, cool the shadows — the whole look in one component.
        var sat = Get<ShadowsMidtonesHighlights>(profile);
        sat.shadows.overrideState = true;
        sat.shadows.value = new Vector4(0.86f, 0.94f, 1.10f, 0f);
        sat.highlights.overrideState = true;
        sat.highlights.value = new Vector4(1.10f, 1.02f, 0.88f, 0f);

        var vig = Get<Vignette>(profile);
        vig.intensity.overrideState = true; vig.intensity.value = 0.28f;
        vig.smoothness.overrideState = true; vig.smoothness.value = 0.45f;

        // Gaussian, far-field only: the sea past the island softens and the whole
        // thing reads as a miniature. Bokeh would look better and cost far more
        // than a Huawei Y5 can spare.
        var dof = Get<DepthOfField>(profile);
        dof.mode.overrideState = true; dof.mode.value = DepthOfFieldMode.Gaussian;
        dof.gaussianStart.overrideState = true; dof.gaussianStart.value = 26f;
        dof.gaussianEnd.overrideState = true; dof.gaussianEnd.value = 60f;
        dof.gaussianMaxRadius.overrideState = true; dof.gaussianMaxRadius.value = 0.8f;

        EditorUtility.SetDirty(profile);
        AssetDatabase.SaveAssets();
        return "grade: ACES, warm bloom, cool shadows / warm highlights, vignette, " +
               "far-field blur so the island reads as a miniature";
    }

    static T Get<T>(VolumeProfile p) where T : VolumeComponent
    {
        var c = p.components.OfType<T>().FirstOrDefault();
        return c != null ? c : p.Add<T>(true);
    }

    // ======================================================================
    [MenuItem("Tools/Great Library/Library/7b. Remove Library Lights")]
    public static void Remove()
    {
        var go = GameObject.Find(ROOT);
        if (go != null) Undo.DestroyObjectImmediate(go);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("[Light] Interior lights removed. The grade, emissives and sun " +
                  "stay — re-run 7 to put the lamps back.");
    }

    // ── helpers ─────────────────────────────────────────────────────────────

    static Transform FindAny(string name) =>
        Object.FindObjectsByType<Transform>(FindObjectsInactive.Exclude,
                                            FindObjectsSortMode.None)
              .FirstOrDefault(t => t.name == name);

    static IEnumerable<Transform> AllNamed(string prefix) =>
        Object.FindObjectsByType<Transform>(FindObjectsInactive.Exclude,
                                            FindObjectsSortMode.None)
              .Where(t => t.name.StartsWith(prefix))
              .ToList();
}
