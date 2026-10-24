using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using SReader.Game.ReadingHouse;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// Seats the Encourager and the Boredom Zombie on their chairs in SC_ReadingHouse_Integration
    /// (frames rendered from their 3D models: EncouragerModel/render_encourager.py "seated",
    /// ZombieModel/render_zombie.py "seated"):
    ///   • the Encourager: five facings (he turns to the student), talk and cheer, and his candy
    ///     speech bubble with encouraging lines (EncouragerBehaviour);
    ///   • the zombie: bored on his chair, slow blinks, the odd yawn, and now and then he gets up,
    ///     sighs, and plops back down; slumps when a word is solved.
    /// Each sprite stands just in front of its chair's seat, lifted so the underside of the thighs rests
    /// on the seat. The original 3D characters are kept (hidden at runtime). Re-running replaces only
    /// what this adds (and removes the earlier standing versions).
    /// </summary>
    public static class SeatedCharactersSetup
    {
        const string ModelEncourager = "SR_CHARACTER_ENCOURAGER_OPT", ModelZombie = "SR_CHARACTER_ZOMBIE_OPT";
        const string EncDir = ReadingHouseSceneBuilder.Root + "/Encourager2D", ZomDir = ReadingHouseSceneBuilder.Root + "/Zombie2D";
        // Model heights (m) from the generator, and the height of the underside of the thighs when seated.
        const float EncHeight = 1.35f, EncSeatContact = 0.36f, ZomHeight = 1.25f, ZomSeatContact = 0.33f;
        static float World(float modelHeight) => Student2DSetup.CharWorldHeight * modelHeight / 1.2507f;

        [MenuItem("Tools/sReader/Reading House/Seat the Encourager and Zombie")]
        public static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new System.InvalidOperationException("Stop Play mode first — the scene can only be changed in Edit mode.");
            Student2DSetup.ImportFrames(EncDir + "/Seated", Student2DSetup.CharH / World(EncHeight));
            Student2DSetup.ImportFrames(ZomDir + "/Seated", Student2DSetup.CharH / World(ZomHeight));
            var enc = BuildEncourager();
            var zom = BuildZombie();
            foreach (var old in new[] { EncDir + "/EncouragerSpriteSet.asset", ZomDir + "/ZombieSpriteSet.asset" })
                if (AssetDatabase.LoadMainAssetAtPath(old) != null || File.Exists(old)) AssetDatabase.DeleteAsset(old);   // the standing versions
            PatchScene(enc, zom, Student2DSetup.SpriteMaterial());
        }

        static int FrameCount(string dir, string name)
        {
            int n = 0;
            while (File.Exists($"{dir}/{name}/{name}_{n:00}.png")) n++;
            return n;
        }

        static StudentSpriteSet.Clip Clip(string dir, string name, bool loop)
        {
            int n = FrameCount(dir, name);
            if (n == 0) throw new System.InvalidOperationException($"No frames for '{name}' in {dir}.");
            return new StudentSpriteSet.Clip
            {
                name = name, loop = loop,
                frames = Enumerable.Range(0, n).Select(i => AssetDatabase.LoadAssetAtPath<Sprite>($"{dir}/{name}/{name}_{i:00}.png")).ToArray(),
            };
        }

        static ActorSpriteSet Asset(string path)
        {
            var set = AssetDatabase.LoadAssetAtPath<ActorSpriteSet>(path);
            if (set == null) { set = ScriptableObject.CreateInstance<ActorSpriteSet>(); AssetDatabase.CreateAsset(set, path); }
            return set;
        }

        static ActorSpriteSet BuildEncourager()
        {
            string dir = EncDir + "/Seated";
            var set = Asset(EncDir + "/EncouragerSeated.asset");
            set.idleFps = 8f;
            set.facings = new[] { -50, -25, 0, 25, 50 }.Select(yaw =>
            {
                string tag = (yaw < 0 ? "m" : "p") + Mathf.Abs(yaw).ToString("00");
                return new ActorSpriteSet.Facing
                {
                    yaw = yaw,
                    idle = Clip(dir, $"seat_idle_{tag}", true),
                    blinkClosed = Clip(dir, $"seat_blink_{tag}", true),
                    actions = new[]
                    {
                        new ActorSpriteSet.Action { name = "talk", clip = Clip(dir, $"seat_talk_{tag}", true), fps = 12f },
                        new ActorSpriteSet.Action { name = "cheer", clip = Clip(dir, $"seat_cheer_{tag}", false), fps = 13f },
                    },
                };
            }).ToArray();
            EditorUtility.SetDirty(set);
            AssetDatabase.SaveAssets();
            return set;
        }

        static ActorSpriteSet BuildZombie()
        {
            string dir = ZomDir + "/Seated";
            var set = Asset(ZomDir + "/ZombieSeated.asset");
            set.idleFps = 7f;                                     // he breathes slowly
            set.facings = new[]
            {
                new ActorSpriteSet.Facing
                {
                    yaw = 0,
                    idle = Clip(dir, "seat_idle", true),
                    blinkHalf = Clip(dir, "seat_blink_half", true),
                    blinkClosed = Clip(dir, "seat_blink_closed", true),
                    actions = new[]
                    {
                        new ActorSpriteSet.Action { name = "yawn", clip = Clip(dir, "seat_yawn", false), fps = 11f },
                        new ActorSpriteSet.Action { name = "slump", clip = Clip(dir, "seat_slump", false), fps = 11f },
                        new ActorSpriteSet.Action { name = "stand", clip = Clip(dir, "stand_sigh_sit", false), fps = 11f },
                    },
                },
            };
            EditorUtility.SetDirty(set);
            AssetDatabase.SaveAssets();
            return set;
        }

        /// <summary>
        /// Just in front of the chair's seat (towards the camera), feet on the floor: the frames sit on the front
        /// edge of the seat with their thighs sloping down, so the sprite's feet (its pivot) stand on the floor.
        /// </summary>
        static Vector3 SeatSpot(Scene scene, Transform model, Camera cam, float modelHeight, float seatContact)
        {
            var seat = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Renderer>(true))
                .Where(r => r.name.StartsWith("Chair_Seat"))
                .OrderBy(r => Vector3.Distance(r.bounds.center, model.position)).FirstOrDefault()
                ?? throw new System.InvalidOperationException($"No chair seat near {model.name}.");
            var b = seat.bounds;
            var toCam = Vector3.ProjectOnPlane(cam.transform.position - b.center, Vector3.up).normalized;
            float front = Mathf.Abs(b.extents.x * toCam.x) + Mathf.Abs(b.extents.z * toCam.z);
            var p = b.center + toCam * (front + 0.04f);
            p.y = b.max.y - seatContact * World(modelHeight) / modelHeight;
            Physics.SyncTransforms();
            foreach (var hit in Physics.RaycastAll(new Vector3(p.x, b.min.y, p.z), Vector3.down, 3f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore).OrderBy(h => h.distance))
            {
                p.y = hit.point.y + 0.01f;                                   // the floor (or rug) in front of the chair
                break;
            }
            return p;
        }

        static void PatchScene(ActorSpriteSet enc, ActorSpriteSet zom, Material spriteMat)
        {
            var open = SceneManager.GetSceneByPath(ReadingHouseSceneBuilder.ScenePath);
            bool wasOpen = open.IsValid() && open.isLoaded;
            var scene = wasOpen ? open : EditorSceneManager.OpenScene(ReadingHouseSceneBuilder.ScenePath, OpenSceneMode.Additive);
            var previousActive = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(scene);
            try
            {
                foreach (var old in scene.GetRootGameObjects().Where(g => g.name is "Encourager 2D" or "Zombie 2D" or "Encourager (seated)" or "Zombie (seated)").ToArray())
                    Object.DestroyImmediate(old);
                var all = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();
                var presenter = all.Select(t => t.GetComponent<ReadingHousePresenter>()).First(p => p != null);
                var hud = all.Select(t => t.GetComponent<ReadingHouseHud>()).FirstOrDefault(h => h != null);
                var studentSprite = all.Select(t => t.GetComponent<StudentSpriteAnimator>()).FirstOrDefault(a => a != null);
                var cam = presenter.cameraRig.viewCamera;
                Transform Model(string n) => all.FirstOrDefault(t => t.name == n) ?? throw new System.InvalidOperationException($"{n} not found.");

                ActorSpriteAnimator Actor(string name, ActorSpriteSet set, Transform model, float h, float contact)
                {
                    var go = new GameObject(name);
                    go.transform.position = SeatSpot(scene, model, cam, h, contact);
                    var sr = go.AddComponent<SpriteRenderer>();
                    sr.sharedMaterial = spriteMat;
                    sr.sprite = set.facings[set.Nearest(0f)].idle.frames[0];
                    sr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    sr.receiveShadows = false;
                    var a = go.AddComponent<ActorSpriteAnimator>();
                    a.viewCamera = cam; a.set = set; a.spriteRenderer = sr; a.model3D = model;
                    return a;
                }

                // The Encourager: turns to the student, candy speech bubble.
                var encActor = Actor("Encourager (seated)", enc, Model(ModelEncourager), EncHeight, EncSeatContact);
                encActor.faceTarget = studentSprite != null ? studentSprite.transform : presenter.studentMover.student;
                encActor.blinkTime = 0.18f;
                var bubbleGo = new GameObject("Candy Speech Bubble");
                bubbleGo.transform.SetParent(encActor.transform, false);
                var doc = bubbleGo.AddComponent<UIDocument>();
                doc.panelSettings = hud != null ? hud.GetComponent<UIDocument>().panelSettings : null;
                doc.sortingOrder = 20;
                var bubble = bubbleGo.AddComponent<CandySpeechBubble>();
                bubble.viewCamera = cam; bubble.speaker = encActor.spriteRenderer;
                var behaviour = encActor.gameObject.AddComponent<EncouragerBehaviour>();
                behaviour.actor = encActor; behaviour.bubble = bubble;
                presenter.encourager = behaviour;

                // The zombie: bored; now and then a yawn, or he gets up, sighs and sits back down.
                var zomActor = Actor("Zombie (seated)", zom, Model(ModelZombie), ZomHeight, ZomSeatContact);
                zomActor.blinkTime = 0.45f;
                zomActor.idleQuirks = new[] { "yawn", "stand", "yawn" };
                zomActor.idleQuirkEvery = new Vector2(10f, 20f);
                presenter.zombie = zomActor;

                EditorUtility.SetDirty(presenter);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"[Seated] Encourager at {encActor.transform.position}, zombie at {zomActor.transform.position}.");
            }
            finally
            {
                if (previousActive.IsValid() && previousActive.isLoaded && previousActive != scene) SceneManager.SetActiveScene(previousActive);
                if (!wasOpen) EditorSceneManager.CloseScene(scene, true);
            }
        }
    }
}
