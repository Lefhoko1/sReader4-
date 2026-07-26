using SReader.Domains.Assignments.Models;

namespace SReader.Domains.Assignments.Services
{
    /// <summary>
    /// Persists a student's in-game progress on the device so an attempt can be
    /// resumed where they left off. Every solved step is recorded; a finished /
    /// submitted game is cleared. Implementations are local-only and synchronous
    /// (the rows are tiny), with an in-memory fallback when SQLite is unavailable.
    /// </summary>
    public interface IGameProgressStore
    {
        /// <summary>The saved progress for this user + assignment, or null if none.</summary>
        GameProgress Load(string userId, string assignmentId);

        /// <summary>Insert or replace the progress for this user + assignment.</summary>
        void Save(GameProgress progress);

        /// <summary>Forget the progress (e.g. once the game is submitted).</summary>
        void Clear(string userId, string assignmentId);
    }
}
