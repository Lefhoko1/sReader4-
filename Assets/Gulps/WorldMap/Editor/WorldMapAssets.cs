using System.IO;
using UnityEditor;
using UnityEngine;

// Places Blender hero assets into the world-map scene over the greybox.
//  Tools > World Map > Place Island
public static class WorldMapAssets
{
    const string IslandFbx = "Assets/Gulps/Island_Land.fbx";
    const string CastleFbx = "Assets/Gulps/Castle.fbx";
    const string OutDir    = "Assets/Gulps/WorldMap";

    // Castle resting pose on the island's top-right cliff (Unity coords).
    static readonly Vector3 CastlePos   = new Vector3(11f, 11f, 16.5f);
    static readonly Vector3 CastleScale = Vector3.one;

    // If the castle plateau ends up facing the CAMERA (front) instead of the back,
    // flip this to true and re-run — Blender->Unity axis handedness can mirror depth.
    const bool RotateY180 = false;

    [MenuItem("Tools/World Map/Rebuild Everything", false, -100)]
    static void RebuildEverything()
    {
        string[] steps =
        {
            "Tools/World Map/Place Island",
            "Tools/World Map/Apply Land Texture",
            "Tools/World Map/Place Path & Nodes",
            "Tools/World Map/Place Castle",
            "Tools/World Map/Place Crystal",
            "Tools/World Map/Place Boat",
            "Tools/World Map/Place Foliage",
            "Tools/World Map/Scatter Trees",
            "Tools/World Map/Setup Water & Lighting",
            "Tools/World Map/Frame Camera",
            "Tools/World Map/Build UI Overlay",
            "Tools/World Map/Build Node Badges",
        };
        foreach (var s in steps)
        {
            bool ok = EditorApplication.ExecuteMenuItem(s);
            if (!ok) Debug.LogWarning("[WorldMap] Rebuild: step failed/not found: " + s);
        }
        var hero = GameObject.Find("Hero_Placeholder");   // hide leftover greybox capsule
        if (hero != null) hero.SetActive(false);
        Debug.Log("[WorldMap] Rebuild Everything complete.");
    }

    [MenuItem("Tools/World Map/Place Island")]
    static void PlaceIsland()
    {
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(IslandFbx);
        if (model == null)
        {
            Debug.LogError("[WorldMap] Island FBX not found at " + IslandFbx +
                           " — let Unity finish importing it, then run again.");
            return;
        }

        var grass = MakeMat("Island_Grass", new Color(0.24f, 0.44f, 0.14f));
        var rock  = MakeMat("Island_Rock",  new Color(0.40f, 0.34f, 0.28f));

        var prev = GameObject.Find("Island_Land");
        if (prev != null) Object.DestroyImmediate(prev);

        var inst = (GameObject)PrefabUtility.InstantiatePrefab(model);
        inst.name = "Island_Land";
        inst.transform.position = Vector3.zero;
        inst.transform.rotation = RotateY180 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

        var root = GameObject.Find("WorldMap_Greybox");
        if (root != null)
        {
            inst.transform.SetParent(root.transform, true);
            var tiers = root.transform.Find("Island");        // hide greybox tier cylinders
            if (tiers != null) tiers.gameObject.SetActive(false);
        }

        // Match by the imported material NAME (submesh order isn't guaranteed).
        foreach (var r in inst.GetComponentsInChildren<MeshRenderer>(true))
        {
            var mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                string n = mats[i] != null ? mats[i].name.ToLowerInvariant() : "";
                if (n.Contains("rock")) mats[i] = rock;
                else if (n.Contains("grass")) mats[i] = grass;
                else mats[i] = (i == 1) ? rock : grass;   // fallback to slot order
            }
            r.sharedMaterials = mats;
        }

