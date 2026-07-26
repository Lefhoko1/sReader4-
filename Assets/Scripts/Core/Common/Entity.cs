namespace SReader.Core.Common
{
    /// <summary>
    /// Base for all domain entities. Ids are strings because Supabase
    /// uses UUIDs; the domain never depends on a database id type.
    /// </summary>
    public abstract class Entity
    {
        public string Id { get; set; }
    }
}
