import bpy,math,json,textwrap
from pathlib import Path
from mathutils import Vector
from bpy_extras.object_utils import world_to_camera_view
OUT=Path(r'C:/Users/11113930/sReader4/GeneratedAssets/ReadingDashboard/20261018_Landscape1520');OUT.mkdir(parents=True,exist_ok=True)
S=bpy.context.scene
assert bpy.data.filepath.endswith('sReader_ReadingHouse_Reference_FINAL.blend'),bpy.data.filepath
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'Approved_Foundation_Backup.blend'))
# Preserve source objects in a hidden, independent archive before editing.
archive=bpy.data.collections.new('SR_BACKUP_ApprovedLandscapeFoundation');S.collection.children.link(archive);mapping={}
for cn in ['SR_Console','SR_Characters','SR_StoneSource']:
 for ob in bpy.data.collections[cn].objects:
  cp=ob.copy()
  if ob.data:cp.data=ob.data.copy()
  cp.name='APPROVED_BACKUP_'+ob.name;archive.objects.link(cp);mapping[ob]=cp
for original,cp in mapping.items():
 cp.parent=mapping.get(original.parent);cp.matrix_world=original.matrix_world.copy()
archive.hide_render=True;archive.hide_viewport=True
console=bpy.data.collections['SR_Console'];chars=bpy.data.collections['SR_Characters'];source=bpy.data.collections['SR_StoneSource'];prod=bpy.data.collections['SR_PRODUCTION_EXPORT']
# Deepen the existing deck by 0.90m toward the player, retaining its width, height, bevel radii and rear edge.
def deepen_box(name,extra):
 o=bpy.data.objects[name];ys=[v.co.y for v in o.data.vertices];old=max(ys)-min(ys)
 for v in o.data.vertices:v.co.y*= (old+extra)/old
 o.location.y-=extra/2
for n in ['SR_Tray_Gold_Lip','SR_Tray_Cyan_Inset']:deepen_box(n,.90)
base=bpy.data.objects['SR_Console_Sculpted_Base'];rear=max(v.co.y for v in base.data.vertices)
for v in base.data.vertices:v.co.y=rear+(v.co.y-rear)*(2.60/1.70)
for n in ['SR_Tray_Front_Orange_Rail','SR_PANEL_POINTS','SR_BUTTON_MENU','SR_Console_Light_Gold','SR_Console_Light','SR_Menu_Star','SR_Points_Star','SR_ANCHOR_POINTS','SR_ANCHOR_MENU','SR_ANCHOR_READ_PASSAGE']:
 o=bpy.data.objects.get(n)
 if not o:continue
 # Star mesh coordinates were authored directly in world space.
 if o.type=='MESH' and n in ['SR_Menu_Star','SR_Points_Star']:
  for v in o.data.vertices:v.co.y-=.90
 else:o.location.y-=.90
region=bpy.data.objects['SR_STONE_PLACEMENT_REGION'];region.location=(0,1.35,1.69);region['bounds_m']=[6.1,1.90];region['x_limits_m']=[-3.05,3.05];region['y_limits_m']=[.40,2.30];region['recommended_row_centres_y_m']=[.60,1.35,2.10];region['row_pitch_m']=.75;region['overflow']='Wrap rows by measured word width. Paginate or scroll additional rows in Unity, never scale text below the readable floor.'
bpy.data.objects['SR_STONE_TRAY_ANCHOR'].location=(0,1.35,1.69)
# Reusable stone pivot at bottom centre; keep fixed caps, stretch centre only.
rt=bpy.data.objects['SR_STONE_PREFAB_SOURCE']
for o in rt.children:o.location.z+=.275
rt['pivot']='Bottom centre of moss contact footprint, Blender local Z up; metres.';rt['centre_width_m']=.32;rt['fixed_cap_radius_x_m']=.18;rt['total_width_rule']='totalWidth = centreWidth + 0.36; move caps to +/- centreWidth/2; scale centre X only';rt['label_rule']='Exactly one dynamic word, use front anchor in overview or top anchor in elevated interaction view. Never stretch text or caps.'
front=bpy.data.objects['SR_STONE_TEXT_ANCHOR'];front.rotation_euler=(math.pi/2,0,0)
top=bpy.data.objects.new('SR_STONE_TEXT_ANCHOR_TOP',None);source.objects.link(top);top.parent=rt;top.location=(0,-.083,.43);top.rotation_euler=(math.radians(32),0,0);top.empty_display_size=.05
# All six states are available to every stone body part; Unity applies the active material.
states=[bpy.data.materials['SR_STONE_STATE_'+n] for n in ['Normal','Selected','Correct','Completed','Deferred','Disabled']]
for o in rt.children:
 if o.type=='MESH' and 'MOSS' not in o.name:
  o.data.materials.clear()
  for m in states:o.data.materials.append(m)
  for f in o.data.polygons:f.material_index=0
