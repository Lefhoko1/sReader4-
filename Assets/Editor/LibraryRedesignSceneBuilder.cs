using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using SReader.EnvironmentPreview;
using Object = UnityEngine.Object;

namespace SReader.EditorTools
{
    [InitializeOnLoad]
    public static class LibraryRedesignSceneBuilder
    {
        const string AssetRoot = "Assets/Art/LibraryRedesign";
        const string ScenePath = "Assets/Scenes/SC_LibraryRedesignPreview.unity";
        const string ExportRelative = "GeneratedAssets/LibraryRedesign/20261017_184255/unity";
        static readonly string Project = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        static string ExportRoot => Path.Combine(Project, ExportRelative);
        static string Request => Path.Combine(ExportRoot, "build.request");
        static string Status => Path.Combine(ExportRoot, "unity_status.json");
        static double nextPoll;
        static bool busy;
        [Serializable] class MaterialData { public string name; public float[] color; public float metallic, roughness, emissionStrength; public float[] emission; }
        [Serializable] class SubMeshData { public int[] indices; }
        [Serializable] class ObjectData { public string name, collection; public float[] position, vertices, normals, uv; public string[] materials; public SubMeshData[] submeshes; }
        [Serializable] class CameraData { public string name; public float[] position, forward, up; public bool orthographic; public float orthoSize, fieldOfView; }
        [Serializable] class LibraryData { public string source; public MaterialData[] materials; public ObjectData[] objects; public CameraData[] cameras; }
        [Serializable] class BuildReport { public string status, scene, source, message; public int objects, triangles, materials, lights, rendererIndex; public string[] sceneBackups; public bool viewSwitchesVerified, runtimeVerified; }
        static LibraryRedesignSceneBuilder() { nextPoll = EditorApplication.timeSinceStartup + 2; EditorApplication.update += Poll; }
        static void Poll()
        {
            if (busy || EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.timeSinceStartup < nextPoll) return;
            nextPoll = EditorApplication.timeSinceStartup + 1;
            if (File.Exists(Request))
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode) { EditorApplication.isPlaying = false; return; }
                File.Delete(Request);
                Build();
            }
            var command = Path.Combine(ExportRoot, "view.request");
            if (File.Exists(command))
            {
                string value = File.ReadAllText(command).Trim(); File.Delete(command);
                if (value == "capture") CaptureAll();
                else if (value == "polish") { Polish(); EditorApplication.delayCall += CaptureAll; }
                else if (value == "play") EditorApplication.isPlaying = true;
                else if (value == "verify") { CaptureAll(); if (EditorApplication.isPlaying) ScreenCapture.CaptureScreenshot(Path.Combine(ExportRoot, "screenshots/04_unity_live.png")); }
                else if (int.TryParse(value, out int index)) Show(index);
            }
        }
        [MenuItem("Tools/sReader/Library Preview/Build and Open Preview")]
        public static void Build()
        {
            if (busy) return;
            if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Build the preview in Edit mode.");
            busy = true;
            var report = new BuildReport { status = "building", scene = ScenePath };
            File.WriteAllText(Status, JsonUtility.ToJson(report, true));
            try
            {
                string dataPath = Path.Combine(ExportRoot, "library_meshes.json");
                var data = JsonUtility.FromJson<LibraryData>(File.ReadAllText(dataPath));
                if (data.objects.Length != 389) throw new InvalidDataException("Incomplete library export.");
                report.source = data.source;
                Shader lit = Shader.Find("Universal Render Pipeline/Lit");
                if (!lit) throw new InvalidOperationException("URP/Lit is unavailable.");
                int rendererIndex = Find3DRenderer(); report.rendererIndex = rendererIndex;
                EnsureFolder(AssetRoot + "/Materials"); EnsureFolder(AssetRoot + "/Meshes"); EnsureFolder(AssetRoot + "/Prefabs");
                var palette = new Dictionary<string, Material>();
                foreach (var source in data.materials)
                {
                    string path = AssetRoot + "/Materials/" + SafeName(source.name) + ".mat";
                    var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                    if (material == null) { material = new Material(lit) { name = source.name }; AssetDatabase.CreateAsset(material, path); }
                    material.shader = lit;
                    material.SetColor("_BaseColor", ColorOf(source.color));
                    material.SetFloat("_Metallic", source.metallic);
                    material.SetFloat("_Smoothness", 1f - source.roughness);
                    material.SetFloat("_Cull", 0); material.doubleSidedGI = true;
                    material.enableInstancing = true;
                    if (source.emissionStrength > 0)
                    {
                        material.EnableKeyword("_EMISSION");
                        material.SetColor("_EmissionColor", ColorOf(source.emission) * source.emissionStrength);
                        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
                    }
                    EditorUtility.SetDirty(material); palette.Add(source.name, material);
                }
                // Preserve dirty scenes as copies before switching; never overwrite a working source scene.
                var backups = new List<string>();
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    var previous = SceneManager.GetSceneAt(i);
                    if (!previous.isLoaded || !previous.isDirty) continue;
                    string directory = "Assets/Scenes/_RedesignBackups/" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    EnsureFolder(directory);
                    string path = directory + "/" + SafeName(string.IsNullOrEmpty(previous.name) ? "Untitled" : previous.name) + "_" + i + ".unity";
                    if (!EditorSceneManager.SaveScene(previous, path, true)) throw new IOException("Could not back up unsaved scene: " + previous.name);
                    backups.Add(path);
                }
                report.sceneBackups = backups.ToArray();
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var root = new GameObject("sReader — Great Library");
                var groups = new Dictionary<string, Transform>();
                foreach (string name in new[] { "Architecture", "Roof", "Interior", "Interactive Props", "Landscaping" })
                {
                    var group = new GameObject(name); group.transform.SetParent(root.transform); groups[name] = group.transform;
                }
                int totalTriangles = 0;
                for (int objectIndex = 0; objectIndex < data.objects.Length; objectIndex++)
                {
                    var source = data.objects[objectIndex];
                    var model = new Mesh { name = source.name };
                    if (source.vertices.Length / 3 > 65535) model.indexFormat = IndexFormat.UInt32;
                    model.vertices = Triples(source.vertices); model.normals = Triples(source.normals);
                    var uv = new Vector2[source.uv.Length / 2];
                    for (int i = 0; i < uv.Length; i++) uv[i] = new Vector2(source.uv[i * 2], source.uv[i * 2 + 1]);
                    model.uv = uv; model.subMeshCount = source.submeshes.Length;
                    for (int i = 0; i < source.submeshes.Length; i++) { model.SetTriangles(source.submeshes[i].indices, i); totalTriangles += source.submeshes[i].indices.Length / 3; }
                    model.RecalculateBounds();
                    string path = AssetRoot + "/Meshes/" + objectIndex.ToString("D3") + "_" + SafeName(source.name) + ".asset";
                    var saved = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                    if (saved) { EditorUtility.CopySerialized(model, saved); Object.DestroyImmediate(model); model = saved; }
                    else AssetDatabase.CreateAsset(model, path);
                    var go = new GameObject(source.name); go.transform.SetParent(groups[source.collection]); go.transform.position = V(source.position);
                    go.AddComponent<MeshFilter>().sharedMesh = model;
                    var renderer = go.AddComponent<MeshRenderer>(); renderer.sharedMaterials = source.materials.Select(n => palette[n]).ToArray();
                    renderer.shadowCastingMode = source.collection == "Interactive Props" && source.name.Contains("Mote") ? ShadowCastingMode.Off : ShadowCastingMode.On;
                    if (source.collection != "Interactive Props" && source.collection != "Roof")
                        GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccludeeStatic | StaticEditorFlags.ReflectionProbeStatic);
                    if (source.name == "SR_Wood_Floor") go.AddComponent<BoxCollider>();
                }
                if (totalTriangles != 73904) throw new InvalidDataException("Export triangle count mismatch: " + totalTriangles);
                // A reusable model prefab contains geometry only; preview controls/lights stay in this scene.
                PrefabUtility.SaveAsPrefabAsset(root, AssetRoot + "/Prefabs/P_GreatLibrary.prefab");
                var cameraObject = new GameObject("Library Camera"); cameraObject.tag = "MainCamera";
                var camera = cameraObject.AddComponent<Camera>(); camera.nearClipPlane = .1f; camera.farClipPlane = 100;
                cameraObject.AddComponent<AudioListener>();
                camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.071f, .078f, .098f); camera.allowHDR = true;
                var additional = camera.GetUniversalAdditionalCameraData(); additional.SetRenderer(rendererIndex); additional.renderPostProcessing = true;
                additional.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                var controls = new GameObject("Library Preview Controls").AddComponent<LibraryRedesignPreview>();
                controls.previewCamera = camera; controls.roof = groups["Roof"].gameObject;
                controls.views = data.cameras.Select((source, index) => new LibraryRedesignPreview.ViewPreset {
                    label = new[] { "Interior", "Exterior", "Book" }[index], position = V(source.position),
                    rotation = Quaternion.LookRotation(V(source.forward), V(source.up)), orthographic = source.orthographic,
                    orthographicSize = source.orthoSize, fieldOfView = source.fieldOfView, showRoof = index == 1
                }).ToArray();
                SetupLighting(); SetupPostProcessing();
                // Native colliders for the actual wall shell, excluding glazing and decorative overlays.
                foreach (var filter in groups["Architecture"].GetComponentsInChildren<MeshFilter>())
                    if (filter.name.Contains("Wall_") || filter.name.Contains("Front_Pier") || filter.name.Contains("spandrel")) filter.gameObject.AddComponent<MeshCollider>().sharedMesh = filter.sharedMesh;
                controls.ShowView(0);
                AssetDatabase.SaveAssets();
                if (!EditorSceneManager.SaveScene(scene, ScenePath)) throw new IOException("Could not save preview scene.");
                report.status = "scene_created"; report.objects = data.objects.Length; report.triangles = totalTriangles; report.materials = palette.Count; report.lights = 4;
                File.WriteAllText(Status, JsonUtility.ToJson(report, true));
                Debug.Log("[LibraryRedesign] Preview created: " + ScenePath + ". Original scenes preserved. Press Play for Interior / Exterior / Book controls.");
                Selection.activeGameObject = cameraObject;
                if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.LookAt(new Vector3(0, 1, 0), Quaternion.Euler(65, 0, 0), 9);
                FocusGameView();
                EditorApplication.delayCall += () => { try { CaptureAll(); } catch (Exception e) { Debug.LogException(e); report.status = "capture_failed"; report.message = e.ToString(); File.WriteAllText(Status, JsonUtility.ToJson(report, true)); } };
            }
            catch (Exception e)
            {
                report.status = "error"; report.message = e.ToString(); File.WriteAllText(Status, JsonUtility.ToJson(report, true)); Debug.LogException(e);
            }
            finally { busy = false; }
        }
        static int Find3DRenderer()
        {
            var pipeline = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (!pipeline) pipeline = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
            if (!pipeline) pipeline = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
            if (!pipeline) throw new InvalidOperationException("This preview requires the project's URP asset.");
            var list = new SerializedObject(pipeline).FindProperty("m_RendererDataList");
            for (int i = 0; i < list.arraySize; i++) if (list.GetArrayElementAtIndex(i).objectReferenceValue is UniversalRendererData) return i;
            throw new InvalidOperationException("No 3D renderer is registered on the active URP asset.");
        }
        static void SetupLighting()
        {
            var root = new GameObject("Library Lighting");
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(.47f, .55f, .65f);
            RenderSettings.ambientEquatorColor = new Color(.38f, .36f, .31f);
            RenderSettings.ambientGroundColor = new Color(.17f, .16f, .14f);
            RenderSettings.ambientIntensity = 1;
            RenderSettings.fog = false;
            var key = NewLight(root.transform, "Warm evening key", LightType.Directional, new Vector3(-5, 12, -8), new Color(1, .88f, .71f), 1.25f, 0);
            key.transform.rotation = Quaternion.LookRotation(new Vector3(5, -12, 8)); key.shadows = LightShadows.Soft; key.shadowStrength = .7f; key.shadowBias = .035f; key.shadowNormalBias = .15f;
            RenderSettings.sun = key;
            NewLight(root.transform, "Reading glow", LightType.Point, new Vector3(0, 3.9f, .4f), new Color(1, .78f, .52f), 3.8f, 7.4f);
            NewLight(root.transform, "Cool window fill", LightType.Point, new Vector3(3.1f, 3.8f, 1.8f), new Color(.62f, .76f, 1), 2, 10);
            NewLight(root.transform, "Entrance lantern spill", LightType.Point, new Vector3(0, 2.8f, -4.9f), new Color(1, .72f, .38f), 2.5f, 5.5f);
        }
        static Light NewLight(Transform root, string name, LightType type, Vector3 position, Color color, float intensity, float range)
        {
            var go = new GameObject(name); go.transform.SetParent(root); go.transform.position = position;
            var light = go.AddComponent<Light>(); light.type = type; light.color = color; light.intensity = intensity; light.range = range; light.shadows = LightShadows.None; return light;
        }
        static void SetupPostProcessing()
        {
            string path = AssetRoot + "/LibraryPreviewPost.asset";
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);
            if (!profile)
            {
                profile = ScriptableObject.CreateInstance<VolumeProfile>(); AssetDatabase.CreateAsset(profile, path);
                var tone = profile.Add<Tonemapping>(); tone.mode.Override(TonemappingMode.ACES);
                var bloom = profile.Add<Bloom>(); bloom.intensity.Override(.20f); bloom.threshold.Override(1.1f); bloom.scatter.Override(.55f);
                var grade = profile.Add<ColorAdjustments>(); grade.postExposure.Override(.2f); grade.contrast.Override(6); grade.saturation.Override(-4);
                foreach (var component in profile.components) AssetDatabase.AddObjectToAsset(component, profile);
                EditorUtility.SetDirty(profile);
            }
            var volume = new GameObject("Library Atmosphere").AddComponent<Volume>(); volume.isGlobal = true; volume.sharedProfile = profile;
        }
        [MenuItem("Tools/sReader/Library Preview/View Interior")]
        static void Interior() => Show(0);
        [MenuItem("Tools/sReader/Library Preview/View Exterior")]
        static void Exterior() => Show(1);
        [MenuItem("Tools/sReader/Library Preview/View Hero Book")]
        static void Hero() => Show(2);
        static void Show(int index)
        {
            var controls = Object.FindFirstObjectByType<LibraryRedesignPreview>();
            if (!controls) return;
            controls.ShowView(index);
            if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(controls.gameObject.scene);
            FocusGameView();
        }
        static void Polish()
        {
            var controls = Object.FindFirstObjectByType<LibraryRedesignPreview>();
            if (!controls) throw new InvalidOperationException("Open the preview scene first.");
            RenderSettings.ambientSkyColor = new Color(.60f, .66f, .73f);
            RenderSettings.ambientEquatorColor = new Color(.49f, .47f, .43f);
            RenderSettings.ambientGroundColor = new Color(.25f, .23f, .21f);
            if (RenderSettings.sun) { RenderSettings.sun.intensity = 1.0f; RenderSettings.sun.color = new Color(1, .94f, .85f); RenderSettings.sun.shadowStrength = .35f; }
            foreach (var renderer in controls.gameObject.scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<MeshRenderer>(true)))
            {
                string name = renderer.name;
                if (name.Contains("Ink") || name.Contains("Motto") || name.Contains("Wordmark") || name.Contains("Text") || name.Contains("Glyph") || name.Contains("Lead") || name.Contains("Seam") || name.Contains("Border") || name.Contains("Mote")) renderer.shadowCastingMode = ShadowCastingMode.Off;
            }
            foreach (string name in new[] { "SR_Amber_Glass", "SR_Book_Magic" })
            {
                var mat = AssetDatabase.LoadAssetAtPath<Material>(AssetRoot + "/Materials/" + name + ".mat");
                if (mat) { mat.SetColor("_EmissionColor", name.Contains("Amber") ? new Color(1f, .68f, .31f) * .65f : new Color(.25f, .70f, .61f) * .85f); EditorUtility.SetDirty(mat); }
            }
            if (!controls.previewCamera.GetComponent<AudioListener>()) controls.previewCamera.gameObject.AddComponent<AudioListener>();
            controls.ShowView(0); AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(controls.gameObject.scene, ScenePath);
        }
        [MenuItem("Tools/sReader/Library Preview/Capture Three Views")]
        public static void CaptureAll()
        {
            var controls = Object.FindFirstObjectByType<LibraryRedesignPreview>();
            if (!controls) throw new InvalidOperationException("Open the library preview scene first.");
            var camera = controls.previewCamera; var originalView = controls.currentView;
            string folder = Path.Combine(ExportRoot, "screenshots"); Directory.CreateDirectory(folder);
            var target = new RenderTexture(1600, 1200, 24, RenderTextureFormat.ARGB32) { antiAliasing = 1 };
            target.Create();
            try
            {
                camera.aspect = 4f / 3f;
                // Warm up a freshly initialized SRP camera before taking the first capture.
                RenderPipeline.SubmitRenderRequest(camera, new UniversalRenderPipeline.SingleCameraRequest { destination = target });
                for (int i = 0; i < controls.views.Length; i++)
                {
                    controls.ShowView(i);
                    if (controls.roof.activeSelf != controls.views[i].showRoof || camera.orthographic != controls.views[i].orthographic) throw new InvalidOperationException("Preview view switch failed.");
                    var request = new UniversalRenderPipeline.SingleCameraRequest { destination = target };
                    if (!RenderPipeline.SupportsRenderRequest(camera, request)) throw new NotSupportedException("The selected renderer cannot capture the preview camera.");
                    RenderPipeline.SubmitRenderRequest(camera, request);
                    var previous = RenderTexture.active;
                    Texture2D image = null;
                    try { RenderTexture.active = target; image = new Texture2D(1600, 1200, TextureFormat.RGB24, false); image.ReadPixels(new Rect(0, 0, 1600, 1200), 0, 0); image.Apply();
                        Color first = image.GetPixel(0, 0); bool hasGeometry = false;
                        for (int y = 100; y < image.height && !hasGeometry; y += 100)
                            for (int x = 100; x < image.width; x += 100) { Color sample = image.GetPixel(x, y); if (Mathf.Abs(sample.r - first.r) + Mathf.Abs(sample.g - first.g) + Mathf.Abs(sample.b - first.b) > .06f) { hasGeometry = true; break; } }
                        if (!hasGeometry) throw new InvalidOperationException("Blank preview capture: " + controls.views[i].label);
                        File.WriteAllBytes(Path.Combine(folder, (i + 1).ToString("D2") + "_unity_" + controls.views[i].label.ToLowerInvariant() + ".png"), image.EncodeToPNG()); }
                    finally { RenderTexture.active = previous; if (image) Object.DestroyImmediate(image); }
                }
            }
            finally { camera.ResetAspect(); controls.ShowView(originalView); target.Release(); Object.DestroyImmediate(target); }
            if (!Application.isPlaying) EditorSceneManager.SaveScene(controls.gameObject.scene, ScenePath);
            var report = JsonUtility.FromJson<BuildReport>(File.ReadAllText(Status)); report.status = "complete"; report.viewSwitchesVerified = true; report.runtimeVerified = Application.isPlaying; report.message = "Preview scene saved, three Unity camera captures written, interior restored.";
            File.WriteAllText(Status, JsonUtility.ToJson(report, true)); FocusGameView();
            Debug.Log("[LibraryRedesign] All three Unity views captured and scene restored to Interior.");
        }
        static void FocusGameView()
        {
            var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");
            if (type == null) return;
            var window = EditorWindow.GetWindow(type);
            try
            {
                var sizesType = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameViewSizes");
                var instance = typeof(ScriptableSingleton<>).MakeGenericType(sizesType).GetProperty("instance").GetValue(null);
                var groupType = sizesType.GetProperty("currentGroupType").GetValue(instance);
                var group = sizesType.GetMethod("GetGroup").Invoke(instance, new[] { groupType });
                var texts = (string[])group.GetType().GetMethod("GetDisplayTexts").Invoke(group, null);
                int index = Array.FindIndex(texts, t => t.Contains("Library Preview"));
                if (index < 0)
                {
                    var sizeType = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameViewSize");
                    var enumType = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameViewSizeType");
                    var value = Activator.CreateInstance(sizeType, Enum.ToObject(enumType, 1), 1600, 1200, "Library Preview");
                    group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { value }); index = texts.Length;
                }
                type.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(window, index);
            }
            catch (Exception e) { Debug.Log("[LibraryRedesign] Game view uses current aspect: " + e.Message); }
            window.Show(); window.Focus(); window.Repaint();
        }
        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\', '/'); EnsureFolder(parent); AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
        static string SafeName(string value) => string.Concat(value.Select(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.' ? c : '_'));
        static Vector3 V(float[] value) => new Vector3(value[0], value[1], value[2]);
        static Color ColorOf(float[] value) => new Color(value[0], value[1], value[2], value.Length > 3 ? value[3] : 1);
        static Vector3[] Triples(float[] source)
        {
            var result = new Vector3[source.Length / 3];
            for (int i = 0; i < result.Length; i++) result[i] = new Vector3(source[i * 3], source[i * 3 + 1], source[i * 3 + 2]);
            return result;
        }
    }
}
