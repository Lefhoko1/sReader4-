using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Domains.Guardians.Models;
using SReader.Domains.Guardians.Repositories;

namespace SReader.Infrastructure.Supabase
{
    /// <summary>TODO: guardian_students table.</summary>
    public sealed class SupabaseGuardianRepository : SupabaseRepositoryBase, IGuardianRepository
    {
        public SupabaseGuardianRepository(AppSettings settings) : base(settings) { }

        public Task<Result> AssignStudentAsync(GuardianStudent link, CancellationToken ct = default)
            => TodoAsync("Assign student");

        public Task<Result> RemoveStudentAsync(string guardianId, string studentId, CancellationToken ct = default)
            => TodoAsync("Remove student");

        public Task<Result<IReadOnlyList<GuardianStudent>>> ListStudentsAsync(string guardianId, CancellationToken ct = default)
            => TodoAsync<IReadOnlyList<GuardianStudent>>("List students");
    }
}
