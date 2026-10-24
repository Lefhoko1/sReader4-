using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using SReader.Game.ReadingHouse;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// The die game at the dashboard (SC_ReadingHouse_Integration):
    ///   • the reading die (Assets/Art/ReadingHouse/Models/Die/SM_ReadingDie.fbx, built by
    ///     GeneratedAssets/Signpost/build_die.py) on the rug in front of the dashboard, a corner towards the
    ///     reading camera so three faces show, with five tiers of materials (wood, silver, gold, crystal,
    ///     rainbow gem) and a sparkle. The game's screen is drawn on the dashboard's display (DieGamePanel);
    ///   • the reading camera at its approved wide view (display, rug, die, student and both helpers).
    /// The stones stay in the scene but are not used while the die is set. Re-running replaces only what this adds.
    /// </summary>
    public static class DieGameSetup
    {
        const string ModelPath = "Assets/Art/ReadingHouse/Models/Die/SM_ReadingDie.fbx";
        const string Folder = "Assets/Art/ReadingHouse/Die";
        const float DieSize = 1.1f;
        static readonly Vector3 DieOnRug = new Vector3(0.55f, 0f, 0.35f);
        static readonly Vector3 ReadingSpot = new Vector3(-1.0f, 0f, 1.7f);
        static readonly Vector3 CameraAt = new Vector3(0f, 3.7f, 5.2f), CameraAim = new Vector3(0f, 1.7f, -1.0f);
        const float CameraFov = 56f;

        [MenuItem("Tools/sReader/Reading House/Set Up Die Game")]
        public static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new System.InvalidOperationException("Stop Play mode first: the scene can only be changed in Edit mode.");
            var imp = (ModelImporter)AssetImporter.GetAtPath(ModelPath);
            imp.globalScale = 1f; imp.importAnimation = false; imp.importCameras = false; imp.importLights = false;
            imp.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            var tiers = Tiers();
            void Map(string n, Material m) => imp.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material), n), m);
            Map("DieBody", tiers[0].body); Map("DiePlate", tiers[0].plate); Map("DieTrim", tiers[0].trim); Map("DieGem", tiers[0].gem);
            imp.SaveAndReimport();

            var open = SceneManager.GetSceneByPath(ReadingHouseSceneBuilder.ScenePath);
            bool wasOpen = open.IsValid() && open.isLoaded;
            var scene = wasOpen ? open : EditorSceneManager.OpenScene(ReadingHouseSceneBuilder.ScenePath, OpenSceneMode.Additive);
            var previousActive = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(scene);
            try
            {
                foreach (var old in scene.GetRootGameObjects().Where(g => g.name == "Reading Die" || g.name == "Word Board (die game)").ToArray())
                    Object.DestroyImmediate(old);
                var all = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();
                var presenter = all.Select(t => t.GetComponent<ReadingHousePresenter>()).First(p => p != null);
                Renderer R(string n) => all.Select(t => t.GetComponent<Renderer>()).FirstOrDefault(r => r != null && r.name == n)
                                        ?? throw new System.InvalidOperationException($"{n} not found.");
                var rug = R("SR_Rug").bounds;
                var cam = presenter.cameraRig.viewCamera;
                var font = presenter.dashboard.sentenceText != null ? presenter.dashboard.sentenceText.font : null;

                // The die: on the rug in front of the dashboard, where the student and both helpers can watch it.
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
                var dieGo = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                PrefabUtility.UnpackPrefabInstance(dieGo, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                dieGo.name = "Reading Die";
                dieGo.transform.localScale = Vector3.one * DieSize;
                dieGo.transform.rotation = Quaternion.identity;
                var b = new Bounds(dieGo.transform.position, Vector3.zero);
                foreach (var r in dieGo.GetComponentsInChildren<Renderer>()) b.Encapsulate(r.bounds);
                float lift = dieGo.transform.position.y - b.min.y;
                dieGo.transform.position = new Vector3(DieOnRug.x, rug.max.y + lift + 0.005f, DieOnRug.z);
                // A corner towards the reading camera: two side faces and the top all read clearly.
                var toCam = Vector3.ProjectOnPlane(CameraAt - dieGo.transform.position, Vector3.up).normalized;
                dieGo.transform.rotation = Quaternion.LookRotation(toCam, Vector3.up) * Quaternion.Euler(0f, 45f, 0f);
                var col = dieGo.AddComponent<BoxCollider>();
                col.size = Vector3.one * 1.02f; col.center = Vector3.zero;
                foreach (var r in dieGo.GetComponentsInChildren<Renderer>()) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

                var sparkleGo = new GameObject("Sparkle");
                sparkleGo.transform.SetParent(dieGo.transform, false);
                var ps = sparkleGo.AddComponent<ParticleSystem>();
                var main = ps.main; main.startLifetime = 1.1f; main.startSpeed = 0.35f; main.startSize = 0.09f;
                main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.9f, 0.5f), new Color(1f, 0.6f, 0.9f));
                main.simulationSpace = ParticleSystemSimulationSpace.World; main.maxParticles = 60;
                var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Sphere; shape.radius = 0.55f;
                var em = ps.emission; em.rateOverTime = 0f;
                var psr = sparkleGo.GetComponent<ParticleSystemRenderer>();
                psr.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/ReadingHouse/Signpost/M_Sign_LanternGlow.mat");

                var die = dieGo.AddComponent<ReadingDie>();
                die.viewCamera = cam; die.tiers = tiers; die.font = font; die.sparkle = ps;
                die.pictureMaterial = Mat("M_Die_Picture", "Universal Render Pipeline/Unlit", Color.white);
                presenter.die = die;
                dieGo.SetActive(false);

                // The student's reading spot: front-left of the rug, so he never hides the Encourager, the die or the Zombie.
                var spot = presenter.studentMover.gameplaySpot;
                spot.position = new Vector3(ReadingSpot.x, spot.position.y, ReadingSpot.z);
                spot.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(dieGo.transform.position - spot.position, Vector3.up).normalized, Vector3.up);
                EditorUtility.SetDirty(spot);

                // The reading camera: the approved wide view.
                var pose = presenter.cameraRig.poses.First(p => p != null && p.view == ReadingHouseView.Gameplay);
                pose.position.position = CameraAt;
                pose.aim.position = CameraAim;
                pose.referenceVerticalFov = CameraFov;
                EditorUtility.SetDirty(pose.position); EditorUtility.SetDirty(pose.aim);
                EditorUtility.SetDirty(presenter.cameraRig);
                EditorUtility.SetDirty(presenter);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"[DieGame] Die at {dieGo.transform.position} (size {DieSize}), camera {CameraAt} → {CameraAim}.");
            }
            finally
            {
                if (previousActive.IsValid() && previousActive != scene) SceneManager.SetActiveScene(previousActive);
                if (!wasOpen) EditorSceneManager.CloseScene(scene, true);
            }
        }

        static Material Mat(string name, string shader, Color c, Texture tex = null, float smooth = 0.4f, float metal = 0f, bool emission = false, float alpha = 1f)
        {
            string path = Folder + "/" + name + ".mat";
            var m = AssetDatabase.LoadAssetAtPath<Material>(path);
            var sh = Shader.Find(shader);
            if (m == null) { m = new Material(sh); AssetDatabase.CreateAsset(m, path); }
            m.shader = sh;
            c.a = alpha;
            m.SetColor("_BaseColor", c);
            if (tex != null) { m.SetTexture("_BaseMap", tex); m.mainTexture = tex; }
            if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smooth);
            if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metal);
            if (emission) { m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor", Color.black); m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None; }
            if (alpha < 1f)
            {
                m.SetFloat("_Surface", 1f); m.SetFloat("_Blend", 0f);
                m.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                m.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                m.SetFloat("_ZWrite", 0f);
                m.SetOverrideTag("RenderType", "Transparent");
                m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            EditorUtility.SetDirty(m);
            return m;
        }

        static Texture2D Tex(string path)
        {
            AssetDatabase.ImportAsset(path);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        /// <summary>Wood → silver → gold → crystal → rainbow gem: the die grows more precious as key words are restored.</summary>
        static ReadingDie.Tier[] Tiers()
        {
            const string lit = "Universal Render Pipeline/Lit";
            var wood = Tex("Assets/Art/ReadingHouse/Signpost/T_SignWood.png");
            var rainbow = Tex(Folder + "/T_DieRainbow.png");
            var cream = new Color32(255, 246, 228, 255);
            var navyInk = new Color32(24, 34, 70, 255);
            return new[]
            {
                new ReadingDie.Tier { name = "Wood", ink = new Color32(70, 40, 18, 255),
                    body = Mat("M_Die_Wood_Body", lit, Color.white, wood, 0.25f),
                    plate = Mat("M_Die_Wood_Plate", lit, cream, null, 0.35f, 0f, true),
                    trim = Mat("M_Die_Wood_Trim", lit, new Color32(225, 170, 80, 255), null, 0.6f, 0.45f),
                    gem = Mat("M_Die_Wood_Gem", lit, new Color32(110, 200, 255, 255), null, 0.9f) },
                new ReadingDie.Tier { name = "Silver", ink = navyInk,
                    body = Mat("M_Die_Silver_Body", lit, new Color32(222, 228, 238, 255), null, 0.6f, 0.45f),
                    plate = Mat("M_Die_Silver_Plate", lit, new Color32(250, 252, 255, 255), null, 0.5f, 0f, true),
                    trim = Mat("M_Die_Silver_Trim", lit, new Color32(160, 180, 210, 255), null, 0.7f, 0.5f),
                    gem = Mat("M_Die_Silver_Gem", lit, new Color32(120, 220, 255, 255), null, 0.95f) },
                new ReadingDie.Tier { name = "Gold", ink = new Color32(80, 46, 10, 255),
                    body = Mat("M_Die_Gold_Body", lit, new Color32(255, 204, 90, 255), null, 0.65f, 0.45f),
                    plate = Mat("M_Die_Gold_Plate", lit, cream, null, 0.5f, 0f, true),
                    trim = Mat("M_Die_Gold_Trim", lit, new Color32(215, 140, 40, 255), null, 0.7f, 0.5f),
                    gem = Mat("M_Die_Gold_Gem", lit, new Color32(255, 80, 110, 255), null, 0.95f) },
                new ReadingDie.Tier { name = "Crystal", ink = navyInk,
                    body = Mat("M_Die_Crystal_Body", lit, new Color32(170, 225, 255, 255), null, 0.97f, 0f, false, 0.6f),
                    plate = Mat("M_Die_Crystal_Plate", lit, new Color32(245, 252, 255, 255), null, 0.8f, 0f, true),
                    trim = Mat("M_Die_Crystal_Trim", lit, new Color32(220, 235, 255, 255), null, 0.85f, 0.4f),
                    gem = Mat("M_Die_Crystal_Gem", lit, new Color32(150, 255, 240, 255), null, 0.98f) },
                new ReadingDie.Tier { name = "Rainbow gem", ink = navyInk,
                    body = Mat("M_Die_Rainbow_Body", lit, Color.white, rainbow, 0.85f, 0.2f),
                    plate = Mat("M_Die_Rainbow_Plate", lit, Color.white, null, 0.6f, 0f, true),
                    trim = Mat("M_Die_Rainbow_Trim", lit, new Color32(255, 210, 90, 255), null, 0.75f, 0.5f),
                    gem = Mat("M_Die_Rainbow_Gem", lit, new Color32(255, 120, 230, 255), null, 0.98f) },
            };
        }
    }
}
