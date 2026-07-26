using System;
using SReader.Core.Common;

namespace SReader.Domains.Sync.Models
{
    public enum SyncOperationType
    {
        Upload,
        Download,
        Delete
    }

    public enum SyncStatus
    {
        Pending,
        InProgress,
        Completed,
        Failed
    }

    /// <summary>One queued offline change waiting to reach the cloud.</summary>
    public class SyncOperation : Entity
    {
        public SyncOperationType OperationType { get; set; }

        /// <summary>Domain entity name, e.g. "Assignment", "UserProfile".</summary>
        public string EntityType { get; set; }
        public string EntityId { get; set; }

        public DateTime CreatedAt { get; set; }
        public SyncStatus Status { get; set; }
        public int RetryCount { get; set; }
    }
}
