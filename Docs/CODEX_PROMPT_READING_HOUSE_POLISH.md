# Codex task — Reading House: colour, lighting & aesthetic polish

You are polishing the look of an existing Unity scene. **Gameplay, layout and UI behaviour are finished and
signed off — this task is visual only: colour, lighting, materials, post-processing and atmosphere.** Read the
whole brief before changing anything.

## Project facts

- Unity **6000.4.9f1**, **URP**. Pipeline asset: `Assets/Settings/UniversalRP.asset`; the house camera uses
  renderer index 1 (`Assets/Settings/Renderer3D.asset`). A Huawei-tuned asset also exists:
  `Assets/Art/MagicalClassroom/URP_HuaweiY5.asset`.
- Scene: **`Assets/Scenes/SC_ReadingHouse_Integration.unity`** — a full 3D reading house (exported from
  Blender) with a garden outside, a big arcade-style "sReader" reading dashboard on the back wall, a chalkboard
  on the right wall, bookshelves, two chairs and a blue rug.
- Target device: **Huawei Y5 2019** (low-end Mali GPU), **landscape 1520 × 720**. The look must stay smooth on
  it: aim for 60 fps, never below 30.
- Today's lighting state: realtime only (1 directional + 2 point lights), **no baked GI, no light probes, no
  reflection probes, no post-processing** (the camera has post-processing off, and there is no Volume), with a
  flat warm ambient colour. That is why the scene looks flat and washed out.

## What the scene contains (do not break any of it)

| Element | Where / how |
|---|---|
| Student (2D sprite rendered from a 3D model, walks with joystick/WASD) | `Student 2D`, `StudentSpriteAnimator`, unlit sprite material `Assets/Art/ReadingHouse/Student2D/M_Student2D.mat` |
| Encourager (boy in blue cap) **seated** on the right-hand chair as you face the dashboard, always turned towards the student; cheers and talks | `Encourager (seated)`, `ActorSpriteAnimator` + `EncouragerBehaviour` |
| Candy speech bubble (pink → peach → lemon gradient, white rim, white text with candy-pink outline, confetti) | `CandySpeechBubble` (UI Toolkit, screen space) |
| Boredom Zombie **seated** on the left-hand chair as you face the dashboard: bored, yawns, sometimes stands up, sighs and sits back down | `Zombie (seated)`, `ActorSpriteAnimator` |
| Dashboard display: illustration with the sentence in **white letters, navy outline, soft shadow — no white banner** | `ReadingHouseDashboard` ("Sentence look" settings) |
| Dashboard display as a game screen: reading toolbar, and the word activities and passage reader in a **navy / gold game palette** | `DashboardScreen : WorldUIScreen` (UI Toolkit into a RenderTexture on `SR_DISPLAY_MAIN`) |
| The sentence as **Word Path stones on the blue rug** in front of the student (gold Key stones for key words, a star when solved) | `MatStoneBoard`, prefabs `Assets/Prefabs/SM_WordStone_A/B/C/Key`, `Assets/Scripts/WordStones.cs` |
| Chalkboard showing the app's real pages (student home, academies, assignments, profile) | `ChalkboardScreen : WorldUIScreen`, `StudentHomeReal.uxml` |
| Camera rig: Overview, Gameplay (reading), TrayFocus (close-up of the display); follow camera while walking; the room "turns" to face the chalkboard | `ReadingHouseCameraRig`, `ReadingHouseFollowCamera`, `ReadingHouseExploreDirector` |
| Front-wall cut-away while the student is inside; frosted sign-in cards outside | `FrontWallCutaway`, `AuthCardLayout`, `FrostedBackdrop` |

## The owner's direction (follow it)

1. **It must feel like a game, not a mobile app.** No cards or panels floating over the 3D scene; UI lives on
   in-world surfaces (the dashboard display, the chalkboard).
2. **Colourful, warm, inviting — "cute", never dull or dark.** No black or grey drabness, no harsh pure white
   panels. The owner's words about the current screen: *"the colours don't look good."*
3. Dashboard text sits straight on the illustration (white, navy outline). **Never bring back a white box
   behind it.**
4. Words are the Word Path stones on the rug, and the key words must stand out (gold).
5. The Encourager's words appear in a **candy-style** bubble, and he only says encouraging things.
6. Motion must be smooth and elegant. The house stays perfectly straight (never tilted), and nothing is
   stretched, squashed or distorted.

## Your scope — what to improve

1. **Lighting.** Make it read as a warm, sunny storybook library:
   - sunlight through the arched windows (soft shafts are welcome)
   - warm pools of light from the wall lamps
   - gentle bounce light and soft shadows that ground the furniture.

   Prefer **baked** lighting (Progressive GPU, Mixed/Baked lights, lightmaps) plus a **Light Probe Group** over
   the walkable floor, the rug and the chairs, and a **reflection probe** for the room. Keep realtime lights to
   a minimum for the device.
2. **Grounding the 2D characters.** The three characters are camera-facing sprites with unlit materials; they
   must not look pasted on.
   - Add soft contact/blob shadows under the seated Encourager and Zombie (the student already has one).
   - Make sure the scene's grade suits them.
   - If you add lighting influence on the sprites, use material/shader parameters only: do **not** re-render or
     edit the sprite frames.
