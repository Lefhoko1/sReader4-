"""The Encourager: modelCreator model -> game sprite frames (headless Blender).

usage:
  blender -b -P render_encourager.py -- weights  <out_dir>   bone regions, four sides
  blender -b -P render_encourager.py -- posetest <out_dir>   clap / cheer arm angles
  blender -b -P render_encourager.py -- facemaps <out_dir>   face close-up + UV map (FACE_EXPR=x checks a painted one)
  blender -b -P render_encourager.py -- facetris <out_dir>   face triangles for face_paint_encourager.py
  blender -b -P render_encourager.py -- render   <frames_root>   (ONLY=a,b limits the sets)

He stands beside the dashboard facing the camera, so every set is a front view.
"""
import math, os, sys
sys.path.insert(0, os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "CharacterSprites"))
import sprite_core as core
from sprite_core import deg, ss, lerp_table, FRONT, Aim
from mathutils import Vector

HERE = os.path.dirname(os.path.abspath(__file__))
args = sys.argv[sys.argv.index("--") + 1:]
MODE, OUT = args[0], args[1]

# Joints from the master mesh's cross-sections (look.py). He faces +Y; his right side is +X.
HIP_Z, NECK_Z, SHOULDER_Z, KNEE_Z = 0.42, 0.87, 0.66, 0.21
BONES = {
    "body": ((0, 0, HIP_Z), (0, 0, NECK_Z)),
    "head": ((0, 0, NECK_Z), (0, 0, 1.30)),
    "leg_right": ((0.105, 0, HIP_Z), (0.105, 0, KNEE_Z)),          # thigh
    "leg_left": ((-0.105, 0, HIP_Z), (-0.105, 0, KNEE_Z)),
    "shin_right": ((0.105, 0, KNEE_Z), (0.105, 0, 0.02)),          # knee down (so he can sit)
    "shin_left": ((-0.105, 0, KNEE_Z), (-0.105, 0, 0.02)),
    "arm_right": ((0.17, 0, SHOULDER_Z), (0.27, 0, 0.37)),
    "arm_left": ((-0.17, 0, SHOULDER_Z), (-0.27, 0, 0.37)),
}
# |x| that separates the hanging arm from the body, slice by slice (there is a clean gap all the way down)
ARM_GAP = [(0.34, 0.20), (0.39, 0.20), (0.40, 0.18), (0.53, 0.18), (0.54, 0.17), (0.56, 0.161),
           (0.58, 0.155), (0.60, 0.147), (0.62, 0.141), (0.64, 0.137)]


def weights(p):
    ax, z = abs(p.x), p.z
    side = "right" if p.x > 0 else "left"
    head = ss(0.85, 0.91, z)
    if z < 0.34: arm = 0.0
    elif z < 0.64:
        b = lerp_table(ARM_GAP, z); arm = ss(b - 0.004, b + 0.004, ax)
    else:
        arm = ss(0.125, 0.165, ax) * (1 - ss(0.66, 0.75, z))   # sleeve blends into the shoulder
    arm *= 1 - head
    leg = (1 - ss(0.34, 0.45, z)) * (1 - arm)
    shin = leg * (1 - ss(KNEE_Z - 0.035, KNEE_Z + 0.035, z))
    leg -= shin
    body = max(0.0, 1 - head - arm - leg - shin)
    return {k: v for k, v in (("head", head), ("arm_" + side, arm), ("leg_" + side, leg), ("shin_" + side, shin),
                              ("body", body)) if v > 1e-4}


CFG = {
    "gen": r"C:\Users\11113930\Documents\mine\modelCreator\my-app\outputs\encourager-boy-20261020-152024",
    "height": 1.35,
    "bones": BONES,
    "parents": {"shin_right": "leg_right", "shin_left": "leg_left"},
    "weights": weights,
    "face": (0.46, 1.08, NECK_Z),       # close-up width, centre height, neck (lowest face triangle)
    "face_dir": os.path.join(HERE, "face"),
}

