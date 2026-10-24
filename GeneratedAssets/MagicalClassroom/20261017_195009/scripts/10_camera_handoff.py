import bpy,math,json
from pathlib import Path
from mathutils import Vector
out=Path(__file__).resolve().parents[1]
scene=bpy.context.scene
assert 'sReader_Classroom' in bpy.data.filepath
poses=[('SR_Camera_Classroom_Portrait',(0,-3.9,3.6),(1.6,1.3,1.4),110),('SR_Camera_Classroom_Board',(.7,.1,2.25),(4.1,.1,2.25),78),('SR_Camera_Classroom_Book',(0,-1.5,3.6),(0,1,0),65)]
for name,pos,target,fov in poses:
 ob=bpy.data.objects[name];ob.location=pos;ob.rotation_euler=(Vector(target)-ob.location).to_track_quat('-Z','Y').to_euler();ob.data.sensor_fit='VERTICAL';ob.data.sensor_height=32;ob.data.lens=16/math.tan(math.radians(fov/2))
for i in range(4):bpy.data.objects['PREVIEW_Word'+str(i)].location.z=2.25+.005*(78-i*70)
scene['handoff_note']='v02 updates portrait cameras and Blender-only sample lettering. Exported architectural FBX remains unchanged from v01. Current assignment lettering and activity feedback are live Unity data.'
scene['unity_scene']='Assets/Scenes/SC_LibraryRedesignPreview.unity'
bpy.ops.wm.save_as_mainfile(filepath=str(out/'sReader_Classroom_v02.blend'))
report={'blend':bpy.data.filepath,'geometry_changed':False,'fbx':'Assets/Art/MagicalClassroom/Models/SM_MagicalClassroom.fbx','unity_live_components':['Current assignment text','Touch input','Camera transitions','Reading UI','TrekSession feedback']}
(out/'reports/blender_camera_handoff.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print(json.dumps(report))
