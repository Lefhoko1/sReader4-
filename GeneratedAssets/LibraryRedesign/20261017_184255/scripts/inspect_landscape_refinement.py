import bpy,json
from mathutils import Vector
names=['SR_Tray_Gold_Lip','SR_Tray_Cyan_Inset','SR_Tray_Front_Orange_Rail','SR_Console_Sculpted_Base','SR_DISPLAY_MAIN','SR_STONE_PLACEMENT_REGION','SR_STONE_PREFAB_SOURCE']
r={'file':bpy.data.filepath,'objects':{n:{'pos':list(bpy.data.objects[n].location),'dimensions':list(bpy.data.objects[n].dimensions)} for n in names if n in bpy.data.objects},'collections':[(c.name,len(c.objects),c.hide_render) for c in bpy.data.collections if c.name.startswith('SR_')],'cameras':[(o.name,list(o.location)) for o in bpy.context.scene.objects if o.type=='CAMERA'],'characters':[(o.name,list(o.location),list(o.scale)) for o in bpy.data.objects if o.name.startswith('SR_CHARACTER')],'preview_fonts':[(o.name,o.data.body) for o in bpy.data.collections['SR_Demo'].objects if o.type=='FONT']}
print(json.dumps(r))
