-- sReader — profile pictures (avatars). Adds the avatar URL column on profiles
-- and a public Storage bucket for the images, with the same role-independent
-- upload policy used for payment proofs.
-- Run in the Supabase SQL editor (project wmfaumjseuzhlwwhaudy). Safe to re-run.

alter table public.user_profiles add column if not exists avatar_url text;

-- Public-read bucket; paths include the user id + a random suffix.
insert into storage.buckets (id, name, public)
values ('avatars', 'avatars', true)
on conflict (id) do update set public = true;

drop policy if exists "avatars upload" on storage.objects;
create policy "avatars upload" on storage.objects
    for insert
    with check (bucket_id = 'avatars' and auth.uid() is not null);

drop policy if exists "avatars update" on storage.objects;
create policy "avatars update" on storage.objects
    for update
    using (bucket_id = 'avatars' and auth.uid() is not null)
    with check (bucket_id = 'avatars' and auth.uid() is not null);

drop policy if exists "avatars read" on storage.objects;
create policy "avatars read" on storage.objects
    for select
    using (bucket_id = 'avatars' and auth.uid() is not null);
