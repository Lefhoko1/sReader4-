using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using SReader.Game.Library.Feel;
using SReader.Game.Library.Reading;

namespace SReader.Game.Library.EditorTools
{
    /// <summary>
    /// Builds the GL-0 greybox Reading Alcove (spec §7). Blockout-before-beauty:
    /// primitives + one warm light, wired to the feel rig and the page presenter,
    /// so the "tap a word → gold ink spreads" moment is playable before any Blender
    /// asset exists. Re-run to rebuild from scratch. Menu:
    /// <b>Tools ▸ Great Library ▸ Build Reading Alcove Greybox</b>.
    /// </summary>
    public static class ReadingAlcoveGreybox
    {
        const string ScenePath = "Assets/Scenes/SC_ReadingRoom.unity";
        const string InkMatPath = "Assets/Art/Materials/M_InkSpread.mat";

        // Bible Ch. 3.2 palette
        static readonly Color LibraryOak = new Color(0.431f, 0.290f, 0.180f); // #6E4A2E
        static readonly Color CoolGrey   = new Color(0.34f, 0.37f, 0.42f);
        static readonly Color AmbientCool = new Color(0.141f, 0.161f, 0.227f); // #24293A
        static readonly Color CandleWarm = new Color(1f, 0.72f, 0.42f);        // ~2700 K

        [MenuItem("Tools/Great Library/Build Reading Alcove Greybox")]
        public static void Build()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // ── environment (Bible Ch. 8.6: near-black cool ambient, precious warm light) ──
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = AmbientCool;

            var warm = new GameObject("WarmKey_CandleLight");
            var light = warm.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = CandleWarm;
            light.intensity = 4f;
            light.range = 5f;
            warm.transform.position = new Vector3(0.25f, 1.35f, -0.55f);   // on the reader's (-Z) side of the page

            // ── camera (frames the page; portrait Game view expected) ──
            var camGO = new GameObject("ReadingCamera");
            camGO.tag = "MainCamera";
            var cam = camGO.AddComponent<Camera>();
            camGO.AddComponent<AudioListener>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.08f, 0.09f, 0.13f);
            // TMP's readable face points -Z, so the reader sits on the -Z side looking toward +Z.
            cam.transform.position = new Vector3(0f, 1.1f, -1.55f);

            // ── greybox geometry ──
            Box("Floor", new Vector3(0f, -0.05f, 0f), new Vector3(6f, 0.1f, 6f), CoolGrey);
            Box("Wall_Back",  new Vector3(0f, 1.5f, 1.4f), new Vector3(4f, 3f, 0.1f), CoolGrey);   // backdrop behind the page (+Z)
            Box("Wall_Left",  new Vector3(-2f, 1.5f, 0f),   new Vector3(0.1f, 3f, 3f), CoolGrey);
            Box("Wall_Right", new Vector3(2f, 1.5f, 0f),    new Vector3(0.1f, 3f, 3f), CoolGrey);
            Box("Desk", new Vector3(0f, 0.39f, 0f), new Vector3(1.4f, 0.78f, 0.8f), LibraryOak);
            Box("Book_Base", new Vector3(0f, 0.80f, 0f), new Vector3(0.62f, 0.06f, 0.44f), LibraryOak);

            // ── the page (world-space TMP the presenter renders onto) ──
            var pageGO = new GameObject("ReadingPage");
            pageGO.transform.position = new Vector3(0f, 0.95f, 0.02f);
            pageGO.transform.rotation = Quaternion.Euler(15f, 0f, 0f);   // reclined toward the reader
            var tmp = pageGO.AddComponent<TextMeshPro>();
            tmp.text = "Reading Alcove — press Play, then tap a glowing word.";
            tmp.color = new Color(0.16f, 0.13f, 0.10f);
            tmp.alignment = TextAlignmentOptions.TopLeft;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 0.4f;
            tmp.fontSizeMax = 3.5f;
            tmp.rectTransform.sizeDelta = new Vector2(0.55f, 0.72f);

            cam.transform.LookAt(new Vector3(0f, 1.0f, 0f));

            // ── feel rig ──
            var inkMat = LoadOrCreateInkMaterial();
            var feelGO = new GameObject("GreatLibraryFeel");
            var director = feelGO.AddComponent<FeedbackDirector>();
            var burst = feelGO.AddComponent<RestorationBurst>();
            Wire(burst, "inkMaterial", inkMat);
            Wire(burst, "director", director);

            // ── presenter wiring ──
            var presenter = pageGO.AddComponent<ReadingPagePresenter>();
            Wire(presenter, "pageText", tmp);
            Wire(presenter, "director", director);
            Wire(presenter, "readingCamera", cam);

            // ── save ──
            Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            Selection.activeGameObject = pageGO;
            Debug.Log($"[Great Library] Built greybox Reading Alcove → {ScenePath}. Press Play, then tap a glowing word.");
        }

        static GameObject Box(string name, Vector3 pos, Vector3 size, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = pos;
            go.transform.localScale = size;
            var mr = go.GetComponent<MeshRenderer>();
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var mat = new Material(shader);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            mr.sharedMaterial = mat;
            return go;
        }

        static Material LoadOrCreateInkMaterial()
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(InkMatPath);
            if (mat != null) return mat;
            var shader = Shader.Find("Custom/InkSpread");
            if (shader == null) { Debug.LogWarning("[Great Library] Custom/InkSpread shader not found — ink will no-op."); return null; }
            Directory.CreateDirectory("Assets/Art/Materials");
            mat = new Material(shader) { name = "M_InkSpread" };
            AssetDatabase.CreateAsset(mat, InkMatPath);
            AssetDatabase.SaveAssets();
            return mat;
        }

        // Assign a private [SerializeField] field on a freshly-created component.
        static void Wire(Component target, string field, Object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(field);
            if (prop == null) { Debug.LogWarning($"[Great Library] '{field}' not found on {target.GetType().Name}."); return; }
            prop.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
