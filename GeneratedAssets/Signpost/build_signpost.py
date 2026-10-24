"""
The Reading House's outdoor signpost and garden bench (headless Blender):
    blender -b -P build_signpost.py -- <out_dir>

Exports SM_Signpost.fbx and SM_Bench.fbx. Real-world metres at the house's scale (the student is
~2.45 m tall). The signpost faces -Y (Blender front); an empty "FrontMarker" sits 1 m in front of it so
Unity can tell which way it faces. Named objects (Unity looks them up by name):
    Post, Arm, Lantern_*, Chain_*, Sign_Header (navy arrow, gold rim: Sign_HeaderRim),
    Sign_Library, Sign_Play, Sign_Progress, Sign_Settings (wooden planks, nail heads).
Material slots: Wood, WoodDark, Navy, Gold, Iron, Glass. Unity assigns its own materials to them.
"""
import math
import os
import random
import sys

import bpy
import bmesh
from mathutils import Vector

OUT = sys.argv[sys.argv.index("--") + 1]
os.makedirs(OUT, exist_ok=True)
random.seed(7)


def clear():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete()
    for block in (bpy.data.meshes, bpy.data.materials, bpy.data.curves):
        for b in list(block):
            block.remove(b)


def mat(name, rgb):
    m = bpy.data.materials.get(name) or bpy.data.materials.new(name)
    m.diffuse_color = (*rgb, 1)
    return m


def box(name, size, loc, material, bevel=0.03, segments=2, rot=(0, 0, 0)):
    bpy.ops.mesh.primitive_cube_add(size=1, location=loc, rotation=rot)
    o = bpy.context.object
    o.name = name
    o.scale = size
    bpy.ops.object.transform_apply(scale=True)
    if bevel > 0:
        b = o.modifiers.new("bevel", "BEVEL")
        b.width = bevel
        b.segments = segments
        b.limit_method = "ANGLE"
    o.data.materials.append(material)
    return o


def cyl(name, r, depth, loc, material, rot=(0, 0, 0), verts=16):
    bpy.ops.mesh.primitive_cylinder_add(radius=r, depth=depth, location=loc, rotation=rot, vertices=verts)
    o = bpy.context.object
    o.name = name
    o.data.materials.append(material)
    return o


def plank_uvs(o):
    """Box-project UVs (wood grain along the plank's length)."""
    bpy.context.view_layer.objects.active = o
    o.select_set(True)
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.uv.cube_project(cube_size=1.2)
    bpy.ops.object.mode_set(mode="OBJECT")
    o.select_set(False)


def apply_mods(o):
    bpy.context.view_layer.objects.active = o
    for m in list(o.modifiers):
        bpy.ops.object.modifier_apply(modifier=m.name)


def arrow_prism(name, w, h, d, tip, loc, material):
    """A sign board with a pointed right end (the 'this way' arrow), centred on loc, face towards -Y."""
    hw, hh = w / 2, h / 2
    pts = [(-hw, -hh), (hw - tip, -hh), (hw, 0), (hw - tip, hh), (-hw, hh)]
    bm = bmesh.new()
    front = [bm.verts.new((x, -d / 2, z)) for x, z in pts]
    back = [bm.verts.new((x, d / 2, z)) for x, z in pts]
    bm.faces.new(list(reversed(front)))
    bm.faces.new(back)
    n = len(pts)
    for i in range(n):
        j = (i + 1) % n
        bm.faces.new((front[i], front[j], back[j], back[i]))
    me = bpy.data.meshes.new(name)
    bmesh.ops.recalc_face_normals(bm, faces=bm.faces)     # outward, so the front face is the one seen
    bm.normal_update()
    bm.to_mesh(me)
    bm.free()
    o = bpy.data.objects.new(name, me)
    bpy.context.collection.objects.link(o)
    o.location = loc
    o.data.materials.append(material)
    b = o.modifiers.new("bevel", "BEVEL")
    b.width = min(0.03, d * 0.3)
    b.segments = 2
    b.limit_method = "ANGLE"
    for p in me.polygons:
        p.use_smooth = False
    return o


