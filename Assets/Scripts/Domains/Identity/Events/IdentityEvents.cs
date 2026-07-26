using SReader.Core.Events;

namespace SReader.Domains.Identity.Events
{
    public sealed class UserRegisteredEvent : IAppEvent
    {
        public string UserId { get; }
        public string Email { get; }

        public UserRegisteredEvent(string userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }

    public sealed class UserLoggedInEvent : IAppEvent
    {
        public string UserId { get; }

        public UserLoggedInEvent(string userId) => UserId = userId;
    }

    public sealed class UserLoggedOutEvent : IAppEvent
    {
        public string UserId { get; }

        public UserLoggedOutEvent(string userId) => UserId = userId;
    }
}
