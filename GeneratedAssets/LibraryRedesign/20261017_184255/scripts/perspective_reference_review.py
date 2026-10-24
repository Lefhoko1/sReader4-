import bpy,math,json
from pathlib import Path
from mathutils import Vector
OUT=Path(r'C:/Users/11113930/sReader4/GeneratedAssets/ReadingDashboard/20261018_ReferencePolish');S=bpy.context.scene
# Perspective exposes the book spines on the side wall, unlike a parallel projection.
cam=bpy.data.objects['CAM_REFERENCE_COMPARISON'];cam.data.type='PERSP';cam.data.lens=40;cam.location=(0,-7.8,3.85);cam.rotation_euler=(Vector((0,1.7,2.45))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.clip_start=.05
for ob in [bpy.data.objects['SR_CHARACTER_ENCOURAGER'],bpy.data.objects['SR_CHARACTER_ZOMBIE']]:ob.location.y=.95
# Keep each chair underneath its seated character.
for ob in bpy.data.collections['SR_Decor'].objects:
 if ob.name.startswith('SR_Chair') or ob.name.startswith('Chair_Leg'):ob.location.y+=.38
for n in ['DEMO_Encourager_Bubble','DEMO_Encourager_Dialogue','DEMO_Zombie_Bubble','DEMO_Zombie_Dialogue']:bpy.data.objects[n].location.y+=.70
# Rebuild the encourager's mouth as a wide curved smile rather than an oval.
rt=bpy.data.objects['SR_CHARACTER_ENCOURAGER'];head=next(o for o in rt.children if 'Head_Pivot' in o.name)
for ob in rt.children_recursive:
 if any(ob.name.startswith(p) for p in ['Encourager_Smile','Encourager_Teeth','Encourager_Lip']):ob.hide_render=True
for n,poly,material,yy in [('Encourager_Smile_Curve',[(-.12,-.117),(-.06,-.139),(0,-.145),(.06,-.139),(.12,-.117),(.10,-.172),(.055,-.207),(0,-.215),(-.055,-.207),(-.10,-.172)],'SR_REF_hair',-.251),('Encourager_Smile_Teeth',[(-.104,-.128),(-.05,-.148),(0,-.153),(.05,-.148),(.104,-.128),(.084,-.158),(.04,-.174),(-.04,-.174),(-.084,-.158)],'SR_REF_eye',-.259)]:
 me=bpy.data.meshes.new(n);me.from_pydata([(x,yy,z) for x,z in poly],[],[tuple(range(len(poly)))]);me.materials.append(bpy.data.materials[material]);ob=bpy.data.objects.new(n,me);bpy.data.collections['SR_Characters'].objects.link(ob);ob.parent=head
S.camera=cam;S.render.resolution_x=1536;S.render.resolution_y=1120;S.cycles.samples=64;S.render.filepath=str(OUT/'08_perspective_final.png');bpy.ops.render.render(write_still=True)
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_ReadingHouse_Reference_FINAL.blend'))
print(json.dumps({'saved':bpy.data.filepath,'render':S.render.filepath}))
