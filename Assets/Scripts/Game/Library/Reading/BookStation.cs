// ===========================================================================
//  BookStation — the lectern: where the book stands, and where the reader does
// ===========================================================================
//  A nook at the side of the world. The station object itself sits on the
//  GROUND (the reader's feet go at its height); the pedestal and the book hang
//  off it as children:
//
//      BookNook            <- BookStation, on the ground, at the side
//        Pedestal          <- the plinth
//        StoryBook         <- the book, tilted like a lectern
//          PageFocus       <- what the camera aims at
//
//  It answers two questions and nothing else:
//      StandPosition — where the reader stands to read
//      LookAt        — what they turn to face when they get there
//
//  The reading spot is DERIVED from the book rather than authored, so dragging
//  the book anywhere (or spinning it) moves the reader with it and the shot
//  still composes. Drop a child called "ReadSpot" in if you want to override it.
//
//  MVVM note: a marker and some geometry. It holds no state and decides nothing.
// ===========================================================================
using UnityEngine;

[ExecuteAlways]
public class BookStation : MonoBehaviour
{
    [Header("Wiring")]
    [Tooltip("The book object. Empty = the first StoryBook under this station, " +
             "or the station itself.")]
    public Transform book;
    [Tooltip("Optional explicit reading spot. Empty = derived from the book (in " +
             "front of the foot of the page), which is what you usually want.")]
    public Transform stand;
    [Tooltip("Optional aim point for the camera — the middle of the open spread. " +
             "Empty = the book's own origin.")]
    public Transform focusTarget;

    [Header("The room around it")]
    [Tooltip("The doorway the reader comes in through — SOCKET_Entrance on the " +
             "Blender library. With this set the reader starts OUTSIDE and walks " +
             "in, which is the only way the player ever sees the room.")]
    public Transform doorway;
    [Tooltip("The middle of the hall — SOCKET_HallCentre. What the camera frames " +
             "during the walk in, before it settles on the page. Empty falls back " +
             "to the book, which means no reveal at all.")]
    public Transform hallFocus;

    /// <summary>What the establishing shot frames. The hall if we have it.</summary>
    public Transform HallFocus => hallFocus != null ? hallFocus : FocusTarget;

    [Header("Where it stands")]
    [Tooltip("The nook is inside a built room. The dressing pass then skips the " +
             "things the room already provides — its own paved terrace, its " +
             "backdrop shelves and columns — so they do not sit inside the hall's " +
             "floor and shelving.")]
    public bool indoors;

    [Header("The reading spot")]
    [Tooltip("How far in front of the book the reader stands, in metres. Measured " +
             "from the book's pivot along the foot of the page.")]
    public float standDistance = 0.95f;
    [Tooltip("Offset the reader sideways, so they don't stand dead-centre in front " +
             "of the pages and hide them from the camera.")]
    public float standSide = 0.35f;

    /// <summary>Where the reader's feet go to read. Always at the station's height.</summary>
    public Vector3 StandPosition
    {
        get
        {
            if (stand != null) return stand.position;

            Transform b = Book;
            // "Up the page" is the book's forward (see StoryBook.TextRotation), so
            // the foot of the page — where a reader would stand — is behind it.
            Vector3 dir = Flat(-b.forward);
            if (dir.sqrMagnitude < 1e-4f) dir = Vector3.back;
            Vector3 side = Vector3.Cross(Vector3.up, dir).normalized;

            Vector3 p = b.position + dir * standDistance + side * standSide;
            p.y = transform.position.y;
            return p;
        }
    }

    /// <summary>What the reader turns to face once they arrive.</summary>
    public Vector3 LookAt => Book.position;

    /// <summary>What the camera frames while the reader is here.</summary>
    public Transform FocusTarget => focusTarget != null ? focusTarget : Book;

    /// <summary>The book, resolved: the assigned one, one below us, or us.</summary>
    public Transform Book
    {
        get
        {
            if (book != null) return book;
            var sb = GetComponentInChildren<StoryBook>(true);
            return sb != null ? sb.transform : transform;
        }
    }

    static Vector3 Flat(Vector3 v) { v.y = 0f; return v; }

    void OnDrawGizmos()
    {
        Vector3 s = StandPosition;
        Gizmos.color = new Color(1f, 0.82f, 0.35f, 0.9f);
        Gizmos.DrawWireSphere(s, 0.35f);
        Gizmos.DrawLine(s, s + Vector3.up * 1.75f);        // the reader, roughly
        Gizmos.color = new Color(0.6f, 0.45f, 1f, 0.9f);
        Gizmos.DrawLine(s + Vector3.up * 1.4f, LookAt);    // the line of sight
    }
}
