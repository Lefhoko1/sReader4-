# sReader — UI Functional Requirements

**Scope:** The screens implemented with **Unity UI Toolkit** (UXML + USS + MVVM controllers) under `Assets/Scripts/UI/`.
**Source of truth:** Derived by auditing the UXML layouts, their `*Controller` MonoBehaviours (views) and the backing `*ViewModel` classes.
**Audience:** Product / QA / engineering — for use as a requirements reference.
**Date:** 2026-07-04

---

## 1. Architecture context (how to read this document)

The UI follows **MVVM**:

- **View (UXML + Controller MonoBehaviour):** renders elements and forwards user input. Controllers only move field values in/out and render state — they hold no business logic.
- **ViewModel:** owns validation and calls domain services; returns a `Result` (success/failure + message). Exposes `IsBusy`, `ErrorMessage`.
- **Services:** injected by `AppCompositionRoot` (Supabase-backed identity, education, assignments, social, locations, files).

### 1.1 Screen inventory

| # | Screen | UXML | Controller | ViewModel |
|---|--------|------|------------|-----------|
| 1 | Landing | `LandingReal.uxml` | `LandingRealController` | — (navigation only) |
| 2 | About Us | `AboutUsReal.uxml` | `AboutUsRealController` | — (navigation only) |
| 3 | Contact Us | `ContactUsReal.uxml` | `ContactUsController` | `ContactUsViewModel` |
| 4 | Register | `RegisterReal.uxml` | `RegisterPageController` | `RegisterViewModel` |
| 5 | Login | `LoginReal.uxml` | `LoginPageController` | `LoginViewModel` |
| 6 | Forgot Password | `ForgotPasswordReal.uxml` | `ForgotPasswordController` | `ForgotPasswordViewModel` |
| 7 | OTP | `OTPReal.uxml` | `OTPController` | `OtpViewModel` |
| 8 | Reset Password | `ResetPasswordReal.uxml` | `ResetPasswordController` | `ResetPasswordViewModel` |
| 9 | Profile | `ProfileReal.uxml` | `ProfilePageController` | `ProfileViewModel` |
| 10 | Student Home | `StudentHomeReal.uxml` | `StudentHomeController` | `StudentHomeViewModel` (+ feature VMs) |
| 11 | Guardian Home | `GuardianHomeReal.uxml` | `GuardianHomeController` | `GuardianHomeViewModel` |
| 12 | Tutor Home | `TutorHomeReal.uxml` | `TutorHomeController` | `TutorHomeViewModel` (+ Academy VM) |

---

## 2. Global / cross-cutting requirements (GEN)

- **GEN-1 — Single-page navigation.** Exactly one screen is visible at a time. `NavigationManager` activates the target page GameObject and deactivates all others. The app opens on the **Landing** screen at startup.
- **GEN-2 — Guarded navigation.** If a navigation target is not assigned, the app stays on the current screen and logs an error rather than showing a blank screen.
- **GEN-3 — Re-entrant views.** Every controller re-registers its callbacks each time its screen is shown (`OnEnable`), because the visual tree is rebuilt on re-activation. Input handlers must never be assumed to persist across a navigation away-and-back.
- **GEN-4 — Placeholder (demo) mode.** When domain services are not wired (no `AppCompositionRoot`), each ViewModel still passes validation and returns success, so the flow proceeds with sample data. This mode must remain functional for design review in-editor.
- **GEN-5 — Busy guard.** While an async action is in flight (`IsBusy`), repeat taps of the submitting control are ignored (no double-submit).
- **GEN-6 — Error surfacing.** Validation and service failures are shown in an inline error label on the screen; the label is hidden again on the next successful action or step change.
- **GEN-7 — Role-based post-login routing.** After a successful sign-in or sign-up, `PostLoginRouter` reads the user's role and routes: Student→Student Home, Guardian→Guardian Home, Tutor/Teacher→Tutor Home, Administrator→Profile. If the role can't be loaded (offline/error), it falls back to **Student Home** rather than trapping the user on the auth screen. In placeholder mode (no router), auth screens fall back to the Profile screen.
- **GEN-8 — Email persistence.** The last email used to sign in is remembered on the device and pre-filled on next launch of the Login screen.
- **GEN-9 — Field theming.** Text inputs are re-skinned at runtime (dark field, light text) so the built-in theme does not render invisible white-on-white boxes.
- **GEN-10 — Validation rules** are centralised in `IdentityValidation` (email format, password policy, OTP length) and applied consistently across screens.

