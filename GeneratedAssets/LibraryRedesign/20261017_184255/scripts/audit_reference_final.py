import bpy,json
from pathlib import Path
from mathutils import Vector
OUT=Path(r'C:/Users/11113930/sReader4/GeneratedAssets/ReadingDashboard/20261018_ReferencePolish');S=bpy.context.scene
for name,src in [('CAM_ROOM_OVERVIEW','CAM_REFERENCE_COMPARISON'),('CAM_STONES_CLOSEUP','CAM_STONES_CLOSEUP_FINAL')]:
 old=bpy.data.objects.get(name);ref=bpy.data.objects[src]
 if old:old.matrix_world=ref.matrix_world.copy();old.data=ref.data.copy()
cam=bpy.data.objects.get('CAM_DASHBOARD_CLOSEUP')
if cam:
 cam.location=(0,-2.1,3.15);cam.rotation_euler=(Vector((0,2.7,3.05))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.type='ORTHO';cam.data.ortho_scale=7.4
cam=bpy.data.objects.get('CAM_CHARACTER_VIEW')
if cam:
 cam.location=(2,-2.5,2.9);cam.rotation_euler=(Vector((0,-.57,1.65))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.type='PERSP';cam.data.lens=45
bpy.ops.file.pack_all()
S.camera=bpy.data.objects['CAM_REFERENCE_COMPARISON']
preview=bpy.data.images.load(str(OUT/'08_perspective_final.png'),check_existing=True)
for screen in bpy.data.screens:
 for area in screen.areas:
  if area.type=='VIEW_3D':area.spaces.active.region_3d.view_perspective='CAMERA'
  if area.type=='IMAGE_EDITOR':area.spaces.active.image=preview
required=['SR_DISPLAY_MAIN','SR_BUTTON_MULTIPLAYER','SR_BUTTON_MENU','SR_PANEL_POINTS','SR_STONE_PREFAB_SOURCE','SR_STONE_TRAY_ANCHOR','SR_STONE_PLACEMENT_REGION','SR_STONE_TEXT_ANCHOR','SR_ANCHOR_ASSIGNMENT_HEADER','SR_ANCHOR_SENTENCE_PROGRESS','SR_ANCHOR_MULTIPLAYER','SR_ANCHOR_READING_CONTENT','CAM_ROOM_OVERVIEW','CAM_DASHBOARD_CLOSEUP','CAM_STONES_CLOSEUP','CAM_CHARACTER_VIEW']
audit={'required_names':{n:n in bpy.data.objects for n in required},'packed_landscape':bpy.data.images.get('preview_landscape.png').packed_file is not None,'all_review_renders_exist':all((OUT/n).exists() for n in ['08_perspective_final.png','09_short_sentence_stones.png','10_long_sentence_stones.png','11_exterior_preserved.png']),'source':str(OUT/'sReader_ReadingHouse_Reference_FINAL.blend'),'unity_files_modified':False}
assert all(audit['required_names'].values()) and audit['packed_landscape'] and audit['all_review_renders_exist']
bpy.ops.wm.save_as_mainfile(filepath=audit['source']);(OUT/'validation.json').write_text(json.dumps(audit,indent=2));print(json.dumps(audit))
