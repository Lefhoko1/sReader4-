using Unity.Profiling;
using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// Small on-screen readout for device testing (development builds and the
    /// editor only): smoothed FPS and frame time, draw calls, SetPass calls,
    /// triangles and memory, read from Unity's built-in profiler counters so the
    /// numbers come from the phone itself, not the editor.
    /// </summary>
    public sealed class ReadingHousePerfOverlay : MonoBehaviour
    {
        ProfilerRecorder drawCalls, setPass, triangles, totalMemory, gfxMemory;
        float smoothedDelta = 1f / 30f;
        GUIStyle style;

        void OnEnable()
        {
            if (!Debug.isDebugBuild) { enabled = false; return; }
            drawCalls = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
            setPass = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
            triangles = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
            totalMemory = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory");
            gfxMemory = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Gfx Used Memory");
        }

        void OnDisable()
        {
            drawCalls.Dispose(); setPass.Dispose(); triangles.Dispose(); totalMemory.Dispose(); gfxMemory.Dispose();
        }

        void Update() => smoothedDelta = Mathf.Lerp(smoothedDelta, Time.unscaledDeltaTime, 0.05f);

        void OnGUI()
        {
            if (style == null)
            {
                // Small, quiet, bottom-right: a dev readout, not part of the game's look.
                var bg = new Texture2D(1, 1); bg.SetPixel(0, 0, new Color(0.07f, 0.08f, 0.1f, 0.45f)); bg.Apply();
                style = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(Screen.height * 0.019f), alignment = TextAnchor.LowerRight, padding = new RectOffset(8, 8, 5, 5) };
                style.normal.background = bg;
                style.normal.textColor = new Color(0.95f, 0.94f, 0.9f, 0.8f);
            }
            var safe = Screen.safeArea;
            string text =
                $"{1f / smoothedDelta:0} fps · {smoothedDelta * 1000f:0.0} ms · {drawCalls.LastValue} draws · {setPass.LastValue} setpass\n" +
                $"{triangles.LastValue / 1000f:0.0}k tris · {totalMemory.LastValue / (1024f * 1024f):0} MB · GPU {gfxMemory.LastValue / (1024f * 1024f):0} MB";
            var size = style.CalcSize(new GUIContent(text));
            GUI.Label(new Rect(safe.xMax - size.x - 8, Screen.height - safe.yMin - size.y - 8, size.x, size.y), text, style);
        }
    }
}