---

## 3. Public / pre-authentication screens

### 3.1 Landing (`LandingReal.uxml`)

**Purpose:** Marketing entry point and dispatch to sign-up / sign-in.

**Controls:** wordmark, paginated feature carousel (4 pages: Welcome, Scheduled reading, Multiplayer reading, Understanding), pager `‹ / ›` + dots, `Start free trial`, `Sign in`, `About Us`, `Contact Us`.

- **FR-LAND-1** — The screen shall present a horizontally paginated carousel of 4 informational pages, one visible at a time, with no vertical scrolling ("game-style" fixed layout).
- **FR-LAND-2** — `‹` / `›` and the dot indicators shall move between pages; the active page's dot is highlighted.
- **FR-LAND-3** — `Start free trial` shall navigate to **Register**.
- **FR-LAND-4** — `Sign in` shall navigate to **Login**.
- **FR-LAND-5** — `About Us` shall navigate to **About**; `Contact Us` shall navigate to **Contact**.
- **FR-LAND-6** — The screen shall communicate that sReader is a paid service with a free trial.

### 3.2 About Us (`AboutUsReal.uxml`)

**Purpose:** Explain the product across multiple paginated info pages.

**Controls:** `‹ Back`, paginated content, `Get started`, `Contact Us`.

- **FR-ABOUT-1** — The screen shall present paginated informational content (paged like the Landing carousel).
- **FR-ABOUT-2** — `Back` shall return to **Landing**.
- **FR-ABOUT-3** — `Get started` shall navigate to **Register**.
- **FR-ABOUT-4** — `Contact Us` shall navigate to **Contact**.

### 3.3 Contact Us (`ContactUsReal.uxml`)

**Purpose:** Let a visitor send a support/enquiry message.

**Controls:** `Back`, fields `Full name` / `Email` / `Subject` / `Message`, `Send`, `Get started`, error label, success label.

- **FR-CONTACT-1** — The user shall be able to enter full name, email, subject and message.
- **FR-CONTACT-2** — On `Send`, the screen shall validate: name is required, email must be valid, message is required. The first failing rule's message is shown.
- **FR-CONTACT-3** — On successful send, a confirmation message ("Thanks! Your message has been sent…") shall be shown and the error label hidden.
- **FR-CONTACT-4** — Submission shall be routed to the support service (`ISupportService.SendMessageAsync`); subject is optional.
- **FR-CONTACT-5** — `Back` returns to **Landing**; `Get started` navigates to **Register**.
- **FR-CONTACT-6** — Repeat `Send` taps while sending shall be ignored.

---

## 4. Authentication screens

### 4.1 Register (`RegisterReal.uxml`) — 3-step wizard

**Purpose:** Create a new account with a chosen role.

**Controls:** `Back`, role segmented control (`Student` / `Guardian` / `Tutor`), 3 steps — **Step 0:** first name, last name, email; **Step 1:** password, confirm password; **Step 2:** Terms toggle + `Create account`; pager `›`; `Sign in` link; Google / Apple buttons; error label.

- **FR-REG-1** — The form shall be a 3-step wizard; `›` advances only when the current step is valid.
- **FR-REG-2 — Step 0 validation:** first and last name required; email must be valid.
- **FR-REG-3 — Step 1 validation:** password must meet the password policy (`IdentityValidation.ValidatePassword`); confirm password must match.
- **FR-REG-4 — Step 2:** the user must accept the Terms of Service before the account is created.
- **FR-REG-5 — Role selection.** The user shall select exactly one role (Student default). Selected role is shown as a filled ("parchment") button; others outlined. The chosen role is submitted with registration.
- **FR-REG-6** — A stale error message shall not carry over when the user changes step.
- **FR-REG-7** — On `Create account`, all validations run again and registration is submitted (`auth.RegisterAsync` with email, password, first/last name, role).
- **FR-REG-8** — On success **with** a live session (email confirmation disabled), the user is routed to their role-specific home (GEN-7). If email confirmation is required (or in placeholder mode), the user is sent to **Login**.
- **FR-REG-9** — `Back` returns to **Landing**; `Sign in` navigates to **Login**.
- **FR-REG-10** — Google / Apple buttons are present as social sign-up entry points (currently log-only placeholders).

