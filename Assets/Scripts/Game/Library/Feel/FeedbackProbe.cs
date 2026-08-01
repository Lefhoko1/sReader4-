using UnityEngine;

namespace SReader.Game.Library.Feel
{
    /// <summary>
    /// A throwaway test harness for the Shared Restoration Grammar (GL-0). Drop it on
    /// any GameObject next to a <see cref="FeedbackDirector"/> (and optionally a
    /// <see cref="RestorationBurst"/>), enter play mode, and fire the three beats at
    /// this transform's position via the component's right-click context menu — so
    /// the feel can be judged before the Reading Alcove exists. Not shipped: delete
    /// once the rituals drive the director for real.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FeedbackProbe : MonoBehaviour
    {
        [SerializeField] FeedbackDirector director;
        [Tooltip("The word used for the pentatonic note + logs.")]
        [SerializeField] string word = "habitat";
        [Tooltip("Stable key so repeated 'Wrong' presses accumulate toward the Owl hint.")]
        [SerializeField] string sourceKey = "probe-gate";

        FeedbackDirector Director => director != null ? director : FeedbackDirector.Instance;

        FeedbackContext Ctx => new FeedbackContext(sourceKey, transform.position, word);

        [ContextMenu("Play Correct")]
        public void PlayCorrect()
        {
            var d = Director;
            if (d == null) { Debug.LogWarning("[FeedbackProbe] No FeedbackDirector in the scene."); return; }
            d.PlayCorrect(Ctx);
        }

        [ContextMenu("Play Incorrect")]
        public void PlayIncorrect()
        {
            var d = Director;
            if (d == null) { Debug.LogWarning("[FeedbackProbe] No FeedbackDirector in the scene."); return; }
            d.PlayIncorrect(Ctx);
        }

        [ContextMenu("Play Completion")]
        public void PlayCompletion()
        {
            var d = Director;
            if (d == null) { Debug.LogWarning("[FeedbackProbe] No FeedbackDirector in the scene."); return; }
            d.PlayCompletion();
        }
    }
}
