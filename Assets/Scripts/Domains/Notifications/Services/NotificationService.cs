using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Events;
using SReader.Core.Logging;
using SReader.Domains.Assignments.Events;
using SReader.Domains.Notifications.Models;
using SReader.Domains.Notifications.Repositories;
using SReader.Domains.Social.Events;

namespace SReader.Domains.Notifications.Services
{
    /// <summary>
    /// Stores and lists notifications, and reacts to events from other
    /// domains (friend requests, submissions) without referencing their
    /// services — event-driven communication per the architecture rules.
    /// </summary>
    public sealed class NotificationService : INotificationService, IDisposable
    {
        readonly INotificationRepository repository;
        readonly CurrentSessionHolder sessionHolder;
        readonly IAppLogger logger;
        readonly IClock clock;
        readonly List<IDisposable> subscriptions = new List<IDisposable>();

        public NotificationService(INotificationRepository repository, CurrentSessionHolder sessionHolder,
                                   IEventBus eventBus, IAppLogger logger, IClock clock)
        {
            this.repository    = Guard.NotNull(repository, nameof(repository));
            this.sessionHolder = Guard.NotNull(sessionHolder, nameof(sessionHolder));
            this.logger        = Guard.NotNull(logger, nameof(logger));
            this.clock         = Guard.NotNull(clock, nameof(clock));
            Guard.NotNull(eventBus, nameof(eventBus));

            subscriptions.Add(eventBus.Subscribe<FriendRequestSentEvent>(OnFriendRequestSent));
            subscriptions.Add(eventBus.Subscribe<AssignmentSubmittedEvent>(OnAssignmentSubmitted));
        }

        public Task<Result<IReadOnlyList<Notification>>> GetMyNotificationsAsync(bool unreadOnly, CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn)
                return Task.FromResult(Result.Fail<IReadOnlyList<Notification>>("Not signed in."));
            return repository.ListForUserAsync(sessionHolder.CurrentUserId, unreadOnly, ct);
        }

        public Task<Result> NotifyAsync(string userId, string title, string message, NotificationType type, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(userId)) return Task.FromResult(Result.Fail("User id is required."));
            return repository.CreateAsync(new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                NotificationType = type,
                IsRead = false,
                CreatedAt = clock.UtcNow
            }, ct);
        }

        public Task<Result> MarkReadAsync(string notificationId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(notificationId))
                return Task.FromResult(Result.Fail("Notification id is required."));
            return repository.MarkReadAsync(notificationId, ct);
        }

        public Task<Result> MarkAllReadAsync(CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            return repository.MarkAllReadAsync(sessionHolder.CurrentUserId, ct);
        }

        async void OnFriendRequestSent(FriendRequestSentEvent evt)
        {
            var result = await NotifyAsync(evt.RecipientId, "New friend request",
                "Someone wants to read with you — open your friends list.", NotificationType.FriendRequest);
            if (result.IsFailure)
                logger.Warning($"Could not create friend-request notification: {result.Error}");
        }

        async void OnAssignmentSubmitted(AssignmentSubmittedEvent evt)
        {
            // The student gets a confirmation entry; tutor fan-out comes
            // with Phase 6 once class rosters are queryable.
            var result = await NotifyAsync(evt.StudentId, "Assignment submitted",
                "Your work was submitted successfully.", NotificationType.AssignmentSubmitted);
            if (result.IsFailure)
                logger.Warning($"Could not create submission notification: {result.Error}");
        }

        public void Dispose()
        {
            foreach (var s in subscriptions) s.Dispose();
            subscriptions.Clear();
        }
    }
}
