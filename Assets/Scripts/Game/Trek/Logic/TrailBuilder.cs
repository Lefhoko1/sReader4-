using System;
using System.Collections.Generic;
using System.Text;
using SReader.Domains.Assignments.Models;

namespace SReader.Game.Trek
{
    /// <summary>
    /// Maps the EXISTING assignment content format (<see cref="AssignmentContent"/>)
    /// into a playable <see cref="TrailData"/>. This is pure, engine-free and
    /// deterministic so it can be unit-tested (spec §2 acceptance).
    ///
    /// Rules (spec §2):
    ///  • one gate per hidden (activity) word;
    ///  • gates sorted by PassageCharIndex (passage order — R-4/R-5);
    ///  • sentence spans computed from the authored sentence structure;
    ///  • malformed/empty content → a text-only trail (all reading, zero gates)
    ///    with <see cref="TrailData.IsTextOnlyFallback"/> set, so no assignment is
    ///    ever unplayable. The caller logs <see cref="TrailData.BuildWarning"/>.
    /// </summary>
    public static class TrailBuilder
    {
        public const string DefaultBiome = "meadows";

        /// <summary>Build a trail from parsed assignment content.</summary>
        public static TrailData Build(string assignmentId, string title, AssignmentContent content, string biomeId = DefaultBiome)
        {
            var trail = new TrailData
            {
                AssignmentId = assignmentId,
                Title = title,
                BiomeId = string.IsNullOrWhiteSpace(biomeId) ? DefaultBiome : biomeId.Trim()
            };

            if (content == null || content.pages == null || content.pages.Count == 0)
            {
                trail.PassageText = "";
                trail.IsTextOnlyFallback = true;
                trail.BuildWarning = "Assignment has no readable content; built a text-only trail (zero gates).";
                return trail;
            }

            var sb = new StringBuilder();

            foreach (var page in content.pages)
            {
                if (page?.sentences == null) continue;

                foreach (var sentence in page.sentences)
                {
                    if (sentence?.tokens == null) continue;

                    int spanStart = sb.Length;

                    foreach (var token in sentence.tokens)
                    {
                        if (token == null) continue;

                        int tokenStart = sb.Length;
                        sb.Append(token.text ?? "");

                        // One gate per hidden (activity) word, at its exact char slot.
                        if (token.IsActivity)
                        {
                            trail.Gates.Add(new GateData
                            {
                                GateId = "gate-" + tokenStart,
                                Type = MapType(token.activity),
                                Word = token.text ?? "",
                                PassageCharIndex = tokenStart,
                                SentenceIndex = 0,                 // resolved in the post-pass below
                                ChallengePayload = token,          // reuse the existing challenge model as-is
                                BasePoints = Math.Max(0, token.points),
                                State = GateState.Locked,
                                HintsUsed = 0
                            });
                        }
                    }

                    int spanEnd = sb.Length;
                    if (spanEnd > spanStart)
                        trail.Sentences.Add(new SentenceSpan(spanStart, spanEnd));
                }
            }

            trail.PassageText = sb.ToString();

            // Defensive: keep gates in strict passage order (context-before-puzzle, R-5).
            trail.Gates.Sort((a, b) => a.PassageCharIndex.CompareTo(b.PassageCharIndex));

            // Resolve each gate's owning sentence from its char index (robust to any
            // empty sentences that were skipped above).
            foreach (var gate in trail.Gates)
                gate.SentenceIndex = FindSentenceIndex(trail.Sentences, gate.PassageCharIndex);

            return trail;
        }

        /// <summary>
        /// Convenience for the bundled sample passage / placeholder mode (GEN-4):
        /// tokenises plain text into a reading-only trail (zero gates).
        /// </summary>
        public static TrailData BuildFromPlainText(string assignmentId, string title, string passageText, string biomeId = DefaultBiome)
        {
            var content = new AssignmentContent();
            content.pages.Add(AssignmentContentBuilder.PageFromText(1, passageText ?? ""));
            return Build(assignmentId, title, content, biomeId);
        }

        static int FindSentenceIndex(List<SentenceSpan> spans, int charIndex)
        {
            if (spans == null || spans.Count == 0) return 0;
            for (int i = 0; i < spans.Count; i++)
                if (charIndex >= spans[i].Start && charIndex < spans[i].End) return i;
            // Char index past the last recorded span → clamp to the final sentence.
            return spans.Count - 1;
        }

        static GateType MapType(ActivityType activity)
        {
            switch (activity)
            {
                case ActivityType.Define:     return GateType.Define;
                case ActivityType.Illustrate: return GateType.Illustrate;
                case ActivityType.FillBlank:  return GateType.Fill;
                default:                      return GateType.Fill;
            }
        }
    }
}
