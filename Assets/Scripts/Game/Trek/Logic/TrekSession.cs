using System;
using System.Collections.Generic;
using System.Linq;

namespace SReader.Game.Trek
{
    /// <summary>Tunables for one trek run. Defaults follow the spec.</summary>
    public sealed class TrekConfig
    {
        /// <summary>Countdown per gate, in seconds. 0 = no timer (untimed trek).</summary>
        public int SecondsPerGate = 45;

        /// <summary>Hint 1 cost — fraction of the word's points lost (R-8.1).</summary>
        public float Hint1Deduction = 0.40f;

        /// <summary>Hint 2 cost (R-8.2).</summary>
        public float Hint2Deduction = 0.70f;

        /// <summary>
        /// Extra fraction lost when the gate's timer expired and it auto-deferred
        /// (R-11: "small time penalty to points" — pace pressure, never a skip).
        /// </summary>
        public float TimeoutPenalty = 0.10f;
    }

    /// <summary>Where the student is in the trek's completion flow.</summary>
    public enum TrekPhase
    {
        Walking,       // between gates (or before the first)
        AtGate,        // a gate panel is open / awaiting solve or defer
        CleanupCamp,   // mandatory final stop: re-queue of deferred gates (R-7)
        Complete       // every gate solved (R-6) — submission may fire (R-9)
    }

    /// <summary>The outcome of a fully cleared trail (R-10).</summary>
    public sealed class TrekResult
    {
        public int Points;               // Σ word points after hint/timeout deductions
        public int TotalBasePoints;      // Σ base points (the denominator)
        public int Marks;                // Points scaled to the assignment's max score
        public int MaxScore;             // the assignment's max score (0 = unscaled)
        public int Stars;                // derived: ≥90% ★★★, ≥70% ★★, cleared ★
        public int GatesSolvedGold;
        public int GatesSolvedSilver;
        public int GatesSolvedBronze;

        /// <summary>Per-word hint usage ("word:hints", guided = 3) for the submission payload (R-10).</summary>
        public List<string> HintReport = new List<string>();
    }

    /// <summary>
    /// The trek's rules engine — pure, deterministic, engine-free, unit-tested.
    /// Owns every Reading &amp; Completion Contract rule that is state (R-4…R-11):
    /// gate ordering, Later/deferral + Cleanup Camp, the hint ladder, timer
    /// semantics, scoring and checkpointing. Presenters only render this state
    /// and forward input (MVVM: no rule lives in a MonoBehaviour).
    /// </summary>
    public sealed class TrekSession
    {
        readonly TrailData trail;
        readonly TrekConfig config;

        // Indices into trail.Gates, in passage order (R-4). The main queue is the
        // first pass; the deferred queue re-presents at Cleanup Camp (R-7).
        readonly List<int> mainQueue = new List<int>();
        readonly List<int> deferredQueue = new List<int>();
        readonly HashSet<int> timedOut = new HashSet<int>();      // gates that auto-deferred on expiry (R-11)
        readonly Dictionary<int, int> pointsByGate = new Dictionary<int, int>();

        int activeGate = -1;             // index into trail.Gates, -1 = none open
        float gateSecondsLeft;
        bool timerPaused;                // Reading View pauses the timer (R-11)

        public TrekSession(TrailData trail, TrekConfig config = null)
        {
            this.trail = trail ?? throw new ArgumentNullException(nameof(trail));
            this.config = config ?? new TrekConfig();

            for (int i = 0; i < trail.Gates.Count; i++)
                if (!IsSolved(trail.Gates[i].State)) mainQueue.Add(i);

            Phase = trail.Gates.Count == 0 || mainQueue.Count == 0
                ? (AllSolved ? TrekPhase.Complete : TrekPhase.Walking)
                : TrekPhase.Walking;
            if (trail.Gates.Count == 0) Phase = TrekPhase.Walking;   // text-only trail: walk + read, complete at the end
        }

        // ── Observable state (presenters render this) ────────────────────────

        public TrekPhase Phase { get; private set; }
        public TrailData Trail => trail;
        public TrekConfig Config => config;
        public float SecondsElapsed { get; private set; }
        public int Points { get; private set; }

