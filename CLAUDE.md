# sReader — Agent Guide

sReader is a Unity (URP) reading app with a UI Toolkit + MVVM front end and a
Supabase-backed domain layer. The student-facing reading game — **The Great
Library of Knowledge** — is a full-3D, hand-painted library world layered on top
of the existing student experience. It *replaces the Story Trek fiction* while
reusing the entire Story Trek logic spine.

## The Great Library — READ THIS BEFORE TOUCHING GAME CODE

The game is governed by **[Docs/GREAT_LIBRARY_SPEC.md](Docs/GREAT_LIBRARY_SPEC.md)**
(how it maps to code — the active brief) and the **Great Library Production
Bible** `Docs/Great_Library_Production_Bible.docx` (what the game is — art
direction, world plan, pipelines, systems). On a code detail the spec wins; on
look/feel the Bible wins; if a task can't satisfy both, **stop and flag it.**
Rules:

- **Work one phase at a time (GL-0 → GL-2).** Do not start a phase until the
  previous phase's acceptance checks pass in-editor.
- **Blockout before beauty / feel before world.** Prove gameplay in grey
  ProBuilder; the Blender art track never blocks a gameplay phase.
- **Never weaken the Restoration Contract (R-1…R-11).** It is already implemented,
  pure and tested, in `TrekSession`. If a task conflicts, stop and flag it.
- **MVVM stays intact.** Views (UXML controllers *and* new 3D scene presenters)
  render state and forward input only. Validation / scoring / persistence /
  networking live in the existing ViewModels + services. Never call
  Supabase/Photon from a presenter.
- **`Docs/STORY_TREK_SPEC.md` is superseded** (kept for history). Do not build new
  Story Trek phases. The logic under it (`TrekSession`, `TrailData`,
  `AssignmentContent`) is the reused spine — see the spec §2.
- Human-only companions — **do not load into context:** the Production Bible
  `.docx` (re-read on demand for exact art wording, but not by default) and the
  old mockup `Docs/sreader-2d-reading-game-design-v3-complete.html`.
- UI functional requirements the game must not break are in
  [Docs/UI-Functional-Requirements.md](Docs/UI-Functional-Requirements.md)
  (GEN-*, FR-SHOME-*, FR-SHELL-*).

## Assembly layout (important)

The project historically compiled everything into the default `Assembly-CSharp`
(no asmdefs). Story Trek introduced a few **narrow** asmdefs so the pure game
logic is unit-testable (asmdef assemblies cannot reference `Assembly-CSharp`, so
tested code must live in an asmdef):

| Assembly | Location | Purpose |
|----------|----------|---------|
| `SReader.Domains.Assignments.Content` | `Assets/Scripts/Domains/Assignments/Content/` | The self-contained assignment **content model** (`AssignmentContent`, `AssignmentContentBuilder`), extracted so game logic can reference it. Auto-referenced, so the rest of `Assembly-CSharp` is unaffected. |
| `SReader.Game.Trek.Logic` | `Assets/Scripts/Game/Trek/Logic/` | Pure, engine-free trek logic (`TrailData`, `TrailBuilder`). Unit-tested. |
| `SReader.Game.Trek.Tests` | `Assets/Tests/EditMode/` | EditMode unit tests. |

**Everything else stays in `Assembly-CSharp`**, including the Trek MonoBehaviour
presenters (`Assets/Scripts/Game/Trek/*.cs`) — they need `NavigationManager` and
the ViewModels, which are in `Assembly-CSharp`. They reference the logic asmdef
(auto-referenced). Rule of thumb: **pure logic → logic asmdef; anything touching
scenes / MonoBehaviours / ViewModels → `Assembly-CSharp`.**

## Conventions

- Navigation is owned by `NavigationManager` (one page GameObject active at a
  time). New full-screen game scenes plug in via `TrekScenePage` (a page wrapper
  that additive-loads a scene and unloads it on hide).
- Controllers re-register callbacks in `OnEnable` (the UIDocument tree rebuilds on
  re-activation) — never cache visual elements across deactivation.
- ViewModels return `Result` and expose `IsBusy` / `ErrorMessage`; views guard
  against double-submit with `IsBusy` and show inline errors.
- Placeholder mode: with no `AppCompositionRoot`, screens run on sample data.
  The Trek runs on `Resources/Trek/SamplePassage`.

## Tests

EditMode tests live in `Assets/Tests/EditMode` (`SReader.Game.Trek.Tests`). Run
them via **Window ▸ General ▸ Test Runner ▸ EditMode**. Keep `TrailBuilder`
logic pure so it stays testable.
