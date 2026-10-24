import bpy,math,json
from pathlib import Path
from mathutils import Vector
OUT=Path(r'C:/Users/11113930/sReader4/GeneratedAssets/ReadingDashboard/20261018_Greybox')
OUT.mkdir(parents=True,exist_ok=True)
S=bpy.context.scene
assert bpy.data.filepath.endswith('sReader_Classroom_v02.blend')
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_House_before_dashboard.blend'))
backup=bpy.data.collections.new('SR_BACKUP_PreDashboard');S.collection.children.link(backup)
for ob in list(S.objects):
 if all(not c.name.startswith('Backup') for c in ob.users_collection):
  cp=ob.copy()
  if ob.data:cp.data=ob.data.copy()
  backup.objects.link(cp)
backup.hide_render=True;backup.hide_viewport=True
COMMON=Path(r'C:/Users/11113930/sReader4/GeneratedAssets/LibraryRedesign/20261017_184255/scripts/common.py')
exec(compile(COMMON.read_text(),str(COMMON),'exec'))
for name in ['Interactive Props','Chalkboard','Preview Assignment Writing']:
 bpy.data.collections[name].hide_render=True;bpy.data.collections[name].hide_viewport=True
preserved=[];archived=[]
for ob in bpy.data.collections['Interior'].objects:
 keep=('Bookshelf_Module' in ob.name or 'lantern' in ob.name.lower() or 'rug' in ob.name.lower())
 if not keep:ob.hide_render=True;ob.hide_set(True);archived.append(ob.name)
 else:preserved.append(ob.name)
C['Roof'].hide_render=True;C['Roof'].hide_viewport=True
col=bpy.data.collections.new('SR_PRODUCTION_Dashboard_Greybox');S.collection.children.link(col);C['Dashboard']=col
root=bpy.data.objects.new('SR_DASHBOARD_ROOT',None);col.objects.link(root)
P.update({'blue':mat('Dashboard_Blockout_Blue','1566AC',0,.34),'cyan':mat('Dashboard_Blockout_Cyan','38BDD2',0,.4),'yellow':mat('Dashboard_Blockout_Gold','FFC04B',0,.4),'navy':mat('Dashboard_Blockout_Navy','113D70',0,.45),'screen':mat('Dashboard_Blank_Content','E5F2ED',0,.8),'purple':mat('Dashboard_Multiplayer','805CC7',0,.45)})
def b(n,p,d,m,edge=.09):
 o=box(n,p,d,m,'Dashboard',edge);o.parent=root;return o
def anchor(n,p):
 o=bpy.data.objects.new(n,None);col.objects.link(o);o.parent=root;o.location=p;o.empty_display_size=.16;return o
# Single physically connected display/console. Front is negative Y.
b('SR_DASHBOARD_BASE',(0,1.28,.83),(6.2,4.6,1.0),'blue',.18)
b('SR_DASHBOARD_FRONT_FASCIA',(0,-1.035,.93),(5.9,.12,.88),'cyan',.12)
b('SR_REAR_DISPLAY_SHELL',(0,3.29,2.83),(6.2,.64,2.94),'blue',.2)
b('SR_REAR_DISPLAY_TRIM',(0,2.94,2.85),(5.94,.16,2.69),'yellow',.16)
b('SR_REAR_DISPLAY_INNER',(0,2.83,2.85),(5.7,.1,2.45),'navy',.13)
b('SR_DISPLAY_MAIN',(0,2.763,2.69),(5.36,.025,1.94),'screen',.08)
b('SR_HEADER_ASSIGNMENT_HOUSING',(-1.52,2.72,4.0),(2.7,.13,.38),'screen',.09)
b('SR_BUTTON_MULTIPLAYER',(2.0,2.71,4.0),(1.47,.16,.38),'purple',.09)
b('SR_NAMEPLATE',(0,2.66,4.28),(1.6,.19,.32),'yellow',.09)
text('SR_sReader_Nameplate','sReader',(0,2.55,4.28),.19,'navy','Dashboard').parent=root
tray=b('SR_CONSOLE_ANGLED_TRAY',(0,1.05,1.5),(6.35,4.17,.22),'yellow',.12);tray.rotation_euler.x=math.radians(7)
region=b('SR_STONE_PLACEMENT_REGION',(0,1.02,1.635),(5.94,3.78,.055),'cyan',.09);region.rotation_euler.x=math.radians(7)
region['purpose']='Runtime placement region; variable count and widths. No fixed stone slots.'
anchor('SR_STONE_TRAY_ANCHOR',(0,-.4,1.51))
for n,p in [('SR_ANCHOR_ASSIGNMENT_HEADER',(-1.52,2.62,4)),('SR_ANCHOR_SENTENCE_PROGRESS',(-1.52,2.61,3.9)),('SR_ANCHOR_MULTIPLAYER',(2,2.60,4)),('SR_ANCHOR_READING_CONTENT',(0,2.72,2.69)),('SR_ANCHOR_POINTS',(-1.83,-1.14,.92)),('SR_ANCHOR_MENU',(1.83,-1.14,.92))]:anchor(n,p)
b('SR_PANEL_POINTS',(-1.83,-1.13,.92),(2.05,.16,.59),'navy',.12)
b('SR_BUTTON_MENU',(1.83,-1.13,.92),(2.05,.16,.59),'navy',.12)
# A separate neutral human scale marker is intentionally a blockout, not a character.
scale=bpy.data.collections.new('SR_PREVIEW_Scale_Marker');S.collection.children.link(scale);C['Scale']=scale
sphere('PREVIEW_Student_Head',(0,-2.15,1.76),(.19,.18,.22),'paper','Scale')
box('PREVIEW_Student_Torso',(0,-2.15,1.22),(.43,.28,.67),'navy','Scale',.07)
for x in [-.13,.13]:box('PREVIEW_Student_Leg',(x,-2.15,.64),(.16,.19,.52),'navy','Scale',.04)
# Reuse the existing rug but clear its working area from under the console.
for ob in bpy.data.collections['Interior'].objects:
 if 'rug' in ob.name.lower() and ob.type=='MESH':
  ob.hide_render=True;ob.hide_set(True)
