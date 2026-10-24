# sReader — Build the Complete 3D Interactive Reading House in Blender

## Your role

You are my lead Blender 3D environment artist, technical artist and Unity asset pipeline engineer.

Your responsibility is to recreate the approved sReader Reading House design as a professional, modular, game-ready 3D environment.

**Use Blender MCP and Blender Python (`bpy`) to perform the actual modelling inside Blender.**

Do not merely generate instructions or scripts for me to run manually. Use the connected tools to create, inspect, save, render and refine the models.

The target engine is Unity 6 URP, and the target device is a Huawei Y5 (2019), with a 720 × 1520 portrait display and limited hardware resources.

## 0. Read first: project-specific notes (these override anything below)

### 0.1 The reference image
The "approved sReader design" is the image file:

`Docs/Photosynthesis Reading Adventure.png`

Open it and study it before you model anything. It is your target for mood,
colours, proportions and materials: the glossy blue and gold plastic
dashboard, warm orange walls, pale moss-based word stones, the blue rug, and
the three characters.

**Where the image and this text disagree, the text wins:**

| Item | Image shows | Build this instead |
|---|---|---|
| Top-right button | "Menu" | Purple **Play with Others** housing (`SR_BUTTON_MULTIPLAYER`) |
| Bottom-right panel | "Sentences Made 2/10" | **Menu** button with a gold star (`SR_BUTTON_MENU`) |
| Word stones | Round balls of equal size | **Variable-width pebble stones** (left cap + stretchable centre + right cap) so long words like "photosynthesis" fit (section 4) |
| Layout | Widescreen | The game is **portrait 720 × 1520**. Long sentences wrap onto several stone rows, and the display must stay readable in portrait (section 9). |
| All text in the image | Painted in | Example content only. Never model or texture any words or numbers. |

### 0.2 Clean up from the previous session (do this before anything else)
1. Run `git checkout -- Assets/Prefabs/Kit`. Your last session stripped the
   `M_TrimLibrary` material from 28 kit prefabs, which breaks older scenes. Do
   not edit those prefabs again.
2. In `GeneratedAssets/`, keep only the final `.blend` file of each piece of
   work, and add the intermediate versions, renders and reports to `.gitignore`.
3. Delete `Assets/Scenes/_RedesignBackups/`. Git is the backup.
4. `SC_LibraryRedesignPreview` and the Magical Classroom work are now
   **superseded** by this brief. Reuse their architecture and props, but build
   the new scene separately (section 0.6).

### 0.3 Match the game's existing stones and sparkle style
- The game already has word stones: `Assets/Prefabs/SM_WordStone_A/B/C` and
  `SM_WordStone_Key` (models in `Assets/Art/Models_WordPath/`), with the
  stone-pad texture `Assets/Resources/T_StonePad.png`. The new variable-width
  stone must feel like the same family: a pale polished pebble, a moss base,
  and a gold variant for key words.
- The game's sparkle palette is in `Assets/Scripts/StoryBook.cs`:
  - bubblegum `#FF73B3`
  - sky `#8CD9FF`
  - gold `#FFD159`
  - mint `#80F2CC`
  - grape `#BF8CFF`

  Use it for effects alongside the dashboard's blue, cyan, orange and gold.

### 0.4 Add an opening scene outside the house
The game opens **outside the Reading House**. Model the pieces it needs:
- A cute bench or front steps outside the entrance, with an anchor
  `SR_ANCHOR_BENCH_SIT`.
- A glowing storybook prop the student picks up: `SR_PROP_STORYBOOK`, plus
  anchors `SR_ANCHOR_BOOK_PICKUP` and `SR_ANCHOR_HAND_BOOK` on the student's
  right hand.
- An entrance door that can swing open on a sensible hinge pivot:
  `SR_DOOR_ENTRANCE`.
