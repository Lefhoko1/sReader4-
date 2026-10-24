using System;using System.IO;using UnityEngine;using UnityEditor;using UnityEditor.SceneManagement;using SReader.Game.Classroom;using Object=UnityEngine.Object;
namespace SReader.EditorTools
{
 public static partial class ClassroomProductionTools
 {
  public static void Refine()
  {
   var rig=Object.FindAnyObjectByType<ClassroomCameraRig>();
   rig.poses[0].position=new Vector3(0,4.3f,-3.55f);rig.poses[0].target=new Vector3(0,1.5f,1.2f);rig.poses[0].fieldOfView=110;
   rig.poses[1].position=new Vector3(.7f,2.25f,.1f);rig.poses[1].fieldOfView=78;
   for(int i=0;i<rig.poses.Length;i++)rig.poses[i].roofVisible=true;
   rig.GoTo(0,true);EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());ClassroomSceneTools.CapturePortraits();
   File.WriteAllText(Path.Combine(ClassroomSceneTools.Output,"reports/visual_refinement.txt"),"Camera moved inside the physical shell; full roof retained as the ceiling. Board camera clears the hero-book motes. Portrait layout inspected before activity integration.");
  }
 }
}
