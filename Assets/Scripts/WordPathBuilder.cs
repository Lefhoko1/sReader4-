// ===========================================================================
//  WordPathBuilder — lays a sentence across stones along a curve.
//  (Own file: Unity needs one MonoBehaviour per file, named after the class.)
// ===========================================================================
//  ONE STONE PER WORD. The river is rebuilt for whichever sentence is chosen,
//  so the number of stones is always the number of words in THAT sentence —
//  never a fixed row left over from a previous one.
//
//  KEY WORDS ARE FLEXIBLE. A sentence may carry none, one, two or many; every
//  word flagged as a key word gets the gold Key stone. Nothing here assumes a
//  single keyword.
// ===========================================================================
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SReader.Domains.Assignments.Models;

public class WordPathBuilder : MonoBehaviour
{
    [Header("Content")]
    [TextArea] public string sentence = "The fox lived near a quiet river";

    [Tooltip("Key words for the sentence above — as many as you like (one, two, more). " +
             "Case and punctuation are ignored.")]
    public List<string> keywords = new List<string> { "river" };

    // Older scenes serialised a single `keyword`. Still honoured — folded into
    // `keywords` by AllKeywords() — so existing scene data keeps working.
    [HideInInspector, SerializeField] string keyword;

    [Header("Stone prefabs (A/B/C + Key)")]
    public GameObject stoneA, stoneB, stoneC, stoneKey;

    [Header("Pool (make the stones once, reuse them for every sentence)")]
    [Tooltip("Off = the old behaviour: destroy and re-instantiate the whole river " +
             "per sentence. On = a fixed set of slots that sink and rise instead.")]
    public bool usePool = true;
    [Tooltip("How many stones the pool holds — the longest sentence you will ever " +
             "show. Sentences shorter than this leave the spare slots under water.")]
    [Range(4, 64)] public int poolSlots = 24;

    [Header("River shape (drawn in Blender)")]
    [Tooltip("The authored curve the words follow. Empty = the straight " +
             "PathStart -> PathEnd line with the sine meander, as before.")]
    public WordRiverPath river;

    [Header("Path")]
    public Transform startPoint, endPoint;
    [Tooltip("How far the river wanders off its centre line, in world units. Kept small — " +
             "the stones must read as ONE winding path, never two banks.")]
    public float curve = 0.8f;
    [Tooltip("Full side-to-side waves over the whole river. Under 1 = a single lazy S.")]
    [Range(0.25f, 2f)] public float meanderWaves = 0.75f;
    [Tooltip("The most a stone may sit sideways of its neighbour, as a fraction of a stone's " +
             "width. Small values keep neighbours on one line instead of abreast.")]
    [Range(0.05f, 0.6f)] public float maxNeighbourStep = 0.3f;
    [Tooltip("Slide every word stone sideways off the line it was authored on, in " +
             "metres, so the reader can walk the line itself instead of over the " +
             "words. Follows the bend: on a Blender curve the offset is taken " +
             "across the river AT EACH STONE, not across one average direction. " +
             "Negative puts them on the other side.")]
    public float stoneSideOffset = 0f;
    public float stoneScale = 1f;

    [Header("Perspective (keeps the far stones readable)")]
    [Tooltip("The path runs away from the camera, so plain even spacing makes the near " +
             "stones giant and the far ones specks. This counteracts it: 0 = raw " +
             "perspective, 1 = every stone the same size on screen.")]
    [Range(0f, 1f)] public float perspectiveCompensation = 0.8f;
    [Tooltip("Limits on how far a stone may shrink / grow away from Stone Scale.")]
    public float minSizeFactor = 0.4f;
    public float maxSizeFactor = 2.5f;
    [Tooltip("Clear space to keep between neighbouring stones, as a fraction of a " +
             "stone's width. The stones shrink, then step sideways, until they fit.")]
    [Range(0f, 1.5f)] public float stoneGap = 0.3f;
    [Tooltip("Smallest the whole set may be shrunk to make a long sentence fit.")]
    [Range(0.15f, 1f)] public float minFitFactor = 0.3f;
    [Tooltip("Hard cap on how wide the meander may swing, as a fraction of screen width.")]
    [Range(0.1f, 0.9f)] public float maxLateralFraction = 0.45f;
    [Tooltip("Camera the river is composed for. Empty = Camera.main.")]
    public Camera viewCamera;

