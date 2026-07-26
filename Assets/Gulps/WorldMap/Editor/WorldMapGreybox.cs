using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;

// Generates a ProBuilder GREYBOX of the World Map (Panel 1 of ultimateDesign.png):
// a stylized island rising out of the sea, a winding path of level nodes climbing
// to a castle, plus boat / crystal / hero placeholders and a fixed diorama camera.
//
// This is composition blockout only — every piece is a real, editable ProBuilder
// mesh meant to be hand-tweaked, then progressively replaced by Blender hero assets.
//
// Run from the menu:  Tools > World Map > Build Greybox
public static class WorldMapGreybox
{
    const string RootName = "WorldMap_Greybox";

    // Tier TOP heights (so nodes/props sit flush). The island is a hill that CLIMBS
    // toward +Z (the back), so each higher tier is also pushed further back.
    const float ShoreTop   = 1.0f;
    const float MidTop     = 3.5f;
    const float UpperTop   = 6.5f;
    const float PlateauTop = 9.0f;   // castle sits here

    static readonly Dictionary<string, Material> _mats = new();

    [MenuItem("Tools/World Map/Build Greybox")]
    static void Build()
    {
        // Idempotent: wipe any previous greybox.
        var existing = GameObject.Find(RootName);
        if (existing != null) Object.DestroyImmediate(existing);
        _mats.Clear();

        var root   = new GameObject(RootName).transform;
        var sea    = new GameObject("Sea").transform;     sea.SetParent(root, false);
        var island = new GameObject("Island").transform;  island.SetParent(root, false);
        var path   = new GameObject("Path").transform;    path.SetParent(root, false);
        var nodes  = new GameObject("Nodes").transform;   nodes.SetParent(root, false);
        var castle = new GameObject("Castle").transform;  castle.SetParent(root, false);
        var props  = new GameObject("Props").transform;   props.SetParent(root, false);

        // ---------------- Sea ----------------
        var seaGo = Plane("Sea_Surface", sea, new Vector3(0, 0, 2), 90f, Col("sea", 0.16f, 0.45f, 0.70f));

        // ---------------- Island tiers: a hill that rises toward the back (+Z) ----------------
        var grass = Col("grass", 0.35f, 0.55f, 0.25f);
        var rock  = Col("rock",  0.45f, 0.42f, 0.38f);
        // center = (x, height/2 stacked on previous top, z pushed back as it rises)
        Cyl("Island_Shore",   island, new Vector3(0, 0.5f,  -2f), 11f, 1.0f, grass, 24);                 // top 1.0
        Cyl("Island_MidSlope",island, new Vector3(0, 2.25f,  2f),  8f, 2.5f, grass, 22);                 // top 3.5
        Cyl("Island_Upper",   island, new Vector3(0, 5.0f,   5f),  5.5f,3.0f, grass, 20);                // top 6.5
        Cyl("Island_Plateau", island, new Vector3(0, 7.75f,  7.5f),3.5f,2.5f, rock,  18);                // top 9.0

        // ---------------- Level nodes switchbacking UP the hill (front/low -> back/high) ----------------
        var nodeMat = Col("node", 0.85f, 0.70f, 0.35f);
        var n1 = new Vector3(-4f, ShoreTop   + 0.2f, -7f);   Cyl("Node_1_MeadowPoint", nodes, n1, 1.3f, 0.4f, nodeMat, 16);
        var n2 = new Vector3( 4f, ShoreTop   + 0.2f, -3f);   Cyl("Node_2_CrystalCove", nodes, n2, 1.3f, 0.4f, nodeMat, 16);
        var n3 = new Vector3(-3f, MidTop     + 0.2f,  1f);   Cyl("Node_3_SunnyCliffs", nodes, n3, 1.3f, 0.4f, nodeMat, 16);
        var n4 = new Vector3( 3f, UpperTop   + 0.2f,  4f);   Cyl("Node_4_BraveHarbor", nodes, n4, 1.3f, 0.4f, nodeMat, 16);
        var n5 = new Vector3(-1.5f, PlateauTop + 0.2f, 6f);  Cyl("Node_5_LockedGate",  nodes, n5, 1.3f, 0.4f, Col("locked", 0.30f, 0.30f, 0.34f), 16);
        var nc = new Vector3(0f,  PlateauTop + 0.2f, 7.5f);  // castle node

        // ---------------- Winding path between nodes ----------------
        var pathMat = Col("path", 0.70f, 0.60f, 0.45f);
        Path("Path_1_2", path, n1, n2, pathMat);
        Path("Path_2_3", path, n2, n3, pathMat);
        Path("Path_3_4", path, n3, n4, pathMat);
        Path("Path_4_5", path, n4, n5, pathMat);
        Path("Path_5_C", path, n5, nc, pathMat);

        // ---------------- Castle blockout (centerpiece) ----------------
        var stone = Col("castle", 0.80f, 0.80f, 0.85f);
        var roof  = Col("roof",   0.60f, 0.20f, 0.20f);
        var dark  = Col("door",   0.18f, 0.14f, 0.10f);
        var cz = 7.5f;
        Cube("Castle_Keep", castle, new Vector3(0, PlateauTop + 2.5f, cz), new Vector3(3.2f, 5f, 3.2f), stone);
        Cube("Castle_Gate", castle, new Vector3(0, PlateauTop + 1.5f, cz - 1.7f), new Vector3(1.2f, 3f, 0.4f), dark);
        var towerOff = new Vector2[] { new(-2.0f, -2.0f), new(2.0f, -2.0f), new(-2.0f, 2.0f), new(2.0f, 2.0f) };
        for (int i = 0; i < towerOff.Length; i++)
        {
            var tp = new Vector3(towerOff[i].x, PlateauTop + 2.75f, cz + towerOff[i].y);
            Cyl($"Castle_Tower_{i}", castle, tp, 0.9f, 5.5f, stone, 12);
            Prism($"Castle_Roof_{i}", castle, tp + new Vector3(0, 3.55f, 0), new Vector3(2.0f, 1.6f, 2.0f), roof);
        }

        // ---------------- Props: crystal, boat, hero placeholder ----------------
        Cube("Crystal", props, new Vector3(5f, ShoreTop + 1.1f, -5f), new Vector3(1.1f, 2.2f, 1.1f), Col("crystal", 0.30f, 0.70f, 0.95f), new Vector3(0, 45f, 0));

        var boatMat = Col("boat", 0.50f, 0.35f, 0.20f);
        Cube("Boat_Hull", props, new Vector3(-11f, 0.6f, -9f), new Vector3(3f, 0.9f, 1.2f), boatMat, new Vector3(0, 15f, 0));
        Cube("Boat_Mast", props, new Vector3(-11f, 2.2f, -9f), new Vector3(0.15f, 3f, 0.15f), boatMat);
        Cube("Boat_Sail", props, new Vector3(-11.4f, 2.3f, -9f), new Vector3(0.1f, 1.6f, 1.3f), Col("sail", 0.90f, 0.90f, 0.85f));

        var hero = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        hero.name = "Hero_Placeholder";
        hero.transform.SetParent(props, true);
        hero.transform.position = new Vector3(-4f, ShoreTop + 1.3f, -7f);
        hero.transform.localScale = Vector3.one * 0.9f;
        hero.GetComponent<Renderer>().sharedMaterial = Col("hero", 0.20f, 0.40f, 0.70f);
        Object.DestroyImmediate(hero.GetComponent<Collider>());

        // ---------------- Fixed diorama camera (~40 deg, portrait framing) ----------------
        var camGo = new GameObject("WorldMapCamera");
        camGo.transform.SetParent(root, false);
        var cam = camGo.AddComponent<Camera>();
        camGo.transform.position = new Vector3(0, 30f, -42f);
        camGo.transform.LookAt(new Vector3(0, 5f, 3f));
        cam.fieldOfView = 46f;
        cam.farClipPlane = 300f;
        // NOTE: set the Game view to a portrait aspect (e.g. 1080x1920) to match the design.

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = root.gameObject;
        if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.FrameSelected();
        Debug.Log("[WorldMap] Greybox built. Frame the 'WorldMapCamera' and set Game view to portrait (1080x1920).");
    }

