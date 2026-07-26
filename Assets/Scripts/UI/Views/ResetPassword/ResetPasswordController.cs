using UnityEngine;
using UnityEngine.UIElements;
using SReader.UI.Bindings;
using SReader.UI.Navigation;
using SReader.UI.ViewModels;

namespace SReader.UI.Views
{
    /// <summary>
    /// View for ResetPasswordReal.uxml. Show/Hide toggles are view-only;
    /// password rules and submission live in ResetPasswordViewModel.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class ResetPasswordController : MonoBehaviour
    {
        [SerializeField] private NavigationManager navigation;

        private VisualElement root;
        private TextField newPasswordField;
        private TextField confirmPasswordField;
        private Label errorLabel;
        private ResetPasswordViewModel viewModel;

        /// <summary>Called by AppCompositionRoot; a service-less ViewModel is used until then.</summary>
        public void Construct(ResetPasswordViewModel vm) => viewModel = vm;

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

            viewModel = viewModel ?? new ResetPasswordViewModel();

            root = GetComponent<UIDocument>().rootVisualElement;

            newPasswordField     = root.Q<TextField>("new-password");
            confirmPasswordField = root.Q<TextField>("confirm-password");
            errorLabel           = root.Q<Label>("error-label");

            SetupToggle("toggle-new-password",     newPasswordField);
            SetupToggle("toggle-confirm-password", confirmPasswordField);

            UIPageBinder.Bind(root, "back-button",   navigation.ShowLogin, this);
            UIPageBinder.Bind(root, "update-button", OnUpdate,             this);
            UIPageBinder.Bind(root, "signin-link",   navigation.ShowLogin, this);
        }

        void SetupToggle(string buttonName, TextField field)
        {
            var btn = root.Q<Button>(buttonName);
            if (btn == null || field == null) return;
            btn.clicked += () =>
            {
                field.isPasswordField = !field.isPasswordField;
                btn.text = field.isPasswordField ? "Show" : "Hide";
            };
        }

        async void OnUpdate()
        {
            if (viewModel.IsBusy) return;

            viewModel.NewPassword     = newPasswordField?.value ?? "";
            viewModel.ConfirmPassword = confirmPasswordField?.value ?? "";

            var result = await viewModel.UpdatePasswordAsync();
            if (result.IsFailure)
            {
                ShowError(viewModel.ErrorMessage);
                return;
            }

            HideError();
            navigation.ShowLogin();
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