        public int TotalGates => trail.Gates.Count;
        public int SolvedGates => trail.Gates.Count(g => IsSolved(g.State));
        public int DeferredCount => deferredQueue.Count;
        public bool AllSolved => trail.Gates.All(g => IsSolved(g.State));

        /// <summary>The gate currently open, or null while walking.</summary>
        public GateData ActiveGate => activeGate >= 0 ? trail.Gates[activeGate] : null;
        public int ActiveGateIndex => activeGate;

        /// <summary>Seconds left on the open gate (0 when untimed or no gate).</summary>
        public float GateSecondsLeft => activeGate >= 0 && config.SecondsPerGate > 0 ? gateSecondsLeft : 0f;
        public bool TimerPaused => timerPaused;

        /// <summary>
        /// The sentence the ribbon should highlight (R-1/R-5): the next gate's
        /// sentence while trekking, or the last sentence once everything is done.
        /// </summary>
        public int CurrentSentenceIndex
        {
            get
            {
                var gate = ActiveGate;
                if (gate != null) return gate.SentenceIndex;
                var next = PeekNextGate();
                if (next != null) return next.SentenceIndex;
                return Math.Max(0, trail.Sentences.Count - 1);
            }
        }

        /// <summary>The gate Kai walks toward next (null when none remain in this phase).</summary>
        public GateData PeekNextGate()
        {
            var queue = Phase == TrekPhase.CleanupCamp ? deferredQueue : mainQueue;
            return queue.Count > 0 ? trail.Gates[queue[0]] : null;
        }

        // ── Gate flow ─────────────────────────────────────────────────────────

        /// <summary>
        /// Kai arrived at the next gate — open it. The presenter must have
        /// scrolled the gate's sentence into the highlight first (R-5).
        /// Returns the opened gate, or null if none remain (→ maybe camp/complete).
        /// </summary>
        public GateData ArriveAtGate()
        {
            if (Phase == TrekPhase.Complete || activeGate >= 0) return ActiveGate;

            var queue = Phase == TrekPhase.CleanupCamp ? deferredQueue : mainQueue;
            if (queue.Count == 0) { AdvancePhase(); return null; }

            activeGate = queue[0];
            queue.RemoveAt(0);
            trail.Gates[activeGate].State = GateState.Active;
            gateSecondsLeft = config.SecondsPerGate;
            timerPaused = false;
            Phase = Phase == TrekPhase.CleanupCamp ? TrekPhase.CleanupCamp : TrekPhase.AtGate;
            return ActiveGate;
        }

        /// <summary>
        /// Advance the hint ladder on the open gate (R-8). Level 1 → −40%,
        /// level 2 → −70%. Returns the new level (clamped at 2; guided is separate).
        /// </summary>
        public int UseHint()
        {
            var gate = RequireActiveGate();
            if (gate.HintsUsed < 2) gate.HintsUsed++;
            return gate.HintsUsed;
        }

        /// <summary>
        /// Switch the open gate to a guided solve (R-8.3): the answer is shown and
        /// stepped through; the word scores 0 but still counts as solved (bronze).
        /// </summary>
        public void RequestGuidedSolve()
        {
            var gate = RequireActiveGate();
            gate.HintsUsed = 3;
        }

        /// <summary>
        /// The open gate was solved (the challenge checked correct, or the guided
        /// walk-through finished). Awards points after hint/timeout deductions and
        /// closes the gate. Returns the points earned for this word.
        /// </summary>
        public int SolveActiveGate()
        {
            var gate = RequireActiveGate();

            float multiplier;
            GateState tier;
            switch (gate.HintsUsed)
            {
                case 0:  multiplier = 1f;                            tier = GateState.SolvedGold;   break;
                case 1:  multiplier = 1f - config.Hint1Deduction;    tier = GateState.SolvedSilver; break;
                case 2:  multiplier = 1f - config.Hint2Deduction;    tier = GateState.SolvedSilver; break;
                default: multiplier = 0f;                            tier = GateState.SolvedBronze; break;   // guided
            }
            if (timedOut.Contains(activeGate)) multiplier *= 1f - config.TimeoutPenalty;   // R-11

            int earned = (int)Math.Round(gate.BasePoints * Math.Max(0f, multiplier));
            gate.State = tier;
            pointsByGate[activeGate] = earned;
            Points += earned;

            activeGate = -1;
            AdvancePhase();
            return earned;
        }

