using System;
using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// Frames for a character who stays in one spot (rendered from its 3D model by a
    /// GeneratedAssets/*/render_*.py script): a breathing idle, blink frames over that idle, and
    /// named actions (e.g. the Encourager's "talk" / "cheer", the Boredom Zombie's "yawn" / "slump").
    ///
    /// A character that always faces the camera has one facing. One that turns towards someone (the
    /// Encourager watching the student) has several, each the same clips rendered turned by a yaw
    /// (+ = towards screen-right). One shared canvas, pivot and pixels-per-unit throughout, so switching
    /// clips or facings never moves or rescales the character.
    /// </summary>
    [CreateAssetMenu(menuName = "sReader/Actor Sprite Set")]
    public sealed class ActorSpriteSet : ScriptableObject
    {
        [Serializable]
        public sealed class Action
        {
            public string name;
            public StudentSpriteSet.Clip clip;
            [Tooltip("Frames per second.")] public float fps = 12f;
        }

        [Serializable]
        public sealed class Facing
        {
            [Tooltip("Degrees the character is turned (+ = towards screen-right).")]
            public float yaw;
            public StudentSpriteSet.Clip idle;
            [Tooltip("Blink frames: each matches the idle pose at the same point in its cycle (half is optional).")]
            public StudentSpriteSet.Clip blinkHalf, blinkClosed;
            public Action[] actions = new Action[0];

            public Action Find(string actionName) => Array.Find(actions, a => a.name == actionName);
        }

        public float idleFps = 8f;
        public Facing[] facings = new Facing[0];

        /// <summary>The facing nearest a yaw.</summary>
        public int Nearest(float yaw)
        {
            int best = 0;
            for (int i = 1; i < facings.Length; i++)
                if (Mathf.Abs(facings[i].yaw - yaw) < Mathf.Abs(facings[best].yaw - yaw)) best = i;
            return best;
        }
    }
}
