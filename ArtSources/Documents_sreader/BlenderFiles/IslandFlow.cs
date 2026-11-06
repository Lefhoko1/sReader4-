// ===========================================================================
//  IslandFlow — the arrival: sea -> library door -> the reading begins
// ===========================================================================
//  Put this on an empty GameObject in SC_IslandVista (the polish menu adds it
//  for you). On Play:
//    1. The camera drifts in from the sea toward the Great Library (~6 s).
//    2. "The Great Library awaits…" fades in, with an ENTER button.
//    3. Tapping Enter -> golden fade -> the ReadingAdventure page opens.
//  Finds the library door automatically (DoorGlow / SOCKET_Entrance in the
//  vista FBX); falls back to sensible coordinates if not found.
// ===========================================================================
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

public class IslandFlow : MonoBehaviour
{
    [Header("Timing")]
    public float approachSeconds = 6f;
    public float fadeSeconds = 1.1f;

    Camera _cam;
    Vector3 _startPos, _endPos, _lookStart, _lookEnd;
    Canvas _ui; Image _fade; Text _title; Button _enter;
    bool _arrived;

    void Start()
    {
        _cam = Camera.main;
        if (_cam == null) { Debug.LogError("[IslandFlow] No MainCamera."); return; }

        // ---- find the door --------------------------------------------
        Vector3 door = new Vector3(0f, 2.6f, -0.4f);      // fallback
        Vector3 islandCenter = Vector3.zero;
        foreach (var t in FindObjectsByType<Transform>(
                     FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (t.name.Contains("DoorGlow") || t.name == "SOCKET_Entrance")
            { door = t.position; break; }
        }
        Vector3 outward = door - islandCenter; outward.y = 0f;
        outward = outward.sqrMagnitude < 0.01f ? Vector3.back
                                               : outward.normalized;

        _endPos   = door + outward * 7.5f + Vector3.up * 0.6f;
        _startPos = door + outward * 26f  + Vector3.up * 6.5f;
        _lookEnd  = door + Vector3.up * 0.2f;
        _lookStart = islandCenter + Vector3.up * 2.8f;

        _cam.transform.position = _startPos;
        _cam.transform.LookAt(_lookStart);

        BuildUI();
        StartCoroutine(Approach());
    }

    // ------------------------------------------------------------------ UI
    void BuildUI()
    {
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
            es.AddComponent<InputSystemUIInputModule>();
#else
            es.AddComponent<StandaloneInputModule>();
#endif
        }
        var go = new GameObject("FlowCanvas");
        _ui = go.AddComponent<Canvas>();
        _ui.renderMode = RenderMode.ScreenSpaceOverlay;
        _ui.sortingOrder = 500;                         // above reading page
        var sc = go.AddComponent<CanvasScaler>();
        sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        sc.referenceResolution = new Vector2(1600, 900);
        go.AddComponent<GraphicRaycaster>();
        var root = go.GetComponent<RectTransform>();
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // full-screen fade image (starts black, reveals the island)
        var fgo = new GameObject("Fade", typeof(RectTransform), typeof(Image));
        var frt = fgo.GetComponent<RectTransform>();
        frt.SetParent(root, false);
        frt.anchorMin = Vector2.zero; frt.anchorMax = Vector2.one;
        frt.offsetMin = frt.offsetMax = Vector2.zero;
        _fade = fgo.GetComponent<Image>();
        _fade.color = Color.black;
        _fade.raycastTarget = false;

        // title
        var tgo = new GameObject("Title", typeof(RectTransform), typeof(Text));
        var trt = tgo.GetComponent<RectTransform>();
        trt.SetParent(root, false);
        trt.anchorMin = trt.anchorMax = new Vector2(0.5f, 0.82f);
        trt.sizeDelta = new Vector2(1200, 90);
        _title = tgo.GetComponent<Text>();
        _title.font = font; _title.fontSize = 44;
        _title.alignment = TextAnchor.MiddleCenter;
        _title.fontStyle = FontStyle.Bold;
        _title.color = new Color(1f, 0.92f, 0.75f, 0f);
        _title.text = "The Great Library awaits…";
        var sh = tgo.AddComponent<Shadow>();
        sh.effectColor = new Color(0, 0, 0, 0.6f);
        sh.effectDistance = new Vector2(2, -2);

        // enter button (hidden until arrival)
        var bgo = new GameObject("Enter", typeof(RectTransform), typeof(Image),
                                 typeof(Button));
        var brt = bgo.GetComponent<RectTransform>();
        brt.SetParent(root, false);
        brt.anchorMin = brt.anchorMax = new Vector2(0.5f, 0.16f);
        brt.sizeDelta = new Vector2(420, 84);
        bgo.GetComponent<Image>().color = new Color(0.949f, 0.698f, 0.298f);
        _enter = bgo.GetComponent<Button>();
        _enter.onClick.AddListener(() => StartCoroutine(EnterLibrary()));
        var lgo = new GameObject("L", typeof(RectTransform), typeof(Text));
        var lrt = lgo.GetComponent<RectTransform>();
        lrt.SetParent(brt, false);
        lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
        lrt.offsetMin = lrt.offsetMax = Vector2.zero;
        var lt = lgo.GetComponent<Text>();
        lt.font = font; lt.fontSize = 30; lt.fontStyle = FontStyle.Bold;
        lt.alignment = TextAnchor.MiddleCenter;
        lt.color = Color.white;
        lt.text = "Enter the Library  →";
        bgo.SetActive(false);
    }

