-- sReader — academies owned by tutors, plus students' join requests.
-- Run in the Supabase SQL editor (project wmfaumjseuzhlwwhaudy) or via
-- `supabase db push`. Safe to re-run.

-- ── academies ─────────────────────────────────────────────────────────────
create table if not exists public.academies (
    id          uuid primary key default gen_random_uuid(),
    owner_id    uuid not null references auth.users (id) on delete cascade,
    name        text not null,
    description text,
    city        text,
    country     text,
    created_at  timestamptz not null default now()
);

create index if not exists academies_owner_idx on public.academies (owner_id);

-- ── academy_join_requests ─────────────────────────────────────────────────
create table if not exists public.academy_join_requests (
    id           uuid primary key default gen_random_uuid(),
    academy_id   uuid not null references public.academies (id) on delete cascade,
    academy_name text,
    student_id   uuid not null references auth.users (id) on delete cascade,
    student_name text,
    status       text not null default 'pending',
    created_at   timestamptz not null default now()
);

create index if not exists join_requests_academy_idx on public.academy_join_requests (academy_id);
create index if not exists join_requests_student_idx on public.academy_join_requests (student_id);

-- ── Row-Level Security ─────────────────────────────────────────────────────
alter table public.academies            enable row level security;
alter table public.academy_join_requests enable row level security;

-- Academies: everyone signed in can browse; only the owner can write.
drop policy if exists "academies readable" on public.academies;
create policy "academies readable" on public.academies
    for select using (auth.uid() is not null);

drop policy if exists "academies owner insert" on public.academies;
create policy "academies owner insert" on public.academies
    for insert with check (owner_id = auth.uid());

drop policy if exists "academies owner update" on public.academies;
create policy "academies owner update" on public.academies
    for update using (owner_id = auth.uid()) with check (owner_id = auth.uid());

drop policy if exists "academies owner delete" on public.academies;
create policy "academies owner delete" on public.academies
    for delete using (owner_id = auth.uid());

-- Join requests:
--   a student creates and reads their own;
--   the owning tutor reads and updates requests for their academies.
drop policy if exists "requests student insert" on public.academy_join_requests;
create policy "requests student insert" on public.academy_join_requests
    for insert with check (student_id = auth.uid());

drop policy if exists "requests student read" on public.academy_join_requests;
create policy "requests student read" on public.academy_join_requests
    for select using (student_id = auth.uid());

drop policy if exists "requests owner read" on public.academy_join_requests;
create policy "requests owner read" on public.academy_join_requests
    for select using (exists (
        select 1 from public.academies a
        where a.id = academy_id and a.owner_id = auth.uid()
    ));

drop policy if exists "requests owner update" on public.academy_join_requests;
create policy "requests owner update" on public.academy_join_requests
    for update using (exists (
        select 1 from public.academies a
        where a.id = academy_id and a.owner_id = auth.uid()
    ));
