using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Domains.Assignments.Models;
using SReader.Domains.Assignments.Services;
using UnityEngine;

namespace SReader.Infrastructure.Realtime
{
    /// <summary>
    /// Minimal Supabase Realtime (Phoenix channels, vsn=1.0.0) client over a single
    /// WebSocket — just enough for instant Broadcast within game rooms. It connects
    /// lazily on the first join, heartbeats, sends/receives broadcasts, and auto-
    /// reconnects (re-joining its channels). Everything runs on Unity's main thread:
    /// the receive loop is started from the main thread so awaits resume there and
    /// handlers can touch the UI directly (same model as SupabaseHttp). Best-effort —
    /// if the socket can't open, <see cref="Connected"/> stays false and callers fall
    /// back to polling.
    ///
    /// Broadcasts are PUBLIC-channel messages (no postgres_changes / presence), so no
    /// extra RLS setup is needed. The DB remains the source of truth; this only
    /// accelerates delivery.
    /// </summary>
    public sealed class SupabaseRealtimeClient : IGameRealtime
    {
        readonly string socketUrl;
        readonly string anonKey;
        readonly CurrentSessionHolder session;

        ClientWebSocket ws;
        CancellationTokenSource cts;
        int refCounter;
        bool starting;

        public bool Connected { get; private set; }

        // topic -> handler for received game events.
        readonly Dictionary<string, Action<GameEvent>> handlers = new Dictionary<string, Action<GameEvent>>();

        public SupabaseRealtimeClient(AppSettings settings, CurrentSessionHolder session)
        {
            this.session = session;
            anonKey = settings != null ? settings.SupabaseAnonKey : "";
            var b = (settings != null ? settings.SupabaseUrl : "") ?? "";
            b = b.TrimEnd('/').Replace("https://", "wss://").Replace("http://", "ws://");
            socketUrl = b + "/realtime/v1/websocket?apikey=" + anonKey + "&vsn=1.0.0";
        }

        string Token => session != null && session.Session != null && !string.IsNullOrEmpty(session.Session.AccessToken)
            ? session.Session.AccessToken : anonKey;

        static string Topic(string sessionId) => "realtime:game:" + sessionId;

        // ── IGameRealtime ────────────────────────────────────────────────────
        public async Task<bool> JoinAsync(string sessionId, Action<GameEvent> onEvent)
        {
            if (string.IsNullOrEmpty(sessionId)) return false;
            var topic = Topic(sessionId);
            handlers[topic] = onEvent;
            if (!await EnsureConnectedAsync()) return false;
            await SendJoinAsync(topic);
            return true;
        }