# Blank optional speech housings: no permanent dialogue and disabled by default.
optional=bpy.data.collections.new('SR_OPTIONAL_SPEECH_BUBBLES');prod.children.link(optional)
for who in ['Encourager','Zombie']:
 old=bpy.data.objects['DEMO_'+who+'_Bubble'];ob=old.copy();ob.data=old.data.copy();optional.objects.link(ob);ob.name='SR_SPEECH_HOUSING_'+who.upper();ob.parent=bpy.data.objects['SR_ANCHOR_'+who.upper()+'_SPEECH'];ob.matrix_world=old.matrix_world.copy();ob['runtime']='Optional blank housing; Unity activates and supplies dialogue. No example dialogue in production.'
optional.hide_render=True;optional.hide_viewport=True
# Hide every old demonstration, including dialogue and previous word layouts.
old_demo=bpy.data.collections['SR_Demo'];old_demo.hide_render=True;old_demo.hide_viewport=True
# Reduce evaluated character topology and consolidate material groups, preserving silhouette and head pivots.
before={};after={};groups_before=0;groups_after=0
def tris(objects):
 dg=bpy.context.evaluated_depsgraph_get();total=0
 for o in objects:
  if o.type=='MESH' and not o.hide_render:
   ev=o.evaluated_get(dg);me=ev.to_mesh();total+=sum(len(p.vertices)-2 for p in me.polygons);ev.to_mesh_clear()
 return total
for name in ['STUDENT','ENCOURAGER','ZOMBIE']:
 root=bpy.data.objects['SR_CHARACTER_'+name];obs=[o for o in root.children_recursive if o.type=='MESH' and not o.hide_render];before[name]=tris(obs);groups_before+=len(obs)
 for o in obs:
  if len(o.data.polygons)>150:
   mod=o.modifiers.new('Mobile topology reduction','DECIMATE');mod.ratio=.52 if any(t in o.name.lower() for t in ['hair','curl']) else .68
   bpy.context.view_layer.objects.active=o
   bpy.ops.object.modifier_apply(modifier=mod.name)
 groups={}
 head=next((o for o in root.children if 'Head_Pivot' in o.name),None)
 for o in obs:
  in_head=head and o in head.children_recursive
  key=('HEAD' if in_head else 'BODY',tuple(m.name if m else '' for m in o.data.materials))
  groups.setdefault(key,[]).append(o)
 for (part,mats),members in groups.items():
  bpy.ops.object.select_all(action='DESELECT')
  for o in members:o.select_set(True)
  bpy.context.view_layer.objects.active=members[0]
  if len(members)>1:bpy.ops.object.join()
  joined=bpy.context.view_layer.objects.active;world=joined.matrix_world.copy();joined.parent=head if part=='HEAD' else root;joined.matrix_world=world;joined.name='SR_'+name+'_'+part+'_'+(mats[0].replace('SR_REF_','') if mats else 'Mesh')
 after[name]=tris(root.children_recursive);groups_after+=len([o for o in root.children_recursive if o.type=='MESH' and not o.hide_render])
