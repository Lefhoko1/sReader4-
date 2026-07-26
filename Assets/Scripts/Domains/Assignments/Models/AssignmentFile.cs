using SReader.Core.Common;

namespace SReader.Domains.Assignments.Models
{
    public class AssignmentFile : Entity
    {
        public string AssignmentId { get; set; }
        public int Version { get; set; }
        public string StoragePath { get; set; }

        /// <summary>Integrity check for downloads — compare before trusting a cached copy.</summary>
        public string Checksum { get; set; }
    }
}
