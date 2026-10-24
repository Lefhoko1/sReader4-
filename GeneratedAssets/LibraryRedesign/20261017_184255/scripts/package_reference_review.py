import bpy,math,json
from pathlib import Path
from mathutils import Vector
OUT=Path(r'C:/Users/11113930/sReader4/GeneratedAssets/ReadingDashboard/20261018_ReferencePolish');S=bpy.context.scene
prod=bpy.data.collections.new('SR_PRODUCTION_EXPORT');S.collection.children.link(prod)
for cn in ['SR_Console','SR_Characters','SR_Decor']:
 c=bpy.data.collections[cn];prod.children.link(c)
 if c.name in S.collection.children:S.collection.children.unlink(c)
prod['export_policy']='Production geometry only; original architecture exported separately. Exclude SR_Demo, all backups, all lights/cameras. Template stone asset is a separate hidden source collection.'
# Demonstrate that the same modular source serves different sentence lengths.
demo=bpy.data.collections['SR_Demo'];original_stones=[o for o in demo.objects if o.name.startswith('DEMO_Stone_')]
original_parts=[o for root in original_stones for o in [root]+list(root.children_recursive)]
for o in original_parts:o.hide_render=True
student=bpy.data.objects['SR_CHARACTER_STUDENT'];student_flags={o:o.hide_render for o in [student]+list(student.children_recursive)}
for o in student_flags:o.hide_render=True
preview=bpy.data.collections.new('SR_PREVIEW_Sentence_Lengths');demo.children.link(preview)
font=bpy.data.objects['DEMO_Sentence'].data.font
src=bpy.data.objects['SR_STONE_PREFAB_SOURCE']
def stone(word,width,pos):
 rt=bpy.data.objects.new('DEMO_DYNAMIC_WORD',None);preview.objects.link(rt);rt.location=pos;cw=max(.06,width-.36)
 for so in src.children:
  if so.type not in {'MESH'}:continue
  o=so.copy();o.data=so.data;preview.objects.link(o);o.parent=rt;o.hide_render=False
  if 'CAP_LEFT' in so.name:o.location.x=-cw/2
  elif 'CAP_RIGHT' in so.name:o.location.x=cw/2
  elif 'STRETCH_CENTRE' in so.name:o.scale.x=cw
 cu=bpy.data.curves.new('DEMO_DYNAMIC_LABEL','FONT');cu.body=word;cu.font=font;cu.size=.15;cu.align_x='CENTER';cu.align_y='CENTER';cu.materials.append(bpy.data.materials['PREVIEW_Legible_Navy'])
 ob=bpy.data.objects.new('DEMO_DYNAMIC_LABEL',cu);preview.objects.link(ob);ob.parent=rt;ob.location=(0,-.14,.015);ob.rotation_euler=(math.pi/2,0,0)
 bpy.context.view_layer.update()
 if ob.dimensions.x>width-.08:ob.scale*= (width-.08)/ob.dimensions.x
 return rt
camdata=bpy.data.cameras.new('CAM_STONES_CLOSEUP_FINAL');cam=bpy.data.objects.new('CAM_STONES_CLOSEUP_FINAL',camdata);bpy.data.collections['SR_Cameras'].objects.link(cam);cam.location=(0,-2,4.25);cam.rotation_euler=(Vector((0,2,1.95))-cam.location).to_track_quat('-Z','Y').to_euler();camdata.type='ORTHO';camdata.ortho_scale=7.0
oldbody=bpy.data.objects['DEMO_Sentence'].data.body
for sentence,filename in [('The sun shines.','09_short_sentence_stones.png'),('Photosynthesis allows green plants to convert sunlight into chemical energy.','10_long_sentence_stones.png')]:
 for o in list(preview.objects):bpy.data.objects.remove(o,do_unlink=True)
 words=sentence.split();widths=[max(.52,.24+.078*len(w)) for w in words];rows=[[]];used=0
 for w,wi in zip(words,widths):
  if used+wi>5.9 and rows[-1]:rows.append([]);used=0
  rows[-1].append((w,wi));used+=wi+.10
 for j,row in enumerate(rows):
  total=sum(w for _,w in row)+.10*(len(row)-1);xx=-total/2
  for word,wi in row:stone(word,wi,(xx+wi/2,2.46-j*.74 if len(rows)>1 else 2.10,1.94));xx+=wi+.10
 ob=bpy.data.objects['DEMO_Sentence'];ob.data.body=sentence
 S.camera=cam;S.render.resolution_x=1400;S.render.resolution_y=750;S.render.filepath=str(OUT/filename);bpy.ops.render.render(write_still=True)
