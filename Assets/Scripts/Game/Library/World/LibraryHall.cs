// ===========================================================================
//  LibraryHall — the parameters of a room built from the modular kit
// ===========================================================================
//  This holds NOTHING but the dials. The assembly lives in the editor script
//  (LibraryHallBuilder), because a room is authored once and then baked into
//  the scene — there is no reason to carry a level generator into the build.
//
//  WHY A COMPONENT AND NOT JUST A MENU ITEM. A hall has a dozen judgement calls
//  in it — how many bays, where the windows fall, which way the wall panels
//  face. Menu items can only ship one opinion. With the numbers on an object you
//  can turn them and press Rebuild until the room looks right, and the scene
//  remembers what it was built from.
//
//  EVERYTHING IS MEASURED, NOTHING IS ASSUMED. The builder reads the wall
//  panel's own width and height and uses that as the module, so the whole room
//  is on one grid by construction and the pieces meet cleanly. Re-export the kit
//  at a different scale and the room still fits itself.
//
//  Bible Ch. 5.3 — the 18-piece modular kit is what unlocks level building.
// ===========================================================================
using UnityEngine;

public class LibraryHall : MonoBehaviour
{
    [Header("Shape (in BAYS — one bay = one wall panel's width)")]
    [Tooltip("Bays across the front wall.")]
    [Range(2, 12)] public int baysWide = 5;
    [Tooltip("Bays from the door to the back wall.")]
    [Range(2, 16)] public int baysDeep = 7;

    [Header("Walls")]
    [Tooltip("Every Nth side-wall panel is a window instead of a solid wall. " +
             "2 = every other bay.")]
    [Range(1, 5)] public int windowEvery = 2;
    [Tooltip("Put the arched doorway in the middle of the front wall.")]
    public bool doorway = true;
    [Tooltip("Spin every wall panel 180°. Wall meshes are usually one-sided — if " +
             "you are looking at the OUTSIDE of the room from within, tick this.")]
    public bool flipWalls;
    [Tooltip("Extra yaw on every wall panel, in degrees. The builder already " +
             "detects whether a panel's width runs along X or Z; this is for when " +
             "the kit disagrees with it anyway.")]
    public float wallYawOffset;

    [Tooltip("Extra yaw on the whole building when it is stood on the island, in " +
             "degrees. The builder aims the doorway at the water the reader arrives " +
             "across, but which way an island's entrance actually faces is authored " +
             "art — this is the override, and it survives a rebuild. 'Turn Hall 90°' " +
             "just adds to it.")]
    public float doorYawOffset;

    [Header("Structure")]
    public bool columns = true;
    public bool beams = true;
    [Tooltip("Line the side and back walls with shelf bays — the thing that makes " +
             "a room read as a LIBRARY rather than a hall.")]
    public bool shelves = true;
    [Tooltip("Gap between a shelf bay and the wall behind it, in metres.")]
    public float shelfInset = 0.12f;

    [Header("Dressing")]
    public bool librarianDesk = true;
    public bool lanterns = true;
    [Tooltip("Move the book nook (desk, chair, book) inside, near the back wall " +
             "facing the door, and re-compose the reading shot around it.")]
    public bool nookInside = true;

    [Header("Kit prefabs (auto-filled from Assets/Prefabs/Kit)")]
    public GameObject floorA, floorB;
    public GameObject wallFull, wallWindow, wallRecess;
    public GameObject column, arch, beam, shelfBay;
    public GameObject deskLibrarian, lanternHanging, candles;

    [Header("Built (read-only — what the last Rebuild measured)")]
    [Tooltip("One bay, in metres: the wall panel's own width.")]
    public float bayWidth;
    [Tooltip("Wall height, in metres.")]
    public float wallHeight;
    [Tooltip("Interior floor size, in metres.")]
    public Vector2 interior;

    /// <summary>Interior width in metres (bays across x bay width).</summary>
    public float Width => baysWide * bayWidth;

    /// <summary>Interior depth in metres.</summary>
    public float Depth => baysDeep * bayWidth;
}
