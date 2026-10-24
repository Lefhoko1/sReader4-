import bpy,json
from mathutils import Vector
print(json.dumps({'file':bpy.data.filepath,'collections':[(c.name,len(c.objects)) for c in bpy.data.collections],'objects':[(o.name,[round(v,2) for v in o.location],[round(v,2) for v in o.dimensions]) for o in bpy.context.scene.objects if o.type=='MESH'] }))
