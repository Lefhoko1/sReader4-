import bpy,json,math
from pathlib import Path
from mathutils import Vector
ROOT=Path(__file__).resolve().parents[1]
out=ROOT/'unity';out.mkdir(exist_ok=True)
roof=bpy.data.collections['Roof'];old=(roof.hide_viewport,roof.hide_render)
roof.hide_viewport=False;roof.hide_render=False;bpy.context.view_layer.update()
dg=bpy.context.evaluated_depsgraph_get()
def unity(v):return [round(v[0],7),round(v[2],7),round(v[1],7)]
def srgb(v):return 12.92*v if v<=.0031308 else 1.055*v**(1/2.4)-.055
materials={};objects=[]
try:
 for colname in ['Architecture','Roof','Interior','Interactive Props','Landscaping']:
  for ob in sorted(bpy.data.collections[colname].objects,key=lambda o:o.name):
   if ob.type not in {'MESH','CURVE','FONT','SURFACE','META'}:continue
   ev=ob.evaluated_get(dg);me=ev.to_mesh(preserve_all_data_layers=True,depsgraph=dg)
   if not me or not me.polygons:ev.to_mesh_clear();continue
   me.calc_loop_triangles();normal=ob.matrix_world.to_3x3().inverted().transposed()
   pivot=sum([ob.matrix_world@Vector(v) for v in ob.bound_box],Vector())/8
   verts=[];normals=[];uv=[];lookup={};sub=[[] for _ in range(max(1,len(me.materials)))];uvlayer=me.uv_layers.active
   for tri in me.loop_triangles:
    ix=[]
    for li in tri.loops:
     vi=me.loops[li].vertex_index;n=(normal@me.corner_normals[li].vector).normalized();t=uvlayer.data[li].uv if uvlayer else (0,0)
     key=(vi,tuple(round(x,5) for x in n),tuple(round(x,5) for x in t))
     if key not in lookup:
      lookup[key]=len(verts)//3;verts+=unity(ob.matrix_world@me.vertices[vi].co-pivot);normals+=unity(n);uv.extend([round(t[0],6),round(t[1],6)])
     ix.append(lookup[key])
    sub[min(tri.material_index,len(sub)-1)]+=[ix[0],ix[2],ix[1]]
   names=[]
   for m in me.materials:
    if not m:continue
    names.append(m.name)
    if m.name not in materials:
     p=m.node_tree.nodes.get('Principled BSDF') if m.use_nodes else None
     base=list(p.inputs['Base Color'].default_value) if p else list(m.diffuse_color)
     em=list(p.inputs['Emission Color'].default_value)[:3] if p else [0,0,0]
     es=p.inputs['Emission Strength'].default_value if p else 0
     materials[m.name]={'name':m.name,'color':[round(srgb(x),6) for x in base[:3]]+[base[3]],'metallic':p.inputs['Metallic'].default_value if p else 0,'roughness':p.inputs['Roughness'].default_value if p else .6,'emission':[round(srgb(x),6) for x in em],'emissionStrength':es}
   if not names:raise RuntimeError('Missing material: '+ob.name)
   objects.append({'name':ob.name,'collection':colname,'position':unity(pivot),'vertices':verts,'normals':normals,'uv':uv,'materials':names,'submeshes':[{'indices':i} for i in sub]})
   ev.to_mesh_clear()
 cameras=[]
 for name in ['SR_Camera_Interior','SR_Camera_Exterior','SR_Camera_Hero']:
  ob=bpy.data.objects[name];da=ob.data
  cameras.append({'name':name,'position':unity(ob.location),'forward':unity(ob.matrix_world.to_3x3()@Vector((0,0,-1))),'up':unity(ob.matrix_world.to_3x3()@Vector((0,1,0))),'orthographic':da.type=='ORTHO','orthoSize':da.ortho_scale*3/8,'fieldOfView':math.degrees(2*math.atan(math.tan(da.angle_x/2)*.75))})
 data={'source':bpy.data.filepath,'coordinateSystem':'Unity Y-up; source world geometry baked, winding reversed for handedness','materials':list(materials.values()),'objects':objects,'cameras':cameras}
 (out/'library_meshes.json').write_text(json.dumps(data,separators=(',',':')),encoding='utf-8')
 print(json.dumps({'export':str(out/'library_meshes.json'),'objects':len(objects),'materials':len(materials),'triangles':sum(len(sm['indices'])//3 for o in objects for sm in o['submeshes']),'bytes':(out/'library_meshes.json').stat().st_size}))
finally:
 roof.hide_viewport,roof.hide_render=old
 bpy.context.view_layer.update()
