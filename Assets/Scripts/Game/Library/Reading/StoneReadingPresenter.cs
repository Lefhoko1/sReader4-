using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using SReader.Domains.Assignments.Models;
using SReader.Game.Trek;                 // GateSolving — the SAME checking the real game uses
using SReader.Game.Library.Feel;         // FeedbackDirector — the restoration "juice" (optional)
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SReader.Game.Library.Reading
{
    /// <summary>
    /// SAMPLE — the "Word-Path" reading level from Docs/ultimateDesign.png (screen 2).
    ///
    /// A sentence is laid out as a winding trail of flat STEPPING STONES — one word
    /// per stone, meandering forward toward the library. Every word is readable (you
    /// walk the sentence); the tutor's KEY words glow gold. Tapping a glowing stone
    /// opens its action — Fill / Define / Illustrate — and solving it lights the
    /// stone and plays the restoration grammar. When every key word on the path is
    /// solved the trail resolves and the next sentence's path rises.
    ///
    /// REUSE, not rewrite: answer-checking is your tested <see cref="GateSolving"/>,
    /// and the content is the EXISTING <see cref="AssignmentContent"/> shape (the
    /// thing your Supabase code already returns). Feed real data via <see cref="SetContent"/>;
    /// scoring/persistence route through TrekSession + StudentAssignmentsViewModel later.
    ///
    /// Runs today on primitive discs so nothing waits on art — drop your Blender
    /// stepping-stone into <see cref="stonePrefab"/> and the same code uses it. Drag
    /// the "StoneReading (Sample)" object to move/rotate the whole path.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class StoneReadingPresenter : MonoBehaviour
    {
        [Header("Optional art (falls back to primitives so it runs now)")]
        [Tooltip("Your Blender stepping-stone prefab. Leave empty to use a generated disc.")]
        [SerializeField] GameObject stonePrefab;
        [Tooltip("Camera to read from. Empty = Camera.main.")]
        [SerializeField] Camera readCamera;
        [Tooltip("Frame the camera on Start so the whole path is visible (good for testing in isolation).")]
        [SerializeField] bool seatCameraOnStart = true;
        [Tooltip("Ride the camera behind the walking reader. Off = one static wide shot with everything visible.")]
        [SerializeField] bool followPawn = false;
        [Tooltip("Play the rise / walk / bob animations. OFF = everything appears in place and holds still, so you can judge the layout.")]
        [SerializeField] bool animate = false;
        [Tooltip("Texture the stones with the rock photo. OFF = flat solid colours.")]
        [SerializeField] bool useStoneTexture = false;
        [Tooltip("Raise the whole path (metres) if stones sink into the terrain/water.")]
        [SerializeField] float pathLift = 0f;

        [Header("Camera framing (tweak live in Play)")]
        [Tooltip("How far back behind the first stone the camera sits.")]
        [SerializeField] float camBack = 3.0f;
        [Tooltip("Sideways offset. Positive = the reader enters from the bottom-LEFT.")]
        [SerializeField] float camSide = 3.0f;
        [Tooltip("Camera height above the water.")]
        [SerializeField] float camHeight = 2.0f;
        [Tooltip("Field of view. LOWER = flatter, less 'scaled'/distorted perspective.")]
        [SerializeField] float camFov = 34f;

        [Header("Path (a winding stepping-stone trail, per ultimateDesign.png)")]
        [SerializeField] float stepGap = 1.5f;           // forward spacing between stones
        [SerializeField] float wanderAmp = 1.0f;         // how far the path meanders sideways
        [SerializeField] float wanderFreq = 0.8f;        // how quickly it snakes
        [Tooltip("Stone size. LOWER = smaller pads with more water between them. This is your 'scale down' control.")]
        [SerializeField] float stoneRadius = 0.5f;       // flat pad radius
        [SerializeField] float labelHeight = 0.5f;       // word height above its stone

        [Header("Palette (Bible Ch. 3.2)")]
        [SerializeField] Color stonePlain   = new Color(0.62f, 0.60f, 0.55f);
        [SerializeField] Color stoneForgot  = new Color(0.28f, 0.25f, 0.22f);
        [SerializeField] Color glowGold     = new Color(0.95f, 0.70f, 0.30f);
        [SerializeField] Color inkColor     = new Color(0.12f, 0.10f, 0.09f);
        [SerializeField] Color restoredInk  = new Color(1f, 0.92f, 0.6f);

        [Header("Content")]
        [Tooltip("Use the built-in sample passage. Uncheck and call SetContent() for real tutor content.")]
        [SerializeField] bool useSampleContent = true;

        // ── runtime ───────────────────────────────────────────────────────────
        readonly List<WordStone> stones = new List<WordStone>();
        readonly List<Chip> chips = new List<Chip>();
        readonly List<string> answer = new List<string>();
        List<string> correct;

        AssignmentContent content;
        List<ContentSentence> sentences;
        int sentenceIndex = -1;
        WordStone activeStone;
        bool ritualOpen, framed;
        int restoredTotal, keywordTotal;
        Material glowMat;
        Texture2D stoneTex;
        float pulse;

        // Sample the rocky centre of the boulder photo (skip its white border) and
        // repeat it so each pad reads as carved stone.
        static readonly Vector2 TexScale = new Vector2(0.55f, 0.55f);
        static readonly Vector2 TexOffset = new Vector2(0.22f, 0.22f);

        GameObject pawn;                         // the reader who walks the sentence
        int pawnIndex;
        const float PawnLift = 0.5f;

        sealed class WordStone
        {
            public GameObject go;
            public Renderer rend;
            public TextMeshPro label;
            public ContentToken token;          // non-null only for a key word
            public bool IsKeyword => token != null && token.IsActivity;
            public bool restored;
            public Vector3 home;
            public float rise;
        }

        sealed class Chip
        {
            public GameObject go;
            public TextMeshPro label;
            public string value;
            public Vector3 basePos;
            public float bobSeed;
        }

        // ── entry ─────────────────────────────────────────────────────────────

        public void SetContent(AssignmentContent c)
        {
            content = c;
            sentences = c?.pages?.SelectMany(p => p.sentences).ToList() ?? new List<ContentSentence>();
            keywordTotal = sentences.SelectMany(s => s.tokens).Count(t => t.IsActivity);
            restoredTotal = 0;
            sentenceIndex = -1;
            framed = false;
            NextSentence();
        }

        void Start()
        {
            MuteArrival();                                        // no fly-in hijacking the camera during the test
            if (readCamera == null) readCamera = Camera.main;
            stoneTex = useStoneTexture ? Resources.Load<Texture2D>("T_StonePad") : null;   // off = flat solid colours
            EnsureGlowMat();
            if (content == null && useSampleContent) SetContent(BuildSampleContent());
            else if (content != null) NextSentence();
        }

        // ── path flow ─────────────────────────────────────────────────────────

        void NextSentence()
        {
            ClearStones();
            sentenceIndex++;
            if (sentences == null || sentenceIndex >= sentences.Count) { Finish(); return; }

            var words = sentences[sentenceIndex].tokens.Where(t => t.isWord).ToList();
            for (int i = 0; i < words.Count; i++)
            {
                // winding trail: march forward (+z), snake sideways (x)
                float z = i * stepGap;
                float x = Mathf.Sin(i * wanderFreq) * wanderAmp;
                var home = transform.TransformPoint(new Vector3(x, 0.08f + pathLift, z));
                stones.Add(MakeStone(words[i], home));
            }
            EnsurePawn();
            pawnIndex = 0;
            pawn.transform.position = stones[0].home + Vector3.up * PawnLift;
            if (seatCameraOnStart && !framed) { framed = true; FrameCamera(); }
            SetNextActiveKeyword();
        }

        // Static shot: sit low and OFF TO THE SIDE, looking across the water so the
        // reader enters from the bottom-left and the path leads to the library. A
        // lower FOV flattens perspective so near stones don't look blown-up.
        void FrameCamera()
        {
            if (readCamera == null || stones.Count == 0) return;
            Vector3 a = stones[0].home, b = stones[stones.Count - 1].home;
            Vector3 fwd = b - a; fwd.y = 0f;
            if (fwd.sqrMagnitude < 0.01f) fwd = transform.forward;
            fwd.Normalize();
            Vector3 right = Vector3.Cross(Vector3.up, fwd).normalized;   // +camSide pushes the path left of frame
            float len = Vector3.Distance(a, b);
            readCamera.fieldOfView = camFov;
            readCamera.transform.position = a - fwd * camBack + right * camSide + Vector3.up * (camHeight + len * 0.05f);
            Vector3 look = Vector3.Lerp(a, b, 0.55f) + Vector3.up * 0.2f;
            readCamera.transform.rotation = Quaternion.LookRotation(look - readCamera.transform.position);
        }

        // Turn off the IslandFlow arrival + its demo canvases so pressing Play shows
        // the scene as-is instead of dollying the camera in.
        void MuteArrival()
        {
            foreach (var n in new[] { "IslandFlow", "FlowCanvas", "ReadingCanvas" })
            {
                var go = GameObject.Find(n);
                if (go != null) go.SetActive(false);
            }
        }

        void SetNextActiveKeyword()
        {
            activeStone = stones.FirstOrDefault(s => s.IsKeyword && !s.restored);
            if (activeStone == null)
            {
                if (animate) StartCoroutine(WalkPawnTo(stones.Count - 1, () => StartCoroutine(ResolveSentence())));
                else StartCoroutine(ResolveSentence());
                return;
            }
            if (animate)
            {
                int idx = stones.IndexOf(activeStone);
                StartCoroutine(WalkPawnTo(idx, () => OpenRitual(activeStone)));   // walk up to it, then present the challenge
            }
            // static mode: the pawn waits at the start and the glowing stone is tapped to solve
        }

        // ── the walking reader ─────────────────────────────────────────────────

        void EnsurePawn()
        {
            if (pawn != null) return;
            pawn = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            pawn.name = "Reader";
            var col = pawn.GetComponent<Collider>();
            if (col != null) Destroy(col);                    // never intercept taps
            pawn.transform.SetParent(transform, true);
            pawn.transform.localScale = new Vector3(0.32f, 0.4f, 0.32f);
            var m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            m.SetColor("_BaseColor", new Color(0.24f, 0.42f, 0.72f));   // adventurer blue (placeholder for the boy)
            pawn.GetComponent<Renderer>().sharedMaterial = m;
        }

        System.Collections.IEnumerator WalkPawnTo(int target, System.Action onArrive)
        {
            while (pawnIndex < target && pawnIndex + 1 < stones.Count)
            {
                yield return Hop(stones[pawnIndex].home, stones[pawnIndex + 1].home);
                pawnIndex++;
                yield return new WaitForSeconds(0.12f);       // a beat to read each word
            }
            onArrive?.Invoke();
        }

        System.Collections.IEnumerator Hop(Vector3 from, Vector3 to)
        {
            Vector3 a = from + Vector3.up * PawnLift, b = to + Vector3.up * PawnLift;
            float e = 0f, d = 0.34f;
            while (e < d)
            {
                e += Time.deltaTime;
                float k = e / d;
                var p = Vector3.Lerp(a, b, k);
                p.y += Mathf.Sin(k * Mathf.PI) * 0.45f;       // little hop arc
                if (pawn) pawn.transform.position = p;
                yield return null;
            }
            if (pawn) pawn.transform.position = b;
        }

        void FollowPawn()
        {
            if (pawn == null || readCamera == null) return;
            Vector3 fwd = PathForward(pawnIndex);
            Vector3 desired = pawn.transform.position - fwd * 3.0f + Vector3.up * 2.1f;
            Vector3 look = pawn.transform.position + fwd * 1.5f + Vector3.up * 0.2f;
            readCamera.transform.position = Vector3.Lerp(readCamera.transform.position, desired, Time.deltaTime * 2.5f);
            readCamera.transform.rotation = Quaternion.Slerp(readCamera.transform.rotation,
                Quaternion.LookRotation(look - readCamera.transform.position), Time.deltaTime * 2.5f);
        }

        Vector3 PathForward(int i)
        {
            if (stones.Count < 2) return transform.forward;
            int a = Mathf.Clamp(i, 0, stones.Count - 2);
            Vector3 f = stones[a + 1].home - stones[a].home; f.y = 0f;
            return f.sqrMagnitude < 0.01f ? transform.forward : f.normalized;
        }

        System.Collections.IEnumerator ResolveSentence()
        {
            foreach (var s in stones) { Flash(s); yield return new WaitForSeconds(0.05f); }
            if (FeedbackDirector.Instance != null) FeedbackDirector.Instance.PlayCompletion();
            yield return new WaitForSeconds(0.5f);
            if (animate)
            {
                float t = 0f;
                while (t < 0.4f)
                {
                    t += Time.deltaTime;
                    foreach (var s in stones) if (s.go) s.go.transform.position = Vector3.Lerp(s.home, s.home - Vector3.up * 1.2f, t / 0.4f);
                    yield return null;
                }
            }
            NextSentence();
        }

        void Finish()
        {
            var go = new GameObject("ReadingDone");
            var t = go.AddComponent<TextMeshPro>();
            t.text = restoredTotal >= keywordTotal && keywordTotal > 0 ? "The page remembers." : "The path rests.";
            t.fontSize = 3; t.alignment = TextAlignmentOptions.Center; t.color = restoredInk;
            go.transform.position = transform.position + Vector3.up * 1.4f;
            Billboard(go.transform);
        }

        // ── the restoration ritual (in-world, not a page) ──────────────────────

        void OpenRitual(WordStone stone)
        {
            ritualOpen = true;
            answer.Clear();
            var token = stone.token;
            var type = MapType(token.activity);
            List<string> pool;
            switch (type)
            {
                case GateType.Fill:   correct = GateSolving.FillCorrect(token);   pool = GateSolving.FillPool(token);   break;
                case GateType.Define: correct = GateSolving.DefineCorrect(token); pool = GateSolving.DefinePool(token); break;
                default:              correct = new List<string> { token.correctImage }; pool = GateSolving.IllustrateOptions(token); break;
            }
            int n = pool.Count;
            float w = (n - 1) * 0.7f;
            for (int i = 0; i < n; i++)
            {
                var pos = stone.home + new Vector3(-w * 0.5f + i * 0.7f, 1.1f, -0.2f);
                chips.Add(MakeChip(pool[i], pos));
            }
        }

        void OnChipTapped(Chip chip, GateGuess guess)
        {
            if (guess == GateGuess.Illustrate)
            {
                if (GateSolving.CheckIllustrate(chip.value, activeStone.token)) RestoreActive();
                else Reject(chip);
                return;
            }
            answer.Add(chip.value);
            RemoveChip(chip);
            bool okSoFar = !answer.Where((a, i) => i < correct.Count &&
                !string.Equals(a, correct[i], System.StringComparison.OrdinalIgnoreCase)).Any();
            if (!okSoFar) { RejectSequence(); return; }
            bool complete = answer.Count == correct.Count &&
                (guess == GateGuess.Fill ? GateSolving.CheckFill(answer, correct) : GateSolving.CheckDefine(answer, correct));
            if (complete) RestoreActive();
        }

        void RestoreActive()
        {
            var s = activeStone;
            s.restored = true;
            restoredTotal++;
            s.label.color = restoredInk;
            s.label.text = s.token.text + "  ★";
            SetStone(s.rend, Color.Lerp(stonePlain, glowGold, 0.5f), glowGold * 0.45f);
            Pop(s.go.transform);
            if (FeedbackDirector.Instance != null)
                FeedbackDirector.Instance.PlayCorrect(new FeedbackContext(s.token.text, s.go.transform.position, s.token.text));
            ClearChips();
            ritualOpen = false;
            SetNextActiveKeyword();
        }

        void Reject(Chip chip)
        {
            if (FeedbackDirector.Instance != null)
                FeedbackDirector.Instance.PlayIncorrect(new FeedbackContext(activeStone.token.text, chip.go.transform.position, activeStone.token.text));
            StartCoroutine(Shake(chip.go.transform));
        }

        void RejectSequence()
        {
            if (FeedbackDirector.Instance != null)
                FeedbackDirector.Instance.PlayIncorrect(new FeedbackContext(activeStone.token.text, activeStone.go.transform.position, activeStone.token.text));
            if (pawn != null) StartCoroutine(Shake(pawn.transform));   // gentle teeter — the "almost fell in" beat, recoverable
            ClearChips();
            OpenRitual(activeStone);
        }

        // ── loop + input ────────────────────────────────────────────────────────

        void Update()
        {
            pulse = (Mathf.Sin(Time.time * 3f) + 1f) * 0.5f;
            if (glowMat != null) glowMat.SetColor("_EmissionColor", glowGold * (0.15f + pulse * 0.45f));

            foreach (var s in stones)
            {
                if (s.go == null) continue;
                if (s.rise < 1f) { s.rise = Mathf.Min(1f, s.rise + Time.deltaTime * 2.2f); s.go.transform.position = Vector3.Lerp(s.home - Vector3.up, s.home, EaseOut(s.rise)); }
                if (s.label != null) { s.label.transform.position = s.go.transform.position + Vector3.up * labelHeight; Billboard(s.label.transform); }
            }
            foreach (var c in chips)
            {
                if (c.go == null) continue;
                if (animate) c.go.transform.position = c.basePos + Vector3.up * Mathf.Sin(Time.time * 2f + c.bobSeed) * 0.06f;
                if (c.label != null)
                {
                    // pull the label toward the camera so the word sits IN FRONT of the
                    // card face instead of buried inside the cube
                    Vector3 toCam = readCamera != null ? (readCamera.transform.position - c.go.transform.position).normalized : Vector3.back;
                    c.label.transform.position = c.go.transform.position + toCam * 0.12f;
                    Billboard(c.label.transform);
                }
            }

            if (seatCameraOnStart) { if (followPawn) FollowPawn(); else FrameCamera(); }   // FrameCamera each frame = live slider tuning
            if (TryGetTap(out var pos)) HandleTap(pos);
        }

        void HandleTap(Vector2 screenPos)
        {
            if (readCamera == null) return;
            if (!Physics.Raycast(readCamera.ScreenPointToRay(screenPos), out var hit, 100f)) return;
            if (ritualOpen)
            {
                var chip = chips.FirstOrDefault(c => c.go == hit.collider.gameObject);
                if (chip != null) OnChipTapped(chip, GuessFor(activeStone.token.activity));
            }
            else if (activeStone != null && hit.collider.gameObject == activeStone.go)
            {
                OpenRitual(activeStone);
            }
        }

        bool TryGetTap(out Vector2 pos)
        {
            pos = default;
#if ENABLE_INPUT_SYSTEM
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            { pos = Touchscreen.current.primaryTouch.position.ReadValue(); return true; }
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            { pos = Mouse.current.position.ReadValue(); return true; }
            return false;
#else
            if (Input.GetMouseButtonDown(0)) { pos = Input.mousePosition; return true; }
            return false;
#endif
        }

        // ── factories ────────────────────────────────────────────────────────────

        WordStone MakeStone(ContentToken tok, Vector3 home)
        {
            var go = stonePrefab != null ? Instantiate(stonePrefab) : GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = "Stone_" + tok.text;
            go.transform.SetParent(transform, true);
            go.transform.localScale = new Vector3(stoneRadius, 0.12f, stoneRadius);   // flat stepping pad
            go.transform.position = animate ? home - Vector3.up : home;               // static = appear in place
            if (go.GetComponent<Collider>() == null) go.AddComponent<CapsuleCollider>();

            var rend = go.GetComponentInChildren<Renderer>();
            bool keyword = tok.IsActivity;
            if (keyword) rend.sharedMaterial = glowMat;                                // shared pulsing gold
            else SetStone(rend, stonePlain, Color.black);

            var label = MakeLabel(tok.text, keyword ? glowGold : inkColor);
            return new WordStone { go = go, rend = rend, label = label, token = keyword ? tok : null, home = home, rise = animate ? 0f : 1f };
        }

        Chip MakeChip(string value, Vector3 pos)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Chip_" + value;
            go.transform.SetParent(transform, true);
            go.transform.localScale = new Vector3(0.55f, 0.55f, 0.1f);
            go.transform.position = pos;
            SetStone(go.GetComponent<Renderer>(), new Color(0.96f, 0.93f, 0.82f), glowGold * 0.2f);
            var label = MakeLabel(value, inkColor);
            return new Chip { go = go, label = label, value = value, basePos = pos, bobSeed = Random.value * 6f };
        }

        // Labels are standalone (never parented to a scaled stone, so text isn't
        // distorted); Update keeps each one hovering above its object, facing the camera.
        TextMeshPro MakeLabel(string text, Color color)
        {
            var go = new GameObject("Label");
            go.transform.SetParent(transform, false);
            var t = go.AddComponent<TextMeshPro>();
            t.text = text; t.color = color; t.alignment = TextAlignmentOptions.Center;
            t.enableAutoSizing = true; t.fontSizeMin = 0.3f; t.fontSizeMax = 1.8f;
            t.GetComponent<RectTransform>().sizeDelta = new Vector2(2.6f, 1f);
            return t;
        }

        // ── helpers ────────────────────────────────────────────────────────────

        void EnsureGlowMat()
        {
            glowMat = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "StoneGlow" };
            ApplyStoneTex(glowMat, Color.Lerp(stoneForgot, Color.white, 0.35f));
            glowMat.EnableKeyword("_EMISSION");
            glowMat.SetColor("_EmissionColor", glowGold * 0.4f);
        }

        void SetStone(Renderer rend, Color baseCol, Color emission)
        {
            if (rend == null) return;
            var m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            ApplyStoneTex(m, baseCol);
            if (emission.maxColorComponent > 0.01f) { m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor", emission); }
            rend.sharedMaterial = m;
        }

        // Put the rock photo on _BaseMap (centre-sampled), tinting toward white so the
        // stone reads instead of being crushed dark. Falls back to a flat tint if the
        // texture didn't import.
        void ApplyStoneTex(Material m, Color tint)
        {
            if (stoneTex != null)
            {
                m.SetTexture("_BaseMap", stoneTex);
                m.SetTextureScale("_BaseMap", TexScale);
                m.SetTextureOffset("_BaseMap", TexOffset);
                m.SetColor("_BaseColor", Color.Lerp(tint, Color.white, 0.6f));
            }
            else m.SetColor("_BaseColor", tint);
            m.SetFloat("_Smoothness", 0.12f);
        }

        void Flash(WordStone s)
        {
            if (s?.rend == null) return;
            var m = s.rend.sharedMaterial;
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", glowGold * 0.85f);
        }

        void Pop(Transform t) => StartCoroutine(PopCo(t));
        System.Collections.IEnumerator PopCo(Transform t)
        {
            Vector3 s = t.localScale; float e = 0f;
            while (e < 0.25f) { e += Time.deltaTime; t.localScale = s * (1f + Mathf.Sin(e / 0.25f * Mathf.PI) * 0.18f); yield return null; }
            t.localScale = s;
        }

        System.Collections.IEnumerator Shake(Transform t)
        {
            Vector3 p = t.position; float e = 0f;
            while (e < 0.3f) { e += Time.deltaTime; t.position = p + t.right * Mathf.Sin(e * 60f) * 0.05f; yield return null; }
            t.position = p;
        }

        void Billboard(Transform t)
        {
            if (readCamera == null) return;
            t.rotation = Quaternion.LookRotation(t.position - readCamera.transform.position);
        }

        void RemoveChip(Chip c) { chips.Remove(c); if (c.label) Destroy(c.label.gameObject); if (c.go) Destroy(c.go); }
        void ClearChips() { foreach (var c in chips) { if (c.label) Destroy(c.label.gameObject); if (c.go) Destroy(c.go); } chips.Clear(); }
        void ClearStones() { foreach (var s in stones) { if (s.label) Destroy(s.label.gameObject); if (s.go) Destroy(s.go); } stones.Clear(); ClearChips(); }

        static float EaseOut(float x) => 1f - (1f - x) * (1f - x);
        static GateType MapType(ActivityType a) => a == ActivityType.Define ? GateType.Define : a == ActivityType.Illustrate ? GateType.Illustrate : GateType.Fill;
        enum GateGuess { Fill, Define, Illustrate }
        static GateGuess GuessFor(ActivityType a) => a == ActivityType.Define ? GateGuess.Define : a == ActivityType.Illustrate ? GateGuess.Illustrate : GateGuess.Fill;

        // ── sample passage (mirrors the Supabase content shape exactly) ──────────

        AssignmentContent BuildSampleContent()
        {
            var c = new AssignmentContent();
            var page = new ContentPage { pageNumber = 1 };
            page.sentences.Add(Sentence(
                W("The"), W("fox"), W("lived"), W("near"), W("a"), W("quiet"),
                Key("river", ActivityType.Define, definition: "a large natural stream of water")));
            page.sentences.Add(Sentence(
                W("Every"), W("morning"), W("he"), W("went"), W("out"), W("to"),
                Key("explore", ActivityType.FillBlank)));
            page.sentences.Add(Sentence(
                W("He"), W("loved"), W("to"), W("find"),
                Key("new", ActivityType.Illustrate,
                    images: new[] { "a bright sunrise", "a locked door", "an old shoe", "a grey wall" },
                    correct: "a bright sunrise"),
                W("things")));
            c.pages.Add(page);
            return c;
        }

        static ContentSentence Sentence(params ContentToken[] toks)
        { var s = new ContentSentence(); s.tokens.AddRange(toks); return s; }

        static ContentToken W(string text) => new ContentToken { text = text, isWord = true, activity = ActivityType.None };

        static ContentToken Key(string text, ActivityType a, string definition = null, string[] images = null, string correct = null)
        {
            var t = new ContentToken { text = text, isWord = true, activity = a, points = 10, definition = definition };
            if (images != null) { t.imageOptions = images.ToList(); t.correctImage = correct; }
            return t;
        }
    }
}
