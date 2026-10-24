"""Paint the student's facial expressions into his texture (system Python + OpenCV).

render_sprites.py facemaps renders an unlit, straight-on close-up of the face
(face/face_color.png) and the UV under every pixel (face/face_uv.png, 16-bit).
This script paints each expression on the close-up -- eyelids from his own
skin tone, a lash line, an open grin -- then writes the painted pixels back
into a copy of the base-colour texture through the UV map:

  face/tex_half.png    eyes half closed      (blink in-between)
  face/tex_closed.png  eyes closed           (blink)
  face/tex_smile.png   open grin, rosy cheeks

usage: python face_paint.py            (writes face/preview_*.png too)
"""
import json, os
import cv2
import numpy as np

HERE = os.path.dirname(os.path.abspath(__file__))
F = os.environ.get("FACE_DIR", os.path.join(HERE, "face"))   # another character's face maps (see EncouragerModel)
meta = json.load(open(os.path.join(F, "face_maps.json")))
TEX_PATH = meta["texture"]

col = cv2.imread(os.path.join(F, "face_color.png"), cv2.IMREAD_UNCHANGED)
base = col[..., :3].astype(np.float32)          # BGR, the texture's own colours
alpha = col[..., 3]
uvimg = cv2.imread(os.path.join(F, "face_uv.png"), cv2.IMREAD_UNCHANGED).astype(np.float64) / 65535
U, V = uvimg[..., 2], uvimg[..., 1]
H, W = base.shape[:2]
yy, xx = np.mgrid[0:H, 0:W]

LASH = np.array([22, 26, 46], np.float32)        # dark brown (BGR)
ROSE = np.array([120, 118, 236], np.float32)


def feather(mask, r):
    m = mask.astype(np.float32)
    return cv2.GaussianBlur(m, (0, 0), r) if r > 0 else m


def blend(img, colour, m):
    m = m[..., None]
    return img * (1 - m) + colour * m


def skin_tone(mask, ring=40):
    """Median colour of the skin just around a feature."""
    k = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (2 * ring + 1, 2 * ring + 1))
    around = (cv2.dilate(mask.astype(np.uint8), k) > 0) & ~mask & (alpha > 250)
    px = base[around]
    # keep only skin-like pixels (bright, warm), not lashes / hair / brows
    lum = px.mean(1)
    px = px[(lum > np.percentile(lum, 40)) & (px[:, 2] > px[:, 0] + 60)]
    return np.median(px, 0)


def eye_mask(cx, cy, box):
    """The whole painted eye (white, iris, rim) around (cx, cy), as a filled convex region."""
    x0, y0, x1, y1 = box
    sub = base[y0:y1, x0:x1]
    skin = skin_tone(np.zeros((H, W), bool) | ((abs(xx - cx) < 5) & (abs(yy - cy) < 5)), ring=300)
    d = np.linalg.norm(sub - skin, axis=2)
    non_skin = (d > 55).astype(np.uint8)
    n, lab = cv2.connectedComponents(non_skin)
    keep = lab == lab[cy - y0, cx - x0]
    hull = cv2.convexHull(np.argwhere(keep)[:, ::-1].astype(np.int32))
    m = np.zeros((H, W), np.uint8)
    cv2.fillConvexPoly(m, hull + np.array([x0, y0]), 1)
    m = cv2.dilate(m, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (13, 13)))
    return m > 0


EYES = [(536, 1170, (260, 860, 820, 1480)), (1480, 1160, (1200, 860, 1760, 1470))]   # the student's
eyes = []


def set_eyes(spec):
    """(centre x, centre y, search box) per eye, in close-up pixels."""
    eyes[:] = [eye_mask(cx, cy, box) for cx, cy, box in spec]


def lid_edge_y(mask, frac):
    """Per column: the eye's top boundary moved down by frac of the eye's height there."""
    cols = np.where(mask.any(0))[0]
    top = np.array([np.argmax(mask[:, c]) for c in cols], float)
    bot = np.array([H - 1 - np.argmax(mask[::-1, c]) for c in cols], float)
    return cols, top + frac * (bot - top), top, bot


