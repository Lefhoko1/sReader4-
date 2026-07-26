using System;
using SReader.Core.Common;

namespace SReader.Domains.Assignments.Models
{
    public class Assignment : Entity
    {
        public string ClassId { get; set; }
        public string ModuleId { get; set; }

        public string Title { get; set; }
        public string Instructions { get; set; }

        /// <summary>
        /// The structured assignment content, held as a JSON string. The schema
        /// lands later; it's written via a dedicated call (not the general
        /// update), so a content-less edit never wipes it.
        /// </summary>
        public string ContentJson { get; set; }

        public bool HasContent => !string.IsNullOrWhiteSpace(ContentJson);

        public DateTime DueDate { get; set; }
        public int MaxScore { get; set; }

        /// <summary>Bumped whenever the assignment content changes — drives re-download.</summary>
        public int Version { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool IsOverdue => DueDate != default && DueDate.ToUniversalTime() < DateTime.UtcNow;

        /// <summary>Whether the due date is within the next 3 days (and not past).</summary>
        public bool IsDueSoon
        {
            get
            {
                if (DueDate == default) return false;
                var due = DueDate.ToUniversalTime();
                return due >= DateTime.UtcNow && due <= DateTime.UtcNow.AddDays(3);
            }
        }
    }
}
