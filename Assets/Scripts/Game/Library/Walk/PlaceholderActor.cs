// ===========================================================================
//  PlaceholderActor — a stand-in reader, built from primitives, animated in code
// ===========================================================================
//  No rig, no clips, no imported model: a capsule body, a head, two legs and
//  two arms, swung by a sine wave that is driven by DISTANCE WALKED rather than
//  by time — so the stride never slides when the walker slows down or stops.
//
//  It exists to answer one question ("does a camera that walks with the reader
//  fix the visibility problem?") without waiting on the Blender track. When the
//  real character arrives, delete this component, drop the model under the same
//  Reader object, and PathWalker will not notice the difference: it only ever
//  sets Moving / Speed / Seated.
//
//  Feet are at the object's origin, so the walker can put the root straight on
//  the road with no offset. Total height ~1.75 m — sized against the stones so
//  you can read the scene at a glance.
//
//  The body parts are marked DontSave: they are rebuilt every load and never
//  clutter the scene file.
// ===========================================================================
using UnityEngine;

[ExecuteAlways]
public class PlaceholderActor : MonoBehaviour
{
    [Header("Build")]
    [Tooltip("Total height in metres, feet to crown. THE ONE NUMBER FOR THE READER'S " +
             "SIZE — the walking camera reads it and puts its eye at the top of it, " +
             "so turning this down moves the lens down with them instead of leaving " +
             "it hovering where their head used to be.\n\n" +
             "1.45 is a child, and a child is what this game is for. It also buys " +
             "the room back: the library hall is 5.6 x 4.2 m with a 3.4 m ceiling, " +
             "and a grown adult standing in it is nearly as tall as the shelves, " +
             "which is what makes everything read as crowded on a phone.")]
    [Range(0.6f, 2.2f)]
    public float height = 1.45f;
    public Color robe = new Color(0.86f, 0.62f, 0.30f);
    public Color skin = new Color(0.95f, 0.82f, 0.66f);
    public Color limbs = new Color(0.42f, 0.32f, 0.24f);

    [Header("Walk")]
    [Tooltip("Strides per metre walked. The swing is driven by distance, not " +
             "time, so the feet keep up with whatever speed the walker uses.")]
    public float stridesPerMetre = 0.62f;
    public float legSwingDegrees = 30f;
    public float armSwingDegrees = 22f;
    public float bobHeight = 0.045f;
    public float leanDegrees = 5f;

    Transform _rig, _hipL, _hipR, _shoulderL, _shoulderR, _body, _head;
    float _phase, _lean, _sit, _stepped;
    bool _seated;

    /// <summary>True while the walker is under way (drives the stride).</summary>
    public bool Moving { get; private set; }

    void OnEnable() { Adopt(); Build(); }

    /// <summary>
    /// Pick a body that is already there back up. The parts are DontSave, so a
    /// scene reload arrives with none and Build() makes them; a play-mode enter or
    /// a disable/enable arrives with them intact and this re-caches the joints
    /// rather than growing a second body.
    /// </summary>
    void Adopt()
    {
        if (_rig != null) return;
        var rig = transform.Find("Rig");
        if (rig == null) return;
        _rig = rig;
        _hipL = rig.Find("HipL"); _hipR = rig.Find("HipR");
        _shoulderL = rig.Find("ShoulderL"); _shoulderR = rig.Find("ShoulderR");
        _body = rig.Find("Body"); _head = rig.Find("Head");
        if (_hipL == null || _hipR == null || _shoulderL == null || _shoulderR == null)
            _rig = null;                       // half a body: throw it away and rebuild
    }

    /// <summary>Rebuild after changing the height or the colours.</summary>
    [ContextMenu("Rebuild Body")]
    public void RebuildBody()
    {
        var rig = transform.Find("Rig");
        if (rig != null) Kill(rig.gameObject);
        _rig = null;
        Build();
    }

    // ── the body ────────────────────────────────────────────────────────────

    void Build()
    {
        if (_rig != null) return;

        float h = Mathf.Max(0.5f, height);
        var rig = new GameObject("Rig");
        rig.hideFlags = HideFlags.DontSave;
        rig.transform.SetParent(transform, false);
        _rig = rig.transform;

        var robeMat = Mat(robe, "M_Actor_Robe");
        var skinMat = Mat(skin, "M_Actor_Skin");
        var limbMat = Mat(limbs, "M_Actor_Limb");

        _body = Part(_rig, PrimitiveType.Capsule, "Body", robeMat,
                     new Vector3(0f, h * 0.66f, 0f),
                     new Vector3(h * 0.30f, h * 0.22f, h * 0.24f));
        _head = Part(_rig, PrimitiveType.Sphere, "Head", skinMat,
                     new Vector3(0f, h * 0.92f, 0f), Vector3.one * (h * 0.21f));

        // a peak on the cap, so which way the reader faces is never in doubt
        Part(_head, PrimitiveType.Cube, "Brow", robeMat,
             new Vector3(0f, 0.15f, 0.62f), new Vector3(0.9f, 0.22f, 0.55f));

        _hipL = Joint(_rig, "HipL", new Vector3(-h * 0.075f, h * 0.47f, 0f));
        _hipR = Joint(_rig, "HipR", new Vector3(h * 0.075f, h * 0.47f, 0f));
        foreach (var hip in new[] { _hipL, _hipR })
            Part(hip, PrimitiveType.Capsule, "Leg", limbMat,
                 new Vector3(0f, -h * 0.235f, 0f),
                 new Vector3(h * 0.09f, h * 0.235f, h * 0.09f));

        _shoulderL = Joint(_rig, "ShoulderL", new Vector3(-h * 0.17f, h * 0.80f, 0f));
        _shoulderR = Joint(_rig, "ShoulderR", new Vector3(h * 0.17f, h * 0.80f, 0f));
        foreach (var sh in new[] { _shoulderL, _shoulderR })
            Part(sh, PrimitiveType.Capsule, "Arm", limbMat,
                 new Vector3(0f, -h * 0.17f, 0f),
                 new Vector3(h * 0.07f, h * 0.17f, h * 0.07f));

        // A fresh body arrives visible. If we were hidden — first person — it has
        // to go back to being hidden, or rebuilding the rig pops the reader's own
        // head into the middle of their own view.
        _skin = null;
        SetVisible(_visible);
    }

