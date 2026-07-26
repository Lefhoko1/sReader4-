namespace SReader.Domains.Social.Models
{
    /// <summary>
    /// One friendship as the signed-in user sees it: the <em>other</em> person
    /// plus the relationship's id, status and direction. Built by the service
    /// by joining a <see cref="Friendship"/> row to the people directory, so the
    /// UI can show a name + avatar without resolving ids itself.
    /// </summary>
    public sealed class FriendView
    {
        public string FriendshipId { get; set; }
        public PersonSummary Person { get; set; }
        public FriendshipStatus Status { get; set; }

        /// <summary>True when the other person sent the request to me (I can accept it).</summary>
        public bool IsIncoming { get; set; }
    }
}
