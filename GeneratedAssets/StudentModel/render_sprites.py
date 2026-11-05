"""Render the rigged 3D student into game sprite frames (headless Blender).

usage:
  blender -b -P render_sprites.py -- test <out_dir>
  blender -b -P render_sprites.py -- render <frames_root>

Model: the user's modelCreator output (character_rigged.glb), textured with the
clean AI-painted base colour (the photo-projection pass doubled the eyes).
Every frame is rendered with the same orthographic camera scale and framing,
so size and foot position are identical across all frames and all sets.
"""
import math, os, sys
import bpy
from mathutils import Vector, Euler

GEN = r"C:\Users\11113930\Documents\mine\modelCreator\my-app\outputs\student-boy-20261020-110631"
RIG = os.path.join(GEN, "intermediate", "character_skinned.glb")   # uncompressed (the web copy uses meshopt)
CLEAN_TEX = r"C:\Users\11113930\sReader4\GeneratedAssets\StudentModel\base_color_fixed.png"   # projected body + AI-painted head (make_texture.py)

args = sys.argv[sys.argv.index("--") + 1:]
MODE, OUT = args[0], args[1]

# Facing / sign conventions, settled by the test render.
FACE_YAW = float(os.environ.get("FACE_YAW", "0"))      # yaw (deg) that turns the model to face camera "front"
LEG_SIGN = float(os.environ.get("LEG_SIGN", "1"))
ARM_SIGN = float(os.environ.get("ARM_SIGN", "1"))

RES_W, RES_H = 640, 960          # rendered at 2x; downscaled to 320 x 480 afterwards
CHAR_H = 1.2507
VIEW_H = CHAR_H * 960 / 896      # character fills 448/480 of the canvas height
ELEV = math.radians(8)


MASTER = os.path.join(GEN, "master.glb")
TEXTURE = os.environ.get("TEXTURE", "clean")   # clean = AI paint (clean face) | projected = generator's final


# Joints measured from the master mesh's cross-sections (diag mode). He is chibi: the head
# runs from the neck at 0.72 m to the top, so the generator's adult-proportion rig is not used.
# He faces +Y; his right side is +X.
HIP_Z, NECK_Z = 0.38, 0.72
BONES = {   # name: (head, tail)
    "body": ((0, 0, HIP_Z), (0, 0, NECK_Z)),
    "head": ((0, 0, NECK_Z), (0, 0, 1.20)),
    "leg_right": ((0.09, 0, HIP_Z), (0.09, 0, 0.02)),
    "leg_left": ((-0.09, 0, HIP_Z), (-0.09, 0, 0.02)),
    "arm_right": ((0.15, 0, 0.60), (0.21, 0, 0.32)),
    "arm_left": ((-0.15, 0, 0.60), (-0.21, 0, 0.32)),
}


def _ss(a, b, x):
    t = min(1.0, max(0.0, (x - a) / (b - a))); return t * t * (3 - 2 * t)


def region_weights(p):
    """Soft per-region weights for one vertex (rest position)."""
    ax, z = abs(p.x), p.z
    side = "right" if p.x > 0 else "left"
    w = {}
    head = _ss(0.70, 0.76, z)
    # arms hang clear of the shorts below the elbow; the sleeve blends into the shoulder; never the backpack
    if z < 0.25: arm = 0.0   # the shoes are as wide as the hands
    elif z < 0.54:
        # the gap between the hanging arm and the body, measured slice by slice (the shorts' hem flares out at 0.30-0.335)
        b = 0.173 if z < 0.302 else 0.19 if z < 0.336 else 0.156 if z < 0.36 else 0.162 if z < 0.40 else 0.154
        arm = _ss(b - 0.003, b + 0.003, ax)
    else: arm = _ss(0.13, 0.16, ax) * (1 - _ss(0.56, 0.64, z)) * (1 - _ss(0.066, 0.078, p.y))   # not the front strap
    arm *= _ss(-0.085, -0.065, p.y) * (1 - head)   # the backpack's sides start at y = -0.09
    leg = (1 - _ss(0.27, 0.36, z)) * (1 - arm)
    body = max(0.0, 1 - head - arm - leg)
    for k, v in (("head", head), ("arm_" + side, arm), ("leg_" + side, leg), ("body", body)):
        if v > 1e-4: w[k] = v
    return w


