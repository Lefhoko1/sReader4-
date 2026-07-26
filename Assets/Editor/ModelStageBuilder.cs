using System;
using System.Reflection;
using SReader.Showcase;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SReader.EditorTools
{
    /// <summary>
    /// Adds the trial1 FBX to the currently-open scene (e.g. SampleScene) so it
    /// shows FULL SCREEN while the game runs, on top of the UI Toolkit screens.
    ///
    /// The model is rendered by a dedicated camera into a RenderTexture, and a
    /// high-sorting-order UI Toolkit overlay paints that texture across the whole
    /// screen (see <see cref="FullscreenModelStage"/>). That's the reliable way
    /// to be visible over the app's overlay UI. Sized to the live screen, it fills
    /// the Huawei Y5 (720x1520) exactly.
    ///
    /// Run from:  Tools ▸ Reading Adventure ▸ Show trial1 Fullscreen (Y5)
    ///
    /// Re-running rebuilds the "Model Stage (Y5)" object. The scene is left dirty
    /// (not auto-saved) so you can review before saving.
    /// </summary>
    public static class ModelStageBuilder
    {
        const string ModelPath = "Assets/Gulps/trial3.fbx";
        const string StageName = "Model Stage (Y5)";
        const string OverlayPanelPath = "Assets/UI Toolkit/Trial1OverlayPanelSettings.asset";
        const string AppPanelPath = "Assets/UI Toolkit/PanelSettings.asset";

        // Huawei Y5 2019 (AMN-LX9): 720 x 1520, 19:9 portrait.
        const int Y5Width = 720;
        const int Y5Height = 1520;

        [MenuItem("Tools/Reading Adventure/Show trial3 Fullscreen (Y5)")]
        static void ShowModel()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
            if (prefab == null)
            {
                EditorUtility.DisplayDialog("Model not found",
                    $"Could not load a model at:\n{ModelPath}\n\nMake sure trial3.fbx is imported.",
                    "OK");
                return;
            }

            // Make sure the FBX brings any camera (and light) authored in Blender
            // across, so we can use Blender's exact framing instead of auto-fitting.
            if (AssetImporter.GetAtPath(ModelPath) is ModelImporter importer && !importer.importCameras)
            {
                importer.importCameras = true;
                importer.importLights = true;
                importer.SaveAndReimport();
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
            }

            var existing = GameObject.Find(StageName);
            if (existing != null)
                Undo.DestroyObjectImmediate(existing);

            var root = new GameObject(StageName);
            Undo.RegisterCreatedObjectUndo(root, "Show trial1 Fullscreen");

            // Model (prefab instance so it stays linked to the FBX).
            var model = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            model.name = prefab.name;
            Undo.RegisterCreatedObjectUndo(model, "Show trial1 Fullscreen");
            model.transform.SetParent(root.transform, false);
            model.transform.localPosition = Vector3.zero;

            // Sunny directional light.
            var lightGo = new GameObject("Stage Sun");
            Undo.RegisterCreatedObjectUndo(lightGo, "Show trial1 Fullscreen");
            lightGo.transform.SetParent(root.transform, false);
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            light.color = new Color(1f, 0.97f, 0.9f);
            light.shadows = LightShadows.Soft;

            // Camera that renders the model (into a RenderTexture at runtime).
            var camGo = new GameObject("Model Stage Camera");
            Undo.RegisterCreatedObjectUndo(camGo, "Show trial3 Fullscreen");
            camGo.transform.SetParent(root.transform, false);
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.55f, 0.78f, 0.95f); // sunny sky

            // Always frame with the reliable auto-framer (FBX camera FOV doesn't
            // survive the round-trip — copying it zooms in wrong). If Blender's
            // camera came across, borrow its viewing ANGLE only; the framer then
            // computes the distance so the island reliably fills the portrait frame.
            cam.fieldOfView = 35f;
            var framer = camGo.AddComponent<PortraitModelFramer>();
            framer.target = model.transform;

            var blenderCam = model.GetComponentInChildren<Camera>(true);
            if (blenderCam != null)
            {
                framer.viewEulerAngles = blenderCam.transform.eulerAngles; // Blender's angle
                blenderCam.enabled = false; // don't let the imported camera render too
                var al = blenderCam.GetComponent<AudioListener>();
                if (al != null) al.enabled = false;
                Debug.Log("[ReadingAdventure] Framing = Blender camera ANGLE + Unity auto-fit (reliable fill).");
            }
            else
            {
                Debug.Log("[ReadingAdventure] No camera in the FBX — using the Unity auto-framer's default angle.");
            }

            framer.Frame();

            // Full-screen overlay (UIDocument + high-sorting PanelSettings) that
            // paints the camera's RenderTexture across the whole screen.
            var overlayGo = new GameObject("Trial1 Fullscreen Overlay");
            Undo.RegisterCreatedObjectUndo(overlayGo, "Show trial1 Fullscreen");
            overlayGo.transform.SetParent(root.transform, false);
            var doc = overlayGo.AddComponent<UIDocument>();
            doc.panelSettings = GetOrCreateOverlayPanel();
            var stage = overlayGo.AddComponent<FullscreenModelStage>();
            stage.modelCamera = cam;

            Selection.activeGameObject = root;
            SceneView.lastActiveSceneView?.FrameSelected();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

            ConfigureForY5();

            Debug.Log($"[ReadingAdventure] trial3 full-screen stage added to scene " +
                      $"'{SceneManager.GetActiveScene().name}'. Press Play to see it; SAVE the scene to keep it.");
        }

        [MenuItem("Tools/Reading Adventure/Configure Player + Game View For Huawei Y5")]
        static void ConfigureY5Menu()
        {
            ConfigureForY5();
            Debug.Log("[ReadingAdventure] Player orientation set to Portrait and Huawei Y5 (720x1520) Game View size ensured.");
        }

        /// <summary>
        /// A PanelSettings whose sortingOrder sits above the app panels (so the
        /// model overlay draws on top). Cloned from the app's PanelSettings to
        /// keep the same theme.
        /// </summary>
        static PanelSettings GetOrCreateOverlayPanel()
        {
            var ps = AssetDatabase.LoadAssetAtPath<PanelSettings>(OverlayPanelPath);
            if (ps == null)
            {
                var src = AssetDatabase.LoadAssetAtPath<PanelSettings>(AppPanelPath);
                ps = src != null
                    ? UnityEngine.Object.Instantiate(src)
                    : ScriptableObject.CreateInstance<PanelSettings>();
                ps.name = "Trial1 Overlay PanelSettings";
                AssetDatabase.CreateAsset(ps, OverlayPanelPath);
            }
            ps.sortingOrder = 100; // above the app's panels (sortingOrder 0)
            EditorUtility.SetDirty(ps);
            AssetDatabase.SaveAssets();
            return ps;
        }

        internal static void ConfigureForY5()
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            TryAddGameViewSize($"Huawei Y5 2019 ({Y5Width}x{Y5Height})", Y5Width, Y5Height);
        }

        /// <summary>
        /// Copies the lens/framing from the Blender-imported camera onto our stage
        /// camera (keeping our own clear flags / background). Forces a vertical gate
        /// fit so the portrait full-screen output never letterboxes.
        /// </summary>
        static void CopyFraming(Camera src, Camera dst)
        {
            dst.fieldOfView = src.fieldOfView;
            dst.usePhysicalProperties = src.usePhysicalProperties;
            if (src.usePhysicalProperties)
            {
                dst.focalLength = src.focalLength;
                dst.sensorSize = src.sensorSize;
                dst.lensShift = src.lensShift;          // preserves Blender's shift_y headroom
                dst.gateFit = Camera.GateFitMode.Vertical;
            }
            dst.nearClipPlane = Mathf.Max(0.01f, src.nearClipPlane);
            dst.farClipPlane = Mathf.Max(dst.farClipPlane, src.farClipPlane);
        }

        /// <summary>
        /// Best-effort: registers a fixed-resolution Game View size via internal
        /// editor APIs. Wrapped so any API drift logs manual steps, not an error.
        /// </summary>
        static void TryAddGameViewSize(string sizeName, int width, int height)
        {
            try
            {
                var asm = typeof(EditorWindow).Assembly;
                var sizesType = asm.GetType("UnityEditor.GameViewSizes");
                var singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
                var instance = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static)
                    .GetValue(null);

                int groupType = (int)sizesType.GetProperty("currentGroupType").GetValue(instance);
                var group = sizesType.GetMethod("GetGroup").Invoke(instance, new object[] { groupType });
                var groupT = group.GetType();

                var existingTexts = (string[])groupT.GetMethod("GetDisplayTexts").Invoke(group, null);
                foreach (var t in existingTexts)
                    if (t != null && t.Contains(sizeName))
                        return;

                var gvSizeType = asm.GetType("UnityEditor.GameViewSize");
                var gvSizeTypeEnum = asm.GetType("UnityEditor.GameViewSizeType");
                var ctor = gvSizeType.GetConstructor(new[] { gvSizeTypeEnum, typeof(int), typeof(int), typeof(string) });
                // GameViewSizeType.FixedResolution == 1
                var newSize = ctor.Invoke(new object[] { Enum.ToObject(gvSizeTypeEnum, 1), width, height, sizeName });
                groupT.GetMethod("AddCustomSize").Invoke(group, new[] { newSize });

                sizesType.GetMethod("SaveToHDD")?.Invoke(instance, null);
                Debug.Log($"[ReadingAdventure] Added Game View size '{sizeName}'. Select it from the Game view dropdown.");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[ReadingAdventure] Couldn't auto-add the Y5 Game View size ({ex.Message}). " +
                                 $"Add it manually: Game view ▸ aspect dropdown ▸ + ▸ Fixed Resolution {width} x {height}.");
            }
        }
    }
}
