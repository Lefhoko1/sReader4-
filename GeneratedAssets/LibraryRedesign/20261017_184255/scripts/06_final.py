import bpy,json,math
from pathlib import Path
from mathutils import Vector
ROOT=Path(__file__).resolve().parents[1]
exec(compile((ROOT/'scripts'/'common.py').read_text(),str(ROOT/'scripts'/'common.py'),'exec'))
# Four reusable bookshelf modules retain all visible books and open compartments.
parts=[o for o in C['Interior'].objects if o.type=='MESH' and o.name.startswith(('SR_BOOK_','SR_SHELF_plank_','SR_Shelf_'))]
def center(o):
 pts=[o.matrix_world@Vector(v) for v in o.bound_box]
 return sum(pts,Vector())/8
bpy.context.view_layer.update()
modules=[]
for side in [-1,1]:
 for row,cy in enumerate([-1.6,1.2]):
  module_name='SR_Bookshelf_Module_'+('West' if side<0 else 'East')+'_'+str(row+1)
  if bpy.data.objects.get(module_name):
   modules.append(module_name);continue
  parts=[o for o in C['Interior'].objects if o.type=='MESH' and o.name.startswith(('SR_BOOK_','SR_SHELF_plank_','SR_Shelf_'))]
  group=[o for o in parts if (center(o).x*side>0) and ((center(o).y<-.2)==(row==0))]
  assert len(group)>30,'Incomplete modular shelf selection'
  bpy.ops.object.select_all(action='DESELECT')
  for o in group:o.select_set(True)
  bpy.context.view_layer.objects.active=group[0]
  bpy.ops.object.convert(target='MESH')
  bpy.ops.object.join()
  o=bpy.context.object;o.name='SR_Bookshelf_Module_'+('West' if side<0 else 'East')+'_'+str(row+1)
  oldcursor=S.cursor.location.copy();S.cursor.location=(side*3.55,cy,.34);bpy.ops.object.origin_set(type='ORIGIN_CURSOR');S.cursor.location=oldcursor
  o['purpose']='Reusable open bookshelf module. Shared palette; books and shelves batched into one mesh. Raw source geometry retained in Backup and v04.'
  modules.append(o.name)
bpy.ops.object.select_all(action='DESELECT')
# Final overview clears the front wall using the view angle alone.
cam=bpy.data.objects['SR_Camera_Interior'];cam.location=(1,-3.3,18.5);cam.rotation_euler=(Vector((0,-.1,.7))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.ortho_scale=13.6
hero=bpy.data.objects['SR_Camera_Hero'];hero.location=(3,-3.75,5.8);hero.rotation_euler=(Vector((.3,-.3,.95))-hero.location).to_track_quat('-Z','Y').to_euler();hero.data.lens=27
# 4:3 inspection renders; named cameras remain editable for Unity framing.
S.render.resolution_x=1600;S.render.resolution_y=1200;S.render.resolution_percentage=100
S.cycles.samples=64;S.cycles.use_denoising=True
# Triangle/material budget includes geometry after bevels and curve tessellation.
C['Roof'].hide_viewport=False;C['Roof'].hide_render=False
bpy.context.view_layer.update();dg=bpy.context.evaluated_depsgraph_get()
triangles=0;objects=0;usedmats=set();breakdown={}
for name,col in C.items():
 if name=='Backup':continue
 ct=0
 for o in col.objects:
  if o.type not in {'MESH','CURVE','FONT','SURFACE','META'}:continue
  ev=o.evaluated_get(dg)
  me=ev.to_mesh()
  if me:
   me.calc_loop_triangles();ct+=len(me.loop_triangles);objects+=1
   usedmats.update(m.name for m in me.materials if m)
  ev.to_mesh_clear()
 triangles+=ct;breakdown[name]=ct
report={'original_objects_preserved':len(C['Backup'].objects),'render_geometry_objects':objects,'evaluated_triangles':triangles,'triangles_by_collection':breakdown,'active_materials':len(usedmats),'lights':4,'shelf_modules':modules,'backup_before_changes':str(ROOT/'sReader_Library_v00_original.blend'),'final_file':str(ROOT/'sReader_Library_FINAL.blend'),'unity_code_changes':False}
assert report['original_objects_preserved']==642
assert len(modules)==4
(ROOT/'scene_report.json').write_text(json.dumps(report,indent=2))
C['Roof'].hide_render=True;C['Roof'].hide_viewport=True;S.camera=cam
S.render.filepath=str(ROOT/'renders'/'02_interior_overview.png')
S['roof_visibility']='Hide/show the Roof collection using both its viewport and render toggles. All roof geometry and gable ornaments are in that collection.'
S['scope']='Blender architectural redesign and presentation. No Unity gameplay/backend/multiplayer changes. Render lights need a baked Unity lighting pass for Android.'
# Final is saved before rendering, and again after restoring the intended default state.
bpy.ops.wm.save_as_mainfile(filepath=str(ROOT/'sReader_Library_v05_modular_final.blend'))
bpy.ops.wm.save_as_mainfile(filepath=str(ROOT/'sReader_Library_FINAL.blend'))
for camera_name,file_name,roof in [('SR_Camera_Exterior','01_exterior_front.png',True),('SR_Camera_Interior','02_interior_overview.png',False),('SR_Camera_Hero','03_hero_activity.png',False)]:
 C['Roof'].hide_render=not roof;S.camera=bpy.data.objects[camera_name];S.render.filepath=str(ROOT/'renders'/file_name)
 bpy.ops.render.render(write_still=True)
 print('FINAL_RENDER',file_name)
C['Roof'].hide_render=True;C['Roof'].hide_viewport=True;S.camera=cam;S.render.filepath=str(ROOT/'renders'/'02_interior_overview.png')
for screen in bpy.data.screens:
 for a in screen.areas:
  if a.type=='VIEW_3D':a.spaces.active.region_3d.view_perspective='CAMERA'
bpy.ops.wm.save_as_mainfile(filepath=str(ROOT/'sReader_Library_FINAL.blend'))
print(json.dumps(report))
