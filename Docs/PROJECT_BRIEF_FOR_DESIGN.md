# sReader — Project Brief for Design Help

I'm building **sReader**, a reading-comprehension app for school-age students,
made in Unity 6 (URP, Android first). I'd like your help **designing what to make
next**, especially the student-facing reading game and its 3D world. Below is
what the app already does, what it looks like, what isn't working, and the
constraints any design has to fit.

---

## 1. What sReader is

A paid learning platform (with a free trial) that connects **tutors**,
**students** and **guardians**:

- **Tutors** run *academies* (like small online schools). They create grades and
  subjects, set prices, accept students, and **write reading assignments**.
- **Students** join academies, receive assignments, and complete them by
  **playing a reading game** built from the assignment text.
- **Guardians** (parents) keep an eye on their children's progress.

Backend: Supabase (accounts, data, file storage). Live multiplayer: Photon or
Supabase Realtime.

## 2. The core learning activity (this must not change)

A tutor pastes a passage and marks certain **key words**. Each key word gets
one of three activities:

| Activity | What the student does |
|---|---|
| **Define** | Put the shuffled words of the definition back in order |
| **Fill the blank** | Put the scrambled letters in order to spell the word |
| **Illustrate** | Pick the picture that matches the word from a few choices |

Rules already built and tested:
- The full passage can always be read. The student meets each key word in
  context, in passage order.
- A student can put a word off with "Later", but every word has to be solved
  before the assignment can finish. There is no skip.
- **Hints:** hint 1 costs 40% of that word's points, hint 2 costs 70%, and a
  "guided solve" gives 0 points but still counts as done. Every student can
  always finish; hints only lower the score.
- An optional timer adds pressure but never forces a skip. When it runs out,
  the word moves to the end.
- Score = the points left on each word, scaled to the tutor's maximum mark. The
  submission only goes in once everything is solved.
- Each word is graded **Gold** (no hints), **Silver** (hints) or **Bronze**
  (guided solve).
- Wrong answers are never punished with red, buzzers or an X. The piece wobbles
  and floats back, and a soft "not yet" tone plays. Right answers get gold ink,
  a rising chime and drifting sparkles.

## 3. The screens that exist today (UI Toolkit)

All app screens follow one dark design system:

- **Colours:** background `#121419`, panels `#232733`, hairlines `#2E3340`,
  **parchment accent `#E9DFC8`**, text `#F2EFE6`, muted text `#98A0AE`
- **Shape:** 12–14px corner radius
- **Layout:** three zones (header with a bookmark ribbon and wordmark, body,
  bottom bar about 9% of the screen)

| Screen | What it does |
|---|---|
| Landing | 4-page swipe carousel (Welcome, Scheduled reading, Multiplayer reading, Understanding), plus Start free trial and Sign in |
| About / Contact | Paged info screen; contact form |
| Register | 3-step wizard: choose a role (Student, Guardian or Tutor), enter name and email, set a password, accept terms |
| Login / Forgot / OTP / Reset | Full password flow, including a 6-digit code |
| Profile | View and edit name, username, bio, country, timezone, location and avatar photo |
| **Student Home** | Bottom tabs: **Home · Academies · Tutors · Assignments · Friends** |
| Tutor Home | Bottom tabs: Students · Academies · Guardians · Assignments |
| Guardian Home | Simple dashboard; its actions are still placeholders |

**Student Home → Home tab:** a "hero" card (resume your current assignment, or
start the next one due), counts for Due soon / Scheduled / Completed, quick
chips, and an animated reading-boy mascot.

**Student Home → Assignments tab:** filter by status, class or title. From here
a student can start, resume, restart or submit an assignment; schedule it for a
time; download it for offline use; or **play it with friends** (host or join by
code, lobby, turn-based shared board). There is also a "Quest Map" view that
shows assignments as islands along a dotted trail.

**Friends tab:** friends, discover, search, requests.