# Preview collection is intentionally separate from the production root.
preview=bpy.data.collections.new('SR_PREVIEW_Landscape_RuntimeMock');S.collection.children.link(preview);preview['export_exclude']=True;preview['purpose']='Removable Blender-only mock of Unity-generated passages, labels, points, progress and variable stone layout.'
hud=bpy.data.collections.new('PREVIEW_HUD');preview.children.link(hud);words_col=bpy.data.collections.new('PREVIEW_WordInstances');preview.children.link(words_col)
# Reuse approved preview header styling and scenic image, never production content.
for n in ['DEMO_Assignment','DEMO_Progress','DEMO_Multiplayer','DEMO_Points_Label','DEMO_Points_Value','DEMO_Menu_Label','DEMO_Progress_Track','DEMO_Progress_Fill','DEMO_Contextual_Illustration']:
 old=bpy.data.objects.get(n)
 if not old:continue
 cp=old.copy();cp.data=old.data.copy() if old.data else None;hud.objects.link(cp);cp.name='LANDSCAPE_PREVIEW_'+n
 if any(k in n for k in ['Points_','Menu_Label']):cp.location.y-=.90
font=bpy.data.objects['DEMO_Sentence'].data.font
navy=bpy.data.materials['PREVIEW_Legible_Navy']
# Full-height passage panel occupies the available display, with runtime padding/scrolling anchors.
bpy.ops.mesh.primitive_cube_add(size=1,location=(0,2.39,2.91));panel=bpy.context.object
for c in list(panel.users_collection):c.objects.unlink(panel)
hud.objects.link(panel);panel.name='PREVIEW_Full_Passage_Backdrop';panel.scale=(5.66,.028,1.82);bpy.ops.object.transform_apply(location=False,rotation=False,scale=True);panel.data.materials.append(bpy.data.materials['SR_REF_bubble']);be=panel.modifiers.new('Soft passage corners','BEVEL');be.width=.09;be.segments=4;panel.modifiers.new('Normals','WEIGHTED_NORMAL')
passage='Plants need energy to grow. Through photosynthesis, green leaves use sunlight to turn water and carbon dioxide into sugar. The sugar stores energy that helps the plant make new roots, stems and leaves. Oxygen is released into the air. When you see a green leaf in sunlight, think of it as a tiny food factory at work.'
lines=textwrap.wrap(passage,width=64)
cu=bpy.data.curves.new('PREVIEW_Passage_Text','FONT');cu.body='\n'.join(lines);cu.font=font;cu.size=.158;cu.align_x='LEFT';cu.align_y='TOP';cu.space_line=1.15;cu.materials.append(navy)
po=bpy.data.objects.new('PREVIEW_Runtime_Passage',cu);hud.objects.link(po);po.location=(-2.65,2.36,3.64);po.rotation_euler=(math.pi/2,0,0)
for n,pos in [('SR_ANCHOR_PASSAGE_TOP_LEFT',(-2.78,2.35,3.82)),('SR_ANCHOR_PASSAGE_BOTTOM_RIGHT',(2.78,2.35,2.0)),('SR_ANCHOR_PASSAGE_SCROLL',(2.83,2.35,2.91)),('SR_ANCHOR_PASSAGE_PAGINATION',(2.15,2.35,2.04))]:
 ob=bpy.data.objects.new(n,None);console.objects.link(ob);ob.parent=bpy.data.objects['SR_DASHBOARD_ROOT'];ob.location=pos;ob.empty_display_size=.06
surface=bpy.data.objects['SR_DISPLAY_MAIN'];surface['usable_content_m']=[5.56,1.82];surface['runtime_layout']='World-space Canvas/TMP; 0.20m horizontal padding; long passages wrap and paginate/scroll. No static words or illustrations on this material.'
# Variable-width stone instances: demonstration only, no fixed production sockets or stone count.
sentence='Photosynthesis allows green plants to convert sunlight into chemical energy. Plants use this energy to grow and produce oxygen.'
words=sentence.split();widths=[max(.48,.23+.074*len(w)) for w in words];rows=[[]];used=0
for w,wi in zip(words,widths):
 if used+wi>5.90 and rows[-1]:rows.append([]);used=0
 rows[-1].append((w,wi));used+=wi+.09
