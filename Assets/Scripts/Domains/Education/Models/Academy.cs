using System;
using SReader.Core.Common;

namespace SReader.Domains.Education.Models
{
    /// <summary>
    /// A learning academy. Owned by exactly one tutor (OwnerId) — that tutor
    /// can edit/delete it and review the students who request to join.
    /// </summary>
    public class Academy : Entity
    {
        public string OwnerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>"City, Country" for display — empty when neither is set.</summary>
        public string LocationSummary
        {
            get
            {
                var city = (City ?? "").Trim();
                var country = (Country ?? "").Trim();
                if (city.Length == 0) return country;
                if (country.Length == 0) return city;
                return $"{city}, {country}";
            }
        }
    }
}
