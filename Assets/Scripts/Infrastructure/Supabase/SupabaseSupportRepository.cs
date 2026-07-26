using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Domains.Administration.Models;
using SReader.Domains.Administration.Repositories;

namespace SReader.Infrastructure.Supabase
{
    /// <summary>TODO: support_messages table.</summary>
    public sealed class SupabaseSupportRepository : SupabaseRepositoryBase, ISupportRepository
    {
        public SupabaseSupportRepository(AppSettings settings) : base(settings) { }

        public Task<Result> SaveMessageAsync(SupportMessage message, CancellationToken ct = default)
            => TodoAsync("Send support message");
    }
}
