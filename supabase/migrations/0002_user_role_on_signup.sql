-- sReader — seed public.users.role (and email/display_name) from the signup
-- metadata so a new account lands with the right role (student / guardian /
-- tutor / teacher / administrator) chosen on the register screen.
-- Run in the Supabase SQL editor (project wmfaumjseuzhlwwhaudy) or via
-- `supabase db push`. Safe to re-run — it just replaces the trigger function.

create or replace function public.handle_new_user()
returns trigger language plpgsql security definer set search_path = public as $$
begin
    insert into public.users (id, email, display_name, role)
    values (
        new.id,
        new.email,
        coalesce(new.raw_user_meta_data ->> 'display_name', ''),
        -- Only accept a known role from the client; anything else defaults to student.
        case lower(coalesce(new.raw_user_meta_data ->> 'role', 'student'))
            when 'guardian'      then 'guardian'
            when 'tutor'         then 'tutor'
            when 'teacher'       then 'teacher'
            when 'administrator' then 'administrator'
            else 'student'
        end
    )
    on conflict (id) do nothing;
    return new;
end;
$$;

drop trigger if exists on_auth_user_created on auth.users;
create trigger on_auth_user_created
    after insert on auth.users
    for each row execute function public.handle_new_user();
