using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Sets up stylized water + warm lighting for the world map.
//  Tools > World Map > Setup Water & Lighting
public static class WorldMapAtmosphere
{
    const string OutDir = "Assets/Gulps/WorldMap";

    [MenuItem("Tools/World Map/Water DEBUG (red URP Lit)")]
    static void WaterDebugRed()
    {
        var prev = GameObject.Find("WorldMap_Water");
        if (prev != null) Object.DestroyImmediate(prev);
        var wp = GameObject.CreatePrimitive(PrimitiveType.Plane);
        wp.name = "WorldMap_Water";
        Object.DestroyImmediate(wp.GetComponent<Collider>());
        wp.transform.position = new Vector3(0f, 0.5f, 0f);   // clearly above sea level
        wp.transform.localScale = new Vector3(40f, 1f, 40f);
        var m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        m.SetColor("_BaseColor", Color.red);
        wp.GetComponent<MeshRenderer>().sharedMaterial = m;
        Debug.Log("[WorldMap] DEBUG: red URP/Lit plane at y=0.5. If you DON'T see red, the plane/camera is the problem, not the shader.");
    }

    // Dumps every active MeshRenderer: name, world center, size, and material(s).
    // Use to identify a stray/misplaced object (e.g. the brown box near the castle).
    [MenuItem("Tools/World Map/List Renderers")]
    static void ListRenderers()
    {
        var sb = new System.Text.StringBuilder("==== ACTIVE RENDERERS (name | center | size | materials) ====\n");
        var all = Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        System.Array.Sort(all, (a, b) => b.bounds.center.y.CompareTo(a.bounds.center.y)); // high to low
        foreach (var r in all)
        {
            if (!r.enabled || !r.gameObject.activeInHierarchy) continue;
            var b = r.bounds;
            string mats = "";
            foreach (var m in r.sharedMaterials) mats += (m != null ? m.name : "null") + " ";
            string path = r.name;
            var t = r.transform.parent;
            while (t != null) { path = t.name + "/" + path; t = t.parent; }
            sb.AppendLine($"  {path}  c=({b.center.x:0.0},{b.center.y:0.0},{b.center.z:0.0})  " +
                          $"s=({b.size.x:0.0},{b.size.y:0.0},{b.size.z:0.0})  [{mats.Trim()}]");
        }
        Debug.Log(sb.ToString());
    }

