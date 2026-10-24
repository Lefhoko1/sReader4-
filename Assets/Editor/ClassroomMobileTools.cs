using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Profiling;
using SReader.Game.Classroom;
using Object=UnityEngine.Object;
namespace SReader.EditorTools
{
    public static partial class ClassroomProductionTools
    {
        public static void Optimise()
        {
            var current = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (!current) throw new InvalidOperationException("URP is required.");
            string path=ClassroomSceneTools.Art+"/URP_HuaweiY5.asset";
            var pipeline=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path);
            if(!pipeline){pipeline=Object.Instantiate(current);pipeline.name="URP_HuaweiY5";AssetDatabase.CreateAsset(pipeline,path);}
            pipeline.renderScale=.8f;pipeline.supportsHDR=false;pipeline.msaaSampleCount=2;
            pipeline.supportsCameraDepthTexture=false;pipeline.supportsCameraOpaqueTexture=false;
            var serialized = new SerializedObject(pipeline); serialized.FindProperty("m_MainLightShadowsSupported").boolValue=false;serialized.FindProperty("m_AdditionalLightShadowsSupported").boolValue=false;
            serialized.FindProperty("m_SoftShadowsSupported").boolValue=false;serialized.FindProperty("m_AnyShadowsSupported").boolValue=false;serialized.FindProperty("m_LightProbeSystem").intValue=0;pipeline.shadowDistance=0;pipeline.maxAdditionalLightsCount=3;
            serialized.FindProperty("m_AdditionalLightsRenderingMode").intValue=(int)LightRenderingMode.PerVertex;serialized.ApplyModifiedPropertiesWithoutUndo();pipeline.shadowDistance=0;pipeline.maxAdditionalLightsCount=3;
            pipeline.supportsDynamicBatching=true;
            var rig=Object.FindAnyObjectByType<ClassroomCameraRig>();
            var data=rig.viewCamera.GetUniversalAdditionalCameraData();data.SetRenderer(1);data.renderPostProcessing=false;
            rig.viewCamera.allowHDR=false;rig.viewCamera.allowMSAA=true;
            var owner=Object.FindAnyObjectByType<ClassroomMobileSettings>();
            if(!owner)owner=new GameObject("Classroom Low End Android Settings").AddComponent<ClassroomMobileSettings>();
            owner.pipeline=pipeline;owner.Apply();
            var root=GameObject.Find("Blender Classroom Geometry");
            int triangles=0;var materials=new HashSet<Material>();
            foreach(var renderer in root.GetComponentsInChildren<MeshRenderer>(true))
            {
                var mesh=renderer.GetComponent<MeshFilter>().sharedMesh;
                for(int s=0;s<mesh.subMeshCount;s++)triangles+=(int)mesh.GetIndexCount(s)/3;
                foreach(var material in renderer.sharedMaterials)if(material){material.enableInstancing=true;materials.Add(material);EditorUtility.SetDirty(material);}
                bool dynamic=renderer.name.StartsWith("SR_WORD_STONE_GOLD_");
                GameObjectUtility.SetStaticEditorFlags(renderer.gameObject,dynamic?0:StaticEditorFlags.BatchingStatic);
                renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
            }
            PlayerSettings.defaultInterfaceOrientation=UIOrientation.Portrait;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android,false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,new[]{GraphicsDeviceType.OpenGLES3});
            EditorUserBuildSettings.androidBuildSubtarget=MobileTextureSubtarget.ETC2;
            EditorUtility.SetDirty(pipeline);AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            File.WriteAllText(Path.Combine(ClassroomSceneTools.Output,"reports/mobile_configuration.json"),JsonUtility.ToJson(new MobileReport{renderScale=.8f,triangles=triangles,materials=materials.Count,realTimeShadows=false,postProcessing=false,textureFallback="ETC2; ASTC requires a supported device build",graphics="OpenGLES3",deviceTested=false},true));
            Debug.Log("[Classroom] Scene-scoped low-end URP, shared materials and static batching configured. Device performance remains unverified.");
        }
        [Serializable] class MobileReport { public float renderScale;public int triangles,materials;public bool realTimeShadows,postProcessing,deviceTested;public string textureFallback,graphics; }

        public static void Bake()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode before baking.");
            var importer=(ModelImporter)AssetImporter.GetAtPath(ClassroomBlenderImport.ModelPath);
            if(!importer.generateSecondaryUV){importer.generateSecondaryUV=true;importer.SaveAndReimport();}
            var root=GameObject.Find("Blender Classroom Geometry");
            foreach(var renderer in root.GetComponentsInChildren<MeshRenderer>(true))
            {
                if(renderer.name.StartsWith("SR_WORD_STONE_GOLD_"))continue;
                var b=renderer.bounds.size;
                if(b.x*b.y*b.z < .001f)continue;
                GameObjectUtility.SetStaticEditorFlags(renderer.gameObject,StaticEditorFlags.BatchingStatic|StaticEditorFlags.ContributeGI);
                renderer.receiveGI=ReceiveGI.Lightmaps;renderer.scaleInLightmap=.7f;
                renderer.shadowCastingMode=ShadowCastingMode.On;
            }
            var lights=Object.FindObjectsByType<Light>();
            foreach(var light in lights){light.lightmapBakeType=LightmapBakeType.Baked;light.shadows=LightShadows.Soft;}
            var probeObject=GameObject.Find("Classroom Light Probes");
            if(!probeObject)
            {
                probeObject=new GameObject("Classroom Light Probes");var group=probeObject.AddComponent<LightProbeGroup>();var points=new List<Vector3>();
                foreach(float x in new[]{-3f,0f,3f})foreach(float z in new[]{-3f,0f,3f})foreach(float y in new[]{.7f,2f,3.5f})points.Add(new Vector3(x,y,z));
                group.probePositions=points.ToArray();
            }
            string path=ClassroomSceneTools.Art+"/ClassroomLighting.asset";
            var settings=AssetDatabase.LoadAssetAtPath<LightingSettings>(path);
            if(!settings){settings=new LightingSettings();AssetDatabase.CreateAsset(settings,path);}
            settings.bakedGI=true;settings.realtimeGI=false;settings.lightmapper=LightingSettings.Lightmapper.ProgressiveGPU;
            settings.lightmapResolution=10;settings.lightmapMaxSize=1024;LightmapSettings.lightmapsMode=LightmapsMode.NonDirectional;
            settings.directSampleCount=16;settings.indirectSampleCount=32;settings.environmentSampleCount=32;settings.maxBounces=2;
            typeof(LightingSettings).GetProperty("directionalMode")?.SetValue(settings,LightmapsMode.NonDirectional);
            Lightmapping.lightingSettings=settings;AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            Lightmapping.bakeCompleted-=BakeFinished;Lightmapping.bakeCompleted+=BakeFinished;
            if(!Lightmapping.BakeAsync())throw new InvalidOperationException("Unity did not start the lighting bake.");
            File.WriteAllText(Path.Combine(ClassroomSceneTools.Output,"reports/bake_status.txt"),"Baking static architecture and 27 light probes.");
        }
        static void BakeFinished()
        {
            Lightmapping.bakeCompleted-=BakeFinished;
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
            File.WriteAllText(Path.Combine(ClassroomSceneTools.Output,"reports/bake_status.txt"),"Completed. Lightmap atlases: "+LightmapSettings.lightmaps.Length+"; probes: "+(LightmapSettings.lightProbes?LightmapSettings.lightProbes.count:0));
            ClassroomSceneTools.CapturePortraits();
        }

        static ProfilerRecorder draws,batches,passes,main,memory;
        static double profileEnd;static int samples;static long sumDraws,sumBatches,sumPasses,sumMain,maxMemory;
        public static void Profile()
        {
            if(!EditorApplication.isPlaying)throw new InvalidOperationException("Profile requires Play Mode.");
            draws=ProfilerRecorder.StartNew(ProfilerCategory.Render,"Draw Calls Count",1);
            batches=ProfilerRecorder.StartNew(ProfilerCategory.Render,"Batches Count",1);
            passes=ProfilerRecorder.StartNew(ProfilerCategory.Render,"SetPass Calls Count",1);
            main=ProfilerRecorder.StartNew(ProfilerCategory.Internal,"Main Thread",1);
            memory=ProfilerRecorder.StartNew(ProfilerCategory.Memory,"Total Used Memory",1);
            samples=0;sumDraws=sumBatches=sumPasses=sumMain=maxMemory=0;profileEnd=EditorApplication.timeSinceStartup+8;
            EditorApplication.update-=ProfileTick;EditorApplication.update+=ProfileTick;
        }
        static void ProfileTick()
        {
            samples++;sumDraws+=UnityStats.drawCalls;sumBatches+=batches.Valid?batches.LastValue:0;sumPasses+=passes.LastValue;sumMain+=main.LastValue;maxMemory=Math.Max(maxMemory,memory.LastValue);
            if(EditorApplication.timeSinceStartup<profileEnd)return;
            EditorApplication.update-=ProfileTick;
            string json=JsonUtility.ToJson(new ProfileReport{environment="Unity Editor on workstation; not Huawei Y5",samples=samples,drawCalls=sumDraws/(double)samples,batches=sumBatches/(double)samples,setPassCalls=sumPasses/(double)samples,mainThreadMs=sumMain/(double)samples/1e6,usedMemoryMB=maxMemory/1048576.0,drawCounterValid=true,mainCounterValid=main.Valid},true);
            draws.Dispose();batches.Dispose();passes.Dispose();main.Dispose();memory.Dispose();
            File.WriteAllText(Path.Combine(ClassroomSceneTools.Output,"reports/profile_"+DateTime.Now.ToString("HHmmss")+".json"),json);
            try
            {
                var type=AppDomain.CurrentDomain.GetAssemblies().SelectMany(a=>{try{return a.GetTypes();}catch(ReflectionTypeLoadException e){return e.Types.Where(t=>t!=null).ToArray();}}).FirstOrDefault(t=>t.Name=="FrameDebuggerUtility");
                if(type==null)throw new InvalidOperationException("FrameDebuggerUtility unavailable.");
                var flags=BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic;
                var set=type.GetMethod("SetEnabled",flags,null,new[]{typeof(bool),typeof(int)},null);
                if(set==null)throw new MissingMethodException("SetEnabled(bool,int)");
                set.Invoke(null,new object[]{true,0});
                var windowType=type.Assembly.GetTypes().FirstOrDefault(t=>t.Name=="FrameDebuggerWindow");
                if(windowType!=null)EditorWindow.GetWindow(windowType).Show();
                set.Invoke(null,new object[]{true,0});
                var gameViewType=typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");
                EditorWindow.GetWindow(gameViewType).Repaint();EditorApplication.QueuePlayerLoopUpdate();
                double captureAt=EditorApplication.timeSinceStartup+1;
                EditorApplication.CallbackFunction capture=null;
                capture=()=>{
                    if(EditorApplication.timeSinceStartup<captureAt)return;
                    EditorApplication.update-=capture;
                    try
                    {
                        var count=type.GetProperty("count",flags)?.GetValue(null);
                        var events=type.GetMethod("GetFrameEvents",flags)?.Invoke(null,null) as Array;
                        var lines=new List<string>{"Frame Debugger enabled for Editor player.","Event count: "+count,"Captured event records: "+(events?.Length??0)};
                        if(events!=null)foreach(var item in events.Cast<object>().Take(40))lines.Add(string.Join("; ",item.GetType().GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic).Select(f=>f.Name+"="+f.GetValue(item))));
                        File.WriteAllLines(Path.Combine(ClassroomSceneTools.Output,"reports/frame_debugger.txt"),lines);
                    }
                    catch(Exception e){File.WriteAllText(Path.Combine(ClassroomSceneTools.Output,"reports/frame_debugger.txt"),"Capture failed: "+e.Message);}
                    finally {set.Invoke(null,new object[]{false,0});if(windowType!=null)EditorWindow.GetWindow(windowType).Close();ClassroomSceneTools.SetGameView(720,1520,"Huawei Y5 2019 Portrait");}
                };
                EditorApplication.update+=capture;
            }
            catch(Exception e){File.WriteAllText(Path.Combine(ClassroomSceneTools.Output,"reports/frame_debugger.txt"),"Attempted, not captured: "+e.Message);}
            Debug.Log("[Classroom] Editor profiling saved; physical phone performance is not measured.");
        }
        [Serializable]class ProfileReport {public string environment;public int samples;public double drawCalls,batches,setPassCalls,mainThreadMs,usedMemoryMB;public bool drawCounterValid,mainCounterValid;}

        public static void BuildAndroid()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode before building.");
            string path=Path.Combine(ClassroomSceneTools.Output,"android/sReader_Classroom_Development.apk");Directory.CreateDirectory(Path.GetDirectoryName(path));
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{ClassroomSceneTools.ScenePath},locationPathName=path,target=BuildTarget.Android,options=BuildOptions.Development});
            File.WriteAllText(Path.Combine(ClassroomSceneTools.Output,"reports/android_build.json"),JsonUtility.ToJson(new AndroidReport{result=report.summary.result.ToString(),apk=path,bytes=(long)report.summary.totalSize,seconds=report.summary.totalTime.TotalSeconds,errors=report.summary.totalErrors,warnings=report.summary.totalWarnings},true));
        }
        [Serializable]class AndroidReport{public string result,apk;public long bytes;public double seconds;public int errors,warnings;}
    }
}
