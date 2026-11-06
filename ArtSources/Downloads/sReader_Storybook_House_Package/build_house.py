import trimesh, numpy as np, math, os
from PIL import Image, ImageDraw
OUT=os.path.dirname(os.path.abspath(__file__))
scene=trimesh.Scene()
C={'stone':[177,167,146,255],'cream':[231,220,193,255],'slate':[48,60,76,255],'roof':[57,68,83,255],'roof2':[71,83,98,255],'wood':[116,76,51,255],'darkwood':[75,51,40,255],'gold':[216,171,91,255],'glass':[96,152,171,255],'light':[255,216,140,255],'grass':[86,116,90,255],'path':[178,166,140,255],'trim':[221,205,171,255],'black':[36,40,46,255],'green':[61,101,70,255]}
def box(name,loc,size,mat):
 m=trimesh.creation.box(extents=size);m.apply_translation(loc);m.visual.face_colors=C[mat];scene.add_geometry(m,geom_name=name)
def cyl(name,loc,r,h,mat,n=12):
 m=trimesh.creation.cylinder(radius=r,height=h,sections=n);m.apply_translation(loc);m.visual.face_colors=C[mat];scene.add_geometry(m,geom_name=name)
def roofprism(name,x0,x1,y0,y1,z_eave,z_peak,mat):
 # roof surface triangular prism mesh with bottom face
 vs=np.array([[x0,y0,z_eave],[x1,y0,z_eave],[(x0+x1)/2,y0,z_peak],[x0,y1,z_eave],[x1,y1,z_eave],[(x0+x1)/2,y1,z_peak]])
 fs=[[0,1,2],[3,5,4],[0,3,4],[0,4,1],[0,2,5],[0,5,3],[1,4,5],[1,5,2]]
 m=trimesh.Trimesh(vertices=vs,faces=fs,process=False);m.visual.face_colors=C[mat];scene.add_geometry(m,geom_name=name)
# ground
box('landscaped_grass',(0,0,-.18),(13,12,.3),'grass')
for i in range(8): box('front_paving_%02d'%i,(0,-5.45+i*.49,.006),(1.42,.39,.055),'path')
# primary structure
box('main_stone_base',(0,.35,.22),(7.5,6.5,.44),'stone')
box('main_cream_walls',(0,.35,2.02),(7.25,6.25,3.35),'cream')
# right annex / sunroom
box('annex_foundation',(4.43,.4,.18),(2.25,4.7,.36),'stone')
box('annex_walls',(4.43,.4,1.65),(2.18,4.6,2.66),'cream')
# main roof gable front-back
roofprism('main_slate_gable_roof',-4.06,4.06,-3.05,3.75,3.62,5.62,'roof')
# two roof slope overlay using panels? shingles horizontal battens roof slopes
for side in [-1,1]:
 for i in range(7):
  x=side*(.55+i*.48)
  z=5.62-(abs(x)/4.06)*2+.027
  for j in range(8):
   y=-2.68+j*.85
   # slim tiles along y, slope tangent omitted
   box('roof_tile_%s_%s_%s'%(side,i,j),(x,y,z),(.38,.78,.045),'roof2' if (i+j)%3==0 else 'roof')
roofprism('annex_gable',3.18,5.75,-2.15,2.95,3.01,3.85,'roof')
# facade beams, vertical trim
for x in [-3.48,3.48]: box('corner_beam',(x,-2.79,2.04),(.16,.14,3.2),'wood')
box('front_crossbeam',(0,-2.82,3.48),(7.1,.17,.18),'wood')
# front door at y=-2.87
box('door_frame',(0,-2.88,1.55),(1.63,.22,2.68),'darkwood')
box('wooden_front_door',(0,-3.012,1.53),(1.36,.07,2.43),'wood')
for x in [-.4,0,.4]: box('door_vertical_plank',(x,-3.06,1.55),(.025,.025,2.3),'darkwood')
cyl('door_knob',(.49,-3.11,1.55),.08,.10,'gold')
# front windows left right
for idx,x in enumerate([-2.45,2.45]):
 box('window_outer_frame_%d'%idx,(x,-2.84,2.18),(1.57,.2,1.75),'wood')
 box('window_glass_%d'%idx,(x,-2.956,2.18),(1.32,.055,1.5),'glass')
 box('window_warm_glow_%d'%idx,(x,-2.988,2.18),(1.22,.018,1.40),'light')
 box('window_mullion_vertical_%d'%idx,(x,-3.02,2.18),(.09,.09,1.59),'darkwood')
 box('window_mullion_horizontal_%d'%idx,(x,-3.02,2.18),(1.42,.09,.09),'darkwood')
 box('window_sill_%d'%idx,(x,-3.09,1.25),(1.82,.4,.14),'stone')