### 4.2 Login (`LoginReal.uxml`)

**Purpose:** Authenticate an existing user.

**Controls:** `‹ Back`, email, password (masked), `Forgot password?`, `Sign in`, Google / Apple, `Create one`, error label.

- **FR-LOGIN-1** — The user shall enter email and password and tap `Sign in`.
- **FR-LOGIN-2 — Validation:** email must be valid; password must be non-empty. Failing validation shows the inline error.
- **FR-LOGIN-3** — On success the email is remembered for next launch (GEN-8) and the user is routed to their role-specific home (GEN-7); in placeholder mode the Profile screen is shown.
- **FR-LOGIN-4** — On failure the service error message is shown inline.
- **FR-LOGIN-5** — The email field is pre-filled from the remembered email if the field is empty.
- **FR-LOGIN-6** — `Back` returns to **Landing**; `Create one` navigates to **Register**; `Forgot password?` navigates to **Forgot Password**.
- **FR-LOGIN-7** — Google / Apple buttons are present as social sign-in entry points (currently log-only placeholders).
- **FR-LOGIN-8** — Repeat `Sign in` taps while signing in shall be ignored.

### 4.3 Forgot Password (`ForgotPasswordReal.uxml`)

**Purpose:** Start the password-reset flow by requesting a code.

**Controls:** `Back`, email, `Send code`, `Sign in` link, error label.

- **FR-FORGOT-1** — The user shall enter their email and tap `Send code`.
- **FR-FORGOT-2 — Validation:** email must be valid.
- **FR-FORGOT-3** — On success, a reset code is requested (`auth.RequestPasswordResetAsync`) and the user advances to the **OTP** screen.
- **FR-FORGOT-4** — `Back` and `Sign in` both return to **Login**.

### 4.4 OTP (`OTPReal.uxml`) — 6-digit code entry

**Purpose:** Verify the emailed reset code.

**Controls:** `Back`, six single-digit boxes `otp-0..otp-5`, `Verify`, `Resend`, `Sign in` link, error label.

- **FR-OTP-1** — The screen shall present six single-character digit boxes.
- **FR-OTP-2 — Auto-advance:** typing a digit moves focus to the next box; only digits are accepted and only the last character typed is kept per box.
- **FR-OTP-3 — Backspace behaviour:** pressing Backspace on an empty box moves focus to the previous box.
- **FR-OTP-4 — Validation:** all 6 digits must be entered before verification (`IdentityValidation.OtpLength`).
- **FR-OTP-5** — On `Verify` success (`auth.VerifyResetCodeAsync`), the user advances to **Reset Password**.
- **FR-OTP-6 — Resend:** clears all boxes, refocuses the first, and re-requests the code.
- **FR-OTP-7** — `Back` returns to **Forgot Password**; `Sign in` navigates to **Login**.

### 4.5 Reset Password (`ResetPasswordReal.uxml`)

**Purpose:** Set a new password after code verification.

**Controls:** `Back`, new password + Show/Hide toggle, confirm password + Show/Hide toggle, `Update password`, `Sign in` link, error label.

- **FR-RESET-1** — The user shall enter a new password and confirmation.
- **FR-RESET-2 — Show/Hide toggles** shall switch each password field between masked and plain text (button label flips Show↔Hide).
- **FR-RESET-3 — Validation:** new password must meet the password policy; confirmation must match.
- **FR-RESET-4** — On success (`auth.CompletePasswordResetAsync`), the user is returned to **Login**.
- **FR-RESET-5** — `Back` and `Sign in` both return to **Login**.

---

