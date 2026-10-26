using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using SReader.Game.ReadingHouse;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// The dashboard as a big freestanding candy board (SC_ReadingHouse_Integration): the arcade housing's
    /// nested decorative parts (tower shell, orange frame, gold piping, bezels, tray, fascia, plinth, side
    /// lights) are switched off (not deleted), and the candy board model (Candy Crush style, glossy plastic;
    /// Assets/Art/ReadingHouse/Models/CandyBoard/SM_CandyBoard.fbx) stands behind the display. Everything that does something stays and keeps
    /// working: the display (game screen + picture), the assignment header with its progress bar, Play with
    /// Others, the sReader nameplate, and Points / Menu (moved up onto the board, under the display).
    /// The board is parented under the Dashboard root, so a tap on it still counts as the dashboard.
    /// Re-running replaces only what this adds.
    /// </summary>
    public static class DashboardChalkboardSetup
    {
        static readonly string[] Decorative =
        {
            "SR_DB_Console_Light", "SR_DB_Console_Light_Housing", "SR_DB_Fascia", "SR_DB_Plinth",
            "SR_DB_Screen_Bezel", "SR_DB_Screen_Gold_Trim", "SR_DB_Screen_Recess",
            "SR_DB_Side_Light_Housing_L", "SR_DB_Side_Light_Housing_R", "SR_DB_Side_Light_L", "SR_DB_Side_Light_R",
            "SR_DB_Tower_Gold_Piping", "SR_DB_Tower_Orange_Frame", "SR_DB_Tower_Shell",
            "SR_DB_Tray_Floor", "SR_DB_Tray_Front_Lip", "SR_DB_Tray_Rim",
            "SR_Nameplate_Blue", "SR_Nameplate_Gold", "SR_Brand",           // the game screen grows up over the nameplate
        };

        const float SlateZ = -2.26f;
        const string CandyModel = "Assets/Art/ReadingHouse/Models/CandyBoard/SM_CandyBoard.fbx";
        const float ControlsX = 1.85f;
        const float ControlsY = 1.28f, ControlsZ = -2.13f;

        public static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new System.InvalidOperationException("Stop Play mode first: the scene can only be changed in Edit mode.");
            ImportCandyModel();
            var scene = SceneManager.GetSceneByPath(ReadingHouseSceneBuilder.ScenePath);
            if (!scene.IsValid() || !scene.isLoaded) scene = EditorSceneManager.OpenScene(ReadingHouseSceneBuilder.ScenePath, OpenSceneMode.Single);
            var all = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();
            var presenter = all.Select(t => t.GetComponent<ReadingHousePresenter>()).First(p => p != null);
            var dash = presenter.dashboard;
            Transform Find(string n) => all.FirstOrDefault(t => t.name == n);

            var dashRoot = Find("SR_DASHBOARD_ROOT");
            var board = dashRoot.root;                                    // "Dashboard"
            foreach (var old in board.GetComponentsInChildren<Transform>(true).Where(t => t.name == "Chalkboard (dashboard)").ToArray())
                Object.DestroyImmediate(old.gameObject);
            all = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();   // without the old board

            // 1. The arcade housing's decorative parts: off.
            int off = 0;
            foreach (var n in Decorative) { var t = Find(n); if (t != null && t.gameObject.activeSelf) { t.gameObject.SetActive(false); off++; } }
            // ...and the nameplate's "sReader" lettering (the game screen covers that spot now).
            var plate = Find("SR_Nameplate_Gold");
            if (plate != null)
                foreach (var label in board.GetComponentsInChildren<TMPro.TMP_Text>(true))
                    if (label.text != null && label.text.Contains("sReader") && Mathf.Abs(label.transform.position.y - 3.72f) < 0.3f && label.gameObject.activeSelf)
                    { label.gameObject.SetActive(false); off++; }

            // 2. Points and Menu (panel, gold trim, their labels): up onto the board, under the display.
            MoveGroup(new[] { Find("SR_PANEL_POINTS"), Find("SR_DB_PANEL_POINTS_Trim"), dash.pointsLabel != null ? dash.pointsLabel.transform : null, dash.pointsValue != null ? dash.pointsValue.transform : null },
                      new Vector3(ControlsX, ControlsY, ControlsZ), all, "SR_PANEL_POINTS");
            MoveGroup(new[] { Find("SR_BUTTON_MENU"), Find("SR_DB_BUTTON_MENU_Trim"), dash.menuLabel != null ? dash.menuLabel.transform : null },
                      new Vector3(-ControlsX, ControlsY, ControlsZ), all, "SR_BUTTON_MENU");

            // The labels sat on anchors tilted along the old sloping console: lay them flat on their panels now,
            // turned like the header's title (which sits on an upright plate), text to the right of the star.
            var upright = dash.assignmentTitle != null ? dash.assignmentTitle.transform.rotation : Quaternion.Euler(0f, 180f, 0f);
            // Each label becomes a child of its own panel, so it stays on it whatever moves its old parent at runtime.
            void Place(TMPro.TextMeshPro label, string panel, float panelX, float dy)
            {
                var host = Find(panel);
                if (label == null || host == null) return;
                Undo.SetTransformParent(label.transform, host, "Label onto its panel");
                label.transform.SetPositionAndRotation(new Vector3(panelX - 0.15f, ControlsY + dy, ControlsZ + 0.08f), upright);
                EditorUtility.SetDirty(label.transform);
            }
            Place(dash.pointsLabel, "SR_PANEL_POINTS", ControlsX, 0.1f);
            Place(dash.pointsValue, "SR_PANEL_POINTS", ControlsX, -0.08f);
            Place(dash.menuLabel, "SR_BUTTON_MENU", -ControlsX, 0f);

            // 3. The candy board (Blender model, Candy Crush style): a glossy berry slate in a chunky pink
            //    jelly frame with a white shine, rainbow gumballs along the top, wrapped candies on the corners,
            //    a mint jelly tray with jelly beans, candy-striped poles on gumdrop feet.
            float floor = presenter.studentMover.gameplaySpot.position.y;
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CandyModel);
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            go.name = "Chalkboard (dashboard)";
            go.transform.SetParent(board, true);
            go.transform.position = new Vector3(0f, floor, SlateZ);
            var marker = go.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "FrontMarker");
            if (marker != null)
            {
                var have = Vector3.ProjectOnPlane(marker.position - go.transform.position, Vector3.up).normalized;
                go.transform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(have, Vector3.forward, Vector3.up), Vector3.up) * go.transform.rotation;   // its front to the room
            }
            foreach (var r in go.GetComponentsInChildren<Renderer>()) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            Animate(go);

            EditorUtility.SetDirty(board.gameObject);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[ChalkDashboard] {off} decorative parts switched off; chalkboard built; Points / Menu moved onto the board.");
        }

        const string CandyFolder = "Assets/Art/ReadingHouse/Models/CandyBoard";

        /// <summary>
        /// Brings the board to life (CandyBoard): the frame gets a glossy flowing pink-to-cream gradient, the white
        /// highlight a travelling shine, the gumballs become the progress lights, plus twinkles and confetti.
        /// </summary>
        static void Animate(GameObject go)
        {
            Renderer Part(string n) => go.GetComponentsInChildren<Renderer>(true).FirstOrDefault(r => r.name == n);
            var lit = Shader.Find("Universal Render Pipeline/Lit");

            var flow = LoadOrCreate(CandyFolder + "/Materials/M_Candy_FrameFlow.mat", lit);
            var grad = Texture(CandyFolder + "/Textures/T_CandyFrameGradient.png");
            flow.SetTexture("_BaseMap", grad); flow.mainTexture = grad;
            flow.SetColor("_BaseColor", Color.white);
            flow.SetFloat("_Smoothness", 0.95f); flow.SetFloat("_Metallic", 0f);
            flow.EnableKeyword("_EMISSION"); flow.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            flow.SetTexture("_EmissionMap", grad); flow.SetColor("_EmissionColor", new Color(0.2f, 0.2f, 0.2f));   // juicy even in shade
            EditorUtility.SetDirty(flow);

            var shineMat = LoadOrCreate(CandyFolder + "/Materials/M_Candy_Shine.mat", lit);
            var band = Texture(CandyFolder + "/Textures/T_CandyShine.png");
            shineMat.SetColor("_BaseColor", new Color(1f, 0.95f, 0.98f));
            shineMat.SetFloat("_Smoothness", 0.98f);
            shineMat.EnableKeyword("_EMISSION"); shineMat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            shineMat.SetTexture("_EmissionMap", band); shineMat.SetColor("_EmissionColor", Color.white * 1.6f);
            EditorUtility.SetDirty(shineMat);

            // The little sweets never sit in shadow: they stay bright and juicy wherever the room's light falls.
            foreach (var r in go.GetComponentsInChildren<Renderer>(true))
            {
                if (r.name.StartsWith("Candy_") || r.name.StartsWith("JellyBean_") || r.name.StartsWith("Gumball_") || r.name.StartsWith("Gumdrop_"))
                    r.receiveShadows = false;
                // The light probes there were baked inside the old arcade tower (dark): the board uses the room's ambient light.
                r.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            }

            var frame = Part("Frame"); var shine = Part("Frame_Highlight");
            if (frame != null) frame.sharedMaterial = flow;
            if (shine != null) shine.sharedMaterial = shineMat;

            var board = go.AddComponent<CandyBoard>();
            board.frame = frame; board.shine = shine;
            // Reading order: left to right as the student sees the board (the room camera looks along -Z, so screen-left is +X).
            board.gumballs = go.GetComponentsInChildren<Renderer>(true).Where(r => r.name.StartsWith("Gumball_"))
                               .OrderByDescending(r => r.bounds.center.x).ToArray();

            var glow = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/ReadingHouse/Signpost/M_Sign_LanternGlow.mat");
            // Twinkles drifting off the frame.
            if (frame != null)
            {
                var tw = new GameObject("Twinkles").AddComponent<ParticleSystem>();
                tw.transform.SetParent(go.transform, false);
                var main = tw.main; main.startLifetime = 1.4f; main.startSpeed = 0.15f; main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.13f);
                main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.95f, 0.8f), new Color(1f, 0.55f, 0.85f));
                main.maxParticles = 80; main.simulationSpace = ParticleSystemSimulationSpace.World;
                var em = tw.emission; em.rateOverTime = 9f;
                var sh = tw.shape; sh.shapeType = ParticleSystemShapeType.MeshRenderer; sh.meshRenderer = (MeshRenderer)frame;
                var sz = tw.sizeOverLifetime; sz.enabled = true; sz.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1), new Keyframe(1, 0)));
                tw.GetComponent<ParticleSystemRenderer>().sharedMaterial = glow;
                board.twinkle = tw;
            }
            // Confetti: bursts from the board when a sentence is finished or a key word restored.
            var cf = new GameObject("Confetti").AddComponent<ParticleSystem>();
            cf.transform.SetParent(go.transform, false);
            cf.transform.position = go.transform.position + new Vector3(0f, 4.3f, 0.35f);
            var cm = cf.main; cm.startLifetime = new ParticleSystem.MinMaxCurve(1.6f, 2.6f); cm.startSpeed = new ParticleSystem.MinMaxCurve(1.5f, 3.5f);
            cm.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.13f); cm.gravityModifier = 0.6f; cm.maxParticles = 400; cm.playOnAwake = false;
            cm.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
            cm.startColor = new ParticleSystem.MinMaxGradient(Rainbow()) { mode = ParticleSystemGradientMode.RandomColor };
            cm.simulationSpace = ParticleSystemSimulationSpace.World;
            var ce = cf.emission; ce.rateOverTime = 0f;
            var cs = cf.shape; cs.shapeType = ParticleSystemShapeType.Box; cs.scale = new Vector3(6f, 0.2f, 0.2f);
            var rot = cf.rotationOverLifetime; rot.enabled = true; rot.z = new ParticleSystem.MinMaxCurve(-6f, 6f);
            var confettiMat = LoadOrCreate(CandyFolder + "/Materials/M_Candy_Confetti.mat", Shader.Find("Universal Render Pipeline/Particles/Unlit"));
            confettiMat.SetColor("_BaseColor", Color.white); EditorUtility.SetDirty(confettiMat);
            cf.GetComponent<ParticleSystemRenderer>().sharedMaterial = confettiMat;
            cf.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            board.confetti = cf;
        }

        static Gradient Rainbow()
        {
            var g = new Gradient();
            g.SetKeys(new[] { new GradientColorKey(new Color(1f, 0.31f, 0.64f), 0f), new GradientColorKey(new Color(1f, 0.82f, 0.25f), 0.25f),
                              new GradientColorKey(new Color(0.24f, 0.86f, 0.43f), 0.5f), new GradientColorKey(new Color(0.18f, 0.66f, 1f), 0.75f),
                              new GradientColorKey(new Color(0.64f, 0.36f, 1f), 1f) },
                      new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            return g;
        }

        static Material LoadOrCreate(string path, Shader shader)
        {
            var m = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (m == null) { m = new Material(shader); AssetDatabase.CreateAsset(m, path); }
            m.shader = shader;
            return m;
        }

        static Texture2D Texture(string path)
        {
            AssetDatabase.ImportAsset(path);
            var imp = (TextureImporter)AssetImporter.GetAtPath(path);
            imp.wrapMode = TextureWrapMode.Repeat; imp.mipmapEnabled = true;
            imp.SetPlatformTextureSettings(new TextureImporterPlatformSettings { name = "Android", overridden = true, maxTextureSize = 512, format = TextureImporterFormat.ASTC_6x6 });
            imp.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        /// <summary>Glossy candy plastic for every part of the board (URP Lit, high smoothness), mapped by the model's slot names.</summary>
        static void ImportCandyModel()
        {
            var imp = (ModelImporter)AssetImporter.GetAtPath(CandyModel);
            imp.globalScale = 1f; imp.importAnimation = false; imp.importCameras = false; imp.importLights = false;
            imp.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            var colours = new (string slot, string hex, float smooth)[]
            {
                ("SlateBerry", "2B1F5E", 0.6f), ("CandyPink", "FF4FA3", 0.88f), ("CandyYellow", "FFD23F", 0.88f),
                ("StripeRed", "FF3355", 0.88f), ("StripeWhite", "FFF6F0", 0.85f), ("GumdropGreen", "3DDC6E", 0.9f),
                ("JellyMint", "6FF2D2", 0.9f), ("CandyOrange", "FF9A1F", 0.88f), ("CandyBlue", "2FA8FF", 0.88f),
                ("CandyPurple", "A35CFF", 0.88f),
            };
            System.IO.Directory.CreateDirectory("Assets/Art/ReadingHouse/Models/CandyBoard/Materials");
            foreach (var (slot, hex, smooth) in colours)
            {
                string path = "Assets/Art/ReadingHouse/Models/CandyBoard/Materials/M_Candy_" + slot + ".mat";
                var m = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (m == null) { m = new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(m, path); }
                ColorUtility.TryParseHtmlString("#" + hex, out var c);
                m.SetColor("_BaseColor", c);
                m.SetFloat("_Smoothness", smooth);
                m.SetFloat("_Metallic", 0f);
                // Candy glows a little from inside (stays bright and juicy even in shadow); the slate does not.
                if (slot != "SlateBerry")
                {
                    m.EnableKeyword("_EMISSION");
                    m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
                    m.SetColor("_EmissionColor", c * 0.32f);
                }
                EditorUtility.SetDirty(m);
                imp.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material), slot), m);
            }
            imp.SaveAndReimport();
        }

        /// <summary>Move a panel and everything belonging to it by the same offset (labels and icons sitting on it come along).</summary>
        static void MoveGroup(Transform[] parts, Vector3 target, Transform[] all, string anchorName)
        {
            var anchor = parts.FirstOrDefault(p => p != null && p.name == anchorName);
            if (anchor == null) return;
            var r = anchor.GetComponent<Renderer>();
            var from = r != null ? r.bounds.center : anchor.position;
            var delta = target - from;
            var group = parts.Where(p => p != null).ToList();
            // Anything else drawn on the panel (an icon, a star) moves too.
            if (r != null)
            {
                var b = r.bounds; b.Expand(new Vector3(0.1f, 0.1f, 0.3f));
                foreach (var t in all)
                {
                    if (group.Contains(t) || t.GetComponent<Renderer>() == null || t.name.StartsWith("SR_DB_")) continue;
                    if (t.root != anchor.root) continue;
                    if (b.Contains(t.GetComponent<Renderer>().bounds.center) && !group.Any(g => t.IsChildOf(g))) group.Add(t);
                }
            }
            foreach (var t in group.Where(t => !group.Any(g => g != t && t.IsChildOf(g))))
            {
                Undo.RecordObject(t, "Move onto the chalkboard");
                t.position += delta;
                PrefabUtility.RecordPrefabInstancePropertyModifications(t);
                EditorUtility.SetDirty(t);
            }
        }
    }
}
