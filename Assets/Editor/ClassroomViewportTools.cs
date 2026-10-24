using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
namespace SReader.EditorTools
{
    public static partial class ClassroomProductionTools
    {
        public static void Finalise()
        {
            var rig=UnityEngine.Object.FindAnyObjectByType<SReader.Game.Classroom.ClassroomCameraRig>();
            rig.poses[0].position=new Vector3(0,3.6f,-3.9f);rig.poses[0].target=new Vector3(1.6f,1.4f,1.3f);rig.poses[0].fieldOfView=110;
            rig.poses[2].target=new Vector3(0,0,1);
            // Keep legacy objects as unreferenced prefabs instead of loading duplicate geometry on the phone.
            ClassroomSceneTools.EnsureFolder(ClassroomSceneTools.Art+"/Backup");
            var scene=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            foreach(var root in scene.GetRootGameObjects().Where(g=>g.name.StartsWith("Legacy ")).ToArray())
            {
                string path=ClassroomSceneTools.Art+"/Backup/"+(root.name.Contains("Chalkboard")?"P_LegacyChalkboard":"P_LegacyNativeLibrary")+".prefab";
                PrefabUtility.SaveAsPrefabAsset(root,path);UnityEngine.Object.DestroyImmediate(root);
            }
            rig.GoTo(0,true);UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();ClassroomSceneTools.CapturePortraits();
            File.WriteAllText(Path.Combine(ClassroomSceneTools.Output,"reports/final_cleanup.txt"),"Legacy scene geometry preserved as Backup prefabs and in scene checkpoints, excluded from the active scene to reduce loaded meshes. Current render geometry remains linked to the Blender FBX.");
        }
        public static void Viewport()
        {
            var type=typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");var window=EditorWindow.GetWindow(type);
            var flags=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance;
            var lines=type.GetProperties(flags).Where(p=>p.GetIndexParameters().Length==0&&new[]{"size","resolution","portrait","rotate","orientation","target"}.Any(k=>p.Name.IndexOf(k,StringComparison.OrdinalIgnoreCase)>=0)).Select(p=>{try{return p.Name+" : "+p.GetValue(window);}catch{return p.Name+" : unavailable";}}).ToList();
            lines.AddRange(type.GetFields(flags).Where(f=>new[]{"size","resolution","portrait","rotate","orientation"}.Any(k=>f.Name.IndexOf(k,StringComparison.OrdinalIgnoreCase)>=0)).Select(f=>f.Name+" : "+f.GetValue(window)));
            var current=type.GetProperty("currentGameViewSize",flags)?.GetValue(window);if(current!=null)foreach(var prop in current.GetType().GetProperties(flags)){try{lines.Add("Size."+prop.Name+" : "+prop.GetValue(current));}catch{}}
            lines.Add("Screen : "+Screen.width+" x "+Screen.height+"; "+Screen.orientation);
            File.WriteAllLines(Path.Combine(ClassroomSceneTools.Output,"reports/viewport.txt"),lines);
            var counters=new System.Collections.Generic.List<Unity.Profiling.LowLevel.Unsafe.ProfilerRecorderHandle>();Unity.Profiling.LowLevel.Unsafe.ProfilerRecorderHandle.GetAvailable(counters);
            File.WriteAllLines(Path.Combine(ClassroomSceneTools.Output,"reports/profiler_counters.txt"),counters.Select(h=>Unity.Profiling.LowLevel.Unsafe.ProfilerRecorderHandle.GetDescription(h).Name).Where(n=>n.IndexOf("Draw",StringComparison.OrdinalIgnoreCase)>=0||n.IndexOf("Batch",StringComparison.OrdinalIgnoreCase)>=0));
        }
    }
}
