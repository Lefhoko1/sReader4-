# sReader Reading House: Unity handoff (v03, before export)

**Source file:** `sReader_Landscape_1520_Refined_v03_Claude.blend`

These earlier files are preserved and untouched:
- `..._Refined_v02_Claude.blend`
- `..._BEFORE_ClaudeRefine.blend`
- Codex's `..._Refined.blend`

Target: Unity 6 URP, landscape 1520 × 720, Huawei Y5 (2019) with a PowerVR
GE8300 GPU.

---

## 1. Word stone (optimised)

| | Before | After |
|---|---|---|
| Triangles per stone | 2,336 | **756 (−68%)** |
| Stone body (cap / centre / cap) | ~1,800 | 288 / 52 / 288 |
| Moss base (cap / centre / cap) | ~530 | 56 / 16 / 56 |
| Visible seams between the parts | Yes | No: the caps are built by sweeping the centre's own cross-section, so the joins match exactly |
| 20-word sentence | 46.7k | **15.1k** |

**Construction for Unity:**

| Measurement | Value |
|---|---|
| Height | 0.40 |
| Depth | 0.26 |
| Moss base | 0.085 high |
| Caps | **0.20** wide each (was 0.18), pivot at the seam |
| Centre piece | 1.0 wide at scale 1; scale X to (stone width − 0.40) |
| Minimum stone width | 0.50 |

- **Text anchors:**
  - `SR_STONE_TEXT_ANCHOR`: front face, for the room and front views
  - `SR_STONE_TEXT_ANCHOR_TOP`: use this one for the top-down stones camera, so
    labels sit on the visible face
- All parts have UVs. Material slots 0–5 are the stone states.
- **Draw calls:** do not leave a stone as 6 separate renderers (20 stones would
  be 120 draw calls). Either:
  - combine the 6 parts into one mesh per stone when it spawns
    (`Mesh.CombineMeshes`), or
  - use GPU instancing: there are only 3 unique stone meshes and 3 moss meshes,
    so all stones sharing a state cost about 6 draw calls.

---

## 2. Scene budget (measured in Blender, render-visible objects)

| Group | Objects | Triangles | Draw calls (no batching) | Materials |
|---|---|---|---|---|
| Architecture + interior (inside view) | 121 | 22,048 | 155 | 21 |
| Dashboard | 30 | 31,516 | 30 | 10 |
| Decor (chairs, rug, plants, lights) | 37 | 14,733 | 37 | 7 |
| Characters (3) | 68 | 54,911 | 68 | 15 |
| Word stones, 20-word sentence (optimised) | 120 | 15,120 | 120 | 2 |
| **Total** | **376** | **138,328** | **410** | **50 unique** |

The front wall and exterior pieces hidden for the cutaway view add another
10,956 triangles. They are behind the gameplay camera.

**Expected Unity draw calls:**
- **Before batching:** about 410 (one per renderer per material).
- **After the recommended setup:** about **55–75**:
  - architecture, decor and dashboard marked static → static batching groups
    them per material, about 35 batches
  - each character combined into one mesh with one atlas material → 3
  - stones instanced → about 6
  - TextMeshPro labels share one font material → 1–3
  - display and illustration → 2

**Remaining over budget (recommendations; not done):**

| Item | Current | Target | Suggested fix |
|---|---|---|---|
| Characters | 54.9k | about 24–30k | Decimate to about 8–10k each; replace curve laces and brows with low-poly mesh or texture |
| Dashboard | 31.5k | about 23k | Remove hidden back and inner faces at export (−25–30%) |
| Total | 138k | about 85–95k | |

**Do not claim device FPS until the game is tested on the Huawei Y5.**

---

## 3. URP material specification

Use **URP Lit** unless noted. Smoothness = 1 − Blender roughness. Clear coat is
not used: URP Complex Lit supports it, but it costs too much on the GE8300.
Instead, raise smoothness by about 0.1 to keep the glossy toy-plastic look.

### Dashboard (new set)

| Material | Base colour | Metallic | Smoothness | Emission | Notes |
|---|---|---|---|---|---|
| SR_MAT_Plastic_DeepBlue | #185CC8 | 0 | 0.80 | — | Blender roughness noise is procedural, not exported |
| SR_MAT_Plastic_Navy | #102C6C | 0 | 0.55 | — | Low gloss on purpose, to avoid mirror reflections |
| SR_MAT_Plastic_Cyan | #46C8F0 | 0 | 0.82 | #46D2FA × 0.35 | Tray glow |
| SR_MAT_Plastic_Orange | #F07610 | 0 | 0.78 | — | |
| SR_MAT_Plastic_Purple | #683AD6 | 0 | 0.80 | — | Play with Others |
| SR_MAT_Plastic_PurpleHi | #8A5CF0 | 0 | 0.88 | — | Button sheen strip |
| SR_MAT_Trim_Gold | #FFBA1E | 0.3 | 0.85 | — | Glossy yellow, not chrome |
| SR_MAT_Housing_Cream | #FCF8EE | 0 | 0.75 | — | Assignment capsule |
| SR_MAT_Light_Warm | #FFE696 | 0 | 0.70 | #FFD678 × 6 (HDR) | Accent lights; bloom picks them up |
| SR_MAT_Display_Blank | #E2F0F6 | 0 | 0.40 | — | Placeholder; runtime content draws over it |
| SR_MAT_Moss | #4F9A3A (average of the ramp) | 0 | 0.25 | — | **Must be baked** (see below) |
| SR_MAT_Wall_Interior_Warm | #F7CE9C | 0 | 0.25 | — | |
| SR_MAT_Ceiling_Warm | #FAE2BE | 0 | 0.20 | — | |

