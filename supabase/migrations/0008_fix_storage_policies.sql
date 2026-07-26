-- sReader — fix proof-of-payment upload RLS. The previous "to authenticated"
-- policies weren't matching (403 "new row violates row-level security policy").
-- Use a role-independent check: any request carrying a valid user JWT
-- (auth.uid() is not null) may upload to the payment-proofs bucket.
-- Run in the Supabase SQL editor (project wmfaumjseuzhlwwhaudy). Safe to re-run.

-- Make sure the bucket exists and is public-read.
insert into storage.buckets (id, name, public)
values ('payment-proofs', 'payment-proofs', true)
on conflict (id) do update set public = true;

-- Replace the upload/update policies (drop both the old and new names).
drop policy if exists "proof images upload" on storage.objects;
drop policy if exists "proof images update" on storage.objects;
drop policy if exists "proof images read"   on storage.objects;

create policy "proof images upload" on storage.objects
    for insert
    with check (bucket_id = 'payment-proofs' and auth.uid() is not null);

create policy "proof images update" on storage.objects
    for update
    using (bucket_id = 'payment-proofs' and auth.uid() is not null)
    with check (bucket_id = 'payment-proofs' and auth.uid() is not null);

-- Authenticated read too (public URL already works, this covers direct reads).
create policy "proof images read" on storage.objects
    for select
    using (bucket_id = 'payment-proofs' and auth.uid() is not null);
