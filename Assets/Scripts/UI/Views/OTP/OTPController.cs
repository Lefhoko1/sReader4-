using UnityEngine;
using UnityEngine.UIElements;
using SReader.UI.Bindings;
using SReader.UI.Navigation;
using SReader.UI.ViewModels;

namespace SReader.UI.Views
{
    /// <summary>
    /// View for OTPReal.uxml. Digit-box focus juggling is view-only
    /// behavior and stays here; code verification lives in OtpViewModel.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class OTPController : MonoBehaviour
    {
        [SerializeField] private NavigationManager navigation;

        private VisualElement root;
        private TextField[] otpFields;
        private Label errorLabel;
        private OtpViewModel viewModel;

        /// <summary>Called by AppCompositionRoot; a service-less ViewModel is used until then.</summary>
        public void Construct(OtpViewModel vm) => viewModel = vm;

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

            viewModel = viewModel ?? new OtpViewModel();

            root = GetComponent<UIDocument>().rootVisualElement;
            errorLabel = root.Q<Label>("error-label");

            otpFields = new TextField[6];
            for (int i = 0; i < 6; i++)
            {
                otpFields[i] = root.Q<TextField>($"otp-{i}");
            }

            // Wire up auto-advance and single-character enforcement
            for (int i = 0; i < 6; i++)
            {
                int index = i;
                otpFields[i]?.RegisterValueChangedCallback(evt => OnDigitChanged(index, evt));
                otpFields[i]?.RegisterCallback<KeyDownEvent>(evt => OnKeyDown(index, evt));
            }

            UIPageBinder.Bind(root, "back-button",   navigation.ShowForgotPassword, this);
            UIPageBinder.Bind(root, "verify-button", OnVerify,                      this);
            UIPageBinder.Bind(root, "resend-button", OnResend,                      this);
            UIPageBinder.Bind(root, "signin-link",   navigation.ShowLogin,          this);
        }

        void OnDigitChanged(int index, ChangeEvent<string> evt)
        {
            var field = otpFields[index];
            if (field == null) return;

            // Keep only the last character typed and strip non-digits
            string raw = evt.newValue;
            string digit = "";
            for (int c = raw.Length - 1; c >= 0; c--)
            {
                if (char.IsDigit(raw[c])) { digit = raw[c].ToString(); break; }
            }

            // Suppress the callback loop while we correct the value
            field.SetValueWithoutNotify(digit);

            if (digit.Length == 1 && index < 5)
            {
                otpFields[index + 1]?.Focus();
            }

            HideError();
        }

        void OnKeyDown(int index, KeyDownEvent evt)
        {
            // Backspace on an empty box moves focus to the previous box
            if (evt.keyCode == KeyCode.Backspace && string.IsNullOrEmpty(otpFields[index]?.value) && index > 0)
            {
                otpFields[index - 1]?.Focus();
            }
        }

        async void OnVerify()
        {
            if (viewModel.IsBusy) return;

            string code = "";
            foreach (var f in otpFields) code += f?.value ?? "";

            var result = await viewModel.VerifyAsync(code);
            if (result.IsFailure)
            {
                ShowError(viewModel.ErrorMessage);
                return;
            }

            HideError();
            navigation.ShowResetPassword();
        }

        async void OnResend()
        {
            foreach (var f in otpFields) f?.SetValueWithoutNotify("");
            otpFields[0]?.Focus();
            HideError();

            await viewModel.ResendAsync();
            Debug.Log("OTP resend requested");
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
