"""
Word Path assets: stepping stones (A/B/C), keyword stone (+gold ring & star),
and the floating Sky Board (sentence container) with row sockets.
Stylized flat-colour materials to match the island look.
"""
import bpy, bmesh, math, os, random
from mathutils import Vector, Matrix, noise

random.seed(7)

def reset():
    bpy.ops.wm.read_factory_settings(use_empty=True)

def link(o): bpy.context.scene.collection.objects.link(o); return o

def cmat(name, rgb, rough=0.85, emit=None, s=0.0):
    m = bpy.data.materials.get(name) or bpy.data.materials.new(name)
    m.use_nodes = True
    nd = m.node_tree.nodes["Principled BSDF"]
    nd.inputs["Base Color"].default_value = (*rgb, 1)
    nd.inputs["Roughness"].default_value = rough
    if emit:
        nd.inputs["Emission Color"].default_value = (*emit, 1)
        nd.inputs["Emission Strength"].default_value = s
    return m

def socket(parent, name, loc, rot=(0,0,0)):
    e = bpy.data.objects.new(name, None)
    e.empty_display_type='PLAIN_AXES'; e.empty_display_size=0.06
    link(e); e.parent=parent; e.location=Vector(loc)
    e.rotation_euler=rot
    return e

# ---------------------------------------------------------------- stones ---
def build_stone(name, seed, rx, ry, h, mat):
    rnd = random.Random(seed)
    me = bpy.data.meshes.new(name); ob = link(bpy.data.objects.new(name, me))
    bm = bmesh.new()
    bmesh.ops.create_icosphere(bm, subdivisions=3, radius=1.0)
    for v in bm.verts:
        v.co.x *= rx; v.co.y *= ry; v.co.z *= h
        # flatten top hard (stepping stone), keep under-belly rounded
        if v.co.z > h*0.35: v.co.z = h*0.35 + (v.co.z - h*0.35)*0.18
    off = Vector((rnd.uniform(0,9), rnd.uniform(0,9), rnd.uniform(0,9)))
    for v in bm.verts:
        n = noise.noise(v.co*2.0 + off)
        calm = max(0.0, min(1.0, (v.co.z)/ (h*0.35)))  # calm on top
        v.co += v.normal * n * 0.06 * (1.0 - 0.75*max(0,calm))
    minz = min(v.co.z for v in bm.verts)
    for v in bm.verts:
        v.co.z -= minz
        if v.co.z < 0.015: v.co.z = 0.0
    bm.normal_update()
    # UV: planar top
    uvl = bm.loops.layers.uv.new("UVMap")
    xs=[v.co.x for v in bm.verts]; ys=[v.co.y for v in bm.verts]
    minx,maxx,miny,maxy=min(xs),max(xs),min(ys),max(ys)
    for f in bm.faces:
        for l in f.loops:
            c=l.vert.co
            l[uvl].uv=((c.x-minx)/(maxx-minx),(c.y-miny)/(maxy-miny))
    bm.to_mesh(me); bm.free()
    me.polygons.foreach_set("use_smooth",[True]*len(me.polygons))
    me.materials.append(mat)
    topz = max(v.co.z for v in me.vertices)
    socket(ob, "SOCKET_Text", (0,0,topz+0.006), (math.radians(0),0,0))
    return ob, topz

def build_star(name, mat, r=0.14):
    me=bpy.data.meshes.new(name); ob=link(bpy.data.objects.new(name,me))
    bm=bmesh.new()
    pts=[]
    for i in range(10):
        a=math.pi/2 + i*math.pi/5
        rad=r if i%2==0 else r*0.45
        pts.append(bm.verts.new((math.cos(a)*rad, math.sin(a)*rad, 0)))
    f=bm.faces.new(pts)
    res=bmesh.ops.extrude_face_region(bm, geom=[f])
    verts=[g for g in res['geom'] if isinstance(g,bmesh.types.BMVert)]
    bmesh.ops.translate(bm, vec=(0,0,0.04), verts=verts)
    bm.normal_update(); bm.to_mesh(me); bm.free()
    me.materials.append(mat)
    return ob

def build_key_stone(stone_mat, gold):
    ob, topz = build_stone("SM_WordStone_Key", 99, 0.55, 0.5, 0.20, stone_mat)
    # gold halo ring hugging the stone
    ring = bpy.data.meshes.new("ring")
    rob = link(bpy.data.objects.new("SM_WordStone_Key_Ring", ring))
    bm = bmesh.new()
    res = bmesh.ops.create_cone(bm, cap_ends=False, segments=28,
                                radius1=0.60, radius2=0.56, depth=0.06)
    bm.to_mesh(ring); bm.free()
    ring.materials.append(gold)
    ring.polygons.foreach_set("use_smooth",[True]*len(ring.polygons))
    rob.parent = ob; rob.location=(0,0,0.05)
    star = build_star("SM_WordStone_Key_Star", gold)
    star.parent = ob; star.location=(0,0.14,topz+0.19)
    star.rotation_euler=(math.radians(70),0,0)
    socket(ob, "SOCKET_Card", (0.45,0,topz+0.55))
    return ob

