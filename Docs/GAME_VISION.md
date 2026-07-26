# GAME_VISION.md — Adventure Re-skin Brief for The Great Library of Knowledge

> **Read this fully before proposing any changes.** This document briefs you
> (Claude Code) on a design/art layer developed in a separate design session,
> and how to integrate it with THIS existing codebase. The guiding rule is at
> the top for a reason.

---

## 0. THE PRIME DIRECTIVE — DO NOT REBUILD

This project is a **near-complete, working educational platform**. It already
does, correctly, in production:

- Academies, courses, subjects, enrollment, payments
- Tutor accounts that own academies and author assignments
- Student accounts: see assignments, attempt, submit, redo
- Reading activities: view assignment paragraphs, hidden/faded keywords,
  **define words by rearranging definitions, fill blanks by rearranging
  words, illustrate/choose-image, hints**
- Turn-based multiplayer reading games via **Photon**
- Data persistence in **Supabase**
- UI built in **Unity UI Toolkit (UXML/USS)**

**None of this is to be rewritten, replaced, or "modernized."** It works. Your
job is NOT to rebuild the LMS. Your job is to make it *feel like an adventure
game* instead of a mobile app, by adding a presentation/feel layer around the
existing logic. When in doubt, WRAP, don't REPLACE. If a task seems to require
changing core reading/assignment/Photon/Supabase logic, STOP and ask the user
first.

---

## 1. WHAT THE USER IS TRYING TO ACHIEVE

The platform currently "feels like a mobile app, not a game." The user's true
intent — from the very beginning — was an **adventure**: a student travels to a
world, and the act of reading and understanding sentences IS the gameplay. The
core educational activities do not change; their *framing and feel* do.

Design pillars (from the companion Production Bible):
1. **Reading IS the game** — activities are not wrapped homework; they are the
   gameplay. (Already true here mechanically — we just need it to FEEL true.)
2. **Knowledge is the player's only magic** — understanding restores a world.
3. **The world pushes back** — an antagonist ("The Forgetting") fades words;
   restoring them pushes fog back. (Maps directly onto existing hidden/faded
   words.)
4. **The Library is the scoreboard** — progress is visible in a 3D world
   (shelves filling, lights returning), not just in menus/percentages.
5. **Small and deep beats wide and thin** — polish one flow fully first.

---

## 2. WHAT WAS BUILT IN THE DESIGN SESSION (available as assets/scripts)

These exist as importable Unity assets and C# scripts the user will place in
the project. Treat them as the "game body" to wrap the existing "LMS brain":

### 2a. 3D Art (Blender-generated FBX, mobile-budgeted, ~correct scale)
- **Modular library kit** (~26 pieces): walls, floors, shelves, arch, columns,
  stairs, beams, balustrade, books, desk, chair, candles, lantern.
- **Clutter**: inkpot, quill, key, scrolls, book stack, globe.
- **Hero statics**: `SM_Book_Hero_Open` (reading surface with clean-UV page
  child), `SM_Librarian_Desk`, `SM_Fountain_Dry/Flowing`, `SM_Door_Rune_Sealed`
  (6 separate rune plates), `SM_Lexicon_Pedestal`.
- **Rigged characters**: `SK_Owl` (6 anim clips: Idle_Sleep, Idle_Perch,
  React_Happy, React_Lean, Hint_Point, Fly_Short), `SK_BookSpirit` (3 clips).
  NOTE: `SK_Librarian` (humanoid) NOT built — needs hand-modeling later.
- **Island + Library exterior**: terrain, sea, the Great Library building
  (glowing windows, dome, towers), trees, dock, boat, floating crystals,
  path, lantern posts. Composed as `SM_Island_Vista`.

  These use flat base-colour materials + one placeholder "trim atlas". They are
  **blockout-plus quality**: correct geometry, needs a painted trim sheet /
  texture pass to look great (see §5, the biggest visual lever).

### 2b. Feel system — `RestorationDirector.cs` (uGUI/world, singleton)
The "juice" layer. Public API to call from ANY successful/failed activity:
- `PlayCorrect(worldPos, [normal], size, wordLength)` — gold ink spread +
  pentatonic chime (pitch by word length) + rising motes + room light swell.
- `PlayIncorrect(worldPos)` — soft low "not yet" tone (NEVER red/buzzer).
- `PlayCompletion([center], [normal])` — page-exhale: bloom pulse, resolving
  arpeggio, light swell, mote burst.
- `DriftBack(piece, homePos, homeRot)` — coroutine: dragged tile wavers and
  eases home on a wrong drop.
- Distance-aware scaling; mobile-safe; audio is synthesized (no clips needed).
- Shaders: `GreatLibrary/InkSpread`, `GreatLibrary/MoteAdditive` (URP).

### 2c. Reference implementations (DEMO ONLY — do not ship over existing UI)
- `ReadingAdventure.cs` — a self-contained uGUI demo of the reading loop
  (passage, the 3 activities, comprehension). It exists to show the *intended
  feel*. The real reading logic already lives in this codebase in UI Toolkit —
  DO NOT replace the working version with this demo. Mine it for feel, framing,
  copy tone ("Restore the word", "Ink Weaving", Owl hints), and effect timing.
