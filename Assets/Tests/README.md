# Tests

Test root per the architecture blueprint. Because every layer below the
UI is plain C# behind interfaces (`Result`-returning async services,
repositories, the event bus), it is unit-testable without running Unity
scenes.

## Setting up (one-time, in the Unity editor)

1. Window ▸ Test Runner ▸ EditMode ▸ "Create EditMode Test Assembly Folder"
   inside this folder.
2. In the generated `.asmdef`, add an assembly reference to
   `Assembly-CSharp` (or, once the project adopts asmdefs, to the Core and
   Domain assemblies).

## What to test first

- `Domains/Identity` — `IdentityValidation`, `AuthenticationService`
  (with fake `IAuthenticationRepository` / `ISessionStore`).
- `Core/Events/EventBus` — subscribe/publish/dispose.
- `Domains/Sync/SyncService` — queue + retry behavior against the
  in-memory `SqliteSyncQueueRepository`.
- `Domains/Education/EducationService` — class-capacity rule.
