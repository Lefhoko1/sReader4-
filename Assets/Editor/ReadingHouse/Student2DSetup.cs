using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using SReader.Game.ReadingHouse;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// Stage B: puts the 2D student into the existing Reading House scene.
    ///   1. Sprite import settings for Student2D/Frames (one pivot between the
    ///      feet, one pixels-per-unit, mip-mapped, ASTC on Android).
    ///   2. The StudentSpriteSet asset (7 clips) and a URP sprite material.
    ///   3. A soft round ground-shadow texture + material.
    ///   4. A "Student 2D" object in SC_ReadingHouse_Integration that follows the
    ///      existing student. The 3D student is kept (hidden at runtime, toggle
    ///      "use2D" to bring it back). Re-running replaces only what this adds.
    /// </summary>
    public static class Student2DSetup
    {
        const string Root = ReadingHouseSceneBuilder.Root + "/Student2D";
        const string FramesDir = Root + "/Frames";
        internal const int CanvasH = 480, Baseline = 8, CharH = 448;
        internal const float CharWorldHeight = 2.45f;   // the 3D student is ~2.5 m from feet to hair top

        [MenuItem("Tools/sReader/Reading House/Set Up 2D Student")]
        public static void Run()
        {
            float ppu = CharH / CharWorldHeight;
            ImportFrames(FramesDir, ppu);
            var set = BuildSet();
            var spriteMat = SpriteMaterial();
            var shadowMat = ShadowMaterial();
            PatchScene(set, spriteMat, shadowMat);
            Debug.Log($"[Student2D] Set up: {ppu:0.#} px per unit, character {CharWorldHeight} m tall.");
        }

        /// <summary>Sprite import for a frames folder: one pivot between the feet, one pixels-per-unit (shared with the Encourager).</summary>
        internal static void ImportFrames(string framesDir, float ppu)
        {
            foreach (var path in Directory.GetFiles(framesDir, "*.png", SearchOption.AllDirectories).Select(p => p.Replace('\\', '/')))
            {
                var imp = (TextureImporter)AssetImporter.GetAtPath(path);
                if (imp == null) { AssetDatabase.ImportAsset(path); imp = (TextureImporter)AssetImporter.GetAtPath(path); }
                imp.textureType = TextureImporterType.Sprite;
                imp.spriteImportMode = SpriteImportMode.Single;
                var s = new TextureImporterSettings();
                imp.ReadTextureSettings(s);
                s.spriteAlignment = (int)SpriteAlignment.Custom;
                s.spritePivot = new Vector2(0.5f, Baseline / (float)CanvasH);
                s.spritePixelsPerUnit = ppu;
                s.spriteMeshType = SpriteMeshType.Tight;
                s.spriteGenerateFallbackPhysicsShape = false;
                imp.SetTextureSettings(s);
                imp.mipmapEnabled = true;                 // he's shown smaller than 448 px most of the time
                imp.alphaIsTransparency = true;
                imp.filterMode = FilterMode.Bilinear;
                imp.wrapMode = TextureWrapMode.Clamp;
                imp.textureCompression = TextureImporterCompression.Compressed;
                imp.SetPlatformTextureSettings(new TextureImporterPlatformSettings
                {
                    name = "Android", overridden = true, maxTextureSize = 512, format = TextureImporterFormat.ASTC_6x6,
                });
                imp.SaveAndReimport();
            }
        }

        /// <summary>Frames are numbered name_00, name_01, ... with no gaps.</summary>
        static int FrameCount(string name)
        {
            int n = 0;
            while (File.Exists($"{FramesDir}/{name}/{name}_{n:00}.png")) n++;
            return n;
        }

        static StudentSpriteSet BuildSet()
        {
            string path = Root + "/StudentSpriteSet.asset";
            var set = AssetDatabase.LoadAssetAtPath<StudentSpriteSet>(path);
            if (set == null) { set = ScriptableObject.CreateInstance<StudentSpriteSet>(); AssetDatabase.CreateAsset(set, path); }
            StudentSpriteSet.Clip C(string name, float fps, bool loop) => new StudentSpriteSet.Clip
            {
                name = name, fps = fps, loop = loop,
                frames = Enumerable.Range(0, FrameCount(name)).Select(i => AssetDatabase.LoadAssetAtPath<Sprite>($"{FramesDir}/{name}/{name}_{i:00}.png")).ToArray(),
            };
            // Frames rendered from the 3D model (render_sprites.py) are full cycles with the bob baked in and no blink frames.
            bool rendered3D = File.Exists($"{FramesDir}/RENDERED_3D.txt");
            set.idlesAreCycles = rendered3D;
            set.framesIncludeBob = rendered3D;
            set.idleFrontBlinkFrames = rendered3D ? new int[0] : new[] { 4, 5 };
            set.idleFront = C("idle_front", 6f, true);
            set.idleBack = C("idle_back", 5f, true);
            set.walkRight = C("walk_right", 11f, true);
            set.walkAway = C("walk_away", 10f, true);
            set.walkToward = C("walk_toward", 10f, true);
            set.turnToBack = C("turn_to_back", 16f, false);
            set.pointBack = C("point_back", 10f, false);
            // face expressions over the front idle (optional: only the 3D renders have them)
            StudentSpriteSet.Clip Opt(string name) => FrameCount(name) > 0 ? C(name, 8f, true) : null;
            set.blinkHalf = Opt("blink_half");
            set.blinkClosed = Opt("blink_closed");
            set.smileFront = Opt("smile_front");
            foreach (var clip in new[] { set.idleFront, set.idleBack, set.walkRight, set.walkAway, set.walkToward, set.turnToBack, set.pointBack })
                if (clip.frames.Any(f => f == null)) throw new System.InvalidOperationException($"Missing sprite frames for '{clip.name}'.");
            EditorUtility.SetDirty(set);
            AssetDatabase.SaveAssets();
            return set;
        }

        internal static Material SpriteMaterial()
        {
            string path = Root + "/M_Student2D.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default") ?? Shader.Find("Sprites/Default");
            if (mat == null) { mat = new Material(shader); AssetDatabase.CreateAsset(mat, path); }
            mat.shader = shader;
            return mat;
        }

        internal static Material ShadowMaterial()
        {
            string texPath = Root + "/T_StudentShadow.png";
            if (!File.Exists(texPath))
            {
                const int n = 64;
                var t = new Texture2D(n, n, TextureFormat.RGBA32, false);
                for (int y = 0; y < n; y++)
                    for (int x = 0; x < n; x++)
                    {
                        float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(n / 2f, n / 2f)) / (n / 2f);
                        float a = Mathf.Clamp01(1f - d); a = a * a * (3f - 2f * a);
                        t.SetPixel(x, y, new Color(0.18f, 0.12f, 0.08f, 0.55f * a));
                    }
                File.WriteAllBytes(texPath, t.EncodeToPNG());
                Object.DestroyImmediate(t);
                AssetDatabase.ImportAsset(texPath);
                var imp = (TextureImporter)AssetImporter.GetAtPath(texPath);
                imp.alphaIsTransparency = true; imp.mipmapEnabled = false; imp.wrapMode = TextureWrapMode.Clamp;
                imp.SaveAndReimport();
            }
            string path = Root + "/M_StudentShadow.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (mat == null) { mat = new Material(shader); AssetDatabase.CreateAsset(mat, path); }
            mat.shader = shader;
            mat.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(texPath));
            mat.SetColor("_BaseColor", Color.white);
            mat.SetFloat("_Surface", 1f); mat.SetFloat("_Blend", 0f);
            mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetFloat("_ZWrite", 0f);
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = 2999;   // just before other transparents, after the floor
            EditorUtility.SetDirty(mat);
            return mat;
        }

        static void PatchScene(StudentSpriteSet set, Material spriteMat, Material shadowMat)
        {
            var open = SceneManager.GetSceneByPath(ReadingHouseSceneBuilder.ScenePath);
            bool wasOpen = open.IsValid() && open.isLoaded;
            var scene = wasOpen ? open : EditorSceneManager.OpenScene(ReadingHouseSceneBuilder.ScenePath, OpenSceneMode.Additive);
            var previousActive = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(scene);   // new GameObjects go to the active scene (in batch mode that's an untitled one)
            try
            {
                foreach (var old in scene.GetRootGameObjects().Where(g => g.name == "Student 2D")) Object.DestroyImmediate(old);
                var presenter = scene.GetRootGameObjects().Select(g => g.GetComponentInChildren<ReadingHousePresenter>(true)).First(p => p != null);
                var student = presenter.studentMover.student;

                var go = new GameObject("Student 2D");
                go.transform.position = student.position;
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sharedMaterial = spriteMat;
                sr.sprite = set.idleFront.frames[0];
                sr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                sr.receiveShadows = false;

                var shadow = GameObject.CreatePrimitive(PrimitiveType.Quad);
                Object.DestroyImmediate(shadow.GetComponent<Collider>());
                shadow.name = "Ground Shadow";
                shadow.transform.SetParent(go.transform, false);
                shadow.transform.localScale = new Vector3(1.25f, 0.62f, 1f);
                var shr = shadow.GetComponent<MeshRenderer>();
                shr.sharedMaterial = shadowMat;
                shr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                var anim = go.AddComponent<StudentSpriteAnimator>();
                anim.student = student;
                anim.viewCamera = presenter.cameraRig.viewCamera;
                anim.mover = presenter.studentMover;
                anim.set = set;
                anim.spriteRenderer = sr;
                anim.shadow = shadow.transform;
                EditorSceneManager.SaveScene(scene);
            }
            finally
            {
                if (previousActive.IsValid() && previousActive.isLoaded && previousActive != scene) SceneManager.SetActiveScene(previousActive);
                if (!wasOpen) EditorSceneManager.CloseScene(scene, true);
            }
        }
    }
}
