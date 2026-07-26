# sReader Navigation Audit

_Generated 2026-06-22. Source of truth: the view controllers under `Assets/Scripts/UI`._

This document maps **every** navigation path in the app, separates what is **wired and working** from what is a **placeholder / not implemented**, and flags structural issues to fix when we refine navigation.

---

## 1. How navigation works (two layers)

The app has **two independent navigation systems**:

### Layer A — Page navigation (`NavigationManager`)
[NavigationManager.cs](Assets/Scripts/UI/Navigation/NavigationManager.cs) holds 12 full-screen pages as `GameObject`s and shows one at a time (`SetActive`). It is a **flat switcher — there is no back stack / history**. Every "Back" button is a hard-coded destination, not a "pop".

Pages: `Landing, Login, Register, About, Contact, ForgotPassword, OTP, ResetPassword, Profile, StudentHome, GuardianHome, TutorHome`.

Post-login routing is centralised in [PostLoginRouter.cs](Assets/Scripts/UI/Navigation/PostLoginRouter.cs) → `NavigationManager.ShowHomeForRole(role)`.

### Layer B — In-app section navigation (home shells)
Once logged in, the **Student** and **Tutor** homes use [ShellHomeController.cs](Assets/Scripts/UI/Views/Home/ShellHomeController.cs): a bottom-nav (sections) → top sub-tabs → swapped center content. Deeper navigation (list → profile → edit, etc.) happens **inside each section's View** by swapping content in place (its own private "back"), again with **no shared history**.

The **Guardian** home is different — it uses [RoleHomeController.cs](Assets/Scripts/UI/Views/Home/RoleHomeController.cs), a simple screen with 3 quick-action buttons and **no bottom-nav shell**. (Inconsistency — see §4.)

---

## 2. Page-navigation graph (Layer A)

Every wired transition, by source page:

| Page | Element | Goes to | Status |
|------|---------|---------|--------|
| **Landing** | `get-started-button` | Register | ✅ |
| | `signin-button` | Login | ✅ |
| | `about-button` | About | ✅ |
| | `contact-button` | Contact | ✅ |
| | (UIPager) | swipe carousel of landing pages | ✅ |
| **About** | `back-button` | Landing | ✅ |
| | `get-started-button` | Register | ✅ |
| | `contact-button` | Contact | ✅ |
| **Contact** | `back-button` | Landing | ✅ |
| | `get-started-button` | Register | ✅ |
| | `send-button` | sends message, stays on page | ✅ |
| **Login** | `back-button` | Landing | ✅ |
| | `create-account-button` | Register | ✅ |
| | `forgot-password-button` | ForgotPassword | ✅ |
| | `signin-button` | → `PostLoginRouter` → role home (fallback Profile) | ✅ |
| | `google-button` | **`Debug.Log` stub** | ❌ Not implemented |
| | `apple-button` | **`Debug.Log` stub** | ❌ Not implemented |
| **Register** | `back-button` | Landing | ✅ |
| | `signin-button` | Login | ✅ |
| | `register-button` | → `PostLoginRouter` → role home (fallback Login) | ✅ |
| | `role-student/guardian/tutor` | segmented role picker (visual only) | ✅ |
| | multi-step `›/‹` (UIPager) | step validation gating | ✅ |
| | `google-button` | **`Debug.Log` stub** | ❌ Not implemented |
| | `apple-button` | **`Debug.Log` stub** | ❌ Not implemented |
| **ForgotPassword** | `back-button` | Login | ✅ |
| | `send-code-button` | OTP | ✅ |
| | `signin-link` | Login | ✅ |
| **OTP** | `back-button` | ForgotPassword | ✅ |
| | `verify-button` | ResetPassword | ✅ |
| | `resend-button` | resends, stays | ✅ |
| | `signin-link` | Login | ✅ |
| **ResetPassword** | `back-button` | Login | ✅ |
| | `update-button` | Login | ✅ |
| | `signin-link` | Login | ✅ |
| **Profile** | `back-button` | edit→view, else → router home / Landing | ✅ |
| | `edit / cancel / save` | view↔edit card swap | ✅ |
| | `profile-avatar` (tap) | change picture | ✅ |
| | `signout-button` | Landing | ✅ |
| **Role homes** | `profile-button` (avatar) | Profile | ✅ |
| | `signout-button` | Landing | ✅ |

