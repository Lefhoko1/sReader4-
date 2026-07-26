using UnityEngine.UIElements;
using SReader.Domains.Education.Models;
using SReader.UI.ViewModels;

namespace SReader.UI.Views.Home
{
    /// <summary>
    /// Student side of academies: "Discover" lists every academy; tapping one
    /// opens a read-only profile showing the grades it offers (and a grade's
    /// courses), with a "Request to join" button. "Enrolled" is a placeholder.
    /// </summary>
    internal sealed class StudentAcademiesView
    {
        readonly AcademyViewModel vm;
        readonly ClassViewModel classVm;
        ClassesView classesView;
        VisualElement content;
        string studentName;

        public StudentAcademiesView(AcademyViewModel vm, ClassViewModel classVm = null)
        {
            this.vm = vm;
            this.classVm = classVm;
        }

        public void Render(VisualElement contentArea, string tab, string studentName)
        {
            content = contentArea;
            this.studentName = studentName;

            if (tab == "Enrolled")
            {
                ShowMyRequests();
                return;
            }

            ShowDiscover();
        }

        async void ShowDiscover()
        {
            content.Clear();
            content.Add(HomeUI.Heading("Discover academies"));

            var loading = HomeUI.Caption("Loading…");
            content.Add(loading);
            var result = await vm.LoadAllAsync();
            loading.RemoveFromHierarchy();

            if (result.IsFailure) { content.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
            if (vm.AllAcademies.Count == 0) { content.Add(HomeUI.Caption("No academies available yet.")); return; }

            foreach (var academy in vm.AllAcademies)
            {
                var card = HomeUI.Card();
                card.Add(HomeUI.Title(academy.Name));
                if (!string.IsNullOrEmpty(academy.LocationSummary))
                    card.Add(HomeUI.Sub("📍 " + academy.LocationSummary));
                card.Add(HomeUI.Sub("Tap to view grades & courses"));
                var captured = academy;
                card.RegisterCallback<ClickEvent>(_ => ShowProfile(captured));
                content.Add(card);
            }
        }

        // ── Read-only academy profile + request to join ──
        void ShowProfile(Academy academy)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", ShowDiscover));
            content.Add(HomeUI.Heading(academy.Name));
            if (!string.IsNullOrEmpty(academy.LocationSummary))
                content.Add(HomeUI.Sub("📍 " + academy.LocationSummary));

            if (!string.IsNullOrWhiteSpace(academy.Description))
            {
                var about = HomeUI.Card();
                about.Add(HomeUI.FieldLabel("About"));
                about.Add(HomeUI.Sub(academy.Description));
                content.Add(about);
            }

            var join = HomeUI.Primary("Request to join", null);
            join.style.marginTop = 8;
            join.style.marginBottom = 8;
            join.clicked += async () =>
            {
                join.SetEnabled(false);
                var r = await vm.RequestToJoinAsync(academy, studentName);
                if (r.IsSuccess) { join.text = "Requested ✓"; }
                else { join.text = "Request to join"; join.SetEnabled(true); content.Add(HomeUI.Status(vm.ErrorMessage, true)); }
            };
            content.Add(join);

            var gradesBox = new VisualElement();
            content.Add(gradesBox);
            RenderGrades(gradesBox, academy);
        }

        async void RenderGrades(VisualElement box, Academy academy)
        {
            box.Clear();
            box.Add(HomeUI.Title("Grades offered"));

            var loading = HomeUI.Caption("Loading…");
            box.Add(loading);
            var result = await vm.LoadGradesAsync(academy.Id);
            loading.RemoveFromHierarchy();

            if (result.IsFailure) { box.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
            if (vm.Grades.Count == 0) { box.Add(HomeUI.Caption("No grades listed yet.")); return; }

            foreach (var grade in vm.Grades)
            {
                var card = HomeUI.Card();
                card.Add(HomeUI.Title(grade.Title, 15));
                card.Add(HomeUI.Sub(StageLabel(grade)));
                var captured = grade;

                var actions = HomeUI.WrapRow();
                actions.style.marginTop = 8;
                actions.Add(Pill(grade.UsesModules ? "Modules" : "Subjects", () => ShowCourses(academy, captured)));
                if (classVm != null)
                    actions.Add(Pill("Classes", () => OpenClasses(academy, captured)));
                card.Add(actions);
                box.Add(card);
            }
        }

        void OpenClasses(Academy academy, AcademyGrade grade)
        {
            classesView = classesView ?? new ClassesView(classVm, isTutor: false);
            classesView.Show(content, academy, grade, studentName, () => ShowProfile(academy));
        }

        // Subjects under a grade, with the same shape as the tutor side: a
        // mini-nav (Subjects · Search), list cards opening a subject profile that
        // carries the Request / free-trial actions.
        void ShowCourses(Academy academy, AcademyGrade grade)
            => ShowCoursesHome(academy, grade, 0);

        void ShowCoursesHome(Academy academy, AcademyGrade grade, int activeTab)
        {
            string noun = grade.UsesModules ? "Modules" : "Subjects";
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => ShowProfile(academy)));
            content.Add(HomeUI.Heading(grade.Title));

