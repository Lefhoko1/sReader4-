using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SReader.Domains.Assignments.Models
{
    /// <summary>
    /// Turns plain page text into the structured <see cref="ContentPage"/> the
    /// authoring tool and game share, plus the shuffles the mini-games need.
    /// (A PDF importer can reuse <see cref="PageFromText"/> on extracted text, or
    /// build pages directly with word/image positions.)
    /// </summary>
    public static class AssignmentContentBuilder
    {
        /// <summary>Tokenise text into sentences of word / separator tokens.</summary>
        public static ContentPage PageFromText(int pageNumber, string text)
        {
            var page = new ContentPage { pageNumber = pageNumber };
            if (string.IsNullOrWhiteSpace(text)) return page;

            var sentence = new ContentSentence();
            var buffer = new StringBuilder();
            bool bufferIsWord = false;

            void Flush()
            {
                if (buffer.Length == 0) return;
                sentence.tokens.Add(new ContentToken { text = buffer.ToString(), isWord = bufferIsWord });
                buffer.Clear();
            }

            void EndSentence()
            {
                Flush();
                if (sentence.tokens.Count > 0) page.sentences.Add(sentence);
                sentence = new ContentSentence();
            }

            foreach (var ch in text.Replace("\r", ""))
            {
                bool isWordChar = char.IsLetterOrDigit(ch);
                if (isWordChar != bufferIsWord && buffer.Length > 0) Flush();
                bufferIsWord = isWordChar;
                buffer.Append(ch);

                if (ch == '.' || ch == '!' || ch == '?' || ch == '\n')
                    EndSentence();
            }
            EndSentence();
            return page;
        }

        /// <summary>The definition split into word tokens, shuffled (for the Define game).</summary>
        public static List<string> ShuffleWords(string definition)
        {
            var words = (definition ?? "")
                .Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            return ShuffledCopyDistinctOrder(words);
        }

        /// <summary>The word's letters, shuffled (for the Fill-the-blanks game).</summary>
        public static List<string> ShuffleLetters(string word)
        {
            var letters = (word ?? "").Where(char.IsLetterOrDigit).Select(c => c.ToString()).ToList();
            return ShuffledCopyDistinctOrder(letters);
        }

        public static List<string> ShuffledCopy(IEnumerable<string> items)
        {
            var list = items?.ToList() ?? new List<string>();
            var rng = new Random();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
            return list;
        }

        // Shuffle but, when there are 2+ items, avoid returning the original order.
        static List<string> ShuffledCopyDistinctOrder(List<string> items)
        {
            if (items.Count < 2) return new List<string>(items);
            var original = string.Join("", items);
            for (int attempt = 0; attempt < 6; attempt++)
            {
                var shuffled = ShuffledCopy(items);
                if (string.Join("", shuffled) != original) return shuffled;
            }
            return ShuffledCopy(items);
        }
    }
}
