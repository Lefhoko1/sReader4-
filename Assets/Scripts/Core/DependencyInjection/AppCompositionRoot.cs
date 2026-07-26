using System;
using UnityEngine;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Core.Events;
using SReader.Core.Logging;
using SReader.Domains.Administration.Repositories;
using SReader.Domains.Administration.Services;
using SReader.Domains.Assignments.Repositories;
using SReader.Domains.Assignments.Services;
using SReader.Domains.Education.Repositories;
using SReader.Domains.Education.Services;
using SReader.Domains.Files.Repositories;
using SReader.Domains.Files.Services;
using SReader.Domains.Guardians.Repositories;
using SReader.Domains.Guardians.Services;
using SReader.Domains.Identity.Repositories;
using SReader.Domains.Identity.Services;
using SReader.Domains.Locations.Repositories;
using SReader.Domains.Locations.Services;
using SReader.Domains.Multiplayer.Repositories;
using SReader.Domains.Multiplayer.Services;
using SReader.Domains.Notifications.Repositories;
using SReader.Domains.Notifications.Services;
using SReader.Domains.Scheduling.Repositories;
using SReader.Domains.Scheduling.Services;
using SReader.Domains.Social.Repositories;
using SReader.Domains.Social.Services;
using SReader.Domains.Sync.Repositories;
using SReader.Domains.Sync.Services;
using SReader.Infrastructure.Networking;
using SReader.Infrastructure.Resend;
using SReader.Infrastructure.Realtime;
using SReader.Infrastructure.SQLite;
using SReader.Infrastructure.Storage;
using SReader.Infrastructure.Supabase;
using SReader.UI.Navigation;
using SReader.UI.ViewModels;
using SReader.UI.ViewModels.Home;
using SReader.UI.Views;
using SReader.UI.Views.Home;

namespace SReader.Core.DependencyInjection
{
    /// <summary>
    /// The single place where the object graph is composed — the only code
    /// allowed to call ServiceContainer.Resolve. Add this component to a
    /// GameObject in the scene, assign the AppSettings asset and the page
    /// views, and every screen switches from placeholder mode to the real
    /// service pipeline (UI → ViewModel → Service → Repository → Supabase).
    /// </summary>
    [DefaultExecutionOrder(-100)] // compose before any page's OnEnable runs
    public sealed class AppCompositionRoot : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private AppSettings settings;

        [Tooltip("Offline SQLite cache. Untick to run online-only (use this to isolate a " +
                 "native-sqlite crash on a device). When off, the app behaves exactly as it " +
                 "did before the offline feature was added.")]
        [SerializeField] private bool enableOfflineStorage = true;

        [Tooltip("EDITOR ONLY: force the app to behave as if offline, so you can test the " +
                 "offline experience on PC without rebuilding to a phone. Run once online to " +
                 "warm the cache, stop, tick this, then Play again to see offline behaviour.")]
        [SerializeField] private bool forceOfflineInEditor = false;

        [Header("Views (assign the page GameObjects' controllers)")]
        [SerializeField] private LoginPageController loginView;
        [SerializeField] private RegisterPageController registerView;
        [SerializeField] private ForgotPasswordController forgotPasswordView;
        [SerializeField] private OTPController otpView;
        [SerializeField] private ResetPasswordController resetPasswordView;
        [SerializeField] private ContactUsController contactUsView;
        [SerializeField] private ProfilePageController profileView;

        [Header("Role-specific home views")]
        [SerializeField] private StudentHomeController studentHomeView;
        [SerializeField] private GuardianHomeController guardianHomeView;
        [SerializeField] private TutorHomeController tutorHomeView;

        [Header("Navigation (for post-login redirect on session restore)")]
        [SerializeField] private SReader.UI.Navigation.NavigationManager navigation;

        ServiceContainer container;
        IAppLogger logger;
        SqliteDatabase sqliteDb;   // held so it can be disposed on teardown
        bool tornDown;

        void Awake()
        {
            if (settings == null)
            {
                Debug.LogWarning("[AppCompositionRoot] No AppSettings asset assigned — " +
                                 "pages stay in placeholder mode. Create one via Assets ▸ Create ▸ sReader ▸ App Settings.", this);
                return;
            }

#if UNITY_EDITOR
            // A domain reload (every time a script recompiles, e.g. after a VS Code
            // save) tears down the managed domain but NOT loaded native libraries.
            // Close the native SQLite handle + the websocket BEFORE that happens, or
            // the leaked native state crashes the editor on reload.
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += Teardown;
#endif
            Application.quitting += Teardown;

            BuildContainer();
            InjectViews();
        }

