import bpy,math,json
from pathlib import Path
from mathutils import Vector
from bpy_extras.object_utils import world_to_camera_view
OUT=Path(r'C:/Users/11113930/sReader4/GeneratedAssets/ReadingDashboard/20261018_Landscape1520');S=bpy.context.scene
assert bpy.data.filepath.endswith('sReader_Landscape_1520_Refined.blend')
preview=bpy.data.collections['SR_PREVIEW_Landscape_RuntimeMock'];preview.hide_render=False;preview.hide_viewport=False
hud=bpy.data.collections['PREVIEW_HUD'];wordscol=bpy.data.collections['PREVIEW_WordInstances'];chars=bpy.data.collections['SR_Characters'];chars.hide_render=False
# The extra depth requires the two seated companions and chairs to stay just ahead of the fascia.
for name in ['SR_CHARACTER_ENCOURAGER','SR_CHARACTER_ZOMBIE']:bpy.data.objects[name].location.y-=.90
for ob in bpy.data.collections['SR_Decor'].objects:
 if ob.name.startswith('SR_Chair') or ob.name.startswith('Chair_Leg'):ob.location.y-=.90
# Top label plane sits above the pebble's convex dome, avoiding intersected letters.
top=bpy.data.objects['SR_STONE_TEXT_ANCHOR_TOP'];top.location=(0,-.016,.529);top.rotation_euler=(math.radians(12),0,0)
labels=[o for o in wordscol.objects if o.type=='FONT'];front=bpy.data.objects['SR_STONE_TEXT_ANCHOR']
for ob in labels:ob.location=front.location;ob.rotation_euler=front.rotation_euler
# Full-passage pagination mock: measure actual font width, keep a readable font size, and paginate overflow.
po=bpy.data.objects['PREVIEW_Runtime_Passage'];cu=po.data;cu.size=.205;cu.space_line=1.0
full=('Plants need energy to grow. Through photosynthesis, green leaves use sunlight to turn water and carbon dioxide into sugar. The sugar stores energy that helps the plant make new roots, stems and leaves. Oxygen is released into the air. Water travels from the roots to the leaves through the stem. Tiny openings in each leaf let gases move in and out. When you see a green leaf in sunlight, think of it as a tiny food factory at work. On cloudy days, plants receive less light, so their food factories may work more slowly. Healthy roots, fresh air and enough sunlight all help a plant to thrive.')
lines=[];current=''
for word in full.split():
 candidate=(current+' '+word).strip();cu.body=candidate;bpy.context.view_layer.update()
 if po.dimensions.x>5.23 and current:lines.append(current);current=word
 else:current=candidate