def export(path, objs):
    bpy.ops.object.select_all(action="DESELECT")
    for o in objs:
        o.select_set(True)
    bpy.ops.export_scene.fbx(filepath=path, use_selection=True, object_types={"MESH", "EMPTY"},
                             apply_scale_options="FBX_SCALE_ALL", axis_forward="-Z", axis_up="Y",
                             apply_unit_scale=True, bake_space_transform=False, mesh_smooth_type="FACE",
                             use_mesh_modifiers=True, add_leaf_bones=False)


# ── the signpost ─────────────────────────────────────────────────────────────
clear()
WOOD, DARK, NAVY, GOLD, IRON, GLASS = (mat("Wood", (0.55, 0.33, 0.16)), mat("WoodDark", (0.35, 0.2, 0.1)),
                                       mat("Navy", (0.07, 0.12, 0.28)), mat("Gold", (0.95, 0.72, 0.3)),
                                       mat("Iron", (0.12, 0.12, 0.13)), mat("Glass", (1.0, 0.8, 0.45)))
objs = []
POST_H = 5.2
post = box("Post", (0.3, 0.3, POST_H), (0, 0, POST_H / 2), DARK, bevel=0.04)
post_cap = box("PostCap", (0.38, 0.38, 0.12), (0, 0, POST_H + 0.06), DARK, bevel=0.03)
arm = box("Arm", (3.1, 0.22, 0.22), (1.45, 0, POST_H - 0.35), DARK, bevel=0.035)
arm_stub = box("ArmStub", (0.7, 0.2, 0.2), (-0.45, 0, POST_H - 0.35), DARK, bevel=0.03)
brace = box("Brace", (0.9, 0.14, 0.14), (0.38, 0, POST_H - 0.75), DARK, bevel=0.025, rot=(0, math.radians(45), 0))
objs += [post, post_cap, arm, arm_stub, brace]

# The header: a navy arrow with a gold rim, hanging from the arm on two chains.
HEADER_Z = POST_H - 1.4
header_rim = arrow_prism("Sign_HeaderRim", 2.75, 0.95, 0.12, 0.42, (1.62, 0, HEADER_Z), GOLD)
header = arrow_prism("Sign_Header", 2.55, 0.78, 0.16, 0.34, (1.58, 0, HEADER_Z), NAVY)
objs += [header_rim, header]
for i, x in enumerate((0.65, 2.45)):
    top, bottom = POST_H - 0.47, HEADER_Z + 0.47
    links = int((top - bottom) / 0.12)
    for k in range(links):
        z = top - (k + 0.5) * (top - bottom) / links
        bpy.ops.mesh.primitive_torus_add(major_radius=0.05, minor_radius=0.014, location=(x, 0, z),
                                         rotation=(math.radians(90), 0, math.radians(90 * (k % 2))))
        l = bpy.context.object
        l.name = f"Chain_{i}_{k}"
        l.data.materials.append(IRON)
        objs.append(l)

# The lantern, hanging from the stub on the left.
LX, LZ = -0.62, POST_H - 0.95
hook = cyl("Lantern_Hook", 0.02, 0.32, (LX, 0, POST_H - 0.55), IRON)
lantern_top = cyl("Lantern_Top", 0.22, 0.12, (LX, 0, LZ + 0.33), IRON, verts=4, rot=(0, 0, math.radians(45)))
roof = bpy.ops.mesh.primitive_cone_add(radius1=0.24, radius2=0.04, depth=0.18, vertices=4, location=(LX, 0, LZ + 0.47),
                                       rotation=(0, 0, math.radians(45)))
lantern_roof = bpy.context.object
lantern_roof.name = "Lantern_Roof"
lantern_roof.data.materials.append(IRON)
lantern_base = cyl("Lantern_Base", 0.2, 0.08, (LX, 0, LZ - 0.3), IRON, verts=4, rot=(0, 0, math.radians(45)))
glass = box("Lantern_Glass", (0.26, 0.26, 0.5), (LX, 0, LZ + 0.02), GLASS, bevel=0.0)
objs += [hook, lantern_top, lantern_roof, lantern_base, glass]
for i, (dx, dy) in enumerate(((1, 1), (1, -1), (-1, 1), (-1, -1))):
    objs.append(box(f"Lantern_Bar_{i}", (0.035, 0.035, 0.6), (LX + dx * 0.14, dy * 0.14, LZ + 0.02), IRON, bevel=0.0))

