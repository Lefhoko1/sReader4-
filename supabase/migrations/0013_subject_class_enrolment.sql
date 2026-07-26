-- sReader — link subjects to classes, and let a tutor enrol a paid student.
-- A subject (academy_courses) can be taught in a class (academy_classes); when
-- the tutor confirms a student's paid enrolment for that subject, the app adds
-- the student to that class so they receive its assignments.
-- Run in the Supabase SQL editor (project wmfaumjseuzhlwwhaudy) or via
-- `supabase db push`. Safe to re-run. Requires 0012_classes_and_assignments.sql.

-- ── academy_courses → class link ────────────────────────────────────────────
alter table public.academy_courses
    add column if not exists class_id uuid references public.academy_classes (id) on delete set null;
alter table public.academy_courses
    add column if not exists class_name text;

create index if not exists academy_courses_class_idx on public.academy_courses (class_id);

-- ── class_enrollments: let the owning tutor enrol a student ──────────────────
-- The auto-enrol on payment-confirm runs under the TUTOR's token, inserting a
-- row whose student_id is the student (not the tutor), so the existing
-- "student inserts their own" policy isn't enough. Allow the academy owner to
-- insert enrolments for classes in their own academies.
drop policy if exists "enrol owner insert" on public.class_enrollments;
create policy "enrol owner insert" on public.class_enrollments
    for insert with check (exists (
        select 1 from public.academy_classes c
        join public.academies a on a.id = c.academy_id
        where c.id = class_id and a.owner_id = auth.uid()
    ));
