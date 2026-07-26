using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SReader.Domains.Assignments.Models;
using SReader.Game.Trek;

namespace SReader.Game.Trek.Tests
{
    /// <summary>
    /// Contract tests for <see cref="TrekSession"/> — the Reading &amp; Completion
    /// Contract rules that are state (R-6…R-11). These are the guarantees the
    /// product is built on; do not weaken them to make a change easier.
    /// </summary>
    public class TrekSessionTests
    {
        static TrailData Trail(params int[] basePoints)
        {
            var trail = new TrailData { AssignmentId = "a1", Title = "T", PassageText = "x" };
            for (int i = 0; i < basePoints.Length; i++)
            {
                trail.Gates.Add(new GateData
                {
                    GateId = "g" + i,
                    Type = GateType.Fill,
                    Word = "word" + i,
                    PassageCharIndex = i * 10,
                    SentenceIndex = i,
                    BasePoints = basePoints[i],
                    State = GateState.Locked
                });
                trail.Sentences.Add(new SentenceSpan(i * 10, i * 10 + 9));
            }
            return trail;
        }

        static TrekConfig Untimed() => new TrekConfig { SecondsPerGate = 0 };

        // ── Basic flow: gates open in passage order ───────────────────────────

        [Test]
        public void Gates_OpenInPassageOrder_AndSolveAdvances()
        {
            var s = new TrekSession(Trail(10, 10, 10), Untimed());

            Assert.AreEqual(TrekPhase.Walking, s.Phase);
            Assert.AreEqual("g0", s.ArriveAtGate().GateId);
            s.SolveActiveGate();
            Assert.AreEqual("g1", s.ArriveAtGate().GateId);
            s.SolveActiveGate();
            Assert.AreEqual("g2", s.ArriveAtGate().GateId);
            s.SolveActiveGate();

            Assert.AreEqual(TrekPhase.Complete, s.Phase);
            Assert.AreEqual(3, s.SolvedGates);
        }

        // ── R-6: no completion with unsolved gates; no skip ──────────────────

        [Test]
        public void R6_CannotComplete_WhileAnyGateUnsolved()
        {
            var s = new TrekSession(Trail(10, 10), Untimed());
            s.ArriveAtGate();
            s.SolveActiveGate();

            Assert.IsFalse(s.CanComplete);
            Assert.Throws<InvalidOperationException>(() => s.CompleteTrail(20));
        }

        // ── R-7: Later defers; deferred gates re-queue at Cleanup Camp ───────

        [Test]
        public void R7_DeferredGates_RequeueAtCleanupCamp_InOrder()
        {
            var s = new TrekSession(Trail(10, 10, 10), Untimed());

            s.ArriveAtGate();          // g0
            s.DeferActiveGate();       // Later
            s.ArriveAtGate();          // g1
            s.SolveActiveGate();
            s.ArriveAtGate();          // g2
            s.DeferActiveGate();       // Later

            // Main pass done, two deferred → mandatory Cleanup Camp, not Complete.
            Assert.AreEqual(TrekPhase.CleanupCamp, s.Phase);
            Assert.AreEqual(2, s.DeferredCount);
            Assert.IsFalse(s.CanComplete);

            // Deferred gates come back in passage order.
            Assert.AreEqual("g0", s.ArriveAtGate().GateId);
            s.SolveActiveGate();
            Assert.AreEqual("g2", s.ArriveAtGate().GateId);
            s.SolveActiveGate();

            Assert.AreEqual(TrekPhase.Complete, s.Phase);
            Assert.IsTrue(s.CanComplete);
        }

        [Test]
        public void R7_TrailWithTwoDeferredGates_CannotComplete_UntilCampClearsThem()
        {
            var s = new TrekSession(Trail(10, 10), Untimed());
            s.ArriveAtGate(); s.DeferActiveGate();
            s.ArriveAtGate(); s.DeferActiveGate();

            Assert.AreEqual(TrekPhase.CleanupCamp, s.Phase);
            Assert.IsFalse(s.CanComplete);

            s.ArriveAtGate(); s.SolveActiveGate();
            Assert.IsFalse(s.CanComplete);              // one still deferred
            s.ArriveAtGate(); s.SolveActiveGate();
            Assert.IsTrue(s.CanComplete);
        }

        // ── R-8: hint ladder — costs marks, never completion ─────────────────

