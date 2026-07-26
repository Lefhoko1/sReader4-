using System;
using SReader.Core.Common;

namespace SReader.Domains.Education.Models
{
    /// <summary>
    /// A course/module under a University grade (program). Primary and
    /// Secondary grades never have these — their certificate is the course.
    /// </summary>
    public class AcademyCourse : Entity
    {
        public string GradeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// The class that teaches this subject/module, if the tutor has assigned
        /// one. When a student's paid enrolment for this subject is confirmed,
        /// they're enrolled into this class so they receive its assignments.
        /// Empty = not yet placed in a class.
        /// </summary>
        public string ClassId { get; set; }
        public string ClassName { get; set; }

        public bool HasClass => !string.IsNullOrEmpty(ClassId);

        /// <summary>Tutoring fee in Botswana Pula. 0 = not priced yet.</summary>
        public double Price { get; set; }

        /// <summary>What the price covers, e.g. "per month", "per term", "8 weeks".</summary>
        public string TimeFrame { get; set; }

        /// <summary>Whether the tutor offers a free trial for this subject/module.</summary>
        public bool FreeTrial { get; set; }

        /// <summary>Closing date ("yyyy-MM-dd"); empty means always open. Past = unavailable.</summary>
        public string Deadline { get; set; }

        public DateTime CreatedAt { get; set; }

        /// <summary>"P150 / per month" for display; "Free" when unpriced.</summary>
        public string PriceSummary
        {
            get
            {
                if (Price <= 0) return "Free";
                var frame = string.IsNullOrWhiteSpace(TimeFrame) ? "" : $" / {TimeFrame.Trim()}";
                return $"P{Price:0.##}{frame}";
            }
        }

        public bool HasDeadline => !string.IsNullOrWhiteSpace(Deadline);

        /// <summary>True once the deadline date has passed (compared in UTC days).</summary>
        public bool IsExpired =>
            HasDeadline && DateTime.TryParse(Deadline, out var d) && d.Date < DateTime.UtcNow.Date;

        /// <summary>A subject/module can only be requested while available.</summary>
        public bool IsAvailable => !IsExpired;

        public string AvailabilityNote =>
            IsExpired ? "Closed — deadline passed" : (HasDeadline ? $"Open until {Deadline}" : "");
    }
}
