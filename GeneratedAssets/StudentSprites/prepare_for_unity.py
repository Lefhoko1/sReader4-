"""Stage B prep: turn the keyed frames into game-ready sprites.

Every frame goes onto ONE shared canvas size, so every sprite in every
animation has the same pivot (bottom centre = between the feet) and the same
pixels-per-unit:
  - height: each set is scaled uniformly so its tallest (upright) frame is
    CHAR_H px. Standing sets (idles, turn) are additionally matched frame by
    frame, so the student never shrinks/grows between poses. Walks and the
    lean keep their natural bob.
  - horizontal: centred on the head+torso (top 55% of the silhouette), not the
    bounding box, so swinging legs and arms don't make him slide sideways.
  - feet on one baseline BASELINE px above the bottom edge.
Never stretched: one scale factor per frame, same for x and y.
"""
import os
import numpy as np
from PIL import Image

ROOT = os.path.dirname(os.path.abspath(__file__))
SRC = os.path.join(ROOT, "frames")
DST = os.path.join(ROOT, "..", "..", "Assets", "Art", "ReadingHouse", "Student2D", "Frames")
CANVAS_W, CANVAS_H, CHAR_H, BASELINE = 320, 480, 448, 8

SETS = {   # name -> match each frame to the set height (standing poses)
    "idle_front": True, "idle_back": True, "turn_to_back": True,
    "walk_right": True, "walk_away": True, "walk_toward": True, "point_back": False,
}

def upper_body_cx(a):
    ys = np.nonzero(a.max(1) > 40)[0]
    top, bot = ys.min(), ys.max()
    band = a[top: top + int(0.55 * (bot - top))].astype(float)
    cols = band.sum(0)
    return (cols * np.arange(a.shape[1])).sum() / max(cols.sum(), 1)

os.makedirs(DST, exist_ok=True)
for name, match_each in SETS.items():
    files = sorted(f for f in os.listdir(os.path.join(SRC, name)) if f.startswith(name + "_") and f[len(name) + 1:-4].isdigit())
    frames = [Image.open(os.path.join(SRC, name, f)).convert("RGBA") for f in files]
    set_scale = CHAR_H / max(f.height for f in frames)
    out = os.path.join(DST, name)
    os.makedirs(out, exist_ok=True)
    for i, f in enumerate(frames):
        s = (CHAR_H / f.height) if match_each else set_scale
        im = f.resize((max(1, round(f.width * s)), max(1, round(f.height * s))), Image.LANCZOS)
        a = np.array(im)[..., 3]
        cx = upper_body_cx(a)
        x = int(round(CANVAS_W / 2 - cx))
        y = CANVAS_H - BASELINE - im.height
        if x < 0 or x + im.width > CANVAS_W or y < 0:
            raise SystemExit(f"{name} frame {i} does not fit the canvas ({im.width}x{im.height} at x={x})")
        can = Image.new("RGBA", (CANVAS_W, CANVAS_H), (0, 0, 0, 0))
        can.alpha_composite(im, (x, y))
        can.save(os.path.join(out, f"{name}_{i:02d}.png"), optimize=True)
    print(f"{name:13s} {len(frames)} frames  scale {set_scale:.3f}{' (per-frame matched)' if match_each else ''}")
print("canvas", CANVAS_W, "x", CANVAS_H, "char", CHAR_H, "baseline", BASELINE)
