using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.UI.Views
{
    /// <summary>
    /// Attach to the same GameObject as the UIDocument for the sReader
    /// register page. Inline UXML styles can only reach the TextField's
    /// root element — the inner "#unity-text-input" child is created at
    /// runtime and skinned by the theme (that big light box you saw).
    /// This script flattens it so the fields render as the slim, dark,
    /// rounded inputs the design intends.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class RegisterPageStyler : MonoBehaviour
    {
        static readonly Color TextColor = new Color32(242, 239, 230, 255); // parchment text

        void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            foreach (var tf in root.Query<TextField>().ToList())
            {
                // The visible box is this inner child, not the TextField itself.
                var input = tf.Q("unity-text-input");
                if (input == null) continue;

                // Kill the theme skin: transparent, borderless, no extra padding.
                input.style.backgroundColor   = Color.clear;
                input.style.borderTopWidth    = 0;
                input.style.borderBottomWidth = 0;
                input.style.borderLeftWidth   = 0;
                input.style.borderRightWidth  = 0;
                input.style.paddingTop        = 0;
                input.style.paddingBottom     = 0;
                input.style.marginTop         = 0;
                input.style.marginBottom      = 0;

                // Center the text vertically inside the slim field.
                input.style.unityTextAlign = TextAnchor.MiddleLeft;
                input.style.color          = TextColor;

                // Cursor + selection colors come from AuthFields.uss
                // (--unity-cursor-color / --unity-selection-color); the old
                // ITextSelection setters are obsolete in Unity 6.
            }
        }
    }
}
