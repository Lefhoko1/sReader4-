-- sReader — the assignment workflow: rich content, student attempts &
-- submissions, and shareable schedules.
-- Run in the Supabase SQL editor (project wmfaumjseuzhlwwhaudy) or via
-- `supabase db push`. Safe to re-run. Requires 0012_classes_and_assignments.sql.

-- ── assignments: content payload ────────────────────────────────────────────
-- Held as TEXT (a JSON string) so the Unity JsonUtility DTO can carry it; the
-- structured content schema arrives later and is written via a dedicated call.
alter table public.assignments add column if not exists content_json text;

-- ── assignment_attempts ─────────────────────────────────────────────────────
-- One row per (assignment, student): tracks attempt status through the flow
-- notstarted → inprogress → completed → submitted → graded.
create table if not exists public.assignment_attempts (
    id            uuid primary key default gen_random_uuid(),
    assignment_id uuid not null references public.assignments (id) on delete cascade,
    student_id    uuid not null references auth.users (id) on delete cascade,
    status        text not null default 'inprogress',
    started_at    timestamptz,
    completed_at  timestamptz,
    updated_at    timestamptz not null default now(),
    unique (assignment_id, student_id)
);

create index if not exists assignment_attempts_assignment_idx on public.assignment_attempts (assignment_id);
create index if not exists assignment_attempts_student_idx    on public.assignment_attempts (student_id);

-- ── assignment_submissions ──────────────────────────────────────────────────
create table if not exists public.assignment_submissions (
    id            uuid primary key default gen_random_uuid(),
    attempt_id    uuid references public.assignment_attempts (id) on delete cascade,
    assignment_id uuid not null references public.assignments (id) on delete cascade,
    student_id    uuid not null references auth.users (id) on delete cascade,
    submitted_at  timestamptz not null default now(),
    file_path     text,
    grade         int,
    feedback      text
);

create index if not exists assignment_submissions_assignment_idx on public.assignment_submissions (assignment_id);
create index if not exists assignment_submissions_student_idx    on public.assignment_submissions (student_id);

-- ── assignment_schedules ────────────────────────────────────────────────────
-- A student's plan to do an assignment at a time. Visible to classmates and
-- friends (unless hidden) so they can plan together.
create table if not exists public.assignment_schedules (
    id               uuid primary key default gen_random_uuid(),
    assignment_id    uuid not null references public.assignments (id) on delete cascade,
    student_id       uuid not null references auth.users (id) on delete cascade,
    student_name     text,
    class_id         uuid,
    class_name       text,
    assignment_title text,
    scheduled_for    timestamptz,
    hidden           boolean not null default false,
    note             text,
    created_at       timestamptz not null default now(),
    updated_at       timestamptz not null default now(),
    unique (assignment_id, student_id)
);

create index if not exists assignment_schedules_assignment_idx on public.assignment_schedules (assignment_id);
create index if not exists assignment_schedules_class_idx      on public.assignment_schedules (class_id);
create index if not exists assignment_schedules_student_idx    on public.assignment_schedules (student_id);

-- ── Row-Level Security ──────────────────────────────────────────────────────
alter table public.assignment_attempts    enable row level security;
alter table public.assignment_submissions enable row level security;
alter table public.assignment_schedules   enable row level security;

-- Attempts: a student owns their own; the owning tutor may read them.
drop policy if exists "attempt student all" on public.assignment_attempts;
create policy "attempt student all" on public.assignment_attempts
    for all using (student_id = auth.uid()) with check (student_id = auth.uid());

drop policy if exists "attempt tutor read" on public.assignment_attempts;
create policy "attempt tutor read" on public.assignment_attempts
    for select using (exists (
        select 1 from public.assignments asg
        join public.academy_classes c on c.id = asg.class_id
        join public.academies a on a.id = c.academy_id
        where asg.id = assignment_id and a.owner_id = auth.uid()
    ));

-- Submissions: a student inserts/reads their own; the owning tutor reads and grades.
drop policy if exists "submission student insert" on public.assignment_submissions;
create policy "submission student insert" on public.assignment_submissions
    for insert with check (student_id = auth.uid());

drop policy if exists "submission student read" on public.assignment_submissions;
create policy "submission student read" on public.assignment_submissions
    for select using (student_id = auth.uid());

drop policy if exists "submission tutor read" on public.assignment_submissions;
create policy "submission tutor read" on public.assignment_submissions
    for select using (exists (
        select 1 from public.assignments asg
        join public.academy_classes c on c.id = asg.class_id
        join public.academies a on a.id = c.academy_id
        where asg.id = assignment_id and a.owner_id = auth.uid()
    ));

drop policy if exists "submission tutor grade" on public.assignment_submissions;
create policy "submission tutor grade" on public.assignment_submissions
    for update using (exists (
        select 1 from public.assignments asg
        join public.academy_classes c on c.id = asg.class_id
        join public.academies a on a.id = c.academy_id
        where asg.id = assignment_id and a.owner_id = auth.uid()
    ));

-- Schedules: owner writes/reads their own; classmates (same class) and friends
-- may read a schedule that isn't hidden.
drop policy if exists "schedule owner all" on public.assignment_schedules;
create policy "schedule owner all" on public.assignment_schedules
    for all using (student_id = auth.uid()) with check (student_id = auth.uid());

drop policy if exists "schedule visible read" on public.assignment_schedules;
create policy "schedule visible read" on public.assignment_schedules
    for select using (
        not hidden and (
            -- a classmate: the viewer is enrolled in the same class
            exists (
                select 1 from public.class_enrollments e
                where e.class_id = assignment_schedules.class_id and e.student_id = auth.uid()
            )
            -- or an accepted friend of the schedule's owner
            or exists (
                select 1 from public.friendships f
                where f.status = 'accepted'
                  and ((f.requester_id = assignment_schedules.student_id and f.recipient_id = auth.uid())
                    or (f.recipient_id = assignment_schedules.student_id and f.requester_id = auth.uid()))
            )
        )
    );