        /// <summary>
        /// "Later" — defer the open gate (R-7). It re-queues at Cleanup Camp, in
        /// order. Deferral by itself never costs marks (R-10).
        /// </summary>
        public void DeferActiveGate()
        {
            var gate = RequireActiveGate();
            gate.State = GateState.Deferred;
            deferredQueue.Add(activeGate);
            activeGate = -1;
            AdvancePhase();
        }

        // ── Timer (R-11) ──────────────────────────────────────────────────────

        /// <summary>Reading View pauses the gate timer (R-2/R-11).</summary>
        public void PauseTimer() => timerPaused = true;
        public void ResumeTimer() => timerPaused = false;

        /// <summary>
        /// Tick the open gate's countdown. Returns true when it expired this tick:
        /// the gate auto-defers to Cleanup Camp with a small points penalty and
        /// the trek continues — the timer never forces a skip (R-11).
        /// </summary>
        public bool TickTimer(float deltaSeconds)
        {
            if (!timerPaused) SecondsElapsed += Math.Max(0f, deltaSeconds);
            if (activeGate < 0 || config.SecondsPerGate <= 0 || timerPaused) return false;

            gateSecondsLeft -= deltaSeconds;
            if (gateSecondsLeft > 0f) return false;

            timedOut.Add(activeGate);
            DeferActiveGate();
            return true;
        }

        // ── Completion (R-6 / R-9 / R-10) ─────────────────────────────────────

        /// <summary>
        /// True only when every gate is solved — the trail can never complete with
        /// an unsolved gate and there is no skip (R-6). Submission may fire only
        /// when this is true (R-9).
        /// </summary>
        public bool CanComplete => AllSolved;

        /// <summary>
        /// Finish the trail. Throws if any gate is unsolved (R-6 is inviolable —
        /// callers must check <see cref="CanComplete"/>). Returns the scored result.
        /// </summary>
        public TrekResult CompleteTrail(int assignmentMaxScore)
        {
            if (!CanComplete)
                throw new InvalidOperationException("R-6: the trail cannot complete while any gate is unsolved.");
            Phase = TrekPhase.Complete;

            var result = new TrekResult
            {
                Points = Points,
                TotalBasePoints = trail.Gates.Sum(g => Math.Max(0, g.BasePoints)),
                MaxScore = Math.Max(0, assignmentMaxScore),
                GatesSolvedGold = trail.Gates.Count(g => g.State == GateState.SolvedGold),
                GatesSolvedSilver = trail.Gates.Count(g => g.State == GateState.SolvedSilver),
                GatesSolvedBronze = trail.Gates.Count(g => g.State == GateState.SolvedBronze),
            };

            // R-10: marks scale to the assignment's max score when both are known.
            result.Marks = result.MaxScore > 0 && result.TotalBasePoints > 0
                ? (int)Math.Round(result.Points / (double)result.TotalBasePoints * result.MaxScore)
                : result.Points;

            double fraction = result.TotalBasePoints > 0 ? result.Points / (double)result.TotalBasePoints : 1.0;
            result.Stars = fraction >= 0.90 ? 3 : fraction >= 0.70 ? 2 : 1;

            foreach (var g in trail.Gates)
                result.HintReport.Add($"{g.Word}:{g.HintsUsed}");
            return result;
        }

        // ── Checkpoint (existing per-device resume, extended — spec §2) ───────