        [Test]
        public void R8_HintLadder_Deductions()
        {
            var s = new TrekSession(Trail(100, 100, 100, 100), Untimed());

            s.ArriveAtGate();                       // no hints → full points, gold
            Assert.AreEqual(100, s.SolveActiveGate());

            s.ArriveAtGate();
            s.UseHint();                            // hint 1 → −40%
            Assert.AreEqual(60, s.SolveActiveGate());

            s.ArriveAtGate();
            s.UseHint(); s.UseHint();               // hint 2 → −70%
            Assert.AreEqual(30, s.SolveActiveGate());

            s.ArriveAtGate();
            s.RequestGuidedSolve();                 // guided → 0 points, still solved
            Assert.AreEqual(0, s.SolveActiveGate());

            Assert.IsTrue(s.CanComplete);           // guided still counts as solved
            var r = s.CompleteTrail(0);
            Assert.AreEqual(1, r.GatesSolvedGold);
            Assert.AreEqual(2, r.GatesSolvedSilver);
            Assert.AreEqual(1, r.GatesSolvedBronze);
            Assert.AreEqual(190, r.Points);
        }

        // ── R-10: scoring — marks scaled to max score; deferral is free ──────

        [Test]
        public void R10_Marks_ScaleToAssignmentMaxScore()
        {
            var s = new TrekSession(Trail(50, 50), Untimed());
            s.ArriveAtGate(); s.SolveActiveGate();
            s.ArriveAtGate(); s.UseHint(); s.SolveActiveGate();   // 50 + 30 = 80 of 100

            var r = s.CompleteTrail(20);
            Assert.AreEqual(80, r.Points);
            Assert.AreEqual(100, r.TotalBasePoints);
            Assert.AreEqual(16, r.Marks);                          // 80% of 20
            Assert.AreEqual(2, r.Stars);                           // 80% → ★★
        }

        [Test]
        public void R10_DeferralAlone_CostsNothing()
        {
            var s = new TrekSession(Trail(100), Untimed());
            s.ArriveAtGate();
            s.DeferActiveGate();                     // Later — free
            Assert.AreEqual(TrekPhase.CleanupCamp, s.Phase);
            s.ArriveAtGate();
            Assert.AreEqual(100, s.SolveActiveGate());   // solved clean at camp → full points
        }

        [Test]
        public void Stars_DeriveFromFraction()
        {
            // 100% → ★★★
            var a = new TrekSession(Trail(10), Untimed());
            a.ArriveAtGate(); a.SolveActiveGate();
            Assert.AreEqual(3, a.CompleteTrail(10).Stars);

            // guided-only → 0% but cleared → ★
            var b = new TrekSession(Trail(10), Untimed());
            b.ArriveAtGate(); b.RequestGuidedSolve(); b.SolveActiveGate();
            Assert.AreEqual(1, b.CompleteTrail(10).Stars);
        }

        // ── R-11: timer defers, never skips; reading view pauses it ──────────

        [Test]
        public void R11_TimerExpiry_AutoDefers_WithSmallPenalty_TrekContinues()
        {
            var s = new TrekSession(Trail(100, 100), new TrekConfig { SecondsPerGate = 10 });

            s.ArriveAtGate();                       // g0 opens, 10s
            bool expired = s.TickTimer(11f);
            Assert.IsTrue(expired);
            Assert.IsNull(s.ActiveGate);            // auto-deferred, not solved, not skipped
            Assert.AreEqual(1, s.DeferredCount);
            Assert.IsFalse(s.CanComplete);

            s.ArriveAtGate(); s.SolveActiveGate();  // g1 clean

            Assert.AreEqual(TrekPhase.CleanupCamp, s.Phase);
            s.ArriveAtGate();                       // g0 returns at camp
            int earned = s.SolveActiveGate();
            Assert.AreEqual(90, earned);            // −10% time penalty (R-11), not zero

            Assert.IsTrue(s.CanComplete);
        }

        [Test]
        public void R11_PausedTimer_DoesNotTick()
        {
            var s = new TrekSession(Trail(10), new TrekConfig { SecondsPerGate = 5 });
            s.ArriveAtGate();
            s.PauseTimer();                          // Reading View open (R-2)
            Assert.IsFalse(s.TickTimer(60f));
            Assert.IsNotNull(s.ActiveGate);          // still open
            s.ResumeTimer();
            Assert.IsTrue(s.TickTimer(6f));
        }

        // ── Checkpoint: exit stays in-progress; restore resumes exactly ──────

