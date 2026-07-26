using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Guardians.Models;

namespace SReader.Domains.Guardians.Services
{
    public interface IGuardianService
    {
        Task<Result> AssignStudentAsync(string guardianId, string studentId, CancellationToken ct = default);
        Task<Result> RemoveStudentAsync(string guardianId, string studentId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<GuardianStudent>>> ListStudentsAsync(string guardianId, CancellationToken ct = default);
    }
}
