using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Domains.Scheduling.Models;
using SReader.Domains.Scheduling.Repositories;

namespace SReader.Infrastructure.Supabase
{
    /// <summary>TODO: schedules / schedule_permissions tables.</summary>
    public sealed class SupabaseScheduleRepository : SupabaseRepositoryBase, IScheduleRepository
    {
        public SupabaseScheduleRepository(AppSettings settings) : base(settings) { }

        public Task<Result<IReadOnlyList<Schedule>>> ListForUserAsync(string userId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
            => TodoAsync<IReadOnlyList<Schedule>>("List schedule");

        public Task<Result> CreateAsync(Schedule schedule, CancellationToken ct = default)
            => TodoAsync("Create schedule entry");

        public Task<Result> UpdateAsync(Schedule schedule, CancellationToken ct = default)
            => TodoAsync("Update schedule entry");

        public Task<Result> DeleteAsync(string scheduleId, CancellationToken ct = default)
            => TodoAsync("Delete schedule entry");

        public Task<Result<SchedulePermission>> GetPermissionAsync(string scheduleId, string userId, CancellationToken ct = default)
            => TodoAsync<SchedulePermission>("Load schedule permission");

        public Task<Result> UpsertPermissionAsync(SchedulePermission permission, CancellationToken ct = default)
            => TodoAsync("Save schedule permission");
    }
}
