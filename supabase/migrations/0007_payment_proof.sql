-- sReader — proof-of-payment image upload. Adds the proof URL column and a
-- Supabase Storage bucket for the images.
-- Run in the Supabase SQL editor (project wmfaumjseuzhlwwhaudy). Safe to re-run.

-- URL of the uploaded proof image on each enrollment request.
alter table public.course_enrollment_requests
    add column if not exists payment_proof_url text;

-- ── Storage bucket for proof images ────────────────────────────────────────
-- Public-read so the tutor can view the image by URL; object paths include a
-- random uuid so they're effectively unguessable.
insert into storage.buckets (id, name, public)
values ('payment-proofs', 'payment-proofs', true)
on conflict (id) do update set public = true;

-- Any signed-in user may upload (students submitting proof). Reads go through
-- the public URL, so no select policy is required.
drop policy if exists "proof images upload" on storage.objects;
create policy "proof images upload" on storage.objects
    for insert to authenticated
    with check (bucket_id = 'payment-proofs');

drop policy if exists "proof images update" on storage.objects;
create policy "proof images update" on storage.objects
    for update to authenticated
    using (bucket_id = 'payment-proofs')
    with check (bucket_id = 'payment-proofs');