def setup_scene():
    """The generator's full-detail mesh (intact UVs, clean paint) on a skeleton fitted to his proportions."""
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.gltf(filepath=MASTER)
    mesh = next(o for o in bpy.context.scene.objects if o.type == "MESH")
    for o in [o for o in bpy.context.scene.objects if o is not mesh]: bpy.data.objects.remove(o)
    mw = mesh.matrix_world.copy(); mesh.parent = None; mesh.matrix_world = mw
    bpy.ops.object.select_all(action="DESELECT")
    bpy.context.view_layer.objects.active = mesh; mesh.select_set(True)
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)

    adata = bpy.data.armatures.new("Rig")
    arm = bpy.data.objects.new("Rig", adata); bpy.context.scene.collection.objects.link(arm)
    bpy.ops.object.select_all(action="DESELECT")
    bpy.context.view_layer.objects.active = arm; arm.select_set(True)
    bpy.ops.object.mode_set(mode="EDIT")
    for name, (h, t) in BONES.items():
        b = adata.edit_bones.new(name); b.head, b.tail = h, t; b.roll = 0
    for name in ("head", "leg_right", "leg_left", "arm_right", "arm_left"):
        adata.edit_bones[name].parent = adata.edit_bones["body"]
    bpy.ops.object.mode_set(mode="OBJECT")

    # the generator fused the hanging hands to the hips with a few triangles; cut those so the arm swings free
    import bmesh
    bm = bmesh.new(); bm.from_mesh(mesh.data)
    def armw(co): w = region_weights(co); return w.get("arm_left", 0) + w.get("arm_right", 0)
    bridge = [f for f in bm.faces if f.calc_center_median().z < 0.45
              and (lambda ws: max(ws) - min(ws) > 0.15)([armw(v.co) for v in f.verts])]
    bmesh.ops.delete(bm, geom=bridge, context="FACES")
    bm.to_mesh(mesh.data); bm.free()
    print("cut hand-hip bridge faces:", len(bridge))

    groups = {n: mesh.vertex_groups.new(name=n) for n in BONES}
    for v in mesh.data.vertices:
        for n, wt in region_weights(v.co).items():
            groups[n].add([v.index], wt, "REPLACE")
    mod = mesh.modifiers.new("Armature", "ARMATURE"); mod.object = arm
    mesh.parent = arm
    # texture, matte cartoon plastic
    img = bpy.data.images.load(CLEAN_TEX if TEXTURE == "fixed" else os.path.join(GEN, "intermediate", "base_color_padded.png")) if TEXTURE != "projected" else None
    for slot in mesh.material_slots:
        nt = slot.material.node_tree
        bsdf = next(n for n in nt.nodes if n.type == "BSDF_PRINCIPLED")
        for link in list(bsdf.inputs["Base Color"].links):
            src = link.from_node
            if src.type == "TEX_IMAGE" and img is not None:
                src.image = img
        for name in ("Metallic",):
            for link in list(bsdf.inputs[name].links): nt.links.remove(link)
            bsdf.inputs[name].default_value = 0.0
        for link in list(bsdf.inputs["Roughness"].links): nt.links.remove(link)
        bsdf.inputs["Roughness"].default_value = 0.55
        # keep the baked detail normal map (it matches the master mesh); NO_NORMALMAP=1 drops it
        if os.environ.get("NO_NORMALMAP") == "1":
            for link in list(bsdf.inputs["Normal"].links): nt.links.remove(link)
    # root for whole-body yaw / bob
    root = bpy.data.objects.new("Root", None)
    bpy.context.scene.collection.objects.link(root)
    arm.parent = root
    root.rotation_euler = (0, 0, math.radians(FACE_YAW))

    sc = bpy.context.scene
    for eng in ("BLENDER_EEVEE_NEXT", "BLENDER_EEVEE"):
        try: sc.render.engine = eng; break
        except TypeError: pass
    sc.render.film_transparent = True
    sc.render.resolution_x, sc.render.resolution_y = RES_W, RES_H
    sc.render.image_settings.file_format = "PNG"
    sc.render.image_settings.color_mode = "RGBA"
    try: sc.view_settings.view_transform = "Standard"
    except TypeError: pass
    world = bpy.data.worlds.new("W"); sc.world = world; world.use_nodes = True
    bg = next(n for n in world.node_tree.nodes if n.type == "BACKGROUND")
    bg.inputs[0].default_value = (0.78, 0.76, 0.74, 1); bg.inputs[1].default_value = 1.0
    sun = bpy.data.lights.new("Key", "SUN"); sun.energy = 2.6; sun.angle = math.radians(25)
    so = bpy.data.objects.new("Key", sun); sc.collection.objects.link(so)
    so.rotation_euler = Euler((math.radians(50), 0, math.radians(-25)))
    fill = bpy.data.lights.new("Fill", "SUN"); fill.energy = 0.9; fill.angle = math.radians(40)
    fo = bpy.data.objects.new("Fill", fill); sc.collection.objects.link(fo)
    fo.rotation_euler = Euler((math.radians(65), 0, math.radians(150)))
    cam = bpy.data.cameras.new("Cam"); cam.type = "ORTHO"; cam.ortho_scale = VIEW_H * RES_W / RES_H if RES_W > RES_H else VIEW_H
    co = bpy.data.objects.new("Cam", cam); sc.collection.objects.link(co); sc.camera = co
    return arm, mesh, root, co


