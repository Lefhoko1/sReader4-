using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The chalkboard as a screen for the app's real pages: StudentHomeReal + StudentHomeController
    /// (wired to the real ViewModels by the house's AppCompositionRoot) render onto the slate, and taps on
    /// the slate become the page's own pointer positions. See <see cref="WorldUIScreen"/>.
    /// </summary>
    public sealed class ChalkboardScreen : WorldUIScreen
    {
        float nextScan;

        void LateUpdate()
        {
            // The pages rebuild their lists as they load: every list on the board can be grabbed and slid.
            if (!IsOpen || Time.unscaledTime < nextScan) return;
            nextScan = Time.unscaledTime + 0.5f;
            Grabbable(page);
            foreach (var extra in extraPages) Grabbable(extra);
        }

        static void Grabbable(UnityEngine.UIElements.UIDocument doc)
        {
            if (doc == null || !doc.isActiveAndEnabled || doc.rootVisualElement == null) return;
            doc.rootVisualElement.Query<UnityEngine.UIElements.ScrollView>().ForEach(sv =>
            {
                if (sv.ClassListContains("rh-drag")) return;
                sv.AddToClassList("rh-drag");
                sv.AddManipulator(SlideDrag.ForScrollView(sv));
                sv.mouseWheelScrollSize = 60f;
            });
        }
    }
}
