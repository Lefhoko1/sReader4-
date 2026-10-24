import bpy,math,json,random
from pathlib import Path
from mathutils import Vector
random.seed(42)
OUT=Path(r'C:/Users/11113930/sReader4/GeneratedAssets/ReadingDashboard/20261018_ReferencePolish');OUT.mkdir(parents=True,exist_ok=True)
assert 'Dashboard_Greybox' in bpy.data.filepath,bpy.data.filepath
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_before_reference_polish.blend'))
COMMON=Path(r'C:/Users/11113930/sReader4/GeneratedAssets/LibraryRedesign/20261017_184255/scripts/common.py');exec(compile(COMMON.read_text(),str(COMMON),'exec'))
for name in ['SR_PRODUCTION_Dashboard_Greybox','SR_PREVIEW_Scale_Marker']:
 c=bpy.data.collections.get(name)
 if c:c.hide_render=True;c.hide_viewport=True
for ob in S.objects:
 if ob.type=='LIGHT':ob.hide_render=True
for ob in C['Architecture'].objects:
 if any(s in ob.name for s in ['Front','Entry','Door','Entrance','Signboard','Lantern','Facade','Emblem']):ob.hide_render=True
C['Roof'].hide_render=True
for key in ['Console','Characters','Decor','Demo','StoneSource','Cameras']:
 c=bpy.data.collections.new('SR_'+key);S.collection.children.link(c);C[key]=c
C['Demo']['usage']='Removable illustrative preview only; never export. Assignment text, screen illustration and word examples are supplied by Unity.'
P.update({k:mat('REF_'+k,h,metal,rough,emit) for k,h,metal,rough,emit in [
 ('blue','0568B3',.12,.24,0),('cyan','12BFD7',.08,.22,0),('golden','FFB51B',.25,.26,0),('orange','F48316',.06,.3,0),('deep','07356C',.05,.25,0),('purple','6544C2',.06,.28,0),('white','F7F2DC',0,.32,0),('light','FFF0AC',0,.3,3),('skin','9D522B',0,.53,0),('skin2','C4773F',0,.52,0),('hair','151021',0,.72,0),('brown','89451E',0,.48,0),('pack','BA692D',0,.38,0),('pants','163A6C',0,.6,0),('zskin','A1B36A',0,.64,0),('zshade','6D7D47',0,.6,0),('hood','514756',0,.7,0),('shoe','A53B34',0,.4,0),('eye','FFF8E4',0,.26,0),('iris','4F2C17',0,.24,0),('moss','3E961D',0,.68,0),('mosslight','78B72B',0,.65,0),('rugblue','07559B',0,.9,0),('bubble','FFF6DA',0,.7,0),('screenblank','D8F1F1',0,.8,0)]})
def root(n,col,loc=(0,0,0)):
 o=bpy.data.objects.new(n,None);C[col].objects.link(o);o.location=loc;return o
def B(n,p,d,m,col='Console',e=.08,parent=None):
 o=box(n,p,d,m,col,e)
 for mod in o.modifiers:
  if mod.type=='BEVEL':mod.segments=5
 o.parent=parent;return o
def E(n,p,sc,m,col='Characters',parent=None):
 o=sphere(n,p,sc,m,col);o.parent=parent;return o
def L(n,pts,r,m,col='Console',parent=None):
 o=line(n,pts,r,m,col);o.parent=parent;return o
def T(n,body,p,size,m='deep',col='Demo',parent=None):
 o=text(n,body,p,size,m,col);o.data.extrude=0;o.data.space_line=1.0;o.parent=parent;return o
def A(n,p,parent=None,col='Console'):
 o=root(n,col,p);o.parent=parent;o.empty_display_size=.09;return o
