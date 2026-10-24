"""Cut a 4x2 character sheet drawn on a flat magenta (#FF00FF) background.

usage: python key_magenta.py <sheet.png> <out_dir> <name>

- Alpha from "how magenta" each pixel is (soft edge, no hard jaggies).
- Despill: removes the pink tint magenta leaves on edge pixels; the
  character has no magenta in it, so this never changes his real colours.
- Each of the 8 cells becomes one transparent PNG, cropped to the character.
- Also writes an animated preview, a strip with all frames feet-aligned, and
  an edge check on a dark background.
"""
import os, sys
import numpy as np
from PIL import Image

src, out, name = sys.argv[1], sys.argv[2], sys.argv[3]
os.makedirs(out, exist_ok=True)
img = np.array(Image.open(src).convert("RGB")).astype(float)
H, W, _ = img.shape
r, g, b = img[..., 0], img[..., 1], img[..., 2]

# magenta-ness: red and blue both high above green
m = (np.minimum(r, b) - g) / 255.0
alpha = 1.0 - np.clip((m - 0.18) / (0.55 - 0.18), 0, 1)     # <0.18 solid, >0.55 gone

# despill: take the magenta excess back out of red and blue
spill = np.clip(np.minimum(r, b) - g, 0, None)
keep = alpha[..., None]
rgb = img.copy()
rgb[..., 0] -= spill * (1 - alpha) * 0.9
rgb[..., 2] -= spill * (1 - alpha) * 0.9
rgb[..., 0] = np.where(alpha < 1, np.minimum(rgb[..., 0], g + np.clip(r - b, 0, None) + 40), rgb[..., 0])
rgb = np.clip(rgb, 0, 255)

rgba = np.dstack([rgb, alpha * 255]).astype(np.uint8)
cw, ch = W // 4, H // 2
frames = []
for row in range(2):
    for col in range(4):
        cell = rgba[row * ch:(row + 1) * ch, col * cw:(col + 1) * cw]
        a = cell[..., 3]
        # drop specks: only keep rows/cols with a meaningful amount of character
        ys, xs = np.nonzero(a > 40)
        y0, y1, x0, x1 = ys.min(), ys.max() + 1, xs.min(), xs.max() + 1
        frames.append(Image.fromarray(cell[y0:y1, x0:x1]))
for i, f in enumerate(frames):
    f.save(f"{out}/{name}_{i:02d}.png")

Wc = max(f.width for f in frames) + 30; Hc = max(f.height for f in frames) + 20
aligned = []
for f in frames:
    can = Image.new("RGBA", (Wc, Hc), (0, 0, 0, 0))
    can.paste(f, ((Wc - f.width) // 2, Hc - 10 - f.height), f)
    aligned.append(can)
strip = Image.new("RGBA", (Wc * len(aligned), Hc), (238, 214, 182, 255))
for i, p in enumerate(aligned): strip.alpha_composite(p, (i * Wc, 0))
strip.convert("RGB").resize((strip.width // 2, strip.height // 2), Image.LANCZOS).save(f"{out}/{name}_strip.png")
dark = Image.new("RGBA", (Wc * 2, Hc), (40, 44, 60, 255))
dark.alpha_composite(aligned[0], (0, 0)); dark.alpha_composite(aligned[5], (Wc, 0))
dark.convert("RGB").save(f"{out}/{name}_edgecheck.png")
gif = []
for p in aligned:
    bgc = Image.new("RGBA", p.size, (238, 214, 182, 255)); bgc.alpha_composite(p); gif.append(bgc.convert("RGB"))
gif[0].save(f"{out}/{name}.gif", save_all=True, append_images=gif[1:], duration=125, loop=0)
print(name, "frames", [(f.width, f.height) for f in frames])
