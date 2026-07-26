using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Domains.Notifications.Models;
using SReader.Domains.Notifications.Repositories;

namespace SReader.Infrastructure.Supabase
{
    /// <summary>TODO Phase 6: notifications table.</summary>
    public sealed class SupabaseNotificationRepository : SupabaseRepositoryBase, INotificationRepository
    {
        public SupabaseNotificationRepository(AppSettings settings) : base(settings) { }

        public Task<Result<IReadOnlyList<Notification>>> ListForUserAsync(string userId, bool unreadOnly, CancellationToken ct = default)
            => TodoAsync<IReadOnlyList<Notification>>("List notifications");

        public Task<Result> CreateAsync(Notification notification, CancellationToken ct = default)
            => TodoAsync("Create notification");

        public Task<Result> MarkReadAsync(string notificationId, CancellationToken ct = default)
            => TodoAsync("Mark notification read");

        public Task<Result> MarkAllReadAsync(string userId, CancellationToken ct = default)
            => TodoAsync("Mark all notifications read");
    }
}
