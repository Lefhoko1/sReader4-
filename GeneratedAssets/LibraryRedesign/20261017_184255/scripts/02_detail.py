import bpy,math,json,random
from pathlib import Path
from mathutils import Vector,Matrix
ROOT=Path(__file__).resolve().parents[1]
exec(compile((ROOT/'scripts'/'common.py').read_text(),str(ROOT/'scripts'/'common.py'),'exec'))
# Shell audit before adding detail.
assert len(C['Backup'].objects)==642
assert all(bpy.data.objects[n].dimensions.z<.2 for n in ['SR_Wood_Floor'])
assert len([o for o in C['Interior'].objects if o.name.startswith('SR_Shelf_Upright')])==8
# A low, composed garden plinth and a welcoming route.
box('SR_Garden_Plinth',(0,-.5,-.25),(14.6,16.2,.42),'slate','Landscaping',.22)
box('SR_Garden_Moss',(0,-.5,-.025),(14.3,15.9,.08),'grass','Landscaping',.16)
for x in [-4.5,4.5]:
 box('SR_Stone_Skirting',(x,0,.47),(.42,8.35,.27),'stoneLight')
 box('SR_Timber_Eaves',(x,0,4.42),(.25,8.45,.18),'wood')
for y in [-4.15,4.15]:box('SR_Stone_Skirting',(0,y,.47),(9.1,.36,.27),'stoneLight')
# Remove only the entry section of the newly created front skirting to keep the threshold clear.
front=[o for o in C['Architecture'].objects if o.name.startswith('SR_Stone_Skirting') and o.location.y< -4][0]
bpy.data.objects.remove(front,do_unlink=True)
for x in [-2.85,2.85]:box('SR_Front_Plinth_Return',(x,-4.15,.47),(3.25,.36,.27),'stoneLight')
for x in [-4.38,4.38]:
 for y in [-4.13,4.13]:
  box('SR_Corner_Timber',(x,y,2.48),(.18,.18,3.98),'wood')
  for z in [.8,3.9]:box('SR_Corner_Band',(x,y-.015,z),(.22,.22,.085),'gold')
for x in [-4.3,-2.23,2.23,4.3]:
 box('SR_Facade_Timber',(x,-4.24,2.46),(.095,.11,3.9),'wood')
box('SR_Front_Eaves',(0,-4.24,4.43),(9.15,.22,.22),'wood')
box('SR_Signboard',(0,-4.31,4.15),(2.5,.12,.46),'slate',bevel=.06)
text('SR_Library_Wordmark','s R e a d e r',(0,-4.385,4.16),.25,'gold')
# Door leaves opened outward; each complete leaf is parented to its hinge.
for side in [-1,1]:
 hinge=bpy.data.objects.new('SR_Entry_Hinge_'+str(side),None);C['Architecture'].objects.link(hinge);hinge.location=(side*1.12,-4.18,.34)
 parts=[]
 leaf=box('SR_Open_Library_Door',(side*.55,-4.23,1.66),(1.06,.12,2.63),'wood');parts.append(leaf)
 for z in [.53,2.7]:parts.append(box('SR_Door_Bronze_Strapping',(side*.55,-4.31,z),(.94,.045,.07),'gold'))
 for x in [-.34,.34]:parts.append(box('SR_Door_Raised_Panel',(side*.55+x,-4.31,1.62),(.035,.04,1.82),'woodLight'))
 parts.append(sphere('SR_Door_Handle',(side*.19,-4.36,1.55),(.045,.04,.045),'gold','Architecture'))
 for o in parts:
  m=o.matrix_world.copy();o.parent=hinge;o.matrix_world=m
 hinge.rotation_euler.z=side*math.radians(72)
# Three shallow, properly nested steps leading onto floor height .33.
for i in range(3):
 box('SR_Entrance_Step_'+str(i),(0,-4.4-i*.42,.27-i*.085),(3.05+i*.34,.95,.12),'stoneLight',bevel=.04)
for i in range(6):
 y=-5.95-i*.44
 box('SR_Path_Flagstone',(math.sin(i*.42)*.16,y,.055),(1.65,.36,.07),'stoneLight','Landscaping',.04)
