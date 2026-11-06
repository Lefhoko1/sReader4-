# Art sources

Editable Blender and model sources for sReader that used to live only on the
development PC, outside the project. Copied here on 2026-11-06 so they are
backed up with the repo. This folder is outside `Assets/`, so Unity does not
import it: the models the game uses are already exported into `Assets/Art/`.

| Folder | Came from | What it is |
|--------|-----------|------------|
| `Downloads/Hero_Characters_FBX/` | `Downloads\Hero_Characters_FBX` | `hero_source.blend` + `hero_gen.py`: book spirit, owl and hero props (book, rune door, fountain) |
| `Downloads/StoryBook_v2_FBX/` | `Downloads\StoryBook_v2_FBX` | `storybook2.blend` + `book2_gen.py`: the storybook model and its textures |
| `Downloads/sReader_Storybook_House_Package/` | `Downloads\sReader_Storybook_House_Package` | Storybook house: `build_house.py` and the exported `.glb` |
| `Downloads/sReader_Magical_Library_House.glb` | `Downloads` | Magical library house export |
| `Downloads/X Bot.fbx` | `Downloads` | X Bot character model |
| `Documents_sreader/BlenderFiles/` | `Documents\mine\sreader\BlenderFiles` | Word stones (`wordstones.blend`, `wordstone_gen.py`), island / kit / storybook FBX zips, early Unity setup scripts, `GAME_VISION.md` |
| `Documents_sreader/newone/` | `Documents\mine\sreader\newone` | Early experiments: `TreeReal.blend`, castle and house trials, `gamedesign.docx` |

Other Blender sources already in the repo are under `GeneratedAssets/`
(`ReadingDashboard/`, `LibraryRedesign/`, `MagicalClassroom/`), including the
Student, Encourager and Zombie character `.blend` files.

Open the `.blend` files with Blender 5.0. Git LFS is needed to get the real
files: without it, models and pictures download as small pointer files.
