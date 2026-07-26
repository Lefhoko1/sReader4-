using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Social.Models;

namespace SReader.Domains.Social.Services
{
    /// <summary>
    /// Friendships for the signed-in user. Read methods return UI-ready
    /// aggregates (the <em>other</em> person already resolved to a name + latest
    /// avatar), and the three relationship lists are deliberately kept apart so
    /// a screen can show friends without mixing in pending requests.
    /// </summary>
    public interface IFriendshipService
    {
        // ── Actions ──
        Task<Result> SendRequestAsync(string recipientId, CancellationToken ct = default);
        Task<Result> AcceptRequestAsync(string friendshipId, CancellationToken ct = default);
        Task<Result> DeclineRequestAsync(string friendshipId, CancellationToken ct = default);
        /// <summary>Withdraw a pending request I sent.</summary>
        Task<Result> CancelRequestAsync(string friendshipId, CancellationToken ct = default);
        Task<Result> RemoveFriendAsync(string friendshipId, CancellationToken ct = default);
        Task<Result> BlockAsync(string friendshipId, CancellationToken ct = default);

        // ── Separate relationship lists (other person resolved) ──
        Task<Result<IReadOnlyList<FriendView>>> ListFriendsAsync(CancellationToken ct = default);
        /// <summary>Pending requests other people sent to me — I accept or reject these.</summary>
        Task<Result<IReadOnlyList<FriendView>>> ListIncomingRequestsAsync(CancellationToken ct = default);
        /// <summary>Pending requests I sent that haven't been answered yet.</summary>
        Task<Result<IReadOnlyList<FriendView>>> ListSentRequestsAsync(CancellationToken ct = default);

        // ── Discovery + profiles ──
        /// <summary>Other students I can befriend, each tagged with our current relationship.</summary>
        Task<Result<IReadOnlyList<DiscoverPerson>>> DiscoverPeopleAsync(string search = null, CancellationToken ct = default);
        /// <summary>One person's public profile, with our relationship, for the profile screen.</summary>
        Task<Result<DiscoverPerson>> GetPersonAsync(string userId, CancellationToken ct = default);
        /// <summary>A student's academies/grades/subjects/tutors, grouped for their profile.</summary>
        Task<Result<IReadOnlyList<StudentAcademicRecord>>> GetStudentAcademicsAsync(string userId, CancellationToken ct = default);

        // ── My friend-visibility settings ──
        Task<Result<FriendSettings>> GetMyFriendSettingsAsync(CancellationToken ct = default);
        Task<Result> UpdateMyFriendSettingsAsync(FriendSettings settings, CancellationToken ct = default);
    }
}
