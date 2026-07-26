-- sReader — grades an academy offers (PSLE / JC / BGCSE / university programs)
-- and the courses under university grades. Primary & secondary grades are
-- certificate-based and carry no courses.
-- Run in the Supabase SQL editor (project wmfaumjseuzhlwwhaudy). Safe to re-run.

-- ── academy_grades ─────────────────────────────────────────────────────────
create table if not exists public.academy_grades (
    id         uuid primary key default gen_random_uuid(),
    academy_id uuid not null references public.academies (id) on delete cascade,
    stage      text not null,            -- primary | secondary | university
    title      text not null,            -- e.g. PSLE, JC, BGCSE, "BSc Computer Science"
    created_at timestamptz not null default now()
);

create index if not exists academy_grades_academy_idx on public.academy_grades (academy_id);

-- ── academy_courses (only under university grades) ─────────────────────────
create table if not exists public.academy_courses (
    id          uuid primary key default gen_random_uuid(),
    grade_id    uuid not null references public.academy_grades (id) on delete cascade,
    name        text not null,
    description text,
    created_at  timestamptz not null default now()
);

create index if not exists academy_courses_grade_idx on public.academy_courses (grade_id);

-- ── Row-Level Security ─────────────────────────────────────────────────────
alter table public.academy_grades  enable row level security;
alter table public.academy_courses enable row level security;

-- Grades: anyone signed in can see what an academy offers; only the academy's
-- owner can add/remove.
drop policy if exists "grades readable" on public.academy_grades;
create policy "grades readable" on public.academy_grades
    for select using (auth.uid() is not null);

drop policy if exists "grades owner insert" on public.academy_grades;
create policy "grades owner insert" on public.academy_grades
    for insert with check (exists (
        select 1 from public.academies a
        where a.id = academy_id and a.owner_id = auth.uid()
    ));

drop policy if exists "grades owner delete" on public.academy_grades;
create policy "grades owner delete" on public.academy_grades
    for delete using (exists (
        select 1 from public.academies a
        where a.id = academy_id and a.owner_id = auth.uid()
    ));

-- Courses: readable by any signed-in user; writable by the owner of the
-- academy that owns the parent grade.
drop policy if exists "courses readable" on public.academy_courses;
create policy "courses readable" on public.academy_courses
    for select using (auth.uid() is not null);

drop policy if exists "courses owner insert" on public.academy_courses;
create policy "courses owner insert" on public.academy_courses
    for insert with check (exists (
        select 1 from public.academy_grades g
        join public.academies a on a.id = g.academy_id
        where g.id = grade_id and a.owner_id = auth.uid()
    ));

drop policy if exists "courses owner delete" on public.academy_courses;
create policy "courses owner delete" on public.academy_courses
    for delete using (exists (
        select 1 from public.academy_grades g
        join public.academies a on a.id = g.academy_id
        where g.id = grade_id and a.owner_id = auth.uid()
    ));