# Bespoke slate roof: inclined slabs, not a solid triangular prism.
a=math.atan2(2.15,4.95)
for side in [-1,1]:
 o=box('SR_Roof_Slope_'+str(side),(side*2.475,0,5.575),(math.hypot(4.95,2.15),9.0,.115),'slate','Roof',.025);o.rotation_euler.y=side*a
 verts=[];faces=[];mi=[]
 # Tile field batched into two meshes, rather than individual tile objects.
 for row in range(7):
  x0=.02+row*.70;x1=min(4.95,x0+.675)
  for j in range(13):
   y0=-4.48+j*.69;y1=min(4.48,y0+.665)
   for lift in [0,.045]:
    for xx,yy in [(x0,y0),(x1,y0),(x1,y1),(x0,y1)]:verts.append((side*xx,yy,6.65-xx*2.15/4.95+lift+.07))
   k=len(verts)-8;faces.extend([tuple(k+i for i in f) for f in [(0,3,2,1),(4,5,6,7),(0,1,5,4),(1,2,6,5),(2,3,7,6),(3,0,4,7)]])
 tiles=mesh('SR_Slate_Tile_Field_'+str(side),verts,faces,'slate','Roof');tiles.data.materials.append(P['slate2'])
 for p in tiles.data.polygons:p.material_index=1 if (p.index//6)%7==0 else 0
 for y in [-4.55,4.55]:line('SR_Gable_Fascia',[(side*5,y,4.47),(0,y,6.7)],.095,'wood','Roof')
 for y in [-4.57,4.57]:line('SR_Gable_Gold_Fillet',[(side*4.96,y,4.52),(0,y,6.72)],.016,'gold','Roof')
line('SR_Ridge_Cap',[(0,-4.6,6.75),(0,4.6,6.75)],.09,'slate2','Roof')
for y in [-4.1,4.1]:prism('SR_Roof_Gable',[(-4.5,4.5),(4.5,4.5),(0,6.46)],.18,'plaster','Roof',y)
# Restrained open-book emblem on the gable, with a single star.
box('SR_Emblem_Plaque',(0,-4.24,5.25),(1.8,.13,.95),'slate','Roof',.12)
for side in [-1,1]:
 line('SR_Emblem_Open_Book',[(0,-4.32,5.02),(side*.56,-4.32,5.13),(side*.56,-4.32,5.53),(0,-4.32,5.42),(0,-4.32,5.02)],.025,'gold','Roof')
 for i in range(2):line('SR_Emblem_Page_Line',[(side*.10,-4.33,5.15+i*.12),(side*.43,-4.33,5.22+i*.12)],.008,'gold','Roof')
line('SR_Emblem_Star',[(0,-4.33,5.94),(.055,-4.33,5.81),(.18,-4.33,5.76),(.055,-4.33,5.71),(0,-4.33,5.58),(-.055,-4.33,5.71),(-.18,-4.33,5.76),(-.055,-4.33,5.81)],.015,'gold','Roof',True)
# Rear gable carries timber tracery. Every roof ornament belongs to Roof.
for y in [-4.23,4.23]:
 line('SR_Gable_Timber',[(0,y,4.52),(0,y,6.38)],.055,'wood','Roof')
# Add warm front lanterns; the emissive panes require no separate point lights.
for x in [-1.62,1.62]:
 box('SR_Lantern_Bracket',(x,-4.48,2.82),(.09,.55,.09),'wood')
 box('SR_Entry_Lantern',(x,-4.68,2.49),(.27,.27,.44),'glow')
 for dx in [-.15,.15]:
  for dy in [-.15,.15]:box('SR_Lantern_Frame',(x+dx,-4.68+dy,2.49),(.025,.025,.47),'gold')
 for z in [2.24,2.74]:box('SR_Lantern_Cap',(x,-4.68,z),(.36,.36,.07),'slate')
# Interior: rug, wall panelling, reading furniture.
box('SR_Reading_Rug',(0,0,.35),(5.8,5.9,.025),'rug','Interior',.03)
for x in [-2.78,2.78]:box('SR_Rug_Gold_Border',(x,0,.366),(.035,5.64,.004),'gold','Interior',0)
for y in [-2.82,2.82]:box('SR_Rug_Gold_Border',(0,y,.366),(5.58,.035,.004),'gold','Interior',0)
# New low panelling along back wall, away from prop circulation.
box('SR_Rear_Wainscot',(0,3.875,.85),(8.7,.055,1.0),'wood','Interior')
box('SR_Rear_Dado',(0,3.81,1.38),(8.7,.13,.10),'woodLight','Interior')
for x in [-4,-3,-2,-1,0,1,2,3,4]:box('SR_Wainscot_Stile',(x,3.83,.85),(.055,.04,.89),'woodLight','Interior')
# Reusable chair parts and cushions complete the original seats/backs.
for y in [-1.65,-.4]:
 for dx in [-.31,.31]:
  for dy in [-.30,.30]:box('SR_Chair_Leg',(-2.25+dx,y+dy,.49),(.08,.08,.31),'wood','Interior')
 for dx in [-.43,.43]:
  box('SR_Chair_Arm',(-2.25+dx,y,.99),(.09,.85,.10),'woodLight','Interior')
  box('SR_Chair_Arm_Post',(-2.25+dx,y-.29,.85),(.075,.075,.3),'wood','Interior')
 box('SR_Chair_Cushion',(-2.25,y,.82),(.73,.72,.12),'teal','Interior',.07)
cyl('SR_Reading_Side_Table',(-2.28,-2.68,.82),.40,.09,'woodLight','Interior')
cyl('SR_Reading_Side_Table_Stem',(-2.28,-2.68,.57),.075,.44,'wood','Interior')
cyl('SR_Reading_Side_Table_Base',(-2.28,-2.68,.36),.26,.05,'wood','Interior')
# Hero book now rests on a purpose-built sloped support.
prism('SR_Book_Stand_Wedge',[(-1.02,1.35),(1.02,1.35),(1.02,1.55),(-1.02,1.55)],.85,'woodLight','Interactive Props',1.0)
pivot=Vector((0,1,1.4));tilt=Matrix.Translation(pivot)@Matrix.Rotation(math.radians(18),4,'X')@Matrix.Translation(-pivot)
for o in list(C['Interactive Props'].objects):
 if o.name.startswith(('SR_HERO_book','SR_HERO_open')):o.matrix_world=tilt@o.matrix_world
# Page lettering is model decoration, not a replacement for Unity reading content.
for side in [-1,1]:
 cx=side*.46
 for row in range(6):
  length=.52 if row<5 else .36
  o=line('SR_Book_Ink_Line',[(cx-length/2,.72+row*.09,1.58),(cx+length/2,.72+row*.09,1.58)],.006,'wood','Interactive Props');o.matrix_world=tilt@o.matrix_world
 o=text('SR_Book_Chapter','KNOWLEDGE' if side<0 else 'REMEMBER',(cx,1.29,1.58),.075,'gold','Interactive Props',(0,0,0));o.matrix_world=tilt@o.matrix_world
box('SR_Desk_Brass_Rail',(0,.448,1.25),(2.85,.045,.055),'gold','Interactive Props')
box('SR_Hero_Plaque',(0,.424,1.08),(1.5,.035,.19),'slate','Interactive Props',.02)
text('SR_Hero_Plaque_Text','THE GREAT LIBRARY',(0,.400,1.08),.105,'gold','Interactive Props')
# Floor-level activity area leaves the central 1.6m-wide route clear.
for i in range(5):
 o=bpy.data.objects['SR_WORD_STONE_GOLD_'+str(i)]
 text('SR_Stone_Glyph',chr(65+i),(o.location.x,o.location.y,.618),.15,'ink','Interactive Props',(0,0,0))
text('SR_Activity_Label','WORD ATELIER',(2.36,-2.96,.372),.17,'gold','Interactive Props',(0,0,0))
# Dedicated Book Spirit interaction point to left of desk, outside central aisle.
cyl('SR_Book_Spirit_Base',(-1.92,1.35,.42),.32,.14,'stoneLight','Interactive Props')
cyl('SR_Book_Spirit_Column',(-1.92,1.35,.91),.12,.89,'wood','Interactive Props')
cyl('SR_Book_Spirit_Rim',(-1.92,1.35,1.38),.29,.07,'gold','Interactive Props')
sphere('SR_Book_Spirit',(-1.92,1.35,1.78),(.16,.14,.24),'magic')
for dx in [-.065,.065]:sphere('SR_Spirit_Eye',(-1.92+dx,1.216,1.80),(.022,.012,.028),'ink')
line('SR_Spirit_Orbit',[(-1.92+.28*math.cos(i*math.tau/48),1.35+.22*math.sin(i*math.tau/48),1.60+.06*math.sin(i*math.tau/48)) for i in range(49)],.009,'gold','Interactive Props')
# Modest visible magic accent above hero book.
random.seed(42)
for i in range(14):
 a=i*2.4;r=.42+random.random()*.45
 sphere('SR_Gold_Ink_Mote',(math.cos(a)*r,1+math.sin(a)*r,1.9+random.random()*.6),(.016,.016,.016),'glow')
# Existing hearth gets an inset bronze surround and modest warm inset.
box('SR_Hearth_Ember',(0,3.59,.78),(.62,.025,.35),'glow','Interior',.03)
box('SR_Hearth_Mantel',(0,3.7,2.4),(2.0,.58,.14),'woodLight','Interior')
text('SR_Library_Motto','WORDS RESTORE WORLDS',(0,3.71,3.26),.20,'gold','Interior')
# Planting is intentionally low along the entrance, not a duplicate building annex.
for side in [-1,1]:
 box('SR_Planter',(side*3.15,-4.88,.23),(2.05,.60,.34),'stone','Landscaping',.06)
 box('SR_Planter_Soil',(side*3.15,-4.88,.41),(1.90,.47,.025),'soil','Landscaping',.02)
 for j in range(5):
  x=side*3.15+(j-2)*.35
  sphere('SR_Low_Sage',(x,-4.88,.53),(.25,.23,.24),'leaves','Landscaping')
# Real lights are consolidated for presentation; emissive fixtures use shared materials.
world=bpy.data.worlds.new('SR_Evening_World');S.world=world;world.use_nodes=True
world.node_tree.nodes['Background'].inputs['Color'].default_value=(.055,.075,.115,1);world.node_tree.nodes['Background'].inputs['Strength'].default_value=.38
area('SR_Key_Warm',(-5,-8,12),(0,0,1.5),1800,(1.0,.80,.58),7)
area('SR_Cool_Sky',(6,4,10),(0,0,2),2100,(.58,.73,1),8)
area('SR_Reading_Softbox',(0,0,4.18),(0,1,.4),560,(1,.73,.44),4.5)
area('SR_Entry_Bounce',(0,-3.1,3.6),(0,-1,.7),140,(1,.68,.36),2.5)
# EEVEE for quick review; only four broad area lights, no per-book lights.
S.render.engine='CYCLES';S.cycles.samples=32;S.cycles.use_denoising=True
S.render.resolution_x=1400;S.render.resolution_y=1100;S.render.resolution_percentage=100
S.render.image_settings.file_format='PNG';S.render.film_transparent=False
S.view_settings.view_transform='AgX'
S.view_settings.look='AgX - Medium High Contrast'
S.view_settings.exposure=.5
camera('SR_Camera_Exterior',(13,-20,12.6),(0,-.3,2.45),ortho=18.5)
camera('SR_Camera_Interior',(11,-16,15.5),(0,.1,1.05),ortho=13.2)
camera('SR_Camera_Hero',(7,-9,8.5),(.2,.2,1.1),lens=51)
S.camera=bpy.data.objects['SR_Camera_Interior']
C['Roof']['instructions']='Hide this single collection for interior views. Includes slopes, tiles, gables, ridge, fascia and gable emblem; no ceiling.'
C['Interactive Props']['instructions']='Preserved hero book/desk, word stones, Lexicon, Owl perch; added Book Spirit. Presentation only; Unity logic untouched.'
# Set Blender to the saved cinematic interior view.
C['Roof'].hide_render=True;C['Roof'].hide_viewport=True
for screen in bpy.data.screens:
 for a in screen.areas:
  if a.type=='VIEW_3D':a.spaces.active.region_3d.view_perspective='CAMERA'
S.render.filepath=str(ROOT/'renders'/'02_interior_overview.png')
bpy.ops.wm.save_as_mainfile(filepath=str(ROOT/'sReader_Library_v02_architecture_atmosphere.blend'))
print(json.dumps({'milestone':'architecture_atmosphere','collections':{k:len(c.objects) for k,c in C.items()},'lights':len([o for o in C['Lighting'].objects if o.type=='LIGHT']),'saved':bpy.data.filepath}))
