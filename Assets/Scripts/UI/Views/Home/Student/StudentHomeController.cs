using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SReader.Domains.Assignments.Models;
using SReader.Domains.Social.Models;
using SReader.Game.Trek;
using SReader.UI.ViewModels.Home;

namespace SReader.UI.Views.Home
{
    /// <summary>View for StudentHomeReal.uxml — student bottom-nav sections.</summary>
    public sealed class StudentHomeController : ShellHomeController<StudentHomeViewModel>
    {
        [Tooltip("Story Trek scene page (TrekScenePage). Optional — when set, assignments show a ‘Story Trek’ launch (Phase 0).")]
        [SerializeField] private TrekScenePage trekPage;

        StudentAcademiesView academiesView;
        StudentFriendsView friendsView;
        StudentAssignmentsView assignmentsView;

        // Set by a home-dashboard tile, consumed when the Assignments section opens.
        string pendingAssignmentId;

        protected override StudentHomeViewModel CreatePlaceholderViewModel() => new StudentHomeViewModel();

        protected override bool TryRenderSection(string navButton, string tab, VisualElement content)
        {
            if (navButton == "nav-home")
            {
                RenderHomeDashboard(content);
                return true;
            }

            if (navButton == "nav-academies" && Academy != null)
            {
                academiesView = academiesView ?? new StudentAcademiesView(Academy, Classes);
                academiesView.Render(content, tab, SelfName);
                return true;
            }

            if (navButton == "nav-tutors")
            {
                RenderTutors(content);
                return true;
            }

            if (navButton == "nav-friends" && Friends != null)
            {
                friendsView = friendsView ?? new StudentFriendsView(Friends);
                friendsView.Render(content, tab);
                return true;
            }

            if (navButton == "nav-assignments" && Assignments != null)
            {
                // "Find a class to enrol" (shown when a student has no assignments)
                // jumps straight to the Academies → Discover section.
                assignmentsView = assignmentsView ?? new StudentAssignmentsView(
                    Assignments,
                    () => GoToSection("nav-academies"),
                    trekPage != null
                        ? (Action<AssignmentView>)(v => trekPage.Launch(v, Assignments))
                        : null);
                // Honour a deep-link from the home dashboard (open one assignment).
                if (pendingAssignmentId != null) { assignmentsView.OpenAssignment(pendingAssignmentId); pendingAssignmentId = null; }
                assignmentsView.Render(content);
                ContentBack = () => assignmentsView.HandleBack();   // wire the system back button
                return true;
            }

            return false;
        }

        protected override Section[] BuildSections() => new[]
        {
            new Section { NavButton = "nav-home",        Title = "Home",        Tabs = new[] { "Today" },               Blurb = "Your day at a glance." },
            new Section { NavButton = "nav-academies",   Title = "Academies",   Tabs = new[] { "Discover", "Enrolled" }, Blurb = "Browse academies and the ones you've joined." },
            new Section { NavButton = "nav-tutors",      Title = "Tutors",      Tabs = new[] { "My tutors" },           Blurb = "The tutors at the academies you're enrolled in." },
            new Section { NavButton = "nav-assignments", Title = "Assignments", Tabs = new[] { "Assignments" },          Blurb = "Your assignments, schedules and submissions." },
            new Section { NavButton = "nav-friends",     Title = "Friends",     Tabs = new[] { "Friends", "Discover", "Search", "Requests" },  Blurb = "Your reading circle, new students to add and pending requests." },
        };