def place_camera(co, yaw_deg):
    """Camera on a circle round the character, looking at it, slightly from above."""
    yaw = math.radians(yaw_deg)
    centre_z = -VIEW_H * 16 / 960 + VIEW_H / 2
    d = 6.0
    pos = Vector((math.sin(yaw) * d * math.cos(ELEV), -math.cos(yaw) * d * math.cos(ELEV), centre_z + d * math.sin(ELEV)))
    co.location = pos
    look = Vector((0, 0, centre_z)) - pos
    co.rotation_euler = look.to_track_quat("-Z", "Y").to_euler()


def pose(arm, **rot):
    for b in arm.pose.bones:
        b.rotation_mode = "XYZ"
        b.rotation_euler = rot.get(b.name, (0, 0, 0))


def render(path):
    bpy.context.scene.render.filepath = path
    bpy.ops.render.render(write_still=True)


def deg(x): return math.radians(x)


if MODE == "weights":
    arm, mesh, root, co = setup_scene()
    os.makedirs(OUT, exist_ok=True)
    pal = {"body": (0.8, 0.8, 0.8), "head": (1, 0.85, 0.2), "arm_left": (1, 0.1, 0.1), "arm_right": (0.1, 0.4, 1),
           "leg_left": (0.1, 0.8, 0.2), "leg_right": (0.8, 0.2, 0.9)}
    names = {g.index: g.name for g in mesh.vertex_groups}
    attr = mesh.data.color_attributes.new("W", "FLOAT_COLOR", "POINT")
    for v in mesh.data.vertices:
        c = pal[names[max(v.groups, key=lambda g: g.weight).group]] if v.groups else (0, 0, 0)
        attr.data[v.index].color = (*c, 1)
    mat = bpy.data.materials.new("W"); mat.use_nodes = True
    nt = mat.node_tree; nt.nodes.clear()
    a = nt.nodes.new("ShaderNodeVertexColor"); a.layer_name = "W"
    e = nt.nodes.new("ShaderNodeEmission"); o = nt.nodes.new("ShaderNodeOutputMaterial")
    nt.links.new(a.outputs[0], e.inputs[0]); nt.links.new(e.outputs[0], o.inputs[0])
    for s in mesh.material_slots: s.material = mat
    pose(arm)
    for yaw in (0, 90, 180, 270):
        place_camera(co, yaw); render(os.path.join(OUT, f"w_{yaw:03d}.png"))
    sys.exit(0)

