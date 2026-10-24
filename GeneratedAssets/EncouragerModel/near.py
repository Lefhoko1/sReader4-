import bpy, sys
from mathutils import Vector
bpy.ops.wm.read_factory_settings(use_empty=True)
for f in ("SM_ReadingHouse_Environment.fbx", "SM_ReadingHouse_Dashboard.fbx", "SM_ReadingHouse_Characters_OPT.fbx"):
    before = set(bpy.data.objects)
    bpy.ops.import_scene.fbx(filepath=r"C:\Users\11113930\sReader4\Assets\Art\ReadingHouse\Models" + "\\" + f)
    for o in set(bpy.data.objects) - before: o["src"] = f
enc = next(o for o in bpy.data.objects if o.name.startswith("SR_CHARACTER_ENCOURAGER"))
c = enc.matrix_world.translation
print("ENC", tuple(round(v, 3) for v in c))
for o in bpy.data.objects:
    if o.type != "MESH" or o.name.startswith(("SR_ENCOURAGER", "Encourager")): continue
    pts = [o.matrix_world @ Vector(b) for b in o.bound_box]
    mn = Vector(map(min, *pts)); mx = Vector(map(max, *pts))
    if mn.x - 0.5 < c.x < mx.x + 0.5 and mn.y - 0.6 < c.y < mx.y + 0.6 and mx.z < 2.0:
        print("NEAR", o.name, o["src"], "x", round(mn.x, 2), round(mx.x, 2), "y", round(mn.y, 2), round(mx.y, 2), "z", round(mn.z, 2), round(mx.z, 2), "parent", o.parent.name if o.parent else "")