    // ----------------------------------------------------------------- helpers
    static Material Col(string key, float r, float g, float b)
    {
        if (_mats.TryGetValue(key, out var m)) return m;
        m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "GB_" + key };
        m.SetColor("_BaseColor", new Color(r, g, b, 1f));
        m.SetFloat("_Smoothness", 0.1f);
        _mats[key] = m;
        return m;
    }

    static GameObject Cube(string name, Transform parent, Vector3 pos, Vector3 size, Material mat, Vector3 euler = default)
    {
        var pb = ShapeGenerator.GenerateCube(PivotLocation.Center, size);
        Setup(pb, name, parent, pos, mat);
        pb.transform.localEulerAngles = euler;
        return pb.gameObject;
    }

    static GameObject Cyl(string name, Transform parent, Vector3 pos, float radius, float height, Material mat, int sides, Vector3 scale = default)
    {
        var pb = ShapeGenerator.GenerateCylinder(PivotLocation.Center, sides, radius, height, 0, -1);
        Setup(pb, name, parent, pos, mat);
        if (scale != default) pb.transform.localScale = scale;
        return pb.gameObject;
    }

    static GameObject Prism(string name, Transform parent, Vector3 pos, Vector3 size, Material mat)
    {
        var pb = ShapeGenerator.GeneratePrism(PivotLocation.Center, size);
        Setup(pb, name, parent, pos, mat);
        return pb.gameObject;
    }

    static GameObject Plane(string name, Transform parent, Vector3 pos, float size, Material mat)
    {
        var pb = ShapeGenerator.GeneratePlane(PivotLocation.Center, size, size, 1, 1, Axis.Up);
        Setup(pb, name, parent, pos, mat);
        return pb.gameObject;
    }

    static void Path(string name, Transform parent, Vector3 a, Vector3 b, Material mat)
    {
        var mid = (a + b) * 0.5f - new Vector3(0, 0.05f, 0);
        float len = Vector3.Distance(a, b) + 0.4f;
        var go = Cube(name, parent, mid, new Vector3(1.4f, 0.22f, len), mat);
        go.transform.rotation = Quaternion.LookRotation((b - a).normalized, Vector3.up);
    }

    static void Setup(ProBuilderMesh pb, string name, Transform parent, Vector3 pos, Material mat)
    {
        pb.gameObject.name = name;
        pb.transform.SetParent(parent, true);
        pb.transform.localPosition = pos;
        pb.GetComponent<MeshRenderer>().sharedMaterial = mat;
        pb.ToMesh();
        pb.Refresh();
    }
}