rig = core.Rig(CFG)
A, R = rig.arm, rig.root

if MODE == "weights":
    rig.weights_view(OUT); sys.exit(0)
if MODE == "facemaps":
    rig.facemaps(OUT, os.environ.get("FACE_EXPR")); sys.exit(0)
if MODE == "facetris":
    rig.facetris(OUT); sys.exit(0)
if MODE == "posetest":
    os.makedirs(OUT, exist_ok=True)
    def hands(r, l=None):
        """Aim both arms at hand targets given for his right hand (left mirrors unless given)."""
        l = l or (-r[0], r[1], r[2])
        return {"arm_right": Aim(rig.hand_dir("arm_right", r)), "arm_left": Aim(rig.hand_dir("arm_left", l))}
    tests = {
        "clap_apart": hands((0.10, 0.27, 0.58)),
        "clap_together": hands((0.035, 0.24, 0.62)),
        "talk": {"arm_right": Aim(rig.hand_dir("arm_right", (0.27, 0.20, 0.50)))},
        "cheer": hands((0.33, 0.04, 0.95)),
    }
    for name, rot in tests.items():
        rig.pose(**rot)
        for yaw in (FRONT, 90):
            rig.place_camera(yaw); rig.render(os.path.join(OUT, f"{name}_{int(yaw):03d}.png"))
    sys.exit(0)

# ── full render ───────────────────────────────────────────────────────────────
def mirror(p): return (-p[0], p[1], p[2])
def lerp(a, b, k): return tuple(x + (y - x) * k for x, y in zip(a, b))
REST_HAND = (0.27, 0.0, 0.37)


def hands(right, left=None, **extra):
    left = left or mirror(right)
    return dict(arm_right=Aim(rig.hand_dir("arm_right", right)), arm_left=Aim(rig.hand_dir("arm_left", left)), **extra)


def breathe(t):
    s = math.sin(2 * math.pi * t)
    return dict(body=(deg(1.0 * s), 0, 0), head=(deg(-1.2 * s), 0, deg(1.5 * math.sin(2 * math.pi * t + 1))),
                arm_right=(deg(-2 * s), 0, 0), arm_left=(deg(-2 * s), 0, 0)), 0.004 * s


def idle(t, i):
    rot, lift = breathe(t); rig.pose(**rot); R.location = (0, 0, lift)


def blink(face):
    return lambda t, i: (idle(t, i), face)[1]


# speech-like mouth pattern (his own face is the open grin)
TALK_MOUTH = [None, "mouth_mid", "mouth_closed", "mouth_mid", None, None, "mouth_mid", "mouth_closed",
              "mouth_closed", "mouth_mid", None, "mouth_mid", "mouth_closed", "mouth_mid", None, "mouth_mid",
              "mouth_closed", "mouth_closed", "mouth_mid", None, None, "mouth_mid", "mouth_closed", "mouth_mid"]


def talk(t, i):
    s = math.sin(2 * math.pi * 2 * t)
    hand = (0.29, 0.17 + 0.03 * s, 0.52 + 0.05 * s)                  # explaining, palm-up gesture
    rig.pose(arm_right=Aim(rig.hand_dir("arm_right", hand)),
             arm_left=(deg(-2 * s), 0, 0),
             body=(deg(1.5), 0, deg(2 * math.sin(2 * math.pi * t))),
             head=(deg(2.5 * math.sin(2 * math.pi * 3 * t)), 0, deg(3 * math.sin(2 * math.pi * t))))
    R.location = (0, 0, 0)
    return TALK_MOUTH[i % len(TALK_MOUTH)]