    [MenuItem("Tools/World Map/Find Magenta (bad materials)")]
    static void FindMagenta()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("==== BAD-MATERIAL SCAN ====");
        int hits = 0;
        foreach (var r in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
        {
            var mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                var m = mats[i];
                bool bad = m == null || m.shader == null ||
                           m.shader.name == "Hidden/InternalErrorShader" ||
                           m.shader.name.Contains("InternalError");
                if (bad)
                {
                    hits++;
                    sb.AppendLine($"  '{GetPath(r.transform)}' slot {i}: mat='{(m ? m.name : "NULL")}' shader='{(m && m.shader ? m.shader.name : "NULL")}' pos={r.transform.position}");
                }
            }
        }
        if (hits == 0) sb.AppendLine("  none found (no InternalErrorShader). The magenta may be a specific object — tell me its rough screen position.");
        Debug.Log(sb.ToString());
    }

    static string GetPath(Transform t)
    {
        string p = t.name;
        while (t.parent != null) { t = t.parent; p = t.name + "/" + p; }
        return p;
    }

    [MenuItem("Tools/World Map/Diagnose Scene")]
    static void Diagnose()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("==== WORLD MAP DIAGNOSE ====");
        sb.AppendLine("-- Cameras (what renders the Game view) --");
        foreach (var c in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
            sb.AppendLine($"  '{c.name}' enabled={c.enabled} depth={c.depth} tag={c.tag} clear={c.clearFlags} cullingMask={c.cullingMask} pos={c.transform.position}");

        var w = GameObject.Find("WorldMap_Water");
        if (w == null) sb.AppendLine("-- WorldMap_Water: NOT FOUND --");
        else
        {
            var r = w.GetComponent<MeshRenderer>();
            string shader = (r != null && r.sharedMaterial != null) ? r.sharedMaterial.shader.name : "NONE";
            sb.AppendLine($"-- WorldMap_Water: active={w.activeInHierarchy} pos={w.transform.position} lossyScale={w.transform.lossyScale} layer={w.layer} shader='{shader}' --");
            if (r != null) sb.AppendLine($"   renderer.enabled={r.enabled} bounds={r.bounds}");
            var parent = w.transform.parent;
            sb.AppendLine($"   parent='{(parent ? parent.name : "<none>")}' parentActive={(parent ? parent.gameObject.activeInHierarchy.ToString() : "n/a")}");
        }

        var cam = GameObject.Find("WorldMapCamera");
        sb.AppendLine(cam ? $"-- WorldMapCamera: pos={cam.transform.position} enabled={cam.GetComponent<Camera>().enabled} --" : "-- WorldMapCamera: NOT FOUND --");
        Debug.Log(sb.ToString());
    }

    [MenuItem("Tools/World Map/Setup Water & Lighting")]
    static void Setup()
    {
        // ---- Water material ----
        var shader = Shader.Find("Custom/StylizedWater");
        if (shader == null)
        {
            Debug.LogError("[WorldMap] 'Custom/StylizedWater' shader not found/compiled yet — let Unity compile, then run again.");
        }
        else
        {
            if (!Directory.Exists(OutDir)) Directory.CreateDirectory(OutDir);
            string mp = OutDir + "/Water.mat";
            AssetDatabase.DeleteAsset(mp);                 // drop any stuck/old material
            var water = new Material(shader);
            AssetDatabase.CreateAsset(water, mp);
            water.SetColor("_ShallowColor", new Color(0.40f, 0.74f, 0.86f, 0.70f));
            water.SetColor("_DeepColor",    new Color(0.18f, 0.52f, 0.74f, 0.92f));
            water.SetColor("_FoamColor",    Color.white);
            water.SetFloat("_WaveScale", 0.7f);
            water.SetFloat("_CapStrength", 0.4f);
            water.SetFloat("_FadeRadius", 60f);
            EditorUtility.SetDirty(water);

            // create a dedicated water plane at sea level (don't rely on the greybox plane)
            var prevW = GameObject.Find("WorldMap_Water");
            if (prevW != null) Object.DestroyImmediate(prevW);
            var wp = GameObject.CreatePrimitive(PrimitiveType.Plane);   // 10x10 units, normal up
            wp.name = "WorldMap_Water";
            Object.DestroyImmediate(wp.GetComponent<Collider>());
            wp.transform.position = new Vector3(0f, 0f, 0f);
            wp.transform.localScale = new Vector3(40f, 1f, 40f);        // 400x400 units
            wp.GetComponent<MeshRenderer>().sharedMaterial = water;

            var wroot = GameObject.Find("WorldMap_Greybox");
            if (wroot != null)
            {
                wp.transform.SetParent(wroot.transform, true);
                var oldSea = wroot.transform.Find("Sea");               // hide greybox sea to avoid overlap
                if (oldSea != null) oldSea.gameObject.SetActive(false);
            }
            Debug.Log("[WorldMap] Created 'WorldMap_Water' plane at y=0 with the water material.");
        }

        // ---- Depth texture for the foam (per-camera AND on the URP asset) ----
        var camGo = GameObject.Find("WorldMapCamera");
        if (camGo != null)
        {
            var cam = camGo.GetComponent<Camera>();
            var data = camGo.GetComponent<UniversalAdditionalCameraData>();
            if (data == null) data = camGo.AddComponent<UniversalAdditionalCameraData>();
            data.requiresDepthOption = CameraOverrideOption.On;
            if (cam != null) cam.depthTextureMode |= DepthTextureMode.Depth;
        }
        var rp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        if (rp != null)
        {
            var so = new SerializedObject(rp);
            var pr = so.FindProperty("m_SupportsCameraDepthTexture") ?? so.FindProperty("m_RequireDepthTexture");
            if (pr != null) { pr.boolValue = true; so.ApplyModifiedProperties(); EditorUtility.SetDirty(rp); }
            Debug.Log("[WorldMap] URP asset depth texture: " + (pr != null ? "enabled (" + pr.name + ")" : "property not found — tick 'Depth Texture' on the URP asset manually"));
        }

        // ---- Warm sun ----
        Light sun = null;
        foreach (var l in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
            if (l.type == LightType.Directional) { sun = l; break; }
        if (sun == null)
        {
            var go = new GameObject("WorldMap_Sun");
            sun = go.AddComponent<Light>();
            sun.type = LightType.Directional;
        }
        sun.color = new Color(1.0f, 0.97f, 0.88f);
        sun.intensity = 1.45f;
        sun.transform.rotation = Quaternion.Euler(42f, -20f, 0f);  // over the camera's shoulder, lights front faces
        sun.shadows = LightShadows.Soft;

        // ---- Bright sky ambient ----
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor     = new Color(0.70f, 0.82f, 0.98f);  // bright sky
        RenderSettings.ambientEquatorColor = new Color(0.62f, 0.66f, 0.64f);
        RenderSettings.ambientGroundColor  = new Color(0.42f, 0.44f, 0.40f);
        RenderSettings.ambientIntensity = 1.0f;

        Debug.Log("[WorldMap] Water + lighting set up. (Foam needs URP 'Depth Texture' — enabled on the camera.)");
    }
}
