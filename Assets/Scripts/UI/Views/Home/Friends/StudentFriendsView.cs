using UnityEngine;
using UnityEngine.UIElements;
using SReader.Domains.Social.Models;
using SReader.UI.ViewModels;

namespace SReader.UI.Views.Home
{
    /// <summary>
    /// Friends section of the student home. Four deliberately separate tabs so
    /// lists never mix:
    ///   • "Friends"  — your accepted friends (tap one to open their profile).
    ///   • "Discover" — every other student you can befriend (no search box).
    ///   • "Search"   — its own screen: type a name / @username to find someone.
    ///   • "Requests" — incoming requests (accept / reject) kept apart from the
    ///                  requests you've sent (cancel).
    /// Tapping a person on any tab opens their profile on its own screen with the
    /// right "Add friend" / "Accept" action.
    /// All rendering is in code in the design-system palette (via HomeUI), so no
    /// UXML / scene changes are needed — it slots into the existing nav-friends
    /// section's content area.
    /// </summary>
    internal sealed class StudentFriendsView
    {
        readonly FriendshipViewModel vm;
        VisualElement content;

        public StudentFriendsView(FriendshipViewModel vm) => this.vm = vm;

        public void Render(VisualElement contentArea, string tab)
        {
            content = contentArea;
            switch (tab)
            {
                case "Discover": ShowDiscover();   break;
                case "Search":   ShowSearch(null); break;
                case "Requests": ShowRequests();   break;
                default:         ShowFriends();    break;
            }
        }