if current:lines.append(current)
linepitch=cu.size*1.04;lines_per_page=7;pages=[lines[i:i+lines_per_page] for i in range(0,len(lines),lines_per_page)];cu.body='\n'.join(pages[0]);po.location=(-2.62,2.355,3.66)
# Runtime preview pagination indicator, excluded with all other example UI.
pagecu=bpy.data.curves.new('PREVIEW_Pagination','FONT');pagecu.body='Page 1 / '+str(len(pages));pagecu.font=cu.font;pagecu.size=.105;pagecu.align_x='RIGHT';pagecu.materials.append(bpy.data.materials['PREVIEW_Legible_Navy'])
pageob=bpy.data.objects.new('PREVIEW_Pagination',pagecu);hud.objects.link(pageob);pageob.location=(2.58,2.35,2.08);pageob.rotation_euler=(math.pi/2,0,0)
# Reading is a front-on passage mode: runtime word instances and characters are culled, not removed.
reading=bpy.data.objects['CAM_DASHBOARD_CLOSEUP'];reading.location=(0,.30,3.42);reading.rotation_euler=(Vector((0,2.48,3.42))-reading.location).to_track_quat('-Z','Y').to_euler();reading.data.ortho_scale=7.30
reading['runtime_visibility_profile']='Reading: show console and live passage; hide word instances and character renderers via camera layers/profile. Keep assignment header and multiplayer visible.'
stones=bpy.data.objects['CAM_STONES_CLOSEUP'];stones.location=(0,.95,4.70);stones.rotation_euler=(Vector((0,1.40,1.70))-stones.location).to_track_quat('-Z','Y').to_euler();stones.data.ortho_scale=6.70
stones['runtime_visibility_profile']='Interaction: show generated word stones and top labels; hide character renderers for the focus view. Student also lies outside the tray camera frustum.'
overview=bpy.data.objects['CAM_ROOM_OVERVIEW'];target=Vector((0,1.5,2.65))
# Fit all visible console/character bounds into 4% margins at 19:9.
required_objs=[o for cn in ['SR_Console','SR_Characters'] for o in bpy.data.collections[cn].objects if o.type=='MESH' and not o.hide_render]
for attempt in range(5):
 bpy.context.view_layer.update();pts=[world_to_camera_view(S,overview,o.matrix_world@Vector(v)) for o in required_objs for v in o.bound_box]
 xmin,xmax=min(p.x for p in pts),max(p.x for p in pts);ymin,ymax=min(p.y for p in pts),max(p.y for p in pts)
 dist=(target-overview.location).length;worldheight=dist*overview.data.sensor_width/overview.data.lens*720/1520
 target.z+=((ymin+ymax)/2-.5)*worldheight
 overview.rotation_euler=(target-overview.location).to_track_quat('-Z','Y').to_euler()
 span=max((xmax-xmin)/.92,(ymax-ymin)/.92)
 if span>1.001:overview.data.lens/=span
bpy.context.view_layer.update()
S.render.resolution_x=1520;S.render.resolution_y=720;S.render.resolution_percentage=100;S.cycles.samples=64
# Overview: all characters visible, optional speech disabled, front-facing word labels.
S.camera=overview;S.render.filepath=str(OUT/'01_classroom_landscape_1520x720.png');bpy.ops.render.render(write_still=True)
# Reading: fill the available display with a page; dynamically hide words/characters for this mode.
wordscol.hide_render=True;chars.hide_render=True;S.camera=reading;S.render.filepath=str(OUT/'02_reading_landscape_1520x720.png');bpy.ops.render.render(write_still=True)
# Interaction: clear tray, three variable-width rows, labels placed above the dome.
wordscol.hide_render=False
for ob in labels:ob.location=top.location;ob.rotation_euler=top.rotation_euler
S.camera=stones;S.render.filepath=str(OUT/'03_stones_landscape_1520x720.png');bpy.ops.render.render(write_still=True)
# Geometric visibility audit independent of the close-up character culling profile.
audit={}
student=bpy.data.objects['SR_CHARACTER_STUDENT']
for cam in [overview,reading,stones]:
 bboxes=[]
 for ob in student.children_recursive:
  if ob.type!='MESH' or ob.hide_render:continue
  p=[world_to_camera_view(S,cam,ob.matrix_world@Vector(v)) for v in ob.bound_box]
  intersects=(min(v.x for v in p)<1 and max(v.x for v in p)>0 and min(v.y for v in p)<1 and max(v.y for v in p)>0 and max(v.z for v in p)>0)
  bboxes.append(intersects)
 audit[cam.name]={'student_bounds_intersect_frustum':any(bboxes),'position':list(cam.location),'rotation_radians':list(cam.rotation_euler),'projection':cam.data.type,'ortho_width_m':cam.data.ortho_scale if cam.data.type=='ORTHO' else None,'visibility_profile':cam.get('runtime_visibility_profile','Overview: all characters visible.')}
