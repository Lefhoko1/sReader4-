using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Scheduling.Models;

namespace SReader.Domains.Scheduling.Services
{
    public interface IScheduleService
    {
        Task<Result<IReadOnlyList<Schedule>>> GetMyScheduleAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);

        /// <summary>Returns another user's schedule only if visibility rules allow it.</summary>
        Task<Result<IReadOnlyList<Schedule>>> GetVisibleScheduleAsync(string ownerUserId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);

        Task<Result> CreateAsync(Schedule schedule, CancellationToken ct = default);
        Task<Result> UpdateAsync(Schedule schedule, CancellationToken ct = default);
        Task<Result> DeleteAsync(string scheduleId, CancellationToken ct = default);
        Task<Result> GrantViewAsync(string scheduleId, string userId, bool canView, CancellationToken ct = default);
    }
}
