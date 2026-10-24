# sReader Reading House: v04 corrections and Unity preparation

**Source file:** `sReader_Landscape_1520_Final_v04_Claude.blend`

These earlier files are preserved:
- v03 (the approved state)
- v02
- `BEFORE_ClaudeRefine` (backup)
- Codex's `Refined`

This document **supersedes `UNITY_HANDOFF_v03.md` where the two differ.** The
material specification and texture-bake list in v03 still apply. The new
materials (progress fill, icons) are listed in section 3.

**Nothing has been exported or imported into Unity.**

---

## 1. Stone module and text anchor

**`SR_STONE_PREFAB_SOURCE`** (pivot at bottom centre) has these children, shown
in the default layout of a 1.40 m stone:

| Child | Local X |
|---|---|
| `SR_STONE_CAP_LEFT`, `SR_STONE_MOSS_CAP_LEFT` | −0.5 (pivot at its seam) |
| `SR_STONE_STRETCH_CENTRE`, `SR_STONE_MOSS_STRETCH_CENTRE` | 0 (1.0 m at scale 1) |
| `SR_STONE_CAP_RIGHT`, `SR_STONE_MOSS_CAP_RIGHT` | +0.5 (pivot at its seam) |
| `SR_STONE_TEXT_ANCHOR` | 0; position (0, −0.136, 0.241) |
| `SR_STONE_TEXT_ANCHOR_TOP` | 0; position (0, −0.06, 0.43) |

**`SR_STONE_TEXT_ANCHOR`** is centred horizontally and vertically on the flat
front face. That face is 0.156 m tall, and the anchor sits 6 mm in front of it.

**The text never stretches.** Both text anchors are children of the **root**,
not of the stretching centre piece. The layout rule:

| Value | Rule |
|---|---|
| Stone width | max(0.50, text width + 0.20) |
| `centre.localScale.x` | stone width − 0.40 |
| Caps | at ±(stone width − 0.40) / 2 |
| Text anchor | never scaled |

These rules are also stored as custom properties on the root.

- Use `SR_STONE_TEXT_ANCHOR_TOP` with `CAM_GAMEPLAY_TRAY_FOCUS`, which looks
  down on the tray.
- 756 triangles per stone. Combine the parts per stone, or use GPU instancing.

## 2. Stone layout (wrapping), verified

**Usable tray width:** 6.30 m. The floor top is at z 1.29.

**Rows:** front row at y 0.95 (`SR_ANCHOR_TRAY_ROW_0`), back row at y 1.85
(`SR_ANCHOR_TRAY_ROW_1`). The rows moved apart from 0.6 to 0.9 m so the front
row no longer hides the back row's labels.

**Algorithm (the rule Unity should implement):**
1. If the sentence fits on one row, use the front row.
2. Otherwise split it into **two balanced rows**. **Line 1 goes on the back row**
   (the top of the screen), so the stones read top to bottom.
3. Use one uniform scale for the whole sentence, reduced from 1.0 to a minimum
   of 0.70 if needed. Below that, paginate.
4. Gap = 0.06 × scale. Each stone's footprint includes the moss overhang
   (0.044 × scale on each side).
5. Each row is centred.

| Sentence | Scale | Rows | Width | Overlaps | Inside tray |
|---|---|---|---|---|---|
| 5 words | 1.00 | 1 × 5 | 3.19 m | none | yes |
| 11 words | 1.00 | 6 + 5 | 3.84 / 3.28 m | none | yes |
| 20 words | 0.88 | 10 + 10 | 6.17 / 5.65 m | none | yes |

## 3. Header and button additions

- **Book icon:** an open book with a leaf. Its parts are
  `SR_HEADER_BOOK_ICON_Cover`, `_PageL`, `_PageR` and `_Leaf`. It is
  decorative.
- **Progress bar:**
  - `SR_HEADER_PROGRESS_TRACK` (navy)
  - `SR_HEADER_PROGRESS_FILL`, material `SR_MAT_Progress_Fill` (#46C45C, small
    emission). **Its pivot is the left end, so set `localScale.x` = progress
    (0–1).** The preview value is 0.2 (2 of 10).
  - Anchor: `SR_ANCHOR_PROGRESS_BAR`.
- **Header text anchors** (left-aligned): `SR_ANCHOR_ASSIGNMENT_HEADER` (title)
  and `SR_ANCHOR_SENTENCE_PROGRESS` ("Sentence n of N").
- **People icon:** two figures, `SR_BTN_MULTI_ICON_*` (cream), on the left of
  the purple capsule. The capsule itself is unchanged. The label anchor
  `SR_ANCHOR_MULTIPLAYER` moved right to x 2.30. New anchor:
  `SR_ANCHOR_MULTIPLAYER_ICON`.

