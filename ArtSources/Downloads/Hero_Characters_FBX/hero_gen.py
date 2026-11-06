"""
=============================================================================
 THE GREAT LIBRARY — Hero & Character Asset Generator (bpy)
=============================================================================
 Generates the remaining Blender assets from the Production Bible manifest:

 CATEGORY B — hero/interactive statics (Models_Hero/, use existing pipeline):
   SM_Book_Hero_Open      the reading surface itself + clean-UV page child
   SM_Librarian_Desk      Entrance Hall hub piece
   SM_Fountain_Dry        two-state pair; Flowing has separate water meshes
   SM_Fountain_Flowing      (water children named *_Water_Glass so the Unity
                             pipeline auto-assigns the transparent material)
   SM_Door_Rune_Sealed    six rune plates as SEPARATE child meshes (each can
                          light individually per Lexicon word)
   SM_Lexicon_Pedestal    centre of the Lexicon Chamber

 CATEGORY C — rigged, ANIMATED characters (Models_Characters/, keep OUT of
 the pipeline folder so their base-colour materials import as-is):
   SK_Owl                 rig: Root/Body/Head/WingL/WingR/Tail/BrowL/BrowR
                          6 clips @24fps: Idle_Sleep, Idle_Perch, React_Happy,
                          React_Lean, Hint_Point, Fly_Short
   SK_BookSpirit          rig: Root/Body/CoverL/CoverR
                          3 clips: Idle_Float, Flutter_Happy, Talk

 NOT generated (deliberately): SK_Librarian — procedural humanoids read as
 mistakes; hand-model per the quality standard.

 Run:  KIT_TEX=/home/claude/kitgen/tex /home/claude/bpyenv/bin/python hero_gen.py
=============================================================================
"""
import os, sys, json, math, random, traceback
os.environ.setdefault("KIT_OUT", "/home/claude/herogen/out")
os.environ.setdefault("KIT_TEX", "/home/claude/kitgen/tex")
sys.path.insert(0, "/home/claude/kitgen")

import bpy, bmesh
from mathutils import Vector, Matrix, Euler

from kit_generator import (reset_scene, link, new_mesh_obj, commit, box,
                           cylinder, _face_layer, _trim_index, uv_trim_map,
                           finish, set_origin, add_socket, tri_count,
                           build_materials, export_fbx, _arch_cut,
                           BEVEL_ARCH, BEVEL_PROP, BEVEL_CLUTTER)

OUT_HERO = "/home/claude/herogen/out/Models_Hero"
OUT_CHAR = "/home/claude/herogen/out/Models_Characters"
SEED = 20260723
HERO_BUDGET = (4000, 8000)
CHAR_BUDGET = (6000, 10000)

# ---------------------------------------------------------------------------
#  small extra helpers
# ---------------------------------------------------------------------------
def sphere(bm, r, segs, rings, center=(0,0,0), scale=(1,1,1), trim="stone"):
    res = bmesh.ops.create_uvsphere(bm, u_segments=segs, v_segments=rings,
                                    radius=r)
    verts = res['verts']
    m = Matrix.Translation(Vector(center)) @ Matrix.Diagonal((*scale,1)).to_4x4()
    bmesh.ops.transform(bm, matrix=m, verts=verts)
    fl = _face_layer(bm)
    for f in {f for v in verts for f in v.link_faces}:
        f[fl] = _trim_index(trim)
    return verts

def apply_all_modifiers(ob):
    bpy.context.view_layer.objects.active = ob
    for o in bpy.context.selected_objects: o.select_set(False)
    ob.select_set(True)
    for m in list(ob.modifiers):
        bpy.ops.object.modifier_apply(modifier=m.name)

def join_into(target, extra):
    bpy.context.view_layer.objects.active = target
    for o in bpy.context.selected_objects: o.select_set(False)
    target.select_set(True); extra.select_set(True)
    bpy.ops.object.join()

