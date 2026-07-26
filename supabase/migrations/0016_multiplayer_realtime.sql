-- sReader — turns the assignment game into a true shared room: live presence
-- (who's in the room, who dropped), a shared turn-based board (one player acts at
-- a time and every screen sees it), and live points. Built on the same Supabase
-- project (no new vendor): the app polls these tables, and Supabase Realtime is
-- enabled on them so the WebSocket fast-path can push the same changes instantly.
-- Run in the Supabase SQL editor (project wmfaumjseuzhlwwhaudy) or `supabase db push`.
-- Safe to re-run. Requires 0015_assignment_game_multiplayer.sql.

-- ── Presence + turn order on participants ───────────────────────────────────
alter table public.assignment_game_participants
    add column if not exists last_seen   timestamptz not null default now(),
    add column if not exists avatar_url   text,
    add column if not exists turn_order   int not null default 0;

create index if not exists game_participants_lastseen_idx
    on public.assignment_game_participants (session_id, last_seen);

-- ── Turn state on the session ───────────────────────────────────────────────
alter table public.assignment_game_sessions
    add column if not exists current_turn_id uuid,   -- whose turn it is (student_id)
    add column if not exists turn_index      int not null default 0,
    add column if not exists state_version   int not null default 0,
    add column if not exists started_at      timestamptz;

-- ── Shared event stream (the "broadcast" log) ───────────────────────────────
-- Every meaningful action — a player tapping/solving a board token, the turn
-- passing, someone finishing — is appended here. Clients read events after the
-- last sequence they saw and apply them, so every screen converges on the same
-- board. seq is a monotonic per-table cursor.
create table if not exists public.assignment_game_events (
    id          uuid primary key default gen_random_uuid(),
    seq         bigserial,
    session_id  uuid not null references public.assignment_game_sessions (id) on delete cascade,
    actor_id    uuid not null references auth.users (id) on delete cascade,
    actor_name  text,
    type        text not null,        -- select | solve | turn | finish | start | reset
    token_index int,                  -- which board activity (deterministic order)
    points      int not null default 0,
    payload     text,                 -- small JSON for anything extra
    created_at  timestamptz not null default now()
);

create index if not exists game_events_session_seq_idx
    on public.assignment_game_events (session_id, seq);

-- ── Row-Level Security ──────────────────────────────────────────────────────
alter table public.assignment_game_events enable row level security;

-- Members of any session (any signed-in user — sessions are lobbies) can read
-- the event stream; a player may only append events authored as themselves.
drop policy if exists "game event readable" on public.assignment_game_events;
create policy "game event readable" on public.assignment_game_events
    for select using (auth.uid() is not null);

drop policy if exists "game event self insert" on public.assignment_game_events;
create policy "game event self insert" on public.assignment_game_events
    for insert with check (actor_id = auth.uid());

-- Any signed-in player may advance the shared session turn state (the turn must
-- be able to pass even when the host has dropped). The session row itself is not
-- sensitive; host-only insert/delete from 0015 still apply.
drop policy if exists "game session turn update" on public.assignment_game_sessions;
create policy "game session turn update" on public.assignment_game_sessions
    for update using (auth.uid() is not null) with check (auth.uid() is not null);

-- ── Enable Supabase Realtime on the multiplayer tables (WebSocket fast-path) ─
-- Adding to the supabase_realtime publication lets the client subscribe over the
-- WebSocket instead of polling. Guarded so re-running never errors.
do $$
begin
    begin execute 'alter publication supabase_realtime add table public.assignment_game_sessions';     exception when duplicate_object then null; when others then null; end;
    begin execute 'alter publication supabase_realtime add table public.assignment_game_participants'; exception when duplicate_object then null; when others then null; end;
    begin execute 'alter publication supabase_realtime add table public.assignment_game_events';       exception when duplicate_object then null; when others then null; end;
end $$;

-- Realtime sends full row data on updates/deletes only when REPLICA IDENTITY FULL.
alter table public.assignment_game_sessions     replica identity full;
alter table public.assignment_game_participants replica identity full;
alter table public.assignment_game_events       replica identity full;
