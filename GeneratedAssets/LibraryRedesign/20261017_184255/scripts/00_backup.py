import bpy,json
from pathlib import Path
ROOT=Path('C:\\Users\\11113930\\sReader4\\GeneratedAssets\\LibraryRedesign\\20261017_184255')
bpy.ops.wm.save_as_mainfile(filepath=str(ROOT/'sReader_Library_v00_original.blend'))
print(json.dumps({"backup":bpy.data.filepath,"objects":len(bpy.context.scene.objects)}))
