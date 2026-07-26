using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SReader.Domains.Assignments.Models;
using SReader.UI.ViewModels;

namespace SReader.UI.Views.Home
{
    /// <summary>
    /// The Quest Map (spec Phase 2): the assignments list rendered as a chain of
    /// islands along a dotted trail, in due-date order. Each island reflects its
    /// state — in-progress carries Kai's token, scheduled pitches a tent, a near
    /// or passed due date brings a storm cloud with the printed date, a downloaded
    /// assignment shows a backpack. Tapping an island opens its detail sheet.
    /// Pure presentation: it renders state and forwards the open action.
    /// </summary>
    internal static class QuestMapView
    {
        // Trail node colours by state (design-system palette).
        static readonly Color StormCol   = HomeUI.Danger;
        static readonly Color DueSoonCol  = HomeUI.Parchment;
        static readonly Color ActiveCol   = HomeUI.Success;
        static readonly Color DoneCol     = HomeUI.Muted;
        static readonly Color CalmCol     = HomeUI.Hairline;

        /// <summary>Render the trail of islands. <paramref name="list"/> is already due-date ordered.</summary>
        public static void Render(VisualElement box, List<AssignmentView> list,
            StudentAssignmentsViewModel vm, Action<AssignmentView> onOpen)
        {
            var legend = new Label("⛈ overdue   🌥 due soon   🧭 in progress   ⛺ scheduled   🎒 offline   ⛳ done");
            legend.style.fontSize = 11;
            legend.style.color = HomeUI.Muted;
            legend.style.whiteSpace = WhiteSpace.Normal;
            legend.style.marginBottom = 8;
            box.Add(legend);

            var trail = new VisualElement();
            box.Add(trail);

            for (int i = 0; i < list.Count; i++)
                trail.Add(Island(list[i], vm, onOpen, isLast: i == list.Count - 1));
        }

        // One island = a trail connector (dotted line + a state dot) on the left,
        // the island card on the right.
        static VisualElement Island(AssignmentView v, StudentAssignmentsViewModel vm,
            Action<AssignmentView> onOpen, bool isLast)
        {
            var a = v.Assignment;
            var state = Classify(v);

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Stretch;

            // ── trail rail (connector + node dot) ──
            var rail = new VisualElement();
            rail.style.width = 28;
            rail.style.alignItems = Align.Center;
            rail.style.flexShrink = 0;

            var dot = new VisualElement();
            dot.style.width = 16; dot.style.height = 16;
            HomeUI.Radius(dot, 8);
            dot.style.backgroundColor = state.Color;
            dot.style.marginTop = 16;
            HomeUI.Border(dot, HomeUI.Bg, 2);
            rail.Add(dot);

            if (!isLast)
            {
                var lineWrap = new VisualElement();
                lineWrap.style.flexGrow = 1;
                lineWrap.style.alignItems = Align.Center;
                lineWrap.style.marginTop = 2;
                // dotted trail: a short stack of faint pips
                for (int k = 0; k < 5; k++)
                {
                    var pip = new VisualElement();
                    pip.style.width = 3; pip.style.height = 3;
                    HomeUI.Radius(pip, 2);
                    pip.style.backgroundColor = HomeUI.Hairline;
                    pip.style.marginTop = 4;
                    lineWrap.Add(pip);
                }
                rail.Add(lineWrap);
            }
            row.Add(rail);

            // ── island card ──
            var card = HomeUI.Card();
            card.style.flexGrow = 1;
            card.style.marginLeft = 8;
            HomeUI.Border(card, state.Color, state.Emphasise ? 2 : 1);

            var head = HomeUI.Row();
            var title = HomeUI.Title(a.Title, 15);
            title.style.flexGrow = 1;
            head.Add(title);
            head.Add(StateBadge(state));
            card.Add(head);

            card.Add(HomeUI.Sub("🏫 " + vm.ClassNameFor(a.ClassId)));

            // Storm-cloud due-date treatment with the printed date (spec Phase 2).
            var due = HomeUI.Sub(state.DueGlyph + " " + state.DueText);
            due.style.color = state.Emphasise ? state.Color : HomeUI.Muted;
            card.Add(due);

            if (v.IsScheduled)
                card.Add(HomeUI.Sub("⛺ Scheduled " + v.MySchedule.ScheduledFor.ToLocalTime().ToString("dd MMM, HH:mm")));

            // Backpack (downloaded for offline) — resolved async, added if present.
            var offline = HomeUI.Sub("");
            offline.style.display = DisplayStyle.None;
            card.Add(offline);
            ResolveDownloaded(vm, a.Id, offline);

            card.RegisterCallback<ClickEvent>(_ => onOpen(v));
            row.Add(card);
            return row;
        }

        static async void ResolveDownloaded(StudentAssignmentsViewModel vm, string assignmentId, Label target)
        {
            var downloaded = await vm.IsDownloadedAsync(assignmentId);
            if (!downloaded || target == null) return;
            target.text = "🎒 Saved on this device";
            target.style.display = DisplayStyle.Flex;
        }

        static VisualElement StateBadge(IslandState s)
        {
            var b = new Label(s.BadgeIcon + " " + s.BadgeText);
            b.style.fontSize = 11;
            b.style.unityFontStyleAndWeight = FontStyle.Bold;
            b.style.color = HomeUI.Bg;
            b.style.backgroundColor = s.Color;
            b.style.paddingLeft = b.style.paddingRight = 8;
            b.style.paddingTop = b.style.paddingBottom = 3;
            HomeUI.Radius(b, 10);
            return b;
        }

        // ── state classification (all synchronous, from AssignmentView) ──

        struct IslandState
        {
            public Color Color;
            public bool Emphasise;      // thicker border / coloured due line
            public string BadgeIcon;
            public string BadgeText;
            public string DueGlyph;
            public string DueText;
        }

        static IslandState Classify(AssignmentView v)
        {
            var a = v.Assignment;
            string dueDate = a.DueDate.ToLocalTime().ToString("ddd dd MMM");

            if (v.Submitted)
                return new IslandState { Color = DoneCol, BadgeIcon = "⛳", BadgeText = "Done", DueGlyph = "✓", DueText = "Submitted" };

            if (v.Status == AttemptStatus.InProgress)
                return new IslandState { Color = ActiveCol, Emphasise = true, BadgeIcon = "🧭", BadgeText = "In progress", DueGlyph = "📅", DueText = "Due " + dueDate };

            if (a.IsOverdue)
                return new IslandState { Color = StormCol, Emphasise = true, BadgeIcon = "⛈", BadgeText = "Overdue", DueGlyph = "⛈", DueText = "Was due " + dueDate };

            if (a.IsDueSoon)
                return new IslandState { Color = DueSoonCol, Emphasise = true, BadgeIcon = "🌥", BadgeText = "Due soon", DueGlyph = "🌥", DueText = "Due " + dueDate };

            if (v.IsScheduled)
                return new IslandState { Color = DueSoonCol, BadgeIcon = "⛺", BadgeText = "Planned", DueGlyph = "📅", DueText = "Due " + dueDate };

            return new IslandState { Color = CalmCol, BadgeIcon = "🏝", BadgeText = "Ahead", DueGlyph = "📅", DueText = "Due " + dueDate };
        }
    }
}