console=root('SR_DASHBOARD_ROOT','Console')
# The front profile is sculpted around a recessed centre, giving a game console silhouette.
profile=[(-3.5,.35),(-3.5,1.25),(-3.23,1.48),(3.23,1.48),(3.5,1.25),(3.5,.35),(2.2,.35),(1.85,.63),(-1.85,.63),(-2.2,.35)]
o=prism('SR_Console_Sculpted_Base',profile,1.7,'blue','Console',y=2.24);o.parent=console
mod=o.modifiers.new('Rounded plastic corners','BEVEL');mod.width=.16;mod.segments=5;o.modifiers.new('Corner normals','WEIGHTED_NORMAL')
B('SR_Tray_Gold_Lip',(0,2.13,1.49),(7.04,1.97,.19),'golden',e=.18,parent=console)
tray=B('SR_Tray_Cyan_Inset',(0,2.18,1.61),(6.65,1.63,.10),'cyan',e=.14,parent=console);tray.rotation_euler.x=.06
B('SR_Tray_Front_Orange_Rail',(0,1.17,1.43),(6.5,.12,.19),'orange',e=.07,parent=console)
B('SR_Display_Integrated_Shell',(0,3.12,2.94),(7.0,.66,3.03),'blue',e=.23,parent=console)
B('SR_Display_Gold_Frame',(0,2.756,2.93),(6.88,.20,2.89),'golden',e=.23,parent=console)
B('SR_Display_Blue_Bezel',(0,2.63,2.93),(6.54,.13,2.60),'blue',e=.2,parent=console)
B('SR_Display_Inner_Navy',(0,2.54,2.91),(6.23,.08,2.31),'deep',e=.17,parent=console)
B('SR_DISPLAY_MAIN',(0,2.485,2.91),(5.98,.025,2.08),'screenblank',e=.13,parent=console)
B('SR_Header_Backbone',(0,3.08,4.55),(6.85,.53,.65),'blue',e=.17,parent=console)
B('SR_Header_Gold_Rail',(0,2.76,4.89),(6.92,.18,.11),'golden',e=.05,parent=console)
B('SR_Assignment_Housing',(-1.85,2.72,4.56),(2.86,.20,.55),'white',e=.18,parent=console)
B('SR_BUTTON_MULTIPLAYER',(2.15,2.72,4.57),(2.15,.22,.51),'purple',e=.18,parent=console)
B('SR_Nameplate_Gold',(0,2.37,4.17),(1.92,.24,.45),'golden',e=.18,parent=console)
B('SR_Nameplate_Blue',(0,2.22,4.17),(1.69,.07,.31),'deep',e=.12,parent=console)
T('SR_Brand','sReader',(0,2.17,4.17),.24,'golden','Console',console)
for x in [-3.23,3.23]:
 B('SR_Display_Light_Gold',(x,2.44,2.92),(.17,.15,.93),'golden',e=.08,parent=console)
 B('SR_Display_Accent_Light',(x,2.34,2.92),(.08,.06,.74),'light',e=.038,parent=console)
B('SR_PANEL_POINTS',(-2.02,1.31,.96),(2.25,.23,.58),'deep',e=.21,parent=console)
B('SR_BUTTON_MENU',(2.02,1.31,.96),(2.25,.23,.58),'deep',e=.21,parent=console)
B('SR_Console_Light_Gold',(0,1.30,1.16),(1.87,.15,.16),'golden',e=.07,parent=console)
B('SR_Console_Light',(0,1.20,1.16),(1.65,.05,.065),'light',e=.03,parent=console)
def star(n,p,r=.19):
 poly=[(p[0]+r*(1 if i%2==0 else .48)*math.sin(i*math.pi/5),p[2]+r*(1 if i%2==0 else .48)*math.cos(i*math.pi/5)) for i in range(10)]
 o=prism(n,poly,.09,'golden','Console',y=p[1]);o.parent=console;mo=o.modifiers.new('Soft star edges','BEVEL');mo.width=.025;mo.segments=3;return o
