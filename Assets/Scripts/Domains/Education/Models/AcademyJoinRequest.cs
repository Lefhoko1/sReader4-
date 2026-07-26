using System;
using SReader.Core.Common;

namespace SReader.Domains.Education.Models
{
    public enum JoinRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }

    /// <summary>
    /// A student's request to join an academy. Created by the student; the
    /// academy's owning tutor approves or rejects it. AcademyName/StudentName
    /// are denormalised so each side can render the other without a join.
    /// </summary>
    public class AcademyJoinRequest : Entity
    {
        public string AcademyId { get; set; }
        public string AcademyName { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public JoinRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
