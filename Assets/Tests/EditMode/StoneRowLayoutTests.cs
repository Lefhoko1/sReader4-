using System.Linq;
using NUnit.Framework;
using SReader.Game.Trek;

namespace SReader.Game.Trek.Tests
{
    /// <summary>
    /// The Reading House tray layout: no overlaps, nothing past the tray edge,
    /// balanced rows and top-to-bottom reading order, for the 5 / 11 / 20-word
    /// sentences verified in Blender. Text widths approximate the Blender
    /// measurements (0.115 m font) so the expected row splits match.
    /// </summary>
    public class StoneRowLayoutTests
    {
        static float[] Widths(params string[] words) => words.Select(w => w.Length * 0.068f).ToArray();

        static void AssertClean(StoneRowLayout.Result r, StoneRowLayout.Settings s)
        {
            foreach (var row in r.Placements.GroupBy(p => p.RowSlot))
            {
                var spans = row.OrderBy(p => p.X)
                    .Select(p => (lo: p.X - (p.StoneWidth / 2f + s.MossOverhang) * r.Scale,
                                  hi: p.X + (p.StoneWidth / 2f + s.MossOverhang) * r.Scale)).ToList();
                for (int i = 0; i + 1 < spans.Count; i++)
                    Assert.LessOrEqual(spans[i].hi, spans[i + 1].lo + 1e-4f, "neighbouring stones overlap");
                Assert.GreaterOrEqual(spans.First().lo, -s.UsableWidth / 2f - 1e-4f, "stone past the left edge");
                Assert.LessOrEqual(spans.Last().hi, s.UsableWidth / 2f + 1e-4f, "stone past the right edge");
            }
        }

        [Test]
        public void FiveWords_OneFrontRow_FullScale()
        {
            var s = new StoneRowLayout.Settings();
            var r = StoneRowLayout.Layout(Widths("The", "sun", "warms", "green", "leaves."), s);
            Assert.AreEqual(1, r.RowCount);
            Assert.AreEqual(1f, r.Scale, 1e-4f);
            Assert.IsTrue(r.Placements.All(p => p.RowSlot == 0));
            AssertClean(r, s);
        }

        [Test]
        public void ElevenWords_TwoBalancedRows_LineOneOnBackRow()
        {
            var s = new StoneRowLayout.Settings();
            var words = "Lefa packed his bag and set off early in the morning.".Split(' ');
            var r = StoneRowLayout.Layout(Widths(words), s);
            Assert.AreEqual(2, r.RowCount);
            int back = r.Placements.Count(p => p.RowSlot == 1), front = r.Placements.Count(p => p.RowSlot == 0);
            Assert.LessOrEqual(System.Math.Abs(back - front), 2, "rows should be balanced");
            Assert.AreEqual(1, r.Placements.First(p => p.WordIndex == 0).RowSlot, "first word belongs on the back row");
            AssertClean(r, s);
        }

        [Test]
        public void TwentyWords_WithPhotosynthesis_FitsTwoRows()
        {
            var s = new StoneRowLayout.Settings();
            var words = ("Photosynthesis allows green plants to convert sunlight into chemical energy " +
                         "that helps them grow strong roots, stems and green leaves.").Split(' ');
            Assert.AreEqual(20, words.Length);
            var r = StoneRowLayout.Layout(Widths(words), s);
            Assert.IsFalse(r.NeedsPagination);
            Assert.AreEqual(2, r.RowCount);
            Assert.AreEqual(20, r.Placements.Count);
            Assert.GreaterOrEqual(r.Scale, s.MinScale);
            AssertClean(r, s);
        }

        [Test]
        public void TooLongForTwoRows_AsksForPagination()
        {
            var words = Enumerable.Repeat("extraordinarily", 30).ToArray();
            var r = StoneRowLayout.Layout(Widths(words));
            Assert.IsTrue(r.NeedsPagination);
            Assert.AreEqual(0, r.Placements.Count);
        }

        [Test]
        public void ShortWord_GetsMinimumStoneWidth()
        {
            var s = new StoneRowLayout.Settings();
            Assert.AreEqual(s.MinStoneWidth, StoneRowLayout.StoneWidth(0.05f, s), 1e-4f);
        }
    }
}