def skin_fill(mask):
    """Skin continued smoothly in from all round the mask (inpainted at low res, so it's soft like real shading)."""
    s = 4
    small = cv2.resize(base.astype(np.uint8), (W // s, H // s), interpolation=cv2.INTER_AREA)
    m = cv2.resize(cv2.dilate(mask.astype(np.uint8), cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (25, 25))), (W // s, H // s), interpolation=cv2.INTER_NEAREST)
    filled = cv2.inpaint(small, m, 12, cv2.INPAINT_TELEA)
    filled = cv2.GaussianBlur(filled, (0, 0), 3)
    return cv2.resize(filled, (W, H), interpolation=cv2.INTER_CUBIC).astype(np.float32)


def eyelids(img, frac, closed):
    for m0 in eyes:
        # cover the painted eye's soft dark fringe too, or it shows as a ring round the lid
        m = cv2.dilate(m0.astype(np.uint8), cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (25, 25))) > 0
        fill = skin_fill(m)
        # match the fill to the skin right outside it, so the lid has no visible edge
        ring = lambda r: cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (r, r))
        outside = (cv2.dilate(m.astype(np.uint8), ring(31)) > 0) & ~m & (alpha > 250)
        inside = m & ~(cv2.erode(m.astype(np.uint8), ring(31)) > 0)
        fill += np.median(base[outside], 0) - np.median(fill[inside], 0)
        cols, edge, top, bot = lid_edge_y(m, frac)
        mid = cols.mean(); half_w = (cols.max() - cols.min()) / 2
        left_side = mid < W / 2
        if closed:
            # corner to corner, sagging gently: a relaxed closed eye
            yl = np.where(m[:, cols.min()])[0].mean(); yr = np.where(m[:, cols.max()])[0].mean()
            k = 1 - ((cols - mid) / half_w) ** 2
            edge = yl + (yr - yl) * (cols - cols.min()) / (2 * half_w) + 0.10 * (bot.max() - top.min()) * k
            lid = m.copy()
        else:
            lid = np.zeros((H, W), bool)
            for c, e in zip(cols, edge):
                lid[:int(e), c] = True
            lid &= m
        # the lid: his own skin, with a soft shadow just above the lash line so it reads as rounded
        lash_y = np.full(W, -1e9); lash_y[cols] = edge
        d = lash_y[None, :] - yy
        shade = 1 - 0.10 * np.exp(-np.clip(d, 0, None) / 30.0) * (d > -5)
        img = blend(img, fill * shade[..., None], np.clip(feather(lid, 10) * 1.3, 0, 1) * (alpha > 0))
        # lash line, thicker in the middle, a flick at the outer corner
        line = np.zeros((H, W), np.uint8)
        pts = [(int(c), int(e)) for c, e in zip(cols[::4], edge[::4])]
        for (x0, y0), (x1, y1) in zip(pts, pts[1:]):
            k = 1 - ((0.5 * (x0 + x1) - mid) / half_w) ** 2
            cv2.line(line, (x0, y0), (x1, y1), 1, int(10 + 14 * max(0, k)), cv2.LINE_AA)
        outer = min(pts, key=lambda p: p[0]) if left_side else max(pts, key=lambda p: p[0])
        flick = (outer[0] + (-38 if left_side else 38), outer[1] - (22 if closed else 26))
        cv2.line(line, outer, flick, 1, 11, cv2.LINE_AA)
        img = blend(img, LASH, feather(line > 0, 1.2))
    return img


def grin(img):
    # erase the painted closed smile
    box = (yy > 1560) & (yy < 1840) & (xx > 600) & (xx < 1400)
    lum = base.mean(2)
    old = box & (base[..., 0] < 75) & (lum < 120) | box & (base[..., 2] - base[..., 0] > 150) & (lum < 125)
    old = cv2.dilate(old.astype(np.uint8), cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (15, 15))) > 0
    skin = skin_tone(old, ring=30)
    img = blend(img, skin, feather(old, 3))
    # open grin: flat-ish smiling top edge, round bottom
    L, R = np.array([660, 1650.]), np.array([1335, 1655.])
    def quad(p0, pm, p1, n=60):
        t = np.linspace(0, 1, n)[:, None]
        c = 2 * pm - 0.5 * (p0 + p1)     # control point so the curve passes through pm
        return (1 - t) ** 2 * p0 + 2 * (1 - t) * t * c + t ** 2 * p1
    top = quad(L, np.array([1000, 1712.]), R)
    bot = quad(R, np.array([1000, 1905.]), L)
    poly = np.vstack([top, bot]).astype(np.int32)
    mouth = np.zeros((H, W), np.uint8); cv2.fillPoly(mouth, [poly], 1, cv2.LINE_AA); mouth = mouth > 0
    img = blend(img, np.array([38, 30, 98], np.float32), feather(mouth, 1.5))
    teeth = np.zeros((H, W), np.uint8)
    cv2.fillPoly(teeth, [np.vstack([top, top[::-1] + [0, 46]]).astype(np.int32)], 1, cv2.LINE_AA)
    img = blend(img, np.array([236, 244, 250], np.float32), feather((teeth > 0) & mouth, 1.2))
    tongue = np.zeros((H, W), np.uint8)
    cv2.ellipse(tongue, (1000, 1872), (175, 62), 0, 0, 360, 1, -1, cv2.LINE_AA)
    img = blend(img, np.array([112, 108, 222], np.float32), feather((tongue > 0) & mouth, 2))
    rim = np.zeros((H, W), np.uint8); cv2.polylines(rim, [poly], True, 1, 9, cv2.LINE_AA)
    img = blend(img, np.array([30, 28, 90], np.float32), feather(rim > 0, 1.2))
    for c in ((650, 1640), (1345, 1645)):   # little dimples at the corners
        d = np.zeros((H, W), np.uint8); cv2.ellipse(d, c, (26, 10), -30 if c[0] < 1000 else 30, 0, 360, 1, -1, cv2.LINE_AA)
        img = blend(img, np.array([60, 80, 170], np.float32), feather(d > 0, 4) * 0.5)
    # rosy cheeks
    for c in ((400, 1500), (1610, 1490)):
        e = np.zeros((H, W), np.uint8); cv2.ellipse(e, c, (150, 95), 0, 0, 360, 1, -1)
        img = blend(img, ROSE, feather(e > 0, 40) * 0.28 * (alpha > 0))
    return img