labels=[];stone_roots=[]
for row_index,row in enumerate(rows):
 y=2.1-row_index*.75;xx=-(sum(wi for _,wi in row)+.09*(len(row)-1))/2
 for word,wi in row:
  root=bpy.data.objects.new('PREVIEW_Word_'+word,None);words_col.objects.link(root);root.location=(xx+wi/2,y,1.68+.06*(y-1.73));stone_roots.append(root);cw=wi-.36
  for so in rt.children:
   if so.type!='MESH':continue
   cp=so.copy();cp.data=so.data;words_col.objects.link(cp);cp.parent=root;cp.hide_render=False;cp.name='PREVIEW_'+so.name
   if 'CAP_LEFT' in so.name:cp.location.x=-cw/2
   elif 'CAP_RIGHT' in so.name:cp.location.x=cw/2
   elif 'STRETCH_CENTRE' in so.name:cp.scale.x=cw
  cu=bpy.data.curves.new('PREVIEW_Word_Label','FONT');cu.body=word;cu.font=font;cu.size=.136;cu.align_x='CENTER';cu.align_y='CENTER';cu.materials.append(navy)
  ob=bpy.data.objects.new('PREVIEW_Word_Label',cu);words_col.objects.link(ob);ob.parent=root;ob.location=front.location;ob.rotation_euler=front.rotation_euler;labels.append(ob)
  bpy.context.view_layer.update()
  if ob.dimensions.x>wi-.08:ob.scale*= (wi-.08)/ob.dimensions.x
  xx+=wi+.09
assert len(rows)<=3,'Demonstration exceeds the three-row page capacity.'
# Landscape-only active camera set. Previous portrait cameras are archived.
archivecam=bpy.data.collections.new('SR_ARCHIVE_PortraitCameras');S.collection.children.link(archivecam);archivecam.hide_viewport=True
for ob in list(S.objects):
 if ob.type=='CAMERA' and ('portrait' in ob.name.lower() or ob.name=='CAM_MOBILE_READING'):
  for c in list(ob.users_collection):c.objects.unlink(ob)
  archivecam.objects.link(ob);ob['status']='Archived, superseded by 1520x720 landscape requirements.'
def camera(name,loc,target,ortho=None,lens=38):
 ob=bpy.data.objects.get(name)
 if not ob:
  ob=bpy.data.objects.new(name,bpy.data.cameras.new(name));bpy.data.collections['SR_Cameras'].objects.link(ob)
 ob.location=loc;ob.rotation_euler=(Vector(target)-ob.location).to_track_quat('-Z','Y').to_euler();ob.data.clip_start=.05;ob.data.clip_end=150;ob.data.sensor_fit='HORIZONTAL';ob.data.type='ORTHO' if ortho else 'PERSP'
 if ortho:ob.data.ortho_scale=ortho
 else:ob.data.lens=lens
 ob['target_resolution']=[1520,720];ob['aspect']='19:9 landscape';return ob
overview=camera('CAM_ROOM_OVERVIEW',(0,-9.8,3.85),(0,1.5,2.65),lens=38)
reading=camera('CAM_DASHBOARD_CLOSEUP',(0,.30,5.0),(0,2.48,3.0),ortho=7.24)
stones=camera('CAM_STONES_CLOSEUP',(0,-.05,4.0),(0,1.35,1.75),ortho=6.70)
reading['purpose']='Full reading passage camera, ahead of student and above rear stones.';stones['purpose']='Elevated tray interaction, ahead of student; use top-facing word text anchor.'
S.render.engine='CYCLES';S.cycles.samples=48;S.cycles.use_denoising=True;S.render.resolution_x=1520;S.render.resolution_y=720;S.render.resolution_percentage=100;S.render.image_settings.file_format='PNG'
S['primary_resolution']=[1520,720];S['orientation']='Landscape first, 19:9';S['unity_integration']='Pending Blender approval. No PlayerSettings or gameplay changes.';S['speech']='Optional blank housings; dynamic Unity dialogue only.'
# Exact-resolution visual checks; preview labels are repositioned by camera as Unity will do.
S.camera=overview;S.render.filepath=str(OUT/'01_classroom_landscape_1520x720.png');bpy.ops.render.render(write_still=True)
S.camera=reading;S.render.filepath=str(OUT/'02_reading_landscape_1520x720.png');bpy.ops.render.render(write_still=True)
for ob in labels:ob.location=top.location;ob.rotation_euler=top.rotation_euler
S.camera=stones;S.render.filepath=str(OUT/'03_stones_landscape_1520x720.png');bpy.ops.render.render(write_still=True)
# Restore production-clean defaults. Previews may be toggled for review but are excluded from exports.
for ob in labels:ob.location=front.location;ob.rotation_euler=front.rotation_euler
preview.hide_render=True;preview.hide_viewport=True
S.camera=overview
bpy.ops.file.pack_all();bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_Landscape_1520_Refined.blend'))
# Independent reusable source libraries; source template stays hidden in the room.
bpy.data.libraries.write(str(OUT/'Dashboard_Landscape_Asset.blend'),{console},fake_user=True)

