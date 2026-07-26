namespace SReader.Domains.Multiplayer.Models
{
    /// <summary>Persistent game profile — stored in Supabase. Live state never is.</summary>
    public class PlayerProfile
    {
        public string UserId { get; set; }

        public int Level { get; set; }
        public int Experience { get; set; }

        public int AvatarVersion { get; set; }
    }
}
