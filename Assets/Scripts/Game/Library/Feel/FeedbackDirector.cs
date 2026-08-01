using System;
using System.Collections.Generic;
using UnityEngine;

namespace SReader.Game.Library.Feel
{
    /// <summary>Outcome of one restoration beat.</summary>
    public enum FeedbackResult { Correct, Incorrect }

    /// <summary>
    /// Where and about-what a feedback beat happens. Presenters fill this in when a
    /// ritual is evaluated; the director and every subscribed channel read it.
    /// Keep it a value type so it allocates nothing on the hot path (spec §3).
    /// </summary>
    public readonly struct FeedbackContext
    {
        /// <summary>Stable key for the thing being solved (the gate id) — used to count consecutive misses.</summary>
        public readonly string SourceKey;
        /// <summary>World-space anchor of the effect (the word/tile position).</summary>
        public readonly Vector3 WorldPosition;
        /// <summary>The key word, if any (drives the chime note by length — Bible Ch. 8.4).</summary>
        public readonly string Word;

        public FeedbackContext(string sourceKey, Vector3 worldPosition, string word = null)
        {
            SourceKey = sourceKey;
            WorldPosition = worldPosition;
            Word = word;
        }
    }

    /// <summary>
    /// The Shared Restoration Grammar tunables (Bible Ch. 2.1). Serialised so a
    /// designer tunes feel without code. One instance lives on the director.
    /// </summary>
    [Serializable]
    public sealed class RestorationGrammar
    {
        [Header("Correct")]
        [Tooltip("Gold-ink spread duration (s).")] public float InkSpreadSeconds = 0.40f;
        [Tooltip("Drifting light motes on a correct answer.")] public int CorrectMotesMin = 6;
        public int CorrectMotesMax = 10;
        [Tooltip("Page brightness added per correct answer (fraction, e.g. 0.02 = +2%).")]
        public float BrightnessStepOnCorrect = 0.02f;

        [Header("Incorrect")]
        [Tooltip("How long the piece wavers before it drifts back (s). No red, no buzzer, no X.")]
        public float WaverSeconds = 0.35f;
        [Tooltip("Consecutive misses on one gate before the Owl offers a context hint (Bible Ch. 2.1).")]
        public int MissesBeforeHint = 2;

        [Header("Completion")]
        [Tooltip("The page-exhale bloom pulse duration (s).")] public float ExhaleSeconds = 1.5f;

        [Header("Audio (pentatonic)")]
        [Tooltip("Rising pentatonic notes; the note is chosen by word length so completed sentences resolve musically (Bible Ch. 8.4).")]
        public AudioClip[] CorrectNotes = Array.Empty<AudioClip>();
        [Tooltip("The single low, unresolved tone for a miss.")] public AudioClip MissTone;
        [Tooltip("The completion / page-exhale resolve.")] public AudioClip CompleteChord;
    }

    /// <summary>
    /// The single service that plays the Shared Restoration Grammar (Bible Ch. 2.1
    /// / spec §1). <b>Every ritual calls this — nobody hand-rolls feedback.</b>
    /// It owns only <i>presentation</i> (MVVM): it never decides whether an answer
    /// is correct (that is <c>TrekSession</c>/<c>GateSolving</c>) — it is told the
    /// outcome and performs the grammar.
    ///
    /// Effects are delivered through <see cref="event"/>s so the InkSpread shader
    /// driver, the motes VFX, the lighting-as-progress rig and the Owl can each
    /// subscribe without the director knowing they exist. Audio it can own
    /// directly (a single <see cref="AudioSource"/>). With no channels wired it
    /// degrades to log lines, so it runs in placeholder mode with zero assets.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FeedbackDirector : MonoBehaviour
    {
        [SerializeField] RestorationGrammar grammar = new RestorationGrammar();
        [Tooltip("Optional — the director plays chimes/tones through this. Falls back to logging when unset.")]
        [SerializeField] AudioSource audioSource;
        [Tooltip("Log every beat to the console (useful before VFX/audio are wired).")]
        [SerializeField] bool logBeats = true;

        /// <summary>The most convenient reference for scene presenters. Set on Awake; null-safe callers should guard.</summary>
        public static FeedbackDirector Instance { get; private set; }

        public RestorationGrammar Grammar => grammar;

        // ── The grammar as events (channels subscribe; the director stays ignorant of them) ──

        /// <summary>Correct answer: play the gold-ink spread at the anchor, count of motes supplied.</summary>
        public event Action<FeedbackContext, int /*motes*/> Correct;
        /// <summary>Wrong answer: the piece wavers and drifts back. No red, no buzzer, no X.</summary>
        public event Action<FeedbackContext> Incorrect;
        /// <summary>Two consecutive misses on one gate — the Owl offers a context hint (never a completion cost).</summary>
        public event Action<FeedbackContext> HintOffered;
        /// <summary>A correct answer's page-brightness step (fraction). Lighting-as-progress binds to the running sum.</summary>
        public event Action<float /*brightnessStep*/> BrightnessRaised;
        /// <summary>The whole page/book completed: the exhale bloom pulse, spine gem +1 step.</summary>
        public event Action CompletionExhaled;

        readonly Dictionary<string, int> missStreak = new Dictionary<string, int>();

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
        }

