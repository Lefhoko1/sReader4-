"""Run in Blender Scripting workspace. Place next to the supplied GLB file."""
import bpy, os, math
from mathutils import Vector
folder = os.path.dirname(bpy.data.filepath) if bpy.data.filepath else os.path.dirname(os.path.abspath(__file__))
# Change this if you saved the files somewhere else:
model_path = os.path.join(folder, 'sReader_Storybook_House.glb')
if not os.path.isfile(model_path):
    raise FileNotFoundError('Put the GLB next to this script or change model_path: ' + model_path)
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)
bpy.ops.import_scene.gltf(filepath=model_path)
# world
world = bpy.data.worlds.new('Soft dusk atmosphere') if not bpy.data.worlds else bpy.data.worlds[0]
bpy.context.scene.world=world
world.use_nodes=True
world.node_tree.nodes.get('Background').inputs['Color'].default_value=(0.25,0.31,0.42,1)
world.node_tree.nodes.get('Background').inputs['Strength'].default_value=0.6
# camera
bpy.ops.object.camera_add(location=(11,-15,11))
cam=bpy.context.object;cam.name='House showcase camera'
direction=Vector((0,0,2))-cam.location
cam.rotation_euler=direction.to_track_quat('-Z','Y').to_euler()
cam.data.type='ORTHO';cam.data.ortho_scale=17
bpy.context.scene.camera=cam
# lights
def area(name,loc,power,size,color):
 bpy.ops.object.light_add(type='AREA',location=loc)
 ob=bpy.context.object;ob.name=name;ob.data.energy=power;ob.data.shape='DISK';ob.data.size=size;ob.data.color=color
 ob.rotation_euler=(Vector((0,0,1))-ob.location).to_track_quat('-Z','Y').to_euler()
area('Warm key light',(-4,-6,12),2400,8,(1,.81,.58))
area('Cool fill light',(7,4,10),1700,9,(.62,.75,1))
area('Front bounce',(0,-10,5),800,6,(1,.86,.69))
scene=bpy.context.scene
scene.render.engine='CYCLES'
scene.cycles.samples=48
scene.render.resolution_x=1400;scene.render.resolution_y=1000;scene.render.resolution_percentage=100
scene.render.image_settings.file_format='PNG'
scene.render.filepath=os.path.join(folder,'sReader_Storybook_House_Render.png')
bpy.ops.wm.save_as_mainfile(filepath=os.path.join(folder,'sReader_Storybook_House.blend'))
print('Scene saved. Press F12 to render.')
