"""Paint the Boredom Zombie's expressions into his texture (system Python + OpenCV).

Uses the student's face painter (StudentModel/face_paint.py) on this character's face maps
(render_zombie.py facemaps + facetris -> face/). His own face is a sulky closed mouth (a thin line
with one little tooth) under heavy, half-closed lids, so:

  face/tex_half.png        eyes nearly shut       (slow blink)
  face/tex_closed.png      eyes closed            (blink, slump)
  face/tex_mouth_mid.png   small open mouth       (muttering)
  face/tex_mouth_open.png  open groan             (grumbling, groan)
  face/tex_yawn.png        eyes shut + big yawn

usage: python face_paint_zombie.py
"""
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__))
os.environ["FACE_DIR"] = os.path.join(HERE, "face")
sys.path.insert(0, os.path.join(HERE, "..", "StudentModel"))
import cv2
import numpy as np
import face_paint as fp
from face_paint import base, blend, feather, H, W, xx, yy

EYES = [(660, 1150, (400, 960, 930, 1350)), (1410, 1170, (1140, 960, 1700, 1360))]
fp.set_eyes(EYES)

# His mouth: the thin sulky line and the tooth hanging under it (close-up pixels).
BOX = (680, 1470, 1380, 1640)


def mouth_mask():
    x0, y0, x1, y1 = BOX
    sub = base[y0:y1, x0:x1]
    skin = fp.skin_tone((abs(xx - 1020) < 5) & (abs(yy - 1400) < 5), ring=120)
    non_skin = (np.linalg.norm(sub - skin, axis=2) > 45).astype(np.uint8)
    n, lab, st, _ = cv2.connectedComponentsWithStats(non_skin)
    keep = np.zeros_like(non_skin, bool)
    for i in range(1, n):
        if st[i, cv2.CC_STAT_AREA] > 40: keep |= lab == i
    m = np.zeros((H, W), np.uint8)
    m[y0:y1, x0:x1] = keep
    return cv2.dilate(m, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (17, 17))) > 0


MOUTH = mouth_mask()
ys, xs = np.nonzero(MOUTH)
LX, RX = xs.min() + 12, xs.max() - 12
LINE_Y = float(np.percentile(ys, 30))          # the lip line (the tooth hangs below it)
MID = (LX + RX) / 2
print("mouth", (LX, RX), "line y", int(LINE_Y))


def erase_mouth(img):
    fill = fp.skin_fill(MOUTH)
    ring = lambda r: cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (r, r))
    outside = (cv2.dilate(MOUTH.astype(np.uint8), ring(31)) > 0) & ~MOUTH & (fp.alpha > 250)
    inside = MOUTH & ~(cv2.erode(MOUTH.astype(np.uint8), ring(31)) > 0)
    fill += np.median(base[outside], 0) - np.median(fill[inside], 0)
    return blend(img, fill, np.clip(feather(MOUTH, 5) * 1.15, 0, 1))


DARK = np.array([40, 30, 70], np.float32)       # inside of the mouth (BGR)
RIM = np.array([34, 30, 64], np.float32)
TONGUE = np.array([112, 108, 210], np.float32)
TOOTH = np.array([228, 238, 242], np.float32)


def opening(img, cx, cy, rx, ry, tongue=True, tooth=True):
    """A soft oval mouth opening, with the tongue at the bottom and his one tooth at the top right."""
    m = np.zeros((H, W), np.uint8)
    cv2.ellipse(m, (int(cx), int(cy)), (int(rx), int(ry)), 0, 0, 360, 1, -1, cv2.LINE_AA)
    m = m > 0
    img = blend(img, DARK, feather(m, 1.5))
    if tongue:
        t = np.zeros((H, W), np.uint8)
        cv2.ellipse(t, (int(cx), int(cy + ry * 0.55)), (int(rx * 0.6), int(ry * 0.45)), 0, 0, 360, 1, -1, cv2.LINE_AA)
        img = blend(img, TONGUE, feather((t > 0) & m, 2))
    rim = np.zeros((H, W), np.uint8)
    cv2.ellipse(rim, (int(cx), int(cy)), (int(rx), int(ry)), 0, 0, 360, 1, 7, cv2.LINE_AA)
    img = blend(img, RIM, feather(rim > 0, 1.2))
    if tooth:
        tx, ty = cx + rx * 0.45, cy - ry * 0.92
        tri = np.array([[tx - 16, ty], [tx + 16, ty], [tx + 2, ty + max(22, ry * 0.45)]], np.int32)
        tm = np.zeros((H, W), np.uint8); cv2.fillPoly(tm, [tri], 1, cv2.LINE_AA)
        img = blend(img, TOOTH, feather(tm > 0, 1.2))
    return img


def mouth_mid(img):
    return opening(erase_mouth(img), MID, LINE_Y + 6, 0.28 * (RX - LX), 30, tongue=False)


def mouth_open(img):
    return opening(erase_mouth(img), MID, LINE_Y + 14, 0.38 * (RX - LX), 50)


def yawn(img):
    img = fp.eyelids(img, 1.0, True)
    return opening(erase_mouth(img), MID, LINE_Y + 16, 0.30 * (RX - LX), 82, tooth=True)


VARIANTS = {
    "half": lambda: fp.eyelids(base.copy(), 0.6, False),
    "closed": lambda: fp.eyelids(base.copy(), 1.0, True),
    "mouth_mid": lambda: mouth_mid(base.copy()),
    "mouth_open": lambda: mouth_open(base.copy()),
    "yawn": lambda: yawn(base.copy()),
}

if __name__ == "__main__":
    for name, fn in VARIANTS.items():
        img = fn()
        prev = np.where(fp.alpha[..., None] > 0, img, 255)
        cv2.imwrite(os.path.join(fp.F, f"preview_{name}.png"), cv2.resize(prev.astype(np.uint8), (768, 768), interpolation=cv2.INTER_AREA))
        fp.write_texture(img, name)
