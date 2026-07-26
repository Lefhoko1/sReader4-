using SReader.Core.Common;

namespace SReader.Domains.Identity.Models
{
    public class PrivacySettings : Entity
    {
        public string UserId { get; set; }

        public bool ShowProfile { get; set; } = true;
        public bool ShowSchedule { get; set; }
        public bool ShowLocation { get; set; }
        public bool ShowAssignments { get; set; }
        public bool ShowOnlineStatus { get; set; } = true;
        public bool ShowFriends { get; set; } = true;
    }
}
