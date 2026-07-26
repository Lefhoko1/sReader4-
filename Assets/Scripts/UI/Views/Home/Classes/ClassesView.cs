using System;
using UnityEngine.UIElements;
using SReader.Domains.Assignments.Models;
using SReader.Domains.Education.Models;
using SReader.UI.ViewModels;

namespace SReader.UI.Views.Home
{
    /// <summary>
    /// The Classes module, with one concern per screen:
    ///   • list      — just the classes in a grade (tap a class to open it; a
    ///                  "New class" button for tutors leads to its own screen).
    ///   • profile   — data about one class, then the actions you can take on it
    ///                  (Edit, Delete, Manage students, Manage assignments, or —
    ///                  for a student — Enrol/Leave and View assignments).
    ///   • create / edit screens are never inline with a list.
    /// The same shape repeats for assignments (list → profile → edit/delete).
    /// Rendered into the home content area, returning via the supplied back action.
    /// </summary>
    internal sealed class ClassesView
    {
        readonly ClassViewModel vm;
        readonly bool isTutor;

        VisualElement content;
        Academy academy;
        AcademyGrade grade;
        Action onExit;

        public ClassesView(ClassViewModel vm, bool isTutor)
        {
            this.vm = vm;
            this.isTutor = isTutor;
        }

        // studentName is accepted for call-site symmetry with the academy views;
        // class enrolment is now payment-driven, so it's no longer used here.
        public void Show(VisualElement contentArea, Academy academy, AcademyGrade grade, string studentName, Action onBack)
        {
            this.content = contentArea;
            this.academy = academy;
            this.grade = grade;
            this.onExit = onBack;
            ShowHome(0);
        }