star('SR_Menu_Star',(1.24,1.13,.98));star('SR_Points_Star',(-2.8,1.13,.99),.16)
for n,p in [('SR_ANCHOR_ASSIGNMENT_HEADER',(-1.85,2.57,4.56)),('SR_ANCHOR_SENTENCE_PROGRESS',(-1.85,2.57,4.42)),('SR_ANCHOR_MULTIPLAYER',(2.15,2.55,4.57)),('SR_ANCHOR_READING_CONTENT',(0,2.44,2.9)),('SR_STONE_TRAY_ANCHOR',(0,1.78,1.68)),('SR_ANCHOR_POINTS',(-2.02,1.15,.96)),('SR_ANCHOR_MENU',(2.02,1.15,.96)),('SR_ANCHOR_PREVIOUS_SENTENCE',(-2.95,1.6,1.67)),('SR_ANCHOR_NEXT_SENTENCE',(2.95,1.6,1.67)),('SR_ANCHOR_READ_PASSAGE',(0,1.15,.91)),('SR_ANCHOR_ASSIGNMENTS',(-1.9,2.5,4.7)),('SR_ANCHOR_INSTRUCTIONS',(0,2.45,2.4))]:A(n,p,console)
region=A('SR_STONE_PLACEMENT_REGION',(0,2.13,1.7),console);region['bounds_m']=[6.1,1.35];region['layout']='Variable word widths; row wrap or paginated views. Preview word stones excluded from production.'
# Modular stone, fixed rounded caps with a horizontally stretchable centre.
source=root('SR_STONE_PREFAB_SOURCE','StoneSource');source['width_rule']='Move caps to +/- centre_width/2; scale centre X only; scale moss centre separately. Fixed cap radius .18; height .46; thickness .24.'
def capmesh(n,side,parent):
 verts=[];faces=[];steps=6;around=16
 for j in range(steps+1):
  a=(j/steps)*math.pi/2
  x=side*.18*math.sin(a);r=math.cos(a)
  for i in range(around):
   t=i*2*math.pi/around;verts.append((x,.12*r*math.cos(t),.23*r*math.sin(t)))
 for j in range(steps):
  for i in range(around):faces.append((j*around+i,j*around+(i+1)%around,(j+1)*around+(i+1)%around,(j+1)*around+i))
 o=mesh(n,verts,faces,'white','StoneSource');o.parent=parent
 for f in o.data.polygons:f.use_smooth=True
 return o
left=capmesh('SR_STONE_CAP_LEFT',-1,source);right=capmesh('SR_STONE_CAP_RIGHT',1,source);left.location.x=-.16;right.location.x=.16
verts=[(x,.12*math.cos(i*2*math.pi/16),.23*math.sin(i*2*math.pi/16)) for x in [-.5,.5] for i in range(16)]
faces=[(i,(i+1)%16,(i+1)%16+16,i+16) for i in range(16)]
center=mesh('SR_STONE_STRETCH_CENTRE',verts,faces,'white','StoneSource');center.parent=source;center.scale.x=.32
for f in center.data.polygons:f.use_smooth=True
A('SR_STONE_TEXT_ANCHOR',(0,-.127,0),source,'StoneSource')
for state,h in [('Normal','F7F2DC'),('Selected','FFD35D'),('Correct','A9E989'),('Completed','9BD8D0'),('Deferred','E6BCF3'),('Disabled','969FA2')]:
 m=mat('STONE_STATE_'+state,h,0,.4);center.data.materials.append(m)
C['StoneSource'].hide_render=True;C['StoneSource'].hide_viewport=True
# Preview instances are separate from all production geometry.
def stone(word,x,y,z,width,index):
 rt=root('DEMO_Stone_'+str(index),'Demo',(x,y,z));cw=max(.05,width-.36)
 for src,xx,sx in [(left,-cw/2,1),(right,cw/2,1),(center,0,cw)]:
  o=src.copy();o.data=src.data;C['Demo'].objects.link(o);o.parent=rt;o.location=(xx,0,0);o.scale=(sx,1,1);o.hide_render=False
 E('DEMO_Moss_Base',(0,.015,-.198),(width*.50,.14,.075),'moss','Demo',rt)
 for k in range(5):E('DEMO_Moss_Tuft',(-width*.4+k*width*.2,-.09,-.18),(.048,.045,.072),'mosslight' if k%2 else 'moss','Demo',rt)
 T('DEMO_Word',word,(0,-.132,.015),min(.13,(width-.07)/max(len(word)*.60,1)),'deep','Demo',rt)
 return rt
