// ===========================================================================
//  RestorationDemo — test harness for the RestorationDirector (F5)
// ===========================================================================
//  Works with EITHER input backend. Unity defines ENABLE_INPUT_SYSTEM when the
//  Input System package is active and ENABLE_LEGACY_INPUT_MANAGER for the old
//  one, so this compiles under "Input System", "Input Manager", or "Both".
//
//  Drop this on any empty GameObject in any scene with a MainCamera, Play, then:
//    Left-click / one-finger tap  -> PlayCorrect at the hit point
//    Right-click / two-finger tap -> PlayIncorrect (low unresolved tone)
//    Press C  / three-finger tap  -> PlayCompletion (the page exhale)
//    Press 1..9 before clicking   -> simulates word length (chime pitch)
//
//  Also auto-registers lights named "Light_Candle" (created by the kit import
//  pipeline) and anything containing "CandleLight" as progress lights, so
//  correct answers visibly brighten the room.
//
//  DELETE this component once real rituals call the director directly.
// ===========================================================================
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class RestorationDemo : MonoBehaviour
{
    int _wordLength = 5;
    Vector3 _lastHit = Vector3.zero;
    Vector3 _lastNormal = Vector3.up;
    Camera _cam;

    void Start()
    {
        _cam = Camera.main;
        foreach (var l in FindObjectsByType<Light>(FindObjectsInactive.Include,
                                                   FindObjectsSortMode.None))
            if (l.name.Contains("Light_Candle") || l.name.Contains("CandleLight"))
                RestorationDirector.Instance.RegisterProgressLight(l);

        Debug.Log("[RestorationDemo] Ready. LClick=Correct  RClick=Incorrect  " +
                  "C=Completion  1-9=word length (chime pitch).");
    }

    // ---------------------------------------------------- INPUT ABSTRACTION
    bool LeftPressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetMouseButtonDown(0)) return true;
#endif
        return false;
    }

    bool RightPressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetMouseButtonDown(1)) return true;
#endif
        return false;
    }

    bool CompletionPressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame)
            return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.C)) return true;
#endif
        return false;
    }

    /// Returns 1..9 if a number key was pressed this frame, else 0.
    int NumberPressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            Key[] keys = { Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4,
                           Key.Digit5, Key.Digit6, Key.Digit7, Key.Digit8,
                           Key.Digit9 };
            for (int i = 0; i < keys.Length; i++)
                if (Keyboard.current[keys[i]].wasPressedThisFrame)
                    return i + 1;
        }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        for (int k = 1; k <= 9; k++)
            if (Input.GetKeyDown(KeyCode.Alpha0 + k)) return k;
#endif
        return 0;
    }

    Vector2 PointerPosition()
    {
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.isPressed)
            return Touchscreen.current.primaryTouch.position.ReadValue();
        if (Mouse.current != null) return Mouse.current.position.ReadValue();
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.touchCount > 0) return Input.GetTouch(0).position;
        return Input.mousePosition;
#else
        return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
#endif
    }

    /// 0 = none this frame, else the number of fingers currently touching.
    int TouchBeganCount()
    {
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current != null)
        {
            bool began = false; int count = 0;
            foreach (var t in Touchscreen.current.touches)
            {
                if (t.press.isPressed) count++;
                if (t.press.wasPressedThisFrame) began = true;
            }
            if (began) return Mathf.Max(count, 1);
        }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Began)
            return Input.touchCount;
#endif
        return 0;
    }

    // ---------------------------------------------------------------- LOOP
    void Update()
    {
        int n = NumberPressed();
        if (n > 0)
        {
            _wordLength = n + 1;              // keys 1..9 -> lengths 2..10
            Debug.Log($"[RestorationDemo] word length = {_wordLength}");
        }

        int fingers = TouchBeganCount();
        if (fingers >= 3) { Completion(); return; }
        if (fingers == 2)
        {
            RestorationDirector.Instance.PlayIncorrect(_lastHit);
            return;
        }
        if (fingers == 1) { RaycastAt(PointerPosition()); return; }

        if (LeftPressed())  RaycastAt(PointerPosition());
        if (RightPressed()) RestorationDirector.Instance.PlayIncorrect(_lastHit);
        if (CompletionPressed()) Completion();
    }

    void RaycastAt(Vector2 screenPos)
    {
        if (_cam == null) _cam = Camera.main;
        if (_cam == null)
        {
            Debug.LogWarning("[RestorationDemo] No MainCamera in scene.");
            return;
        }

        var ray = _cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out var hit, 100f))
        {
            _lastHit = hit.point;
            _lastNormal = hit.normal;
        }
        else
        {
            // No collider: play in the air facing the camera so the demo still
            // works on blockouts and empty scenes.
            _lastHit = ray.origin + ray.direction * 1.5f;
            _lastNormal = -ray.direction;
        }
        RestorationDirector.Instance.PlayCorrect(
            _lastHit, _lastNormal, 0.5f, _wordLength);
    }

    void Completion()
    {
        RestorationDirector.Instance.PlayCompletion(_lastHit, _lastNormal, 0.9f);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(12, 12, 720, 24),
            "Restoration demo — LClick: correct | RClick: incorrect | " +
            "C: completion | 1-9: word length");
    }
}