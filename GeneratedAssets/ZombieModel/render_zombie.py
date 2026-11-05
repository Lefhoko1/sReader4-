"""The Boredom Zombie: modelCreator model -> game sprite frames (headless Blender).

usage:
  blender -b -P render_zombie.py -- weights  <out_dir>   bone regions, four sides
  blender -b -P render_zombie.py -- posetest <out_dir>   key poses of each move
  blender -b -P render_zombie.py -- facemaps <out_dir>   face close-up + UV map (FACE_EXPR=x checks a painted one)
  blender -b -P render_zombie.py -- facetris <out_dir>   face triangles for face_paint_zombie.py
  blender -b -P render_zombie.py -- render   <frames_root>   (ONLY=a,b limits the sets)

He stands beside the dashboard facing the camera: a sleepy, grumpy kid zombie (hoodie, jeans,
red sneakers), so every move is slow and slouchy. His hoodie's sleeves are fused to its body above
the hands, so arm moves stay small (dangle, shrug, stretch) — big raises would tear the hoodie.
"""
import math, os, sys
sys.path.insert(0, os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "CharacterSprites"))
import sprite_core as core
from sprite_core import deg, ss, lerp_table, FRONT, Aim

HERE = os.path.dirname(os.path.abspath(__file__))
args = sys.argv[sys.argv.index("--") + 1:]
MODE, OUT = args[0], args[1]

# Joints from the master mesh's cross-sections (look.py, armscan.py). He faces +Y; his right side is +X.
HIP_Z, NECK_Z, SHOULDER_Z, KNEE_Z = 0.38, 0.78, 0.60, 0.19
BONES = {
    "body": ((0, 0, HIP_Z), (0, 0, NECK_Z)),
    "head": ((0, 0, NECK_Z), (0, 0, 1.20)),
    "leg_right": ((0.09, 0, HIP_Z), (0.09, 0, KNEE_Z)),            # thigh
    "leg_left": ((-0.09, 0, HIP_Z), (-0.09, 0, KNEE_Z)),
    "shin_right": ((0.09, 0, KNEE_Z), (0.09, 0, 0.02)),            # knee down (so he can sit)
    "shin_left": ((-0.09, 0, KNEE_Z), (-0.09, 0, 0.02)),
    "arm_right": ((0.16, 0, SHOULDER_Z), (0.225, 0, 0.29)),
    "arm_left": ((-0.16, 0, SHOULDER_Z), (-0.225, 0, 0.29)),
}
# |x| between the hanging arm (sleeve) and the hoodie's body, slice by slice: a clean gap up to 0.43,
# then the outer band of the hoodie (the sleeve is about 9 cm wide) until it blends into the shoulder.
ARM_GAP = [(0.27, 0.178), (0.30, 0.173), (0.32, 0.152), (0.34, 0.151), (0.36, 0.153), (0.38, 0.159),
           (0.40, 0.161), (0.42, 0.167), (0.44, 0.172), (0.50, 0.168), (0.54, 0.160)]


def weights(p):
    ax, z = abs(p.x), p.z
    side = "right" if p.x > 0 else "left"
    head = ss(0.76, 0.81, z)
    if z < 0.27: arm = 0.0                                    # the sneakers are as wide as the hands
    elif z < 0.54:
        b = lerp_table(ARM_GAP, z); arm = ss(b - 0.004, b + 0.004, ax)
    else:
        arm = ss(0.13, 0.16, ax) * (1 - ss(0.56, 0.66, z))   # sleeve into the shoulder
    arm *= 1 - head
    leg = (1 - ss(0.31, 0.41, z)) * (1 - arm)
    shin = leg * (1 - ss(KNEE_Z - 0.035, KNEE_Z + 0.035, z))
    leg -= shin
    body = max(0.0, 1 - head - arm - leg - shin)
    return {k: v for k, v in (("head", head), ("arm_" + side, arm), ("leg_" + side, leg), ("shin_" + side, shin),
                              ("body", body)) if v > 1e-4}


