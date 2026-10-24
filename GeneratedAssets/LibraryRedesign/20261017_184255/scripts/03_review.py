import bpy,json,time
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
s=bpy.context.scene
prefs=bpy.context.preferences.addons['cycles'].preferences
try:
 prefs.compute_device_type='OPTIX';prefs.get_devices()
 for d in prefs.devices:d.use=d.type=='OPTIX'
 if any(d.type=='OPTIX' for d in prefs.devices):s.cycles.device='GPU'
except Exception as e:print('GPU fallback',str(e))
s.cycles.samples=24;s.render.resolution_x=1200;s.render.resolution_y=1000
bpy.data.collections['Roof'].hide_render=True;bpy.data.collections['Roof'].hide_viewport=True
s.camera=bpy.data.objects['SR_Camera_Interior'];s.render.filepath=str(ROOT/'renders'/'review_interior.png')
bpy.ops.render.render(write_still=True)
print(json.dumps({'render':s.render.filepath,'devices':[(d.name,d.type,d.use) for d in prefs.devices]}))
