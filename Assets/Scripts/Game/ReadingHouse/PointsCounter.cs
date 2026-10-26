using System.Collections;
using TMPro;
using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The points badge standing on the mat (the winged gold star over the blue "POINTS" plate): it counts the
    /// student's taps on the die while a key word is played, +1 for a right piece and −1 for a wrong one (never
    /// below 0), to keep them concentrating. The number pops on every change, flashes green or pink, and a "+1"
    /// or "−1" floats up and fades; going up throws a few sparkles. These are game points shown live: the
    /// assignment's graded score still comes only from TrekSession. Presentation only.
    /// </summary>
    public sealed class PointsCounter : MonoBehaviour
    {
        public TextMeshPro number;
        public TextMeshPro floater;
        public ParticleSystem sparkle;
        public Color numberColour = new Color32(150, 72, 10, 255);
        public Color upColour = new Color32(60, 190, 90, 255);
        public Color downColour = new Color32(235, 70, 110, 255);

        public int Value { get; private set; }
        Coroutine pop, rise;
        Vector3 numberScale, floaterStart;

        void Awake()
        {
            if (number != null) numberScale = number.transform.localScale;
            if (floater != null) { floaterStart = floater.transform.localPosition; floater.alpha = 0f; }
            Set(0, false);
        }

        /// <summary>Back to 0 (a fresh start of the assignment).</summary>
        public void ResetTo(int value = 0) => Set(value, false);

        /// <summary>Add (or take away) points: the badge reacts.</summary>
        public void Add(int delta)
        {
            int before = Value;
            Set(Mathf.Max(0, Value + delta), true);
            int shown = Value - before;
            if (floater != null)
            {
                floater.text = delta > 0 ? "+" + delta : "−" + Mathf.Abs(delta);
                floater.color = delta > 0 ? upColour : downColour;
                if (rise != null) StopCoroutine(rise);
                rise = StartCoroutine(Rise());
            }
            if (delta > 0 && sparkle != null) sparkle.Emit(14);
            if (number != null)
            {
                if (pop != null) StopCoroutine(pop);
                pop = StartCoroutine(Pop(delta > 0 ? upColour : downColour, shown != 0 || delta > 0));
            }
        }

        void Set(int v, bool animate)
        {
            Value = v;
            if (number != null) { number.text = v.ToString(); number.color = numberColour; }
        }

        IEnumerator Pop(Color flash, bool big)
        {
            for (float t = 0f; t < 0.45f; t += Time.deltaTime)
            {
                float k = t / 0.45f;
                float s = 1f + (big ? 0.45f : 0.2f) * Mathf.Sin(k * Mathf.PI);
                number.transform.localScale = numberScale * s;
                number.color = Color.Lerp(flash, numberColour, k);
                yield return null;
            }
            number.transform.localScale = numberScale;
            number.color = numberColour;
            pop = null;
        }

        IEnumerator Rise()
        {
            for (float t = 0f; t < 0.9f; t += Time.deltaTime)
            {
                float k = t / 0.9f;
                floater.transform.localPosition = floaterStart + Vector3.up * (0.35f * k);
                floater.alpha = k < 0.15f ? k / 0.15f : 1f - Mathf.InverseLerp(0.5f, 1f, k);
                yield return null;
            }
            floater.alpha = 0f;
            floater.transform.localPosition = floaterStart;
            rise = null;
        }
    }
}
