using System;
using SReader.Core.Common;

namespace SReader.Domains.Education.Models
{
    public enum EnrollmentStatus
    {
        Requested,         // student asked; awaiting their payment
        PaymentSubmitted,  // student paid and submitted proof; awaiting tutor
        Enrolled,          // tutor confirmed payment and enrolled the student
        Rejected           // tutor declined
    }

    /// <summary>
    /// A paid request by a student to be tutored in a specific subject/module.
    /// Flow: Requested → (student pays + submits proof) → PaymentSubmitted →
    /// (tutor confirms) → Enrolled. OwnerId is the academy's tutor, denormalised
    /// so they can list requests across their academies.
    /// </summary>
    public class CourseEnrollmentRequest : Entity
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string GradeTitle { get; set; }
        public string AcademyId { get; set; }
        public string AcademyName { get; set; }
        public string OwnerId { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public EnrollmentStatus Status { get; set; }

        /// <summary>Free-trial requests skip payment — the tutor can enroll directly.</summary>
        public bool IsFreeTrial { get; set; }

        /// <summary>Student's payment confirmation: reference / txn id / note.</summary>
        public string PaymentReference { get; set; }

        /// <summary>URL of the uploaded proof-of-payment image (Supabase Storage).</summary>
        public string PaymentProofUrl { get; set; }

        public bool HasProof => !string.IsNullOrWhiteSpace(PaymentProofUrl);

        public DateTime CreatedAt { get; set; }
    }
}
