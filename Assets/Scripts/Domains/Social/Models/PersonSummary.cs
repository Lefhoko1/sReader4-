using SReader.Domains.Identity.Models;

namespace SReader.Domains.Social.Models
{
    /// <summary>
    /// The public, friend-safe view of another user: just enough to show them
    /// in a directory, request card or profile screen. Sourced from the
    /// <c>directory_profiles</c> view (users joined to their latest avatar), so
    /// it never carries private fields like email.
    /// </summary>
    public sealed class PersonSummary
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public UserRole Role { get; set; } = UserRole.Student;

        /// <summary>Latest uploaded profile picture (avatar_url), may be null.</summary>
        public string AvatarUrl { get; set; }

        /// <summary>Public "About" fields, surfaced on the profile screen.</summary>
        public string Bio { get; set; }
        public string Country { get; set; }

        /// <summary>Best human label: display name, else @username, else a short id.</summary>
        public string Label
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(DisplayName)) return DisplayName;
                if (!string.IsNullOrWhiteSpace(Username)) return "@" + Username;
                return string.IsNullOrEmpty(UserId) ? "Student" : "Student " + UserId.Substring(0, System.Math.Min(6, UserId.Length));
            }
        }

        public string RoleLabel => Role.ToString();
    }
}
