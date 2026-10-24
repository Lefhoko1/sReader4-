using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// Builds a development APK of ONLY the Reading House integration scene for
    /// the Huawei Y5 (2019) device test. The few Player Settings it needs
    /// (landscape, OpenGL ES 3, a separate test package id so the real sReader
    /// app on the phone is not replaced) are changed for the build and restored
    /// afterwards, even if the build fails. Build Settings scenes are not touched.
    /// </summary>
    public static class ReadingHouseAndroidBuild
    {
        public static readonly string ApkPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "GeneratedAssets", "ReadingHouseUnity", "build", "sReader_ReadingHouse_Test.apk"));

        [MenuItem("Tools/sReader/Reading House/Build Android Test APK")]
        public static void Build()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ApkPath));

            // remember everything we change
            var orientation = PlayerSettings.defaultInterfaceOrientation;
            bool rotL = PlayerSettings.allowedAutorotateToLandscapeLeft, rotR = PlayerSettings.allowedAutorotateToLandscapeRight;
            bool rotP = PlayerSettings.allowedAutorotateToPortrait, rotPU = PlayerSettings.allowedAutorotateToPortraitUpsideDown;
            bool defaultApis = PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android);
            var apis = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
            string appId = PlayerSettings.GetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android);
            string product = PlayerSettings.productName;
            bool appBundle = EditorUserBuildSettings.buildAppBundle;

            try
            {
                PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
                PlayerSettings.allowedAutorotateToLandscapeLeft = true;
                PlayerSettings.allowedAutorotateToLandscapeRight = true;
                PlayerSettings.allowedAutorotateToPortrait = false;
                PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
                PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
                PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android, appId + ".readinghousetest");
                PlayerSettings.productName = "sReader Reading House Test";
                EditorUserBuildSettings.buildAppBundle = false;

                var options = new BuildPlayerOptions
                {
                    scenes = new[] { ReadingHouseSceneBuilder.ScenePath },
                    locationPathName = ApkPath,
                    target = BuildTarget.Android,
                    targetGroup = BuildTargetGroup.Android,
                    options = BuildOptions.Development,
                };
                var report = BuildPipeline.BuildPlayer(options);
                var s = report.summary;
                string summary = $"Result {s.result}, {s.totalErrors} errors, {s.totalWarnings} warnings, " +
                                 $"size {s.totalSize / (1024f * 1024f):0.0} MB, time {s.totalTime:mm\\:ss}, output {s.outputPath}";
                File.WriteAllText(Path.ChangeExtension(ApkPath, ".build.txt"), summary);
                if (s.result != BuildResult.Succeeded) throw new Exception("Android build failed: " + summary);
                Debug.Log("[ReadingHouse] " + summary);
            }
            finally
            {
                PlayerSettings.defaultInterfaceOrientation = orientation;
                PlayerSettings.allowedAutorotateToLandscapeLeft = rotL;
                PlayerSettings.allowedAutorotateToLandscapeRight = rotR;
                PlayerSettings.allowedAutorotateToPortrait = rotP;
                PlayerSettings.allowedAutorotateToPortraitUpsideDown = rotPU;
                PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, apis);
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, defaultApis);
                PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android, appId);
                PlayerSettings.productName = product;
                EditorUserBuildSettings.buildAppBundle = appBundle;
                AssetDatabase.SaveAssets();
            }
        }
    }
}
