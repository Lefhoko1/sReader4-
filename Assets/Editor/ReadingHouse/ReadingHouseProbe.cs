using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using SReader.Game.ReadingHouse;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>Logs where the key pieces of SC_ReadingHouse_Integration are (rug, dashboard display, spots, camera poses).</summary>
    public static class ReadingHouseProbe
    {
        public static void Run()
        {
            var scene = SceneManager.GetSceneByPath(ReadingHouseSceneBuilder.ScenePath);
            bool opened = false;
            if (!scene.IsValid() || !scene.isLoaded) { scene = EditorSceneManager.OpenScene(ReadingHouseSceneBuilder.ScenePath, OpenSceneMode.Additive); opened = true; }
            try
            {
                var all = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();
                void B(string prefix)
                {
                    foreach (var r in all.Select(t => t.GetComponent<Renderer>()).Where(r => r != null && r.name.StartsWith(prefix)).Take(4))
                        Debug.Log($"[Probe] {r.name}: centre {r.bounds.center:F2} size {r.bounds.size:F2} (parent {r.transform.parent?.name})");
                }
                B("SR_Rug"); B("SR_DISPLAY"); B("SR_DB_Tray"); B("SR_Wood_Floor"); B("Chair_Seat");
                var p = all.Select(t => t.GetComponent<ReadingHousePresenter>()).First(x => x != null);
                Debug.Log($"[Probe] gameplay spot {p.studentMover.gameplaySpot.position:F2} fwd {p.studentMover.gameplaySpot.forward:F2}; overview spot {p.studentMover.overviewSpot.position:F2}");
                foreach (var pose in p.cameraRig.poses)
                    if (pose?.position != null) Debug.Log($"[Probe] camera {pose.view}: at {pose.position.position:F2} aim {pose.aim.position:F2} fov {pose.referenceVerticalFov}");
                Debug.Log($"[Probe] tray at {p.tray.transform.position:F2}; dashboard at {p.dashboard.transform.position:F2}");
            }
            finally { if (opened) EditorSceneManager.CloseScene(scene, true); }
        }
    }
}
