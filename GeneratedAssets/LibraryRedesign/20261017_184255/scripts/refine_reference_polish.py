import bpy,math,json
from pathlib import Path
from mathutils import Vector
OUT=Path(r'C:/Users/11113930/sReader4/GeneratedAssets/ReadingDashboard/20261018_ReferencePolish');S=bpy.context.scene
# Hide foreground facade pieces only for this cutaway; retain every original mesh.
front=[]
for ob in bpy.data.collections['Architecture'].objects:
 if ob.type not in {'MESH','CURVE','FONT'}:continue
 coords=[ob.matrix_world@Vector(v) for v in ob.bound_box]
 if min(p.y for p in coords)<-3.78 and max(p.z for p in coords)>.65:
  ob.hide_render=True;front.append(ob.name)
# Shelves remain along the left wall, clear of the seated encourager.
for ob in bpy.data.collections['Interior'].objects:
 if 'Bookshelf_Module' in ob.name:ob.location.x-=.48
# Present the whole group with breathing room rather than clipped furniture.
cam=bpy.data.objects['CAM_REFERENCE_COMPARISON'];cam.data.ortho_scale=9.55
cam.location=(0,-3.65,3.30);cam.rotation_euler=(Vector((0,1.7,2.5))-cam.location).to_track_quat('-Z','Y').to_euler()
S.view_settings.exposure=-1.35
fontpath=Path('C:/Windows/Fonts/arialbd.ttf')
font=bpy.data.fonts.load(str(fontpath)) if fontpath.exists() else None
# Unlit lettering is restricted to removable preview content.
def ink(name,rgb):
 m=bpy.data.materials.new(name);m.use_nodes=True;n=m.node_tree.nodes;n.clear();e=n.new('ShaderNodeEmission');e.inputs[0].default_value=(*rgb,1);e.inputs[1].default_value=.65;o=n.new('ShaderNodeOutputMaterial');m.node_tree.links.new(e.outputs[0],o.inputs[0]);return m
navy=ink('PREVIEW_Legible_Navy',(.005,.02,.075));white=ink('PREVIEW_Legible_White',(1,.98,.9));gold=ink('PREVIEW_Legible_Gold',(1,.58,.025))
for ob in bpy.data.collections['SR_Demo'].objects:
 if ob.type=='FONT':
  if font:ob.data.font=font
  ob.data.materials.clear();ob.data.materials.append(white if any(k in ob.name for k in ['Menu_Label','Multiplayer']) else gold if 'Points_Value' in ob.name else navy)
  if ob.name.startswith('DEMO_Word'):ob.data.size*=1.08
# Expand title card for bold font without running past its edges.
ob=bpy.data.objects.get('DEMO_Sentence');ob.data.size=.158
# The main student has tight, dark curls rather than scattered isolated tufts.
rt=bpy.data.objects['SR_CHARACTER_STUDENT'];head=next(c for c in rt.children if 'Head_Pivot' in c.name)
mat=bpy.data.materials['SR_REF_hair']
bpy.ops.mesh.primitive_uv_sphere_add(segments=24,ring_count=12,radius=1);hair=bpy.context.object
for c in list(hair.users_collection):c.objects.unlink(hair)
bpy.data.collections['SR_Characters'].objects.link(hair);hair.name='Student_Hair_Solid_Cap';hair.parent=head;hair.location=(0,.005,.18);hair.scale=(.30,.255,.215);hair.data.materials.append(mat)
for f in hair.data.polygons:f.use_smooth=True
# Make the friendly face more visible from the presentation view.
for n in ['SR_CHARACTER_ENCOURAGER','SR_CHARACTER_ZOMBIE']:
 ob=bpy.data.objects[n];ob.location.y=.27
# Export-ready source collections remain text-free except permanent sReader branding.
S.camera=cam;S.cycles.samples=64;S.render.resolution_x=1536;S.render.resolution_y=1120;S.render.filepath=str(OUT/'04_reference_refined.png')
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_ReadingHouse_Reference_v03.blend'))
bpy.ops.render.render(write_still=True)
# Full portrait overview; camera cutaway has no foreground facade occlusion.
da=bpy.data.cameras.new('CAM_ROOM_OVERVIEW_PORTRAIT');pc=bpy.data.objects.new('CAM_ROOM_OVERVIEW_PORTRAIT',da);bpy.data.collections['SR_Cameras'].objects.link(pc);pc.location=cam.location;pc.rotation_euler=cam.rotation_euler;da.type='ORTHO';da.ortho_scale=17.3
S.camera=pc;S.render.resolution_x=720;S.render.resolution_y=1520;S.render.filepath=str(OUT/'05_portrait_room.png');bpy.ops.render.render(write_still=True)
S.camera=cam;S.render.resolution_x=1536;S.render.resolution_y=1120
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_ReadingHouse_Reference_v03.blend'))
for name in ['STUDENT','ENCOURAGER','ZOMBIE']:
 rt=bpy.data.objects['SR_CHARACTER_'+name];bpy.data.libraries.write(str(OUT/('Character_'+name+'.blend')),{rt}|set(rt.children_recursive),fake_user=True)
print(json.dumps({'saved':bpy.data.filepath,'render':str(OUT/'04_reference_refined.png'),'front_cutaway_hidden':front}))
