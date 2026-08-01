using System.Collections;
using UnityEngine;

namespace SReader.Game.Library.Feel
{
    /// <summary>
    /// The shared correct-answer visual (Bible Ch. 8.4 "P_RestorationBurst" — one
    /// prefab reused everywhere). Subscribes to <see cref="FeedbackDirector.Correct"/>
    /// and, at the word's world position, plays the gold-ink spread (the
    /// <c>Custom/InkSpread</c> material animated 0->1 over the grammar's
    /// InkSpreadSeconds) and emits drifting light motes.
    ///
    /// A channel, not a rule: it only listens and renders (MVVM). With no ink
    /// material or motes system assigned it no-ops quietly, so the director still
    /// runs headlessly.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RestorationBurst : MonoBehaviour
    {
        [Tooltip("Optional — the director whose Correct beat drives this. Falls back to FeedbackDirector.Instance.")]
        [SerializeField] FeedbackDirector director;

        [Header("Ink spread")]
        [Tooltip("A material using the Custom/InkSpread shader.")]
        [SerializeField] Material inkMaterial;
        [Tooltip("World size of the ink quad (metres).")]
        [SerializeField] float inkSize = 0.6f;
        [Tooltip("Seconds the settled ink takes to fade out after it has spread.")]
        [SerializeField] float inkFadeSeconds = 0.5f;
        [Tooltip("Billboard the ink quad to the main camera each burst.")]
        [SerializeField] bool faceCamera = true;

        [Header("Motes")]
        [Tooltip("Optional — a ParticleSystem burst-emitted per correct answer (count from the grammar).")]
        [SerializeField] ParticleSystem motes;

        static readonly int ProgressId = Shader.PropertyToID("_Progress");
        static readonly int AlphaId    = Shader.PropertyToID("_Alpha");

        static Mesh quad;   // shared unit quad (XY plane, UV 0..1)

        FeedbackDirector Bound => director != null ? director : FeedbackDirector.Instance;

        void OnEnable()
        {
            if (director == null) director = FeedbackDirector.Instance;
            if (director == null) director = FindObjectOfType<FeedbackDirector>();
            if (director != null) director.Correct += OnCorrect;
        }

        void OnDisable()
        {
            if (director != null) director.Correct -= OnCorrect;
        }

        void OnCorrect(FeedbackContext ctx, int moteCount)
        {
            if (inkMaterial != null) StartCoroutine(SpreadInk(ctx.WorldPosition));

            if (motes != null)
            {
                motes.transform.position = ctx.WorldPosition;
                motes.Emit(moteCount);
            }
        }

        IEnumerator SpreadInk(Vector3 worldPos)
        {
            float spread = Bound != null ? Bound.Grammar.InkSpreadSeconds : 0.4f;

            var go = new GameObject("InkBurst");
            go.transform.position = worldPos;
            go.transform.localScale = Vector3.one * inkSize;
            if (faceCamera && Camera.main != null)
                go.transform.rotation = Camera.main.transform.rotation;

            var mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = Quad();
            var mr = go.AddComponent<MeshRenderer>();
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
            var mat = new Material(inkMaterial);           // per-burst instance, destroyed on cleanup
            mr.sharedMaterial = mat;

            // Spread the gold ink outward (0 -> 1).
            for (float t = 0f; t < spread; t += Time.deltaTime)
            {
                mat.SetFloat(ProgressId, spread > 0f ? t / spread : 1f);
                yield return null;
            }
            mat.SetFloat(ProgressId, 1f);

            // Settle: fade the burst out (the restored look lives on the page text, not here).
            for (float t = 0f; t < inkFadeSeconds; t += Time.deltaTime)
            {
                mat.SetFloat(AlphaId, inkFadeSeconds > 0f ? 1f - t / inkFadeSeconds : 0f);
                yield return null;
            }

            Destroy(mat);
            Destroy(go);
        }

        static Mesh Quad()
        {
            if (quad != null) return quad;
            quad = new Mesh { name = "InkSpreadQuad" };
            quad.vertices = new[]
            {
                new Vector3(-0.5f, -0.5f, 0f), new Vector3(0.5f, -0.5f, 0f),
                new Vector3(0.5f,  0.5f, 0f), new Vector3(-0.5f, 0.5f, 0f)
            };
            quad.uv        = new[] { new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1) };
            quad.triangles = new[] { 0, 2, 1, 0, 3, 2 };
            quad.RecalculateBounds();
            return quad;
        }
    }
}
