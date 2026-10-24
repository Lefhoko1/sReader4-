using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using SReader.Game.Classroom;
using Object=UnityEngine.Object;
namespace SReader.EditorTools
{
    [InitializeOnLoad]public static class ClassroomPortraitChecks
    {
        static string Request=>Path.Combine(ClassroomSceneTools.Output,"portraits.request");
        static int step;static double next;static ClassroomPresenter presenter;
        static readonly List<string> checks=new List<string>();
        static ClassroomPortraitChecks(){EditorApplication.update+=Tick;}
        public static void Start(){File.WriteAllText(Request,"run");if(!EditorApplication.isPlaying)EditorApplication.isPlaying=true;}
        static void Require(bool value,string message){if(!value)throw new InvalidOperationException(message);checks.Add(message);}
        static void Capture(string name)=>ScreenCapture.CaptureScreenshot(Path.Combine(ClassroomSceneTools.Output,"screenshots/"+name+".png"));
        static void Size(int height){ClassroomSceneTools.SetGameView(720,height,"Classroom portrait 720x"+height);Screen.orientation=ScreenOrientation.Portrait;}
        static void Reading(int height,string file)
        {
            Require(Screen.width==720&&Screen.height==height,"Runtime renders at 720 x "+height);
            Require(presenter.ReadingOpen,"Full passage reader opens at portrait height "+height);
            var doc=presenter.GetComponent<UIDocument>().rootVisualElement;
            var label=presenter.Overlay.Query<Label>().ToList().First(l=>l.text==presenter.Session.Trail.PassageText);
            Require(label.worldBound.width>200&&label.worldBound.xMin>=0&&label.worldBound.xMax<=doc.worldBound.width+1,"Reading text wraps within portrait safe-area width "+height);
            var close=presenter.Overlay.Query<Button>().ToList().Single(b=>b.text=="Close book");
            Require(close.worldBound.height>=48&&close.worldBound.xMax<=doc.worldBound.width+1,"Close book target fits portrait viewport "+height);
            Capture(file);
        }
        static void Tick()
        {
            if(!File.Exists(Request)||!EditorApplication.isPlaying||EditorApplication.isCompiling||EditorApplication.timeSinceStartup<next)return;
            try
            {
                if(step==0){presenter=Object.FindAnyObjectByType<ClassroomPresenter>();if(!presenter||presenter.Session==null)return;checks.Clear();Size(1280);}
                else if(step==1)presenter.OpenBook();
                else if(step==2)Reading(1280,"10_reading_720x1280");
                else if(step==3)presenter.Back();
                else if(step==4)Size(1600);
                else if(step==5)presenter.OpenBook();
                else if(step==6)Reading(1600,"11_reading_720x1600");
                else if(step==7)presenter.Back();
                else if(step==8)Size(1520);
                else if(step==9){presenter.rig.GoTo(1,true);}
                else if(step==10)
                {
                    var board=presenter.board;var corners=new Vector3[4];board.wordButtons[0].GetComponent<RectTransform>().GetWorldCorners(corners);
                    float target=Vector3.Distance(presenter.rig.viewCamera.WorldToScreenPoint(corners[0]),presenter.rig.viewCamera.WorldToScreenPoint(corners[1]));
                    Require(target>=88,"World-space word targets exceed 44 logical pixels at 720 x 1520");Capture("12_play_chalkboard");
                }
                else if(step==11)presenter.OpenBook();
                else if(step==12)Reading(1520,"05_book_reading");
                else if(step==13)presenter.Back();
                else if(step==14)Capture("09_play_classroom");
                else{File.WriteAllText(Path.Combine(ClassroomSceneTools.Output,"reports/portrait_verification.json"),JsonUtility.ToJson(new Result{passed=true,checks=checks.ToArray()},true));File.Delete(Request);step=0;Debug.Log("[Classroom] Three portrait aspect ratios and readable book/board touch targets verified.");return;}
                step++;next=EditorApplication.timeSinceStartup+1.5;
            }
            catch(Exception e){File.WriteAllText(Path.Combine(ClassroomSceneTools.Output,"reports/portrait_verification.json"),JsonUtility.ToJson(new Result{passed=false,error=e.ToString(),checks=checks.ToArray()},true));File.Delete(Request);step=0;Debug.LogException(e);}
        }
        [Serializable]class Result{public bool passed;public string error;public string[] checks;}
    }
    public static partial class ClassroomProductionTools{public static void Portraits()=>ClassroomPortraitChecks.Start();}
}
