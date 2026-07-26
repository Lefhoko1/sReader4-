using System;
using System.Collections.Generic;

namespace SReader.Domains.Assignments.Models
{
    /// <summary>
    /// A student's saved progress in one assignment's game, so they can leave and
    /// continue later. Records every solved activity (by its deterministic board
    /// index — the SHARED order both solo and multiplayer agree on) plus the
    /// running score. Stored locally on the device (SQLite), one row per
    /// user + assignment.
    /// </summary>
    public sealed class GameProgress
    {
        public string UserId { get; set; }
        public string AssignmentId { get; set; }

        /// <summary>"solo" | "multiplayer" — how it was last played.</summary>
        public string Mode { get; set; } = "solo";

        /// <summary>The multiplayer room id, if this was a shared game.</summary>
        public string SessionId { get; set; }

        /// <summary>Deterministic board indices of the activities already solved.</summary>
        public List<int> SolvedIndices { get; set; } = new List<int>();

        public int Score { get; set; }
        public int Total { get; set; }
        public bool Finished { get; set; }
        public DateTime UpdatedUtc { get; set; }

        /// <summary>True when there's something worth resuming (unfinished, with solves).</summary>
        public bool HasProgress => !Finished && SolvedIndices != null && SolvedIndices.Count > 0;
    }
}