CFG = {
    "gen": r"C:\Users\11113930\Documents\mine\modelCreator\my-app\outputs\boredom-zombie-20261021-223213",
    "height": 1.25,
    "bones": BONES,
    "parents": {"shin_right": "leg_right", "shin_left": "leg_left"},
    "weights": weights,
    "cut_bridges_below": 0.43,
    "face": (0.48, 0.92, NECK_Z),       # close-up width, centre height, neck (lowest face triangle)
    "face_dir": os.path.join(HERE, "face"),
}

rig = core.Rig(CFG)
A, R = rig.arm, rig.root
FWD = -1.0   # body / head bones point up: a positive X tilt leans him BACK, so forward is negative


def mirror(p): return (-p[0], p[1], p[2])
def lerp(a, b, k): return tuple(x + (y - x) * k for x, y in zip(a, b))
REST_HAND = (0.225, 0.0, 0.29)


def hands(right, left=None, **extra):
    left = left or mirror(right)
    return dict(arm_right=Aim(rig.hand_dir("arm_right", right)), arm_left=Aim(rig.hand_dir("arm_left", left)), **extra)


def slouch(t, amount=1.0):
    """His resting slouch: head droops a little, a slow side-to-side sway, arms dangling with it."""
    s = math.sin(2 * math.pi * t)
    c = math.sin(2 * math.pi * t + 1.3)
    return dict(body=(deg(FWD * 2.5 * amount + 0.8 * s), 0, deg(1.8 * c)),
                head=(deg(FWD * 6 * amount - 1.5 * s), deg(2.0 * c), deg(-2.5 * c)),
                arm_right=(deg(-1.5 * s), 0, deg(2 * c)), arm_left=(deg(-1.5 * s), 0, deg(2 * c))), 0.003 * s


def idle(t, i):
    rot, lift = slouch(t); rig.pose(**rot); R.location = (0, 0, lift)


def blink(face):
    return lambda t, i: (idle(t, i), face)[1]


# grumbling: his sulky closed mouth, then muttering
GRUMBLE_MOUTH = [None, None, "mouth_mid", "mouth_open", "mouth_mid", None, "mouth_mid", "mouth_mid",
                 None, None, "mouth_open", "mouth_mid", None, "mouth_mid", None, None,
                 "mouth_mid", "mouth_open", "mouth_open", "mouth_mid", None, None, "mouth_mid", None]


def grumble(t, i):
    rot, lift = slouch(t)
    s = math.sin(2 * math.pi * 2 * t)
    rot["head"] = (deg(FWD * (5 + 2 * math.sin(2 * math.pi * 3 * t))), deg(3 * s), deg(-5 * math.sin(2 * math.pi * t)))   # slow, fed-up head shake
    rot["arm_right"] = Aim(rig.hand_dir("arm_right", (0.25, 0.10 + 0.03 * s, 0.36 + 0.03 * s)))                   # a lazy wave of the hand
    rig.pose(**rot); R.location = (0, 0, lift)
    return GRUMBLE_MOUTH[i % len(GRUMBLE_MOUTH)]


def yawn(t, i):
    """Arms stretch out a little, head tips back, a huge yawn, then he sags back down."""
    k = ss(0.0, 0.35, t) * (1 - ss(0.72, 1.0, t))
    rot, lift = slouch(t, 1.0 - k)
    rot["head"] = (deg(FWD * 6 * (1 - k) + 12 * k), 0, 0)      # tips back for the yawn
    rot["body"] = (deg(FWD * 2.5 * (1 - k) + 3 * k), 0, 0)
    reach = lerp(REST_HAND, (0.30, 0.05, 0.36), k)
    rot.update(hands(reach))
    rig.pose(**rot); R.location = (0, 0, 0.01 * k)
    return "yawn" if 0.22 < t < 0.82 else ("mouth_mid" if 0.12 < t < 0.9 else None)


