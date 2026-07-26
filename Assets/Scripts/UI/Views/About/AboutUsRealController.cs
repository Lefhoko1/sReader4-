using UnityEngine;
using UnityEngine.UIElements;
using SReader.UI.Bindings;
using SReader.UI.Navigation;

namespace SReader.UI.Views
{
    /// <summary>View for AboutUsReal.uxml — pure navigation, no ViewModel needed.</summary>
    [RequireComponent(typeof(UIDocument))]
    public class AboutUsRealController : MonoBehaviour
    {
        [SerializeField] private NavigationManager navigation;

        // OnEnable instead of Start: the UIDocument rebuilds its visual tree
        // every time the page GameObject is re-activated by NavigationManager,
        // so callbacks must be re-registered on each activation.
        void OnEnable()
        {
            if (navigation == null)
            {
                Debug.LogError($"[{name}] Navigation is not assigned in the Inspector — page buttons will not work.", this);
                return;
            }

            var root = GetComponent<UIDocument>().rootVisualElement;

            UIPageBinder.Bind(root, "back-button",        navigation.ShowLanding,  this);
            UIPageBinder.Bind(root, "get-started-button", navigation.ShowRegister, this);
            UIPageBinder.Bind(root, "contact-button",     navigation.ShowContact,  this);

            UIPager.Setup(root, this);
        }
    }
}
