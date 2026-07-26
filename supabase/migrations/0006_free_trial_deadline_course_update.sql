-- sReader — free trials, course deadlines, and the missing UPDATE policy that
-- was silently blocking course/price edits (PATCH affected 0 rows under RLS).
-- Run in the Supabase SQL editor (project wmfaumjseuzhlwwhaudy). Safe to re-run.

-- ── new course fields ──────────────────────────────────────────────────────
alter table public.academy_courses add column if not exists free_trial boolean not null default false;
alter table public.academy_courses add column if not exists deadline   text;   -- 'yyyy-MM-dd' or null

-- ── free-trial flag on enrollment requests ─────────────────────────────────
alter table public.course_enrollment_requests add column if not exists is_free_trial boolean not null default false;

-- ── FIX: academy_courses had insert/delete policies but no UPDATE policy, so
--        editing a course (e.g. its price) updated nothing. Add it.
drop policy if exists "courses owner update" on public.academy_courses;
create policy "courses owner update" on public.academy_courses
    for update using (exists (
        select 1 from public.academy_grades g
        join public.academies a on a.id = g.academy_id
        where g.id = grade_id and a.owner_id = auth.uid()
    ))
    with check (exists (
        select 1 from public.academy_grades g
        join public.academies a on a.id = g.academy_id
        where g.id = grade_id and a.owner_id = auth.uid()
    ));