def groan(t, i):
    """'Huh, whatever': a slow shrug, palms out, head rolls back, mouth open in a groan."""
    k = ss(0.0, 0.3, t) * (1 - ss(0.7, 1.0, t))
    rot, lift = slouch(t)
    rot.update(hands(lerp(REST_HAND, (0.29, 0.12, 0.38), k)))
    rot["head"] = (deg(FWD * 6 * (1 - k) + 8 * k), 0, deg(6 * k))   # rolls back: "ugh"
    rot["body"] = (deg(FWD * 2.5), 0, deg(-2 * k))
    rig.pose(**rot); R.location = (0, 0, 0.012 * k)
    return "mouth_open" if 0.25 < t < 0.75 else None


def slump(t, i):
    """Defeated: he droops forward, head hangs, arms dangle in front, eyes closed — then slowly comes back up."""
    k = ss(0.0, 0.3, t) * (1 - ss(0.75, 1.0, t))
    rot = hands(lerp(REST_HAND, (0.16, 0.12, 0.30), k),
                body=(deg(FWD * (2.5 + 12 * k)), 0, deg(3 * k)), head=(deg(FWD * (6 + 18 * k)), 0, deg(-4 * k)))
    rig.pose(**rot); R.location = (0, 0, -0.02 * k)
    return "closed" if k > 0.6 else ("half" if k > 0.25 else None)


# ── seated on his chair (the house): bored ─────────────────────────────────────────
LAP_HAND = (0.10, 0.13, 0.33)              # hands flopped on his thighs
SIT_THIGH = 50                             # thighs angled 40 deg below level: feet down on the floor (not dangling)


def seated(k=1.0):
    """k = 1 sitting (thighs sloping down, shins upright, feet on the floor), 0 standing."""
    return dict(leg_right=(deg(SIT_THIGH * k), 0, 0), leg_left=(deg(SIT_THIGH * k), 0, 0),
                shin_right=(deg(-SIT_THIGH * k), 0, 0), shin_left=(deg(-SIT_THIGH * k), 0, 0))


def bored(t, droop=1.0):
    """His bored sit: slumped forward, head lolling to one side, a slow heavy sway."""
    s_ = math.sin(2 * math.pi * t)
    c = math.sin(2 * math.pi * t + 1.3)
    return dict(body=(deg(FWD * 7 * droop + 0.8 * s_), 0, deg(2.5 * c)),
                head=(deg(FWD * 10 * droop - 1.5 * s_), deg(9 * droop + 2 * c), deg(-4 * c))), 0.003 * s_


def seat_idle(t, i):
    rot, lift = bored(t)
    rot.update(seated())
    rot.update(hands(LAP_HAND))
    rot["shin_right"] = (deg(-SIT_THIGH + 4 * math.sin(2 * math.pi * t)), 0, 0)    # one foot idly tapping
    rig.pose(**rot); R.location = (0, 0, lift)


def seat_blink(face):
    return lambda t, i: (seat_idle(t, i), face)[1]


def seat_yawn(t, i):
    k = ss(0.0, 0.35, t) * (1 - ss(0.72, 1.0, t))
    rot, lift = bored(t, 1.0 - k)
    rot.update(seated())
    rot.update(hands(lerp(LAP_HAND, (0.28, 0.06, 0.40), k)))
    rot["head"] = (deg(FWD * 10 * (1 - k) + 12 * k), deg(9 * (1 - k)), 0)
    rot["body"] = (deg(FWD * 7 * (1 - k) + 3 * k), 0, 0)
    rig.pose(**rot); R.location = (0, 0, 0.008 * k)
    return "yawn" if 0.22 < t < 0.82 else ("mouth_mid" if 0.12 < t < 0.9 else None)


def seat_slump(t, i):
    """A word was solved: he sinks even lower in his chair, head hanging, eyes shut — then back to bored."""
    k = ss(0.0, 0.3, t) * (1 - ss(0.75, 1.0, t))
    rot, lift = bored(t, 1.0 + 1.4 * k)
    rot.update(seated())
    rot.update(hands(lerp(LAP_HAND, (0.12, 0.15, 0.27), k)))
    rig.pose(**rot); R.location = (0, 0, -0.012 * k + lift)
    return "closed" if k > 0.6 else ("half" if k > 0.25 else None)


