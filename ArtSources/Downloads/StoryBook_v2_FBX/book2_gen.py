"""
SM_StoryBook v2 — hero-book construction, candy palette, painted textures,
ruled sentence lines, flip page, same socket names as StoryBook.cs expects.
"""
import bpy, bmesh, math, os
from mathutils import Vector, Matrix, Euler

TEX=os.path.abspath("tex")

def reset(): bpy.ops.wm.read_factory_settings(use_empty=True)
def link(o): bpy.context.scene.collection.objects.link(o); return o

def cmat(name, rgb, rough=0.5, emit=None, s=0.0, tex=None):
    m=bpy.data.materials.new(name); m.use_nodes=True
    m.diffuse_color=(*rgb,1)                    # workbench 'MATERIAL' view fix
    nd=m.node_tree.nodes["Principled BSDF"]
    nd.inputs["Base Color"].default_value=(*rgb,1)
    nd.inputs["Roughness"].default_value=rough
    if emit:
        nd.inputs["Emission Color"].default_value=(*emit,1)
        nd.inputs["Emission Strength"].default_value=s
    if tex:
        t=m.node_tree.nodes.new("ShaderNodeTexImage")
        t.image=bpy.data.images.load(os.path.join(TEX,tex))
        m.node_tree.links.new(t.outputs["Color"],nd.inputs["Base Color"])
        m.node_tree.nodes.active=t; t.select=True
    return m

def socket(parent,name,loc,rot=(0,0,0)):
    e=bpy.data.objects.new(name,None); e.empty_display_type='PLAIN_AXES'
    e.empty_display_size=0.04; link(e); e.parent=parent
    e.location=Vector(loc); e.rotation_euler=Euler(rot); return e

def page_surface(name, side, mat, w=0.34, h=0.46, segs=12, lift=0.0,
                 origin_at_spine=False):
    me=bpy.data.meshes.new(name); ob=link(bpy.data.objects.new(name,me))
    bm=bmesh.new(); rows=[]
    for j in range(segs+1):
        row=[]
        for i in range(segs+1):
            u=i/segs; v=j/segs
            x=side*(0.045+u*w)
            y=(v-0.5)*h
            near=1.0-u
            z=lift+0.014*math.sin(u*math.pi)-0.026*near*near
            row.append(bm.verts.new((x,y,z)))
        rows.append(row)
    uvl=bm.loops.layers.uv.new("UVMap")
    for j in range(segs):
        for i in range(segs):
            a,b,c,d=rows[j][i],rows[j][i+1],rows[j+1][i+1],rows[j+1][i]
            f=bm.faces.new((a,b,c,d) if side>0 else (a,d,c,b))
            for l in f.loops:
                lx=abs(l.vert.co.x)-0.045
                l[uvl].uv=(lx/w,(l.vert.co.y/h)+0.5)
    bm.normal_update(); bm.to_mesh(me); bm.free()
    me.polygons.foreach_set("use_smooth",[True]*len(me.polygons))
    me.materials.append(mat)
    return ob

