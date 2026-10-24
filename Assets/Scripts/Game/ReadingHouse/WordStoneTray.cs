using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using SReader.Game.Trek;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// Builds the current sentence's word stones on the dashboard tray. Measures
    /// each word with a stone label, asks the pure <see cref="StoneRowLayout"/>
    /// where everything goes, and places pooled <see cref="WordStoneView"/>s on
    /// the two row anchors exported from Blender. Stones are reused between
    /// sentences. Presentation only.
    /// </summary>
    public sealed class WordStoneTray : MonoBehaviour
    {
        [Header("Wiring (set by the scene builder)")]
        public WordStoneView stonePrefab;
        public Transform frontRowAnchor;     // SR_ANCHOR_TRAY_ROW_0
        public Transform backRowAnchor;      // SR_ANCHOR_TRAY_ROW_1
        public Transform trayLeftMarker;     // SR_ANCHOR_PREVIOUS_SENTENCE: reading starts on this side
        public Transform trayRightMarker;    // SR_ANCHOR_NEXT_SENTENCE

        [Tooltip("Label line height in metres at stone scale 1 (matches the Blender preview text).")]
        public float labelLineHeight = 0.14f;

        readonly List<WordStoneView> pool = new List<WordStoneView>();
        readonly List<WordStoneView> active = new List<WordStoneView>();
        readonly StoneRowLayout.Settings settings = new StoneRowLayout.Settings();
        WordStoneView measureProbe;
        float labelFontSize = -1f;

        public IReadOnlyList<WordStoneView> Active => active;
        public StoneRowLayout.Result LastLayout { get; private set; }

        /// <summary>Lay out a sentence. Returns false when it is too long for two rows (the caller paginates).</summary>
        public bool Show(IReadOnlyList<string> words, Func<int, StoneState> stateOf)
        {
            HideAll();
            if (words == null || words.Count == 0) return true;

            var widths = new float[words.Count];
            for (int i = 0; i < words.Count; i++) widths[i] = MeasureWord(words[i]);

            var layout = StoneRowLayout.Layout(widths, settings);
            LastLayout = layout;
            if (layout.NeedsPagination) return false;

            Vector3 along = (trayRightMarker.position - trayLeftMarker.position).normalized;
            foreach (var p in layout.Placements)
            {
                var stone = TakeFromPool();
                var row = p.RowSlot == 0 ? frontRowAnchor : backRowAnchor;
                stone.transform.SetPositionAndRotation(row.position + along * p.X, transform.rotation);
                stone.transform.localScale = Vector3.one * layout.Scale;
                stone.Show(p.WordIndex, words[p.WordIndex], p.StoneWidth);
                stone.SetState(stateOf != null ? stateOf(p.WordIndex) : StoneState.Normal);
            }
            return true;
        }

        /// <summary>How many words, starting at <paramref name="start"/>, fit on the two rows (at least one).</summary>
        public int FitCount(IReadOnlyList<string> words, int start)
        {
            int best = 1;
            var widths = new List<float>();
            for (int i = start; i < words.Count; i++)
            {
                widths.Add(MeasureWord(words[i]));
                if (StoneRowLayout.Layout(widths.ToArray(), settings).NeedsPagination) break;
                best = widths.Count;
            }
            return best;
        }

        public void HideAll()
        {
            foreach (var s in active) s.gameObject.SetActive(false);
            active.Clear();
        }

        /// <summary>Unscaled width (metres) a word needs on a stone label.</summary>
        public float MeasureWord(string word)
        {
            if (measureProbe == null)
            {
                measureProbe = CreateStone("WordStone_MeasureProbe");
                measureProbe.gameObject.SetActive(false);
            }
            var tmp = measureProbe.label;
            return tmp.GetPreferredValues(word).x * tmp.transform.localScale.x;
        }

        WordStoneView TakeFromPool()
        {
            WordStoneView stone = null;
            foreach (var s in pool)
                if (!s.gameObject.activeSelf) { stone = s; break; }
            if (stone == null)
            {
                stone = CreateStone($"WordStone_{pool.Count:00}");
                pool.Add(stone);
            }
            stone.gameObject.SetActive(true);
            active.Add(stone);
            return stone;
        }

        WordStoneView CreateStone(string objectName)
        {
            var stone = Instantiate(stonePrefab, transform);
            stone.name = objectName;
            if (labelFontSize < 0f) labelFontSize = FontSizeForLineHeight(stone.label, labelLineHeight);
            stone.label.fontSize = labelFontSize;
            return stone;
        }

        /// <summary>TextMeshPro sizes are font units, not metres: find the size whose line is `height` tall.</summary>
        static float FontSizeForLineHeight(TextMeshPro tmp, float height)
        {
            tmp.fontSize = 1f;
            float h = tmp.GetPreferredValues("Hg").y * tmp.transform.localScale.y;
            return h > 1e-5f ? height / h : 1f;
        }
    }
}
