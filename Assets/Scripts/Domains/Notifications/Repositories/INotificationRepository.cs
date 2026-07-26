using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Notifications.Models;

namespace SReader.Domains.Notifications.Repositories
{
    public interface INotificationRepository
    {
        Task<Result<IReadOnlyList<Notification>>> ListForUserAsync(string userId, bool unreadOnly, CancellationToken ct = default);
        Task<Result> CreateAsync(Notification notification, CancellationToken ct = default);
        Task<Result> MarkReadAsync(string notificationId, CancellationToken ct = default);
        Task<Result> MarkAllReadAsync(string userId, CancellationToken ct = default);
    }
}
