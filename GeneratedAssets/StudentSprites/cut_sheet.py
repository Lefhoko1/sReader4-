"""Cut an 8-frame (4x2) character sheet with a non-flat background using GrabCut.
usage: python cut_sheet.py <sheet.png> <out_dir> <name>
"""
import os, sys
import cv2
import numpy as np
from PIL import Image

src, out, name = sys.argv[1], sys.argv[2], sys.argv[3]
os.makedirs(out, exist_ok=True)
img = cv2.imread(src)
H, W = img.shape[:2]
cw, ch = W // 4, H // 2
frames = []
for r in range(2):
    for c in range(4):
        cell = img[r * ch:(r + 1) * ch, c * cw:(c + 1) * cw].copy()
        h, w = cell.shape[:2]
        mask = np.full((h, w), cv2.GC_PR_BGD, np.uint8)
        # certain background: a band along the cell edges
        mask[:6, :] = mask[-4:, :] = mask[:, :10] = mask[:, -10:] = cv2.GC_BGD
        # find the character's strong edges (the glow is blurry, the outline is sharp)
        gray = cv2.cvtColor(cell, cv2.COLOR_BGR2GRAY)
        edges = cv2.Canny(gray, 60, 140)
        ys, xs = np.nonzero(edges[10:-10, 14:-14])
        x0, x1 = xs.min() + 14, xs.max() + 14
        y0, y1 = ys.min() + 10, ys.max() + 10
        mask[y0:y1, x0:x1] = cv2.GC_PR_FGD
        # certain foreground: the middle of the body (face, shirt, shorts)
        cx = (x0 + x1) // 2
        mask[int(y0 + 0.18 * (y1 - y0)):int(y0 + 0.80 * (y1 - y0)), cx - int(0.16 * (x1 - x0)):cx + int(0.16 * (x1 - x0))] = cv2.GC_FGD
        # lower legs and shoes: strong saturated colour = him; mid-grey haze = the glow
        hsv = cv2.cvtColor(cell, cv2.COLOR_BGR2HSV)
        s, v = hsv[..., 1].astype(int), hsv[..., 2].astype(int)
        legs = np.zeros((h, w), bool); legs[int(y0 + 0.70 * (y1 - y0)):y1 + 4, x0:x1] = True
        mask[legs & (s > 150) & (v > 120)] = cv2.GC_FGD          # bright saturated shoe blue
        mask[legs & (s < 70) & (v > 215)] = cv2.GC_PR_FGD        # white socks / soles
        mask[legs & (s < 90) & (v < 185)] = cv2.GC_BGD           # grey haze between and around the legs
        bgd, fgd = np.zeros((1, 65), np.float64), np.zeros((1, 65), np.float64)
        cv2.grabCut(cell, mask, None, bgd, fgd, 6, cv2.GC_INIT_WITH_MASK)
        fg = np.where((mask == cv2.GC_FGD) | (mask == cv2.GC_PR_FGD), 255, 0).astype(np.uint8)
        # leg zone clean-up: keep only skin, white socks and shoe blue; drop the hazy glow between the legs
        hue = hsv[..., 0].astype(int)
        skin = (hue < 25) & (s > 50) & (v > 120)
        sock = (s < 70) & (v > 215)
        strong = s > 170                       # shorts navy, shoe blue: any strongly coloured pixel stays
        haze = legs & ~(skin | sock | strong)
        fg[haze] = 0
        # keep the biggest piece, fill holes
        n, lab, stats, _ = cv2.connectedComponentsWithStats(fg, 8)
        if n > 1:
            keep = 1 + np.argmax(stats[1:, cv2.CC_STAT_AREA])
            fg = np.where(lab == keep, 255, 0).astype(np.uint8)
        ff = fg.copy(); m2 = np.zeros((h + 2, w + 2), np.uint8); cv2.floodFill(ff, m2, (0, 0), 255)
        fg = fg | cv2.bitwise_not(ff)
        # soft 1px edge
        fg = cv2.GaussianBlur(cv2.erode(fg, np.ones((2, 2), np.uint8)), (3, 3), 0)
        rgba = cv2.cvtColor(cell, cv2.COLOR_BGR2BGRA); rgba[..., 3] = fg
        bx, by, bw, bh = cv2.boundingRect((fg > 20).astype(np.uint8))
        frames.append(rgba[by:by + bh, bx:bx + bw])
for i, f in enumerate(frames):
    cv2.imwrite(f"{out}/{name}_{i:02d}.png", f)
# aligned preview: feet on one baseline, centred
Wc = max(f.shape[1] for f in frames) + 30; Hc = max(f.shape[0] for f in frames) + 20
pil = []
for f in frames:
    can = Image.new("RGBA", (Wc, Hc), (0, 0, 0, 0))
    im = Image.fromarray(cv2.cvtColor(f, cv2.COLOR_BGRA2RGBA))
    can.paste(im, ((Wc - im.width) // 2, Hc - 10 - im.height), im)
    pil.append(can)
strip = Image.new("RGBA", (Wc * 8, Hc), (238, 214, 182, 255))
for i, p in enumerate(pil): strip.alpha_composite(p, (i * Wc, 0))
strip.convert("RGB").resize((strip.width // 2, strip.height // 2), Image.LANCZOS).save(f"{out}/{name}_strip.png")
dark = Image.new("RGBA", (Wc * 2, Hc), (40, 44, 60, 255)); dark.alpha_composite(pil[0], (0, 0)); dark.alpha_composite(pil[5], (Wc, 0))
dark.convert("RGB").save(f"{out}/{name}_edgecheck.png")
gif = []
for p in pil:
    bg = Image.new("RGBA", p.size, (238, 214, 182, 255)); bg.alpha_composite(p); gif.append(bg.convert("RGB"))
gif[0].save(f"{out}/{name}.gif", save_all=True, append_images=gif[1:], duration=125, loop=0)
print("frames", [f.shape[:2] for f in frames])
