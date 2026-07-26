using System;
using UnityEngine;
using UnityEngine.UIElements;
using SReader.Core.Common;
using SReader.Domains.Education.Models;
using SReader.UI.ViewModels;

namespace SReader.UI.Views.Home
{
    /// <summary>
    /// Renders the tutor's Academies CRUD into the home content area:
    ///   "My academies" — your academies; tap one to open its profile
    ///   "All"          — every academy; tap one to open its profile
    ///   "Requests"     — approve / reject students' join requests
    /// "Create" lives in the upper nav (see TutorHomeController.DecorateTopMenu).
    /// An academy's profile is where Edit / Delete / Back live. Self-contained:
    /// every action re-renders into the same content element.
    /// </summary>
    internal sealed class TutorAcademiesView
    {
        readonly AcademyViewModel vm;
        readonly ClassViewModel classVm;
        ClassesView classesView;
        VisualElement content;
        string currentTab = "My academies";

        public TutorAcademiesView(AcademyViewModel vm, ClassViewModel classVm = null)
        {
            this.vm = vm;
            this.classVm = classVm;
        }

        public void Render(VisualElement contentArea, string tab)
        {
            content = contentArea;
            currentTab = tab;
            switch (tab)
            {
                case "All":      ShowAll();      break;
                case "Requests": ShowRequests(); break;
                default:         ShowMine();     break; // "My academies"
            }
        }

        /// <summary>Invoked from the upper-nav "＋ Create" action.</summary>
        public void ShowCreate()
        {
            if (content != null) ShowForm(null);
        }

