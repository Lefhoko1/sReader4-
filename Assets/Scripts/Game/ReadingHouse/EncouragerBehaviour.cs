using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The Encourager on his chair: he always turns to face the student (ActorSpriteAnimator facings),
    /// cheers when the student comes into the house, and says only encouraging things, in a candy
    /// speech bubble. The house (presenter / walk-in director) tells him what happened; he picks the
    /// words and the move. Presentation only.
    /// </summary>
    public sealed class EncouragerBehaviour : MonoBehaviour
    {
        public ActorSpriteAnimator actor;
        public CandySpeechBubble bubble;

        [Header("What he says")]
        public string[] welcome =
        {
            "Welcome to the Reading House, superstar!",
            "Hooray, you're here! Let's read something amazing!",
            "Yay! My favourite reader just walked in!",
        };
        public string[] greet =
        {
            "You've got this — let's read!",
            "One word at a time. I believe in you!",
            "Ready, reader? This is going to be fun!",
        };
        public string[] encourage =
        {
            "Keep going — you're doing great!",
            "Every sentence makes you stronger!",
            "You're a reading champion!",
            "Nice and steady. You can do it!",
        };
        public string[] pick =
        {
            "Great pick!",
            "Ooh, good choice — go for it!",
            "You know this one!",
        };
        public string[] praise =
        {
            "Amazing — you did it!",
            "Brilliant! That word shines gold now!",
            "Woohoo! You're on fire!",
            "Fantastic reading!",
        };
        public string[] nudge =
        {
            "Let's restore the glowing word first!",
            "Try the shining stone first — you've got this!",
        };
        public string[] finish =
        {
            "You finished! I'm SO proud of you!",
            "Every word restored — what a star!",
        };

        public void Welcome() => Cheer(Pick(welcome), 3.6f);
        public void Greet() => Say(Pick(greet), 3.2f);
        public void Encourage() => Say(Pick(encourage), 3f);
        public void GoodPick() => Say(Pick(pick), 2.4f);
        public void Praise() => Cheer(Pick(praise), 3.2f);
        public void Nudge() => Say(Pick(nudge), 2.8f);
        public void Finished() => Cheer(Pick(finish), 4f);

        /// <summary>Talk (mouth and a gesture) while the bubble shows.</summary>
        public void Say(string text, float seconds)
        {
            if (actor != null) actor.PlayFor("talk", Mathf.Min(seconds, 2.6f));
            if (bubble != null) bubble.Show(text, seconds);
        }

        /// <summary>Arms up, bouncing on the chair, with something happy to say.</summary>
        public void Cheer(string text, float seconds)
        {
            if (actor != null) actor.Play("cheer");
            if (bubble != null) bubble.Show(text, seconds);
        }

        static string Pick(string[] lines) => lines == null || lines.Length == 0 ? "" : lines[Random.Range(0, lines.Length)];
    }
}
