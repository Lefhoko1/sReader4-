"""Shared headless-Blender machinery for turning a modelCreator character into game sprites.

A character script (e.g. GeneratedAssets/EncouragerModel/render_encourager.py) supplies a
config dict and its own poses; this module does the rest:

  cfg = {
      "gen":   generator output folder (master.glb + intermediate/base_color_padded.png),
      "height": character height in metres (feet at z = 0, facing +Y, his right = +X),
      "bones": {name: (head, tail)} - "body" is the root, all others parent to it,
      "weights": fn(co) -> {bone: weight}  (rest position, soft region weights),
      "face": (ortho_scale, centre_z, neck_z) for the straight-on face close-up,
  }

Lessons carried over from the student (see the student-3d-sprite-pipeline memory):
  - use master.glb + the clean AI-painted base colour: the rigged copy's UVs crack and the
    photo-projected texture doubles the eyes;
  - the generator's own rig assumes adult proportions, so the skeleton is fitted here;
  - faces have doubled surfaces that z-fight, so expression textures are painted by
    rasterising every front-facing triangle (facetris), not by mapping visible pixels.
"""
import math, os, sys, json
import bpy, bmesh
from mathutils import Vector, Euler

RES_W, RES_H = 640, 960          # rendered at 2x; downscaled to 320 x 480 for Unity
ELEV = math.radians(8)
FRONT, BACK, RIGHT = 180.0, 0.0, 90.0   # camera yaw: 180 sees his face


def deg(x): return math.radians(x)


def ss(a, b, x):
    t = min(1.0, max(0.0, (x - a) / (b - a))); return t * t * (3 - 2 * t)


class Aim:
    """Pose value: point the bone along this direction (armature space), no twist."""
    def __init__(self, direction): self.direction = tuple(direction)


def lerp_table(table, z):
    """Piecewise-linear lookup in [(z, value), ...] sorted by z."""
    if z <= table[0][0]: return table[0][1]
    for (z0, v0), (z1, v1) in zip(table, table[1:]):
        if z <= z1: return v0 + (v1 - v0) * (z - z0) / (z1 - z0)
    return table[-1][1]


