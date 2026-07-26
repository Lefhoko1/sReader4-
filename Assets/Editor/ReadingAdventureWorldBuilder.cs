using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.SceneManagement;

namespace SReader.EditorTools
{
    /// <summary>
    /// Builds a stylized children's-storybook PROTOTYPE BLOCKOUT for the
    /// "Reading Adventure" URP mobile game, directly inside the active scene,
    /// using Unity ProBuilder.
    ///
    /// Run it from:  Tools ▸ ProBuilder ▸ Build Reading Adventure World
    ///
    /// Everything is generated as separate, editable ProBuilderMesh objects
    /// (no single combined mesh) so any section can later be swapped for a
    /// finished asset. Re-running the command deletes and rebuilds the
    /// "Reading Adventure World" root, leaving the rest of the scene untouched.
    ///
    /// This is a layout/proportion blockout only — no fine detail, no trees.
    /// Flat areas are intentionally left open (and marked under Decorations)
    /// for forests/props to be added later.
    /// </summary>
    public static class ReadingAdventureWorldBuilder
    {
        // ── Palette (bright, exaggerated, sunny storybook) ──────────────────
        static readonly Color Grass      = new Color(0.46f, 0.74f, 0.34f);
        static readonly Color GrassHill  = new Color(0.40f, 0.67f, 0.30f);
        static readonly Color Water      = new Color(0.27f, 0.58f, 0.86f);
        static readonly Color CliffStone = new Color(0.60f, 0.49f, 0.38f);
        static readonly Color RiverBank  = new Color(0.55f, 0.45f, 0.32f);
        static readonly Color PathStone  = new Color(0.82f, 0.73f, 0.55f);
        static readonly Color CastleStone= new Color(0.89f, 0.83f, 0.70f);
        static readonly Color NodeStone  = new Color(0.93f, 0.81f, 0.53f);
        static readonly Color RoofRed    = new Color(0.87f, 0.35f, 0.30f);
        static readonly Color RoofBlue   = new Color(0.30f, 0.52f, 0.82f);

        const string RootName = "Reading Adventure World";
        const string ScenePath = "Assets/Scenes/ReadingAdventureWorld.unity";
        const float GrassTopY = 0f;        // grass surface
        const float IslandBottomY = -6f;   // underside of the landmass slab

        static readonly Dictionary<string, Material> _materials = new();
        static int _cliffIdx;

        [MenuItem("Tools/ProBuilder/Build Reading Adventure World")]
        public static void Build()
        {
            if (!EnsureDedicatedScene())
                return; // user cancelled the "save current scene?" prompt

            _materials.Clear();
            _cliffIdx = 0;

            var existing = GameObject.Find(RootName);
            if (existing != null)
                Undo.DestroyObjectImmediate(existing);

            var world = NewParent(RootName, null);
            Undo.RegisterCreatedObjectUndo(world, "Build Reading Adventure World");

            var terrain     = NewParent("Terrain",     world.transform);
            var cliffs      = NewParent("Cliffs",       world.transform);
            var river       = NewParent("River",        world.transform);
            var bridge      = NewParent("Bridge",       world.transform);
            var paths       = NewParent("Paths",        world.transform);
            var castle      = NewParent("Castle",       world.transform);
            var levelNodes  = NewParent("LevelNodes",   world.transform);
            var decorations = NewParent("Decorations",  world.transform);

            BuildTerrain(terrain.transform);
            BuildCliffs(cliffs.transform);
            BuildRiver(river.transform, RiverPoints());
            BuildPathsAndNodes(paths.transform, levelNodes.transform);
            BuildBridge(bridge.transform);
            BuildCastle(castle.transform);
            BuildDecorationMarkers(decorations.transform);
            BuildLighting(world.transform);
            BuildCamera(world.transform);

            AssetDatabase.SaveAssets();
            Selection.activeGameObject = world;
            SceneView.lastActiveSceneView?.FrameSelected();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

            Debug.Log($"[ReadingAdventure] Prototype world built and saved to '{ScenePath}'. " +
                      "Press Play (or open the Game view) to see the isometric layout through 'World Camera'.");
        }

