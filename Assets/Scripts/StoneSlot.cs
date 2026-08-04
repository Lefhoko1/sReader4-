// ===========================================================================
//  StoneSlot — one reusable place in the river, holding BOTH stones.
// ===========================================================================
//  A slot owns a normal stone and a gold key stone sitting at the same spot.
//  Which one a word needs is decided by enabling one and disabling the other —
//  no prefab is instantiated or destroyed once the pool is warm.
//
//  Hiding is done by SINKING the slot under the water rather than switching it
//  off on the spot: the stone slides down out of sight, and only once it is
//  fully under does the GameObject go inactive. That buys three things at once —
//  the "uproot" look, no popping, and no cost at all while hidden (Unity does
//  not tick Update on an inactive object, so a sunk stone stops raycasting).
//
//  The slot is what the builder positions and scales. The stones are children at
//  local zero, so WordStone's tap-hop (which moves localPosition) never fights
//  the sink.
// ===========================================================================
using UnityEngine;

public class StoneSlot : MonoBehaviour
{
    public WordStone normal;
    public WordStone key;

    [Tooltip("How far under the water the stone goes when hidden, in world units.")]
    public float sinkDepth = 1.4f;
    [Tooltip("Metres per second the stone rises and sinks.")]
    public float speed = 2.6f;

    // Where this slot stands when raised, IN ITS PARENT'S SPACE.
    //
    // Serialized, because the slots are authored in Blender and the position has to
    // survive a recompile and a scene reload — a plain private field does not, and
    // losing it sends the stone to the world origin.
    //
    // LOCAL, not world, and that distinction is the whole bug it was written with:
    // a cached WORLD position goes stale the moment anybody moves or rotates the
    // island root. Every stone then snaps back to where it used to be before the
    // rotation, which looks like the river collapsing into a heap. Held against the
    // parent, the slots simply travel with the world, as they should.
    [SerializeField, HideInInspector] Vector3 _homeLocal;
    float _offset;          // how far below home it is right now (0 = up)
    float _target;          // where it is heading
    bool _isKey;

    /// <summary>The stone this slot is currently showing (null if hidden).</summary>
    public WordStone Active => !Raised ? null : (_isKey ? key : normal);

    /// <summary>True while this slot is raised (or on its way up).</summary>
    public bool Raised { get; private set; }

    /// <summary>
    /// Where this slot sits when raised. Zero means it has never been placed —
    /// worth checking before raising, because a slot that has been sunk is no
    /// longer standing at its own home and re-deriving one from the transform
    /// would bake the sunk position in for good.
    /// </summary>
    public Vector3 Home =>
        transform.parent != null ? transform.parent.TransformPoint(_homeLocal) : _homeLocal;

    /// <summary>Put the slot's raised position / facing / size. Does not raise it.</summary>
    public void Place(Vector3 worldPos, Quaternion rot, Vector3 scale)
    {
        _homeLocal = transform.parent != null
            ? transform.parent.InverseTransformPoint(worldPos)
            : worldPos;
        transform.rotation = rot;
        transform.localScale = scale;
        transform.position = Home - Vector3.up * _offset;
        sinkDepth = MeasuredSinkDepth();
    }

    /// <summary>
    /// Far enough under to be out of sight, measured from THE STONE — never from
    /// the slot's own scale.
    ///
    /// This used to be scale.y * 2.2, which was fine while the slots were built in
    /// Unity at scale 1. The slots are authored in Blender now and arrive as FBX
    /// empties, and an empty has no meaningful scale — the importer handed these
    /// ones about 100. So "hide this stone" quietly meant "drop it 220 metres",
    /// and because the next bind read the slot's home back off that sunk
    /// transform, the hole got deeper every single time: -220, then -750.
    ///
    /// The stone's rendered height is the only honest measure of how far it has to
    /// go to disappear, and it cannot be poisoned by a number nobody authored.
    /// </summary>
    float MeasuredSinkDepth()
    {
        float tallest = 0f;
        foreach (var ws in new[] { normal, key })
        {
            if (ws == null) continue;
            foreach (var r in ws.GetComponentsInChildren<Renderer>(true))
                tallest = Mathf.Max(tallest, r.bounds.size.y);
        }
        // a stone and a bit, so the top clears the water; never a runaway number
        return Mathf.Clamp(tallest * 2.2f, 1.0f, 4.0f);
    }

    /// <summary>Show this slot, as a key word or a plain one.</summary>
    public void Raise(bool isKey, bool instant = false)
    {
        _isKey = isKey;
        gameObject.SetActive(true);
        if (normal != null) normal.gameObject.SetActive(!isKey);
        if (key != null) key.gameObject.SetActive(isKey);

        Raised = true;
        _target = 0f;
        if (instant || !Application.isPlaying)
        {
            _offset = 0f;
            transform.position = Home;
        }
        enabled = true;
    }

    /// <summary>Hide this slot by sinking it under the water.</summary>
    public void Sink(bool instant = false)
    {
        Raised = false;
        _target = sinkDepth;
        if (instant || !Application.isPlaying)
        {
            _offset = sinkDepth;
            transform.position = Home - Vector3.up * _offset;
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);   // it must live long enough to animate down
        enabled = true;
    }

    void Update()
    {
        if (Mathf.Approximately(_offset, _target))
        {
            // settled — stop ticking, and switch off entirely once fully under
            enabled = false;
            if (!Raised) gameObject.SetActive(false);
            return;
        }

        _offset = Mathf.MoveTowards(_offset, _target, speed * Time.deltaTime);
        transform.position = Home - Vector3.up * _offset;

        if (!Raised && _offset >= sinkDepth - 0.001f)
        {
            enabled = false;
            gameObject.SetActive(false);
        }
    }
}
