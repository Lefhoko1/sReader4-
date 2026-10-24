"""
The reading die (headless Blender):  blender -b -P build_die.py -- <out_dir>

One richly made 1 m die for the Reading House word game: a softly rounded body, a raised rounded
plate on every face (the words / pictures sit on these), a slim trim frame round each plate and a
little faceted gem on every corner. Exports SM_ReadingDie.fbx.
Objects: Body, Plate_PX/NX/PY/NY/PZ/NZ (Blender axes: +X, -X, +Y, -Y, +Z, -Z), Trim_*, Gem_*.
Material slots: DieBody, DiePlate, DieTrim, DieGem (Unity swaps them per die tier).
"""
import math
import os
import sys

import bpy
import bmesh
from mathutils import Vector, Matrix

OUT = sys.argv[sys.argv.index("--") + 1]
os.makedirs(OUT, exist_ok=True)

bpy.ops.object.select_all(action="SELECT")
bpy.ops.object.delete()


def mat(name, rgb):
    m = bpy.data.materials.new(name)
    m.diffuse_color = (*rgb, 1)
    return m


BODY, PLATE, TRIM, GEM = mat("DieBody", (0.6, 0.38, 0.2)), mat("DiePlate", (1, 0.96, 0.88)), mat("DieTrim", (0.95, 0.72, 0.3)), mat("DieGem", (0.4, 0.8, 1))
objs = []


def apply(o):
    bpy.context.view_layer.objects.active = o
    for m in list(o.modifiers):
        bpy.ops.object.modifier_apply(modifier=m.name)


# The body: a 1 m cube with generously rounded edges and corners.
bpy.ops.mesh.primitive_cube_add(size=1.0)
body = bpy.context.object
body.name = "Body"
b = body.modifiers.new("bevel", "BEVEL")
b.width = 0.11
b.segments = 6
b.limit_method = "NONE"
body.data.materials.append(BODY)
apply(body)
for p in body.data.polygons:
    p.use_smooth = True
objs.append(body)


def rounded_square(name, size, depth, radius, material, segs=6):
    """A rounded-square slab centred on the origin in XY, depth along Z."""
    bm = bmesh.new()
    hs = size / 2 - radius
    pts = []
    for cx, cy, a0 in ((hs, hs, 0), (-hs, hs, 90), (-hs, -hs, 180), (hs, -hs, 270)):
        for k in range(segs + 1):
            a = math.radians(a0 + 90 * k / segs)
            pts.append((cx + radius * math.cos(a), cy + radius * math.sin(a)))
    top = [bm.verts.new((x, y, depth / 2)) for x, y in pts]
    bot = [bm.verts.new((x, y, -depth / 2)) for x, y in pts]
    bm.faces.new(top)
    bm.faces.new(list(reversed(bot)))
    n = len(pts)
    for i in range(n):
        j = (i + 1) % n
        bm.faces.new((bot[i], bot[j], top[j], top[i]))
    bmesh.ops.recalc_face_normals(bm, faces=bm.faces)
    me = bpy.data.meshes.new(name)
    bm.to_mesh(me)
    bm.free()
    o = bpy.data.objects.new(name, me)
    bpy.context.collection.objects.link(o)
    o.data.materials.append(material)
    bv = o.modifiers.new("bevel", "BEVEL")
    bv.width = min(0.012, depth * 0.4)
    bv.segments = 2
    bv.limit_method = "ANGLE"
    return o


def frame(name, outer, inner, depth, radius, material):
    o = rounded_square(name, outer, depth, radius, material)
    cut = rounded_square(name + "_cut", inner, depth * 3, radius * 0.8, material)
    apply(o)
    bo = o.modifiers.new("cut", "BOOLEAN")
    bo.object = cut
    bo.operation = "DIFFERENCE"
    apply(o)
    bpy.data.objects.remove(cut)
    return o


FACES = {"PX": Vector((1, 0, 0)), "NX": Vector((-1, 0, 0)), "PY": Vector((0, 1, 0)),
         "NY": Vector((0, -1, 0)), "PZ": Vector((0, 0, 1)), "NZ": Vector((0, 0, -1))}
for tag, n in FACES.items():
    rot = Vector((0, 0, 1)).rotation_difference(n).to_matrix().to_4x4()
    plate = rounded_square(f"Plate_{tag}", 0.74, 0.03, 0.09, PLATE)
    plate.matrix_world = Matrix.Translation(n * 0.505) @ rot
    trim = frame(f"Trim_{tag}", 0.84, 0.76, 0.026, 0.12, TRIM)
    trim.matrix_world = Matrix.Translation(n * 0.507) @ rot
    objs += [plate, trim]

# A small faceted gem on each corner.
for i, (sx, sy, sz) in enumerate([(x, y, z) for x in (-1, 1) for y in (-1, 1) for z in (-1, 1)]):
    d = Vector((sx, sy, sz)).normalized()
    bpy.ops.mesh.primitive_ico_sphere_add(subdivisions=1, radius=0.07, location=d * 0.79)
    g = bpy.context.object
    g.name = f"Gem_{i}"
    g.data.materials.append(GEM)
    objs.append(g)

for o in objs:
    apply(o)
    o.select_set(False)
# Bake every part's placement into its mesh (identity transforms export unambiguously).
for o in objs:
    o.data.transform(o.matrix_world)
    o.matrix_world = Matrix.Identity(4)

bpy.ops.object.select_all(action="DESELECT")
for o in objs:
    o.select_set(True)
bpy.ops.export_scene.fbx(filepath=os.path.join(OUT, "SM_ReadingDie.fbx"), use_selection=True, object_types={"MESH"},
                         apply_scale_options="FBX_SCALE_ALL", axis_forward="-Z", axis_up="Y", apply_unit_scale=True,
                         mesh_smooth_type="FACE", use_mesh_modifiers=True, add_leaf_bones=False)
print("DIE_DONE")
