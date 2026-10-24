# sReader library architectural redesign

Open `sReader_Library_FINAL.blend` for the completed Blender stage. The default camera is `SR_Camera_Interior` and the roof is hidden for interior inspection.

## Preservation and milestones

- `sReader_Library_v00_original.blend`: native backup saved before structural changes.
- `v01_shell`: hollow shell, separate floor, open shelves and reused props.
- `v02_architecture_atmosphere`: entrance, arched glazing, slate roof, materials, lighting and cameras.
- `v03_visual_refinement`: side-window openings and first render corrections.
- `v04_layout_verified`: measured GLB prop placement, owl and lantern refinements.
- `v05_modular_final`: consolidated bookshelf modules and final framing.
- `sReader_Library_FINAL.blend`: final scene, restored to its interior view after all three renders.

The hidden `Backup` collection preserves all 642 original objects and their original hierarchy/geometry. `Backup_Superseded_Working_Copies` preserves five replaced copies. There are no active overlapping imported building shells, foundations or ceilings. No Unity source, scoring, Supabase or multiplayer code was changed.

## Collections and cameras

`Architecture`, `Roof`, `Interior`, `Interactive Props`, `Lighting`, `Landscaping`, `Backup`.

For exterior rendering, enable both viewport and render visibility for `Roof` and select `SR_Camera_Exterior` as the scene camera. For interior rendering, disable both visibility toggles for `Roof` and select `SR_Camera_Interior` or `SR_Camera_Hero`. Keep `Backup` hidden in viewport and renders.

Every roof slope, tile field, gable, ridge, fascia and gable ornament is in `Roof`. The roof has no solid ceiling volume. The hollow shell remains complete with the roof hidden.

Four named `SR_Bookshelf_Module_*` meshes retain open compartments, visible books and the shared palette; the earlier milestone preserves the separate editable pieces. Interior dimensions are approximately 8.72m by 7.82m, with a 2.32m-wide entrance. The central route x=-0.8..0.8 remains clear from the threshold to the hero desk. World-bounds checks confirm the five stones sit on the rug and remain clear of the route and shelves; see `layout_audit.json`.

## Final previews

- `renders/01_exterior_front.png`
- `renders/02_interior_overview.png`
- `renders/03_hero_activity.png`

Final images use Cycles, GPU rendering, 64 samples with denoising, and 1600 x 1200 output. Earlier `review_*` images document visual inspection and corrections.

## Remaining production work

The Blender redesign is complete. Unity import/prefab integration, colliders, lightmap UVs and baked lighting, runtime label replacement, Android draw-call profiling and device testing remain. The stone A-E glyphs and page line art are environment decoration; they do not replace gameplay content. No GLB/FBX export or Unity scene replacement was made in this task.

The presentation uses 4 broad area lights, 24 shared active materials and 73,904 evaluated triangles across 389 renderable objects before Unity batching. These figures are an asset inventory, not measured Android performance. Bake the lighting and batch static modules during Unity integration.

The `scripts` folder records the build stages executed through Blender MCP on port 9876. Stages contain scene mutations and are not a general-purpose idempotent import pipeline.
