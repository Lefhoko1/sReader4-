using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SReader.Domains.Assignments.Models
{
    /// <summary>
    /// What a student does with a word in the attempt game. Each interactive word
    /// carries exactly one activity.
    /// </summary>
    public enum ActivityType { None = 0, Define = 1, Illustrate = 2, FillBlank = 3 }

    /// <summary>
    /// One token of page content — a word or the punctuation/space between words.
    /// Interactive words (isWord && activity != None) become the game's tappable,
    /// underlined targets. The structure is intentionally flat and field-based so
    /// Unity's JsonUtility can (de)serialise the whole tree to the content_json
    /// column. (A PDF importer can later emit this same shape per page.)
    /// </summary>
    [Serializable]
    public class ContentToken
    {
        public string text;            // the word, or the separator (space/comma/…)
        public bool isWord;            // false = punctuation/whitespace, never interactive
        public ActivityType activity = ActivityType.None;

        public string definition;                       // Define: the meaning to arrange
        public List<string> imageOptions = new List<string>();  // Illustrate: correct + distractors
        public string correctImage;                     // Illustrate: which option is right
        public int points = 10;

        public bool IsActivity => isWord && activity != ActivityType.None;
    }

    [Serializable]
    public class ContentSentence
    {
        public List<ContentToken> tokens = new List<ContentToken>();
    }

    [Serializable]
    public class ContentPage
    {
        public int pageNumber;
        public string sourcePdf;       // optional provenance for a future PDF importer
        public List<ContentSentence> sentences = new List<ContentSentence>();
        public List<string> imageUrls = new List<string>();   // images lifted off the page

        public IEnumerable<ContentToken> ActivityTokens =>
            sentences.SelectMany(s => s.tokens).Where(t => t.IsActivity);
    }

    [Serializable]
    public class AssignmentContent
    {
        public int version = 1;
        public List<ContentPage> pages = new List<ContentPage>();

        public IEnumerable<ContentToken> AllActivities => pages.SelectMany(p => p.ActivityTokens);
        public int ActivityCount => AllActivities.Count();
        public int TotalPoints => AllActivities.Sum(t => Math.Max(0, t.points));
        public bool HasActivities => ActivityCount > 0;
    }

    /// <summary>(De)serialises <see cref="AssignmentContent"/> to/from the content_json string.</summary>
    public static class AssignmentContentCodec
    {
        public static string ToJson(AssignmentContent content)
            => content == null ? null : JsonUtility.ToJson(content);

        public static AssignmentContent FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            try { return JsonUtility.FromJson<AssignmentContent>(json); }
            catch { return null; }
        }
    }
}
