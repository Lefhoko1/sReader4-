import bpy,math,json
from pathlib import Path
from mathutils import Vector,Matrix
OUT=Path(__file__).resolve().parents[1]
COMMON=Path(r'C:/Users/11113930/sReader4/GeneratedAssets/LibraryRedesign/20261017_184255/scripts/common.py')
assert 'sReader_Library_FINAL.blend' in bpy.data.filepath, 'Unexpected Blender source; inspect before running.'
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_Classroom_v00_before_layout.blend'))
exec(compile(COMMON.read_text(),str(COMMON),'exec'))
# Apply the same measured classroom arrangement in the native Blender source.
def center(o):return sum((o.matrix_world@Vector(v) for v in o.bound_box),Vector())/8
shelves=sorted([o for o in bpy.context.scene.objects if o.name.startswith('SR_Bookshelf_Module_')],key=lambda o:o.name)
for ob,row in zip(shelves,[-2.76,-.92,.92,2.76]):
 c=center(ob)
 ob.matrix_world=Matrix.Translation(c)@Matrix.Rotation(math.pi if 'East' in ob.name else 0,4,'Z')@Matrix.Diagonal((1,.74,1,1))@Matrix.Translation(-c)@ob.matrix_world
 bpy.context.view_layer.update();c=center(ob);ob.location+=Vector((-3.55,row,c.z))-c
 ob['source_role']='Classroom bookshelf wall; source for Unity FBX'
# Chalkboard is real source geometry, mounted against the east wall.
col=bpy.data.collections.new('Chalkboard');bpy.context.scene.collection.children.link(col);C['Chalkboard']=col
P['board']=mat('Classroom_Chalkboard','263C32')
def boardbox(name,pos,size,material):
 x,y,z=pos;w,h,d=size
 return box(name,(4.24+z,.1-x,2.25+y),(d,w,h),material,'Chalkboard',.012)
boardbox('SR_Classroom_Board_Surface',(0,0,0),(6.15,3.03,.12),'board')
for side in [-1,1]:
 boardbox('SR_Classroom_Board_Frame_V',(side*3.11,0,-.10),(.16,3.25,.17),'wood')
 boardbox('SR_Classroom_Board_Frame_H',(0,side*1.56,-.10),(6.35,.16,.17),'wood')
for x in [-1,1]:
 for y in [-1,1]:
  boardbox('SR_Classroom_Board_Gold_Corner',(x*3.10,y*1.55,-.20),(.21,.21,.035),'gold')
  boardbox('SR_Classroom_Board_Corner_Inset',(x*3.10,y*1.55,-.225),(.09,.09,.02),'board')
boardbox('SR_Classroom_Chalk_Tray',(0,-1.62,-.24),(5.8,.11,.36),'wood')
boardbox('SR_Classroom_Chalk',(-.5,-1.54,-.25),(.21,.055,.055),'paper')
# Source socket for live Unity lettering; Blender preview writing is excluded from export.
socket=bpy.data.objects.new('SOCKET_Chalkboard_Live_Text',None);col.objects.link(socket);socket.location=(4.06,.1,2.25);socket.empty_display_size=.25
ann=bpy.data.collections.new('Preview Assignment Writing');bpy.context.scene.collection.children.link(ann);C['Preview Assignment Writing']=ann
q=Matrix(((0,0,-1),(-1,0,0),(0,1,0))).to_quaternion()
for name,body,height,size in [('Kicker',"TODAY'S READING",3.51,.105),('Title','Meadow at Dawn',3.20,.155),('Words','Words to restore',2.86,.115),('Word0','dew',2.63,.15),('Word1','habitat',2.34,.15),('Word2','butterfly',2.05,.15),('Word3','nectar',1.76,.15),('Counter','Words completed  0 / 4',1.17,.115),('Note','Practice passage',1.0,.08)]:
 ob=text('PREVIEW_'+name,body,(4.035,.1,height),size,'paper','Preview Assignment Writing');ob.rotation_euler=q.to_euler();ob['note']='Blender-only sample lettering. Unity Canvas receives current assignment data.'
# Roof lining and modest rafters improve the indoor ceiling without adding a ceiling slab.
for side in [-1,1]:
 a=math.atan2(2.15,4.95)
 o=box('SR_Classroom_Roof_Lining',(side*2.475,0,5.49),(math.hypot(4.95,2.15),8.62,.055),'wood','Roof',.012);o.rotation_euler.y=side*a
 for y in [-3.3,-1.1,1.1,3.3]:
  o=box('SR_Classroom_Rafter',(side*2.475,y,5.405),(math.hypot(4.95,2.15),.11,.13),'woodLight','Roof',.012);o.rotation_euler.y=side*a