    [Header("Framing (how big and how near the river reads on screen)")]
    [Tooltip("Compose the river into a band of the SCREEN rather than trusting where " +
             "PathStart / PathEnd happen to sit. The line through those two points still " +
             "sets the river's direction — this only slides the ends along it.")]
    public bool frameOnScreen = true;
    [Tooltip("Where the FIRST stone sits, up the screen. 0 = bottom, 1 = top. Lower = " +
             "nearer the camera and bigger.")]
    [Range(0f, 1f)] public float nearScreenY = 0.30f;
    [Tooltip("Where the LAST stone sits, up the screen. Keep it below the island's shore.")]
    [Range(0f, 1f)] public float farScreenY = 0.60f;
    [Tooltip("How wide a stone should read on screen, as a fraction of screen width. " +
             "This is what makes a far stone as legible as a near one. 0 = off, use " +
             "Stone Scale and raw perspective instead.")]
    [Range(0f, 0.6f)] public float stoneScreenWidth = 0.2f;
    [Tooltip("How far the ends may slide back toward the camera, as a multiple of the " +
             "PathStart→PathEnd length. 0 = never nearer than PathStart.")]
    [Range(0f, 2f)] public float nearReach = 1.2f;
    [Tooltip("How far PAST PathEnd the far end may reach, as a multiple of the path " +
             "length. PathEnd lands mid-screen, so a little overshoot claims the open " +
             "water below the shore. Drop to 1 if stones start landing on the island.")]
    [Range(1f, 2f)] public float farReach = 1.3f;

    [Header("Clearance (keep the first stone out from behind the book)")]
    [Tooltip("Always skip this much of the near end of the path before the first stone.")]
    [Range(0f, 0.6f)] public float startInset = 0.04f;
    [Tooltip("Object the first stone must not hide behind — the storybook. The near end " +
             "of the path slides along until stone 1 is clear of it on screen. " +
             "StoryBook wires itself in here automatically.")]
    public Transform avoid;
    [Tooltip("Extra clearance around the avoided object, as a fraction of screen height. " +
             "Keep it small — every bit of it is dead water between the book and stone one.")]
    [Range(0f, 0.3f)] public float avoidMargin = 0.015f;

    /// <summary>The stones currently in the river, in reading order.</summary>
    public IReadOnlyList<WordStone> Stones => _stones;
    readonly List<WordStone> _stones = new List<WordStone>();
    readonly List<float> _u = new List<float>();      // each stone's place along the path, 0-1
    readonly WordStonePool _pool = new WordStonePool();
    Vector3 _lastEye = new Vector3(float.MaxValue, 0f, 0f);
    Transform _avoidCached; Renderer[] _avoidRends;   // the book's renderers, looked up once
    float _unitDiameter;                              // a stone's width at scale 1

    /// <summary>One word's worth of path: what it says, whether it is a key word.</summary>
    struct Slot { public string text; public bool isKey; public ContentToken token; }

    [ContextMenu("Build Path")]
    public void Build() => BuildSentence(sentence, AllKeywords());

    /// <summary>
    /// Take the river down. Pooled, that sinks the stones and keeps them; unpooled
    /// it destroys them, as before. Callers must use THIS rather than deleting the
    /// builder's children themselves — doing that destroys the pool.
    /// </summary>
    public void Clear() => ClearStones();

    /// <summary>
    /// Lay out a plain sentence: one stone per word, and a gold Key stone for every
    /// word in <paramref name="keyWords"/> (there may be any number of them).
    /// </summary>
    public List<WordStone> BuildSentence(string text, IEnumerable<string> keyWords,
                                         bool showWords = true)
    {
        var keys = new HashSet<string>(
            (keyWords ?? Enumerable.Empty<string>()).Select(Normalise));
        keys.Remove("");

        var slots = SplitWords(text)
            .Select(w => new Slot { text = w.TrimEnd('.'), isKey = keys.Contains(Normalise(w)) })
            .Where(s => s.text.Length > 0)
            .ToList();
        return BuildSlots(slots, showWords);
    }