VARIANTS = {
    "half": lambda: eyelids(base.copy(), 0.5, False),
    "closed": lambda: eyelids(base.copy(), 1.0, True),
    "smile": lambda: grin(base.copy()),
}


def write_texture(painted, name):
    """Fill every front-facing head triangle's texels from the painted close-up (face/tri_*.npy from
    render_sprites.py facetris). Rasterising in UV space paints ALL surface layers -- the eyes and mouth
    have doubled surfaces that z-fight, so mapping only the visible pixel left whole patches unpainted."""
    tex = cv2.imread(TEX_PATH, cv2.IMREAD_UNCHANGED)[..., :3].astype(np.float32)
    TH, TW = tex.shape[:2]
    diff = (np.abs(painted - base).max(2) > 1.0) & (alpha > 0)
    tri_uv = np.load(os.path.join(F, "tri_uv.npy"))
    tri_scr = np.load(os.path.join(F, "tri_scr.npy"))
    tuv = np.stack([tri_uv[..., 0] * TW, (1 - tri_uv[..., 1]) * TH], -1)        # texel space
    tsc = np.stack([tri_scr[..., 0] * W, (1 - tri_scr[..., 1]) * H], -1)       # close-up pixel space
    ys, xs = np.nonzero(diff)
    x0, x1, y0, y1 = xs.min() - 4, xs.max() + 4, ys.min() - 4, ys.max() + 4
    near = (tsc[..., 0].max(1) >= x0) & (tsc[..., 0].min(1) <= x1) & (tsc[..., 1].max(1) >= y0) & (tsc[..., 1].min(1) <= y1)
    acc = np.zeros((TH, TW, 3), np.float64); cnt = np.zeros((TH, TW))
    for P, S in zip(tuv[near], tsc[near]):
        (ax, ay), (bx, by), (cx, cy) = P
        det = (by - cy) * (ax - cx) + (cx - bx) * (ay - cy)
        if abs(det) < 1e-9: continue
        gx0, gx1 = int(np.floor(P[:, 0].min())) - 1, int(np.ceil(P[:, 0].max())) + 1
        gy0, gy1 = int(np.floor(P[:, 1].min())) - 1, int(np.ceil(P[:, 1].max())) + 1
        gy, gx = np.mgrid[max(gy0, 0):min(gy1, TH - 1) + 1, max(gx0, 0):min(gx1, TW - 1) + 1]
        px, py = gx + 0.5, gy + 0.5
        l1 = ((by - cy) * (px - cx) + (cx - bx) * (py - cy)) / det
        l2 = ((cy - ay) * (px - cx) + (ax - cx) * (py - cy)) / det
        l3 = 1 - l1 - l2
        inside = (l1 >= -0.3) & (l2 >= -0.3) & (l3 >= -0.3)     # a little past the edge, so chart borders are covered too
        if not inside.any(): continue
        sx = np.clip((l1 * S[0, 0] + l2 * S[1, 0] + l3 * S[2, 0])[inside].astype(int), 0, W - 1)
        sy = np.clip((l1 * S[0, 1] + l2 * S[1, 1] + l3 * S[2, 1])[inside].astype(int), 0, H - 1)
        hit = diff[sy, sx]
        if not hit.any(): continue
        tyy, txx = gy[inside][hit], gx[inside][hit]
        np.add.at(acc, (tyy, txx), painted[sy[hit], sx[hit]])
        np.add.at(cnt, (tyy, txx), 1)
    got = cnt > 0
    tex[got] = acc[got] / cnt[got][:, None]
    out = os.path.join(F, f"tex_{name}.png")
    cv2.imwrite(out, np.clip(tex, 0, 255).astype(np.uint8))
    print(name, "texels painted", int(got.sum()), "->", out)


if __name__ == "__main__":
    set_eyes(EYES)
    for name, fn in VARIANTS.items():
        img = fn()
        prev = np.where(alpha[..., None] > 0, img, 255)
        cv2.imwrite(os.path.join(F, f"preview_{name}.png"), cv2.resize(prev.astype(np.uint8), (768, 768), interpolation=cv2.INTER_AREA))
        write_texture(img, name)