def stand_sigh_sit(t, i):
    """Every so often: he gets up off the chair, hangs his head with a big sigh, and plops back down, discouraged."""
    up = ss(0.0, 0.22, t) * (1 - ss(0.72, 0.9, t))            # 0 sitting -> 1 standing -> 0 sitting
    sigh = ss(0.28, 0.42, t) * (1 - ss(0.58, 0.7, t))         # the sigh while standing
    plop = math.sin(math.pi * min(1.0, max(0.0, (t - 0.88) / 0.12)))   # a little bounce as he lands
    rot, lift = bored(t, 1.0 - 0.6 * up + 1.2 * sigh)
    rot.update(seated(1.0 - up))
    rot.update(hands(lerp(LAP_HAND, REST_HAND, up)))
    if sigh > 0: rot["head"] = (deg(FWD * (6 + 20 * sigh)), deg(4 * sigh), 0)
    rig.pose(**rot)
    R.location = (0, 0, lift + 0.01 * up - 0.01 * plop)
    if sigh > 0.5: return "closed"
    return "mouth_mid" if 0.3 < t < 0.6 else None


def seated_sets():
    return [("seat_idle", FRONT, lambda t, i: seat_idle(t, i), 32, True),
            ("seat_blink_half", FRONT, seat_blink("half"), 16, True),
            ("seat_blink_closed", FRONT, seat_blink("closed"), 16, True),
            ("seat_yawn", FRONT, seat_yawn, 28, False),
            ("seat_slump", FRONT, seat_slump, 28, False),
            ("stand_sigh_sit", FRONT, stand_sigh_sit, 48, False)]


if MODE == "seatposetest":
    os.makedirs(OUT, exist_ok=True)
    for name, fn, t in (("idle", seat_idle, 0.25), ("yawn", seat_yawn, 0.5), ("slump", seat_slump, 0.5),
                        ("stand_a", stand_sigh_sit, 0.12), ("stand_b", stand_sigh_sit, 0.5), ("stand_c", stand_sigh_sit, 0.95)):
        R.location = (0, 0, 0); fn(t, 0)
        rig.place_camera(FRONT); rig.render(os.path.join(OUT, f"{name}.png"))
    seat_idle(0.25, 0); rig.place_camera(90); rig.render(os.path.join(OUT, "idle_side.png"))
    sys.exit(0)

if MODE == "seated":
    only = os.environ.get("ONLY")
    rig.render_sets(OUT, seated_sets(), only.split(",") if only else None)
    sys.exit(0)

if MODE == "weights":
    rig.weights_view(OUT); sys.exit(0)
if MODE == "facemaps":
    rig.facemaps(OUT, os.environ.get("FACE_EXPR")); sys.exit(0)
if MODE == "facetris":
    rig.facetris(OUT); sys.exit(0)
if MODE == "posetest":
    os.makedirs(OUT, exist_ok=True)
    for name, fn, t in (("idle", idle, 0.25), ("grumble", grumble, 0.3), ("yawn", yawn, 0.5),
                        ("groan", groan, 0.5), ("slump", slump, 0.5)):
        R.location = (0, 0, 0); fn(t, 0)
        for yaw in (FRONT, 90):
            rig.place_camera(yaw); rig.render(os.path.join(OUT, f"{name}_{int(yaw):03d}.png"))
    sys.exit(0)

if MODE == "render":
    SETS = [
        ("idle", FRONT, idle, 32, True),
        ("blink_half", FRONT, blink("half"), 16, True),
        ("blink_closed", FRONT, blink("closed"), 16, True),
        ("grumble", FRONT, grumble, 24, True),
        ("yawn", FRONT, yawn, 28, False),
        ("groan", FRONT, groan, 24, False),
        ("slump", FRONT, slump, 28, False),
    ]
    only = os.environ.get("ONLY")
    rig.render_sets(OUT, SETS, only.split(",") if only else None)
