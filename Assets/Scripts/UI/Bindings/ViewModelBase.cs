using System;

namespace SReader.UI.Bindings
{
    /// <summary>
    /// Base for all ViewModels. Plain C# (never a MonoBehaviour) — holds
    /// screen state and calls application services; the View only renders
    /// state and forwards clicks. Views subscribe to Changed to refresh.
    /// </summary>
    public abstract class ViewModelBase
    {
        /// <summary>Raised with the property name whenever observable state changes.</summary>
        public event Action<string> Changed;

        bool isBusy;
        string errorMessage = "";

        /// <summary>True while an async operation is running — disable submit buttons.</summary>
        public bool IsBusy
        {
            get => isBusy;
            protected set
            {
                if (isBusy == value) return;
                isBusy = value;
                Raise(nameof(IsBusy));
            }
        }

        /// <summary>Empty string when there is no error to show.</summary>
        public string ErrorMessage
        {
            get => errorMessage;
            protected set
            {
                value = value ?? "";
                if (errorMessage == value) return;
                errorMessage = value;
                Raise(nameof(ErrorMessage));
            }
        }

        public void ClearError() => ErrorMessage = "";

        protected void Raise(string propertyName) => Changed?.Invoke(propertyName);

        /// <summary>
        /// Used when the ViewModel has no service yet (no AppCompositionRoot
        /// in the scene): input was validated, so report success and let the
        /// View continue its placeholder flow. Real backend results replace
        /// this automatically once services are injected.
        /// </summary>
        protected Core.Common.Result PlaceholderOk()
        {
            ErrorMessage = "";
            return Core.Common.Result.Ok();
        }
    }
}
