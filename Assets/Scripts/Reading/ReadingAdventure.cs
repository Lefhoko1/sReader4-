// ===========================================================================
//  ReadingAdventure — the playable heart of the game, in one file
// ===========================================================================
//  THIS is the reading adventure: a tutor's assignment arrives as a page,
//  the student reads a real passage, keywords glow gold, and each keyword is
//  restored through one of the game's rituals:
//     • Ink Weaving        — fill the blank by ordering scrambled letters
//     • Definition Reforge — rebuild the definition from phrase shards
//     • Vision Restoration — choose the image that matches the word
//  Every success fires RestorationDirector.PlayCorrect (gold ink + chime),
//  finishing the page fires PlayCompletion (the exhale). A comprehension
//  question closes the loop. All UI is built at runtime — no scene setup.
//
//  USE: empty GameObject in any scene with a MainCamera -> Add Component ->
//  ReadingAdventure -> Play. Works with mouse and touch.
//
//  The Assignment class below IS the tutor contract (Bible Ch. 8.1): your
//  tutor portal publishes this as JSON; here one sample is embedded so you
//  can SEE the adventure today. Swap `BuildSampleAssignment()` for a JSON
//  load and real tutor content flows straight in.
// ===========================================================================
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

public class ReadingAdventure : MonoBehaviour
{
    // ================================================== TUTOR DATA CONTRACT
    [System.Serializable] public class Keyword
    {
        public string word;
        public string definition;
        public string[] definitionShards;   // correct order
        public string blankSentence;        // contains ____
        public string activity;             // "blank" | "define" | "image"
        public string[] imageLabels;        // 4 options (placeholder art)
        public Color[] imageColors;
        public int correctImage;
        [System.NonSerialized] public bool restored;
    }
    [System.Serializable] public class Assignment
    {
        public string title, tutor, passage, question;
        public string[] answers; public int correctAnswer;
        public Keyword[] keywords;
    }

    static Assignment BuildSampleAssignment() => new Assignment
    {
        title = "The Island of the Great Library",
        tutor = "Master Scholar Elara",
        passage =
            "Long ago, ships from every land made the voyage across the " +
            "silver sea to a small green island. On its hill stood the " +
            "Great Library, whose windows glowed like candles at dusk. " +
            "Inside were ancient books that held everything people had " +
            "ever learned. When the Forgetting came, words faded from the " +
            "pages. Now young Guardians read each book carefully, because " +
            "only true understanding can restore the lost knowledge.",
        question = "Why do the Guardians read the books so carefully?",
        answers = new[] {
            "Because reading makes the time pass",
            "Because only understanding restores knowledge",
            "Because the Library is cold at night" },
        correctAnswer = 1,
        keywords = new[]
        {
            new Keyword {
                word = "voyage", activity = "blank",
                definition = "a long journey, especially by sea",
                blankSentence = "Ships from every land made the ____ " +
                                "across the silver sea.",
            },
            new Keyword {
                word = "ancient", activity = "define",
                definition = "very old; from a time long, long ago",
                definitionShards = new[] { "very old;", "from a time",
                                           "long, long ago" },
            },
            new Keyword {
                word = "restore", activity = "image",
                definition = "to bring something back to how it was",
                imageLabels = new[] { "A broken vase left in pieces",
                                      "A faded page turning bright again",
                                      "A closed door", "A sleeping cat" },
                imageColors = new[] {
                    new Color(0.55f,0.32f,0.30f), new Color(0.95f,0.70f,0.30f),
                    new Color(0.35f,0.32f,0.38f), new Color(0.40f,0.45f,0.55f)},
                correctImage = 1,
            },
        }
    };

