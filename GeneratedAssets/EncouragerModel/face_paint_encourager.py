"""Paint the Encourager's expressions into his texture (system Python + OpenCV).

Uses the student's face painter (StudentModel/face_paint.py) on this character's face maps
(render_encourager.py facemaps + facetris -> face/). His own face is an open grin, so:

  face/tex_half.png          eyes half closed  (blink)
  face/tex_closed.png        eyes closed       (blink)
  face/tex_mouth_mid.png     mouth half open   (talking)
  face/tex_mouth_closed.png  closed smile      (talking)

usage: python face_paint_encourager.py
"""
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__))
os.environ["FACE_DIR"] = os.path.join(HERE, "face")
sys.path.insert(0, os.path.join(HERE, "..", "StudentModel"))
import cv2
import numpy as np
import face_paint as fp
from face_paint import base, blend, feather, H, W, xx, yy

fp.set_eyes([(670, 1110, (430, 900, 920, 1330)), (1380, 1110, (1130, 900, 1630, 1330))])


def mouth_mask():
    """The painted open grin (dark inside, teeth, tongue) as a filled region."""
    x0, y0, x1, y1 = 640, 1400, 1420, 1720
    sub = base[y0:y1, x0:x1]
    skin = fp.skin_tone((abs(xx - 1030) < 5) & (abs(yy - 1300) < 5), ring=120)
    non_skin = (np.linalg.norm(sub - skin, axis=2) > 60).astype(np.uint8)
    _, lab = cv2.connectedComponents(non_skin)
    keep = lab == lab[1550 - y0, 1020 - x0]
    hull = cv2.convexHull(np.argwhere(keep)[:, ::-1].astype(np.int32))
    m = np.zeros((H, W), np.uint8); cv2.fillConvexPoly(m, hull + np.array([x0, y0]), 1)
    return cv2.dilate(m, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (15, 15))) > 0


MOUTH = mouth_mask()
_cols = np.where(MOUTH.any(0))[0]
LX, RX = _cols.min() + 10, _cols.max() - 10                       # corners
LY = np.where(MOUTH[:, LX])[0].mean(); RY = np.where(MOUTH[:, RX])[0].mean()
MID = (LX + RX) / 2
DEPTH = np.ptp(np.where(MOUTH[:, int(MID)])[0])                     # grin height at the middle
print("mouth corners", (LX, int(LY)), (RX, int(RY)), "depth", DEPTH)


def quad(p0, pm, p1, n=60):
    t = np.linspace(0, 1, n)[:, None]
    c = 2 * np.asarray(pm, float) - 0.5 * (np.asarray(p0, float) + np.asarray(p1, float))
    return (1 - t) ** 2 * np.asarray(p0, float) + 2 * (1 - t) * t * c + t ** 2 * np.asarray(p1, float)


def erase_mouth(img):
    """His skin continued over the grin (colour-matched to the skin around it)."""
    fill = fp.skin_fill(MOUTH)
    ring = lambda r: cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (r, r))
    outside = (cv2.dilate(MOUTH.astype(np.uint8), ring(31)) > 0) & ~MOUTH & (fp.alpha > 250)
    inside = MOUTH & ~(cv2.erode(MOUTH.astype(np.uint8), ring(31)) > 0)
    fill += np.median(base[outside], 0) - np.median(fill[inside], 0)
    return blend(img, fill, np.clip(feather(MOUTH, 6) * 1.15, 0, 1))


LIP = np.array([28, 30, 96], np.float32)       # dark warm red-brown (BGR)


def mouth_closed(img):
    img = erase_mouth(img)
    line = np.zeros((H, W), np.uint8)
    pts = quad((LX, LY), (MID, (LY + RY) / 2 + 0.38 * DEPTH), (RX, RY)).astype(np.int32)
    for i, ((x0, y0), (x1, y1)) in enumerate(zip(pts, pts[1:])):
        k = 1 - (2 * i / len(pts) - 1) ** 2
        cv2.line(line, (int(x0), int(y0)), (int(x1), int(y1)), 1, int(11 + 10 * k), cv2.LINE_AA)
    return blend(img, LIP, feather(line > 0, 1.3))


def mouth_mid(img):
    img = erase_mouth(img)
    inset = 0.16 * (RX - LX)
    L, R = (LX + inset, LY + 6), (RX - inset, RY + 6)
    top = quad(L, (MID, (LY + RY) / 2 + 0.22 * DEPTH), R)
    bot = quad(R, (MID, (LY + RY) / 2 + 0.78 * DEPTH), L)
    poly = np.vstack([top, bot]).astype(np.int32)
    m = np.zeros((H, W), np.uint8); cv2.fillPoly(m, [poly], 1, cv2.LINE_AA); m = m > 0
    img = blend(img, np.array([38, 30, 98], np.float32), feather(m, 1.5))
    teeth = np.zeros((H, W), np.uint8)
    cv2.fillPoly(teeth, [np.vstack([top, top[::-1] + [0, 0.16 * DEPTH]]).astype(np.int32)], 1, cv2.LINE_AA)
    img = blend(img, np.array([236, 244, 250], np.float32), feather((teeth > 0) & m, 1.2))
    tongue = np.zeros((H, W), np.uint8)
    cv2.ellipse(tongue, (int(MID), int((LY + RY) / 2 + 0.66 * DEPTH)), (int(0.22 * (RX - LX)), int(0.16 * DEPTH)), 0, 0, 360, 1, -1, cv2.LINE_AA)
    img = blend(img, np.array([112, 108, 222], np.float32), feather((tongue > 0) & m, 2))
    rim = np.zeros((H, W), np.uint8); cv2.polylines(rim, [poly], True, 1, 8, cv2.LINE_AA)
    return blend(img, np.array([30, 28, 90], np.float32), feather(rim > 0, 1.2))


VARIANTS = {
    "half": lambda: fp.eyelids(base.copy(), 0.5, False),
    "closed": lambda: fp.eyelids(base.copy(), 1.0, True),
    "mouth_mid": lambda: mouth_mid(base.copy()),
    "mouth_closed": lambda: mouth_closed(base.copy()),
}

if __name__ == "__main__":
    for name, fn in VARIANTS.items():
        img = fn()
        prev = np.where(fp.alpha[..., None] > 0, img, 255)
        cv2.imwrite(os.path.join(fp.F, f"preview_{name}.png"), cv2.resize(prev.astype(np.uint8), (768, 768), interpolation=cv2.INTER_AREA))
        fp.write_texture(img, name)