preview.hide_render=True;preview.hide_viewport=True
for o in original_parts:o.hide_render=False
for o,state in student_flags.items():o.hide_render=state
bpy.data.objects['DEMO_Sentence'].data.body=oldbody
# Exterior proof with the original shell/roof enabled temporarily.
arch=bpy.data.collections['Architecture'];flags={o:o.hide_render for o in arch.objects};roof=bpy.data.collections['Roof'];roof_flag=roof.hide_render
for o in arch.objects:o.hide_render=False
roof.hide_render=False
wc=S.world.node_tree.nodes.get('Background');oldstrength=wc.inputs['Strength'].default_value;wc.inputs['Strength'].default_value=.75
oldexposure=S.view_settings.exposure;S.view_settings.exposure=0
camdata=bpy.data.cameras.new('CAM_EXTERIOR_FINAL');ext=bpy.data.objects.new('CAM_EXTERIOR_FINAL',camdata);bpy.data.collections['SR_Cameras'].objects.link(ext);ext.location=(12,-17,11);ext.rotation_euler=(Vector((0,0,2.4))-ext.location).to_track_quat('-Z','Y').to_euler();camdata.lens=45
S.camera=ext;S.render.resolution_x=1400;S.render.resolution_y=1100;S.render.filepath=str(OUT/'11_exterior_preserved.png');bpy.ops.render.render(write_still=True)
for o,state in flags.items():o.hide_render=state
roof.hide_render=roof_flag;wc.inputs['Strength'].default_value=oldstrength;S.view_settings.exposure=oldexposure
# Refresh asset libraries after the final expression and camera composition changes.
for name in ['STUDENT','ENCOURAGER','ZOMBIE']:
 rt=bpy.data.objects['SR_CHARACTER_'+name];bpy.data.libraries.write(str(OUT/('Character_'+name+'.blend')),{rt}|set(rt.children_recursive),fake_user=True)
S.camera=bpy.data.objects['CAM_REFERENCE_COMPARISON'];S.render.resolution_x=1536;S.render.resolution_y=1120;S.render.filepath=str(OUT/'08_perspective_final.png')
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_ReadingHouse_Reference_FINAL.blend'))
manifest=json.loads((OUT/'manifest.json').read_text());manifest['latest_render']='08_perspective_final.png';manifest['production_collection']=prod.name;manifest['stone_examples']=['09_short_sentence_stones.png','10_long_sentence_stones.png'];manifest['exterior']='11_exterior_preserved.png';manifest['final_objects']={cn:[o.name for o in bpy.data.collections[cn].objects] for cn in ['SR_Console','SR_Characters','SR_Decor','SR_StoneSource']};manifest['all_renders']=[p.name for p in OUT.glob('*.png') if p.name!='preview_landscape.png'];(OUT/'manifest.json').write_text(json.dumps(manifest,indent=2))
with (OUT/'HANDOFF.md').open('a') as f:f.write('\nFinal source: sReader_ReadingHouse_Reference_FINAL.blend. Main review: 08_perspective_final.png. SR_PRODUCTION_EXPORT organises console, characters and new decor; original Architecture/Roof/Interior remain separate. Demonstration collections are excluded. Nine-word-plus example wraps to two rows using the same fixed-cap stone geometry. Exterior render temporarily restores the preserved facade and roof. Latest added asset triangles: console 11,444; characters about 90,120; new decor about 14,670; reusable stone source 876. These are measured scene counts, not Android performance results. Retopology/mesh combining and device profiling remain before a mobile production release.\n')
print(json.dumps({'saved':bpy.data.filepath,'examples':manifest['stone_examples'],'production_collection':prod.name,'required_names_valid':all(manifest['required_names'].values())}))
