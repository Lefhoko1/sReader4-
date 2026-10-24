using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// A soft, blurred copy of the 3D scene for frosted-glass UI. Captured ONCE
    /// when the camera comes to rest (not every frame), by rendering the camera
    /// at half size and box-filtering it down and back up — cheap enough for a
    /// low-end phone and needs no custom shader.
    /// </summary>
    public static class FrostedBackdrop
    {
        public static RenderTexture Texture { get; private set; }
        public static event Action Changed;

        public static void Capture(Camera cam)
        {
            if (cam == null) return;
            int w = Mathf.Max(64, Screen.width / 2), h = Mathf.Max(32, Screen.height / 2);
            var full = RenderTexture.GetTemporary(w, h, 24, RenderTextureFormat.Default);
            var request = new UniversalRenderPipeline.SingleCameraRequest { destination = full };
            if (RenderPipeline.SupportsRenderRequest(cam, request)) RenderPipeline.SubmitRenderRequest(cam, request);
            else { var prior = cam.targetTexture; cam.targetTexture = full; cam.Render(); cam.targetTexture = prior; }

            // Down to 1/16 and back up to 1/4: each bilinear step widens the blur.
            var a = RenderTexture.GetTemporary(w / 2, h / 2);
            var b = RenderTexture.GetTemporary(w / 4, h / 4);
            var c = RenderTexture.GetTemporary(w / 8, h / 8);
            var d = RenderTexture.GetTemporary(w / 4, h / 4);
            foreach (var rt in new[] { full, a, b, c, d }) rt.filterMode = FilterMode.Bilinear;
            Graphics.Blit(full, a);
            Graphics.Blit(a, b);
            Graphics.Blit(b, c);
            Graphics.Blit(c, d);

            if (Texture == null || Texture.width != w / 2 || Texture.height != h / 2)
            {
                if (Texture != null) Texture.Release();
                Texture = new RenderTexture(w / 2, h / 2, 0) { filterMode = FilterMode.Bilinear, name = "FrostedBackdrop" };
            }
            Graphics.Blit(d, Texture);
            foreach (var rt in new[] { full, a, b, c, d }) RenderTexture.ReleaseTemporary(rt);
            Changed?.Invoke();
        }
    }
}
