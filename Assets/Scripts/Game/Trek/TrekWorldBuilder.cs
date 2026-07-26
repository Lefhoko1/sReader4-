using System.Collections.Generic;
using UnityEngine;

namespace SReader.Game.Trek
{
    /// <summary>
    /// Builds the Trek's 2D world procedurally: parallax sky / hills / treeline,
    /// the ground path, one wooden gate per key word, a checkpoint flag and Kai.
    /// Everything is tinted quads from a shared 1×1 sprite plus the existing
    /// mascot sheet for Kai — the Phase 1 target is the mockup's wireframe;
    /// commissioned biome art swaps in at Phase 5 without touching the flow.
    ///
    /// All placement is LOCAL to the parent so the presenter can park the whole
    /// world far from the scene's other content (world map meshes live near the
    /// origin); use <see cref="WorldX"/> to convert a trail X for the camera/Kai.
    /// </summary>
    internal sealed class TrekWorldBuilder
    {
        public const float FirstGateX = 5f;
        public const float GateSpacing = 6f;
        public const float GroundY = -2.2f;

        public KaiMotor Kai { get; private set; }
        public readonly List<Transform> GateMarkers = new List<Transform>();
        Transform flag;
        Transform root;

        /// <summary>Trail-local X of a gate.</summary>
        public float GateX(int gateIndex) => FirstGateX + gateIndex * GateSpacing;
        public float TrailEndX(int gateCount) => FirstGateX + gateCount * GateSpacing + 5f;

        /// <summary>Convert a trail-local X to world space (root offset, no scale/rotation).</summary>
        public float WorldX(float localX) => root != null ? root.position.x + localX : localX;
        public float WorldGroundY => root != null ? root.position.y + GroundY : GroundY;

        /// <summary>Build the world under <paramref name="parent"/> and set up parallax against <paramref name="cam"/>.</summary>
        public void Build(Transform parent, TrailData trail, Camera cam)
        {
            root = parent;
            float endX = TrailEndX(trail.Gates.Count) + 10f;
            float startX = -12f;

            // ── Parallax layers (factor: ~0 sky … 1 world-locked) ──
            var sky = Layer(parent, "Sky", cam, 0.02f);
            Quad(sky, TrekTheme.SkyTop, (startX + endX) / 2f, 8f, endX - startX + 90, 26f, -50);

            var far = Layer(parent, "HillsFar", cam, 0.18f);
            for (float x = startX - 20; x < endX + 30; x += 9f)
                Quad(far, TrekTheme.HillFar, x, GroundY + 1.4f, 7f, 7f, -40, diamond: true);

            var near = Layer(parent, "Treeline", cam, 0.45f);
            for (float x = startX - 20; x < endX + 30; x += 4.5f)
                Quad(near, TrekTheme.HillNear, x, GroundY + 0.9f, 3.2f, 3.2f, -30, diamond: true);

            // ── Ground + path (world-locked) ──
            var ground = new GameObject("Ground").transform;
            ground.SetParent(parent, false);
            Quad(ground, TrekTheme.Ground, (startX + endX) / 2f, GroundY - 3.1f, endX - startX + 40, 6f, -20);
            Quad(ground, TrekTheme.PathTan, (startX + endX) / 2f, GroundY - 0.24f, endX - startX + 40, 0.42f, -19);
            for (float x = startX; x < endX + 10; x += 1.4f)
                Quad(ground, TrekTheme.Paper, x, GroundY - 0.24f, 0.5f, 0.08f, -18);

            // ── Gates ──
            GateMarkers.Clear();
            for (int i = 0; i < trail.Gates.Count; i++)
            {
                var g = new GameObject("Gate_" + i).transform;
                g.SetParent(parent, false);
                Quad(g, TrekTheme.GateWood, GateX(i) - 0.55f, GroundY + 0.9f, 0.22f, 1.8f, -10);
                Quad(g, TrekTheme.GateWood, GateX(i) + 0.55f, GroundY + 0.9f, 0.22f, 1.8f, -10);
                Quad(g, TrekTheme.GateWood, GateX(i), GroundY + 1.85f, 1.6f, 0.22f, -10);
                var rune = Quad(g, TrekTheme.Violet, GateX(i), GroundY + 1.0f, 0.7f, 0.7f, -9, diamond: true);
                rune.name = "Rune";
                GateMarkers.Add(g);
            }

            // ── Finish arch ──
            float fx = TrailEndX(trail.Gates.Count);
            var fin = new GameObject("Finish").transform;
            fin.SetParent(parent, false);
            Quad(fin, TrekTheme.Sun, fx - 1f, GroundY + 1.2f, 0.3f, 2.4f, -10);
            Quad(fin, TrekTheme.Sun, fx + 1f, GroundY + 1.2f, 0.3f, 2.4f, -10);
            Quad(fin, TrekTheme.Coral, fx, GroundY + 2.5f, 2.3f, 0.35f, -10);

            // ── Checkpoint flag (marks the resume point, R-9) ──
            var f = new GameObject("CheckpointFlag").transform;
            f.SetParent(parent, false);
            Quad(f, TrekTheme.Ink, 0f, 0.9f, 0.08f, 1.8f, -8);
            Quad(f, TrekTheme.Coral, 0.28f, 1.5f, 0.55f, 0.4f, -8);
            flag = f;
            f.gameObject.SetActive(false);

            // ── Kai ──
            var kai = new GameObject("Kai");
            kai.transform.SetParent(parent, false);
            kai.transform.localPosition = new Vector3(0f, GroundY, 0f);
            var sr = kai.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 10;
            var frames = LoadKaiFrames();
            if (frames.Length > 0)
            {
                kai.transform.localScale = Vector3.one * 1.5f;   // sprite pivot at feet
            }
            else
            {
                // capsule fallback so the trek runs with zero art imported
                sr.sprite = TrekTheme.SolidSprite;
                sr.color = TrekTheme.Coral;
                kai.transform.localScale = new Vector3(0.55f, 1.2f, 1f);
                kai.transform.localPosition = new Vector3(0f, GroundY + 0.6f, 0f);
            }
            Kai = kai.AddComponent<KaiMotor>();
            Kai.Init(sr, frames);
        }