# ===========================================================================
#  B1 — SM_Book_Hero_Open
# ===========================================================================
def build_book_hero(MAT):
    root, bm = new_mesh_obj("SM_Book_Hero_Open")
    # covers: hinged at spine (x=0), slight open angle
    for side in (-1, 1):
        cov = box(bm, (0.31, 0.44, 0.028), (side*0.155, 0, 0), "leather")
        piv = Matrix.Translation((0,0,0)) @ \
              Matrix.Rotation(math.radians(-6*side), 4, 'Y') @ \
              Matrix.Translation((0,0,0.014))
        bmesh.ops.transform(bm, matrix=piv, verts=cov)
        # gilded cover edge
        edge = box(bm, (0.315, 0.445, 0.006), (side*0.157, 0, 0), "gold_band")
        bmesh.ops.transform(bm, matrix=piv @ Matrix.Translation((0,0,0.018)),
                            verts=edge)
    # spine block
    box(bm, (0.07, 0.44, 0.05), (0, 0, 0.028), "leather")
    # page block sides: stepped slabs approximating the curved stack
    for side in (-1, 1):
        for i, (w, h) in enumerate([(0.27,0.030),(0.24,0.026),(0.20,0.020)]):
            box(bm, (w, 0.415 - i*0.008, h),
                (side*(0.03 + w/2), 0, 0.045 + sum(
                    x[1] for x in [(0.27,0.030),(0.24,0.026),(0.20,0.020)][:i])
                 + h/2), "parchment")
    commit(root, bm)
    finish(root, BEVEL_PROP, MAT)

    # --- page spread child: gently curved grid with CLEAN 0-1 UVs -----------
    pages = bpy.data.objects.new("SM_Book_Hero_Open_Pages",
                                 bpy.data.meshes.new("pages"))
    link(pages)
    pb = bmesh.new()
    res = bmesh.ops.create_grid(pb, x_segments=16, y_segments=8, size=1.0)
    for v in res['verts']:
        px, py = v.co.x, v.co.y            # -1..1
        v.co.x = px * 0.285
        v.co.y = py * 0.205
        # curve: valley at the spine, gentle dome per page
        v.co.z = 0.125 + 0.018*math.sin(min(abs(px),1.0)*math.pi) \
                       - 0.010*abs(px)*0.3
    uvl = pb.loops.layers.uv.new("UVMap")
    for f in pb.faces:
        for l in f.loops:
            l[uvl].uv = ((l.vert.co.x/0.285)*0.5 + 0.5,
                         (l.vert.co.y/0.205)*0.5 + 0.5)
    pb.to_mesh(pages.data); pb.free()
    # thickness
    sol = pages.modifiers.new("Sol", 'SOLIDIFY'); sol.thickness = 0.012
    apply_all_modifiers(pages)
    pages.data.polygons.foreach_set("use_smooth",
                                    [True]*len(pages.data.polygons))
    pgmat = bpy.data.materials.get("M_BookPages") or \
            bpy.data.materials.new("M_BookPages")
    pgmat.use_nodes = True
    pgmat.node_tree.nodes["Principled BSDF"].inputs["Base Color"]\
         .default_value = (0.957, 0.914, 0.827, 1)
    pages.data.materials.append(pgmat)
    pages.parent = root

    # gem + sockets
    gem = bpy.data.objects.new("SM_Book_Hero_Open_Gem",
                               bpy.data.meshes.new("gem"))
    link(gem)
    gb = bmesh.new()
    bmesh.ops.create_icosphere(gb, subdivisions=1, radius=0.022)
    gb.to_mesh(gem.data); gb.free()
    gem.location = (0, -0.20, 0.062)
    gmat = bpy.data.materials.new("M_SpineGem"); gmat.use_nodes = True
    nd = gmat.node_tree.nodes["Principled BSDF"]
    nd.inputs["Base Color"].default_value = (0.357, 0.247, 0.659, 1)
    nd.inputs["Emission Color"].default_value = (0.55, 0.4, 1.0, 1)
    nd.inputs["Emission Strength"].default_value = 1.5
    gem.data.materials.append(gmat)
    gem.parent = root
    add_socket(root, "SOCKET_SpineGem", (0, -0.20, 0.062))
    add_socket(root, "SOCKET_PageCenter", (0, 0, 0.16))
    set_origin(root, (0, 0, 0))
    return root

# ===========================================================================
#  B2 — SM_Librarian_Desk
# ===========================================================================
def build_librarian_desk(MAT):
    ob, bm = new_mesh_obj("SM_Librarian_Desk")
    box(bm, (2.2, 0.85, 0.06), (0, 0, 1.02), "planks_wide")        # counter top
    box(bm, (2.26, 0.91, 0.03), (0, 0, 1.055), "gold_band")        # gilt lip
    box(bm, (2.1, 0.75, 0.96), (0, 0, 0.51), "wainscot")           # body
    for i in range(3):                                              # drawers
        box(bm, (0.56, 0.03, 0.22), (-0.66 + i*0.66, -0.39, 0.78), "planks_trim")
        box(bm, (0.10, 0.05, 0.03), (-0.66 + i*0.66, -0.41, 0.78), "gold_band")
    box(bm, (2.1, 0.75, 0.10), (0, 0, 0.05), "planks_trim")        # plinth
    # raised back panel with sign board
    box(bm, (2.2, 0.10, 0.55), (0, 0.42, 1.33), "wainscot")
    box(bm, (2.24, 0.12, 0.06), (0, 0.42, 1.63), "gold_band")
    box(bm, (1.1, 0.04, 0.30), (0, 0.365, 1.34), "parchment")      # sign
    commit(ob, bm)
    finish(ob, BEVEL_PROP, MAT)
    add_socket(ob, "SOCKET_Lamp",  ( 0.85, 0.15, 1.06))
    add_socket(ob, "SOCKET_Books", (-0.75, 0.10, 1.06))
    add_socket(ob, "SOCKET_Bell",  ( 0.35, -0.15, 1.06))
    set_origin(ob, (0, 0, 0))
    return ob

