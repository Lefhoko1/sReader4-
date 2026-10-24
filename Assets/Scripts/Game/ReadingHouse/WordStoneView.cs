using TMPro;
using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>Visual state of one word stone. Rendering only — the rules behind a state live in TrekSession.</summary>
    public enum StoneState { Normal, KeyWord, Selected, Completed, Deferred, Disabled }

    /// <summary>
    /// One reusable word stone (the modular Blender asset: left cap, stretchable
    /// centre, right cap, each with a moss base). Sizes its parts to a width,
    /// puts the word on its label and swaps its state material. The label is a
    /// child of the stone ROOT, never of the stretching centre, so text is never
    /// stretched. Presentation only: it renders state and reports taps.
    /// </summary>
    public sealed class WordStoneView : MonoBehaviour
    {
        public const float CapWidth = 0.20f;   // matches the Blender module (handoff v04 §1)

        [Header("Modular parts (found by name when the prefab is built)")]
        public Transform capLeft, centre, capRight, mossCapLeft, mossCentre, mossCapRight;
        public TextMeshPro label;
        public BoxCollider tapCollider;

        [Header("State materials (index = StoneState)")]
        public Material[] stateMaterials = new Material[6];

        public int WordIndex { get; private set; }
        public string Word { get; private set; }
        public StoneState State { get; private set; }

        Renderer[] stoneRenderers;
        Vector3 capAxis = Vector3.right;   // the stone's local "along the word" axis, read from the cap layout

        void Awake() => CacheRenderers();

        void CacheRenderers()
        {
            if (stoneRenderers != null) return;
            stoneRenderers = new[] { capLeft.GetComponent<Renderer>(), centre.GetComponent<Renderer>(), capRight.GetComponent<Renderer>() };
            var span = capRight.localPosition - capLeft.localPosition;
            if (span.sqrMagnitude > 1e-6f) capAxis = span.normalized;
        }

        /// <summary>Show a word on a stone of the given (unscaled) width.</summary>
        public void Show(int wordIndex, string word, float stoneWidth)
        {
            CacheRenderers();
            WordIndex = wordIndex;
            Word = word;
            label.text = word;

            float stretch = Mathf.Max(0.02f, stoneWidth - 2f * CapWidth);
            var s = centre.localScale; s.x = stretch; centre.localScale = s;
            s = mossCentre.localScale; s.x = stretch; mossCentre.localScale = s;
            capLeft.localPosition = -capAxis * (stretch / 2f);
            capRight.localPosition = capAxis * (stretch / 2f);
            mossCapLeft.localPosition = capLeft.localPosition;
            mossCapRight.localPosition = capRight.localPosition;

            if (tapCollider != null)
            {
                var size = tapCollider.size;
                size.x = stoneWidth;
                tapCollider.size = size;
            }
        }

        public void SetState(StoneState state)
        {
            CacheRenderers();
            State = state;
            var mat = stateMaterials != null && (int)state < stateMaterials.Length ? stateMaterials[(int)state] : null;
            if (mat == null) return;
            foreach (var r in stoneRenderers)
            {
                if (r == null) continue;
                var mats = r.sharedMaterials;
                for (int i = 0; i < mats.Length; i++) mats[i] = mat;
                r.sharedMaterials = mats;
            }
        }
    }
}
