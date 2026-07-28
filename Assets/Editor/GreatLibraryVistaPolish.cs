// ===========================================================================
//  GreatLibraryVistaPolish — one-click atmosphere for the island scene
// ===========================================================================
//  Tools > Great Library > Island > 4. Polish Vista & Add Flow
//
//  Fixes the "flat cartoon" look and framing in the OPEN scene:
//    • Camera: wider framing (sees the whole island + dome), FOV 38,
//      solid sky-blue background that blends into…
//    • Fog: linear atmospheric fog so the sea melts into the sky instead of
//      ending in a hard horizon line — the single biggest depth win
//    • Sun: warm colour, soft shadows, tuned intensity
//    • Ambient: trilight (sky / horizon / ground) so shapes shade properly
//    • Water & grass material touch-ups (spec highlight on the sea)
//    • Adds the IslandFlow object (camera arrival -> Enter -> reading)
//      and removes any bare ReadingAdventure object so the page no longer
//      pops over the house — the flow now owns when reading begins.
//
//  Run it with SC_IslandVista open. Safe to re-run.
// ===========================================================================
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class GreatLibraryVistaPolish
{
    const string MAT_DIR = "Assets/Art/Materials_Island";

    [MenuItem("Tools/Great Library/Island/4. Polish Vista && Add Flow")]
    public static void Polish()
    {
        // ---------------- camera ---------------------------------------
        var camGO = GameObject.Find("Main Camera");
        if (camGO == null)
        {
            Debug.LogError("[Polish] No 'Main Camera' in the open scene — " +
                           "open SC_IslandVista first.");
            return;
        }
        var cam = camGO.GetComponent<Camera>();
        camGO.transform.position = new Vector3(3.5f, 6.0f, -27f);
        camGO.transform.rotation = Quaternion.Euler(6.5f, -6f, 0f);
        cam.fieldOfView = 38f;
        cam.farClipPlane = 300f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.62f, 0.78f, 0.94f);   // sky

        // ---------------- fog + ambient --------------------------------
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = new Color(0.66f, 0.80f, 0.92f);
        RenderSettings.fogStartDistance = 18f;
        RenderSettings.fogEndDistance = 85f;

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor     = new Color(0.72f, 0.82f, 0.94f);
        RenderSettings.ambientEquatorColor = new Color(0.52f, 0.62f, 0.70f);
        RenderSettings.ambientGroundColor  = new Color(0.30f, 0.40f, 0.34f);

        // ---------------- sun ------------------------------------------
        var sunGO = GameObject.Find("Directional Light");
        if (sunGO != null)
        {
            sunGO.transform.rotation = Quaternion.Euler(42f, -48f, 0f);
            var sun = sunGO.GetComponent<Light>();
            sun.color = new Color(1.0f, 0.93f, 0.80f);
            sun.intensity = 1.35f;
            sun.shadows = LightShadows.Soft;
            sun.shadowStrength = 0.65f;
        }

        // ---------------- material touch-ups ---------------------------
        void Touch(string name, System.Action<Material> f)
        {
            var m = AssetDatabase.LoadAssetAtPath<Material>(
                $"{MAT_DIR}/{name}.mat");
            if (m != null) { f(m); EditorUtility.SetDirty(m); }
        }
        Touch("M_Sea", m =>
        {
            // Step 5 moves M_Sea onto GreatLibrary/StylizedSea. Don't drag it
            // back to flat URP/Lit paint if that has already run.
            if (m.HasProperty("_ShallowColor")) return;
            m.SetColor("_BaseColor", new Color(0.18f, 0.46f, 0.56f));
            m.SetFloat("_Smoothness", 0.82f);           // sun sparkle
        });
        Touch("M_Grass", m =>
            m.SetColor("_BaseColor", new Color(0.34f, 0.58f, 0.25f)));
        Touch("M_WindowGlow", m =>
            m.SetColor("_EmissionColor",
                       new Color(1.0f, 0.72f, 0.30f) * 3.5f));
        AssetDatabase.SaveAssets();

        // ---------------- flow object ----------------------------------
        // remove bare ReadingAdventure objects (flow owns reading now)
        var raType = FindType("ReadingAdventure");
        if (raType != null)
        {
            foreach (var c in Object.FindObjectsByType(
                         raType, FindObjectsSortMode.None)
                         .OfType<Component>().ToArray())
            {
                Debug.Log("[Polish] Removing bare ReadingAdventure object '" +
                          c.gameObject.name + "' — IslandFlow starts reading " +
                          "after the door now.");
                Object.DestroyImmediate(c.gameObject);
            }
        }
        var flowType = FindType("IslandFlow");
        if (flowType == null)
        {
            Debug.LogWarning("[Polish] IslandFlow.cs not found — copy it to " +
                             "Assets/Scripts/Reading and re-run this menu.");
        }
        else if (Object.FindAnyObjectByType(flowType) == null)
        {
            var go = new GameObject("IslandFlow");
            go.AddComponent(flowType);
        }

        EditorSceneManager.MarkAllScenesDirty();
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("[Polish] Vista polished: fog, warm sun + soft shadows, " +
                  "trilight ambient, reframed camera, sea sparkle, flow added. " +
                  "Press Play for the full arrival.");
    }

    static System.Type FindType(string name) =>
        System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => { try { return a.GetTypes(); }
                               catch { return new System.Type[0]; } })
            .FirstOrDefault(t => t.Name == name);
}
