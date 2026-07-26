using UnityEngine;
using UnityEngine.UIElements;
using SReader.Core.Common;
using SReader.Domains.Identity.Models;
using SReader.UI.Bindings;
using SReader.UI.Navigation;
using SReader.UI.ViewModels;

namespace SReader.UI.Views
{
    /// <summary>
    /// View for RegisterReal.uxml. Step validation and registration live in
    /// RegisterViewModel — this class only moves field values in and
    /// renders errors out.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class RegisterPageController : MonoBehaviour
    {
        [SerializeField] private NavigationManager navigation;

        private VisualElement root;
        private Label errorLabel;
        private RegisterViewModel viewModel;
        private PostLoginRouter router;

        /// <summary>Called by AppCompositionRoot; a service-less ViewModel is used until then.</summary>
        public void Construct(RegisterViewModel vm) => viewModel = vm;

        /// <summary>Supplies the role-aware post-login router used after a successful sign-up.</summary>
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

            viewModel = viewModel ?? new RegisterViewModel();

            root = GetComponent<UIDocument>().rootVisualElement;
            errorLabel = root.Q<Label>("error-label");

            UIPageBinder.Bind(root, "back-button",     navigation.ShowLanding, this);
            UIPageBinder.Bind(root, "signin-button",   navigation.ShowLogin,   this);
            UIPageBinder.Bind(root, "google-button",   () => Debug.Log("Google register clicked"), this);
            UIPageBinder.Bind(root, "apple-button",    () => Debug.Log("Apple register clicked"),  this);
            UIPageBinder.Bind(root, "register-button", OnRegister,             this);

            // Role picker — a three-way segmented control. The view holds only
            // the visual selected-state; the chosen role lives on the ViewModel.
            UIPageBinder.Bind(root, "role-student",  () => SelectRole(UserRole.Student),  this);
            UIPageBinder.Bind(root, "role-guardian", () => SelectRole(UserRole.Guardian), this);
            UIPageBinder.Bind(root, "role-tutor",    () => SelectRole(UserRole.Tutor),    this);
            SelectRole(viewModel.Role);

            // Multi-step form: "›" only advances when the current step is
            // valid, and a stale error never follows the user to a new step.
            UIPager.Setup(root, this, beforeNext: ValidateStep, onPageChanged: _ => HideError());
        }

        // Step 0 = names + email, step 1 = passwords, step 2 = terms + submit.
        bool ValidateStep(int step)
        {
            PullFieldsIntoViewModel();

            Result result;
            switch (step)
            {
                case 0:  result = viewModel.ValidateIdentityStep(); break;
                case 1:  result = viewModel.ValidateSecurityStep(); break;
                default: result = Result.Ok(); break;
            }

            if (result.IsFailure)
            {
                ShowError(viewModel.ErrorMessage);
                return false;
            }

            HideError();
            return true;
        }

        async void OnRegister()
        {
            if (viewModel.IsBusy) return;

            PullFieldsIntoViewModel();

            var result = await viewModel.RegisterAsync();
            if (result.IsFailure)
            {
                ShowError(viewModel.ErrorMessage);
                return;
            }

            HideError();
            Debug.Log("Registration submitted");

            // If sign-up returned a live session (email confirmation disabled),
            // land straight on the new account's role-specific home. When
            // confirmation is required the ViewModel surfaces a message instead
            // and the user signs in later.
            if (router != null) await router.RouteAsync();
            else navigation.ShowLogin();
        }

        // Parchment fill = selected; outlined = unselected — mirrors the
        // primary/secondary button treatment used elsewhere in the auth UI.
        void SelectRole(UserRole role)
        {
            viewModel.Role = role;
            PaintRole("role-student",  role == UserRole.Student);
            PaintRole("role-guardian", role == UserRole.Guardian);
            PaintRole("role-tutor",    role == UserRole.Tutor);
        }

        void PaintRole(string buttonName, bool selected)
        {
            var button = root.Q<Button>(buttonName);
            if (button == null) return;

            if (selected)
            {
                button.style.backgroundColor = new Color(233f / 255f, 223f / 255f, 200f / 255f);
                button.style.color = new Color(18f / 255f, 20f / 255f, 25f / 255f);
                button.style.unityFontStyleAndWeight = FontStyle.Bold;
            }
            else
            {
                button.style.backgroundColor = Color.clear;
                button.style.color = new Color(242f / 255f, 239f / 255f, 230f / 255f);
                button.style.unityFontStyleAndWeight = FontStyle.Normal;
            }
        }

        void PullFieldsIntoViewModel()
        {
            viewModel.FirstName       = root.Q<TextField>("first-name")?.value?.Trim() ?? "";
            viewModel.LastName        = root.Q<TextField>("last-name")?.value?.Trim() ?? "";
            viewModel.Email           = root.Q<TextField>("email")?.value?.Trim() ?? "";
            viewModel.Password        = root.Q<TextField>("password")?.value ?? "";
            viewModel.ConfirmPassword = root.Q<TextField>("confirm-password")?.value ?? "";
            viewModel.AcceptedTerms   = root.Q<Toggle>("terms-toggle")?.value ?? false;
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
