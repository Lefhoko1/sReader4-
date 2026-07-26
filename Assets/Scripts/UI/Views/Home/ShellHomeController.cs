using UnityEngine;
using UnityEngine.UIElements;
using SReader.Core.Common;
using SReader.UI.Bindings;
using SReader.UI.Navigation;
using SReader.UI.ViewModels;
using SReader.UI.ViewModels.Home;

namespace SReader.UI.Views.Home
{
    /// <summary>
    /// Shared LinkedIn-style home shell. The bottom navigation selects a
    /// section; each section drives the top sub-menu, and the chosen sub-tab
    /// swaps the center content. The avatar opens the shared ProfilePage. Role
    /// subclasses only declare their <see cref="Section"/>s and build their
    /// placeholder ViewModel — all rendering, navigation and styling live here
    /// so the Student and Tutor homes stay identical in behaviour and look.
    /// Section content is placeholder until the data features land.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public abstract class ShellHomeController<TViewModel> : MonoBehaviour
        where TViewModel : RoleHomeViewModel
    {
        [SerializeField] private NavigationManager navigation;

        // ── Design-system palette (mirrors the UXML) ──
        protected static readonly Color Panel     = Rgb(35, 39, 51);
        protected static readonly Color Hairline  = Rgb(46, 51, 64);
        protected static readonly Color Parchment = Rgb(233, 223, 200);
        protected static readonly Color Text      = Rgb(242, 239, 230);
        protected static readonly Color Muted     = Rgb(152, 160, 174);

        /// <summary>A bottom-nav destination and the sub-tabs it shows up top.</summary>
        protected sealed class Section
        {
            public string NavButton;   // element name of the bottom-nav button
            public string Title;        // shown in the content header
            public string[] Tabs;       // top-menu sub-tabs
            public string Blurb;        // one-line description under the header
        }

        /// <summary>The role's bottom-nav sections, left to right.</summary>
        protected abstract Section[] BuildSections();

        /// <summary>Service-less ViewModel used before AppCompositionRoot wires services.</summary>
        protected abstract TViewModel CreatePlaceholderViewModel();

        /// <summary>
        /// Override to render a section's content yourself (e.g. the Academies
        /// CRUD). Return true if handled; false falls back to placeholder cards.
        /// </summary>
        protected virtual bool TryRenderSection(string navButton, string tab, VisualElement content) => false;

        /// <summary>Shared Academies CRUD backing (injected by AppCompositionRoot).</summary>
        protected AcademyViewModel Academy { get; private set; }
        public void SetAcademyViewModel(AcademyViewModel vm) => Academy = vm;

        /// <summary>Friends / requests / discovery backing (injected by AppCompositionRoot).</summary>
        protected FriendshipViewModel Friends { get; private set; }
        public void SetFriendshipViewModel(FriendshipViewModel vm) => Friends = vm;

        /// <summary>Classes / enrolment / assignments backing (injected by AppCompositionRoot).</summary>
        protected ClassViewModel Classes { get; private set; }
        public void SetClassViewModel(ClassViewModel vm) => Classes = vm;

        /// <summary>Student assignments module backing (injected by AppCompositionRoot).</summary>
        protected StudentAssignmentsViewModel Assignments { get; private set; }
        public void SetAssignmentsViewModel(StudentAssignmentsViewModel vm) => Assignments = vm;

        /// <summary>Online/offline source for the "showing saved data" banner (injected by AppCompositionRoot).</summary>
        IConnectivity connectivity;
        public void SetConnectivity(IConnectivity c) => connectivity = c;
        OfflineStatus offlineStatus;
        public void SetOfflineStatus(OfflineStatus s) => offlineStatus = s;
        VisualElement offlineBanner;

        /// <summary>The signed-in user's display name / email, for the requests they raise.</summary>
        protected string SelfName => string.IsNullOrWhiteSpace(viewModel?.DisplayName) ? viewModel?.Email : viewModel.DisplayName;

        Section[] sections;
        VisualElement root;
        VisualElement topMenu;
        ScrollView contentArea;
        TViewModel viewModel;

        int activeSection;
        int activeTab;

        /// <summary>
        /// Optional back handler for the section currently on screen. A section
        /// that owns deep in-content navigation (e.g. Assignments) sets this in
        /// <see cref="TryRenderSection"/> so the Android/system back button can pop
        /// one screen. Returns true when it handled the press. Reset to null on
        /// every section/tab change.
        /// </summary>
        protected System.Func<bool> ContentBack;

        // Remembers the last system-back press so two quick presses on a home root
        // mean "exit" (Android convention) rather than silently quitting on one.
        float lastBackPress = -10f;

        /// <summary>Called by AppCompositionRoot; a service-less ViewModel is used until then.</summary>
        public void Construct(TViewModel vm) => viewModel = vm;

        async void OnEnable()
        {
            if (navigation == null)
            {
                Debug.LogError($"[{name}] Navigation is not assigned in the Inspector — page buttons will not work.", this);
                return;
            }

            viewModel = viewModel ?? CreatePlaceholderViewModel();
            sections = BuildSections();

            root = GetComponent<UIDocument>().rootVisualElement;
            topMenu = root.Q<VisualElement>("top-menu");
            contentArea = root.Q<ScrollView>("content-area");

            // System back button / gesture (registering the same handler twice is
            // a no-op, so this is safe across page re-activations).
            root.RegisterCallback<NavigationCancelEvent>(OnSystemBack);

            // A thin top banner that appears whenever we're offline (the user is
            // then looking at the last cached copy of their data). Polled, so it
            // clears itself the moment a connection returns.
            offlineBanner = HomeOfflineBanner.Build();
            root.Insert(0, offlineBanner);
            RefreshOfflineBanner();
            InvokeRepeating(nameof(RefreshOfflineBanner), 1f, 3f);

            UIPageBinder.Bind(root, "profile-button", navigation.ShowProfile, this);
            UIPageBinder.Bind(root, "signout-button", OnSignOut, this);

            // Bottom navigation — each button selects its section.
            for (int i = 0; i < sections.Length; i++)
            {
                int index = i; // capture
                UIPageBinder.Bind(root, sections[i].NavButton, () => SelectSection(index), this);
            }

            await viewModel.LoadAsync();
            RenderIdentity();

            SelectSection(0);
        }

        void OnDisable()
        {
            CancelInvoke(nameof(RefreshOfflineBanner));
            root?.UnregisterCallback<NavigationCancelEvent>(OnSystemBack);
        }

        // ── Android / system back button ─────────────────────────────────────
        // UI Toolkit raises NavigationCancelEvent for the "cancel/back" input
        // (the Android hardware back + gesture, Escape on desktop) regardless of
        // which input backend is active — so we use it instead of UnityEngine.Input
        // (which throws when the Input System package is the active handler).
        // Order: let the active section pop one of its own screens; else jump to
        // the first (home) section; else confirm-to-exit.
        void OnSystemBack(NavigationCancelEvent evt)
        {
            if (root == null) return;
            evt.StopPropagation();

            if (ContentBack != null && ContentBack()) return;

            if (activeSection != 0) { SelectSection(0); return; }

            if (Time.unscaledTime - lastBackPress < 2f) Application.Quit();
            else { lastBackPress = Time.unscaledTime; ShowBackToast("Press back again to exit"); }
        }

        /// <summary>Open the shared Profile page (used by home-screen shortcuts).</summary>
        protected void OpenProfile() => navigation.ShowProfile();

        /// <summary>Switch the bottom-nav to a named section (used for cross-section shortcuts).</summary>
        protected void GoToSection(string navButton) => GoToSection(navButton, 0);

        /// <summary>Switch to a named section and open one of its sub-tabs.</summary>
        protected void GoToSection(string navButton, int tabIndex)
        {
            if (sections == null) return;
            for (int i = 0; i < sections.Length; i++)
                if (sections[i].NavButton == navButton)
                {
                    SelectSection(i);
                    if (tabIndex > 0 && tabIndex < sections[i].Tabs.Length) SelectTab(tabIndex);
                    return;
                }
        }

        // A transient centred chip on the page root; auto-removed after ~1.8s.
        void ShowBackToast(string message)
        {
            if (root == null) return;
            var toast = new Label(message);
            toast.style.position = Position.Absolute;
            toast.style.bottom = 80;
            toast.style.left = 0;
            toast.style.right = 0;
            toast.style.unityTextAlign = TextAnchor.MiddleCenter;
            toast.style.color = Text;
            toast.style.fontSize = 13;
            toast.pickingMode = PickingMode.Ignore;
            root.Add(toast);
            toast.schedule.Execute(() => toast.RemoveFromHierarchy()).StartingIn(1800);
        }

        void RefreshOfflineBanner() => HomeOfflineBanner.Refresh(offlineBanner, connectivity, offlineStatus);

        void RenderIdentity()
        {
            var greeting = root.Q<Label>("greeting");
            if (greeting != null) greeting.text = viewModel.Greeting;

            // Show the user's uploaded avatar if they have one.
            var avatar = root.Q<VisualElement>("profile-button");
            if (avatar != null && !string.IsNullOrEmpty(viewModel.AvatarUrl))
                HomeUI.LoadImageInto(avatar, viewModel.AvatarUrl);
        }

        // ── Bottom nav → section ──
        void SelectSection(int index)
        {
            activeSection = index;
            activeTab = 0;

            for (int i = 0; i < sections.Length; i++)
            {
                var btn = root.Q<Button>(sections[i].NavButton);
                if (btn != null) btn.style.color = (i == index) ? Parchment : Muted;
            }

            BuildTopMenu();
            RenderContent();
        }

        // ── Section → top sub-menu ──
        void BuildTopMenu()
        {
            if (topMenu == null) return;
            topMenu.Clear();

            var section = sections[activeSection];
            for (int i = 0; i < section.Tabs.Length; i++)
            {
                int index = i; // capture
                var tab = new Button(() => SelectTab(index)) { text = section.Tabs[i] };
                StyleTopTab(tab, i == activeTab);
                topMenu.Add(tab);
            }

            // Let a subclass append a section action to the upper nav (e.g. "Create").
            DecorateTopMenu(section.NavButton, topMenu);
        }

        /// <summary>Append section-specific actions to the upper nav (right side).</summary>
        protected virtual void DecorateTopMenu(string navButton, VisualElement topMenu) { }

        void SelectTab(int index)
        {
            activeTab = index;
            // Only the sub-tab buttons (added first) are restyled — not any
            // decoration the subclass appended after them (e.g. "Create").
            int tabCount = sections[activeSection].Tabs.Length;
            for (int i = 0; i < topMenu.childCount && i < tabCount; i++)
                if (topMenu[i] is Button b) StyleTopTab(b, i == index);
            RenderContent();
        }

        // ── Selection → center content ──
        void RenderContent()
        {
            if (contentArea == null) return;
            contentArea.Clear();

            // Each section opts in to its own system-back handling; clear it first
            // so a section without deep navigation doesn't inherit the last one's.
            ContentBack = null;

            var section = sections[activeSection];
            var tab = section.Tabs[activeTab];

            // Let a subclass own this section's body (e.g. Academies CRUD).
            if (TryRenderSection(section.NavButton, tab, contentArea)) return;

            contentArea.Add(Heading($"{section.Title} · {tab}"));
            contentArea.Add(Caption(section.Blurb));

            for (int i = 1; i <= 3; i++)
                contentArea.Add(PlaceholderCard($"{tab} item {i}", "Content for this section is coming soon."));
        }

        async void OnSignOut()
        {
            await viewModel.SignOutAsync();
            navigation.ShowLanding();
        }

        // ── UI builders (inline styles match the design system) ──

        static Label Heading(string text)
        {
            var label = new Label(text);
            label.style.fontSize = 20;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.color = Text;
            label.style.marginBottom = 4;
            label.style.whiteSpace = WhiteSpace.Normal;
            return label;
        }

        static Label Caption(string text)
        {
            var label = new Label(text);
            label.style.fontSize = 14;
            label.style.color = Muted;
            label.style.marginBottom = 16;
            label.style.whiteSpace = WhiteSpace.Normal;
            return label;
        }

        static VisualElement PlaceholderCard(string title, string body)
        {
            var card = new VisualElement();
            card.style.backgroundColor = Panel;
            card.style.borderTopLeftRadius = card.style.borderTopRightRadius = 12;
            card.style.borderBottomLeftRadius = card.style.borderBottomRightRadius = 12;
            SetBorder(card, Hairline, 1);
            card.style.paddingTop = card.style.paddingBottom = 14;
            card.style.paddingLeft = card.style.paddingRight = 14;
            card.style.marginBottom = 10;

            var t = new Label(title);
            t.style.fontSize = 15;
            t.style.unityFontStyleAndWeight = FontStyle.Bold;
            t.style.color = Text;
            t.style.marginBottom = 4;

            var b = new Label(body);
            b.style.fontSize = 13;
            b.style.color = Muted;
            b.style.whiteSpace = WhiteSpace.Normal;

            card.Add(t);
            card.Add(b);
            return card;
        }

        static void StyleTopTab(Button tab, bool selected)
        {
            tab.style.backgroundColor = Color.clear;
            tab.style.color = selected ? Parchment : Muted;
            tab.style.unityFontStyleAndWeight = selected ? FontStyle.Bold : FontStyle.Normal;
            tab.style.fontSize = 14;
            tab.style.marginRight = 18;
            tab.style.paddingLeft = tab.style.paddingRight = 0;
            tab.style.paddingTop = tab.style.paddingBottom = 4;
            SetBorder(tab, Color.clear, 0);
            tab.style.borderBottomWidth = selected ? 2 : 0;
            tab.style.borderBottomColor = Parchment;
        }

        static void SetBorder(VisualElement el, Color color, float width)
        {
            el.style.borderLeftWidth = el.style.borderRightWidth = width;
            el.style.borderTopWidth = el.style.borderBottomWidth = width;
            el.style.borderLeftColor = el.style.borderRightColor = color;
            el.style.borderTopColor = el.style.borderBottomColor = color;
        }

        static Color Rgb(int r, int g, int b) => new Color(r / 255f, g / 255f, b / 255f);
    }
}
