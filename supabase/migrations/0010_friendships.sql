-- sReader — student friendships, friend-visibility settings, and a friend-safe
-- public directory of users (for discovering and viewing other students).
-- Run in the Supabase SQL editor (project wmfaumjseuzhlwwhaudy) or via
-- `supabase db push`. Safe to re-run.

-- ── friendships ────────────────────────────────────────────────────────────
-- One row per relationship. requester_id sends, recipient_id receives.
-- status: 'pending' | 'accepted' | 'declined' | 'blocked'.
create table if not exists public.friendships (
    id           uuid primary key default gen_random_uuid(),
    requester_id uuid not null references auth.users (id) on delete cascade,
    recipient_id uuid not null references auth.users (id) on delete cascade,
    status       text not null default 'pending',
    created_at   timestamptz not null default now(),
    -- at most one row per ordered pair; either party can still be looked up by
    -- the OR-filter the app uses, so we don't need a second canonical column.
    unique (requester_id, recipient_id),
    check (requester_id <> recipient_id)
);

create index if not exists friendships_requester_idx on public.friendships (requester_id);
create index if not exists friendships_recipient_idx on public.friendships (recipient_id);

-- ── friend_settings ────────────────────────────────────────────────────────
-- What a user's accepted friends are allowed to see (one row per user).
create table if not exists public.friend_settings (
    user_id               uuid primary key references auth.users (id) on delete cascade,
    can_see_profile       boolean not null default true,
    can_see_schedule      boolean not null default false,
    can_see_assignments   boolean not null default false,
    can_see_location      boolean not null default false,
    can_see_online_status boolean not null default true
);

-- ── Row-Level Security ──────────────────────────────────────────────────────
alter table public.friendships    enable row level security;
alter table public.friend_settings enable row level security;

-- Friendships: a user only ever sees / changes rows they are part of.
drop policy if exists "friendships read own" on public.friendships;
create policy "friendships read own" on public.friendships
    for select using (auth.uid() = requester_id or auth.uid() = recipient_id);

-- Only the requester can create the request, and only for themselves.
drop policy if exists "friendships requester insert" on public.friendships;
create policy "friendships requester insert" on public.friendships
    for insert with check (auth.uid() = requester_id);

-- Either party can update the row (recipient accepts/declines; either blocks;
-- requester can revive a declined request).
drop policy if exists "friendships party update" on public.friendships;
create policy "friendships party update" on public.friendships
    for update using (auth.uid() = requester_id or auth.uid() = recipient_id)
            with check (auth.uid() = requester_id or auth.uid() = recipient_id);

-- Either party can delete (remove a friend, cancel a request).
drop policy if exists "friendships party delete" on public.friendships;
create policy "friendships party delete" on public.friendships
    for delete using (auth.uid() = requester_id or auth.uid() = recipient_id);

-- Friend settings: each user owns exactly their own row.
drop policy if exists "friend_settings own row" on public.friend_settings;
create policy "friend_settings own row" on public.friend_settings
    for all using (auth.uid() = user_id) with check (auth.uid() = user_id);

-- ── directory_profiles ──────────────────────────────────────────────────────
-- A friend-safe, read-only directory so signed-in users can discover and view
-- one another. It exposes ONLY public columns (no email): id, username,
-- display_name, role and the latest avatar. The view runs with the owner's
-- rights (security definer, the default), deliberately bypassing the per-row
-- RLS on `users`/`user_profiles` so the whole student body is discoverable —
-- while still hiding private fields because they simply aren't selected.
create or replace view public.directory_profiles as
    select u.id,
           u.username,
           u.display_name,
           u.role,
           p.avatar_url
    from public.users u
    left join public.user_profiles p on p.user_id = u.id;

-- Only signed-in users may read the directory.
revoke all on public.directory_profiles from anon;
grant select on public.directory_profiles to authenticated;