class Rig:
    def __init__(self, cfg):
        self.cfg = cfg
        self.view_h = cfg["height"] * 960 / 896      # character fills 448/480 of the canvas height
        self.arm, self.mesh, self.root, self.cam = self._setup()
        node = next(n for n in self.mesh.material_slots[0].material.node_tree.nodes if n.type == "BSDF_PRINCIPLED")
        self.face_node = node.inputs["Base Color"].links[0].from_node
        self.faces = {None: self.face_node.image}
        self.face_dir = cfg.get("face_dir")

    # ── scene ────────────────────────────────────────────────────────────────
    def _setup(self):
        cfg = self.cfg
        bpy.ops.wm.read_factory_settings(use_empty=True)
        bpy.ops.import_scene.gltf(filepath=os.path.join(cfg["gen"], "master.glb"))
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
        for name, (h, t) in cfg["bones"].items():
            b = adata.edit_bones.new(name); b.head, b.tail = h, t; b.roll = 0
        for name in cfg["bones"]:
            if name != "body": adata.edit_bones[name].parent = adata.edit_bones[cfg.get("parents", {}).get(name, "body")]
        bpy.ops.object.mode_set(mode="OBJECT")

        if cfg.get("cut_bridges_below"):
            # faces joining a hanging hand to the body (generators fuse touching surfaces): cut, so the arm swings free
            bm = bmesh.new(); bm.from_mesh(mesh.data)
            def armw(co): w = cfg["weights"](co); return sum(v for k, v in w.items() if k.startswith("arm"))
            bridge = [f for f in bm.faces if f.calc_center_median().z < cfg["cut_bridges_below"]
                      and (lambda ws: max(ws) - min(ws) > 0.15)([armw(v.co) for v in f.verts])]
            bmesh.ops.delete(bm, geom=bridge, context="FACES")
            bm.to_mesh(mesh.data); bm.free()
            print("cut bridge faces:", len(bridge))

        groups = {n: mesh.vertex_groups.new(name=n) for n in cfg["bones"]}
        for v in mesh.data.vertices:
            for n, wt in cfg["weights"](v.co).items():
                groups[n].add([v.index], wt, "REPLACE")
        mod = mesh.modifiers.new("Armature", "ARMATURE"); mod.object = arm
        mesh.parent = arm

        img = bpy.data.images.load(os.path.join(cfg["gen"], "intermediate", "base_color_padded.png"))
        for slot in mesh.material_slots:
            nt = slot.material.node_tree
            bsdf = next(n for n in nt.nodes if n.type == "BSDF_PRINCIPLED")
            for link in list(bsdf.inputs["Base Color"].links):
                if link.from_node.type == "TEX_IMAGE": link.from_node.image = img
            for link in list(bsdf.inputs["Metallic"].links): nt.links.remove(link)
            bsdf.inputs["Metallic"].default_value = 0.0
            for link in list(bsdf.inputs["Roughness"].links): nt.links.remove(link)
            bsdf.inputs["Roughness"].default_value = 0.55

        root = bpy.data.objects.new("Root", None)
        bpy.context.scene.collection.objects.link(root)
        arm.parent = root

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
        cam = bpy.data.cameras.new("Cam"); cam.type = "ORTHO"; cam.ortho_scale = self.view_h
        co = bpy.data.objects.new("Cam", cam); sc.collection.objects.link(co); sc.camera = co
        return arm, mesh, root, co

    def place_camera(self, yaw_deg):
        """Camera on a circle round the character, slightly from above; feet 16 px above the bottom edge."""
        yaw = math.radians(yaw_deg)
        centre_z = -self.view_h * 16 / 960 + self.view_h / 2
        d = 6.0
        pos = Vector((math.sin(yaw) * d * math.cos(ELEV), -math.cos(yaw) * d * math.cos(ELEV), centre_z + d * math.sin(ELEV)))
        self.cam.location = pos
        self.cam.rotation_euler = (Vector((0, 0, centre_z)) - pos).to_track_quat("-Z", "Y").to_euler()

    def pose(self, **rot):
        """Per bone: an XYZ euler tuple (radians), or Aim(direction) to point the bone that way (armature space)."""
        for b in self.arm.pose.bones:
            v = rot.get(b.name, (0, 0, 0))
            if isinstance(v, Aim):
                rest = b.bone.matrix_local.to_3x3()                        # bone rest frame in armature space
                swing = (rest @ Vector((0, 1, 0))).rotation_difference(Vector(v.direction).normalized())
                b.rotation_mode = "QUATERNION"
                b.rotation_quaternion = (rest.inverted() @ swing.to_matrix() @ rest).to_quaternion()
            else:
                b.rotation_mode = "XYZ"
                b.rotation_euler = v

    def hand_dir(self, bone, target):
        """Direction from a bone's head to a target point (armature space), for Aim."""
        return Vector(target) - self.arm.data.bones[bone].head_local

    def render(self, path):
        bpy.context.scene.render.filepath = path
        bpy.ops.render.render(write_still=True)

    def set_face(self, expr):
        """Swap the base colour for an expression texture painted into face_dir (None = the normal face)."""
        if expr not in self.faces:
            self.faces[expr] = bpy.data.images.load(os.path.join(self.face_dir, f"tex_{expr}.png"))
        self.face_node.image = self.faces[expr]

    # ── tools ────────────────────────────────────────────────────────────────
    def weights_view(self, out):
        """Render the bone regions as flat colours from four sides."""
        os.makedirs(out, exist_ok=True)
        pal = [(0.8, 0.8, 0.8), (1, 0.85, 0.2), (1, 0.1, 0.1), (0.1, 0.4, 1), (0.1, 0.8, 0.2), (0.8, 0.2, 0.9), (0.2, 0.9, 0.9), (0.9, 0.5, 0.1)]
        names = {g.index: g.name for g in self.mesh.vertex_groups}
        colour = {n: pal[i % len(pal)] for i, n in enumerate(self.cfg["bones"])}
        attr = self.mesh.data.color_attributes.new("W", "FLOAT_COLOR", "POINT")
        for v in self.mesh.data.vertices:
            c = colour[names[max(v.groups, key=lambda g: g.weight).group]] if v.groups else (0, 0, 0)
            attr.data[v.index].color = (*c, 1)
        mat = bpy.data.materials.new("W"); mat.use_nodes = True
        nt = mat.node_tree; nt.nodes.clear()
        a = nt.nodes.new("ShaderNodeVertexColor"); a.layer_name = "W"
        e = nt.nodes.new("ShaderNodeEmission"); o = nt.nodes.new("ShaderNodeOutputMaterial")
        nt.links.new(a.outputs[0], e.inputs[0]); nt.links.new(e.outputs[0], o.inputs[0])
        for s in self.mesh.material_slots: s.material = mat
        self.pose()
        for yaw in (0, 90, 180, 270):
            self.place_camera(yaw); self.render(os.path.join(out, f"w_{yaw:03d}.png"))

    def _face_camera(self):
        sc = bpy.context.scene
        scale, z, _ = self.cfg["face"]
        sc.render.resolution_x = sc.render.resolution_y = 2048
        self.cam.data.ortho_scale = scale
        self.cam.location = (0, 6, z); self.cam.rotation_euler = Euler((math.radians(90), 0, math.radians(180)))
        bpy.context.view_layer.update()

    def facemaps(self, out, expr=None):
        """Unlit straight-on face close-up (texture colour) and the 16-bit UV under every pixel."""
        os.makedirs(out, exist_ok=True)
        self.pose(); self._face_camera()
        sc = bpy.context.scene
        try: sc.eevee.taa_render_samples = 1
        except AttributeError: pass
        sc.render.dither_intensity = 0.0
        tex = self.face_node.image if expr is None else bpy.data.images.load(os.path.join(self.face_dir, f"tex_{expr}.png"))
        def flat(kind):
            mat = bpy.data.materials.new(kind); mat.use_nodes = True
            nt = mat.node_tree; nt.nodes.clear()
            e = nt.nodes.new("ShaderNodeEmission"); o = nt.nodes.new("ShaderNodeOutputMaterial")
            nt.links.new(e.outputs[0], o.inputs[0])
            if kind == "uv":
                uv = nt.nodes.new("ShaderNodeUVMap"); nt.links.new(uv.outputs[0], e.inputs[0])
            else:
                t = nt.nodes.new("ShaderNodeTexImage"); t.image = tex; nt.links.new(t.outputs[0], e.inputs[0])
            for s in self.mesh.material_slots: s.material = mat
        flat("color"); sc.view_settings.view_transform = "Standard"
        self.render(os.path.join(out, "face_color.png"))
        if expr is not None: return
        flat("uv"); sc.view_settings.view_transform = "Raw"; sc.render.image_settings.color_depth = "16"
        self.render(os.path.join(out, "face_uv.png"))
        json.dump({"texture": bpy.path.abspath(tex.filepath)}, open(os.path.join(out, "face_maps.json"), "w"))

    def facetris(self, out):
        """Every front-facing head triangle: UVs and where it lands in the face close-up (for painting all layers)."""
        import numpy as np
        from bpy_extras.object_utils import world_to_camera_view
        self.pose(); self._face_camera()
        sc = bpy.context.scene
        me = self.mesh.data; me.calc_loop_triangles()
        uvl = me.uv_layers.active.data
        M = self.mesh.matrix_world
        scr_v = np.array([tuple(world_to_camera_view(sc, self.cam, M @ v.co))[:2] for v in me.vertices])
        neck = self.cfg["face"][2]
        uvs, scrs = [], []
        for t in me.loop_triangles:
            if (M.to_3x3() @ t.normal).y < 0.05 or (M @ t.center).z < neck: continue
            s = scr_v[list(t.vertices)]
            if (s < 0).all() or (s > 1).all(): continue
            uvs.append([tuple(uvl[li].uv) for li in t.loops]); scrs.append(s)
        np.save(os.path.join(out, "tri_uv.npy"), np.array(uvs, np.float64))
        np.save(os.path.join(out, "tri_scr.npy"), np.array(scrs, np.float64))
        print("face triangles", len(uvs))

    def render_sets(self, out, sets, only=None):
        """sets: [(name, camera yaw, fn(t, frame_index) -> face or None, frames, loop)]."""
        for name, yaw, fn, n, loop in sets:
            if only and name not in only: continue
            d = os.path.join(out, name); os.makedirs(d, exist_ok=True)
            self.place_camera(yaw)
            for i in range(n):
                self.root.location = (0, 0, 0)
                face = fn(i / n if loop else i / max(1, n - 1), i)
                self.set_face(face)
                self.render(os.path.join(d, f"{name}_{i:02d}.png"))
            self.set_face(None)
            print("rendered", name, n)
