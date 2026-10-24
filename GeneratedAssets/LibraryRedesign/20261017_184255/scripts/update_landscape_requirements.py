from pathlib import Path
p=Path(r'C:/Users/11113930/sReader4/Docs/BlenderVisionForCodex.txt')
s=p.read_text(encoding='utf-8-sig')
backup=p.with_name('BlenderVisionForCodex.before_landscape_20261018.txt')
if not backup.exists():backup.write_text(s,encoding='utf-8')
s=s.replace('with a 720 × 1520 portrait display','with a 1520 × 720 (19:9 landscape) primary game resolution')
a=s.index('## 9. Mobile-first camera composition');b=s.index('## 10. Blender and Unity responsibilities')
s=s[:a]+'''## 9. Landscape-first camera composition

Primary target: Huawei Y5 (2019), 1520 × 720, 19:9 landscape.
Landscape is the primary gameplay orientation. Preserve the approved visual foundation and refine the existing scene. Portrait cameras are archival references only and must not drive current layout decisions.

Create and validate landscape presets:

`CAM_ROOM_OVERVIEW` — complete classroom, student visible, encourager left and discourager right.

`CAM_DASHBOARD_CLOSEUP` — unobstructed display using the available surface for long passages; Unity supplies wrapping, scrolling or pagination.

`CAM_STONES_CLOSEUP` — unobstructed tray, positioned ahead of the student, with multiple rows and variable-width stones.

`CAM_CHARACTER_VIEW` — optional character framing.

Render and inspect the three primary views at exactly 1520 × 720. Readability must be assessed in these rendered compositions and later on the physical device. A Blender render does not prove Unity touch behaviour or 30 FPS performance.

Use camera transitions; the player need not walk around the room. Keep the student in the overview and outside the reading/stone interaction frusta. Speech-bubble housings are optional and blank; Unity controls their activation and dialogue.

Android integration, after explicit approval of the refined Blender work: use autorotation restricted to Landscape Left and Landscape Right if supported. Disable portrait orientations at that integration stage. Do not change PlayerSettings or gameplay during the Blender phase.

'''+s[b:]
s=s.replace('Create the mobile camera presets and render the classroom at 720 × 1520.','Create the landscape camera presets and render the overview, reading display and unobstructed stone interaction at 1520 × 720.')
s=s.replace('- Huawei Y5 portrait camera preview.','- Huawei Y5 landscape overview, reading close-up and stone interaction at 1520 × 720.')
s=s.replace('Build the dashboard greybox first and show me the results before investing time in detailed characters or polishing.','The reference-polished dashboard and classroom are approved as the visual foundation. Refine existing geometry; do not rebuild the dashboard. Complete the landscape refinements and show the required renders before Unity integration.')
s=s.replace('## 15. Start now','## 15. Current refinement stage — landscape first')
a=s.index('Begin by inspecting the existing Blender scene through Blender MCP.',s.index('## 15.'))
s=s[:a]+'''Inspect the approved current Blender scene through the local Blender MCP connection. Save a backup, retain the approved glossy blue/gold/cyan materials and layout, deepen the reusable tray for variable counts/widths and multiple rows, and use the full reading surface for dynamic long passages.

Deliver an updated .blend, 1520 × 720 classroom overview, reading close-up, unobstructed stone interaction close-up, and a brief geometry/runtime handoff. Disable demonstration content and example speech in the production source. Keep production collections separate from backups, previews and cameras.

Do not modify Unity gameplay or Android settings yet. Wait for approval of the refined Blender work before integration.

The final goal remains a reusable 3D reading classroom with dynamic assignments, passages, words, scores, progress and dialogue.
'''
p.write_text(s,encoding='utf-8')
