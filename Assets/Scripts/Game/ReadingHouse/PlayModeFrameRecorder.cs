using System.IO;
using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// Test helper (used by the editor capture tools): records N consecutive game frames at a fixed
    /// game rate (Time.captureFramerate) to PNG files, so motion smoothness can
    /// be reviewed frame by frame. Removes itself when done.
    /// </summary>
    public sealed class PlayModeFrameRecorder : MonoBehaviour
    {
        public string folder, prefix;
        public int frames = 30, fps = 20;
        int done;

        public bool Finished => done >= frames;

        void OnEnable() { Time.captureFramerate = fps; Directory.CreateDirectory(folder); }

        void LateUpdate()
        {
            if (done >= frames) return;
            ScreenCapture.CaptureScreenshot(Path.Combine(folder, $"{prefix}_{done:000}.png"));
            done++;
            if (done >= frames) { Time.captureFramerate = 0; }
        }

        void OnDisable() => Time.captureFramerate = 0;
    }
}
