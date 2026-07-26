using UnityEngine.UIElements;
using SReader.UI.ViewModels.Home;

namespace SReader.UI.Views.Home
{
    /// <summary>View for TutorHomeReal.uxml — tutor bottom-nav sections.</summary>
    public sealed class TutorHomeController : ShellHomeController<TutorHomeViewModel>
    {
        TutorAcademiesView academiesView;

        protected override TutorHomeViewModel CreatePlaceholderViewModel() => new TutorHomeViewModel();

        protected override Section[] BuildSections() => new[]
        {
            new Section { NavButton = "nav-students",    Title = "Students",    Tabs = new[] { "My students", "Requests" },              Blurb = "The learners you teach and pending requests." },
            new Section { NavButton = "nav-academies",   Title = "Academies",   Tabs = new[] { "My academies", "All", "Requests" },      Blurb = "Create and manage your academies." },
            new Section { NavButton = "nav-guardians",   Title = "Guardians",   Tabs = new[] { "All", "Messages" },                      Blurb = "Parents and guardians of your students." },
            new Section { NavButton = "nav-assignments", Title = "Assignments", Tabs = new[] { "Active", "Drafts", "Submissions" },       Blurb = "Work you've set and submissions to review." },
        };

        TutorAcademiesView Academies => academiesView = academiesView ?? new TutorAcademiesView(Academy, Classes);

        protected override bool TryRenderSection(string navButton, string tab, VisualElement content)
        {
            if (navButton != "nav-academies" || Academy == null) return false;
            Academies.Render(content, tab);
            return true;
        }

        protected override void DecorateTopMenu(string navButton, VisualElement topMenu)
        {
            if (navButton != "nav-academies" || Academy == null) return;

            // Push the actions to the right: Payments + Create.
            var spacer = new VisualElement();
            spacer.style.flexGrow = 1;
            topMenu.Add(spacer);
            topMenu.Add(HomeUI.Link("Payments", () => Academies.ShowPaymentDetails()));
            topMenu.Add(HomeUI.Link("＋ Create", () => Academies.ShowCreate()));
        }
    }
}