# ===========================================================================
#  B3/B4 — Fountains (dry & flowing)
# ===========================================================================
def _fountain_body(name, MAT):
    ob, bm = new_mesh_obj(name)
    cylinder(bm, 1.18, 0.10, 16, (0, 0, 0.05), "stone")            # base slab
    cylinder(bm, 1.02, 0.38, 16, (0, 0, 0.29), "stone")            # basin wall
    cylinder(bm, 1.10, 0.09, 16, (0, 0, 0.505), "gold_band")       # basin rim
    cylinder(bm, 0.15, 0.75, 12, (0, 0, 0.85), "stone", r2=0.12)   # column
    cylinder(bm, 0.48, 0.10, 14, (0, 0, 1.27), "stone", r2=0.42)   # upper bowl
    cylinder(bm, 0.52, 0.06, 14, (0, 0, 1.33), "gold_band")        # bowl rim
    sphere(bm, 0.09, 10, 8, (0, 0, 1.46), trim="gold_band")        # finial
    # four little book-spout blocks around the column
    for k in range(4):
        a = k * math.pi/2
        box(bm, (0.16, 0.10, 0.10),
            (math.cos(a)*0.24, math.sin(a)*0.24, 1.12), "leather")
    commit(ob, bm)
    finish(ob, BEVEL_PROP, MAT)
    return ob

def build_fountain(MAT, flowing):
    name = "SM_Fountain_Flowing" if flowing else "SM_Fountain_Dry"
    ob = _fountain_body(name, MAT)
    if flowing:
        w = bpy.data.objects.new(name + "_Water_Glass",
                                 bpy.data.meshes.new("water"))
        link(w)
        wb = bmesh.new()
        fl = wb.faces.layers.int.new("trim")
        c1 = bmesh.ops.create_cone(wb, cap_ends=True, segments=16,
                                   radius1=0.96, radius2=0.96, depth=0.02)
        bmesh.ops.transform(wb, matrix=Matrix.Translation((0,0,0.46)),
                            verts=c1['verts'])
        c2 = bmesh.ops.create_cone(wb, cap_ends=True, segments=14,
                                   radius1=0.40, radius2=0.40, depth=0.02)
        bmesh.ops.transform(wb, matrix=Matrix.Translation((0,0,1.30)),
                            verts=c2['verts'])
        c3 = bmesh.ops.create_cone(wb, cap_ends=False, segments=10,
                                   radius1=0.045, radius2=0.03, depth=0.78)
        bmesh.ops.transform(wb, matrix=Matrix.Translation((0,0,0.88)),
                            verts=c3['verts'])
        wb.to_mesh(w.data); wb.free()
        w.data.polygons.foreach_set("use_smooth",
                                    [True]*len(w.data.polygons))
        glass = bpy.data.materials.get("M_LibraryGlass")
        if glass: w.data.materials.append(glass)
        w.parent = ob
        add_socket(ob, "SOCKET_WaterFX", (0, 0, 1.34))
    add_socket(ob, "SOCKET_KnowledgeGlow", (0, 0, 1.46))
    set_origin(ob, (0, 0, 0))
    return ob

# ===========================================================================
#  B5 — SM_Door_Rune_Sealed
# ===========================================================================
def build_rune_door(MAT):
    ob, bm = new_mesh_obj("SM_Door_Rune_Sealed")
    box(bm, (3.0, 0.45, 3.8), (0, 0, 1.9), "stone")                # frame block
    commit(ob, bm)
    _arch_cut(ob, 2.1, 2.8, 2.15, depth=1.2)                       # opening
    # door slabs filling the opening (recessed)
    bm2 = bmesh.new()
    fl2 = bm2.faces.layers.int.new("trim")
    def tag(verts, t):
        for f in {f for v in verts for f in v.link_faces}:
            f[fl2] = _trim_index(t)
    for side in (-1, 1):
        v = bmesh.ops.create_cube(bm2, size=1.0)['verts']
        bmesh.ops.transform(bm2, matrix=Matrix.Translation((side*0.5, 0.05, 1.05))
                            @ Matrix.Diagonal((0.98, 0.10, 2.1, 1)).to_4x4(),
                            verts=v)
        tag(v, "wainscot")
        for pz in (0.45, 1.15, 1.75):                              # plank bands
            b = bmesh.ops.create_cube(bm2, size=1.0)['verts']
            bmesh.ops.transform(bm2, matrix=Matrix.Translation((side*0.5, -0.01, pz))
                                @ Matrix.Diagonal((0.9, 0.03, 0.10, 1)).to_4x4(),
                                verts=b)
            tag(b, "gold_band")
    # arched door top filler
    top = bmesh.ops.create_cone(bm2, cap_ends=True, segments=16,
                                radius1=1.02, radius2=1.02, depth=0.10)
    bmesh.ops.transform(bm2, matrix=Matrix.Rotation(math.radians(90),4,'X'),
                        verts=top['verts'])
    bmesh.ops.transform(bm2, matrix=Matrix.Translation((0, 0.05, 2.12)),
                        verts=top['verts'])
    tag(top['verts'], "wainscot")
    slab = bpy.data.objects.new("slab", bpy.data.meshes.new("slab"))
    bm2.to_mesh(slab.data); bm2.free(); link(slab)
    join_into(ob, slab)
    finish(ob, BEVEL_PROP, MAT)

    # six rune plates as SEPARATE children (Lexicon lock lights them 1-by-1)
    rune_positions = [(-0.55, 0.9), (0.55, 0.9), (-0.55, 1.5), (0.55, 1.5),
                      (-0.30, 2.15), (0.30, 2.15)]
    rmat = bpy.data.materials.new("M_RunePlate"); rmat.use_nodes = True
    nd = rmat.node_tree.nodes["Principled BSDF"]
    nd.inputs["Base Color"].default_value = (0.30, 0.24, 0.45, 1)
    nd.inputs["Emission Color"].default_value = (0.55, 0.4, 1.0, 1)
    nd.inputs["Emission Strength"].default_value = 0.0     # lit at runtime
    for i, (rx, rz) in enumerate(rune_positions):
        rp = bpy.data.objects.new(f"SM_Door_Rune_Sealed_RunePlate_{i+1:02d}",
                                  bpy.data.meshes.new(f"rune{i}"))
        link(rp)
        rb = bmesh.new()
        bmesh.ops.create_cone(rb, cap_ends=True, segments=10,
                              radius1=0.10, radius2=0.085, depth=0.035)
        rb.to_mesh(rp.data); rb.free()
        rp.rotation_euler = Euler((math.radians(90), 0, 0))
        rp.location = (rx, -0.02, rz)
        rp.data.materials.append(rmat)
        rp.parent = ob
    add_socket(ob, "SOCKET_UnsealFX", (0, -0.1, 1.6))
    set_origin(ob, (0, 0, 0))
    return ob

