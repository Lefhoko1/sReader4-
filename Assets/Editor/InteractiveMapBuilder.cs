using System.IO;
using SReader.Showcase;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SReader.EditorTools
{
    /// <summary>
    /// Builds a dedicated, INTERACTIVE world-map scene from trial3.fbx:
    ///   • camera renders straight to the screen (no RenderTexture / overlay)
    ///   • drag to orbit, pinch / wheel to zoom, tap a level node to select it
    ///   • mesh colliders are added so taps hit the 3D model directly
    ///
    /// Run from:  Tools ▸ Reading Adventure ▸ Build Interactive Map Scene (trial3)
    ///
    /// Creates / opens Assets/Scenes/ReadingAdventureMap.unity and saves it.
    /// SampleScene (the app) is untouched.
    /// </summary>
    public static class InteractiveMapBuilder
    {
        const string ModelPath = "Assets/Gulps/trial3.fbx";
        const string ScenePath = "Assets/Scenes/ReadingAdventureMap.unity";
        const string RootName = "Reading Adventure Map";

        [MenuItem("Tools/Reading Adventure/Build Interactive Map Scene (trial3)")]
        static void Build()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
            if (prefab == null)
            {
                EditorUtility.DisplayDialog("Model not found",
                    $"Could not load a model at:\n{ModelPath}\n\nExport trial3.fbx from Blender into Assets/Gulps first.",
                    "OK");
                return;
            }

            // Bring across any Blender camera/light (used for the starting angle).
            if (AssetImporter.GetAtPath(ModelPath) is ModelImporter importer &&
                (!importer.importCameras || !importer.importLights))
            {
                importer.importCameras = true;
                importer.importLights = true;
                importer.SaveAndReimport();
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
            }

            if (!EnsureDedicatedScene()) return;

            var existing = GameObject.Find(RootName);
            if (existing != null) Undo.DestroyObjectImmediate(existing);

            var root = new GameObject(RootName);
            Undo.RegisterCreatedObjectUndo(root, "Build Interactive Map");

            // Model.
            var model = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            model.name = prefab.name;
            Undo.RegisterCreatedObjectUndo(model, "Build Interactive Map");
            model.transform.SetParent(root.transform, false);
            model.transform.localPosition = Vector3.zero;

            // Colliders so taps can hit the 3D world.
            int colliders = 0;
            foreach (var mf in model.GetComponentsInChildren<MeshFilter>())
            {
                if (mf.sharedMesh == null) continue;
                if (mf.GetComponent<Collider>() != null) continue;
                Undo.AddComponent<MeshCollider>(mf.gameObject);
                colliders++;
            }

            // Light: use the one from Blender if present, else add a sun.
            if (model.GetComponentInChildren<Light>() == null)
            {
                var lightGo = new GameObject("Sun");
                Undo.RegisterCreatedObjectUndo(lightGo, "Build Interactive Map");
                lightGo.transform.SetParent(root.transform, false);
                lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                var light = lightGo.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.1f;
                light.color = new Color(1f, 0.97f, 0.9f);
                light.shadows = LightShadows.Soft;
            }

            // Camera (renders directly to screen → naturally full screen).
            var camGo = new GameObject("Map Camera");
            Undo.RegisterCreatedObjectUndo(camGo, "Build Interactive Map");
            camGo.transform.SetParent(root.transform, false);
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.55f, 0.78f, 0.95f); // sunny sky
            cam.fieldOfView = 35f;
            camGo.AddComponent<AudioListener>();

            var controller = camGo.AddComponent<InteractiveMapController>();
            controller.model = model.transform;

            // Borrow Blender's viewing angle for the starting orbit, and disable
            // the imported camera so it doesn't render alongside ours.
            var blenderCam = model.GetComponentInChildren<Camera>(true);
            if (blenderCam != null)
            {
                var e = blenderCam.transform.eulerAngles;
                controller.yaw = e.y;
                controller.pitch = Mathf.Clamp(NormalizePitch(e.x), controller.minPitch, controller.maxPitch);
                blenderCam.enabled = false;
                var al = blenderCam.GetComponent<AudioListener>();
                if (al != null) al.enabled = false;
            }
            controller.Recompute();

            Selection.activeGameObject = root;
            SceneView.lastActiveSceneView?.FrameSelected();

            ModelStageBuilder.ConfigureForY5();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

            Debug.Log($"[ReadingAdventure] Interactive map built in '{ScenePath}' " +
                      $"({colliders} colliders). Press Play: drag = orbit, pinch/wheel = zoom, tap = pick a node.");
        }

        static float NormalizePitch(float x) => x > 180f ? x - 360f : x;

        static bool EnsureDedicatedScene()
        {
            if (SceneManager.GetActiveScene().path == ScenePath) return true;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return false;

            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
                AssetDatabase.CreateFolder("Assets", "Scenes");

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
    }
}
