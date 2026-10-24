import bpy, sys, math
from mathutils import Vector
out = sys.argv[sys.argv.index("--") + 1]
bpy.ops.object.select_all(action="SELECT"); bpy.ops.object.delete()
bpy.ops.import_scene.fbx(filepath=r"C:/Users/11113930/sReader4/Assets/Art/ReadingHouse/Models/Signpost/SM_Signpost.fbx")
bpy.ops.import_scene.fbx(filepath=r"C:/Users/11113930/sReader4/Assets/Art/ReadingHouse/Models/Signpost/SM_Bench.fbx")
for o in bpy.context.scene.objects:
    if o.name.startswith(("Seat","Back","Leg","Rail","Arm_","ArmPost")) : o.location.x += 4.5
for m in bpy.data.materials:
    m.use_nodes = True
    b = next(n for n in m.node_tree.nodes if n.type == "BSDF_PRINCIPLED"); b.inputs["Base Color"].default_value = m.diffuse_color
bpy.ops.object.camera_add(location=(2.5, -11, 2.6)); cam = bpy.context.object
cam.rotation_euler = (math.radians(88), 0, 0); bpy.context.scene.camera = cam; cam.data.lens = 35
bpy.ops.object.light_add(type="SUN", location=(3, -5, 8)); bpy.context.object.rotation_euler = (math.radians(50), 0, math.radians(-30)); bpy.context.object.data.energy = 4
w = bpy.context.scene.world or bpy.data.worlds.new("w"); bpy.context.scene.world = w
w.use_nodes = True; w.node_tree.nodes["Background"].inputs[0].default_value = (0.6, 0.75, 0.9, 1)
s = bpy.context.scene
try: s.render.engine = "BLENDER_EEVEE_NEXT"
except TypeError: s.render.engine = "BLENDER_EEVEE"
s.render.resolution_x, s.render.resolution_y = 900, 600; s.render.filepath = out
bpy.ops.render.render(write_still=True)
