using System.Collections.Generic;
using NUnit.Framework;
using SReader.Domains.Assignments.Models;
using SReader.Game.Trek;

namespace SReader.Game.Trek.Tests
{
    /// <summary>
    /// Unit tests for <see cref="TrailBuilder"/> — Phase 0 acceptance ("TrailBuilder
    /// tests green"). Pure logic, no scene / play mode needed.
    /// </summary>
    public class TrailBuilderTests
    {
        // ── helpers ──────────────────────────────────────────────────────────

        static ContentToken Word(string text) =>
            new ContentToken { text = text, isWord = true, activity = ActivityType.None };

        static ContentToken Sep(string text) =>
            new ContentToken { text = text, isWord = false, activity = ActivityType.None };

        static ContentToken Activity(string text, ActivityType type, int points = 10)
        {
            var t = new ContentToken { text = text, isWord = true, activity = type, points = points };
            if (type == ActivityType.Define) t.definition = "a small furry animal";
            if (type == ActivityType.Illustrate) { t.imageOptions = new List<string> { "a.png", "b.png" }; t.correctImage = "a.png"; }
            return t;
        }

        static AssignmentContent Content(params ContentToken[] tokens)
        {
            var sentence = new ContentSentence();
            sentence.tokens.AddRange(tokens);
            var page = new ContentPage { pageNumber = 1 };
            page.sentences.Add(sentence);
            var content = new AssignmentContent();
            content.pages.Add(page);
            return content;
        }

        // ── malformed / empty input → text-only fallback (spec §2) ───────────

        [Test]
        public void Build_NullContent_ReturnsTextOnlyFallback()
        {
            var trail = TrailBuilder.Build("a1", "Title", null);

            Assert.IsTrue(trail.IsTextOnlyFallback);
            Assert.IsNotNull(trail.BuildWarning);
            Assert.IsFalse(trail.HasGates);
            Assert.AreEqual("", trail.PassageText);
            Assert.AreEqual("a1", trail.AssignmentId);
        }

        [Test]
        public void Build_EmptyPages_ReturnsTextOnlyFallback()
        {
            var trail = TrailBuilder.Build("a1", "Title", new AssignmentContent());

            Assert.IsTrue(trail.IsTextOnlyFallback);
            Assert.IsFalse(trail.HasGates);
        }

        [Test]
        public void Build_ContentWithNoActivities_IsValidReadingOnlyTrail_NotFallback()
        {
            var trail = TrailBuilder.Build("a1", "Title", Content(Word("Hello"), Sep(" "), Word("world"), Sep(".")));

            Assert.IsFalse(trail.IsTextOnlyFallback);   // valid content, just no gates
            Assert.IsFalse(trail.HasGates);
            Assert.AreEqual("Hello world.", trail.PassageText);
        }

        // ── passage reconstruction is verbatim ───────────────────────────────

        [Test]
        public void Build_PassageText_IsVerbatimConcatenationOfTokens()
        {
            var content = Content(Word("The"), Sep(" "), Activity("cat", ActivityType.Define),
                                   Sep(" "), Word("sat"), Sep("."));
            var trail = TrailBuilder.Build("a1", "T", content);

            Assert.AreEqual("The cat sat.", trail.PassageText);
        }

        // ── one gate per hidden word, at the correct char slot ───────────────

        [Test]
        public void Build_OneActivityWord_CreatesOneGate_AtCorrectCharIndex()
        {
            var content = Content(Word("The"), Sep(" "), Activity("cat", ActivityType.Define),
                                   Sep(" "), Word("sat"), Sep("."));
            var trail = TrailBuilder.Build("a1", "T", content);

            Assert.AreEqual(1, trail.Gates.Count);
            var gate = trail.Gates[0];
            Assert.AreEqual("cat", gate.Word);
            Assert.AreEqual(4, gate.PassageCharIndex);                 // "The " = 4 chars
            Assert.AreEqual(GateType.Define, gate.Type);
            Assert.AreEqual(GateState.Locked, gate.State);
            Assert.AreEqual(0, gate.HintsUsed);
            Assert.AreSame(content.pages[0].sentences[0].tokens[2], gate.ChallengePayload);  // reused as-is
        }

        // ── gates sorted in passage order (R-4 / R-5) ────────────────────────

