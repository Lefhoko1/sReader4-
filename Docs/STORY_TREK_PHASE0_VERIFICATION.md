# Story Trek — Phase 0 verification (in-editor)

Phase 0 delivers the **foundations**. This file lists exactly what to check in
the Unity Editor to confirm the Phase 0 acceptance criteria, since they can't be
run headless. Do these before starting Phase 1.

## What was built

| Deliverable | Where |
|-------------|-------|
| Content model extracted to its own asmdef | `Assets/Scripts/Domains/Assignments/Content/` |
| `TrailData` + data model (spec §2) | `Assets/Scripts/Game/Trek/Logic/TrailData.cs` |
| `TrailBuilder` (maps existing content → trail) | `Assets/Scripts/Game/Trek/Logic/TrailBuilder.cs` |
| Unit tests | `Assets/Tests/EditMode/TrailBuilderTests.cs` |
| `TrekScenePage` (NavigationManager page + additive loader) | `Assets/Scripts/Game/Trek/TrekScenePage.cs` |
| `NavigationManager.ShowTrek()` + `trekPage` slot | `Assets/Scripts/UI/Navigation/NavigationManager.cs` |
| USS theme tokens | `Assets/UI/Game/TrekTokens.uss` |
| Sample passage (placeholder mode) | `Assets/Resources/Trek/SamplePassage.txt` |
| Assignment-detail launch (guarded) | `StudentAssignmentsView` "▶ Story Trek" pill + `StudentHomeController.trekPage` |

## 1. Compilation

1. Open the project; let Unity reimport (it will generate `.meta` files for the
   new folders/files and compile the new assemblies).
2. **Console has no compile errors.** In particular the moved content files
   (`AssignmentContent`, `AssignmentContentBuilder`) now live in
   `SReader.Domains.Assignments.Content` — confirm `AssignmentService`,
   `StudentAssignmentsViewModel`, `AssignmentContentEditorView`, `AssignmentGameView`
   still compile (they reference the types by namespace, which is unchanged).

## 2. TrailBuilder tests green  ✅ (Phase 0 acceptance)

1. **Window ▸ General ▸ Test Runner ▸ EditMode ▸ Run All.**
2. All tests in `SReader.Game.Trek.Tests / TrailBuilderTests` pass, including the
   malformed-input fallback cases.

## 3. Empty Trek opens from assignment detail and returns  ✅

Scene wiring (one-time, in the scene that holds `NavigationManager`):

1. Create a new UI page GameObject (duplicate an existing page for the
   `UIDocument` + `PanelSettings` setup is easiest). Name it **`TrekPage`**.
2. Add the **`TrekScenePage`** component to it; assign its `Navigation` field to
   the scene `NavigationManager`. Leave `Trek Scene Name` empty (overlay-only for
   Phase 0).
3. Select the `NavigationManager` GameObject → assign **`Trek Page`** = `TrekPage`.
4. Select the **Student Home** page's `StudentHomeController` → assign its
   **`Trek Page`** field = the `TrekScenePage` on `TrekPage`.

Then play:

5. Sign in as a student (or run placeholder mode) → **Student Home ▸ Assignments**
   ▸ open an assignment ▸ tap **▶ Story Trek**.
6. The Trek page shows "Story Trek · Phase 0" and a line like
   `TrailBuilder → N sentences · 0 gates` (proves TrailBuilder + the sample
   passage are wired).
7. Tap **‹ Back** (or the Android back / Esc) → you return to the student home.

## 4. Back-stack + placeholder mode intact  ✅

- The home shell's own back behaviour (FR-SHELL-6: section → home → confirm-exit)
  still works — Trek is a separate full-screen page and doesn't interfere.
- With no `AppCompositionRoot` in the scene, the Trek still opens and builds a
  trail from `Resources/Trek/SamplePassage` (GEN-4).

## Notes / deferred to later phases

- The "▶ Story Trek" pill only appears when `StudentHomeController.trekPage` is
  assigned; unassigned = feature hidden (safe).
- Additive scene loading is implemented but optional: assign a `Trek Scene Name`
  (added to Build Settings) once the Phase 1 Trek scene exists; until then it runs
  overlay-only and logs nothing fatal if the scene is absent.
- The real ribbon / gates / reading view / presenter arrive in Phase 1.
