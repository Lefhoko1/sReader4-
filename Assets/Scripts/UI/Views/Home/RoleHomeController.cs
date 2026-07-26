using UnityEngine;
using UnityEngine.UIElements;
using SReader.Core.Common;
using SReader.UI.Bindings;
using SReader.UI.Navigation;
using SReader.UI.ViewModels.Home;

namespace SReader.UI.Views.Home
{
    /// <summary>
    /// Shared view behaviour for the role-specific home screens. Renders the
    /// greeting/title/tagline and wires the common "Profile" and "Sign out"
    /// buttons; role subclasses bind their own quick-action buttons in
    /// <see cref="BindRoleActions"/>. Mirrors the OnEnable re-binding pattern
    /// used by the other pages (NavigationManager re-activates the GameObject,
    /// rebuilding the UIDocument's visual tree).
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public abstract class RoleHomeController<TViewModel> : MonoBehaviour
        where TViewModel : RoleHomeViewModel
    {
        [SerializeField] private NavigationManager navigation;

        protected VisualElement Root { get; private set; }
        protected TViewModel ViewModel { get; private set; }

        /// <summary>Online/offline source for the "showing saved data" banner (injected by AppCompositionRoot).</summary>
        IConnectivity connectivity;
        public void SetConnectivity(IConnectivity c) => connectivity = c;
        OfflineStatus offlineStatus;
        public void SetOfflineStatus(OfflineStatus s) => offlineStatus = s;
        VisualElement offlineBanner;

        /// <summary>Called by AppCompositionRoot; a service-less ViewModel is used until then.</summary>
        public void Construct(TViewModel vm) => ViewModel = vm;

        /// <summary>Builds the placeholder ViewModel used before services are wired.</summary>
        protected abstract TViewModel CreatePlaceholderViewModel();

        /// <summary>Bind the buttons unique to this role's home (optional).</summary>
        protected virtual void BindRoleActions(VisualElement root) { }

        async void OnEnable()
        {
            if (navigation == null)
            {
                Debug.LogError($"[{name}] Navigation is not assigned in the Inspector — page buttons will not work.", this);
                return;
            }

            ViewModel = ViewModel ?? CreatePlaceholderViewModel();

            Root = GetComponent<UIDocument>().rootVisualElement;

            // Top banner shown while offline (the user is viewing cached data).
            offlineBanner = HomeOfflineBanner.Build();
            Root.Insert(0, offlineBanner);
            RefreshOfflineBanner();
            InvokeRepeating(nameof(RefreshOfflineBanner), 1f, 3f);

            UIPageBinder.Bind(Root, "profile-button", navigation.ShowProfile, this);
            UIPageBinder.Bind(Root, "signout-button", OnSignOut, this);
            BindRoleActions(Root);

            await ViewModel.LoadAsync();
            Populate();
        }

        void OnDisable() => CancelInvoke(nameof(RefreshOfflineBanner));

        void RefreshOfflineBanner() => HomeOfflineBanner.Refresh(offlineBanner, connectivity, offlineStatus);

        void Populate()
        {
            SetLabel("greeting", ViewModel.Greeting);
            SetLabel("home-title", ViewModel.Title);
            SetLabel("home-tagline", ViewModel.Tagline);
        }

        async void OnSignOut()
        {
            await ViewModel.SignOutAsync();
            navigation.ShowLanding();
        }

        protected void SetLabel(string name, string text)
        {
            var label = Root.Q<Label>(name);
            if (label != null) label.text = text;
        }
    }
}