        /// <summary>
        /// Makes the dedicated Reading Adventure scene the active scene, creating
        /// it the first time. Returns false if the user cancels the prompt to save
        /// the currently-open scene. Leaves the app's SampleScene untouched.
        /// </summary>
        static bool EnsureDedicatedScene()
        {
            if (SceneManager.GetActiveScene().path == ScenePath)
                return true; // already in it (e.g. re-running the build)

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return false;

            EnsureFolder("Assets/Scenes");

            if (File.Exists(ScenePath))
            {
                EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            }
            else
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(scene, ScenePath);
            }
            return true;
        }

        // ── World sections ──────────────────────────────────────────────────

        static void BuildTerrain(Transform p)
        {
            var grass = Mat("Grass", Grass);
            var hillMat = Mat("GrassHill", GrassHill);

            // 120 x 120 island slab, top surface flush with y = 0.
            var island = Box("Island_Grass", p,
                new Vector3(0f, (IslandBottomY + GrassTopY) * 0.5f, 0f),
                new Vector3(120f, GrassTopY - IslandBottomY, 120f),
                0f, grass);
            // Round the top rim so the grass slopes down to the cliffs instead
            // of meeting them at a hard 90° edge.
            RoundTopEdges(island, 2.5f);

            // Floating chunk underneath (cone, apex pointing down).
            Cone("Island_Underside", p, new Vector3(0f, IslandBottomY - 25f, 0f),
                60f, 50f, 14, Mat("CliffStone", CliffStone), flip: true);

            // Broad, very low mounds give the ground a gentle roll instead of a
            // flat plate. Low-poly icospheres (subdiv 1) keep this mobile-cheap.
            Mound("Slope_W",  p, new Vector3(-34f, -3.5f, 6f),  22f, 0.22f, grass);
            Mound("Slope_NE", p, new Vector3(38f, -3.5f, 22f),  24f, 0.20f, grass);
            Mound("Slope_S",  p, new Vector3(8f, -3.5f, -40f),  20f, 0.22f, grass);

            // Smaller, slightly steeper hills on open ground — kept away from the
            // path, castle and river so those read clearly.
            Hill("Hill_NW", p, new Vector3(-44f, -2f, 35f), 11f, 0.40f, hillMat);
            Hill("Hill_NE", p, new Vector3(40f, -2f, 30f), 13f, 0.38f, hillMat);
            Hill("Hill_E",  p, new Vector3(46f, -2f, -8f), 12f, 0.40f, hillMat);
            Hill("Hill_SE", p, new Vector3(24f, -2f, -46f), 10f, 0.42f, hillMat);
        }

        static void BuildCliffs(Transform p)
        {
            var mat = Mat("CliffStone", CliffStone);
            var riverStart = new Vector2(-58f, 52f);  // NW notch (river enters)
            var riverEnd   = new Vector2(34f, -58f);  // SE notch (river exits)
            const float ext = 55f;

            for (float x = -50f; x <= 50f; x += 20f)
            {
                TryCliff("Cliff_N", p, new Vector3(x, 0f, ext),  new Vector3(22f, 0f, 12f), riverStart, riverEnd, mat);
                TryCliff("Cliff_S", p, new Vector3(x, 0f, -ext), new Vector3(22f, 0f, 12f), riverStart, riverEnd, mat);
            }
            for (float z = -50f; z <= 50f; z += 20f)
            {
                TryCliff("Cliff_E", p, new Vector3(ext, 0f, z),  new Vector3(12f, 0f, 22f), riverStart, riverEnd, mat);
                TryCliff("Cliff_W", p, new Vector3(-ext, 0f, z), new Vector3(12f, 0f, 22f), riverStart, riverEnd, mat);
            }
        }

        static void TryCliff(string baseName, Transform p, Vector3 pos, Vector3 footprint,
            Vector2 riverStart, Vector2 riverEnd, Material mat)
        {
            var xz = new Vector2(pos.x, pos.z);
            if (Vector2.Distance(xz, riverStart) < 18f || Vector2.Distance(xz, riverEnd) < 18f)
                return; // leave a gap so the river can enter / exit the island

            float h = 9f + Mathf.PerlinNoise(pos.x * 0.05f + 3.1f, pos.z * 0.05f + 7.7f) * 7f;
            var center = new Vector3(pos.x, (IslandBottomY + h) * 0.5f, pos.z);
            var size = new Vector3(footprint.x, h - IslandBottomY, footprint.z);
            var cliff = Box($"{baseName}_{_cliffIdx++:00}", p, center, size, 0f, mat);
            // Bevel the top edges so each cliff reads as sloped, weathered rock
            // rather than a sharp block.
            RoundTopEdges(cliff, 2.6f);
        }