        [Test]
        public void Build_MultipleActivities_GatesInPassageOrder()
        {
            var content = Content(
                Activity("alpha", ActivityType.FillBlank), Sep(" "),
                Word("then"), Sep(" "),
                Activity("beta", ActivityType.Illustrate), Sep(" "),
                Activity("gamma", ActivityType.Define), Sep("."));
            var trail = TrailBuilder.Build("a1", "T", content);

            Assert.AreEqual(3, trail.Gates.Count);
            CollectionAssert.AreEqual(
                new[] { "alpha", "beta", "gamma" },
                new[] { trail.Gates[0].Word, trail.Gates[1].Word, trail.Gates[2].Word });

            // strictly increasing char indices
            Assert.Less(trail.Gates[0].PassageCharIndex, trail.Gates[1].PassageCharIndex);
            Assert.Less(trail.Gates[1].PassageCharIndex, trail.Gates[2].PassageCharIndex);
        }

        // ── activity → gate type mapping ─────────────────────────────────────

        [Test]
        public void Build_MapsActivityTypesToGateTypes()
        {
            Assert.AreEqual(GateType.Define, TrailBuilder.Build("a", "t", Content(Activity("w", ActivityType.Define))).Gates[0].Type);
            Assert.AreEqual(GateType.Illustrate, TrailBuilder.Build("a", "t", Content(Activity("w", ActivityType.Illustrate))).Gates[0].Type);
            Assert.AreEqual(GateType.Fill, TrailBuilder.Build("a", "t", Content(Activity("w", ActivityType.FillBlank))).Gates[0].Type);
        }

        // ── scoring inputs ───────────────────────────────────────────────────

        [Test]
        public void Build_UsesTokenPoints_AndClampsNegativeToZero()
        {
            Assert.AreEqual(25, TrailBuilder.Build("a", "t", Content(Activity("w", ActivityType.FillBlank, 25))).Gates[0].BasePoints);
            Assert.AreEqual(0, TrailBuilder.Build("a", "t", Content(Activity("w", ActivityType.FillBlank, -5))).Gates[0].BasePoints);
        }

        // ── sentence spans + gate ownership ──────────────────────────────────

        [Test]
        public void Build_ResolvesGateSentenceIndex_AcrossTwoSentences()
        {
            // Two sentences on one page: gate lives in the second sentence.
            var s1 = new ContentSentence();
            s1.tokens.AddRange(new[] { Word("One"), Sep(" "), Word("two"), Sep(". ") });
            var s2 = new ContentSentence();
            s2.tokens.AddRange(new[] { Word("Find"), Sep(" "), Activity("me", ActivityType.FillBlank), Sep(".") });
            var page = new ContentPage { pageNumber = 1 };
            page.sentences.Add(s1);
            page.sentences.Add(s2);
            var content = new AssignmentContent();
            content.pages.Add(page);

            var trail = TrailBuilder.Build("a1", "T", content);

            Assert.AreEqual(2, trail.Sentences.Count);
            Assert.AreEqual(1, trail.Gates.Count);
            Assert.AreEqual(1, trail.Gates[0].SentenceIndex);   // owned by the 2nd sentence

            // the sentence span actually contains the gate's char index
            var span = trail.Sentences[trail.Gates[0].SentenceIndex];
            Assert.LessOrEqual(span.Start, trail.Gates[0].PassageCharIndex);
            Assert.Less(trail.Gates[0].PassageCharIndex, span.End);
        }

        // ── biome default ────────────────────────────────────────────────────

        [Test]
        public void Build_DefaultsBiome_WhenBlank()
        {
            Assert.AreEqual("meadows", TrailBuilder.Build("a", "t", Content(Word("hi")), "  ").BiomeId);
            Assert.AreEqual("storybook", TrailBuilder.Build("a", "t", Content(Word("hi")), "storybook").BiomeId);
        }

        // ── plain-text convenience (placeholder mode / GEN-4) ────────────────

        [Test]
        public void BuildFromPlainText_ProducesReadingOnlyTrail_WithSentences()
        {
            var trail = TrailBuilder.BuildFromPlainText("sample", "Sample", "Hello there. How are you?");

            Assert.IsFalse(trail.HasGates);
            Assert.IsFalse(trail.IsTextOnlyFallback);            // there IS content, it's just gate-free
            Assert.AreEqual("Hello there. How are you?", trail.PassageText);
            Assert.AreEqual(2, trail.Sentences.Count);
        }
    }
}
