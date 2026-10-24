using System;
using System.Collections.Generic;

namespace SReader.Game.Trek
{
    /// <summary>
    /// Lays one sentence's word stones out on the Reading House tray. Pure and
    /// engine-free so it stays unit-testable: the presenter measures each word
    /// (TextMeshPro) and hands in the widths; this decides scale, rows and
    /// positions. It is the same rule set that was verified in Blender for 5,
    /// 11 and 20-word sentences (Reading House handoff v04 §2).
    ///
    ///  • One row if the sentence fits; otherwise two balanced rows.
    ///  • With two rows, line 1 sits on the BACK row (top of the screen), so the
    ///    stones read top-to-bottom like the sentence does.
    ///  • One uniform scale per sentence, stepped down to <see cref="Settings.MinScale"/>;
    ///    below that the sentence must be paginated.
    ///  • Each footprint includes the moss overhang, so neighbours never touch.
    /// </summary>
    public static class StoneRowLayout
    {
        public sealed class Settings
        {
            public float UsableWidth = 6.30f;     // tray width available to stones (m)
            public float Gap = 0.06f;             // between footprints, before scaling
            public float MossOverhang = 0.044f;   // moss reaches past the stone on each side
            public float TextPadding = 0.20f;     // stone width = text width + padding
            public float MinStoneWidth = 0.50f;
            public float MinScale = 0.70f;
            public float ScaleStep = 0.02f;
            public int MaxRows = 2;
        }

        /// <summary>Where one word's stone goes. X is the stone centre along the tray (0 = middle).</summary>
        public struct Placement
        {
            public int WordIndex;
            public int RowSlot;        // 0 = front row, 1 = back row
            public float X;
            public float StoneWidth;   // unscaled stone width; the root is scaled by Result.Scale
        }

        public sealed class Result
        {
            public float Scale = 1f;
            public int RowCount;
            public bool NeedsPagination;
            public readonly List<Placement> Placements = new List<Placement>();
        }

        public static float StoneWidth(float textWidth, Settings s)
            => Math.Max(s.MinStoneWidth, textWidth + s.TextPadding);

        public static Result Layout(IReadOnlyList<float> textWidths, Settings settings = null)
        {
            var s = settings ?? new Settings();
            var result = new Result();
            int n = textWidths?.Count ?? 0;
            if (n == 0) return result;

            for (float scale = 1f; scale >= s.MinScale - 1e-4f; scale -= s.ScaleStep)
            {
                var footprints = new float[n];
                for (int i = 0; i < n; i++)
                    footprints[i] = (StoneWidth(textWidths[i], s) + 2f * s.MossOverhang) * scale;
                float gap = s.Gap * scale;

                if (RowWidth(footprints, 0, n, gap) <= s.UsableWidth)
                    return Fill(result, textWidths, s, scale, footprints, gap, new[] { (0, n) });

                if (s.MaxRows < 2) continue;
                int bestSplit = -1;
                float bestWidest = float.MaxValue;
                for (int k = 1; k < n; k++)
                {
                    float a = RowWidth(footprints, 0, k, gap), b = RowWidth(footprints, k, n, gap);
                    if (a > s.UsableWidth || b > s.UsableWidth) continue;
                    float widest = Math.Max(a, b);
                    if (widest < bestWidest) { bestWidest = widest; bestSplit = k; }
                }
                if (bestSplit > 0)
                    return Fill(result, textWidths, s, scale, footprints, gap, new[] { (0, bestSplit), (bestSplit, n) });
            }

            result.NeedsPagination = true;
            return result;
        }

        static float RowWidth(float[] footprints, int from, int to, float gap)
        {
            float w = 0f;
            for (int i = from; i < to; i++) w += footprints[i];
            return w + gap * Math.Max(0, to - from - 1);
        }

        static Result Fill(Result result, IReadOnlyList<float> textWidths, Settings s, float scale,
            float[] footprints, float gap, (int from, int to)[] lines)
        {
            result.Scale = scale;
            result.RowCount = lines.Length;
            for (int line = 0; line < lines.Length; line++)
            {
                // two rows: line 1 -> back slot (1), line 2 -> front slot (0)
                int slot = lines.Length == 1 ? 0 : 1 - line;
                var (from, to) = lines[line];
                float x = -RowWidth(footprints, from, to, gap) / 2f;
                for (int i = from; i < to; i++)
                {
                    result.Placements.Add(new Placement
                    {
                        WordIndex = i,
                        RowSlot = slot,
                        X = x + footprints[i] / 2f,
                        StoneWidth = StoneWidth(textWidths[i], s),
                    });
                    x += footprints[i] + gap;
                }
            }
            return result;
        }
    }
}
