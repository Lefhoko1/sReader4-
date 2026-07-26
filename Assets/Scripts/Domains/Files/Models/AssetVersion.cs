using System;
using SReader.Core.Common;

namespace SReader.Domains.Files.Models
{
    public enum AssetType
    {
        ProfileImage,
        Assignment,
        Lesson,
        Avatar
    }

    /// <summary>
    /// Version record for every downloadable asset — the heart of the
    /// offline-first design: compare remote Version/Checksum with the
    /// local copy to know whether a re-download is needed.
    /// </summary>
    public class AssetVersion : Entity
    {
        public AssetType AssetType { get; set; }

        /// <summary>Id of the owning entity (user id for ProfileImage, assignment id, ...).</summary>
        public string AssetId { get; set; }

        public int Version { get; set; }
        public string Checksum { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
