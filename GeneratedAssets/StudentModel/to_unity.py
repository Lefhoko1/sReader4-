"""Blender renders (render_sprites.py, 640 x 960) -> Unity sprite frames (320 x 480).

The renders already share one orthographic scale and one floor line (feet 8 px
above the bottom after downscaling), so this only halves them; it never
re-fits or re-centres a frame, which would make him jitter. Resizing is done on
premultiplied colour so the transparent edge gets no dark fringe.

usage: python to_unity.py [render_dir] [unity_frames_dir]
"""
import os, sys
from PIL import Image

HERE = os.path.dirname(os.path.abspath(__file__))
SRC = sys.argv[1] if len(sys.argv) > 1 else os.path.join(HERE, "render")
DST = sys.argv[2] if len(sys.argv) > 2 else os.path.join(HERE, "..", "..", "Assets", "Art", "ReadingHouse", "Student2D", "Frames")
W, H = 320, 480

for name in sorted(os.listdir(SRC)):
    src = os.path.join(SRC, name)
    if not os.path.isdir(src): continue
    files = sorted(f for f in os.listdir(src) if f.endswith(".png"))
    out = os.path.join(DST, name)
    os.makedirs(out, exist_ok=True)
    # drop stale frames past the new count (with their .meta) so the setup tool counts correctly
    for f in os.listdir(out):
        stem = f.split(".")[0]
        if stem.startswith(name + "_") and stem[len(name) + 1:].isdigit() and int(stem[len(name) + 1:]) >= len(files):
            os.remove(os.path.join(out, f))
    for i, f in enumerate(files):
        im = Image.open(os.path.join(src, f)).convert("RGBA").convert("RGBa")
        im = im.resize((W, H), Image.LANCZOS).convert("RGBA")
        im.save(os.path.join(out, f"{name}_{i:02d}.png"), optimize=True)
    print(f"{name:13s} {len(files)} frames")

with open(os.path.join(DST, "RENDERED_3D.txt"), "w") as fh:
    fh.write("Frames rendered from the 3D student by GeneratedAssets/StudentModel/render_sprites.py.\n")
