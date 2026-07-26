using UnityEngine;
using UnityEngine.UIElements;
using SReader.UI.Bindings;
using SReader.UI.Navigation;
using SReader.UI.ViewModels;
using SReader.UI.Views.Home;

namespace SReader.UI.Views
{
    /// <summary>
    /// View for ProfileReal.uxml — the signed-in landing page. Renders the
    /// current user's profile, toggles between view/edit cards, and forwards
    /// save/sign-out to ProfileViewModel (MVVM). All persistence (Supabase
    /// sync) lives in the ViewModel and services.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class ProfilePageController : MonoBehaviour
    {
        [SerializeField] private NavigationManager navigation;

        private VisualElement root;
        private VisualElement avatar;
        private VisualElement viewCard;
        private VisualElement editCard;
        private Label errorLabel;
        private Label statusLabel;
        private ProfileViewModel viewModel;
        private PostLoginRouter router;

        /// <summary>Called by AppCompositionRoot; a service-less ViewModel is used until then.</summary>
        public void Construct(ProfileViewModel vm) => viewModel = vm;

        /// <summary>Supplies the role-aware router so "Back" returns to the right home.</summary>
        public void SetPostLoginRouter(PostLoginRouter router) => this.router = router;

        // OnEnable instead of Start: NavigationManager re-activates the page
        // GameObject, which rebuilds the UIDocument's visual tree — callbacks
        // must be re-registered (and data reloaded) on each activation.
        async void OnEnable()
        {
            if (navigation == null)
            {
                Debug.LogError($"[{name}] Navigation is not assigned in the Inspector — page buttons will not work.", this);
                return;
            }

            viewModel = viewModel ?? new ProfileViewModel();

            root = GetComponent<UIDocument>().rootVisualElement;
            avatar = root.Q<VisualElement>("profile-avatar");
            viewCard = root.Q<VisualElement>("view-card");
            editCard = root.Q<VisualElement>("edit-card");
            errorLabel = root.Q<Label>("error-label");
            statusLabel = root.Q<Label>("status-label");

            // Tap the avatar to change the profile picture (WhatsApp-style).
            if (avatar != null)
            {
                avatar.RegisterCallback<ClickEvent>(_ => OnChangeAvatar());
                avatar.tooltip = "Tap to change photo";
            }

            UIPageBinder.Bind(root, "back-button",    OnBack,     this);
            UIPageBinder.Bind(root, "edit-button",    ShowEdit,   this);
            UIPageBinder.Bind(root, "cancel-button",  ShowView,   this);
            UIPageBinder.Bind(root, "save-button",    OnSave,     this);
            UIPageBinder.Bind(root, "signout-button", OnSignOut,  this);

            await viewModel.LoadAsync();
            PopulateView();
            PopulateEditFields();
            RefreshAvatar();
            ShowView();
        }

        void RefreshAvatar()
        {
            if (avatar != null && !string.IsNullOrEmpty(viewModel.AvatarUrl))
                HomeUI.LoadImageInto(avatar, viewModel.AvatarUrl);
        }

        async void OnChangeAvatar()
        {
            if (viewModel.IsBusy) return;
            HideMessages();

            var result = await viewModel.ChangeProfilePictureAsync();
            if (result.IsSuccess)
            {
                if (avatar != null) HomeUI.LoadImageInto(avatar, result.Value);
                ShowMessage(statusLabel, "Profile picture updated.");
            }
            else if (!string.IsNullOrEmpty(viewModel.ErrorMessage))
            {
                ShowMessage(errorLabel, viewModel.ErrorMessage);
            }
        }

        void PopulateView()
        {
            SetLabel("greeting",     string.IsNullOrEmpty(viewModel.DisplayName) ? "Welcome" : $"Welcome, {viewModel.DisplayName}");
            SetLabel("account-line", BuildAccountLine());

            SetLabel("view-display-name", Dash(viewModel.DisplayName));
            SetLabel("view-username",     Dash(viewModel.Username));
            SetLabel("view-bio",          Dash(viewModel.Bio));
            SetLabel("view-country",      Dash(JoinDot(viewModel.Country, viewModel.Timezone)));
            SetLabel("view-location",     Dash(BuildLocationSummary()));
        }

        void PopulateEditFields()
        {
            SetField("display-name", viewModel.DisplayName);
            SetField("username",     viewModel.Username);
            SetField("bio",          viewModel.Bio);
            SetField("country",      viewModel.Country);
            SetField("timezone",     viewModel.Timezone);
            SetField("loc-country",  viewModel.LocationCountry);
            SetField("province",     viewModel.Province);
            SetField("city",         viewModel.City);
            SetField("address",      viewModel.Address);
        }

        void ShowEdit()
        {
            HideMessages();
            PopulateEditFields();
            if (viewCard != null) viewCard.style.display = DisplayStyle.None;
            if (editCard != null) editCard.style.display = DisplayStyle.Flex;
        }

        void ShowView()
        {
            HideMessages();
            if (editCard != null) editCard.style.display = DisplayStyle.None;
            if (viewCard != null) viewCard.style.display = DisplayStyle.Flex;
        }

        async void OnSave()
        {
            if (viewModel.IsBusy) return;
            HideMessages();

            // Pull edited values into the ViewModel before saving.
            viewModel.DisplayName     = Field("display-name");
            viewModel.Username        = Field("username");
            viewModel.Bio             = Field("bio");
            viewModel.Country         = Field("country");
            viewModel.Timezone        = Field("timezone");
            viewModel.LocationCountry = Field("loc-country");
            viewModel.Province        = Field("province");
            viewModel.City            = Field("city");
            viewModel.Address         = Field("address");

            var result = await viewModel.SaveAsync();
            if (result.IsFailure)
            {
                ShowMessage(errorLabel, viewModel.ErrorMessage);
                return;
            }

            PopulateView();
            ShowMessage(statusLabel, viewModel.StatusMessage);
            Debug.Log("Profile saved and synced.");
        }

        // If editing, "Back" first returns to the read-only view; from there it
        // returns to the user's role-specific home.
        async void OnBack()
        {
            if (editCard != null && editCard.style.display == DisplayStyle.Flex)
            {
                ShowView();
                return;
            }

            if (router != null) await router.RouteAsync();
            else navigation.ShowLanding();
        }

        async void OnSignOut()
        {
            await viewModel.SignOutAsync();
            navigation.ShowLanding();
        }

        // ── helpers ──

        string BuildAccountLine()
        {
            var email = viewModel.Email;
            var role  = viewModel.Role;
            if (string.IsNullOrEmpty(email)) return role;
            return string.IsNullOrEmpty(role) ? email : $"{email} · {role}";
        }

        string BuildLocationSummary()
        {
            var parts = new System.Collections.Generic.List<string>();
            foreach (var p in new[] { viewModel.City, viewModel.Province, viewModel.LocationCountry })
                if (!string.IsNullOrWhiteSpace(p)) parts.Add(p.Trim());
            return string.Join(", ", parts);
        }

        static string JoinDot(string a, string b)
        {
            a = (a ?? "").Trim();
            b = (b ?? "").Trim();
            if (a.Length == 0) return b;
            if (b.Length == 0) return a;
            return $"{a} · {b}";
        }

        static string Dash(string value) => string.IsNullOrWhiteSpace(value) ? "—" : value.Trim();

        void SetLabel(string name, string text)
        {
            var label = root.Q<Label>(name);
            if (label != null) label.text = text;
        }

        void SetField(string name, string value)
        {
            var field = root.Q<TextField>(name);
            if (field != null) field.value = value ?? "";
        }

        string Field(string name) => root.Q<TextField>(name)?.value ?? "";

        void ShowMessage(Label label, string text)
        {
            if (label == null) return;
            label.text = text;
            label.style.display = string.IsNullOrEmpty(text) ? DisplayStyle.None : DisplayStyle.Flex;
        }

        void HideMessages()
        {
            if (errorLabel != null) errorLabel.style.display = DisplayStyle.None;
            if (statusLabel != null) statusLabel.style.display = DisplayStyle.None;
        }
    }
}