    // ================================================== PALETTE / STATE
    static readonly Color Parchment = new Color(0.957f, 0.914f, 0.827f);
    static readonly Color Ink       = new Color(0.17f, 0.13f, 0.10f);
    static readonly Color Gold      = new Color(0.949f, 0.698f, 0.298f);
    static readonly Color GoldDeep  = new Color(0.788f, 0.482f, 0.176f);
    static readonly Color Violet    = new Color(0.357f, 0.247f, 0.659f);
    static readonly Color Fog       = new Color(0.549f, 0.592f, 0.659f);

    Assignment _a;
    Font _font;
    Canvas _canvas;
    RectTransform _root, _page;
    Text _passageText;
    readonly List<Button> _kwButtons = new List<Button>();
    Camera _cam;

    void Start()
    {
        _a = BuildSampleAssignment();
        _cam = Camera.main;
        _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        BuildCanvas();
        ShowPassage();
    }

    // ================================================== UI SCAFFOLD
    void BuildCanvas()
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
        var go = new GameObject("ReadingCanvas");
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var sc = go.AddComponent<CanvasScaler>();
        sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        sc.referenceResolution = new Vector2(1600, 900);
        sc.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        _root = go.GetComponent<RectTransform>();

        _page = Panel(_root, new Vector2(0.5f, 0.5f), new Vector2(900, 780),
                      Parchment);
        var edge = Panel(_page, new Vector2(0.5f, 0.5f), new Vector2(916, 796),
                         GoldDeep);
        edge.SetAsFirstSibling();
    }

    RectTransform Panel(RectTransform parent, Vector2 anchor, Vector2 size,
                        Color col)
    {
        var go = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = rt.anchorMax = anchor;
        rt.sizeDelta = size;
        go.GetComponent<Image>().color = col;
        return rt;
    }

    Text Label(RectTransform parent, string text, int fs, Vector2 anchoredPos,
               Vector2 size, TextAnchor align = TextAnchor.UpperLeft,
               Color? col = null, FontStyle style = FontStyle.Normal)
    {
        var go = new GameObject("Label", typeof(RectTransform), typeof(Text));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos; rt.sizeDelta = size;
        var t = go.GetComponent<Text>();
        t.font = _font; t.fontSize = fs; t.text = text;
        t.alignment = align; t.color = col ?? Ink; t.fontStyle = style;
        t.supportRichText = true;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        return t;
    }

    Button Btn(RectTransform parent, string text, Vector2 pos, Vector2 size,
               Color bg, Color fg, int fs, System.Action onClick)
    {
        var go = new GameObject("Btn", typeof(RectTransform), typeof(Image),
                                typeof(Button));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        go.GetComponent<Image>().color = bg;
        var b = go.GetComponent<Button>();
        b.onClick.AddListener(() => onClick());
        var l = Label(rt, text, fs, Vector2.zero, size,
                      TextAnchor.MiddleCenter, fg, FontStyle.Bold);
        l.rectTransform.anchoredPosition = Vector2.zero;
        return b;
    }

    void ClearPage()
    {
        for (int i = _page.childCount - 1; i >= 0; i--)
            Destroy(_page.GetChild(i).gameObject);
        _kwButtons.Clear();
    }

    Vector3 WorldOf(RectTransform rt)   // effect anchor for the director
    {
        Vector3 screen = rt.position;
        if (_cam == null) _cam = Camera.main;
        return _cam != null
            ? _cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, 1.2f))
            : Vector3.zero;
    }

    // ================================================== PASSAGE SCREEN
    void ShowPassage()
    {
        ClearPage();
        Label(_page, "TODAY'S READING", 20, new Vector2(0, 350),
              new Vector2(840, 30), TextAnchor.MiddleCenter, GoldDeep,
              FontStyle.Bold);
        Label(_page, _a.title, 34, new Vector2(0, 308),
              new Vector2(840, 44), TextAnchor.MiddleCenter, Violet,
              FontStyle.Bold);
        Label(_page, "assigned by " + _a.tutor, 18, new Vector2(0, 274),
              new Vector2(840, 26), TextAnchor.MiddleCenter, Fog,
              FontStyle.Italic);

        _passageText = Label(_page, DecoratedPassage(), 26,
                             new Vector2(0, 70), new Vector2(800, 330));
        _passageText.lineSpacing = 1.25f;

        int done = _a.keywords.Count(k => k.restored);
        Label(_page, done < _a.keywords.Length
                  ? "Faded words shimmer in gold. Restore them:"
                  : "All words restored! One question remains…",
              22, new Vector2(0, -140), new Vector2(800, 30),
              TextAnchor.MiddleCenter, GoldDeep, FontStyle.Italic);

        float x = -(_a.keywords.Length - 1) * 135f;
        foreach (var kw in _a.keywords)
        {
            var k = kw;
            var b = Btn(_page,
                        k.restored ? "✓ " + k.word : "Restore \"" + k.word + "\"",
                        new Vector2(x, -200), new Vector2(240, 62),
                        k.restored ? new Color(0.8f, 0.86f, 0.74f) : Gold,
                        k.restored ? Ink : Color.white, 22,
                        () => { if (!k.restored) StartActivity(k); });
            b.interactable = !k.restored;
            _kwButtons.Add(b);
            x += 270f;
        }
        if (done == _a.keywords.Length)
            Btn(_page, "Answer the Scholar's Question →",
                new Vector2(0, -300), new Vector2(460, 66), Violet,
                Color.white, 24, ShowQuestion);
        else
            Label(_page, $"{done} / {_a.keywords.Length} restored", 20,
                  new Vector2(0, -300), new Vector2(300, 26),
                  TextAnchor.MiddleCenter, Fog);
    }

    string DecoratedPassage()
    {
        string p = _a.passage;
        foreach (var k in _a.keywords)
        {
            string tag = k.restored
                ? $"<color=#{ColorUtility.ToHtmlStringRGB(GoldDeep)}><b>{k.word}</b></color>"
                : $"<color=#{ColorUtility.ToHtmlStringRGB(Fog)}><b><i>{k.word}</i></b></color>";
            p = p.Replace(k.word, tag);
        }
        return p;
    }

    // ================================================== ACTIVITY ROUTER
    void StartActivity(Keyword k)
    {
        ClearPage();
        switch (k.activity)
        {
            case "blank":  BuildInkWeaving(k); break;
            case "define": BuildDefinitionReforge(k); break;
            default:       BuildVisionRestoration(k); break;
        }
    }

    void ActivityHeader(string ritual, string prompt)
    {
        Label(_page, ritual, 20, new Vector2(0, 350), new Vector2(840, 28),
              TextAnchor.MiddleCenter, Violet, FontStyle.Bold);
        Label(_page, prompt, 26, new Vector2(0, 300), new Vector2(800, 70),
              TextAnchor.MiddleCenter, Ink);
    }

    void Succeed(Keyword k, RectTransform at)
    {
        k.restored = true;
        RestorationDirector.Instance.PlayCorrect(WorldOf(at), 0.35f,
                                                 k.word.Length);
        StartCoroutine(BackToPassage(0.9f));
    }
    void Fail(RectTransform shakeTarget)
    {
        RestorationDirector.Instance.PlayIncorrect(WorldOf(shakeTarget));
        if (shakeTarget != null) StartCoroutine(Shake(shakeTarget));
    }
    IEnumerator BackToPassage(float delay)
    { yield return new WaitForSeconds(delay); ShowPassage(); }
    IEnumerator Shake(RectTransform rt)
    {
        Vector2 home = rt.anchoredPosition;
        for (float t = 0; t < 0.3f; t += Time.deltaTime)
        {
            rt.anchoredPosition = home + new Vector2(
                Mathf.Sin(t * 55f) * 7f * (1 - t / 0.3f), 0);
            yield return null;
        }
        rt.anchoredPosition = home;
    }

    // ============================================ RITUAL 1: INK WEAVING ====
    void BuildInkWeaving(Keyword k)
    {
        ActivityHeader("— INK WEAVING —",
            "The word has faded. Weave the ink drops back in order:");
        var sentence = Label(_page, k.blankSentence, 28, new Vector2(0, 190),
                             new Vector2(800, 90), TextAnchor.MiddleCenter);
        var slot = Panel(_page, new Vector2(0.5f, 0.5f), new Vector2(420, 70),
                         new Color(0.90f, 0.85f, 0.74f));
        slot.anchoredPosition = new Vector2(0, 80);
        var slotText = Label(slot, "", 34, Vector2.zero, new Vector2(400, 60),
                             TextAnchor.MiddleCenter, GoldDeep, FontStyle.Bold);

        string built = "";
        var letters = k.word.ToCharArray().Select(c => c.ToString()).ToList();
        var order = Enumerable.Range(0, letters.Count)
                              .OrderBy(_ => Random.value).ToList();
        var letterBtns = new List<Button>();
        float x0 = -(letters.Count - 1) * 45f;
        for (int i = 0; i < order.Count; i++)
        {
            int src = order[i];
            Button b = null;
            b = Btn(_page, letters[src].ToUpper(),
                    new Vector2(x0 + i * 90f, -60), new Vector2(74, 74),
                    Gold, Color.white, 34, () =>
            {
                built += letters[src];
                slotText.text = built;
                b.interactable = false;
                b.image.color = new Color(0.85f, 0.82f, 0.72f);
                if (built.Length == k.word.Length)
                {
                    if (built == k.word) Succeed(k, slot);
                    else
                    {
                        Fail(slot);
                        built = ""; slotText.text = "";
                        foreach (var lb in letterBtns)
                        { lb.interactable = true; lb.image.color = Gold; }
                    }
                }
            });
            letterBtns.Add(b);
        }
        Btn(_page, "Clear", new Vector2(0, -170), new Vector2(150, 50),
            Fog, Color.white, 20, () =>
        {
            built = ""; slotText.text = "";
            foreach (var lb in letterBtns)
            { lb.interactable = true; lb.image.color = Gold; }
        });
        HintFooter(k);
    }

    // ====================================== RITUAL 2: DEFINITION REFORGE ===
    void BuildDefinitionReforge(Keyword k)
    {
        ActivityHeader("— DEFINITION REFORGING —",
            $"The definition of <b>{k.word}</b> lies shattered. " +
            "Reforge the shards in order:");
        var anvil = Panel(_page, new Vector2(0.5f, 0.5f), new Vector2(760, 80),
                          new Color(0.90f, 0.85f, 0.74f));
        anvil.anchoredPosition = new Vector2(0, 140);
        var forged = Label(anvil, "", 26, Vector2.zero, new Vector2(730, 70),
                           TextAnchor.MiddleCenter, GoldDeep, FontStyle.Bold);

        var picked = new List<int>();
        var shardBtns = new List<Button>();
        var order = Enumerable.Range(0, k.definitionShards.Length)
                              .OrderBy(_ => Random.value).ToList();
        // guarantee scrambled start
        if (order.SequenceEqual(Enumerable.Range(0, order.Count)))
            order.Reverse();
        for (int i = 0; i < order.Count; i++)
        {
            int src = order[i];
            Button b = null;
            b = Btn(_page, k.definitionShards[src],
                    new Vector2(0, 20 - i * 95f), new Vector2(560, 74),
                    Violet, Color.white, 24, () =>
            {
                picked.Add(src);
                forged.text = string.Join(" ",
                    picked.Select(p => k.definitionShards[p]));
                b.interactable = false;
                b.image.color = new Color(0.75f, 0.72f, 0.82f);
                if (picked.Count == k.definitionShards.Length)
                {
                    if (picked.SequenceEqual(
                            Enumerable.Range(0, picked.Count)))
                        Succeed(k, anvil);
                    else
                    {
                        Fail(anvil);
                        picked.Clear(); forged.text = "";
                        foreach (var sb in shardBtns)
                        { sb.interactable = true; sb.image.color = Violet; }
                    }
                }
            });
            shardBtns.Add(b);
        }
        HintFooter(k);
    }

    // ===================================== RITUAL 3: VISION RESTORATION ====
    void BuildVisionRestoration(Keyword k)
    {
        ActivityHeader("— VISION RESTORATION —",
            $"Which vision truly shows <b>{k.word}</b>? Choose, and the " +
            "picture will paint itself back into the book.");
        for (int i = 0; i < k.imageLabels.Length; i++)
        {
            int idx = i;
            float x = (i % 2 == 0) ? -210 : 210;
            float y = (i < 2) ? 110 : -110;
            var frame = Panel(_page, new Vector2(0.5f, 0.5f),
                              new Vector2(380, 190), GoldDeep);
            frame.anchoredPosition = new Vector2(x, y);
            var b = Btn(frame, "", Vector2.zero, new Vector2(366, 176),
                        k.imageColors[idx], Color.white, 20, () =>
            {
                if (idx == k.correctImage) Succeed(k, frame);
                else Fail(frame);
            });
            Label(b.GetComponent<RectTransform>(), k.imageLabels[idx], 21,
                  Vector2.zero, new Vector2(340, 160),
                  TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
        }
        HintFooter(k);
    }

    void HintFooter(Keyword k)
    {
        Btn(_page, "🦉 Hint", new Vector2(-340, -320), new Vector2(150, 54),
            new Color(0.55f, 0.42f, 0.28f), Color.white, 20, () =>
        {
            Label(_page, "Owl whispers: \"" + k.definition + "\"", 20,
                  new Vector2(60, -320), new Vector2(560, 50),
                  TextAnchor.MiddleLeft, Fog, FontStyle.Italic);
        });
        Btn(_page, "← Back", new Vector2(340, 320), new Vector2(130, 46),
            Fog, Color.white, 18, ShowPassage);
    }

    // ================================================== COMPREHENSION ======
    void ShowQuestion()
    {
        ClearPage();
        Label(_page, "— THE SCHOLAR'S QUESTION —", 22, new Vector2(0, 320),
              new Vector2(840, 30), TextAnchor.MiddleCenter, Violet,
              FontStyle.Bold);
        var qrt = Label(_page, _a.question, 30, new Vector2(0, 230),
              new Vector2(780, 90), TextAnchor.MiddleCenter).rectTransform;
        for (int i = 0; i < _a.answers.Length; i++)
        {
            int idx = i;
            Btn(_page, _a.answers[i], new Vector2(0, 90 - i * 110f),
                new Vector2(680, 84), Parchment * 0.94f, Ink, 24, () =>
            {
                if (idx == _a.correctAnswer) ShowVictory();
                else Fail(qrt);
            });
        }
    }

    void ShowVictory()
    {
        ClearPage();
        RestorationDirector.Instance.PlayCompletion();
        Label(_page, "✦ KNOWLEDGE RESTORED ✦", 44, new Vector2(0, 140),
              new Vector2(840, 60), TextAnchor.MiddleCenter, GoldDeep,
              FontStyle.Bold);
        Label(_page, "\"" + _a.title + "\" shines again on the Library's " +
              "shelves.\nThe fog retreats a little further.", 26,
              new Vector2(0, 40), new Vector2(760, 90),
              TextAnchor.MiddleCenter, Ink, FontStyle.Italic);
        Label(_page, "+120 XP        +3 words added to your Lexicon", 24,
              new Vector2(0, -60), new Vector2(760, 40),
              TextAnchor.MiddleCenter, Violet, FontStyle.Bold);
        Btn(_page, "Read it again", new Vector2(0, -180),
            new Vector2(300, 64), Gold, Color.white, 24, () =>
        {
            foreach (var k in _a.keywords) k.restored = false;
            ShowPassage();
        });
    }
}
