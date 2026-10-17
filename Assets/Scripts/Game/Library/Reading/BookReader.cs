// ===========================================================================
//  BookReader — walk up to the book and press E to read it
// ===========================================================================
//  THE PROBLEM THIS SOLVES. In the Pavilion the mouse is the camera: the
//  Starter Assets controller locks the cursor and feeds every mouse movement to
//  look-around. StoryBook picks its sentences by clicking them, and a click can
//  never arrive while the cursor is locked to the middle of the screen — so the
//  book is placed, lit, scaled and completely unusable.
//
//  The two want the same input and cannot both have it, so they take turns. Walk
//  into range, press E, and the mouse becomes a mouse: the cursor appears, look
//  stops, and the sentences are clickable. Press E again (or Escape) and it goes
//  back to being the camera.
//
//  The player's own controller is not disabled — only its claim on the cursor is
//  released, and put back exactly as it was found. Anything else would leave the
//  Pavilion's own movement broken if this component were removed.
// ===========================================================================
using UnityEngine;
using StarterAssets;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class BookReader : MonoBehaviour
{
    [Header("Wiring")]
    [Tooltip("The book being read. Empty = the StoryBook on this object or below it.")]
    public StoryBook book;
    [Tooltip("Who walks up to it. Empty = whatever is tagged Player.")]
    public Transform player;

    [Header("Reach")]
    [Tooltip("How close the reader has to be for the book to offer itself, in metres.")]
    public float range = 3f;
    [Tooltip("Show a prompt on screen while in range. Placeholder UI — it is drawn " +
             "with IMGUI so it needs no assets, and should be replaced by the " +
             "project's own once this behaviour is settled.")]
    public bool showPrompt = true;

    /// <summary>True while the mouse belongs to the book rather than the camera.</summary>
    public bool Reading { get; private set; }

    StarterAssetsInputs _input;
    bool _wasLocked, _wasLooking;

    void Awake()
    {
        if (book == null) book = GetComponentInChildren<StoryBook>(true);
    }

    void Update()
    {
        if (player == null)
        {
            var tagged = GameObject.FindGameObjectWithTag("Player");
            if (tagged != null) player = tagged.transform;
        }
        if (_input == null && player != null)
            _input = player.GetComponentInChildren<StarterAssetsInputs>(true);

        if (Reading)
        {
            if (Pressed() || Cancelled()) Stop();
            return;
        }
        if (InRange() && Pressed()) Begin();
    }

    bool InRange()
    {
        if (player == null) return false;
        // flat distance: standing on a different step should not put the book out
        // of reach when you are right beside it
        Vector3 d = player.position - transform.position;
        d.y = 0f;
        return d.sqrMagnitude <= range * range;
    }

    /// <summary>Hand the mouse to the book.</summary>
    public void Begin()
    {
        Reading = true;
        if (_input != null)
        {
            _wasLocked = _input.cursorLocked;
            _wasLooking = _input.cursorInputForLook;
            _input.cursorLocked = false;
            _input.cursorInputForLook = false;   // stop the camera eating mouse movement
            _input.look = Vector2.zero;          // and drop whatever it was mid-way through
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>Give the mouse back to the camera, exactly as it was.</summary>
    public void Stop()
    {
        Reading = false;
        if (_input != null)
        {
            _input.cursorLocked = _wasLocked;
            _input.cursorInputForLook = _wasLooking;
        }
        Cursor.lockState = _wasLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !_wasLocked;
    }

    // Leaving play mode with the cursor hidden is a nuisance to recover from.
    void OnDisable() { if (Reading) Stop(); }

    static bool Pressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.E);
#endif
    }

    static bool Cancelled()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }

    void OnGUI()
    {
        if (!showPrompt) return;
        if (!Reading && !InRange()) return;

        string msg = Reading
            ? "Click a sentence to send it to the stones      E / Esc — done"
            : "E — read the book";

        var style = new GUIStyle(GUI.skin.box)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(16, 16, 10, 10),
        };
        var size = style.CalcSize(new GUIContent(msg));
        GUI.Box(new Rect((Screen.width - size.x) * 0.5f, Screen.height - size.y - 48f,
                         size.x, size.y), msg, style);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.95f, 0.8f, 0.35f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
