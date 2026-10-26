using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The candy board behind the dashboard's display, alive: its glossy pink jelly frame carries a pink-to-cream
    /// candy gradient that flows slowly round it, with a bright shine travelling along the white highlight; soft
    /// twinkles drift off the frame. It shows the reading progress like the die does: the rainbow gumballs along
    /// the top light up one by one as the passage is read (the next one blinks, unlit ones stay dull), and a
    /// finished sentence or a restored key word fires a confetti burst while every gumball flashes.
    /// Presentation only: the presenter tells it the progress (0..1) and when to celebrate.
    /// </summary>
    public sealed class CandyBoard : MonoBehaviour
    {
        public Renderer frame;
        public Renderer shine;
        [Tooltip("The gumballs along the top, in reading order (left to right as the student sees them).")]
        public Renderer[] gumballs = new Renderer[0];
        public ParticleSystem twinkle;
        public ParticleSystem confetti;

        [Header("Flow")]
        public float flowSpeed = 0.035f;
        public float shineSpeed = 0.22f;
        public Vector2 frameTiling = new Vector2(6f, 1f);
        public Vector2 shineTiling = new Vector2(2f, 1f);

        float progress, flashUntil;
        MaterialPropertyBlock block;
        Color[] gumColours;
        float[] litAt;
        static readonly int BaseMapST = Shader.PropertyToID("_BaseMap_ST");
        static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        public float Progress => progress;

        void Awake()
        {
            block = new MaterialPropertyBlock();
            gumColours = new Color[gumballs.Length];
            litAt = new float[gumballs.Length];
            for (int i = 0; i < gumballs.Length; i++)
            {
                var m = gumballs[i] != null ? gumballs[i].sharedMaterial : null;
                gumColours[i] = m != null && m.HasProperty(BaseColor) ? m.GetColor(BaseColor) : Color.white;
                litAt[i] = -1f;
            }
        }

        /// <summary>How far through the passage (0..1): that share of the gumballs is lit.</summary>
        public void SetProgress(float p)
        {
            p = Mathf.Clamp01(p);
            int before = Lit(progress), after = Lit(p);
            for (int i = before; i < after && i < litAt.Length; i++) litAt[i] = Time.time;   // newly lit: they pop
            progress = p;
        }

        int Lit(float p) => Mathf.Clamp(Mathf.RoundToInt(p * gumballs.Length), 0, gumballs.Length);

        /// <summary>A sentence finished / a key word restored: confetti and a flash of every gumball.</summary>
        public void Celebrate(int amount = 90)
        {
            if (confetti != null) confetti.Emit(amount);
            if (twinkle != null) twinkle.Emit(30);
            flashUntil = Time.time + 0.6f;
        }

        void Update()
        {
            float t = Time.time;
            if (frame != null)
            {
                frame.GetPropertyBlock(block);
                block.SetVector(BaseMapST, new Vector4(frameTiling.x, frameTiling.y, Mathf.Repeat(t * flowSpeed, 1f), 0f));
                frame.SetPropertyBlock(block);
            }
            if (shine != null)
            {
                shine.GetPropertyBlock(block);
                block.SetVector(BaseMapST, new Vector4(shineTiling.x, shineTiling.y, Mathf.Repeat(-t * shineSpeed, 1f), 0f));
                shine.SetPropertyBlock(block);
            }

            int lit = Lit(progress);
            bool flash = t < flashUntil;
            for (int i = 0; i < gumballs.Length; i++)
            {
                var r = gumballs[i];
                if (r == null) continue;
                var c = gumColours[i];
                bool on = i < lit;
                bool next = i == lit;
                float glow;
                Color colour;
                if (on || flash)
                {
                    colour = c;
                    glow = 0.45f + 0.25f * Mathf.Sin(t * 3f + i * 0.7f) + (flash ? 0.8f : 0f);
                }
                else
                {
                    float grey = c.grayscale;
                    colour = Color.Lerp(c, new Color(grey, grey, grey), 0.65f) * 0.7f;           // dull until reached
                    glow = next ? 0.25f + 0.25f * Mathf.Sin(t * 6f) : 0f;                       // the next one blinks
                }
                r.GetPropertyBlock(block);
                block.SetColor(BaseColor, colour);
                block.SetColor(EmissionColor, c * Mathf.Max(0f, glow));
                r.SetPropertyBlock(block);
                // a newly lit gumball pops
                float since = litAt[i] < 0f ? 99f : t - litAt[i];
                float pop = since < 0.5f ? 1f + 0.5f * Mathf.Sin(since / 0.5f * Mathf.PI) : 1f;
                r.transform.localScale = Vector3.one * pop * BaseScale(r);
            }
        }

        readonly System.Collections.Generic.Dictionary<Renderer, float> baseScales = new System.Collections.Generic.Dictionary<Renderer, float>();
        float BaseScale(Renderer r)
        {
            if (!baseScales.TryGetValue(r, out var s)) { s = r.transform.localScale.x; baseScales[r] = s; }
            return s;
        }
    }
}