**Tutor side:** create and manage academies, grades and subjects (with prices
and free trials), approve students, check payment proof, write assignments by
pasting text and tapping words to attach activities, and review submissions.

## 4. The reading game, as it stands

There are **several versions of the game**, and that is the main problem:

1. **The live version (what students actually play):** a flat 2D screen inside
   the app. The passage shows as text with key words underlined; tapping one
   opens its activity as a pop-up. It works fully, including multiplayer, saved
   progress, mastery tracking and a review pass at the end. **It feels like a
   mobile app, not a game.**
2. **3D prototypes** (not yet linked to real assignments):
   - An **open storybook** on a stand. Clicking a sentence makes its words fly
     off the page as glowing chips that land on **stepping stones across a
     river**. Key-word stones glow gold. Stepping to one opens its activity as
     floating tablets in the 3D world. Solved sentences get ticked off on the
     book.
   - An **island** with a "Great Library" building. The camera drifts in from
     the sea and you tap Enter.
   - A **modern pavilion/house** sample scene, where I put the book in the
     living room for a walk-around, first-person version. **I'm dropping it.**
     It doesn't feel like it's about reading, and it's heavy and hard to work
     with.
3. **"The Great Library of Knowledge"**: the planned direction, which hasn't
   been built yet. A hand-painted 3D library world. The fiction: "The
   Forgetting" has faded the library's words, and reading restores them.
   Shelves fill up and lights come back on as the student masters words ("the
   Library is the scoreboard"). Planned characters: an **Owl** that gives the
   hints and a **Book Spirit**. The three activities get story names: **Ink
   Weaving** (fill), **Definition Reforging** (define) and **Vision
   Restoration** (illustrate).

**Already available to build with:** a modular library kit (walls, shelves,
arches, columns, stairs, desk, chair, candles, lanterns, books, scrolls, globe,
inkpot), an open hero book, a dry/flowing fountain, a rune-sealed door, a
Lexicon pedestal, and a rigged Owl and Book Spirit with simple animations. The
feedback effects (gold ink, chimes, sparkles, the "page exhale" on completion)
are coded and work.

## 5. What I need from you

I'm replacing the pavilion with a **purpose-built reading space**. I like the
cosy *sitting-room* feel and want something like it that is clearly **a
library / reading room**. Please help me design:

1. **The reading room / library space:** its layout, the mood, where the book
   sits, where the word stones or activities happen, how the student moves
   (fixed camera, tap-to-move or free walk), and how it changes as the student
   makes progress.
2. **How one assignment plays out in 3D**, step by step: arrive, read the
   passage, meet a key word, do the activity, use hints, finish, celebrate.
   Keep the rules in section 2 exactly as they are.
3. **How the app connects to the world:** should the Student Home itself become
   a 3D space (for example, the library is the home and each assignment is a
   book on a shelf)? Or should the app stay a menu that launches the world?
4. **Progress you can see:** what grows or restores in the room across many
   assignments (shelves, lights, the fountain, the Lexicon of collected words).
5. **A visual direction** that fits the current colours (dark slate with
   parchment and gold), plus a few alternatives.
6. **Small and deep first:** what is the smallest version that would *feel*
   magical, which I could build and test before anything else?

## 6. Constraints any design must fit

- **Phones first** (Android, touch). It must run well on mid-range devices and
  work with taps; there is no keyboard or mouse-look on a phone.
- I'm **one developer**. I can model simple things in Blender and block out
  levels in Unity's ProBuilder. Fewer, reusable pieces beat lots of unique ones.
- Prove the gameplay first in plain grey blocks; the art comes after.
- The learning rules, scoring, multiplayer and backend stay as they are. The
  design is a new *presentation layer* on top.
- The audience is school-age children, so it should be warm and calm with no
  harsh failure feedback, and the text has to be readable.

Please start with your questions, then give me 2–3 distinct concepts for the
reading room and the in-world assignment flow, each with a rough sketch or
layout description.
