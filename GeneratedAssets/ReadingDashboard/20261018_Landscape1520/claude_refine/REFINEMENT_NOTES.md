# Dashboard visual refinement — notes

**Files** (all in `GeneratedAssets/ReadingDashboard/20261018_Landscape1520/`):

| File | What it is |
|---|---|
| `sReader_Landscape_1520_Refined_v02_Claude.blend` | The refined version |
| `sReader_Landscape_1520_Refined.blend` | Codex's last version, untouched |
| `sReader_Landscape_1520_BEFORE_ClaudeRefine.blend` | Backup taken before any change |

Renders are in this folder, all at 1520 × 720.

## What changed
- **Dashboard rebuilt from rounded geometry.** Every part is a true
  rounded-rectangle outline, extruded and bevelled. The gold trims are
  continuous tubes that follow the rounded outline without breaks. The old
  box parts are kept, hidden, in `SR_BACKUP_BoxyDashboard`.
- **Parts:**
  - lower fascia with a recessed plinth
  - gold-rimmed tray with a recessed, softly glowing cyan floor and an orange
    front lip
  - screen tower with an orange outer frame and gold piping
  - recessed screen: blue bezel, gold tube trim, navy recess
  - capsule header (assignment panel), purple **Play with Others** capsule
    with a top sheen
  - gold nameplate over the top of the screen
  - Points and Menu capsules with gold trims and stars
  - gold-housed accent lights
- **Required names are kept:** `SR_DISPLAY_MAIN`, `SR_BUTTON_MULTIPLAYER`,
  `SR_BUTTON_MENU`, `SR_PANEL_POINTS`, `SR_Assignment_Housing`, `SR_Brand`
  (editable text) and every `SR_ANCHOR_*`. **All anchors were moved onto the
  new geometry.** New anchors: `SR_ANCHOR_TRAY_ROW_0` and
  `SR_ANCHOR_TRAY_ROW_1`.
- **Stone rebuilt** as a three-part pillow pebble with an individual lumpy
  moss base (see the stone layout table below).
- **Materials:** a separate dashboard set, so the characters are unaffected:
  - `SR_MAT_Plastic_DeepBlue`, `_Navy`, `_Cyan`, `_Orange`, `_Purple`,
    `_PurpleHi`
  - `SR_MAT_Trim_Gold`
  - `SR_MAT_Housing_Cream`
  - `SR_MAT_Stone_Pale`
  - `SR_MAT_Moss`
  - `SR_MAT_Light_Warm`
  - `SR_MAT_Display_Blank`

  They are glossy clear-coat plastic with slight roughness variation; gold is
  only lightly metallic.
- **Room:**
  - a closed warm ceiling with an orange cove (previously open to the sky)
  - warm peach on the inward-facing wall faces only (the exterior is still
    cream)
  - relit: warm key, top and fill lights, wall glows and a ceiling cove light
  - AgX Punchy look
- **Display:** `SR_DISPLAY_MAIN` and `SR_DISPLAY_ILLUSTRATION` both have clean
  0–1 UVs. The illustration is a replaceable material slot
  (`SR_PREVIEW_ONLY_Landscape`). The sentence banner and all text are preview
  objects only.

## Stone layout for Unity
| Measurement | Value |
|---|---|
| Height | 0.40 |
| Depth | 0.26 |
| Moss base | 0.075 high |
| Pivot | Bottom centre |
| Each cap | 0.18 wide, pivot at its seam |
| Centre piece | 1.0 wide at scale 1; scale X to (stone width − 0.36) |
| Minimum stone width | 0.48 |

- The text anchor is the front-face centre, `SR_STONE_TEXT_ANCHOR`.
- Material slots 0–5 hold the stone states: Normal, Selected, Correct,
  Completed, Deferred, Disabled.
- **Tray:** usable area 6.6 × 1.2 m; floor top at z 1.29. Row 0 is at y 1.08
  (the room camera only shows this row); row 1 is at y 1.68 (the stones
  close-up camera shows both rows). In the room view, use one row and page
  through long sentences.

## Cameras
| Camera | Type | Notes |
|---|---|---|
| `CAM_ROOM_OVERVIEW` | Perspective | Re-aimed |
| `CAM_DASHBOARD_CLOSEUP` | Orthographic | Scale 7.3; flat reading view, no distortion |
| `CAM_STONES_CLOSEUP` | Perspective | Was orthographic, which showed the ceiling as a dark band |
| `CAM_THREE_QUARTER` | Perspective | |
| `CAM_DETAIL_SCREEN_CORNER` | Perspective | Close-up camera |
| `CAM_DETAIL_HEADER` | Perspective | Close-up camera |
| `CAM_DETAIL_TRAY_CONTROLS` | Perspective | Close-up camera |
| `CAM_CHARACTER_VIEW` | Perspective | |

## Triangle counts
| Item | Triangles |
|---|---|
| Dashboard | ~31.5k (was 47k before the segment reduction) |
| One stone | ~2.3k (an 11-stone row is ~26k) |

More can be cut at export: delete the hidden back and inner faces of the
frames and the shell (estimated −25–30%), and use a cheaper stone level of
detail for the room view.

## Not done / open
- **No Unity export yet.** It is waiting for approval, as the brief asks.
- **Characters were not refined.** The zombie still has spiky purple hair and
  looks cross rather than tired.
- Some preview stone labels render in italics (a Blender font quirk). They are
  preview-only; Unity uses TextMeshPro.
- The procedural roughness variation and moss noise are Blender-only. For
  Unity, use the material colours plus URP smoothness, or bake them into
  small textures.