# ===========================================================================
#  B6 — SM_Lexicon_Pedestal
# ===========================================================================
def build_lexicon_pedestal(MAT):
    ob, bm = new_mesh_obj("SM_Lexicon_Pedestal")
    cylinder(bm, 0.55, 0.10, 14, (0, 0, 0.05), "stone")            # base
    cylinder(bm, 0.42, 0.06, 14, (0, 0, 0.13), "gold_band")
    cylinder(bm, 0.22, 0.80, 12, (0, 0, 0.56), "stone", r2=0.17)   # column
    cylinder(bm, 0.30, 0.06, 12, (0, 0, 0.99), "gold_band")        # capital
    lect = box(bm, (0.52, 0.42, 0.05), (0, 0, 1.10), "planks_wide")# lectern
    bmesh.ops.transform(bm, matrix=Matrix.Translation((0,0,1.10)) @
                        Matrix.Rotation(math.radians(-18), 4, 'X') @
                        Matrix.Translation((0,0,-1.10)), verts=lect)
    lip = box(bm, (0.52, 0.04, 0.05), (0, -0.20, 1.05), "gold_band")
    bmesh.ops.transform(bm, matrix=Matrix.Translation((0,0,1.10)) @
                        Matrix.Rotation(math.radians(-18), 4, 'X') @
                        Matrix.Translation((0,0,-1.10)), verts=lip)
    commit(ob, bm)
    finish(ob, BEVEL_PROP, MAT)
    # floating rune ring child (rotates at runtime)
    ring = bpy.data.objects.new("SM_Lexicon_Pedestal_Ring",
                                bpy.data.meshes.new("ring"))
    link(ring)
    rb = bmesh.new()
    bmesh.ops.create_cone(rb, cap_ends=False, segments=20,
                          radius1=0.46, radius2=0.46, depth=0.05)
    rb.to_mesh(ring.data); rb.free()
    ring.location = (0, 0, 1.45)
    rmat = bpy.data.materials.new("M_LexiconRing"); rmat.use_nodes = True
    nd = rmat.node_tree.nodes["Principled BSDF"]
    nd.inputs["Base Color"].default_value = (0.357, 0.247, 0.659, 1)
    nd.inputs["Emission Color"].default_value = (0.6, 0.45, 1.0, 1)
    nd.inputs["Emission Strength"].default_value = 1.2
    ring.data.materials.append(rmat)
    ring.parent = ob
    add_socket(ob, "SOCKET_BookRest",  (0, 0, 1.16))
    add_socket(ob, "SOCKET_WordOrbit", (0, 0, 1.45))
    set_origin(ob, (0, 0, 0))
    return ob

# ===========================================================================
#  CATEGORY C — RIGGED CHARACTERS
# ===========================================================================
def make_color_mat(name, rgb, emit=None, strength=0.0):
    m = bpy.data.materials.get(name) or bpy.data.materials.new(name)
    m.use_nodes = True
    nd = m.node_tree.nodes["Principled BSDF"]
    nd.inputs["Base Color"].default_value = (*rgb, 1)
    nd.inputs["Roughness"].default_value = 0.75
    if emit:
        nd.inputs["Emission Color"].default_value = (*emit, 1)
        nd.inputs["Emission Strength"].default_value = strength
    return m