# Cameras correspond to Unity's presentation poses, while an inspection view exposes the source layout.
for name,pos,target,fov in [('SR_Camera_Classroom_Portrait',(0,-3.55,4.3),(.45,1.2,1.5),110),('SR_Camera_Classroom_Board',(.7,.1,2.25),(4.1,.1,2.25),78),('SR_Camera_Classroom_Book',(0,-1.7,4.1),(0,.9,1.25),66)]:
 ob=camera(name,pos,target);ob.data.sensor_fit='VERTICAL';ob.data.sensor_height=32;ob.data.lens=16/math.tan(math.radians(fov/2))
proof=camera('SR_Camera_Classroom_Inspection',(-10,-14,15),(0,0,1.4),ortho=13.0)
S.camera=proof;S.render.resolution_x=1400;S.render.resolution_y=1100;S.cycles.samples=48
C['Roof'].hide_viewport=True;C['Roof'].hide_render=True
S['source_of_truth']='Classroom layout and all architectural mesh geometry are authored in this .blend. Exported FBX imported by Unity. Live assignment lettering, touch and camera animation are Unity components.'
S['unity_export']='Assets/Art/MagicalClassroom/Models/SM_MagicalClassroom.fbx'
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_Classroom_v01.blend'))
# Export only active design collections. Original backup and sample board lettering are never exported.
exportdir=Path(r'C:/Users/11113930/sReader4/Assets/Art/MagicalClassroom/Models');exportdir.mkdir(parents=True,exist_ok=True)
path=exportdir/'SM_MagicalClassroom.fbx'
assert not path.exists(), 'Refusing to replace an existing FBX without inspection.'
tmp=bpy.data.collections.new('_FBX_EXPORT_TEMP');S.collection.children.link(tmp)
renamed=[];made=[];meshes=[];dg=bpy.context.evaluated_depsgraph_get()
C['Roof'].hide_viewport=False;C['Roof'].hide_render=False;bpy.context.view_layer.update()
try:
 for cname in ['Architecture','Roof','Interior','Interactive Props','Landscaping','Chalkboard']:
  parent=bpy.data.objects.new(cname,None);tmp.objects.link(parent);made.append(parent)
  for ob in list(C[cname].objects):
   if ob.type not in {'MESH','CURVE','FONT'}:continue
   name=ob.name;ob.name='__SOURCE_'+name;renamed.append((ob,name))
   me=bpy.data.meshes.new_from_object(ob.evaluated_get(dg),preserve_all_data_layers=True,depsgraph=dg);meshes.append(me)
   copy=bpy.data.objects.new(name,me);tmp.objects.link(copy);copy.parent=parent;copy.matrix_world=ob.matrix_world;made.append(copy)
 anchors=bpy.data.objects.new('ExportAnchors',None);tmp.objects.link(anchors);made.append(anchors)
 for name,pos in [('SR_ANCHOR_ORIGIN',(0,0,0)),('SR_ANCHOR_X',(1,0,0)),('SR_ANCHOR_Y',(0,1,0)),('SR_ANCHOR_Z',(0,0,1)),('SOCKET_Chalkboard_Live_Text',(4.06,.1,2.25))]:
  ob=bpy.data.objects.new(name,None);tmp.objects.link(ob);ob.parent=anchors;ob.location=pos;made.append(ob)
 bpy.ops.object.select_all(action='DESELECT')
 for ob in made:ob.select_set(True)
 bpy.context.view_layer.objects.active=made[0]
 bpy.ops.export_scene.fbx(filepath=str(path),use_selection=True,object_types={'MESH','EMPTY'},axis_forward='-Z',axis_up='Y',global_scale=1,apply_unit_scale=True,use_mesh_modifiers=False,mesh_smooth_type='FACE',add_leaf_bones=False,bake_anim=False,path_mode='AUTO')
finally:
 for ob in made:bpy.data.objects.remove(ob,do_unlink=True)
 for me in meshes:
  if me.users==0:bpy.data.meshes.remove(me)
 bpy.data.collections.remove(tmp)
 for ob,name in renamed:ob.name=name
 C['Roof'].hide_viewport=True;C['Roof'].hide_render=True
S.camera=proof;S.render.filepath=str(OUT/'screenshots/04_blender_classroom_source.png');bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_Classroom_v01.blend'));bpy.ops.render.render(write_still=True)
# Leave Blender on the classroom source in a roof-hidden inspection view.
for screen in bpy.data.screens:
 for area in screen.areas:
  if area.type=='VIEW_3D':area.spaces.active.region_3d.view_perspective='CAMERA'
report={'blend':bpy.data.filepath,'fbx':str(path),'fbx_bytes':path.stat().st_size,'shelf_modules':len(shelves),'board_objects':len(col.objects),'preserved_originals':len(C['Backup'].objects),'runtime_text_socket':list(socket.location)}
(OUT/'reports/blender_source_export.json').write_text(json.dumps(report,indent=2));print(json.dumps(report))
