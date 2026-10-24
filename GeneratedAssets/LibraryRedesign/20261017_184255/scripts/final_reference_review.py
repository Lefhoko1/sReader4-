import bpy,math,json
from pathlib import Path
from mathutils import Vector
OUT=Path(r'C:/Users/11113930/sReader4/GeneratedAssets/ReadingDashboard/20261018_ReferencePolish');S=bpy.context.scene
# Clear the front shelf side-panel from the reference camera while keeping three rear modules.
shelves=sorted([o for o in bpy.data.collections['Interior'].objects if 'Bookshelf_Module' in o.name],key=lambda o:o.location.y)
for i,o in enumerate(shelves):
 if i==0:o.hide_render=True;o.hide_set(True)
 else:o.location.y+=.30
# A cream gable infill closes the roof-hidden cutaway above the tall display.
me=bpy.data.meshes.new('SR_Cutaway_Back_Infill');me.from_pydata([(-4.36,3.90,4.46),(4.36,3.90,4.46),(0,3.90,6.46)],[],[(0,1,2)]);me.materials.append(bpy.data.materials['SR_Parchment'])
o=bpy.data.objects.new('SR_Rear_Gable_Infill',me);bpy.data.collections['SR_Decor'].objects.link(o)
# Restore the illustration's brightness after the exposure correction.
m=bpy.data.materials['SR_PREVIEW_ONLY_Landscape'];next(n for n in m.node_tree.nodes if n.type=='EMISSION').inputs['Strength'].default_value=2.15
for name,strength in [('PREVIEW_Legible_White',3.2),('PREVIEW_Legible_Gold',2.1)]:
 m=bpy.data.materials[name];next(n for n in m.node_tree.nodes if n.type=='EMISSION').inputs['Strength'].default_value=strength
m=bpy.data.materials['PREVIEW_Legible_White']
o=bpy.data.objects['DEMO_Points_Label'];o.data.materials.clear();o.data.materials.append(m)
# Larger student silhouette and warmer, smiling encourager expression.
student=bpy.data.objects['SR_CHARACTER_STUDENT'];student.scale=(1.16,1.16,1.16)
for o in bpy.data.objects['SR_CHARACTER_ENCOURAGER'].children_recursive:
 if o.name=='Encourager_Smile':o.scale.z*=.68;o.scale.x*=1.12
 if o.name=='Encourager_Teeth':o.location.z-=.01
# Bring the cap brim up slightly so the encourager's bright eyes read clearly.
for o in bpy.data.objects['SR_CHARACTER_ENCOURAGER'].children_recursive:
 if 'Cap_Brim' in o.name:o.location.z+=.035
# Update all removable speech-card typography for a clearer comparison.
for n in ['DEMO_Encourager_Dialogue','DEMO_Zombie_Dialogue']:
 o=bpy.data.objects[n];o.data.size=.14
# A real reusable moss base accompanies the three-part stone source.
c=bpy.data.collections['SR_StoneSource'];rt=bpy.data.objects['SR_STONE_PREFAB_SOURCE']
for side in [-1,1]:
 bpy.ops.mesh.primitive_uv_sphere_add(segments=16,ring_count=8,radius=1);o=bpy.context.object
 for cl in list(o.users_collection):cl.objects.unlink(o)
 c.objects.link(o);o.name='SR_STONE_MOSS_CAP_LEFT' if side<0 else 'SR_STONE_MOSS_CAP_RIGHT';o.parent=rt;o.location=(side*.16,.02,-.21);o.scale=(.18,.13,.065);o.data.materials.append(bpy.data.materials['SR_REF_moss'])
 for p in o.data.polygons:p.use_smooth=True
bpy.ops.mesh.primitive_cube_add(size=1);o=bpy.context.object
for cl in list(o.users_collection):cl.objects.unlink(o)
c.objects.link(o);o.name='SR_STONE_MOSS_STRETCH_CENTRE';o.parent=rt;o.location=(0,.02,-.21);o.scale=(.32,.24,.09);o.data.materials.append(bpy.data.materials['SR_REF_moss'])
# Actual evaluated triangle counts for handoff; no performance claims.
dg=bpy.context.evaluated_depsgraph_get();counts={}
for cn in ['SR_Console','SR_Characters','SR_Decor','SR_StoneSource']:
 triangles=0
 for o in bpy.data.collections[cn].objects:
  if o.type=='MESH':
   ev=o.evaluated_get(dg);me=ev.to_mesh();triangles+=sum(len(f.vertices)-2 for f in me.polygons);ev.to_mesh_clear()
 counts[cn]=triangles
S.camera=bpy.data.objects['CAM_REFERENCE_COMPARISON'];S.cycles.samples=64;S.render.resolution_x=1536;S.render.resolution_y=1120;S.render.filepath=str(OUT/'06_reference_final.png')
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_ReadingHouse_Reference_FINAL.blend'));bpy.ops.render.render(write_still=True)
# Regenerate mobile overview from the final geometry and presentation materials.
S.camera=bpy.data.objects['CAM_ROOM_OVERVIEW_PORTRAIT'];S.render.resolution_x=720;S.render.resolution_y=1520;S.render.filepath=str(OUT/'07_portrait_final.png');bpy.ops.render.render(write_still=True)
S.camera=bpy.data.objects['CAM_REFERENCE_COMPARISON'];S.render.resolution_x=1536;S.render.resolution_y=1120
for name,cn in [('Dashboard_Asset','SR_Console'),('Variable_Width_Stone_Asset','SR_StoneSource')]:bpy.data.libraries.write(str(OUT/(name+'.blend')),{bpy.data.collections[cn]},fake_user=True)
for name in ['STUDENT','ENCOURAGER','ZOMBIE']:
 rt=bpy.data.objects['SR_CHARACTER_'+name];bpy.data.libraries.write(str(OUT/('Character_'+name+'.blend')),{rt}|set(rt.children_recursive),fake_user=True)
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_ReadingHouse_Reference_FINAL.blend'))
manifest=json.loads((OUT/'manifest.json').read_text());manifest['source']=bpy.data.filepath;manifest['evaluated_triangle_counts_new_assets']=counts;manifest['latest_render']='06_reference_final.png';manifest['preview_landscape']='preview_landscape.png';manifest['stone_source_objects']=[o.name for o in c.objects];(OUT/'manifest.json').write_text(json.dumps(manifest,indent=2))
print(json.dumps({'saved':bpy.data.filepath,'triangles':counts,'render':str(OUT/'06_reference_final.png')}))