## 5. Profile (`ProfileReal.uxml`)

**Purpose:** Signed-in landing / account screen — view and edit identity, profile and location; change avatar; sign out.

**Controls:** `Back`, tappable avatar, greeting + account line, **view card** (display name, username, bio, country·timezone, location summary), **edit card** (display name, username, bio, country, timezone, location country, province, city, address), `Edit`, `Cancel`, `Save`, `Sign out`, error + status labels.

- **FR-PROF-1** — On open, the screen shall load the current user, their profile and saved location and display them read-only. Empty values render as "—".
- **FR-PROF-2** — The greeting shall read "Welcome, {display name}" (or "Welcome" if none); the account line shows "{email} · {role}".
- **FR-PROF-3 — Edit/View toggle:** `Edit` shows the editable card pre-filled with current values; `Cancel` returns to the read-only view.
- **FR-PROF-4 — Save** shall persist edits across three stores in order — account (display name, username), profile (bio, country, timezone), location (country, province, city, address) — and show "Profile synced." on success. Any store's failure shows its error and stops.
- **FR-PROF-5 — Change avatar:** tapping the avatar picks an image, uploads it to the `avatars` bucket, saves the URL on the profile, and updates the displayed avatar. A cancelled pick is silent; other failures show an error.
- **FR-PROF-6 — Back behaviour:** if the edit card is open, `Back` first returns to the read-only view; otherwise it routes to the user's role-specific home (GEN-7), or Landing in placeholder mode.
- **FR-PROF-7 — Sign out** shall end the session and return to **Landing**.
- **FR-PROF-8** — In placeholder mode the screen shows sample data ("Reader") and Save reports "Saved (placeholder mode)."

---

## 6. Home screens (post-login)

Three role-specific homes share the greeting/identity + sign-out pattern. **Student** and **Tutor** use a richer "shell" (`ShellHomeController`) with bottom navigation, top sub-tabs and swappable content; **Guardian** uses the simpler `RoleHomeController`.

### 6.0 Shared home behaviour (SHELL) — Student & Tutor

- **FR-SHELL-1 — Bottom navigation** selects a section; the active nav button is highlighted (parchment vs muted).
- **FR-SHELL-2 — Top sub-menu** shows the selected section's sub-tabs; the active tab is underlined; selecting a tab swaps the centre content.
- **FR-SHELL-3 — Identity:** greeting shows "Welcome, {short name}" (username, else first name); the user's uploaded avatar is loaded into the profile button when present.
- **FR-SHELL-4 — Profile shortcut** opens the shared Profile screen; **Sign out** ends the session and returns to Landing.
- **FR-SHELL-5 — Offline banner:** a thin top banner appears whenever the device is offline (user viewing cached data); it is polled (~every 3 s) and clears itself when connectivity returns.
- **FR-SHELL-6 — System/Android back:** back first lets the active section pop its own in-content screen; else jumps to the first (home) section; else requires a second press within 2 s to exit (with a "Press back again to exit" toast).
- **FR-SHELL-7 — Cross-section shortcuts:** the app may deep-link to another section and optionally a specific sub-tab.
- **FR-SHELL-8** — Sections without dedicated content render placeholder cards ("Content for this section is coming soon.").

### 6.1 Student Home (`StudentHomeReal.uxml`)

**Sections (bottom nav):** Home, Academies, Tutors, Assignments, Friends.