        Selection.activeGameObject = inst;
        Debug.Log("[WorldMap] Island placed (greybox tiers hidden). " +
                  "If the raised castle plateau faces the camera, set RotateY180=true and re-run.");
    }

    // Applies Assets/Sprites/landTexture.png to the island's grass surface via a
    // top-down planar projection (Custom/LandPlanar). Cliffs keep their rock material
    // because planar XZ projection smears on vertical faces. Run AFTER Place Island.
    const string LandTexPng = "Assets/Sprites/landTexture.png";

    [MenuItem("Tools/World Map/Apply Land Texture")]
    static void ApplyLandTexture()
    {
        // 1. Make sure the PNG is imported as a repeating 3D texture (not a UI sprite).
        var imp = AssetImporter.GetAtPath(LandTexPng) as TextureImporter;
        if (imp == null)
        {
            Debug.LogError("[WorldMap] Land texture not found at " + LandTexPng +
                           " — let Unity finish importing it, then run again.");
            return;
        }
        if (imp.textureType != TextureImporterType.Default ||
            imp.wrapMode    != TextureWrapMode.Repeat ||
            !imp.mipmapEnabled)
        {
            imp.textureType   = TextureImporterType.Default;
            imp.wrapMode      = TextureWrapMode.Repeat;
            imp.mipmapEnabled = true;
            imp.SaveAndReimport();
        }
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(LandTexPng);

        // 2. Find the placed island in the scene.
        var island = GameObject.Find("Island_Land");
        if (island == null)
        {
            Debug.LogError("[WorldMap] No 'Island_Land' in scene — run Place Island first.");
            return;
        }

        // 3. Build/refresh the planar-land material, auto-fitting one tile to the island.
        var shader = Shader.Find("Custom/LandPlanar");
        if (shader == null)
        {
            Debug.LogError("[WorldMap] Shader 'Custom/LandPlanar' not found — let Unity compile " +
                           "Assets/Gulps/WorldMap/LandPlanar.shader, then run again.");
            return;
        }
        if (!Directory.Exists(OutDir)) Directory.CreateDirectory(OutDir);
        string matPath = OutDir + "/Island_Land_Tex.mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (mat == null) { mat = new Material(shader); AssetDatabase.CreateAsset(mat, matPath); }
        mat.shader = shader;
        mat.SetTexture("_BaseMap", tex);

        // Fit roughly one texture tile across the island's grass footprint so the art
        // reads as gentle rolling gradient rather than obvious repeats.
        var b = GrassBounds(island);
        float span = Mathf.Max(b.size.x, b.size.z, 1f);
        mat.SetFloat("_WorldScale", 1f / span);
        EditorUtility.SetDirty(mat);

        // 4. Assign to grass submeshes only (name contains "grass"); leave rock cliffs.
        int slots = 0;
        foreach (var r in island.GetComponentsInChildren<MeshRenderer>(true))
        {
            var mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                string n = mats[i] != null ? mats[i].name.ToLowerInvariant() : "";
                if (n.Contains("rock")) continue;                 // keep cliffs
                mats[i] = mat; slots++;
            }
            r.sharedMaterials = mats;
        }

        AssetDatabase.SaveAssets();
        Selection.activeGameObject = island;
        Debug.Log($"[WorldMap] Land texture applied to {slots} grass slot(s) " +
                  $"(tile span {span:0.0}u). Tweak _WorldScale / _GreenMinV / _GreenMaxV on " +
                  "Island_Land_Tex.mat to taste.");
    }

    // World-space bounds of the island's grass (non-rock) renderers, for tile fitting.
    static Bounds GrassBounds(GameObject island)
    {
        bool any = false; var b = new Bounds();
        foreach (var r in island.GetComponentsInChildren<MeshRenderer>(true))
        {
            string n0 = r.sharedMaterial != null ? r.sharedMaterial.name.ToLowerInvariant() : "";
            if (n0.Contains("rock")) continue;
            if (!any) { b = r.bounds; any = true; } else b.Encapsulate(r.bounds);
        }
        if (!any)
            foreach (var r in island.GetComponentsInChildren<MeshRenderer>(true))
            { if (!any) { b = r.bounds; any = true; } else b.Encapsulate(r.bounds); }
        return b;
    }

    [MenuItem("Tools/World Map/Place Castle")]
    static void PlaceCastle()
    {
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(CastleFbx);
        if (model == null)
        {
            Debug.LogError("[WorldMap] Castle FBX not found at " + CastleFbx +
                           " — let Unity finish importing it, then run again.");
            return;
        }

        // material name -> our URP material
        var lookup = new System.Collections.Generic.Dictionary<string, Material>
        {
            { "stone", MakeMat("Castle_Stone", new Color(0.82f, 0.80f, 0.74f)) },
            { "roof",  MakeMat("Castle_Roof",  new Color(0.20f, 0.32f, 0.62f)) },
            { "flag",  MakeMat("Castle_Flag",  new Color(0.80f, 0.18f, 0.18f)) },
            { "wood",  MakeMat("Castle_Wood",  new Color(0.25f, 0.16f, 0.10f)) },
            { "gold",  MakeMat("Castle_Gold",  new Color(0.85f, 0.70f, 0.30f)) },
        };

        var prev = GameObject.Find("Castle_Hero");
        if (prev != null) Object.DestroyImmediate(prev);

        var inst = (GameObject)PrefabUtility.InstantiatePrefab(model);
        inst.name = "Castle_Hero";
        float seatY = SampleSurfaceY("Island_Land", CastlePos.x, CastlePos.z, 2.2f, CastlePos.y) - 0.15f;
        inst.transform.position = new Vector3(CastlePos.x, seatY, CastlePos.z);
        inst.transform.localScale = CastleScale;

        var root = GameObject.Find("WorldMap_Greybox");
        if (root != null)
        {
            inst.transform.SetParent(root.transform, true);
            var gb = root.transform.Find("Castle");           // hide greybox castle
            if (gb != null) gb.gameObject.SetActive(false);
        }

        foreach (var r in inst.GetComponentsInChildren<MeshRenderer>(true))
        {
            var mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                string n = mats[i] != null ? mats[i].name.ToLowerInvariant() : "";
                foreach (var kv in lookup) if (n.Contains(kv.Key)) { mats[i] = kv.Value; break; }
            }
            r.sharedMaterials = mats;
        }

        Selection.activeGameObject = inst;
        Debug.Log("[WorldMap] Castle placed on the plateau (greybox castle hidden). " +
                  "Nudge CastlePos if it floats or sinks.");
    }

    [MenuItem("Tools/World Map/Place Crystal")]
    static void PlaceCrystal()
    {
        var model = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Gulps/Crystal.fbx");
        if (model == null) { Debug.LogError("[WorldMap] Crystal.fbx not found."); return; }

        var gem = MakeEmissiveMat("Crystal_Gem", new Color(0.25f, 0.65f, 0.95f), 2.2f);
        var rock = MakeMat("Crystal_Rock", new Color(0.35f, 0.33f, 0.30f));

        var prev = GameObject.Find("Crystal_Hero");
        if (prev != null) Object.DestroyImmediate(prev);
        var inst = (GameObject)PrefabUtility.InstantiatePrefab(model);
        inst.name = "Crystal_Hero";
        float x = 12f, z = -3f;
        float y = SampleSurfaceY("Island_Land", x, z, 1.2f, 2.0f) - 0.1f;
        inst.transform.position = new Vector3(x, y, z);

        var root = GameObject.Find("WorldMap_Greybox");
        if (root != null)
        {
            inst.transform.SetParent(root.transform, true);
            HideGreybox(root, "Crystal");
        }
        AssignByName(inst, ("gem", gem), ("rock", rock));
        Selection.activeGameObject = inst;
        Debug.Log("[WorldMap] Crystal placed on the shore (emissive).");
    }

    [MenuItem("Tools/World Map/Place Boat")]
    static void PlaceBoat()
    {
        var model = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Gulps/Boat.fbx");
        if (model == null) { Debug.LogError("[WorldMap] Boat.fbx not found."); return; }

        var wood = MakeMat("Boat_Wood", new Color(0.45f, 0.30f, 0.16f));
        var sail = MakeMat("Boat_Sail", new Color(0.90f, 0.88f, 0.80f));
        var dark = MakeMat("Boat_Dark", new Color(0.28f, 0.18f, 0.10f));
        var flag = MakeMat("Boat_Flag", new Color(0.80f, 0.20f, 0.20f));

        var prev = GameObject.Find("Boat_Hero");
        if (prev != null) Object.DestroyImmediate(prev);
        var inst = (GameObject)PrefabUtility.InstantiatePrefab(model);
        inst.name = "Boat_Hero";
        // Find actual water: scan the island surface for a below-sea-level spot,
        // preferring one toward the left-front of the frame.
        float bx = 0f, bz = 6f, bestScore = float.PositiveInfinity;
        bool found = false;
        for (float x = -15f; x <= 15f; x += 1.5f)
            for (float z = -13f; z <= 21f; z += 1.5f)
            {
                float sy = SampleSurfaceY("Island_Land", x, z, 1.3f, 99f);
                if (sy < -1.0f)   // underwater
                {
                    float score = x + 0.4f * Mathf.Abs(z - 6f);   // prefer left & mid-frame
                    if (score < bestScore) { bestScore = score; bx = x; bz = z; found = true; }
                }
            }
        inst.transform.position = new Vector3(bx, -0.15f, bz);
        inst.transform.rotation = Quaternion.Euler(0, 40f, 0);
        if (!found) Debug.LogWarning("[WorldMap] Boat: no water spot found; placed at origin.");

        var root = GameObject.Find("WorldMap_Greybox");
        if (root != null)
        {
            inst.transform.SetParent(root.transform, true);
            HideGreybox(root, "Boat_Hull", "Boat_Mast", "Boat_Sail");
        }
        AssignByName(inst, ("wood", wood), ("sail", sail), ("dark", dark), ("flag", flag));
        Selection.activeGameObject = inst;
        Debug.Log("[WorldMap] Boat placed in the sea.");
    }

    [MenuItem("Tools/World Map/Place Foliage")]
    static void PlaceFoliage()
    {
        var model = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Gulps/Foliage.fbx");
        if (model == null) { Debug.LogError("[WorldMap] Foliage.fbx not found."); return; }

        var rock = MakeMat("Foliage_Rock", new Color(0.42f, 0.40f, 0.36f));
        var bush = MakeMat("Foliage_Bush", new Color(0.20f, 0.42f, 0.15f));

        var prev = GameObject.Find("Foliage_Hero");
        if (prev != null) Object.DestroyImmediate(prev);
        var inst = (GameObject)PrefabUtility.InstantiatePrefab(model);
        inst.name = "Foliage_Hero";
        inst.transform.position = Vector3.zero;

        var root = GameObject.Find("WorldMap_Greybox");
        if (root != null) inst.transform.SetParent(root.transform, true);

        AssignByName(inst, ("rock", rock), ("bush", bush));
        Selection.activeGameObject = inst;
        Debug.Log("[WorldMap] Foliage scattered on the island.");
    }

    [MenuItem("Tools/World Map/Place Path & Nodes")]
    static void PlacePath()
    {
        var model = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Gulps/Pathway.fbx");
        if (model == null) { Debug.LogError("[WorldMap] Pathway.fbx not found."); return; }

        var path   = MakeMat("Path_Stone",  new Color(0.72f, 0.62f, 0.45f));
        var nbase  = MakeMat("Node_Base",   new Color(0.55f, 0.52f, 0.48f));
        var gold   = MakeMat("Node_Gold",   new Color(0.88f, 0.72f, 0.34f));
        var locked = MakeMat("Node_Locked", new Color(0.40f, 0.40f, 0.46f));

        var prev = GameObject.Find("Pathway_Hero");
        if (prev != null) Object.DestroyImmediate(prev);
        var inst = (GameObject)PrefabUtility.InstantiatePrefab(model);
        inst.name = "Pathway_Hero";
        inst.transform.position = Vector3.zero;

        var root = GameObject.Find("WorldMap_Greybox");
        if (root != null)
        {
            inst.transform.SetParent(root.transform, true);
            var p = root.transform.Find("Path");  if (p != null) p.gameObject.SetActive(false);
            var n = root.transform.Find("Nodes"); if (n != null) n.gameObject.SetActive(false);
        }
        // submesh order: Path_Stone, Node_Base, Node_Gold, Node_Locked
        AssignByName(inst, ("path_stone", path), ("node_base", nbase),
                           ("node_gold", gold), ("node_locked", locked));
        Selection.activeGameObject = inst;
        Debug.Log("[WorldMap] Path & nodes placed; greybox path/nodes hidden.");
    }

    [MenuItem("Tools/World Map/Scatter Trees")]
    static void ScatterTrees()
    {
        string fbx = FindFbxWithMesh("Tree_Leaves");
        if (fbx == null) { Debug.LogError("[WorldMap] No tree FBX (mesh 'Tree_Leaves') found."); return; }
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(fbx);

        var shader = Shader.Find("Custom/TreeWind");
        bool wind = shader != null;
        if (!wind) shader = Shader.Find("Universal Render Pipeline/Lit");
        var bark = MakeTreeMat("TreeScatter_Bark", new Color(0.27f, 0.17f, 0.09f), shader, false);
        var leaf = MakeTreeMat("TreeScatter_Leaf", new Color(0.18f, 0.35f, 0.12f), shader, true);

        var prev = GameObject.Find("Trees_Hero");
        if (prev != null) Object.DestroyImmediate(prev);
        var grp = new GameObject("Trees_Hero").transform;
        var root = GameObject.Find("WorldMap_Greybox");
        if (root != null) grp.SetParent(root.transform, false);

        var rng = new System.Random(99);
        int placed = 0, tries = 0;
        while (placed < 18 && tries < 900)
        {
            tries++;
            float x = (float)(rng.NextDouble() * 33 - 16);   // [-16, 17]
            float z = (float)(rng.NextDouble() * 37 - 14);   // [-14, 23]
            if (Mathf.Sqrt((x - 9f) * (x - 9f) + (z - 16.5f) * (z - 16.5f)) < 5f) continue; // clear of castle
            float y = SampleSurfaceY("Island_Land", x, z, 1.0f, -99f);
            if (y < 1.8f) continue;                                   // land only (above shore)

            var t = (GameObject)PrefabUtility.InstantiatePrefab(model);
            t.name = "Tree_" + placed;
            t.transform.SetParent(grp, true);
            t.transform.position = new Vector3(x, y - 0.2f, z);
            float s = 0.13f + (float)rng.NextDouble() * 0.06f;
            t.transform.localScale = new Vector3(s, s, s);
            t.transform.rotation = Quaternion.Euler(0, (float)rng.NextDouble() * 360f, 0);
            foreach (var r in t.GetComponentsInChildren<MeshRenderer>(true))
            {
                var m = r.gameObject.name.ToLowerInvariant().Contains("leaf") ? leaf : bark;
                var mats = r.sharedMaterials;
                for (int i = 0; i < mats.Length; i++) mats[i] = m;
                r.sharedMaterials = mats;
            }
            placed++;
        }
        Debug.Log($"[WorldMap] Scattered {placed} trees" + (wind ? " (wind shader)." : " (no TreeWind shader found — static)."));
    }

    static Material MakeTreeMat(string name, Color c, Shader sh, bool cullOff)
    {
        if (!Directory.Exists(OutDir)) Directory.CreateDirectory(OutDir);
        string p = OutDir + "/" + name + ".mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(p);
        if (m == null) { m = new Material(sh); AssetDatabase.CreateAsset(m, p); }
        m.shader = sh;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (m.HasProperty("_Cull")) m.SetFloat("_Cull", cullOff ? 0f : 2f);
        EditorUtility.SetDirty(m);
        return m;
    }

    static string FindFbxWithMesh(string meshName)
    {
        foreach (var g in AssetDatabase.FindAssets("t:Mesh"))
        {
            var p = AssetDatabase.GUIDToAssetPath(g);
            if (!p.ToLowerInvariant().EndsWith(".fbx")) continue;
            foreach (var o in AssetDatabase.LoadAllAssetsAtPath(p))
                if (o is Mesh mm && mm.name == meshName) return p;
        }
        return null;
    }

    static void HideGreybox(GameObject root, params string[] names)
    {
        foreach (var n in names)
        {
            var t = root.transform.Find("Props/" + n) ?? root.transform.Find(n);
            if (t == null)
                foreach (var x in root.GetComponentsInChildren<Transform>(true))
                    if (x.name == n) { t = x; break; }
            if (t != null) t.gameObject.SetActive(false);
        }
    }

    static void AssignByName(GameObject inst, params (string key, Material mat)[] map)
    {
        foreach (var r in inst.GetComponentsInChildren<MeshRenderer>(true))
        {
            var mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                string n = mats[i] != null ? mats[i].name.ToLowerInvariant() : "";
                Material chosen = map.Length > 0 ? map[0].mat : mats[i];   // fallback so nothing stays magenta
                foreach (var kv in map) if (n.Contains(kv.key)) { chosen = kv.mat; break; }
                mats[i] = chosen;
            }
            r.sharedMaterials = mats;
        }
    }

    static Material MakeEmissiveMat(string name, Color c, float intensity)
    {
        var m = MakeMat(name, c);
        m.EnableKeyword("_EMISSION");
        m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        m.SetColor("_EmissionColor", c * intensity);
        EditorUtility.SetDirty(m);
        return m;
    }

    // Highest island-surface Y within `radius` of (x,z) — so props sit flush on the terrain.
    static float SampleSurfaceY(string islandName, float x, float z, float radius, float fallback)
    {
        var go = GameObject.Find(islandName);
        var mf = go != null ? go.GetComponentInChildren<MeshFilter>() : null;
        if (mf == null || mf.sharedMesh == null) return fallback;
        var verts = mf.sharedMesh.vertices;
        var tf = mf.transform;
        float best = float.NegativeInfinity; float r2 = radius * radius;
        foreach (var v in verts)
        {
            var w = tf.TransformPoint(v);
            float dx = w.x - x, dz = w.z - z;
            if (dx * dx + dz * dz <= r2 && w.y > best) best = w.y;
        }
        return float.IsNegativeInfinity(best) ? fallback : best;
    }

    static Material MakeMat(string name, Color c)
    {
        if (!Directory.Exists(OutDir)) Directory.CreateDirectory(OutDir);
        string p = OutDir + "/" + name + ".mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(p);
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (m == null) { m = new Material(shader); AssetDatabase.CreateAsset(m, p); }
        m.shader = shader;
        m.SetColor("_BaseColor", c);
        m.SetFloat("_Smoothness", 0.1f);
        EditorUtility.SetDirty(m);
        return m;
    }
}