        static Vector3[] RiverPoints() => new[]
        {
            new Vector3(-58f, 0f, 52f),  // upper-left, through the cliff notch
            new Vector3(-32f, 0f, 32f),
            new Vector3(-40f, 0f, 8f),
            new Vector3(-12f, 0f, -8f),
            new Vector3(10f,  0f, -24f),
            new Vector3(0f,   0f, -42f),
            new Vector3(34f,  0f, -58f), // lower-right, through the cliff notch
        };

        static void BuildRiver(Transform p, Vector3[] control)
        {
            var water = Mat("Water", Water);
            var bank = Mat("RiverBank", RiverBank);
            const float baseWidth = 11f, bankW = 3.5f;

            // Smooth the control points into a flowing curve so the river bends
            // naturally instead of turning at hard corners.
            var pts = Smooth(control, 3);

            for (int i = 0; i < pts.Length - 1; i++)
            {
                var a = pts[i];
                var b = pts[i + 1];
                var mid = (a + b) * 0.5f;
                var dir = b - a;
                float len = dir.magnitude;
                if (len < 0.001f) continue;
                float yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                var dn = dir.normalized;
                var perp = new Vector3(dn.z, 0f, -dn.x);

                // Gently vary the width so the channel widens and narrows.
                float width = baseWidth * (1f + 0.18f * Mathf.Sin(i * 0.5f));

                // Water sits in a shallow trough, top a touch below the grass.
                // Overlap segments generously so curves stay watertight.
                const float waterH = 3f, waterTop = -0.6f;
                Box($"River_Water_{i:00}", p,
                    new Vector3(mid.x, waterTop - waterH * 0.5f, mid.z),
                    new Vector3(width, waterH, len + 1.5f), yaw, water);

                // Raised banks on either side, with rounded tops.
                const float bankH = 2.4f, bankTop = 0.8f;
                float by = bankTop - bankH * 0.5f;
                float off = width * 0.5f + bankW * 0.5f;
                var bl = Box($"River_Bank_L_{i:00}", p, mid + perp * off + Vector3.up * by,
                    new Vector3(bankW, bankH, len + 1.5f), yaw, bank);
                var br = Box($"River_Bank_R_{i:00}", p, mid - perp * off + Vector3.up * by,
                    new Vector3(bankW, bankH, len + 1.5f), yaw, bank);
                RoundTopEdges(bl, 0.7f);
                RoundTopEdges(br, 0.7f);
            }
        }

        static void BuildBridge(Transform p)
        {
            var mat = Mat("BridgeStone", PathStone);
            // Oriented along the river flow where the central path crosses it.
            var a = new Vector3(-12f, 0f, -8f);
            var b = new Vector3(10f, 0f, -24f);
            float yaw = Mathf.Atan2((b - a).x, (b - a).z) * Mathf.Rad2Deg;
            var q = Quaternion.Euler(0f, yaw, 0f);
            var center = new Vector3(-1f, 0f, -16f);

            const float deckTop = 2.2f, deckTh = 1.2f;
            var deck = Box("Bridge_Deck", p, new Vector3(center.x, deckTop - deckTh * 0.5f, center.z),
                new Vector3(24f, deckTh, 9f), yaw, mat);
            RoundTopEdges(deck, 0.4f);

            for (int s = -1; s <= 1; s += 2)
            {
                var rOff = q * new Vector3(0f, 0f, 4.25f * s);
                Box($"Bridge_Rail_{(s < 0 ? "L" : "R")}", p,
                    new Vector3(center.x + rOff.x, deckTop + 0.5f, center.z + rOff.z),
                    new Vector3(24f, 1f, 0.5f), yaw, mat);

                var pOff = q * new Vector3(8f * s, 0f, 0f);
                Box($"Bridge_Pillar_{(s < 0 ? "L" : "R")}", p,
                    new Vector3(center.x + pOff.x, 0f, center.z + pOff.z),
                    new Vector3(2.5f, 8f, 2.5f), yaw, mat);
            }
        }

