using System;
using SReader.Core.Common;

namespace SReader.Domains.Scheduling.Models
{
    public enum ScheduleType
    {
        Assignment,
        Class,
        Lesson,
        Meeting,
        Reminder,
        Event
    }

    public class Schedule : Entity
    {
        public string UserId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public ScheduleType ScheduleType { get; set; }
    }
}
