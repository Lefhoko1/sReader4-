using System;

namespace SReader.Core.Common
{
    /// <summary>
    /// Time abstraction so domain logic (due dates, schedules, sync
    /// timestamps) is testable without depending on the machine clock.
    /// </summary>
    public interface IClock
    {
        DateTime UtcNow { get; }
    }

    public sealed class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