# The four planks, nailed to the post, each a little uneven.
PLANKS = ("Sign_Library", "Sign_Play", "Sign_Progress", "Sign_Settings")
for i, name in enumerate(PLANKS):
    z = HEADER_Z - 0.92 - i * 0.74
    tilt = math.radians(random.uniform(-1.6, 1.6))
    p = arrow_prism(name, 2.35, 0.64, 0.12, 0.0001, (1.0, -0.22, z), WOOD)
    p.rotation_euler = (0, tilt, 0)
    objs.append(p)
    for k, nx in enumerate((-0.12, 0.12)):
        n = cyl(f"{name}_Nail_{k}", 0.035, 0.03, (nx, -0.29, z + (0.14 if k else -0.14)), IRON, rot=(math.radians(90), 0, 0), verts=10)
        objs.append(n)
    for k, nx in enumerate((2.0,)):
        n = cyl(f"{name}_Nail_end", 0.03, 0.03, (nx, -0.29, z), IRON, rot=(math.radians(90), 0, 0), verts=10)
        objs.append(n)

for o in objs:
    if o.type == "MESH":
        apply_mods(o)
        if o.data.materials and o.data.materials[0] in (WOOD, DARK):
            plank_uvs(o)

bpy.ops.object.empty_add(location=(0, -1, 0))
front = bpy.context.object
front.name = "FrontMarker"
objs.append(front)
export(os.path.join(OUT, "SM_Signpost.fbx"), objs)

# ── the garden bench ─────────────────────────────────────────────────────────
clear()
WOOD, DARK, IRON = mat("Wood", (0.55, 0.33, 0.16)), mat("WoodDark", (0.35, 0.2, 0.1)), mat("Iron", (0.12, 0.12, 0.13))
objs = []
L, SEAT_Z = 2.6, 0.82
for i in range(3):
    objs.append(box(f"Seat_{i}", (L, 0.2, 0.07), (0, -0.25 + i * 0.23, SEAT_Z), WOOD, bevel=0.02))
for i in range(3):
    objs.append(box(f"Back_{i}", (L, 0.07, 0.17), (0, 0.38, SEAT_Z + 0.35 + i * 0.24), WOOD, bevel=0.02,
                    rot=(math.radians(-10), 0, 0)))
for sx in (-1, 1):
    x = sx * (L / 2 - 0.18)
    objs.append(box(f"LegFront_{sx}", (0.13, 0.13, SEAT_Z), (x, -0.3, SEAT_Z / 2), DARK, bevel=0.02))
    objs.append(box(f"LegBack_{sx}", (0.13, 0.13, SEAT_Z + 1.0), (x, 0.4, (SEAT_Z + 1.0) / 2), DARK, bevel=0.02,
                    rot=(math.radians(-10), 0, 0)))
    objs.append(box(f"Rail_{sx}", (0.12, 0.85, 0.1), (x, 0.05, SEAT_Z - 0.12), DARK, bevel=0.02))
    objs.append(box(f"Arm_{sx}", (0.16, 0.85, 0.08), (x + sx * 0.02, 0.0, SEAT_Z + 0.42), WOOD, bevel=0.025))
    objs.append(box(f"ArmPost_{sx}", (0.1, 0.1, 0.42), (x, -0.32, SEAT_Z + 0.21), DARK, bevel=0.02))
for o in objs:
    apply_mods(o)
    plank_uvs(o)
bpy.ops.object.empty_add(location=(0, -1, 0))
front = bpy.context.object
front.name = "FrontMarker"
objs.append(front)
export(os.path.join(OUT, "SM_Bench.fbx"), objs)
print("SIGNPOST_DONE")