if MODE == "facemaps":
    # Straight-on, unlit close-ups of the face: its texture colour, and the UV under every pixel
    # (16-bit, no anti-aliasing), so face_paint.py can paint expressions in screen space and
    # write them back into the texture.
    arm, mesh, root, co = setup_scene()
    os.makedirs(OUT, exist_ok=True)
    pose(arm)
    sc = bpy.context.scene
    FACE_RES, FACE_SCALE, FACE_Z = 2048, 0.40, 0.93
    sc.render.resolution_x = sc.render.resolution_y = FACE_RES
    co.data.ortho_scale = FACE_SCALE
    co.location = (0, 6, FACE_Z); co.rotation_euler = Euler((math.radians(90), 0, math.radians(180)))
    for attr, val in (("taa_render_samples", 1),):
        try: setattr(sc.eevee, attr, val)
        except AttributeError: pass
    try: sc.render.filter_size = 0.0
    except Exception: pass
    sc.render.dither_intensity = 0.0
    bsdf = next(n for n in mesh.material_slots[0].material.node_tree.nodes if n.type == "BSDF_PRINCIPLED")
    tex = bsdf.inputs["Base Color"].links[0].from_node.image
    if os.environ.get("FACE_EXPR"):   # check a painted expression texture as it lands on the model
        tex = bpy.data.images.load(os.path.join(os.path.dirname(os.path.abspath(__file__)), "face", "tex_" + os.environ["FACE_EXPR"] + ".png"))
    print("texture", tex.name, tex.size[:], tex.filepath)
    def flat(kind):
        mat = bpy.data.materials.new(kind); mat.use_nodes = True
        nt = mat.node_tree; nt.nodes.clear()
        e = nt.nodes.new("ShaderNodeEmission"); o = nt.nodes.new("ShaderNodeOutputMaterial")
        nt.links.new(e.outputs[0], o.inputs[0])
        if kind == "uv":
            uv = nt.nodes.new("ShaderNodeUVMap"); nt.links.new(uv.outputs[0], e.inputs[0])
        else:
            t = nt.nodes.new("ShaderNodeTexImage"); t.image = tex
            nt.links.new(t.outputs[0], e.inputs[0])
        for s in mesh.material_slots: s.material = mat
    flat("color")
    sc.view_settings.view_transform = "Standard"
    render(os.path.join(OUT, "face_color.png"))
    if os.environ.get("FACE_EXPR"): sys.exit(0)
    flat("uv")
    sc.view_settings.view_transform = "Raw"
    sc.render.image_settings.color_depth = "16"
    sc.render.film_transparent = True
    render(os.path.join(OUT, "face_uv.png"))
    import json
    json.dump({"res": FACE_RES, "scale": FACE_SCALE, "z": FACE_Z, "texture": bpy.path.abspath(tex.filepath) if tex.filepath else tex.name,
               "tex_size": list(tex.size)}, open(os.path.join(OUT, "face_maps.json"), "w"))
    sys.exit(0)

if MODE == "facetris":
    # Every front-facing triangle of the head: its UVs and where it lands in the facemaps close-up.
    # face_paint.py fills each triangle's texels from the painted close-up -- including the doubled
    # surfaces round the eyes and mouth that a visible-pixel mapping misses (they z-fight).
    import numpy as np
    from bpy_extras.object_utils import world_to_camera_view
    arm, mesh, root, co = setup_scene()
    pose(arm)
    sc = bpy.context.scene
    FACE_RES, FACE_SCALE, FACE_Z = 2048, 0.40, 0.93
    sc.render.resolution_x = sc.render.resolution_y = FACE_RES
    co.data.ortho_scale = FACE_SCALE
    co.location = (0, 6, FACE_Z); co.rotation_euler = Euler((math.radians(90), 0, math.radians(180)))
    bpy.context.view_layer.update()
    me = mesh.data
    me.calc_loop_triangles()
    uvl = me.uv_layers.active.data
    M = mesh.matrix_world
    scr_v = np.array([tuple(world_to_camera_view(sc, co, M @ v.co))[:2] for v in me.vertices])
    uvs, scrs = [], []
    for t in me.loop_triangles:
        n = (M.to_3x3() @ t.normal)
        if n.y < 0.05 or (M @ t.center).z < 0.70: continue
        s = scr_v[list(t.vertices)]
        if (s < 0).all() or (s > 1).all(): continue
        uvs.append([tuple(uvl[li].uv) for li in t.loops]); scrs.append(s)
    np.save(os.path.join(OUT, "tri_uv.npy"), np.array(uvs, np.float64))
    np.save(os.path.join(OUT, "tri_scr.npy"), np.array(scrs, np.float64))
    print("face triangles", len(uvs))
    sys.exit(0)

