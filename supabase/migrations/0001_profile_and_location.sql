-- sReader — profile, privacy and location tables for the signed-in landing page.
-- Run in the Supabase SQL editor (project wmfaumjseuzhlwwhaudy) or via
-- `supabase db push`. Every table is keyed by the auth user's UUID and
-- protected by Row-Level Security so a user can only read/write their own row.

-- ── users ────────────────────────────────────────────────────────────────
create table if not exists public.users (
    id           uuid primary key references auth.users (id) on delete cascade,
    email        text,
    username     text unique,
    display_name text,
    role         text not null default 'student',
    status       text not null default 'active',
    created_at   timestamptz not null default now(),
    updated_at   timestamptz not null default now()
);

-- ── user_profiles ───────────────────────────────────────────────────────
create table if not exists public.user_profiles (
    user_id          uuid primary key references auth.users (id) on delete cascade,
    bio              text,
    country          text,
    timezone         text,
    profile_image_id text,
    updated_at       timestamptz not null default now()
);

-- ── privacy_settings ─────────────────────────────────────────────────────
create table if not exists public.privacy_settings (
    user_id            uuid primary key references auth.users (id) on delete cascade,
    show_profile       boolean not null default true,
    show_schedule      boolean not null default false,
    show_location      boolean not null default false,
    show_assignments   boolean not null default false,
    show_online_status boolean not null default true,
    show_friends       boolean not null default true
);

-- ── user_locations ───────────────────────────────────────────────────────
create table if not exists public.user_locations (
    user_id    uuid primary key references auth.users (id) on delete cascade,
    country    text,
    province   text,
    city       text,
    address    text,
    latitude   double precision not null default 0,
    longitude  double precision not null default 0,
    updated_at timestamptz not null default now()
);

-- ── Row-Level Security: each user owns exactly their own rows ─────────────
alter table public.users            enable row level security;
alter table public.user_profiles    enable row level security;
alter table public.privacy_settings enable row level security;
alter table public.user_locations   enable row level security;

create policy "own user row"     on public.users            for all using (auth.uid() = id)      with check (auth.uid() = id);
create policy "own profile row"  on public.user_profiles    for all using (auth.uid() = user_id) with check (auth.uid() = user_id);
create policy "own privacy row"  on public.privacy_settings for all using (auth.uid() = user_id) with check (auth.uid() = user_id);
create policy "own location row" on public.user_locations   for all using (auth.uid() = user_id) with check (auth.uid() = user_id);

-- ── Seed a users row automatically when an auth user is created ───────────
create or replace function public.handle_new_user()
returns trigger language plpgsql security definer set search_path = public as $$
begin
    insert into public.users (id, email, display_name)
    values (new.id, new.email, coalesce(new.raw_user_meta_data ->> 'display_name', ''))
    on conflict (id) do nothing;
    return new;
end;
$$;

drop trigger if exists on_auth_user_created on auth.users;
create trigger on_auth_user_created
    after insert on auth.users
    for each row execute function public.handle_new_user();
