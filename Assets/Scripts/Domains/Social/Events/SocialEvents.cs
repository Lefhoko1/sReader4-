using SReader.Core.Events;

namespace SReader.Domains.Social.Events
{
    public sealed class FriendRequestSentEvent : IAppEvent
    {
        public string RequesterId { get; }
        public string RecipientId { get; }

        public FriendRequestSentEvent(string requesterId, string recipientId)
        {
            RequesterId = requesterId;
            RecipientId = recipientId;
        }
    }

    public sealed class FriendRequestAcceptedEvent : IAppEvent
    {
        public string FriendshipId { get; }

        public FriendRequestAcceptedEvent(string friendshipId) => FriendshipId = friendshipId;
    }
}