        // ── Home dashboard (nav-home) ────────────────────────────────────────
        // A launchpad instead of placeholder cards: what the student is doing now
        // (continue / next up), quick stats, the next scheduled item, and one-tap
        // links to Assignments, Friends, Academies and Profile. Built compact so it
        // fits without scrolling, and the tiles fade/slide in with LeanTween.
        async void RenderHomeDashboard(VisualElement content)
        {
            var loading = HomeUI.Caption("Loading your day…");
            content.Add(loading);

            if (Assignments != null) await Assignments.LoadAsync();
            loading.RemoveFromHierarchy();

            // Pull the highlights from the assignments the student is enrolled in.
            AssignmentView resume = null, nextUp = null;
            int dueSoon = 0, scheduled = 0, completed = 0;

            if (Assignments != null)
            {
                foreach (var v in Assignments.MyAssignments)
                {
                    if (resume == null && v.Status == AttemptStatus.InProgress) resume = v;
                    if (!v.Submitted && v.Assignment != null && v.Assignment.IsDueSoon) dueSoon++;
                    if (v.Completed) completed++;
                    if (v.IsScheduled) scheduled++;
                    if (!v.Submitted && v.Assignment != null &&
                        (nextUp == null || v.Assignment.DueDate < nextUp.Assignment.DueDate))
                        nextUp = v;
                }
            }

            // Home is a fixed, non-scrolling "game" screen: fill the viewport and
            // clip rather than scroll. The blocks share the height (space-between).
            if (content is ScrollView sv) sv.contentContainer.style.flexGrow = 1;

            var root = new VisualElement();
            root.style.flexGrow = 1;
            root.style.overflow = Overflow.Hidden;
            root.style.justifyContent = Justify.SpaceBetween;
            content.Add(root);

            // Game-like backdrop behind the cards (shows through the gaps between
            // blocks). Added first so it paints behind the content; ignores taps.
            root.Add(HomeArt.Backdrop(HomeArt.HomeBackground, 0.72f));

            int order = 0;

            // Base Camp trailhead kicker (spec Phase 2).
            var kicker = new Label("⛺ BASE CAMP");
            kicker.style.fontSize = 11;
            kicker.style.letterSpacing = 2;
            kicker.style.unityFontStyleAndWeight = FontStyle.Bold;
            kicker.style.color = new Color(233f / 255f, 223f / 255f, 200f / 255f);
            kicker.style.flexShrink = 0;
            kicker.style.marginBottom = 2;
            root.Add(kicker); AnimateIn(kicker, order++);

            var hero = BuildHero(resume, nextUp);
            hero.style.flexShrink = 0;
            root.Add(hero); AnimateIn(hero, order++);

            var stats = BuildStatsRow(dueSoon, scheduled, completed);
            stats.style.flexShrink = 0;
            root.Add(stats); AnimateIn(stats, order++);

            // Buttons (not lists) that lead to the student's classes and subjects.
            var learn = BuildLearnLinks();
            learn.style.flexShrink = 0;
            root.Add(learn); AnimateIn(learn, order++);

            var links = BuildQuickLinks();
            links.style.flexShrink = 0;
            root.Add(links); AnimateIn(links, order++);
        }

        // Two buttons (not lists) that lead to the student's classes and subjects.
        VisualElement BuildLearnLinks()
        {
            return HomeUI.ChipRow(
                HomeUI.Chip("🏫 My classes", () => GoToSection("nav-assignments")),
                HomeUI.Chip("📘 My subjects", () => GoToSection("nav-academies", 1)));
        }

        // ── Tutors (nav-tutors) ──────────────────────────────────────────────
        // The student's actual tutors: the staff at the academies they're enrolled
        // in. Sourced from the friend-safe student_academics view (confirmed
        // enrolments only), grouped one card per tutor with their academies and
        // the subjects they teach this student.
        async void RenderTutors(VisualElement content)
        {
            content.Add(HomeUI.Heading("My tutors"));
            content.Add(HomeUI.Caption("The tutors at the academies you're enrolled in."));

            var myId = Assignments?.CurrentUserId;
            if (Friends == null || string.IsNullOrEmpty(myId))
            {
                content.Add(HomeUI.Caption("Sign in to see your tutors."));
                return;
            }

            var loading = HomeUI.Caption("Loading…");
            content.Add(loading);
            var r = await Friends.LoadAcademicsAsync(myId);
            loading.RemoveFromHierarchy();

            if (r.IsFailure) { content.Add(HomeUI.Status(Friends.ErrorMessage, true)); return; }

            // One card per tutor (a tutor may teach across grades / academies).
            var byTutor = new Dictionary<string, List<StudentAcademicRecord>>();
            var order = new List<string>();
            foreach (var rec in Friends.Academics)
            {
                var key = string.IsNullOrWhiteSpace(rec.TutorName) ? "Your tutor" : rec.TutorName;
                if (!byTutor.TryGetValue(key, out var list)) { list = new List<StudentAcademicRecord>(); byTutor[key] = list; order.Add(key); }
                list.Add(rec);
            }

            if (order.Count == 0)
            {
                content.Add(HomeUI.Caption("No tutors yet. Enrol in an academy's subject and your tutor will appear here."));
                var find = HomeUI.Primary("Find a class", () => GoToSection("nav-academies"));
                find.style.marginTop = 8;
                content.Add(find);
                return;
            }

            int idx = 0;
            foreach (var key in order)
            {
                var card = HomeUI.Card();
                card.Add(HomeUI.Title(key, 16));
                foreach (var rec in byTutor[key])
                {
                    card.Add(HomeUI.Sub("🎓 " + rec.AcademyName
                        + (string.IsNullOrWhiteSpace(rec.GradeTitle) ? "" : " · " + rec.GradeTitle)));
                    if (rec.Subjects.Count > 0)
                        card.Add(HomeUI.Sub("📘 " + string.Join(", ", rec.Subjects)));
                }
                content.Add(card);
                AnimateIn(card, idx++);
            }
        }

