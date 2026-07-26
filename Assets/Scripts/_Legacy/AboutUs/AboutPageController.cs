using UnityEngine;
using UnityEngine.UIElements;
using SReader.UI.Navigation;

public class AboutPageController : MonoBehaviour
{
    [SerializeField] private NavigationManager navigation;

    // OnEnable instead of Start: the UIDocument rebuilds its visual tree
    // every time the page GameObject is re-activated by NavigationManager,
    // so callbacks must be re-registered on each activation.
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("backButton").clicked += navigation.ShowLanding;
    }
}