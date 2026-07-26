using System;
using System.Collections.Generic;
using System.Linq;
using SReader.Domains.Assignments.Models;

namespace SReader.Game.Trek
{
    /// <summary>
    /// Validation for the three gate challenges. The checking semantics are the
    /// SAME as today's game (spec constraint: reuse, don't rewrite):
    ///   • Define — arrange the definition's words into order (ordinal compare);
    ///   • Fill   — arrange the word's letters (case-insensitive compare);
    ///   • Illustrate — pick the token's correctImage.
    /// Shuffles reuse <see cref="AssignmentContentBuilder"/> so piece pools behave
    /// exactly like the existing mini-games. Also computes the hint-ladder level-2
    /// assists (R-8.2): reveal half the letters / eliminate two images / lock two
    /// pieces. Pure and unit-tested; panels only render.
    /// </summary>
    public static class GateSolving
    {
        // ── Correct sequences (the answer key) ───────────────────────────────

        /// <summary>Define: the definition split into ordered words.</summary>
        public static List<string> DefineCorrect(ContentToken token) =>
            (token?.definition ?? "")
                .Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

        /// <summary>Fill: the word's letters/digits in order.</summary>
        public static List<string> FillCorrect(ContentToken token) =>
            (token?.text ?? "").Where(char.IsLetterOrDigit).Select(c => c.ToString()).ToList();

        /// <summary>Define: shuffled word pool (same shuffle the current game uses).</summary>
        public static List<string> DefinePool(ContentToken token) =>
            AssignmentContentBuilder.ShuffleWords(token?.definition);

        /// <summary>Fill: shuffled letter pool.</summary>
        public static List<string> FillPool(ContentToken token) =>
            AssignmentContentBuilder.ShuffleLetters(token?.text);

        /// <summary>Illustrate: shuffled image options.</summary>
        public static List<string> IllustrateOptions(ContentToken token) =>
            AssignmentContentBuilder.ShuffledCopy(token?.imageOptions ?? new List<string>());

        // ── Checking (identical semantics to AssignmentGameView) ─────────────

        /// <summary>Define check: exact ordered match (ordinal, like today).</summary>
        public static bool CheckDefine(IReadOnlyList<string> answer, IReadOnlyList<string> correct) =>
            string.Join("|", answer) == string.Join("|", correct);

        /// <summary>Fill check: ordered letters, case-insensitive (like today).</summary>
        public static bool CheckFill(IReadOnlyList<string> answer, IReadOnlyList<string> correct) =>
            string.Equals(string.Join("", answer), string.Join("", correct), StringComparison.OrdinalIgnoreCase);

        /// <summary>Illustrate check: the tapped option is the token's correct image.</summary>
        public static bool CheckIllustrate(string tapped, ContentToken token) =>
            !string.IsNullOrEmpty(tapped) && tapped == token?.correctImage;

        /// <summary>
        /// "Right pieces, wrong order" — used for the encouraging feedback line
        /// (same nudge the current game gives).
        /// </summary>
        public static bool SamePiecesWrongOrder(IReadOnlyList<string> answer, IReadOnlyList<string> correct, bool caseInsensitive)
        {
            if (answer.Count != correct.Count) return false;
            var cmp = caseInsensitive ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
            return answer.OrderBy(x => x, cmp).SequenceEqual(correct.OrderBy(x => x, cmp), cmp);
        }

        // ── Hint 2 assists (R-8.2) ────────────────────────────────────────────

        /// <summary>
        /// Fill hint 2: reveal half the letters — the indices (into the correct
        /// sequence) to show pre-placed. First ⌈n/2⌉ positions, so the student
        /// still finishes the word themselves.
        /// </summary>
        public static List<int> FillRevealIndices(IReadOnlyList<string> correct)
        {
            int n = correct?.Count ?? 0;
            int reveal = (n + 1) / 2;
            var list = new List<int>(reveal);
            for (int i = 0; i < reveal; i++) list.Add(i);
            return list;
        }

        /// <summary>Define hint 2: lock the first N pieces in place (spec: two).</summary>
        public const int DefineLockedPieces = 2;

        /// <summary>
        /// Illustrate hint 2: eliminate two wrong images. Returns the option urls
        /// to grey out (never the correct one; at most two).
        /// </summary>
        public static List<string> IllustrateEliminations(IReadOnlyList<string> options, ContentToken token)
        {
            return (options ?? Array.Empty<string>())
                .Where(o => o != token?.correctImage)
                .Take(2)
                .ToList();
        }
    }
}
