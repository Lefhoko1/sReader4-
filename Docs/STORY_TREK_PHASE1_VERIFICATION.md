# Story Trek — Phase 1 verification (in-editor)

Phase 1 delivers **The Trek** — the solo reading game. This file lists the
in-editor acceptance checks (spec §4 Phase 1) plus what changed. No scene
re-wiring is needed: the Phase 0 `TrekPage` now runs the full game.

## What was built

**Pure logic (`SReader.Game.Trek.Logic`, unit-tested):**
- `TrekSession` — the rules engine for R-4…R-11: gate order, `Later` + Cleanup
  Camp (R-6/R-7), hint ladder −40%/−70%/guided-0 (R-8), timer auto-defer with
  small penalty + pause (R-11), marks scaled to max score + stars (R-10),
  checkpoint export/restore.
- `GateSolving` — the existing challenges' checking semantics (ordered
  join-compare; case-insensitive letters; correct-image match) + hint-2 assists
  (reveal half / lock two / eliminate two).

**Presentation (`Assembly-CSharp`, `Assets/Scripts/Game/Trek/`):**
- `TrekPresenter` — orchestrator; binds TrailData + TrekSession + the existing
  `StudentAssignmentsViewModel` (attempt, per-solve progress save, submission).
- `TrekWorldBuilder` + `ParallaxLayer` + `KaiMotor` — procedural 2D world
  (sky/hills/treeline parallax, path, wooden gates with violet runes, finish
  arch, checkpoint flag); Kai uses the existing MascotBoy frames (capsule
  fallback if art missing); tweened deterministic movement, camera follow.
- `TrekHud` + `RibbonBinder` — glass chips (Read, WORD GATE n/m, timer, ★ points)
  and the story ribbon (dimmed-read / lantern-current / muted-upcoming, sealed
  `[⬚⬚⬚]` slots; rebuilds only on state change — no per-frame allocations).
- `ReadingViewController` — full-passage overlay (R-2/R-3), pauses the timer.
- `GatePanelHost` — Word Forge / Scroll of Order / Picture Gate (same checking,
  restyled), hint ladder buttons, **Guide me** (0 ★, appears after hint 2 or two
  wrong tries), **⏭ Later**.
- `CleanupCampController`, `TrailCompleteView`, `TrekCheckpointStore`
  (additive PlayerPrefs side-car for deferred/hints/elapsed — no schema change).
- `TrekScenePage` — now hosts the presenter; richer `Launch(view, vm, onExit)`.
- `StudentAssignmentsViewModel.RecordGameResultAsync` gained an optional `note`
  param (additive) so hint usage rides the submission reference (R-10).

## 1. Compile + tests

1. Console clean after reimport.
2. **Test Runner ▸ EditMode ▸ Run All** — `TrailBuilderTests`, `TrekSessionTests`,
   `GateSolvingTests` all green. The TrekSession suite IS the Reading &
   Completion Contract (R-6, R-7, R-8, R-10, R-11, checkpoint round-trip).

## 2. Core play loop (FR-SHOME-5.2 / 5.3 in the new scene)

1. Play → student → Assignments → open an assignment **that has content**
   (or any assignment; malformed/empty content = reading-only trail by design)
   → **▶ Story Trek**.
2. Expect: storybook world (sky/hills/path), Kai walks to the first gate, the
   ribbon at the bottom reads along and lands on the gate's sentence, then the
   gate panel slides up (R-5: context before puzzle).
3. Solve a gate → "+N ★" toast, rune turns gold/green, word unseals in the
   ribbon, checkpoint flag moves, Kai hops and walks on.
4. **Hints:** Hint (−40%) → clue; Hint (−70%) → half the letters pre-placed /
   two pieces locked / two images knocked out; **Guide me** → tap the glowing
   pieces, 0 points, still counts as solved.
5. **⏭ Later** on 2 gates → after the last main gate, the **Cleanup Camp** sheet
   appears; the trail cannot finish until both deferred gates are solved there
   (R-6/R-7). Verify there is no way to reach Trail Complete while one is open.
6. **Timer:** let a gate's countdown expire → toast "moved to Cleanup Camp",
   trek continues (never skips). Open **📖 Read** mid-gate → timer chip shows ⏸
   (paused, R-11); the passage shows sealed + won words (R-2).
7. **Trail Complete:** marks ("16/20" style), stars, "Submitted to your tutor ✓".
   Check the tutor side / DB got a submission with `Game score: … · hints …`.
   Submission must appear ONLY after full clearance (R-9).

## 3. Leave-and-resume (Phase 1 acceptance)

1. Mid-trek — after solving ≥1 gate and deferring ≥1 — press Back/Esc.
2. Relaunch the same assignment's trek. Expect: solved runes already painted,
   the flag at the last solved gate, Kai standing past it, points restored,
   the deferred gate still queued for Cleanup Camp, elapsed time kept.
3. Complete the trek → progress + side-car cleared; "Play again" (existing
   reset) starts fresh.

## 4. Offline downloaded assignment (Phase 1 acceptance)

1. Download an assignment (existing ⬇), go offline, play the trek to the end.
2. Trail Complete should show the coral note "will be submitted next time…" —
   the run is saved (finished) and **re-submits automatically** on next launch
   of that trek while online. Verify the submission then lands exactly once.

## 5. Placeholder mode (GEN-4)

With no AppCompositionRoot (or launching TrekPage with nothing pending): the
trek runs the bundled sample passage WITH practice gates (Define + Fill seeded
in code) and Trail Complete says "Practice trek — nothing was submitted."

## Known Phase 1 boundaries (by design, per spec)

- "Read first" from the island detail sheet (R-3's untimed pre-read) arrives
  with the Phase 2 Quest Map; in-trek 📖 Read is available from the first step.
- Exit returns to the Student Home root (deep back-stack into the assignment
  profile is part of the Phase 2 map/detail work).
- Tap-to-hear on won words is an affordance only — audio lands with Phase 5
  read-aloud. Biome art is the wireframe target; Phase 5 is the fancy pass.
- Emote row and multiplayer board are Phase 3 (Campfire & Board Race).
