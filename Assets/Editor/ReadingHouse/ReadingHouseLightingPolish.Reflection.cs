using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;
namespace SReader.EditorTools.ReadingHouse
{
    public static partial class ReadingHouseLightingPolish
    {
        static void BakeRoomCubemap()
        {
            var probe = All<ReflectionProbe>().Single();
            var hidden = All<Renderer>().Where(r => r.transform.root.name == "Characters" || r is SpriteRenderer || r.GetComponent<TMPro.TMP_Text>() != null).Select(r => (r, r.enabled)).ToArray();
            var cameraGO = new GameObject("Temporary room reflection camera") { hideFlags = HideFlags.HideAndDontSave };
            var cube = new Cubemap(64, TextureFormat.RGBA32, true) { name = "Reflection_Room_ASTC", wrapMode = TextureWrapMode.Clamp };
            try
            {
                foreach (var state in hidden) state.r.enabled = false;
                var camera = cameraGO.AddComponent<Camera>(); camera.enabled = false;
                camera.transform.position = probe.transform.position;
                camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = Hex("D3C8AF");
                camera.nearClipPlane = .1f; camera.farClipPlane = 30; camera.cullingMask = ~0;
                camera.allowHDR = false; camera.allowMSAA = false;
                var data = camera.GetUniversalAdditionalCameraData(); data.SetRenderer(2); data.renderPostProcessing = false;
                data.requiresDepthOption = CameraOverrideOption.Off; data.requiresColorOption = CameraOverrideOption.Off;
                if (!camera.RenderToCubemap(cube)) throw new InvalidOperationException("Explicit 3D camera could not render the room cubemap");
                cube.Apply(true, false);
                var pixels = Enumerable.Range(0, 6).SelectMany(face => cube.GetPixels((CubemapFace)face)).ToArray();
                float mean = pixels.Average(c => c.grayscale);
                float variance = pixels.Average(c => (c.grayscale - mean) * (c.grayscale - mean));
                if (variance < .00001f) throw new InvalidOperationException("The room reflection is uniform; refusing an empty bake");
                EditorUtility.CompressCubemapTexture(cube, TextureFormat.ASTC_6x6, 70);
                string path = Art + "/Reflection_Room_ASTC.cubemap";
                var existing = AssetDatabase.LoadAssetAtPath<Cubemap>(path);
                if (existing == null) AssetDatabase.CreateAsset(cube, path);
                else { EditorUtility.CopySerialized(cube, existing); Object.DestroyImmediate(cube); cube = existing; EditorUtility.SetDirty(existing); }
                probe.mode = ReflectionProbeMode.Custom; probe.customBakedTexture = cube;
                File.WriteAllText(Output + "/reflection-result.txt", $"PASS: 64px static room cubemap, 3D renderer index 2, ASTC 6x6, variance={variance:F6}, mean={mean:F4}\n");
            }
            finally
            {
                Object.DestroyImmediate(cameraGO);
                foreach (var state in hidden) state.r.enabled = state.enabled;
                if (!AssetDatabase.Contains(cube)) Object.DestroyImmediate(cube);
            }
            AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(House());
        }
    }
}
