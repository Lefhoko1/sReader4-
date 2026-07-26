using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Scheduling.Models;

namespace SReader.Domains.Scheduling.Repositories
{
    public interface IScheduleRepository
    {
        Task<Result<IReadOnlyList<Schedule>>> ListForUserAsync(string userId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);
        Task<Result> CreateAsync(Schedule schedule, CancellationToken ct = default);
        Task<Result> UpdateAsync(Schedule schedule, CancellationToken ct = default);
        Task<Result> DeleteAsync(string scheduleId, CancellationToken ct = default);

        Task<Result<SchedulePermission>> GetPermissionAsync(string scheduleId, string userId, CancellationToken ct = default);
        Task<Result> UpsertPermissionAsync(SchedulePermission permission, CancellationToken ct = default);
    }
}