words='Lefa packed his bag and set off early in the morning.'.split();widths=[.26+.045*len(w) for w in words];gap=.065;total=sum(widths)+gap*(len(words)-1);xx=-total/2
for i,(w,wi) in enumerate(zip(words,widths)):
 stone(w,xx+wi/2,1.91,1.93,wi,i);xx+=wi+gap
T('DEMO_Assignment','Photosynthesis',(-1.78,2.59,4.66),.18)
T('DEMO_Progress','Sentence 2 of 10',(-1.78,2.585,4.46),.115)
B('DEMO_Progress_Track',(-1.85,2.58,4.35),(1.96,.035,.055),'stone','Demo',.02)
B('DEMO_Progress_Fill',(-2.55,2.55,4.35),(.55,.04,.055),'moss','Demo',.02)
T('DEMO_Multiplayer','Play with Others',(2.15,2.57,4.57),.17,'white')
T('DEMO_Points_Label','Your Points',(-1.97,1.17,1.08),.14,'cyan')
T('DEMO_Points_Value','120',(-1.97,1.17,.87),.25,'golden')
T('DEMO_Menu_Label','Menu',(2.03,1.17,.98),.26,'white')
B('DEMO_Sentence_Card',(0,2.39,3.02),(5.34,.035,.60),'bubble','Demo',.17)
T('DEMO_Sentence','Lefa packed his bag and set off early in the morning.',(0,2.36,3.02),.175)
# Soft toy-like reusable characters. Local front faces -Y.
def limb(n,a,b,r,m,parent):
 mid=(Vector(a)+Vector(b))/2;o=E(n,mid,(r,r,(Vector(a)-Vector(b)).length/2+r*.45),m,parent=parent);o.rotation_euler=(Vector(b)-Vector(a)).to_track_quat('Z','Y').to_euler();return o
def face(prefix,head,skin,zombie=False):
 E(prefix+'_Head',(0,0,0),(.29,.245,.33),skin,parent=head)
 for sx in [-1,1]:
  E(prefix+'_Ear',(sx*.285,-.005,-.015),(.073,.052,.10),skin,parent=head)
  E(prefix+'_Ear_Inner',(sx*.315,-.041,-.012),(.028,.02,.049),'zshade' if zombie else 'skin',parent=head)
  E(prefix+'_Eye_White',(sx*.112,-.218,.055),(.087,.052,.109 if not zombie else .082),'eye',parent=head)
  E(prefix+'_Iris',(sx*.112,-.264,.05),(.047,.017,.062),'iris',parent=head)
  E(prefix+'_Pupil',(sx*.112,-.279,.053),(.024,.008,.041),'hair',parent=head)
  E(prefix+'_Catchlight',(sx*.112-.013,-.287,.078),(.013,.004,.018),'eye',parent=head)
  brow=[(sx*.06,-.236,.18),(sx*.115,-.245,.20 if not zombie else .15),(sx*.18,-.225,.16)]
  L(prefix+'_Brow',brow,.018,'hair','Characters',head)
 E(prefix+'_Nose',(0,-.253,-.035),(.055,.065,.067),skin,parent=head)
 if zombie:
  L(prefix+'_Mouth',[(-.10,-.221,-.18),(0,-.252,-.155),(.11,-.22,-.185)],.017,'zshade','Characters',head)
  B(prefix+'_Tooth',(.035,-.248,-.17),(.034,.024,.034),'white','Characters',.004,head)
  for x in [-.19,.19]:E(prefix+'_Cheek', (x,-.21,-.10),(.056,.012,.031),'zshade',parent=head)
 else:
  E(prefix+'_Smile',(0,-.217,-.151),(.117,.045,.069),'hair',parent=head)
  B(prefix+'_Teeth',(0,-.259,-.129),(.15,.015,.03),'eye','Characters',.012,head)
  E(prefix+'_Lip',(0,-.242,-.199),(.07,.013,.016),'skin',parent=head)