- `IslandFlow.cs` — camera arrival (sea → library door) → "Enter the Library"
  → golden fade → reading begins. This is the adventure framing to adopt.
- Editor automation: import pipeline, material setup, prefab building, island
  scene setup, vista polish (fog/lighting/bloom).

---

## 3. THE INTEGRATION PLAN (what to actually do, in order)

### The core architectural reconciliation
- **KEEP:** all reading/assignment/activity logic, Photon multiplayer,
  Supabase, tutor authoring, payments, enrollment — untouched.
- **KEEP:** UI Toolkit for dense/text-heavy screens (assignment lists, tutor
  dashboards, course browsing). It is the right tool there.
- **ADD:** a 3D world layer (island + library interior) as the *navigation
  shell and stage*. The student moves through a world; screens open from it.
- **ADD:** `RestorationDirector` calls at the existing activity success/failure
  points so every correct answer produces the game "juice".
- **ADD:** adventure framing/copy over existing screens (fiction, not new
  logic).

### KNOWN TECHNICAL FRICTION — resolve deliberately, don't paper over
1. **UI Toolkit vs uGUI/world-space.** Existing UI is UI Toolkit; the feel
   effects and 3D world are uGUI + URP 3D. These render on different paths.
   Recommended approach:
   - Keep activity UIs in UI Toolkit.
   - Render the 3D world (island, interior, owl) *behind* the UI Toolkit
     panels via the camera + a `PanelSettings` sort order, OR show the world on
     transition screens (arrival, map, between-assignment moments) and the UI
     Toolkit reading panel on top.
   - Fire `RestorationDirector` effects in the 3D/world layer, triggered by C#
     events raised from the UI Toolkit activity callbacks. The effect plays in
     the world layer at a screen-projected position; the UI Toolkit panel
     stays as-is. **Confirm this bridging approach with the user before large
     changes.**
2. **Render pipeline.** Effects/shaders assume **URP**. Verify the project is
   URP; if Built-in, the shaders need porting (flag this to the user).
3. **Input system.** Feel/demo scripts support both input backends via
   `#if ENABLE_INPUT_SYSTEM`. Match the project's setting.
4. **Materials for island/characters** must NOT go through the kit trim
   pipeline (they'd get the wooden atlas). Keep them in separate folders.

### Suggested phase order (small and deep)
1. **Audit & report** (do this first, change nothing): read the existing
   reading-activity code and the assets in §2. Produce a written map of:
   where each activity's success/failure is detected; where assignment data is
   loaded from Supabase; how screens are shown/hidden; whether URP is active.
   Report back before coding.
2. **Juice on existing activities**: raise events at the existing correct/
   wrong/complete points; have the world layer play `RestorationDirector`
   effects. Nothing else changes. This alone transforms feel.
3. **Adventure shell**: add the island vista as the entry/home; `IslandFlow`
   arrival; "Enter the Library" opens the existing assignment flow.
4. **Library interior stage**: build a ReadingRoom scene from the kit; the
   existing reading panel appears in it; the hero Book/Owl are present.
5. **Progress-as-world**: reflect existing completion data on 3D shelves
   (books appear/illuminate as assignments are completed) — read-only from
   Supabase, no schema changes.
6. Later: decay/spaced-repetition review, Lexicon spending, `SK_Librarian`,
   painted textures.

---

## 4. TUTOR CONTENT CONTRACT (already satisfied here)

The design session assumed a JSON assignment shape (title, tutor, passage,
keywords[] with word/definition/definitionShards/blank/imageOptions,
comprehension question). **This codebase already has real assignment data via
Supabase** — do NOT introduce a parallel JSON system. Instead, map the existing
Supabase assignment model to the concepts above where needed for the world
layer (e.g., which words are "restored" = which activities are completed).

---

## 5. THE SINGLE BIGGEST VISUAL LEVER (for the user, not code)

The 3D assets are correct but wear placeholder flat colours. The largest jump
toward a "Candy Crush-grade" look is **art, not code**: a painted 2048 trim
sheet for the library kit, painted character skins, and a colour/lighting/UI
polish pass. This needs a 2D/texture artist. No script produces this. Set
expectations accordingly: code makes it *work and feel*; art makes it *pretty*.

---

## 6. WHAT TO ASK THE USER BEFORE STARTING

1. Confirm URP vs Built-in render pipeline.
2. Confirm input system (new / old / both).
3. Approve the "UI Toolkit stays, world layer wraps it" bridging approach in
   §3, OR discuss an alternative (e.g., migrating one activity to uGUI as a
   showcase).
4. Which single flow to make adventurous FIRST (recommend: one assignment,
   start-to-finish, with arrival + juice + shelf growth).

Produce the §3.1 audit report FIRST. Do not modify working systems until the
user has approved a plan based on that audit.
