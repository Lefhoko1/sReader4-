using System;
using SReader.Core.Common;

namespace SReader.Domains.Administration.Models
{
    /// <summary>A "Contact Us" message sent by a user or visitor.</summary>
    public class SupportMessage : Entity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
    }
}