# Font cap-height projection in the reading view.
cu.body='Hg';bpy.context.view_layer.update();pts=[world_to_camera_view(S,reading,po.matrix_world@Vector(v)) for v in po.bound_box];font_height_px=(max(v.y for v in pts)-min(v.y for v in pts))*720;cu.body='\n'.join(pages[0])
# Restore the clean source defaults; previews and optional speech are explicitly off.
chars.hide_render=False
for ob in labels:ob.location=top.location;ob.rotation_euler=top.rotation_euler
preview.hide_render=True;preview.hide_viewport=True;S.camera=overview
bpy.ops.file.pack_all();bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_Landscape_1520_Refined.blend'))
for name in ['STUDENT','ENCOURAGER','ZOMBIE']:
 rt=bpy.data.objects['SR_CHARACTER_'+name];bpy.data.libraries.write(str(OUT/('Character_'+name+'.blend')),{rt}|set(rt.children_recursive),fake_user=True)
bpy.data.libraries.write(str(OUT/'VariableWidth_Stone_Asset.blend'),{bpy.data.collections['SR_StoneSource']},fake_user=True)
def count(collection):
 dg=bpy.context.evaluated_depsgraph_get();triangles=0;meshes=0
 for ob in collection.objects:
  if ob.type=='MESH' and not ob.hide_render:
   ev=ob.evaluated_get(dg);me=ev.to_mesh();triangles+=sum(len(p.vertices)-2 for p in me.polygons);meshes+=1;ev.to_mesh_clear()
 return {'triangles':triangles,'mesh_renderers':meshes}
required=['SR_DISPLAY_MAIN','SR_STONE_PREFAB_SOURCE','SR_STONE_TEXT_ANCHOR','SR_STONE_TEXT_ANCHOR_TOP','SR_STONE_TRAY_ANCHOR','SR_STONE_PLACEMENT_REGION','SR_ANCHOR_ASSIGNMENT_HEADER','SR_ANCHOR_SENTENCE_PROGRESS','SR_ANCHOR_MULTIPLAYER','SR_ANCHOR_READING_CONTENT','SR_PANEL_POINTS','SR_BUTTON_MENU','SR_BUTTON_MULTIPLAYER']
report={'source':bpy.data.filepath,'resolution':[1520,720],'aspect':'19:9 landscape','required_names':{n:n in bpy.data.objects for n in required},'tray_depth_before_m':1.97,'tray_depth_after_m':2.87,'usable_region_m':[6.1,1.90],'row_centres_y_m':[.60,1.35,2.10],'example_stone_count':len([o for o in wordscol.objects if o.type=='EMPTY']),'example_row_count':3,'assignment_example_words':len(full.split()),'example_pages':len(pages),'example_page_lines':[len(p) for p in pages],'reading_font_projected_height_px':round(font_height_px,1),'camera_audit':audit,'character_before':count(bpy.data.collections['SR_BACKUP_ApprovedLandscapeFoundation']),'character_after':count(chars),'production_previews_disabled':preview.hide_render and bpy.data.collections['SR_Demo'].hide_render,'optional_speech_disabled':bpy.data.collections['SR_OPTIONAL_SPEECH_BUBBLES'].hide_render,'production_surface_materials':[m.name for m in bpy.data.objects['SR_DISPLAY_MAIN'].data.materials],'unity_modified':False,'frame_rate_verified':False,'android_orientation_next_step':'After Blender approval: use autorotation with Landscape Left/Right enabled and both portrait flags disabled. Existing project enables both landscape flags and both portrait flags; unchanged.'}
assert all(report['required_names'].values())
assert not audit[reading.name]['student_bounds_intersect_frustum'] and not audit[stones.name]['student_bounds_intersect_frustum'],'Student requires additional focus-camera framing.'
(OUT/'validation.json').write_text(json.dumps(report,indent=2))
manifest={cn:[{'name':o.name,'type':o.type,'parent':o.parent.name if o.parent else None} for o in bpy.data.collections[cn].objects] for cn in ['SR_Console','SR_Characters','SR_Decor','SR_StoneSource','SR_OPTIONAL_SPEECH_BUBBLES']};(OUT/'object_manifest.json').write_text(json.dumps(manifest,indent=2))
print(json.dumps(report))
