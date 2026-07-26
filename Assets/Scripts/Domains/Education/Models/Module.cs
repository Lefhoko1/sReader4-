using SReader.Core.Common;

namespace SReader.Domains.Education.Models
{
    public class Module : Entity
    {
        public string ClassId { get; set; }
        public string Title { get; set; }
        public int OrderIndex { get; set; }
    }
}