### Stone states (URP Lit; swap the material at runtime, or use one material with `_BaseColor` set per state)

| State | Normal | Selected | Correct | Completed | Deferred | Disabled |
|---|---|---|---|---|---|---|
| Colour | #F7F2DC | #FFD35D | #A9E989 | #9BD8D0 | #E6BCF3 | #969FA2 |

Smoothness is 0.6 for every state. For Selected and Correct, add emission
(×0.3) so the stone glows.

### Characters and architecture
Every `SR_REF_*`, `SR_Book_*`, wood, stone and gold material is a **plain
constant colour**, with no procedural nodes. Copy them directly using the same
rule: base colour hex, smoothness = 1 − roughness, metallic as set.

A few have emission:
- `SR_REF_light`: #FFF0AC × 3
- `SR_Amber_Glass`: #FFD398 × 1.6

---

## 4. Textures: what must be baked or exported

| Item | Status | Action |
|---|---|---|
| `preview_landscape.png` (2172 × 724) | Image, **preview only** | Export as a *sample* illustration texture (compress to 2048 × 683, ASTC 6×6). It is assigned at runtime to `SR_DISPLAY_ILLUSTRATION`. |
| SR_MAT_Moss (noise colour ramp + bump) | **Procedural, will not transfer** | Bake to a tiling 256² albedo + normal, then apply in **world-space / triplanar UVs** (Shader Graph). The stretched centre piece makes ordinary UVs stretch. Cheaper alternative: flat #4F9A3A plus vertex-colour variation. |
| Dashboard plastics (noise → roughness) | **Procedural, will not transfer** | Drop it. Use constant smoothness from the table. It is too subtle to matter on a 720p phone. |
| SR_PREVIEW_ONLY_Landscape (emission shader) | Preview shader | Not exported. In Unity, use URP Unlit or Lit with the illustration texture. |
| All other materials | Constant colour | Nothing to bake. |

There are no other image textures in the export set.

---

## 5. Runtime-content verification

I checked every text object in the file:

- **Dynamic content: none is in the export set.** All of the following live
  only in preview or demo collections (`PREVIEW_HUD`, `PREVIEW_Stones`,
  `SR_Demo`, `SR_PREVIEW_*`, the backups) and must not be exported:
  - assignment name ("Photosynthesis") and progress ("Sentence 2 of 10")
  - passage and sentence text
  - stone word labels
  - points ("120", "Your Points")
  - "Menu" and "Play with Others" labels
  - encourager and zombie dialogue
- **Speech bubbles** (`SR_*_BODY_bubble`) are empty meshes with no text, and
  their anchors (`SR_ANCHOR_*_SPEECH`) are empties.
- **Text-like meshes in the export set** (`SR_PANEL_POINTS`, its trim,
  `SR_Points_Star`) are housings and icons only.
- **Fixed decorative labels in the export set.** These never change, which the
  brief allows. Confirm you want them kept:
  - `SR_Brand` "sReader" on the dashboard nameplate
  - `SR_Library_Wordmark` "s R e a d e r" on the exterior
  - `SR_Library_Motto` "WORDS RESTORE WORLDS". It is hidden; exclude it from
    export, or confirm.
- **No image texture contains text.**

**Export rule (for the export step):** export only the collections
`SR_Console` (minus FONT objects except `SR_Brand`), `SR_StoneSource`,
`SR_Characters`, `SR_Decor`, `Architecture`, `Interior` and `Landscaping`,
render-visible objects only. Never export `PREVIEW_*`, `SR_Demo`,
`SR_PREVIEW_*`, `*BACKUP*` or `*_OLD`.

---

## 6. Zombie expression

The face now reads as tired and doubtful:
- heavy, droopy upper eyelids (new meshes `SR_ZOMBIE_HEAD_lid_L` / `_R`)
- worried brows with the inner ends lifted and one brow raised (sceptical)
- soft under-eye bags
- a lopsided, deflated frown
- the eye sparkles were removed

Position, hair, outfit, colours and the hand-on-chin pose are unchanged.

`Character_ZOMBIE.blend` (the separate asset file) was **not** updated. The
main scene file is the source for export.

## 7. Renders (`claude_refine/`, 1520 × 720)

| File | Shows |
|---|---|
| `01_dashboard_front` | Dashboard front |
| `02_three_quarter` | Three-quarter view |
| `03_closeup_screen_corner_gold_trim` | Screen corner and gold trim |
| `04_closeup_header_multiplayer` | Header row and multiplayer button |
| `05_closeup_tray_lower_controls` | Tray and lower controls |
| `06_classroom_all_characters` | Whole classroom with all three characters |
| `07_stones_long_sentence_photosynthesis` | Long sentence, two rows |
| `08_stones_short_sentence` | Short sentence |
| `09a_stone_closeup_BEFORE_2336tris` / `09b_stone_closeup_AFTER_756tris` | Stone before and after |
| `10_stones_twenty_word_sentence` | 20-word sentence |
| `11_zombie_expression_closeup` | Zombie face |