- Path anchors `SR_ANCHOR_DOOR_OUTSIDE`, `SR_ANCHOR_DOOR_INSIDE` and
  `SR_ANCHOR_STUDENT_DASHBOARD` (the student's standing spot on the rug).
- Extra camera presets: `CAM_OUTSIDE_WIDE` (house and student on the bench)
  and `CAM_DOOR_FOLLOW`.

The sequence is: the student sits outside for a while, picks up the book,
stands, walks in, and stands at the dashboard. Claude Code will animate this
in Unity. You only need to make the student's skeleton or hierarchy rig-ready.

### 0.5 Particles (required)
Create lightweight, mobile-friendly particle prefabs in Unity (URP
*Particles/Unlit*, additive, one shared sprite atlas) under
`Assets/Art/ReadingHouse/FX/`. Make them bright, cute and polished, like a
casual puzzle game:

| Prefab | Use |
|---|---|
| `FX_BookSparkle_Idle` | Sparkles around the waiting storybook (loops) |
| `FX_DoorLightSpill` | Warm motes when the door opens |
| `FX_StoneSelect` | Soft glow pulse on a selected stone |
| `FX_WordCorrect` | Gold splash, stars and a rising sparkle column |
| `FX_WordNotYet` | Gentle lilac puff, never red |
| `FX_StoneComplete` | Ring flash when a stone turns gold |
| `FX_ButtonPress` | Small candy puff |
| `FX_SentenceComplete` | Confetti and stars across the display |
| `FX_Ambient_Motes` | Slow floating motes in the room (loops) |

**Limits:** each one-shot stays under about 60 particles, under about 200
particles are alive in total, and there are no lit particles. Do not wire them
to gameplay; Claude Code triggers them by name.

### 0.6 Unity delivery
- Import into `Assets/Art/ReadingHouse/` (Models, Materials, Textures, FX,
  Prefabs).
- Build a new scene, `Assets/Scenes/SC_ReadingHouse.unity`, with:
  - the house and interior
  - the dashboard
  - all three characters placed
  - every `SR_*` anchor and `CAM_*` preset present as a named Transform
  - the FX prefabs placed but idle
  - static meshes marked static with lightmap UVs
- Keep the names exactly as written. Claude Code's integration code finds
  objects by these names.
- Do not write gameplay, UI or multiplayer scripts. Any editor scripts you
  write for building the scene must be readable C#, one statement per line.
- Write `Assets/Art/ReadingHouse/HANDOFF.md`: the naming manifest, plus the
  stone's dimensions (cap width, height, centre-stretch rule) and screenshots.

### 0.7 Internet
If a download or package step fails with "no internet", say so clearly and
continue with local work. Do not stop the whole task over it.

---

## 1. Approved visual reference

Use `Docs/Photosynthesis Reading Adventure.png` as the primary visual reference (see section 0.1).

The design includes:

- A cosy, stylised library house with a blue slate roof, cream walls, arched windows and a central entrance.
- An interior with bookshelves along one side only.
- A large, colourful, integrated arcade-style reading dashboard occupying approximately half the interior room.
- A tall rear display attached to the dashboard and positioned against the back wall.
- A raised, forward-projecting console surface containing interchangeable word stones.
- Glossy blue, cyan, orange and golden-yellow plastic surfaces.
- A central student character standing in front of the dashboard.
- An encouraging character seated on the left.
- A humorous, mildly discouraging zombie character seated on the right.
- Warm lighting, simple furniture, a rug and restrained decorative plants.

The dashboard must feel like a high-quality, colourful educational game inspired by casual puzzle games such as Candy Crush, not like a traditional wooden classroom desk.

Avoid making it resemble a bed, coffin, headboard, arcade vending machine or separate mobile-app dashboard.

The dashboard and its rear display must be **one integrated physical 3D structure**.

## 2. Inspect the existing project first

Before modelling:

1. Locate the existing Blender library model and the latest approved Unity preview.
2. Inspect the existing architecture, roof, entrance, shelves, hero book, word stones, Owl perch and other reusable assets.
3. Read existing Blender handoff documentation.
4. Identify which objects can be reused or improved.
5. Save a backup before changing any geometry.
6. Report which source file you selected and why.

Do not rebuild working architecture unnecessarily.

Preserve original objects in an organised, hidden backup collection.

## 3. Build the main reading dashboard

This is the most important asset.

Create a large integrated 3D console occupying approximately half the classroom floor area and spanning most of the back wall.

### Rear display

The vertical rear section must be tall enough to display long reading passages.

Create:

- A large rectangular display surface with softly rounded corners.
- A glossy blue outer frame.
- Thick golden-yellow decorative trim.
- Restrained illuminated accent strips.
- An integrated sReader nameplate.
- A flat, clean, untextured content surface for dynamic Unity text and imagery.

The rear display must not contain permanent assignment text.

Its surface should support different reading passages, sentence displays, activity instructions and contextual illustrations.

Create a named placeholder surface:

`SR_DISPLAY_MAIN`

Create additional named anchors:

`SR_ANCHOR_ASSIGNMENT_HEADER`

`SR_ANCHOR_SENTENCE_PROGRESS`

`SR_ANCHOR_MULTIPLAYER`

`SR_ANCHOR_READING_CONTENT`

The actual text and interface elements will be generated dynamically in Unity.

### Top-left assignment header

Reserve space for:

- Assignment title, for example Photosynthesis.
- Sentence progress, for example Sentence 2 of 10.
- A progress indicator.

Assignment titles may be long, so the header must support wrapping, resizing or controlled truncation at runtime.

Do not model the words Photosynthesis or Sentence 2 of 10 into the geometry.

### Top-right multiplayer control

Create a purple, rounded button housing for:

`Play with Others`

Name it:

`SR_BUTTON_MULTIPLAYER`

Its label and behaviour will be assigned in Unity.

### Main reading display

The large rear screen will show:

- The current sentence or reading passage.
- Contextual illustrations or backgrounds.
- Instructions when needed.
- Activity feedback.

Do not permanently embed example sentences or illustrations into the Blender mesh.

Provide a clean, appropriately proportioned display plane and clear UI attachment points.

Long passages must remain readable through dynamic scrolling, pagination or another suitable Unity interface.

## 4. Build the forward console and dynamic stone tray

Create a broad, raised, gently angled console surface extending toward the student.

Use smooth rounded plastic surfaces with blue and gold accents.

This surface must hold **reusable word stones**.

### Critical requirement: variable number of stones

Different sentences contain different numbers of words.

Examples:

Short sentence:

"The sun shines."

Longer sentence:

"Photosynthesis allows green plants to convert sunlight into chemical energy."

The number of stones must not be fixed.

**Do not permanently model 12 or 15 stones into the dashboard.**

Instead:

1. Build one reusable stone mesh.
2. Give it a sensible pivot and scale.
3. Provide a reusable stone socket or placement region.
4. Create a named stone tray anchor.
5. Design the tray so Unity can instantiate, position, wrap, paginate or scroll stones dynamically.

Required names:

`SR_STONE_PREFAB_SOURCE`

`SR_STONE_TRAY_ANCHOR`

`SR_STONE_PLACEMENT_REGION`

`SR_STONE_TEXT_ANCHOR`

### One stone represents one word

Each stone contains exactly one word.

Examples:

- The
- forest
- photosynthesis
- extraordinary
- morning

A stone must never contain an entire sentence or multiple words as a default representation.

### Long words

Some words are much longer than others.

Create a stone design that supports variable widths while preserving a consistent height and appearance.

The design should accommodate long words such as:

- Photosynthesis
- Extraordinary
- Understanding
- Communication

Consider a modular three-part mesh consisting of a left cap, stretchable centre and right cap, or another implementation that can expand horizontally without visibly distorting the shape.

Do not stretch a complete stone mesh in a way that makes its rounded ends look unnatural.

The stone should look like a polished pale magical pebble, with a subtle green moss base and optional glow.

Do not engrave any permanent words into the Blender mesh.

### Stone states

Prepare material slots or variants for:

- Normal
- Selected
- Correct
- Completed
- Deferred
- Disabled

Unity will control these states dynamically.

Use lightweight materials suitable for Android.

## 5. Stone layout behaviour

Blender is responsible for creating the assets and layout anchors.

Unity will be responsible for runtime stone management.

The final design must support:

- A variable number of words per sentence.
- Variable stone widths.
- Multiple rows when necessary.
- Horizontal or paginated navigation for long sentences.
- Touch-friendly selection.
- Long words without text overflow.
- Stone highlighting and selection.
- Sentence changes without reloading the Blender model.

Do not create a separate Blender scene for every sentence length.

Do not hardcode the example sentence into any texture or material.

The system must remain reusable for thousands of different reading assignments.

## 6. Front dashboard controls

The console's front-facing lower structure should contain two distinct areas.

### Left — Points display

Create a polished dark-blue display housing with a small trophy or gold star.

Name:

`SR_PANEL_POINTS`

The number of points must be dynamic.

Example value 120 is for demonstration only.

### Right — Menu control

Create a rounded blue Menu button with a gold star icon.

Name:

`SR_BUTTON_MENU`

This must occupy the lower-right position previously used for the Sentences Made display.

Do not put the Menu button in the top-right multiplayer position.

The top-right must remain:

`Play with Others`

### Additional controls

Where appropriate, provide unobtrusive attachment points for:

- Assignments
- Previous sentence
- Next sentence
- Read passage
- Activity instructions

Avoid cluttering the stone tray with too many buttons.

Keep the physical design simple enough to implement and maintain.

## 7. Model the three characters

Create three separate reusable 3D character assets.

### Main student

A friendly student with:

- Stylised proportions.
- Curly hair.
- Blue school-style clothing.
- Small backpack.
- Comfortable shoes.

The student stands on the rug facing the dashboard.

Create a clean hierarchy suitable for later rigging and animation.

Do not make the character permanently attached to the environment.

### Encourager — left chair

Create a friendly, cheerful character seated on a simple chair to the left of the dashboard.

Visual characteristics:

- Blue cap or hoodie.
- Friendly face.
- Open book.
- Encouraging hand gesture.

The character supports the student during reading activities.

Provide a separate speech-bubble anchor.

Example encouragement:

"Great job! You can do it!"

Do not embed the dialogue permanently in the model.

### Discourager — right chair

Create a humorous, cartoon-style zombie character seated to the right.

Characteristics:

- Muted green skin.
- Slightly tired or doubtful expression.
- Casual clothing.
- Comical rather than frightening appearance.

The zombie represents discouragement and doubt, but should remain suitable for children.

Provide a separate speech-bubble anchor.

Example dialogue:

"This is hard... You'll never finish."

Dialogue must be dynamic and must not be permanently baked into textures.

The student should ultimately receive positive, confidence-building feedback.

### Character technical requirements

- Separate meshes and materials.
- Organised object hierarchy.
- Appropriate pivots.
- Reasonable polygon budgets.
- Rig-ready structure where practical.
- No mandatory high-cost animation system at this stage.

Create the models first; advanced animation can be a later phase.

## 8. Environment and room layout

Keep the existing Reading House exterior.

Inside the room:

- Dashboard against the back wall.
- Bookshelves along one side wall.
- Clear opposite wall with restrained decoration.
- Student standing centrally.
- Encourager seated on the left.
- Zombie seated on the right.
- Blue rug in front of the dashboard.
- Warm wall lights.
- Minimal plants and furniture.

Keep a clear camera sightline to the dashboard and word stones.

Do not overcrowd the room.

The dashboard should be the dominant feature.

## 9. Mobile-first camera composition

The physical test device is Huawei Y5 (2019).

Resolution:

720 × 1520, portrait.

The current concept art is widescreen, but the actual game must work on a narrow portrait phone.

Create Blender camera presets for:

`CAM_ROOM_OVERVIEW`

`CAM_DASHBOARD_CLOSEUP`

`CAM_STONES_CLOSEUP`

`CAM_CHARACTER_VIEW`

The room overview should establish the setting.

The dashboard close-up must make reading text comfortable.

The stone close-up must provide large enough touch targets for individual words.

The player should not need to physically walk around the room to interact with the dashboard.

Use camera transitions and touch interactions.

Render and inspect the layout at the actual 720 × 1520 aspect ratio.

Do not claim that the widescreen concept art alone proves mobile readability.

## 10. Blender and Unity responsibilities

### Blender creates

- Building and interior geometry.
- Integrated dashboard geometry.
- Rear screen frame and content plane.
- Reusable stone meshes.
- Stone placement region.
- Points display housing.
- Menu button housing.
- Multiplayer button housing.
- Character models.
- Chairs, bookshelves and decorative objects.
- Named interaction anchors.
- Materials and UVs.
- Camera reference transforms.

### Unity controls dynamically

- Assignment title.
- Current sentence.
- Reading passage.
- Sentence progress.
- Player points.
- Stone count.
- Stone words.
- Stone positions and widths.
- Selected stone.
- Completed and deferred states.
- Activity instructions.
- Multiplayer state.
- Menu behaviour.
- Character dialogue.
- Interaction events.

Do not bake dynamic content into Blender textures or geometry.

Use Unity's existing assignment, reading and scoring systems as the source of truth.

Do not create a second scoring or reading engine.

For production, use a suitable combination of TextMeshPro, world-space Canvas, reusable prefabs and dynamic layout logic.

UI Toolkit may continue to handle the existing reading screens, but it must not be assumed to support world-space rendering natively.

## 11. Mobile performance requirements

Optimise for an older Android device with approximately 2 GB RAM.

Initial target: 30 FPS, subject to physical testing.

Use:

- Low-poly or moderately detailed stylised meshes.
- Efficient material reuse.
- Small texture atlases where appropriate.
- Compressed textures.
- Simple lighting.
- Baked static illumination where practical.
- Minimal transparency.
- Lightweight emissive details.
- Simple colliders.
- No unnecessarily dense subdivision meshes.

Keep the colourful, polished appearance without relying on expensive effects.

Avoid hundreds of individually expensive decorative meshes.

## 12. Mandatory Blender workflow

Work in stages.

### Stage 1 — Inspection

Inspect the existing Blender project and report reusable assets, model dimensions and room layout.

### Stage 2 — Dashboard greybox

Build the integrated dashboard in Blender using simple geometry.

Verify that it occupies approximately half the room and fits against the back wall.

Create the rear display, angled stone tray and lower front panel.

### Stage 3 — Reusable stones

Build the reusable stone system, including variable-width support and named layout anchors.

Show example arrangements for short and long sentences using demonstration objects only.

### Stage 4 — Visual polish

Apply the approved blue, cyan, yellow and orange materials.

Improve rounded edges, proportions, lighting and decorative details.

### Stage 5 — Characters

Create the student, encourager and discourager as separate reusable assets.

Place them in the scene.

### Stage 6 — Camera validation

Create the mobile camera presets and render the classroom at 720 × 1520.

Verify the screen and stones remain usable in close-up.

### Stage 7 — Export

After visual approval, export the assets to Unity-compatible GLB or FBX files.

Do not export hidden backup objects or unnecessary Blender-only helpers.

## 13. Deliverables

Provide:

1. A complete, organised `.blend` source file.
2. A separate dashboard asset.
3. A reusable variable-width word stone asset.
4. Three separate character assets.
5. Game-ready materials.
6. Named anchors and interaction points.
7. A production export collection.
8. Unity-compatible exported models.
9. An object and naming manifest.
10. A short technical handoff explaining how Unity should dynamically populate the display and stones.

Render these previews:

- Exterior of the Reading House.
- Interior overview showing the complete dashboard.
- Dashboard close-up.
- Stone tray showing a short sentence.
- Stone tray showing a longer sentence containing "photosynthesis".
- Huawei Y5 portrait camera preview.

## 14. Approval and safety rules

Create a backup before significant changes.

You may inspect and perform reversible work immediately.

Build the dashboard greybox first and show me the results before investing time in detailed characters or polishing.

Do not delete the original library.

Do not modify the existing Unity scoring, Supabase, multiplayer or assignment systems.

Do not hardcode words, scores, sentence counts or assignment titles into 3D models.

Do not claim to have created, rendered, exported or tested anything unless the connected tools confirm it.

## 15. Start now

Begin by inspecting the existing Blender scene through Blender MCP.

Identify the building dimensions and back wall where the dashboard should be installed.

Then create the integrated dashboard greybox, occupying approximately half the room, with a tall rear display, forward stone tray and front points/Menu panels.

Show me a Blender render of this first stage and wait for my approval before detailed modelling.

**The final goal is a beautiful, reusable 3D educational game environment where every assignment, sentence, word stone, score and interaction can change dynamically without requiring new Blender models.**