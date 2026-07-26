namespace SReader.Domains.Social.Models
{
    /// <summary>What a user's accepted friends are allowed to see.</summary>
    public class FriendSettings
    {
        public string UserId { get; set; }

        public bool CanSeeProfile { get; set; } = true;
        public bool CanSeeSchedule { get; set; }
        public bool CanSeeAssignments { get; set; }
        public bool CanSeeLocation { get; set; }
        public bool CanSeeOnlineStatus { get; set; } = true;
    }
}