box('SR_Rug_Blockout',(0,-2.38,.35),(4.65,2.45,.035),'blue','Dashboard',.08)
for n,loc,target,lens in [('CAM_ROOM_OVERVIEW',(0,-3.65,3.65),(0,1.15,2.1),18),('CAM_DASHBOARD_CLOSEUP',(0,-2.6,3.3),(0,1.6,2.5),19),('CAM_STONES_CLOSEUP',(0,-1.8,3.8),(0,1.0,1.6),22),('CAM_CHARACTER_VIEW',(2.8,-3.4,2.4),(0,-2.15,1.1),40)]:camera(n,loc,target,lens)
area('SR_Greybox_Key',(0,-2,4.1),(0,2,1.8),1100,(1,.84,.65),5)
area('SR_Greybox_Fill',(2,1,4.2),(0,1,1.4),650,(.65,.83,1),3)
S.render.engine='CYCLES';S.cycles.samples=24;S.cycles.use_denoising=True
S.render.resolution_percentage=100;S.render.image_settings.file_format='PNG'
S.camera=bpy.data.objects['CAM_ROOM_OVERVIEW'];S.render.resolution_x=1200;S.render.resolution_y=900
S.render.filepath=str(OUT/'01_dashboard_greybox_overview.png')
S['dashboard_stage']='Greybox for approval; detail, characters and exports pending visual approval.'
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_House_Dashboard_Greybox.blend'))
bpy.ops.render.render(write_still=True)
S.camera=bpy.data.objects['CAM_DASHBOARD_CLOSEUP'];S.render.filepath=str(OUT/'02_dashboard_closeup.png');bpy.ops.render.render(write_still=True)
S.render.resolution_x=720;S.render.resolution_y=1520
S.camera=bpy.data.objects['CAM_STONES_CLOSEUP'];S.render.filepath=str(OUT/'03_portrait_720x1520.png');bpy.ops.render.render(write_still=True)
S.camera=bpy.data.objects['CAM_ROOM_OVERVIEW'];S.render.resolution_x=1200;S.render.resolution_y=900
for screen in bpy.data.screens:
 for a in screen.areas:
  if a.type=='VIEW_3D':a.spaces.active.region_3d.view_perspective='CAMERA'
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'sReader_House_Dashboard_Greybox.blend'))
report={'source':'sReader_Classroom_v02.blend','room_floor_m':[8.72,7.72],'back_wall_y':4.05,'dashboard_footprint_m':[6.2,4.6],'floor_coverage':6.2*4.6/(8.72*7.72),'preserved':preserved,'archived_interior':archived,'dashboard_objects':[o.name for o in col.objects],'stage':'Greybox awaiting visual approval; no Unity exports or system changes.'}
(OUT/'manifest.json').write_text(json.dumps(report,indent=2))
(OUT/'HANDOFF.md').write_text('Stage 1-2 greybox. Original source and hidden SR_BACKUP_PreDashboard preserved. Dashboard at rear +Y wall, front faces -Y, Z up. Blank SR_DISPLAY_MAIN and named runtime anchors; no assignment text or fixed stones. Runtime tray must support wrapping/pagination. Portrait is a geometry framing preview, not a readability or device performance test. Detailed stone asset, characters, polishing and Unity exports await approval as requested in Docs/BlenderVisionForCodex.txt section 14-15.\n')
print(json.dumps({'saved':bpy.data.filepath,'renders':[str(p) for p in OUT.glob('*.png')],'coverage':report['floor_coverage'],'dashboard_objects':len(col.objects)}))