        // ── My academies ──
        async void ShowMine()
        {
            content.Clear();
            content.Add(HomeUI.Heading("My academies"));

            var loading = HomeUI.Caption("Loading…");
            content.Add(loading);
            var result = await vm.LoadMineAsync();
            loading.RemoveFromHierarchy();

            if (result.IsFailure) { content.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
            if (vm.MyAcademies.Count == 0)
            {
                content.Add(HomeUI.Caption("You haven't created any academies yet. Use “＋ Create” above."));
                return;
            }

            foreach (var academy in vm.MyAcademies)
                content.Add(AcademyCard(academy));
        }

        // ── All academies ──
        async void ShowAll()
        {
            content.Clear();
            content.Add(HomeUI.Heading("All academies"));

            var loading = HomeUI.Caption("Loading…");
            content.Add(loading);
            var result = await vm.LoadAllAsync();
            loading.RemoveFromHierarchy();

            if (result.IsFailure) { content.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
            if (vm.AllAcademies.Count == 0) { content.Add(HomeUI.Caption("No academies have been created yet.")); return; }

            foreach (var academy in vm.AllAcademies)
                content.Add(AcademyCard(academy));
        }

        // A list card: shows a summary and opens the academy profile on click.
        VisualElement AcademyCard(Academy academy)
        {
            var card = HomeUI.Card();
            card.Add(HomeUI.Title(academy.Name));
            if (!string.IsNullOrEmpty(academy.LocationSummary))
                card.Add(HomeUI.Sub("📍 " + academy.LocationSummary));
            if (academy.OwnerId == vm.CurrentUserId)
                card.Add(HomeUI.Sub("• Yours"));

            card.RegisterCallback<ClickEvent>(_ => ShowProfile(academy));
            return card;
        }

        // ── Academy profile: header + mini-nav (About · Grades) ──
        // Keeps the profile uncluttered — each concern lives behind a tab, and
        // actions are compact pill buttons (the login Apple/Google style), not
        // long full-width bars.
        void ShowProfile(Academy academy)
        {
            bool canManage = academy.OwnerId == vm.CurrentUserId;

            content.Clear();
            content.Add(HomeUI.Link("‹ Back", BackToList));
            content.Add(HomeUI.Heading(academy.Name));
            if (!string.IsNullOrEmpty(academy.LocationSummary))
                content.Add(HomeUI.Sub("📍 " + academy.LocationSummary));

            var menu = HomeUI.Row();
            menu.style.marginTop = 8;
            menu.style.marginBottom = 6;
            content.Add(menu);
            var tabBox = new VisualElement();
            content.Add(tabBox);

            var tabs = new[] { "About", "Grades" };
            var buttons = new Button[tabs.Length];
            System.Action<int> select = i =>
            {
                for (int j = 0; j < buttons.Length; j++) HomeUI.StyleTab(buttons[j], j == i);
                tabBox.Clear();
                if (i == 0) RenderAbout(tabBox, academy, canManage);
                else        RenderGrades(tabBox, academy, canManage);
            };
            for (int i = 0; i < tabs.Length; i++)
            {
                int idx = i;
                buttons[i] = new Button(() => select(idx)) { text = tabs[i] };
                menu.Add(buttons[i]);
            }
            select(0);
        }

        // ── "About" tab: description + grouped academy actions ──
        void RenderAbout(VisualElement box, Academy academy, bool canManage)
        {
            var card = HomeUI.Card();
            card.Add(HomeUI.FieldLabel("About"));
            card.Add(HomeUI.Sub(string.IsNullOrWhiteSpace(academy.Description) ? "No description yet." : academy.Description));
            box.Add(card);

            if (canManage)
            {
                var actions = HomeUI.ChipRow(
                    HomeUI.Chip("Edit", () => ShowForm(academy)),
                    HomeUI.Chip("Delete", () => ConfirmDelete(academy), danger: true));
                actions.style.marginTop = 8;
                box.Add(actions);
            }
        }

        // ── Grades on the profile ──
        async void RenderGrades(VisualElement box, Academy academy, bool canManage)
        {
            box.Clear();

            var header = HomeUI.Row();
            var title = HomeUI.Title("Grades offered");
            title.style.flexGrow = 1;
            header.Add(title);
            if (canManage) header.Add(HomeUI.Link("＋ Add", () => ShowAddGrade(academy)));
            box.Add(header);

            var loading = HomeUI.Caption("Loading…");
            box.Add(loading);
            var result = await vm.LoadGradesAsync(academy.Id);
            loading.RemoveFromHierarchy();

            if (result.IsFailure) { box.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
            if (vm.Grades.Count == 0) { box.Add(HomeUI.Caption("No grades listed yet.")); return; }

            foreach (var grade in vm.Grades)
                box.Add(GradeRow(academy, grade, canManage));
        }

        VisualElement GradeRow(Academy academy, AcademyGrade grade, bool canManage)
        {
            var card = HomeUI.Card();
            card.Add(HomeUI.Title(grade.Title, 15));
            card.Add(HomeUI.Sub(StageLabel(grade)));

            // Compact pill actions that wrap onto a second line on narrow screens.
            var actions = HomeUI.WrapRow();
            actions.style.marginTop = 8;

            actions.Add(Pill(grade.UsesModules ? "Modules" : "Subjects", () => ShowCourses(academy, grade, canManage)));

            if (classVm != null)
                actions.Add(Pill(canManage ? "Classes" : "View classes", () => OpenClasses(academy, grade, canManage)));

            if (canManage)
                actions.Add(Pill("Remove", async () =>
                {
                    var r = await vm.DeleteGradeAsync(grade.Id);
                    if (r.IsSuccess) ShowProfile(academy);
                    else card.Add(HomeUI.Status(vm.ErrorMessage, true));
                }, danger: true));

            card.Add(actions);
            return card;
        }

        // A chip with the right-margin/bottom-margin used inside a WrapRow.
        static Button Pill(string text, System.Action onClick, bool danger = false)
        {
            var b = HomeUI.Chip(text, onClick, danger);
            b.style.marginRight = 8;
            b.style.marginBottom = 6;
            return b;
        }

        void OpenClasses(Academy academy, AcademyGrade grade, bool canManage)
        {
            classesView = classesView ?? new ClassesView(classVm, isTutor: canManage);
            classesView.Show(content, academy, grade, vm.CurrentUserId, () => ShowProfile(academy));
        }

        static string StageLabel(AcademyGrade grade)
        {
            if (grade.Stage == EducationStage.University) return "University · program (modules)";
            var cert = BotswanaCurriculum.CertificateFor(grade.Stage, grade.Title);
            return $"{cert} · subjects";
        }

        // ── Add a grade (stage picker → certificate / program) ──
        void ShowAddGrade(Academy academy)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => ShowProfile(academy)));
            content.Add(HomeUI.Heading("Add grade"));

            content.Add(HomeUI.FieldLabel("Stage"));
            var stageRow = HomeUI.Row();
            content.Add(stageRow);

            var detail = new VisualElement();
            content.Add(detail);

            var status = HomeUI.Status("", true);
            status.style.display = DisplayStyle.None;
            content.Add(status);

            var stage = EducationStage.Primary;
            HomeDropdown levelDropdown = null;
            TextField programField = null;

            void RebuildDetail()
            {
                detail.Clear();
                if (stage == EducationStage.Primary)
                {
                    detail.Add(HomeUI.FieldLabel("Grade (PSLE — Standard 1–7)"));
                    levelDropdown = HomeUI.Dropdown(BotswanaCurriculum.PrimaryLevels);
                    detail.Add(levelDropdown);
                    detail.Add(HomeUI.Sub("You'll add subjects to this grade next."));
                }
                else if (stage == EducationStage.Secondary)
                {
                    detail.Add(HomeUI.FieldLabel("Grade (JC: Form 1–3 · BGCSE: Form 4–5)"));
                    levelDropdown = HomeUI.Dropdown(BotswanaCurriculum.SecondaryLevels);
                    detail.Add(levelDropdown);
                    detail.Add(HomeUI.Sub("You'll add subjects to this grade next."));
                }
                else
                {
                    detail.Add(HomeUI.FieldLabel("Program name (e.g. BSc Computer Science)"));
                    programField = HomeUI.Field(null);
                    detail.Add(programField);
                    detail.Add(HomeUI.Sub("You'll add modules to this program next."));
                }
            }

            void PaintStages()
            {
                stageRow.Clear();
                stageRow.Add(Seg("Primary", stage == EducationStage.Primary, () => { stage = EducationStage.Primary; PaintStages(); RebuildDetail(); }));
                stageRow.Add(Seg("Secondary", stage == EducationStage.Secondary, () => { stage = EducationStage.Secondary; PaintStages(); RebuildDetail(); }));
                stageRow.Add(Seg("University", stage == EducationStage.University, () => { stage = EducationStage.University; PaintStages(); RebuildDetail(); }));
            }
            PaintStages();
            RebuildDetail();

            var save = HomeUI.Primary("Add grade", async () =>
            {
                string gradeTitle = stage == EducationStage.University ? programField?.value : levelDropdown?.value;

                var result = await vm.AddGradeAsync(academy.Id, stage, gradeTitle);
                if (result.IsSuccess) ShowProfile(academy);
                else { status.text = vm.ErrorMessage; status.style.display = DisplayStyle.Flex; }
            });
            save.style.marginTop = 10;
            content.Add(save);
        }

