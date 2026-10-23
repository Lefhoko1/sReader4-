using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using SReader.Game.ReadingHouse;
using Object = UnityEngine.Object;

namespace SReader.EditorTools.ReadingHouse
{
    // Visual-only tooling. Never calls scene generators, loads sample content, or submits an attempt.
    [InitializeOnLoad]
    public static partial class ReadingHouseLightingPolish
    {
        const string ScenePath = "Assets/Scenes/SC_ReadingHouse_Integration.unity";
        const string Art = "Assets/Art/ReadingHouse/Lighting";
        const string Output = "GeneratedAssets/ReadingHousePolish/20261023";
        const string Command = Output + "/command.txt";
        static double next;
        static ReadingHouseLightingPolish() { EditorApplication.update += Poll; }
        static void Poll()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.timeSinceStartup < next) return;
            next = EditorApplication.timeSinceStartup + .5;
            if (!File.Exists(Command)) return;
            var cmd = File.ReadAllText(Command).Trim(); File.Delete(Command);
            try
            {
                if (cmd == "audit") Audit();
                else if (cmd.StartsWith("capture ")) Capture(cmd.Substring(8));
                else if (cmd.StartsWith("play ")) StartPlayCapture(cmd.Substring(5));
                else if (cmd == "stop") EditorApplication.isPlaying = false;
                else RunPolish(cmd);
                File.WriteAllText(Output + "/result.txt", "OK " + cmd);
            }
            catch (Exception ex) { File.WriteAllText(Output + "/result.txt", "FAILED " + cmd + "\n" + ex); Debug.LogException(ex); }
        }
        static Scene House()
        {
            var scene = SceneManager.GetActiveScene();
            if (scene.path != ScenePath) throw new InvalidOperationException("Only the existing Reading House scene may be used.");
            return scene;
        }
        static T[] All<T>() where T : Component => House().GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<T>(true)).ToArray();
        static string P(Transform t) => t.parent == null ? t.name : P(t.parent) + "/" + t.name;
        static void Audit()
        {
            var lines = new List<string> { "playing=" + EditorApplication.isPlaying + " dirty=" + House().isDirty,
                "GPU=" + SystemInfo.graphicsDeviceName, "pipeline=" + AssetDatabase.GetAssetPath(GraphicsSettings.currentRenderPipeline),
                "ambient=" + RenderSettings.ambientMode + " sky=" + RenderSettings.ambientSkyColor + " equator=" + RenderSettings.ambientEquatorColor + " ground=" + RenderSettings.ambientGroundColor };
            foreach (var l in All<Light>()) lines.Add($"LIGHT {P(l.transform)} position={l.transform.position:F3} rotation={l.transform.eulerAngles:F2} type={l.type} bake={l.lightmapBakeType} color={l.color} intensity={l.intensity} range={l.range} shadows={l.shadows}");
            foreach (var c in All<Camera>()) lines.Add($"CAM {P(c.transform)} renderer={EditorJsonUtility.ToJson(c.GetUniversalAdditionalCameraData())}");
            foreach (var r in All<Renderer>())
                if (r.name.Contains("Rug") || r.name.Contains("Floor") || r.name.Contains("Chair_Seat") || r.name.Contains("Lamp") || r.name.Contains("Window") || r is SpriteRenderer)
                    lines.Add($"RENDERER {P(r.transform)} bounds={r.bounds.center:F3}/{r.bounds.size:F3} enabled={r.enabled} static={GameObjectUtility.GetStaticEditorFlags(r.gameObject)} mats={string.Join(",", r.sharedMaterials.Select(m => m == null ? "null" : m.name))}");
            foreach (var m in All<Renderer>().SelectMany(r => r.sharedMaterials).Where(m => m != null).Distinct().OrderBy(m => m.name))
                lines.Add($"MAT {m.name} shader={m.shader.name} path={AssetDatabase.GetAssetPath(m)} color={(m.HasProperty("_BaseColor") ? m.GetColor("_BaseColor").ToString() : "-")} smooth={(m.HasProperty("_Smoothness") ? m.GetFloat("_Smoothness").ToString() : "-")} emission={(m.HasProperty("_EmissionColor") ? m.GetColor("_EmissionColor").ToString() : "-")}");
            File.WriteAllLines(Output + "/audit.txt", lines);
        }
        static void Shot(Camera cam, string path)
        {
            var rt = RenderTexture.GetTemporary(1520, 720, 24, RenderTextureFormat.ARGBHalf);
            var active = RenderTexture.active;
            var target = cam.targetTexture;
            try
            {
                cam.aspect = 1520f / 720f;
                var request = new UniversalRenderPipeline.SingleCameraRequest { destination = rt };
                if (!RenderPipeline.SupportsRenderRequest(cam, request)) throw new InvalidOperationException("URP camera capture not supported");
                RenderPipeline.SubmitRenderRequest(cam, request);
                RenderTexture.active = rt;
                var tex = new Texture2D(1520, 720, TextureFormat.RGB24, false);
                tex.ReadPixels(new Rect(0, 0, 1520, 720), 0, 0); tex.Apply();
                File.WriteAllBytes(path, tex.EncodeToPNG()); Object.DestroyImmediate(tex);
            }
            finally { cam.targetTexture = target; RenderTexture.active = active; RenderTexture.ReleaseTemporary(rt); }
        }
        // Uses the approved rig / follow projections. All temporary preview state is restored without saving.
        static void Capture(string stage)
        {
            if (EditorApplication.isPlaying) throw new InvalidOperationException("Editor previews require Edit mode");
            var scene = House(); bool dirty = scene.isDirty;
            var transforms = All<Transform>().Select(t => (t, t.localPosition, t.localRotation, t.localScale)).ToArray();
            var renderers = All<Renderer>().Select(r => (r, r.enabled)).ToArray();
            var cam = All<ReadingHouseCameraRig>().Single().viewCamera;
            string cameraJson = EditorJsonUtility.ToJson(cam);
            var dir = Output + "/" + stage; Directory.CreateDirectory(dir);
            var d = All<ReadingHouseExploreDirector>().Single(); var p = d.presenter;
            var stats = new List<string> { "1520x720 editor camera renders; no sample content loaded. Activity/chalkboard surfaces are geometry previews, not signed-in UI.", "UnityStats are editor counters at capture time, not Android performance." };
            void Save(string name)
            {
                Shot(cam, dir + "/" + name + ".png");
                stats.Add($"{name}: batches={UnityStats.drawCalls}, setPass={UnityStats.setPassCalls}, triangles={UnityStats.triangles}, vertices={UnityStats.vertices}, renderTime={UnityStats.renderTime * 1000:F2}ms");
            }
            void Student(Vector3 pos, Quaternion rot)
            {
                p.studentMover.student.SetPositionAndRotation(pos, rot);
                d.studentSprite.transform.SetPositionAndRotation(pos, Quaternion.Euler(0, cam.transform.eulerAngles.y, 0));
            }
            try
            {
                cam.aspect = 1520f / 720f;
                foreach (var a in All<ActorSpriteAnimator>()) if (a.model3D != null) foreach (var r in a.model3D.GetComponentsInChildren<Renderer>(true)) r.enabled = false;
                foreach (var r in p.studentMover.student.GetComponentsInChildren<Renderer>(true)) r.enabled = false;
                d.follow.SnapToPose(d.exteriorCameraPos.position, d.exteriorCameraAim.position);
                Student(d.studentStart.position, d.studentStart.rotation); Save("01_exterior");
                foreach (var cut in All<FrontWallCutaway>()) { foreach (var r in cut.renderers) if (r != null) r.enabled = false; foreach (var l in cut.labels) if (l != null) l.enabled = false; }
                var pos = p.studentMover.overviewSpot.position;
                var fwd = Vector3.ProjectOnPlane(d.follow.viewDirection, Vector3.up).normalized;
                d.follow.SnapToPose(pos - fwd * d.follow.distance + Vector3.up * d.follow.height, pos + Vector3.up * d.follow.lookHeight);
                Student(pos, p.studentMover.overviewSpot.rotation); Save("02_follow_inside");
                p.cameraRig.GoTo(ReadingHouseView.Gameplay, true);
                Student(p.studentMover.gameplaySpot.position, p.studentMover.gameplaySpot.rotation); Save("03_reading");
                p.cameraRig.GoTo(ReadingHouseView.TrayFocus, true); Save("04_activity_surface_preview");
                var slate = d.chalkboardScreen.slate.bounds;
                bool thinX = slate.extents.x < slate.extents.z;
                var normal = thinX ? Vector3.right : Vector3.forward;
                if (Vector3.Dot(d.door.houseCentre.position - slate.center, normal) < 0) normal = -normal;
                float width = 2f * (thinX ? slate.extents.z : slate.extents.x), height = slate.size.y;
                float vfov = d.follow.fieldOfView * Mathf.Deg2Rad;
                float hfov = 2 * Mathf.Atan(Mathf.Tan(vfov * .5f) * cam.aspect);
                float dist = Mathf.Max(height / (2 * Mathf.Tan(vfov * .5f)), width / (2 * Mathf.Tan(hfov * .5f))) / d.boardFraming;
                d.follow.SnapToPose(slate.center + normal * dist, slate.center); Save("05_chalkboard_surface_preview");
                File.WriteAllLines(dir + "/stats.txt", stats);
            }
            finally
            {
                foreach (var s in transforms) { if (!s.t.localPosition.Equals(s.localPosition)) s.t.localPosition = s.localPosition; if (!s.t.localRotation.Equals(s.localRotation)) s.t.localRotation = s.localRotation; if (!s.t.localScale.Equals(s.localScale)) s.t.localScale = s.localScale; }
                foreach (var s in renderers) s.r.enabled = s.enabled;
                // TMP_Text.enabled is a component flag, independent of MeshRenderer.
                foreach (var cut in All<FrontWallCutaway>()) foreach (var l in cut.labels) if (l != null) l.enabled = true;
                EditorJsonUtility.FromJsonOverwrite(cameraJson, cam);
                d.follow.Release();
                if (!dirty) typeof(EditorSceneManager).GetMethod("ClearSceneDirtiness", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(null, new object[] { scene });
            }
        }
        static partial void RunPolish(string cmd);
        static partial void StartPlayCapture(string stage);
    }
}
