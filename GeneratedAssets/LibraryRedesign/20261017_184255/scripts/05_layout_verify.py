import bpy,math,json
from pathlib import Path
from mathutils import Vector,Matrix
ROOT=Path(__file__).resolve().parents[1]
exec(compile((ROOT/'scripts'/'common.py').read_text(),str(ROOT/'scripts'/'common.py'),'exec'))
bpy.context.view_layer.update()
def bounds(o):
 pts=[o.matrix_world@Vector(v) for v in o.bound_box]
 return Vector(tuple(min(v[i] for v in pts) for i in range(3))),Vector(tuple(max(v[i] for v in pts) for i in range(3)))
def center_at(o,target):
 lo,hi=bounds(o);o.location+=Vector(target)-(lo+hi)/2;bpy.context.view_layer.update()
# GLB meshes have baked vertex positions. Place by measured bounds, not transform origin.
for suffix,y in [('-2.4',-1.65),('2.4',-.4)]:
 center_at(bpy.data.objects['SR_reading_chair_seat_'+suffix],(-2.25,y,.69))
 center_at(bpy.data.objects['SR_reading_chair_back_'+suffix],(-2.25,y+.39,1.14))
labels=sorted([o for o in C['Interactive Props'].objects if o.name.startswith('SR_Stone_Glyph')],key=lambda o:o.name)
for i in range(5):
 x=1.64+(i%2)*.78;y=-2.40+(i//2)*.80
 center_at(bpy.data.objects['SR_WORD_STONE_'+str(i)],(x,y,.50))
 center_at(bpy.data.objects['SR_WORD_STONE_GOLD_'+str(i)],(x,y,.65))
 labels[i].location=(x,y,.675)
bpy.data.objects['SR_Activity_Label'].location=(2.02,-2.96,.372)
# Proper wedge slope: the book clears the desk surface along its entire depth.
o=bpy.data.objects['SR_Book_Stand_Wedge']
verts=[(x,y,z) for z in [1.35] for x,y in [(-1.02,.5),(1.02,.5),(1.02,1.5),(-1.02,1.5)]]
verts +=[(x,y,1.38+(y-.5)*.29) for x,y in [(-1.02,.5),(1.02,.5),(1.02,1.5),(-1.02,1.5)]]
me=bpy.data.meshes.new('SR_Sloped_Stand');me.from_pydata(verts,[],[(0,3,2,1),(4,5,6,7),(0,1,5,4),(1,2,6,5),(2,3,7,6),(3,0,4,7)]);me.materials.append(P['wood']);me.update();o.data=me;o.location=(0,0,0)
for ob in list(C['Interactive Props'].objects):
 if ob.name.startswith(('SR_HERO_book','SR_HERO_open','SR_Book_Ink_Line','SR_Book_Chapter')):ob.location.z+=.18
# Give the Lexicon's preserved open-book prop a real two-page silhouette.
o=bpy.data.objects['SR_LEXICON_open_book'];lo,hi=bounds(o);center=(lo+hi)/2
# Retain this copy in an archive subcollection; original imported asset is also in Backup.
archive=bpy.data.collections.new('Backup_Superseded_Working_Copies');C['Backup'].children.link(archive)
for ob in [o]+[v for v in list(C['Interactive Props'].objects) if v.name.startswith('SR_interior_lantern')]:
 for c in list(ob.users_collection):c.objects.unlink(ob)
 archive.objects.link(ob)
for side in [-1,1]:
 ob=box('SR_Lexicon_Page',(2+side*.24,2.8,1.66),(.47,.69,.075),'paper','Interactive Props',.015);ob.rotation_euler.y=side*math.radians(9)
 for row in range(5):line('SR_Lexicon_Ink',[(2+side*.24-.15,2.58+row*.1,1.715),(2+side*.24+.15,2.58+row*.1,1.715)],.004,'wood','Interactive Props')
line('SR_Lexicon_Bookmark',[(2,2.45,1.72),(2,3.05,1.72)],.018,'gold','Interactive Props')
# The existing Owl perch now has a small, restrained library owl.
sphere('SR_Owl_Body',(-2,2.8,2.27),(.24,.21,.33),'slate2')
sphere('SR_Owl_Head',(-2,2.76,2.59),(.27,.22,.23),'stoneLight')
for side in [-1,1]:
 sphere('SR_Owl_Wing',(-2+side*.20,2.82,2.25),(.10,.18,.25),'slate')
 sphere('SR_Owl_Eye_Disc',(-2+side*.112,2.563,2.62),(.10,.025,.105),'paper')
 sphere('SR_Owl_Eye',(-2+side*.112,2.536,2.62),(.048,.012,.055),'gold')
 sphere('SR_Owl_Pupil',(-2+side*.112,2.524,2.62),(.023,.008,.035),'ink')
 line('SR_Owl_Feet',[(-2+side*.08,2.62,1.98),(-2+side*.08,2.76,1.98)],.025,'gold','Interactive Props')
prism('SR_Owl_Beak',[(-2.045,2.55),(-1.955,2.55),(-2,2.45)],.07,'gold','Interactive Props',2.50)
# Lantern cages replace the opaque preview cylinders; their raw geometry is preserved.
for x in [-3.65,3.65]:
 box('SR_Interior_Sconce_Bracket',(x,3.56,2.8),(.08,.6,.07),'wood','Interior')
 line('SR_Interior_Lantern_Stem',[(x,3.29,2.8),(x,3.29,2.66)],.015,'gold','Interior')
 box('SR_Interior_Lantern_Glass',(x,3.29,2.47),(.19,.19,.29),'glow','Interior',.025)
 for dx in [-.13,.13]:
  for dy in [-.13,.13]:box('SR_Sconce_Cage',(x+dx,3.29+dy,2.47),(.018,.018,.34),'gold','Interior',.004)
 for z in [2.28,2.66]:box('SR_Sconce_Cap',(x,3.29,z),(.33,.33,.06),'slate','Interior',.015)
# Close-up camera is inside the building so the facade cannot hide the word activity.
cam=bpy.data.objects['SR_Camera_Hero'];cam.location=(3,-3.75,5.8);cam.rotation_euler=(Vector((.30,.25,1.05))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.lens=31
# Add a subtle ring around activity disks, without adding realtime lights.
for i in range(5):
 o=bpy.data.objects['SR_WORD_STONE_'+str(i)];lo,hi=bounds(o);p=(lo+hi)/2
 line('SR_Stone_Base_Inlay',[(p.x+.30*math.cos(k*math.tau/48),p.y+.30*math.sin(k*math.tau/48),.638) for k in range(48)],.008,'gold','Interactive Props',True)
# Route and original-object preservation audits.
bpy.context.view_layer.update()
report={'original_objects':len(C['Backup'].objects),'archive_working_copies':len(archive.objects),'stones':[], 'chair_centres':[], 'roof_objects':len(C['Roof'].objects)}
for i in range(5):
 o=bpy.data.objects['SR_WORD_STONE_'+str(i)];lo,hi=bounds(o)
 assert lo.x>.8 and hi.x<3.10,'Activity stone obstructs aisle/shelf'
 assert abs(lo.z-.365)<.005,'Stone not seated on rug'
 report['stones'].append({'name':o.name,'min':list(lo),'max':list(hi)})
for suffix in ['-2.4','2.4']:
 lo,hi=bounds(bpy.data.objects['SR_reading_chair_seat_'+suffix]);report['chair_centres'].append(list((lo+hi)/2))
assert len(C['Backup'].objects)==642
(ROOT/'layout_audit.json').write_text(json.dumps(report,indent=2))
C['Roof'].hide_render=True;C['Roof'].hide_viewport=True;S.camera=bpy.data.objects['SR_Camera_Interior']
bpy.ops.wm.save_as_mainfile(filepath=str(ROOT/'sReader_Library_v04_layout_verified.blend'))
S.cycles.samples=24;S.render.resolution_x=1200;S.render.resolution_y=1000
for camera_name,file_name in [('SR_Camera_Interior','review_interior_v3.png'),('SR_Camera_Hero','review_hero_v2.png')]:
 S.camera=bpy.data.objects[camera_name];S.render.filepath=str(ROOT/'renders'/file_name);bpy.ops.render.render(write_still=True)
print(json.dumps(report))