def shoe(n,p,matkey,parent):
 E(n+'_Sole',(p[0],p[1]-.08,p[2]-.045),(.17,.275,.062),'eye',parent=parent)
 E(n+'_Upper',(p[0],p[1]-.07,p[2]+.025),(.15,.245,.10),matkey,parent=parent)
 for j in range(3):L(n+'_Lace',[(p[0]-.08,p[1]-.12+j*.055,p[2]+.11),(p[0]+.08,p[1]-.12+j*.055,p[2]+.11)],.008,'eye','Characters',parent)
def curls(head,cap=False):
 for j in range(52 if not cap else 20):
  ang=random.uniform(0,2*math.pi);zz=random.uniform(.05,.34);r=math.sqrt(max(0,1-(zz/.39)**2))
  E('Hair_Curl',(.29*r*math.cos(ang),.25*r*math.sin(ang),zz),(.07,.065,.075),'hair',parent=head)
def chair(x,y):
 rt=root('SR_Chair','Decor',(x,y,0));B('Chair_Seat',(0,0,.94),(.78,.70,.16),'golden','Decor',.08,rt)
 B('Chair_Back',(0,.30,1.29),(.72,.12,.62),'woodLight','Decor',.08,rt)
 for sx in [-1,1]:
  for sy in [-1,1]:limb('Chair_Leg',(x+sx*.26,y+sy*.23,.92),(x+sx*.34,y+sy*.31,.35),.045,'woodLight',None)
 return rt
# Standing student, viewed from behind with visible backpack and curly hair.
student=root('SR_CHARACTER_STUDENT','Characters',(0,-.57,.34));student.rotation_euler.z=math.pi
B('Student_Shorts',(0,0,.72),(.48,.34,.38),'blue','Characters',.10,student)
E('Student_Shirt',(0,0,1.14),(.31,.20,.43),'blue',parent=student)
B('Student_Collar',(0,-.17,1.43),(.35,.07,.08),'white','Characters',.03,student)
head=A('Student_Head_Pivot',(0,0,1.77),student,'Characters');face('Student',head,'skin');curls(head)
for sx in [-1,1]:
 limb('Student_Leg',(sx*.14,0,.69),(sx*.15,-.015,.19),.105,'skin',student);shoe('Student_Shoe',(sx*.16,-.02,.12),'blue',student)
 limb('Student_Sleeve',(sx*.24,0,1.40),(sx*.34,-.015,1.15),.115,'blue',student)
 limb('Student_Arm',(sx*.34,-.015,1.15),(sx*.36,-.02,.85),.075,'skin',student)
 E('Student_Hand',(sx*.36,-.035,.81),(.082,.065,.105),'skin',parent=student)
 L('Student_Backpack_Strap',[(sx*.20,-.03,1.45),(sx*.23,-.18,1.25),(sx*.18,-.16,.93)],.035,'brown','Characters',student)
