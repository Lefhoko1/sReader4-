// ===========================================================================
//  WordStonePool — the river's stones, made once and reused for every sentence.
// ===========================================================================
//  Rebuilding the river used to destroy every stone and instantiate a fresh set
//  per sentence, and each new stone built itself a TextMeshPro label and a
//  collider from scratch. The pool makes the set once: after that a sentence
//  change is a handful of transform writes and some SetActive calls.
//
//  Each pool entry is a StoneSlot holding BOTH variants (plain + gold key) at
//  the same spot, so switching a word between the two costs nothing.
//
//  Not a MonoBehaviour — WordPathBuilder owns one and drives it.
// ===========================================================================
using System.Collections.Generic;
using UnityEngine;

public class WordStonePool
{
    readonly List<StoneSlot> _slots = new List<StoneSlot>();
    Transform _parent;
    int _live;                  // slots handed out in the current build

    public int Count => _slots.Count;

    /// <summary>
    /// Make sure the pool holds at least <paramref name="want"/> slots, parented
    /// under <paramref name="parent"/>. Cheap to call every build — it only does
    /// work when the pool has to grow.
    /// </summary>
    public void EnsureCapacity(int want, Transform parent,
                               GameObject[] variants, GameObject keyPrefab)
    {
        _parent = parent;
        if (variants == null || variants.Length == 0)
        {
            if (keyPrefab == null) return;
            variants = new[] { keyPrefab };
        }

        // Several editor setup tools clear the builder's children wholesale, which
        // takes the pool's slots with them. Drop the corpses before counting, or the
        // pool believes it is full and hands out destroyed objects.
        _slots.RemoveAll(s => s == null);

        while (_slots.Count < want)
        {
            int i = _slots.Count;
            var root = new GameObject($"Slot_{i:00}");
            root.transform.SetParent(parent, false);
            var slot = root.AddComponent<StoneSlot>();

            slot.normal = MakeStone(variants[i % variants.Length], root.transform, "Normal");
            slot.key = MakeStone(keyPrefab != null ? keyPrefab : variants[i % variants.Length],
                                 root.transform, "Key");
            if (slot.key != null) slot.key.isKeyword = true;
            if (slot.normal != null) slot.normal.isKeyword = false;

            slot.Sink(true);
            _slots.Add(slot);
        }
    }

    static WordStone MakeStone(GameObject prefab, Transform parent, string name)
    {
        if (prefab == null) return null;
        var go = Object.Instantiate(prefab, parent);
        go.name = name;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        return go.GetComponent<WordStone>() ?? go.AddComponent<WordStone>();
    }

    /// <summary>Start handing out slots for a new sentence.</summary>
    public void Begin() => _live = 0;

    /// <summary>
    /// Take the next slot, showing <paramref name="isKey"/>'s variant with this
    /// word on it. Returns the WordStone the rest of the game talks to, or null
    /// if the pool has run dry.
    /// </summary>
    public WordStone Take(string word, bool isKey, bool endsSentence, bool showWord)
    {
        if (_live >= _slots.Count) return null;
        var slot = _slots[_live++];

        slot.Raise(isKey);
        var ws = slot.Active;
        if (ws == null) return null;

        // A pooled stone carries whatever the last sentence left on it — its
        // listeners, its solved star, its tutor content. All of that has to go,
        // or word 3 of this sentence quietly answers for word 3 of the last one.
        ws.ResetForReuse();
        ws.word = showWord ? word : "";
        ws.isKeyword = isKey;
        ws.endsSentence = showWord && endsSentence;
        ws.Relabel();
        return ws;
    }

    /// <summary>Sink every slot the sentence did not use.</summary>
    public void EndBuild()
    {
        for (int i = _live; i < _slots.Count; i++) _slots[i].Sink();
    }

    /// <summary>Sink the whole river (used where the old code cleared the stones).</summary>
    public void SinkAll(bool instant = false)
    {
        _live = 0;
        foreach (var s in _slots) if (s != null) s.Sink(instant);
    }

    /// <summary>The slot backing the i-th stone of the current sentence.</summary>
    public StoneSlot SlotAt(int i) =>
        i >= 0 && i < _live && i < _slots.Count ? _slots[i] : null;

    /// <summary>Throw the pool away — the prefabs or the parent changed.</summary>
    public void Dispose()
    {
        foreach (var s in _slots)
        {
            if (s == null) continue;
            if (Application.isPlaying) Object.Destroy(s.gameObject);
            else Object.DestroyImmediate(s.gameObject);
        }
        _slots.Clear();
        _live = 0;
        _parent = null;
    }

    /// <summary>True if the pool was built under a different parent.</summary>
    public bool ParentChanged(Transform parent) => _parent != parent;
}
