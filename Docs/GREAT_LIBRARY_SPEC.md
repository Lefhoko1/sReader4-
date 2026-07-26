# The Great Library of Knowledge — Implementation Spec for Claude Code

**Purpose:** Bind the *Great Library Production Bible* to the existing sReader
codebase, so the whole team (currently you + Claude) builds the Bible's vision
without re-deriving the pedagogy, the data contract, or the MVVM spine that
already works.

**Source-of-truth split — do not blur it:**
- **`Docs/Great_Library_Production_Bible.docx`** is the source of truth for
  *what the game is*: art direction, world plan, Blender/Photoshop pipeline,
  restoration rituals, systems design, task board. It is a `.docx`; its text has
  been read once — re-open it (not the companion HTML mockup) when an art or
  production question needs the exact wording.
- **This file** is the source of truth for *how it maps to code*: the frozen
  spine, the ritual↔code map, the data contract, scene/naming law, and the
  reconciled phase plan (GL-0 → GL-2). When the two disagree on a code detail,
  this file wins; when they disagree on look/feel, the Bible wins. If a task
  can't satisfy both, **stop and flag it.**

**Supersedes Story Trek.** `Docs/STORY_TREK_SPEC.md` is retained for history but
is **no longer the active brief**. The Great Library *replaces the trek fiction*
(Kai, trails, islands) while **reusing the entire Story Trek logic spine** — see
§2 and the fiction remap in §3. Do not build new Story Trek phases.

**How to use this file:** Work strictly one phase at a time (GL-0 → GL-1 → GL-2).
Do not start a phase until the previous phase's acceptance checks pass in-editor.

---

## 0. Non-negotiable constraints (read before every phase)

1. **Full 3D, built blockout-first, effectively solo.** Every space is playable
   in grey ProBuilder before any Blender asset replaces a block (Bible Ch. 4,
   "Blockout before beauty"). Art production is a *parallel track that never sits
   on the gameplay critical path* (§7, Art Track). Gameplay is proven in grey.
2. **The order is the message (Bible Ch. 10).** Feel first (the shared
   restoration grammar), then the reading engine, then the world. If GL-0
   produces a page where placing one word feels magical, everything after is
   decoration on a proven core.
3. **Architecture stays MVVM.** Views — UXML controllers *and* new 3D scene
   presenters (MonoBehaviours) — render state and forward input only. All
   validation / scoring / persistence / networking stays in the existing
   ViewModels + services. **Never** move a rule into a MonoBehaviour; **never**
   call Supabase / Photon from a presenter.
4. **NavigationManager still owns screens.** New full-screen scenes load
   **additively** behind a page wrapper it activates/deactivates (the pattern
   `TrekScenePage` established). Unassigned targets = no-op + error log.
5. **Re-entrancy:** every controller/presenter re-registers callbacks in
   `OnEnable`; never cache visual elements across deactivation.
6. **Placeholder mode must keep working:** with no `AppCompositionRoot`, the
   Reading Room runs on the bundled sample book (§4).
7. **Marks are the source of truth.** Stars / XP / Lexicon tier / world
   restoration % are *derived, recomputable* values. Never store a derived value
   that can't be rebuilt from marks + attempts + submissions + the mastery
   stream.
8. **Reuse, don't rewrite.** Challenge checking, attempt state, submissions,
   scheduling, download, multiplayer session services and their ViewModels are
   the same ones used today. New scenes bind to them (§2).

---

## 1. The Restoration Contract (R-1…R-11 — inviolable)

The Reading & Completion Contract from Story Trek **is** the Bible's Reading
Engine (Ch. 8.2) plus the Shared Restoration Grammar (Ch. 2.1). It is already
implemented, pure and unit-tested, in `TrekSession`. **Keep the R-IDs.** The only
change is the fiction the presenter wraps around them.

- **R-1 Passage always visible.** The full tutor-authored passage is always
  accessible. In the Bible this is the open Book page: already-restored text is
  crisp gold-ink; the current line is lantern-lit; upcoming text is fog-touched
  and faded (Bible "Entering the Page", Ch. 2). *Restored space is warm; unread
  space is cool* (Bible Ch. 3.2 rule).
