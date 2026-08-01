// ===========================================================================
//  RiversideWalkDemo — the conductor for the walking prototype
// ===========================================================================
//  Press Play and this puts one sentence on the river, lays the road beside it,
//  and sends the reader off: stop at every word, climb the island, sit on the
//  mat. Nothing else in the scene has to cooperate.
//
//  It exists because three different things can lay out that river — the
//  StoryBook (words flung onto stones), WordPathGame (real assignment content
//  from the board) and the builder's own inspector sentence. For a prototype
//  about CAMERA and MOVEMENT, one deterministic sentence is what you want; the
//  setup menu switches the other two off and this drives the builder directly.
//
//  When the walk graduates from prototype to game, delete this and let
//  WordPathGame call road.Rebuild() + walker.Begin() after it chooses a
//  sentence. PathWalker's events (onArrive / onLeave / onSeated) are already
//  the seam for that.
// ===========================================================================
using System.Collections;
using UnityEngine;

public class RiversideWalkDemo : MonoBehaviour
{
    [Header("Wiring")]
    public WordPathBuilder path;
    public RiversideRoad road;
    public PathWalker walker;

    [Header("Content")]
    [Tooltip("Lay this sentence out on Play. Empty = whatever the WordPathBuilder " +
             "already has in its own Sentence field.")]
    [TextArea] public string sentence = "";
    [Tooltip("Rebuild the stones on Play. Off = walk whatever is already there.")]
    public bool buildSentenceOnPlay = true;

    [Header("Log")]
    [Tooltip("Print each stop to the Console — handy while judging the pacing.")]
    public bool logStops = true;

    void Awake()
    {
        // The demo owns the start, so the walker must not also start itself.
        if (walker != null) walker.autoPlay = false;
    }

    IEnumerator Start()
    {
        if (path == null) path = FindAnyObjectByType<WordPathBuilder>();
        if (road == null) road = FindAnyObjectByType<RiversideRoad>();
        if (walker == null) walker = FindAnyObjectByType<PathWalker>();
        if (path == null || road == null || walker == null)
        {
            Debug.LogError("[Walk] Demo needs a WordPathBuilder, a RiversideRoad " +
                           "and a PathWalker. Run Tools ▸ Great Library ▸ Island ▸ " +
                           "6. Build Riverside Walk.");
            yield break;
        }

        // one frame, so anything else that clears or rebuilds the river on Start
        // has already had its turn
        yield return null;

        if (buildSentenceOnPlay)
        {
            if (!string.IsNullOrWhiteSpace(sentence)) path.sentence = sentence;
            path.Build();
        }
        road.Rebuild();

        if (logStops)
        {
            walker.onArrive.AddListener(i =>
            {
                var s = road.Stone(i);
                var w = s != null ? s.GetComponent<WordStone>() : null;
                Debug.Log($"[Walk] stop {i + 1}/{road.StopCount} — " +
                          $"\"{(w != null ? w.word : "?")}\" at " +
                          $"{road.StopDistance(i):0.0} m");
            });
            walker.onSeated.AddListener(() =>
                Debug.Log($"[Walk] seated on the mat after {road.Length:0.0} m."));
        }

        walker.Begin();
    }
}
