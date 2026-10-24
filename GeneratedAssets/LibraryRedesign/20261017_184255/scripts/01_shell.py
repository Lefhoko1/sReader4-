import bpy,json,math
from pathlib import Path
from mathutils import Matrix,Vector
ROOT=Path(__file__).resolve().parents[1]
# The native original has been saved before any structural mutation.
assert (ROOT/'sReader_Library_v00_original.blend').is_file()
originals=list(bpy.context.scene.objects)
for name in ['Architecture','Roof','Interior','Interactive Props','Lighting','Landscaping','Backup']:
 c=bpy.data.collections.new(name);bpy.context.scene.collection.children.link(c)
backup=bpy.data.collections['Backup']
for o in originals:
 for c in list(o.users_collection):c.objects.unlink(o)
 backup.objects.link(o)
backup.hide_render=True;backup.hide_viewport=True
backup['purpose']='All 642 original objects, original mesh data and transforms; preserved before structural changes.'
exec(compile((ROOT/'scripts'/'common.py').read_text(),str(ROOT/'scripts'/'common.py'),'exec'))
# Copies of useful props, with independent meshes; originals remain unmodified.
convert=Matrix(((1,0,0,0),(0,0,1,0),(0,-1,0,0),(0,0,0,1)))
for src in originals:
 n=src.name
 keep=n.startswith(('HERO_','desk_leg_','WORD_STONE_','BOOK_','SHELF_plank_','LEXICON_','OWL_','reading_chair_','hearth_')) or (src.parent and src.parent.name=='world.001' and n.startswith(('tree_','interior_lantern')))
 if not keep:continue
 o=src.copy();o.data=src.data.copy() if src.data else None;o.parent=None;o.matrix_world=convert@src.matrix_world
 o.name='SR_'+n
 C['Interior' if n.startswith(('BOOK_','SHELF_','reading_','hearth_')) else ('Landscaping' if n.startswith('tree_') else 'Interactive Props')].objects.link(o)
 o.hide_set(False);o.hide_viewport=False;o.hide_render=False
 if o.type=='MESH':
  for mod in list(o.modifiers):o.modifiers.remove(mod)
  if n.startswith('BOOK_'):material=mat('Book_'+str(sum(map(ord,n))%6),['315054','68414A','6C6344','3D4D66','765039','606B55'][sum(map(ord,n))%6])
  elif n.startswith(('HERO_open','LEXICON_open')):material=P['paper']
  elif 'GOLD' in n or 'gold' in n:material=P['gold']
  elif n.startswith('WORD_'):material=P['slate2']
  elif n.startswith('tree_crown'):material=P['leaves']
  elif n.startswith('tree_trunk'):material=P['wood']
  elif n.startswith(('LEXICON_stone','OWL_perch_column','hearth_stone')):material=P['stoneLight']
  elif n.startswith('reading_'):material=P['teal']
  elif n.startswith('HERO_book_base'):material=P['teal']
  else:material=P['wood']
  o.data.materials.clear();o.data.materials.append(material)
  if not n.startswith('BOOK_'):
   b=o.modifiers.new('Crafted edge','BEVEL');b.width=.018;b.segments=2
# One plinth, one floating floor. No old shell/porch/foundation in active collections.
box('SR_Foundation',(0,0,.12),(9.65,8.65,.24),'stone',bevel=.06)
box('SR_Wood_Floor',(0,0,.285),(8.72,7.72,.09),'floor',bevel=.018)
# Thin plank seams instead of overlapping floor slabs.
for i in range(22):
 x=-4.13+i*.393
 box('SR_Plank_Seam',(x,0,.333),(.008,7.65,.003),'wood',bevel=0)
box('SR_Wall_Left',(-4.5,0,2.42),(.28,8.4,4.16),'plaster')
box('SR_Wall_Right',(4.5,0,2.42),(.28,8.4,4.16),'plaster')
box('SR_Wall_Rear',(0,4.05,2.42),(8.72,.28,4.16),'plaster')
# Front shell: genuine door and arched window openings.
for side in [-1,1]:
 cx=side*3.05;r=.65
 window('SR_Front_Window_'+str(side),cx,-4.08,1.2,2.9,r)
 wall_opening('SR_WindowPanel_'+str(side),cx,-4.05,2*r,1.2,2.9,r,4.5)
 for lo,hi in [(1.16,2.4),(3.7,4.36)]:
  box('SR_Front_Pier',(side*(lo+hi)/2,-4.05,2.42),(hi-lo,.28,4.16),'plaster')
wall_opening('SR_Entry',0,-4.05,2.32,.34,2.65,1.16,4.5)
archtrim('SR_Entry_Carved_Portal',0,-4.26,.34,2.65,1.25,width=.12)
archtrim('SR_Entry_Bronze_Inlay',0,-4.39,.34,2.65,1.18,width=.025,material='gold')
# Proper open shelf frames: the preserved horizontal planks and books remain.
for side in [-1,1]:
 for j,cy in enumerate([-1.6,1.2]):
  for end in [-1,1]:box('SR_Shelf_Upright',(side*3.55,cy+end*1.12,2.05),(.82,.12,3.4),'wood','Interior')
  box('SR_Shelf_Back',(side*3.94,cy,2.05),(.07,2.3,3.4),'wood','Interior',.01)
  box('SR_Shelf_Cornice',(side*3.55,cy,3.82),(.94,2.46,.15),'woodLight','Interior')
  box('SR_Shelf_Plinth',(side*3.55,cy,.43),(.9,2.4,.16),'woodLight','Interior')
# Move original stones to the right activity alcove, preserving each disk assembly.
for i in range(5):
 for prefix in ['SR_WORD_STONE_','SR_WORD_STONE_GOLD_']:
  o=bpy.data.objects[prefix+str(i)];o.location.x=1.95+(i%2)*.78;o.location.y=-2.4+(i//2)*.8
# Chairs: arrange into left-hand reading nook, away from entrance aisle.
for srcx,target in [('-2.4',(-2.25,-1.65)),('2.4',(-2.25,-.4))]:
 for part in ['seat','back']:
  o=bpy.data.objects['SR_reading_chair_'+part+'_'+srcx];o.location.x=target[0];o.location.y=target[1]+(.39 if part=='back' else 0)
# Interactive stations retain accessible clearance behind hero desk.
S['design_notes']='Z-up design. Entrance 2.32m wide. Central route x=-0.8..0.8 remains clear from y=-4.0 to desk y=.475. Roof is independent; no ceiling spans room.'
assert len(backup.objects)==len(originals)
assert not any(o.name=='main_cream_walls' for col in [C['Architecture']] for o in col.objects)
bpy.ops.wm.save_as_mainfile(filepath=str(ROOT/'sReader_Library_v01_shell.blend'))
print(json.dumps({'milestone':'hollow_shell_open_shelves','backup_objects':len(backup.objects),'active_meshes':sum(o.type=='MESH' for c in C.values() if c!=backup for o in c.objects),'saved':bpy.data.filepath}))
