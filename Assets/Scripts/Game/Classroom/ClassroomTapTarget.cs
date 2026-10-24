using UnityEngine;
namespace SReader.Game.Classroom
{
    public sealed class ClassroomTapTarget : MonoBehaviour
    {
        public enum Kind { Board, Book, Word, Lexicon }
        public Kind kind;
        public int wordIndex;
    }
}
