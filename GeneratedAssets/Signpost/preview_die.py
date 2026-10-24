import bpy, sys, math
out = sys.argv[sys.argv.index("--") + 1]
bpy.ops.object.select_all(action="SELECT"); bpy.ops.object.delete()
bpy.ops.import_scene.fbx(filepath=r"C:/Users/11113930/sReader4/Assets/Art/ReadingHouse/Models/Die/SM_ReadingDie.fbx")
cols = {"DieBody": (0.55,0.33,0.16,1), "DiePlate": (1,0.95,0.85,1), "DieTrim": (0.95,0.7,0.25,1), "DieGem": (0.3,0.75,1,1)}
for m in bpy.data.materials:
    m.use_nodes = True
    b = next(n for n in m.node_tree.nodes if n.type == "BSDF_PRINCIPLED")
    for k, v in cols.items():
        if m.name.startswith(k): b.inputs["Base Color"].default_value = v
    if m.name.startswith(("DieTrim",)): b.inputs["Metallic"].default_value = 1; b.inputs["Roughness"].default_value = 0.3
for o in bpy.context.scene.objects: o.rotation_euler.z += math.radians(35)
bpy.ops.object.camera_add(location=(0, -3.2, 2.0)); cam = bpy.context.object
cam.rotation_euler = (math.radians(62), 0, 0); bpy.context.scene.camera = cam
bpy.ops.object.light_add(type="SUN"); bpy.context.object.rotation_euler = (math.radians(40), math.radians(20), math.radians(-30)); bpy.context.object.data.energy = 4
w = bpy.data.worlds.new("w"); bpy.context.scene.world = w; w.use_nodes = True; w.node_tree.nodes["Background"].inputs[0].default_value = (0.5,0.6,0.75,1)
s = bpy.context.scene
try: s.render.engine = "BLENDER_EEVEE_NEXT"
except TypeError: s.render.engine = "BLENDER_EEVEE"
s.render.resolution_x = s.render.resolution_y = 500; s.render.filepath = out
bpy.ops.render.render(write_still=True)
