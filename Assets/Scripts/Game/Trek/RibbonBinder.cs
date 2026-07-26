using System.Text;
using UnityEngine.UIElements;

namespace SReader.Game.Trek
{
    /// <summary>
    /// The story ribbon (R-1): a strip above the HUD's bottom edge showing the
    /// passage continuously, synced to progress — already-read text dimmed,
    /// the current sentence highlighted ("lantern"), upcoming text muted.
    /// Key words not yet won render as sealed slots (⬚). Tapping the ribbon
    /// opens Reading View (R-2).
    ///
    /// Perf: the label text is rebuilt ONLY when the sentence index or a gate
    /// state changes — never per frame (spec §3).
    /// </summary>
    internal sealed class RibbonBinder
    {
        TrailData trail;
        Label label;
        VisualElement root;
        int sentence = -1;
        readonly StringBuilder sb = new StringBuilder(512);

        // Storybook ink shades as rich-text hex (ribbon sits on paper).
        const string ReadHex = "#9AA394";       // dimmed but readable (R-1)
        const string CurrentHex = "#1D2417";    // ink — the lantern line
        const string UpcomingHex = "#B9C0B2";   // visible, muted
        const string SealHex = "#9A86F2";       // sealed slots (violet)
        const string WonHex = "#2E9C55";        // won words (leaf, darkened for paper)

        public VisualElement Build(TrailData trailData, System.Action onTap)
        {
            trail = trailData;

            root = new VisualElement();
            root.style.backgroundColor = TrekTheme.Paper;
            TrekTheme.Round(root, 14);
            TrekTheme.Border(root, TrekTheme.Line, 1);
            root.style.paddingLeft = root.style.paddingRight = 14;
            root.style.paddingTop = root.style.paddingBottom = 10;
            root.style.marginLeft = root.style.marginRight = 10;
            root.style.marginBottom = 8;

            label = new Label("");
            label.enableRichText = true;
            label.style.fontSize = 14;
            label.style.whiteSpace = WhiteSpace.Normal;
            label.style.color = TrekTheme.Ink;
            root.Add(label);

            var hint = new Label("tap to read the whole passage");
            hint.style.fontSize = 10;
            hint.style.color = TrekTheme.Muted;
            hint.style.unityTextAlign = UnityEngine.TextAnchor.MiddleRight;
            root.Add(hint);

            if (onTap != null)
                root.RegisterCallback<ClickEvent>(_ => onTap());

            SetSentence(0, force: true);
            return root;
        }

        /// <summary>Move the lantern to a sentence (rebuilds only on change).</summary>
        public void SetSentence(int index, bool force = false)
        {
            if (trail == null || label == null) return;
            int clamped = Clamp(index);
            if (!force && clamped == sentence) return;
            sentence = clamped;
            Rebuild();
        }

        /// <summary>A gate changed state (word won / sealed) — refresh the text.</summary>
        public void Refresh() => Rebuild();

        int Clamp(int i)
        {
            if (trail.Sentences.Count == 0) return 0;
            if (i < 0) return 0;
            if (i >= trail.Sentences.Count) return trail.Sentences.Count - 1;
            return i;
        }

        void Rebuild()
        {
            sb.Length = 0;
            // A window of prev · current · next keeps the ribbon at ~3 lines while
            // staying continuous with progress; the full page is Reading View's job.
            for (int i = sentence - 1; i <= sentence + 1; i++)
            {
                if (i < 0 || i >= trail.Sentences.Count) continue;
                string hex = i < sentence ? ReadHex : i == sentence ? CurrentHex : UpcomingHex;
                bool bold = i == sentence;
                sb.Append("<color=").Append(hex).Append('>');
                if (bold) sb.Append("<b>");
                AppendSentence(sb, i, i == sentence);
                if (bold) sb.Append("</b>");
                sb.Append("</color> ");
            }
            label.text = sb.ToString();
        }

        // Writes one sentence, replacing each not-yet-won key word with a sealed
        // slot and tinting won words (R-1 / R-2 seal rules).
        void AppendSentence(StringBuilder to, int sentenceIndex, bool isCurrent)
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
                    to.Append("<color=").Append(WonHex).Append("><b>").Append(Escape(gate.Word)).Append("</b></color>");
                else
                    to.Append("<color=").Append(SealHex).Append('>').Append('[').Append(Seal(len)).Append(']').Append("</color>");

                cursor = start + len;
            }

            to.Append(Escape(trail.PassageText.Substring(cursor, span.End - cursor)));
        }

        static string Seal(int wordLength)
        {
            int n = wordLength < 3 ? 3 : wordLength > 10 ? 10 : wordLength;
            return new string('⬚', n);
        }

        static string Escape(string s) => s.Replace("<", "<​");   // neutralise accidental rich-text tags
    }
}
