using System.Collections;
using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// One sign on the signpost outside the Reading House: the house's menu, in the world. A tap
    /// (routed by <see cref="ReadingHouseInput"/>) knocks the plank (a quick wobble with a wooden knock;
    /// the hanging header swings on its chains) and tells the walk-in director which sign it was
    /// (<see cref="ReadingHouseExploreDirector.OnSignpost"/>). Presentation only.
    /// </summary>
    public sealed class SignpostButton : MonoBehaviour
    {
        public enum Kind { Header, Library, Play, Progress, Settings }

        public Kind kind;
        [Tooltip("Hangs on chains: swings about its top edge instead of wobbling.")]
        public bool hanging;

        Quaternion restRotation;
        Vector3 restPosition;
        Coroutine motion;

        void Awake()
        {
            restRotation = transform.localRotation;
            restPosition = transform.localPosition;
        }

        public void Press()
        {
            if (motion != null) StopCoroutine(motion);
            transform.localRotation = restRotation;
            transform.localPosition = restPosition;
            motion = StartCoroutine(hanging ? Swing() : Wobble());
            Knock();
        }

        IEnumerator Wobble()
        {
            // A knock: the plank twists back and forth about the post, settling quickly.
            var axis = transform.parent != null ? transform.parent.InverseTransformDirection(Vector3.up) : Vector3.up;
            for (float t = 0f; t < 0.55f; t += Time.deltaTime)
            {
                float a = 7f * Mathf.Exp(-7f * t) * Mathf.Sin(t * 38f);
                transform.localRotation = Quaternion.AngleAxis(a, axis) * restRotation;
                transform.localPosition = restPosition + Vector3.up * (0.03f * Mathf.Exp(-9f * t) * Mathf.Abs(Mathf.Sin(t * 30f)));
                yield return null;
            }
            transform.localRotation = restRotation;
            transform.localPosition = restPosition;
            motion = null;
        }

        IEnumerator Swing()
        {
            // Swings forward and back on its chains (about the top edge), easing to rest.
            var r = GetComponent<Renderer>();
            var top = r != null ? new Vector3(r.bounds.center.x, r.bounds.max.y, r.bounds.center.z) : transform.position;
            var pivotLocal = transform.parent != null ? transform.parent.InverseTransformPoint(top) : top;
            var axisWorld = transform.right;
            var axis = transform.parent != null ? transform.parent.InverseTransformDirection(axisWorld) : axisWorld;
            for (float t = 0f; t < 1.4f; t += Time.deltaTime)
            {
                float a = 12f * Mathf.Exp(-3.2f * t) * Mathf.Sin(t * 11f);
                var q = Quaternion.AngleAxis(a, axis);
                transform.localRotation = q * restRotation;
                transform.localPosition = pivotLocal + q * (restPosition - pivotLocal);
                yield return null;
            }
            transform.localRotation = restRotation;
            transform.localPosition = restPosition;
            motion = null;
        }

        static AudioClip knockClip;
        static AudioSource knockSource;

        /// <summary>A short hollow wooden knock (synthesised once).</summary>
        static void Knock()
        {
            if (knockClip == null)
            {
                const int rate = 22050; int n = rate / 6;
                var data = new float[n];
                var rng = new System.Random(5);
                for (int i = 0; i < n; i++)
                {
                    float t = i / (float)rate;
                    float body = Mathf.Sin(2f * Mathf.PI * 210f * t) * 0.6f + Mathf.Sin(2f * Mathf.PI * 470f * t) * 0.3f;
                    float click = (float)(rng.NextDouble() * 2 - 1) * Mathf.Exp(-t * 300f);
                    data[i] = (body * Mathf.Exp(-t * 28f) + click * 0.5f) * 0.6f;
                }
                knockClip = AudioClip.Create("SignKnock", n, 1, rate, false);
                knockClip.SetData(data, 0);
            }
            if (knockSource == null)
            {
                var go = new GameObject("Signpost Knock") { hideFlags = HideFlags.DontSave };
                knockSource = go.AddComponent<AudioSource>();
                knockSource.playOnAwake = false; knockSource.spatialBlend = 0f; knockSource.volume = 0.6f;
            }
            knockSource.PlayOneShot(knockClip);
        }
    }
}
