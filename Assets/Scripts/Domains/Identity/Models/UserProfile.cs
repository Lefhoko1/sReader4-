using SReader.Core.Common;

namespace SReader.Domains.Identity.Models
{
    public class UserProfile : Entity
    {
        public string UserId { get; set; }
        public string Bio { get; set; }
        public string Country { get; set; }
        public string Timezone { get; set; }

        /// <summary>References an AssetVersion in the Files domain (versioned download).</summary>
        public string ProfileImageId { get; set; }

        /// <summary>Public URL of the user's uploaded profile picture (Supabase Storage).</summary>
        public string ProfileImageUrl { get; set; }
    }
}
