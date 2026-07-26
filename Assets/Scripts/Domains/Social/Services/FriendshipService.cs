using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Events;
using SReader.Domains.Identity.Models;
using SReader.Domains.Social.Events;
using SReader.Domains.Social.Models;
using SReader.Domains.Social.Repositories;

namespace SReader.Domains.Social.Services
{
    public sealed class FriendshipService : IFriendshipService
    {
        readonly IFriendshipRepository repository;
        readonly CurrentSessionHolder sessionHolder;
        readonly IEventBus eventBus;
        readonly IClock clock;

        public FriendshipService(IFriendshipRepository repository, CurrentSessionHolder sessionHolder, IEventBus eventBus, IClock clock)
        {
            this.repository    = Guard.NotNull(repository, nameof(repository));
            this.sessionHolder = Guard.NotNull(sessionHolder, nameof(sessionHolder));
            this.eventBus      = Guard.NotNull(eventBus, nameof(eventBus));
            this.clock         = Guard.NotNull(clock, nameof(clock));
        }

        // ── Actions ─────────────────────────────────────────────────────────

        public async Task<Result> SendRequestAsync(string recipientId, CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn) return Result.Fail("Not signed in.");
            var me = sessionHolder.CurrentUserId;

            if (string.IsNullOrEmpty(recipientId)) return Result.Fail("Recipient is required.");
            if (recipientId == me) return Result.Fail("You cannot send a friend request to yourself.");

            var existing = await repository.FindBetweenAsync(me, recipientId, ct);
            if (existing.IsSuccess && existing.Value != null)
            {
                var row = existing.Value;
                switch (row.Status)
                {
                    case FriendshipStatus.Accepted: return Result.Fail("You are already friends.");
                    case FriendshipStatus.Blocked:  return Result.Fail("This user cannot be added.");
                    case FriendshipStatus.Pending:
                        return row.RecipientId == me
                            ? Result.Fail("This person already sent you a request — check your Requests.")
                            : Result.Fail("A request is already pending.");
                    case FriendshipStatus.Declined:
                        // Re-offer a request I previously had declined by reviving the row.
                        if (row.RequesterId == me)
                        {
                            var revived = await repository.UpdateStatusAsync(row.Id, FriendshipStatus.Pending, ct);
                            if (revived.IsSuccess) eventBus.Publish(new FriendRequestSentEvent(me, recipientId));
                            return revived;
                        }
                        break; // they declined me before — a fresh request in my direction is fine.
                }
            }

            var friendship = new Friendship
            {
                RequesterId = me,
                RecipientId = recipientId,
                Status = FriendshipStatus.Pending,
                CreatedAt = clock.UtcNow
            };

            var created = await repository.CreateAsync(friendship, ct);
            if (created.IsFailure) return created;

            eventBus.Publish(new FriendRequestSentEvent(me, recipientId));
            return Result.Ok();
        }

        public async Task<Result> AcceptRequestAsync(string friendshipId, CancellationToken ct = default)
        {
            var result = await ChangeStatusAsync(friendshipId, FriendshipStatus.Accepted, ct);
            if (result.IsSuccess)
                eventBus.Publish(new FriendRequestAcceptedEvent(friendshipId));
            return result;
        }

        public Task<Result> DeclineRequestAsync(string friendshipId, CancellationToken ct = default)
            => ChangeStatusAsync(friendshipId, FriendshipStatus.Declined, ct);

        public Task<Result> BlockAsync(string friendshipId, CancellationToken ct = default)
            => ChangeStatusAsync(friendshipId, FriendshipStatus.Blocked, ct);

        public Task<Result> CancelRequestAsync(string friendshipId, CancellationToken ct = default)
            => RemoveFriendAsync(friendshipId);