        // ── Friends list ─────────────────────────────────────────────────────
        async void ShowFriends()
        {
            content.Clear();
            content.Add(HomeUI.Heading("Your friends"));
            content.Add(HomeUI.Caption("Tap a friend to open their profile."));

            var loading = HomeUI.Caption("Loading…");
            content.Add(loading);
            var result = await vm.LoadFriendsAsync();
            loading.RemoveFromHierarchy();

            if (result.IsFailure) { content.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
            if (vm.Friends.Count == 0)
            {
                content.Add(HomeUI.Caption("No friends yet. Open Discover to find other students."));
                return;
            }

            foreach (var f in vm.Friends)
            {
                var captured = f;
                var card = PersonCard(f.Person, onTap: () => ShowProfile(captured.Person.UserId, ShowFriends));
                var remove = HomeUI.Danger_("Remove", null);
                remove.style.marginTop = 8;
                remove.clicked += async () =>
                {
                    remove.SetEnabled(false);
                    var r = await vm.RemoveAsync(captured.FriendshipId);
                    if (r.IsSuccess) ShowFriends();
                    else { remove.SetEnabled(true); card.Add(HomeUI.Status(vm.ErrorMessage, true)); }
                };
                card.Add(remove);
                content.Add(card);
            }
        }

        // ── Discover other students (plain list, no search box) ──────────────
        async void ShowDiscover()
        {
            content.Clear();
            content.Add(HomeUI.Heading("Discover students"));
            content.Add(HomeUI.Caption("Browse other students and send a friend request. Use the Search tab to find someone by name."));

            var loading = HomeUI.Caption("Loading…");
            content.Add(loading);
            var result = await vm.LoadDiscoverAsync(null);
            loading.RemoveFromHierarchy();

            if (result.IsFailure) { content.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
            if (vm.Discovered.Count == 0)
            {
                content.Add(HomeUI.Caption("No other students yet."));
                return;
            }

            RenderPeople(vm.Discovered, ShowDiscover);
        }

        // ── Search: its own screen ───────────────────────────────────────────
        async void ShowSearch(string query)
        {
            content.Clear();
            content.Add(HomeUI.Heading("Search students"));
            content.Add(HomeUI.Caption("Type a name or @username, then tap Search."));

            content.Add(HomeUI.FieldLabel("Name or @username"));
            var searchField = HomeUI.Field(query);
            content.Add(searchField);
            var searchBtn = HomeUI.Primary("Search", () => ShowSearch(searchField.value));
            searchBtn.style.marginTop = 4;
            searchBtn.style.marginBottom = 12;
            content.Add(searchBtn);

            // Nothing typed yet — just show the field.
            if (string.IsNullOrWhiteSpace(query)) return;

            var loading = HomeUI.Caption("Searching…");
            content.Add(loading);
            var result = await vm.LoadDiscoverAsync(query);
            loading.RemoveFromHierarchy();

            if (result.IsFailure) { content.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
            if (vm.Discovered.Count == 0)
            {
                content.Add(HomeUI.Caption($"No students match \"{query}\"."));
                return;
            }

            RenderPeople(vm.Discovered, () => ShowSearch(query));
        }

        /// <summary>Render a list of discoverable people with their relationship action.</summary>
        void RenderPeople(System.Collections.Generic.IReadOnlyList<DiscoverPerson> people, System.Action reload)
        {
            foreach (var p in people)
            {
                var captured = p;
                var card = PersonCard(p.Person, onTap: () => ShowProfile(captured.Person.UserId, reload));
                card.Add(RelationshipAction(captured, reload));
                content.Add(card);
            }
        }

        // ── Requests: incoming (accept/reject) kept apart from sent (cancel) ──
        async void ShowRequests()
        {
            content.Clear();
            content.Add(HomeUI.Heading("Friend requests"));

            // Received
            content.Add(HomeUI.Title("Requests received"));
            var loadingIn = HomeUI.Caption("Loading…");
            content.Add(loadingIn);
            var inResult = await vm.LoadIncomingAsync();
            loadingIn.RemoveFromHierarchy();

            if (inResult.IsFailure) content.Add(HomeUI.Status(vm.ErrorMessage, true));
            else if (vm.Incoming.Count == 0) content.Add(HomeUI.Caption("No requests waiting on you."));
            else
                foreach (var f in vm.Incoming)
                {
                    var captured = f;
                    var card = PersonCard(f.Person, onTap: () => ShowProfile(captured.Person.UserId, ShowRequests));

                    var actions = HomeUI.Row();
                    actions.style.marginTop = 8;
                    var accept = HomeUI.Primary("Accept", null);
                    accept.style.flexGrow = 1;
                    accept.style.marginRight = 8;
                    accept.clicked += async () =>
                    {
                        accept.SetEnabled(false);
                        var r = await vm.AcceptAsync(captured.FriendshipId);
                        if (r.IsSuccess) ShowRequests();
                        else { accept.SetEnabled(true); card.Add(HomeUI.Status(vm.ErrorMessage, true)); }
                    };
                    var reject = HomeUI.Danger_("Reject", null);
                    reject.style.flexGrow = 1;
                    reject.clicked += async () =>
                    {
                        reject.SetEnabled(false);
                        var r = await vm.DeclineAsync(captured.FriendshipId);
                        if (r.IsSuccess) ShowRequests();
                        else { reject.SetEnabled(true); card.Add(HomeUI.Status(vm.ErrorMessage, true)); }
                    };
                    actions.Add(accept);
                    actions.Add(reject);
                    card.Add(actions);
                    content.Add(card);
                }

            // Sent
            var sentHeading = HomeUI.Title("Requests sent");
            sentHeading.style.marginTop = 18;
            content.Add(sentHeading);
            var loadingOut = HomeUI.Caption("Loading…");
            content.Add(loadingOut);
            var outResult = await vm.LoadSentAsync();
            loadingOut.RemoveFromHierarchy();

            if (outResult.IsFailure) content.Add(HomeUI.Status(vm.ErrorMessage, true));
            else if (vm.Sent.Count == 0) content.Add(HomeUI.Caption("You haven't sent any requests."));
            else
                foreach (var f in vm.Sent)
                {
                    var captured = f;
                    var card = PersonCard(f.Person, onTap: () => ShowProfile(captured.Person.UserId, ShowRequests));
                    card.Add(HomeUI.Sub("⏳ Pending — awaiting their response"));
                    var cancel = HomeUI.Outline("Cancel request", null);
                    cancel.style.marginTop = 8;
                    cancel.clicked += async () =>
                    {
                        cancel.SetEnabled(false);
                        var r = await vm.CancelAsync(captured.FriendshipId);
                        if (r.IsSuccess) ShowRequests();
                        else { cancel.SetEnabled(true); card.Add(HomeUI.Status(vm.ErrorMessage, true)); }
                    };
                    card.Add(cancel);
                    content.Add(card);
                }
        }

        // ── Standalone profile screen for a selected person ──────────────────
        async void ShowProfile(string userId, System.Action onBack)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", onBack ?? (() => ShowDiscover())));

            var loading = HomeUI.Caption("Loading profile…");
            content.Add(loading);
            var result = await vm.LoadPersonAsync(userId);
            loading.RemoveFromHierarchy();

            if (result.IsFailure || vm.SelectedPerson == null)
            {
                content.Add(HomeUI.Status(vm.ErrorMessage ?? "Could not load this profile.", true));
                return;
            }

            var person = vm.SelectedPerson.Person;

            // Centered avatar + identity header.
            var header = new VisualElement();
            header.style.alignItems = Align.Center;
            header.style.marginTop = 8;
            header.style.marginBottom = 12;
            header.Add(Avatar(person.AvatarUrl, 96));
            var name = HomeUI.Title(person.Label, 20);
            name.style.marginTop = 10;
            name.style.unityTextAlign = TextAnchor.MiddleCenter;
            header.Add(name);
            if (!string.IsNullOrWhiteSpace(person.Username))
            {
                var handle = HomeUI.Sub("@" + person.Username);
                handle.style.unityTextAlign = TextAnchor.MiddleCenter;
                header.Add(handle);
            }
            var role = HomeUI.Sub(person.RoleLabel);
            role.style.unityTextAlign = TextAnchor.MiddleCenter;
            header.Add(role);
            content.Add(header);

            // Relationship action(s).
            var card = HomeUI.Card();
            card.Add(HomeUI.FieldLabel("Friendship"));
            card.Add(RelationshipAction(vm.SelectedPerson, reload: () => ShowProfile(userId, onBack)));
            content.Add(card);

            // Sub-menu: swap the lower area between "About" and "Academies" so the
            // profile can carry the data students bond over (school, grade,
            // subjects, staff) without leaving the screen.
            var subMenu = HomeUI.Row();
            subMenu.style.marginTop = 6;
            subMenu.style.marginBottom = 4;
            content.Add(subMenu);

            var subContent = new VisualElement();
            content.Add(subContent);

            var tabs = new[] { "About", "Academies" };
            var buttons = new Button[tabs.Length];
            System.Action<int> select = null;
            select = i =>
            {
                for (int j = 0; j < buttons.Length; j++) StyleSubTab(buttons[j], j == i);
                subContent.Clear();
                if (i == 0) RenderAbout(subContent, person);
                else        RenderAcademics(subContent, userId);
            };
            for (int i = 0; i < tabs.Length; i++)
            {
                int idx = i;
                var b = new Button(() => select(idx)) { text = tabs[i] };
                buttons[i] = b;
                subMenu.Add(b);
            }
            select(0);
        }

        // ── Profile sub-sections ─────────────────────────────────────────────

        void RenderAbout(VisualElement box, PersonSummary person)
        {
            var card = HomeUI.Card();
            card.Add(HomeUI.FieldLabel("About"));
            if (!string.IsNullOrWhiteSpace(person.Bio)) card.Add(HomeUI.Sub(person.Bio));
            else card.Add(HomeUI.Sub("This student hasn't added a bio yet."));

            if (!string.IsNullOrWhiteSpace(person.Country))
            {
                card.Add(HomeUI.FieldLabel("Country"));
                card.Add(HomeUI.Sub("📍 " + person.Country));
            }
            box.Add(card);
        }

        async void RenderAcademics(VisualElement box, string userId)
        {
            var loading = HomeUI.Caption("Loading academies…");
            box.Add(loading);
            var result = await vm.LoadAcademicsAsync(userId);
            loading.RemoveFromHierarchy();

            if (result.IsFailure) { box.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
            if (vm.Academics.Count == 0)
            {
                box.Add(HomeUI.Caption("Not enrolled at any academy yet."));
                return;
            }

            foreach (var rec in vm.Academics)
            {
                var card = HomeUI.Card();
                card.Add(HomeUI.Title(string.IsNullOrWhiteSpace(rec.AcademyName) ? "Academy" : rec.AcademyName, 15));
                if (!string.IsNullOrWhiteSpace(rec.GradeTitle))
                    card.Add(HomeUI.Sub("🎓 " + rec.GradeTitle));
                if (!string.IsNullOrWhiteSpace(rec.TutorName))
                    card.Add(HomeUI.Sub("👤 Staff: " + rec.TutorName));

                if (rec.Subjects.Count > 0)
                {
                    card.Add(HomeUI.FieldLabel(rec.Subjects.Count == 1 ? "Subject" : "Subjects"));
                    card.Add(HomeUI.Sub("📚 " + string.Join(" · ", rec.Subjects)));
                }
                box.Add(card);
            }
        }

        static void StyleSubTab(Button tab, bool selected)
        {
            tab.style.backgroundColor = Color.clear;
            tab.style.color = selected ? HomeUI.Parchment : HomeUI.Muted;
            tab.style.unityFontStyleAndWeight = selected ? FontStyle.Bold : FontStyle.Normal;
            tab.style.fontSize = 14;
            tab.style.marginRight = 18;
            tab.style.paddingLeft = tab.style.paddingRight = 0;
            tab.style.paddingTop = tab.style.paddingBottom = 4;
            HomeUI.Border(tab, Color.clear, 0);
            tab.style.borderBottomWidth = selected ? 2 : 0;
            tab.style.borderBottomColor = HomeUI.Parchment;
        }

        // ── Shared builders ──────────────────────────────────────────────────

        /// <summary>A tappable card with an avatar, name and role for one person.</summary>
        VisualElement PersonCard(PersonSummary person, System.Action onTap)
        {
            var card = HomeUI.Card();

            var row = HomeUI.Row();
            var avatar = Avatar(person.AvatarUrl, 48);
            avatar.style.marginRight = 12;
            row.Add(avatar);

            var col = new VisualElement();
            col.style.flexGrow = 1;
            col.Add(HomeUI.Title(person.Label, 15));
            col.Add(HomeUI.Sub(person.RoleLabel));
            row.Add(col);

            card.Add(row);
            if (onTap != null) card.RegisterCallback<ClickEvent>(_ => onTap());
            return card;
        }

        /// <summary>The right action for the current relationship state.</summary>
        VisualElement RelationshipAction(DiscoverPerson dp, System.Action reload)
        {
            switch (dp.State)
            {
                case RelationshipState.Friends:
                {
                    var box = new VisualElement();
                    var label = HomeUI.Sub("✓ You are friends");
                    box.Add(label);
                    return box;
                }
                case RelationshipState.RequestSent:
                {
                    var cancel = HomeUI.Outline("Requested ✓ — cancel", null);
                    cancel.style.marginTop = 8;
                    cancel.clicked += async () =>
                    {
                        cancel.SetEnabled(false);
                        var r = await vm.CancelAsync(dp.FriendshipId);
                        if (r.IsSuccess) reload();
                        else { cancel.SetEnabled(true); }
                    };
                    return cancel;
                }
                case RelationshipState.RequestReceived:
                {
                    var actions = HomeUI.Row();
                    actions.style.marginTop = 8;
                    var accept = HomeUI.Primary("Accept", null);
                    accept.style.flexGrow = 1;
                    accept.style.marginRight = 8;
                    accept.clicked += async () =>
                    {
                        accept.SetEnabled(false);
                        var r = await vm.AcceptAsync(dp.FriendshipId);
                        if (r.IsSuccess) reload();
                        else accept.SetEnabled(true);
                    };
                    var reject = HomeUI.Danger_("Reject", null);
                    reject.style.flexGrow = 1;
                    reject.clicked += async () =>
                    {
                        reject.SetEnabled(false);
                        var r = await vm.DeclineAsync(dp.FriendshipId);
                        if (r.IsSuccess) reload();
                        else reject.SetEnabled(true);
                    };
                    actions.Add(accept);
                    actions.Add(reject);
                    return actions;
                }
                case RelationshipState.Blocked:
                {
                    var b = HomeUI.Outline("Blocked", null);
                    b.style.marginTop = 8;
                    b.SetEnabled(false);
                    return b;
                }
                default:
                {
                    var add = HomeUI.Primary("Add friend", null);
                    add.style.marginTop = 8;
                    add.clicked += async () =>
                    {
                        add.SetEnabled(false);
                        var r = await vm.SendRequestAsync(dp.Person.UserId);
                        if (r.IsSuccess) reload();
                        else { add.SetEnabled(true); }
                    };
                    return add;
                }
            }
        }

        /// <summary>A circular avatar; shows the latest uploaded picture, else a parchment placeholder.</summary>
        static VisualElement Avatar(string url, float size)
        {
            var a = new VisualElement();
            a.style.width = size;
            a.style.height = size;
            a.style.flexShrink = 0;
            a.style.backgroundColor = HomeUI.Parchment;
            HomeUI.CoverBackground(a);
            a.style.overflow = Overflow.Hidden;
            HomeUI.Radius(a, size / 2f);
            if (!string.IsNullOrEmpty(url)) HomeUI.LoadImageInto(a, url);
            return a;
        }
    }
}