B('Student_Backpack',(0,.23,1.13),(.49,.25,.66),'pack','Characters',.13,student)
B('Student_Backpack_Pocket',(0,.389,.99),(.38,.08,.24),'brown','Characters',.065,student)
L('Student_Backpack_Handle',[(-.09,.23,1.46),(-.09,.25,1.56),(.09,.25,1.56),(.09,.23,1.46)],.022,'brown','Characters',student)
B('Student_Pack_Badge',(0,.435,1.24),(.10,.018,.10),'golden','Characters',.014,student)
for zombie,x in [(False,-3.28),(True,3.28)]:
 name='Zombie' if zombie else 'Encourager';chair(x,.60)
 rt=root('SR_CHARACTER_'+name.upper(),'Characters',(x,.57,.35));rt.rotation_euler.z= -.12 if zombie else .12
 skin='zskin' if zombie else 'skin2';cloth='hood' if zombie else 'blue'
 E(name+'_Hood',(0,.05,1.01),(.32,.23,.38),cloth,parent=rt)
 E(name+'_Chest',(0,-.01,1.02),(.28,.19,.33),cloth,parent=rt)
 if not zombie:B(name+'_Hoodie_Panel',(0,-.185,1.09),(.26,.025,.34),'white','Characters',.05,rt)
 head=A(name+'_Head_Pivot',(0,-.02,1.51),rt,'Characters');head.rotation_euler.y=.12 if zombie else -.08
 face(name,head,skin,zombie)
 if zombie:
  for j in range(22):
   ang=random.random()*math.tau;px=.24*math.cos(ang);py=.20*math.sin(ang)
   limb('Zombie_Hair_Tuft',(px,py,.20),(px+.06,py+.03,.37+random.random()*.08),.035,'hood',head)
  E('Zombie_Eyelid_L',( -.112,-.251,.103),(.085,.016,.031),'zskin',parent=head)
  E('Zombie_Eyelid_R',(.112,-.251,.103),(.085,.016,.031),'zskin',parent=head)
 else:
  curls(head,True)
  E('Encourager_Blue_Cap',(0,.005,.225),(.318,.275,.20),'blue',parent=head)
  E('Encourager_Cap_Brim',(0,-.27,.205),(.30,.22,.035),'blue',parent=head)
  B('Encourager_Cap_Badge',(0,-.273,.31),(.16,.03,.11),'golden','Characters',.025,head)
 for sx in [-1,1]:
  limb(name+'_Thigh',(sx*.15,0,.65),(sx*.23,-.35,.63),.13,'pants',rt)
  limb(name+'_Shin',(sx*.23,-.35,.63),(sx*.24,-.38,.18),.09,'pants',rt)
  shoe(name+'_Sneaker',(sx*.24,-.45,.13),'shoe' if zombie else 'blue',rt)
 if zombie:
  limb('Zombie_UpperArm',(-.25,0,1.19),(-.37,-.21,.93),.10,cloth,rt)
  limb('Zombie_Forearm',(-.37,-.21,.93),(-.16,-.36,1.35),.075,skin,rt)
  E('Zombie_Chin_Hand',(-.13,-.35,1.32),(.08,.07,.11),skin,parent=rt)
  limb('Zombie_RightArm',(.27,0,1.15),(.33,-.33,.77),.10,cloth,rt)
  E('Zombie_RightHand',(.29,-.39,.70),(.095,.07,.11),skin,parent=rt)
 else:
  limb('Encourager_LeftSleeve',(-.25,0,1.2),(-.35,-.16,.92),.10,cloth,rt)
  limb('Encourager_LeftHand',(-.35,-.16,.92),(-.22,-.40,.87),.075,skin,rt)
  limb('Encourager_RightSleeve',(.25,0,1.20),(.39,-.10,1.05),.11,cloth,rt)
  limb('Encourager_RightArm',(.39,-.1,1.05),(.46,-.27,1.43),.07,skin,rt)
  E('Encourager_ThumbsUp_Fist',(.46,-.27,1.46),(.09,.07,.095),skin,parent=rt)
  E('Encourager_Thumb',(.42,-.28,1.58),(.041,.045,.11),skin,parent=rt)
  for sx in [-1,1]:
   ob=B('Encourager_Open_Book',(sx*.16,-.40,.87),(.32,.40,.045),'white','Characters',.015,rt);ob.rotation_euler.y=sx*.17
   ob=B('Encourager_Book_Cover',(sx*.16,-.40,.84),(.34,.42,.025),'golden','Characters',.015,rt);ob.rotation_euler.y=sx*.17
 A('SR_ANCHOR_'+name.upper()+'_SPEECH',(0,-.10,2.18),rt,'Characters')
 # Preview speech bubbles sit outside the reading surface.
 B('DEMO_'+name+'_Bubble',(x, .30,2.84),(1.34,.09,.63),'bubble','Demo',.17)
 T('DEMO_'+name+'_Dialogue','This is hard...\nKeep trying?' if zombie else 'Great job!\nYou can do it!',(x,.235,2.84),.125)
