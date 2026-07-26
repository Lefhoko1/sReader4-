using System;
using SReader.Core.Common;

namespace SReader.Domains.Notifications.Models
{
    public enum NotificationType
    {
        AssignmentReminder,
        FriendRequest,
        AssignmentSubmitted,
        ClassInvite,
        MultiplayerInvite
    }

    public class Notification : Entity
    {
        public string UserId { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }

        public NotificationType NotificationType { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
