using UnityEngine;

namespace SReader.Game.Trek
{
    /// <summary>
    /// Scrolls one background layer at a fraction of the camera's horizontal
    /// movement (spec §3: sky, mountains, treeline at fractional speeds). Pure
    /// transform math per frame, no allocations.
    /// </summary>
    internal sealed class ParallaxLayer : MonoBehaviour
    {
        [Range(0f, 1f)] public float factor = 0.5f;   // 0 = pinned to camera (sky), 1 = world-locked

        Transform cam;
        float camStartX;
        float selfStartX;

        public void Init(Transform followCamera)
        {
            cam = followCamera;
            camStartX = cam.position.x;
            selfStartX = transform.position.x;
        }

        void LateUpdate()
        {
            if (cam == null) return;
            float camDelta = cam.position.x - camStartX;
            var p = transform.position;
            // Moving WITH the camera by (1-factor) makes the layer appear to
            // scroll at `factor` of world speed.
            p.x = selfStartX + camDelta * (1f - factor);
            transform.position = p;
        }
    }
}
