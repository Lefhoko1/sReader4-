using System;
using SReader.Core.Common;

namespace SReader.Domains.Social.Models
{
    public enum FriendshipStatus
    {
        Pending,
        Accepted,
        Declined,
        Blocked
    }

    public class Friendship : Entity
    {
        public string RequesterId { get; set; }
        public string RecipientId { get; set; }

        public FriendshipStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
