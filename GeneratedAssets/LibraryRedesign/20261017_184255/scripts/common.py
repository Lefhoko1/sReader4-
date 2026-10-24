import bpy, math, random, json
from pathlib import Path
from mathutils import Vector, Matrix
ROOT=Path(__file__).resolve().parents[1]
S=bpy.context.scene
C={name:bpy.data.collections.get(name) for name in ['Architecture','Roof','Interior','Interactive Props','Lighting','Landscaping','Backup']}
def mat(name,h,metal=0,rough=.55,emission=0):
 m=bpy.data.materials.get('SR_'+name)
 if m:return m
 m=bpy.data.materials.new('SR_'+name);m.use_nodes=True
 rgb=tuple(int(h[i:i+2],16)/255 for i in (0,2,4))
 rgb=tuple(v/12.92 if v<=.04045 else ((v+.055)/1.055)**2.4 for v in rgb)
 p=m.node_tree.nodes.get('Principled BSDF');p.inputs['Base Color'].default_value=(*rgb,1);p.inputs['Metallic'].default_value=metal;p.inputs['Roughness'].default_value=rough
 if emission:p.inputs['Emission Color'].default_value=(*rgb,1);p.inputs['Emission Strength'].default_value=emission
 m.diffuse_color=(*rgb,1);return m
P={
 'plaster':mat('Parchment','E9DFC8'), 'stone':mat('Limestone','888C8D'),'stoneLight':mat('Carved_Limestone','C0B49C'),
 'slate':mat('Slate','232733',.15,.52),'slate2':mat('Slate_Light','343D4E',.1,.6),'gold':mat('Gold','D6AE58',.72,.32),
 'wood':mat('Walnut','513728'),'woodLight':mat('Oak','825D3D'),'floor':mat('Floor','604532'),'ink':mat('Ink','121419'),
 'paper':mat('Paper','F2EFE6'),'teal':mat('Deep_Teal','294D50'),'rug':mat('Midnight_Rug','263C44'),
 'glow':mat('Amber_Glass','FFD398',.05,.3,1.6),'magic':mat('Book_Magic','9EDDD5',.15,.25,2.3),
 'leaves':mat('Sage_Leaves','536D54'),'grass':mat('Moss','3B5044'),'soil':mat('Garden_Soil','302E27')}
def link(o,col):
 for c in list(o.users_collection):c.objects.unlink(o)
 C[col].objects.link(o)
 return o
def finish(o,name,material,col,bevel=0):
 o.name=name;link(o,col)
 if material and hasattr(o.data,'materials'):o.data.materials.clear();o.data.materials.append(P.get(material,material))
 if bevel and o.type=='MESH':
  b=o.modifiers.new('Soft crafted edges','BEVEL');b.width=bevel;b.segments=2
  b=o.modifiers.new('Weighted corner normals','WEIGHTED_NORMAL')
 return o
def box(name,loc,size,material,col='Architecture',bevel=.025):
 bpy.ops.mesh.primitive_cube_add(size=1,location=loc);o=bpy.context.object;o.scale=size;bpy.ops.object.transform_apply(location=False,rotation=False,scale=True)
 return finish(o,name,material,col,bevel)
def cyl(name,loc,r,depth,material,col='Architecture',vertices=32):
 bpy.ops.mesh.primitive_cylinder_add(vertices=vertices,radius=r,depth=depth,location=loc)
 return finish(bpy.context.object,name,material,col,.016)
def sphere(name,loc,scale,material,col='Interactive Props'):
 bpy.ops.mesh.primitive_uv_sphere_add(segments=20,ring_count=12,radius=1,location=loc);o=bpy.context.object;o.scale=scale
 for p in o.data.polygons:p.use_smooth=True
 return finish(o,name,material,col)
def mesh(name,verts,faces,material,col='Architecture'):
 me=bpy.data.meshes.new(name);me.from_pydata(verts,[],faces);me.update();o=bpy.data.objects.new(name,me);C[col].objects.link(o)
 if material:me.materials.append(P.get(material,material))
 return o
def prism(name,poly,depth,material,col='Architecture',y=0):
 # closed extrusion of a polygon expressed in x,z
 n=len(poly);verts=[(x,y-depth/2,z) for x,z in poly]+[(x,y+depth/2,z) for x,z in poly]
 faces=[tuple(reversed(range(n))),tuple(range(n,2*n))]+[(i,(i+1)%n,(i+1)%n+n,i+n) for i in range(n)]
 return mesh(name,verts,faces,material,col)