if MODE == "pointtest":
    arm, mesh, root, co = setup_scene()
    os.makedirs(OUT, exist_ok=True)
    for i, (fwd, side) in enumerate([(70, 25), (70, -25), (95, 0), (85, 20)]):
        pose(arm, arm_right=(deg(fwd), 0, deg(side)))
        for yaw in (0, 90):
            place_camera(co, yaw); render(os.path.join(OUT, f"p{i}_{yaw:03d}.png"))
    sys.exit(0)

if MODE == "diag":
    arm, mesh, root, co = setup_scene()
    import collections
    names = {g.index: g.name for g in mesh.vertex_groups}
    stats = collections.defaultdict(list)
    for v in mesh.data.vertices:
        if not v.groups: stats["none"].append(v.co); continue
        g = max(v.groups, key=lambda g: g.weight)
        stats[names[g.group]].append(v.co)
    for k, pts in stats.items():
        xs, ys, zs = zip(*[(p.x, p.y, p.z) for p in pts])
        print(f"{k:10s} n={len(pts):6d} x[{min(xs):.3f},{max(xs):.3f}] y[{min(ys):.3f},{max(ys):.3f}] z[{min(zs):.3f},{max(zs):.3f}]")
    def clusters(vals, gap=0.012):
        vals = sorted(vals); out = [[vals[0], vals[0]]]
        for x in vals[1:]:
            if x - out[-1][1] > gap: out.append([x, x])
            else: out[-1][1] = x
        return [(round(a, 3), round(b, 3)) for a, b in out]
    for i in range(50, 90):
        z = i * 0.005
        sl = [v.co for v in mesh.data.vertices if abs(v.co.z - z) < 0.0025 and v.co.x > 0.13 and v.co.y > -0.085]
        if sl: print(f"A {z:.3f} x {clusters([p.x for p in sl], 0.004)}")
    for i in range(0, 64):
        z = i * 0.02
        sl = [v.co for v in mesh.data.vertices if abs(v.co.z - z) < 0.006]
        if not sl: continue
        print(f"Z {z:.2f} x {clusters([p.x for p in sl])}  y {clusters([p.y for p in sl])}")
    chest = [v.co for v in mesh.data.vertices if 0.80 < v.co.z < 0.95 and abs(v.co.x) < 0.04]
    print("chest y range", min(p.y for p in chest), max(p.y for p in chest))
    for z in (0.75, 0.85, 0.95, 1.05):
        sl = [v.co for v in mesh.data.vertices if abs(v.co.z - z) < 0.01]
        print("slice z", z, "x", round(min(p.x for p in sl), 3), round(max(p.x for p in sl), 3), "y", round(min(p.y for p in sl), 3), round(max(p.y for p in sl), 3))
    sys.exit(0)

if MODE == "test":
    os.makedirs(OUT, exist_ok=True)
    arm, mesh, root, co = setup_scene()
    print("bones:", [b.name for b in arm.pose.bones])
    for b in arm.data.bones:
        print(b.name, tuple(round(v, 3) for v in b.head_local), tuple(round(v, 3) for v in b.tail_local))
    pose(arm)
    for n in mesh.material_slots[0].material.node_tree.nodes: print("node", n.type, n.name, getattr(getattr(n, "image", None), "name", ""))
    for yaw in (0, 90, 180, 270):
        place_camera(co, yaw); render(os.path.join(OUT, f"cam_{yaw:03d}.png"))
    pose(arm, leg_left=(deg(24), 0, 0), leg_right=(deg(-24), 0, 0), arm_right=(deg(40), 0, 0), arm_left=(deg(-20), 0, 0))
    place_camera(co, 90); render(os.path.join(OUT, "pose_side_090.png"))
    place_camera(co, 270); render(os.path.join(OUT, "pose_side_270.png"))
    place_camera(co, 0); render(os.path.join(OUT, "pose_back_000.png"))
    sys.exit(0)

# ── full render ───────────────────────────────────────────────────────────────
arm, mesh, root, co = setup_scene()
HIP = HIP_Z
FRONT, RIGHT, BACK = 180.0, float(os.environ.get("RIGHT_YAW", "90")), 0.0   # camera yaw 0 sees his back