        // The single most useful next action: resume an in-progress attempt, else
        // open the soonest-due assignment, else point a new student to a class.
        VisualElement BuildHero(AssignmentView resume, AssignmentView nextUp)
        {
            var card = HomeUI.Card();
            card.style.paddingTop = card.style.paddingBottom = 12;

            // Lay the hero out as a row: text/action on the left, the looping
            // reading-boy mascot (sprite-sheet animation) on the right.
            var row = HomeUI.Row();
            row.style.alignItems = Align.Center;
            card.Add(row);

            var text = new VisualElement();
            text.style.flexGrow = 1;
            text.style.flexShrink = 1;
            row.Add(text);

            if (resume != null)
            {
                text.Add(HomeUI.FieldLabel("CONTINUE WHERE YOU LEFT OFF"));
                text.Add(HomeUI.Title(SafeTitle(resume), 18));
                text.Add(HomeUI.Sub("🏫 " + ClassNameOf(resume) + "  ·  In progress"));
                var go = HomeUI.Primary("Resume", () => OpenAssignmentInSection(resume.Assignment.Id));
                go.style.marginTop = 12;
                text.Add(go);
            }
            else if (nextUp != null)
            {
                text.Add(HomeUI.FieldLabel("NEXT UP"));
                text.Add(HomeUI.Title(SafeTitle(nextUp), 18));
                text.Add(HomeUI.Sub("📅 Due " + nextUp.Assignment.DueDate.ToLocalTime().ToString("ddd dd MMM")
                    + "  ·  " + ClassNameOf(nextUp)));
                var go = HomeUI.Primary("Open assignment", () => OpenAssignmentInSection(nextUp.Assignment.Id));
                go.style.marginTop = 12;
                text.Add(go);
            }
            else
            {
                text.Add(HomeUI.FieldLabel("WELCOME"));
                text.Add(HomeUI.Title("You're all set", 18));
                text.Add(HomeUI.Sub("No assignments yet. Join a class to start receiving work."));
                var go = HomeUI.Primary("Find a class", () => GoToSection("nav-academies"));
                go.style.marginTop = 12;
                text.Add(go);
            }

            var mascot = HomeArt.MascotBoy(84f);
            mascot.style.flexShrink = 0;
            mascot.style.marginLeft = 8;
            row.Add(mascot);

            return card;
        }

        VisualElement BuildStatsRow(int dueSoon, int scheduled, int completed)
        {
            var row = HomeUI.Row();
            row.style.marginLeft = -4; row.style.marginRight = -4;   // align edges with the cards
            row.Add(StatTile("Due soon", dueSoon, () => GoToSection("nav-assignments")));
            row.Add(StatTile("Scheduled", scheduled, () => GoToSection("nav-assignments")));
            row.Add(StatTile("Completed", completed, () => GoToSection("nav-assignments")));
            return row;
        }

