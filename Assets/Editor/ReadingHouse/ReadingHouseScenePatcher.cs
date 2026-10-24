using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using SReader.Game.ReadingHouse;
using Object = UnityEngine.Object;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// Adds the walk-in experience to the EXISTING Reading House scene, in place
    /// (the scene is not rebuilt): colliders on the house, a walking student, the
    /// follow camera and joystick, the front doors (closed until sign-in), the
    /// Log in / Register door panels wired to the app's own Login / Register pages
    /// and AppCompositionRoot (reused unchanged), the chalkboard text, and the
    /// opening wide shot of the house. Re-running replaces only what it added.
    /// </summary>
    public static class ReadingHouseScenePatcher
    {
        const string GroupName = "Walk-in & Sign-in";
        const string PanelSettingsPath = ReadingHouseSceneBuilder.Root + "/UI/RH_SignIn_PanelSettings.asset";
        const string AppPanelSettingsGuid = "1b5b8df501a0dea4c908a7957dd48eee";
        const string LoginUxml = "Assets/Scripts/UI/Views/Login/LoginReal.uxml";
        const string RegisterUxml = "Assets/Scripts/UI/Views/Register/RegisterReal.uxml";
        const string AppSettingsPath = "Assets/Settings/AppSettings.asset";
        const int IgnoreRaycastLayer = 2;   // Unity built-in "Ignore Raycast": still collides with the student

        [MenuItem("Tools/sReader/Reading House/Add Walk-in And Sign-in To Scene")]
        public static void Apply()
        {
            var open = SceneManager.GetSceneByPath(ReadingHouseSceneBuilder.ScenePath);
            bool wasOpen = open.IsValid() && open.isLoaded;
            var scene = wasOpen ? open : EditorSceneManager.OpenScene(ReadingHouseSceneBuilder.ScenePath, OpenSceneMode.Additive);
            var previous = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(scene);
            try { Patch(scene); EditorSceneManager.SaveScene(scene); }
            finally
            {
                if (!wasOpen)
                {
                    if (previous.IsValid() && previous.isLoaded) SceneManager.SetActiveScene(previous);
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
            Debug.Log("[ReadingHouse] Walk-in & sign-in added to " + ReadingHouseSceneBuilder.ScenePath);
        }

        static void Patch(Scene scene)
        {
            GameObject Root(string n) => scene.GetRootGameObjects().First(g => g.name == n);
            var env = Root("Environment").transform;
            var dash = Root("Dashboard").transform;
            var chars = Root("Characters").transform;
            var presenter = scene.GetRootGameObjects().Select(g => g.GetComponentInChildren<ReadingHousePresenter>(true)).First(p => p != null);
            var input = presenter.GetComponent<ReadingHouseInput>();
            var rig = presenter.cameraRig;
            var cam = rig.viewCamera;
            var student = presenter.studentMover.student;

            // Replace anything a previous run added.
            foreach (var old in scene.GetRootGameObjects().Where(g => g.name == GroupName || g.name == "Sign-in Bootstrap")) Object.DestroyImmediate(old);
            foreach (var c in new Component[] { student.GetComponent<StudentWalker>(), student.GetComponent<CharacterController>(), cam.GetComponent<ReadingHouseFollowCamera>() })
                if (c != null) Object.DestroyImmediate(c);

            // 1. Colliders so the student is blocked by the house, dashboard and furniture.
            foreach (var root in new[] { env, dash, chars })
                foreach (var mr in root.GetComponentsInChildren<MeshRenderer>(true))
                {
                    if (mr.transform.IsChildOf(student) || mr.GetComponent<Collider>() != null) continue;
                    var mf = mr.GetComponent<MeshFilter>();
                    if (mf == null || mf.sharedMesh == null) continue;
                    mr.gameObject.AddComponent<MeshCollider>().sharedMesh = mf.sharedMesh;
                }

            var group = new GameObject(GroupName).transform;

            // 2. Front doors (closed until sign-in).
            var hingeL = ReadingHouseSceneBuilder.Find(env, "SR_Entry_Hinge_-1");
            var hingeR = ReadingHouseSceneBuilder.Find(env, "SR_Entry_Hinge_1");
            var houseCentre = ReadingHouseSceneBuilder.Find(dash, "SR_DASHBOARD_ROOT");
            var doorCentre = (hingeL.position + hingeR.position) * 0.5f;
            var outward = Vector3.ProjectOnPlane(doorCentre - houseCentre.position, Vector3.up).normalized;
            var doorGo = new GameObject("Front Door");
            doorGo.transform.SetParent(group, false);
            doorGo.transform.SetPositionAndRotation(doorCentre, Quaternion.LookRotation(outward, Vector3.up));
            var blocker = doorGo.AddComponent<BoxCollider>();
            doorGo.layer = IgnoreRaycastLayer;   // blocks walking, never a tap
            blocker.center = new Vector3(0f, 1.5f, 0f);
            blocker.size = new Vector3(Vector3.Distance(hingeL.position, hingeR.position) + 0.3f, 3.0f, 0.35f);
            var door = doorGo.AddComponent<HouseDoor>();
            door.leftHinge = hingeL; door.rightHinge = hingeR; door.student = student;
            door.doorwayBlocker = blocker; door.houseCentre = houseCentre;

            // 3. Walking student, joystick, follow camera.
            var cc = student.gameObject.AddComponent<CharacterController>();
            // The controller is scaled by the student root's own scale (1.16), so size it in WORLD metres:
            // 2.2 m tall keeps it under the door's fanlight bar (2.97 m above the floor).
            float rootScale = Mathf.Max(0.01f, student.lossyScale.y);
            cc.height = 2.2f / rootScale; cc.radius = 0.4f / rootScale; cc.center = new Vector3(0f, 1.12f / rootScale, 0f);
            cc.stepOffset = 0.35f / rootScale; cc.slopeLimit = 50f; cc.skinWidth = 0.04f / rootScale;
            var walker = student.gameObject.AddComponent<StudentWalker>();
            var joystick = new GameObject("Touch Joystick").AddComponent<TouchJoystick>();
            joystick.transform.SetParent(group, false);
            walker.input = joystick; walker.viewCamera = cam.transform;
            var follow = cam.gameObject.AddComponent<ReadingHouseFollowCamera>();
            follow.viewCamera = cam; follow.target = student; follow.viewDirection = -outward;
            cam.backgroundColor = new Color32(0xBF, 0xE6, 0xF7, 0xFF);   // sky
            cam.farClipPlane = 120f;

            // 4. Opening shot of the whole front, and the student's start spot beside the path.
            var right = Vector3.Cross(Vector3.up, -outward);   // camera-right while looking at the house
            // Raised and slightly to the side, so the roof surface shows as well as the whole front.
            // One field of view (40°) for every explore shot, so nothing ever appears to zoom.
            var camPos = Marker(group, "Exterior Camera", new Vector3(doorCentre.x, 0f, doorCentre.z) + outward * 13.0f + right * 3.4f + Vector3.up * 6.9f);
            var camAim = Marker(group, "Exterior Camera Aim", new Vector3(houseCentre.position.x, 2.0f, houseCentre.position.z) + outward * 2.2f);
            // Sign-in shot: the camera looks a little to the left of the house, so the house sits in the
            // right half of the screen and the card has calm space on the left. Camera only — the house never moves.
            var signPos = Marker(group, "Sign-in Camera", new Vector3(doorCentre.x, 0f, doorCentre.z) + outward * 11.5f - right * 1.2f + Vector3.up * 4.6f);
            var signAim = Marker(group, "Sign-in Camera Aim", new Vector3(doorCentre.x, 2.1f, doorCentre.z) - right * 4.4f);

            // Window glass: soften the Blender glow, which blows out to white under Unity's lighting.
            var glass = AssetDatabase.LoadAssetAtPath<Material>(ReadingHouseSceneBuilder.Root + "/Materials/SR_Amber_Glass.mat");
            if (glass != null)
            {
                glass.SetColor("_BaseColor", new Color32(0xF6, 0xE3, 0xB8, 0xFF));
                glass.SetColor("_EmissionColor", new Color(0.55f, 0.42f, 0.22f));
                glass.SetFloat("_Smoothness", 0.85f);
                EditorUtility.SetDirty(glass);
            }
            Physics.SyncTransforms();   // the colliders were just added

            // Invisible walls round the edge of the garden ground, so he can never walk off it.
            var groundBounds = ReadingHouseSceneBuilder.Find(env, "SR_Garden_Plinth").GetComponent<Renderer>().bounds;
            var edge = new GameObject("Ground Edge Walls").transform;
            edge.SetParent(group, false);
            float top = groundBounds.max.y, wallH = 3f, thick = 0.4f;
            Vector3 gc = groundBounds.center, e = groundBounds.extents;
            foreach (var (pos, size) in new[]
            {
                (new Vector3(gc.x, top + wallH / 2, groundBounds.max.z + thick / 2), new Vector3(e.x * 2 + 2 * thick, wallH, thick)),
                (new Vector3(gc.x, top + wallH / 2, groundBounds.min.z - thick / 2), new Vector3(e.x * 2 + 2 * thick, wallH, thick)),
                (new Vector3(groundBounds.max.x + thick / 2, top + wallH / 2, gc.z), new Vector3(thick, wallH, e.z * 2)),
                (new Vector3(groundBounds.min.x - thick / 2, top + wallH / 2, gc.z), new Vector3(thick, wallH, e.z * 2)),
            })
            {
                var w = new GameObject("Edge") { layer = IgnoreRaycastLayer }.AddComponent<BoxCollider>();
                w.transform.SetParent(edge, false);
                w.transform.position = pos;
                w.size = size;
            }
            // Front-yard fences from the house's front corners to the ground edges: the student stays in
            // the front yard (or inside), so the follow camera behind him is never inside the house.
            var houseBounds = ReadingHouseSceneBuilder.Find(env, "SR_Foundation").GetComponent<Renderer>().bounds;
            float frontZ = houseBounds.max.z - 0.3f;
            foreach (var (fromX, toX) in new[] { (houseBounds.max.x - 0.3f, groundBounds.max.x), (groundBounds.min.x, houseBounds.min.x + 0.3f) })
            {
                var w = new GameObject("Front Yard Fence") { layer = IgnoreRaycastLayer }.AddComponent<BoxCollider>();
                w.transform.SetParent(edge, false);
                w.transform.position = new Vector3((fromX + toX) / 2f, top + wallH / 2, frontZ);
                w.size = new Vector3(Mathf.Abs(toX - fromX), wallH, thick);
            }

            // Start beside the path, well inside the ground (checked: there must be floor under him).
            var startPos = doorCentre + outward * 3.0f - right * 2.8f;
            startPos.x = Mathf.Clamp(startPos.x, groundBounds.min.x + 1f, groundBounds.max.x - 1f);
            startPos.z = Mathf.Clamp(startPos.z, groundBounds.min.z + 1f, groundBounds.max.z - 1f);
            if (!Physics.Raycast(new Vector3(startPos.x, top + 6f, startPos.z), Vector3.down, out var ground, 20f))
                throw new InvalidOperationException($"No ground under the student's start spot {startPos} — check the garden mesh.");
            startPos.y = ground.point.y;
            var start = Marker(group, "Student Start (outside)", startPos);
            start.rotation = presenter.studentMover.overviewSpot.rotation;   // facing the house, like his overview spot

            // 5. Door panels: tap targets + runtime labels.
            var wallLabels = new System.Collections.Generic.List<TMP_Text>();
            foreach (var (kind, key, title, sub, button) in new[]
            {
                (DoorPanelTarget.Kind.Login, "LOGIN", "Log in", "Have an account?", "Sign in"),
                (DoorPanelTarget.Kind.Register, "REGISTER", "Register", "New here?", "Join"),
            })
            {
                foreach (var t in env.GetComponentsInChildren<Transform>(true).Where(t => t.name.StartsWith("SR_DOOR_PANEL_" + key) || t.name == "SR_DOOR_BUTTON_" + key))
                {
                    var target = t.gameObject.AddComponent<DoorPanelTarget>();
                    target.kind = kind;
                }
                wallLabels.Add(Label(group, ReadingHouseSceneBuilder.Find(env, $"SR_ANCHOR_{key}_LABEL"), $"<size=55%>{sub}</size>\n{title}", 0.13f, 0.66f, 0.42f, "#16233F", -outward));
                wallLabels.Add(Label(group, ReadingHouseSceneBuilder.Find(env, $"SR_ANCHOR_{key}_BUTTON"), button, 0.09f, 0.58f, 0.2f, "#FFFFFF", -outward));
            }

            // 6. Chalkboard text.
            var chalkAnchor = ReadingHouseSceneBuilder.Find(env, "SR_ANCHOR_CHALKBOARD");
            var slate = ReadingHouseSceneBuilder.Find(env, "SR_CHALKBOARD_Slate").GetComponent<Renderer>();
            var intoWall = Vector3.ProjectOnPlane(slate.bounds.center - chalkAnchor.position, Vector3.up).normalized;
            var chalkText = Label(group, chalkAnchor, "Today's reading", 0.12f, 1.8f, 1.0f, "#F2EFE6", intoWall);
            chalkText.alignment = TextAlignmentOptions.Top;
            chalkText.textWrappingMode = TextWrappingModes.Normal;
            chalkText.fontStyle = FontStyles.Normal;
            var chalk = chalkText.gameObject.AddComponent<ChalkboardView>();
            chalk.text = chalkText;
            presenter.chalkboard = chalk;

            // 7. The app's own sign-in pages + composition root, reused unchanged (inactive until the bootstrap decides).
            var signIn = new GameObject("Sign-in (app pages, reused)");
            signIn.transform.SetParent(group, false);
            signIn.SetActive(false);
            var es = new GameObject("EventSystem", typeof(EventSystem));
            es.transform.SetParent(signIn.transform, false);
            AddInputModule(es);
            var navGo = new GameObject("NavigationManager");
            navGo.transform.SetParent(signIn.transform, false);
            var nav = navGo.AddComponent<SReader.UI.Navigation.NavigationManager>();
            var outside = Child(signIn.transform, "Outside (landing)");
            var signedIn = Child(signIn.transform, "Signed In (opens the doors)");
            signedIn.SetActive(false);
            var unlock = signedIn.AddComponent<DoorUnlockPage>();
            unlock.door = door;
            var panel = SignInPanelSettings();
            var login = Page(signIn.transform, "Login", LoginUxml, panel, "LoginPageController", nav);
            var register = Page(signIn.transform, "Register", RegisterUxml, panel, "RegisterPageController", nav, "RegisterPageStyler");
            SetRefs(nav, ("landingPage", outside), ("loginPage", login.gameObject), ("registerPage", register.gameObject),
                ("profilePage", signedIn), ("studentHomePage", signedIn), ("guardianHomePage", signedIn), ("tutorHomePage", signedIn));
            var rootGo = Child(signIn.transform, "App Composition Root (reused)");
            var composition = rootGo.AddComponent<SReader.Core.DependencyInjection.AppCompositionRoot>();
            SetRefs(composition, ("settings", AssetDatabase.LoadAssetAtPath<Object>(AppSettingsPath)), ("loginView", login),
                ("registerView", register), ("navigation", nav));

            var boot = new GameObject("Sign-in Bootstrap").AddComponent<ReadingHouseAuthBootstrap>();
            boot.signInGroup = signIn; boot.door = door;

            // 8. Dollhouse cut-away: the front wall (everything in the front-wall slab, not the roof or the
            //    garden) fades out while the student is inside. Meshes are not moved or scaled.
            float innerZ = houseBounds.max.z - 0.75f;
            var skip = new[] { "Garden", "Path", "Step", "Planter", "Sage", "tree", "Foundation", "Plinth", "Moss", "Soil", "Skirting", "Plank" };
            var wallRenderers = env.GetComponentsInChildren<Renderer>(true)
                .Where(r => r.bounds.min.z > innerZ - 0.1f && r.bounds.max.y > 0.6f && !skip.Any(s => r.name.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0))
                .ToArray();
            var cut = new GameObject("Front Wall Cut-away").AddComponent<FrontWallCutaway>();
            cut.transform.SetParent(group, false);
            cut.renderers = wallRenderers;
            cut.labels = wallLabels.ToArray();
            cut.student = student;
            cut.doorway = doorGo.transform;
            cut.outward = outward;
            Debug.Log($"[ReadingHouse] Front-wall cut-away: {wallRenderers.Length} renderers.");
            // Keeps URP's transparent Lit variant in Android builds (the fade copies need it).
            KeepTransparentVariant(group);

            // 9. HUD (hint pill + veil) in the same UI Toolkit panel as the sign-in card, drawn beneath it.
            var hudGo = new GameObject("HUD (UI Toolkit)");
            hudGo.transform.SetParent(group, false);
            var hudDoc = hudGo.AddComponent<UIDocument>();
            hudDoc.panelSettings = SignInPanelSettings();
            hudDoc.sortingOrder = -10;
            var hud = hudGo.AddComponent<ReadingHouseHud>();

            follow.distance = 5.4f; follow.height = 3.0f; follow.lookHeight = 1.5f; follow.fieldOfView = 40f;

            // 10. The director ties it together.
            AssetDatabase.SaveAssets();
            var director = new GameObject("Walk-in Director").AddComponent<ReadingHouseExploreDirector>();
            director.transform.SetParent(group, false);
            director.presenter = presenter; director.cameraRig = rig; director.follow = follow; director.walker = walker;
            director.joystick = joystick; director.door = door; director.studentStart = start;
            director.exteriorCameraPos = camPos; director.exteriorCameraAim = camAim;
            director.signInCameraPos = signPos; director.signInCameraAim = signAim;
            director.hud = hud;
            director.signInNavigation = nav; director.loginPage = login.gameObject; director.registerPage = register.gameObject;
            unlock.director = director;
            input.director = director; input.joystick = joystick;
            EditorUtility.SetDirty(presenter); EditorUtility.SetDirty(input);
        }

        static void KeepTransparentVariant(Transform parent)
        {
            const string path = ReadingHouseSceneBuilder.Root + "/Materials/RH_Lit_Transparent_VariantKeeper.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.SetFloat("_Surface", 1f);
                mat.SetFloat("_Blend", 0f);
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = 3000;
                AssetDatabase.CreateAsset(mat, path);
            }
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = "Transparent Variant Keeper (hidden)";
            Object.DestroyImmediate(go.GetComponent<Collider>());
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(0f, -50f, 0f);
            var r = go.GetComponent<MeshRenderer>();
            r.sharedMaterial = mat;
            r.enabled = false;
        }

        static Transform Marker(Transform parent, string name, Vector3 pos)
        {
            var t = new GameObject(name).transform;
            t.SetParent(parent, false);
            t.position = pos;
            return t;
        }

        static GameObject Child(Transform parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go;
        }

        static TextMeshPro Label(Transform parent, Transform anchor, string text, float lineHeight, float width, float height, string hex, Vector3 forward)
        {
            var go = new GameObject("Label " + anchor.name.Replace("SR_ANCHOR_", ""), typeof(TextMeshPro));
            go.transform.SetParent(parent, false);
            go.transform.SetPositionAndRotation(anchor.position, Quaternion.LookRotation(forward, Vector3.up));
            var t = go.GetComponent<TextMeshPro>();
            t.text = text;
            t.alignment = TextAlignmentOptions.Center;
            t.textWrappingMode = TextWrappingModes.NoWrap;
            t.fontStyle = FontStyles.Bold;
            t.richText = true;
            t.color = ColorUtility.TryParseHtmlString(hex, out var c) ? c : Color.white;
            if (TMP_Settings.defaultFontAsset != null) t.font = TMP_Settings.defaultFontAsset;
            t.fontSize = 1f;
            float h = t.GetPreferredValues("Hg").y;
            t.fontSize = h > 1e-5f ? lineHeight / h : 1f;
            t.rectTransform.sizeDelta = new Vector2(width, height);
            return t;
        }

        static MonoBehaviour Page(Transform parent, string name, string uxmlPath, PanelSettings panel, string controllerType, Object navigation, string extraType = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.SetActive(false);
            var doc = go.AddComponent<UIDocument>();
            doc.panelSettings = panel;
            doc.visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            if (extraType != null) go.AddComponent(TypeNamed(extraType));
            var controller = (MonoBehaviour)go.AddComponent(TypeNamed(controllerType));
            SetRefs(controller, ("navigation", navigation));
            go.AddComponent<AuthCardLayout>();
            return controller;
        }

        static Type TypeNamed(string name) =>
            TypeCache.GetTypesDerivedFrom<MonoBehaviour>().First(t => t.Name == name && t.Assembly.GetName().Name == "Assembly-CSharp");

        static void SetRefs(Object target, params (string field, Object value)[] refs)
        {
            var so = new SerializedObject(target);
            foreach (var (field, value) in refs)
            {
                var p = so.FindProperty(field);
                if (p == null) throw new InvalidOperationException($"{target.GetType().Name} has no serialized field '{field}'.");
                p.objectReferenceValue = value;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void AddInputModule(GameObject es)
        {
            var module = TypeCache.GetTypesDerivedFrom<BaseInputModule>().FirstOrDefault(t => t.Name == "InputSystemUIInputModule");
            if (module != null) es.AddComponent(module);
        }

        /// <summary>The app's panel settings (same theme), rescaled for the 1520 × 720 landscape screen.</summary>
        static PanelSettings SignInPanelSettings()
        {
            var existing = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            if (existing != null) return existing;
            var app = AssetDatabase.LoadAssetAtPath<PanelSettings>(AssetDatabase.GUIDToAssetPath(AppPanelSettingsGuid));
            var copy = app != null ? Object.Instantiate(app) : ScriptableObject.CreateInstance<PanelSettings>();
            copy.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            copy.referenceResolution = new Vector2Int(1520, 720);
            copy.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            copy.match = 1f;
            copy.sortingOrder = 50;
            var folder = ReadingHouseSceneBuilder.Root + "/UI";
            if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder(ReadingHouseSceneBuilder.Root, "UI");
            AssetDatabase.CreateAsset(copy, PanelSettingsPath);
            return copy;
        }
    }
}
