"""Quick look at a generator output: 4 views with either base colour, plus cross-section slices.
usage: blender -b -P look.py -- <GEN dir> <out dir>"""
import bpy, math, os, sys
from mathutils import Vector
GEN, OUT = sys.argv[sys.argv.index("--") + 1:][:2]
os.makedirs(OUT, exist_ok=True)
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=os.path.join(GEN, "master.glb"))
mesh = next(o for o in bpy.context.scene.objects if o.type == "MESH")
mw = mesh.matrix_world.copy(); mesh.parent = None; mesh.matrix_world = mw
bpy.context.view_layer.objects.active = mesh; mesh.select_set(True)
bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
zs = [v.co.z for v in mesh.data.vertices]; xs = [v.co.x for v in mesh.data.vertices]; ys = [v.co.y for v in mesh.data.vertices]
print("BOUNDS x", round(min(xs),3), round(max(xs),3), "y", round(min(ys),3), round(max(ys),3), "z", round(min(zs),3), round(max(zs),3))
def clusters(vals, gap=0.012):
    vals = sorted(vals); out = [[vals[0], vals[0]]]
    for x in vals[1:]:
        if x - out[-1][1] > gap: out.append([x, x])
        else: out[-1][1] = x
    return [(round(a, 3), round(b, 3)) for a, b in out]
H = max(zs)
for i in range(0, 70):
    z = i * 0.02
    if z > H: break
    sl = [v.co for v in mesh.data.vertices if abs(v.co.z - z) < 0.006]
    if sl: print(f"Z {z:.2f} x {clusters([p.x for p in sl])}  y {clusters([p.y for p in sl])}")
sc = bpy.context.scene
for eng in ("BLENDER_EEVEE_NEXT", "BLENDER_EEVEE"):
    try: sc.render.engine = eng; break
    except TypeError: pass
sc.render.film_transparent = True; sc.render.resolution_x, sc.render.resolution_y = 480, 720
sc.view_settings.view_transform = "Standard"
w = bpy.data.worlds.new("W"); sc.world = w; w.use_nodes = True
next(n for n in w.node_tree.nodes if n.type=="BACKGROUND").inputs[0].default_value = (0.78,0.76,0.74,1)
sun = bpy.data.lights.new("S","SUN"); sun.energy = 2.6; so = bpy.data.objects.new("S", sun); sc.collection.objects.link(so); so.rotation_euler = (math.radians(50), 0, math.radians(-25))
cam = bpy.data.cameras.new("C"); cam.type = "ORTHO"; cam.ortho_scale = H * 1.1
co = bpy.data.objects.new("C", cam); sc.collection.objects.link(co); sc.camera = co
mat = mesh.material_slots[0].material; nt = mat.node_tree
bsdf = next(n for n in nt.nodes if n.type == "BSDF_PRINCIPLED")
texnode = bsdf.inputs["Base Color"].links[0].from_node
for l in list(bsdf.inputs["Metallic"].links): nt.links.remove(l)
bsdf.inputs["Metallic"].default_value = 0
for name in ("clean", "projected"):
    texnode.image = bpy.data.images.load(os.path.join(GEN, "intermediate", "base_color_padded.png" if name == "clean" else "base_color_projected_padded.png"))
    for yaw in (0, 90, 180, 270):
        a = math.radians(yaw); d = 6
        co.location = (math.sin(a) * d, -math.cos(a) * d, H / 2)
        co.rotation_euler = (Vector((0, 0, H / 2)) - co.location).to_track_quat("-Z", "Y").to_euler()
        sc.render.filepath = os.path.join(OUT, f"{name}_{yaw:03d}.png"); bpy.ops.render.render(write_still=True)