        // A small segmented-control option button.
        static Button Seg(string text, bool selected, Action onClick)
        {
            var b = HomeUI.Outline(text, onClick);
            b.style.marginRight = 8;
            b.style.flexGrow = 1;
            if (selected)
            {
                b.style.backgroundColor = HomeUI.Parchment;
                b.style.color = HomeUI.Bg;
                b.style.unityFontStyleAndWeight = FontStyle.Bold;
            }
            return b;
        }

        // ── Subjects (PSLE/JC/BGCSE) or modules (university) under a grade ──
        // Same shape as Classes: a mini-nav (list · New · Search); list cards open
        // a subject profile that carries the Edit / Remove actions.
        void ShowCourses(Academy academy, AcademyGrade grade, bool canManage)
            => ShowCoursesHome(academy, grade, canManage, 0);

        void ShowCoursesHome(Academy academy, AcademyGrade grade, bool canManage, int activeTab)
        {
            string noun = grade.UsesModules ? "Modules" : "Subjects";
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => ShowProfile(academy)));
            content.Add(HomeUI.Heading(grade.Title));

            var tabs = canManage ? new[] { noun, "New", "Search" } : new[] { noun, "Search" };

            var menu = HomeUI.Row();
            menu.style.marginTop = 8;
            menu.style.marginBottom = 6;
            content.Add(menu);
            var tabBox = new VisualElement();
            content.Add(tabBox);