        [Test]
        public void Checkpoint_RoundTrips_GateStates_DeferredQueue_HintsAndTime()
        {
            var trail = Trail(10, 20, 30);
            var s = new TrekSession(trail, Untimed());
            s.ArriveAtGate(); s.UseHint(); s.SolveActiveGate();    // g0 silver (6 pts)
            s.ArriveAtGate(); s.DeferActiveGate();                 // g1 deferred
            s.TickTimer(12.5f);                                    // elapsed time accrues

            var cp = s.ToCheckpoint();
            var solved = s.SolvedBoardIndices();
            int score = s.Points;

            // Fresh trail (as if relaunched), restore.
            var trail2 = Trail(10, 20, 30);
            var r = TrekSession.Restore(trail2, Untimed(), solved, score, cp);

            Assert.AreEqual(score, r.Points);
            Assert.AreEqual(12.5f, r.SecondsElapsed, 0.01f);
            Assert.AreEqual(1, r.SolvedGates);
            Assert.AreEqual(1, r.DeferredCount);
            Assert.AreEqual(1, trail2.Gates[0].HintsUsed);

            // Continue: g2 is next on the main pass, then camp re-presents g1.
            Assert.AreEqual("g2", r.ArriveAtGate().GateId);
            r.SolveActiveGate();
            Assert.AreEqual(TrekPhase.CleanupCamp, r.Phase);
            Assert.AreEqual("g1", r.ArriveAtGate().GateId);
            r.SolveActiveGate();
            Assert.IsTrue(r.CanComplete);
        }

        [Test]
        public void HintReport_ListsPerWordUsage()
        {
            var s = new TrekSession(Trail(10, 10), Untimed());
            s.ArriveAtGate(); s.UseHint(); s.SolveActiveGate();
            s.ArriveAtGate(); s.RequestGuidedSolve(); s.SolveActiveGate();

            var r = s.CompleteTrail(0);
            CollectionAssert.AreEqual(new[] { "word0:1", "word1:3" }, r.HintReport);
        }

        // ── Text-only fallback trail (zero gates) is playable ────────────────

        [Test]
        public void TextOnlyTrail_ZeroGates_CanCompleteImmediately()
        {
            var trail = new TrailData { AssignmentId = "a", PassageText = "read me." };
            trail.Sentences.Add(new SentenceSpan(0, 8));
            var s = new TrekSession(trail, Untimed());

            Assert.IsTrue(s.CanComplete);            // nothing to solve — reading-only
            var r = s.CompleteTrail(10);
            // No gates = nothing was missable: a full read earns full stars.
            Assert.AreEqual(3, r.Stars);
        }
    }

    /// <summary>Checking semantics must match the existing game exactly.</summary>
    public class GateSolvingTests
    {
        static ContentToken Fill(string word) => new ContentToken { text = word, isWord = true, activity = ActivityType.FillBlank };
        static ContentToken Define(string word, string def) => new ContentToken { text = word, isWord = true, activity = ActivityType.Define, definition = def };

        [Test]
        public void Fill_CaseInsensitive_OrderedMatch()
        {
            var correct = GateSolving.FillCorrect(Fill("Cat"));
            Assert.IsTrue(GateSolving.CheckFill(new List<string> { "c", "A", "t" }, correct));
            Assert.IsFalse(GateSolving.CheckFill(new List<string> { "a", "c", "t" }, correct));
        }

        [Test]
        public void Define_OrdinalOrderedMatch()
        {
            var correct = GateSolving.DefineCorrect(Define("cat", "a small animal"));
            Assert.IsTrue(GateSolving.CheckDefine(new List<string> { "a", "small", "animal" }, correct));
            Assert.IsFalse(GateSolving.CheckDefine(new List<string> { "small", "a", "animal" }, correct));
        }

        [Test]
        public void Illustrate_MatchesCorrectImageOnly()
        {
            var t = new ContentToken { text = "cat", isWord = true, activity = ActivityType.Illustrate, correctImage = "cat.png", imageOptions = new List<string> { "cat.png", "dog.png", "fox.png" } };
            Assert.IsTrue(GateSolving.CheckIllustrate("cat.png", t));
            Assert.IsFalse(GateSolving.CheckIllustrate("dog.png", t));
        }

        [Test]
        public void Hint2_RevealsHalfLetters_LocksTwoPieces_EliminatesTwoWrongImages()
        {
            Assert.AreEqual(new List<int> { 0, 1, 2 }, GateSolving.FillRevealIndices(GateSolving.FillCorrect(Fill("title"))));   // 5 → 3

            var t = new ContentToken { correctImage = "ok.png", imageOptions = new List<string> { "a.png", "ok.png", "b.png", "c.png" } };
            var gone = GateSolving.IllustrateEliminations(t.imageOptions, t);
            Assert.AreEqual(2, gone.Count);
            CollectionAssert.DoesNotContain(gone, "ok.png");
        }

        [Test]
        public void SamePiecesWrongOrder_GivesTheNudge()
        {
            Assert.IsTrue(GateSolving.SamePiecesWrongOrder(new List<string> { "b", "a" }, new List<string> { "a", "b" }, false));
            Assert.IsFalse(GateSolving.SamePiecesWrongOrder(new List<string> { "a" }, new List<string> { "a", "b" }, false));
        }
    }
}
