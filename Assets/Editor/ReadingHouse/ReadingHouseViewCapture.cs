using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using SReader.Game.ReadingHouse;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// Renders the Reading House walk-in flow at 1520 × 720 without entering Play
    /// mode: the opening shot (whole front, student outside), the closed doors
    /// with the sign-in panels, walking inside, the chalkboard, and the reading
    /// view. Output: GeneratedAssets/ReadingHouseUnity/wholehouse. The scene is
    /// reloaded/closed afterwards, so nothing in it changes.
    /// </summary>
    public static class ReadingHouseViewCapture
    {
        static readonly string Out = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "GeneratedAssets", "ReadingHouseUnity", "wholehouse"));

        [MenuItem("Tools/sReader/Reading House/Capture Walk-in Views")]
        public static void Run()
        {
            Directory.CreateDirectory(Out);
            foreach (var f in Directory.GetFiles(Out, "*.png")) File.Delete(f);
            var (scene, discard) = ReadingHouseSceneBuilder.OpenForCheck();
            var rt = new RenderTexture(1520, 720, 24);
            try
            {
                var roots = scene.GetRootGameObjects();
                var presenter = roots.Select(r => r.GetComponentInChildren<ReadingHousePresenter>(true)).First(p => p != null);
                var director = roots.Select(r => r.GetComponentInChildren<ReadingHouseExploreDirector>(true)).First(d => d != null);
                var cam = presenter.cameraRig.viewCamera;
                var student = presenter.studentMover.student;
                var door = director.door;
                presenter.dashboard.SetControlLabels(presenter.pointsLabel, presenter.menuLabel, presenter.playWithOthersLabel);
                presenter.dashboard.SetIllustration(presenter.sampleIllustration);
                presenter.dashboard.SetDialogueVisible(false);
                presenter.LoadFromBestSource();   // editor preview content; also fills the chalkboard

                cam.usePhysicalProperties = false;
                cam.lensShift = Vector2.zero;
                var outward = Vector3.ProjectOnPlane(door.transform.position - door.houseCentre.position, Vector3.up).normalized;

                // 1. Opening shot: the whole front, student beside the path.
                student.SetPositionAndRotation(director.studentStart.position, director.studentStart.rotation);
                Look(cam, director.exteriorCameraPos.position, director.exteriorCameraAim.position, director.exteriorFov);
                Render(cam, rt, "01_start_whole_house_student_outside.png");

                // 2. Walked up to the closed doors and the sign-in panels.
                student.position = door.transform.position + outward * 1.9f;
                Look(cam, door.transform.position + outward * 6.2f + Vector3.up * 2.5f, door.transform.position + Vector3.up * 1.6f, 48f);
                Render(cam, rt, "02_at_the_door_closed_signin_panels.png");

                // 3. After sign-in: doors swung open, student walked in (follow-camera framing, inside the room).
                door.leftHinge.rotation = Quaternion.AngleAxis(95f, Vector3.up) * door.leftHinge.rotation;
                door.rightHinge.rotation = Quaternion.AngleAxis(-95f, Vector3.up) * door.rightHinge.rotation;
                Look(cam, door.transform.position + outward * 6.5f + Vector3.up * 2.4f, door.transform.position + Vector3.up * 1.4f, 46f);
                student.position = door.transform.position + outward * 1.6f;
                Render(cam, rt, "03a_after_signin_doors_open.png");
                student.SetPositionAndRotation(presenter.studentMover.overviewSpot.position + outward * 0.4f, presenter.studentMover.overviewSpot.rotation);
                var focus = student.position + Vector3.up * 1.7f;
                Look(cam, student.position + outward * 2.6f + Vector3.up * 2.2f, focus, 60f);
                Render(cam, rt, "03b_inside_walking_follow_camera.png");

                // 4. Walked to the right wall: the chalkboard.
                var chalk = presenter.chalkboard.transform;
                var intoWall = chalk.forward;
                student.position = new Vector3(chalk.position.x, student.position.y, chalk.position.z) - intoWall * 1.6f + outward * 0.9f;
                student.rotation = Quaternion.LookRotation(intoWall, Vector3.up) * Quaternion.Inverse(Quaternion.LookRotation(-outward, Vector3.up)) * presenter.studentMover.overviewSpot.rotation;
                Look(cam, chalk.position - intoWall * 3.4f + outward * 1.5f + Vector3.down * 0.6f, chalk.position + Vector3.down * 0.6f, 58f);
                Render(cam, rt, "04_chalkboard_right_wall.png");

                // 5. Reading view (existing gameplay camera).
                presenter.studentMover.ToGameplay(immediate: true);
                presenter.cameraRig.GoTo(ReadingHouseView.Gameplay, immediate: true);
                Render(cam, rt, "05_reading_view.png");
            }
            finally
            {
                rt.Release();
                Object.DestroyImmediate(rt);
                discard();
            }
        }

        static void Look(Camera cam, Vector3 pos, Vector3 aim, float fov)
        {
            cam.usePhysicalProperties = false;
            cam.lensShift = Vector2.zero;
            cam.transform.SetPositionAndRotation(pos, Quaternion.LookRotation(aim - pos, Vector3.up));
            cam.fieldOfView = fov;
        }

        static void Render(Camera cam, RenderTexture rt, string file)
        {
            var request = new UniversalRenderPipeline.SingleCameraRequest { destination = rt };
            if (RenderPipeline.SupportsRenderRequest(cam, request)) RenderPipeline.SubmitRenderRequest(cam, request);
            var prior = RenderTexture.active; RenderTexture.active = rt;
            var tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0); tex.Apply();
            RenderTexture.active = prior;
            File.WriteAllBytes(Path.Combine(Out, file), tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
        }
    }
}
