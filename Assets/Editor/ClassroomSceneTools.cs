using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using SReader.Game.Classroom;
using SReader.Game.Trek;
using SReader.Domains.Assignments.Models;
using Object=UnityEngine.Object;
namespace SReader.EditorTools
{
 [InitializeOnLoad] public static class ClassroomSceneTools
 {
  public const string ScenePath="Assets/Scenes/SC_LibraryRedesignPreview.unity";
  public const string Art="Assets/Art/MagicalClassroom";
  static string Project=>Path.GetFullPath(Path.Combine(Application.dataPath,".."));
  public static string Output
  {
   get { string pointer=Path.Combine(Project,"GeneratedAssets/MagicalClassroom/active.txt");
    if(File.Exists(pointer)){string candidate=File.ReadAllText(pointer).Trim();if(Directory.Exists(candidate))return candidate;}
    return Path.Combine(Project,"GeneratedAssets/MagicalClassroom/20261017_195009"); }
  }
  static string Request=>Path.Combine(Output,"command.txt");
  static bool busy; static double next;
  static ClassroomSceneTools(){EditorApplication.update+=Poll;}
  static void Poll()
  {
   if(busy||EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.timeSinceStartup<next)return;
   next=EditorApplication.timeSinceStartup+.5;
   if(!File.Exists(Request))return;
   string cmd=File.ReadAllText(Request).Trim();
   if((cmd=="layout"||cmd=="integrate"||cmd=="optimise"||cmd=="android"||cmd=="source-import"||cmd=="bake")&&EditorApplication.isPlayingOrWillChangePlaymode){EditorApplication.isPlaying=false;return;}
   File.Delete(Request);busy=true;
   try{
    if(cmd=="refresh")AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    else if(cmd=="layout")BuildLayout();
    else if(cmd=="capture")CapturePortraits();
    else if(cmd=="play")EditorApplication.isPlaying=true;
    else if(cmd=="stop")EditorApplication.isPlaying=false;
    else if(cmd.StartsWith("view:")){var r=Object.FindAnyObjectByType<ClassroomCameraRig>();r.GoTo(int.Parse(cmd.Substring(5)),true);}
    else RunExtended(cmd);
   }catch(Exception e){File.WriteAllText(Path.Combine(Output,"reports/error.txt"),e.ToString());Debug.LogException(e);}
   finally{busy=false;}
  }
  static void RunExtended(string cmd)
  {
   // Integration and optimisation extensions are discovered only when explicitly requested.
   var name=cmd=="integrate"?"Integrate":cmd=="optimise"?"Optimise":cmd=="android"?"BuildAndroid":cmd=="verify"?"Verify":cmd=="profile"?"Profile":cmd=="bake"?"Bake":cmd=="finalise"?"Finalise":cmd=="portraits"?"Portraits":cmd=="viewport"?"Viewport":cmd=="refine"?"Refine":cmd=="source-import"?"ImportSource":null;
   if(name==null)return;
   var type=typeof(ClassroomSceneTools).Assembly.GetType("SReader.EditorTools.ClassroomProductionTools");
   if(type==null)throw new InvalidOperationException("Production extension not installed yet.");
   type.GetMethod(name,BindingFlags.Public|BindingFlags.Static).Invoke(null,null);
  }
  [MenuItem("Tools/sReader/Magical Classroom/Build Classroom Layout")]
  public static void BuildLayout()
  {
   if(Object.FindAnyObjectByType<ClassroomCameraRig>())throw new InvalidOperationException("Classroom already exists. Open the saved scene or reimport the Blender FBX.");
   if(EditorSceneManager.GetActiveScene().path!=ScenePath)EditorSceneManager.OpenScene(ScenePath);
   var old=Object.FindAnyObjectByType<SReader.EnvironmentPreview.LibraryRedesignPreview>();
   var cam=old?old.previewCamera:Camera.main;
   var roof=GameObject.Find("sReader — Great Library").transform.Find("Roof").gameObject;
   if(old)Object.DestroyImmediate(old);
   var existing=Object.FindAnyObjectByType<ClassroomCameraRig>();
   if(existing)throw new InvalidOperationException("Layout already exists; use saved scene instead of rebuilding it.");
   EnsureFolder(Art);
   var rig=new GameObject("Classroom Camera Rig").AddComponent<ClassroomCameraRig>();rig.viewCamera=cam;rig.roof=roof;
   rig.poses=new[]{
    Pose("Classroom",new Vector3(.7f,4.3f,-6.3f),new Vector3(.4f,1.6f,.45f),76),
    Pose("Chalkboard",new Vector3(-.9f,2.25f,.1f),new Vector3(4.1f,2.25f,.1f),56),
    Pose("Hero Book",new Vector3(0,4.1f,-1.7f),new Vector3(0,1.25f,.9f),66),
    Pose("Exterior",new Vector3(16,12,-23),new Vector3(0,2,0),64,true)};
   var root=GameObject.Find("sReader — Great Library");var architecture=root.transform.Find("Architecture");
   var front=new GameObject("Front Facade Cutaway");front.transform.SetParent(root.transform);
   foreach(var r in architecture.GetComponentsInChildren<MeshRenderer>(true).ToArray())
    if(r.bounds.center.z< -3.75f)r.transform.SetParent(front.transform,true);
   rig.frontCutaway=front;
   var shelves=root.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.name.StartsWith("SR_Bookshelf_Module_")).OrderBy(r=>r.name).ToArray();
   if(shelves.Length!=4)throw new InvalidOperationException("Expected four preserved shelf modules.");
   float[] positions={-2.76f,-.92f,.92f,2.76f};
   for(int i=0;i<shelves.Length;i++){var t=shelves[i].transform;t.position=new Vector3(-3.55f,t.position.y,positions[i]);t.rotation=Quaternion.Euler(0,t.name.Contains("East")?180:0,0);t.localScale=new Vector3(1,1,.74f);}
   var boardRoot=new GameObject("Classroom Chalkboard — East Wall");boardRoot.transform.position=new Vector3(4.24f,2.25f,.1f);boardRoot.transform.rotation=Quaternion.Euler(0,90,0);
   var green=Material("M_Chalkboard","263C32");var wood=Load("SR_Walnut");var gold=Load("SR_Gold");
   Box("Wall Mounted Board",boardRoot.transform,Vector3.zero,new Vector3(6.15f,3.03f,.12f),green);
   for(int side=-1;side<=1;side+=2){Box("Carved Frame",boardRoot.transform,new Vector3(side*3.11f,0,-.10f),new Vector3(.16f,3.25f,.17f),wood);Box("Carved Frame",boardRoot.transform,new Vector3(0,side*1.56f,-.10f),new Vector3(6.35f,.16f,.17f),wood);}
   for(int x=-1;x<=1;x+=2)for(int y=-1;y<=1;y+=2){Box("Antique Gold Corner",boardRoot.transform,new Vector3(x*3.10f,y*1.55f,-.20f),new Vector3(.21f,.21f,.035f),gold);Box("Inset Corner",boardRoot.transform,new Vector3(x*3.10f,y*1.55f,-.225f),new Vector3(.09f,.09f,.02f),green);}
   Box("Chalk Tray",boardRoot.transform,new Vector3(0,-1.62f,-.24f),new Vector3(5.8f,.11f,.36f),wood);
   Box("Chalk",boardRoot.transform,new Vector3(-.5f,-1.54f,-.25f),new Vector3(.21f,.055f,.055f),Load("SR_Paper"));
   var tap=boardRoot.AddComponent<ClassroomTapTarget>();tap.kind=ClassroomTapTarget.Kind.Board;
   var col=boardRoot.AddComponent<BoxCollider>();col.size=new Vector3(6.35f,3.25f,.3f);col.center=new Vector3(0,0,-.1f);
   var canvasObject=new GameObject("Chalk Lettering",typeof(RectTransform),typeof(Canvas),typeof(GraphicRaycaster));canvasObject.transform.SetParent(boardRoot.transform,false);canvasObject.transform.localPosition=new Vector3(0,0,-.18f);canvasObject.transform.localScale=Vector3.one*.005f;
   var canvas=canvasObject.GetComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;canvas.worldCamera=cam;canvas.sortingOrder=5;
   var rect=canvasObject.GetComponent<RectTransform>();rect.sizeDelta=new Vector2(480,580);
   var board=boardRoot.AddComponent<ClassroomBoard>();board.rig=rig;board.previewContent=AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/Books/SampleBook_Nature_MeadowAtDawn.json");
   Label(canvas.transform,"TODAY'S READING",new Vector2(0,252),new Vector2(460,32),19);
   board.assignmentTitle=Label(canvas.transform,"",new Vector2(0,190),new Vector2(460,95),29);
   Label(canvas.transform,"Words to restore",new Vector2(0,122),new Vector2(460,30),21);
   board.wordButtons=new Button[5];board.wordLabels=new TMP_Text[5];
   for(int i=0;i<5;i++)
   {
    var buttonObject=new GameObject("Word Touch Target "+i,typeof(RectTransform),typeof(Image),typeof(Button));buttonObject.transform.SetParent(canvas.transform,false);var rt=buttonObject.GetComponent<RectTransform>();rt.sizeDelta=new Vector2(440,56);rt.anchoredPosition=new Vector2(0,76-i*58);
    buttonObject.GetComponent<Image>().color=new Color(0,0,0,.035f);board.wordButtons[i]=buttonObject.GetComponent<Button>();var colors=board.wordButtons[i].colors;colors.highlightedColor=new Color(.75f,.78f,.65f,.3f);board.wordButtons[i].colors=colors;
    board.wordLabels[i]=Label(buttonObject.transform,"",Vector2.zero,new Vector2(410,54),28);
   }
   board.completion=Label(canvas.transform,"",new Vector2(0,-216),new Vector2(460,32),21);
   board.notice=Label(canvas.transform,"",new Vector2(0,-247),new Vector2(470,26),15);
   board.pagination=Label(canvas.transform,"",new Vector2(0,-279),new Vector2(470,27),14);
   board.pagination.raycastTarget=true;var more=board.pagination.gameObject.AddComponent<Button>();more.onClick.AddListener(board.NextPage);
   board.Bind(TrailBuilder.Build("classroom-practice","Meadow at Dawn",AssignmentContentCodec.FromJson(board.previewContent.text)),null);
   foreach(int side in new[]{-1,1})
   {
    var ornament=new GameObject("Chalk Constellation",typeof(TextMeshPro));ornament.transform.SetParent(boardRoot.transform,false);ornament.transform.localPosition=new Vector3(side*2.15f,.15f,-.19f);ornament.transform.localRotation=Quaternion.Euler(0,180,0);var tmp=ornament.GetComponent<TextMeshPro>();tmp.font=TMP_Settings.defaultFontAsset;tmp.fontSize=2.2f;tmp.alignment=TextAlignmentOptions.Center;tmp.color=new Color(.65f,.70f,.54f);tmp.text=side<0?"READ\n.\nDISCOVER":"WORDS\n.\nRESTORE";tmp.rectTransform.sizeDelta=new Vector2(1.3f,1.5f);
   }
   var bookHit=new GameObject("Hero Book Interaction");bookHit.transform.position=new Vector3(0,1.65f,1);var bc=bookHit.AddComponent<BoxCollider>();bc.size=new Vector3(2.3f,.55f,1.4f);bookHit.AddComponent<ClassroomTapTarget>().kind=ClassroomTapTarget.Kind.Book;
   var lexicon=new GameObject("Lexicon Interaction");lexicon.transform.position=new Vector3(2,1.2f,2.8f);lexicon.AddComponent<BoxCollider>().size=new Vector3(1.2f,1.6f,1.2f);lexicon.AddComponent<ClassroomTapTarget>().kind=ClassroomTapTarget.Kind.Lexicon;
   foreach(var r in root.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.name.StartsWith("SR_WORD_STONE_")&&!r.name.Contains("GOLD"))) { var t=r.gameObject.AddComponent<ClassroomTapTarget>();t.kind=ClassroomTapTarget.Kind.Word;t.wordIndex=int.Parse(r.name.Substring(r.name.LastIndexOf('_')+1));r.gameObject.AddComponent<BoxCollider>(); }
   if(!Object.FindAnyObjectByType<EventSystem>()){var ev=new GameObject("Classroom Touch Events",typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
    ev.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>().AssignDefaultActions();
#else
    ev.AddComponent<StandaloneInputModule>();
#endif
   }
   rig.GoTo(0,true);EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
   SetGameView(720,1520,"Huawei Y5 2019 Portrait");
   File.WriteAllText(Path.Combine(Output,"reports/layout.json"),JsonUtility.ToJson(new LayoutReport{scene=ScenePath,shelfModules=shelves.Length,boardMountedX=boardRoot.transform.position.x,boardSize="6.35 x 3.25 m frame; 2.4 x 2.9 m readable centre",stage="B-C layout saved"},true));
   Debug.Log("[Classroom] Layout and portrait camera poses saved. Capture next before integrating activities.");
  }
  [Serializable] class LayoutReport{public string scene,boardSize,stage;public int shelfModules;public float boardMountedX;}
  static ClassroomCameraRig.Pose Pose(string name,Vector3 pos,Vector3 target,float fov,bool roof=false)=>new ClassroomCameraRig.Pose{name=name,position=pos,target=target,fieldOfView=fov,roofVisible=roof};
  public static void EnsureFolder(string path){if(AssetDatabase.IsValidFolder(path))return;EnsureFolder(Path.GetDirectoryName(path).Replace('\\','/'));AssetDatabase.CreateFolder(Path.GetDirectoryName(path).Replace('\\','/'),Path.GetFileName(path));}
  static Material Load(string name)=>AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/LibraryRedesign/Materials/"+name+".mat");
  public static Material Material(string name,string hex)
  {
   string path=Art+"/"+name+".mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);if(mat)return mat;
   ColorUtility.TryParseHtmlString("#"+hex,out var color);mat=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name,color=color};mat.SetFloat("_Smoothness",.1f);AssetDatabase.CreateAsset(mat,path);return mat;
  }
  static GameObject Box(string name,Transform parent,Vector3 pos,Vector3 size,Material material){var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=pos;go.transform.localScale=size;go.GetComponent<MeshRenderer>().sharedMaterial=material;Object.DestroyImmediate(go.GetComponent<Collider>());return go;}
  static TMP_Text Label(Transform parent,string text,Vector2 pos,Vector2 size,float font)
  {
   var go=new GameObject("Chalk "+(text.Length>0?text:"Label"),typeof(RectTransform),typeof(TextMeshProUGUI));go.transform.SetParent(parent,false);var rt=go.GetComponent<RectTransform>();rt.anchoredPosition=pos;rt.sizeDelta=size;var tmp=go.GetComponent<TextMeshProUGUI>();tmp.font=TMP_Settings.defaultFontAsset;tmp.text=text;tmp.fontSize=font;tmp.color=new Color(.95f,.94f,.87f);tmp.alignment=TextAlignmentOptions.Center;tmp.raycastTarget=false;return tmp;
  }
  public static void CapturePortraits()
  {
   var rig=Object.FindAnyObjectByType<ClassroomCameraRig>();if(!rig)throw new Exception("Classroom rig missing.");
   int original=rig.currentView;var cam=rig.viewCamera;var texture=new RenderTexture(720,1520,24);texture.Create();
   try{
    cam.aspect=720f/1520;RenderPipeline.SubmitRenderRequest(cam,new UniversalRenderPipeline.SingleCameraRequest{destination=texture});
    for(int i=0;i<3;i++){rig.GoTo(i,true);Canvas.ForceUpdateCanvases();RenderPipeline.SubmitRenderRequest(cam,new UniversalRenderPipeline.SingleCameraRequest{destination=texture});var prior=RenderTexture.active;RenderTexture.active=texture;var image=new Texture2D(720,1520,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,720,1520),0,0);image.Apply();File.WriteAllBytes(Path.Combine(Output,"screenshots/"+(i+1).ToString("D2")+"_"+rig.poses[i].name.Replace(' ','_')+".png"),image.EncodeToPNG());Object.DestroyImmediate(image);RenderTexture.active=prior;}
   }finally{texture.Release();Object.DestroyImmediate(texture);cam.ResetAspect();rig.GoTo(original,true);}
   if(!Application.isPlaying)EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
   SetGameView(720,1520,"Huawei Y5 2019 Portrait");Debug.Log("[Classroom] Portrait overview, board and book captured.");
  }
  public static void SetGameView(int width,int height,string label)
  {
   var asm=typeof(EditorWindow).Assembly;var type=asm.GetType("UnityEditor.GameView");var window=EditorWindow.GetWindow(type);
   var sizes=asm.GetType("UnityEditor.GameViewSizes");var instance=typeof(ScriptableSingleton<>).MakeGenericType(sizes).GetProperty("instance").GetValue(null);var group=sizes.GetMethod("GetGroup").Invoke(instance,new[]{sizes.GetProperty("currentGroupType").GetValue(instance)});
   var texts=(string[])group.GetType().GetMethod("GetDisplayTexts").Invoke(group,null);int index=Array.FindIndex(texts,t=>t.Contains(label));
   if(index<0){var st=asm.GetType("UnityEditor.GameViewSize");var en=asm.GetType("UnityEditor.GameViewSizeType");var item=Activator.CreateInstance(st,Enum.ToObject(en,1),width,height,label);group.GetType().GetMethod("AddCustomSize").Invoke(group,new[]{item});index=texts.Length;}
   type.GetProperty("selectedSizeIndex",BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic)?.SetValue(window,index);window.Focus();window.Repaint();
  }
 }
}
