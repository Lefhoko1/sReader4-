import bpy,math,json
from pathlib import Path
from mathutils import Vector
O=Path(r'C:/Users/11113930/sReader4/GeneratedAssets/ReadingDashboard/20261018_Greybox');S=bpy.context.scene
S.view_settings.exposure=-2.3
cam=bpy.data.objects['CAM_ROOM_OVERVIEW'];cam.data.lens=14;cam.location=(0,-3.7,3.5);cam.rotation_euler=(Vector((0,1,2.05))-cam.location).to_track_quat('-Z','Y').to_euler()
S.camera=cam;S.render.resolution_x=1200;S.render.resolution_y=900;S.render.filepath=str(O/'01_dashboard_greybox_overview.png');bpy.ops.render.render(write_still=True)
S.camera=bpy.data.objects['CAM_DASHBOARD_CLOSEUP'];S.render.filepath=str(O/'02_dashboard_closeup.png');bpy.ops.render.render(write_still=True)
da=bpy.data.cameras.new('CAM_PORTRAIT_OVERVIEW');pc=bpy.data.objects.new('CAM_PORTRAIT_OVERVIEW',da);bpy.data.collections['Lighting'].objects.link(pc);pc.location=(0,-3.7,4.0);pc.rotation_euler=(Vector((0,.8,2.1))-pc.location).to_track_quat('-Z','Y').to_euler();da.type='ORTHO';da.ortho_scale=15.0
S.camera=pc;S.render.resolution_x=720;S.render.resolution_y=1520;S.render.filepath=str(O/'03_portrait_720x1520.png');bpy.ops.render.render(write_still=True)
S.camera=cam;S.render.resolution_x=1200;S.render.resolution_y=900
bpy.ops.wm.save_as_mainfile(filepath=str(O/'sReader_House_Dashboard_Greybox.blend'))
print('Saved exposure and camera revisions; all three renders complete.')