        static Vector3[] PathPoints() => new[]
        {
            new Vector3(0f,  0f, 40f),  // base of the castle platform (top centre)
            new Vector3(-6f, 0f, 24f),
            new Vector3(7f,  0f, 8f),
            new Vector3(-5f, 0f, -8f),
            new Vector3(6f,  0f, -26f),
            new Vector3(0f,  0f, -46f), // toward the lower edge of the island
        };

        static void BuildPathsAndNodes(Transform paths, Transform nodes)
        {
            var pmat = Mat("Path", PathStone);
            var nmat = Mat("Node", NodeStone);
            // Smooth the path so it curves gently down the island.
            var pts = Smooth(PathPoints(), 3);
            const float pathW = 8f, th = 0.5f, top = 0.2f;

            for (int i = 0; i < pts.Length - 1; i++)
            {
                var a = pts[i];
                var b = pts[i + 1];
                var mid = (a + b) * 0.5f;
                var dir = b - a;
                float len = dir.magnitude;
                if (len < 0.001f) continue;
                float yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                Box($"Path_Segment_{i:00}", paths,
                    new Vector3(mid.x, top - th * 0.5f, mid.z),
                    new Vector3(pathW, th, len + 1.2f), yaw, pmat);
            }

            // 12 evenly-spaced circular stepping-stone platforms (future
            // chapter nodes), following the curve. Radius leaves a clear ring
            // around each one; 16 sides + a beveled rim keeps them rounded but cheap.
            var spots = SampleEven(pts, 12);
            for (int i = 0; i < spots.Count; i++)
            {
                var node = Cyl($"LevelNode_{i + 1:00}", nodes,
                    new Vector3(spots[i].x, 0.5f, spots[i].z),
                    3.0f, 1.0f, 16, nmat);
                RoundTopEdges(node, 0.3f);
            }
        }

        static void BuildCastle(Transform p)
        {
            var stone = Mat("CastleStone", CastleStone);
            var roofB = Mat("RoofBlue", RoofBlue);
            var roofR = Mat("RoofRed", RoofRed);
            var c = new Vector3(0f, 0f, 45f); // top centre

            // Two-tier circular platform with softly rounded rims.
            var baseTier = Cyl("Castle_Platform_Base", p, new Vector3(c.x, 1.5f, c.z), 17f, 3f, 32, stone);
            var topTier  = Cyl("Castle_Platform_Top",  p, new Vector3(c.x, 3.8f, c.z), 12f, 2f, 32, stone);
            RoundTopEdges(baseTier, 0.8f);
            RoundTopEdges(topTier, 0.7f);

            // Central keep with a roof.
            Cyl("Castle_Keep", p, new Vector3(c.x, 9f, c.z), 5f, 10f, 16, stone);
            Cone("Castle_Keep_Roof", p, new Vector3(c.x, 16.5f, c.z), 6f, 5f, 16, roofB);

            // Four corner towers with roofs.
            const float ring = 9f;
            for (int i = 0; i < 4; i++)
            {
                float a = (45f + i * 90f) * Mathf.Deg2Rad;
                float tx = c.x + Mathf.Cos(a) * ring;
                float tz = c.z + Mathf.Sin(a) * ring;
                Cyl($"Castle_Tower_{i + 1}", p, new Vector3(tx, 8f, tz), 2.2f, 12f, 12, stone);
                Cone($"Castle_Tower_{i + 1}_Roof", p, new Vector3(tx, 16f, tz), 3f, 4f, 12, roofR);
            }

            // Gatehouse facing the path.
            Box("Castle_Gate", p, new Vector3(c.x, 5f, c.z - 15f), new Vector3(7f, 6f, 3f), 0f, stone);
        }