- **R-2 Reading View.** A full-passage overlay, available before / during
  (pauses timer) / after. Un-restored keywords render as sealed slots; restored
  keywords render kindled with a tap-to-hear affordance.
- **R-3 "Read first"** opens Reading View untimed before the attempt begins.
- **R-4 One gate per keyword,** in passage order. Gate types (existing logic,
  reskinned as rituals — §3): **Definition Reforging**, **Vision Restoration**,
  **Ink Weaving**.
- **R-5 Context before puzzle.** The Book must have brought the keyword's
  sentence into the lantern-lit position before its ritual opens (Word Kindling
  is the tap that opens it).
- **R-6 No skipping to done.** A book cannot complete while any ritual is
  unsolved. There is no skip.
- **R-7 "Later" defers** a ritual to a mandatory final restoration pass before
  the page seals (Story Trek's Cleanup Camp). Leaving it forward requires zero
  unsolved rituals.
- **R-8 Hint ladder** guarantees completion at a *marks* cost, never a
  *completion* cost: Hint 1 (−40%), Hint 2 (−70%), Guided solve (0 pts, still
  counts, mints a "bronze" Lexicon/Journal card). Bible grammar: *after two
  misses the Owl offers a context hint* (Ch. 2.1) — the Owl is the hint voice.
- **R-9 Submission fires only on full clearance** (`CompleteTrail` → existing
  submission path). Early exit stays *in progress* at the checkpoint. Never
  record a submission for an uncleared book.
- **R-10 Scoring:** `marks = Σ word points after deductions`, scaled to the
  assignment's max score. Deferrals alone cost nothing; hints/guided/timeouts do.
  Report per-word hint usage in the submission payload (additively).
- **R-11 Timer** pressures pace, never forces a skip: on expiry a ritual
  auto-defers to the final pass (small points penalty). Reading View pauses it.
- **R-12 (reserved)** `GateType.Comprehension` — the Bible's end-of-passage
  comprehension panel (Ch. 8.2 COMPREHEND). Modelled, not built yet.

**Restoration grammar (Bible Ch. 2.1) is one shared system (Bible Ch. 8.4),
built first (GL-0):** correct = gold-ink spread + rising chime + light motes +2%
brightness; incorrect = piece wavers back, low unresolved tone, *no red/buzzer/X*;
completion = the page exhales (1.5 s bloom), spine gem brightens one step. Every
ritual calls this via a single `FeedbackDirector`. Nobody hand-rolls feedback.

---

## 2. The frozen spine (reused untouched — no rewrite)

These carry over from Story Trek / the existing app **as-is**. The 3D pivot does
not touch them; they have no idea whether the view is a 2D ribbon or a 3D Book.

| Reused | Where | Role in the Bible |
|--------|-------|-------------------|
| `TrekSession`, `GateSolving` | `SReader.Game.Trek.Logic` | The Reading Engine rules (Ch. 8.2) + completion contract. Pure, tested. |
| `TrailData`, `TrailBuilder`, `TrekCheckpoint` | `SReader.Game.Trek.Logic` | The playable shape of one book + resume point. |
| `AssignmentContent` / `ContentToken` / codec | `SReader.Domains.Assignments.Content` | **The BookData contract** (Ch. 8.1) — see §4. |
| ViewModels + domain services | `Assembly-CSharp` (`Assets/Scripts/UI`, `.../Domains`) | Validation, scoring, submission, scheduling, download, multiplayer. |
| Supabase repositories, submission path, `GameProgress` | `.../Infrastructure`, `.../Domains/Assignments` | Persistence + backend. Mastery/telemetry is *added* to this, not replacing it. |
| `NavigationManager`, page-wrapper pattern | `Assembly-CSharp` | Additive scene hosting (Ch. 7.3 flow). |
| UI Toolkit chrome (parchment panels, HUD, Lexicon, ceremony) | `Assets/UI` | Stays UI Toolkit + TMP (Bible Ch. 3.5, 7.4 `P_UI_*`). Re-themed, not rebuilt. |

**Assembly rule (unchanged):** pure logic → logic asmdef; anything touching
scenes / MonoBehaviours / ViewModels → `Assembly-CSharp`. New 3D presenters are
MonoBehaviours → `Assembly-CSharp`; they reference the logic asmdef.

---

## 3. Ritual ↔ code map, and the fiction remap

The five rituals are the existing activities renamed — **the pedagogy is
unchanged** (Bible Ch. 2). `ActivityType` / `GateType` values do **not** change;
only the presenter's fiction and styling do.

| Bible ritual (Ch. 2) | Existing type | Presenter role |
|---|---|---|
| **Entering the Page** (read paragraph) | reading (R-1/R-2) | Book page projection + Reading View |
| **Word Kindling** (tap glowing keyword) | the gate-arrival tap (R-4/R-5) | keyword glows on the page; tap lifts it as a rune → Lexicon fly-in |
| **Ink Weaving** (scrambled letter tiles) | `ActivityType.FillBlank` → `GateType.Fill` | letter tiles = loose ink droplets → drag to order |
| **Definition Reforging** (reorder shards) | `ActivityType.Define` → `GateType.Define` | phrase-shards on a stone anvil → reorder → forge-fuse |
| **Vision Restoration** (pick the true image) | `ActivityType.Illustrate` → `GateType.Illustrate` | grey frame + ghost-image options → colour-snap |

**Fiction remap (string/theme layer — code names may be renamed later, but never
at the cost of a passing build):**

| Story Trek | Great Library |
|---|---|
| The Trek / trail / Kai walking | Entering the Page / the Reading Alcove |
| Gate | Restoration ritual |
| Arriving at a gate | Word Kindling |
| Cleanup Camp (deferred queue) | Final restoration before the page seals |
| Trail Complete ceremony | Page-exhale RESTORE ceremony |
| Quest Map / islands / Reading Adventure world map | Library Hub (Entrance Hall) + shelves |
| Biome | Wing (Children's / Science / History / Nature) |
| Word Journal | The Lexicon |

**Note the 3→5 framing:** the Bible's five rituals = your three interactive
challenge types **+** reading (Entering the Page) **+** the universal keyword tap
(Word Kindling). No new challenge *checking* is required; Word Kindling is the
shared entry affordance, not a new scored puzzle.

---

## 4. The BookData contract (freeze first — Bible Ch. 8.1)

`AssignmentContent` **is** BookData. GL-0 formalises it, it does not replace it:
- Document the field-by-field mapping from the existing tutor product to
  BookData (`bookId`←assignment id, `paragraphs[]`←`pages[].sentences[].tokens`,
  `keywords[]`←interactive `ContentToken`s with `definition` / `imageOptions` +
  `correctImage` / points, etc.).
- Add the Bible fields the current model lacks *additively* and only when a phase
  needs them: `coverRecipe` (base/ornament/sigil ids — Ch. 6.4, needed GL-2),
  `subject` (→ Wing/biome), reserved `questions[]` (R-12). Do **not** break the
  existing `content_json` shape or `JsonUtility` round-trip.
- Ship **2 hand-written sample books** as the frozen reference + placeholder-mode
  content. The game only *reads* content and *records* results — it never edits
  content.

`TrailBuilder` rules stay: one gate per interactive word; gates sorted by
passage position; malformed content → log + text-only fallback (all reading,
zero rituals) so no assignment is ever unplayable.

---

## 5. Scenes, folders & naming (Bible Ch. 7, Ch. 9 — "the law")

**Scene flow (Ch. 7.3):** `Boot → Login → LibraryHub (Entrance Hall) →
ReadingRoom (the Alcove, most-polished scene) → Wing_* (additive) →`
TutorPortal stays the existing web app; the game consumes its published JSON.
New scenes load additively via the NavigationManager page-wrapper pattern.

**Folders (Ch. 7.2) — create as needed, don't churn what exists:**
`Assets/Art/{Models,Textures,Materials,UI}`, `Assets/Audio`, `Assets/Prefabs`,
`Assets/Scenes`, `Assets/Content` (ScriptableObjects: ritual configs, decay
settings, reward tables), `Assets/Settings`. Existing `Assets/Scripts` layout
(Core / Domains / Infrastructure / UI / Game) is kept.

**Naming law (Ch. 9.1):** `SM_` static mesh, `SK_` skinned, `T_Thing_Map`
texture, `M_Thing` material, `P_Thing` prefab, `SC_Name` scene, PascalCase
scripts by feature, `SK target_Action` clips. **Nothing enters the Unity project
unless it follows this naming and its task exists in the phase plan (§7).**

---

## 6. Art direction (the Bible owns this — pointers only)

Do not duplicate the Bible here; obey it. The load-bearing rules:
- **Style (Ch. 3.1):** stylized hand-painted warm miniature-world — "a candlelit
  library inside a snow globe". Rounded, chunky, readable by a 7-year-old.
- **The visual battle is WARMTH (restored) vs FOG (The Forgetting).** A
  screenshot of any room should reveal its mastery % with no UI (Ch. 3.2).
- **Lighting IS the progress bar (Ch. 3.4 / 8.6):** restoration physically adds
  amber light sources. Dim cool ambient so every warm source reads as precious.
- **Books are instances, not architecture (Ch. 5.5):** shelves ship empty; Unity
  scatters book instances so shelves visibly fill as knowledge is restored — the
  core progress image of the whole game.
- **The trim sheet (Ch. 6.3)** textures the entire modular kit — the single
  most-leveraged art task. **The modular kit (Ch. 5.3)** of 18 pieces unlocks all
  level building — model it before any unique prop.
- **UI (Ch. 3.5):** parchment panels, oak trim, violet rune accents, wax-seal
  buttons; ≥64 px touch targets; ink-dark body text on parchment, never
  light-on-dark for reading; dyslexia-friendly font toggle.

---

## 7. Phase plan (GL-0 → GL-2, reconciled with Bible Ch. 10)

Sequential. Each phase's acceptance must pass **in-editor** before the next. The
**Art Track** runs in parallel and never blocks a gameplay phase.

### GL-0 — Foundations & Feel (Bible Phase 0)
- **Freeze BookData** (§4): document the mapping; write 2 sample books. *(pure
  logic, no art — the natural first task.)*
- **Project baseline:** URP 3D settings, packages (TextMeshPro, Addressables,
  Cinemachine, Input System, ProBuilder, Newtonsoft), quality tiers, Ch. 7.2
  folders, empty scene set.
- **Shared feel systems (Bible F5 — highest leverage):** `FeedbackDirector`
  (single service for correct/incorrect/complete grammar, §1), InkSpread URP
  Shader Graph, motes VFX Graph, pentatonic chime set.
- **Greybox `SC_ReadingRoom`:** ProBuilder alcove (desk, chair, shelf, window,
  Book) at Bible scale (Ch. 5.1); a **placeholder Book** whose page is a
  RenderTexture/TMP projection of BookData text.

**Accept:** project compiles, EditMode tests green; the grey alcove is walkable;
tapping a placeholder word plays the full correct-grammar (ink spread + chime +
motes) through `FeedbackDirector`; placeholder mode loads a sample book.

### GL-1 — Vertical Slice: the grey alcove, all five rituals (Bible Phase 1)
- **Reading engine LOAD/READ** projected onto the Book page (port from the 2D
  ribbon; `TrekSession` unchanged): keyword glow, fog/damage masks on
  keywords/blanks.
- **Word Kindling:** tap keyword on the page → rune lift → Lexicon fly-in.
- **Three ritual panels** (Ink Weaving / Definition Reforging / Vision
  Restoration) as UI-Toolkit panels themed per Ch. 3.5, each calling
  `FeedbackDirector`; checking via existing `GateSolving`.
- **RESTORE ceremony:** page exhale → book flies to shelf → XP/Lexicon grant →
  save → submission (existing path). Contract R-6…R-11 intact.
- **Lighting-as-progress (greybox):** rituals add amber point lights as they
  complete.
- **Owl (placeholder):** the hint voice (R-8) — capsule/stand-in is fine in grey.

**Accept:** one sample book is fully playable end-to-end in the grey alcove; all
five rituals fire the shared grammar; a book with 2 deferred rituals cannot
complete until the final restoration pass clears them; exit-and-resume restores
ritual states, deferred queue and elapsed time; offline downloaded assignment
plays with zero network calls and queues the submission; submission fires only on
full clearance. **Then playtest the feel** (Bible's thesis — Ch. 10 close).

### GL-2 — MVP: the living library (Bible Phase 2)
- **LibraryHub:** the Entrance Hall, **rebuilt from the existing "Reading
  Adventure" 3D diorama** (`Assets/Gulps/WorldMap/`). Decision (2026-07-20): the
  outdoor island fiction is retired; its **systems carry over** — the fixed
  portrait diorama camera rig, `WorldAnchoredUI` camera-tracked badges, the
  node/progress + star-tier model, and the editor "Place X" asset pipeline. The
  **space moves indoors** to a candlelit library room: assignments wait as books
  on the Librarian's desk / shelves; tapping a book enters its Reading Alcove;
  progress reads as lit lamps + filling shelves (warmth vs fog), not stars on an
  outdoor trail. Wings load additively.
- **Decay / Flicker scheduler (net-new, Ch. 8.3):** SM-2 per keyword per student;
  Flickering state on login (cap 3/day, oldest-due first); re-stabilization
  micro-session; absence rule (pause after 5 days, ≤3 flickers on return).
- **Lexicon Chamber + Rune Door** word-spend interactions.
- **Book cover generator (Ch. 6.4):** composite cover = base + ornament + sigil
  at runtime from `coverRecipe`.
- **Backend sync + telemetry (Ch. 11):** mastery event stream added to the
  existing reporting (`MasteryEvent`, `SessionEvent`, `DecayEvent`, `WorldEvent`).
- **Accessibility pass (Ch. 3.5):** dyslexia font toggle, text size, colour-safe
  check, audio cues, reduced-motion.

**Accept:** the Hub → alcove → shelf loop is complete; due keywords surface as
flickers and re-stabilize; wipe local derived data → Lexicon / ranks / world %
recompute identically from synced marks + mastery events; mastery events reach
the tutor reports.

### Art Track (parallel — Bible Ch. 5 / 6, solo-paced)
Never on the gameplay critical path. In rough priority:
1. Blender standards file (mannequin, 0.5 m grid, export preset — Ch. 5.1).
2. The **trim sheet** (Ch. 6.3) + parchment/page set + clutter atlas.
3. **Modular kit pieces 1–10** (Ch. 5.3) → replace `SC_ReadingRoom` greybox.
4. Hero **Book** + desk/chair/candle/lantern (kit 15–18); the **Owl** (SK_Owl,
   6 clips) + expression sheet.
5. Book cover element set (Ch. 6.4); kit book set 11–14; remaining kit + wings.

Use the existing Blender-over-TCP workflow. Each asset is "done" only when it
stands correctly in a Unity test scene under game lighting (Ch. 5.5 step 8 /
Ch. 9.3 Definition of Done), replacing exactly one greybox.

### Later (Bible Phase 3 — deliberately unplanned)
Additional Wings, one per curriculum term, re-using the kit. Written by pilot
telemetry (Ch. 11.1). Do not pre-commit.

---

## 8. Guardrails for the agent

- **Never weaken the Restoration Contract (R-1…R-11)** to make a task easier. If a
  task conflicts, stop and flag it.
- **Never bypass ViewModels** to call Supabase/Photon from a presenter.
- **Never break the BookData / submission / marks schemas.** Extend additively;
  flag any schema change.
- **Blockout before beauty:** never let missing 3D art block a gameplay phase —
  greybox and move on.
- **Feel before world:** if `FeedbackDirector` and the reading engine aren't
  proven, don't build rooms.
- **Prefer restyling existing UXML panels** over rebuilding them.
- **Keep each commit inside one phase;** reference the R-rule / GL-phase / Bible
  chapter it satisfies.
- Claude cannot run Unity in this environment — in-editor acceptance
  (Test Runner, play-mode, scene saves) is the user's step.
