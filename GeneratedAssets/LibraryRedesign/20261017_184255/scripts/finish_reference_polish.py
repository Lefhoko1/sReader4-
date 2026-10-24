import bpy,math,json,bmesh
from pathlib import Path
from mathutils import Vector
OUT=Path(r'C:/Users/11113930/sReader4/GeneratedAssets/ReadingDashboard/20261018_ReferencePolish');S=bpy.context.scene
# Free the required production names from the archived greybox.
old=bpy.data.collections.get('SR_PRODUCTION_Dashboard_Greybox')
for ob in old.objects:ob.name='ARCHIVED_Greybox_'+ob.name
required=['SR_DASHBOARD_ROOT','SR_DISPLAY_MAIN','SR_BUTTON_MULTIPLAYER','SR_BUTTON_MENU','SR_PANEL_POINTS','SR_ANCHOR_ASSIGNMENT_HEADER','SR_ANCHOR_SENTENCE_PROGRESS','SR_ANCHOR_MULTIPLAYER','SR_ANCHOR_READING_CONTENT','SR_STONE_TRAY_ANCHOR','SR_STONE_PLACEMENT_REGION','SR_ANCHOR_POINTS','SR_ANCHOR_MENU']
for name in required:
 for colname in ['SR_Console']:
  for ob in bpy.data.collections[colname].objects:
   if ob.name.split('.')[0]==name:ob.name=name
# Scenic content is a removable preview plane; production SR_DISPLAY_MAIN stays untextured.
img=bpy.data.images.load(str(OUT/'preview_landscape.png'),check_existing=True)
mat=bpy.data.materials.new('SR_PREVIEW_ONLY_Landscape');mat.use_nodes=True
nodes=mat.node_tree.nodes;nodes.clear();tex=nodes.new('ShaderNodeTexImage');tex.image=img
emit=nodes.new('ShaderNodeEmission');emit.inputs['Strength'].default_value=.85
out=nodes.new('ShaderNodeOutputMaterial');mat.node_tree.links.new(tex.outputs['Color'],emit.inputs['Color']);mat.node_tree.links.new(emit.outputs[0],out.inputs[0])
# A rounded image mesh with UV coordinates, placed behind the preview sentence card.
w,h,r=5.98,2.08,.13;verts=[(0,2.455,2.91)];uv=[(.5,.5)]
for cx,cz,start in [(w/2-r,-h/2+r,-math.pi/2),(w/2-r,h/2-r,0),(-w/2+r,h/2-r,math.pi/2),(-w/2+r,-h/2+r,math.pi)]:
 for i in range(9):
  a=start+i*math.pi/16;x=cx+r*math.cos(a);z=cz+r*math.sin(a);verts.append((x,2.455,2.91+z));uv.append((x/w+.5,z/h+.5))
faces=[(0,i,1 if i==len(verts)-1 else i+1) for i in range(1,len(verts))]
me=bpy.data.meshes.new('PREVIEW_Illustration_Mesh');me.from_pydata(verts,[],faces);me.materials.append(mat);layer=me.uv_layers.new(name='UVMap')
for f in me.polygons:
 for li in f.loop_indices:layer.data[li].uv=uv[me.loops[li].vertex_index]
ob=bpy.data.objects.new('DEMO_Contextual_Illustration',me);bpy.data.collections['SR_Demo'].objects.link(ob)
# Non-overlapping UV coordinates for procedural single-colour production materials.
for cname in ['SR_Console','SR_Characters','SR_Decor','SR_StoneSource']:
 for ob in bpy.data.collections[cname].objects:
  if ob.type!='MESH':continue
  me=ob.data
  if not me.uv_layers:
   lay=me.uv_layers.new(name='UVMap')
   for p in me.polygons:
    axis=max(range(3),key=lambda a:abs(p.normal[a]));a,b=[i for i in range(3) if i!=axis]
    for li in p.loop_indices:
     co=me.vertices[me.loops[li].vertex_index].co;lay.data[li].uv=(co[a]*.1+.5,co[b]*.1+.5)
# Decorative chair legs belong with furniture, never with reusable characters.
for ob in list(bpy.data.collections['SR_Characters'].objects):
 if ob.name.startswith('Chair_Leg'):
  bpy.data.collections['SR_Characters'].objects.unlink(ob);bpy.data.collections['SR_Decor'].objects.link(ob)
