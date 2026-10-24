from pathlib import Path
p=Path(r'C:/Users/11113930/sReader4/GeneratedAssets/LibraryRedesign/20261017_184255/scripts/refine_landscape_1520.py')
s=p.read_text();s=s.replace("in_head=head and (o.parent==head or head in o.parent_recursive)","in_head=head and o in head.children_recursive")
s=s.replace("bpy.data.libraries.write(str(OUT/'Dashboard_Landscape_Asset.blend')),{console},fake_user=True)","bpy.data.libraries.write(str(OUT/'Dashboard_Landscape_Asset.blend'),{console},fake_user=True)")
s+='''
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
'''
compile(s,str(p),'exec');p.write_text(s,encoding='utf-8');print('Blender refinement script compiled successfully.')
