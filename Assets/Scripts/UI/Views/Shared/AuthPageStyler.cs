using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.UI.Views
{
    /// <summary>
    /// Drop this on any auth-page GameObject that also has a UIDocument.
    /// Strips the built-in theme skin from every TextField's inner
    /// "#unity-text-input" child so the slim dark inputs render correctly
    /// — identical to RegisterPageStyler but reusable across all pages.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class AuthPageStyler : MonoBehaviour
    {
        static readonly Color TextColor = new Color32(242, 239, 230, 255);

        void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            foreach (var tf in root.Query<TextField>().ToList())
            {
                var input = tf.Q("unity-text-input");
                if (input == null) continue;

                input.style.backgroundColor   = Color.clear;
                input.style.borderTopWidth    = 0;
                input.style.borderBottomWidth = 0;
                input.style.borderLeftWidth   = 0;
                input.style.borderRightWidth  = 0;
                input.style.paddingTop        = 0;
                input.style.paddingBottom     = 0;
                input.style.marginTop         = 0;
                input.style.marginBottom      = 0;
                input.style.unityTextAlign    = TextAnchor.MiddleLeft;
                input.style.color             = TextColor;

                // Cursor + selection colors come from AuthFields.uss
                // (--unity-cursor-color / --unity-selection-color); the old
                // ITextSelection setters are obsolete in Unity 6.
            }
        }
    }
}