            var buttons = new Button[tabs.Length];
            System.Action<int> select = i =>
            {
                for (int j = 0; j < buttons.Length; j++) HomeUI.StyleTab(buttons[j], j == i);
                tabBox.Clear();
                var nm = tabs[i];
                if (nm == "New")         RenderAddCourse(tabBox, academy, grade, canManage);
                else if (nm == "Search") RenderCourseSearch(tabBox, academy, grade, canManage);
                else                     RenderCourseList(tabBox, academy, grade, canManage, null);
            };
            for (int i = 0; i < tabs.Length; i++)
            {
                int idx = i;
                buttons[i] = new Button(() => select(idx)) { text = tabs[i] };
                menu.Add(buttons[i]);
            }
            select(activeTab);
        }

        async void RenderCourseList(VisualElement box, Academy academy, AcademyGrade grade, bool canManage, string query)
        {
            box.Clear();
            string noun = grade.UsesModules ? "modules" : "subjects";

            var loading = HomeUI.Caption("Loading…");
            box.Add(loading);
            var result = await vm.LoadCoursesAsync(grade.Id);
            loading.RemoveFromHierarchy();

            if (result.IsFailure) { box.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }

            var shown = 0;
            foreach (var course in vm.Courses)
            {
                if (!string.IsNullOrWhiteSpace(query) &&
                    (course.Name ?? "").IndexOf(query.Trim(), System.StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                shown++;

                var captured = course;
                var card = HomeUI.Card();
                card.Add(HomeUI.Title(course.Name, 15));
                card.Add(HomeUI.Sub("💰 " + course.PriceSummary + (course.FreeTrial ? "  ·  Free trial" : "")));
                if (course.HasClass) card.Add(HomeUI.Sub("🏫 " + course.ClassName));
                card.Add(HomeUI.Sub("Tap to open"));
                card.RegisterCallback<ClickEvent>(_ => ShowCourseProfile(academy, grade, captured, canManage));
                box.Add(card);
            }

            if (shown == 0)
                box.Add(HomeUI.Caption(string.IsNullOrWhiteSpace(query)
                    ? (canManage ? $"No {noun} yet. Use the “New” tab to add one." : $"No {noun} yet.")
                    : $"No {noun} match \"{query.Trim()}\"."));
        }

        void RenderCourseSearch(VisualElement box, Academy academy, AcademyGrade grade, bool canManage)
        {
            string noun = grade.UsesModules ? "modules" : "subjects";
            box.Add(HomeUI.FieldLabel($"Search {noun} by name"));
            var field = HomeUI.Field(null);
            box.Add(field);

            var results = new VisualElement();
            var search = HomeUI.Primary("Search", () => RenderCourseList(results, academy, grade, canManage, field.value));
            search.style.marginTop = 4;
            search.style.marginBottom = 8;
            box.Add(search);
            box.Add(results);
        }

        // ── Subject profile: data, then Edit / Remove (compact pills) ──
        void ShowCourseProfile(Academy academy, AcademyGrade grade, AcademyCourse course, bool canManage)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => ShowCoursesHome(academy, grade, canManage, 0)));
            content.Add(HomeUI.Heading(course.Name));

            var info = HomeUI.Card();
            info.Add(HomeUI.FieldLabel("Price"));
            info.Add(HomeUI.Sub("💰 " + course.PriceSummary + (course.FreeTrial ? "  ·  Free trial offered" : "")));
            if (!string.IsNullOrEmpty(course.AvailabilityNote))
            {
                info.Add(HomeUI.FieldLabel("Availability"));
                info.Add(HomeUI.Sub("⏳ " + course.AvailabilityNote));
            }
            if (!string.IsNullOrWhiteSpace(course.Description))
            {
                info.Add(HomeUI.FieldLabel("About"));
                info.Add(HomeUI.Sub(course.Description));
            }
            info.Add(HomeUI.FieldLabel("Class"));
            info.Add(HomeUI.Sub(course.HasClass
                ? "🏫 " + course.ClassName
                : "Not assigned to a class yet — add it to a class so paid students join it."));
            content.Add(info);

            if (canManage)
            {
                var actions = HomeUI.ChipRow(
                    HomeUI.Chip("Edit / price", () => ShowEditCourse(academy, grade, course, canManage)),
                    HomeUI.Chip("Remove", async () =>
                    {
                        var r = await vm.DeleteCourseAsync(course.Id);
                        if (r.IsSuccess) ShowCoursesHome(academy, grade, canManage, 0);
                        else content.Add(HomeUI.Status(vm.ErrorMessage, true));
                    }, danger: true));
                actions.style.marginTop = 8;
                content.Add(actions);
            }
        }

        void ShowEditCourse(Academy academy, AcademyGrade grade, AcademyCourse course, bool canManage)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => ShowCourseProfile(academy, grade, course, canManage)));
            content.Add(HomeUI.Heading(grade.UsesModules ? "Edit module" : "Edit subject"));

            content.Add(HomeUI.FieldLabel("Name"));
            var name = HomeUI.Field(course.Name);
            content.Add(name);
            content.Add(HomeUI.FieldLabel("Description"));
            var description = HomeUI.Field(course.Description, multiline: true);
            content.Add(description);
            content.Add(HomeUI.FieldLabel("Price (Pula)"));
            var price = HomeUI.Field(course.Price > 0 ? course.Price.ToString("0.##") : "");
            content.Add(price);
            content.Add(HomeUI.FieldLabel("Time frame (e.g. per month, per term, 8 weeks)"));
            var timeFrame = HomeUI.Field(course.TimeFrame);
            content.Add(timeFrame);
            var freeTrial = new Toggle("Offer a free trial") { value = course.FreeTrial };
            freeTrial.style.marginTop = 6;
            freeTrial.style.color = HomeUI.Text;
            content.Add(freeTrial);
            content.Add(HomeUI.FieldLabel("Deadline (YYYY-MM-DD, optional — closes after this date)"));
            var deadline = HomeUI.Field(course.Deadline);
            content.Add(deadline);

            var status = HomeUI.Status("", true);
            status.style.display = DisplayStyle.None;
            content.Add(status);

            var save = HomeUI.Primary("Save", async () =>
            {
                course.Name = name.value;
                course.Description = description.value;
                course.Price = ParsePrice(price.value);
                course.TimeFrame = timeFrame.value;
                course.FreeTrial = freeTrial.value;
                course.Deadline = deadline.value;
                var result = await vm.UpdateCourseAsync(course);
                if (result.IsSuccess) ShowCourseProfile(academy, grade, course, canManage);
                else { status.text = vm.ErrorMessage; status.style.display = DisplayStyle.Flex; }
            });
            save.style.marginTop = 8;
            content.Add(save);
        }

        // ── Tutor payment details (single source of truth) ──
        public void ShowPaymentDetails()
        {
            if (content == null) return;
            RenderPaymentDetails();
        }

        async void RenderPaymentDetails()
        {
            content.Clear();
            content.Add(HomeUI.Heading("Payment details"));
            content.Add(HomeUI.Caption("Students see these to pay for your subjects/modules, then submit proof."));

            var loading = HomeUI.Caption("Loading…");
            content.Add(loading);
            await vm.LoadMyPaymentDetailsAsync();
            loading.RemoveFromHierarchy();

            content.Add(HomeUI.FieldLabel("Account holder name"));
            var accountName = HomeUI.Field(vm.Payment.AccountName);
            content.Add(accountName);
            content.Add(HomeUI.FieldLabel("FNB account number"));
            var fnb = HomeUI.Field(vm.Payment.FnbAccount);
            content.Add(fnb);
            content.Add(HomeUI.FieldLabel("Orange Money number"));
            var orange = HomeUI.Field(vm.Payment.OrangeMoney);
            content.Add(orange);

            var status = HomeUI.Status("", true);
            status.style.display = DisplayStyle.None;
            content.Add(status);

            var save = HomeUI.Primary("Save payment details", async () =>
            {
                var result = await vm.SavePaymentDetailsAsync(accountName.value, fnb.value, orange.value);
                if (result.IsSuccess) { status.text = "Saved."; status.style.color = HomeUI.Success; status.style.display = DisplayStyle.Flex; }
                else { status.text = vm.ErrorMessage; status.style.color = HomeUI.Danger; status.style.display = DisplayStyle.Flex; }
            });
            save.style.marginTop = 10;
            content.Add(save);
        }

        // The add subject/module form, rendered into the "New" tab.
        void RenderAddCourse(VisualElement box, Academy academy, AcademyGrade grade, bool canManage)
        {
            var status = HomeUI.Status("", true);
            status.style.display = DisplayStyle.None;

            if (grade.UsesModules)
            {
                // University: free-text module (e.g. Java module, C++ module).
                box.Add(HomeUI.FieldLabel("Module name"));
                var name = HomeUI.Field(null);
                box.Add(name);
                box.Add(HomeUI.FieldLabel("Description"));
                var description = HomeUI.Field(null, multiline: true);
                box.Add(description);

                var price = AddPriceFields(box, out var timeFrame, out var freeTrial, out var deadline);
                box.Add(status);

                var save = HomeUI.Primary("Add module", async () =>
                {
                    var result = await vm.AddCourseAsync(grade.Id, name.value, description.value, ParsePrice(price.value), timeFrame.value, freeTrial.value, deadline.value);
                    if (result.IsSuccess) ShowCoursesHome(academy, grade, canManage, 0);
                    else { status.text = vm.ErrorMessage; status.style.display = DisplayStyle.Flex; }
                });
                save.style.marginTop = 8;
                box.Add(save);
            }
            else
            {
                // PSLE / JC / BGCSE: pick a subject from the Botswana catalogue.
                box.Add(HomeUI.FieldLabel("Subject"));
                var subject = HomeUI.Dropdown(BotswanaCurriculum.Subjects);
                box.Add(subject);

                var price = AddPriceFields(box, out var timeFrame, out var freeTrial, out var deadline);
                box.Add(status);

                var save = HomeUI.Primary("Add subject", async () =>
                {
                    var result = await vm.AddCourseAsync(grade.Id, subject.value, null, ParsePrice(price.value), timeFrame.value, freeTrial.value, deadline.value);
                    if (result.IsSuccess) ShowCoursesHome(academy, grade, canManage, 0);
                    else { status.text = vm.ErrorMessage; status.style.display = DisplayStyle.Flex; }
                });
                save.style.marginTop = 8;
                box.Add(save);
            }
        }

        // Adds the shared pricing inputs (price, time frame, free trial, deadline)
        // to the given container and returns the price field; the rest via out params.
        TextField AddPriceFields(VisualElement box, out TextField timeFrame, out Toggle freeTrial, out TextField deadline)
        {
            box.Add(HomeUI.FieldLabel("Price (Pula)"));
            var price = HomeUI.Field(null);
            box.Add(price);
            box.Add(HomeUI.FieldLabel("Time frame (e.g. per month, per term, 8 weeks)"));
            timeFrame = HomeUI.Field(null);
            box.Add(timeFrame);
            freeTrial = new Toggle("Offer a free trial") { value = false };
            freeTrial.style.marginTop = 6;
            freeTrial.style.color = HomeUI.Text;
            box.Add(freeTrial);
            box.Add(HomeUI.FieldLabel("Deadline (YYYY-MM-DD, optional — closes after this date)"));
            deadline = HomeUI.Field(null);
            box.Add(deadline);
            return price;
        }

        static double ParsePrice(string s) => double.TryParse(s, out var p) ? p : 0;

        void ConfirmDelete(Academy academy)
        {
            content.Clear();
            content.Add(HomeUI.Heading("Delete academy"));
            content.Add(HomeUI.Caption($"Delete “{academy.Name}”? This can't be undone."));

            var yes = HomeUI.Danger_("Yes, delete", async () =>
            {
                var result = await vm.DeleteAsync(academy.Id);
                if (result.IsSuccess) BackToList();
                else content.Add(HomeUI.Status(vm.ErrorMessage, true));
            });
            yes.style.marginBottom = 8;
            content.Add(yes);
            content.Add(HomeUI.Outline("Cancel", () => ShowProfile(academy)));
        }

        // ── Create / edit form ──
        void ShowForm(Academy existing)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => { if (existing == null) BackToList(); else ShowProfile(existing); }));
            content.Add(HomeUI.Heading(existing == null ? "Create academy" : "Edit academy"));

            content.Add(HomeUI.FieldLabel("Name"));
            var name = HomeUI.Field(existing?.Name);
            content.Add(name);

            content.Add(HomeUI.FieldLabel("Description"));
            var description = HomeUI.Field(existing?.Description, multiline: true);
            content.Add(description);

            content.Add(HomeUI.FieldLabel("City"));
            var city = HomeUI.Field(existing?.City);
            content.Add(city);

            content.Add(HomeUI.FieldLabel("Country"));
            var country = HomeUI.Field(existing?.Country);
            content.Add(country);

            var status = HomeUI.Status("", true);
            status.style.display = DisplayStyle.None;
            content.Add(status);

            var save = HomeUI.Primary(existing == null ? "Create" : "Save changes", async () =>
            {
                Result result;
                if (existing == null)
                {
                    result = await vm.CreateAsync(name.value, description.value, city.value, country.value);
                }
                else
                {
                    existing.Name = name.value;
                    existing.Description = description.value;
                    existing.City = city.value;
                    existing.Country = country.value;
                    result = await vm.UpdateAsync(existing);
                }

                if (result.IsSuccess)
                {
                    if (existing == null) BackToList();
                    else ShowProfile(existing);
                }
                else { status.text = vm.ErrorMessage; status.style.display = DisplayStyle.Flex; }
            });
            save.style.marginTop = 8;
            save.style.marginBottom = 8;
            content.Add(save);
        }

        void BackToList()
        {
            if (currentTab == "All") ShowAll();
            else ShowMine();
        }

        // ── Requests: paid enrollments first, then academy join requests ──
        async void ShowRequests()
        {
            content.Clear();
            content.Add(HomeUI.Heading("Enrollment requests"));
            content.Add(HomeUI.Caption("Confirm a student once their payment proof checks out."));

            var loading = HomeUI.Caption("Loading…");
            content.Add(loading);
            var enrollResult = await vm.LoadOwnerEnrollmentsAsync();
            loading.RemoveFromHierarchy();

            if (enrollResult.IsFailure) content.Add(HomeUI.Status(vm.ErrorMessage, true));
            else if (vm.Enrollments.Count == 0) content.Add(HomeUI.Caption("No enrollment requests yet."));
            else foreach (var e in vm.Enrollments) content.Add(EnrollmentCard(e));

            // Academy-level join requests (membership) below.
            content.Add(HomeUI.Heading("Academy join requests"));
            var loading2 = HomeUI.Caption("Loading…");
            content.Add(loading2);
            var joinResult = await vm.LoadRequestsAsync();
            loading2.RemoveFromHierarchy();

            if (joinResult.IsFailure) content.Add(HomeUI.Status(vm.ErrorMessage, true));
            else if (vm.Requests.Count == 0) content.Add(HomeUI.Caption("No join requests yet."));
            else foreach (var request in vm.Requests) content.Add(RequestCard(request));
        }

        VisualElement EnrollmentCard(CourseEnrollmentRequest e)
        {
            var card = HomeUI.Card();
            card.Add(HomeUI.Title(e.StudentName));
            card.Add(HomeUI.Sub($"{e.CourseName} · {e.GradeTitle} · {e.AcademyName}"));
            card.Add(HomeUI.Sub((e.IsFreeTrial ? "🎁 Free trial · " : "💳 Paid · ") + "Status: " + e.Status));
            if (!string.IsNullOrWhiteSpace(e.PaymentReference))
                card.Add(HomeUI.Sub("Reference: " + e.PaymentReference));

            // View the uploaded proof image before enrolling.
            if (e.HasProof)
            {
                var imageBox = HomeUI.ImageBox();
                imageBox.style.display = DisplayStyle.None;
                var view = HomeUI.Outline("View payment proof", () =>
                {
                    imageBox.style.display = DisplayStyle.Flex;
                    HomeUI.LoadImageInto(imageBox, e.PaymentProofUrl);
                });
                view.style.marginTop = 8;
                card.Add(view);
                card.Add(imageBox);
            }
            else if (!e.IsFreeTrial && e.Status == EnrollmentStatus.PaymentSubmitted)
            {
                card.Add(HomeUI.Sub("No proof image attached."));
            }

            // Paid students may only be enrolled AFTER they submit payment;
            // free-trial students can be enrolled straight away.
            bool canEnroll = e.IsFreeTrial ? e.Status == EnrollmentStatus.Requested
                                           : e.Status == EnrollmentStatus.PaymentSubmitted;
            bool canReject = e.Status == EnrollmentStatus.Requested || e.Status == EnrollmentStatus.PaymentSubmitted;

            if (!e.IsFreeTrial && e.Status == EnrollmentStatus.Requested)
                card.Add(HomeUI.Sub("Awaiting the student's payment before you can enroll."));

            if (canEnroll || canReject)
            {
                var actions = HomeUI.Row();
                actions.style.marginTop = 10;
                if (canEnroll)
                {
                    var enroll = HomeUI.Primary("Enroll", async () =>
                    {
                        var r = await vm.ConfirmEnrollmentAsync(e);
                        if (r.IsSuccess) ShowRequests();
                        else card.Add(HomeUI.Status(vm.ErrorMessage, true));
                    });
                    enroll.style.marginRight = 8;
                    enroll.style.flexGrow = 1;
                    actions.Add(enroll);
                }
                if (canReject)
                {
                    var reject = HomeUI.Danger_("Reject", async () =>
                    {
                        var r = await vm.RejectEnrollmentAsync(e.Id);
                        if (r.IsSuccess) ShowRequests();
                        else card.Add(HomeUI.Status(vm.ErrorMessage, true));
                    });
                    reject.style.flexGrow = 1;
                    actions.Add(reject);
                }
                card.Add(actions);
            }
            return card;
        }

        VisualElement RequestCard(AcademyJoinRequest request)
        {
            var card = HomeUI.Card();
            card.Add(HomeUI.Title(request.StudentName));
            card.Add(HomeUI.Sub($"wants to join {request.AcademyName}"));

            if (request.Status == JoinRequestStatus.Pending)
            {
                var actions = HomeUI.Row();
                actions.style.marginTop = 10;
                var approve = HomeUI.Primary("Approve", async () =>
                {
                    var r = await vm.ApproveAsync(request.Id);
                    if (r.IsSuccess) ShowRequests();
                    else card.Add(HomeUI.Status(vm.ErrorMessage, true));
                });
                approve.style.marginRight = 8;
                approve.style.flexGrow = 1;
                actions.Add(approve);
                var reject = HomeUI.Danger_("Reject", async () =>
                {
                    var r = await vm.RejectAsync(request.Id);
                    if (r.IsSuccess) ShowRequests();
                    else card.Add(HomeUI.Status(vm.ErrorMessage, true));
                });
                reject.style.flexGrow = 1;
                actions.Add(reject);
                card.Add(actions);
            }
            else
            {
                card.Add(HomeUI.Status(request.Status.ToString(), request.Status == JoinRequestStatus.Rejected));
            }
            return card;
        }
    }
}
