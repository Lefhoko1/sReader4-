using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Core.Configuration;

namespace SReader.Infrastructure.Supabase
{
    /// <summary>
    /// Shared plumbing for every Supabase-backed repository. The concrete
    /// REST/postgrest calls are Phase-gated; until implemented each method
    /// returns a clear failure Result instead of throwing, so the UI shows
    /// a readable message rather than crashing.
    /// </summary>
    public abstract class SupabaseRepositoryBase
    {
        protected AppSettings Settings { get; }

        protected SupabaseRepositoryBase(AppSettings settings)
        {
            Settings = Guard.NotNull(settings, nameof(settings));
        }

        protected Result CheckConfigured()
        {
            return Settings.IsSupabaseConfigured
                ? Result.Ok()
                : Result.Fail("Supabase is not configured — set the URL and anon key on the AppSettings asset.");
        }

        protected Task<Result> TodoAsync(string operation)
            => Task.FromResult(Todo(operation));

        protected Task<Result<T>> TodoAsync<T>(string operation)
            => Task.FromResult(Result.Fail<T>(Todo(operation).Error));

        Result Todo(string operation)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;
            return Result.Fail($"'{operation}' is not connected to Supabase yet.");
        }
    }
}