        /// <summary>Snapshot the run so the student can leave and continue (R-9: an early exit stays *in progress*).</summary>
        public TrekCheckpoint ToCheckpoint()
        {
            var cp = new TrekCheckpoint
            {
                AssignmentId = trail.AssignmentId,
                SecondsElapsed = SecondsElapsed,
                PointsSoFar = Points,
                LastSolvedGateIndex = LastSolvedIndex(),
            };
            foreach (var i in deferredQueue) cp.DeferredGateIds.Add(trail.Gates[i].GateId);
            if (activeGate >= 0 && trail.Gates[activeGate].State == GateState.Deferred)
                cp.DeferredGateIds.Add(trail.Gates[activeGate].GateId);
            foreach (var g in trail.Gates)
                if (g.HintsUsed > 0) cp.HintsUsedByGate[g.GateId] = g.HintsUsed;
            return cp;
        }

        /// <summary>
        /// Rebuild a session from the trail + saved state. `solvedBoardIndices` and
        /// `score` come from the EXISTING GameProgress resume point; the checkpoint
        /// carries the trek extras (deferred queue, hints, elapsed).
        /// </summary>
        public static TrekSession Restore(TrailData trail, TrekConfig config,
            IReadOnlyList<int> solvedBoardIndices, int score, TrekCheckpoint checkpoint)
        {
            // Re-mark solved gates first (board index i == gate i: both are document order).
            if (solvedBoardIndices != null)
                foreach (var i in solvedBoardIndices)
                    if (i >= 0 && i < trail.Gates.Count && !IsSolved(trail.Gates[i].State))
                        trail.Gates[i].State = trail.Gates[i].HintsUsed >= 3 ? GateState.SolvedBronze
                            : trail.Gates[i].HintsUsed > 0 ? GateState.SolvedSilver : GateState.SolvedGold;

            if (checkpoint != null)
            {
                foreach (var g in trail.Gates)
                    if (checkpoint.HintsUsedByGate.TryGetValue(g.GateId, out var h)) g.HintsUsed = h;

                foreach (var id in checkpoint.DeferredGateIds)
                {
                    var g = trail.Gates.FirstOrDefault(x => x.GateId == id);
                    if (g != null && !IsSolved(g.State)) g.State = GateState.Deferred;
                }
            }

            var session = new TrekSession(trail, config) { Points = score };
            if (checkpoint != null) session.SecondsElapsed = checkpoint.SecondsElapsed;

            // Deferred gates leave the main queue and join the deferred queue, in passage order.
            for (int i = 0; i < trail.Gates.Count; i++)
                if (trail.Gates[i].State == GateState.Deferred)
                {
                    session.mainQueue.Remove(i);
                    session.deferredQueue.Add(i);
                }
            session.deferredQueue.Sort();
            session.AdvancePhaseIfIdle();
            return session;
        }

        /// <summary>Solved gate indices in the SHARED deterministic board order (for GameProgress).</summary>
        public List<int> SolvedBoardIndices()
        {
            var list = new List<int>();
            for (int i = 0; i < trail.Gates.Count; i++)
                if (IsSolved(trail.Gates[i].State)) list.Add(i);
            return list;
        }

        // ── internals ────────────────────────────────────────────────────────

        void AdvancePhase()
        {
            if (activeGate >= 0) return;

            if (mainQueue.Count > 0) { Phase = TrekPhase.Walking; return; }

            // Main pass done. Deferred gates make Cleanup Camp mandatory (R-7);
            // leaving it forward requires zero unsolved gates.
            if (deferredQueue.Count > 0) { Phase = TrekPhase.CleanupCamp; return; }

            Phase = AllSolved || trail.Gates.Count == 0 ? TrekPhase.Complete : TrekPhase.Walking;
            if (Phase == TrekPhase.Complete && trail.Gates.Count == 0) Phase = TrekPhase.Complete;
        }

        void AdvancePhaseIfIdle() { if (activeGate < 0) AdvancePhase(); }

        int LastSolvedIndex()
        {
            int last = -1;
            for (int i = 0; i < trail.Gates.Count; i++)
                if (IsSolved(trail.Gates[i].State)) last = i;
            return last;
        }

        GateData RequireActiveGate()
        {
            if (activeGate < 0) throw new InvalidOperationException("No gate is open.");
            return trail.Gates[activeGate];
        }

        static bool IsSolved(GateState s)
            => s == GateState.SolvedGold || s == GateState.SolvedSilver || s == GateState.SolvedBronze;
    }
}
