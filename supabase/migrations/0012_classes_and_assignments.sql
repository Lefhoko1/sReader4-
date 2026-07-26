-- sReader — the Classes module: classes belong to a grade within an academy,
-- students enrol into a class, and tutors assign assignments to a class.
-- Run in the Supabase SQL editor (project wmfaumjseuzhlwwhaudy) or via
-- `supabase db push`. Safe to re-run. Requires 0003_academies.sql and
-- 0004_grades_courses.sql first.

-- ── academy_classes ─────────────────────────────────────────────────────────
-- A class under a grade (e.g. "Form 2 Blue" under "BGCSE"). owner_id is the
-- academy owner (the tutor). academy_name/grade_title are denormalised so a
-- student's "My classes" list reads without extra joins.
create table if not exists public.academy_classes (
    id           uuid primary key default gen_random_uuid(),
    academy_id   uuid not null references public.academies (id) on delete cascade,
    grade_id     uuid not null references public.academy_grades (id) on delete cascade,
    owner_id     uuid not null references auth.users (id) on delete cascade,
    academy_name text,
    grade_title  text,
    name         text not null,
    description  text,
    max_students int not null default 0,
    created_at   timestamptz not null default now()
);

create index if not exists academy_classes_grade_idx   on public.academy_classes (grade_id);
create index if not exists academy_classes_academy_idx on public.academy_classes (academy_id);

-- ── class_enrollments ───────────────────────────────────────────────────────
-- A student's membership of a class. Denormalised names for roster / my-classes.
create table if not exists public.class_enrollments (
    id           uuid primary key default gen_random_uuid(),
    class_id     uuid not null references public.academy_classes (id) on delete cascade,
    student_id   uuid not null references auth.users (id) on delete cascade,
    student_name text,
    class_name   text,
    grade_title  text,
    academy_id   uuid,
    academy_name text,
    created_at   timestamptz not null default now(),
    unique (class_id, student_id)
);

create index if not exists class_enrollments_class_idx   on public.class_enrollments (class_id);
create index if not exists class_enrollments_student_idx on public.class_enrollments (student_id);

-- ── assignments ─────────────────────────────────────────────────────────────
-- An assignment assigned to a class.
create table if not exists public.assignments (
    id           uuid primary key default gen_random_uuid(),
    class_id     uuid not null references public.academy_classes (id) on delete cascade,
    title        text not null,
    instructions text,
    due_date     timestamptz,
    max_score    int not null default 0,
    version      int not null default 1,
    created_at   timestamptz not null default now(),
    updated_at   timestamptz not null default now()
);

create index if not exists assignments_class_idx on public.assignments (class_id);

-- ── Row-Level Security ──────────────────────────────────────────────────────
alter table public.academy_classes   enable row level security;
alter table public.class_enrollments enable row level security;
alter table public.assignments       enable row level security;

-- Classes: any signed-in user can browse; only the academy owner writes.
drop policy if exists "classes readable" on public.academy_classes;
create policy "classes readable" on public.academy_classes
    for select using (auth.uid() is not null);

drop policy if exists "classes owner write" on public.academy_classes;
create policy "classes owner write" on public.academy_classes
    for all
    using (exists (select 1 from public.academies a where a.id = academy_id and a.owner_id = auth.uid()))
    with check (exists (select 1 from public.academies a where a.id = academy_id and a.owner_id = auth.uid()));

-- Enrolments:
--   a student inserts / reads / deletes their own membership;
--   the owning tutor reads the roster (and may remove a student).
drop policy if exists "enrol student insert" on public.class_enrollments;
create policy "enrol student insert" on public.class_enrollments
    for insert with check (student_id = auth.uid());

drop policy if exists "enrol student read" on public.class_enrollments;
create policy "enrol student read" on public.class_enrollments
    for select using (student_id = auth.uid());

drop policy if exists "enrol student delete" on public.class_enrollments;
create policy "enrol student delete" on public.class_enrollments
    for delete using (student_id = auth.uid());

drop policy if exists "enrol owner read" on public.class_enrollments;
create policy "enrol owner read" on public.class_enrollments
    for select using (exists (
        select 1 from public.academy_classes c
        join public.academies a on a.id = c.academy_id
        where c.id = class_id and a.owner_id = auth.uid()
    ));

drop policy if exists "enrol owner delete" on public.class_enrollments;
create policy "enrol owner delete" on public.class_enrollments
    for delete using (exists (
        select 1 from public.academy_classes c
        join public.academies a on a.id = c.academy_id
        where c.id = class_id and a.owner_id = auth.uid()
    ));

-- Assignments: any signed-in user can read; only the owning tutor writes.
drop policy if exists "assignments readable" on public.assignments;
create policy "assignments readable" on public.assignments
    for select using (auth.uid() is not null);

drop policy if exists "assignments owner write" on public.assignments;
create policy "assignments owner write" on public.assignments
    for all
    using (exists (
        select 1 from public.academy_classes c
        join public.academies a on a.id = c.academy_id
        where c.id = class_id and a.owner_id = auth.uid()
    ))
    with check (exists (
        select 1 from public.academy_classes c
        join public.academies a on a.id = c.academy_id
        where c.id = class_id and a.owner_id = auth.uid()
    ));
