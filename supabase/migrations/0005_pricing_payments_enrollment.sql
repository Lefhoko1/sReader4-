-- sReader — paid tutoring: per-course pricing, tutor payment details, and
-- paid enrollment requests (student pays, submits proof, tutor enrolls).
-- Run in the Supabase SQL editor (project wmfaumjseuzhlwwhaudy). Safe to re-run.

-- ── pricing on courses/subjects/modules ────────────────────────────────────
alter table public.academy_courses add column if not exists price       numeric not null default 0;
alter table public.academy_courses add column if not exists time_frame  text;

-- ── tutor payment details (single source of truth, one row per tutor) ──────
create table if not exists public.tutor_payment_details (
    user_id      uuid primary key references auth.users (id) on delete cascade,
    account_name text,
    fnb_account  text,
    orange_money text,
    updated_at   timestamptz not null default now()
);

alter table public.tutor_payment_details enable row level security;

-- Readable by any signed-in user (a student must see how to pay the tutor);
-- writable only by the owner.
drop policy if exists "payment details readable" on public.tutor_payment_details;
create policy "payment details readable" on public.tutor_payment_details
    for select using (auth.uid() is not null);

drop policy if exists "payment details owner write" on public.tutor_payment_details;
create policy "payment details owner write" on public.tutor_payment_details
    for all using (user_id = auth.uid()) with check (user_id = auth.uid());

-- ── course enrollment requests (paid) ──────────────────────────────────────
create table if not exists public.course_enrollment_requests (
    id                uuid primary key default gen_random_uuid(),
    course_id         uuid not null references public.academy_courses (id) on delete cascade,
    course_name       text,
    grade_title       text,
    academy_id        uuid references public.academies (id) on delete cascade,
    academy_name      text,
    owner_id          uuid not null references auth.users (id) on delete cascade,
    student_id        uuid not null references auth.users (id) on delete cascade,
    student_name      text,
    status            text not null default 'requested',
    payment_reference text,
    created_at        timestamptz not null default now()
);

create index if not exists enrollment_owner_idx   on public.course_enrollment_requests (owner_id);
create index if not exists enrollment_student_idx on public.course_enrollment_requests (student_id);

alter table public.course_enrollment_requests enable row level security;

-- Student creates and reads their own; the owning tutor reads theirs. Both may
-- update their own rows (student submits payment, tutor enrolls/rejects).
drop policy if exists "enrollment student insert" on public.course_enrollment_requests;
create policy "enrollment student insert" on public.course_enrollment_requests
    for insert with check (student_id = auth.uid());

drop policy if exists "enrollment student read" on public.course_enrollment_requests;
create policy "enrollment student read" on public.course_enrollment_requests
    for select using (student_id = auth.uid());

drop policy if exists "enrollment owner read" on public.course_enrollment_requests;
create policy "enrollment owner read" on public.course_enrollment_requests
    for select using (owner_id = auth.uid());

drop policy if exists "enrollment student update" on public.course_enrollment_requests;
create policy "enrollment student update" on public.course_enrollment_requests
    for update using (student_id = auth.uid()) with check (student_id = auth.uid());

drop policy if exists "enrollment owner update" on public.course_enrollment_requests;
create policy "enrollment owner update" on public.course_enrollment_requests
    for update using (owner_id = auth.uid()) with check (owner_id = auth.uid());
