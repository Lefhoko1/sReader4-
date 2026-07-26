# sReader Story Trek — Implementation Spec for Claude Code

**Purpose:** Turn the existing sReader student experience into a 2D reading game ("Story Trek") without breaking any UI functional requirement.
**Companion document:** `sreader-2d-reading-game-design-v3-complete.html` (visual mockups — for humans; do not load into context).
**How to use this file:** Reference it from `CLAUDE.md`. Work strictly one phase at a time (Phase 0 → 5). Do not start a phase until the previous phase's acceptance checks pass in-editor.

---

## 0. Non-negotiable constraints (read before every phase)

1. **Architecture stays MVVM.** Views (UXML controllers AND new 2D scene presenters) render state and forward input only. All logic stays in existing ViewModels/services. Never move validation, scoring, persistence or networking into a MonoBehaviour view.
2. **NavigationManager remains the owner of screens (GEN-1/2).** New 2D scenes load **additively** behind a `ScenePage` wrapper GameObject that NavigationManager activates/deactivates like any page. Unassigned targets = no-op + error log.
3. **Re-entrancy (GEN-3):** every controller/presenter re-registers callbacks in `OnEnable`. Never cache visual elements across deactivation.
4. **Placeholder mode (GEN-4) must keep working:** with no `AppCompositionRoot`, the Trek must run on a bundled sample passage.
5. **Busy guard (GEN-5), inline errors (GEN-6):** unchanged patterns in all new UI.
6. **Do not touch:** Landing, About, Contact, Register, Login, Forgot/OTP/Reset, Profile, Tutor Home, Guardian Home — except shared USS theme tokens.
7. **Reuse, don't rewrite:** question/challenge checking, attempt state, submissions, scheduling, download, multiplayer session services and their ViewModels are the same ones used today. New scenes bind to them.
8. **Marks are the source of truth.** Stars/XP/Journal tiers are derived, recomputable values. Never store a derived value that can't be rebuilt from marks + attempts + submissions.

---

## 1. The Reading & Completion Contract (core of the product — implement exactly)

The game exists so the student **reads the passage and understands its key words**. These rules enforce that:

### 1.1 Passage visibility
- **R-1** The full tutor-authored passage is always accessible. The in-game **story ribbon** shows it continuously, synced to progress:
  - already-read text: dimmed (readable, not hidden)
  - current sentence: highlighted ("lantern" style)
  - upcoming text: visible, muted
- **R-2** Tapping the ribbon (or a `Read` HUD button) opens **Reading View**: the entire passage as a scrollable page overlay. Available before starting, mid-trek (pauses timer), and after completion. Key words not yet won render as sealed slots `[⬚⬚⬚]`; won words render highlighted with a tap-to-hear affordance.
- **R-3** A **"Read first"** option on the island detail sheet opens Reading View before the trek starts (untimed). Starting the trek from there begins the attempt.

### 1.2 Key-word gates
- **R-4** Every key word the tutor hid in the passage becomes exactly one gate on the trail, in passage order. Gate challenge types (existing logic, reskinned): **Define** (arrange the definition), **Illustrate** (pick the matching image), **Fill** (spell the word).
- **R-5** The student reads up to the gate before solving it: the ribbon must have scrolled the gate's sentence into the highlighted position before the gate panel opens (context before puzzle).

