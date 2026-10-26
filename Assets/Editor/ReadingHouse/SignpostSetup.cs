using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using SReader.Game.ReadingHouse;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// The signpost and garden bench in front of the Reading House (SC_ReadingHouse_Integration), as in the
    /// approved picture: a wooden post left of the path with a hanging lantern, a navy "sReader" arrow on
    /// chains, and four planks (Library · Play · My Progress · Settings) that are the house's menu: each is
    /// a <see cref="SignpostButton"/>. A bench sits on the lawn right of the path.
    ///
    /// Models: Assets/Art/ReadingHouse/Models/Signpost/SM_Signpost.fbx, SM_Bench.fbx (built by
    /// GeneratedAssets/Signpost/build_signpost.py). Words are TextMeshPro, icons are small unlit quads.
    /// Re-running replaces only what this adds.
    /// </summary>
    public static class SignpostSetup
    {
        const string ModelFolder = "Assets/Art/ReadingHouse/Models/Signpost";
        const string ArtFolder = "Assets/Art/ReadingHouse/Signpost";

        static readonly (string obj, SignpostButton.Kind kind, string label, string icon)[] Signs =
        {
            ("Sign_Header", SignpostButton.Kind.Header, "sReader", "T_Icon_Header"),
            ("Sign_Library", SignpostButton.Kind.Library, "Library", "T_Icon_Library"),
            ("Sign_Play", SignpostButton.Kind.Play, "Play", "T_Icon_Play"),
            ("Sign_Progress", SignpostButton.Kind.Progress, "My Progress", "T_Icon_Progress"),
            ("Sign_Settings", SignpostButton.Kind.Settings, "Settings", "T_Icon_Settings"),
        };

        [MenuItem("Tools/sReader/Reading House/Set Up Signpost + Bench")]
        public static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new System.InvalidOperationException("Stop Play mode first: the scene can only be changed in Edit mode.");

            var mats = Materials();
            ImportModel(ModelFolder + "/SM_Signpost.fbx", mats);
            ImportModel(ModelFolder + "/SM_Bench.fbx", mats);

            var open = SceneManager.GetSceneByPath(ReadingHouseSceneBuilder.ScenePath);
            bool wasOpen = open.IsValid() && open.isLoaded;
            var scene = wasOpen ? open : EditorSceneManager.OpenScene(ReadingHouseSceneBuilder.ScenePath, OpenSceneMode.Additive);
            var previousActive = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(scene);
            try
            {
                foreach (var old in scene.GetRootGameObjects().Where(g => g.name == "Signpost (menu)" || g.name == "Garden Bench").ToArray())
                    Object.DestroyImmediate(old);
                var all = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();
                var door = all.Select(t => t.GetComponent<HouseDoor>()).First(d => d != null);
                var director = all.Select(t => t.GetComponent<ReadingHouseExploreDirector>()).First(d => d != null);
                var presenter = all.Select(t => t.GetComponent<ReadingHousePresenter>()).First(p => p != null);
                var font = presenter.dashboard != null && presenter.dashboard.sentenceText != null ? presenter.dashboard.sentenceText.font : TMP_Settings.defaultFontAsset;

                // Where: from the opening shot, the post stands left of the path, the bench right of it.
                var outward = Vector3.ProjectOnPlane(door.transform.position - door.houseCentre.position, Vector3.up).normalized;
                var camPos = director.exteriorCameraPos.position;
                var view = Vector3.ProjectOnPlane(director.exteriorCameraAim.position - camPos, Vector3.up).normalized;
                var screenRight = Vector3.Cross(Vector3.up, view).normalized;
                float groundY = director.studentStart.position.y;

                // Right at the house, as in the picture: the post just in front of its left front corner (the planks reach
                // across towards the left window), the bench by the right front corner, turned towards the path.
                var house = all.Select(t => t.GetComponent<Renderer>()).First(r => r != null && r.name == "SR_Foundation").bounds;
                // The house's own axes (its front faces "outward"); "along" runs left → right across its front as seen from the path.
                var along = Vector3.Cross(Vector3.up, outward).normalized;
                if (Vector3.Dot(along, screenRight) < 0f) along = -along;
                float halfW = Mathf.Abs(house.extents.x * along.x) + Mathf.Abs(house.extents.z * along.z);
                float halfD = Mathf.Abs(house.extents.x * outward.x) + Mathf.Abs(house.extents.z * outward.z);
                var frontMid = new Vector3(house.center.x, groundY, house.center.z) + outward * halfD;
                var leftCorner = frontMid - along * halfW;
                var rightCorner = frontMid + along * halfW;
                var postAt = leftCorner + outward * 1.1f + along * 0.1f;
                // Facing straight out from the house front (parallel to the wall, as in the picture), so the planks never reach into it.
                var post = Place(ModelFolder + "/SM_Signpost.fbx", "Signpost (menu)", Ground(postAt, groundY), postAt + outward * 10f);
                // Mirror left-to-right, so the planks reach towards the house and the arrow points at the door (as in the
                // picture), the lantern on the outer side. The words and icons cancel the mirror (Decorate divides by lossyScale).
                post.transform.localScale = new Vector3(-1f, 1f, 1f);
                var benchAt = rightCorner + outward * 1.15f - along * 0.35f;
                // The bench turned three-quarters towards the path (as in the picture), not square to the camera.
                var pathPoint = door.transform.position + outward * 3.0f;
                var benchLook = benchAt + (Vector3.ProjectOnPlane(camPos - benchAt, Vector3.up).normalized + Vector3.ProjectOnPlane(pathPoint - benchAt, Vector3.up).normalized * 0.35f);   // facing the viewer, a little towards the path
                var bench = Place(ModelFolder + "/SM_Bench.fbx", "Garden Bench", Ground(benchAt, groundY), benchLook);
                // Make sure its seat (not its back) faces the way it should: turn it round if the export's front marker disagrees.
                var seat = bench.GetComponentsInChildren<Renderer>().FirstOrDefault(r => r.name == "Seat_0");
                var backrest = bench.GetComponentsInChildren<Renderer>().FirstOrDefault(r => r.name == "Back_1");
                if (seat != null && backrest != null &&
                    Vector3.Dot(seat.bounds.center - backrest.bounds.center, Vector3.ProjectOnPlane(benchLook - benchAt, Vector3.up)) < 0f)
                    bench.transform.RotateAround(bench.transform.position, Vector3.up, 180f);

                // Solid where it stands (the student walks round the post and the bench).
                AddBlocker(post.transform, "Post", 0.5f);
                AddBlocker(bench.transform, null, 1.0f);

                var front = Front(post.transform);
                foreach (var (objName, kind, label, icon) in Signs)
                {
                    var sign = post.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == objName);
                    if (sign == null) { Debug.LogWarning($"[Signpost] {objName} missing in the model."); continue; }
                    var col = sign.gameObject.AddComponent<BoxCollider>();
                    var button = sign.gameObject.AddComponent<SignpostButton>();
                    button.kind = kind;
                    button.hanging = kind == SignpostButton.Kind.Header;
                    Decorate(sign, col, front, label, icon, font, kind == SignpostButton.Kind.Header, mats.icon);
                }

                // The lantern glows (a soft camera-facing halo; no real-time light).
                var glass = post.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "Lantern_Glass");
                if (glass != null) Glow(glass, mats.glow, 1.3f);

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"[Signpost] Signpost at {post.transform.position}, bench at {bench.transform.position}.");
            }
            finally
            {
                if (previousActive.IsValid() && previousActive != scene) SceneManager.SetActiveScene(previousActive);
                if (!wasOpen) EditorSceneManager.CloseScene(scene, true);
            }
        }

        struct Mats { public Material wood, woodDark, navy, gold, iron, glass, icon, glow, leaf; }

        static Mats Materials()
        {
            var lit = Shader.Find("Universal Render Pipeline/Simple Lit");
            var unlit = Shader.Find("Universal Render Pipeline/Unlit");
            var woodTex = Tex(ArtFolder + "/T_PlankWood.png", true);
            Material M(string name, Shader sh, Color c, Texture t = null, float smooth = 0.15f)
            {
                string path = ArtFolder + "/" + name + ".mat";
                var m = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (m == null) { m = new Material(sh); AssetDatabase.CreateAsset(m, path); }
                m.shader = sh;
                m.SetColor("_BaseColor", c);
                if (t != null) { m.SetTexture("_BaseMap", t); m.mainTexture = t; }
                if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smooth);
                EditorUtility.SetDirty(m);
                return m;
            }
            var gold = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/ReadingHouse/Lighting/Materials/M_RH_SR_Gold.mat")
                       ?? M("M_Sign_Gold", lit, new Color32(240, 186, 82, 255), null, 0.55f);
            var icon = M("M_Sign_Icon", unlit, Color.white);
            Transparent(icon);
            var glow = M("M_Sign_LanternGlow", unlit, new Color(1f, 0.85f, 0.6f, 0.85f), Tex(ArtFolder + "/T_LanternGlow.png", false));
            Transparent(glow, additive: true);
            return new Mats
            {
                wood = M("M_Sign_Wood", lit, Color.white, woodTex),
                woodDark = M("M_Sign_WoodDark", lit, new Color(0.62f, 0.52f, 0.46f), woodTex),
                leaf = M("M_Sign_Leaf", lit, new Color32(78, 148, 52, 255), null, 0.2f),
                navy = M("M_Sign_Navy", lit, new Color32(24, 40, 86, 255), null, 0.35f),
                gold = gold,
                iron = M("M_Sign_Iron", lit, new Color32(38, 36, 40, 255), null, 0.3f),
                glass = M("M_Sign_LanternGlass", unlit, new Color32(255, 214, 140, 255)),
                icon = icon,
                glow = glow,
            };
        }

        static Texture2D Tex(string path, bool repeat)
        {
            AssetDatabase.ImportAsset(path);
            var imp = (TextureImporter)AssetImporter.GetAtPath(path);
            imp.wrapMode = repeat ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
            imp.alphaIsTransparency = !repeat;
            imp.SetPlatformTextureSettings(new TextureImporterPlatformSettings { name = "Android", overridden = true, maxTextureSize = 512, format = TextureImporterFormat.ASTC_6x6 });
            imp.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        static void Transparent(Material m, bool additive = false)
        {
            m.SetFloat("_Surface", 1f);
            m.SetFloat("_Blend", additive ? 2f : 0f);
            m.SetFloat("_SrcBlend", (float)(additive ? UnityEngine.Rendering.BlendMode.One : UnityEngine.Rendering.BlendMode.SrcAlpha));
            m.SetFloat("_DstBlend", (float)(additive ? UnityEngine.Rendering.BlendMode.One : UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha));
            m.SetFloat("_ZWrite", 0f);
            m.SetOverrideTag("RenderType", "Transparent");
            m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            EditorUtility.SetDirty(m);
        }

        static void ImportModel(string path, Mats mats)
        {
            var imp = (ModelImporter)AssetImporter.GetAtPath(path);
            imp.globalScale = 1f;
            imp.useFileScale = true;
            imp.importCameras = false; imp.importLights = false;
            imp.importAnimation = false;
            imp.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            void Map(string name, Material m) => imp.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material), name), m);
            Map("Wood", mats.wood); Map("WoodDark", mats.woodDark); Map("Navy", mats.navy);
            Map("Gold", mats.gold); Map("Iron", mats.iron); Map("Glass", mats.glass); Map("Leaf", mats.leaf);
            imp.SaveAndReimport();
        }

        static Vector3 Ground(Vector3 p, float fallback)
        {
            Physics.SyncTransforms();
            p.y = fallback;
            foreach (var h in Physics.RaycastAll(p + Vector3.up * 5f, Vector3.down, 12f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore).OrderBy(h => h.distance))
            { p.y = h.point.y; break; }
            return p;
        }

        /// <summary>Instantiate a model at a spot, turned so its front faces the opening camera.</summary>
        static GameObject Place(string path, string name, Vector3 at, Vector3 lookFrom)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            go.name = name;
            go.transform.position = at;
            var want = Vector3.ProjectOnPlane(lookFrom - at, Vector3.up).normalized;
            var have = Front(go.transform);
            go.transform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(have, want, Vector3.up), Vector3.up) * go.transform.rotation;   // a turn about the vertical only
            foreach (var r in go.GetComponentsInChildren<Renderer>()) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            return go;
        }

        static Vector3 Front(Transform root)
        {
            var marker = root.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "FrontMarker");
            var d = marker != null ? Vector3.ProjectOnPlane(marker.position - root.position, Vector3.up) : -root.forward;
            return d.sqrMagnitude > 1e-6f ? d.normalized : Vector3.forward;
        }

        static void AddBlocker(Transform root, string partName, float radius)
        {
            var b = new Bounds(root.position, Vector3.zero); bool any = false;
            foreach (var r in root.GetComponentsInChildren<Renderer>())
                if (partName == null || r.name == partName) { if (any) b.Encapsulate(r.bounds); else { b = r.bounds; any = true; } }
            var go = new GameObject("Blocker") { layer = 2 };                     // blocks walking, never a tap
            go.transform.SetParent(root, false);
            go.transform.rotation = Quaternion.identity;
            var col = go.AddComponent<BoxCollider>();
            col.center = go.transform.InverseTransformPoint(b.center);
            var s = go.transform.lossyScale;
            col.size = new Vector3(Mathf.Max(b.size.x, radius) / s.x, b.size.y / s.y, Mathf.Max(b.size.z, radius) / s.z);
        }

        /// <summary>The sign's icon and word, just in front of its face, read left to right from the path.</summary>
        static void Decorate(Transform sign, BoxCollider box, Vector3 front, string label, string iconName, TMP_FontAsset font, bool header, Material iconMat)
        {
            var viewer = -front;                                                   // the direction a reader looks
            var right = Vector3.Cross(Vector3.up, viewer).normalized;
            // The board's own box (tight, in its local axes), measured along the reader's right / up / front.
            float minR = float.MaxValue, maxR = float.MinValue, minU = float.MaxValue, maxU = float.MinValue, maxF = float.MinValue;
            for (int i = 0; i < 8; i++)
            {
                var c = box.center + Vector3.Scale(box.size * 0.5f, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                var w = sign.TransformPoint(c);
                minR = Mathf.Min(minR, Vector3.Dot(w, right)); maxR = Mathf.Max(maxR, Vector3.Dot(w, right));
                minU = Mathf.Min(minU, w.y); maxU = Mathf.Max(maxU, w.y);
                maxF = Mathf.Max(maxF, Vector3.Dot(w, front));
            }
            var centre = sign.TransformPoint(box.center);
            float width = maxR - minR, height = maxU - minU;
            var face = centre + front * (maxF - Vector3.Dot(centre, front) + 0.012f) + right * ((minR + maxR) * 0.5f - Vector3.Dot(centre, right));
            if (header) face -= right * width * 0.06f;                             // the arrow's point takes the right end
            float iconSize = height * (header ? 0.78f : 0.72f);
            var iconAt = face - right * (width * 0.5f - iconSize * 0.5f - width * (header ? 0.07f : 0.06f));
            var rot = Quaternion.LookRotation(viewer, Vector3.up);

            var icon = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Object.DestroyImmediate(icon.GetComponent<Collider>());
            icon.name = "Icon";
            icon.transform.SetParent(sign, true);
            icon.transform.SetPositionAndRotation(iconAt, rot);
            icon.transform.localScale = Vector3.one;
            var ls = icon.transform.lossyScale;
            icon.transform.localScale = new Vector3(iconSize / ls.x, iconSize / ls.y, 1f / ls.z);
            var mr = icon.GetComponent<MeshRenderer>();
            var m = new Material(iconMat) { name = "M_Sign_" + iconName };
            var tex = Tex(ArtFolder + "/" + iconName + ".png", false);
            m.SetTexture("_BaseMap", tex); m.mainTexture = tex;
            string mpath = ArtFolder + "/M_" + iconName + ".mat";
            AssetDatabase.DeleteAsset(mpath); AssetDatabase.CreateAsset(m, mpath);
            mr.sharedMaterial = m;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            var textGo = new GameObject("Label");
            textGo.transform.SetParent(sign, true);
            float textLeft = width * 0.5f - (iconSize + width * (header ? 0.12f : 0.1f));
            float textWidth = width * (header ? 0.62f : 0.66f);
            textGo.transform.SetPositionAndRotation(face - right * (textLeft - textWidth * 0.5f), rot);
            textGo.transform.localScale = Vector3.one;
            var tls = textGo.transform.lossyScale;
            textGo.transform.localScale = new Vector3(1f / tls.x, 1f / tls.y, 1f / tls.z);
            var tmp = textGo.AddComponent<TextMeshPro>();
            tmp.font = font;
            tmp.text = label;
            tmp.fontStyle = FontStyles.Bold;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 1f; tmp.fontSizeMax = header ? 7f : 5.5f;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.rectTransform.sizeDelta = new Vector2(textWidth, height * 0.8f);
            tmp.color = header ? new Color32(255, 240, 205, 255) : new Color32(255, 246, 226, 255);
            tmp.fontSharedMaterial = OutlinePreset(font, header ? "Header" : "Plank", header ? new Color32(10, 20, 50, 255) : new Color32(70, 38, 14, 255));
            tmp.enableWordWrapping = false;
        }

        /// <summary>A shared font material with a dark outline (setting outline on the label itself would copy the material per label).</summary>
        static Material OutlinePreset(TMP_FontAsset font, string name, Color outline)
        {
            string path = ArtFolder + "/M_SignText_" + name + ".mat";
            var m = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (m == null) { m = new Material(font.material); AssetDatabase.CreateAsset(m, path); }
            m.CopyPropertiesFromMaterial(font.material);
            m.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.22f);
            m.SetColor(ShaderUtilities.ID_OutlineColor, outline);
            m.SetFloat(ShaderUtilities.ID_FaceDilate, 0.1f);
            m.EnableKeyword(ShaderUtilities.Keyword_Outline);
            EditorUtility.SetDirty(m);
            return m;
        }

        static void Glow(Transform at, Material glow, float size)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Object.DestroyImmediate(go.GetComponent<Collider>());
            go.name = "Lantern Glow";
            go.transform.SetParent(at, false);
            go.transform.localPosition = Vector3.zero;
            var ls = at.lossyScale;
            go.transform.localScale = new Vector3(size / ls.x, size / ls.y, size / ls.z);
            var mr = go.GetComponent<MeshRenderer>();
            mr.sharedMaterial = glow;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            go.AddComponent<FaceCamera>();
        }
    }
}