        void OnDestroy() { if (Instance == this) Instance = null; }

        // ── The three beats every ritual calls ────────────────────────────────

        /// <summary>Play the correct-answer grammar and clear the gate's miss streak.</summary>
        public void PlayCorrect(in FeedbackContext ctx)
        {
            if (!string.IsNullOrEmpty(ctx.SourceKey)) missStreak.Remove(ctx.SourceKey);

            int motes = UnityEngine.Random.Range(grammar.CorrectMotesMin, grammar.CorrectMotesMax + 1);
            if (logBeats) Debug.Log($"[Feel] CORRECT '{ctx.Word}' @ {ctx.WorldPosition} — ink {grammar.InkSpreadSeconds}s, {motes} motes, +{grammar.BrightnessStepOnCorrect:P0}");

            PlayNoteForWord(ctx.Word);
            Correct?.Invoke(ctx, motes);
            BrightnessRaised?.Invoke(grammar.BrightnessStepOnCorrect);
        }

        /// <summary>
        /// Play the incorrect-answer grammar. Returns true when this miss crossed the
        /// threshold and the Owl hint was offered (Bible Ch. 2.1) — presenters can
        /// use the return to surface the hint affordance.
        /// </summary>
        public bool PlayIncorrect(in FeedbackContext ctx)
        {
            int streak = 0;
            if (!string.IsNullOrEmpty(ctx.SourceKey))
            {
                missStreak.TryGetValue(ctx.SourceKey, out streak);
                streak++;
                missStreak[ctx.SourceKey] = streak;
            }

            if (logBeats) Debug.Log($"[Feel] INCORRECT '{ctx.Word}' — waver {grammar.WaverSeconds}s, low tone (miss {streak})");

            PlayOneShot(grammar.MissTone);
            Incorrect?.Invoke(ctx);

            bool offerHint = streak >= grammar.MissesBeforeHint;
            if (offerHint)
            {
                if (logBeats) Debug.Log($"[Feel] Owl offers a context hint for '{ctx.Word}' (after {streak} misses)");
                HintOffered?.Invoke(ctx);
            }
            return offerHint;
        }

        /// <summary>Play the completion / page-exhale beat (Bible Ch. 2.1 — slow bloom, spine gem +1).</summary>
        public void PlayCompletion()
        {
            if (logBeats) Debug.Log($"[Feel] COMPLETE — page exhales {grammar.ExhaleSeconds}s, spine gem +1");
            PlayOneShot(grammar.CompleteChord);
            CompletionExhaled?.Invoke();
        }

        /// <summary>Forget a gate's miss streak (e.g. on defer / re-entry).</summary>
        public void ResetMisses(string sourceKey)
        {
            if (!string.IsNullOrEmpty(sourceKey)) missStreak.Remove(sourceKey);
        }

        // ── audio ──────────────────────────────────────────────────────────────

        // The note rises with word length so a completed sentence resolves musically (Bible Ch. 8.4).
        void PlayNoteForWord(string word)
        {
            var notes = grammar.CorrectNotes;
            if (notes == null || notes.Length == 0) return;
            int len = string.IsNullOrEmpty(word) ? 1 : word.Length;
            int index = Mathf.Clamp(len - 1, 0, notes.Length - 1);
            PlayOneShot(notes[index]);
        }

        void PlayOneShot(AudioClip clip)
        {
            if (clip == null) return;
            if (audioSource != null) audioSource.PlayOneShot(clip);
            else if (Camera.main != null) AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
        }
    }
}