bpy.data.libraries.write(str(OUT/'VariableWidth_Stone_Asset.blend'),{source},fake_user=True)
for name in ['STUDENT','ENCOURAGER','ZOMBIE']:
 root=bpy.data.objects['SR_CHARACTER_'+name];bpy.data.libraries.write(str(OUT/('Character_'+name+'.blend')),{root}|set(root.children_recursive),fake_user=True)
# Frustum audit: the student's entire geometry must be behind both gameplay cameras.
student=bpy.data.objects['SR_CHARACTER_STUDENT']
audit={}
for cam in [overview,reading,stones]:
 depths=[(cam.matrix_world.inverted()@(ob.matrix_world@Vector(v))).z for ob in student.children_recursive if ob.type=='MESH' and not ob.hide_render for v in ob.bound_box]
 audit[cam.name]={'student_behind_camera':all(z>0 for z in depths),'student_depth_range_camera_z':[min(depths),max(depths)],'position':list(cam.location),'rotation_radians':list(cam.rotation_euler),'type':cam.data.type,'orthographic_width_m':cam.data.ortho_scale if cam.data.type=='ORTHO' else None}
assert audit[reading.name]['student_behind_camera'] and audit[stones.name]['student_behind_camera']
required=['SR_DISPLAY_MAIN','SR_STONE_PREFAB_SOURCE','SR_STONE_TEXT_ANCHOR','SR_STONE_TEXT_ANCHOR_TOP','SR_STONE_TRAY_ANCHOR','SR_STONE_PLACEMENT_REGION','SR_ANCHOR_ASSIGNMENT_HEADER','SR_ANCHOR_SENTENCE_PROGRESS','SR_ANCHOR_MULTIPLAYER','SR_ANCHOR_READING_CONTENT','SR_PANEL_POINTS','SR_BUTTON_MENU','SR_BUTTON_MULTIPLAYER']
report={'source':bpy.data.filepath,'resolution':[1520,720],'aspect':'19:9 landscape','required_names':{n:n in bpy.data.objects for n in required},'tray_depth_before_m':1.97,'tray_depth_after_m':2.87,'usable_region_m':[6.1,1.9],'example_stone_count':len(words),'example_row_count':len(rows),'example_row_words':[[w for w,wi in row] for row in rows],'character_triangles_before':before,'character_triangles_after':after,'character_mesh_renderers_before':groups_before,'character_mesh_renderers_after':groups_after,'camera_audit':audit,'production_previews_disabled':preview.hide_render and old_demo.hide_render,'optional_speech_disabled':optional.hide_render,'long_passage_example_words':len(passage.split()),'passage_example_lines':len(lines),'production_surface_materials':[m.name for m in surface.data.materials],'unity_modified':False,'frame_rate_verified':False,'android_orientation_next_step':'Both landscape flags are currently enabled, alongside portrait flags. After Blender approval, restrict autorotation to left/right landscape. No settings changed.'}
assert all(report['required_names'].values())
(OUT/'validation.json').write_text(json.dumps(report,indent=2))
manifest={cn:[{'name':o.name,'type':o.type,'parent':o.parent.name if o.parent else None} for o in bpy.data.collections[cn].objects] for cn in ['SR_Console','SR_Characters','SR_Decor','SR_StoneSource','SR_OPTIONAL_SPEECH_BUBBLES']};(OUT/'object_manifest.json').write_text(json.dumps(manifest,indent=2))
print(json.dumps(report))
