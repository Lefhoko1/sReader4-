using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;
namespace SReader.EditorTools.ReadingHouse
{
    public static partial class ReadingHouseLightingPolish
    {
        static Color Hex(string value) { ColorUtility.TryParseHtmlString("#" + value, out var color); return color; }
        static string HexOf(Color c) => "#" + ColorUtility.ToHtmlStringRGBA(c);
        static GameObject LightingRoot()
        {
            return House().GetRootGameObjects().FirstOrDefault(g => g.name == "Reading House Lighting") ?? new GameObject("Reading House Lighting");
        }
        static partial void RunPolish(string cmd)
        {
            if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play mode before changing visual assets");
            House();
            switch (cmd) {
                case "palette": Palette(); break;
                case "lighting": Lighting(); break;
                case "tune": TuneLighting(); break;
                case "cleanup": CleanupPreview(); break;
                case "isolate": IsolateRenderer(); GoldKeyMaterial(); break;
                case "keymatte": MatteKey(); break;
                case "buildreport": BuildMessages(); break;
                case "mobilebake": Lightmapping.lightingSettings.directionalityMode = LightmapsMode.NonDirectional; EditorUtility.SetDirty(Lightmapping.lightingSettings); AssetDatabase.SaveAssets(); break;
                case "bake":
                    Lightmapping.bakeCompleted -= BakeCompleted; Lightmapping.bakeCompleted += BakeCompleted;
                    if (!Lightmapping.BakeAsync()) throw new InvalidOperationException("Unity did not start the bake");
                    break;
                case "finish": FinishBake(); break;
                case "validate": Validate(); break;
                case "android": ReadingHouseAndroidBuild.Build(); break;
                default: throw new ArgumentException("Unknown visual tool command: " + cmd);
            }
        }
        static void Palette()
        {
            Directory.CreateDirectory(Art + "/Materials"); AssetDatabase.Refresh();
            var changes = new List<string> { "source\told base\tnew base\told smoothness\tnew smoothness\tnew metallic\told emission\tnew emission\tnew asset" };
            // Physical surfaces only. Display RT materials, sentence/bubble controllers and sprite art are excluded.
            var specs = new Dictionary<string, (string color, float smooth, float metal, string emission, float strength)> {
                ["SR_MAT_Wall_Interior_Warm"] = ("EAD0AA", .16f, 0, "000000", 0),
                ["SR_MAT_Ceiling_Warm"] = ("F1DEBB", .12f, 0, "000000", 0),
                ["SR_MAT_Housing_Cream"] = ("E9DABC", .32f, 0, "000000", 0),
                ["SR_MAT_Plastic_Cyan"] = ("66BEB9", .30f, .03f, "57BCB6", .13f),
                ["SR_MAT_Plastic_Orange"] = ("CD914B", .36f, .20f, "000000", 0),
                ["SR_MAT_Trim_Gold"] = ("E1B765", .44f, .28f, "000000", 0),
                ["SR_MAT_Plastic_Navy"] = ("283F68", .29f, .02f, "000000", 0),
                ["SR_MAT_Plastic_DeepBlue"] = ("416F94", .30f, .02f, "000000", 0),
                ["SR_MAT_Plastic_Purple"] = ("70529F", .34f, .02f, "000000", 0),
                ["SR_MAT_Plastic_PurpleHi"] = ("9B7FC2", .38f, .02f, "000000", 0),
                ["SR_MAT_Light_Warm"] = ("FFE3A0", .30f, 0, "FFE0A0", 2.2f),
                ["SR_REF_light"] = ("FFE6AC", .28f, 0, "FFE1A2", 2.0f),
                ["SR_Amber_Glass"] = ("FFE6B4", .34f, .02f, "FFE2B3", .8f),
                ["SR_Floor"] = ("AA805D", .20f, 0, "000000", 0),
                ["SR_Oak"] = ("AC8457", .24f, 0, "000000", 0),
                ["SR_Walnut"] = ("886346", .23f, 0, "000000", 0),
                ["SR_REF_brown"] = ("98704F", .23f, 0, "000000", 0),
                ["SR_REF_rugblue"] = ("387C9D", .08f, 0, "000000", 0),
                ["SR_REF_golden"] = ("DDB669", .36f, .16f, "000000", 0),
                ["SR_Gold"] = ("CFA45D", .40f, .30f, "000000", 0),
                ["SR_Limestone"] = ("C2B59D", .14f, 0, "000000", 0),
                ["SR_Carved_Limestone"] = ("D8C7A5", .16f, 0, "000000", 0),
                ["SR_MAT_Chalkboard"] = ("294A48", .10f, 0, "000000", 0),
                ["SR_Slate_Light"] = ("7A91A2", .17f, 0, "000000", 0),
                ["SR_Slate"] = ("48596F", .18f, 0, "000000", 0),
                ["SR_Moss"] = ("587649", .10f, 0, "000000", 0),
                ["SR_Sage_Leaves"] = ("78935B", .10f, 0, "000000", 0),
                ["SR_Garden_Soil"] = ("736444", .10f, 0, "000000", 0),
                ["SR_MAT_Moss"] = ("75A154", .10f, 0, "000000", 0),
            };
            var replacements = new Dictionary<string, Material>();
            var source = All<Renderer>().SelectMany(r => r.sharedMaterials).Where(m => m != null && specs.ContainsKey(m.name)).GroupBy(m => m.name);
            foreach (var group in source) {
                var old = group.OrderByDescending(m => m.shader.name == "Universal Render Pipeline/Lit").First(); var spec = specs[group.Key];
                var path = Art + "/Materials/M_RH_" + group.Key + ".mat";
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat == null) { mat = new Material(old) { name = "M_RH_" + group.Key }; AssetDatabase.CreateAsset(mat, path); }
                mat.shader = Shader.Find("Universal Render Pipeline/Lit");
                var emission = Hex(spec.emission) * spec.strength; emission.a = 1;
                mat.SetColor("_BaseColor", Hex(spec.color)); mat.SetColor("_Color", Hex(spec.color));
                mat.SetFloat("_Smoothness", spec.smooth); mat.SetFloat("_Metallic", spec.metal); mat.SetColor("_EmissionColor", emission);
                if (spec.strength > 0) { mat.EnableKeyword("_EMISSION"); mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive; }
                else { mat.DisableKeyword("_EMISSION"); mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack; }
                EditorUtility.SetDirty(mat); replacements[group.Key] = mat;
                changes.Add($"{group.Key}\t{(old.HasProperty("_BaseColor") ? HexOf(old.GetColor("_BaseColor")) : "embedded 2D shader")}\t#{spec.color}\t{(old.HasProperty("_Smoothness") ? old.GetFloat("_Smoothness").ToString(System.Globalization.CultureInfo.InvariantCulture) : "-")}\t{spec.smooth.ToString(System.Globalization.CultureInfo.InvariantCulture)}\t{spec.metal.ToString(System.Globalization.CultureInfo.InvariantCulture)}\t{(old.HasProperty("_EmissionColor") ? old.GetColor("_EmissionColor").ToString() : "-")}\t{emission}\t{path}");
            }
            foreach (var r in All<Renderer>()) {
                if (r.transform.root.name != "Environment" && r.transform.root.name != "Dashboard") continue;
                var mats = r.sharedMaterials; bool changed = false;
                for (int i = 0; i < mats.Length; i++) if (mats[i] != null && replacements.TryGetValue(mats[i].name, out var replacement)) { mats[i] = replacement; changed = true; }
                if (changed) { r.sharedMaterials = mats; PrefabUtility.RecordPrefabInstancePropertyModifications(r); }
            }
            File.WriteAllLines(Output + "/materials.tsv", changes);
            // Reuse the existing soft contact texture; no character art is altered.
            var shadow = new Material(Shader.Find("Universal Render Pipeline/Unlit")) { name = "M_RH_SeatedContactShadow" };
            shadow.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/ReadingHouse/Student2D/T_StudentShadow.png"));
            shadow.SetColor("_BaseColor", new Color(.32f, .23f, .18f, .64f));
            shadow.SetFloat("_Surface", 1); shadow.SetFloat("_Blend", 0); shadow.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha); shadow.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha); shadow.SetFloat("_ZWrite", 0);
            shadow.SetOverrideTag("RenderType", "Transparent"); shadow.EnableKeyword("_SURFACE_TYPE_TRANSPARENT"); shadow.renderQueue = 2990;
            AssetDatabase.CreateAsset(shadow, Art + "/Materials/M_RH_SeatedContactShadow.mat");
            foreach (var actor in All<SReader.Game.ReadingHouse.ActorSpriteAnimator>()) {
                if (actor.shadow != null) continue;
                var go = GameObject.CreatePrimitive(PrimitiveType.Quad); Object.DestroyImmediate(go.GetComponent<Collider>());
                go.name = actor.name + " Contact Shadow";
                go.transform.SetParent(LightingRoot().transform, false);
                var pos = actor.transform.position; pos.y = .342f;
                go.transform.SetPositionAndRotation(pos, Quaternion.Euler(90, 0, 0)); go.transform.localScale = new Vector3(.88f, .62f, 1);
                var r = go.GetComponent<MeshRenderer>(); r.sharedMaterial = shadow; r.shadowCastingMode = ShadowCastingMode.Off; r.receiveShadows = false;
            }
            AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(House());
        }
        static void Lighting()
        {
            var root = LightingRoot();
            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, Art + "/VP_ReadingHouse_SunnyLibrary.asset");
            T Add<T>() where T : VolumeComponent { var c = profile.Add<T>(false); AssetDatabase.AddObjectToAsset(c, profile); return c; }
            Add<Tonemapping>().mode.Override(TonemappingMode.Neutral);
            var wb = Add<WhiteBalance>(); wb.temperature.Override(4); wb.tint.Override(1);
            var grade = Add<ColorAdjustments>(); grade.postExposure.Override(.10f); grade.contrast.Override(5); grade.saturation.Override(5);
            var bloom = Add<Bloom>(); bloom.threshold.Override(1.5f); bloom.intensity.Override(.12f); bloom.scatter.Override(.42f); bloom.clamp.Override(4); bloom.highQualityFiltering.Override(false); bloom.downscale.Override(BloomDownscaleMode.Quarter); bloom.maxIterations.Override(2);
            var vignette = Add<Vignette>(); vignette.intensity.Override(.10f); vignette.smoothness.Override(.4f); vignette.color.Override(Hex("483B56"));
            var volume = root.AddComponent<Volume>(); volume.isGlobal = true; volume.priority = 5; volume.sharedProfile = profile;
            var cam = All<SReader.Game.ReadingHouse.ReadingHouseCameraRig>().Single().viewCamera;
            var data = cam.GetUniversalAdditionalCameraData(); data.renderPostProcessing = true; data.renderShadows = true; data.requiresDepthOption = CameraOverrideOption.Off; data.requiresColorOption = CameraOverrideOption.Off;
            cam.allowHDR = true;
            var renderer = Object.Instantiate(AssetDatabase.LoadAssetAtPath<UniversalRendererData>("Assets/Settings/Renderer3D.asset"));
            renderer.name = "Renderer_ReadingHouse_Mobile";
            renderer.rendererFeatures.Clear();
            AssetDatabase.CreateAsset(renderer, Art + "/Renderer_ReadingHouse_Mobile.asset");
            var rendererSO = new SerializedObject(renderer);
            rendererSO.FindProperty("postProcessData").objectReferenceValue = AssetDatabase.LoadAssetAtPath<PostProcessData>("Packages/com.unity.render-pipelines.universal/Runtime/Data/PostProcessData.asset");
            rendererSO.FindProperty("m_IntermediateTextureMode").intValue = 0;
            rendererSO.ApplyModifiedPropertiesWithoutUndo(); renderer.SetDirty();
            // Preserve all existing pipeline settings except the house renderer and legacy probe support.
            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/UniversalRP.asset");
            var pipelineSO = new SerializedObject(pipeline);
            var rendererList = pipelineSO.FindProperty("m_RendererDataList"); if (rendererList.arraySize < 3) rendererList.arraySize = 3; rendererList.GetArrayElementAtIndex(2).objectReferenceValue = renderer; data.SetRenderer(2);
            pipelineSO.FindProperty("m_LightProbeSystem").intValue = 0;
            pipelineSO.ApplyModifiedPropertiesWithoutUndo();
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = Hex("CDDDE7"); RenderSettings.ambientEquatorColor = Hex("B5BAB2"); RenderSettings.ambientGroundColor = Hex("81725D");
            RenderSettings.ambientIntensity = 1; RenderSettings.reflectionIntensity = .35f;
            var lights = All<Light>();
            foreach (var l in lights) {
                if (l.type == LightType.Directional) {
                    l.transform.rotation = Quaternion.Euler(42, -150, 0); l.color = Hex("FFF0D4"); l.intensity = 1.1f;
                    l.lightmapBakeType = LightmapBakeType.Baked; l.shadows = LightShadows.Soft; l.shadowStrength = .65f; l.shadowBias = .04f; l.shadowNormalBias = .25f; RenderSettings.sun = l;
                } else { l.lightmapBakeType = LightmapBakeType.Baked; l.color = Hex("FFD69C"); l.intensity = 2.0f; l.range = 4.5f; l.shadows = LightShadows.Soft; l.shapeRadius = .35f; }
            }
            // Large, baked window lights provide soft indoor daylight without runtime passes.
            foreach (var window in All<MeshRenderer>().Where(r => r.name.Contains("Side_Window") && r.name.EndsWith("glazing")).ToArray()) {
                window.shadowCastingMode = ShadowCastingMode.Off;
                var go = new GameObject("Baked daylight " + window.name); go.transform.SetParent(root.transform, false);
                var b = window.bounds; var inward = Vector3.ProjectOnPlane(new Vector3(0, b.center.y, 0) - b.center, Vector3.up).normalized;
                go.transform.SetPositionAndRotation(b.center + inward * .12f, Quaternion.LookRotation(inward, Vector3.up));
                var l = go.AddComponent<Light>(); l.type = LightType.Rectangle; l.lightmapBakeType = LightmapBakeType.Baked; l.areaSize = new Vector2(.9f, 1.8f); l.color = Hex("FFE8C5"); l.intensity = 1.8f; l.range = 7; l.shadows = LightShadows.Soft;
            }
            var probesGO = new GameObject("Light Probes - floor rug chairs"); probesGO.transform.SetParent(root.transform, false);
            var probes = probesGO.AddComponent<LightProbeGroup>(); var positions = new List<Vector3>();
            foreach (float y in new[] { .52f, 1.35f, 2.4f }) foreach (float x in new[] { -3.5f, -1.8f, 0, 1.8f, 3.5f }) foreach (float z in new[] { -3f, -1.4f, .4f, 2.6f }) positions.Add(new Vector3(x, y, z));
            foreach (float y in new[] { .52f, 1.7f }) foreach (float x in new[] { -4f, 0, 4f }) foreach (float z in new[] { 4.8f, 8f, 11f }) positions.Add(new Vector3(x, y, z));
            probes.probePositions = positions.ToArray();
            var reflectionGO = new GameObject("Baked Room Reflection"); reflectionGO.transform.SetParent(root.transform, false); reflectionGO.transform.position = new Vector3(0, 1.8f, 0);
            var reflection = reflectionGO.AddComponent<ReflectionProbe>(); reflection.mode = ReflectionProbeMode.Baked; reflection.resolution = 64; reflection.size = new Vector3(8.5f, 4.5f, 7.5f); reflection.center = new Vector3(0, .1f, 0); reflection.intensity = .45f; reflection.boxProjection = false; reflection.blendDistance = .5f; reflection.clearFlags = ReflectionProbeClearFlags.SolidColor; reflection.backgroundColor = Hex("D3C8AF"); reflection.hdr = false; reflection.cullingMask = ~0;
            // Only enable secondary UV generation on the two static model importers. Character meshes/art stay untouched.
            foreach (string model in new[] { "SM_ReadingHouse_Environment", "SM_ReadingHouse_Dashboard" }) {
                var importer = (ModelImporter)AssetImporter.GetAtPath("Assets/Art/ReadingHouse/Models/" + model + ".fbx");
                importer.generateSecondaryUV = true; importer.secondaryUVMinLightmapResolution = 10; importer.secondaryUVMinObjectScale = 1; importer.SaveAndReimport();
            }
            int baked = 0, probeLit = 0;
            var cutaways = All<SReader.Game.ReadingHouse.FrontWallCutaway>().SelectMany(c => c.renderers).ToHashSet();
            foreach (var r in All<MeshRenderer>()) {
                if (r.transform.root.name != "Environment" && r.transform.root.name != "Dashboard") continue;
                if (r.name.Contains("glazing")) { r.shadowCastingMode = ShadowCastingMode.Off; continue; }
                if (cutaways.Contains(r) || r.name.Contains("DISPLAY") || r.name.Contains("Door_Leaf")) continue;
                var mf = r.GetComponent<MeshFilter>(); if (mf == null || mf.sharedMesh == null) continue;
                var size = r.bounds.size;
                bool large = Mathf.Max(size.x, size.y, size.z) > .75f && size.x * size.y + size.y * size.z + size.z * size.x > .6f;
                r.receiveGI = large ? ReceiveGI.Lightmaps : ReceiveGI.LightProbes;
                r.lightProbeUsage = LightProbeUsage.BlendProbes;
                GameObjectUtility.SetStaticEditorFlags(r.gameObject, GameObjectUtility.GetStaticEditorFlags(r.gameObject) | StaticEditorFlags.ContributeGI);
                r.scaleInLightmap = large ? (Mathf.Max(size.x, size.y, size.z) > 3 ? 1 : .6f) : 0;
                if (large) baked++; else probeLit++;
                PrefabUtility.RecordPrefabInstancePropertyModifications(r);
                PrefabUtility.RecordPrefabInstancePropertyModifications(r.gameObject);
            }
            var settings = new LightingSettings { name = "LS_ReadingHouse_Mobile_Baked" };
            settings.directionalityMode = LightmapsMode.NonDirectional; settings.bakedGI = true; settings.realtimeGI = false; settings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;
            settings.lightmapResolution = 10; settings.lightmapMaxSize = 1024; settings.lightmapPadding = 3;
            settings.directSampleCount = 32; settings.indirectSampleCount = 128; settings.environmentSampleCount = 64; settings.maxBounces = 3;
            settings.mixedBakeMode = MixedLightingMode.Shadowmask;
            settings.ao = true; settings.aoMaxDistance = .35f; settings.aoExponentIndirect = .65f; settings.aoExponentDirect = 0;
            settings.lightmapCompression = LightmapCompression.HighQuality; settings.lightProbeSampleCountMultiplier = 2;
            AssetDatabase.CreateAsset(settings, Art + "/LS_ReadingHouse_Mobile_Baked.lighting"); Lightmapping.lightingSettings = settings;
            LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
            File.WriteAllText(Output + "/lighting-setup.txt", $"Baked lightmap renderers={baked}; probe-lit small renderers={probeLit}; probes={positions.Count}; baked window lights={All<Light>().Count(l => l.type == LightType.Rectangle)}; GPU={SystemInfo.graphicsDeviceName}\n");
            AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(House());
        }
        static void BakeCompleted() { Lightmapping.bakeCompleted -= BakeCompleted; FinishBake(); }
        static void FinishBake()
        {
            AssetDatabase.Refresh();
            // Unity initially emits bake products beside the scene. Move them into the reviewable Lighting folder.
            const string bakeFolder = "Assets/Scenes/SC_ReadingHouse_Integration";
            if (AssetDatabase.IsValidFolder(bakeFolder)) {
                foreach (var guid in AssetDatabase.FindAssets("", new[] { bakeFolder })) {
                    var path = AssetDatabase.GUIDToAssetPath(guid); if (AssetDatabase.IsValidFolder(path)) continue;
                    string destination = Art + "/" + Path.GetFileName(path);
                    if (path != destination) {
                        if (File.Exists(destination)) destination = AssetDatabase.GenerateUniqueAssetPath(destination);
                        var error = AssetDatabase.MoveAsset(path, destination); if (!string.IsNullOrEmpty(error)) Debug.LogWarning(error);
                    }
                }
            }
            BakeRoomCubemap();
            AssetDatabase.Refresh();
            foreach (string path in Directory.GetFiles(Art, "*", SearchOption.AllDirectories).Where(p => p.EndsWith(".exr") || p.EndsWith(".png"))) {
                var importer = AssetImporter.GetAtPath(path.Replace('\\', '/')) as TextureImporter;
                if (importer == null) continue;
                var platform = importer.GetPlatformTextureSettings("Android"); platform.overridden = true; platform.maxTextureSize = 1024; platform.format = TextureImporterFormat.ASTC_6x6; platform.compressionQuality = 70;
                importer.SetPlatformTextureSettings(platform); importer.SaveAndReimport();
            }
            AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(House());
            var maps = LightmapSettings.lightmaps;
            File.WriteAllLines(Output + "/bake-result.txt", new[] { "lightmaps=" + maps.Length, "lightingData=" + AssetDatabase.GetAssetPath(Lightmapping.lightingDataAsset), "probes=" + (LightmapSettings.lightProbes == null ? 0 : LightmapSettings.lightProbes.count) }.Concat(maps.Select(m => $"{AssetDatabase.GetAssetPath(m.lightmapColor)} {m.lightmapColor?.width}x{m.lightmapColor?.height}")));
        }
        static void TuneLighting()
        {
            foreach (var light in All<Light>().Where(l => l.type == LightType.Rectangle)) { light.color = Hex("FFF1DD"); light.intensity = 4.5f; }
            void Fill(string name, Vector3 position, Vector3 direction, Vector2 size, float intensity, string color)
            {
                var go = new GameObject(name); go.transform.SetParent(LightingRoot().transform, false);
                go.transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction, Vector3.up));
                var light = go.AddComponent<Light>(); light.type = LightType.Rectangle; light.areaSize = size;
                light.lightmapBakeType = LightmapBakeType.Baked; light.intensity = intensity; light.color = Hex(color); light.range = 9; light.shadows = LightShadows.Soft;
            }
            Fill("Baked soft sky bounce - front", new Vector3(0, 2.6f, 3.2f), Vector3.back, new Vector2(6, 2.5f), 2.4f, "E6F1FF");
            Fill("Baked soft sky bounce - ceiling", new Vector3(0, 3.9f, -.2f), new Vector3(0, -1, .001f), new Vector2(7, 5), 1.4f, "EDF2FF");
            var rug = AssetDatabase.LoadAssetAtPath<Material>(Art + "/Materials/M_RH_SR_REF_rugblue.mat");
            rug.SetColor("_BaseColor", Hex("2069A4")); rug.SetColor("_Color", Hex("2069A4")); EditorUtility.SetDirty(rug);
            AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(House());
        }
        static void CleanupPreview()
        {
            // Assigning an unchanged imported quaternion during preview can normalize it and create a redundant override.
            var ids = new HashSet<long> { -8369905298956550269, -6246968032396232732, -5911943615363941590, -429566468442726374 };
            foreach (var transform in All<Transform>()) {
                var source = PrefabUtility.GetCorrespondingObjectFromSource(transform);
                if (source != null && AssetDatabase.TryGetGUIDAndLocalFileIdentifier(source, out string guid, out long id) && ids.Contains(id)) {
                    var property = new SerializedObject(transform).FindProperty("m_LocalRotation");
                    if (property.prefabOverride) PrefabUtility.RevertPropertyOverride(property, InteractionMode.AutomatedAction);
                }
            }
            // Keep a clear blue exterior after Neutral's highlight roll-off.
            All<Camera>().Single().backgroundColor = new Color(.24f, .55f, .93f, 1);
            AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(House());
        }
        static void IsolateRenderer()
        {
            var pipeline = new SerializedObject(AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/UniversalRP.asset"));
            var renderers = pipeline.FindProperty("m_RendererDataList");
            if (renderers.arraySize < 3) renderers.arraySize = 3;
            renderers.GetArrayElementAtIndex(1).objectReferenceValue = AssetDatabase.LoadAssetAtPath<UniversalRendererData>("Assets/Settings/Renderer3D.asset");
            renderers.GetArrayElementAtIndex(2).objectReferenceValue = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(Art + "/Renderer_ReadingHouse_Mobile.asset");
            pipeline.ApplyModifiedPropertiesWithoutUndo();
            All<Camera>().Single().GetUniversalAdditionalCameraData().SetRenderer(2);
            AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(House());
        }
        static void GoldKeyMaterial()
        {
            var board = All<SReader.Game.ReadingHouse.MatStoneBoard>().Single();
            const string matPath = Art + "/Materials/M_RH_KeyStone_Gold.mat";
            const string prefabPath = Art + "/P_KeyStone_Gold.prefab";
            var material = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (material == null) { material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "M_RH_KeyStone_Gold" }; AssetDatabase.CreateAsset(material, matPath); }
            material.SetColor("_BaseColor", Hex("F3CA6C")); material.SetColor("_Color", Hex("F3CA6C"));
            material.SetFloat("_Smoothness", .24f); material.SetFloat("_Metallic", .08f);
            material.SetColor("_EmissionColor", Color.black); material.DisableKeyword("_EMISSION"); material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            EditorUtility.SetDirty(material);
            var originalPath = AssetDatabase.GetAssetPath(board.keyPrefab);
            var contents = PrefabUtility.LoadPrefabContents(originalPath);
            try {
                var stone = contents.GetComponentInChildren<WordStone>(true);
                string stoneBefore = JsonUtility.ToJson(stone);
                var poses = contents.GetComponentsInChildren<Transform>(true).Select(t => (t, t.localPosition, t.localRotation, t.localScale)).ToArray();
                if (!File.Exists(Output + "/key-material-before.txt")) File.WriteAllLines(Output + "/key-material-before.txt", contents.GetComponentsInChildren<MeshRenderer>(true).SelectMany(r => r.sharedMaterials).Where(m => m != null).Distinct().Select(m => $"{m.name}: shader={m.shader.name}, base={(m.HasProperty("_BaseColor") ? m.GetColor("_BaseColor").ToString() : m.HasProperty("_Color") ? m.GetColor("_Color").ToString() : "embedded default")}"));
                foreach (var renderer in contents.GetComponentsInChildren<MeshRenderer>(true)) {
                    if (renderer.GetComponent<TMPro.TMP_Text>() != null) continue;
                    renderer.sharedMaterials = renderer.sharedMaterials.Select(m => material).ToArray();
                }
                if (JsonUtility.ToJson(stone) != stoneBefore || poses.Any(p => !p.t.localPosition.Equals(p.localPosition) || !p.t.localRotation.Equals(p.localRotation) || !p.t.localScale.Equals(p.localScale))) throw new InvalidOperationException("Key material pass changed a non-visual prefab field");
                PrefabUtility.SaveAsPrefabAsset(contents, prefabPath);
            }
            finally { PrefabUtility.UnloadPrefabContents(contents); }
            board.keyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            EditorUtility.SetDirty(board);
            AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(House());
        }
        static void MatteKey()
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(Art + "/Materials/M_RH_KeyStone_Gold.mat");
            material.SetColor("_EmissionColor", Color.black); material.DisableKeyword("_EMISSION");
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            EditorUtility.SetDirty(material); AssetDatabase.SaveAssets();
        }
        static void BuildMessages()
        {
            var report = UnityEditor.Build.Reporting.BuildReport.GetLatestReport();
            if (report == null) throw new InvalidOperationException("No Android build report available");
            var errors = report.steps.SelectMany(s => s.messages.Where(m => m.type == LogType.Error || m.type == LogType.Exception).Select(m => $"{s.name}: [{m.type}] {m.content}"));
            File.WriteAllLines(Output + "/android-messages.txt", new[] { $"Result={report.summary.result}, errors={report.summary.totalErrors}, warnings={report.summary.totalWarnings}" }.Concat(errors));
        }
        static void Validate()
        {
            var failures = new List<string>();
            var volume = All<Volume>().Single(v => v.gameObject.name == "Reading House Lighting");
            if (!volume.isGlobal || volume.sharedProfile == null) failures.Add("Missing global grade");
            if (!All<Camera>().Single().GetUniversalAdditionalCameraData().renderPostProcessing) failures.Add("Camera post disabled");
            if (All<Light>().Count(l => l.lightmapBakeType != LightmapBakeType.Baked && l.shadows != LightShadows.None) > 1) failures.Add("Too many realtime shadow lights");
            if (All<Light>().Any(l => l.type != LightType.Directional && l.lightmapBakeType != LightmapBakeType.Baked)) failures.Add("Realtime local light remains");
            if (All<LightProbeGroup>().Length == 0 || All<ReflectionProbe>().Length == 0) failures.Add("Missing room probes");
            if (LightmapSettings.lightmaps.Length == 0) failures.Add("No baked lightmaps");
            foreach (var r in All<SReader.Game.ReadingHouse.ActorSpriteAnimator>()) if (!All<MeshRenderer>().Any(s => s.name == r.name + " Contact Shadow")) failures.Add("Missing actor contact shadow " + r.name);
            File.WriteAllText(Output + "/validation.txt", failures.Count == 0 ? "PASS visual configuration\n" : string.Join("\n", failures));
            if (failures.Count > 0) throw new InvalidOperationException(string.Join("; ", failures));
        }
    }
}
