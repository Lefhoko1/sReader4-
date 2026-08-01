// ===========================================================================
//  WordPathGame — the loop: board -> stones -> challenges -> back to the board
// ===========================================================================
//  1. The passage appears on the SkyBoard / open book as a list of sentences.
//  2. Tap a sentence  -> it lays out along the river as stepping stones.
//  3. Tap a glowing key stone -> the real Define / Fill / Illustrate challenge
//     (checked by the existing, tested GateSolving).
//  4. Solve every key word -> the sentence is ticked off on the board, and the
//     next one can be chosen. Finish them all -> the passage is restored.
//
//  CONTENT: ships with a photosynthesis sample built in the EXACT shape your
//  Supabase content_json already uses (AssignmentContent). To run real tutor
//  content instead, call Load(title, content) with the content your
//  StudentAssignmentsViewModel returns — nothing else changes.
// ===========================================================================
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SReader.Domains.Assignments.Models;

public class WordPathGame : MonoBehaviour
{
    [Header("Wiring")]
    public WordPathBuilder path;
    public SentenceBoard board;

    [Header("Content")]
    public string title = "Photosynthesis";
    [Tooltip("Use the built-in photosynthesis sample. Uncheck to drive real Supabase content via Load().")]
    public bool useSampleContent = true;

    AssignmentContent _content;
    List<ContentSentence> _sentences = new List<ContentSentence>();
    List<WordStone> _stones = new List<WordStone>();
    int _current = -1;

    void Start()
    {
        if (board != null) board.onSentenceChosen += ChooseSentence;
        if (useSampleContent) Load(title, Photosynthesis());
    }

    /// <summary>Drive the game with any assignment content (sample or Supabase).</summary>
    public void Load(string passageTitle, AssignmentContent content)
    {
        _content = content;
        _sentences = content?.pages?.SelectMany(p => p.sentences).ToList() ?? new List<ContentSentence>();
        if (board != null) board.Show(passageTitle, content);
        ClearPath();
    }

    void ChooseSentence(int index)
    {
        if (path == null || index < 0 || index >= _sentences.Count) return;
        _current = index;
        board?.Highlight(index);
        WordChallenge.CloseAny();
        _stones = path.BuildFromSentence(_sentences[index]);

        // every key stone opens its real challenge on tap
        foreach (var s in _stones)
        {
            if (!s.isKeyword || s.token == null) continue;
            var stone = s;
            stone.suppressCard = true;                  // the challenge replaces the callout card
            stone.onWordClicked.RemoveAllListeners();
            stone.onWordClicked.AddListener((w, isKey) =>
            {
                if (stone.solved || WordChallenge.IsOpen) return;
                WordChallenge.Open(stone, () => OnKeyWordSolved(stone));
            });
        }
    }

    void OnKeyWordSolved(WordStone stone)
    {
        bool sentenceDone = _stones.All(s => !s.isKeyword || s.solved);
        if (!sentenceDone) return;

        board?.MarkDone(_current);
        if (board != null && board.AllDone())
            Debug.Log("[WordPath] Passage restored — every sentence complete.");
        else
            Debug.Log($"[WordPath] Sentence {_current + 1} complete — choose the next one on the board.");
    }

    void ClearPath()
    {
        if (path == null) return;
        // Ask the builder rather than deleting its children: when the river is
        // pooled those children ARE the pool, and destroying them here would throw
        // away the very thing that makes a sentence change cheap.
        path.Clear();
        _stones.Clear();
    }

    // ── the photosynthesis sample (same shape as Supabase content_json) ──────

    public static AssignmentContent Photosynthesis()
    {
        var c = new AssignmentContent();
        var page = new ContentPage { pageNumber = 1 };

        page.sentences.Add(S(
            W("Green"), W("plants"), W("make"), W("their"), W("own"), W("food"), W("using"),
            Key("sunlight", ActivityType.Define, "light that comes from the sun")));

        page.sentences.Add(S(
            W("The"), W("leaves"), W("take"), W("in"), W("a"), W("gas"), W("called"),
            Key("carbon", ActivityType.FillBlank), W("dioxide"), W("from"), W("the"), W("air")));

        // two key words in one sentence — the sentence is only ticked off once BOTH
        // are restored (OnKeyWordSolved waits for every key stone)
        page.sentences.Add(S(
            W("Roots"), W("pull"),
            Key("water", ActivityType.Illustrate,
                images: new[] { "a clear stream", "a dry stone", "a candle", "a closed book" },
                correct: "a clear stream"),
            W("up"), W("from"), W("the"),
            Key("soil", ActivityType.Define, "the ground that plants grow in")));

        page.sentences.Add(S(
            W("Inside"), W("each"), W("leaf"), W("a"), W("green"), W("colour"), W("called"),
            Key("chlorophyll", ActivityType.FillBlank), W("traps"), W("the"), W("light")));

        page.sentences.Add(S(
            W("The"), W("plant"), W("makes"),
            Key("sugar", ActivityType.Define, "a sweet food that gives energy"),
            W("and"), W("lets"), W("out"), W("oxygen")));

        c.pages.Add(page);
        return c;
    }

    static ContentSentence S(params ContentToken[] toks)
    { var s = new ContentSentence(); s.tokens.AddRange(toks); return s; }

    static ContentToken W(string text) =>
        new ContentToken { text = text, isWord = true, activity = ActivityType.None };

    static ContentToken Key(string text, ActivityType a, string definition = null,
                            string[] images = null, string correct = null)
    {
        var t = new ContentToken { text = text, isWord = true, activity = a, points = 10, definition = definition };
        if (images != null) { t.imageOptions = images.ToList(); t.correctImage = correct; }
        return t;
    }
}
