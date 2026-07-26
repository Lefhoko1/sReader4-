using UnityEngine;
using UnityEngine.UI;
using SReader.UI.Navigation;

public class LandingPageController : MonoBehaviour
{
    [SerializeField] private NavigationManager navigation;
    
    [Header("Buttons")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button aboutButton;
    [SerializeField] private Button registerButton;

    private void Start()
    {
        if (loginButton != null && navigation != null)
        {
            loginButton.onClick.AddListener(navigation.ShowLogin);
        }

        if (aboutButton != null && navigation != null)
        {
            aboutButton.onClick.AddListener(navigation.ShowAbout);
        }

        if (registerButton != null && navigation != null)
        {
            registerButton.onClick.AddListener(navigation.ShowRegister);
        }
    }
}