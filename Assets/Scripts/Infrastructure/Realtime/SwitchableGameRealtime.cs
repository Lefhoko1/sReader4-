using System;
using System.Threading.Tasks;
using SReader.Core.Configuration;
using SReader.Domains.Assignments.Models;
using SReader.Domains.Assignments.Services;

namespace SReader.Infrastructure.Realtime
{
    /// <summary>
    /// Routes <see cref="IGameRealtime"/> calls to the backend the user has selected
    /// (<see cref="MultiplayerBackendSetting"/>) — Supabase Realtime or Photon PUN.
    /// The choice is explicit; if Photon is selected but not compiled in (photon is
    /// null) it uses Supabase so the app never hard-fails.
    /// </summary>
    public sealed class SwitchableGameRealtime : IGameRealtime
    {
        readonly MultiplayerBackendSetting setting;
        readonly IGameRealtime supabase;
        readonly IGameRealtime photon;   // null when PUN isn't imported

        public SwitchableGameRealtime(MultiplayerBackendSetting setting, IGameRealtime supabase, IGameRealtime photon)
        {
            this.setting = setting;
            this.supabase = supabase;
            this.photon = photon;
        }

        IGameRealtime Active =>
            (setting != null && setting.UsingPhoton && photon != null) ? photon : supabase;

        public bool Connected => Active != null && Active.Connected;

        public Task<bool> JoinAsync(string sessionId, Action<GameEvent> onEvent)
            => Active != null ? Active.JoinAsync(sessionId, onEvent) : Task.FromResult(false);

        public void Leave(string sessionId) => Active?.Leave(sessionId);

        public Task BroadcastAsync(string sessionId, GameEvent gameEvent)
            => Active != null ? Active.BroadcastAsync(sessionId, gameEvent) : Task.CompletedTask;
    }
}
