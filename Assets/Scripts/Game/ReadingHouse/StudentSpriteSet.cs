using System;
using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The 2D student's animation frames (built by the Student 2D setup tool from
    /// Assets/Art/ReadingHouse/Student2D/Frames). Every sprite shares one canvas,
    /// one pivot (between the feet) and one pixels-per-unit, so switching clips
    /// never moves or rescales him.
    /// </summary>
    [CreateAssetMenu(menuName = "sReader/Student Sprite Set")]
    public sealed class StudentSpriteSet : ScriptableObject
    {
        [Serializable]
        public sealed class Clip
        {
            public string name;
            public Sprite[] frames;
            public float fps = 10f;
            public bool loop = true;
        }

        public Clip idleFront, idleBack, walkRight, walkAway, walkToward, turnToBack, pointBack;

        [Tooltip("Optional face expressions over the front idle (3D renders): each frame matches the idle pose at the same point in the cycle.")]
        public Clip blinkHalf, blinkClosed, smileFront;

        [Tooltip("Idle-front frames that are part of the blink (played occasionally, not every loop).")]
        public int[] idleFrontBlinkFrames = { 4, 5 };

        [Tooltip("On: the idles are full cycles (rendered from the 3D model) and simply loop. Off: hand-drawn idles, played back and forth.")]
        public bool idlesAreCycles;

        [Tooltip("On: the frames already contain the step bob and breathing, so the animator adds none.")]
        public bool framesIncludeBob;
    }
}