def build_armature(name, bone_defs):
    """bone_defs: list of (name, head, tail, parent_name_or_None)"""
    arm_data = bpy.data.armatures.new(name + "_Arm")
    arm = bpy.data.objects.new(name + "_Armature", arm_data)
    link(arm)
    bpy.context.view_layer.objects.active = arm
    for o in bpy.context.selected_objects: o.select_set(False)
    arm.select_set(True)
    bpy.ops.object.mode_set(mode='EDIT')
    ebs = {}
    for bname, head, tail, parent in bone_defs:
        eb = arm_data.edit_bones.new(bname)
        eb.head = Vector(head); eb.tail = Vector(tail)
        if parent: eb.parent = ebs[parent]
        ebs[bname] = eb
    bpy.ops.object.mode_set(mode='OBJECT')
    return arm

def rigid_skin(mesh_obj, arm, group_ranges):
    """group_ranges: {bone_name: [(v_start, v_end), ...]} weight-1 skinning."""
    for bone, ranges in group_ranges.items():
        vg = mesh_obj.vertex_groups.new(name=bone)
        idx = []
        for a, b in ranges: idx.extend(range(a, b))
        vg.add(idx, 1.0, 'REPLACE')
    mod = mesh_obj.modifiers.new("Armature", 'ARMATURE')
    mod.object = arm
    mesh_obj.parent = arm

def key_pose(arm, action_name, fps_len, keys):
    """keys: list of (frame, bone, channel, value) where channel in
       'rx ry rz' (degrees) | 'lx ly lz' (metres) | 'sx sy sz' (scale)."""
    if arm.animation_data is None: arm.animation_data_create()
    act = bpy.data.actions.new(action_name)
    act.use_fake_user = True
    arm.animation_data.action = act
    # reset pose
    for pb in arm.pose.bones:
        pb.rotation_mode = 'XYZ'
        pb.rotation_euler = (0, 0, 0)
        pb.location = (0, 0, 0)
        pb.scale = (1, 1, 1)
    for frame, bone, chan, val in keys:
        pb = arm.pose.bones[bone]
        if   chan[0] == 'r':
            e = list(pb.rotation_euler)
            e['xyz'.index(chan[1])] = math.radians(val)
            pb.rotation_euler = e
            pb.keyframe_insert("rotation_euler", frame=frame)
        elif chan[0] == 'l':
            l = list(pb.location)
            l['xyz'.index(chan[1])] = val
            pb.location = l
            pb.keyframe_insert("location", frame=frame)
        elif chan[0] == 's':
            s = list(pb.scale)
            s['xyz'.index(chan[1])] = val
            pb.scale = s
            pb.keyframe_insert("scale", frame=frame)
    act.frame_range  # touch
    return act