        static void BuildDecorationMarkers(Transform p)
        {
            // Empty markers flagging flat areas reserved for future forests/props.
            var spots = new (string name, Vector3 pos)[]
            {
                ("ForestArea_NW_flat_reserved", new Vector3(-42f, 0f, 12f)),
                ("ForestArea_NE_flat_reserved", new Vector3(38f, 0f, 8f)),
                ("ForestArea_W_flat_reserved",  new Vector3(-46f, 0f, -30f)),
                ("ForestArea_SE_flat_reserved", new Vector3(28f, 0f, -30f)),
            };
            foreach (var (name, pos) in spots)
                NewParent(name, p).transform.localPosition = pos;
        }

        static void BuildLighting(Transform p)
        {
            var go = NewParent("Sun (Directional Light)", p);
            go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var l = go.AddComponent<Light>();
            l.type = LightType.Directional;
            l.color = new Color(1f, 0.96f, 0.84f);
            l.intensity = 1.15f;
            l.shadows = LightShadows.Soft;
            RenderSettings.ambientLight = new Color(0.62f, 0.68f, 0.74f);
        }

        static void BuildCamera(Transform p)
        {
            var go = NewParent("World Camera", p);
            go.tag = "MainCamera"; // it's the only camera in this dedicated scene
            var cam = go.AddComponent<Camera>();
            cam.fieldOfView = 32f;
            cam.nearClipPlane = 0.3f;
            cam.farClipPlane = 2000f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.55f, 0.78f, 0.95f); // sunny sky

