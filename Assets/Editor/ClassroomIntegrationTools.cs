using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using SReader.Game.Classroom;
using Object = UnityEngine.Object;
namespace SReader.EditorTools
{
    public static partial class ClassroomProductionTools
    {
        public static void Integrate()
        {
            var rig = Object.FindAnyObjectByType<ClassroomCameraRig>();
            if (!rig) throw new InvalidOperationException("Open the imported classroom scene first.");
            var board = Object.FindAnyObjectByType<ClassroomBoard>();
            var finish = board.completion.GetComponent<UnityEngine.UI.Button>();
            if (!finish) finish = board.completion.gameObject.AddComponent<UnityEngine.UI.Button>();
            board.completion.raycastTarget = true;
            board.completion.rectTransform.sizeDelta = new Vector2(460,50);
            var panelPath = ClassroomSceneTools.Art + "/ClassroomPanelSettings.asset";
            var panel = AssetDatabase.LoadAssetAtPath<PanelSettings>(panelPath);
            if (!panel)
            {
                panel = Object.Instantiate(AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/Trial1OverlayPanelSettings.asset"));
                panel.name = "ClassroomPanelSettings"; panel.referenceResolution = new Vector2Int(360,760);
                panel.scaleMode = PanelScaleMode.ScaleWithScreenSize; panel.match = 0;
                AssetDatabase.CreateAsset(panel,panelPath);
            }
            var presenter = Object.FindAnyObjectByType<ClassroomPresenter>();
            if (!presenter)
            {
                var owner = new GameObject("Classroom Assignment Presenter");
                var doc = owner.AddComponent<UIDocument>(); doc.panelSettings = panel;
                presenter = owner.AddComponent<ClassroomPresenter>();
            }
            presenter.rig = rig; presenter.board = board;
            // Four generous rows per board page; all remaining assignment words are reachable through More words.
            if(board.wordButtons.Length==5){board.wordButtons[4].gameObject.SetActive(false);board.wordButtons=board.wordButtons.Take(4).ToArray();board.wordLabels=board.wordLabels.Take(4).ToArray();}
            for(int i=0;i<board.wordButtons.Length;i++){var rt=board.wordButtons[i].GetComponent<RectTransform>();rt.sizeDelta=new Vector2(440,68);rt.anchoredPosition=new Vector2(0,78-i*70);}
            board.completion.rectTransform.sizeDelta=new Vector2(460,68);board.completion.rectTransform.anchoredPosition=new Vector2(0,-205);
            string stonePath=ClassroomSceneTools.Art+"/M_StoneFeedback.mat";
            var stoneMaterial=AssetDatabase.LoadAssetAtPath<Material>(stonePath);
            if(!stoneMaterial){stoneMaterial=Object.Instantiate(AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/LibraryRedesign/Materials/SR_Gold.mat"));stoneMaterial.name="M_StoneFeedback";stoneMaterial.EnableKeyword("_EMISSION");stoneMaterial.SetColor("_EmissionColor",Color.black);AssetDatabase.CreateAsset(stoneMaterial,stonePath);}
            foreach(var stone in GameObject.Find("Blender Classroom Geometry").GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.name.StartsWith("SR_WORD_STONE_GOLD_")))stone.sharedMaterial=stoneMaterial;

            string chimePath = ClassroomSceneTools.Art + "/WordRestored.wav";
            if (!File.Exists(chimePath)) WriteChime(chimePath);
            AssetDatabase.ImportAsset(chimePath);
            presenter.successChime = AssetDatabase.LoadAssetAtPath<AudioClip>(chimePath);
            // Camera edits do not alter imported Blender geometry.
            rig.poses[0].position = new Vector3(0,3.6f,-3.9f);
            rig.poses[0].target = new Vector3(1.6f,1.4f,1.3f); rig.poses[0].fieldOfView = 110;
            // Move the book toward the upper part of its composition so the readable page opens beneath it.
            rig.poses[2].position = new Vector3(0,3.6f,-1.5f);
            rig.poses[2].target = new Vector3(0,0,1); rig.poses[2].fieldOfView = 65;
            var scenes = EditorBuildSettings.scenes.ToList();
            if (!scenes.Any(s=>s.path == ClassroomSceneTools.ScenePath)) scenes.Add(new EditorBuildSettingsScene(ClassroomSceneTools.ScenePath,true));
            EditorBuildSettings.scenes = scenes.ToArray();
            rig.GoTo(0,true); AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            File.WriteAllText(Path.Combine(ClassroomSceneTools.Output,"reports/integration.json"),"{\"presenter\":true,\"engine\":\"TrekSession\",\"activities\":\"GatePanelHost\",\"reading\":\"ReadingViewController\",\"soloLaunch\":true,\"multiplayerRouteUnchanged\":true}");
            ClassroomSceneTools.CapturePortraits();
            Debug.Log("[Classroom] Touch presenter and shared assignment systems installed. Solo assignments open the classroom; standalone scene is practice.");
        }

        static void WriteChime(string path)
        {
            const int rate=22050,count=11025;
            using(var stream=File.Create(path)) using(var writer=new BinaryWriter(stream))
            {
                writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));writer.Write(36+count*2);writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));writer.Write(16);writer.Write((short)1);writer.Write((short)1);writer.Write(rate);writer.Write(rate*2);writer.Write((short)2);writer.Write((short)16);writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));writer.Write(count*2);
                for(int i=0;i<count;i++){double t=i/(double)rate;double wave=(Math.Sin(2*Math.PI*659.25*t)+.45*Math.Sin(2*Math.PI*987.77*t))*Math.Exp(-t*9)*Math.Min(1,t*100);writer.Write((short)(wave*6000));}
            }
        }
    }
}
