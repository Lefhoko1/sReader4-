using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using SReader.Game.Classroom;
using SReader.Game.Trek;
using SReader.Domains.Assignments.Models;
using Object=UnityEngine.Object;
namespace SReader.EditorTools
{
 public sealed class ClassroomBlenderImport:AssetPostprocessor
 {
  public const string ModelPath="Assets/Art/MagicalClassroom/Models/SM_MagicalClassroom.fbx";
  void OnPreprocessModel()
  {
   if(assetPath!=ModelPath)return;
   var importer=(ModelImporter)assetImporter;importer.globalScale=1;importer.useFileScale=true;importer.importAnimation=false;importer.importCameras=false;importer.importLights=false;importer.isReadable=false;importer.meshCompression=ModelImporterMeshCompression.Off;importer.importNormals=ModelImporterNormals.Import;importer.materialImportMode=ModelImporterMaterialImportMode.ImportStandard;importer.optimizeMeshVertices=true;importer.optimizeMeshPolygons=true;
  }
  Material OnAssignMaterialModel(Material source,Renderer renderer)
  {
   if(assetPath!=ModelPath)return null;
   if(source.name=="SR_Classroom_Chalkboard")return AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/MagicalClassroom/M_Chalkboard.mat");
   return AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/LibraryRedesign/Materials/"+source.name+".mat");
  }
 }
 public static partial class ClassroomProductionTools
 {
  public static void ImportSource()
  {
   if(EditorSceneManager.GetActiveScene().path!=ClassroomSceneTools.ScenePath)EditorSceneManager.OpenScene(ClassroomSceneTools.ScenePath);
   if(GameObject.Find("Blender Classroom Geometry")) { ClassroomSceneTools.EnsureFolder("Assets/Scenes/_RedesignBackups"); EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(),"Assets/Scenes/_RedesignBackups/Classroom_FBX_Import_Attempt_"+DateTime.Now.ToString("yyyyMMdd_HHmmss")+".unity",true); EditorSceneManager.OpenScene(ClassroomSceneTools.ScenePath,OpenSceneMode.Single); }
   AssetDatabase.ImportAsset(ClassroomBlenderImport.ModelPath,ImportAssetOptions.ForceUpdate|ImportAssetOptions.ForceSynchronousImport);
   var existingSource=GameObject.Find("Blender Classroom Geometry");
   if(existingSource)
   {
    var transforms=existingSource.GetComponentsInChildren<Transform>(true);
    var currentBoard=Object.FindAnyObjectByType<ClassroomBoard>();
    var socket=transforms.FirstOrDefault(t=>t.name.StartsWith("SOCKET_Chalkboard_Live_Text"));
    if(!currentBoard||!socket)throw new InvalidOperationException("Existing classroom is missing its live board or Blender socket.");
    currentBoard.GetComponentInChildren<Canvas>().transform.position=socket.position;
    var currentRig=Object.FindAnyObjectByType<ClassroomCameraRig>();currentRig.roof=transforms.Single(t=>t.name=="Roof").gameObject;
    if(existingSource.GetComponentsInChildren<MeshRenderer>(true).Any(r=>AssetDatabase.GetAssetPath(r.GetComponent<MeshFilter>().sharedMesh)!=ClassroomBlenderImport.ModelPath))throw new InvalidOperationException("Classroom contains geometry outside the source FBX.");
    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());ClassroomSceneTools.CapturePortraits();Debug.Log("[Classroom] Existing linked Blender source refreshed; live assignment components retained.");return;
   }
   var asset=AssetDatabase.LoadAssetAtPath<GameObject>(ClassroomBlenderImport.ModelPath);if(!asset)throw new InvalidOperationException("FBX failed to import.");
   var sourceRoot=new GameObject("Blender Classroom Geometry");var model=(GameObject)PrefabUtility.InstantiatePrefab(asset);model.transform.SetParent(sourceRoot.transform,false);
   Transform Find(string name) { var transforms=model.GetComponentsInChildren<Transform>(true); return transforms.FirstOrDefault(t=>t.name==name) ?? transforms.Single(t=>t.name.StartsWith(name+".")); }
   Vector3 origin=Find("SR_ANCHOR_ORIGIN").position;
   var imported=Matrix4x4.identity;imported.SetColumn(0,Find("SR_ANCHOR_X").position-origin);imported.SetColumn(1,Find("SR_ANCHOR_Y").position-origin);imported.SetColumn(2,Find("SR_ANCHOR_Z").position-origin);imported.SetColumn(3,new Vector4(origin.x,origin.y,origin.z,1));
   imported[3,0]=imported[3,1]=imported[3,2]=0;
   var wanted=Matrix4x4.identity;wanted.SetColumn(0,new Vector4(1,0,0,0));wanted.SetColumn(1,new Vector4(0,0,1,0));wanted.SetColumn(2,new Vector4(0,1,0,0));
   Matrix4x4 correction=wanted*imported.inverse;var q=Quaternion.LookRotation(correction.GetColumn(2),correction.GetColumn(1));
   sourceRoot.transform.SetPositionAndRotation(correction.GetColumn(3),q);
   sourceRoot.transform.localScale=new Vector3(Vector3.Dot(correction.GetColumn(0),q*Vector3.right),((Vector3)correction.GetColumn(1)).magnitude,((Vector3)correction.GetColumn(2)).magnitude);
   float error=Mathf.Max(Vector3.Distance(Find("SR_ANCHOR_X").position,Vector3.right),Vector3.Distance(Find("SR_ANCHOR_Y").position,Vector3.forward),Vector3.Distance(Find("SR_ANCHOR_Z").position,Vector3.up));
   if(error>.002f)throw new InvalidOperationException("FBX axis/unit alignment failed: "+error);
   var rig=Object.FindAnyObjectByType<ClassroomCameraRig>();
   var legacy=GameObject.Find("sReader \u2014 Great Library");legacy.name="Legacy Native Mesh Geometry \u2014 Backed Up";legacy.SetActive(false);
   var board=Object.FindAnyObjectByType<ClassroomBoard>();var oldBoard=board.gameObject;
   var live=new GameObject("Chalkboard Live Assignment");live.transform.SetPositionAndRotation(new Vector3(4.24f,2.25f,.1f),Quaternion.Euler(0,90,0));
   var newBoard=live.AddComponent<ClassroomBoard>();EditorUtility.CopySerialized(board,newBoard);
   var canvas=oldBoard.GetComponentInChildren<Canvas>();canvas.transform.SetParent(live.transform,true);canvas.transform.position=Find("SOCKET_Chalkboard_Live_Text").position;canvas.transform.rotation=Quaternion.Euler(0,90,0);canvas.transform.localScale=Vector3.one*.005f;
   var collider=live.AddComponent<BoxCollider>();collider.size=new Vector3(6.35f,3.25f,.3f);collider.center=new Vector3(0,0,-.1f);live.AddComponent<ClassroomTapTarget>().kind=ClassroomTapTarget.Kind.Board;
   oldBoard.name="Legacy Unity Chalkboard Geometry \u2014 Backed Up";oldBoard.SetActive(false);
   newBoard.rig=rig;newBoard.Bind(TrailBuilder.Build("classroom-practice","Meadow at Dawn",AssignmentContentCodec.FromJson(newBoard.previewContent.text)),null);
   rig.roof=Find("Roof").gameObject;rig.frontCutaway=null;rig.poses[0].target=new Vector3(.45f,1.5f,1.2f);
   foreach(var renderer in sourceRoot.GetComponentsInChildren<MeshRenderer>(true))
   {
    if(renderer.sharedMaterials.Any(m=>!m||m.shader.name.Contains("InternalError")))throw new InvalidOperationException("Missing URP material on "+renderer.name);
    if(renderer.name=="SR_Wood_Floor")renderer.gameObject.AddComponent<BoxCollider>();
    if(renderer.name.StartsWith("SR_WORD_STONE_")&&!renderer.name.Contains("GOLD")){var target=renderer.gameObject.AddComponent<ClassroomTapTarget>();target.kind=ClassroomTapTarget.Kind.Word;target.wordIndex=int.Parse(renderer.name.Substring(renderer.name.LastIndexOf('_')+1));renderer.gameObject.AddComponent<BoxCollider>();}
   }
   var shelves=sourceRoot.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.name.StartsWith("SR_Bookshelf_Module_")).ToArray();
   if(shelves.Length!=4||shelves.Any(r=>Mathf.Abs(r.bounds.center.x+3.55f)>.02f))throw new InvalidOperationException("Imported shelving is not on the source west wall.");
   rig.GoTo(0,true);AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
   File.WriteAllText(Path.Combine(ClassroomSceneTools.Output,"reports/unity_fbx_import.json"),JsonUtility.ToJson(new SourceReport{blend="sReader_Classroom_v01.blend",model=ClassroomBlenderImport.ModelPath,renderers=sourceRoot.GetComponentsInChildren<MeshRenderer>(true).Length,shelfModules=shelves.Length,axisAlignmentError=error,boardSocket=Find("SOCKET_Chalkboard_Live_Text").position,linkedModel=PrefabUtility.IsPartOfPrefabInstance(model)},true));
   File.WriteAllText(Path.Combine(ClassroomSceneTools.Output,"reports/error.txt"),"");ClassroomSceneTools.CapturePortraits();Debug.Log("[Classroom] Scene now renders the linked Blender FBX. Legacy Unity-built geometry retained inactive.");
  }
  [Serializable]class SourceReport{public string blend,model;public int renderers,shelfModules;public float axisAlignmentError;public Vector3 boardSocket;public bool linkedModel;}
 }
}
