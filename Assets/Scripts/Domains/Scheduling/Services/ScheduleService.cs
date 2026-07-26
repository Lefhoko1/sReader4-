using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Domains.Identity.Repositories;
using SReader.Domains.Scheduling.Models;
using SReader.Domains.Scheduling.Repositories;

namespace SReader.Domains.Scheduling.Services
{
    public sealed class ScheduleService : IScheduleService
    {
        readonly IScheduleRepository repository;
        readonly IUserRepository userRepository;
        readonly CurrentSessionHolder sessionHolder;

        public ScheduleService(IScheduleRepository repository, IUserRepository userRepository, CurrentSessionHolder sessionHolder)
        {
            this.repository     = Guard.NotNull(repository, nameof(repository));
            this.userRepository = Guard.NotNull(userRepository, nameof(userRepository));
            this.sessionHolder  = Guard.NotNull(sessionHolder, nameof(sessionHolder));
        }

        public Task<Result<IReadOnlyList<Schedule>>> GetMyScheduleAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn)
                return Task.FromResult(Result.Fail<IReadOnlyList<Schedule>>("Not signed in."));
            return repository.ListForUserAsync(sessionHolder.CurrentUserId, fromUtc, toUtc, ct);
        }

        public async Task<Result<IReadOnlyList<Schedule>>> GetVisibleScheduleAsync(string ownerUserId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn)
                return Result.Fail<IReadOnlyList<Schedule>>("Not signed in.");

            // Visibility rule: the owner's privacy settings gate the whole schedule.
            var privacy = await userRepository.GetPrivacySettingsAsync(ownerUserId, ct);
            if (privacy.IsFailure) return Result.Fail<IReadOnlyList<Schedule>>(privacy.Error);
            if (privacy.Value != null && !privacy.Value.ShowSchedule)
                return Result.Fail<IReadOnlyList<Schedule>>("This user's schedule is private.");

            return await repository.ListForUserAsync(ownerUserId, fromUtc, toUtc, ct);
        }

        public Task<Result> CreateAsync(Schedule schedule, CancellationToken ct = default)
        {
            var validation = Validate(schedule);
            if (validation.IsFailure) return Task.FromResult(validation);
            return repository.CreateAsync(schedule, ct);
        }

        public Task<Result> UpdateAsync(Schedule schedule, CancellationToken ct = default)
        {
            var validation = Validate(schedule);
            if (validation.IsFailure) return Task.FromResult(validation);
            return repository.UpdateAsync(schedule, ct);
        }

        public Task<Result> DeleteAsync(string scheduleId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(scheduleId))
                return Task.FromResult(Result.Fail("Schedule id is required."));
            return repository.DeleteAsync(scheduleId, ct);
        }

        public Task<Result> GrantViewAsync(string scheduleId, string userId, bool canView, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(scheduleId) || string.IsNullOrEmpty(userId))
                return Task.FromResult(Result.Fail("Schedule and user ids are required."));
            return repository.UpsertPermissionAsync(new SchedulePermission
            {
                ScheduleId = scheduleId,
                UserId = userId,
                CanView = canView
            }, ct);
        }

        static Result Validate(Schedule schedule)
        {
            if (schedule == null) return Result.Fail("Schedule is required.");
            if (string.IsNullOrWhiteSpace(schedule.Title)) return Result.Fail("A schedule needs a title.");
            if (schedule.EndTime <= schedule.StartTime) return Result.Fail("End time must be after start time.");
            return Result.Ok();
        }
    }
}
