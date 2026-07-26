// ===========================================================================
//  RestorationDirector — the shared restoration-feel service (Bible F5 / Ch. 8.4)
// ===========================================================================
//  ONE service plays the correct / incorrect / completion grammar (Ch. 2.1).
//  Every ritual (Word Kindling, Ink Weaving, Definition Reforging, Vision
//  Restoration) calls this — nobody hand-rolls feedback.
//
//    RestorationDirector.Instance.PlayCorrect(worldPos, surfaceNormal, size, wordLength);
//    RestorationDirector.Instance.PlayIncorrect(worldPos);
//    RestorationDirector.Instance.PlayCompletion(pageCenter, surfaceNormal);
//    StartCoroutine(RestorationDirector.DriftBack(tile, homePos, homeRot));
//
//  Design notes (mobile-first, per Bible):
//   * CORRECT   -> gold ink spread (0.4 s) + rising pentatonic chime picked by
//                  word length + 6–10 drifting motes + tiny room brighten.
//   * INCORRECT -> NO red, NO buzzer. A low unresolved tone; the caller uses
//                  DriftBack() so the piece "wavers and returns", message is
//                  "not yet", never "wrong".
//   * COMPLETE  -> page exhale: 1.5 s bloom pulse, mote bloom, resolving
//                  arpeggio, registered lights swell.
//   * All audio is SYNTHESIZED at startup (no clips needed). A sound designer
//     can override by assigning clips in the inspector later.
//   * Ink quads are pooled; particles are one pooled system; zero per-call
//     allocations after warmup.
//
//  SETUP: none required — it self-creates on first use. Optional inspector
//  hookups if you place one in the scene: URP Volume (bloom pulse), room
//  lights (brighten-with-progress), custom audio clips.
//
//  BUILD NOTE: add "GreatLibrary/InkSpread" and "GreatLibrary/MoteAdditive"
//  to Project Settings > Graphics > Always Included Shaders (Shader.Find is
//  stripped from device builds otherwise).
// ===========================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RestorationDirector : MonoBehaviour
{
    // ------------------------------------------------------------ SINGLETON
    static RestorationDirector _instance;
    public static RestorationDirector Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<RestorationDirector>();
                if (_instance == null)
                {
                    var go = new GameObject("RestorationDirector");
                    _instance = go.AddComponent<RestorationDirector>();
                }
            }
            return _instance;
        }
    }

    // ------------------------------------------------------------ INSPECTOR
    [Header("Optional scene hookups")]
    [Tooltip("URP Volume with a Bloom override — pulsed on completion.")]
    public Volume postVolume;
    [Tooltip("Lights that brighten as knowledge is restored (Ch. 3.4).")]
    public List<Light> progressLights = new List<Light>();
    [Tooltip("Max intensity progress lights may reach.")]
    public float progressLightMax = 2.2f;

    [Header("Optional audio overrides (else synthesized)")]
    public AudioClip[] chimeOverride;      // ordered low -> high
    public AudioClip missOverride;

    [Header("Tuning")]
    public Color inkColor  = new Color(0.83f, 0.62f, 0.25f, 1f);
    public Color moteColor = new Color(1.0f,  0.82f, 0.45f, 1f);
    [Range(0.1f, 1f)] public float sfxVolume = 0.6f;

    [Header("Effect scale")]
    [Tooltip("Global multiplier on every ink spread. Raise if effects look " +
             "too small in your scene.")]
    public float effectScale = 1f;
    [Tooltip("Keep effects a consistent on-screen size regardless of how far " +
             "the camera is. Recommended ON for world-space reading surfaces.")]
    public bool scaleWithDistance = true;
    [Tooltip("Camera distance (m) at which the requested size is used as-is.")]
    public float referenceDistance = 1.5f;
    [Tooltip("Clamp on distance scaling so far objects don't get absurd.")]
    public float maxDistanceScale = 8f;
    [Tooltip("Multiplier on mote particle size.")]
    public float moteScale = 1f;

    // ------------------------------------------------------------ EVENTS
    public event System.Action OnCorrect;
    public event System.Action OnIncorrect;
    public event System.Action OnCompletion;

    // ------------------------------------------------------------ INTERNAL
    Material _inkMat, _moteMat;
    readonly Queue<MeshRenderer> _inkPool = new Queue<MeshRenderer>();
    ParticleSystem _motes;
    AudioSource _chimeSrc, _missSrc;
    AudioClip[] _chimes;                   // pentatonic set, low -> high
    AudioClip _miss;
    AudioClip[] _arpeggio;                 // completion chord notes
    MaterialPropertyBlock _mpb;
    static readonly int ID_Progress = Shader.PropertyToID("_Progress");
    static readonly int ID_Fade     = Shader.PropertyToID("_Fade");
    static readonly int ID_InkCol   = Shader.PropertyToID("_InkColor");

    // Pentatonic (C major): C5 D5 E5 G5 A5 C6
    static readonly float[] PENTA = { 523.25f, 587.33f, 659.25f,
                                      783.99f, 880.00f, 1046.50f };

    void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        _mpb = new MaterialPropertyBlock();
        BuildMaterials();
        BuildAudio();
        BuildMotes();
        if (postVolume == null) postVolume = FindAnyObjectByType<Volume>();
    }

    // Remembered by PlayCorrect so no-argument overloads can reuse them.
    Vector3 _lastPos = Vector3.zero;
    Vector3 _lastNormal = Vector3.up;

    /// Normal that faces the main camera — sensible default when the caller
    /// only knows a position (e.g. a UI/world point on the reading page).
    Vector3 FacingCameraNormal()
    {
        var cam = Camera.main;
        return cam != null ? -cam.transform.forward : Vector3.up;
    }

    // ======================================================== PUBLIC API ===
    /// Correct answer at a world position on a surface (page, panel...).
    /// wordLength picks the chime note: short words low, long words high,
    /// so a completed sentence resolves into a small melody (Bible Ch. 8.4).
    public void PlayCorrect(Vector3 worldPos, Vector3 surfaceNormal,
                            float size = 0.25f, int wordLength = 5)
    {
        StartCoroutine(InkSpreadRoutine(worldPos, surfaceNormal, size, 0.4f, false));
        EmitMotes(worldPos, Random.Range(6, 11), 0.3f);
        PlayChime(wordLength);
        BrightenRoom(0.02f);
        _lastPos = worldPos;
        _lastNormal = surfaceNormal;
        OnCorrect?.Invoke();
    }

    /// Convenience: position only — effect faces the camera.
    public void PlayCorrect(Vector3 worldPos, float size = 0.25f,
                            int wordLength = 5)
        => PlayCorrect(worldPos, FacingCameraNormal(), size, wordLength);

    /// Incorrect: low unresolved tone only. Pair with DriftBack() on the
    /// dragged piece. Never red, never a buzzer (Bible Ch. 2.1 / 8.2).
    public void PlayIncorrect(Vector3 worldPos)
    {
        _missSrc.PlayOneShot(_miss, sfxVolume * 0.7f);
        OnIncorrect?.Invoke();
    }

    /// Page/book completion: the exhale (Bible Ch. 2.1).
    public void PlayCompletion(Vector3 center, Vector3 surfaceNormal,
                               float radius = 0.5f)
    {
        StartCoroutine(InkSpreadRoutine(center, surfaceNormal,
                                        radius * 2f, 1.1f, true));
        StartCoroutine(CompletionArpeggio());
        StartCoroutine(BloomPulse(1.5f, 1.6f));
        StartCoroutine(LightSwell(1.5f));
        EmitMotes(center, 26, radius);
        _lastPos = center;
        _lastNormal = surfaceNormal;
        OnCompletion?.Invoke();
    }

    /// Convenience: center only — effect faces the camera.
    public void PlayCompletion(Vector3 center, float radius = 0.5f)
        => PlayCompletion(center, FacingCameraNormal(), radius);

    /// Convenience: no arguments — exhale at the last correct-answer spot
    /// (or in front of the camera if nothing has been answered yet).
    public void PlayCompletion()
    {
        Vector3 c = _lastPos;
        Vector3 n = _lastNormal;
        if (c == Vector3.zero)
        {
            var cam = Camera.main;
            if (cam != null)
            {
                c = cam.transform.position + cam.transform.forward * 1.2f;
                n = -cam.transform.forward;
            }
        }
        PlayCompletion(c, n, 0.5f);
    }

    /// Shared "waver and return" for wrong drag-drops. Host it anywhere:
    ///   StartCoroutine(RestorationDirector.DriftBack(tile, homePos, homeRot));
    public static IEnumerator DriftBack(Transform piece, Vector3 homePos,
                                        Quaternion homeRot, float duration = 0.45f)
    {
        Vector3 start = piece.position;
        Quaternion startRot = piece.rotation;
        float t = 0f;
        // brief waver in place (the "not yet" hesitation)
        while (t < 0.12f)
        {
            t += Time.deltaTime;
            piece.position = start + piece.right *
                             (Mathf.Sin(t * 55f) * 0.006f * (1f - t / 0.12f));
            yield return null;
        }
        // ease home
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float e = 1f - Mathf.Pow(1f - Mathf.Clamp01(t / duration), 3f);
            piece.position = Vector3.LerpUnclamped(start, homePos, e);
            piece.rotation = Quaternion.SlerpUnclamped(startRot, homeRot, e);
            yield return null;
        }
        piece.position = homePos;
        piece.rotation = homeRot;
    }

    /// Register a light that should grow as the room is restored.
    public void RegisterProgressLight(Light l)
    {
        if (l != null && !progressLights.Contains(l)) progressLights.Add(l);
    }

    // ==================================================== INK SPREAD =======
    void BuildMaterials()
    {
        var inkShader  = Shader.Find("GreatLibrary/InkSpread");
        var moteShader = Shader.Find("GreatLibrary/MoteAdditive");
        if (inkShader == null || moteShader == null)
        {
            Debug.LogError("[RestorationDirector] Shaders not found. Ensure " +
                "InkSpread.shader and MoteAdditive.shader are in the project " +
                "(and in Always Included Shaders for device builds).");
        }
        _inkMat  = inkShader  != null ? new Material(inkShader)  : null;
        _moteMat = moteShader != null ? new Material(moteShader) : null;
        if (_inkMat != null) _inkMat.SetColor(ID_InkCol, inkColor);
    }

    /// Applies effectScale and (optionally) camera-distance compensation so
    /// an effect reads the same on screen whether the surface is near or far.
    float ScaledSize(float requested, Vector3 worldPos)
    {
        float s = requested * Mathf.Max(0.01f, effectScale);
        if (scaleWithDistance)
        {
            var cam = Camera.main;
            if (cam != null)
            {
                float d = Vector3.Distance(cam.transform.position, worldPos);
                float k = Mathf.Clamp(d / Mathf.Max(0.1f, referenceDistance),
                                      0.25f, Mathf.Max(1f, maxDistanceScale));
                s *= k;
            }
        }
        return s;
    }

    MeshRenderer GetInkQuad()
    {
        if (_inkPool.Count > 0)
        {
            var r = _inkPool.Dequeue();
            r.gameObject.SetActive(true);
            return r;
        }
        var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = "InkSpreadQuad";
        Destroy(go.GetComponent<Collider>());
        go.transform.SetParent(transform, false);
        var mr = go.GetComponent<MeshRenderer>();
        mr.sharedMaterial = _inkMat;
        mr.shadowCastingMode = ShadowCastingMode.Off;
        mr.receiveShadows = false;
        return mr;
    }

    IEnumerator InkSpreadRoutine(Vector3 pos, Vector3 normal, float size,
                                 float spreadTime, bool slowFade)
    {
        if (_inkMat == null) yield break;
        var mr = GetInkQuad();
        var t  = mr.transform;
        t.position = pos + normal.normalized * 0.006f;   // hover off surface
        t.rotation = Quaternion.LookRotation(-normal);
        t.localScale = Vector3.one * ScaledSize(size, pos);

        float time = 0f;
        while (time < spreadTime)                         // reveal 0 -> 1
        {
            time += Time.deltaTime;
            float p = Mathf.Clamp01(time / spreadTime);
            p = 1f - Mathf.Pow(1f - p, 2f);               // ease-out
            _mpb.SetFloat(ID_Progress, p);
            _mpb.SetFloat(ID_Fade, 1f);
            mr.SetPropertyBlock(_mpb);
            yield return null;
        }
        float hold = slowFade ? 0.45f : 0.18f;
        yield return new WaitForSeconds(hold);
        float fadeDur = slowFade ? 0.9f : 0.35f;          // dissolve away
        time = 0f;
        while (time < fadeDur)
        {
            time += Time.deltaTime;
            _mpb.SetFloat(ID_Progress, 1f);
            _mpb.SetFloat(ID_Fade, 1f - Mathf.Clamp01(time / fadeDur));
            mr.SetPropertyBlock(_mpb);
            yield return null;
        }
        mr.gameObject.SetActive(false);
        _inkPool.Enqueue(mr);
    }

    // ==================================================== MOTES ============
    void BuildMotes()
    {
        var go = new GameObject("Motes");
        go.transform.SetParent(transform, false);
        _motes = go.AddComponent<ParticleSystem>();

        var main = _motes.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime   = new ParticleSystem.MinMaxCurve(1.1f, 1.9f);
        main.startSpeed      = new ParticleSystem.MinMaxCurve(0.05f, 0.22f);
        main.startSize       = new ParticleSystem.MinMaxCurve(0.02f, 0.07f);
        main.startColor      = moteColor;
        main.gravityModifier = -0.02f;                    // gently rise
        main.maxParticles    = 128;                       // mobile cap
        main.playOnAwake     = false;

        var emission = _motes.emission; emission.enabled = false;
        var shape    = _motes.shape;    shape.enabled    = false;

        var col = _motes.colorOverLifetime;
        col.enabled = true;
        var grad = new Gradient();
        grad.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(1f, 0.15f),
                    new GradientAlphaKey(0f, 1f) });
        col.color = grad;

        var sol = _motes.sizeOverLifetime;
        sol.enabled = true;
        sol.size = new ParticleSystem.MinMaxCurve(1f,
            new AnimationCurve(new Keyframe(0f, 0.6f), new Keyframe(0.2f, 1f),
                               new Keyframe(1f, 0.15f)));

        var noise = _motes.noise;                          // drifting wander
        noise.enabled = true;
        noise.strength = 0.06f;
        noise.frequency = 0.6f;

        var psr = go.GetComponent<ParticleSystemRenderer>();
        psr.sharedMaterial = _moteMat;
        psr.shadowCastingMode = ShadowCastingMode.Off;
        psr.receiveShadows = false;
    }

    void EmitMotes(Vector3 pos, int count, float spread)
    {
        if (_motes == null) return;
        float k = ScaledSize(1f, pos) * moteScale;      // same distance logic
        var ep = new ParticleSystem.EmitParams();
        ep.applyShapeToPosition = false;
        for (int i = 0; i < count; i++)
        {
            ep.position = pos + Random.insideUnitSphere * spread * 0.4f * k;
            ep.startSize = Random.Range(0.02f, 0.07f) * k;
            ep.startLifetime = Random.Range(1.1f, 1.9f);
            _motes.Emit(ep, 1);
        }
    }

    // ==================================================== AUDIO (SYNTH) ====
    void BuildAudio()
    {
        _chimeSrc = gameObject.AddComponent<AudioSource>();
        _missSrc  = gameObject.AddComponent<AudioSource>();
        _chimeSrc.playOnAwake = _missSrc.playOnAwake = false;
        _chimeSrc.spatialBlend = _missSrc.spatialBlend = 0f;   // UI-ish, 2D

        if (chimeOverride != null && chimeOverride.Length > 0)
            _chimes = chimeOverride;
        else
        {
            _chimes = new AudioClip[PENTA.Length];
            for (int i = 0; i < PENTA.Length; i++)
                _chimes[i] = SynthChime("chime" + i, PENTA[i], 0.85f);
        }
        _miss = missOverride != null ? missOverride
                                     : SynthMiss("miss", 196.0f, 0.7f);
        // Completion arpeggio: C5 E5 G5 C6 (resolving major)
        _arpeggio = new[] { _chimes[0], _chimes[2], _chimes[3],
                            _chimes[_chimes.Length - 1] };
    }

    void PlayChime(int wordLength)
    {
        // 3-letter words -> lowest note, 10+ -> highest
        int idx = Mathf.Clamp(Mathf.RoundToInt(
            Mathf.InverseLerp(3f, 10f, wordLength) * (_chimes.Length - 1)),
            0, _chimes.Length - 1);
        _chimeSrc.PlayOneShot(_chimes[idx], sfxVolume);
    }

    IEnumerator CompletionArpeggio()
    {
        float[] delays = { 0f, 0.09f, 0.18f, 0.32f };
        for (int i = 0; i < _arpeggio.Length; i++)
        {
            yield return new WaitForSeconds(delays[i]);
            _chimeSrc.PlayOneShot(_arpeggio[i], sfxVolume * 0.9f);
        }
    }

    /// Bell-like tone: sine + harmonics, fast attack, exponential decay.
    static AudioClip SynthChime(string name, float freq, float dur)
    {
        int sr = AudioSettings.outputSampleRate;
        int n  = Mathf.CeilToInt(sr * dur);
        var data = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t   = i / (float)sr;
            float env = Mathf.Min(t / 0.006f, 1f) * Mathf.Exp(-t * 5.5f);
            float s   = Mathf.Sin(2f * Mathf.PI * freq * t)
                      + 0.32f * Mathf.Sin(2f * Mathf.PI * freq * 2f * t)
                      + 0.14f * Mathf.Sin(2f * Mathf.PI * freq * 3f * t)
                      + 0.05f * Mathf.Sin(2f * Mathf.PI * freq * 4.2f * t);
            data[i] = s * env * 0.32f;
        }
        var clip = AudioClip.Create(name, n, 1, sr, false);
        clip.SetData(data, 0);
        return clip;
    }

    /// Low unresolved "not yet": soft low tone with a gentle downward bend.
    static AudioClip SynthMiss(string name, float freq, float dur)
    {
        int sr = AudioSettings.outputSampleRate;
        int n  = Mathf.CeilToInt(sr * dur);
        var data = new float[n];
        float phase = 0f;
        for (int i = 0; i < n; i++)
        {
            float t   = i / (float)sr;
            float f   = freq * (1f - 0.06f * (t / dur));     // slight bend down
            phase    += 2f * Mathf.PI * f / sr;
            float env = Mathf.Min(t / 0.02f, 1f) * Mathf.Exp(-t * 3.2f);
            float s   = Mathf.Sin(phase) + 0.2f * Mathf.Sin(phase * 2f);
            data[i]   = s * env * 0.22f;
        }
        var clip = AudioClip.Create(name, n, 1, sr, false);
        clip.SetData(data, 0);
        return clip;
    }

    // ==================================================== ROOM RESPONSE ====
    void BrightenRoom(float add)
    {
        foreach (var l in progressLights)
            if (l != null)
                l.intensity = Mathf.Min(l.intensity + add * progressLightMax,
                                        progressLightMax);
    }

    IEnumerator LightSwell(float dur)
    {
        var bases = new List<float>();
        foreach (var l in progressLights) bases.Add(l != null ? l.intensity : 0f);
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float k = Mathf.Sin(Mathf.Clamp01(t / dur) * Mathf.PI); // up & down
            for (int i = 0; i < progressLights.Count; i++)
                if (progressLights[i] != null)
                    progressLights[i].intensity = bases[i] * (1f + 0.35f * k);
            yield return null;
        }
        for (int i = 0; i < progressLights.Count; i++)
            if (progressLights[i] != null)
                progressLights[i].intensity = bases[i];
    }

    IEnumerator BloomPulse(float dur, float boost)
    {
        if (postVolume == null || postVolume.profile == null) yield break;
        if (!postVolume.profile.TryGet<Bloom>(out var bloom)) yield break;
        float baseI = bloom.intensity.value;
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float k = Mathf.Sin(Mathf.Clamp01(t / dur) * Mathf.PI);
            bloom.intensity.value = baseI + (boost - 1f) * baseI * k
                                         + (baseI <= 0.01f ? k * 0.6f : 0f);
            yield return null;
        }
        bloom.intensity.value = baseI;
    }
}