        void OnDestroy() => Teardown();

        /// <summary>
        /// Releases native/background resources deterministically (idempotent).
        /// Without this the open SQLite connection and the realtime socket leak
        /// across editor domain reloads and crash the process.
        /// </summary>
        void Teardown()
        {
            if (tornDown) return;
            tornDown = true;

#if UNITY_EDITOR
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload -= Teardown;
#endif
            Application.quitting -= Teardown;

            // Stop the realtime websocket and its receive/heartbeat loops.
            try { container?.Resolve<SupabaseRealtimeClient>()?.Shutdown(); } catch { /* not built / already gone */ }

            // Drop the static read-cache reference before the db it points at is closed.
            try { SupabaseHttp.ReadCache = null; } catch { /* ignore */ }

            // Close the native SQLite connection last.
            try { sqliteDb?.Dispose(); } catch { /* ignore */ }
            sqliteDb = null;
        }

        async void Start()
        {
            if (container == null) return;

            // Session restore (Phase 2): silently resume the previous login.
            var auth = container.Resolve<IAuthenticationService>();
            var restored = await auth.RestoreSessionAsync();
            logger.Info(restored.IsSuccess ? "Previous session restored." : $"No session restored: {restored.Error}");

            // A restored login skips the auth pages and lands straight on the
            // role-specific home.
            if (restored.IsSuccess && navigation != null)
            {
                var users = container.Resolve<IUserService>();
                await new PostLoginRouter(navigation, users).RouteAsync();
            }
        }

