using UnityEngine.UIElements;

namespace SReader.Game.Trek
{
    /// <summary>
    /// Trail Complete (R-9/R-10): the celebration AND the paperwork on one screen.
    /// Marks stay real ("16/20"), stars are derived for the child, and the
    /// submission line says explicitly whether the result reached the tutor —
    /// students and tutors must both trust that playing counted.
    /// </summary>
    internal sealed class TrailCompleteView
    {
        VisualElement overlay;
        public bool IsOpen => overlay != null;

        public void Open(VisualElement host, TrekResult result, bool submitted, string submissionNote,
            System.Action onExit, System.Action onRead)
        {
            Close();

            overlay = new VisualElement();
            overlay.style.position = Position.Absolute;
            overlay.style.left = 0; overlay.style.right = 0; overlay.style.top = 0; overlay.style.bottom = 0;
            overlay.style.backgroundColor = new UnityEngine.Color(0f, 0f, 0f, 0.5f);
            overlay.style.justifyContent = Justify.Center;

            var card = TrekTheme.CardBox();
            card.style.marginLeft = card.style.marginRight = 22;
            card.style.alignItems = Align.Center;
            overlay.Add(card);

            var banner = new Label("TRAIL COMPLETE!");
            banner.style.fontSize = 26;
            banner.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;
            banner.style.color = TrekTheme.Ink;
            banner.style.letterSpacing = 2;
            card.Add(banner);

            var stars = new Label(new string('★', result.Stars) + new string('☆', 3 - result.Stars));
            stars.style.fontSize = 34;
            stars.style.color = TrekTheme.Sun;
            stars.style.marginTop = 4;
            card.Add(stars);

            // Marks stay real (R-10).
            var marks = new Label(result.MaxScore > 0
                ? $"{result.Marks}/{result.MaxScore}"
                : $"{result.Points}/{result.TotalBasePoints}");
            marks.style.fontSize = 20;
            marks.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;
            marks.style.color = TrekTheme.Ink;
            card.Add(marks);

            var tiers = TrekTheme.Sub(
                $"🥇 {result.GatesSolvedGold} clean  ·  🥈 {result.GatesSolvedSilver} hinted  ·  🥉 {result.GatesSolvedBronze} guided", 12);
            tiers.style.marginTop = 2;
            card.Add(tiers);

            // The paperwork, explicit (R-9).
            var sub = new Label(submitted ? "Submitted to your tutor ✓" : submissionNote);
            sub.style.fontSize = 14;
            sub.style.marginTop = 10;
            sub.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;
            sub.style.color = submitted ? TrekTheme.Leaf : TrekTheme.Coral;
            sub.style.whiteSpace = WhiteSpace.Normal;
            sub.style.unityTextAlign = UnityEngine.TextAnchor.MiddleCenter;
            card.Add(sub);

            var actions = new VisualElement();
            actions.style.flexDirection = FlexDirection.Row;
            actions.style.marginTop = 14;
            card.Add(actions);

            var read = TrekTheme.Ghost("📖 Read the story", onRead);
            read.style.marginRight = 8;
            actions.Add(read);
            actions.Add(TrekTheme.Primary("Back to camp", onExit));

            host.Add(overlay);
        }

        public void Close()
        {
            overlay?.RemoveFromHierarchy();
            overlay = null;
        }
    }
}