        public void Leave(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId)) return;
            var topic = Topic(sessionId);
            handlers.Remove(topic);
            if (Connected) _ = SendRawAsync(Envelope(topic, "phx_leave", "{}"));
        }

        /// <summary>
        /// Stops the socket and all background loops for good. Called at app/teardown
        /// (and before an editor domain reload) so the native ClientWebSocket and its
        /// receive/heartbeat tasks don't leak across a reload — a leaked native socket
        /// is a known cause of editor crashes and mid-session instability on device.
        /// </summary>
        public void Shutdown()
        {
            handlers.Clear();
            Connected = false;
            try { cts?.Cancel(); } catch { /* already disposed */ }
            try { ws?.Abort(); }  catch { /* already gone */ }
            try { ws?.Dispose(); } catch { /* already gone */ }
            ws = null;
        }

        public Task BroadcastAsync(string sessionId, GameEvent e)
        {
            if (!Connected || e == null || string.IsNullOrEmpty(sessionId)) return Task.CompletedTask;
            var inner = JsonUtility.ToJson(Wire.From(e));   // JsonUtility escapes the nested payload string for us
            var payload = "{\"type\":\"broadcast\",\"event\":\"ev\",\"payload\":" + inner + "}";
            return SendRawAsync(Envelope(Topic(sessionId), "broadcast", payload));
        }

        // ── Connection ─────────────────────────────────────────────────────
        async Task<bool> EnsureConnectedAsync()
        {
            if (Connected) return true;
            if (starting) return false;
            starting = true;
            try
            {
                cts = new CancellationTokenSource();
                ws = new ClientWebSocket();
                await ws.ConnectAsync(new Uri(socketUrl), cts.Token);
                Connected = true;
                _ = ReceiveLoop();
                _ = HeartbeatLoop();
                foreach (var t in new List<string>(handlers.Keys)) await SendJoinAsync(t);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[Realtime] connect failed (falling back to polling): " + ex.Message);
                Connected = false;
                return false;
            }
            finally { starting = false; }
        }

        async Task SendJoinAsync(string topic)
        {
            if (!Connected) return;
            // Public channel; self:false so we don't receive our own broadcasts.
            var cfg = "{\"config\":{\"broadcast\":{\"self\":false,\"ack\":false},\"presence\":{\"key\":\"\"},\"postgres_changes\":[]},\"access_token\":\"" + Token + "\"}";
            await SendRawAsync(Envelope(topic, "phx_join", cfg));
        }

        async Task HeartbeatLoop()
        {
            try
            {
                while (Connected && ws != null && ws.State == WebSocketState.Open)
                {
                    await Task.Delay(25000, cts.Token);
                    await SendRawAsync("{\"topic\":\"phoenix\",\"event\":\"heartbeat\",\"payload\":{},\"ref\":\"" + NextRef() + "\"}");
                }
            }
            catch { /* ends on disconnect/cancel */ }
        }

        async Task ReceiveLoop()
        {
            var buffer = new byte[16 * 1024];
            var sb = new StringBuilder();
            try
            {
                while (ws != null && ws.State == WebSocketState.Open)
                {
                    sb.Clear();
                    WebSocketReceiveResult res;
                    do
                    {
                        res = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
                        if (res.MessageType == WebSocketMessageType.Close) { await CloseAsync(); return; }
                        sb.Append(Encoding.UTF8.GetString(buffer, 0, res.Count));
                    } while (!res.EndOfMessage);

                    try { HandleMessage(sb.ToString()); }
                    catch (Exception ex) { Debug.LogWarning("[Realtime] bad message: " + ex.Message); }
                }
            }
            catch (OperationCanceledException) { /* leaving */ }
            catch (Exception ex) { Debug.LogWarning("[Realtime] receive stopped: " + ex.Message); }
            finally
            {
                Connected = false;
                _ = ReconnectSoon();
            }
        }

        async Task ReconnectSoon()
        {
            if (handlers.Count == 0) return;            // nobody waiting → stay closed
            await Task.Delay(2000);
            if (!Connected && handlers.Count > 0) await EnsureConnectedAsync();
        }

        void HandleMessage(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return;
            var head = JsonUtility.FromJson<Head>(raw);
            if (head == null || head.@event != "broadcast") return;   // only game broadcasts matter

            var env = JsonUtility.FromJson<BroadcastEnvelope>(raw);
            if (env == null || env.payload == null || env.payload.payload == null) return;
            if (!handlers.TryGetValue(env.topic, out var handler) || handler == null) return;

            handler(env.payload.payload.ToModel());
        }

        async Task SendRawAsync(string json)
        {
            try
            {
                if (ws == null || ws.State != WebSocketState.Open) return;
                var bytes = Encoding.UTF8.GetBytes(json);
                await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cts.Token);
            }
            catch (Exception ex) { Debug.LogWarning("[Realtime] send failed: " + ex.Message); }
        }

        async Task CloseAsync()
        {
            Connected = false;
            try { if (ws != null && ws.State == WebSocketState.Open) await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", CancellationToken.None); }
            catch { /* ignore */ }
        }

        string Envelope(string topic, string evt, string payloadJson)
            => "{\"topic\":\"" + topic + "\",\"event\":\"" + evt + "\",\"payload\":" + payloadJson + ",\"ref\":\"" + NextRef() + "\"}";

        string NextRef() => (++refCounter).ToString();

        // ── Wire DTOs (JsonUtility-friendly; short keys keep the payload small) ──
        [Serializable] class Head { public string @event; }

        [Serializable]
        class BroadcastEnvelope
        {
            public string topic;
            public string @event;
            public BroadcastPayload payload;
        }

        [Serializable]
        class BroadcastPayload
        {
            public string type;
            public string @event;
            public Wire payload;
        }

        [Serializable]
        class Wire
        {
            public long seq;
            public string sid;   // session id
            public string t;     // type
            public int ti;       // token index
            public int p;        // points
            public string pl;    // payload (mirror json / turn id)
            public string a;     // actor id
            public string an;    // actor name
            public int sc;       // actor total score

            public static Wire From(GameEvent e) => new Wire
            {
                seq = e.Seq, sid = e.SessionId, t = e.Type, ti = e.TokenIndex,
                p = e.Points, pl = e.Payload, a = e.ActorId, an = e.ActorName, sc = e.ActorScore
            };

            public GameEvent ToModel() => new GameEvent
            {
                Seq = seq, SessionId = sid, Type = t, TokenIndex = ti, Points = p,
                Payload = string.IsNullOrEmpty(pl) ? null : pl, ActorId = a, ActorName = an,
                ActorScore = sc, CreatedAt = DateTime.UtcNow
            };
        }
    }
}
