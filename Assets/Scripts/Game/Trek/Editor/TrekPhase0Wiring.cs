using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.SceneManagement;
using SReader.UI.Navigation;
using SReader.UI.Views.Home;

namespace SReader.Game.Trek.EditorTools
{
    /// <summary>
    /// One-click Phase 0 scene wiring for Story Trek. Creates the TrekPage
    /// GameObject (UIDocument + <see cref="TrekScenePage"/>), copies a PanelSettings
    /// from an existing page, and assigns every serialized reference
    /// (TrekScenePage.navigation, NavigationManager.trekPage,
    /// StudentHomeController.trekPage). Run it with your main scene open, then save.
    ///
    /// Editor-only convenience — safe to delete once the scene is wired.
    /// </summary>
    public static class TrekPhase0Wiring
    {
        [MenuItem("Tools/Story Trek/Wire Phase 0 Scene")]
        public static void Wire()
        {
            var nav = FindInScene<NavigationManager>();
            if (nav == null)
            {
                EditorUtility.DisplayDialog("Story Trek",
                    "No NavigationManager found in the open scene.\n\nOpen your main scene (Assets/Scenes/SampleScene.unity) and run this again.",
                    "OK");
                return;
            }

            // Reuse an existing TrekPage if one is already there; else create it.
            var trek = FindInScene<TrekScenePage>();
            GameObject trekGo;
            if (trek != null)
            {
                trekGo = trek.gameObject;
            }
            else
            {
                trekGo = new GameObject("TrekPage");
                var home = FindInScene<StudentHomeController>();
                if (home != null && home.transform.parent != null)
                    trekGo.transform.SetParent(home.transform.parent, false);   // sit beside the other pages
                Undo.RegisterCreatedObjectUndo(trekGo, "Create TrekPage");
                trek = trekGo.AddComponent<TrekScenePage>();   // auto-adds the required UIDocument
            }

            // UIDocument + PanelSettings (copy from any existing page, else any project asset).
            var doc = trekGo.GetComponent<UIDocument>();
            if (doc == null) doc = trekGo.AddComponent<UIDocument>();
            if (doc.panelSettings == null)
            {
                var panel = FindExistingPanelSettings(doc);
                if (panel != null)
                {
                    doc.panelSettings = panel;
                    EditorUtility.SetDirty(doc);
                }
                else
                {
                    Debug.LogWarning("[Story Trek] No PanelSettings found. Assign one to the TrekPage's UIDocument manually.");
                }
            }

            // Wire the three serialized references (private [SerializeField] — set via SerializedObject).
            SetRef(trek, "navigation", nav);
            SetRef(nav, "trekPage", trekGo);

            var homeCtrl = FindInScene<StudentHomeController>();
            if (homeCtrl != null) SetRef(homeCtrl, "trekPage", trek);

            // Pages start inactive; NavigationManager activates them on demand.
            trekGo.SetActive(false);

            EditorUtility.SetDirty(trekGo);
            EditorUtility.SetDirty(nav);
            if (homeCtrl != null) EditorUtility.SetDirty(homeCtrl);
            EditorSceneManager.MarkSceneDirty(trekGo.scene);

            var missingHome = homeCtrl == null
                ? "\n\nNOTE: no StudentHomeController found in this scene — assign its 'Trek Page' field manually if the student home lives elsewhere."
                : "";

            Debug.Log($"[Story Trek] Phase 0 wiring complete on scene '{trekGo.scene.name}'. Save the scene (Ctrl+S).");
            EditorUtility.DisplayDialog("Story Trek",
                $"Phase 0 scene wiring complete on '{trekGo.scene.name}'.\n\n" +
                "• Created/updated TrekPage (UIDocument + TrekScenePage)\n" +
                "• Wired NavigationManager.trekPage\n" +
                (homeCtrl != null ? "• Wired StudentHomeController.trekPage\n" : "") +
                "\nNow SAVE the scene (Ctrl+S)." + missingHome,
                "OK");

            Selection.activeGameObject = trekGo;
        }

        static PanelSettings FindExistingPanelSettings(UIDocument self)
        {
            // Prefer a PanelSettings already used by another page in the scene, so
            // the Trek renders at the same scale/sort as the rest of the UI.
            var fromScene = Resources.FindObjectsOfTypeAll<UIDocument>()
                .Where(d => d != self && !EditorUtility.IsPersistent(d) && d.panelSettings != null)
                .Select(d => d.panelSettings)
                .FirstOrDefault();
            if (fromScene != null) return fromScene;

            // Otherwise, any PanelSettings asset in the project.
            var guid = AssetDatabase.FindAssets("t:PanelSettings").FirstOrDefault();
            return string.IsNullOrEmpty(guid)
                ? null
                : AssetDatabase.LoadAssetAtPath<PanelSettings>(AssetDatabase.GUIDToAssetPath(guid));
        }

        static T FindInScene<T>() where T : Component =>
            Resources.FindObjectsOfTypeAll<T>()
                .FirstOrDefault(c => !EditorUtility.IsPersistent(c)
                                     && c.gameObject.scene.IsValid()
                                     && c.gameObject.scene.isLoaded);

        static void SetRef(Object target, string field, Object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(field);
            if (prop == null)
            {
                Debug.LogWarning($"[Story Trek] Serialized field '{field}' not found on {target.GetType().Name}.");
                return;
            }
            prop.objectReferenceValue = value;
            so.ApplyModifiedProperties();
        }
    }
}
