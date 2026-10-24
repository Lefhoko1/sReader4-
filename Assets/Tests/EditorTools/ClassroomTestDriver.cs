using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
namespace SReader.Classroom.TestDriver
{
    [InitializeOnLoad] public sealed class ClassroomTestDriver:ICallbacks
    {
        static TestRunnerApi api;
        static string Output=>File.Exists("GeneratedAssets/MagicalClassroom/active.txt") ? File.ReadAllText("GeneratedAssets/MagicalClassroom/active.txt").Trim() : Path.GetFullPath("GeneratedAssets/MagicalClassroom/20261017_195009");
        static ClassroomTestDriver(){EditorApplication.update+=Poll;}
        static void Poll()
        {
            string path=Path.Combine(Output,"tests.request");
            if(EditorApplication.isCompiling||EditorApplication.isPlaying||!File.Exists(path))return;
            File.Delete(path);api=ScriptableObject.CreateInstance<TestRunnerApi>();api.RegisterCallbacks(new ClassroomTestDriver());
            api.Execute(new ExecutionSettings(new Filter{testMode=TestMode.EditMode,assemblyNames=new[]{"SReader.Game.Trek.Tests"}}){runSynchronously=true});
        }
        public void RunStarted(ITestAdaptor tests){}
        public void TestStarted(ITestAdaptor test){}
        public void TestFinished(ITestResultAdaptor result){}
        public void RunFinished(ITestResultAdaptor result)
        {
            TestRunnerApi.SaveResultToFile(result,Path.Combine(Output,"reports/editmode_results.xml"));
            File.WriteAllText(Path.Combine(Output,"reports/editmode_results.json"),JsonUtility.ToJson(new Report{state=result.ResultState,passed=result.PassCount,failed=result.FailCount,skipped=result.SkipCount,duration=result.Duration},true));
            Debug.Log("[Classroom] Existing Trek contract tests: "+result.PassCount+" passed, "+result.FailCount+" failed.");
        }
        [Serializable]class Report{public string state;public int passed,failed,skipped;public double duration;}
    }
}
