using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using SReader.Game.Classroom;
using SReader.Game.Trek;
using SReader.Domains.Assignments.Models;
using Object=UnityEngine.Object;
namespace SReader.EditorTools
{
    [InitializeOnLoad]
    public static class ClassroomVerification
    {
        static string Request=>Path.Combine(ClassroomSceneTools.Output,"verify.request");
        static ClassroomPresenter presenter;
        static int stage;
        static double next,deadline;
        static readonly List<string> checks=new List<string>();
        static ClassroomVerification(){EditorApplication.update+=Tick;}
        public static void Start()
        {
            File.WriteAllText(Request,"start");
            if(!EditorApplication.isPlaying)EditorApplication.isPlaying=true;
        }
        static void Require(bool ok,string check){if(!ok)throw new InvalidOperationException(check);checks.Add(check);}
        static void Click(UnityEngine.UIElements.Button button)
        {
            if(button==null)throw new InvalidOperationException("Expected activity button is missing.");
            using(var evt=NavigationSubmitEvent.GetPooled()){evt.target=button;button.SendEvent(evt);}
        }
        static UnityEngine.UIElements.Button Button(string starts)=>presenter.Overlay.Query<UnityEngine.UIElements.Button>().ToList().FirstOrDefault(b=>b.enabledInHierarchy&&b.text.StartsWith(starts));
        static void Arrange(int skip=0)
        {
            var gate=presenter.Session.ActiveGate;var token=(ContentToken)gate.ChallengePayload;
            var pieces=gate.Type==GateType.Fill?GateSolving.FillCorrect(token):GateSolving.DefineCorrect(token);
            foreach(var piece in pieces.Skip(skip))
                Click(presenter.Overlay.Query<UnityEngine.UIElements.Button>().ToList().LastOrDefault(b=>b.enabledInHierarchy&&b.text==piece));
            if(presenter.Session.ActiveGate!=null)Click(Button("Check"));
        }
        static void Capture(string name)=>ScreenCapture.CaptureScreenshot(Path.Combine(ClassroomSceneTools.Output,"screenshots/"+name+".png"));
        static void Tick()
        {
            if(!File.Exists(Request)||!EditorApplication.isPlaying||EditorApplication.isCompiling)return;
            if(EditorApplication.timeSinceStartup<next)return;
            try
            {
                if(stage==0)
                {
                    presenter=Object.FindAnyObjectByType<ClassroomPresenter>();if(!presenter||presenter.Session==null)return;
                    checks.Clear();deadline=EditorApplication.timeSinceStartup+90;
                    Require(presenter.Session.TotalGates==4,"Bundled assignment binds all four gates");
                    Require(GameObject.Find("Blender Classroom Geometry")!=null,"Linked Blender geometry active");
                    Require(Object.FindAnyObjectByType<ClassroomBoard>().assignmentTitle.text=="Meadow at Dawn","Board title comes from assignment data");
                    presenter.OpenBook();
                }
                else if(stage==1)
                {
                    Require(presenter.ReadingOpen,"Hero book opens the reading interface after camera movement");
                    Require(presenter.Overlay.Query<Label>().ToList().Any(l=>l.text==presenter.Session.Trail.PassageText),"Full original passage is readable, including unsolved words");
                    Require(presenter.Session.TimerPaused,"Reading pauses the word timer");Capture("05_book_reading");
                }
                else if(stage==2){presenter.Back();}
                else if(stage==3){presenter.SelectWord(0);}
                else if(stage==4)
                {
                    Require(presenter.ActivityOpen,"Chalkboard selection opens the existing Fill activity");Require(presenter.Overlay.Query<Label>().ToList().Any(l=>l.text.Contains("____")),"Fill activity keeps a blank in its sentence while the book retains the complete passage");Capture("06_fill_activity");
                }
                else if(stage==5)
                {
                    Arrange();Require(presenter.Session.Trail.Gates[0].State==GateState.SolvedGold,"Fill solve awards Gold through TrekSession");presenter.SelectWord(1);
                }
                else if(stage==6)
                {
                    Click(Button("⏭ Later"));Require(presenter.Session.Trail.Gates[1].State==GateState.Deferred,"Later defers the Define word without skipping completion");
                    Require(!presenter.Session.CanComplete,"Completion is blocked while required words remain");presenter.SelectWord(2);
                }
                else if(stage==7)
                {
                    Require(presenter.ActivityOpen,"Illustrate activity opens");Capture("07_picture_activity");
                }
                else if(stage==8)
                {
                    var token=(ContentToken)presenter.Session.ActiveGate.ChallengePayload;
                    var picture=presenter.Overlay.Query<VisualElement>("picture-option").ToList().Single(e=>(string)e.userData==token.correctImage);
                    using(var evt=ClickEvent.GetPooled()){evt.target=picture;picture.SendEvent(evt);}
                    Require(presenter.Session.Trail.Gates[2].State==GateState.SolvedGold,"Correct picture awards Gold through the existing activity callback");presenter.SelectWord(3);
                }
                else if(stage==9)
                {
                    Click(Button("Hint"));Require(presenter.Session.ActiveGate.HintsUsed==1,"First hint uses the shared hint ladder");Arrange();
                    Require(presenter.Session.Trail.Gates[3].State==GateState.SolvedSilver,"Hinted Define solve awards Silver");
                    Require(presenter.Session.PeekNextGate()==presenter.Session.Trail.Gates[1],"Deferred word returns after the main passage order");presenter.SelectWord(1);
                }
                else if(stage==10)
                {
                    Click(Button("Hint"));Click(Button("Hint"));Click(Button("Guide me"));
                    Require(presenter.Session.ActiveGate.HintsUsed==3,"Two hints and guided solve use the shared engine");Arrange(2);
                    Require(presenter.Session.Trail.Gates[1].State==GateState.SolvedBronze,"Guided solve completes the required word as Bronze");
                    Require(presenter.Session.CanComplete&&presenter.Session.SolvedGates==4,"All four required activities complete before finishing");
                    int expected=presenter.Session.Trail.Gates[0].BasePoints+presenter.Session.Trail.Gates[2].BasePoints+(int)Math.Round(presenter.Session.Trail.Gates[3].BasePoints*.6f);
                    Require(presenter.Session.Points==expected,"Full points, 40 percent hint deduction and zero guided points are preserved");
                    Require(Object.FindAnyObjectByType<ClassroomBoard>().wordLabels.All(l=>!l.gameObject.activeSelf||l.color.r>.8f),"Completed chalkboard words turn gold");Capture("08_completed_board");
                }
                else if(stage==11){presenter.Back();}
                else if(stage==12){Capture("09_play_classroom");}
                else
                {
                    File.WriteAllText(Path.Combine(ClassroomSceneTools.Output,"reports/play_verification.json"),JsonUtility.ToJson(new Result{passed=true,checks=checks.ToArray()},true));
                    File.Delete(Request);stage=0;Debug.Log("[Classroom] Book, Fill, Define, Illustrate, Later, hint scoring and gold feedback verified in Play Mode.");return;
                }
                stage++;next=EditorApplication.timeSinceStartup+1.2;
                if(stage>0&&EditorApplication.timeSinceStartup>deadline)throw new TimeoutException("Classroom verification timed out.");
            }
            catch(Exception e)
            {
                File.WriteAllText(Path.Combine(ClassroomSceneTools.Output,"reports/play_verification.json"),JsonUtility.ToJson(new Result{passed=false,error=e.ToString(),checks=checks.ToArray()},true));
                File.Delete(Request);stage=0;Debug.LogException(e);
            }
        }
        [Serializable]class Result{public bool passed;public string error;public string[] checks;}
    }
    public static partial class ClassroomProductionTools {public static void Verify()=>ClassroomVerification.Start();}
}