def clap(t, i):
    c = 0.5 - 0.5 * math.cos(2 * math.pi * 2 * t)                     # two claps per cycle
    c = c ** 1.6                                                          # hands meet sharply
    hand = lerp((0.17, 0.25, 0.66), (0.035, 0.25, 0.67), c)
    rig.pose(**hands(hand, body=(deg(2), 0, 0), head=(deg(-3 * c), 0, 0)))
    R.location = (0, 0, 0.006 * c)


def cheer(t, i):
    """Arms straight up beside his head with a happy wiggle, eyes squeezed shut in a grin, a small hop."""
    up = ss(0.0, 0.25, t) * (1 - ss(0.78, 1.0, t))
    wig = math.sin(2 * math.pi * 3 * t) * up
    def hand(sign):
        shoulder = rig.arm.data.bones["arm_right" if sign > 0 else "arm_left"].head_local
        top = shoulder + 0.307 * Vector((sign * (0.33 + 0.08 * wig), 0.12, 1.0)).normalized()
        return lerp(tuple(REST_HAND if sign > 0 else mirror(REST_HAND)), tuple(top), up)
    rig.pose(**hands(hand(1), hand(-1), head=(deg(-5 * up), 0, deg(4 * wig))))
    hop = 0.03 * math.sin(math.pi * min(1.0, max(0.0, (t - 0.08) / 0.30)))
    R.location = (0, 0, hop)
    return "closed" if up > 0.6 else None


def wave(t, i):
    s = math.sin(2 * math.pi * 2 * t)
    rot, lift = breathe(t)
    rot["arm_right"] = Aim(rig.hand_dir("arm_right", (0.31 + 0.06 * s, 0.06, 1.02)))
    rot["head"] = (0, 0, deg(4 * s))
    rig.pose(**rot); R.location = (0, 0, lift)


# ── seated, on his chair (the house): five facings so he always turns to the student ─────────
SEAT_YAWS = [-50, -25, 0, 25, 50]          # + turns him towards screen-right (camera yaw 180 looks at his face)
LAP_HAND = (0.15, 0.15, 0.36)              # hands resting on his thighs, near the knees
SIT_THIGH = 50                             # thighs angled 40 deg below level: shins reach down to the floor (not dangling)


def seated_base(yaw):
    """Sitting on the front of the seat: thighs sloping down, shins upright, feet on the floor; turned by yaw."""
    R.rotation_euler = (0, 0, math.radians(yaw))
    return dict(leg_right=(deg(SIT_THIGH), 0, 0), leg_left=(deg(SIT_THIGH), 0, 0),
                shin_right=(deg(-SIT_THIGH), 0, 0), shin_left=(deg(-SIT_THIGH), 0, 0))


def seat_idle(yaw):
    def fn(t, i):
        s_ = math.sin(2 * math.pi * t)
        rot = seated_base(yaw)
        sw = 0.05 * math.sin(2 * math.pi * t + 0.8)            # feet swing a little: a happy fidget
        rot["shin_right"] = (deg(-SIT_THIGH + 5 * math.sin(2 * math.pi * t)), 0, 0)
        rot["shin_left"] = (deg(-SIT_THIGH - 5 * math.sin(2 * math.pi * t)), 0, 0)
        rot.update(hands((LAP_HAND[0], LAP_HAND[1], LAP_HAND[2] + 0.004 * s_)))
        rot["body"] = (deg(1.0 * s_), 0, 0)
        rot["head"] = (deg(-1.2 * s_), 0, deg(2 * math.sin(2 * math.pi * t + 1)))
        rig.pose(**rot); R.location = (0, 0, 0.003 * s_)
        return None
    return fn


def seat_blink(yaw):
    idle = seat_idle(yaw)
    return lambda t, i: (idle(t, i), "closed")[1]


