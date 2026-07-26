# Story Trek — Phase 2 verification (in-editor)

Phase 2 delivers the **Quest Map & Base Camp** — the world around the game. No
scene re-wiring; it reskins the existing student Assignments + Home dashboard.

## What was built

- **`QuestMapView`** — the assignments list rendered as islands along a dotted,
  due-date-ordered trail. Per-island state (all synchronous from `AssignmentView`,
  plus an async backpack check): 🧭 in progress, ⛈ overdue (storm cloud + printed
  date), 🌥 due soon, ⛺ scheduled, 🎒 downloaded, ⛳ done. Tapping opens the detail.
- **`StudentAssignmentsView`** — the "My assignments" and Search tabs now render
  the Quest Map (filters, class filter, title search and empty-state find-a-class
  all unchanged, FR-SHOME-5.1 / 5.8).
- **Island detail sheet** (`ShowProfile`) — now includes **📖 Read first** (R-3)
  and **♻ Reset trail**, alongside the existing Start/Resume/Story Trek/Submit/
  Schedule/Download/Play again. Due line uses the storm-cloud treatment.
  - **Read first** opens the full passage (Reading View overlay), untimed, with
    sealed `[⬚⬚⬚]` words and a **▶ Start the trek** button that begins the attempt.
  - **Reset trail** clears the attempt AND the trek checkpoint — both the solved
    gates (existing `GameProgress`) and the deferred-queue/hints side-car
    (`TrekCheckpointStore`), so the trek truly starts fresh. "Play again" now
    clears the side-car too (was a latent staleness bug).
- **`ReadingViewController`** — gained an optional Start button for Read first.
- **Base Camp** — the home dashboard now carries a "⛺ BASE CAMP" trailhead
  kicker; the existing FR-SHOME-1 hero (resume → next-up → find-a-class), stat
  signposts, learn chips and quick links are unchanged.

## 1. Compile + tests
1. Console clean after reimport.
2. EditMode tests still green (Phase 0/1 suites unaffected).

## 2. Quest Map (FR-SHOME-5.1) — acceptance
1. Play → student → **Assignments ▸ My assignments**.
2. Expect a **trail of islands** in due-date order (not flat cards), each with a
   state badge and a storm cloud + printed date on overdue/due-soon items.
3. Filter pills (All / Due soon / Overdue / Submitted / Completed / Scheduled),
   the class dropdown (if in >1 class), and **Search** all still filter the trail.
4. With no assignments: the "Find a class to enrol" jump still appears (5.8).
5. Home dashboard "My classes"/dashboard deep-links (5.7) still open the right
   island detail.

## 3. Detail sheet — every action reachable (acceptance)
Open an island. Confirm all are present/working:
- **Start / Resume trek** (▶) → launches the Trek (Phase 1).
- **📖 Read first** → full passage overlay, sealed words, **▶ Start the trek**
  begins the attempt; **✕ Close** returns without starting.
- **Submit**, **Schedule / Edit schedule**, **Download**, **Play again** (when
  completed) — unchanged.
- **♻ Reset trail** (when started/completed & not submitted) → wipes progress.

## 4. Reset clears the checkpoint incl. deferred queue (acceptance)
1. Play a trek, **defer at least one gate** (⏭ Later) and solve one, then exit.
2. Back on the detail sheet, tap **♻ Reset trail**.
3. Relaunch the trek → it must start **completely fresh**: no solved runes, no
   planted flag, empty deferred queue, 0 points. (This is the Phase 2 acceptance
   that Reset clears the checkpoint including the deferred queue.)

## 5. Back-stack (FR-SHELL-6) — acceptance
From an open island: system/Android back pops **detail → map**; from the map,
back drops to the **home section**; a second back confirms-to-exit. Read first
and Reset re-render in place without corrupting the stack.

## Notes / deferred
- The island trail is a vertical connector layout (robust on phone); the mockup's
  free-floating archipelago art is a Phase 5 visual pass.
- Multiplayer entry (mode select) is untouched and still reachable; the Campfire
  redesign is Phase 3.