        void BuildContainer()
        {
            container = new ServiceContainer();

            // ── Core ──
            logger = new UnityAppLogger();
            container.RegisterInstance(settings);
            container.RegisterInstance<IAppLogger>(logger);
            container.RegisterInstance<IClock>(new SystemClock());
            container.RegisterInstance<IEventBus>(new EventBus());
            container.RegisterInstance(new CurrentSessionHolder());
            container.RegisterInstance(new PasswordResetFlow());
            container.RegisterInstance<ISessionStore>(new PlayerPrefsSessionStore());
            var unityConnectivity = new UnityConnectivity();
#if UNITY_EDITOR
            unityConnectivity.ForceOffline = forceOfflineInEditor;
            if (forceOfflineInEditor) logger.Info("[Connectivity] Force Offline is ON (editor) — app will behave as offline.");
#endif
            container.RegisterInstance<IConnectivity>(unityConnectivity);

            var offlineStatus = new OfflineStatus();
            container.RegisterInstance(offlineStatus);

            // Shared on-disk SQLite database (offline cache + sync queue + academy mirror).
            // Opening it P/Invokes the native sqlite3 library; if that library is
            // missing on this platform we MUST NOT crash the whole app — offline
            // features just switch off and everything else runs against Supabase.
            sqliteDb = null;
            try
            {
                if (!enableOfflineStorage)
                {
                    logger.Info("[SQLite] Offline storage disabled in the Inspector — running online-only.");
                    throw new InvalidOperationException("Offline storage disabled.");
                }

                sqliteDb = new SqliteDatabase("sreader.db", logger);
                container.RegisterInstance(sqliteDb);

                // Read-through cache for every Supabase GET: successful reads are
                // stored and replayed when offline, so the signed-in user's data
                // stays visible without a connection.
                SupabaseHttp.ReadCache = new SqliteSupabaseReadCache(sqliteDb, container.Resolve<CurrentSessionHolder>());

                offlineStatus.StorageReady = true;
                offlineStatus.Detail = "ready";
                logger.Info("[SQLite] Offline storage ready — data will be cached on this device.");
            }
            catch (Exception ex)
            {
                sqliteDb = null;
                offlineStatus.StorageReady = false;
                offlineStatus.Detail = enableOfflineStorage ? $"unavailable: {ex.Message}" : "disabled";
                logger.Warning("[SQLite] Native sqlite3 unavailable — offline cache/sync disabled, " +
                               $"running online-only. Add the platform's sqlite3 native library to enable it. ({ex.Message})");
            }

            // ── Infrastructure (repositories & gateways) ──
            container.RegisterSingleton<IAuthenticationRepository>(c =>
            {
                var supabase = new SupabaseAuthenticationRepository(settings);
                // Offline login (credentials cached on this device) only when
                // SQLite is available; otherwise plain online-only Supabase auth.
                return sqliteDb != null
                    ? (IAuthenticationRepository)new OfflineAuthenticationRepository(
                        supabase, new OfflineCredentialStore(sqliteDb), c.Resolve<IConnectivity>(), c.Resolve<IAppLogger>())
                    : supabase;
            });
            container.RegisterSingleton<IUserRepository>(c => new SupabaseUserRepository(settings, c.Resolve<CurrentSessionHolder>()));
            container.RegisterSingleton<IGuardianRepository>(c => new SupabaseGuardianRepository(settings));
            container.RegisterSingleton<IEducationRepository>(c =>
            {
                var supabase = new SupabaseEducationRepository(settings, c.Resolve<CurrentSessionHolder>());
                // Offline academy mirror only when SQLite is available; otherwise plain Supabase.
                return sqliteDb != null
                    ? (IEducationRepository)new OfflineFirstEducationRepository(
                        supabase, sqliteDb, c.Resolve<IConnectivity>(), c.Resolve<IAppLogger>())
                    : supabase;
            });
            container.RegisterSingleton<IImagePicker>(c => new NativeGalleryImagePicker());
            container.RegisterSingleton<IPaymentProofUploader>(c => new SupabasePaymentProofUploader(settings, c.Resolve<CurrentSessionHolder>()));
            container.RegisterSingleton<IImageUploader>(c => new SupabaseImageUploader(settings, c.Resolve<CurrentSessionHolder>()));
            container.RegisterSingleton<IAssignmentRepository>(c =>
            {
                var supabase = new SupabaseAssignmentRepository(settings, c.Resolve<CurrentSessionHolder>());
                // Offline schedule mirror only when SQLite is available; otherwise plain Supabase.
                return sqliteDb != null
                    ? (IAssignmentRepository)new OfflineFirstAssignmentRepository(
                        supabase, sqliteDb, c.Resolve<IConnectivity>(), c.Resolve<IAppLogger>())
                    : supabase;
            });
            container.RegisterSingleton<IScheduleRepository>(c => new SupabaseScheduleRepository(settings));
            container.RegisterSingleton<IFriendshipRepository>(c =>
            {
                var supabase = new SupabaseFriendshipRepository(settings, c.Resolve<CurrentSessionHolder>());
                // Offline friendship mirror only when SQLite is available; otherwise plain Supabase.
                return sqliteDb != null
                    ? (IFriendshipRepository)new OfflineFirstFriendshipRepository(
                        supabase, sqliteDb, c.Resolve<IConnectivity>(), c.Resolve<IAppLogger>())
                    : supabase;
            });
            container.RegisterSingleton<INotificationRepository>(c => new SupabaseNotificationRepository(settings));
            container.RegisterSingleton<IPlayerRepository>(c => new SupabasePlayerRepository(settings));
            container.RegisterSingleton<IUserLocationRepository>(c => new SupabaseUserLocationRepository(settings, c.Resolve<CurrentSessionHolder>()));
            container.RegisterSingleton<IAssetVersionRepository>(c => new SupabaseAssetVersionRepository(settings));
            container.RegisterSingleton<ISupportRepository>(c => new SupabaseSupportRepository(settings));
            container.RegisterSingleton<ISyncQueueRepository>(c => sqliteDb != null
                ? (ISyncQueueRepository)new SqliteSyncQueueRepository(sqliteDb)
                : new InMemorySyncQueueRepository());
            container.RegisterSingleton<ILocalCache>(c => sqliteDb != null
                ? (ILocalCache)new SqliteLocalCache(sqliteDb)
                : new InMemoryLocalCache());
            // Per-device game resume points (every solved step) for solo + multiplayer.
            container.RegisterSingleton<IGameProgressStore>(c => sqliteDb != null
                ? (IGameProgressStore)new SqliteGameProgressStore(sqliteDb)
                : new InMemoryGameProgressStore());
            container.RegisterSingleton<IFileStorage>(c => new LocalFileStorage(Application.persistentDataPath));
            container.RegisterSingleton<IEmailSender>(c => new ResendEmailSender(settings, c.Resolve<IAppLogger>()));
            container.RegisterSingleton<INetworkService>(c => new NetcodeNetworkService(c.Resolve<IAppLogger>()));

            // ── Application services (one per domain) ──
            container.RegisterSingleton<IAuthenticationService>(c => new AuthenticationService(
                c.Resolve<IAuthenticationRepository>(), c.Resolve<ISessionStore>(), c.Resolve<CurrentSessionHolder>(),
                c.Resolve<PasswordResetFlow>(), c.Resolve<IEventBus>(), c.Resolve<IAppLogger>(), c.Resolve<IClock>(),
                c.Resolve<IConnectivity>()));

            container.RegisterSingleton<IUserService>(c => new UserService(
                c.Resolve<IUserRepository>(), c.Resolve<CurrentSessionHolder>(), c.Resolve<IAppLogger>()));

            container.RegisterSingleton<IGuardianService>(c => new GuardianService(
                c.Resolve<IGuardianRepository>(), c.Resolve<IClock>()));

            container.RegisterSingleton<IEducationService>(c => new EducationService(
                c.Resolve<IEducationRepository>(), c.Resolve<CurrentSessionHolder>(), c.Resolve<IClock>()));

            container.RegisterSingleton<IAssignmentService>(c => new AssignmentService(
                c.Resolve<IAssignmentRepository>(), c.Resolve<CurrentSessionHolder>(),
                c.Resolve<IEventBus>(), c.Resolve<IAppLogger>(), c.Resolve<IClock>()));

            container.RegisterSingleton<IAssignmentGameRepository>(c =>
                new SupabaseAssignmentGameRepository(settings, c.Resolve<CurrentSessionHolder>()));

            // ── Multiplayer backend: Supabase Realtime vs Photon PUN ──────────
            // The user PICKS one in-app (no silent fallback). Photon is only
            // selectable when PUN is imported (PHOTON_UNITY_NETWORKING). Both the
            // realtime router and the service router resolve the SAME Photon object
            // (one PUN connection/room owns both roles).
#if PHOTON_UNITY_NETWORKING
            var multiplayerBackend = new MultiplayerBackendSetting(photonAvailable: true);
            container.RegisterSingleton<PhotonGameNetwork>(c =>
                new PhotonGameNetwork(settings, c.Resolve<CurrentSessionHolder>()));
#else
            var multiplayerBackend = new MultiplayerBackendSetting(photonAvailable: false);
#endif
            container.RegisterSingleton<MultiplayerBackendSetting>(c => multiplayerBackend);
            container.RegisterSingleton<SupabaseRealtimeClient>(c =>
                new SupabaseRealtimeClient(settings, c.Resolve<CurrentSessionHolder>()));

            container.RegisterSingleton<IGameRealtime>(c => new SwitchableGameRealtime(
                c.Resolve<MultiplayerBackendSetting>(),
                c.Resolve<SupabaseRealtimeClient>(),
#if PHOTON_UNITY_NETWORKING
                c.Resolve<PhotonGameNetwork>()));
#else
                null));