        /// <summary>Put Kai at a trail-local X (used for spawn/resume placement).</summary>
        public void PlaceKai(float localX)
        {
            if (Kai == null) return;
            var p = Kai.transform.localPosition;
            p.x = localX;
            Kai.transform.localPosition = p;
        }

        /// <summary>Plant / move the checkpoint flag beside a gate (the visible resume point).</summary>
        public void PlantFlag(int lastSolvedGateIndex)
        {
            if (flag == null) return;
            if (lastSolvedGateIndex < 0) { flag.gameObject.SetActive(false); return; }
            flag.gameObject.SetActive(true);
            flag.localPosition = new Vector3(GateX(lastSolvedGateIndex) + 1.1f, GroundY, 0f);
        }

        /// <summary>Recolour a gate's rune when its state changes.</summary>
        public void PaintGate(int gateIndex, GateState state)
        {
            if (gateIndex < 0 || gateIndex >= GateMarkers.Count) return;
            var rune = GateMarkers[gateIndex].Find("Rune");
            var sr = rune != null ? rune.GetComponent<SpriteRenderer>() : null;
            if (sr == null) return;
            switch (state)
            {
                case GateState.SolvedGold:   sr.color = TrekTheme.Sun;    break;
                case GateState.SolvedSilver: sr.color = TrekTheme.Leaf;   break;
                case GateState.SolvedBronze: sr.color = TrekTheme.Leaf;   break;
                case GateState.Deferred:     sr.color = TrekTheme.Line;   break;
                default:                     sr.color = TrekTheme.Violet; break;
            }
        }

        // ── helpers ──

        static Transform Layer(Transform parent, string name, Camera cam, float factor)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var p = go.AddComponent<ParallaxLayer>();
            p.factor = factor;
            p.Init(cam.transform);
            return go.transform;
        }

        static Transform Quad(Transform parent, Color color, float x, float y, float w, float h, int order, bool diamond = false)
        {
            var go = new GameObject("q");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(x, y, 0f);
            go.transform.localScale = new Vector3(w, h, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = TrekTheme.SolidSprite;
            sr.color = color;
            sr.sortingOrder = order;
            // "diamond" hills/runes are quads rotated 45° — wireframe target only.
            if (diamond) go.transform.localRotation = Quaternion.Euler(0, 0, 45f);
            return go.transform;
        }

        static Sprite[] LoadKaiFrames()
        {
            var textures = Resources.LoadAll<Texture2D>("Art/MascotBoy");
            if (textures == null || textures.Length == 0) return new Sprite[0];
            System.Array.Sort(textures, (a, b) => string.CompareOrdinal(a.name, b.name));
            var sprites = new List<Sprite>(textures.Length);
            foreach (var t in textures)
            {
                if (t == null) continue;
                sprites.Add(Sprite.Create(t, new Rect(0, 0, t.width, t.height),
                    new Vector2(0.5f, 0f), Mathf.Max(t.width, t.height)));   // pivot at feet
            }
            return sprites.ToArray();
        }
    }
}
