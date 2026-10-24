import bpy,json
from pathlib import Path
from mathutils import Vector
ROOT=Path(__file__).resolve().parents[1]
s=bpy.context.scene
cam=bpy.data.objects['SR_Camera_Hero'];target=Vector((.3,-.3,.95));cam.location=target+(cam.location-target)*1.22;cam.rotation_euler=(target-cam.location).to_track_quat('-Z','Y').to_euler()
s.camera=cam;s.render.filepath=str(ROOT/'renders'/'03_hero_activity.png');bpy.ops.render.render(write_still=True)
s.camera=bpy.data.objects['SR_Camera_Interior'];s.render.filepath=str(ROOT/'renders'/'02_interior_overview.png')
bpy.data.collections['Roof'].hide_render=True;bpy.data.collections['Roof'].hide_viewport=True
bpy.ops.wm.save_as_mainfile(filepath=str(ROOT/'sReader_Library_FINAL.blend'))
print(json.dumps({'saved':bpy.data.filepath,'camera':s.camera.name,'roof_hidden':bpy.data.collections['Roof'].hide_render,'original_objects':len(bpy.data.collections['Backup'].objects),'preview':'03_hero_activity.png'}))
