namespace SReader.Domains.Education.Models
{
    /// <summary>
    /// The schooling stage a grade/program belongs to. Primary and Secondary
    /// are certificate-based — the certificate itself is the "course", so no
    /// courses are registered under them. University programs register courses
    /// (modules).
    /// </summary>
    public enum EducationStage
    {
        Primary,     // PSLE — certificate is the course
        Secondary,   // JC, BGCSE — certificate is the course
        University   // programs with registered courses/modules
    }
}