#endif

            container.RegisterSingleton<IAssignmentGameService>(c => new SwitchableAssignmentGameService(
                c.Resolve<MultiplayerBackendSetting>(),
                new AssignmentGameService(
                    c.Resolve<IAssignmentGameRepository>(), c.Resolve<CurrentSessionHolder>(), c.Resolve<IClock>(),
                    c.Resolve<SupabaseRealtimeClient>()),   // Supabase service always broadcasts on the Supabase socket
#if PHOTON_UNITY_NETWORKING
                c.Resolve<PhotonGameNetwork>()));
#else
                null));
#endif

            container.RegisterSingleton<IScheduleService>(c => new ScheduleService(
                c.Resolve<IScheduleRepository>(), c.Resolve<IUserRepository>(), c.Resolve<CurrentSessionHolder>()));

            container.RegisterSingleton<IFriendshipService>(c => new FriendshipService(
                c.Resolve<IFriendshipRepository>(), c.Resolve<CurrentSessionHolder>(), c.Resolve<IEventBus>(), c.Resolve<IClock>()));

            container.RegisterSingleton<INotificationService>(c => new NotificationService(
                c.Resolve<INotificationRepository>(), c.Resolve<CurrentSessionHolder>(), c.Resolve<IEventBus>(),
                c.Resolve<IAppLogger>(), c.Resolve<IClock>()));

            container.RegisterSingleton<IMultiplayerService>(c => new MultiplayerService(
                c.Resolve<IPlayerRepository>(), c.Resolve<INetworkService>(), c.Resolve<CurrentSessionHolder>(),
                c.Resolve<IAppLogger>(), c.Resolve<IClock>()));

            // Fully qualified: UnityEngine.LocationService (the GPS API) clashes with ours.
            container.RegisterSingleton<ILocationService>(c => new Domains.Locations.Services.LocationService(
                c.Resolve<IUserLocationRepository>(), c.Resolve<IUserRepository>(), c.Resolve<CurrentSessionHolder>()));

            container.RegisterSingleton<IFileVersioningService>(c => new FileVersioningService(
                c.Resolve<IAssetVersionRepository>(), c.Resolve<IClock>()));

            container.RegisterSingleton<ISyncService>(c => new SyncService(
                c.Resolve<ISyncQueueRepository>(), c.Resolve<IAppLogger>(), c.Resolve<IClock>()));

            container.RegisterSingleton<ISupportService>(c => new SupportService(
                c.Resolve<ISupportRepository>(), c.Resolve<IEmailSender>(), c.Resolve<IAppLogger>(), c.Resolve<IClock>()));

            container.RegisterSingleton<OfflineCacheWarmer>(c => new OfflineCacheWarmer(
                c.Resolve<IEventBus>(), c.Resolve<IEducationService>(), c.Resolve<IAssignmentService>(),
                c.Resolve<IUserService>(), c.Resolve<IFriendshipService>(), c.Resolve<CurrentSessionHolder>(),
                c.Resolve<IConnectivity>(), c.Resolve<IAppLogger>()));

            // Eagerly created so its event subscriptions (friend requests,
            // submissions) are live from app start.
            container.Resolve<INotificationService>();

            // Eagerly created so it's subscribed to UserLoggedInEvent before the
            // session-restore in Start() fires it — warms the offline cache on login.
            container.Resolve<OfflineCacheWarmer>();

            logger.Info("Composition root initialised.");
        }

        void InjectViews()
        {
            var auth     = container.Resolve<IAuthenticationService>();
            var support  = container.Resolve<ISupportService>();
            var users    = container.Resolve<IUserService>();
            var location = container.Resolve<ILocationService>();

            // One router decides the post-login landing for both sign-in and sign-up.
            var router = navigation != null ? new PostLoginRouter(navigation, users) : null;

            // Academies CRUD + join requests, shared by the student and tutor homes.
            var education = container.Resolve<IEducationService>();
            var academyVm = new AcademyViewModel(education, container.Resolve<CurrentSessionHolder>(),
                container.Resolve<IImagePicker>(), container.Resolve<IPaymentProofUploader>());

            // Friends / requests / discovery for the student home.
            var friendshipVm = new FriendshipViewModel(container.Resolve<IFriendshipService>());

            // Classes / enrolment / assignments, shared by the student and tutor homes.
            var classVm = new ClassViewModel(education, container.Resolve<IAssignmentService>());

            // The student assignments module (aggregated across enrolled classes).
            var assignmentsVm = new StudentAssignmentsViewModel(
                container.Resolve<IAssignmentService>(), education, users,
                container.Resolve<IFileStorage>(), container.Resolve<CurrentSessionHolder>(),
                container.Resolve<IAssignmentGameService>(), container.Resolve<IGameRealtime>(),
                container.Resolve<MultiplayerBackendSetting>(), container.Resolve<IGameProgressStore>());

            Inject(loginView,          v => { v.Construct(new LoginViewModel(auth)); v.SetPostLoginRouter(router); });
            Inject(registerView,       v => { v.Construct(new RegisterViewModel(auth)); v.SetPostLoginRouter(router); });
            Inject(forgotPasswordView, v => v.Construct(new ForgotPasswordViewModel(auth)));
            Inject(otpView,            v => v.Construct(new OtpViewModel(auth)));
            Inject(resetPasswordView,  v => v.Construct(new ResetPasswordViewModel(auth)));
            Inject(contactUsView,      v => v.Construct(new ContactUsViewModel(support)));
            Inject(profileView,        v => { v.Construct(new ProfileViewModel(users, location, () => auth.LogoutAsync(),
                                                container.Resolve<IImagePicker>(), container.Resolve<IImageUploader>())); v.SetPostLoginRouter(router); });

            var connectivity = container.Resolve<IConnectivity>();
            var offlineStatus = container.Resolve<OfflineStatus>();
            Inject(studentHomeView,    v => { v.Construct(new StudentHomeViewModel(users, () => auth.LogoutAsync())); v.SetAcademyViewModel(academyVm); v.SetFriendshipViewModel(friendshipVm); v.SetClassViewModel(classVm); v.SetAssignmentsViewModel(assignmentsVm); v.SetConnectivity(connectivity); v.SetOfflineStatus(offlineStatus); });
            Inject(guardianHomeView,   v => { v.Construct(new GuardianHomeViewModel(users, () => auth.LogoutAsync())); v.SetConnectivity(connectivity); v.SetOfflineStatus(offlineStatus); });
            Inject(tutorHomeView,      v => { v.Construct(new TutorHomeViewModel(users, () => auth.LogoutAsync())); v.SetAcademyViewModel(academyVm); v.SetClassViewModel(classVm); v.SetConnectivity(connectivity); v.SetOfflineStatus(offlineStatus); });
        }

        void Inject<TView>(TView view, System.Action<TView> construct) where TView : MonoBehaviour
        {
            if (view == null)
            {
                logger.Warning($"{typeof(TView).Name} is not assigned on the AppCompositionRoot — that page stays in placeholder mode.");
                return;
            }
            construct(view);
        }
    }
}
