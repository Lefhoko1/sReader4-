using System.Collections.Generic;

namespace SReader.Game.Trek
{
    /// <summary>
    /// The playable shape of one assignment, built by <see cref="TrailBuilder"/>
    /// from the EXISTING assignment content format (never a new authoring format).
    /// Everything here is derived and recomputable — see spec §0.8. Presenters
    /// bind to this; they never re-derive gates from raw content themselves.
    /// </summary>
    public sealed class TrailData
    {
        public string AssignmentId;
        public string Title;
        public string PassageText;                              // full passage, verbatim
        public List<SentenceSpan> Sentences = new List<SentenceSpan>();  // start/end indices into PassageText
        public List<GateData> Gates = new List<GateData>();     // passage order (by PassageCharIndex)
        public string BiomeId = "meadows";                      // from course/subject, default "meadows"

        /// <summary>
        /// True when the source content was missing/malformed and this trail was
        /// built text-only (all reading, zero gates) so no assignment is unplayable
        /// (spec §2 TrailBuilder rules). The presenter logs <see cref="BuildWarning"/>.
        /// </summary>
        public bool IsTextOnlyFallback;

        /// <summary>Human-readable reason the fallback fired, for the presenter to log. Null when fine.</summary>
        public string BuildWarning;

        public bool HasGates => Gates != null && Gates.Count > 0;
    }

    /// <summary>Half-open character range [Start, End) into <see cref="TrailData.PassageText"/>.</summary>
    public sealed class SentenceSpan
    {
        public int Start;
        public int End;

        public SentenceSpan() { }
        public SentenceSpan(int start, int end) { Start = start; End = end; }

        public int Length => End - Start;
    }

    /// <summary>Gate challenge kinds. Comprehension is reserved (R-12) — modelled, not built yet.</summary>
    public enum GateType { Define, Illustrate, Fill, Comprehension /*reserved*/ }

    /// <summary>
    /// Lifecycle of a single gate. The three Solved* tiers mirror the hint ladder
    /// (R-8): Gold = no hints, Silver = hinted, Bronze = guided solve.
    /// </summary>
    public enum GateState { Locked, Active, Deferred, SolvedGold, SolvedSilver, SolvedBronze }

    /// <summary>
    /// One key word the tutor hid, turned into exactly one gate on the trail
    /// (R-4). Challenge checking reuses the existing model as-is via
    /// <see cref="ChallengePayload"/> (kept as object so the logic assembly stays
    /// free of the Assembly-CSharp challenge/UI types).
    /// </summary>
    public sealed class GateData
    {
        public string GateId;
        public GateType Type;
        public string Word;                 // the key word
        public int PassageCharIndex;        // where its blank sits in PassageText
        public int SentenceIndex;           // owning sentence (for R-5 and Journal)
        public object ChallengePayload;     // existing challenge model, reused as-is
        public int BasePoints;
        public GateState State = GateState.Locked;
        public int HintsUsed;               // 0..2, 3 = guided (R-8)
    }

    /// <summary>
    /// Resume point for one trek. This is the EXISTING per-device save (GameProgress)
    /// extended with the deferred queue, hint usage and elapsed time so a student
    /// can leave mid-trek and return to the exact checkpoint (spec §2).
    /// </summary>
    public sealed class TrekCheckpoint
    {
        public string AssignmentId;
        public int LastSolvedGateIndex = -1;                        // Kai stands after this gate (-1 = start)
        public List<string> DeferredGateIds = new List<string>();
        public Dictionary<string, int> HintsUsedByGate = new Dictionary<string, int>();
        public float SecondsElapsed;
        public int PointsSoFar;
    }
}