    /// <summary>
    /// Lay out an explicit word list with its own key-word flags — used when the
    /// caller has already decided which words are key (e.g. StoryBook's inline
    /// markers). <paramref name="isKeyword"/> is parallel to <paramref name="words"/>.
    /// </summary>
    public List<WordStone> BuildWords(IList<string> words, IList<bool> isKeyword,
                                      bool showWords = true)
    {
        var slots = new List<Slot>();
        for (int i = 0; words != null && i < words.Count; i++)
        {
            var w = (words[i] ?? "").Trim();
            if (w.Length == 0) continue;
            slots.Add(new Slot
            {
                text = w,
                isKey = isKeyword != null && i < isKeyword.Count && isKeyword[i]
            });
        }
        return BuildSlots(slots, showWords);
    }

    /// <summary>
    /// Build from REAL assignment content: one stone per word, and EVERY word with
    /// an activity (Define / Fill / Illustrate) becomes a Key stone carrying its
    /// <see cref="ContentToken"/> — one sentence may have several. Used by
    /// WordPathGame (the Supabase path).
    /// </summary>
    public List<WordStone> BuildFromSentence(ContentSentence sentence)
    {
        if (sentence == null)
        {
            Debug.LogWarning("[WordPath] Need a sentence.");
            ClearStones();
            return new List<WordStone>();
        }

        var slots = sentence.tokens
            .Where(t => t.isWord)
            .Select(t => new Slot { text = t.text, isKey = t.IsActivity,
                                    token = t.IsActivity ? t : null })
            .ToList();
        return BuildSlots(slots, true);
    }

    // ── the one place stones are actually made ──────────────────────────────

    List<WordStone> BuildSlots(List<Slot> slots, bool showWords)
    {
        ClearStones();
        if (startPoint == null || endPoint == null)
        { Debug.LogWarning("[WordPath] Assign start & end points."); return new List<WordStone>(); }
        if (slots == null || slots.Count == 0) return new List<WordStone>();

        var variants = new[] { stoneA, stoneB, stoneC }.Where(p => p != null).ToArray();

        if (usePool) BuildPooled(slots, showWords, variants);
        else BuildInstantiated(slots, showWords, variants);

        ApplyPerspective();          // spacing + size are solved against the camera
        Debug.Log($"[WordPath] {(usePool ? "Raised" : "Built")} {_stones.Count} stones for " +
                  $"{slots.Count} words ({_stones.Count(s => s.isKeyword)} key)" +
                  $"{(river != null && river.Valid ? $", along {river.name}" : "")}.");
        return new List<WordStone>(_stones);
    }

