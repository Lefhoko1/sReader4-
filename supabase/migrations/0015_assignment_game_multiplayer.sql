-- sReader — multiplayer sessions for the assignment game. Students host/join a
-- session for an assignment and play together while a live scoreboard tracks
-- everyone. (Playing solo needs no session at all.)
-- Run in the Supabase SQL editor (project wmfaumjseuzhlwwhaudy) or via
-- `supabase db push`. Safe to re-run. Requires 0014_assignment_workflow.sql.

-- ── assignment_game_sessions ────────────────────────────────────────────────
create table if not exists public.assignment_game_sessions (
    id               uuid primary key default gen_random_uuid(),
    assignment_id    uuid not null references public.assignments (id) on delete cascade,
    class_id         uuid,
    host_id          uuid not null references auth.users (id) on delete cascade,
    host_name        text,
    assignment_title text,
    code             text unique,
    status           text not null default 'open',   -- open | playing | finished
    created_at       timestamptz not null default now()
);

create index if not exists game_sessions_assignment_idx on public.assignment_game_sessions (assignment_id);
create index if not exists game_sessions_code_idx       on public.assignment_game_sessions (code);

-- ── assignment_game_participants ────────────────────────────────────────────
create table if not exists public.assignment_game_participants (
    id           uuid primary key default gen_random_uuid(),
    session_id   uuid not null references public.assignment_game_sessions (id) on delete cascade,
    student_id   uuid not null references auth.users (id) on delete cascade,
    student_name text,
    score        int not null default 0,
    finished     boolean not null default false,
    updated_at   timestamptz not null default now(),
    unique (session_id, student_id)
);

create index if not exists game_participants_session_idx on public.assignment_game_participants (session_id);

-- ── Row-Level Security ──────────────────────────────────────────────────────
alter table public.assignment_game_sessions     enable row level security;
alter table public.assignment_game_participants enable row level security;

-- Sessions are lobbies (not sensitive): any signed-in user can read/join; only
-- the host writes the session itself.
drop policy if exists "game session readable" on public.assignment_game_sessions;
create policy "game session readable" on public.assignment_game_sessions
    for select using (auth.uid() is not null);

drop policy if exists "game session host insert" on public.assignment_game_sessions;
create policy "game session host insert" on public.assignment_game_sessions
    for insert with check (host_id = auth.uid());

drop policy if exists "game session host update" on public.assignment_game_sessions;
create policy "game session host update" on public.assignment_game_sessions
    for update using (host_id = auth.uid()) with check (host_id = auth.uid());

drop policy if exists "game session host delete" on public.assignment_game_sessions;
create policy "game session host delete" on public.assignment_game_sessions
    for delete using (host_id = auth.uid());

-- Participants: anyone signed in can read the scoreboard; a player only writes
-- their own row.
drop policy if exists "game participant readable" on public.assignment_game_participants;
create policy "game participant readable" on public.assignment_game_participants
    for select using (auth.uid() is not null);

drop policy if exists "game participant self insert" on public.assignment_game_participants;
create policy "game participant self insert" on public.assignment_game_participants
    for insert with check (student_id = auth.uid());

drop policy if exists "game participant self update" on public.assignment_game_participants;
create policy "game participant self update" on public.assignment_game_participants
    for update using (student_id = auth.uid()) with check (student_id = auth.uid());

drop policy if exists "game participant self delete" on public.assignment_game_participants;
create policy "game participant self delete" on public.assignment_game_participants
    for delete using (student_id = auth.uid());