### 1.3 Completion integrity — no skipping to "done"
- **R-6** **A trail cannot complete while any gate is unsolved.** There is no skip.
- **R-7** The old `Pass` becomes **`Later`**: it defers the gate. Deferred gates re-queue at **Cleanup Camp**, a mandatory final stop before the finish line where all deferred gates are presented again (in order). Leaving Cleanup Camp forward requires zero unsolved gates.
- **R-8** **Hint ladder** guarantees completion is always achievable, at a marks cost, never a completion cost:
  1. Hint 1: existing hint (−40% of that word's points)
  2. Hint 2: reveal half the letters / eliminate two images / lock two pieces (−70%)
  3. Guided solve: step-through with the answer shown (0 points for the word, still counts as solved; Journal card mints as silhouette-upgradeable "bronze")
- **R-9** **Submission fires only on full clearance.** `TrailCompleted` → record result via the existing submission path. If the student exits earlier, the attempt remains *in progress* at the checkpoint (existing per-device resume). Never record a submission for an uncleared trail.
- **R-10** Scoring: `marks = Σ word points after hint deductions`, scaled to the assignment's max score. Deferrals don't cost marks by themselves; hints and guided solves do. Report hint/guided usage per word in the submission payload if the schema allows (tutors should see *how* it was completed).
- **R-11** Timer semantics: the timer pressures pace but never forces a skip. On timer expiry at a gate: gate auto-defers to Cleanup Camp (small time penalty to points), trek continues. Reading View pauses the timer.

### 1.4 (Reserved) Sentence comprehension checks
- **R-12** Design-for, don't build yet: `GateType.Comprehension` — a tutor-authored one-question check bound to a sentence span, same completion rules. Keep the enum and data model open for it.

---

## 2. Data model (Phase 0 deliverable)

```csharp
// Built by TrailBuilder from the EXISTING assignment content format.
// Do not invent a new authoring format; map from what tutors already produce.
public sealed class TrailData {
    public string AssignmentId;
    public string Title;
    public string PassageText;              // full passage, verbatim
    public List<SentenceSpan> Sentences;    // start/end indices into PassageText
    public List<GateData> Gates;            // passage order
    public string BiomeId;                  // from course/subject, default "meadows"
}

public sealed class SentenceSpan { public int Start; public int End; }

public enum GateType { Define, Illustrate, Fill, Comprehension /*reserved*/ }
public enum GateState { Locked, Active, Deferred, SolvedGold, SolvedSilver, SolvedBronze }

public sealed class GateData {
    public string GateId;
    public GateType Type;
    public string Word;                 // the key word
    public int PassageCharIndex;        // where its blank sits in PassageText
    public int SentenceIndex;           // owning sentence (for R-5 and Journal)
    public object ChallengePayload;     // existing challenge model, reused as-is
    public int BasePoints;
    public GateState State;
    public int HintsUsed;               // 0..2, 3 = guided
}

// Resume point (existing per-device save, extended):
public sealed class TrekCheckpoint {
    public string AssignmentId;
    public int LastSolvedGateIndex;     // Kai stands after this gate
    public List<string> DeferredGateIds;
    public Dictionary<string,int> HintsUsedByGate;
    public float SecondsElapsed;
    public int PointsSoFar;
}
```

**TrailBuilder rules:** one gate per hidden word; gates sorted by `PassageCharIndex`; sentence spans computed by simple sentence segmentation (., !, ? + fallback for abbreviations later); malformed content → log + fall back to a text-only trail (all reading, zero gates) so no assignment is unplayable.

---

## 3. Scene & file layout

```
Assets/Scripts/Game/
  Trek/
    TrekScenePage.cs          // NavigationManager page wrapper; additive load/unload
    TrekPresenter.cs          // binds TrailData + existing game ViewModel to the scene
    KaiMotor.cs               // tweened deterministic movement (no physics)
    ParallaxLayer.cs
    RibbonBinder.cs           // UI Toolkit ribbon <-> progress sync (R-1)
    ReadingViewController.cs  // full-passage overlay (R-2/R-3), pauses timer
    GatePanelHost.cs          // shows existing challenge panels; hint ladder (R-8)
    CleanupCampController.cs  // deferred-gate queue (R-7)
    TrailBuilder.cs
  BoardRace/                  // Phase 3
  Meta/                       // Phase 4: WordJournal, Ranks, Streak, Planner glue
Assets/UI/Game/               // UXML/USS overlays (HUD, ribbon, gates, reading view)
Assets/Art/Biomes/{meadows,storybook}/
```

Rendering: URP 2D. Kai = SpriteRenderer + Animator (idle/walk/celebrate). Movement by tween (DOTween or coroutine lerp). Overlays via one `UIDocument` per scene. Target 60 fps at 1280×720 on the lowest-spec device; no per-frame allocations in `RibbonBinder`.

---

## 4. Phases (work strictly in order)

### Phase 0 — Foundations
Tasks: `ScenePage` additive loader under NavigationManager; `TrailBuilder` + `TrailData` from current content format + unit tests incl. malformed input; USS theme tokens; sample passage asset for placeholder mode.
**Accept:** empty Trek scene opens from assignment detail and returns; back-stack (FR-SHELL-6) and placeholder mode (GEN-4) intact; `TrailBuilder` tests green.

### Phase 1 — The Trek (solo reading game)
Tasks: parallax + path + Kai; RibbonBinder states (R-1); Reading View (R-2/R-3); gate flow with R-5 ordering; existing three challenges hosted in GatePanelHost; hint ladder (R-8); `Later` + Cleanup Camp (R-6/R-7); timer semantics (R-11); checkpoint save/restore mapped to existing resume point; Trail Complete screen — marks, "Submitted to your tutor ✓", stars derived (≥90% ★★★ / ≥70% ★★ / cleared ★); submission only on clearance (R-9/R-10).
**Accept:** FR-SHOME-5.2 & 5.3 pass in the new scene; a trail with 2 deferred gates cannot be completed until Cleanup Camp clears them; exit-and-resume restores gate states, deferred queue and elapsed time; offline downloaded assignment plays with zero network calls and queues the submission.

### Phase 2 — Quest Map & Base Camp
Tasks: island map from assignment data (due-date order); legend-chip filters + class filter + title search (5.1); island detail sheet (Start / Read first / Resume / Reset trail / Submit / 🗓 / ⬇); storm-cloud urgency with printed dates; Base Camp visuals per mockup (hero priority per FR-SHOME-1); empty state → Academies Discover deep link (5.8); dashboard deep link (5.7).
**Accept:** every action reachable in today's list is reachable on the map; Reset clears checkpoint incl. deferred queue; back pops map→detail→camp correctly.

### Phase 3 — Campfire & Board Race
Tasks: Campfire screen (host; join by code; open-sessions list; backend toggle "Quick flame/Photon" vs "Ember/Supabase", remembered per device); lobby (seats, presence flare/grey, host-only Start); Board Race scene (tile track from data, avatar tokens from Profile, turn marker, advance action, `?` tiles open the same GatePanelHost); event ticker from existing stream; leave/rejoin. Multiplayer uses the same completion rules per gate; match scoring per existing session logic.
**Accept:** FR-SHOME-5.4 passes on both backends, 2–4 players, two devices, including a mid-game disconnect + rejoin.

### Phase 4 — Meta & planning
Tasks: Word Journal (cards mint on solve: gold no-hint / silver hinted / bronze guided / grey silhouette = still deferred at last exit; search; word-of-the-day on Base Camp); Reader Ranks from XP (Page→Scribe→Wordsmith→Storykeeper→Legend); streak (pause, never reset-shame); Expedition Planner in full (pitch/move/strike tent, note, private-camp hide toggle, Plan-with-others list — FR-SHOME-5.5).
**Accept:** wipe local derived data → recompute Journal/ranks/streak from synced marks/attempts and get identical results.

### Phase 5 — Fancy pass (polish + accessibility, one pass)
Tasks: juice checklist from the design doc (gate-open clunk + word-flies-to-Journal, calm error shake, checkpoint flag ripple, star slams with 4px shake, wax-seal submit stamp, token hop squash-and-stretch, idle map motion, universal button spring); biome skins (meadows + storybook); layered theme + ~24 SFX behind mute; **accessibility:** dyslexia-friendly font toggle, text size S/M/L, optional read-aloud with word-sync highlight, reduced-motion mode, no colour-only meaning.
**Accept:** matches the presentation-target mockup on-device at 60 fps; reduced-motion run of every screen fully playable.

---

## 5. Guardrails for the agent

- Never weaken the Reading & Completion Contract to make a task easier. If a task conflicts with R-1…R-11, stop and flag it.
- Never bypass ViewModels to call Supabase/Photon from a presenter.
- Never change submission or marks schemas without flagging; extend payloads only additively.
- Prefer restyling existing UXML challenge panels over rebuilding them.
- Keep every commit inside one phase; reference the FR/R-rule IDs it satisfies in the commit message.
