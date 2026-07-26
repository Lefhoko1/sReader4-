using System;
using SReader.Core.Common;

namespace SReader.Domains.Identity.Models
{
    public enum UserRole
    {
        Student,
        Guardian,
        Tutor,
        Teacher,
        Administrator
    }

    public enum UserStatus
    {
        PendingVerification,
        Active,
        Suspended,
        Deactivated
    }

    public class User : Entity
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
