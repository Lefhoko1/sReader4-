using UnityEngine;
using UnityEngine.UIElements;
using SReader.UI.Bindings;
using SReader.UI.Navigation;
using SReader.UI.ViewModels;

namespace SReader.UI.Views
{
    /// <summary>View for ContactUsReal.uxml — validation/sending in ContactUsViewModel.</summary>
    [RequireComponent(typeof(UIDocument))]
    public class ContactUsController : MonoBehaviour
    {
        [SerializeField] private NavigationManager navigation;

        private VisualElement root;
        private Label errorLabel;
        private Label successLabel;
        private ContactUsViewModel viewModel;

        /// <summary>Called by AppCompositionRoot; a service-less ViewModel is used until then.</summary>
        public void Construct(ContactUsViewModel vm) => viewModel = vm;

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

            viewModel = viewModel ?? new ContactUsViewModel();

            root         = GetComponent<UIDocument>().rootVisualElement;
            errorLabel   = root.Q<Label>("error-label");
            successLabel = root.Q<Label>("success-label");

            UIPageBinder.Bind(root, "back-button",        navigation.ShowLanding,  this);
            UIPageBinder.Bind(root, "get-started-button", navigation.ShowRegister, this);
            UIPageBinder.Bind(root, "send-button",        OnSend,                  this);
        }

        async void OnSend()
        {
            if (viewModel.IsBusy) return;

            viewModel.FullName = root.Q<TextField>("full-name")?.value?.Trim() ?? "";
            viewModel.Email    = root.Q<TextField>("email")?.value?.Trim() ?? "";
            viewModel.Subject  = root.Q<TextField>("subject")?.value?.Trim() ?? "";
            viewModel.Message  = root.Q<TextField>("message")?.value?.Trim() ?? "";

            var result = await viewModel.SendAsync();
            if (result.IsFailure)
            {
                ShowError(viewModel.ErrorMessage);
                return;
            }

            HideError();
            ShowSuccess("Thanks! Your message has been sent — we'll be in touch soon.");
        }

        void ShowError(string message)
        {
            if (successLabel != null) successLabel.style.display = DisplayStyle.None;
            if (errorLabel == null) return;
            errorLabel.text = message;
            errorLabel.style.display = DisplayStyle.Flex;
        }

        void HideError()
        {
            if (errorLabel == null) return;
            errorLabel.style.display = DisplayStyle.None;
        }

        void ShowSuccess(string message)
        {
            if (successLabel == null) return;
            successLabel.text = message;
            successLabel.style.display = DisplayStyle.Flex;
        }
    }
}
