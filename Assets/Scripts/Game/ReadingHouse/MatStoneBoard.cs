using System;
using System.Collections.Generic;
using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The sentence as Word Path stones on the rug in front of the dashboard — the same stones and
    /// WordStone script as the Word Path (SM_WordStone_A/B/C, and SM_WordStone_Key for key words with
    /// their gold aura and chime; a solved word gets its star). Laid out in reading order in rows across
    /// the rug, the first row nearest the dashboard, kept clear of where the student stands; each stone is
    /// sized to its word, and they rise out of the mat (WordStonePool / StoneSlot). Presentation only:
    /// the presenter says which words are key / solved / next and gets told which word was tapped.
    /// </summary>
    public sealed class MatStoneBoard : MonoBehaviour
    {
        [Header("Stones (the Word Path prefabs)")]
        public GameObject[] stonePrefabs = new GameObject[0];
        public GameObject keyPrefab;

        [Header("Where")]
        [Tooltip("The rug the stones lie on.")]
        public Renderer mat;
        [Tooltip("The student's reading spot: the rows stop short of it.")]
        public Transform student;
        public Camera viewCamera;
        [Tooltip("What the rows face: the dashboard's display (rows run across it, reading order away from it).")]
        public Transform dashboard;
        public float edgeMargin = 0.28f;
        public float rowSpacing = 0.62f;
        [Tooltip("Gap kept between the first row and the dashboard end of the rug (so the dashboard's panels never hide it).")]
        public float dashboardInset = 0.75f;
        public int maxRows = 3;
        [Tooltip("Space kept clear in front of the student (metres).")]
        public float studentClearance = 0.4f;
        public float gap = 0.18f;

        [Header("Size")]
        [Tooltip("Every stone is this wide (metres): neat rows, like game tiles.")]
        public float stoneWidth = 0.66f;
        [Tooltip("Size of the words printed on the stones (WordStone.labelScale); long words are printed smaller to fit.")]
        public float labelScale = 1.0f;
        [Tooltip("Stand the words up facing the camera (easier to read than lying flat at this angle).")]
        public bool wordsStandUp = true;

        readonly WordStonePool pool = new WordStonePool();
        readonly List<WordStone> live = new List<WordStone>();
        readonly Dictionary<WordStone, int> index = new Dictionary<WordStone, int>();
        readonly Dictionary<Mesh, float> unitWidths = new Dictionary<Mesh, float>();

        /// <summary>The words this sentence page shows (absolute word index → stone).</summary>
        public IReadOnlyList<WordStone> Live => live;

        void Awake() => WordStone.InputLocked = true;          // the house's input routes taps (WordStone's own camera lookup is not used)
        void OnDestroy() => WordStone.InputLocked = false;

        Vector3 TowardDashboard => dashboard != null
            ? Vector3.ProjectOnPlane(dashboard.position - mat.bounds.center, Vector3.up).normalized
            : Vector3.ProjectOnPlane(viewCamera.transform.forward, Vector3.up).normalized;
        Vector3 Right => Vector3.Cross(Vector3.up, TowardDashboard).normalized;

        float StoneWidth(string word) => stoneWidth;
        /// <summary>Label size for a word: as large as labelScale allows while it still fits on its stone.</summary>
        float LabelFor(string word) => labelScale;

        /// <summary>Row centres along the rug, far (dashboard side) to near, stopping short of the student.</summary>
        List<Vector3> Rows(out float halfWidth)
        {
            var b = mat.bounds;
            var fwd = TowardDashboard; var right = Right;
            halfWidth = Mathf.Abs(Vector3.Dot(b.extents, new Vector3(Mathf.Abs(right.x), 0, Mathf.Abs(right.z)))) - edgeMargin;
            float depth = Mathf.Abs(Vector3.Dot(b.extents, new Vector3(Mathf.Abs(fwd.x), 0, Mathf.Abs(fwd.z))));
            var far = b.center + fwd * (depth - dashboardInset);
            var rows = new List<Vector3>();
            float studentAlong = student != null ? Vector3.Dot(student.position - b.center, -fwd) : float.MaxValue;
            for (int r = 0; r < maxRows; r++)
            {
                var c = far - fwd * (r * rowSpacing);
                if (Vector3.Dot(c - b.center, -fwd) > studentAlong - studentClearance) break;
                c.y = b.max.y;
                rows.Add(c);
            }
            return rows;
        }

        /// <summary>How many words, from <paramref name="start"/>, fit on the rug at once (at least one).</summary>
        public int FitCount(IReadOnlyList<string> words, int start)
        {
            var rows = Rows(out float half);
            int row = 0, n = 0; float x = 0f;
            for (int i = start; i < words.Count; i++)
            {
                float w = StoneWidth(words[i]);
                if (x > 0f && x + w > half * 2f) { row++; x = 0f; }
                if (row >= rows.Count) break;
                x += w + gap; n++;
            }
            return Mathf.Max(1, n);
        }

        /// <summary>Lay words [start, end) on the rug. isKey / isSolved take the absolute word index.</summary>
        public void Show(IReadOnlyList<string> words, int start, int end, Func<int, bool> isKey, Func<int, bool> isSolved, Func<int, bool> isNext)
        {
            live.Clear(); index.Clear();
            var variants = stonePrefabs.Length > 0 ? stonePrefabs : new[] { keyPrefab };
            if (pool.ParentChanged(transform)) pool.Dispose();
            pool.EnsureCapacity(Mathf.Max(end - start, 1), transform, variants, keyPrefab);
            pool.Begin();

            // Lay out the rows first (each row centred), then raise a stone in every place.
            var rows = Rows(out float half);
            var places = new List<(int word, int row, float x, float w)>();
            int r = 0; float x = 0f;
            var rowWidth = new List<float> { 0f };
            for (int i = start; i < end; i++)
            {
                float w = StoneWidth(words[i]);
                if (x > 0f && x + w > half * 2f) { rowWidth[r] = x - gap; r++; x = 0f; rowWidth.Add(0f); }
                if (r >= rows.Count) break;
                places.Add((i, r, x, w));
                x += w + gap;
            }
            if (r < rowWidth.Count) rowWidth[r] = x - gap;

            var right = Right;
            var face = Quaternion.LookRotation(-TowardDashboard, Vector3.up);
            foreach (var (word, row, px, w) in places)
            {
                var ws = pool.Take(words[word], isKey(word), false, true);
                if (ws == null) break;
                ws.labelScale = LabelFor(words[word]);
                ws.maxLabelWidth = w + gap * 0.6f;
                ws.labelAboveSurface = true;
                ws.labelCamera = viewCamera;
                ws.wordHeight = 0.075f;
                ws.textColor = new Color32(24, 38, 63, 255);
                ws.keywordTextColor = new Color32(42, 34, 19, 255);
                ws.layFlat = !wordsStandUp;
                ws.suppressCard = true;
                ws.RefreshLabelStyle();
                var slot = ws.GetComponentInParent<StoneSlot>();
                float scale = w / Mathf.Max(0.05f, UnitWidth(ws));                 // each stone variant measured on its own
                var centre = rows[row] + right * (px + w * 0.5f - rowWidth[row] * 0.5f);
                slot.Place(centre, face, Vector3.one * scale);
                if (isSolved(word)) ws.MarkSolved();
                live.Add(ws); index[ws] = word;
            }
            pool.EndBuild();
        }

        void LateUpdate()
        {
            // The student walks over the rug: the stones' colliders (made by WordStone) are triggers — taps still find them.
            foreach (var ws in live)
                if (ws != null)
                    foreach (var c in ws.GetComponentsInChildren<Collider>())
                        if (!c.isTrigger) c.isTrigger = true;
        }

        public void HideAll()
        {
            pool.SinkAll();
            live.Clear(); index.Clear();
        }

        /// <summary>The absolute word index a tapped stone carries (false if it is not one of ours).</summary>
        public bool TryGetWord(WordStone stone, out int wordIndex) => index.TryGetValue(stone, out wordIndex);

        float UnitWidth(WordStone ws)
        {
            var mf = ws.GetComponentInChildren<MeshFilter>();
            var mesh = mf != null ? mf.sharedMesh : null;
            if (mesh != null && unitWidths.TryGetValue(mesh, out float w)) return w;
            w = MeasureUnit(ws);
            if (mesh != null) unitWidths[mesh] = w;
            return w;
        }

        static float MeasureUnit(WordStone ws)
        {
            var t = ws.transform;
            var saved = t.localScale; t.localScale = Vector3.one;
            float w = 0.5f;
            var rends = ws.GetComponentsInChildren<Renderer>(true);
            foreach (var rd in rends)
                if (!(rd is ParticleSystemRenderer) && rd.GetComponent<TMPro.TextMeshPro>() == null)
                { var s = rd.bounds.size; w = Mathf.Max(s.x, s.z); break; }
            t.localScale = saved;
            return w / Mathf.Max(0.0001f, t.lossyScale.x);
        }
    }
}