def walk(t):
    """t in [0,1): one full cycle (two steps)."""
    s = math.sin(2 * math.pi * t)
    leg = 24 * s
    rot = {
        "leg_left": (deg(LEG_SIGN * leg), 0, 0),
        "leg_right": (deg(-LEG_SIGN * leg), 0, 0),
        "arm_left": (deg(-ARM_SIGN * 20 * s), 0, 0),
        "arm_right": (deg(ARM_SIGN * 20 * s), 0, 0),
        "body": (deg(3), 0, deg(4 * s)),
        "head": (deg(-2), 0, deg(-3 * s)),
    }
    pose(arm, **rot)
    # keep the planted foot on the floor (rigid legs swinging from the hip), plus a little spring
    drop = HIP * (1 - math.cos(deg(abs(leg))))
    bob = 0.012 * (0.5 - 0.5 * math.cos(4 * math.pi * t))
    root.location = (0, 0, -drop + bob)


def idle(t):
    s = math.sin(2 * math.pi * t)
    pose(arm, body=(deg(1.2 * s), 0, 0), head=(deg(-1.5 * s), 0, deg(1.5 * math.sin(2 * math.pi * t + 1))),
         arm_left=(deg(-2 * s), 0, deg(-1.5)), arm_right=(deg(-2 * s), 0, deg(1.5)))
    root.location = (0, 0, 0.004 * s)


def point(t):
    k = min(1.0, t / 0.5); k = k * k * (3 - 2 * k)
    lean = max(0.0, (t - 0.5) / 0.5); lean = lean * lean * (3 - 2 * lean)
    # raise the right arm forward and out to the side, so it reads past his shoulder from behind
    pose(arm, arm_right=(deg(ARM_SIGN * (70 * k + 8 * lean)), 0, deg(-25 * k)),
         body=(deg(4 * lean), 0, 0), head=(deg(3 * lean), 0, 0))
    root.location = (0, 0, 0)


FACE_DIR = os.path.join(os.path.dirname(os.path.abspath(__file__)), "face")
_face_node = next(n for n in mesh.material_slots[0].material.node_tree.nodes if n.type == "BSDF_PRINCIPLED").inputs["Base Color"].links[0].from_node
_face_images = {None: _face_node.image}


def set_face(expr):
    """Swap the base colour for an expression texture painted by face_paint.py (None = his normal face)."""
    if expr not in _face_images:
        _face_images[expr] = bpy.data.images.load(os.path.join(FACE_DIR, f"tex_{expr}.png"))
    _face_node.image = _face_images[expr]


# name, camera yaw, motion, frames, loop, face, idle-phase step (blink sets sample every 2nd idle phase)
SETS = [
    ("walk_toward", FRONT, walk, 24, True, None, 1),
    ("walk_away", BACK, walk, 24, True, None, 1),
    ("walk_right", RIGHT, walk, 24, True, None, 1),
    ("idle_front", FRONT, idle, 32, True, None, 1),
    ("idle_back", BACK, idle, 32, True, None, 1),
    ("point_back", BACK, point, 24, False, None, 1),
    ("blink_half", FRONT, idle, 16, True, "half", 2),
    ("blink_closed", FRONT, idle, 16, True, "closed", 2),
    ("smile_front", FRONT, idle, 32, True, "smile", 1),
]
only = os.environ.get("ONLY")
for name, cam_yaw, fn, n, loop, face, step in SETS:
    if only and name not in only.split(","): continue
    d = os.path.join(OUT, name); os.makedirs(d, exist_ok=True)
    place_camera(co, cam_yaw)
    set_face(face)
    for i in range(n):
        fn(i / n if loop else i / (n - 1))   # n frames spread over one cycle, so blink frame k = idle frame 2k
        render(os.path.join(d, f"{name}_{i:02d}.png"))
    print("rendered", name, n)
set_face(None)

# turn: front -> back, the camera stays in front while he turns round
if not only or "turn_to_back" in only:
    d = os.path.join(OUT, "turn_to_back"); os.makedirs(d, exist_ok=True)
    place_camera(co, FRONT)
    n = 12
    for i in range(n):
        k = i / (n - 1); k = k * k * (3 - 2 * k)
        pose(arm); root.location = (0, 0, 0)
        root.rotation_euler = (0, 0, math.radians(FACE_YAW + 180 * k))
        render(os.path.join(d, f"turn_to_back_{i:02d}.png"))
    root.rotation_euler = (0, 0, math.radians(FACE_YAW))
    print("rendered turn_to_back", n)
