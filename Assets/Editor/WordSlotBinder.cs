// ===========================================================================
//  WordSlotBinder — adopt the word stones authored in Blender
// ===========================================================================
//  Tools > Great Library > Island > 9. Bind Authored Word Slots
//
//  Blender now owns WHERE the words land. The island ships, at every one of the
//  twenty positions beside the walk line, a slot empty with BOTH stones hanging
//  off it at local zero:
//
//      WORDSLOT_07                     (empty: the position and the facing)
//        WORDSLOT_07_Normal            + WORDSLOT_07_Normal_SOCKET_Text
//        WORDSLOT_07_Key               + _SOCKET_Text, _SOCKET_Card, _Ring, _Star
//
//  Both stones sit at local zero on purpose: which one is showing can then never
//  move the word, because neither of them carries the position — the empty does.
//  That is exactly the contract StoneSlot was already written against, so this
//  binder is all that stands between the two.
//
//  WHAT THIS REPLACES. WordStonePool used to Instantiate() a normal and a key
//  prefab per slot and parent them under a generated Slot_NN. Those forty objects
//  now arrive with the world, correctly placed, visible in Blender before anyone
//  opens Unity. This does not create geometry; it only adopts it.
//
//  Safe to re-run: everything here is idempotent, and re-running is exactly what
//  you do after re-importing the island.
// ===========================================================================
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class WordSlotBinder
{
    const string SLOT_PREFIX = "WORDSLOT_";

    /// <summary>
    /// Height the stones sit at, in metres above the water. Found by hand in the
    /// scene: at the waterline they read as half sunk, and this is where they read
    /// as lying ON the river.
    /// </summary>
    const float stoneLift = 0.15f;
    const float waterLine = 0f;
    const string TEXT_SOCKET = "SOCKET_Text";
    const string CARD_SOCKET = "SOCKET_Card";

    [MenuItem("Tools/Great Library/Island/10. Bind Authored Word Slots", true)]
    static bool BindValidate() => !Application.isPlaying;

    [MenuItem("Tools/Great Library/Island/10. Bind Authored Word Slots")]
    public static void Bind()
    {
        var slots = FindSlots();
        if (slots.Count == 0)
        {
            Debug.LogError(
                $"[Slots] No '{SLOT_PREFIX}*' objects in the open scene. They come in " +
                "with SM_Island_World.fbx — run 'Tools ▸ Great Library ▸ Island ▸ " +
                "0. Rebuild World From Blender' first, and check the FBX was " +
                "re-exported after the stones were placed in Blender.");
            return;
        }

        int bound = 0, missingNormal = 0, missingKey = 0, sockets = 0, noSocket = 0;

        for (int i = 0; i < slots.Count; i++)
        {
            var t = slots[i];

            // ROTATION IS NOT TOUCHED. The FBX already carries it: a -90° X on every
            // node, which is what turns a Blender Z-up stone into one lying flat in
            // Unity. Two attempts to "improve" it made this worse — first a
            // LookRotation that wiped the X and stood every stone on edge, then a
            // "preserve the tilt" version that read the tilt back off a rotation the
            // previous run had already destroyed, and so faithfully preserved the
            // damage. The authored value is right; the job here is to leave it alone.
            //
            // A corrupted rotation therefore cannot be repaired by re-binding. Delete
            // IslandWorld and re-run 0 to get clean ones from the FBX.
            Undo.RecordObject(t, "Bind Authored Word Slots");

            // Sit the stone proud of the water rather than half in it.
            t.position = new Vector3(t.position.x, waterLine + stoneLift, t.position.z);

            var slot = t.GetComponent<StoneSlot>();
            if (slot == null) slot = Undo.AddComponent<StoneSlot>(t.gameObject);
            Undo.RecordObject(slot, "Bind Authored Word Slots");

            var normal = Adopt(t, "_Normal", isKey: false, ref sockets, ref noSocket);
            var key = Adopt(t, "_Key", isKey: true, ref sockets, ref noSocket);
            if (normal == null) missingNormal++;
            if (key == null) missingKey++;

            slot.normal = normal;
            slot.key = key;

            // Seed the slot's home. On a first bind the transform IS the authored
            // position, straight from the FBX. On a RE-bind the slot may currently
            // be sunk, and re-deriving a home from that would drive the stone a
            // little further under the water every time — so a home it already
            // remembers wins.
            Vector3 home = slot.Home.sqrMagnitude > 0.0001f ? slot.Home : t.position;
            slot.Place(home, t.rotation, t.localScale);

            // Show the plain stone for now so binding never looks like it deleted
            // the river. Which stones ACTUALLY belong up is the sentence's call,
            // and ReapplySentence() below hands it straight back.
            slot.Raise(false, instant: true);

            EditorUtility.SetDirty(slot);
            bound++;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = slots[0].gameObject;

        Transform first = slots[0];

        Debug.Log(
            $"[Slots] Bound {bound} authored slot(s) from Blender.\n" +
            $"  • rotation left exactly as the FBX authored it; " +
            $"{first.name} euler {first.rotation.eulerAngles}\n" +
            $"  • sockets renamed to '{TEXT_SOCKET}': {sockets}; stones with NO label " +
            $"socket at all: {noSocket} of {bound * 2} " +
            (noSocket > 0
                ? "— those will come up blank, the FBX did not carry their socket child"
                : "— every stone can be labelled") + "\n" +
            (missingNormal > 0 ? $"  ! {missingNormal} slot(s) had no _Normal child\n" : "") +
            (missingKey > 0 ? $"  ! {missingKey} slot(s) had no _Key child\n" : "") +
            $"  • {first.name} at {first.position}\n" +
            $"  • {ReapplySentence()}");

        WarnAboutThePool();
    }

    /// <summary>
    /// Give the sentence the last word.
    ///
    /// Binding raises a plain stone on EVERY slot so the river is visible while you
    /// wire it up — but the world rebuild assigns the sentence BEFORE this runs, so
    /// left alone the bind would overwrite a seven-word river with twenty blank
    /// stones. Rebuilding here puts the words back, and the cache is dropped first
    /// because the slots it points at may have just been re-imported.
    /// </summary>
    static string ReapplySentence()
    {
        var builder = Object.FindAnyObjectByType<WordPathBuilder>(FindObjectsInactive.Include);
        if (builder == null)
            return "no WordPathBuilder — all stones left showing. 10b / 10c toggle them.";
        if (!builder.useAuthoredSlots)
            return $"'{builder.name}' has Use Authored Slots OFF, so it is still making " +
                   "its own stones — tick it on to use these.";

        builder.ForgetAuthoredSlots();
        builder.Build();
        return $"sentence re-applied: {builder.Stones.Count} stone(s) carry words, the " +
               "rest are back under the water. 10b shows them all again.";
    }

    /// <summary>
    /// Delete the pooled stones WordPathBuilder used to make for itself.
    ///
    /// They leak. The pool is a plain C# object owned by the builder, so a script
    /// recompile forgets its list while the GameObjects it made live on in the
    /// scene — every rebuild then stacked another full set on the same river
    /// (24 a time, 312 by the time we noticed). Now that the stones are authored in
    /// Blender the pool has no job at all, and this clears out what it left.
    /// </summary>
    [MenuItem("Tools/Great Library/Island/10d. Remove Leftover Pooled Stones")]
    public static void RemovePooled()
    {
        var builder = Object.FindAnyObjectByType<WordPathBuilder>(FindObjectsInactive.Include);
        if (builder == null) { Debug.Log("[Slots] No WordPathBuilder in the scene."); return; }

        var doomed = builder.GetComponentsInChildren<StoneSlot>(true)
                            .Where(s => s != null && s.transform.parent == builder.transform)
                            .Select(s => s.gameObject)
                            .ToList();
        if (doomed.Count == 0)
        {
            Debug.Log($"[Slots] '{builder.name}' has no pooled stones left — the river " +
                      "is entirely the Blender-authored one.");
            return;
        }

        foreach (var go in doomed) Undo.DestroyObjectImmediate(go);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"[Slots] Removed {doomed.Count} leftover pooled stone(s) from " +
                  $"'{builder.name}'. Only the authored stones remain. (Undo restores them.)");
    }

    /// <summary>
    /// The pool has not been retired yet, and it builds its own stones on the same
    /// river. Until word assignment moves over to the authored slots you get BOTH
    /// sets — which reads as "the import duplicated everything" if nobody says so.
    /// </summary>
    static void WarnAboutThePool()
    {
        var builder = Object.FindAnyObjectByType<WordPathBuilder>(FindObjectsInactive.Include);
        if (builder == null) return;

        int pooled = builder.GetComponentsInChildren<StoneSlot>(true)
                            .Count(s => s.transform.parent == builder.transform);
        if (pooled == 0) return;

        Debug.LogWarning(
            $"[Slots] '{builder.name}' is still carrying {pooled} pooled stone(s) of its " +
            "own on the same river, left over from before the stones were authored in " +
            "Blender. You will see TWO sets until they go. Run " +
            "'10d. Remove Leftover Pooled Stones'.");
    }

    // ── looking at them ─────────────────────────────────────────────────────
    //  StoneSlot hides a stone by SINKING it and then switching the GameObject
    //  off, which is right in play and alarming in the editor: the slots simply
    //  disappear. These two put the river back on screen without entering play.

    [MenuItem("Tools/Great Library/Island/10b. Show All Word Stones")]
    public static void ShowAll() => SetAll(raise: true);

    [MenuItem("Tools/Great Library/Island/10c. Hide All Word Stones")]
    public static void HideAll() => SetAll(raise: false);

    static void SetAll(bool raise)
    {
        var slots = FindSlots();
        int n = 0;
        foreach (var t in slots)
        {
            var slot = t.GetComponent<StoneSlot>();
            if (slot == null) continue;
            Undo.RecordObject(slot, raise ? "Show Word Stones" : "Hide Word Stones");
            Undo.RecordObject(t, raise ? "Show Word Stones" : "Hide Word Stones");

            if (raise)
            {
                if (slot.Home.sqrMagnitude < 0.0001f)
                    slot.Place(t.position, t.rotation, t.localScale);  // never placed
                slot.Raise(false, instant: true);
            }
            else slot.Sink(true);
            EditorUtility.SetDirty(slot);
            n++;
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"[Slots] {(raise ? "Raised" : "Sank")} {n} word slot(s)." +
                  (n == 0 ? " None are bound yet — run '10. Bind Authored Word Slots'." : ""));
    }

    /// <summary>
    /// Take one of a slot's two stones and make it something WordStone can drive:
    /// the label socket named exactly what it looks for, the component attached,
    /// and the key flag set from which child it is.
    /// </summary>
    static WordStone Adopt(Transform slot, string suffix, bool isKey,
                           ref int sockets, ref int noSocket)
    {
        Transform stone = null;
        foreach (Transform c in slot)
            if (c.name.EndsWith(suffix)) { stone = c; break; }
        if (stone == null) return null;

        bool hasSocket = false;
        foreach (Transform c in stone)
            if (c.name.Contains(TEXT_SOCKET)) { hasSocket = true; break; }
        if (!hasSocket) noSocket++;

        // WordStone finds its label with transform.Find("SOCKET_Text"), so the name
        // has to be exactly that. It arrives as anything but.
        //
        // MATCH BY Contains, NOT EndsWith. Blender cannot hold forty objects called
        // SOCKET_Text, and neither could wordstones.blend hold four — each template
        // got a uniquifying suffix, so the sockets come through as
        // "WORDSLOT_04_Normal_SOCKET_Text.002": prefixed AND suffixed. An EndsWith
        // test matched only stone A's unsuffixed socket, which is why 33 of 40
        // stones looked like they had no socket at all.
        foreach (Transform c in stone)
        {
            bool isText = c.name.Contains(TEXT_SOCKET);
            bool isCard = !isText && c.name.Contains(CARD_SOCKET);
            if (!isText && !isCard) continue;

            string want = isText ? TEXT_SOCKET : CARD_SOCKET;
            if (c.name == want) continue;

            Undo.RecordObject(c.gameObject, "Bind Authored Word Slots");
            c.name = want;
            if (isText) sockets++;
        }

        var ws = stone.GetComponent<WordStone>();
        if (ws == null) ws = Undo.AddComponent<WordStone>(stone.gameObject);
        Undo.RecordObject(ws, "Bind Authored Word Slots");
        ws.isKeyword = isKey;

        // Push the label settings onto the component, not just into the defaults.
        // A WordStone added in an earlier session has its own serialized values and
        // would keep the old 0.3 m offset — which, before it was corrected to world
        // units, meant the word hung fifteen metres above the river.
        ws.wordHeight = 0.06f;
        ws.layFlat = true;
        ws.hopOnTap = false;
        ws.labelScale = 1f;

        EditorUtility.SetDirty(ws);
        return ws;
    }

    /// <summary>The authored slots, in reading order — WORDSLOT_00 is furthest out to sea.</summary>
    static List<Transform> FindSlots()
    {
        var found = new List<Transform>();
        foreach (var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
                if (t.name.StartsWith(SLOT_PREFIX) && t.childCount > 0 &&
                    t.GetComponent<MeshFilter>() == null)
                    found.Add(t);

        // Name order IS reading order: the exporter numbers them along the river.
        found.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
        return found;
    }

    static Vector3 Flat(Vector3 v)
    {
        v.y = 0f;
        return v.sqrMagnitude > 1e-6f ? v.normalized : Vector3.forward;
    }
}
