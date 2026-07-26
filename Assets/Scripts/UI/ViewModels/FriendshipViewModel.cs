using System.Collections.Generic;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Social.Models;
using SReader.Domains.Social.Services;
using SReader.UI.Bindings;

namespace SReader.UI.ViewModels
{
    /// <summary>
    /// Backs the Friends section on the student home: the friends list, the
    /// incoming/sent request lists (kept apart), the Discover directory and a
    /// single person's profile screen. Holds the loaded lists and forwards
    /// send/accept/decline/cancel/remove to IFriendshipService. A null service
    /// means placeholder mode (no AppCompositionRoot in the scene).
    /// </summary>
    public sealed class FriendshipViewModel : ViewModelBase
    {
        readonly IFriendshipService friends;

        public IReadOnlyList<FriendView> Friends { get; private set; } = new List<FriendView>();
        public IReadOnlyList<FriendView> Incoming { get; private set; } = new List<FriendView>();
        public IReadOnlyList<FriendView> Sent { get; private set; } = new List<FriendView>();
        public IReadOnlyList<DiscoverPerson> Discovered { get; private set; } = new List<DiscoverPerson>();
        public DiscoverPerson SelectedPerson { get; private set; }
        public IReadOnlyList<StudentAcademicRecord> Academics { get; private set; } = new List<StudentAcademicRecord>();

        public FriendshipViewModel(IFriendshipService friends = null) => this.friends = friends;

        // ── Loads ──

        public async Task<Result> LoadFriendsAsync()
        {
            if (friends == null) return PlaceholderOk();
            IsBusy = true;
            var result = await friends.ListFriendsAsync();
            IsBusy = false;
            if (result.IsSuccess) Friends = result.Value;
            return Report(result);
        }

        public async Task<Result> LoadIncomingAsync()
        {
            if (friends == null) return PlaceholderOk();
            IsBusy = true;
            var result = await friends.ListIncomingRequestsAsync();
            IsBusy = false;
            if (result.IsSuccess) Incoming = result.Value;
            return Report(result);
        }

        public async Task<Result> LoadSentAsync()
        {
            if (friends == null) return PlaceholderOk();
            IsBusy = true;
            var result = await friends.ListSentRequestsAsync();
            IsBusy = false;
            if (result.IsSuccess) Sent = result.Value;
            return Report(result);
        }

        public async Task<Result> LoadDiscoverAsync(string search = null)
        {
            if (friends == null) return PlaceholderOk();
            IsBusy = true;
            var result = await friends.DiscoverPeopleAsync(search);
            IsBusy = false;
            if (result.IsSuccess) Discovered = result.Value;
            return Report(result);
        }

        public async Task<Result> LoadPersonAsync(string userId)
        {
            if (friends == null) return PlaceholderOk();
            IsBusy = true;
            var result = await friends.GetPersonAsync(userId);
            IsBusy = false;
            if (result.IsSuccess) SelectedPerson = result.Value;
            return Report(result);
        }

        public async Task<Result> LoadAcademicsAsync(string userId)
        {
            if (friends == null) return PlaceholderOk();
            IsBusy = true;
            var result = await friends.GetStudentAcademicsAsync(userId);
            IsBusy = false;
            if (result.IsSuccess) Academics = result.Value;
            return Report(result);
        }

        // ── Actions ──

        public async Task<Result> SendRequestAsync(string recipientId)
        {
            if (friends == null) return PlaceholderOk();
            IsBusy = true;
            var result = await friends.SendRequestAsync(recipientId);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> AcceptAsync(string friendshipId)
        {
            if (friends == null) return PlaceholderOk();
            IsBusy = true;
            var result = await friends.AcceptRequestAsync(friendshipId);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> DeclineAsync(string friendshipId)
        {
            if (friends == null) return PlaceholderOk();
            IsBusy = true;
            var result = await friends.DeclineRequestAsync(friendshipId);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> CancelAsync(string friendshipId)
        {
            if (friends == null) return PlaceholderOk();
            IsBusy = true;
            var result = await friends.CancelRequestAsync(friendshipId);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> RemoveAsync(string friendshipId)
        {
            if (friends == null) return PlaceholderOk();
            IsBusy = true;
            var result = await friends.RemoveFriendAsync(friendshipId);
            IsBusy = false;
            return Report(result);
        }

        Result Report(Result result)
        {
            ErrorMessage = result.IsFailure ? result.Error : "";
            return result;
        }

        Result Report<T>(Result<T> result)
        {
            ErrorMessage = result.IsFailure ? result.Error : "";
            return result.IsSuccess ? Result.Ok() : Result.Fail(result.Error);
        }
    }
}