        // ── Screen 1: classes home (mini-nav: Classes · New · Search) ─────────
        void ShowHome(int activeTab)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => onExit?.Invoke()));
            content.Add(HomeUI.Heading("Classes"));
            content.Add(HomeUI.Caption($"{grade.Title} · {academy.Name}"));

            // Tutors create classes; students only browse / search.
            var tabs = isTutor ? new[] { "Classes", "New", "Search" } : new[] { "Classes", "Search" };

            var menu = HomeUI.Row();
            menu.style.marginBottom = 8;
            content.Add(menu);
            var tabBox = new VisualElement();
            content.Add(tabBox);

            var buttons = new Button[tabs.Length];
            System.Action<int> select = i =>
            {
                for (int j = 0; j < buttons.Length; j++) StyleSubTab(buttons[j], j == i);
                tabBox.Clear();
                var name = tabs[i];
                if (name == "New")          RenderCreate(tabBox);
                else if (name == "Search")  RenderSearch(tabBox);
                else                        RenderClassList(tabBox, null);
            };
            for (int i = 0; i < tabs.Length; i++)
            {
                int idx = i;
                buttons[i] = new Button(() => select(idx)) { text = tabs[i] };
                menu.Add(buttons[i]);
            }
            select(activeTab);
        }

        // The class list (optionally filtered by a search query).
        async void RenderClassList(VisualElement box, string query)
        {
            box.Clear();
            var loading = HomeUI.Caption("Loading classes…");
            box.Add(loading);
            if (!isTutor) await vm.LoadMyClassesAsync();   // to badge "Enrolled"
            var result = await vm.LoadClassesAsync(grade.Id);
            loading.RemoveFromHierarchy();

            if (result.IsFailure) { box.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }

            var shown = 0;
            foreach (var cls in vm.Classes)
            {
                if (!string.IsNullOrWhiteSpace(query) &&
                    (cls.Name ?? "").IndexOf(query.Trim(), System.StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                shown++;

                var captured = cls;
                var card = HomeUI.Card();
                card.Add(HomeUI.Title(cls.Name, 15));
                card.Add(HomeUI.Sub("👥 " + cls.CapacityLabel));
                if (!isTutor && vm.IsEnrolledIn(cls.Id))
                    card.Add(HomeUI.Sub("✓ Enrolled"));
                card.Add(HomeUI.Sub("Tap to open"));
                card.RegisterCallback<ClickEvent>(_ => ShowProfile(captured));
                box.Add(card);
            }

            if (shown == 0)
                box.Add(HomeUI.Caption(string.IsNullOrWhiteSpace(query)
                    ? (isTutor ? "No classes yet. Use the “New” tab to add one." : "No classes in this grade yet.")
                    : $"No classes match \"{query.Trim()}\"."));
        }

        // The create-class form (its own tab).
        void RenderCreate(VisualElement box)
        {
            box.Add(HomeUI.FieldLabel("Class name"));
            var name = HomeUI.Field(null);
            box.Add(name);
            box.Add(HomeUI.FieldLabel("Description (optional)"));
            var desc = HomeUI.Field(null, multiline: true);
            box.Add(desc);
            box.Add(HomeUI.FieldLabel("Max students (0 = no limit)"));
            var cap = HomeUI.Field("0");
            box.Add(cap);

            var status = Hidden(HomeUI.Status("", true));
            box.Add(status);

            var create = HomeUI.Primary("Create class", null);
            create.style.marginTop = 8;
            create.clicked += async () =>
            {
                status.style.display = DisplayStyle.None;
                int.TryParse(cap.value, out var max);
                create.SetEnabled(false);
                var r = await vm.CreateClassAsync(academy, grade, name.value, desc.value, max);
                create.SetEnabled(true);
                if (r.IsSuccess) ShowHome(0);   // jump to the Classes tab to see it
                else Surface(status, vm.ErrorMessage);
            };
            box.Add(create);
        }

        // The search-classes screen (its own tab).
        void RenderSearch(VisualElement box)
        {
            box.Add(HomeUI.FieldLabel("Search classes by name"));
            var field = HomeUI.Field(null);
            box.Add(field);

            var results = new VisualElement();
            var search = HomeUI.Primary("Search", () => RenderClassList(results, field.value));
            search.style.marginTop = 4;
            search.style.marginBottom = 8;
            box.Add(search);
            box.Add(results);
        }

        // ── Screen 3: class profile (data + actions) ─────────────────────────
        async void ShowProfile(AcademyClass cls)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => ShowHome(0)));
            content.Add(HomeUI.Heading(cls.Name));
            content.Add(HomeUI.Caption($"{cls.GradeTitle} · {cls.AcademyName}"));

            var info = HomeUI.Card();
            info.Add(HomeUI.FieldLabel("Capacity"));
            info.Add(HomeUI.Sub("👥 " + cls.CapacityLabel));
            if (!string.IsNullOrWhiteSpace(cls.Description))
            {
                info.Add(HomeUI.FieldLabel("About"));
                info.Add(HomeUI.Sub(cls.Description));
            }
            content.Add(info);

            if (isTutor)
            {
                // Live enrolled count.
                var countLabel = HomeUI.Sub("Loading roster…");
                content.Add(countLabel);
                var roster = await vm.LoadRosterAsync(cls.Id);
                countLabel.text = roster.IsSuccess ? $"👥 {vm.Roster.Count} enrolled" : "";

                content.Add(Action_("Manage subjects", () => ShowClassSubjects(cls)));
                content.Add(Action_("Manage students", () => ShowRoster(cls)));
                content.Add(Action_("Manage assignments", () => ShowAssignments(cls)));
                content.Add(Action_("Edit class", () => ShowEdit(cls)));
                content.Add(ConfirmDanger("Delete class", async () =>
                {
                    var r = await vm.DeleteClassAsync(cls.Id);
                    if (r.IsSuccess) ShowHome(0);
                    else content.Add(HomeUI.Status(vm.ErrorMessage, true));
                }));
            }
            else
            {
                // Enrolment is payment-driven: a student joins a class by paying
                // for one of its subjects and the tutor confirming it. So here we
                // show their status and only reveal assignments once enrolled.
                await vm.LoadMyClassesAsync();
                if (vm.IsEnrolledIn(cls.Id))
                {
                    content.Add(HomeUI.Status("✓ You're enrolled in this class.", false));
                    content.Add(Action_("View assignments", () => ShowAssignments(cls)));
                }
                else
                {
                    var note = HomeUI.Card();
                    note.Add(HomeUI.Sub("🔒 You're not enrolled in this class yet."));
                    note.Add(HomeUI.Sub("Enrol by paying for one of its subjects under this grade — once your tutor confirms the payment you'll be added here and see its assignments."));
                    content.Add(note);
                }
            }
        }

        // ── Screen: subjects taught in this class ────────────────────────────
        // A mini-nav keeps the two concerns apart: "In this class" (assigned
        // subjects, with remove) and "Assign new" (subjects not yet in it, with
        // add). Entry point loads the grade's subjects, then renders the tabs.
        void ShowClassSubjects(AcademyClass cls) => LoadSubjects(cls, 0);

        async void LoadSubjects(AcademyClass cls, int activeTab)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => ShowProfile(cls)));
            content.Add(HomeUI.Heading("Subjects in this class"));
            content.Add(HomeUI.Caption("A student who pays for one of these subjects is enrolled into this class."));

            var loading = HomeUI.Caption("Loading subjects…");
            content.Add(loading);
            var result = await vm.LoadGradeSubjectsAsync(cls.GradeId);
            loading.RemoveFromHierarchy();

            if (result.IsFailure) { content.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
            if (vm.GradeSubjects.Count == 0)
            {
                content.Add(HomeUI.Caption("No subjects in this grade yet. Add subjects to the grade first."));
                return;
            }

            // Mini-nav.
            var menu = HomeUI.Row();
            menu.style.marginBottom = 8;
            content.Add(menu);
            var listBox = new VisualElement();
            content.Add(listBox);

            var tabs = new[] { "In this class", "Assign new" };
            var buttons = new Button[tabs.Length];
            System.Action<int> select = i =>
            {
                for (int j = 0; j < buttons.Length; j++) StyleSubTab(buttons[j], j == i);
                listBox.Clear();
                RenderSubjectList(cls, listBox, inThisClass: i == 0, currentTab: i);
            };
            for (int i = 0; i < tabs.Length; i++)
            {
                int idx = i;
                buttons[i] = new Button(() => select(idx)) { text = tabs[i] };
                menu.Add(buttons[i]);
            }
            select(activeTab);
        }

        void RenderSubjectList(AcademyClass cls, VisualElement box, bool inThisClass, int currentTab)
        {
            int shown = 0;
            foreach (var subject in vm.GradeSubjects)
            {
                var here = subject.ClassId == cls.Id;
                if (inThisClass != here) continue;   // filter to the active tab
                shown++;

                var captured = subject;
                var card = HomeUI.Card();
                card.Add(HomeUI.Title(subject.Name, 15));

                if (inThisClass)
                {
                    var remove = HomeUI.Danger_("Remove from class", null);
                    remove.style.marginTop = 8;
                    remove.clicked += async () =>
                    {
                        remove.SetEnabled(false);
                        var r = await vm.UnassignSubjectAsync(captured);
                        if (r.IsSuccess) LoadSubjects(cls, currentTab);
                        else { remove.SetEnabled(true); card.Add(HomeUI.Status(vm.ErrorMessage, true)); }
                    };
                    card.Add(remove);
                }
                else
                {
                    if (subject.HasClass)
                        card.Add(HomeUI.Sub("Currently in: " + subject.ClassName));
                    var add = HomeUI.Primary("Add to this class", null);
                    add.style.marginTop = 8;
                    add.clicked += async () =>
                    {
                        add.SetEnabled(false);
                        var r = await vm.AssignSubjectAsync(captured, cls);
                        if (r.IsSuccess) LoadSubjects(cls, currentTab);
                        else { add.SetEnabled(true); card.Add(HomeUI.Status(vm.ErrorMessage, true)); }
                    };
                    card.Add(add);
                }
                box.Add(card);
            }

            if (shown == 0)
                box.Add(HomeUI.Caption(inThisClass
                    ? "No subjects assigned to this class yet. Use “Assign new”."
                    : "Every subject in this grade is already in this class."));
        }

        // ── Screen 4: edit a class (its own screen) ──────────────────────────
        void ShowEdit(AcademyClass cls)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => ShowProfile(cls)));
            content.Add(HomeUI.Heading("Edit class"));

            content.Add(HomeUI.FieldLabel("Class name"));
            var name = HomeUI.Field(cls.Name);
            content.Add(name);
            content.Add(HomeUI.FieldLabel("Description (optional)"));
            var desc = HomeUI.Field(cls.Description, multiline: true);
            content.Add(desc);
            content.Add(HomeUI.FieldLabel("Max students (0 = no limit)"));
            var cap = HomeUI.Field(cls.MaxStudents.ToString());
            content.Add(cap);

            var status = Hidden(HomeUI.Status("", true));
            content.Add(status);

            var save = HomeUI.Primary("Save changes", null);
            save.style.marginTop = 8;
            save.clicked += async () =>
            {
                status.style.display = DisplayStyle.None;
                int.TryParse(cap.value, out var max);
                save.SetEnabled(false);
                var r = await vm.UpdateClassAsync(cls, name.value, desc.value, max);
                save.SetEnabled(true);
                if (r.IsSuccess) ShowProfile(cls);
                else Surface(status, vm.ErrorMessage);
            };
            content.Add(save);
        }

        // ── Screen 5: roster (tutor) ─────────────────────────────────────────
        async void ShowRoster(AcademyClass cls)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => ShowProfile(cls)));
            content.Add(HomeUI.Heading("Students"));
            content.Add(HomeUI.Caption(cls.Name));

            var loading = HomeUI.Caption("Loading students…");
            content.Add(loading);
            var result = await vm.LoadRosterAsync(cls.Id);
            loading.RemoveFromHierarchy();

            if (result.IsFailure) { content.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
            if (vm.Roster.Count == 0) { content.Add(HomeUI.Caption("No students enrolled yet.")); return; }

            content.Add(HomeUI.Sub($"{vm.Roster.Count} enrolled"));
            foreach (var e in vm.Roster)
            {
                var card = HomeUI.Card();
                card.Add(HomeUI.Title(string.IsNullOrWhiteSpace(e.StudentName) ? "A student" : e.StudentName, 14));
                content.Add(card);
            }
        }

        // ── Screen 6: assignments list ───────────────────────────────────────
        async void ShowAssignments(AcademyClass cls)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => ShowProfile(cls)));
            content.Add(HomeUI.Heading("Assignments"));
            content.Add(HomeUI.Caption(cls.Name));

            if (isTutor)
            {
                var add = HomeUI.Primary("＋ New assignment", () => ShowCreateAssignment(cls));
                add.style.marginBottom = 12;
                content.Add(add);
            }

            var loading = HomeUI.Caption("Loading assignments…");
            content.Add(loading);
            var result = await vm.LoadAssignmentsAsync(cls.Id);
            loading.RemoveFromHierarchy();

            if (result.IsFailure) { content.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
            if (vm.Assignments.Count == 0)
            {
                content.Add(HomeUI.Caption(isTutor ? "No assignments yet. Tap “New assignment”." : "No assignments set yet."));
                return;
            }

            foreach (var a in vm.Assignments)
            {
                var captured = a;
                var card = HomeUI.Card();
                card.Add(HomeUI.Title(a.Title, 15));
                card.Add(HomeUI.Sub("📅 Due " + a.DueDate.ToLocalTime().ToString("ddd dd MMM yyyy")));
                card.Add(HomeUI.Sub("Tap to open"));
                card.RegisterCallback<ClickEvent>(_ => ShowAssignment(cls, captured));
                content.Add(card);
            }
        }

        // ── Screen 7: create an assignment (its own screen) ──────────────────
        void ShowCreateAssignment(AcademyClass cls)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => ShowAssignments(cls)));
            content.Add(HomeUI.Heading("New assignment"));
            content.Add(HomeUI.Caption(cls.Name));

            AssignmentForm(null, out var title, out var instructions, out var due, out var score, out var status);

            var add = HomeUI.Primary("Assign to class", null);
            add.style.marginTop = 8;
            add.clicked += async () =>
            {
                status.style.display = DisplayStyle.None;
                if (!DateTime.TryParse(due.value, out var dueDate)) { Surface(status, "Enter the due date as yyyy-MM-dd."); return; }
                int.TryParse(score.value, out var maxScore);
                add.SetEnabled(false);
                var r = await vm.CreateAssignmentAsync(cls.Id, title.value, instructions.value, dueDate, maxScore);
                add.SetEnabled(true);
                if (r.IsSuccess) ShowAssignments(cls);
                else Surface(status, vm.ErrorMessage);
            };
            content.Add(add);
        }

        // ── Screen 8: assignment profile (data + actions) ────────────────────
        void ShowAssignment(AcademyClass cls, Assignment a)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => ShowAssignments(cls)));
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

            info.Add(HomeUI.FieldLabel("Game content"));
            info.Add(HomeUI.Sub(a.HasContent ? "🎮 Interactive content added." : "No interactive content yet."));

            if (isTutor)
            {
                content.Add(Action_("Edit content", () => OpenContentEditor(cls, a)));
                content.Add(Action_("Edit assignment", () => ShowEditAssignment(cls, a)));
                content.Add(ConfirmDanger("Delete assignment", async () =>
                {
                    var r = await vm.DeleteAssignmentAsync(a.Id);
                    if (r.IsSuccess) ShowAssignments(cls);
                    else content.Add(HomeUI.Status(vm.ErrorMessage, true));
                }));
            }
        }

        void OpenContentEditor(AcademyClass cls, Assignment a)
        {
            var editor = new AssignmentContentEditorView(vm);
            editor.Show(content, a.Id, a.Title, a.ContentJson, () => ShowAssignment(cls, a));
        }

        // ── Screen 9: edit an assignment (its own screen) ────────────────────
        void ShowEditAssignment(AcademyClass cls, Assignment a)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => ShowAssignment(cls, a)));
            content.Add(HomeUI.Heading("Edit assignment"));

            AssignmentForm(a, out var title, out var instructions, out var due, out var score, out var status);

            var save = HomeUI.Primary("Save changes", null);
            save.style.marginTop = 8;
            save.clicked += async () =>
            {
                status.style.display = DisplayStyle.None;
                if (!DateTime.TryParse(due.value, out var dueDate)) { Surface(status, "Enter the due date as yyyy-MM-dd."); return; }
                int.TryParse(score.value, out var maxScore);
                save.SetEnabled(false);
                var r = await vm.UpdateAssignmentAsync(a, title.value, instructions.value, dueDate, maxScore);
                save.SetEnabled(true);
                if (r.IsSuccess) ShowAssignment(cls, a);
                else Surface(status, vm.ErrorMessage);
            };
            content.Add(save);
        }

        // ── Shared form for create/edit assignment ───────────────────────────
        void AssignmentForm(Assignment existing, out TextField title, out TextField instructions,
            out TextField due, out TextField score, out Label status)
        {
            content.Add(HomeUI.FieldLabel("Assignment title"));
            title = HomeUI.Field(existing?.Title);
            content.Add(title);
            content.Add(HomeUI.FieldLabel("Instructions (optional)"));
            instructions = HomeUI.Field(existing?.Instructions, multiline: true);
            content.Add(instructions);
            content.Add(HomeUI.FieldLabel("Due date (yyyy-MM-dd)"));
            var dueDefault = existing?.DueDate.ToLocalTime().ToString("yyyy-MM-dd") ?? DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-dd");
            due = HomeUI.Field(dueDefault);
            content.Add(due);
            content.Add(HomeUI.FieldLabel("Max score (0 = ungraded)"));
            score = HomeUI.Field((existing?.MaxScore ?? 0).ToString());
            content.Add(score);
            status = Hidden(HomeUI.Status("", true));
            content.Add(status);
        }

        // ── Small builders ───────────────────────────────────────────────────

        // A full-width action button used to stack profile actions vertically.
        static Button Action_(string text, Action onClick)
        {
            var b = HomeUI.Outline(text, onClick);
            b.style.marginTop = 8;
            return b;
        }

        // A destructive button that asks for a second tap before firing.
        static Button ConfirmDanger(string text, Action onConfirm)
        {
            var b = HomeUI.Danger_(text, null);
            b.style.marginTop = 8;
            bool armed = false;
            b.clicked += () =>
            {
                if (!armed) { armed = true; b.text = "Tap again to confirm"; return; }
                b.SetEnabled(false);
                onConfirm();
            };
            return b;
        }

        // Mini-nav sub-tab styling (parchment underline when selected).
        static void StyleSubTab(Button tab, bool selected)
        {
            tab.style.backgroundColor = UnityEngine.Color.clear;
            tab.style.color = selected ? HomeUI.Parchment : HomeUI.Muted;
            tab.style.unityFontStyleAndWeight = selected ? UnityEngine.FontStyle.Bold : UnityEngine.FontStyle.Normal;
            tab.style.fontSize = 14;
            tab.style.marginRight = 18;
            tab.style.paddingLeft = tab.style.paddingRight = 0;
            tab.style.paddingTop = tab.style.paddingBottom = 4;
            HomeUI.Border(tab, UnityEngine.Color.clear, 0);
            tab.style.borderBottomWidth = selected ? 2 : 0;
            tab.style.borderBottomColor = HomeUI.Parchment;
        }

        static Label Hidden(Label l) { l.style.display = DisplayStyle.None; return l; }

        static void Surface(Label status, string message)
        {
            status.text = message;
            status.style.display = DisplayStyle.Flex;
        }
    }
}
