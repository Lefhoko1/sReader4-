import bpy,math,json
from pathlib import Path
from mathutils import Vector,Matrix
ROOT=Path(__file__).resolve().parents[1]
exec(compile((ROOT/'scripts'/'common.py').read_text(),str(ROOT/'scripts'/'common.py'),'exec'))
for o in C['Architecture'].objects:
 if o.name.startswith('SR_Door_Handle'):o.scale=(.045,.04,.045)
# Real side window openings replace only the two freshly generated plain wall panels.
for n in ['SR_Wall_Left','SR_Wall_Right']:bpy.data.objects.remove(bpy.data.objects[n],do_unlink=True)
for side in [-1,1]:
 before=set(bpy.data.objects)
 for cx in [-1.8,1.8]:
  window('SR_Side_Window_'+str(side)+'_'+str(cx),cx,-4.5,1.35,3.0,.68)
  wall_opening('SR_Side_Wall_Arch',cx,-4.5,1.36,1.35,3,.68,4.5)
 for lo,hi in [(-4.2,-2.48),(-1.12,1.12),(2.48,4.2)]:box('SR_Side_Wall_Pier',((lo+hi)/2,-4.5,2.42),(hi-lo,.28,4.16),'plaster')
 rot=Matrix.Rotation(side*math.pi/2,4,'Z')
 bpy.context.view_layer.update()
 for o in set(bpy.data.objects)-before:o.matrix_world=rot@o.matrix_world
# Fireplace reads as a dark opening instead of another wooden panel.
o=bpy.data.objects['SR_hearth_opening'];o.data.materials.clear();o.data.materials.append(P['ink'])
# Inset metal basket and a quiet ember glow.
for x in [-.32,-.16,0,.16,.32]:box('SR_Hearth_Grate',(x,3.54,.67),(.025,.045,.44),'slate','Interior',.008)
box('SR_Hearth_Grate_Rail',(0,3.52,.82),(.78,.05,.035),'slate','Interior',.01)
# Correct palette balance; warm fixtures against cool evening fill.
S.view_settings.exposure=0
bpy.data.lights['SR_Key_Warm'].energy=1350
bpy.data.lights['SR_Key_Warm'].color=(1,.85,.68)
bpy.data.lights['SR_Reading_Softbox'].energy=400
bpy.data.lights['SR_Reading_Softbox'].color=(1,.82,.62)
# Top-down cinematic overview includes both banks of shelves and all five stones.
cam=bpy.data.objects['SR_Camera_Interior'];cam.location=(2.7,-7,17);cam.rotation_euler=(Vector((0,0,.9))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.ortho_scale=12.1
hero=bpy.data.objects['SR_Camera_Hero'];hero.location=(5.7,-6.8,8.9);hero.rotation_euler=(Vector((.35,.15,1.0))-hero.location).to_track_quat('-Z','Y').to_euler();hero.data.lens=43
# Table legs and upholstery coordinates are audited against floor top.
C['Roof'].hide_render=True;C['Roof'].hide_viewport=True
S.camera=cam
bpy.ops.wm.save_as_mainfile(filepath=str(ROOT/'sReader_Library_v03_visual_refinement.blend'))
S.cycles.samples=24;S.render.resolution_x=1200;S.render.resolution_y=1000
for camera_name,file_name,roof in [('SR_Camera_Interior','review_interior_v2.png',False),('SR_Camera_Exterior','review_exterior.png',True),('SR_Camera_Hero','review_hero.png',False)]:
 C['Roof'].hide_render=not roof;S.camera=bpy.data.objects[camera_name];S.render.filepath=str(ROOT/'renders'/file_name);bpy.ops.render.render(write_still=True)
 print('RENDERED',file_name)
C['Roof'].hide_render=True;S.camera=cam
print('Visual refinement review complete')