def line(name,points,r,material,col='Architecture',cyclic=False):
 cu=bpy.data.curves.new(name,'CURVE');cu.dimensions='3D';cu.resolution_u=1;cu.bevel_depth=r;cu.bevel_resolution=2
 sp=cu.splines.new('POLY');sp.points.add(len(points)-1)
 for p,co in zip(sp.points,points):p.co=(*co,1)
 sp.use_cyclic_u=cyclic;o=bpy.data.objects.new(name,cu);C[col].objects.link(o);cu.materials.append(P.get(material,material));return o
def text(name,body,loc,size,material,col='Architecture',rotation=(math.pi/2,0,0),align='CENTER'):
 cu=bpy.data.curves.new(name,'FONT');cu.body=body;cu.size=size;cu.align_x=align;cu.align_y='CENTER';cu.extrude=.0008;cu.resolution_u=6
 o=bpy.data.objects.new(name,cu);C[col].objects.link(o);o.location=loc;o.rotation_euler=rotation;cu.materials.append(P.get(material,material));return o
def archpoly(cx,bottom,spring,r,n=24):
 return [(cx-r,bottom),(cx+r,bottom)]+[(cx+r*math.cos(i*math.pi/n),spring+r*math.sin(i*math.pi/n)) for i in range(n+1)]
def archtrim(name,cx,y,bottom,spring,r,material='stoneLight',width=.09,col='Architecture'):
 pts=[(cx-r,y,bottom),(cx-r,y,spring)]+[(cx+r*math.cos(math.pi-i*math.pi/32),y,spring+r*math.sin(math.pi-i*math.pi/32)) for i in range(33)]+[(cx+r,y,bottom)]
 return line(name,pts,width,material,col)
def wall_opening(name,cx,y,width,bottom,spring,r,top):
 # panel between opening boundary and flat wall top; follows arch without a ceiling
 pts=[(cx-r,top),(cx+r,top)]+[(cx+r*math.cos(i*math.pi/24),spring+r*math.sin(i*math.pi/24)) for i in range(25)]
 prism(name+'_spandrel',pts,.28,'plaster',y=y)
 if bottom>.34:box(name+'_apron',(cx,y,(bottom+.34)/2),(width,.28,bottom-.34),'plaster')
def window(name,cx,y,bottom=1.25,spring=2.9,r=.62,side=False):
 objs0=set(bpy.data.objects)
 prism(name+'_glazing',archpoly(cx,bottom,spring,r),.025,'glow',y=y)
 archtrim(name+'_stone_arch',cx,y-.05,bottom-.05,spring,r+.09,width=.08)
 archtrim(name+'_inner_bronze',cx,y-.09,bottom,spring,r,width=.026,material='gold')
 box(name+'_sill',(cx,y-.04,bottom-.08),(2*r+.35,.48,.12),'stoneLight')
 line(name+'_mullion',[(cx,y-.065,bottom),(cx,y-.065,spring+r)],.022,'wood')
 for z in [bottom+.58,bottom+1.15]:line(name+'_transom',[(cx-r,y-.065,z),(cx+r,y-.065,z)],.022,'wood')
 # paired diamond lead details
 for x in [cx-r/2,cx+r/2]:line(name+'_lead',[(x,y-.07,2.0),(x+.12,y-.07,2.2),(x,y-.07,2.4),(x-.12,y-.07,2.2)],.012,'gold',cyclic=True)
 if side:
  for o in set(bpy.data.objects)-objs0:o.matrix_world=Matrix.Rotation(math.pi/2,4,'Z')@o.matrix_world

def camera(name,loc,target,lens=48,ortho=None):
 da=bpy.data.cameras.new(name);o=bpy.data.objects.new(name,da);C['Lighting'].objects.link(o);o.location=loc;o.rotation_euler=(Vector(target)-o.location).to_track_quat('-Z','Y').to_euler();da.lens=lens;da.clip_end=250
 if ortho:da.type='ORTHO';da.ortho_scale=ortho
 return o
def area(name,loc,target,power,color,size):
 da=bpy.data.lights.new(name,'AREA');da.energy=power;da.color=color;da.shape='DISK';da.size=size;o=bpy.data.objects.new(name,da);C['Lighting'].objects.link(o);o.location=loc;o.rotation_euler=(Vector(target)-o.location).to_track_quat('-Z','Y').to_euler();return o