- **FR-SHOME-1 — Home dashboard** shall present a launchpad: a **hero** action (resume an in-progress attempt, else open the soonest-due assignment, else "Find a class"), a **stats row** (Due soon / Scheduled / Completed counts), quick chips (My classes, My subjects) and quick links (Assignments, Friends, Academies, Profile). Tiles animate in; the screen is fixed (non-scrolling) with a game-style backdrop and mascot.
- **FR-SHOME-2 — Academies section** (tabs Discover / Enrolled): browse all academies and the ones joined; request to join.
- **FR-SHOME-3 — Tutors section:** list the student's tutors (staff at academies they're enrolled in), one card per tutor showing academy/grade and the subjects they teach the student. Empty state offers "Find a class".
- **FR-SHOME-4 — Friends section** (tabs Friends / Discover / Search / Requests): view friends, discover/search people, send/accept/decline/cancel friend requests, remove a friend, and view a person's profile.
- **FR-SHOME-5 — Assignments section:** aggregate assignments across all enrolled classes, each tagged with the student's own attempt status and schedule.
  - **FR-SHOME-5.1 — Filter/search:** filter by All / Due soon / Overdue / Submitted / Completed / Scheduled, by class, and by title text.
  - **FR-SHOME-5.2 — Attempt:** start an attempt, reset an attempt to replay (clearing the saved resume point), and submit.
  - **FR-SHOME-5.3 — Reading game:** play assignment content as a game; solo progress is saved per device so the student can leave and resume; the game result is recorded as a submission.
  - **FR-SHOME-5.4 — Multiplayer:** host or join a reading-game session (by code, from an open-sessions list, or a listed session), with a lobby, live presence/heartbeat, turn-based board, live event stream, and leave. Backend is selectable (Supabase Realtime or Photon) and remembered per device.
  - **FR-SHOME-5.5 — Scheduling:** schedule/reschedule an assignment for a chosen time with a note, toggle a schedule hidden, and unschedule.
  - **FR-SHOME-5.6 — Offline download:** download an assignment to the device for offline access, and detect whether it's already downloaded.
  - **FR-SHOME-5.7 — Deep link:** opening an assignment from the Home dashboard shall open that assignment's detail directly within the Assignments section.
  - **FR-SHOME-5.8 — Empty state:** when the student has no assignments, offer a jump to Academies → Discover to enrol.

### 6.2 Tutor Home (`TutorHomeReal.uxml`)

**Sections (bottom nav):** Students, Academies, Guardians, Assignments.

- **FR-THOME-1 — Students section** (tabs My students / Requests): the learners taught and pending requests.
- **FR-THOME-2 — Academies section** (tabs My academies / All / Requests) — full CRUD:
  - **FR-THOME-2.1** — Create, edit and delete academies (name, description, city, country).
  - **FR-THOME-2.2** — Manage grades (add/delete per stage) and courses/subjects (add/edit/delete with price, time frame, free-trial flag, deadline).
  - **FR-THOME-2.3** — Approve or reject student join requests.
  - **FR-THOME-2.4 — Payment details:** a `Payments` action lets the tutor view/save payout details (account name, FNB, Orange).
  - **FR-THOME-2.5 — Paid enrolment:** review enrolment requests, view submitted payment proof, and confirm or reject enrolment.
  - **FR-THOME-2.6** — A `＋ Create` action is available in the Academies top menu.
- **FR-THOME-3 — Guardians section** (tabs All / Messages): parents/guardians of the tutor's students.
- **FR-THOME-4 — Assignments section** (tabs Active / Drafts / Submissions): create/edit/delete assignments for a class (title, instructions, due date, max score), author assignment content, and review submissions.

### 6.3 Guardian Home (`GuardianHomeReal.uxml`)

**Purpose:** Oversee students in the guardian's care. (Simpler home — placeholder quick actions pending data features.)

- **FR-GHOME-1** — The screen shall greet the guardian and show the "Guardian dashboard" title and tagline.
- **FR-GHOME-2** — Quick actions shall be present: **My students**, **Progress**, **Link a student** (currently log-only placeholders to be wired to guardian features).
- **FR-GHOME-3** — Profile shortcut and Sign out behave per FR-SHELL-4; the offline banner per FR-SHELL-5.

---

## 7. Open items / notes for the spec

- **Social login** (Google/Apple) on Login and Register is UI-only (logs a click); no OAuth is wired yet.
- **Guardian home** actions and several Student/Tutor sub-tabs (e.g. Students, Guardians, Messages) currently render placeholder cards until their data features land.
- **Multiplayer reading game** (hosting, turns, presence, Photon/Supabase backends, resume) is substantial functionality embedded in the Student Assignments module rather than a standalone screen.
- All screens depend on `NavigationManager` page GameObjects being assigned in the scene; unassigned targets are no-ops with an error log (GEN-2).