# Dedicated portrait framing crops to the interaction area, supporting runtime pagination.
da=bpy.data.cameras.new('CAM_MOBILE_READING');cam=bpy.data.objects.new('CAM_MOBILE_READING',da);bpy.data.collections['SR_Cameras'].objects.link(cam)
cam.location=(0,-2.7,3.25);cam.rotation_euler=(Vector((0,2.3,2.95))-cam.location).to_track_quat('-Z','Y').to_euler();da.type='ORTHO';da.ortho_scale=5.5
# Save packed source, plus independently loadable asset libraries.
img.pack();img.filepath='//preview_landscape.png'
S.camera=bpy.data.objects['CAM_REFERENCE_COMPARISON'];S.cycles.samples=64
S.render.resolution_x=1536;S.render.resolution_y=1120;S.render.filepath=str(OUT/'02_reference_polished.png')
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_ReadingHouse_Reference_v02.blend'))
bpy.ops.render.render(write_still=True)
for name,cn in [('Dashboard_Asset','SR_Console'),('Variable_Width_Stone_Asset','SR_StoneSource')]:bpy.data.libraries.write(str(OUT/(name+'.blend')),{bpy.data.collections[cn]},fake_user=True)
for name in ['STUDENT','ENCOURAGER','ZOMBIE']:
 rt=bpy.data.objects['SR_CHARACTER_'+name];objects={rt}|set(rt.children_recursive);bpy.data.libraries.write(str(OUT/('Character_'+name+'.blend')),objects,fake_user=True)
# Portrait proof deliberately does not claim final runtime readability.
S.camera=cam;S.render.resolution_x=720;S.render.resolution_y=1520;S.render.filepath=str(OUT/'03_portrait_reading.png');bpy.ops.render.render(write_still=True)
S.camera=bpy.data.objects['CAM_REFERENCE_COMPARISON'];S.render.resolution_x=1536;S.render.resolution_y=1120
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_ReadingHouse_Reference_v02.blend'))
manifest={'source':bpy.data.filepath,'reference':'Docs/Photosynthesis Reading Adventure.png','preview_only_collection':'SR_Demo','required_names':{n:n in bpy.data.objects for n in required},'collections':{cn:[{'name':o.name,'type':o.type,'parent':o.parent.name if o.parent else None,'mesh_polygons':len(o.data.polygons) if o.type=='MESH' else None} for o in bpy.data.collections[cn].objects] for cn in ['SR_Console','SR_StoneSource','SR_Characters','SR_Decor']},'note':'Mobile performance untested; character assets are separate mesh hierarchies, not rigged. Unity exports await visual review.'}
(OUT/'manifest.json').write_text(json.dumps(manifest,indent=2))
(OUT/'HANDOFF.md').write_text('Reference polish v02\n\nProduction: SR_Console, SR_Characters, SR_Decor. SR_StoneSource is hidden to avoid displaying the reusable template. All sample text, screen imagery, speech bubbles and word-stone examples are in SR_Demo; exclude it from exports. SR_DISPLAY_MAIN is clean and untextured.\n\nStone width: fixed hemisphere caps (radius X .18m) move to +/- centreWidth/2; SR_STONE_STRETCH_CENTRE scales only in X. Total width = centreWidth + .36m. Text fits one word and attaches at SR_STONE_TEXT_ANCHOR; Unity computes widths and wraps/paginates across the 6.1 x 1.35m region. Six material variants are supplied. Normal meshes use opaque single-colour materials.\n\nAssignment and score displays are preview examples only. Unity should use existing assignment/scoring systems and TMP/world-space Canvas at named anchors. Portrait framing is a geometry review, not physical Huawei testing. No Unity code or existing exported models were changed. Character roots allow independent loading and later rigging; no skeleton/animations yet.\n\nExterior and roof are preserved but hidden for cutaway interior presentation. Original source and previous greybox are backed up. Re-enable original architecture render flags and Roof for exterior views.\n')
(OUT/'IMAGE_PROMPT.txt').write_text('Built-in image_gen; illustration-story. Removable preview scene illustration only. Lush panoramic casual puzzle game landscape, turquoise river, green meadows, layered blue mountains, cyan sky, leafy trees framing both sides and flowers in bottom corners. Quiet pale blue centre for text. Warm sunlight, polished family animation forms. No people, no room, no UI, no text. Saved as preview_landscape.png.\n')
print(json.dumps({'saved':bpy.data.filepath,'required_names_valid':all(manifest['required_names'].values()),'renders':[str(p) for p in OUT.glob('0*.png')]}))