    // ------------------------------------------------------------ APPROACH
    IEnumerator Approach()
    {
        // reveal from black
        for (float t = 0; t < 1.2f; t += Time.deltaTime)
        {
            _fade.color = new Color(0, 0, 0, 1f - t / 1.2f);
            yield return null;
        }
        _fade.color = Color.clear;

        for (float t = 0; t < approachSeconds; t += Time.deltaTime)
        {
            float k = t / approachSeconds;
            k = k * k * (3f - 2f * k);                    // smoothstep
            _cam.transform.position = Vector3.Lerp(_startPos, _endPos, k);
            Vector3 look = Vector3.Lerp(_lookStart, _lookEnd, k);
            _cam.transform.rotation = Quaternion.Slerp(
                _cam.transform.rotation,
                Quaternion.LookRotation(look - _cam.transform.position),
                Time.deltaTime * 3f);
            if (k > 0.45f)
                _title.color = new Color(1f, 0.92f, 0.75f,
                                         Mathf.Min(1f, (k - 0.45f) * 3f));
            yield return null;
        }
        _arrived = true;
        _enter.gameObject.SetActive(true);
        // gentle idle sway while waiting
        StartCoroutine(IdleSway());
    }

    IEnumerator IdleSway()
    {
        Vector3 basePos = _cam.transform.position;
        float t = 0f;
        while (_arrived)
        {
            t += Time.deltaTime;
            _cam.transform.position = basePos +
                new Vector3(Mathf.Sin(t * 0.4f) * 0.12f, 
                            Mathf.Sin(t * 0.55f) * 0.06f, 0f);
            yield return null;
        }
    }

    // ------------------------------------------------------- ENTER + READ
    IEnumerator EnterLibrary()
    {
        _arrived = false;
        _enter.gameObject.SetActive(false);
        _title.text = "";
        _fade.raycastTarget = true;

        // golden fade in
        var gold = new Color(0.98f, 0.88f, 0.62f);
        for (float t = 0; t < fadeSeconds; t += Time.deltaTime)
        {
            _fade.color = new Color(gold.r, gold.g, gold.b, t / fadeSeconds);
            yield return null;
        }

        // start the reading adventure behind the fade
        if (GetComponent<ReadingAdventure>() == null)
            gameObject.AddComponent<ReadingAdventure>();
        yield return null;                                // let it build UI

        // fade back out to reveal the page
        for (float t = 0; t < fadeSeconds; t += Time.deltaTime)
        {
            _fade.color = new Color(gold.r, gold.g, gold.b,
                                    1f - t / fadeSeconds);
            yield return null;
        }
        _fade.color = Color.clear;
        _fade.raycastTarget = false;
    }
}