# ---------------------------------------------------------------- SK_Owl ---
def build_owl():
    body_mat = make_color_mat("M_Owl_Body",  (0.43, 0.30, 0.20))
    face_mat = make_color_mat("M_Owl_Face",  (0.93, 0.87, 0.74))
    beak_mat = make_color_mat("M_Owl_Beak",  (0.95, 0.70, 0.30))
    eye_mat  = make_color_mat("M_Owl_Eye",   (0.12, 0.10, 0.09))

    ob = bpy.data.objects.new("SK_Owl", bpy.data.meshes.new("owl"))
    link(ob)
    bm = bmesh.new()
    bm.faces.layers.int.new("trim")     # keep layer expectations harmless
    vr, fr = {}, {}                     # vertex / face ranges per bone / mat

    def mark(bone, mat_idx):
        v0, f0 = len(bm.verts), len(bm.faces)
        def done():
            vr.setdefault(bone, []).append((v0, len(bm.verts)))
            fr.setdefault(mat_idx, []).append((f0, len(bm.faces)))
        return done

    d = mark("Body", 0);  sphere(bm, 0.16, 16, 12, (0, 0, 0.24),
                                 (1.0, 0.92, 1.18)); d()
    d = mark("Body", 1);  sphere(bm, 0.115, 12, 8, (0, -0.065, 0.20),
                                 (0.85, 0.55, 1.05)); d()          # belly
    d = mark("Head", 0);  sphere(bm, 0.125, 16, 12, (0, 0, 0.455),
                                 (1.0, 0.96, 0.94)); d()
    d = mark("Head", 1)                                            # face disc
    sphere(bm, 0.105, 12, 8, (0, -0.055, 0.455), (0.95, 0.45, 0.9)); d()
    for sx in (-1, 1):                                             # eyes
        d = mark("Head", 3)
        sphere(bm, 0.032, 8, 6, (sx*0.048, -0.115, 0.475)); d()
    d = mark("Head", 2)                                            # beak
    bk = bmesh.ops.create_cone(bm, cap_ends=True, segments=6,
                               radius1=0.028, radius2=0.004, depth=0.06)
    bmesh.ops.transform(bm, matrix=Matrix.Translation((0, -0.135, 0.435)) @
                        Matrix.Rotation(math.radians(-100), 4, 'X'),
                        verts=bk['verts']); d()
    for sx, bone in ((-1, "BrowL"), (1, "BrowR")):                 # brows
        d = mark(bone, 0)
        br = box(bm, (0.075, 0.03, 0.02), (sx*0.05, -0.105, 0.525), "leather")
        bmesh.ops.transform(bm, matrix=Matrix.Translation((sx*0.05,-0.105,0.525))
                            @ Matrix.Rotation(math.radians(sx*12), 4, 'Y')
                            @ Matrix.Translation((-sx*0.05,0.105,-0.525)),
                            verts=br); d()
    for sx, bone in ((-1, "WingL"), (1, "WingR")):                 # wings
        d = mark(bone, 0)
        sphere(bm, 0.115, 12, 8, (sx*0.155, 0.01, 0.26),
               (0.32, 0.75, 1.05)); d()
    d = mark("Tail", 0)                                            # tail fan
    for k, ang in enumerate((-18, 0, 18)):
        tl = box(bm, (0.05, 0.14, 0.018), (0, 0.16, 0.13), "leather")
        bmesh.ops.transform(bm, matrix=Matrix.Translation((0, 0.10, 0.14)) @
                            Matrix.Rotation(math.radians(ang), 4, 'Z') @
                            Matrix.Rotation(math.radians(30), 4, 'X') @
                            Matrix.Translation((0, 0.06, 0)), verts=tl)
    d()
    d = mark("Root", 2)                                            # feet
    for sx in (-1, 1):
        cylinder(bm, 0.022, 0.05, 8, (sx*0.06, -0.02, 0.045))
        box(bm, (0.05, 0.07, 0.02), (sx*0.06, -0.05, 0.012), "leather")
    d()
    commit(ob, bm)
    ob.data.polygons.foreach_set("use_smooth", [True]*len(ob.data.polygons))
    for m in (body_mat, face_mat, beak_mat, eye_mat):
        ob.data.materials.append(m)
    for mat_idx, ranges in fr.items():
        for a, b in ranges:
            for p in range(a, b):
                ob.data.polygons[p].material_index = mat_idx

    arm = build_armature("SK_Owl", [
        ("Root",  (0,0,0.00), (0,0,0.10), None),
        ("Body",  (0,0,0.10), (0,0,0.36), "Root"),
        ("Head",  (0,0,0.36), (0,0,0.56), "Body"),
        ("BrowL", (-0.05,-0.10,0.52), (-0.09,-0.10,0.55), "Head"),
        ("BrowR", ( 0.05,-0.10,0.52), ( 0.09,-0.10,0.55), "Head"),
        ("WingL", (-0.13,0.01,0.32), (-0.24,0.02,0.20), "Body"),
        ("WingR", ( 0.13,0.01,0.32), ( 0.24,0.02,0.20), "Body"),
        ("Tail",  (0,0.10,0.16), (0,0.24,0.10), "Body"),
    ])
    rigid_skin(ob, arm, vr)

    # ---- six clips @24fps -------------------------------------------------
    bpy.context.scene.render.fps = 24
    key_pose(arm, "Owl_Idle_Sleep", 48, [
        (1,"Head","rx",22),(24,"Head","rx",25),(48,"Head","rx",22),
        (1,"Body","sy",1.0),(24,"Body","sy",1.035),(48,"Body","sy",1.0),
        (1,"WingL","ry",-14),(48,"WingL","ry",-14),
        (1,"WingR","ry",14),(48,"WingR","ry",14)])
    key_pose(arm, "Owl_Idle_Perch", 48, [
        (1,"Body","rz",-2),(24,"Body","rz",2),(48,"Body","rz",-2),
        (1,"Head","rz",-7),(16,"Head","rz",6),(34,"Head","rz",-4),
        (48,"Head","rz",-7),
        (10,"BrowL","rx",0),(14,"BrowL","rx",-14),(20,"BrowL","rx",0)])
    key_pose(arm, "Owl_React_Happy", 24, [
        (1,"Root","lz",0),(6,"Root","lz",0.055),(11,"Root","lz",0),
        (14,"Root","lz",0.04),(18,"Root","lz",0),
        (1,"WingL","ry",0),(5,"WingL","ry",-55),(10,"WingL","ry",0),
        (14,"WingL","ry",-45),(19,"WingL","ry",0),
        (1,"WingR","ry",0),(5,"WingR","ry",55),(10,"WingR","ry",0),
        (14,"WingR","ry",45),(19,"WingR","ry",0),
        (1,"BrowL","rx",0),(5,"BrowL","rx",-18),(19,"BrowL","rx",0),
        (1,"BrowR","rx",0),(5,"BrowR","rx",-18),(19,"BrowR","rx",0)])
    key_pose(arm, "Owl_React_Lean", 24, [
        (1,"Body","rx",0),(8,"Body","rx",-13),(18,"Body","rx",-13),
        (24,"Body","rx",0),
        (1,"Head","rx",0),(8,"Head","rx",-7),(18,"Head","rx",-7),
        (24,"Head","rx",0)])
    key_pose(arm, "Owl_Hint_Point", 36, [
        (1,"WingL","ry",0),(8,"WingL","ry",-95),(26,"WingL","ry",-95),
        (36,"WingL","ry",0),
        (1,"WingL","rx",0),(8,"WingL","rx",-25),(26,"WingL","rx",-25),
        (36,"WingL","rx",0),
        (1,"BrowL","rx",0),(8,"BrowL","rx",-16),(30,"BrowL","rx",0),
        (1,"BrowR","rx",0),(8,"BrowR","rx",-16),(30,"BrowR","rx",0),
        (1,"Head","rz",0),(8,"Head","rz",-10),(30,"Head","rz",0)])
    key_pose(arm, "Owl_Fly_Short", 32, [
        (1,"WingL","ry",-60),(5,"WingL","ry",35),(9,"WingL","ry",-60),
        (13,"WingL","ry",35),(17,"WingL","ry",-60),(21,"WingL","ry",35),
        (25,"WingL","ry",-60),(29,"WingL","ry",35),(32,"WingL","ry",-60),
        (1,"WingR","ry",60),(5,"WingR","ry",-35),(9,"WingR","ry",60),
        (13,"WingR","ry",-35),(17,"WingR","ry",60),(21,"WingR","ry",-35),
        (25,"WingR","ry",60),(29,"WingR","ry",-35),(32,"WingR","ry",60),
        (1,"Root","lz",0),(9,"Root","lz",0.05),(17,"Root","lz",0),
        (25,"Root","lz",0.05),(32,"Root","lz",0)])
    return arm, ob