3. **Colour harmony and materials.** Build one cohesive warm palette across walls, floor, rug, bookshelves,
   chairs, chalkboard frame and the dashboard. In particular:
   - The dashboard's large **flat neon-cyan tray area** and the harsh orange frames look cheap. Make them
     richer: a softer aqua glow, amber/gold trim with sensible smoothness, subtle emissive accents.
   - Keep the dashboard's identity: arcade machine, navy panels, gold/orange trim, purple "Play with Others"
     pill.
   - Give materials consistent roughness/smoothness; no plastic-white or pure-black surfaces.
4. **Post-processing (mobile-safe).** Add a global URP Volume and turn post-processing on for the house
   camera:
   - tonemapping (Neutral or ACES)
   - colour adjustments (warm white balance, a touch more saturation and contrast)
   - **subtle** bloom (high threshold: lamps, windows, gold Key stones, the display bezel lights)
   - a light vignette.

   No SSAO, motion blur, depth of field, screen-space reflections or anything heavy on this device.
5. **Atmosphere (cheap).** Optional: a few dust motes in the window light, a soft glow on the gold Key stones
   and lamps, the warm haze of a sunny room. Keep particle counts very low.
6. **Outside the house** (the opening shot and the sign-in area): the same warm, sunny grade, so walking in
   feels continuous.
7. **Readability checks.** After grading, everything below must still read clearly:
   - the dashboard sentence (white/navy)
   - the navy/gold activity panels on the display (they are unlit UI in a RenderTexture, so check them after
     tonemapping and colour adjustments)
   - the stone words
   - the chalkboard pages
   - the candy bubble.

## Do NOT change (hard constraints)

- **No gameplay, logic or data changes.** Don't touch presenters, ViewModels, services, `TrekSession`,
  Supabase/Photon code, `AppCompositionRoot`, navigation, input, or the attempt/submission flow. All data must
  stay real database data: **never re-enable sample/mock content** (`ReadingHousePresenter.allowSampleContent`
  stays as the director sets it).
- **No UI Toolkit rework.** Don't edit any `.uxml`/`.uss` file or UI controller. Leave the palettes in
  `DashboardScreen`, `CandySpeechBubble` and `ReadingHouseDashboard` as they are. You may *propose* small
  colour tweaks in your report, but don't make them.
- **No layout or transform changes.** Don't move, scale or rotate:
  - the house, the dashboard, the chairs, the rug, the chalkboard or the door
  - the camera poses (Overview, Gameplay, TrayFocus) or the follow-camera settings
  - the student's start and reading spots.

  Don't change `MatStoneBoard` layout values, the knob, the chalkboard/dashboard screen geometry, or the
  front-wall cut-away.
- **Don't edit character art.** Leave the sprite frames under `Assets/Art/ReadingHouse/Student2D`,
  `Encourager2D` and `Zombie2D`, their sprite sets, and the animators untouched. Don't re-run the Blender
  pipelines under `GeneratedAssets/`.
- **Don't replace or remodel the Blender house**, and don't swap in other assets.
- **No new scenes. Edit `SC_ReadingHouse_Integration` in place.** Don't play-test or modify `SampleScene`.
- **Don't run the scene generators** (`ReadingHouseSceneBuilder.BuildAll`, `ReadingHouseScenePatcher.Apply`, or
  the bridge commands `build`, `patch`, `all`, `patchcapture`). They regenerate parts of the scene and would
  wipe later additions (seated characters, chalkboard screen, dashboard screen, stones on the rug).
- **Don't break the RenderTexture screens.** The chalkboard and the dashboard display render UI Toolkit into
  RenderTextures drawn with URP Unlit materials (the dashboard's is transparent and premultiplied). Don't
  convert them to lit materials, and keep them readable.
- **Stay within the device budget:** no extra realtime shadow-casting lights, at most one realtime directional
  light, modest lightmap resolution/texture sizes, ASTC compression.

## How to work

1. Work on a git branch (e.g. `polish/reading-house-lighting`). Commit in small, clearly named steps, and
   **don't push** unless asked.
2. **Before you change anything**, take screenshots at 1520 × 720 of:
   - the opening exterior shot
   - walking inside (follow camera)
   - the reading view at the dashboard
   - an activity open on the display (TrayFocus)
   - the chalkboard open.

   Take the same shots after each step.
3. A sign-in is needed to get inside the house. If you can't sign in, inspect the interior in the editor Scene
   view using the camera poses. Never fake data.
4. Put the lighting settings, Volume profile and changed materials in clearly named assets under
   `Assets/Art/ReadingHouse/Lighting/`, so every change can be reviewed and reverted.
5. Measure: editor Stats (batches/setpass, tris) before and after, plus the frame time if you can build for
   Android (`Tools ▸ sReader ▸ Reading House` holds the Android build tool).

## Deliverables

- The polished scene, with before/after screenshots for each of the camera shots above.
- A short report listing:
  - every change: lighting, bake settings, probes, the Volume profile values, materials changed (old → new)
  - performance numbers before and after
  - anything you wanted to change but didn't because of the constraints, written as suggestions.
