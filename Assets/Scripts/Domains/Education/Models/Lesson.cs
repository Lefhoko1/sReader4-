using SReader.Core.Common;

namespace SReader.Domains.Education.Models
{
    public class Lesson : Entity
    {
        public string ModuleId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        /// <summary>Storage path of the downloadable lesson content (versioned via Files domain).</summary>
        public string ContentPath { get; set; }
    }
}
