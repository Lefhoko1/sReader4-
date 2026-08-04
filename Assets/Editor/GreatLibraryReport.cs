// ===========================================================================
//  GreatLibraryReport — say what is ACTUALLY in the scene, in one go
// ===========================================================================
//  Tools > Great Library > Island > ?. Report What Is Actually There
//
//  WHY THIS EXISTS. Every fix in this system has been made blind: a change goes
//  in, a screenshot comes back, and the gap between them gets filled with a
//  guess. Guesses have cost a round trip each — the label that inherited the
//  stone's scale, the home position cached in world space, the socket matched
//  with EndsWith instead of Contains. Every one of those was visible in the
//  scene data the whole time and nobody looked, because looking meant knowing
//  which of forty things to look at.
//
//  So this looks at all of them at once and prints the numbers. It changes
//  NOTHING. Run it, paste the output, and the next fix is made from facts.
//
//  Run it in edit mode for the authored state, and again while playing for what
//  the game actually did with it — the two differing is itself the answer to a
//  whole class of "it looked right in the editor" problems.
// ===========================================================================
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class GreatLibraryReport
{
    [MenuItem("Tools/Great Library/Island/?. Report What Is Actually There")]
    public static void Report()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[Report] {(Application.isPlaying ? "PLAYING" : "edit mode")} — " +
                      $"scene '{EditorSceneManager.GetActiveScene().name}'");

        World(sb);
        Slots(sb);
        Builder(sb);
        Road(sb);
        Reader(sb);
        Shot(sb);

        Debug.Log(sb.ToString());
    }

    // ── the world root: its transform is the thing everything else hangs off ──
    static void World(StringBuilder sb)
    {
        var w = Find("IslandWorld");
        if (w == null) { sb.AppendLine("WORLD: no 'IslandWorld' — run 0."); return; }

        sb.AppendLine($"WORLD: pos {V(w.position)}  rot {V(w.eulerAngles)}  " +
                      $"scale {V(w.lossyScale)}  active {w.gameObject.activeInHierarchy}");
        if (w.eulerAngles.sqrMagnitude > 1f || (w.position).sqrMagnitude > 0.01f)
            sb.AppendLine("       ^ NOT at identity. Anything cached in world space " +
                          "before this moved is now stale.");
    }

    // ── the authored stones ─────────────────────────────────────────────────
    static void Slots(StringBuilder sb)
    {
        var slots = All<StoneSlot>()
            .Where(s => s.name.StartsWith("WORDSLOT_"))
            .OrderBy(s => s.name, System.StringComparer.Ordinal).ToList();

        if (slots.Count == 0)
        {
            sb.AppendLine("SLOTS: none found. Either the FBX has no WORDSLOT_ objects " +
                          "or 10 was never run.");
            return;
        }

        int raised = 0, wired = 0, noHome = 0, withText = 0;
        float minGap = float.MaxValue;
        var lines = new List<string>();

        for (int i = 0; i < slots.Count; i++)
        {
            var s = slots[i];
            bool up = s.gameObject.activeInHierarchy;
            if (up) raised++;
            if (s.normal != null && s.key != null) wired++;
            if (s.Home.sqrMagnitude < 0.0001f) noHome++;

            if (i > 0)
            {
                float gap = Vector3.Distance(s.Home, slots[i - 1].Home);
                if (gap < minGap) minGap = gap;
            }

            var ws = s.normal;
            var tmp = ws != null ? ws.GetComponentInChildren<TextMeshPro>(true) : null;
            if (tmp != null && !string.IsNullOrEmpty(tmp.text)) withText++;

            if (i < 3 || i == slots.Count - 1)
                lines.Add($"       {s.name}: home {V(s.Home)} at {V(s.transform.position)}" +
                          $" {(up ? "UP" : "sunk")}" +
                          $" word '{(ws != null ? ws.word : "-")}'" +
                          $" label '{(tmp != null ? tmp.text : "NONE")}'" +
                          $" labelWorldY {(tmp != null ? tmp.transform.lossyScale.y.ToString("0.00") : "-")}");
        }

        sb.AppendLine($"SLOTS: {slots.Count} found, {wired} fully wired, {raised} showing, " +
                      $"{withText} carrying a label, {noHome} with no home set");
        sb.AppendLine($"       closest two stones: {minGap:0.00} m apart");
        foreach (var l in lines) sb.AppendLine(l);
        if (noHome > 0)
            sb.AppendLine("       ^ a slot with no home raises to the world ORIGIN. Run 10.");
    }

    static void Builder(StringBuilder sb)
    {
        var b = Object.FindAnyObjectByType<WordPathBuilder>(FindObjectsInactive.Include);
        if (b == null) { sb.AppendLine("BUILDER: none."); return; }

        sb.AppendLine($"BUILDER: '{b.name}' active {b.gameObject.activeInHierarchy}, " +
                      $"authored slots {(b.useAuthoredSlots ? "ON" : "OFF")}, " +
                      $"{b.Stones.Count} stone(s) carrying this sentence, " +
                      $"sentence \"{Trim(b.sentence)}\"");
        if (!b.useAuthoredSlots)
            sb.AppendLine("       ^ OFF means it is still making its own stones, " +
                          "ignoring everything placed in Blender.");
    }

    static void Road(StringBuilder sb)
    {
        var r = Object.FindAnyObjectByType<RiversideRoad>(FindObjectsInactive.Include);
        if (r == null) { sb.AppendLine("ROAD: none — nothing to walk."); return; }

        sb.AppendLine($"ROAD: '{r.name}' active {r.gameObject.activeInHierarchy}, " +
                      $"{r.StopCount} stop(s) over {r.Length:0.0} m, " +
                      $"side offset {r.sideOffset:0.00}, mesh {(r.drawMesh ? "ON" : "off")}");
        if (r.StopCount > 0)
            sb.AppendLine($"      starts {V(r.PositionAt(0f))} → ends {V(r.PositionAt(r.Length))}");
    }

    static void Reader(StringBuilder sb)
    {
        var w = Object.FindAnyObjectByType<PathWalker>(FindObjectsInactive.Include);
        if (w == null) { sb.AppendLine("READER: none."); return; }

        sb.AppendLine($"READER: at {V(w.transform.position)}, {w.Distance:0.00} m along, " +
                      $"control {(w.followPointer ? "BY HAND" : "automatic")}, " +
                      $"seated {w.Seated}, " +
                      $"at stone {(w.CurrentStone != null ? w.CurrentStone.name : "none")}");

        var road = w.road;
        if (road != null && road.StopCount > 0)
        {
            float toFirst = Vector3.Distance(w.transform.position, road.PositionAt(road.StopDistance(0)));
            sb.AppendLine($"        {toFirst:0.0} m from the first word " +
                          (toFirst > 8f ? "— that is a long way; they may be starting at the wrong end." : ""));
        }
    }

    static void Shot(StringBuilder sb)
    {
        var cam = Camera.main;
        if (cam == null) { sb.AppendLine("CAMERA: no Camera.main!"); return; }

        var wc = cam.GetComponent<WalkCamera>();
        var walker = Object.FindAnyObjectByType<PathWalker>(FindObjectsInactive.Include);
        float d = walker != null
            ? Vector3.Distance(cam.transform.position, walker.transform.position) : -1f;

        sb.AppendLine($"CAMERA: at {V(cam.transform.position)}, fov {cam.fieldOfView:0.0}, " +
                      $"aspect {cam.aspect:0.00}, {d:0.0} m from the reader");
        if (wc != null)
        {
            sb.AppendLine($"        shotDistance {wc.shotDistance:0.00}, " +
                          $"aspectCompensation {wc.aspectCompensation:0.00}, " +
                          $"holdAtStones {wc.holdShotAtStones}, drive {wc.driveCamera}");
            // The setting that silently overrides every other setting.
            sb.AppendLine($"        focus: {(wc.focus != null ? wc.focus.name : "none (following the reader)")}" +
                          (wc.focus != null
                              ? "  ← FRAMING THAT, NOT THE WALK. Every travel/read " +
                                "value is ignored while this is set."
                              : ""));
        }

        // How big is a word on screen? The number every "I can't read it" turns on.
        var tmp = All<TextMeshPro>().FirstOrDefault(t => t.GetComponentInParent<WordStone>() != null &&
                                                         !string.IsNullOrEmpty(t.text));
        if (tmp == null) { sb.AppendLine("        no word label found in the scene at all."); return; }

        // Measure from the RENDERER's world bounds. TMP_Text.bounds is in local
        // space, and multiplying it by a world-space up vector — which is what this
        // did — gives a number with no meaning: it reported a word as 2054 px tall
        // on a 1520 px screen and called it "readable".
        var rend = tmp.GetComponent<Renderer>();
        if (rend == null) { sb.AppendLine("        label has no renderer to measure."); return; }

        Bounds wb = rend.bounds;
        Vector3 lo = cam.WorldToScreenPoint(new Vector3(wb.center.x, wb.min.y, wb.center.z));
        Vector3 hi = cam.WorldToScreenPoint(new Vector3(wb.center.x, wb.max.y, wb.center.z));
        float px = (lo.z > 0f && hi.z > 0f) ? Mathf.Abs(hi.y - lo.y) : -1f;

        sb.AppendLine($"        word '{tmp.text}': {wb.size.y:0.00} m tall in world, " +
                      (px < 0f
                          ? "BEHIND THE CAMERA."
                          : $"about {px:0} px on a {cam.pixelHeight} px screen " +
                            (px < 14f ? "— TOO SMALL TO READ."
                             : px > cam.pixelHeight * 0.5f ? "— ENORMOUS, filling the frame."
                             : "— readable.")));
    }

    // ── helpers ─────────────────────────────────────────────────────────────
    static string V(Vector3 v) => $"({v.x:0.00}, {v.y:0.00}, {v.z:0.00})";
    static string Trim(string s) =>
        string.IsNullOrEmpty(s) ? "" : (s.Length > 40 ? s.Substring(0, 40) + "…" : s);

    static IEnumerable<T> All<T>() where T : Component =>
        Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);

    static Transform Find(string name)
    {
        foreach (var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
                if (t.name == name) return t;
        return null;
    }
}
