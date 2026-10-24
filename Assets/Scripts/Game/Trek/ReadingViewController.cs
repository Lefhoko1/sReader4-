using System.Text;
using UnityEngine.UIElements;

namespace SReader.Game.Trek
{
    /// <summary>
    /// Reading View (R-2/R-3): the entire passage as a scrollable page overlay.
    /// Available before starting, mid-trek (the presenter pauses the timer while
    /// it is open) and after completion. Key words not yet won render sealed
    /// (⬚⬚⬚); won words render highlighted with a tap-to-hear affordance (the
    /// audio itself ships with the Phase 5 read-aloud work).
    /// </summary>
    internal sealed class ReadingViewController
    {
        VisualElement overlay;
        Label page;
        TrailData trail;
        bool showFullPassage;

        public bool IsOpen => overlay != null;

        /// <summary>
        /// Show the overlay. <paramref name="onClose"/> fires when it is dismissed.
        /// <paramref name="startAction"/>, when supplied, adds a primary button
        /// (e.g. "Start the trek") for the R-3 "Read first" flow.
        /// </summary>
        public void Open(VisualElement host, TrailData trailData, int currentSentence,
            System.Action onClose, (string label, System.Action onStart)? startAction = null, bool classroomMode = false)
        {
            if (IsOpen) return;
            trail = trailData;
            showFullPassage = classroomMode;

            overlay = new VisualElement();
            overlay.style.position = Position.Absolute;
            overlay.style.left = 0; overlay.style.right = 0; overlay.style.top = 0; overlay.style.bottom = 0;
            overlay.style.backgroundColor = new UnityEngine.Color(0f, 0f, 0f, 0.45f);

            var card = new VisualElement();
            card.style.flexGrow = 1;
            card.style.marginLeft = card.style.marginRight = 14;
            card.style.marginTop = 40;
            card.style.marginBottom = 20;
            card.style.backgroundColor = classroomMode ? new UnityEngine.Color(.96f,.92f,.81f) : TrekTheme.Paper;
            TrekTheme.Round(card, classroomMode ? 3 : 18);
            TrekTheme.Border(card, TrekTheme.Line, 1);
            card.style.paddingLeft = card.style.paddingRight = 18;
            card.style.paddingTop = card.style.paddingBottom = 14;
            if (classroomMode)
            {
                card.style.position = Position.Absolute;
                card.style.left = card.style.right = 14;
                card.style.top = Length.Percent(35); card.style.bottom = 14;
                card.style.marginLeft = card.style.marginRight = card.style.marginTop = card.style.marginBottom = 0;
            }
            overlay.Add(card);

            var head = new VisualElement();
            head.style.flexDirection = FlexDirection.Row;
            head.style.alignItems = Align.Center;
            card.Add(head);

            var title = TrekTheme.Title(string.IsNullOrEmpty(trail.Title) ? "The passage" : trail.Title, 18);
            title.style.flexGrow = 1;
            title.style.flexShrink = 1;
            head.Add(title);
            if (startAction.HasValue)
            {
                var start = TrekTheme.Primary(startAction.Value.label, () => Close(startAction.Value.onStart));
                start.style.marginRight = 8;
                head.Add(start);
            }
            head.Add(TrekTheme.Ghost(classroomMode ? "Close book" : startAction.HasValue ? "✕ Close" : "✕ Back to trail", () => Close(onClose)));

            var guidance = TrekTheme.Sub(classroomMode ? "Read the full passage. Restore its words on the teaching board." : "Sealed words open as you clear their gates. Won words are lit — tap-to-hear arrives with read-aloud.");
            if(classroomMode) guidance.style.color = new UnityEngine.Color(.32f,.34f,.28f);
            card.Add(guidance);

            var scroll = new ScrollView(ScrollViewMode.Vertical);
            scroll.style.flexGrow = 1;
            scroll.style.minHeight = 0;
            scroll.style.marginTop = 10;
            card.Add(scroll);

            page = new Label { enableRichText = true };
            page.style.fontSize = classroomMode ? 19 : 16;
            page.style.color = TrekTheme.Ink;
            page.style.whiteSpace = WhiteSpace.Normal;
            scroll.Add(page);

            Refresh(currentSentence);
            host.Add(overlay);
        }

        public void Close(System.Action onClose = null)
        {
            overlay?.RemoveFromHierarchy();
            overlay = null;
            onClose?.Invoke();
        }

        /// <summary>Re-render the passage (a word was won, or the lantern moved).</summary>
        public void Refresh(int currentSentence)
        {
            if (page == null || trail == null) return;

            if (showFullPassage) { page.enableRichText = false; page.text = trail.PassageText; return; }
            var sb = new StringBuilder(trail.PassageText.Length + 256);
            for (int i = 0; i < trail.Sentences.Count; i++)
            {
                bool current = i == currentSentence;
                if (current) sb.Append("<b>");
                AppendSentence(sb, i);
                if (current) sb.Append("</b>");
                sb.Append(' ');
            }
            page.text = sb.ToString();
        }

        void AppendSentence(StringBuilder to, int sentenceIndex)
        {
            var span = trail.Sentences[sentenceIndex];
            int cursor = span.Start;

            foreach (var gate in trail.Gates)
            {
                if (gate.SentenceIndex != sentenceIndex) continue;
                int start = gate.PassageCharIndex;
                int len = gate.Word?.Length ?? 0;
                if (start < cursor || start + len > span.End) continue;

                to.Append(Escape(trail.PassageText.Substring(cursor, start - cursor)));

                bool won = gate.State == GateState.SolvedGold || gate.State == GateState.SolvedSilver || gate.State == GateState.SolvedBronze;
                if (won)
                    to.Append("<color=#2E9C55><b>").Append(Escape(gate.Word)).Append("</b> 🔊</color>");
                else
                    to.Append("<color=#9A86F2>[").Append(new string('⬚', len < 3 ? 3 : len > 10 ? 10 : len)).Append("]</color>");

                cursor = start + len;
            }

            to.Append(Escape(trail.PassageText.Substring(cursor, span.End - cursor)));
        }

        static string Escape(string s) => s.Replace("<", "<​");
    }
}
