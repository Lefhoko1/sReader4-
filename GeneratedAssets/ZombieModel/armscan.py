import bpy, sys, os
GEN = sys.argv[sys.argv.index("--") + 1]
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=os.path.join(GEN, "master.glb"))
mesh = next(o for o in bpy.context.scene.objects if o.type == "MESH")
mw = mesh.matrix_world.copy(); mesh.parent = None; mesh.matrix_world = mw
bpy.context.view_layer.objects.active = mesh; mesh.select_set(True)
bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
def clusters(vals, gap):
    vals = sorted(vals); out = [[vals[0], vals[0]]]
    for x in vals[1:]:
        if x - out[-1][1] > gap: out.append([x, x])
        else: out[-1][1] = x
    return [(round(a, 3), round(b, 3)) for a, b in out]
for i in range(84, 136):
    z = i * 0.005
    sl = [v.co for v in mesh.data.vertices if abs(v.co.z - z) < 0.0025 and v.co.x > 0.10]
    if sl: print(f"A {z:.3f} x {clusters([p.x for p in sl], 0.004)}  y {clusters([p.y for p in sl], 0.006)}")
