namespace SReader.Domains.Social.Models
{
    /// <summary>How the signed-in user currently relates to a person in Discover.</summary>
    public enum RelationshipState
    {
        None,            // no friendship row — can send a request
        RequestSent,     // I sent a pending request — can cancel
        RequestReceived, // they sent me a pending request — can accept / reject
        Friends,         // accepted — can remove
        Blocked
    }

    /// <summary>
    /// A discoverable person plus my relationship to them, so the Discover list
    /// can render the right action (Add friend / Pending / Accept / Friends ✓).
    /// </summary>
    public sealed class DiscoverPerson
    {
        public PersonSummary Person { get; set; }
        public RelationshipState State { get; set; }

        /// <summary>The friendship row id when a relationship already exists (else null).</summary>
        public string FriendshipId { get; set; }
    }
}