# annex windows on front and side
box('annex_window_frame',(4.43,-2.00,1.78),(1.65,.14,1.64),'wood')
box('annex_window_glass',(4.43,-2.09,1.78),(1.44,.06,1.42),'light')
box('annex_window_cross',(4.43,-2.14,1.78),(.08,.08,1.55),'darkwood')
box('annex_window_horizontal',(4.43,-2.14,1.78),(1.53,.08,.08),'darkwood')
# dormer roof at left front
box('dormer_face',(-1.8,-2.62,4.45),(1.28,.45,1.05),'cream')
roofprism('dormer_roof',-2.56,-1.04,-3.03,-2.05,4.91,5.48,'roof2')
box('dormer_window',(-1.8,-2.9,4.51),(.77,.07,.66),'light')
box('dormer_cross',(-1.8,-2.96,4.51),(.055,.05,.71),'darkwood')
# chimney
box('chimney_stone',(2.62,1.7,5.17),(.85,.85,2.2),'stone')
box('chimney_cap',(2.62,1.7,6.31),(1.1,1.1,.16),'slate')
# porch entrance, steps
box('porch_landing',(0,-3.47,.24),(2.9,1.4,.32),'stone')
for i in range(3):box('porch_step_%d'%i,(0,-4.15-i*.32,.17-i*.05),(2.5-i*.2,.32,.2),'path')
for x in [-1.35,1.35]:
 cyl('porch_post_base_%s'%x,(x,-3.84,.62),.17,.45,'stone')
 box('porch_post_%s'%x,(x,-3.84,1.85),(.17,.17,2.1),'wood')
box('porch_beam',(0,-3.84,2.97),(3.1,.2,.2),'wood')
roofprism('porch_roof',-1.72,1.72,-4.25,-3.25,3.03,3.7,'roof2')
# front hanging lamp
cyl('porch_lantern',(0,-3.82,2.73),.13,.35,'gold')
# flower beds and planters
for side in [-1,1]:
 for i in range(4):
  x=side*(1.65+i*.45)
  cyl('flower_bush_%s_%d'%(side,i),(x,-3.5,.36),.28,.46,'green')
 for i in range(3):
  x=side*(1.85+i*.65)
  cyl('flower_center_%s_%d'%(side,i),(x,-3.74,.64),.09,.08,'gold')
# trees in landscape
for x,y,scale in [(-5.1,2.3,1.0),(6.0,3.5,.85),(-5.7,-3.5,.7)]:
 cyl('tree_trunk_%s'%x,(x,y,.9*scale),.19*scale,1.8*scale,'wood')
 for z,r in [(1.9,1.0),(2.5,.83),(3.05,.55)]:
  m=trimesh.creation.icosphere(subdivisions=1,radius=r*scale);m.apply_translation((x,y,z*scale));m.visual.face_colors=C['green'];scene.add_geometry(m,geom_name='tree_crown')
# rear side wall accent beams
for y in [-1.6,0,1.6,3.2]:box('side_timber_%s'%y,(-3.69,y,2.2),(.12,.12,2.7),'wood')
# export
scene.export(os.path.join(OUT,'sReader_Storybook_House.glb'))
print('exported',len(scene.geometry),'meshes')
