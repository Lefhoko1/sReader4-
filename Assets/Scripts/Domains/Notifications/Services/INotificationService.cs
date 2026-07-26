using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Notifications.Models;

namespace SReader.Domains.Notifications.Services
{
    public interface INotificationService
    {
        Task<Result<IReadOnlyList<Notification>>> GetMyNotificationsAsync(bool unreadOnly, CancellationToken ct = default);
        Task<Result> NotifyAsync(string userId, string title, string message, NotificationType type, CancellationToken ct = default);
        Task<Result> MarkReadAsync(string notificationId, CancellationToken ct = default);
        Task<Result> MarkAllReadAsync(CancellationToken ct = default);
    }
}
