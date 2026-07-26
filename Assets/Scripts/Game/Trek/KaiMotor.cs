using System;
using System.Collections;
using UnityEngine;

namespace SReader.Game.Trek
{
    /// <summary>
    /// Kai's deterministic, tweened movement (no physics — spec §3): walk to an X,
    /// report progress every frame (the ribbon binds to it), celebrate on solves.
    /// Also animates the sprite: cycles walk frames while moving, idles otherwise.
    /// Frames come from the existing mascot sheet (Resources/Art/MascotBoy); a
    /// tinted capsule fallback keeps the trek playable with no art imported.
    /// </summary>
    internal sealed class KaiMotor : MonoBehaviour
    {
        const float WalkSpeed = 2.6f;        // world units / second
        const float WalkFps = 10f;

        SpriteRenderer sr;
        Sprite[] frames = Array.Empty<Sprite>();
        Coroutine moving;
        float animClock;
        int frame;

        public bool IsMoving => moving != null;

        public void Init(SpriteRenderer renderer, Sprite[] walkFrames)
        {
            sr = renderer;
            frames = walkFrames ?? Array.Empty<Sprite>();
            if (frames.Length > 0) sr.sprite = frames[0];
        }

        /// <summary>
        /// Walk to <paramref name="targetX"/>. <paramref name="onProgress"/> gets
        /// 0→1 along the way (drives the ribbon), <paramref name="onArrive"/> fires
        /// once at the end. A new call cancels the previous walk.
        /// </summary>
        public void WalkTo(float targetX, Action<float> onProgress, Action onArrive)
        {
            if (moving != null) StopCoroutine(moving);
            moving = StartCoroutine(Walk(targetX, onProgress, onArrive));
        }

        public void StopWalk()
        {
            if (moving != null) StopCoroutine(moving);
            moving = null;
        }

        IEnumerator Walk(float targetX, Action<float> onProgress, Action onArrive)
        {
            float startX = transform.position.x;
            float dist = Mathf.Abs(targetX - startX);
            float dir = Mathf.Sign(targetX - startX);
            if (sr != null) sr.flipX = dir < 0;

            if (dist < 0.01f)
            {
                moving = null;
                onProgress?.Invoke(1f);
                onArrive?.Invoke();
                yield break;
            }

            float t = 0f;
            while (t < 1f)
            {
                t = Mathf.Min(1f, t + Time.deltaTime * WalkSpeed / dist);
                var p = transform.position;
                p.x = Mathf.Lerp(startX, targetX, t);
                transform.position = p;
                Animate();
                onProgress?.Invoke(t);
                yield return null;
            }
            moving = null;
            onArrive?.Invoke();
        }

        /// <summary>A little squash-and-hop on a solved gate.</summary>
        public void Celebrate()
        {
            StartCoroutine(Hop());
        }

        IEnumerator Hop()
        {
            float baseY = transform.position.y;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 3f;
                var p = transform.position;
                p.y = baseY + Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI) * 0.45f;
                transform.position = p;
                yield return null;
            }
            var end = transform.position;
            end.y = baseY;
            transform.position = end;
        }

        void Animate()
        {
            if (sr == null || frames.Length < 2) return;
            animClock += Time.deltaTime;
            if (animClock < 1f / WalkFps) return;
            animClock = 0f;
            frame = (frame + 1) % frames.Length;
            sr.sprite = frames[frame];
        }

        void Update()
        {
            // Gentle idle bob while standing (kept subtle; reduced-motion pass in Phase 5).
            if (!IsMoving && sr != null && frames.Length > 0 && sr.sprite != frames[0])
                sr.sprite = frames[0];
        }
    }
}