## 4. Cameras

| Camera | Use |
|---|---|
| `CAM_ROOM_OVERVIEW` | Full classroom, for overview and transitions. Unchanged. |
| `CAM_GAMEPLAY_LANDSCAPE` | **Main gameplay view.** Position (0, −3.6, 3.8), 20 mm, aimed at (0, 1.8, 2.3). Shows the whole dashboard: header, screen, both stone rows, Points and Menu. |
| `CAM_GAMEPLAY_TRAY_FOCUS` | Closer view of the screen and tray for the arranging activities. Position (0, −1.8, 4.25), 18 mm. |

**The student must move to a gameplay spot for either gameplay camera.** His
hair reaches 2.87 m, so standing at the overview spot he crosses the front of
the tray from any camera height under the ceiling. Framing him out from behind
would need a lens wider than 14 mm, which distorts too much.

The fix is two anchors:
- `SR_ANCHOR_STUDENT_OVERVIEW`: his current spot.
- `SR_ANCHOR_STUDENT_GAMEPLAY`: (−2.4, −1.6), turned toward the screen.

When gameplay starts, Unity tweens him from the overview spot to the gameplay
spot.

Verified by ray-casting from both gameplay cameras to:
- 162 stone sample points
- 39 screen sample points
- 30 header and button sample points

**No point is blocked by any character.** He is completely outside the
tray-focus frame.

In the overview, two-row sentences partly overlap and the student covers the
middle. That is expected, because the overview is not used for interaction.

## 5. Optimised characters (`SR_Characters_Optimised`, suffix `_OPT`)

| Character | Original | Optimised |
|---|---|---|
| Student | 22,780 | ~10.5k (hair 13,024 → 4,500) |
| Encourager | 18,159 | ~12.4k (hair 5,164 → 3,500) |
| Zombie | 15,304 | ~8.5k |
| **Total** | **54.9k** | **~31.5k** |

- **The originals are untouched** in `SR_Characters`.
- **Facial features were excluded from decimation:** eyes, irises, the zombie's
  lids, brows, mouths, eye bags and teeth. The expressions are identical (see
  renders 17 and 18).
- **Hair:** the original's smooth normals were transferred onto the optimised
  hair. Very close up from behind it is slightly lumpier than the original
  (render 20a against 20b). At gameplay and overview distances the difference
  is not visible.
- The optional speech-bubble copies are hidden, the same as the originals.

## 6. Export set: `SR_EXPORT_UNITY` (hidden; for grouping only)

| Group | Objects | Triangles | Materials | Notes |
|---|---|---|---|---|
| EXPORT_Environment | 162 | 31.6k | 26 | 31 curve objects to convert to mesh at export |
| EXPORT_Dashboard | 64 | 33.9k | 12 | Includes `SR_DISPLAY_ILLUSTRATION` and `SR_Brand` (fixed "sReader") |
| EXPORT_StoneModule | 9 | 756 | 7 | 6 parts, 2 anchors, root |
| EXPORT_Characters | 81 | 31.5k | 15 | Optimised versions plus the two student anchors |
| EXPORT_Cameras | 7 | — | — | |

**Verification results** (`EXPORT_VERIFICATION_v04.json`):
- **0 empty material slots.**
- **0 unapplied scales.** Scale was baked into 65 objects. The only scaled
  object left is the progress fill, on purpose.
- **0 parents outside their export group.**
- **All 29 required objects present.**
- **The only text object exported is the fixed `SR_Brand`.** No preview text,
  stone labels, points, progress values or dialogue are in the export set.

**Scene total:** about 98k triangles for the static scene and characters, plus
about 15k for a 20-word sentence. Removing hidden back faces on the dashboard
at export takes off about 8k more. **Frame rate is unknown until the game is
tested on the Huawei Y5.**

## 7. Renders (`claude_refine/`, 1520 × 720)

| File | Shows |
|---|---|
| `06_classroom_all_characters` | Overview |
| `12_gameplay_camera_5_words` / `_11_words` / `_20_words` | Gameplay view at three sentence lengths |
| `13_overview_student_at_gameplay_spot` | Overview with the student at his gameplay spot |
| `14_tray_focus_20_words` | Tray-focus camera |
| `15_final_tray_closeup` | Final tray close-up |
| `16_overview_OPTIMISED_characters` | Overview with the optimised characters |
| `17_zombie_closeup_OPTIMISED` | Optimised zombie close-up |
| `18_encourager_closeup_OPTIMISED` / `_ORIGINAL` | Encourager close-up, optimised and original |
| `19_header_book_icon_progress_people_icon` | Header additions |
| `20a/20b_student_hair_ORIGINAL/OPTIMISED` | Student hair, original and optimised |
