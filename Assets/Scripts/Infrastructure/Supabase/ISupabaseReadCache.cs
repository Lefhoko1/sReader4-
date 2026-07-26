namespace SReader.Infrastructure.Supabase
{
    /// <summary>
    /// Read-through cache for Supabase GET responses. Stores the raw PostgREST
    /// JSON keyed by request URL (scoped to the signed-in user) so the same
    /// bytes can be replayed when the device is offline. The repositories'
    /// existing DTO parsers consume the replayed JSON unchanged.
    /// </summary>
    public interface ISupabaseReadCache
    {
        /// <summary>Remember the JSON body of a successful GET.</summary>
        void Store(string url, string json);

        /// <summary>Return the last good JSON for this GET, if any.</summary>
        bool TryGet(string url, out string json);
    }
}
