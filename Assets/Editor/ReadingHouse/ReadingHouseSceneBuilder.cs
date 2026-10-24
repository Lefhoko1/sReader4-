using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using SReader.Game.ReadingHouse;
using Object = UnityEngine.Object;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// Builds the Reading House integration from the Blender v04 export:
    ///   1. URP materials from Data/reading_house_spec.json (one per Blender material)
    ///   2. FBX import settings + material remaps
    ///   3. the reusable word-stone prefab
    ///   4. a NEW scene, Assets/Scenes/SC_ReadingHouse_Integration.unity
    /// The scene is built additively and saved on its own, so whatever scene is
    /// open stays untouched, and no existing scene, script or build setting is
    /// modified. Safe to re-run: everything it makes is regenerated.
    /// </summary>
    public static class ReadingHouseSceneBuilder
    {
        public const string Root = "Assets/Art/ReadingHouse";
        public const string ScenePath = "Assets/Scenes/SC_ReadingHouse_Integration.unity";
        public const string StonePrefabPath = Root + "/Prefabs/P_WordStone.prefab";
        const string SpecPath = Root + "/Data/reading_house_spec.json";
        const string IllustrationPath = Root + "/Textures/T_Display_Illustration_Sample.png";
        const string SampleBookPath = "Assets/Resources/Books/SampleBook_Nature_MeadowAtDawn.json";
        const int Renderer3DIndex = 1;   // UniversalRP renderer list: 0 = 2D, 1 = 3D

        static readonly string[] Models =
        {
            Root + "/Models/SM_ReadingHouse_Environment.fbx",
            Root + "/Models/SM_ReadingHouse_Dashboard.fbx",
            Root + "/Models/SM_ReadingHouse_Characters_OPT.fbx",
            Root + "/Models/SM_WordStone_Modular.fbx",
        };

        [Serializable] class Spec { public MaterialSpec[] materials; public CameraSpec[] cameras; }
        [Serializable] class MaterialSpec { public string name, @base, emission, texture; public float metallic, smoothness, emission_intensity, alpha; public bool unlit; }
        [Serializable] class CameraSpec { public string key; public float vertical_fov_deg; }

        [MenuItem("Tools/sReader/Reading House/Build Integration Scene")]
        public static void BuildAll()
        {
            var spec = JsonUtility.FromJson<Spec>(File.ReadAllText(SpecPath));
            var materials = BuildMaterials(spec);
            ConfigureModels(materials);
            var stone = BuildStonePrefab(materials);
            BuildScene(spec, materials, stone);
            Debug.Log($"[ReadingHouse] Built {ScenePath}");
        }

        // ── 1. materials ─────────────────────────────────────────────────────

        static Dictionary<string, Material> BuildMaterials(Spec spec)
        {
            string folder = Root + "/Materials";
            EnsureFolder(folder);
            var lit = Shader.Find("Universal Render Pipeline/Lit");
            var unlit = Shader.Find("Universal Render Pipeline/Unlit");
            var made = new Dictionary<string, Material>();

            foreach (var m in spec.materials)
            {
                var mat = LoadOrCreate($"{folder}/{m.name}.mat", m.unlit ? unlit : lit);
                if (m.unlit)
                {
                    mat.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(IllustrationPath));
                    mat.SetColor("_BaseColor", Color.white);
                }
                else ApplyLit(mat, Hex(m.@base), m.metallic, m.smoothness, m.emission, m.emission_intensity);
                made[m.name] = mat;
            }

            // Word-stone state materials (StoneState order) and display helpers.
            made["RH_Stone_Normal"] = Lit(folder, "RH_Stone_Normal", "#F7F2DC", 0.6f);
            made["RH_Stone_KeyWord"] = Lit(folder, "RH_Stone_KeyWord", "#FFD35D", 0.65f, "#FFC23A", 0.35f);
            made["RH_Stone_Selected"] = Lit(folder, "RH_Stone_Selected", "#FFFFFF", 0.7f, "#7FE3FF", 0.8f);
            made["RH_Stone_Completed"] = Lit(folder, "RH_Stone_Completed", "#A9E989", 0.6f);
            made["RH_Stone_Deferred"] = Lit(folder, "RH_Stone_Deferred", "#E6BCF3", 0.6f);
            made["RH_Stone_Disabled"] = Lit(folder, "RH_Stone_Disabled", "#969FA2", 0.4f);
            var panel = LoadOrCreate($"{folder}/RH_Sentence_Panel.mat", unlit);
            panel.SetColor("_BaseColor", Hex("#FBF8F0"));
            made["RH_Sentence_Panel"] = panel;
            AssetDatabase.SaveAssets();
            return made;
        }

        static Material Lit(string folder, string name, string hex, float smoothness, string emission = null, float intensity = 0f)
        {
            var mat = LoadOrCreate($"{folder}/{name}.mat", Shader.Find("Universal Render Pipeline/Lit"));
            ApplyLit(mat, Hex(hex), 0f, smoothness, emission, intensity);
            return mat;
        }

        static void ApplyLit(Material mat, Color baseColor, float metallic, float smoothness, string emissionHex, float intensity)
        {
            mat.SetColor("_BaseColor", baseColor);
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Smoothness", Mathf.Clamp01(smoothness));
            if (!string.IsNullOrEmpty(emissionHex) && intensity > 0f)
            {
                mat.EnableKeyword("_EMISSION");
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                mat.SetColor("_EmissionColor", Hex(emissionHex) * Mathf.Min(intensity, 3f));
            }
            else
            {
                mat.DisableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.black);
            }
            mat.enableInstancing = true;
            EditorUtility.SetDirty(mat);
        }

        static Material LoadOrCreate(string path, Shader shader)
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) { mat = new Material(shader); AssetDatabase.CreateAsset(mat, path); }
            else if (mat.shader != shader) mat.shader = shader;
            return mat;
        }

        // ── 2. model import settings ─────────────────────────────────────────

        static void ConfigureModels(Dictionary<string, Material> materials)
        {
            foreach (var path in Models)
            {
                var importer = (ModelImporter)AssetImporter.GetAtPath(path);
                if (importer == null) throw new FileNotFoundException("Reading House model missing — run the Blender export first.", path);
                importer.globalScale = 1f;
                importer.useFileScale = true;
                importer.bakeAxisConversion = false;
                importer.importCameras = false;
                importer.importLights = false;
                importer.importAnimation = false;
                importer.animationType = ModelImporterAnimationType.None;
                importer.importNormals = ModelImporterNormals.Import;       // keeps the transferred smooth normals
                importer.importTangents = ModelImporterTangents.None;       // no normal maps
                importer.isReadable = false;
                importer.optimizeMeshPolygons = true;
                importer.optimizeMeshVertices = true;
                importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;

                foreach (var embedded in AssetDatabase.LoadAllAssetsAtPath(path).OfType<Material>())
                    if (materials.TryGetValue(embedded.name, out var mat))
                        importer.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material), embedded.name), mat);
                importer.SaveAndReimport();
            }
        }

        // ── 3. word stone prefab ─────────────────────────────────────────────

        static WordStoneView BuildStonePrefab(Dictionary<string, Material> materials)
        {
            EnsureFolder(Root + "/Prefabs");
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(Models[3]);
            var wrapper = (GameObject)PrefabUtility.InstantiatePrefab(model);
            PrefabUtility.UnpackPrefabInstance(wrapper, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            // Unity collapses a single-root FBX into the file root, so the stone root may BE the wrapper.
            var root = wrapper.transform.name == "SR_STONE_PREFAB_SOURCE" || HasChild(wrapper.transform, "SR_STONE_CAP_LEFT", direct: true)
                ? wrapper.transform
                : Find(wrapper.transform, "SR_STONE_PREFAB_SOURCE");
            if (root != wrapper.transform)
            {
                root.SetParent(null, true);
                Object.DestroyImmediate(wrapper);
            }
            root.name = "P_WordStone";
            root.position = Vector3.zero;

            var view = root.gameObject.AddComponent<WordStoneView>();
            view.capLeft = Find(root, "SR_STONE_CAP_LEFT");
            view.centre = Find(root, "SR_STONE_STRETCH_CENTRE");
            view.capRight = Find(root, "SR_STONE_CAP_RIGHT");
            view.mossCapLeft = Find(root, "SR_STONE_MOSS_CAP_LEFT");
            view.mossCentre = Find(root, "SR_STONE_MOSS_STRETCH_CENTRE");
            view.mossCapRight = Find(root, "SR_STONE_MOSS_CAP_RIGHT");
            foreach (var r in new[] { view.mossCapLeft, view.mossCentre, view.mossCapRight })
                SetAll(r.GetComponent<Renderer>(), materials["SR_MAT_Moss"]);

            // Label on the text anchor, a child of the ROOT so it never stretches.
            var anchor = Find(root, "SR_STONE_TEXT_ANCHOR");
            var into = Vector3.ProjectOnPlane(root.position - anchor.position, Vector3.up).normalized;
            var labelGo = new GameObject("Label", typeof(TextMeshPro));
            labelGo.transform.SetParent(root, false);
            labelGo.transform.position = anchor.position;
            labelGo.transform.rotation = Quaternion.LookRotation(into, Vector3.up);
            var tmp = labelGo.GetComponent<TextMeshPro>();
            ConfigureText(tmp, "Word", TextAlignmentOptions.Center, Hex("#1C2B4A"));
            tmp.rectTransform.sizeDelta = new Vector2(4f, 1f);
            view.label = tmp;

            var box = root.gameObject.AddComponent<BoxCollider>();
            box.center = new Vector3(0f, 0.24f, 0f);
            box.size = new Vector3(1.4f, 0.48f, 0.4f);
            view.tapCollider = box;

            view.stateMaterials = new[]
            {
                materials["RH_Stone_Normal"], materials["RH_Stone_KeyWord"], materials["RH_Stone_Selected"],
                materials["RH_Stone_Completed"], materials["RH_Stone_Deferred"], materials["RH_Stone_Disabled"],
            };
            foreach (var t in new[] { view.capLeft, view.centre, view.capRight })
                SetAll(t.GetComponent<Renderer>(), materials["RH_Stone_Normal"]);

            PrefabUtility.SaveAsPrefabAsset(root.gameObject, StonePrefabPath);
            Object.DestroyImmediate(root.gameObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(StonePrefabPath, ImportAssetOptions.ForceUpdate);
            return AssetDatabase.LoadAssetAtPath<GameObject>(StonePrefabPath).GetComponent<WordStoneView>();   // fresh reference after the re-save
        }

        // ── 4. the integration scene ─────────────────────────────────────────

        static void BuildScene(Spec spec, Dictionary<string, Material> materials, WordStoneView stonePrefab)
        {
            // The integration scene is generated: if a copy is open, drop it so the rebuild can be saved.
            // If it is the only open scene, build straight into a fresh single scene and leave that open.
            var open = SceneManager.GetSceneByPath(ScenePath);
            bool additive = true;
            if (open.IsValid() && open.isLoaded)
            {
                if (SceneManager.sceneCount > 1) EditorSceneManager.CloseScene(open, true);
                else additive = false;
            }
            // Unity refuses an additive new scene next to an unsaved untitled one.
            for (int i = 0; i < SceneManager.sceneCount; i++)
                if (string.IsNullOrEmpty(SceneManager.GetSceneAt(i).path)) additive = false;
            var previousActive = SceneManager.GetActiveScene();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, additive ? NewSceneMode.Additive : NewSceneMode.Single);
            SceneManager.SetActiveScene(scene);
            try
            {
                var env = Spawn(Models[0], "Environment");
                var dash = Spawn(Models[1], "Dashboard");
                var chars = Spawn(Models[2], "Characters");
                MarkStatic(env);
                MarkStatic(dash);
                var fill = Find(dash.transform, "SR_HEADER_PROGRESS_FILL");
                GameObjectUtility.SetStaticEditorFlags(fill.gameObject, (StaticEditorFlags)0);   // scaled at runtime

                BuildLighting();
                var cam = BuildCamera();

                var rig = new GameObject("Camera Rig").AddComponent<ReadingHouseCameraRig>();
                rig.viewCamera = cam;
                rig.poses = new[]
                {
                    Pose(env.transform, ReadingHouseView.Overview, "OVERVIEW", spec),
                    Pose(env.transform, ReadingHouseView.Gameplay, "GAMEPLAY", spec),
                    Pose(env.transform, ReadingHouseView.TrayFocus, "TRAY_FOCUS", spec),
                };

                var mover = new GameObject("Student Mover").AddComponent<ReadingHouseStudentMover>();
                mover.student = Find(chars.transform, "SR_CHARACTER_STUDENT_OPT");
                mover.overviewSpot = Find(chars.transform, "SR_ANCHOR_STUDENT_OVERVIEW");
                mover.gameplaySpot = Find(chars.transform, "SR_ANCHOR_STUDENT_GAMEPLAY");

                var dashboard = BuildDashboardBinder(dash.transform, chars.transform, materials);

                var trayGo = new GameObject("Word Stone Tray");
                var dashRoot = Find(dash.transform, "SR_DASHBOARD_ROOT");
                trayGo.transform.SetPositionAndRotation(Find(dash.transform, "SR_STONE_TRAY_ANCHOR").position, dashRoot.rotation);
                var tray = trayGo.AddComponent<WordStoneTray>();
                tray.stonePrefab = stonePrefab;
                tray.frontRowAnchor = Find(dash.transform, "SR_ANCHOR_TRAY_ROW_0");
                tray.backRowAnchor = Find(dash.transform, "SR_ANCHOR_TRAY_ROW_1");
                tray.trayLeftMarker = Find(dash.transform, "SR_ANCHOR_PREVIOUS_SENTENCE");
                tray.trayRightMarker = Find(dash.transform, "SR_ANCHOR_NEXT_SENTENCE");

                var presenterGo = new GameObject("Reading House Presenter");
                var presenter = presenterGo.AddComponent<ReadingHousePresenter>();
                presenter.dashboard = dashboard;
                presenter.tray = tray;
                presenter.cameraRig = rig;
                presenter.studentMover = mover;
                presenter.sampleBook = AssetDatabase.LoadAssetAtPath<TextAsset>(SampleBookPath);
                presenter.sampleIllustration = AssetDatabase.LoadAssetAtPath<Texture2D>(IllustrationPath);

                var input = presenterGo.AddComponent<ReadingHouseInput>();
                input.viewCamera = cam;
                input.presenter = presenter;
                input.cameraRig = rig;
                presenterGo.AddComponent<ReadingHousePerfOverlay>();

                rig.GoTo(ReadingHouseView.Overview, immediate: true);
                EnsureFolder(Path.GetDirectoryName(ScenePath).Replace('\\', '/'));
                EditorSceneManager.SaveScene(scene, ScenePath);
            }
            finally
            {
                if (additive)
                {
                    if (previousActive.IsValid() && previousActive.isLoaded) SceneManager.SetActiveScene(previousActive);
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        /// <summary>
        /// Open the integration scene for a read-only check. Returns a callback that discards
        /// every change afterwards: closes it if it was opened here, or reloads it from disk
        /// if it was already the scene open in the editor.
        /// </summary>
        public static (Scene scene, Action discard) OpenForCheck()
        {
            var already = SceneManager.GetSceneByPath(ScenePath);
            bool wasOpen = already.IsValid() && already.isLoaded;
            // Always start from the saved file so asset references are fresh.
            var scene = wasOpen && SceneManager.sceneCount == 1
                ? EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single)
                : EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Additive);
            if (wasOpen && SceneManager.sceneCount > 1) wasOpen = false;
            var previous = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(scene);
            return (scene, () =>
            {
                if (wasOpen) EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
                else
                {
                    if (previous.IsValid() && previous.isLoaded) SceneManager.SetActiveScene(previous);
                    EditorSceneManager.CloseScene(scene, true);
                }
            });
        }

        static GameObject Spawn(string modelPath, string name)
        {
            var go = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(modelPath));
            go.name = name;
            return go;
        }

        static void MarkStatic(GameObject root)
        {
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
                GameObjectUtility.SetStaticEditorFlags(t.gameObject,
                    StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic);
        }

        static void BuildLighting()
        {
            var sun = new GameObject("Key Light (warm)").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = Hex("#FFE2BE");
            sun.intensity = 1.05f;
            sun.shadows = LightShadows.None;   // Huawei Y5: no realtime shadows for now
            sun.transform.rotation = Quaternion.Euler(48f, 18f, 0f);

            foreach (var (name, pos, color, range, intensity) in new[]
            {
                ("Wall Glow Left", new Vector3(-3.9f, 3.0f, -2.0f), "#FFB070", 6f, 1.4f),
                ("Wall Glow Right", new Vector3(3.9f, 3.0f, -2.0f), "#FFB070", 6f, 1.4f),
            })
            {
                var l = new GameObject(name).AddComponent<Light>();
                l.type = LightType.Point;
                l.color = Hex(color);
                l.range = range;
                l.intensity = intensity;
                l.shadows = LightShadows.None;
                l.transform.position = pos;
            }

            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = Hex("#FFEAD2");
            RenderSettings.ambientEquatorColor = Hex("#F0C8A0");
            RenderSettings.ambientGroundColor = Hex("#6E4C37");
            RenderSettings.skybox = null;
            RenderSettings.fog = false;
        }

        static Camera BuildCamera()
        {
            var go = new GameObject("Reading House Camera") { tag = "MainCamera" };
            var cam = go.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Hex("#F2D9B8");
            cam.nearClipPlane = 0.05f;
            cam.farClipPlane = 60f;
            go.AddComponent<AudioListener>();
            var data = go.AddComponent<UniversalAdditionalCameraData>();
            data.SetRenderer(Renderer3DIndex);
            data.renderPostProcessing = false;
            data.antialiasing = AntialiasingMode.None;
            data.renderShadows = false;
            return cam;
        }

        static ReadingHouseCameraRig.Pose Pose(Transform env, ReadingHouseView view, string key, Spec spec)
        {
            var fov = spec.cameras.FirstOrDefault(c => c.key == key)?.vertical_fov_deg ?? 45f;
            return new ReadingHouseCameraRig.Pose
            {
                view = view,
                position = Find(env, "SR_CAMPOS_" + key),
                aim = Find(env, "SR_CAMTGT_" + key),
                referenceVerticalFov = fov,
                keepVerticals = view != ReadingHouseView.TrayFocus,   // the tray view looks steeply down on purpose
            };
        }

        static ReadingHouseDashboard BuildDashboardBinder(Transform dash, Transform chars, Dictionary<string, Material> materials)
        {
            var binder = new GameObject("Dashboard Binder").AddComponent<ReadingHouseDashboard>();
            // "into the dashboard": from the stone tray towards the display, flattened.
            var into = Vector3.ProjectOnPlane(Find(dash, "SR_ANCHOR_READING_CONTENT").position - Find(dash, "SR_STONE_TRAY_ANCHOR").position, Vector3.up).normalized;
            var along = (Find(dash, "SR_ANCHOR_NEXT_SENTENCE").position - Find(dash, "SR_ANCHOR_PREVIOUS_SENTENCE").position).normalized;
            var up = Vector3.Cross(into, along).normalized;
            if (Vector3.Dot(up, Vector3.up) < 0f) up = -up;

            TextMeshPro Text(string name, string anchor, Vector3 offset, string sample, float lineHeight, TextAlignmentOptions align, string hex, float boxWidth)
            {
                var a = Find(dash, anchor);
                var go = new GameObject(name, typeof(TextMeshPro));
                go.transform.SetParent(binder.transform, false);
                go.transform.position = a.position + along * offset.x + up * offset.y - into * (0.012f + offset.z);
                go.transform.rotation = Quaternion.LookRotation(into, up);
                var t = go.GetComponent<TextMeshPro>();
                ConfigureText(t, sample, align, Hex(hex));
                t.fontSize = FontSizeForLineHeight(t, lineHeight);
                t.rectTransform.sizeDelta = new Vector2(boxWidth / Mathf.Max(1e-4f, go.transform.lossyScale.x), lineHeight * 1.6f);
                if (align == TextAlignmentOptions.Left || align == TextAlignmentOptions.MidlineLeft)
                    t.rectTransform.pivot = new Vector2(0f, 0.5f);
                return t;
            }

            binder.assignmentTitle = Text("Assignment Title", "SR_ANCHOR_ASSIGNMENT_HEADER", Vector3.zero, "Assignment", 0.17f, TextAlignmentOptions.MidlineLeft, "#16233F", 1.7f);
            binder.sentenceProgress = Text("Sentence Progress", "SR_ANCHOR_SENTENCE_PROGRESS", Vector3.zero, "Sentence 1 of 1", 0.09f, TextAlignmentOptions.MidlineLeft, "#3B4766", 1.7f);
            binder.multiplayerLabel = Text("Play With Others Label", "SR_ANCHOR_MULTIPLAYER", Vector3.zero, "Play with Others", 0.16f, TextAlignmentOptions.Center, "#FFFFFF", 1.6f);
            binder.pointsLabel = Text("Points Label", "SR_ANCHOR_POINTS", new Vector3(0.15f, 0.1f, 0f), "Your Points", 0.1f, TextAlignmentOptions.Center, "#FFFFFF", 1.4f);
            binder.pointsValue = Text("Points Value", "SR_ANCHOR_POINTS", new Vector3(0.15f, -0.08f, 0f), "0", 0.2f, TextAlignmentOptions.Center, "#FFC23A", 1.4f);
            binder.menuLabel = Text("Menu Label", "SR_ANCHOR_MENU", new Vector3(0.15f, 0f, 0f), "Menu", 0.22f, TextAlignmentOptions.Center, "#FFFFFF", 1.4f);
            binder.sentenceText = Text("Sentence Text", "SR_ANCHOR_READING_CONTENT", new Vector3(0f, 0.42f, 0.006f), "Sentence", 0.2f, TextAlignmentOptions.Center, "#16233F", 4.6f);
            // Long sentences wrap onto two lines and shrink (down to 60%) rather than being cut off.
            var st = binder.sentenceText;
            st.textWrappingMode = TextWrappingModes.Normal;
            st.overflowMode = TextOverflowModes.Truncate;
            st.enableAutoSizing = true;
            st.fontSizeMax = st.fontSize;
            st.fontSizeMin = st.fontSize * 0.6f;
            st.rectTransform.sizeDelta = new Vector2(st.rectTransform.sizeDelta.x, 0.6f / Mathf.Max(1e-4f, st.transform.lossyScale.y));

            // Soft panel behind the sentence (the concept's white banner).
            var panel = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Object.DestroyImmediate(panel.GetComponent<Collider>());
            panel.name = "Sentence Panel";
            panel.transform.SetParent(binder.transform, false);
            var content = Find(dash, "SR_ANCHOR_READING_CONTENT").position;
            panel.transform.position = content + up * 0.42f - into * 0.006f;
            panel.transform.rotation = Quaternion.LookRotation(into, up);
            panel.transform.localScale = new Vector3(4.9f, 0.66f, 1f);
            panel.GetComponent<Renderer>().sharedMaterial = materials["RH_Sentence_Panel"];
            binder.sentencePanel = panel.GetComponent<Renderer>();

            binder.progressFill = Find(dash, "SR_HEADER_PROGRESS_FILL");
            binder.illustration = Find(dash, "SR_DISPLAY_ILLUSTRATION").GetComponent<Renderer>();

            var centre = Find(dash, "SR_ANCHOR_READING_CONTENT").position;
            (binder.encouragerLine, binder.encouragerSpeech, binder.encouragerOverviewPos, binder.encouragerGameplayPos) =
                Speech(binder.transform, chars, "SR_ANCHOR_ENCOURAGER_SPEECH", into, up, centre, materials["RH_Sentence_Panel"]);
            (binder.zombieLine, binder.zombieSpeech, binder.zombieOverviewPos, binder.zombieGameplayPos) =
                Speech(binder.transform, chars, "SR_ANCHOR_ZOMBIE_SPEECH", into, up, centre, materials["RH_Sentence_Panel"]);

            foreach (var (obj, control) in new[]
            {
                ("SR_BUTTON_MENU", DashboardControl.Menu), ("SR_BUTTON_MULTIPLAYER", DashboardControl.PlayWithOthers),
                ("SR_PANEL_POINTS", DashboardControl.Points), ("SR_Assignment_Housing", DashboardControl.Assignment),
            })
            {
                var t = Find(dash, obj).gameObject;
                if (t.GetComponent<BoxCollider>() == null) t.AddComponent<BoxCollider>();
                var target = t.GetComponent<DashboardControlTarget>();
                if (target == null) target = t.AddComponent<DashboardControlTarget>();
                target.control = control;
            }
            return binder;
        }

        /// <summary>A character's speech line on a small cream panel, pulled 0.5 m in towards the dashboard so it stays on screen.</summary>
        static (TextMeshPro text, Transform root, Vector3 overviewPos, Vector3 gameplayPos) Speech(
            Transform parent, Transform chars, string anchorPrefix, Vector3 into, Vector3 up, Vector3 dashboardCentre, Material panelMaterial)
        {
            var a = chars.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name.StartsWith(anchorPrefix));
            if (a == null) { Debug.LogWarning($"[ReadingHouse] {anchorPrefix} not found; that character's line is skipped."); return (null, null, default, default); }
            var inward = Vector3.ProjectOnPlane(dashboardCentre - a.position, Vector3.up);
            inward = Vector3.Project(inward, Vector3.Cross(up, into)).normalized;   // sideways only
            var pos = a.position + inward * 0.5f;
            var gameplayPos = a.position + inward * 1.35f + up * 0.2f;            // the gameplay camera crops the sides
            var rot = Quaternion.LookRotation(into, up);
            string label = anchorPrefix.Replace("SR_ANCHOR_", "");

            var root = new GameObject(label + " Speech").transform;
            root.SetParent(parent, false);
            root.SetPositionAndRotation(pos, rot);

            var panel = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Object.DestroyImmediate(panel.GetComponent<Collider>());
            panel.name = label + " Panel";
            panel.transform.SetParent(root, false);
            panel.transform.localPosition = Vector3.forward * 0.01f;
            panel.transform.localScale = new Vector3(1.6f, 0.46f, 1f);
            panel.GetComponent<Renderer>().sharedMaterial = panelMaterial;

            var go = new GameObject(label + " Line", typeof(TextMeshPro));
            go.transform.SetParent(root, false);
            var t = go.GetComponent<TextMeshPro>();
            ConfigureText(t, "…", TextAlignmentOptions.Center, Hex("#16233F"));
            t.fontSize = FontSizeForLineHeight(t, 0.13f);
            t.textWrappingMode = TextWrappingModes.Normal;
            t.rectTransform.sizeDelta = new Vector2(1.45f / Mathf.Max(1e-4f, go.transform.lossyScale.x), 0.42f);
            return (t, root, pos, gameplayPos);
        }

        // ── helpers ──────────────────────────────────────────────────────────

        static void ConfigureText(TextMeshPro t, string sample, TextAlignmentOptions align, Color color)
        {
            t.text = sample;
            t.alignment = align;
            t.color = color;
            t.textWrappingMode = TextWrappingModes.NoWrap;
            t.overflowMode = TextOverflowModes.Ellipsis;
            t.fontStyle = FontStyles.Bold;
            if (TMP_Settings.defaultFontAsset != null) t.font = TMP_Settings.defaultFontAsset;
        }

        static float FontSizeForLineHeight(TextMeshPro t, float height)
        {
            t.fontSize = 1f;
            float h = t.GetPreferredValues("Hg").y * t.transform.lossyScale.y;
            return h > 1e-5f ? height / h : 1f;
        }

        static void SetAll(Renderer r, Material m)
        {
            if (r == null) return;
            var mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++) mats[i] = m;
            r.sharedMaterials = mats;
        }

        public static Transform Find(Transform root, string name)
        {
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
                if (t.name == name) return t;
            throw new InvalidOperationException($"'{name}' not found under '{root.name}' — check the Blender export names.");
        }

        static bool HasChild(Transform root, string name, bool direct)
        {
            if (direct) { foreach (Transform c in root) if (c.name == name) return true; return false; }
            return root.GetComponentsInChildren<Transform>(true).Any(t => t.name == name);
        }

        static Color Hex(string hex) => ColorUtility.TryParseHtmlString(hex, out var c) ? c : Color.magenta;

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parent = Path.GetDirectoryName(path).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
    }
}
