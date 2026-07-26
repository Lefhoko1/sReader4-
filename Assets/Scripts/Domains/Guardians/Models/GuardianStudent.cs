using System;

namespace SReader.Domains.Guardians.Models
{
    /// <summary>Link between a guardian account and a student account.</summary>
    public class GuardianStudent
    {
        public string GuardianId { get; set; }
        public string StudentId { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}
