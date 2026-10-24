import bpy, math, os, sys
from mathutils import Vector, Euler
OUT = sys.argv[sys.argv.index("--") + 1]
os.makedirs(OUT, exist_ok=True)
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.fbx(filepath=r"C:\Users\11113930\sReader4\Assets\Art\ReadingHouse\Models\SM_ReadingHouse_Characters_OPT.fbx")
root = next(o for o in bpy.data.objects if o.name.startswith("SR_CHARACTER_ENCOURAGER"))
parts = [root] + list(root.children_recursive)
for o in bpy.data.objects:
    if o not in parts: o.hide_render = True
print("ROOT", root.name, root.type, tuple(round(v,3) for v in root.location), tuple(round(math.degrees(v),1) for v in root.rotation_euler), tuple(round(v,3) for v in root.scale))
mn = Vector((1e9,)*3); mx = Vector((-1e9,)*3)
for o in parts:
    if o.type != "MESH": continue
    for c in o.bound_box:
        w = o.matrix_world @ Vector(c); mn = Vector(map(min, mn, w)); mx = Vector(map(max, mx, w))
    print("PART", o.name, len(o.data.vertices), [m.name for m in o.data.materials][:3], "arm" if any(md.type=="ARMATURE" for md in o.modifiers) else "", len(o.vertex_groups))
print("BOUNDS", tuple(round(v,3) for v in mn), tuple(round(v,3) for v in mx))
print("ARMATURES", [o.name for o in bpy.data.objects if o.type=="ARMATURE"])
sc = bpy.context.scene
for eng in ("BLENDER_EEVEE_NEXT", "BLENDER_EEVEE"):
    try: sc.render.engine = eng; break
    except TypeError: pass
sc.render.film_transparent = True; sc.render.resolution_x, sc.render.resolution_y = 480, 640
w = bpy.data.worlds.new("W"); sc.world = w; w.use_nodes = True
next(n for n in w.node_tree.nodes if n.type=="BACKGROUND").inputs[0].default_value = (0.8,0.8,0.8,1)
sun = bpy.data.lights.new("S","SUN"); sun.energy = 3; so = bpy.data.objects.new("S", sun); sc.collection.objects.link(so); so.rotation_euler = (0.8, 0, -0.4)
cam = bpy.data.cameras.new("C"); cam.type = "ORTHO"; co = bpy.data.objects.new("C", cam); sc.collection.objects.link(co); sc.camera = co
ctr = (mn + mx) / 2; size = max(mx - mn) * 1.15; cam.ortho_scale = size
for yaw in (0, 90, 180, 270):
    a = math.radians(yaw); d = 10
    co.location = ctr + Vector((math.sin(a)*d, -math.cos(a)*d, 0))
    co.rotation_euler = (ctr - co.location).to_track_quat("-Z", "Y").to_euler()
    sc.render.filepath = os.path.join(OUT, f"view_{yaw:03d}.png"); bpy.ops.render.render(write_still=True)