# Room dressing and a rug with inlaid gold border.
B('SR_Rug',(0,-.70,.353),(5.65,3.00,.045),'rugblue','Decor',.16)
L('SR_Rug_Gold_Inlay',[(-2.65,-2.02,.38),(2.65,-2.02,.38),(2.65,.62,.38),(-2.65,.62,.38)],.014,'golden','Decor').data.splines[0].use_cyclic_u=True
for x in [-3.72,3.72]:
 pot=cyl('SR_Plant_Pot',(x,2.60,.65),.23,.57,'white','Decor',24)
 cyl('SR_Plant_Rim',(x,2.60,.94),.25,.08,'golden','Decor',24)
 for j in range(7):
  ang=j*math.tau/7;end=Vector((x+.30*math.cos(ang),2.60+.24*math.sin(ang),1.30+random.random()*.5))
  o=E('SR_Plant_Leaf',(Vector((x,2.60,.98))+end)/2,(.08,.055,(end-Vector((x,2.60,.98))).length*.65),'moss','Decor');o.rotation_euler=(end-Vector((x,2.60,.98))).to_track_quat('Z','Y').to_euler()
# Controlled warm interior illumination.
S.world.use_nodes=True;S.world.node_tree.nodes.get('Background').inputs['Color'].default_value=(.30,.40,.57,1);S.world.node_tree.nodes.get('Background').inputs['Strength'].default_value=.22
area('REF_Key',(-2,-1,4.8),(0,2,2),850,(1,.81,.58),4)
area('REF_Fill',(3,-1,3.7),(0,2,2),550,(.58,.78,1),3)
area('REF_Top',(0,2.6,5.4),(0,1,1.4),700,(1,.85,.56),3)
area('REF_Character_Key',(0,-3.1,3.5),(0,.4,1.4),300,(1,.80,.65),3)
for x in [-3.91,3.91]:
 B('SR_Wall_Light_Frame',(x,2.4,3.25),(.19,.25,.67),'golden','Decor',.08)
 B('SR_Wall_Light_Glass',(x,2.23,3.25),(.11,.10,.52),'light','Decor',.045)
cam=camera('CAM_REFERENCE_COMPARISON',(0,-3.65,3.35),(0,1.65,2.40),ortho=8.7)
cam.data.clip_start=.05
S.camera=cam;S.render.engine='CYCLES';S.cycles.samples=48;S.cycles.use_denoising=True
S.render.resolution_x=1536;S.render.resolution_y=1120;S.render.resolution_percentage=100;S.render.image_settings.file_format='PNG';S.view_settings.exposure=0
S.view_settings.view_transform='AgX'
try:S.view_settings.look='AgX - Medium High Contrast'
except:pass
S['source_reference']='Docs/Photosynthesis Reading Adventure.png';S['stage']='Reference-matched geometry pass; all sample reading content is removable in SR_Demo.'
S['exterior_note']='Architecture exterior preserved. Front facade and roof hidden for interior cutaway renders only.'
for screen in bpy.data.screens:
 for ar in screen.areas:
  if ar.type=='VIEW_3D':ar.spaces.active.region_3d.view_perspective='CAMERA'
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_ReadingHouse_Reference_v01.blend'))
S.render.filepath=str(OUT/'01_reference_comparison.png');bpy.ops.render.render(write_still=True)
print(json.dumps({'saved':bpy.data.filepath,'preview':S.render.filepath,'objects':{k:len(C[k].objects) for k in ['Console','Characters','Decor','Demo','StoneSource']}}))
