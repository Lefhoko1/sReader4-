using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using SReader.Domains.Assignments.Models;
using SReader.UI.ViewModels;

namespace SReader.UI.Views.Home
{
    /// <summary>
    /// Tutor authoring for the assignment game. A page's text is typed/pasted and
    /// tokenised into words; tapping a word attaches an activity (Define /
    /// Illustrate / Fill-the-blanks). Saved as content_json, which the game then
    /// plays. (When the PDF importer lands it will produce the same
    /// <see cref="AssignmentContent"/>; this stays the manual/edit path.)
    /// </summary>
    internal sealed class AssignmentContentEditorView
    {
        readonly ClassViewModel vm;
        VisualElement content;
        AssignmentSummary target;   // id + title
        AssignmentContent working;
        Action onBack;

        public AssignmentContentEditorView(ClassViewModel vm) => this.vm = vm;

        public void Show(VisualElement contentArea, string assignmentId, string title, string existingJson, Action onBack)
        {
            this.content = contentArea;
            this.target = new AssignmentSummary { Id = assignmentId, Title = title };
            this.working = AssignmentContentCodec.FromJson(existingJson) ?? new AssignmentContent();
            this.onBack = onBack;
            ShowHome();
        }

        // ── Overview ─────────────────────────────────────────────────────────
        void ShowHome()
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => onBack?.Invoke()));
            content.Add(HomeUI.Heading("Content"));
            content.Add(HomeUI.Caption(target.Title));

            var summary = HomeUI.Card();
            summary.Add(HomeUI.FieldLabel("This assignment"));
            summary.Add(HomeUI.Sub($"📄 {working.pages.Count} page(s)  ·  🎯 {working.ActivityCount} activities  ·  ⭐ {working.TotalPoints} pts"));
            content.Add(summary);

            var add = HomeUI.Chip("＋ Add page from text", () => ShowAddPage());
            add.style.marginBottom = 8;
            content.Add(add);

            foreach (var page in working.pages)
            {
                var captured = page;
                var card = HomeUI.Card();
                card.Add(HomeUI.Title("Page " + page.pageNumber, 15));
                card.Add(HomeUI.Sub($"{page.ActivityTokens.Count()} activities"));
                var row = HomeUI.WrapRow();
                row.style.marginTop = 8;
                var edit = HomeUI.Chip("Edit words", () => ShowPageWords(captured));
                edit.style.marginRight = 8; edit.style.marginBottom = 6;
                row.Add(edit);
                var remove = HomeUI.Chip("Remove page", () => { working.pages.Remove(captured); ShowHome(); }, danger: true);
                remove.style.marginBottom = 6;
                row.Add(remove);
                card.Add(row);
                content.Add(card);
            }

            var status = HomeUI.Status("", true);
            status.style.display = DisplayStyle.None;
            content.Add(status);

            var save = HomeUI.Primary("Save content", async () =>
            {
                var r = await vm.SaveAssignmentContentAsync(target.Id, working);
                status.style.display = DisplayStyle.Flex;
                status.text = r.IsSuccess ? "Saved ✓" : vm.ErrorMessage;
                status.style.color = r.IsSuccess ? HomeUI.Success : HomeUI.Danger;
            });
            save.style.marginTop = 10;
            content.Add(save);
        }

        // ── Add a page from typed text ───────────────────────────────────────
        void ShowAddPage()
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", ShowHome));
            content.Add(HomeUI.Heading("Add page"));
            content.Add(HomeUI.Caption("Paste or type the page text. You'll tap words to add activities next."));

            content.Add(HomeUI.FieldLabel("Page text"));
            var text = HomeUI.Field(null, multiline: true);
            text.style.minHeight = 160;
            content.Add(text);

            var addBtn = HomeUI.Primary("Tokenise into a page", () =>
            {
                var pageNumber = working.pages.Count + 1;
                var page = AssignmentContentBuilder.PageFromText(pageNumber, text.value);
                working.pages.Add(page);
                ShowPageWords(page);
            });
            addBtn.style.marginTop = 8;
            content.Add(addBtn);
        }

        // ── Tap a word to attach an activity ─────────────────────────────────
        void ShowPageWords(ContentPage page)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", ShowHome));
            content.Add(HomeUI.Heading("Page " + page.pageNumber));
            content.Add(HomeUI.Caption("Tap a word to add or change its activity. Highlighted words already have one."));

            foreach (var sentence in page.sentences)
            {
                var line = HomeUI.WrapRow();
                line.style.marginBottom = 8;
                foreach (var token in sentence.tokens)
                {
                    if (!token.isWord) continue;   // skip spaces/punctuation in the editor
                    var captured = token;
                    var chip = HomeUI.Chip(token.text + ActivityBadge(token), () => ShowWordEditor(page, captured), false);
                    chip.style.marginRight = 6;
                    chip.style.marginBottom = 6;
                    if (token.IsActivity) { chip.style.backgroundColor = HomeUI.Parchment; chip.style.color = HomeUI.Bg; }
                    line.Add(chip);
                }
                content.Add(line);
            }
        }

        static string ActivityBadge(ContentToken t)
        {
            switch (t.activity)
            {
                case ActivityType.Define:     return "  ·  define";
                case ActivityType.Illustrate: return "  ·  image";
                case ActivityType.FillBlank:  return "  ·  fill";
                default:                      return "";
            }
        }

        // ── Per-word activity editor ─────────────────────────────────────────
        void ShowWordEditor(ContentPage page, ContentToken token)
        {
            content.Clear();
            content.Add(HomeUI.Link("‹ Back", () => ShowPageWords(page)));
            content.Add(HomeUI.Heading("Word: " + token.text));

            var chosen = token.activity;
            var detail = new VisualElement();

            // Per-type fields, captured so Apply can read them.
            TextField definition = null, correctImage = null, distractors = null, points = null;

            void Rebuild()
            {
                detail.Clear();
                points = HomeUI.Field(token.points > 0 ? token.points.ToString() : "10");

                if (chosen == ActivityType.Define)
                {
                    detail.Add(HomeUI.FieldLabel("Definition (the student arranges these words)"));
                    definition = HomeUI.Field(token.definition, multiline: true);
                    detail.Add(definition);
                }
                else if (chosen == ActivityType.FillBlank)
                {
                    detail.Add(HomeUI.Sub("The student rearranges this word's letters to spell it. No extra input needed."));
                }
                else if (chosen == ActivityType.Illustrate)
                {
                    detail.Add(HomeUI.FieldLabel("Correct image URL"));
                    correctImage = HomeUI.Field(token.correctImage);
                    detail.Add(correctImage);
                    detail.Add(HomeUI.FieldLabel("Wrong image URLs (one per line)"));
                    var others = token.imageOptions != null
                        ? string.Join("\n", token.imageOptions.Where(u => u != token.correctImage))
                        : "";
                    distractors = HomeUI.Field(others, multiline: true);
                    detail.Add(distractors);
                }
                else
                {
                    detail.Add(HomeUI.Sub("No activity — this word is plain text in the game."));
                }

                if (chosen != ActivityType.None)
                {
                    detail.Add(HomeUI.FieldLabel("Points"));
                    detail.Add(points);
                }
            }

            // Activity type picker.
            var seg = HomeUI.WrapRow();
            void Paint()
            {
                seg.Clear();
                seg.Add(TypePill("None", ActivityType.None, chosen, t => { chosen = t; Paint(); Rebuild(); }));
                seg.Add(TypePill("Define", ActivityType.Define, chosen, t => { chosen = t; Paint(); Rebuild(); }));
                seg.Add(TypePill("Fill blanks", ActivityType.FillBlank, chosen, t => { chosen = t; Paint(); Rebuild(); }));
                seg.Add(TypePill("Illustrate", ActivityType.Illustrate, chosen, t => { chosen = t; Paint(); Rebuild(); }));
            }
            Paint();
            Rebuild();
            content.Add(seg);
            content.Add(detail);

            var status = HomeUI.Status("", true);
            status.style.display = DisplayStyle.None;
            content.Add(status);

            var apply = HomeUI.Primary("Apply", () =>
            {
                token.activity = chosen;
                int.TryParse(points?.value, out var pts);
                token.points = pts > 0 ? pts : 10;

                if (chosen == ActivityType.Define)
                {
                    if (string.IsNullOrWhiteSpace(definition?.value)) { Fail(status, "Enter a definition."); return; }
                    token.definition = definition.value.Trim();
                }
                else if (chosen == ActivityType.Illustrate)
                {
                    if (string.IsNullOrWhiteSpace(correctImage?.value)) { Fail(status, "Enter the correct image URL."); return; }
                    token.correctImage = correctImage.value.Trim();
                    var opts = new List<string> { token.correctImage };
                    if (!string.IsNullOrWhiteSpace(distractors?.value))
                        opts.AddRange(distractors.value.Split('\n').Select(s => s.Trim()).Where(s => s.Length > 0));
                    token.imageOptions = opts;
                }
                ShowPageWords(page);
            });
            apply.style.marginTop = 10;
            content.Add(apply);
        }

        static Button TypePill(string label, ActivityType type, ActivityType current, Action<ActivityType> onPick)
        {
            var b = HomeUI.Chip(label, () => onPick(type), false);
            b.style.marginRight = 8;
            b.style.marginBottom = 6;
            if (current == type) { b.style.backgroundColor = HomeUI.Parchment; b.style.color = HomeUI.Bg; }
            return b;
        }

        static void Fail(Label status, string message)
        {
            status.text = message;
            status.style.color = HomeUI.Danger;
            status.style.display = DisplayStyle.Flex;
        }

        sealed class AssignmentSummary { public string Id; public string Title; }
    }
}
