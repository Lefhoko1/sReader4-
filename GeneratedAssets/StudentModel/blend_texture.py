"""Step 2 of the student's texture fix (system Python): head mask from head_uv_polys.json, then blend.

Projected texture for the body (no seam cracks), AI-painted texture for the head (clean eyes).
"""
import json, os
import numpy as np
from PIL import Image, ImageDraw, ImageFilter

GEN = r"C:\Users\11113930\Documents\mine\modelCreator\my-app\outputs\student-boy-20261020-110631"
OUT = os.path.dirname(os.path.abspath(__file__))
S = 4096

mask = Image.new("L", (S, S), 0)
draw = ImageDraw.Draw(mask)
for poly in json.load(open(os.path.join(OUT, "head_uv_polys.json"))):
    draw.polygon([(u * S, (1 - v) * S) for u, v in poly], fill=255)
mask = mask.filter(ImageFilter.MaxFilter(9)).filter(ImageFilter.GaussianBlur(2))
mask.save(os.path.join(OUT, "head_mask.png"))

b = np.array(Image.open(os.path.join(GEN, "intermediate", "base_color_padded.png")).convert("RGB")).astype(np.float32)
p = np.array(Image.open(os.path.join(GEN, "intermediate", "base_color_projected_padded.png")).convert("RGB")).astype(np.float32)
m = np.array(mask).astype(np.float32)[..., None] / 255
Image.fromarray((b * m + p * (1 - m)).clip(0, 255).astype(np.uint8)).save(os.path.join(OUT, "base_color_fixed.png"))
print("head mask coverage", float((m > 0.5).mean()))