    static Transform Joint(Transform parent, string name, Vector3 localPos)
    {
        var go = new GameObject(name) { hideFlags = HideFlags.DontSave };
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        return go.transform;
    }

    static Transform Part(Transform parent, PrimitiveType type, string name,
                          Material mat, Vector3 localPos, Vector3 localScale)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.hideFlags = HideFlags.DontSave;
        // No collision anywhere on the actor: the stones are tapped with a raycast
        // and a body in the way would eat the tap. Switched off rather than
        // destroyed — destroying components while enabling is asking for trouble.
        var col = go.GetComponent<Collider>();
        if (col != null) col.enabled = false;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localScale = localScale;
        go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        return go.transform;
    }

    static Material Mat(Color c, string name)
    {
        var sh = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        var m = new Material(sh) { name = name, hideFlags = HideFlags.DontSave };
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (m.HasProperty("_Color")) m.SetColor("_Color", c);
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", 0.15f);
        return m;
    }

    static void Kill(Object o)
    {
        if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
    }

    // ── the animation ───────────────────────────────────────────────────────

    /// <summary>
    /// Advance the stride by <paramref name="metres"/> walked this frame. Stop
    /// calling it (or pass 0) and the pose eases back to a resting stance.
    /// </summary>
    public void Step(float metres)
    {
        _stepped = Mathf.Max(0f, metres);
        _phase += _stepped * stridesPerMetre * Mathf.PI * 2f;
    }

    /// <summary>Fold down onto the mat (or stand back up).</summary>
    public void SetSeated(bool seated) { _seated = seated; }

    /// <summary>
    /// Show or hide the body.
    ///
    /// You cannot see your own head. When the camera moves to the reader's eyes it
    /// is INSIDE this model — the skull is a sphere 0.3 m across and the cap's peak
    /// reaches further forward than the lens does — so the whole frame becomes the
    /// inside of a brown capsule, which reads exactly like "the character is far too
    /// big". Hiding the body is what first person means; nothing else here changes.
    /// </summary>
    public void SetVisible(bool visible)
    {
        if (_visible == visible && _skin != null) return;
        _visible = visible;

        if (_skin == null || System.Array.IndexOf(_skin, null) >= 0)
            _skin = GetComponentsInChildren<MeshRenderer>(true);
        foreach (var r in _skin)
            if (r != null) r.enabled = visible;
    }

    MeshRenderer[] _skin;
    bool _visible = true;

    // One pose per frame, after whoever is driving the walk has moved. Step()
    // only ever banks distance; this spends it and then clears it, so a walker
    // that stops calling Step simply idles.
    void LateUpdate()
    {
        float speed = _stepped / Mathf.Max(Time.deltaTime, 0.0001f);
        Moving = _stepped > 0.0001f;
        Pose(Mathf.Clamp01(speed / 1.4f));
        _stepped = 0f;
    }

    void Pose(float speed01)
    {
        if (_rig == null) return;
        float dt = Mathf.Max(Time.deltaTime, 0.0001f);
        float h = Mathf.Max(0.5f, height);

        _sit = Mathf.MoveTowards(_sit, _seated ? 1f : 0f, dt / 0.7f);
        _lean = Mathf.MoveTowards(_lean, Moving ? 1f : 0f, dt / 0.35f);

        float swing = Mathf.Sin(_phase) * (1f - _sit);
        float legs = legSwingDegrees * swing * Mathf.Max(speed01, Moving ? 0.6f : 0f);
        float arms = -armSwingDegrees * swing * Mathf.Max(speed01, Moving ? 0.6f : 0f);

        // seated: knees forward, arms resting, a slight settle back into the mat
        float sitLeg = Mathf.Lerp(0f, -78f, _sit);
        float sitArm = Mathf.Lerp(0f, -22f, _sit);

        _hipL.localRotation = Quaternion.Euler(legs + sitLeg, 0f, 0f);
        _hipR.localRotation = Quaternion.Euler(-legs + sitLeg, 0f, 0f);
        _shoulderL.localRotation = Quaternion.Euler(arms + sitArm, 0f, 0f);
        _shoulderR.localRotation = Quaternion.Euler(-arms + sitArm, 0f, 0f);

        // a walk bobs twice per stride; standing still, it breathes
        float bob = Moving
            ? Mathf.Abs(Mathf.Sin(_phase)) * bobHeight
            : Mathf.Sin(Time.time * 1.6f) * 0.012f;
        _rig.localPosition = new Vector3(0f, bob - _sit * h * 0.42f, _sit * h * 0.05f);
        _rig.localRotation = Quaternion.Euler(_lean * leanDegrees * (1f - _sit) -
                                              _sit * 6f, 0f, 0f);

        if (_body != null)
            _body.localRotation = Quaternion.Euler(0f, -swing * 6f, 0f);
        if (_head != null)
            _head.localRotation = Quaternion.Euler(Mathf.Sin(_phase * 2f) * 2.5f *
                                                   (1f - _sit), 0f, 0f);
    }
}
