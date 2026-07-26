using UnityEngine.UIElements;
using SReader.Domains.Assignments.Models;
using SReader.Domains.Education.Models;
using SReader.UI.ViewModels;

namespace SReader.UI.Views.Home
{
    /// <summary>
    /// The student's "Assignments" home section: the classes they've been
    /// enrolled into (by paying for a subject the class teaches and having the
    /// tutor confirm it), and the assignments set in each. Read-only — students
    /// don't manage classes here; they just see the work assigned to them.
    /// List → class → assignment, one concern per screen.
    /// </summary>
    internal sealed class StudentClassAssignmentsView
    {
        readonly ClassViewModel vm;
        VisualElement content;

        public StudentClassAssignmentsView(ClassViewModel vm) => this.vm = vm;

        public void Render(VisualElement contentArea)
        {
            content = contentArea;
            ShowMyClasses();
        }

        // ── My enrolled classes ──────────────────────────────────────────────
        async void ShowMyClasses()
        {
            content.Clear();
            content.Add(HomeUI.Heading("My classes"));
            content.Add(HomeUI.Caption("Classes you're enrolled in. Tap one to see its assignments."));

            var loading = HomeUI.Caption("Loading…");
            content.Add(loading);
            var result = await vm.LoadMyClassesAsync();
            loading.RemoveFromHierarchy();

            if (result.IsFailure) { content.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
            if (vm.MyClasses.Count == 0)
            {
                content.Add(HomeUI.Caption("You're not enrolled in any class yet. Pay for a subject under an academy's grade and, once your tutor confirms it, the class appears here."));
                return;
            }

            foreach (var enr in vm.MyClasses)
            {
                var captured = enr;
                var card = HomeUI.Card();
                card.Add(HomeUI.Title(string.IsNullOrWhiteSpace(enr.ClassName) ? "Class" : enr.ClassName, 15));
                card.Add(HomeUI.Sub($"{enr.GradeTitle} · {enr.AcademyName}"));
                card.Add(HomeUI.Sub("Tap to see assignments"));
                card.RegisterCallback<ClickEvent>(_ => ShowAssignments(captured));
                content.Add(card);
            }
        }

        // ── Assignments in one class ─────────────────────────────────────────
        async void ShowAssignments(Enrollment enr)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", ShowMyClasses));
            content.Add(HomeUI.Heading(string.IsNullOrWhiteSpace(enr.ClassName) ? "Assignments" : enr.ClassName));
            content.Add(HomeUI.Caption($"{enr.GradeTitle} · {enr.AcademyName}"));

            var loading = HomeUI.Caption("Loading assignments…");
            content.Add(loading);
            var result = await vm.LoadAssignmentsAsync(enr.ClassId);
            loading.RemoveFromHierarchy();

            if (result.IsFailure) { content.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
            if (vm.Assignments.Count == 0)
            {
                content.Add(HomeUI.Caption("No assignments set for this class yet."));
                return;
            }

            foreach (var a in vm.Assignments)
            {
                var captured = a;
                var card = HomeUI.Card();
                card.Add(HomeUI.Title(a.Title, 15));
                card.Add(HomeUI.Sub("📅 Due " + a.DueDate.ToLocalTime().ToString("ddd dd MMM yyyy")));
                card.Add(HomeUI.Sub("Tap to open"));
                card.RegisterCallback<ClickEvent>(_ => ShowAssignment(enr, captured));
                content.Add(card);
            }
        }

        // ── One assignment (read-only) ───────────────────────────────────────
        void ShowAssignment(Enrollment enr, Assignment a)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => ShowAssignments(enr)));
            content.Add(HomeUI.Heading(a.Title));

            var info = HomeUI.Card();
            info.Add(HomeUI.FieldLabel("Due"));
            info.Add(HomeUI.Sub("📅 " + a.DueDate.ToLocalTime().ToString("dddd dd MMM yyyy")));
            info.Add(HomeUI.FieldLabel("Marks"));
            info.Add(HomeUI.Sub(a.MaxScore > 0 ? "🏆 Out of " + a.MaxScore : "Ungraded"));
            if (!string.IsNullOrWhiteSpace(a.Instructions))
            {
                info.Add(HomeUI.FieldLabel("Instructions"));
                info.Add(HomeUI.Sub(a.Instructions));
            }
            content.Add(info);
        }
    }
}
