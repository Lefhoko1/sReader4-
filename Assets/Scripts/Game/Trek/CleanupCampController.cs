using System.Collections.Generic;
using UnityEngine.UIElements;

namespace SReader.Game.Trek
{
    /// <summary>
    /// Cleanup Camp (R-7): the mandatory final stop before the finish line.
    /// Every gate the student deferred with "Later" re-queues here, in passage
    /// order; leaving camp forward requires zero unsolved gates (R-6). This
    /// controller is only the campfire sheet — the queue itself lives in
    /// TrekSession; the presenter opens each gate panel in turn.
    /// </summary>
    internal sealed class CleanupCampController
    {
        VisualElement overlay;
        public bool IsOpen => overlay != null;

        /// <summary>Show the camp sheet listing the words still sealed.</summary>
        public void Open(VisualElement host, IReadOnlyList<GateData> deferredInOrder, System.Action onOpenNextGate)
        {
            Close();

            overlay = new VisualElement();
            overlay.style.position = Position.Absolute;
            overlay.style.left = 0; overlay.style.right = 0; overlay.style.top = 0; overlay.style.bottom = 0;
            overlay.style.justifyContent = Justify.FlexEnd;

            var sheet = TrekTheme.CardBox();
            sheet.style.marginLeft = sheet.style.marginRight = 10;
            sheet.style.marginBottom = 86;
            overlay.Add(sheet);

            var kicker = new Label("⛺ CLEANUP CAMP");
            kicker.style.fontSize = 11;
            kicker.style.letterSpacing = 2;
            kicker.style.color = TrekTheme.Coral;
            kicker.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;
            sheet.Add(kicker);

            int n = deferredInOrder?.Count ?? 0;
            sheet.Add(TrekTheme.Title(n == 1 ? "One sealed word left" : n + " sealed words left", 18));
            sheet.Add(TrekTheme.Sub("The words you saved for later are waiting here. Clear every gate to finish the trail — hints can always get you through."));

            // The sealed words, in the order they'll be presented.
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexWrap = Wrap.Wrap;
            row.style.marginTop = 8;
            sheet.Add(row);
            if (deferredInOrder != null)
                foreach (var g in deferredInOrder)
                {
                    var chip = new Label("[" + new string('⬚', System.Math.Min(6, System.Math.Max(3, g.Word?.Length ?? 3))) + "]");
                    chip.style.color = TrekTheme.Violet;
                    chip.style.fontSize = 14;
                    chip.style.marginRight = 8;
                    row.Add(chip);
                }

            var go = TrekTheme.Primary(n > 0 ? "Open the next gate" : "Continue", () => { Close(); onOpenNextGate(); });
            go.style.marginTop = 10;
            sheet.Add(go);

            host.Add(overlay);
        }

        public void Close()
        {
            overlay?.RemoveFromHierarchy();
            overlay = null;
        }
    }
}
