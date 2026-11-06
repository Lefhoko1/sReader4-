// ===========================================================================
//  RestorationDemo — test harness for the RestorationDirector (F5)
// ===========================================================================
//  Drop this on any GameObject in SC_ReadingRoom, press Play, then:
//
//    DESKTOP:  Left-click any surface  -> PlayCorrect at the hit point
//              Right-click             -> PlayIncorrect (low unresolved tone)
//              Press C                 -> PlayCompletion at the last hit
//              Press 1..9 before click -> simulates word length (chime pitch)
//
//    MOBILE:   One-finger tap          -> PlayCorrect
//              Two-finger tap          -> PlayIncorrect
//              Three-finger tap        -> PlayCompletion
//
//  It also auto-registers every Light named "Light_Candle" (created by the
//  import pipeline on candle sockets) plus anything containing "CandleLight"
//  as progress lights, so correct answers visibly brighten your alcove.
//
//  DELETE this component once the real rituals call the director directly.
// ===========================================================================
using UnityEngine;

public class RestorationDemo : MonoBehaviour
{
    int _wordLength = 5;
    Vector3 _lastHit = Vector3.zero;
    Vector3 _lastNormal = Vector3.up;
    Camera _cam;

    void Start()
    {
        _cam = Camera.main;
        // Hook up the candle lights the import pipeline created, plus any
        // scene light with "CandleLight" in its name (e.g. WarmKey_CandleLight)
        foreach (var l in FindObjectsByType<Light>(FindObjectsInactive.Include,
                                                   FindObjectsSortMode.None))
            if (l.name.Contains("Light_Candle") || l.name.Contains("CandleLight"))
                RestorationDirector.Instance.RegisterProgressLight(l);

        Debug.Log("[RestorationDemo] Ready. LClick=Correct  RClick=Incorrect  " +
                  "C=Completion  1-9=word length (chime pitch).");
    }

    void Update()
    {
        // ---- word length keys (affects chime pitch) ----
        for (int k = 1; k <= 9; k++)
            if (Input.GetKeyDown(KeyCode.Alpha0 + k))
            {
                _wordLength = k + 1;      // keys 1..9 -> lengths 2..10
                Debug.Log($"[RestorationDemo] word length = {_wordLength}");
            }

        // ---- touch (mobile) ----
        if (Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (Input.touchCount >= 3) { Completion(); return; }
            if (Input.touchCount == 2)
            {
                RestorationDirector.Instance.PlayIncorrect(_lastHit);
                return;
            }
            RaycastAt(Input.GetTouch(0).position, correct: true);
            return;
        }

        // ---- mouse (editor / desktop) ----
        if (Input.GetMouseButtonDown(0)) RaycastAt(Input.mousePosition, true);
        if (Input.GetMouseButtonDown(1))
            RestorationDirector.Instance.PlayIncorrect(_lastHit);
        if (Input.GetKeyDown(KeyCode.C)) Completion();
    }

    void RaycastAt(Vector2 screenPos, bool correct)
    {
        if (_cam == null) _cam = Camera.main;
        if (_cam == null) return;
        var ray = _cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out var hit, 100f))
        {
            _lastHit = hit.point;
            _lastNormal = hit.normal;
            if (correct)
                RestorationDirector.Instance.PlayCorrect(
                    hit.point, hit.normal, 0.22f, _wordLength);
        }
        else
        {
            // No collider hit: play in the air facing the camera so the demo
            // still works on blockouts without colliders.
            var p = ray.origin + ray.direction * 1.5f;
            _lastHit = p; _lastNormal = -ray.direction;
            if (correct)
                RestorationDirector.Instance.PlayCorrect(
                    p, -ray.direction, 0.22f, _wordLength);
        }
    }

    void Completion()
    {
        RestorationDirector.Instance.PlayCompletion(_lastHit, _lastNormal, 0.45f);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(12, 12, 640, 24),
            "Feedback demo — LClick: correct | RClick: incorrect | C: completion | 1-9: word length");
    }
}
