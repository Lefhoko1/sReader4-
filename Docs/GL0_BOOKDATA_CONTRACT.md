# GL-0 — The BookData Contract (frozen)

**Status:** frozen 2026-07-20. This is the Bible's task **F1** ("Freeze BookData
JSON schema") realised against the code that already exists. It governs the
boundary between the tutor product (content authoring) and the game (which only
*reads* content and *records* results — never edits content). Change it only
additively, and flag any change per `GREAT_LIBRARY_SPEC.md` §8.

## 1. What BookData *is* in this codebase

The Bible (Ch. 8.1) calls the per-book document **BookData**. In sReader that
document already exists — it is **`AssignmentContent`**, serialised to the
`content_json` column on an assignment. There is **no new authoring format**:

```
tutor authoring UI (AssignmentContentEditorView)
      │  writes
      ▼
Assignment.ContentJson  ("content_json" column, a JSON string)
      │  AssignmentContentCodec.FromJson  (JsonUtility)
      ▼
AssignmentContent  ──►  TrailBuilder.Build(id, title, content)  ──►  TrailData
```

- Model: `Assets/Scripts/Domains/Assignments/Content/AssignmentContent.cs`
- Codec: `AssignmentContentCodec.ToJson` / `FromJson` (Unity `JsonUtility`)
- Reader (student side): `StudentAssignmentsViewModel.ContentFor(assignment)` →
  `TrailBuilder.Build(...)`

**`JsonUtility` implications (do not violate):** field **names must match
exactly**; enums serialise as their **integer** value; every field is emitted
(no omit-if-default); `null` collections deserialise as empty. That is why the
sample books in §4 spell out every field and use integer `activity` values.

## 2. Field-by-field: Bible BookData ↔ `AssignmentContent`

| Bible BookData (Ch. 8.1) | Where it lives today | Notes |
|---|---|---|
| `bookId` | `Assignment.Id` (the wrapper, not the content) | content is keyed by its assignment |
| `title` | `Assignment.Title` | wrapper |
| `subject` | `Assignment` course/subject | wrapper; also drives Wing/biome |
| `coverRecipe` (base/ornament/sigil) | — **not yet** | **additive, GL-2** (Bible Ch. 6.4 cover generator) |
| `paragraphs[].text` | `AssignmentContent.pages[].sentences[].tokens[]` | a page ≈ a paragraph; passage text is the concatenation of every `token.text` (verbatim) |
| `keywords[].word` | interactive `ContentToken.text` (`isWord && activity != None`) | one keyword = one gate/ritual |
| `keywords[].definition` | `ContentToken.definition` | Definition Reforging + Word Kindling meaning |
| `keywords[].definitionShards[]` | derived at runtime (`AssignmentContentBuilder.ShuffleWords`) | not stored; recomputable |
| `keywords[].imageOptions[]` + correct index | `ContentToken.imageOptions[]` + `correctImage` | Vision Restoration |
| `keywords[].blankIndex` / `scrambledTiles[]` | derived at runtime (`AssignmentContentBuilder.ShuffleLetters`) | not stored; recomputable |
| `keywords[].ritualType` | `ContentToken.activity` (`ActivityType`) | see §3 |
| `keywords[].points` | `ContentToken.points` | base points before hint/timeout deductions (R-10) |
| `questions[]` (comprehension) | — **reserved** (R-12) | `GateType.Comprehension` modelled, not built |

**Provenance-only field:** `ContentPage.sourcePdf` (optional; a future PDF
importer can stamp it). `ContentPage.imageUrls` holds images lifted off a page
(not yet consumed by the game).

## 3. `activity` (the ritual type) — the enum contract

`ActivityType` (in `AssignmentContent.cs`) is the *stored* value; `JsonUtility`
writes it as an integer. `TrailBuilder.MapType` projects it to `GateType`, which
the presenter dresses as a Bible ritual:

| `activity` int | `ActivityType` | `GateType` | Bible ritual |
|:---:|---|---|---|
| `0` | `None` | — (not interactive) | plain passage text (Entering the Page) |
| `1` | `Define` | `Define` | **Definition Reforging** |
| `2` | `Illustrate` | `Illustrate` | **Vision Restoration** |
| `3` | `FillBlank` | `Fill` | **Ink Weaving** |

**Word Kindling** is not a separate stored type — it is the universal
tap-the-glowing-keyword entry that precedes any of the three challenges
(`GREAT_LIBRARY_SPEC.md` §3). **Entering the Page** is reading, not a keyword.

## 4. The two frozen sample books

Hand-authored reference content + placeholder-mode fodder. Pure
`AssignmentContent` JSON (no wrapper fields), so `AssignmentContentCodec.FromJson`
loads them unchanged.

| File (`Assets/Resources/Books/`) | Fiction / Wing | Gates (passage order) |
|---|---|---|
| `SampleBook_Nature_MeadowAtDawn.json` | Nature Wing — "The Meadow at Dawn" | `dew` (Ink Weaving), `habitat` (Definition Reforging), `butterfly` (Vision Restoration), `nectar` (Definition Reforging) |
| `SampleBook_Science_FirstMachines.json` | Science Wing — "The First Machines" | `machines` (Ink Weaving), `lever` (Definition Reforging), `friction` (Definition Reforging), `gear` (Vision Restoration) |

Each: 2 pages/paragraphs, 4 gates, exercises **all three** interactive rituals +
reading. Load path (for a future placeholder loader):
`Resources.Load<TextAsset>("Books/SampleBook_Nature_MeadowAtDawn")`.

Regeneration note: the tokenisation (word vs separator split on
letter/digit; sentence break on `.!?\n`) mirrors
`AssignmentContentBuilder.PageFromText`, so the reconstructed passage and gate
char-indices match what `TrailBuilder` computes.

## 5. Invariants (the freeze)

1. **Read-only for the game.** The game never edits content; it reads it and
   records results (marks/attempts/submissions/mastery). Marks are the source of
   truth; shards/tiles/stars are derived and recomputable.
2. **One keyword → one gate → one ritual,** in passage order (R-4/R-5).
3. **Additive changes only.** New fields (`coverRecipe`, `subject`,
   `questions[]`) are added when a phase needs them, defaulting safely so old
   `content_json` still deserialises. Never rename or repurpose an existing
   field.
4. **Malformed content is never fatal.** Missing/empty content →
   `TrailBuilder` text-only fallback (all reading, zero gates), so no assignment
   is unplayable.

## 6. Acceptance (in-editor)

`Assets/Tests/EditMode/SampleBookContractTests.cs` (run via **Window ▸ General ▸
Test Runner ▸ EditMode**) proves: both sample books deserialise via the codec;
round-trip (`ToJson`→`FromJson`) preserves gate count and types; `TrailBuilder`
builds 4 gates each in passage order with the ritual types in the table above;
and the reconstructed `PassageText` contains each keyword. Green = contract holds.
