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
        Nook(sb);
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

        // Which of the two walks is running. They are not alternatives you pick —
        // the reader is on a line outdoors and on a floor indoors — so "which one"
        // is really "where are they", and getting it wrong looks like the controls
        // having changed meaning for no reason.
        string walk = w.Roaming
            ? "FREE on the library floor — up/down walks the way they face, " +
              "left/right steers"
            : "along the road — up/down moves along it, left/right turns on the spot";
        sb.AppendLine("        walk: " + walk +
                      (w.roamIndoors ? "" : " (Roam Indoors is OFF)"));
        if (LibraryFloor.Known)
        {
            var f = LibraryFloor.Plan;
            sb.AppendLine($"        library floor: {f.size.x:0.0} x {f.size.z:0.0} m, " +
                          $"{(LibraryFloor.Contains(w.transform.position) ? "reader is ON it" : "reader is off it")}" +
                          Door(w));
        }
        else
        {
            sb.AppendLine("        library floor: not found — no 'Floor_*' tiles, so " +
                          "there is nowhere to walk freely and the road is all there is.");
        }

        var road = w.road;
        if (road != null && road.StopCount > 0)
        {
            float toFirst = Vector3.Distance(w.transform.position, road.PositionAt(road.StopDistance(0)));
            sb.AppendLine($"        {toFirst:0.0} m from the first word " +
                          (toFirst > 8f ? "— that is a long way; they may be starting at the wrong end." : ""));
        }
    }

    /// <summary>
    /// The way out of the library, and where it comes from.
    ///
    /// PathWalker.libraryDoor is wired when the reader first steps onto the floor,
    /// not in the scene — so it is null in edit mode however healthy the wiring is.
    /// The thing actually worth checking is the BookStation's doorway, because that
    /// is what it gets wired FROM.
    /// </summary>
    static string Door(PathWalker w)
    {
        if (w.libraryDoor != null) return $", way out via {w.libraryDoor.name}";

        var station = Object.FindAnyObjectByType<BookStation>(FindObjectsInactive.Include);
        if (station != null && station.doorway != null)
            return $", way out via {station.doorway.name} (the walker picks it up from " +
                   "the BookStation when the reader first steps inside)";

        return " — NO DOORWAY on the BookStation, so the free walk has no way out of " +
               "the room and the arrival reveal is off";
    }

    /// <summary>
    /// Is the book on the desk? They are authored in two different places — the desk
    /// in Blender, the nook in the scene — so this is the pair that silently drifts
    /// apart every time the library is re-exported, and it is invisible until you
    /// happen to look at a book hanging in mid-air.
    /// </summary>
    static void Nook(StringBuilder sb)
    {
        var station = Object.FindAnyObjectByType<BookStation>(FindObjectsInactive.Include);
        if (station == null) { sb.AppendLine("NOOK: no BookStation."); return; }

        Transform socket = null;
        foreach (var t in All<Transform>())
            if (t.name == station.deskSocketName) { socket = t; break; }

        sb.AppendLine($"NOOK: '{station.name}' at {V(station.transform.position)}, " +
                      $"reading spot {V(station.StandPosition)}, indoors {station.indoors}");

        if (socket == null)
        {
            sb.AppendLine($"      no '{station.deskSocketName}' in the scene — nothing to " +
                          "sit on, so the nook stays wherever it was put.");
        }
        else
        {
            float off = Vector3.Distance(station.transform.position, socket.position);
            sb.AppendLine($"      {station.deskSocketName} at {V(socket.position)} — " +
                          (off < 0.02f
                              ? "the nook is ON it."
                              : $"the nook is {off:0.00} m ADRIFT of it" +
                                (station.sitOnTheAuthoredDesk
                                    ? ". Sit On The Authored Desk is on, so toggling the " +
                                      "BookNook object off and on again will seat it."
                                    : ". Sit On The Authored Desk is OFF — tick it, or " +
                                      "move the nook by hand.")));
        }

        var book = station.Book;
        if (book != null)
        {
            var rends = book.GetComponentsInChildren<Renderer>(true);
            if (rends.Length > 0)
            {
                var b = rends[0].bounds;
                foreach (var r in rends) b.Encapsulate(r.bounds);
                sb.AppendLine($"      book bottom y {b.min.y:0.00}; a desk top is 2.03 — " +
                              (Mathf.Abs(b.min.y - 2.03f) < 0.12f
                                  ? "resting on it."
                                  : "NOT resting on a desk at that height."));
            }
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

        // THE SIZE OF EVERYTHING, IN ONE PLACE. "Too big" is never about one object;
        // it is the reader, the lens and the room read against each other, and each
        // of the three used to be written down somewhere different.
        float across = 2f * Mathf.Atan(Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) *
                                       cam.aspect) * Mathf.Rad2Deg;
        sb.AppendLine($"        lens: {cam.fieldOfView:0} deg tall x {across:0} deg across, " +
                      $"near clip {cam.nearClipPlane:0.00} m, far {cam.farClipPlane:0} m" +
                      (cam.nearClipPlane > 0.25f
                          ? " — NEAR CLIP IS BIG: anything closer than that is cut " +
                            "away, and indoors the reader stands within it of the desk."
                          : ""));
        if (wc != null)
        {
            sb.AppendLine($"        reader: {wc.ReaderHeight:0.00} m tall" +
                          (wc.ReaderHeight <= 0.2f ? " (could not measure them)" : "") +
                          $", eye at {wc.EyeHeight:0.00} m, shots aim at {wc.HeadHeight:0.00} m" +
                          (wc.sizeFromTheReader
                              ? " — all measured off the reader"
                              : " — from Eye Height / Head Height, NOT the reader, so " +
                                "resizing them leaves the lens where they used to be"));

            // How much of the world one screen holds at the distance things are at.
            if (walker != null && d > 0.05f)
                sb.AppendLine($"        at {d:0.0} m the frame holds " +
                              $"{2f * d * Mathf.Tan(across * 0.5f * Mathf.Deg2Rad):0.00} m across " +
                              $"and {2f * d * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad):0.00} m up");
        }
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

            // Is the eye view on, and does the camera think there is a library?
            var hall = wc.HallBounds;
            bool found = hall.size.sqrMagnitude > 0.01f;
            sb.AppendLine($"        indoors: {wc.IndoorsBlend:0.00} " +
                          (wc.IndoorsBlend > 0.5f ? "(eye view ON)"
                           : wc.IndoorsBlend > 0f ? "(easing in)"
                           : "(outdoor follow shot)"));
            // The camera only measures the hall inside Frame(), which does not run
            // in edit mode — so an empty cache here means "not looked yet", NOT
            // "no library". Saying the second when the first is true sends people
            // hunting for missing floor tiles that are sitting right there.
            if (found)
                sb.AppendLine($"        hall from '{wc.hallFloorPrefix}*': " +
                              $"x {hall.min.x:0.0}…{hall.max.x:0.0}, " +
                              $"z {hall.min.z:0.0}…{hall.max.z:0.0}, " +
                              $"y {hall.min.y:0.0}…{hall.max.y:0.0}");
            else if (LibraryFloor.Known)
            {
                var f = LibraryFloor.Plan;
                found = true;
                hall = f;
                sb.AppendLine($"        hall from '{wc.hallFloorPrefix}*': " +
                              $"x {f.min.x:0.0}…{f.max.x:0.0}, z {f.min.z:0.0}…{f.max.z:0.0}" +
                              (Application.isPlaying ? "" :
                               " (measured here — the camera works this out during play)"));
            }
            else
                sb.AppendLine($"        hall from '{wc.hallFloorPrefix}*': NOT FOUND — no " +
                              "floor tiles by that name, so the camera can never know " +
                              "the reader is inside.");

            var wk = Object.FindAnyObjectByType<PathWalker>(FindObjectsInactive.Include);
            if (found && wk != null)
            {
                Vector3 f = wk.transform.position;
                bool within = f.x > hall.min.x && f.x < hall.max.x &&
                              f.z > hall.min.z && f.z < hall.max.z;
                sb.AppendLine($"        reader {V(f)} is {(within ? "INSIDE" : "outside")} " +
                              $"that footprint" +
                              (within && wc.IndoorsBlend <= 0f
                                  ? " — but indoors reads 0, so the height test is " +
                                    "rejecting it."
                                  : ""));

                // A partial blend indoors is its own failure and does not look like
                // one: the camera is neither the eye nor the boom but half of each,
                // which is a position no shot ever asked for.
                if (within && wc.IndoorsBlend > 0.02f && wc.IndoorsBlend < 0.98f)
                {
                    float slack = Mathf.Min(hall.extents.x - Mathf.Abs(f.x - hall.center.x),
                                            hall.extents.z - Mathf.Abs(f.z - hall.center.z));
                    sb.AppendLine($"        ^ only {wc.IndoorsBlend:0.00} indoors while " +
                                  $"standing {slack:0.00} m in from the nearest wall. " +
                                  $"Threshold Blend is {wc.thresholdBlend:0.00} m, and " +
                                  "that ramp is measured from the wall — so it is also " +
                                  "the width of the band where the reader half counts " +
                                  "as outside. Anything near a metre leaves no fully " +
                                  "indoor spot in a room this size; 0.35 is the " +
                                  "authored value. (Values over 0.6 are held at 0.6.)");
                }
            }

            // THE ONE THAT ANSWERS "WHY CAN I SEE OUTSIDE FROM INSIDE". A shot is an
            // interior only if the LENS is in the room; everything else — the reader
            // being inside, the room being sealed — is beside the point if the camera
            // solved its distance out on the porch.
            if (found)
            {
                sb.AppendLine($"        shot is an interior: {wc.ShotInsideBlend:0.00} " +
                              (wc.ShotInsideBlend > 0f
                                  ? $"(walls closed for the camera, hall height " +
                                    $"{wc.hallHeight:0.0} m, ceiling y {wc.HallCeilingY:0.0})"
                                  : "(open air — no wall is holding the lens in)"));

                Vector3 c = cam.transform.position;
                bool lensIn = c.x > hall.min.x && c.x < hall.max.x &&
                              c.z > hall.min.z && c.z < hall.max.z &&
                              c.y > hall.max.y && c.y < wc.HallCeilingY;
                sb.AppendLine($"        LENS is {(lensIn ? "INSIDE the room" : "OUTSIDE the room")}" +
                              (wc.ShotInsideBlend > 0.5f && !lensIn
                                  ? " — the shot says interior but the camera is not in " +
                                    "it. Untick nothing; this is a bug."
                                  : "") +
                              (wc.PushedBackIn > 0.01f
                                  ? $"; the shot asked to be {wc.PushedBackIn:0.00} m " +
                                    "further out and was refused"
                                  : ""));
                if (!wc.stayInsideTheHall)
                    sb.AppendLine("        ^ Stay Inside The Hall is OFF, so nothing is " +
                                  "stopping the lens leaving the building.");
            }
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