# ---------------------------------------------------------- SK_BookSpirit --
def build_book_spirit():
    cov_mat  = make_color_mat("M_Spirit_Cover", (0.373, 0.765, 0.698))
    page_mat = make_color_mat("M_Spirit_Pages", (0.96, 0.93, 0.85))
    glow_mat = make_color_mat("M_Spirit_Glow",  (0.5, 1.0, 0.9),
                              emit=(0.4, 1.0, 0.85), strength=2.0)
    ob = bpy.data.objects.new("SK_BookSpirit", bpy.data.meshes.new("spirit"))
    link(ob)
    bm = bmesh.new()
    bm.faces.layers.int.new("trim")
    vr, fr = {}, {}
    def mark(bone, mat_idx):
        v0, f0 = len(bm.verts), len(bm.faces)
        def done():
            vr.setdefault(bone, []).append((v0, len(bm.verts)))
            fr.setdefault(mat_idx, []).append((f0, len(bm.faces)))
        return done

    d = mark("SpiritBody", 1)                              # page block core
    box(bm, (0.06, 0.24, 0.30), (0, 0, 0.35), "parchment"); d()
    for sx, bone in ((-1, "CoverL"), (1, "CoverR")):       # cover wings
        d = mark(bone, 0)
        c = box(bm, (0.02, 0.26, 0.32), (sx*0.13, 0, 0.35), "leather")
        bmesh.ops.transform(bm, matrix=Matrix.Translation((0,0,0.35)) @
                            Matrix.Rotation(math.radians(-sx*30), 4, 'Z') @
                            Matrix.Translation((0,0,-0.35)), verts=c)
        d()
    d = mark("SpiritBody", 2)                              # glow heart
    sphere(bm, 0.045, 10, 8, (0, -0.02, 0.35)); d()
    d = mark("SpiritBody", 0)                              # ribbon tail
    for k in range(3):
        t = box(bm, (0.03, 0.03, 0.10), (0, 0.02*k, 0.16 - k*0.075), "leather")
        bmesh.ops.transform(bm, matrix=Matrix.Rotation(
            math.radians(8*(k+1)), 4, 'X'), verts=t)
    d()
    commit(ob, bm)
    ob.data.polygons.foreach_set("use_smooth", [True]*len(ob.data.polygons))
    for m in (cov_mat, page_mat, glow_mat):
        ob.data.materials.append(m)
    for mat_idx, ranges in fr.items():
        for a, b in ranges:
            for p in range(a, b):
                ob.data.polygons[p].material_index = mat_idx

    arm = build_armature("SK_BookSpirit", [
        ("SpiritRoot",   (0,0,0),    (0,0,0.10), None),
        ("SpiritBody",   (0,0,0.10), (0,0,0.50), "SpiritRoot"),
        ("CoverL", (-0.03,0,0.35), (-0.20,0,0.35), "SpiritBody"),
        ("CoverR", ( 0.03,0,0.35), ( 0.20,0,0.35), "SpiritBody"),
    ])
    rigid_skin(ob, arm, vr)

    bpy.context.scene.render.fps = 24
    key_pose(arm, "Spirit_Idle_Float", 48, [
        (1,"SpiritRoot","lz",0),(24,"SpiritRoot","lz",0.05),(48,"SpiritRoot","lz",0),
        (1,"CoverL","rz",0),(12,"CoverL","rz",14),(24,"CoverL","rz",0),
        (36,"CoverL","rz",14),(48,"CoverL","rz",0),
        (1,"CoverR","rz",0),(12,"CoverR","rz",-14),(24,"CoverR","rz",0),
        (36,"CoverR","rz",-14),(48,"CoverR","rz",0),
        (1,"SpiritBody","rz",-3),(24,"SpiritBody","rz",3),(48,"SpiritBody","rz",-3)])
    key_pose(arm, "Spirit_Flutter_Happy", 24, [
        (1,"CoverL","rz",5),(4,"CoverL","rz",45),(7,"CoverL","rz",5),
        (10,"CoverL","rz",45),(13,"CoverL","rz",5),(16,"CoverL","rz",45),
        (19,"CoverL","rz",5),
        (1,"CoverR","rz",-5),(4,"CoverR","rz",-45),(7,"CoverR","rz",-5),
        (10,"CoverR","rz",-45),(13,"CoverR","rz",-5),(16,"CoverR","rz",-45),
        (19,"CoverR","rz",-5),
        (1,"SpiritRoot","lz",0),(10,"SpiritRoot","lz",0.09),(19,"SpiritRoot","lz",0)])
    key_pose(arm, "Spirit_Talk", 36, [
        (1,"CoverL","rz",8),(6,"CoverL","rz",26),(12,"CoverL","rz",10),
        (18,"CoverL","rz",30),(24,"CoverL","rz",8),(30,"CoverL","rz",22),
        (36,"CoverL","rz",8),
        (1,"CoverR","rz",-8),(6,"CoverR","rz",-26),(12,"CoverR","rz",-10),
        (18,"CoverR","rz",-30),(24,"CoverR","rz",-8),(30,"CoverR","rz",-22),
        (36,"CoverR","rz",-8)])
    return arm, ob