            // Isometric framing (Clash Royale / Magic Kingdoms style) that fits the whole island.
            var target = new Vector3(0f, 2f, -4f);
            go.transform.rotation = Quaternion.Euler(38f, 45f, 0f);
            go.transform.position = target - go.transform.forward * 300f;
        }

        // ── ProBuilder primitive helpers ────────────────────────────────────

        static ProBuilderMesh Box(string name, Transform parent, Vector3 center, Vector3 size, float yaw, Material mat)
        {
            var pb = ShapeGenerator.GenerateCube(PivotLocation.Center, size);
            pb.gameObject.name = name;
            pb.transform.SetParent(parent, false);
            pb.transform.localPosition = center;
            pb.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
            Finish(pb, mat);
            return pb;
        }

        static ProBuilderMesh Cyl(string name, Transform parent, Vector3 center, float radius, float height, int sides, Material mat)
        {
            var pb = ShapeGenerator.GenerateCylinder(PivotLocation.Center, sides, radius, height, 0, -1);
            pb.gameObject.name = name;
            pb.transform.SetParent(parent, false);
            pb.transform.localPosition = center;
            Finish(pb, mat);
            return pb;
        }

        static ProBuilderMesh Cone(string name, Transform parent, Vector3 center, float radius, float height, int sides, Material mat, bool flip = false)
        {
            var pb = ShapeGenerator.GenerateCone(PivotLocation.Center, radius, height, sides);
            pb.gameObject.name = name;
            pb.transform.SetParent(parent, false);
            pb.transform.localPosition = center;
            if (flip) pb.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
            Finish(pb, mat);
            return pb;
        }

        static ProBuilderMesh Hill(string name, Transform parent, Vector3 center, float radius, float flatten, Material mat)
            => Sphere(name, parent, center, radius, flatten, mat, subdivisions: 2);

        // Broad, very low-poly mound for gentle rolling slopes (subdiv 1 ≈ 80 tris).
        static ProBuilderMesh Mound(string name, Transform parent, Vector3 center, float radius, float flatten, Material mat)
            => Sphere(name, parent, center, radius, flatten, mat, subdivisions: 1);

        static ProBuilderMesh Sphere(string name, Transform parent, Vector3 center, float radius, float flatten, Material mat, int subdivisions)
        {
            var pb = ShapeGenerator.GenerateIcosahedron(PivotLocation.Center, radius, subdivisions, true, true);
            pb.gameObject.name = name;
            pb.transform.SetParent(parent, false);
            pb.transform.localPosition = center;
            pb.transform.localScale = new Vector3(1f, flatten, 1f);
            Finish(pb, mat);
            return pb;
        }

        static void Finish(ProBuilderMesh pb, Material mat)
        {
            pb.ToMesh();
            pb.Refresh();
            var r = pb.GetComponent<MeshRenderer>();
            if (r != null && mat != null) r.sharedMaterial = mat;
            Undo.RegisterCreatedObjectUndo(pb.gameObject, "Build Reading Adventure World");
        }

        /// <summary>
        /// Bevels the top edges of a generated primitive, turning a hard 90°
        /// edge into a softer chamfered/sloped one. Failures (e.g. amount too
        /// large for the face) are swallowed so one object can't abort the build.
        /// </summary>
        static void RoundTopEdges(ProBuilderMesh pb, float amount)
        {
            try
            {
                var verts = pb.positions;
                if (verts == null || verts.Count == 0) return;

                float maxY = float.MinValue;
                for (int i = 0; i < verts.Count; i++)
                    if (verts[i].y > maxY) maxY = verts[i].y;

                const float tol = 0.05f;
                var edges = new List<Edge>();
                foreach (var f in pb.faces)
                    foreach (var e in f.edges)
                        if (verts[e.a].y > maxY - tol && verts[e.b].y > maxY - tol)
                            edges.Add(e);

                edges = edges.Distinct().ToList();
                if (edges.Count == 0) return;

                Bevel.BevelEdges(pb, edges, amount);
                pb.ToMesh();
                pb.Refresh();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[ReadingAdventure] Bevel skipped on '{pb.name}': {ex.Message}");
            }
        }

        // ── Misc helpers ────────────────────────────────────────────────────

        // Catmull-Rom smoothing: turns a coarse control polyline into a flowing
        // curve. samplesPerSegment controls density (kept low to limit polys).
        static Vector3[] Smooth(Vector3[] ctrl, int samplesPerSegment)
        {
            if (ctrl == null || ctrl.Length < 3 || samplesPerSegment < 1) return ctrl;

            var pts = new List<Vector3>();
            for (int i = 0; i < ctrl.Length - 1; i++)
            {
                var p0 = ctrl[Mathf.Max(i - 1, 0)];
                var p1 = ctrl[i];
                var p2 = ctrl[i + 1];
                var p3 = ctrl[Mathf.Min(i + 2, ctrl.Length - 1)];
                for (int s = 0; s < samplesPerSegment; s++)
                    pts.Add(CatmullRom(p0, p1, p2, p3, s / (float)samplesPerSegment));
            }
            pts.Add(ctrl[ctrl.Length - 1]);
            return pts.ToArray();
        }

        static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float t2 = t * t, t3 = t2 * t;
            return 0.5f * ((2f * p1)
                + (-p0 + p2) * t
                + (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2
                + (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
        }

        static GameObject NewParent(string name, Transform parent)
        {
            var go = new GameObject(name);
            if (parent != null) go.transform.SetParent(parent, false);
            return go;
        }

        static List<Vector3> SampleEven(Vector3[] pts, int count)
        {
            var seg = new float[pts.Length - 1];
            float total = 0f;
            for (int i = 0; i < seg.Length; i++) { seg[i] = (pts[i + 1] - pts[i]).magnitude; total += seg[i]; }

            var result = new List<Vector3>(count);
            for (int n = 0; n < count; n++)
            {
                float f = count == 1 ? 0.5f : Mathf.Lerp(0.05f, 0.97f, n / (float)(count - 1));
                float d = f * total;
                int si = 0; float acc = 0f;
                while (si < seg.Length - 1 && acc + seg[si] < d) { acc += seg[si]; si++; }
                float t = seg[si] <= 0.0001f ? 0f : (d - acc) / seg[si];
                result.Add(Vector3.Lerp(pts[si], pts[si + 1], t));
            }
            return result;
        }

        static Material Mat(string name, Color color)
        {
            if (_materials.TryGetValue(name, out var cached)) return cached;

            const string dir = "Assets/Materials/PrototypeWorld";
            EnsureFolder(dir);
            string path = $"{dir}/{name}.mat";

            var m = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (m == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                m = new Material(shader);
                ApplyColor(m, color);
                AssetDatabase.CreateAsset(m, path);
            }
            else
            {
                ApplyColor(m, color);
            }

            _materials[name] = m;
            return m;
        }

        static void ApplyColor(Material m, Color c)
        {
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
            if (m.HasProperty("_Color")) m.SetColor("_Color", c);
            m.color = c;
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace("\\", "/");
            string leaf = Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, leaf);
        }
    }
}