def seat_talk(yaw):
    def fn(t, i):
        s_ = math.sin(2 * math.pi * 2 * t)
        rot = seated_base(yaw)
        rot.update(hands(LAP_HAND))
        rot["arm_right"] = Aim(rig.hand_dir("arm_right", (0.27, 0.24 + 0.03 * s_, 0.58 + 0.05 * s_)))   # explaining
        rot["body"] = (deg(-1.5), 0, deg(2 * math.sin(2 * math.pi * t)))
        rot["head"] = (deg(2.5 * math.sin(2 * math.pi * 3 * t)), 0, deg(3 * math.sin(2 * math.pi * t)))
        rig.pose(**rot); R.location = (0, 0, 0)
        return TALK_MOUTH[i % len(TALK_MOUTH)]
    return fn


def seat_cheer(yaw):
    """Arms flung up beside his head, eyes squeezed shut in a grin, bouncing on the chair."""
    def fn(t, i):
        up = ss(0.0, 0.22, t) * (1 - ss(0.8, 1.0, t))
        wig = math.sin(2 * math.pi * 3 * t) * up
        def hand(sign):
            shoulder = rig.arm.data.bones["arm_right" if sign > 0 else "arm_left"].head_local
            top = shoulder + 0.307 * Vector((sign * (0.33 + 0.08 * wig), 0.12, 1.0)).normalized()
            lap = LAP_HAND if sign > 0 else mirror(LAP_HAND)
            return lerp(lap, tuple(top), up)
        rot = seated_base(yaw)
        rot.update(hands(hand(1), hand(-1)))
        rot["head"] = (deg(-5 * up), 0, deg(4 * wig))
        kick = 18 * up * math.sin(2 * math.pi * 2 * t)            # happy feet
        rot["shin_right"] = (deg(-SIT_THIGH + kick), 0, 0)
        rot["shin_left"] = (deg(-SIT_THIGH - kick), 0, 0)
        rig.pose(**rot)
        R.location = (0, 0, 0.018 * abs(math.sin(2 * math.pi * 2 * t)) * up)   # bouncing on the seat
        return "closed" if up > 0.6 else None
    return fn


def seated_sets():
    sets = []
    for yaw in SEAT_YAWS:
        tag = f"{'m' if yaw < 0 else 'p'}{abs(yaw):02d}"
        sets += [(f"seat_idle_{tag}", FRONT, seat_idle(yaw), 16, True),
                 (f"seat_blink_{tag}", FRONT, seat_blink(yaw), 8, True),
                 (f"seat_talk_{tag}", FRONT, seat_talk(yaw), 16, True),
                 (f"seat_cheer_{tag}", FRONT, seat_cheer(yaw), 20, False)]
    return sets


if MODE == "seatposetest":
    os.makedirs(OUT, exist_ok=True)
    for yaw in (-50, 0, 50):
        for name, fn, t in (("idle", seat_idle(yaw), 0.25), ("talk", seat_talk(yaw), 0.3), ("cheer", seat_cheer(yaw), 0.5)):
            R.location = (0, 0, 0); fn(t, 0)
            rig.place_camera(FRONT); rig.render(os.path.join(OUT, f"{name}_{yaw:+03d}.png"))
    R.rotation_euler = (0, 0, 0); seat_idle(0)(0.25, 0)
    rig.place_camera(90); rig.render(os.path.join(OUT, "idle_side.png"))
    sys.exit(0)

if MODE == "seated":
    only = os.environ.get("ONLY")
    rig.render_sets(OUT, seated_sets(), only.split(",") if only else None)
    sys.exit(0)

if MODE == "render":
    SETS = [
        ("idle", FRONT, idle, 32, True),
        ("blink_half", FRONT, blink("half"), 16, True),
        ("blink_closed", FRONT, blink("closed"), 16, True),
        ("talk", FRONT, talk, 24, True),
        ("clap", FRONT, clap, 24, True),
        ("cheer", FRONT, cheer, 24, False),
        ("wave", FRONT, wave, 24, True),
    ]
    only = os.environ.get("ONLY")
    rig.render_sets(OUT, SETS, only.split(",") if only else None)
