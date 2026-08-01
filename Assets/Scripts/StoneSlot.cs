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

    Vector3 _home;          // where the slot sits when fully raised
    float _offset;          // how far below home it is right now (0 = up)
    float _target;          // where it is heading
    bool _isKey;

    /// <summary>The stone this slot is currently showing (null if hidden).</summary>
    public WordStone Active => !Raised ? null : (_isKey ? key : normal);

    /// <summary>True while this slot is raised (or on its way up).</summary>
    public bool Raised { get; private set; }

    /// <summary>Put the slot's raised position / facing / size. Does not raise it.</summary>
    public void Place(Vector3 worldPos, Quaternion rot, Vector3 scale)
    {
        _home = worldPos;
        transform.rotation = rot;
        transform.localScale = scale;
        transform.position = _home - Vector3.up * _offset;
        // sink depth has to clear the stone itself, however big it was scaled
        sinkDepth = Mathf.Max(1.0f, scale.y * 2.2f);
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
            transform.position = _home;
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
            transform.position = _home - Vector3.up * _offset;
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
        transform.position = _home - Vector3.up * _offset;

        if (!Raised && _offset >= sinkDepth - 0.001f)
        {
            enabled = false;
            gameObject.SetActive(false);
        }
    }
}
