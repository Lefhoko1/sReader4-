using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// Lets tooling outside the editor run the Reading House editor tools in the
    /// open Unity instance. Write one command word to
    /// GeneratedAssets/ReadingHouseUnity/bridge/command.txt; the bridge runs it
    /// on the editor's main thread and writes result.txt (OK / FAILED + message)
    /// and log.txt (every console message logged during the command).
    ///
    /// Commands: refresh, build, verify, android, all.
    /// Every command is also on the Tools ▸ sReader ▸ Reading House menu.
    /// </summary>
    [InitializeOnLoad]
    public static class ReadingHouseBridge
    {
        static readonly string Folder = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "GeneratedAssets", "ReadingHouseUnity", "bridge"));
        static string CommandFile => Path.Combine(Folder, "command.txt");
        static double nextPoll;
        static bool busy;

        static ReadingHouseBridge()
        {
            Directory.CreateDirectory(Folder);
            File.WriteAllText(Path.Combine(Folder, "ready.txt"), DateTime.Now.ToString("s"));
            EditorApplication.update += Poll;
        }

        static void Poll()
        {
            if (busy || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            if (EditorApplication.timeSinceStartup < nextPoll) return;
            nextPoll = EditorApplication.timeSinceStartup + 0.5;
            if (!File.Exists(CommandFile)) return;

            string command = File.ReadAllText(CommandFile).Trim().ToLowerInvariant();
            File.Delete(CommandFile);
            Run(command);
        }

        /// <summary>Append the Reading House scene to Build Settings (needed to load it at runtime). Order and other entries are untouched.</summary>
        [MenuItem("Tools/sReader/Reading House/Add Scene To Build Settings")]
        public static void AddSceneToBuild()
        {
            var scenes = EditorBuildSettings.scenes.ToList();
            if (scenes.Any(s => s.path == ReadingHouseSceneBuilder.ScenePath)) return;
            scenes.Add(new EditorBuildSettingsScene(ReadingHouseSceneBuilder.ScenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            AssetDatabase.SaveAssets();
        }

        static void Run(string command)
        {
            busy = true;
            var log = new StringBuilder();
            void Capture(string message, string stack, LogType type)
            {
                log.AppendLine($"[{type}] {message}");
                if (type == LogType.Exception || type == LogType.Error) log.AppendLine(stack);
            }
            Application.logMessageReceived += Capture;
            string result;
            try
            {
                switch (command)
                {
                    case "refresh": AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate); break;
                    case "build": ReadingHouseSceneBuilder.BuildAll(); break;
                    case "verify": ReadingHouseVerification.RunAll(); break;
                    case "android": ReadingHouseAndroidBuild.Build(); break;
                    case "diag": ReadingHouseDiagnostics.Run(); break;
                    case "livetest": ReadingHouseLiveTest.Start(); break;
                    case "capture": ReadingHouseViewCapture.Run(); break;
                    case "walkdiag": ReadingHouseWalkDiagnostics.Run(); break;
                    case "playcapture": ReadingHousePlayCapture.Start(); break;
                    case "student2d": Student2DSetup.Run(); break;
                    case "student2dcapture": Student2DPlayCapture.Start(); break;
                    case "chalkboard": ChalkboardSetup.Run(); break;
                    case "seated": SeatedCharactersSetup.Run(); break;
                    case "probe": ReadingHouseProbe.Run(); break;
                    case "readingmat": ReadingMatSetup.Run(); break;
                    case "backdrop": WorldBackdropSetup.Run(); break;
                    case "chalkboardcapture": ChalkboardCapture.Start(); break;
                    case "patch": ReadingHouseScenePatcher.Apply(); break;
                    case "patchcapture": ReadingHouseScenePatcher.Apply(); ReadingHouseViewCapture.Run(); break;
                    case "addscene": AddSceneToBuild(); break;
                    case "all": ReadingHouseSceneBuilder.BuildAll(); ReadingHouseVerification.RunAll(); break;
                    default: throw new ArgumentException($"Unknown command '{command}'.");
                }
                result = $"OK {command} {DateTime.Now:s}";
            }
            catch (Exception e)
            {
                result = $"FAILED {command} {DateTime.Now:s}\n{e}";
                Debug.LogException(e);
            }
            finally
            {
                Application.logMessageReceived -= Capture;
                busy = false;
            }
            File.WriteAllText(Path.Combine(Folder, "log.txt"), log.ToString());
            File.WriteAllText(Path.Combine(Folder, "result.txt"), result);
        }
    }
}
