using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Events;
using SReader.Core.Logging;
using SReader.Domains.Assignments.Services;
using SReader.Domains.Education.Services;
using SReader.Domains.Identity.Events;
using SReader.Domains.Identity.Services;
using SReader.Domains.Social.Services;

namespace SReader.Domains.Sync.Services
{
    /// <summary>
    /// Proactively fills the offline read cache for the signed-in user. It
    /// subscribes to <see cref="UserLoggedInEvent"/> — fired on both a fresh
    /// login and a session restore — and, while online, fetches the user's key
    /// data: profile, the academies they own and/or are enrolled in (with each
    /// academy's grades and courses), and everything they've subscribed to
    /// (enrollment requests, join requests, payment details). Each call simply
    /// primes the GET cache, so a later offline launch has a local copy even of
    /// screens the user hasn't opened yet. Best-effort: failures are logged and
    /// ignored, and it never blocks the UI (runs fire-and-forget).
    /// </summary>
    public sealed class OfflineCacheWarmer : IDisposable
    {
        readonly IEducationService education;
        readonly IAssignmentService assignments;
        readonly IUserService users;
        readonly IFriendshipService friends;
        readonly CurrentSessionHolder session;
        readonly IConnectivity connectivity;
        readonly IAppLogger logger;
        readonly IDisposable subscription;

        bool warming;

        public OfflineCacheWarmer(IEventBus eventBus, IEducationService education, IAssignmentService assignments,
            IUserService users, IFriendshipService friends, CurrentSessionHolder session,
            IConnectivity connectivity, IAppLogger logger)
        {
            Guard.NotNull(eventBus, nameof(eventBus));
            this.education    = Guard.NotNull(education, nameof(education));
            this.assignments  = Guard.NotNull(assignments, nameof(assignments));
            this.users        = Guard.NotNull(users, nameof(users));
            this.friends      = Guard.NotNull(friends, nameof(friends));
            this.session      = Guard.NotNull(session, nameof(session));
            this.connectivity = Guard.NotNull(connectivity, nameof(connectivity));
            this.logger       = Guard.NotNull(logger, nameof(logger));

            subscription = eventBus.Subscribe<UserLoggedInEvent>(_ => FireAndForget());
        }

        void FireAndForget()
        {
            if (warming) return;
            _ = WarmAsync();
        }

        public async Task WarmAsync(CancellationToken ct = default)
        {
            if (warming || !connectivity.IsOnline || !session.IsSignedIn) return;
            warming = true;
            try
            {
                // Identity / profile.
                await users.GetCurrentUserAsync(ct);
                if (!string.IsNullOrEmpty(session.CurrentUserId))
                    await users.GetProfileAsync(session.CurrentUserId, ct);

                // Discovery list + the academies this user owns, with full structure.
                await education.ListAcademiesAsync(ct);
                var mine = await education.ListMyAcademiesAsync(ct);
                if (mine.IsSuccess)
                    foreach (var academy in mine.Value)
                        await CacheAcademyStructure(academy.Id, ct);

                // Everything the user "subscribed to".
                var myEnrollments = await education.ListMyEnrollmentRequestsAsync(ct);
                await education.ListEnrollmentRequestsForOwnerAsync(ct);
                await education.ListMyAcademyRequestsAsync(ct);
                await education.GetMyPaymentDetailsAsync(ct);

                // Cache the structure of academies the student is enrolled in / requested.
                if (myEnrollments.IsSuccess)
                {
                    var seen = new HashSet<string>();
                    foreach (var req in myEnrollments.Value)
                        if (!string.IsNullOrEmpty(req.AcademyId) && seen.Add(req.AcademyId))
                            await CacheAcademyStructure(req.AcademyId, ct);
                }

                // Assignments: the classes the student is enrolled in, every
                // assignment in each (with its playable content), plus the student's
                // own attempt status and schedules — so a student can play their
                // assignments alone offline, even screens they haven't opened yet.
                var classes = await education.ListMyClassesAsync(ct);
                if (classes.IsSuccess)
                {
                    var seenClasses = new HashSet<string>();
                    foreach (var enrolment in classes.Value)
                        if (!string.IsNullOrEmpty(enrolment.ClassId) && seenClasses.Add(enrolment.ClassId))
                            await assignments.ListForClassAsync(enrolment.ClassId, ct);
                }
                await assignments.ListMyAttemptsAsync(ct);
                await assignments.ListMySchedulesAsync(ct);

                // Friends: the people directory, the friend list and both request
                // lists, so the whole Friends section works offline.
                await friends.DiscoverPeopleAsync(null, ct);
                await friends.ListFriendsAsync(ct);
                await friends.ListIncomingRequestsAsync(ct);
                await friends.ListSentRequestsAsync(ct);

                logger.Info("[Offline] Cache warm-up complete for the signed-in user.");
            }
            catch (Exception ex)
            {
                logger.Warning($"[Offline] Cache warm-up stopped early: {ex.Message}");
            }
            finally
            {
                warming = false;
            }
        }

        async Task CacheAcademyStructure(string academyId, CancellationToken ct)
        {
            var grades = await education.ListGradesAsync(academyId, ct);
            if (grades.IsFailure) return;
            foreach (var grade in grades.Value)
                await education.ListCoursesAsync(grade.Id, ct);
        }

        public void Dispose() => subscription?.Dispose();
    }
}