### PostLoginRouter role mapping
| Role | Lands on | Status |
|------|----------|--------|
| Student (default) | StudentHome | ✅ |
| Guardian | GuardianHome | ⚠️ shell is placeholder (§3) |
| Tutor | TutorHome | ✅ |
| Teacher | TutorHome (reuses tutor) | ✅ |
| Administrator | Profile | ❌ No admin console exists |

---

## 3. In-app section navigation (Layer B) — implemented vs placeholder

### Student home — `ShellHomeController` ([StudentHomeController.cs](Assets/Scripts/UI/Views/Home/Student/StudentHomeController.cs))
| Bottom-nav section | Sub-tabs | Backing view | Status |
|--------------------|----------|--------------|--------|
| `nav-home` | Feed, Activity | — | ❌ **Placeholder cards** ("Content coming soon") |
| `nav-academies` | Discover, Enrolled | `StudentAcademiesView` | ✅ Implemented |
| `nav-tutors` | Find, My tutors | — | ❌ **Placeholder cards** |
| `nav-assignments` | Assignments | `StudentAssignmentsView` | ✅ Implemented |
| `nav-friends` | Friends, Discover, Search, Requests | `StudentFriendsView` | ✅ Implemented |

### Tutor home — `ShellHomeController` ([TutorHomeController.cs](Assets/Scripts/UI/Views/Home/Tutor/TutorHomeController.cs))
| Bottom-nav section | Sub-tabs | Backing view | Status |
|--------------------|----------|--------------|--------|
| `nav-students` | My students, Requests | — | ❌ **Placeholder cards** |
| `nav-academies` | My academies, All, Requests (+ "Payments", "＋ Create" links) | `TutorAcademiesView` | ✅ Implemented (deep: grades→classes→subjects→assignments) |
| `nav-guardians` | All, Messages | — | ❌ **Placeholder cards** |
| `nav-assignments` | Active, Drafts, Submissions | — | ❌ **Placeholder cards** (real assignments live under Academies → class, not here) |

### Guardian home — `RoleHomeController` ([GuardianHomeController.cs](Assets/Scripts/UI/Views/Home/Guardian/GuardianHomeController.cs))
| Element | Action | Status |
|---------|--------|--------|
| `my-students-button` | `Debug.Log` stub | ❌ Not implemented |
| `progress-button` | `Debug.Log` stub | ❌ Not implemented |
| `link-student-button` | `Debug.Log` stub | ❌ Not implemented |
| `profile-button`, `signout-button` | Profile / Landing | ✅ |

> The entire Guardian experience is unbuilt — 3 dead buttons, no bottom-nav shell, no sections.

---

## 4. Findings — what to fix when refining navigation

### A. Not-implemented navigation items (summary)
1. **Google / Apple OAuth** — 4 stub buttons across Login + Register.
2. **Student `nav-home`** (Feed/Activity) — placeholder.
3. **Student `nav-tutors`** (Find/My tutors) — placeholder.
4. **Tutor `nav-students`** (My students/Requests) — placeholder.
5. **Tutor `nav-guardians`** (All/Messages) — placeholder.
6. **Tutor `nav-assignments`** (Active/Drafts/Submissions) — placeholder; real assignment management is buried under Academies → grade → class.
7. **Guardian home** — entirely placeholder (3 stub buttons).
8. **Administrator role** — no console; silently dumped on Profile.

