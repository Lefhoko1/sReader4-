using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using SReader.Game.ReadingHouse;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// The world round the Reading House (SC_ReadingHouse_Integration): a painted panorama on a wide
    /// curved backdrop far behind and beside the house, so the opening shot and every view through the
    /// windows and doors look out on a storybook landscape instead of a flat sky colour.
    ///
    /// The picture is <c>Assets/Art/ReadingHouse/Backdrop/T_WorldBackdrop.png</c>. Replace that file with
    /// the painted world image (a wide landscape, about 4:1, horizon a third of the way up) and it
    /// appears in the scene: nothing else to change. Until then a soft generated placeholder is used.
    /// Unlit and shadow-free (one draw call). Also sets the student's steady walking pace.
    /// Re-running replaces only what this adds.
    /// </summary>
    public static class WorldBackdropSetup
    {
        const string Folder = "Assets/Art/ReadingHouse/Backdrop";
        const string TexturePath = Folder + "/T_WorldBackdrop.png";
        const string MaterialPath = Folder + "/M_WorldBackdrop.mat";
        const string MeshPath = Folder + "/SM_WorldBackdrop.asset";
        const float Radius = 70f, Height = 46f, ArcDegrees = 240f, Sink = 6f;

        [MenuItem("Tools/sReader/Reading House/Set Up World Backdrop")]
        public static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new System.InvalidOperationException("Stop Play mode first: the scene can only be changed in Edit mode.");
            var open = SceneManager.GetSceneByPath(ReadingHouseSceneBuilder.ScenePath);
            bool wasOpen = open.IsValid() && open.isLoaded;
            var scene = wasOpen ? open : EditorSceneManager.OpenScene(ReadingHouseSceneBuilder.ScenePath, OpenSceneMode.Additive);
            var previousActive = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(scene);
            try
            {
                foreach (var old in scene.GetRootGameObjects().Where(g => g.name == "World Backdrop").ToArray())
                    Object.DestroyImmediate(old);
                var all = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();
                var door = all.Select(t => t.GetComponent<HouseDoor>()).First(d => d != null);
                var walker = all.Select(t => t.GetComponent<StudentWalker>()).FirstOrDefault(w => w != null);
                var presenter = all.Select(t => t.GetComponent<ReadingHousePresenter>()).First(p => p != null);

                Directory.CreateDirectory(Folder);
                if (!File.Exists(TexturePath)) WritePlaceholder(TexturePath);
                AssetDatabase.ImportAsset(TexturePath);
                var importer = (TextureImporter)AssetImporter.GetAtPath(TexturePath);
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.mipmapEnabled = false;
                importer.maxTextureSize = 4096;
                importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings { name = "Android", overridden = true, maxTextureSize = 2048, format = TextureImporterFormat.ASTC_6x6 });
                importer.SaveAndReimport();
                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(TexturePath);

                var mat = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
                if (mat == null)
                {
                    mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                    AssetDatabase.CreateAsset(mat, MaterialPath);
                }
                mat.SetTexture("_BaseMap", tex);
                mat.mainTexture = tex;
                mat.SetColor("_BaseColor", Color.white);
                EditorUtility.SetDirty(mat);

                var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(MeshPath);
                if (mesh == null) { mesh = new Mesh { name = "SM_WorldBackdrop" }; AssetDatabase.CreateAsset(mesh, MeshPath); }
                BuildArc(mesh);
                EditorUtility.SetDirty(mesh);

                // Centred on the house, the arc's middle straight behind it (seen from the front path).
                var centre = door.houseCentre.position;
                var outward = Vector3.ProjectOnPlane(door.transform.position - centre, Vector3.up).normalized;
                var go = new GameObject("World Backdrop");
                go.transform.SetPositionAndRotation(new Vector3(centre.x, centre.y - Sink, centre.z), Quaternion.LookRotation(-outward, Vector3.up));
                go.AddComponent<MeshFilter>().sharedMesh = mesh;
                var mr = go.AddComponent<MeshRenderer>();
                mr.sharedMaterial = mat;
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
                mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                GameObjectUtility.SetStaticEditorFlags(go, 0);

                var cam = presenter.cameraRig.viewCamera;
                if (cam.farClipPlane < Radius * 2.2f) cam.farClipPlane = Radius * 2.2f;

                if (walker != null)
                {
                    walker.speed = 1.35f;
                    walker.turnRate = 300f;
                    EditorUtility.SetDirty(walker);
                }

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                AssetDatabase.SaveAssets();
                Debug.Log($"[WorldBackdrop] Backdrop at {go.transform.position}, facing {go.transform.forward}; texture {tex.width}x{tex.height}; far clip {cam.farClipPlane}.");
            }
            finally
            {
                if (previousActive.IsValid() && previousActive != scene) SceneManager.SetActiveScene(previousActive);
                if (!wasOpen) EditorSceneManager.CloseScene(scene, true);
            }
        }

        /// <summary>The inside of a cylinder segment (faces inward), the picture's middle at local +Z.</summary>
        static void BuildArc(Mesh mesh)
        {
            const int segments = 64;
            var verts = new Vector3[(segments + 1) * 2];
            var uvs = new Vector2[verts.Length];
            var tris = new int[segments * 6];
            for (int i = 0; i <= segments; i++)
            {
                float u = i / (float)segments;
                float a = (u - 0.5f) * ArcDegrees * Mathf.Deg2Rad;
                var dir = new Vector3(-Mathf.Sin(a), 0f, Mathf.Cos(a));          // left to right as seen from the centre
                verts[i * 2] = dir * Radius;
                verts[i * 2 + 1] = dir * Radius + Vector3.up * Height;
                uvs[i * 2] = new Vector2(1f - u, 0f);
                uvs[i * 2 + 1] = new Vector2(1f - u, 1f);
            }
            for (int i = 0; i < segments; i++)
            {
                int v = i * 2, t = i * 6;
                tris[t] = v; tris[t + 1] = v + 1; tris[t + 2] = v + 2;
                tris[t + 3] = v + 1; tris[t + 4] = v + 3; tris[t + 5] = v + 2;
            }
            mesh.Clear();
            mesh.vertices = verts; mesh.uv = uvs; mesh.triangles = tris;
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            // Inward-facing check: the normal at the middle must point back at the centre.
            var mid = segments / 2 * 2;
            if (Vector3.Dot(mesh.normals[mid], -verts[mid]) < 0f)
            {
                for (int t = 0; t < tris.Length; t += 3) (tris[t + 1], tris[t + 2]) = (tris[t + 2], tris[t + 1]);
                mesh.triangles = tris;
                mesh.RecalculateNormals();
            }
        }

        /// <summary>A soft storybook placeholder: warm sky, a few clouds, rolling green hills, until the painted world arrives.</summary>
        static void WritePlaceholder(string path)
        {
            const int w = 2048, h = 512;
            var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
            var px = new Color[w * h];
            Color skyTop = new Color32(120, 190, 240, 255), skyLow = new Color32(255, 236, 200, 255);
            Color far = new Color32(150, 200, 170, 255), mid = new Color32(110, 180, 110, 255), near = new Color32(86, 160, 84, 255);
            for (int x = 0; x < w; x++)
            {
                float u = x / (float)w * Mathf.PI * 2f;
                float hFar = 0.36f + 0.05f * Mathf.Sin(u * 3f + 1f) + 0.03f * Mathf.Sin(u * 7f);
                float hMid = 0.30f + 0.06f * Mathf.Sin(u * 2f + 2.5f) + 0.025f * Mathf.Sin(u * 9f + 1f);
                float hNear = 0.22f + 0.05f * Mathf.Sin(u * 4f + 0.7f) + 0.02f * Mathf.Sin(u * 13f);
                for (int y = 0; y < h; y++)
                {
                    float v = y / (float)h;
                    Color c = Color.Lerp(skyLow, skyTop, Mathf.SmoothStep(0.25f, 1f, v));
                    float cloud = Mathf.PerlinNoise(x * 0.004f, y * 0.012f) * Mathf.SmoothStep(0.55f, 0.9f, v);
                    c = Color.Lerp(c, Color.white, Mathf.SmoothStep(0.32f, 0.5f, cloud) * 0.8f);
                    if (v < hFar) c = Color.Lerp(far, c, 0.25f);
                    if (v < hMid) c = mid * (0.92f + 0.08f * Mathf.PerlinNoise(x * 0.02f, y * 0.02f));
                    if (v < hNear) c = near * (0.9f + 0.1f * Mathf.PerlinNoise(x * 0.03f + 5f, y * 0.03f));
                    c.a = 1f;
                    px[y * w + x] = c;
                }
            }
            tex.SetPixels(px);
            tex.Apply();
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
        }
    }
}