# ===========================================================================
#  CHARACTER EXPORT (mesh + armature + all action takes)
# ===========================================================================
def export_character(arm, mesh, out_dir):
    os.makedirs(out_dir, exist_ok=True)
    for o in bpy.context.selected_objects: o.select_set(False)
    arm.select_set(True); mesh.select_set(True)
    bpy.context.view_layer.objects.active = arm
    tri = mesh.modifiers.new("ExpTri", 'TRIANGULATE')
    path = os.path.join(out_dir, mesh.name + ".fbx")
    bpy.ops.export_scene.fbx(
        filepath=path, use_selection=True,
        axis_forward='-Z', axis_up='Y',
        apply_scale_options='FBX_SCALE_UNITS',
        use_mesh_modifiers=True, mesh_smooth_type='OFF',
        use_tspace=False, add_leaf_bones=False,
        object_types={'MESH', 'ARMATURE'},
        bake_anim=True, bake_anim_use_all_bones=True,
        bake_anim_use_nla_strips=False, bake_anim_use_all_actions=True,
        bake_anim_force_startend_keying=True, bake_anim_step=1.0)
    mesh.modifiers.remove(tri)
    return path

# ===========================================================================
#  MAIN
# ===========================================================================
def main():
    print("=" * 70)
    print(" HERO & CHARACTER GENERATOR")
    print("=" * 70)
    os.makedirs(OUT_HERO, exist_ok=True)
    os.makedirs(OUT_CHAR, exist_ok=True)
    reset_scene()
    MAT, GLASS = build_materials()
    manifest, col_x = [], 0.0

    heroes = [build_book_hero, build_librarian_desk,
              lambda M: build_fountain(M, False),
              lambda M: build_fountain(M, True),
              build_rune_door, build_lexicon_pedestal]
    for b in heroes:
        try:
            ob = b(MAT)
            tris = tri_count(ob) + sum(tri_count(c) for c in
                                       ob.children_recursive if c.type=='MESH')
            lo, hi = HERO_BUDGET
            status = "OK" if tris <= hi else "OVER"
            export_fbx(ob, OUT_HERO)
            manifest.append(dict(name=ob.name, tris=tris, kind="hero",
                                 status=status,
                                 children=[c.name for c in ob.children_recursive]))
            print(f"  {ob.name:28s} {tris:6d} tris [{status}]")
            ob.location.x = col_x; col_x += max(ob.dimensions.x, 1.2) + 1.0
        except Exception:
            print("  FAILED:", b); traceback.print_exc()

    for builder in (build_owl, build_book_spirit):
        try:
            arm, mesh = builder()
            tris = tri_count(mesh)
            lo, hi = CHAR_BUDGET
            status = "OK" if tris <= hi else "OVER"
            export_character(arm, mesh, OUT_CHAR)
            pref = "Owl" if mesh.name == "SK_Owl" else "Spirit"
            clips = [a.name for a in bpy.data.actions
                     if a.name.startswith(pref)]
            manifest.append(dict(name=mesh.name, tris=tris, kind="character",
                                 status=status, clips=sorted(set(clips))))
            print(f"  {mesh.name:28s} {tris:6d} tris [{status}] "
                  f"({len(set(clips))} clips)")
            arm.location.x = col_x; col_x += 1.6
        except Exception:
            print("  FAILED:", builder); traceback.print_exc()

    with open("/home/claude/herogen/out/hero_manifest.json", "w") as f:
        json.dump(manifest, f, indent=2)
    bpy.ops.wm.save_as_mainfile(
        filepath="/home/claude/herogen/out/hero_source.blend")
    print("-" * 70)
    print(f"  {len(manifest)} assets exported")

if __name__ == "__main__":
    main()
