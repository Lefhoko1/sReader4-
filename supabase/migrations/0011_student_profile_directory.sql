-- sReader — richer, friend-safe student profiles for the Friends feature.
-- Adds public bio/country to the directory and a read-only view of each
-- student's academic life (academies, grade, subjects and tutor/staff) so
-- other signed-in students can see what they have in common before adding them.
-- Run in the Supabase SQL editor (project wmfaumjseuzhlwwhaudy) or via
-- `supabase db push`. Safe to re-run. Requires 0010_friendships.sql first.

-- ── directory_profiles (extended) ───────────────────────────────────────────
-- Append bio + country so a viewed profile can show an "About" section without
-- granting access to the per-row-RLS user_profiles table. New columns are added
-- AFTER the existing ones, which CREATE OR REPLACE VIEW allows.
create or replace view public.directory_profiles as
    select u.id,
           u.username,
           u.display_name,
           u.role,
           p.avatar_url,
           p.bio,
           p.country
    from public.users u
    left join public.user_profiles p on p.user_id = u.id;

revoke all on public.directory_profiles from anon;
grant select on public.directory_profiles to authenticated;

-- ── student_academics ───────────────────────────────────────────────────────
-- One row per (student, enrolled subject): the academy, grade, subject/module
-- and the tutor (staff) who teaches it. Sourced from confirmed enrolments only
-- (status = 'enrolled'), and exposes NO payment or contact data. Like the
-- directory it runs with the owner's rights (security definer, default) so any
-- signed-in user can read another student's academic summary, while the private
-- enrolment/payment columns are simply never selected.
create or replace view public.student_academics as
    select e.student_id,
           e.academy_id,
           e.academy_name,
           e.grade_title,
           e.course_name,
           e.owner_id          as tutor_id,
           t.display_name      as tutor_name
    from public.course_enrollment_requests e
    left join public.users t on t.id = e.owner_id
    where e.status = 'enrolled';

revoke all on public.student_academics from anon;
grant select on public.student_academics to authenticated;