        // Uniform tile height so every stat / quick-link card is the same size
        // (equal width comes from flex-grow:1 + flex-basis:0).
        const float TileHeight = 68f;

        VisualElement StatTile(string label, int count, Action onClick)
        {
            var t = EqualTile();
            var n = HomeUI.Title(count.ToString(), 22);
            n.style.unityTextAlign = TextAnchor.MiddleCenter;
            var l = HomeUI.Sub(label);
            l.style.width = Length.Percent(100);
            l.style.unityTextAlign = TextAnchor.MiddleCenter;
            t.Add(n); t.Add(l);
            t.RegisterCallback<ClickEvent>(_ => onClick());
            return t;
        }

        // A square-ish card of fixed size used by the stat + quick-link rows.
        VisualElement EqualTile()
        {
            var t = HomeUI.Card();
            t.style.flexGrow = 1; t.style.flexBasis = 0;
            t.style.height = TileHeight;
            t.style.marginLeft = 3; t.style.marginRight = 3;
            t.style.paddingTop = t.style.paddingBottom = 8;
            t.style.paddingLeft = t.style.paddingRight = 2;
            t.style.alignItems = Align.Center;
            t.style.justifyContent = Justify.Center;   // centre content within the fixed height
            return t;
        }

        VisualElement BuildQuickLinks()
        {
            var wrap = new VisualElement();
            wrap.Add(HomeUI.FieldLabel("QUICK LINKS"));

            var row = HomeUI.Row();
            row.style.marginLeft = -4; row.style.marginRight = -4;
            row.Add(LinkTile("📚", "Assignments", () => GoToSection("nav-assignments")));
            row.Add(LinkTile("👥", "Friends", () => GoToSection("nav-friends")));
            row.Add(LinkTile("🎓", "Academies", () => GoToSection("nav-academies")));
            row.Add(LinkTile("👤", "Profile", OpenProfile));
            wrap.Add(row);
            return wrap;
        }

        VisualElement LinkTile(string icon, string label, Action onClick)
        {
            var t = EqualTile();

            var i = new Label(icon);
            i.style.fontSize = 20;
            i.style.unityTextAlign = TextAnchor.MiddleCenter;
            i.style.marginBottom = 4;
            var l = HomeUI.Sub(label);
            l.style.width = Length.Percent(100);
            l.style.unityTextAlign = TextAnchor.MiddleCenter;
            l.style.marginTop = 0;
            l.style.fontSize = 11;
            l.style.whiteSpace = WhiteSpace.NoWrap;   // keep the label on one line
            l.style.overflow = Overflow.Hidden;
            t.Add(i); t.Add(l);
            t.RegisterCallback<ClickEvent>(_ => onClick());
            return t;
        }

        // Deep-link: remember the assignment, then switch to the Assignments
        // section which opens straight to its profile (see TryRenderSection).
        void OpenAssignmentInSection(string assignmentId)
        {
            pendingAssignmentId = assignmentId;
            GoToSection("nav-assignments");
        }

        string ClassNameOf(AssignmentView v)
            => Assignments != null ? Assignments.ClassNameFor(v.Assignment?.ClassId) : "Class";

        static string SafeTitle(AssignmentView v)
            => v.Assignment != null && !string.IsNullOrWhiteSpace(v.Assignment.Title) ? v.Assignment.Title : "Assignment";

        // Smooth fade + slide-up as each tile appears (LeanTween). Falls back to
        // fully visible if tweening is unavailable, so the dashboard is never blank.
        void AnimateIn(VisualElement el, int order)
        {
            try
            {
                el.style.opacity = 0f;
                LeanTween.value(gameObject, 0f, 1f, 0.28f)
                    .setDelay(order * 0.06f)
                    .setEase(LeanTweenType.easeOutCubic)
                    .setOnUpdate((float v) =>
                    {
                        el.style.opacity = v;
                        el.style.translate = new Translate(0f, (1f - v) * 10f, 0f);
                    })
                    .setOnComplete(() =>
                    {
                        el.style.opacity = 1f;
                        el.style.translate = new Translate(0f, 0f, 0f);
                    });
            }
            catch
            {
                el.style.opacity = 1f;
            }
        }
    }
}
