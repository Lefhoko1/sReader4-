using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.UI.Bindings
{
    /// <summary>
    /// Safe button binding for UI Toolkit pages. A missing element logs a
    /// clear warning instead of throwing — an unhandled exception inside
    /// OnEnable silently kills every binding that comes after it, which
    /// makes the remaining buttons on the page appear "dead".
    /// </summary>
    public static class UIPageBinder
    {
        public static void Bind(VisualElement root, string buttonName, System.Action onClick, MonoBehaviour context)
        {
            var button = root?.Q<Button>(buttonName);
            if (button == null)
            {
                Debug.LogWarning($"[{context.name}] Button '{buttonName}' not found — check the UIDocument's Source Asset matches this controller.", context);
                return;
            }
            button.clicked += onClick;
        }
    }
}
