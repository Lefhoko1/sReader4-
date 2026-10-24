using System.Text;
using TMPro;
using UnityEngine;
using SReader.Game.Trek;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The chalkboard on the right wall: today's assignment title and its key
    /// words in chalk, straight from the TrailData the presenter already built
    /// (solved words are ticked from the gate state TrekSession keeps).
    /// Presentation only.
    /// </summary>
    public sealed class ChalkboardView : MonoBehaviour
    {
        public TextMeshPro text;

        /// <summary>Nothing chosen yet: invite the student to use the board.</summary>
        public void ShowPrompt()
        {
            if (text != null) text.text = "<size=125%>My assignments</size>\n\n<size=85%>Tap the board to open your\nassignments, academies and classes.</size>";
        }

        public void Show(TrailData trail)
        {
            if (text == null) return;
            if (trail == null) { text.text = ""; return; }
            var sb = new StringBuilder();
            sb.AppendLine("<size=125%>Today's reading</size>");
            sb.AppendLine($"<b>{trail.Title}</b>");
            sb.AppendLine();
            if (trail.Gates.Count == 0) sb.AppendLine("<size=85%>Read the passage on the board.</size>");
            else
            {
                sb.AppendLine("<size=85%>Words to restore</size>");
                foreach (var g in trail.Gates)
                {
                    bool solved = g.State == GateState.SolvedGold || g.State == GateState.SolvedSilver || g.State == GateState.SolvedBronze;
                    sb.AppendLine(solved ? $"<color=#F2C14E>✓ {g.Word}</color>" : g.Word);
                }
            }
            text.text = sb.ToString();
        }
    }
}
