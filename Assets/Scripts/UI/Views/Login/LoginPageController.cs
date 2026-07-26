using UnityEngine;
using UnityEngine.UIElements;
using SReader.UI.Bindings;
using SReader.UI.Navigation;
using SReader.UI.ViewModels;

namespace SReader.UI.Views
{
    /// <summary>
    /// View for LoginReal.uxml — renders state and forwards clicks only;
    /// sign-in logic lives in LoginViewModel (MVVM).
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class LoginPageController : MonoBehaviour
    {
        [SerializeField] private NavigationManager navigation;

        // Remembers the last email used to sign in, so it is pre-filled next launch.
        private const string LastEmailKey = "sreader.auth.lastEmail";

        private VisualElement root;
        private Label errorLabel;
        private LoginViewModel viewModel;
        private PostLoginRouter router;

        /// <summary>Called by AppCompositionRoot; a service-less ViewModel is used until then.</summary>
        public void Construct(LoginViewModel vm) => viewModel = vm;

        /// <summary>
        /// Supplies the role-aware post-login router. Until it is set (placeholder
        /// mode) sign-in falls back to the profile screen.
        /// </summary>
        public void SetPostLoginRouter(PostLoginRouter router) => this.router = router;

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

            viewModel = viewModel ?? new LoginViewModel();

            root = GetComponent<UIDocument>().rootVisualElement;
            errorLabel = root.Q<Label>("error-label");

            // Strip the built-in theme skin off each TextField's inner
            // "#unity-text-input" child — otherwise it renders a white box with
            // white text (invisible). Same fix as AuthPageStyler/RegisterPageStyler,
            // done here so the login page doesn't depend on the styler component
            // being attached in the scene.
            SkinTextInputs();

            UIPageBinder.Bind(root, "back-button",            navigation.ShowLanding,        this);
            UIPageBinder.Bind(root, "create-account-button",  navigation.ShowRegister,       this);
            UIPageBinder.Bind(root, "forgot-password-button", navigation.ShowForgotPassword, this);
            UIPageBinder.Bind(root, "google-button",          () => Debug.Log("Google login clicked"), this);
            UIPageBinder.Bind(root, "apple-button",           () => Debug.Log("Apple login clicked"),  this);
            UIPageBinder.Bind(root, "signin-button",          OnSignIn,                      this);

            // Pre-fill the remembered email from PlayerPrefs (stored on last sign-in).
            var emailField = root.Q<TextField>("email");
            var remembered = PlayerPrefs.GetString(LastEmailKey, "");
            if (emailField != null && !string.IsNullOrEmpty(remembered) && string.IsNullOrEmpty(emailField.value))
                emailField.value = remembered;
        }

        // The visible input box is the TextField's inner child, created at runtime
        // by the theme; inline UXML styles can't reach it. Clear its background and
        // set the text colour so the dark fields render correctly.
        void SkinTextInputs()
        {
            Color textColor = new Color32(242, 239, 230, 255);
            foreach (var tf in root.Query<TextField>().ToList())
            {
                var input = tf.Q("unity-text-input");
                if (input == null) continue;
                input.style.backgroundColor = Color.clear;
                input.style.borderTopWidth = input.style.borderBottomWidth = 0;
                input.style.borderLeftWidth = input.style.borderRightWidth = 0;
                input.style.paddingTop = input.style.paddingBottom = 0;
                input.style.marginTop = input.style.marginBottom = 0;
                input.style.unityTextAlign = TextAnchor.MiddleLeft;
                input.style.color = textColor;
            }
        }

        async void OnSignIn()
        {
            if (viewModel.IsBusy) return;

            viewModel.Email    = root.Q<TextField>("email")?.value ?? "";
            viewModel.Password = root.Q<TextField>("password")?.value ?? "";

            var result = await viewModel.SignInAsync();
            if (result.IsFailure)
            {
                ShowError(viewModel.ErrorMessage);
                return;
            }

            HideError();

            // Remember the email for next launch (the full session is persisted
            // separately by PlayerPrefsSessionStore via AuthenticationService).
            PlayerPrefs.SetString(LastEmailKey, viewModel.Email);
            PlayerPrefs.Save();

            Debug.Log("Sign in succeeded");

            // Route to the role-specific home; fall back to the profile screen
            // in placeholder mode (no router wired).
            if (router != null) await router.RouteAsync();
            else navigation.ShowProfile();
        }

        void ShowError(string message)
        {
            if (errorLabel == null) return;
            errorLabel.text = message;
            errorLabel.style.display = DisplayStyle.Flex;
        }

        void HideError()
        {
            if (errorLabel == null) return;
            errorLabel.style.display = DisplayStyle.None;
        }
    }
}
