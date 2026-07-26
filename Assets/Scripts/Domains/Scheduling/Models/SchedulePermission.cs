namespace SReader.Domains.Scheduling.Models
{
    /// <summary>Per-user visibility grant on someone else's schedule entry.</summary>
    public class SchedulePermission
    {
        public string ScheduleId { get; set; }
        public string UserId { get; set; }
        public bool CanView { get; set; }
    }
}
