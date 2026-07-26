using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Social.Models;

namespace SReader.Domains.Social.Repositories
{
    public interface IFriendshipRepository
    {
        Task<Result<Friendship>> GetAsync(string friendshipId, CancellationToken ct = default);
        Task<Result<Friendship>> FindBetweenAsync(string userA, string userB, CancellationToken ct = default);
        Task<Result<IReadOnlyList<Friendship>>> ListForUserAsync(string userId, FriendshipStatus status, CancellationToken ct = default);
        /// <summary>Every friendship the user is part of, in any status (both directions).</summary>
        Task<Result<IReadOnlyList<Friendship>>> ListAllForUserAsync(string userId, CancellationToken ct = default);

        Task<Result> CreateAsync(Friendship friendship, CancellationToken ct = default);
        Task<Result> UpdateStatusAsync(string friendshipId, FriendshipStatus status, CancellationToken ct = default);
        Task<Result> DeleteAsync(string friendshipId, CancellationToken ct = default);

        Task<Result<FriendSettings>> GetFriendSettingsAsync(string userId, CancellationToken ct = default);
        Task<Result> UpsertFriendSettingsAsync(FriendSettings settings, CancellationToken ct = default);

        // ── People directory (for discovering and viewing other users) ──
        /// <summary>Everyone in the friend-safe public directory (name, role, latest avatar).</summary>
        Task<Result<IReadOnlyList<PersonSummary>>> ListPeopleAsync(CancellationToken ct = default);
        Task<Result<PersonSummary>> GetPersonAsync(string userId, CancellationToken ct = default);

        /// <summary>A student's enrolled subjects (academy, grade, subject, tutor) for their public profile.</summary>
        Task<Result<IReadOnlyList<StudentSubject>>> ListStudentAcademicsAsync(string studentId, CancellationToken ct = default);
    }
}