        public Task<Result> RemoveFriendAsync(string friendshipId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(friendshipId))
                return Task.FromResult(Result.Fail("Friendship id is required."));
            return repository.DeleteAsync(friendshipId, ct);
        }

        // ── Separate relationship lists ──────────────────────────────────────

        public async Task<Result<IReadOnlyList<FriendView>>> ListFriendsAsync(CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn) return Result.Fail<IReadOnlyList<FriendView>>("Not signed in.");
            var me = sessionHolder.CurrentUserId;

            var rows = await repository.ListForUserAsync(me, FriendshipStatus.Accepted, ct);
            if (rows.IsFailure) return Result.Fail<IReadOnlyList<FriendView>>(rows.Error);

            var directory = await BuildDirectoryAsync(ct);
            var views = rows.Value.Select(f => ToView(f, me, directory)).ToList();
            return Result.Ok<IReadOnlyList<FriendView>>(views);
        }

        public async Task<Result<IReadOnlyList<FriendView>>> ListIncomingRequestsAsync(CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn) return Result.Fail<IReadOnlyList<FriendView>>("Not signed in.");
            var me = sessionHolder.CurrentUserId;

            var rows = await repository.ListForUserAsync(me, FriendshipStatus.Pending, ct);
            if (rows.IsFailure) return Result.Fail<IReadOnlyList<FriendView>>(rows.Error);

            var directory = await BuildDirectoryAsync(ct);
            var views = rows.Value
                .Where(f => f.RecipientId == me)        // requests sent TO me
                .Select(f => ToView(f, me, directory))
                .ToList();
            return Result.Ok<IReadOnlyList<FriendView>>(views);
        }

        public async Task<Result<IReadOnlyList<FriendView>>> ListSentRequestsAsync(CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn) return Result.Fail<IReadOnlyList<FriendView>>("Not signed in.");
            var me = sessionHolder.CurrentUserId;

            var rows = await repository.ListForUserAsync(me, FriendshipStatus.Pending, ct);
            if (rows.IsFailure) return Result.Fail<IReadOnlyList<FriendView>>(rows.Error);

            var directory = await BuildDirectoryAsync(ct);
            var views = rows.Value
                .Where(f => f.RequesterId == me)        // requests I sent
                .Select(f => ToView(f, me, directory))
                .ToList();
            return Result.Ok<IReadOnlyList<FriendView>>(views);
        }

        // ── Discovery + profiles ─────────────────────────────────────────────

        public async Task<Result<IReadOnlyList<DiscoverPerson>>> DiscoverPeopleAsync(string search = null, CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn) return Result.Fail<IReadOnlyList<DiscoverPerson>>("Not signed in.");
            var me = sessionHolder.CurrentUserId;

            var peopleResult = await repository.ListPeopleAsync(ct);
            if (peopleResult.IsFailure) return Result.Fail<IReadOnlyList<DiscoverPerson>>(peopleResult.Error);

            var relationships = await BuildRelationshipMapAsync(me, ct);

            var needle = (search ?? "").Trim();
            var list = new List<DiscoverPerson>();
            foreach (var person in peopleResult.Value)
            {
                if (person.UserId == me) continue;             // never list myself
                if (person.Role != UserRole.Student) continue; // students discover other students
                if (!Matches(person, needle)) continue;

                relationships.TryGetValue(person.UserId, out var rel);
                list.Add(new DiscoverPerson
                {
                    Person = person,
                    State = rel.state,
                    FriendshipId = rel.id
                });
            }

            // People I'm not yet connected to first, then by name.
            var ordered = list
                .OrderBy(p => p.State == RelationshipState.Friends ? 1 : 0)
                .ThenBy(p => p.Person.Label, System.StringComparer.OrdinalIgnoreCase)
                .ToList();
            return Result.Ok<IReadOnlyList<DiscoverPerson>>(ordered);
        }

        public async Task<Result<DiscoverPerson>> GetPersonAsync(string userId, CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn) return Result.Fail<DiscoverPerson>("Not signed in.");
            if (string.IsNullOrEmpty(userId)) return Result.Fail<DiscoverPerson>("User id is required.");
            var me = sessionHolder.CurrentUserId;

            var personResult = await repository.GetPersonAsync(userId, ct);
            if (personResult.IsFailure) return Result.Fail<DiscoverPerson>(personResult.Error);

            var state = RelationshipState.None;
            string friendshipId = null;
            var between = await repository.FindBetweenAsync(me, userId, ct);
            if (between.IsSuccess && between.Value != null)
            {
                var rel = Relationship(between.Value, me);
                state = rel.state;
                friendshipId = rel.id;
            }

            return Result.Ok(new DiscoverPerson { Person = personResult.Value, State = state, FriendshipId = friendshipId });
        }

        public async Task<Result<IReadOnlyList<StudentAcademicRecord>>> GetStudentAcademicsAsync(string userId, CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn) return Result.Fail<IReadOnlyList<StudentAcademicRecord>>("Not signed in.");
            if (string.IsNullOrEmpty(userId)) return Result.Fail<IReadOnlyList<StudentAcademicRecord>>("User id is required.");

            var rows = await repository.ListStudentAcademicsAsync(userId, ct);
            if (rows.IsFailure) return Result.Fail<IReadOnlyList<StudentAcademicRecord>>(rows.Error);

            // Group the per-subject rows into one record per academy+grade.
            var byKey = new Dictionary<string, StudentAcademicRecord>();
            var order = new List<string>();
            foreach (var s in rows.Value)
            {
                var key = (s.AcademyId ?? "") + "|" + (s.GradeTitle ?? "");
                if (!byKey.TryGetValue(key, out var record))
                {
                    record = new StudentAcademicRecord
                    {
                        AcademyId   = s.AcademyId,
                        AcademyName = s.AcademyName,
                        GradeTitle  = s.GradeTitle,
                        TutorName   = s.TutorName
                    };
                    byKey[key] = record;
                    order.Add(key);
                }
                if (!string.IsNullOrWhiteSpace(s.CourseName) && !record.Subjects.Contains(s.CourseName))
                    record.Subjects.Add(s.CourseName);
            }

            var list = order.Select(k => byKey[k]).ToList();
            return Result.Ok<IReadOnlyList<StudentAcademicRecord>>(list);
        }

        // ── My friend-visibility settings ────────────────────────────────────

        public Task<Result<FriendSettings>> GetMyFriendSettingsAsync(CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn)
                return Task.FromResult(Result.Fail<FriendSettings>("Not signed in."));
            return repository.GetFriendSettingsAsync(sessionHolder.CurrentUserId, ct);
        }

        public Task<Result> UpdateMyFriendSettingsAsync(FriendSettings settings, CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            settings.UserId = sessionHolder.CurrentUserId;
            return repository.UpsertFriendSettingsAsync(settings, ct);
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        Task<Result> ChangeStatusAsync(string friendshipId, FriendshipStatus status, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(friendshipId))
                return Task.FromResult(Result.Fail("Friendship id is required."));
            return repository.UpdateStatusAsync(friendshipId, status, ct);
        }

        /// <summary>The signed-in user's id → person summary, for resolving the "other" side.</summary>
        async Task<Dictionary<string, PersonSummary>> BuildDirectoryAsync(CancellationToken ct)
        {
            var map = new Dictionary<string, PersonSummary>();
            var people = await repository.ListPeopleAsync(ct);
            if (people.IsSuccess)
                foreach (var p in people.Value)
                    if (!string.IsNullOrEmpty(p.UserId))
                        map[p.UserId] = p;
            return map;
        }

        async Task<Dictionary<string, (RelationshipState state, string id)>> BuildRelationshipMapAsync(string me, CancellationToken ct)
        {
            var map = new Dictionary<string, (RelationshipState, string)>();
            var all = await repository.ListAllForUserAsync(me, ct);
            if (all.IsFailure) return map;

            foreach (var f in all.Value)
            {
                var other = f.RequesterId == me ? f.RecipientId : f.RequesterId;
                if (string.IsNullOrEmpty(other)) continue;
                var rel = Relationship(f, me);
                // A real relationship (friends/pending/blocked) wins over a stale declined row.
                if (rel.state == RelationshipState.None) continue;
                map[other] = (rel.state, rel.id);
            }
            return map;
        }

        FriendView ToView(Friendship f, string me, IReadOnlyDictionary<string, PersonSummary> directory)
        {
            var otherId = f.RequesterId == me ? f.RecipientId : f.RequesterId;
            directory.TryGetValue(otherId, out var person);
            return new FriendView
            {
                FriendshipId = f.Id,
                Person = person ?? new PersonSummary { UserId = otherId },
                Status = f.Status,
                IsIncoming = f.RecipientId == me
            };
        }

        static (RelationshipState state, string id) Relationship(Friendship f, string me)
        {
            switch (f.Status)
            {
                case FriendshipStatus.Accepted: return (RelationshipState.Friends, f.Id);
                case FriendshipStatus.Blocked:  return (RelationshipState.Blocked, f.Id);
                case FriendshipStatus.Pending:
                    return f.RequesterId == me
                        ? (RelationshipState.RequestSent, f.Id)
                        : (RelationshipState.RequestReceived, f.Id);
                default: return (RelationshipState.None, null); // declined → treat as no relationship
            }
        }

        static bool Matches(PersonSummary p, string needle)
        {
            if (string.IsNullOrEmpty(needle)) return true;
            needle = needle.ToLowerInvariant();
            return (p.DisplayName ?? "").ToLowerInvariant().Contains(needle)
                || (p.Username ?? "").ToLowerInvariant().Contains(needle);
        }
    }
}
