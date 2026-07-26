using UnityEngine;
using UnityEngine.UIElements;
using SReader.UI.Bindings;
using SReader.UI.Navigation;
using SReader.UI.ViewModels;

namespace SReader.UI.Views
{
    /// <summary>View for ForgotPasswordReal.uxml — logic in ForgotPasswordViewModel.</summary>
    [RequireComponent(typeof(UIDocument))]
    public class ForgotPasswordController : MonoBehaviour
    {
        [SerializeField] private NavigationManager navigation;

        private VisualElement root;
        private TextField emailField;
        private Label errorLabel;
        private ForgotPasswordViewModel viewModel;

        /// <summary>Called by AppCompositionRoot; a service-less ViewModel is used until then.</summary>
        public void Construct(ForgotPasswordViewModel vm) => viewModel = vm;

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

            viewModel = viewModel ?? new ForgotPasswordViewModel();

            root = GetComponent<UIDocument>().rootVisualElement;

            emailField = root.Q<TextField>("email");
            errorLabel = root.Q<Label>("error-label");

            UIPageBinder.Bind(root, "back-button",      navigation.ShowLogin, this);
            UIPageBinder.Bind(root, "send-code-button", OnSendCode,           this);
            UIPageBinder.Bind(root, "signin-link",      navigation.ShowLogin, this);
        }

        async void OnSendCode()
        {
            if (viewModel.IsBusy) return;

            viewModel.Email = emailField?.value?.Trim() ?? "";

            var result = await viewModel.SendCodeAsync();
            if (result.IsFailure)
            {
                ShowError(viewModel.ErrorMessage);
                return;
            }

            HideError();
            navigation.ShowOTP();
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