    /// <summary>Reuse the pool: raise the stones this sentence needs, sink the rest.</summary>
    void BuildPooled(List<Slot> slots, bool showWords, GameObject[] variants)
    {
        if (slots.Count > poolSlots)
            Debug.LogWarning($"[WordPath] '{slots[0].text}...' is {slots.Count} words but the " +
                             $"pool holds {poolSlots}. Raise Pool Slots — the overflow is dropped.");

        if (_pool.ParentChanged(transform)) _pool.Dispose();
        _pool.EnsureCapacity(poolSlots, transform, variants, stoneKey);
        _pool.Begin();

        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            var ws = _pool.Take(slot.text, slot.isKey,
                                i == slots.Count - 1, showWords);
            if (ws == null) break;                       // pool ran dry
            ws.token = slot.token;
            if (_stones.Count == 0) MeasureStone(ws.gameObject);
            _stones.Add(ws);
            _u.Add(slots.Count == 1 ? 0.5f : i / (float)(slots.Count - 1));
        }
        _pool.EndBuild();
    }

    /// <summary>The original behaviour — one fresh Instantiate per word.</summary>
    void BuildInstantiated(List<Slot> slots, bool showWords, GameObject[] variants)
    {
        Vector3 a = startPoint.position, b = endPoint.position;
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            // the whole path is shared out over however many words there are, so a
            // four-word sentence spans the river just as a twelve-word one does
            float u = slots.Count == 1 ? 0.5f : i / (float)(slots.Count - 1);

            var prefab = slot.isKey && stoneKey != null ? stoneKey
                       : variants.Length > 0 ? variants[i % variants.Length]
                       : stoneKey;
            if (prefab == null) continue;

            var go = Instantiate(prefab, Vector3.Lerp(a, b, u),
                Quaternion.Euler(0, Random.Range(-14f, 14f), 0), transform);
            go.name = $"Stone_{i:00}_{slot.text}";

            var ws = go.GetComponent<WordStone>() ?? go.AddComponent<WordStone>();
            ws.word = showWords ? slot.text : "";
            ws.isKeyword = slot.isKey;
            ws.token = slot.token;
            // a blank stone must not show a lone full stop — whoever fills it sets this
            ws.endsSentence = showWords && i == slots.Count - 1;
            if (_stones.Count == 0) MeasureStone(go);   // the set is sized from one stone
            _stones.Add(ws);
            _u.Add(u);
        }
    }

    /// <summary>
    /// Place and size every stone so the river reads evenly FROM THE CAMERA.
    ///
    /// Two things fight readability when a path recedes from a fixed camera: even
    /// world spacing bunches up in the distance, and even world size makes near
    /// stones giant and far ones specks. Both are fixed by the same 1/z trick that
    /// perspective-correct texture mapping uses —
    ///   • spacing: interpolate 1/distance, not position, so world gaps GROW with
    ///     distance and the gaps look even on screen;
    ///   • size: scale each stone by its distance / the path midpoint's distance,
    ///     so it covers roughly the same screen area wherever it sits.
    /// `perspectiveCompensation` blends between raw perspective (0) and fully even
    /// (1). With no camera it degrades to plain even spacing.
    ///
    /// On top of that, FRAMING decides how big and how near the river reads:
    /// `frameOnScreen` slides both ends along the PathStart→PathEnd line until the
    /// stones fill the screen band `nearScreenY`…`farScreenY`, and
    /// `stoneScreenWidth` solves each stone's scale so it covers that fraction of
    /// the screen at whatever depth it ended up. Between them the river no longer
    /// depends on the two path markers being placed at flattering distances.
    /// </summary>
    public void ApplyPerspective()
    {
        if (startPoint == null || endPoint == null || _stones.Count == 0) return;

        Vector3 a0 = startPoint.position, b0 = endPoint.position;
        Vector3 a = a0, b = b0;
        Vector3 side = Vector3.Cross((b0 - a0).normalized, Vector3.up);

        var cam = viewCamera != null ? viewCamera : Camera.main;

        // A river drawn in Blender is authored IN WORLD SPACE: it bends where the
        // artist put the bend, round the boat and in to the dock. So the screen
        // framing below — which slides the ends along the straight PathStart→PathEnd
        // line — has nothing to slide and would only drag the words off the curve.
        // The camera still gets its say through the spacing and the per-stone size.
        bool onRiver = river != null && river.Valid;

        if (onRiver)
        {
            a = river.SampleByT(0f);
            b = river.SampleByT(1f);
        }
        else if (frameOnScreen && cam != null)
        {
            // Slide BOTH ends along the PathStart→PathEnd line until the river fills
            // the screen band we want. `t` may go negative, which walks the near end
            // back toward the camera — that is what stops the stones huddling in the
            // middle distance, tiny, with dead water in front of them.
            float tNear = SolveScreenY(cam, a0, b0, nearScreenY, -nearReach, farReach);
            float tFar = SolveScreenY(cam, a0, b0, farScreenY, tNear + 0.05f, farReach);
            if (tFar <= tNear) tFar = 1f;                     // degenerate view: use it all
            tNear = ClearOfAvoid(cam, a0, b0, tNear, tFar);   // ...but never behind the book
            a = Vector3.LerpUnclamped(a0, b0, tNear);
            b = Vector3.LerpUnclamped(a0, b0, tFar);
        }
        else
        {
            // the near end of the river slides along until the first stone is out from
            // behind the book — everything below is laid out over what's left
            a = Vector3.Lerp(a0, b0, FindClearStart(cam, a0, b0));
        }

        bool fix = cam != null && perspectiveCompensation > 0.001f;
        Vector3 eye = fix ? cam.transform.position : Vector3.zero;

        float dA = fix ? Mathf.Max(0.01f, Vector3.Distance(eye, a)) : 1f;
        float dB = fix ? Mathf.Max(0.01f, Vector3.Distance(eye, b)) : 1f;
        float dMid = fix ? Vector3.Distance(eye, Vector3.Lerp(a, b, 0.5f)) : 1f;
        // if the path runs across the view rather than into it there is nothing to
        // correct, and this quietly does nothing
        bool recedes = fix && Mathf.Abs(dB - dA) > 0.01f;

        int n = Mathf.Min(_stones.Count, _u.Count);

        // ── pass 1: where each stone sits along the centre line ─────────────
        var centre = new Vector3[n];
        var size = new float[n];
        for (int i = 0; i < n; i++)
        {
            float u = _u[i];
            float t = u;
            if (recedes)
            {
                float d = 1f / Mathf.Lerp(1f / dA, 1f / dB, u);
                t = Mathf.Lerp(u, Mathf.Clamp01((d - dA) / (dB - dA)), perspectiveCompensation);
            }
            // `t` is a fraction of the way ALONG the river, so on a curve it has to
            // be measured by arc length — spacing the words by vertex index would
            // bunch them wherever the artist happened to click more points.
            centre[i] = onRiver ? river.SampleByT(t) : Vector3.Lerp(a, b, t);

            bool byScreen = cam != null && stoneScreenWidth > 0.001f && _unitDiameter > 0.0001f;
            if (byScreen)
            {
                // Solve the size instead of guessing it: whatever its depth, the stone
                // covers `stoneScreenWidth` of the screen. Near stones stop being giant
                // and far ones stop being specks, exactly, rather than approximately.
                float d = Mathf.Max(0.01f, Vector3.Distance(cam.transform.position, centre[i]));
                float wantWorld = PixelsToWorld(cam, Screen.width * stoneScreenWidth, d);
                size[i] = Mathf.Clamp(wantWorld / (_unitDiameter * Mathf.Max(0.0001f, stoneScale)),
                                      0.05f, 20f);
            }
            else
                size[i] = fix
                    ? Mathf.Clamp(Mathf.Lerp(1f, Vector3.Distance(eye, centre[i]) /
                                                 Mathf.Max(0.01f, dMid), perspectiveCompensation),
                                  minSizeFactor, maxSizeFactor)
                    : 1f;
        }

        // ── pass 2: make them FIT the room they actually have on screen ─────
        // Sideways room is NOT a way out of crowding: stepping a stone off the line
        // far enough to matter puts it abreast of its neighbour, and the river reads
        // as two banks instead of one path. So the only lever here is size — shrink
        // the set, but ONLY by as much as the stones genuinely overlap.
        float fit = 1f;
        if (cam != null && n > 1 && _unitDiameter > 0.0001f)
        {
            Vector3 dir = (b - a).normalized;
            float worst = 1f;
            for (int i = 1; i < n; i++)
            {
                Vector3 p = cam.WorldToScreenPoint(centre[i - 1]);
                Vector3 q = cam.WorldToScreenPoint(centre[i]);
                if (p.z <= 0f || q.z <= 0f) continue;
                float gapPx = Vector2.Distance(new Vector2(p.x, p.y), new Vector2(q.x, q.y));

                // Judge the crowding against the stones' footprint ALONG the direction
                // they are separated in, not against their width. A flat stone on a
                // path running away from the camera covers only a fraction of its
                // width that way — measuring the width shrank the whole river for
                // an overlap that was never going to happen.
                float footPx = 0.5f *
                    (FootprintPx(cam, centre[i - 1], dir, _unitDiameter * stoneScale * size[i - 1]) +
                     FootprintPx(cam, centre[i],     dir, _unitDiameter * stoneScale * size[i]));
                float wanted = footPx * (1f + stoneGap);
                if (gapPx > 0.01f && wanted > gapPx) worst = Mathf.Min(worst, gapPx / wanted);
            }
            fit = Mathf.Clamp(worst, minFitFactor, 1f);
        }

        // ── the meander: ONE river, a slow wander off the centre line ───────
        // Amplitude is held down to `maxNeighbourStep` of a stone's width PER GAP,
        // so consecutive stones stay visibly on the same line — the path bends,
        // it never splits.
        float amplitude = curve;
        if (n > 1)
        {
            float biggest = 1f;
            for (int i = 0; i < n; i++) biggest = Mathf.Max(biggest, size[i]);

            if (_unitDiameter > 0.0001f)
            {
                // sin's steepest slope over one gap is 2*pi*waves / (n-1)
                float slope = 2f * Mathf.PI * meanderWaves / (n - 1);
                float stepRoom = _unitDiameter * stoneScale * fit * maxNeighbourStep;
                amplitude = Mathf.Min(amplitude,
                                      stepRoom / Mathf.Max(0.0001f, slope * biggest));
            }
            if (fix)
                amplitude = Mathf.Min(amplitude,
                    PixelsToWorld(cam, Screen.width * maxLateralFraction * 0.5f, dMid) / biggest);
        }

        // ── pass 3: place them ──────────────────────────────────────────────
        for (int i = 0; i < n; i++)
        {
            var ws = _stones[i];
            if (ws == null) continue;

            // The sine meander exists to bend a STRAIGHT line into a river. A curve
            // from Blender is already bent, and adding the wave on top would wobble
            // the artist's shape, so on a river the offset is zero.
            float lateral = !onRiver && n > 1
                ? amplitude * size[i] * Mathf.Sin(_u[i] * meanderWaves * 2f * Mathf.PI)
                : 0f;                                   // a lone stone sits mid-river

            // Standing the words to one side of the line. Taken across the river at
            // THIS stone rather than across the whole path, or the offset would cut
            // the corner on every bend and the words would drift back onto the line
            // exactly where it turns.
            Vector3 across = onRiver
                ? Vector3.Cross(river.DirectionAtT(_u[i]), Vector3.up)
                : side;
            Vector3 pos = centre[i] + side * lateral + across * stoneSideOffset;
            Vector3 scale = Vector3.one * stoneScale * size[i] * fit *
                            (ws.isKeyword ? 1.25f : 1f);
            // face the stone along the river, so a curve does not leave them all
            // squared up to one arbitrary direction
            Quaternion rot = onRiver
                ? Quaternion.LookRotation(river.DirectionAtT(_u[i]), Vector3.up)
                : ws.transform.rotation;

            // With the pool it is the SLOT that gets moved — the stone sits at local
            // zero inside it, so its tap-hop and the slot's sink never fight.
            var slot = usePool ? _pool.SlotAt(i) : null;
            if (slot != null) slot.Place(pos, rot, scale);
            else
            {
                ws.transform.position = pos;
                ws.transform.rotation = rot;
                ws.transform.localScale = scale;
            }
        }

        _lastEye = fix ? eye : _lastEye;
    }

    /// <summary>
    /// How many pixels a stone of <paramref name="worldWidth"/> covers ALONG
    /// <paramref name="dir"/> at this point. Foreshortening is the whole point: the
    /// spacing test has to compare like with like.
    /// </summary>
    static float FootprintPx(Camera cam, Vector3 centre, Vector3 dir, float worldWidth)
    {
        Vector3 p = cam.WorldToScreenPoint(centre - dir * worldWidth * 0.5f);
        Vector3 q = cam.WorldToScreenPoint(centre + dir * worldWidth * 0.5f);
        if (p.z <= 0f || q.z <= 0f) return 0f;
        return Vector2.Distance(new Vector2(p.x, p.y), new Vector2(q.x, q.y));
    }

    /// <summary>
    /// Walk the (extended) PathStart→PathEnd line for the point that lands at
    /// <paramref name="screenY01"/> up the screen. Returns that point's `t`, where
    /// 0 = PathStart and 1 = PathEnd; negative t is nearer the camera than
    /// PathStart. Falls back to <paramref name="lo"/> if the band is never crossed.
    /// </summary>
    static float SolveScreenY(Camera cam, Vector3 a0, Vector3 b0, float screenY01,
                              float lo, float hi)
    {
        float target = screenY01 * Screen.height;
        float best = lo, bestErr = float.MaxValue;
        const int steps = 96;
        for (int i = 0; i <= steps; i++)
        {
            float t = Mathf.Lerp(lo, hi, i / (float)steps);
            Vector3 sp = cam.WorldToScreenPoint(Vector3.LerpUnclamped(a0, b0, t));
            if (sp.z <= 0f) continue;                       // behind the camera
            float err = Mathf.Abs(sp.y - target);
            if (err < bestErr) { bestErr = err; best = t; }
        }
        return best;
    }

    /// <summary>
    /// Push <paramref name="t"/> along the line until the first stone is out from
    /// behind the book. Same job as <see cref="FindClearStart"/>, but on the framed
    /// (possibly extended) line rather than the raw start→end span.
    /// </summary>
    float ClearOfAvoid(Camera cam, Vector3 a0, Vector3 b0, float t, float tMax)
    {
        if (cam == null || avoid == null) return t;
        if (!AvoidScreenRect(cam, out var rect)) return t;

        float pad = Screen.height * avoidMargin;
        rect = Rect.MinMaxRect(rect.xMin - pad, rect.yMin - pad,
                               rect.xMax + pad, rect.yMax + pad);

        float step = Mathf.Max(0.005f, (tMax - t) / 120f);
        for (float s = t; s <= tMax; s += step)
        {
            Vector3 sp = cam.WorldToScreenPoint(Vector3.LerpUnclamped(a0, b0, s));
            if (sp.z <= 0f) continue;
            if (!rect.Contains(new Vector2(sp.x, sp.y))) return s;
        }

        Debug.LogWarning($"[WordPath] '{avoid.name}' covers the whole framed river — " +
                         "raise Near Screen Y, or move the book down the frame.");
        return t;
    }

    /// <summary>How many world units one screen pixel covers at this distance.</summary>
    static float PixelsToWorld(Camera cam, float pixels, float distance)
    {
        if (Screen.height <= 0) return pixels;
        float worldPerPixel = cam.orthographic
            ? cam.orthographicSize * 2f / Screen.height
            : 2f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) / Screen.height;
        return pixels * worldPerPixel;
    }

    /// <summary>A stone prefab's width in world units at scale 1, measured once.</summary>
    void MeasureStone(GameObject go)
    {
        var r = go.GetComponentInChildren<Renderer>();
        if (r == null) { _unitDiameter = 1f; return; }
        var s = go.transform.lossyScale;
        float k = Mathf.Max(0.0001f, Mathf.Max(Mathf.Abs(s.x), Mathf.Abs(s.z)));
        _unitDiameter = Mathf.Max(r.bounds.size.x, r.bounds.size.z) / k;
    }

    /// <summary>
    /// How far along the path the first stone must sit to be visible. The first
    /// stone lands exactly on the path line (its sideways curve offset is sin(0) = 0),
    /// so this walks that line until the stone's screen point is outside the book's
    /// screen rectangle. Returns a fraction of the path, never more than 0.6.
    /// </summary>
    float FindClearStart(Camera cam, Vector3 a, Vector3 b)
    {
        float t = Mathf.Clamp(startInset, 0f, 0.6f);
        if (cam == null || avoid == null) return t;
        if (!AvoidScreenRect(cam, out var rect)) return t;

        float pad = Screen.height * avoidMargin;
        rect = Rect.MinMaxRect(rect.xMin - pad, rect.yMin - pad,
                               rect.xMax + pad, rect.yMax + pad);

        for (; t <= 0.6f; t += 0.01f)
        {
            Vector3 sp = cam.WorldToScreenPoint(Vector3.Lerp(a, b, t));
            if (sp.z <= 0f) continue;                       // behind the camera
            if (!rect.Contains(new Vector2(sp.x, sp.y))) return t;
        }

        Debug.LogWarning($"[WordPath] '{avoid.name}' covers the near end of the path — " +
                         "the first stone is inset as far as it can go. Move the start " +
                         "point, or the book, further apart.");
        return 0.6f;
    }

    /// <summary>The avoided object's bounds, as a rectangle on screen.</summary>
    bool AvoidScreenRect(Camera cam, out Rect rect)
    {
        rect = default;
        if (_avoidCached != avoid)
        { _avoidCached = avoid; _avoidRends = avoid != null ? avoid.GetComponentsInChildren<Renderer>() : null; }
        if (_avoidRends == null || _avoidRends.Length == 0) return false;

        bool any = false;
        var bounds = new Bounds();
        foreach (var r in _avoidRends)
        {
            if (r == null) continue;
            if (!any) { bounds = r.bounds; any = true; } else bounds.Encapsulate(r.bounds);
        }
        if (!any) return false;

        float xMin = float.MaxValue, xMax = float.MinValue;
        float yMin = float.MaxValue, yMax = float.MinValue;
        for (int i = 0; i < 8; i++)
        {
            var corner = new Vector3((i & 1) == 0 ? bounds.min.x : bounds.max.x,
                                     (i & 2) == 0 ? bounds.min.y : bounds.max.y,
                                     (i & 4) == 0 ? bounds.min.z : bounds.max.z);
            var sp = cam.WorldToScreenPoint(corner);
            if (sp.z <= 0f) continue;
            xMin = Mathf.Min(xMin, sp.x); xMax = Mathf.Max(xMax, sp.x);
            yMin = Mathf.Min(yMin, sp.y); yMax = Mathf.Max(yMax, sp.y);
        }
        if (xMin > xMax) return false;

        rect = Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        return true;
    }

    // The camera flies in at the start of the scene (IslandFlow), so the layout is
    // re-solved while it moves and then left alone. A still camera means a stone's
    // tap-hop is never fought by this.
    void LateUpdate()
    {
        if (perspectiveCompensation <= 0.001f || _stones.Count == 0) return;
        var cam = viewCamera != null ? viewCamera : Camera.main;
        if (cam == null) return;
        if ((cam.transform.position - _lastEye).sqrMagnitude < 0.0004f) return;   // ~2 cm
        ApplyPerspective();
    }

    // ── helpers ─────────────────────────────────────────────────────────────

    /// <summary>The inspector list plus any legacy single `keyword`.</summary>
    public IEnumerable<string> AllKeywords()
    {
        if (keywords != null)
            foreach (var k in keywords)
                if (!string.IsNullOrWhiteSpace(k)) yield return k;
        if (!string.IsNullOrWhiteSpace(keyword)) yield return keyword;
    }

    public static IEnumerable<string> SplitWords(string text) =>
        (text ?? "").Trim()
            .Split(new[] { ' ', '\t', '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

    /// <summary>Compare words ignoring case, punctuation and key-word markers.</summary>
    public static string Normalise(string w) =>
        (w ?? "").Trim()
            .Trim('*', '[', ']', '.', ',', '!', '?', ';', ':', '"', '\'', '(', ')')
            .ToLowerInvariant();

    void ClearStones()
    {
        _stones.Clear();
        _u.Clear();

        if (usePool)
        {
            // The pool is the point — nothing is destroyed. Sink whatever is up;
            // the next build raises what it needs straight back out of the water.
            _pool.SinkAll();
            // ...but sweep away stones left by a previous non-pooled build (or by
            // an older scene that serialised them as children), or they float there
            // for ever with no one owning them.
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var c = transform.GetChild(i).gameObject;
                if (c.GetComponent<StoneSlot>() != null) continue;
                if (Application.isPlaying) Destroy(c); else DestroyImmediate(c);
            }
            return;
        }

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var c = transform.GetChild(i).gameObject;
            if (Application.isPlaying) Destroy(c); else DestroyImmediate(c);
        }
    }
}
