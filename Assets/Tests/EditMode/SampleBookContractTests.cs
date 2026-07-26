using System.Linq;
using NUnit.Framework;
using UnityEngine;
using SReader.Domains.Assignments.Models;
using SReader.Game.Trek;

namespace SReader.Game.Trek.Tests
{
    /// <summary>
    /// GL-0 acceptance for the frozen BookData contract (Docs/GL0_BOOKDATA_CONTRACT.md).
    /// Proves the two hand-authored sample books deserialise via the existing codec,
    /// round-trip losslessly, and build the expected rituals via <see cref="TrailBuilder"/>.
    /// Pure logic + Resources load; no play mode needed.
    /// </summary>
    public class SampleBookContractTests
    {
        const string NatureBook = "Books/SampleBook_Nature_MeadowAtDawn";
        const string ScienceBook = "Books/SampleBook_Science_FirstMachines";

        static AssignmentContent Load(string resourcePath)
        {
            var asset = Resources.Load<TextAsset>(resourcePath);
            Assert.IsNotNull(asset, $"Sample book not found in Resources: {resourcePath} " +
                                    "(expected under Assets/Resources/Books/).");
            var content = AssignmentContentCodec.FromJson(asset.text);
            Assert.IsNotNull(content, $"Sample book failed to deserialise: {resourcePath}");
            return content;
        }

        [TestCase(NatureBook)]
        [TestCase(ScienceBook)]
        public void SampleBook_Deserialises_WithTwoPagesAndFourActivities(string path)
        {
            var content = Load(path);
            Assert.AreEqual(2, content.pages.Count, "each sample book has 2 pages/paragraphs");
            Assert.AreEqual(4, content.ActivityCount, "each sample book has 4 interactive keywords");
            Assert.IsTrue(content.HasActivities);
        }

        [TestCase(NatureBook)]
        [TestCase(ScienceBook)]
        public void SampleBook_RoundTrips_PreservingActivities(string path)
        {
            var original = Load(path);
            var reparsed = AssignmentContentCodec.FromJson(AssignmentContentCodec.ToJson(original));

            Assert.IsNotNull(reparsed);
            Assert.AreEqual(original.ActivityCount, reparsed.ActivityCount);
            CollectionAssert.AreEqual(
                original.AllActivities.Select(t => (t.text, t.activity)).ToList(),
                reparsed.AllActivities.Select(t => (t.text, t.activity)).ToList(),
                "round-trip must preserve each keyword and its ritual type");
        }

        [Test]
        public void NatureBook_BuildsExpectedGatesInPassageOrder()
        {
            var trail = TrailBuilder.Build("nature-sample", "The Meadow at Dawn", Load(NatureBook));
            AssertGates(trail,
                ("dew", GateType.Fill),
                ("habitat", GateType.Define),
                ("butterfly", GateType.Illustrate),
                ("nectar", GateType.Define));
        }

        [Test]
        public void ScienceBook_BuildsExpectedGatesInPassageOrder()
        {
            var trail = TrailBuilder.Build("science-sample", "The First Machines", Load(ScienceBook));
            AssertGates(trail,
                ("machines", GateType.Fill),
                ("lever", GateType.Define),
                ("friction", GateType.Define),
                ("gear", GateType.Illustrate));
        }

        static void AssertGates(TrailData trail, params (string word, GateType type)[] expected)
        {
            Assert.IsFalse(trail.IsTextOnlyFallback, "well-formed content must not fall back to text-only");
            Assert.AreEqual(expected.Length, trail.Gates.Count, "gate count");

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].word, trail.Gates[i].Word, $"gate {i} word");
                Assert.AreEqual(expected[i].type, trail.Gates[i].Type, $"gate {i} ritual type");
                StringAssert.Contains(expected[i].word, trail.PassageText,
                    $"passage text must contain keyword '{expected[i].word}' (R-1: full passage verbatim)");
            }

            // Gates are strictly in passage order (context-before-puzzle, R-4/R-5).
            for (int i = 1; i < trail.Gates.Count; i++)
                Assert.Less(trail.Gates[i - 1].PassageCharIndex, trail.Gates[i].PassageCharIndex,
                    "gates must be sorted by passage position");
        }
    }
}