def build():
    grapeC = cmat("M_Book_CoverL",(0.49,0.35,0.78),0.42,tex="T_Book_CoverL.png")
    pinkC  = cmat("M_Book_CoverR",(1.0,0.50,0.66),0.42,tex="T_Book_CoverR.png")
    gold   = cmat("M_Book_Gold",(1.0,0.80,0.35),0.30,emit=(1,0.75,0.3),s=0.8)
    pagemat= cmat("M_Book_Page",(1.0,0.98,0.93),0.75,tex="T_Book_Page.png")
    pedge  = cmat("M_Book_PageEdge",(0.99,0.94,0.84),0.8)
    ribbon = cmat("M_Book_Ribbon",(1.0,0.42,0.52),0.5)
    mint   = cmat("M_Book_Mint",(0.45,0.90,0.78),0.4,emit=(0.4,0.9,0.78),s=0.6)
    gem    = cmat("M_Book_Gem",(0.55,0.45,1.0),0.15,emit=(0.6,0.45,1.0),s=2.5)
    MATS=[grapeC,pinkC,gold,pedge,ribbon,mint,gem]
    MI={m.name:i for i,m in enumerate(MATS)}

    me=bpy.data.meshes.new("SM_StoryBook"); ob=link(bpy.data.objects.new("SM_StoryBook",me))
    bm=bmesh.new()
    def bx(size,center,mi):
        v=bmesh.ops.create_cube(bm,size=1.0)['verts']
        bmesh.ops.transform(bm,matrix=Matrix.Translation(Vector(center)) @
                            Matrix.Diagonal((*size,1)).to_4x4(),verts=v)
        for f in {f for vv in v for f in vv.link_faces}: f.material_index=mi
        return v
    def tilt(verts, side, ang):
        piv=(Matrix.Translation((0,0,-0.030)) @
             Matrix.Rotation(-side*math.radians(ang),4,'Y') @
             Matrix.Translation((0,0,0.030)))
        bmesh.ops.transform(bm,matrix=piv,verts=verts)

    CW,CH=0.42,0.55
    uvl=bm.loops.layers.uv.new("UVMap")
    for side,mi in ((-1,MI["M_Book_CoverL"]),(1,MI["M_Book_CoverR"])):
        c=bx((CW,CH,0.026),(side*(CW/2+0.015),0,-0.040),mi)
        # UV the cover top face 0-1 so the painted design shows
        for f in {f for vv in c for f in vv.link_faces}:
            if f.normal.z>0.5 or f.normal.z<-0.5:
                for l in f.loops:
                    lx=(l.vert.co.x - side*0.015)/ (CW) * side + 0.5
                    l[uvl].uv=(lx,(l.vert.co.y/CH)+0.5)
        g=bx((CW+0.012,CH+0.012,0.008),(side*(CW/2+0.015),0,-0.055),MI["M_Book_Gold"])
        tilt(c,side,7); tilt(g,side,7)
        # stepped cream page stack (the hero-book look)
        for k,(w2,h2) in enumerate([(CW-0.05,0.026),(CW-0.09,0.020),(CW-0.13,0.015)]):
            p=bx((w2,CH-0.05-k*0.012,h2),
                 (side*(0.045+w2/2),0,-0.028+sum(x[1] for x in
                  [(0,0.026),(0,0.020)][:k])+h2/2),MI["M_Book_PageEdge"])
            tilt(p,side,5)
    # spine roll + gem
    sp=bmesh.ops.create_cone(bm,cap_ends=True,segments=12,
                             radius1=0.040,radius2=0.040,depth=CH)
    bmesh.ops.transform(bm,matrix=Matrix.Translation((0,0,-0.046)) @
                        Matrix.Rotation(math.radians(90),4,'X'),verts=sp['verts'])
    for f in {f for vv in sp['verts'] for f in vv.link_faces}:
        f.material_index=MI["M_Book_Gold"]
    gm=bmesh.ops.create_icosphere(bm,subdivisions=1,radius=0.020)
    bmesh.ops.transform(bm,matrix=Matrix.Translation((0,-CH/2-0.01,-0.040)),
                        verts=gm['verts'])
    for f in {f for vv in gm['verts'] for f in vv.link_faces}:
        f.material_index=MI["M_Book_Gem"]
    # mint/gold page tabs on right edge
    for i,ty in enumerate((0.16,0.05,-0.06,-0.17)):
        bx((0.030,0.055,0.010),(CW+0.035,ty,-0.020),
           MI["M_Book_Mint"] if i%2 else MI["M_Book_Gold"])
    bm.to_mesh(me); bm.free()
    for m in MATS: me.materials.append(m)
    me.polygons.foreach_set("use_smooth",[True]*len(me.polygons))
    # gentle bevel for quality
    bpy.context.view_layer.objects.active=ob
    for o in bpy.context.selected_objects: o.select_set(False)
    ob.select_set(True)
    bv=ob.modifiers.new("Bev",'BEVEL'); bv.width=0.006; bv.segments=2
    bv.limit_method='ANGLE'; bv.angle_limit=math.radians(50)
    bpy.ops.object.modifier_apply(modifier=bv.name)

    # page spreads (ruled texture) + flip page
    pl=page_surface("SM_StoryBook_PageLeft",-1,pagemat); pl.parent=ob
    pr=page_surface("SM_StoryBook_PageRight",1,pagemat); pr.parent=ob
    fp=page_surface("SM_StoryBook_FlipPage",1,pagemat,lift=0.004); fp.parent=ob

    # ribbon
    rme=bpy.data.meshes.new("rib"); rob=link(bpy.data.objects.new("SM_StoryBook_Ribbon",rme))
    rb=bmesh.new()
    v=bmesh.ops.create_cube(rb,size=1.0)['verts']
    bmesh.ops.transform(rb,matrix=Matrix.Translation((0.09,-0.34,-0.010)) @
                        Euler((math.radians(12),0,math.radians(4))).to_matrix().to_4x4() @
                        Matrix.Diagonal((0.028,0.20,0.004,1)).to_4x4(),verts=v)
    rb.to_mesh(rme); rb.free(); rme.materials.append(ribbon); rob.parent=ob

    # sockets — SAME NAMES StoryBook.cs uses (script needs no changes)
    socket(ob,"SOCKET_Title",(0,0.225,0.012))
    socket(ob,"SOCKET_LaunchPoint",(0,0,0.10))
    socket(ob,"SOCKET_SpineGem",(0,-CH/2-0.01,-0.040))
    socket(ob,"SOCKET_NextPage",( CW+0.04,-0.24,0.006))
    socket(ob,"SOCKET_PrevPage",(-CW-0.04,-0.24,0.006))
    # line sockets aligned to the painted ruled lines (v .80.. -> y=(v-.5)*h)
    h=0.46
    for i,v in enumerate((0.80,0.68,0.56,0.44,0.32,0.20)):
        y=(v-0.5)*h
        socket(ob,f"SOCKET_LineL_{i+1}",(-0.215,y,0.020))
        socket(ob,f"SOCKET_LineR_{i+1}",( 0.215,y,0.020))
    return ob

def export(ob,out):
    os.makedirs(out,exist_ok=True)
    for o in bpy.context.selected_objects: o.select_set(False)
    ob.select_set(True)
    for c in ob.children_recursive: c.select_set(True)
    bpy.context.view_layer.objects.active=ob
    mods=[]
    for o in [ob]+[c for c in ob.children_recursive if c.type=='MESH']:
        mods.append((o,o.modifiers.new("Tri",'TRIANGULATE').name))
    bpy.ops.export_scene.fbx(filepath=os.path.join(out,"SM_StoryBook.fbx"),
        use_selection=True,axis_forward='-Z',axis_up='Y',
        apply_scale_options='FBX_SCALE_UNITS',use_mesh_modifiers=True,
        mesh_smooth_type='OFF',use_tspace=False,add_leaf_bones=False,
        bake_space_transform=True,object_types={'MESH','EMPTY'},
        path_mode='COPY',embed_textures=True)
    for o,n in mods: o.modifiers.remove(o.modifiers[n])

reset()
book=build()
export(book,"/home/claude/book2/out")
tris=sum(len(p.vertices)-2 for p in book.data.polygons)+sum(
    sum(len(p.vertices)-2 for p in c.data.polygons)
    for c in book.children_recursive if c.type=='MESH')
bpy.ops.wm.save_as_mainfile(filepath="/home/claude/book2/out/storybook2.blend")
print("TRIS",tris)
