"""Step 1 of the student's texture fix (headless Blender): export the head's UV polygons.

usage: blender -b -P make_texture.py      then: python blend_texture.py

The generator gives two base colours for the same UV layout:
  - base_color_padded: AI-painted, clean face, but seam cracks on the body;
  - base_color_projected_padded: photos painted on top, no body cracks, but
    the face is doubled (the drawings don't line up with the 3D face).
blend_texture.py takes the projected texture for the body and the AI paint for
the head (everything above the chin), using these polygons as the mask.
"""
import json, os
import bpy

GEN = r"C:\Users\11113930\Documents\mine\modelCreator\my-app\outputs\student-boy-20261020-110631"
OUT = r"C:\Users\11113930\sReader4\GeneratedAssets\StudentModel"
NECK_Z = 1.17          # just below the chin (rig landmark neck_z = 1.188)

bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=os.path.join(GEN, "intermediate", "character_skinned.glb"))
mesh = next(o for o in bpy.context.scene.objects if o.type == "MESH")
uv = mesh.data.uv_layers.active.data
M = mesh.matrix_world
polys = [[[uv[li].uv[0], uv[li].uv[1]] for li in p.loop_indices]
         for p in mesh.data.polygons if (M @ p.center).z >= NECK_Z]
json.dump(polys, open(os.path.join(OUT, "head_uv_polys.json"), "w"))
print("head polygons", len(polys))