# -------------------------------------------------------------- sky board --
def build_sky_board(wood, wood_dark, parch, parch_row, gold):
    me=bpy.data.meshes.new("SM_SkyBoard")
    ob=link(bpy.data.objects.new("SM_SkyBoard", me))
    bm=bmesh.new()
    def bx(size,center,mat_i):
        v=bmesh.ops.create_cube(bm,size=1.0)['verts']
        bmesh.ops.transform(bm,matrix=Matrix.Translation(Vector(center)) @
                            Matrix.Diagonal((*size,1)).to_4x4(),verts=v)
        for f in {f for vv in v for f in vv.link_faces}: f.material_index=mat_i
        return v
    W,H=2.4,1.7
    bx((W,0.10,H),(0,0,0),0)                              # wood backing
    bx((W-0.16,0.06,H-0.16),(0,-0.045,0),2)               # parchment face
    # frame planks
    bx((W+0.10,0.14,0.16),(0,0,H/2),1)                    # top
    bx((W+0.10,0.14,0.16),(0,0,-H/2),1)                   # bottom
    bx((0.16,0.14,H+0.10),(-W/2,0,0),1)                   # left
    bx((0.16,0.14,H+0.10),( W/2,0,0),1)                   # right
    # title plank + 5 row strips (rows get UI text via sockets)
    bx((W-0.5,0.05,0.24),(0,-0.075,H/2-0.28),1)
    rows=5; row_h=0.20; gap=0.055
    z0=H/2-0.28-0.24
    for i in range(rows):
        z=z0-0.10-i*(row_h+gap)
        bx((W-0.42,0.045,row_h),(0,-0.075,z),3)
    # small gold corner studs
    for sx in (-1,1):
        for sz in (-1,1):
            bx((0.09,0.16,0.09),(sx*(W/2),0,sz*(H/2)),4)
    bm.to_mesh(me); bm.free()
    for m in (wood, wood_dark, parch, parch_row, gold):
        me.materials.append(m)
    # sockets: title + rows (front face -Y)
    socket(ob,"SOCKET_Title",(0,-0.11,H/2-0.28))
    for i in range(rows):
        z=z0-0.10-i*(row_h+gap)
        socket(ob,f"SOCKET_Row_{i+1}",(0,-0.11,z))
    socket(ob,"SOCKET_Hang",(0,0,H/2+0.05))
    return ob

# ------------------------------------------------------------------ export -
def export(ob, out):
    for o in bpy.context.selected_objects: o.select_set(False)
    ob.select_set(True)
    for c in ob.children_recursive: c.select_set(True)
    bpy.context.view_layer.objects.active=ob
    mods=[]
    for o in [ob]+[c for c in ob.children_recursive if c.type=='MESH']:
        mods.append((o,o.modifiers.new("Tri",'TRIANGULATE').name))
    bpy.ops.export_scene.fbx(filepath=os.path.join(out,ob.name+".fbx"),
        use_selection=True, axis_forward='-Z', axis_up='Y',
        apply_scale_options='FBX_SCALE_UNITS', use_mesh_modifiers=True,
        mesh_smooth_type='OFF', use_tspace=False, add_leaf_bones=False,
        bake_space_transform=True, object_types={'MESH','EMPTY'})
    for o,n in mods: o.modifiers.remove(o.modifiers[n])

reset()
stone = cmat("M_WordStone", (0.86,0.82,0.72), 0.9)
stone_warm = cmat("M_WordStone_Warm", (0.88,0.80,0.62), 0.9)
gold = cmat("M_KeyGold", (0.95,0.72,0.30), 0.4,
            emit=(1.0,0.75,0.30), s=2.0)
wood = cmat("M_SignWood", (0.55,0.38,0.20), 0.85)
wood_dark = cmat("M_SignWoodDark", (0.42,0.28,0.15), 0.85)
parch = cmat("M_SignParchment", (0.93,0.87,0.72), 0.9)
parch_row = cmat("M_SignRow", (0.97,0.93,0.82), 0.9)

out="/home/claude/wordstones/out"; os.makedirs(out,exist_ok=True)
tot=0; x=0
for i,(nm,sd,rx,ry,h,mt) in enumerate([
    ("SM_WordStone_A",1,0.48,0.44,0.16,stone),
    ("SM_WordStone_B",2,0.52,0.40,0.15,stone_warm),
    ("SM_WordStone_C",3,0.44,0.46,0.17,stone)]):
    ob,_=build_stone(nm,sd,rx,ry,h,mt); export(ob,out)
    tris=sum(len(p.vertices)-2 for p in ob.data.polygons); tot+=tris
    ob.location.x=x; x+=1.3
    print(f"  {nm:22s} {tris} tris")
key=build_key_stone(stone, gold); export(key,out)
kt=sum(len(p.vertices)-2 for p in key.data.polygons)+sum(
    sum(len(p.vertices)-2 for p in c.data.polygons)
    for c in key.children_recursive if c.type=='MESH')
print(f"  SM_WordStone_Key       {kt} tris"); key.location.x=x; x+=1.6
board=build_sky_board(wood,wood_dark,parch,parch_row,gold); export(board,out)
bt=sum(len(p.vertices)-2 for p in board.data.polygons)
print(f"  SM_SkyBoard            {bt} tris")
board.location=(x+1.2,0,1.2)
bpy.ops.wm.save_as_mainfile(filepath=os.path.join(out,"wordstones.blend"))
print("TOTAL", tot+kt+bt)