### B. Structural issues to address
- **No back stack / history.** Both layers are flat. "Back" buttons are hard-coded destinations, so the same screen can only ever go "back" to one place regardless of how it was reached (e.g. Profile is reachable from any role home + post-login, but its Back always routes via `PostLoginRouter`). A small navigation stack would make Back predictable.
- **Two inconsistent home patterns.** Student/Tutor use the bottom-nav `ShellHomeController`; Guardian uses the flat `RoleHomeController`. Guardian should be migrated onto the shell for consistency (and to host real sections).
- **Profile/Settings has no nav entry point** beyond tapping the avatar — easy to miss; no settings screen at all.
- **Placeholder sections look identical to real ones** (same tabs, same "item 1/2/3" cards). A user can't tell a finished feature from an unbuilt one. Consider hiding unbuilt sections or showing an explicit "Coming soon" state.
- **Deep navigation lives inside each View** (academies/classes/friends/assignments swap content internally with their own Back) — not visible to `NavigationManager`. Fine functionally, but means there's no global "where am I" or breadcrumb.
- **Label drift:** Student `nav-assignments` section title/tab is the generic "Assignments" while it actually renders the full `StudentAssignmentsView`; tutor assignment work is under "Academies", not the "Assignments" tab — confusing taxonomy.

### C. Suggested refinement priorities
1. Decide the canonical **bottom-nav set per role** and delete/merge placeholder sections (don't ship dead tabs).
2. Build **Guardian** onto `ShellHomeController` with real sections (link student, progress, students).
3. Either implement or remove **Google/Apple** buttons (don't ship stubs).
4. Route **Administrator** somewhere real (admin console) or hide the role.
5. Introduce a lightweight **navigation history/back stack** so Back is consistent across both layers.
6. Surface the real **tutor assignment** flow as a first-class section instead of nesting it under Academies.

---

## 5b. Re-check (2026-06-26) — after your edits

**Edits found in [StudentAssignmentsView.cs](Assets/Scripts/UI/Views/Home/Assignments/StudentAssignmentsView.cs) + its ViewModel — all improve flow seamlessness:**
- ✅ **Resume-in-progress game** — `LoadProgress`/`SaveProgress` + `SetResume`/`SetProgressSink`: a student can leave a game and continue where they left off.
- ✅ **"Play again"** after completion (`ResetAttemptAsync`) — re-enters the mode-select cleanly.
- ✅ **Multiplayer Exit is now clean** — game `onBack` calls `LeaveSessionAsync()` then returns to the assignment profile, so you're no longer yanked back into the board and the turn doesn't stall on you.
- ✅ **Photon backend option** for instant multiplayer (picked in the lobby, remembered per device).

**Still NOT seamless (unchanged — these are the structural gaps to fix next):**
- ❌ **No Android system back-button / back-gesture handling anywhere** (verified: only legacy files + on-screen buttons). On device the hardware back does nothing or risks quitting. _Biggest mobile UX gap._
- ❌ **No shared back stack** — every `‹ Back` is one hard-coded hop; can't skip levels (§6.6 #1).
- ❌ **Back loses your place + tab** — course list resets to tab 0, academy reopens at "About", lists lose scroll (§6.6 #2).
- ❌ **Switching bottom-nav section/sub-tab wipes any deep screen** — no resume (§6.6 #3).
- ❌ **Two student assignment screens** (rich vs read-only) still unlinked (§6.4 / §6.6 #7).

---

## 5c. Implemented (2026-06-26) — first seamless-navigation slice

Shipped a contained, revertible slice on the **student Assignments flow** (backups in scratchpad `nav-backup-2026-06-26/`):
- **New [`NavStack`](Assets/Scripts/UI/Navigation/NavStack.cs)** — a reusable in-content back stack (Root / RootSilent / Push / Back).
- **[StudentAssignmentsView](Assets/Scripts/UI/Views/Home/Assignments/StudentAssignmentsView.cs) routed through it** — list ↔ profile ↔ submit ↔ schedule. `‹ Back` is now real multi-level history and **restores the mini-nav tab you were on** (was: reset to tab 0). The play/game flow keeps its own on-screen Exit.
- **Android / system back button** wired in [ShellHomeController](Assets/Scripts/UI/Views/Home/ShellHomeController.cs) (`Update` → Escape): pop the active section's screen → else jump to the Home section → else confirm-to-exit toast. Applies to Student **and** Tutor homes (graceful: sections without a handler just go to Home/exit).
- **"Find a class to enrol"** shortcut on the empty assignments state → jumps straight to Academies › Discover (cross-section via new `GoToSection`), so a student with no assignments is never stranded.

**Not yet done (next slices, pending your sign-off on the feel):** roll `NavStack` out to TutorAcademiesView / StudentAcademiesView / ClassesView / StudentFriendsView; per-section resume; auth-page back. Needs a Unity in-editor compile to confirm (no standalone .NET pack).

### Update — system back button fix + Home dashboard (2026-06-26)
- **Input System crash fixed:** the project has the Input System package active, so the per-frame `UnityEngine.Input.GetKeyDown` in `Update()` threw ~every frame. Replaced with UI Toolkit's native `NavigationCancelEvent` (registered on the panel root, no `Update`, no `UnityEngine.Input`). _Caveat: the physical Android back may need binding to the Input System "Cancel" action to fire on-device; desktop Escape + in-app `‹ Back` work now._
- **Real student Home dashboard** (replaces the `nav-home` "Feed/Activity" placeholder cards — [StudentHomeController](Assets/Scripts/UI/Views/Home/Student/StudentHomeController.cs)). The `nav-home` section is now a single **"Today"** launchpad showing live data from `StudentAssignmentsViewModel`:
  - **Hero tile** = the most useful next action: *Resume* an in-progress attempt → else *Open* the soonest-due assignment → else *Find a class*. The button **deep-links straight into that assignment's profile** (new `StudentAssignmentsView.OpenAssignment` + a pending-id handoff through `GoToSection`).
  - **Stat tiles** — Due soon / Scheduled / Completed counts (tap → Assignments).
  - **"Up next on your schedule"** tile — the next scheduled session → opens that assignment.
  - **Quick links** — Assignments · Friends · Academies · Profile (one tap each).
  - Tiles **fade + slide in with LeanTween** (`LeanTween.value` → opacity/translate, staggered; falls back to visible if tweening is unavailable). Built compact to avoid scrolling.
  - **Quick-link & stat tiles are now uniform** — shared `EqualTile()` with a fixed `TileHeight` (equal width via flex-grow:1/flex-basis:0), so all four quick links are always the same size regardless of label length.
- **Tutors section is now real** (`nav-tutors`, was "Find / My tutors" dummy cards). Single **"My tutors"** tab listing the actual tutors at the academies the student is enrolled in — sourced from the friend-safe `student_academics` view via `FriendshipViewModel.LoadAcademicsAsync(myId)`, grouped one card per tutor showing their academy/grade and the subjects they teach the student. Empty state offers "Find a class".
- **Even filter grid on Assignments** — the "All / Due soon / Overdue / Submitted / Completed / Scheduled" pills are now a uniform **3×2 grid** (equal width via `flex-basis: 31.5%`, fixed 36px height, even gaps) instead of a ragged content-width wrap.
- **Classes & subjects on Home** — the dashboard's `BuildLearning` card shows **MY CLASSES** (from `Assignments.ClassNames`; tap a class → Assignments pre-filtered to it via `StudentAssignmentsView.OpenClass`) and **MY SUBJECTS** (enrolled courses from `Academy.Enrollments`; tap → Academies › Enrolled via new `GoToSection(navButton, tabIndex)` overload). Both visible and one-tap navigable from Home.
- **Home is now a fixed, non-scrolling screen** ("can't scroll in a game"): the dashboard fills the viewport and **clips instead of scrolls** — the ScrollView's `contentContainer` gets `flex-grow:1`, the dashboard root is `flex-grow:1` + `overflow:Hidden` + `justify:space-between`, the hero/stats/quick-links are `flex-shrink:0`, and the classes/subjects card is the flexible block that yields space first. Trimmed to fit: dropped the separate schedule tile, tile height 88→68, hero padding 18→12.

---

## 6. Education & Academy module — deep navigation map ⭐ (most important)

This is the heart of the app and where almost all real navigation happens. **None of it goes through `NavigationManager`** — every screen is drawn by clearing and rebuilding the home's `content-area` from inside a View class (`content.Clear()` then re-add). Each screen hard-codes its own `‹ Back` target, so there is **no history and no memory of where you were** (see findings in §6.6).

### 6.0 The object hierarchy
- `nav-academies` (bottom-nav) → `TutorAcademiesView` / `StudentAcademiesView`
- a grade's **Subjects/Modules** → `ShowCoursesHome` (same View)
- a grade's **Classes** → `ClassesView` (shared, `isTutor` flag) — launched with an `onBack` that returns to the academy profile
- a class's **assignments** → still inside `ClassesView`; **content authoring** → `AssignmentContentEditorView`
- the student's separate `nav-assignments` → `StudentAssignmentsView` → the game → `AssignmentGameView`

### 6.1 Tutor flow — full screen tree
```
[Bottom nav] nav-academies        sub-tabs: My academies · All · Requests
                                  upper-nav links: "Payments"  "＋ Create"
│
├─ My academies / All ─► (list) Academy card ──► ACADEMY PROFILE
│     ‹Back → list                               mini-nav: About | Grades
│        ├─ About ── description + [Edit] [Delete]
│        │     Edit   → Create/Edit form ──► back to profile
│        │     Delete → confirm screen ──► Yes → list / Cancel → profile
│        └─ Grades ── [＋ Add] + Grade rows
│              ＋ Add → Add-grade (Primary/Secondary/University picker) ──► profile
│              Grade row pills:  [Subjects|Modules]  [Classes]  [Remove]
│                 │
│                 ├─ Subjects/Modules ──► COURSES HOME   mini-nav: Subjects | New | Search
│                 │     ├─ Subjects (list) → Course card → COURSE PROFILE
│                 │     │        [Edit / price] → edit form → profile
│                 │     │        [Remove] → list
│                 │     ├─ New    → add-subject/module form → list
│                 │     └─ Search → field → results → Course profile
│                 │
│                 ├─ Classes ──► ClassesView  (onBack → academy profile)  ……see 6.3
│                 └─ Remove  → deletes grade → reopens profile
│
├─ Requests ──► [Enrollment requests]  (Enroll / Reject per card; "View payment proof")
│               [Academy join requests] (Approve / Reject)
│
├─ ＋ Create (upper nav) → Create academy form → list
└─ Payments  (upper nav) → Payment-details form (FNB / Orange Money) → save
```

### 6.2 Student flow — full screen tree
```
[Bottom nav] nav-academies        sub-tabs: Discover · Enrolled
│
├─ Discover ─► (list) Academy card ──► ACADEMY PROFILE (read-only)
│     ‹Back → Discover                 [Request to join]  + Grades list
│        Grade row pills:  [Subjects|Modules]  [Classes]
│           ├─ Subjects ──► COURSES HOME   mini-nav: Subjects | Search
│           │     list → Course card → COURSE PROFILE
│           │        [Request (pay)]   (+ [Start free trial] if offered)
│           └─ Classes ──► ClassesView (read-only)  ……see 6.3
│
└─ Enrolled  ─► "My requests"   ⚠ tab labelled "Enrolled" but screen says "My requests"
        Request card → [Pay & submit proof]  (only when Requested & paid)
           → ShowPay: tutor payment details + reference + ATTACH PROOF IMAGE + Submit → back
```

### 6.3 Classes — shared sub-tree (`ClassesView`, entered from a grade)
```
ClassesView.Show ──► CLASSES HOME   ‹Back → (academy profile)
                     mini-nav: Classes | New | Search   (tutor)
                               Classes | Search          (student)
│
├─ Classes (list) → Class card ──► CLASS PROFILE
│     TUTOR actions:
│        [Manage subjects] → mini-nav: In this class | Assign new   (add / remove subject↔class link)
│        [Manage students] → Roster
│        [Manage assignments] → ASSIGNMENTS LIST   ……see 6.4-A
│        [Edit class] → edit form → profile
│        [Delete class] → confirm → Classes home
│     STUDENT:
│        enrolled  → "✓ enrolled" + [View assignments] (read-only)   ……see 6.4-C
│        not yet   → 🔒 "pay for one of its subjects" note (dead-end here)
├─ New (tutor) → create-class form → Classes home
└─ Search → field → results
```

### 6.4 Assignments — there are **THREE different doors** to assignments
| Door | Path | View | Capability |
|------|------|------|-----------|
| **A. Tutor authoring** | nav-academies → academy → Grades → grade → **Classes** → class → Manage assignments → assignment | `ClassesView` | Create/edit/delete, **Edit content** (game authoring via `AssignmentContentEditorView`) |
| **B. Student (rich)** | **nav-assignments** (its own bottom-nav section) → My assignments / Search / Schedules → assignment profile | `StudentAssignmentsView` | Attempt, **play game** (solo/multiplayer), Submit, Schedule, Download, "Plan with others" |
| **C. Student (read-only, duplicate)** | nav-academies → academy → grade → Classes → class → **View assignments** → assignment | `ClassesView` | Read-only list/detail — **no attempt, no game** |

> ⚠️ **Doors B and C show the same assignments through two different screens with different powers.** A student can land on a stripped-down read-only assignment (C) and never discover the rich game flow (B), or vice-versa. They don't link to each other.

### 6.5 Game & multiplayer sub-flow (from door B, "Start attempt")
```
Assignment profile → [Start/Continue attempt] → MODE SELECT
   │   (if no game content: shows "Nothing to play yet" and stops)
   ├─ Play solo ─────────► AssignmentGameView (board)  → finish → Assignments home
   └─ Play with others ─► LOBBY (Host / Join by code / Open games list)
            → GAME ROOM (shareable code + live avatars + Start)   ‹Leave → Lobby
                  host Start (or auto-follow when host starts) → shared turn-based board
                        → finish → Assignments home
   game ‹Back: solo → assignment profile ; multiplayer → game room
```

### 6.6 Findings & refinement priorities — education module
**Movement / "where am I" problems (the core ask):**
1. **No back stack — every `‹ Back` is a single hard-coded jump.** Deep paths are 6–8 levels (e.g. tutor editing a subject's price: nav → academy card → profile → Grades tab → Subjects pill → course card → profile → Edit). Backing out is one coded step at a time and **cannot skip levels**.
2. **Back loses your place and your tab.** Returning from a Course profile always calls `ShowCoursesHome(...,0)` → resets to the first sub-tab even if you arrived from "Search". Returning to an Academy profile reopens the **About** tab even if you were on **Grades**. Returning from `ClassesView` to the academy profile likewise resets to About. Lists lose scroll position.
3. **Switching bottom-nav section or sub-tab wipes any deep screen.** `ShellHomeController.RenderContent()` rebuilds from scratch and the Views always restart at their home screen (`ShowMine()` / `ShowHome(0)` / `ShowDiscover()`), so tapping another tab and coming back drops you at the top — there is no "resume".
4. **A grade is not a destination.** Unlike academies/classes/subjects (which follow list → profile → actions), a grade is just a card with inline pills (Subjects/Classes/Remove) — inconsistent with the module's own rule and a dead spot in the hierarchy.
5. **The subject↔class link is edited from two far-apart places.** A subject lives under *grade → Subjects*, but is attached to a class from inside *class → Manage subjects*. The relationship is circular and hard to discover.
6. **Student enrolment is an invisible, split journey.** To get into a class a student must: Discover → academy → grade → Subjects → subject → *Request (pay)*, then switch to the **Enrolled** tab → *Pay & submit proof*, then wait for tutor confirmation — only then does the class stop showing 🔒. Three screens across two tabs with no guided flow.
7. **Two student assignment screens** (doors B & C above) with different capabilities and no link between them.

**Label / consistency issues:**
8. Student academies tab **"Enrolled"** opens a screen titled **"My requests"** — rename one.
9. Tutor **"Requests"** merges paid-enrollment requests and academy-join requests on one screen — two concepts, one list.
10. Tutor **"Assignments"** bottom-nav section is a placeholder (§3), while real tutor assignments live deep under **Academies → class**. The taxonomy points users to the wrong place.

**Suggested refinements:**
- Introduce a **navigation stack** (push/pop) shared by these Views so `‹ Back` always returns to the exact previous screen with its tab/scroll intact, and allow multi-level back-out.
- Give each entity a **breadcrumb header** (Academy › Grade › Class › Assignment) so users can see depth and jump up.
- Make **grade** a real profile screen (list → profile → [Subjects][Classes][Edit][Delete]) to match the rest.
- **Unify the two student assignment screens** — door C should deep-link into door B's rich profile.
- Turn student enrolment into one **guided flow** (subject → pay → proof → status) instead of two tabs.
- Reconsider whether **subject→class linking** should be doable from the subject profile too (symmetry).

---

## 8. Recommended plan for seamless navigation (design only — not yet implemented)

The goal: `‹ Back` and the **Android system back button/gesture** always return to the exact previous screen — same sub-tab, same scroll — and can step back through the whole trail, across auth pages and the deep education/academy/assignment screens alike.

### 8.1 Add one shared back stack
Introduce a tiny `NavStack` (plain C# class, no Unity deps) that stores `Action` "show this screen again" closures:
- `Push(Action render)` — call at the **top of every `Show*`/`Render*` screen** in the in-content views (TutorAcademiesView, StudentAcademiesView, ClassesView, StudentAssignmentsView, StudentFriendsView). The closure captures the screen's args **and its active tab**, so re-running it restores the exact view.
- `Back()` — pops the current entry and re-invokes the previous closure. Every `‹ Back` link calls `NavStack.Back()` instead of a hard-coded `ShowX()`.
- `Reset(Action root)` — called when a bottom-nav section is selected, to start that section's trail fresh.
- One stack instance lives on the home shell and is handed to each View (they already receive their backing VMs the same way).

This removes findings §6.6 #1 and #2 in one move: Back becomes real history, and because each closure re-runs the screen with its own tab index, the tab/place is preserved.

### 8.2 Wire the Android back button
In `ShellHomeController` (and the auth-page controllers), poll `Input.GetKeyDown(KeyCode.Escape)` in `Update()` — on Android the system back maps to Escape:
- If an overlay/modal/game is open → close it.
- Else if the in-content `NavStack` has history → `NavStack.Back()`.
- Else if not on a section root → go to the section root.
- Else (on a home root) → confirm-to-exit (double-tap "Press back again to exit"), never silently quit.

For the **auth/page layer** ([NavigationManager](Assets/Scripts/UI/Navigation/NavigationManager.cs)), give it a small page history too so back from Login → Landing, OTP → ForgotPassword, etc. uses the same gesture.

### 8.3 Remember each section's place when switching bottom-nav tabs
`ShellHomeController.RenderContent()` currently rebuilds from scratch every time. Keep a per-section saved `NavStack` (or just the last screen closure) so returning to a section resumes where you were instead of restarting at its root (finding §6.6 #3). _Optional / lower priority — only if the "restart at root" behaviour proves annoying in testing._

### 8.4 Link the two student assignment screens
Make the read-only class assignment view (door C, `ClassesView` → View assignments) deep-link into the rich `StudentAssignmentsView` profile (door B) for the same assignment, so students always reach the full attempt/game flow (findings §6.4 / §6.6 #7).

### 8.5 Small consistency fixes (cheap, high-polish)
- Rename student academies tab **"Enrolled"** → **"My requests"** (or retitle the screen) so the label matches.
- Give **grade** a real profile screen to match list → profile → actions.
- Add a lightweight **breadcrumb** header (Academy › Grade › Class › Assignment) so depth is visible.

### 8.6 Suggested order
1. §8.2 Android back button wired to existing per-screen back — _immediate mobile win, low risk._
2. §8.1 Shared `NavStack` — _the core of "seamless"; convert `‹ Back` links to `Back()`._
3. §8.5 label/breadcrumb polish.
4. §8.4 link the two assignment screens.
5. §8.3 per-section resume (only if needed).

---

## 9. Shortening student routes to assignment-related tasks (design only)

**Principle: surface + deep-link, don't nest.** Anything a student needs regularly should be one tap from the home screen or a badge — not found by walking Academy › Grade › Subject › Class.

### 9.1 Current tap-counts (measured from the code)
| Task | Current route | Taps | Verdict |
|------|---------------|------|---------|
| **Do an assignment** (already enrolled) | nav-assignments → card → Start attempt → Play | ~4 | ✅ already short |
| **Get enrolled** (prereq for *any* assignments) | nav-academies → academy → profile → grade "Subjects" → subject → Request (pay) → switch "Enrolled" tab → Pay & submit proof → Attach → Submit | ~9 + external payment + tutor wait | ❌ long, split across two sub-tabs |
| **Resume a half-finished game** | (no entry point — must re-navigate to the exact assignment) | — | ❌ invisible despite resume-data existing |
| **Assignments from a class** | nav-academies → academy → grade → Classes → class → View assignments (read-only) | ~6 | ❌ long *and* a dead-end (door C) |
| **Home screen** | nav-home → placeholder cards | — | ❌ the first screen every student sees does nothing |

### 9.2 Fixes, in priority order
1. **Turn `nav-home` into a launchpad** (biggest win — it's currently dead). One-tap tiles: **Continue** (resume in-progress games — the data already exists via `LoadProgress`), **Due soon / Overdue**, **New assignments**, **Today's plan**. Collapses the common 4-tap journey to 1.
2. **Badge the Assignments bottom-nav icon** with due-soon/overdue counts.
3. **Collapse enrolment into one guided flow** — after "Request (pay)", flow straight into Pay → proof instead of making the student hunt the "Enrolled" tab. Give the assignments empty-state a **"Find a class to enrol"** button that deep-links into Discover so a new student is never stranded.
4. **Deep-link door C → door B** — a class's "View assignments" jumps into the rich assignment profile (attempt/game/schedule), not the read-only dead-end.
5. **Add a "My classes" shortcut** so a student reaches their class directly instead of academy → grade → Classes → class.

These are independent of the §8 back-stack work and can ship separately; together they mean a student reaches "continue my work", "what's due", and "enrol" from the home screen or a badge rather than by descending the hierarchy.

---

## 7. File reference
- Page switching: [NavigationManager.cs](Assets/Scripts/UI/Navigation/NavigationManager.cs), [PostLoginRouter.cs](Assets/Scripts/UI/Navigation/PostLoginRouter.cs)
- Shell / role homes: [ShellHomeController.cs](Assets/Scripts/UI/Views/Home/ShellHomeController.cs), [RoleHomeController.cs](Assets/Scripts/UI/Views/Home/RoleHomeController.cs)
- Bottom-nav definitions: [StudentHomeController.cs](Assets/Scripts/UI/Views/Home/Student/StudentHomeController.cs), [TutorHomeController.cs](Assets/Scripts/UI/Views/Home/Tutor/TutorHomeController.cs), [GuardianHomeController.cs](Assets/Scripts/UI/Views/Home/Guardian/GuardianHomeController.cs)
- Auth/landing pages: [Landing](Assets/Scripts/UI/Views/Landing/LandingRealController.cs), [About](Assets/Scripts/UI/Views/About/AboutUsRealController.cs), [Contact](Assets/Scripts/UI/Views/ContactUs/ContactUsController.cs), [Login](Assets/Scripts/UI/Views/Login/LoginPageController.cs), [Register](Assets/Scripts/UI/Views/Register/RegisterPageController.cs), [ForgotPassword](Assets/Scripts/UI/Views/ForgotPassword/ForgotPasswordController.cs), [OTP](Assets/Scripts/UI/Views/OTP/OTPController.cs), [ResetPassword](Assets/Scripts/UI/Views/ResetPassword/ResetPasswordController.cs), [Profile](Assets/Scripts/UI/Views/Profile/ProfilePageController.cs)
- Pager/binder helpers: [UIPager.cs](Assets/Scripts/UI/Bindings/UIPager.cs), [UIPageBinder.cs](Assets/Scripts/UI/Bindings/UIPageBinder.cs)
</content>
</invoke>