            var tabs = new[] { noun, "Search" };
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
                if (tabs[i] == "Search") RenderCourseSearch(tabBox, academy, grade);
                else                     RenderCourseList(tabBox, academy, grade, null);
            };
            for (int i = 0; i < tabs.Length; i++)
            {
                int idx = i;
                buttons[i] = new Button(() => select(idx)) { text = tabs[i] };
                menu.Add(buttons[i]);
            }
            select(activeTab);
        }

        async void RenderCourseList(VisualElement box, Academy academy, AcademyGrade grade, string query)
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
                card.Add(HomeUI.Sub("💰 " + course.PriceSummary + (course.FreeTrial ? "  ·  Free trial available" : "")));
                if (!course.IsAvailable) card.Add(HomeUI.Sub("⏳ Closed"));
                card.Add(HomeUI.Sub("Tap to open"));
                card.RegisterCallback<ClickEvent>(_ => ShowCourseProfile(academy, grade, captured));
                box.Add(card);
            }

            if (shown == 0)
                box.Add(HomeUI.Caption(string.IsNullOrWhiteSpace(query)
                    ? $"No {noun} listed yet."
                    : $"No {noun} match \"{query.Trim()}\"."));
        }

        void RenderCourseSearch(VisualElement box, Academy academy, AcademyGrade grade)
        {
            string noun = grade.UsesModules ? "modules" : "subjects";
            box.Add(HomeUI.FieldLabel($"Search {noun} by name"));
            var field = HomeUI.Field(null);
            box.Add(field);

            var results = new VisualElement();
            var search = HomeUI.Primary("Search", () => RenderCourseList(results, academy, grade, field.value));
            search.style.marginTop = 4;
            search.style.marginBottom = 8;
            box.Add(search);
            box.Add(results);
        }

        // ── Subject profile: details, then the Request / free-trial actions ──
        void ShowCourseProfile(Academy academy, AcademyGrade grade, AcademyCourse course)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => ShowCoursesHome(academy, grade, 0)));
            content.Add(HomeUI.Heading(course.Name));

            var info = HomeUI.Card();
            info.Add(HomeUI.FieldLabel("Price"));
            info.Add(HomeUI.Sub("💰 " + course.PriceSummary + (course.FreeTrial ? "  ·  Free trial available" : "")));
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
            content.Add(info);

            var status = HomeUI.Status("", true);
            status.style.display = DisplayStyle.None;
            content.Add(status);

            if (!course.IsAvailable)
            {
                content.Add(HomeUI.Caption("This subject is closed — its deadline has passed."));
                return;
            }

            var request = HomeUI.Chip("Request (pay)", null);
            request.clicked += async () =>
            {
                request.SetEnabled(false);
                var r = await vm.RequestEnrollmentAsync(course, academy, grade.Title, studentName, false);
                if (r.IsSuccess) Surface(status, "Requested ✓ — open Enrolled to pay.", false);
                else { request.SetEnabled(true); Surface(status, vm.ErrorMessage, true); }
            };

            VisualElement actions;
            if (course.FreeTrial)
            {
                var trial = HomeUI.Chip("Start free trial", null);
                trial.clicked += async () =>
                {
                    trial.SetEnabled(false);
                    var r = await vm.RequestEnrollmentAsync(course, academy, grade.Title, studentName, true);
                    if (r.IsSuccess) Surface(status, "Free trial requested ✓.", false);
                    else { trial.SetEnabled(true); Surface(status, vm.ErrorMessage, true); }
                };
                actions = HomeUI.ChipRow(request, trial);
            }
            else
            {
                actions = HomeUI.WrapRow();
                actions.Add(request);
            }
            actions.style.marginTop = 8;
            content.Add(actions);
        }

        static void Surface(Label status, string message, bool isError)
        {
            status.text = message;
            status.style.color = isError ? HomeUI.Danger : HomeUI.Success;
            status.style.display = DisplayStyle.Flex;
        }

        // A chip with the margins used inside a WrapRow.
        static Button Pill(string text, System.Action onClick, bool danger = false)
        {
            var b = HomeUI.Chip(text, onClick, danger);
            b.style.marginRight = 8;
            b.style.marginBottom = 6;
            return b;
        }

        // ── My enrollment requests + payment submission ──
        async void ShowMyRequests()
        {
            content.Clear();
            content.Add(HomeUI.Heading("My requests"));
            content.Add(HomeUI.Caption("Pay the tutor, then submit your proof. They enroll you once confirmed."));

            var loading = HomeUI.Caption("Loading…");
            content.Add(loading);
            var result = await vm.LoadMyEnrollmentsAsync();
            loading.RemoveFromHierarchy();

            if (result.IsFailure) { content.Add(HomeUI.Status(vm.ErrorMessage, true)); return; }
            if (vm.Enrollments.Count == 0) { content.Add(HomeUI.Caption("No requests yet. Find a subject/module under Discover.")); return; }

            foreach (var e in vm.Enrollments)
            {
                var captured = e;
                var card = HomeUI.Card();
                card.Add(HomeUI.Title(e.CourseName, 15));
                card.Add(HomeUI.Sub($"{e.GradeTitle} · {e.AcademyName}"));
                card.Add(HomeUI.Sub((e.IsFreeTrial ? "🎁 Free trial · " : "💳 Paid · ") + StatusText(e.Status, e.IsFreeTrial)));

                // Paid requests need payment; free-trial requests just wait for the tutor.
                if (e.Status == EnrollmentStatus.Requested && !e.IsFreeTrial)
                {
                    var pay = HomeUI.Primary("Pay & submit proof", () => ShowPay(captured));
                    pay.style.marginTop = 8;
                    card.Add(pay);
                }
                content.Add(card);
            }
        }

        async void ShowPay(CourseEnrollmentRequest request)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", ShowMyRequests));
            content.Add(HomeUI.Heading("Pay for " + request.CourseName));

            var loading = HomeUI.Caption("Loading payment details…");
            content.Add(loading);
            await vm.LoadPaymentDetailsForAsync(request.OwnerId);
            loading.RemoveFromHierarchy();

            var details = HomeUI.Card();
            details.Add(HomeUI.FieldLabel("Pay the tutor using"));
            if (!string.IsNullOrWhiteSpace(vm.Payment.AccountName)) details.Add(HomeUI.Sub("Name: " + vm.Payment.AccountName));
            if (!string.IsNullOrWhiteSpace(vm.Payment.FnbAccount)) details.Add(HomeUI.Sub("FNB: " + vm.Payment.FnbAccount));
            if (!string.IsNullOrWhiteSpace(vm.Payment.OrangeMoney)) details.Add(HomeUI.Sub("Orange Money: " + vm.Payment.OrangeMoney));
            if (vm.Payment.IsEmpty) details.Add(HomeUI.Sub("The tutor hasn't added payment details yet — check back soon."));
            content.Add(details);

            content.Add(HomeUI.FieldLabel("Payment reference / note (optional)"));
            var reference = HomeUI.Field(null);
            content.Add(reference);

            content.Add(HomeUI.FieldLabel("Proof of payment (image)"));
            string proofUrl = null;
            var preview = HomeUI.ImageBox(200);
            preview.style.display = DisplayStyle.None;

            var status = HomeUI.Status("", true);
            status.style.display = DisplayStyle.None;

            var attach = HomeUI.Outline("Attach proof image", null);
            attach.clicked += async () =>
            {
                status.style.display = DisplayStyle.None;
                attach.SetEnabled(false);
                attach.text = "Uploading…";
                var r = await vm.PickAndUploadProofAsync(request.Id);
                if (r.IsSuccess)
                {
                    proofUrl = r.Value;
                    attach.text = "Proof attached ✓ — tap to change";
                    attach.SetEnabled(true);
                    preview.style.display = DisplayStyle.Flex;
                    HomeUI.LoadImageInto(preview, proofUrl);
                }
                else
                {
                    attach.text = "Attach proof image";
                    attach.SetEnabled(true);
                    // Surface the real reason (e.g. missing bucket / permission).
                    if (!string.IsNullOrEmpty(vm.ErrorMessage))
                    {
                        status.text = "Upload failed: " + vm.ErrorMessage;
                        status.style.display = DisplayStyle.Flex;
                    }
                }
            };
            content.Add(attach);
            content.Add(preview);
            content.Add(status);

            var submit = HomeUI.Primary("Submit payment proof", async () =>
            {
                if (string.IsNullOrEmpty(proofUrl))
                {
                    status.text = "Attach your proof-of-payment image first.";
                    status.style.display = DisplayStyle.Flex;
                    return;
                }
                var r = await vm.SubmitPaymentAsync(request.Id, reference.value, proofUrl);
                if (r.IsSuccess) ShowMyRequests();
                else { status.text = vm.ErrorMessage; status.style.display = DisplayStyle.Flex; }
            });
            submit.style.marginTop = 8;
            content.Add(submit);
        }

        static string StatusText(EnrollmentStatus status, bool isFreeTrial)
        {
            switch (status)
            {
                case EnrollmentStatus.Requested:        return isFreeTrial ? "Requested — awaiting tutor" : "Requested — awaiting your payment";
                case EnrollmentStatus.PaymentSubmitted: return "Payment submitted — awaiting tutor";
                case EnrollmentStatus.Enrolled:         return "Enrolled ✓";
                default:                                return "Rejected";
            }
        }

        static string StageLabel(AcademyGrade grade)
        {
            if (grade.Stage == EducationStage.University) return "University · program (modules)";
            var cert = BotswanaCurriculum.CertificateFor(grade.Stage, grade.Title);
            return $"{cert} · subjects";
        }
    }
}
