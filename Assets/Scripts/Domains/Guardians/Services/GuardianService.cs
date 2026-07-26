using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Guardians.Models;
using SReader.Domains.Guardians.Repositories;

namespace SReader.Domains.Guardians.Services
{
    public sealed class GuardianService : IGuardianService
    {
        readonly IGuardianRepository repository;
        readonly IClock clock;

        public GuardianService(IGuardianRepository repository, IClock clock)
        {
            this.repository = Guard.NotNull(repository, nameof(repository));
            this.clock      = Guard.NotNull(clock, nameof(clock));
        }

        public Task<Result> AssignStudentAsync(string guardianId, string studentId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(guardianId) || string.IsNullOrEmpty(studentId))
                return Task.FromResult(Result.Fail("Guardian and student ids are required."));
            if (guardianId == studentId)
                return Task.FromResult(Result.Fail("A user cannot be their own guardian."));

            return repository.AssignStudentAsync(new GuardianStudent
            {
                GuardianId = guardianId,
                StudentId = studentId,
                AssignedAt = clock.UtcNow
            }, ct);
        }

        public Task<Result> RemoveStudentAsync(string guardianId, string studentId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(guardianId) || string.IsNullOrEmpty(studentId))
                return Task.FromResult(Result.Fail("Guardian and student ids are required."));
            return repository.RemoveStudentAsync(guardianId, studentId, ct);
        }

        public Task<Result<IReadOnlyList<GuardianStudent>>> ListStudentsAsync(string guardianId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(guardianId))
                return Task.FromResult(Result.Fail<IReadOnlyList<GuardianStudent>>("Guardian id is required."));
            return repository.ListStudentsAsync(guardianId, ct);
        }
    }
}
