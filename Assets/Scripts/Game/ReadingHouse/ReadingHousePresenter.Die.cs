using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using SReader.Domains.Assignments.Models;
using SReader.Game.Trek;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The die game: reading at the dashboard with one die on the rug instead of a rug of stones.
    ///
    ///   • The dashboard's display becomes the game screen (<see cref="DieGamePanel"/>): the sentence being
    ///     read, styled as in the app's reading game (fill-in words a blank, define words gold, picture words
    ///     pink, restored words green), with the words picked so far bright and the rest dim.
    ///   • The student rebuilds the sentence word by word with the die: its three visible faces show the next
    ///     word and two decoys; a right pick lights the word up and the die rolls on, a wrong one makes it shake
    ///     its head. A streak counter climbs while they keep going. Everyone in the room watches the die.
    ///   • At a key word the die becomes the activity, with the same checking (GateSolving), hint ladder,
    ///     Guide me and Later as the Classroom's panels:
    ///       Define: the definition's pieces, shuffled, on the die; flip until the next piece shows, tap it and
    ///       it drops into its place in the definition on the display;
    ///       Fill: the word's letters, the same way;
    ///       Illustrate: the pictures, large and numbered on the display, and on the die's faces; tap the right one.
    ///   • Each key word restored makes the die more precious (wood → silver → gold → crystal → rainbow
    ///     gem); long streaks make it sparkle. A finished sentence: the die hops, the next sentence comes.
    /// Points, hints, Later, the timer and saving all go through TrekSession exactly as before; the streak
    /// and the die's tiers are presentation only.
    /// </summary>
    public sealed partial class ReadingHousePresenter
    {
        [Header("Die game (replaces the stones when set)")]
        public ReadingDie die;

        DieGamePanel dieBoard;                                         // on the dashboard's display while playing
        bool DieMode => die != null && dashboardScreen != null && Application.isPlaying;

        enum DieStep { Off, Sentence, Gate, Done }
        DieStep step;
        int buildIndex;
        readonly HashSet<int> builtSentences = new HashSet<int>();
        readonly List<ReadingDie.Choice> faceChoices = new List<ReadingDie.Choice>();
        readonly List<string> faceKeys = new List<string>();      // what each face means (word / piece / image url)
        int streak;
        float nextNudge;
        StudentSpriteAnimator studentLook;
        Transform encouragerUsualTarget;

        // the key word being restored on the die
        GateData dieGate;
        ContentToken token;
        List<string> correct, pool, answer, options;
        int locked, wrongTries, window;
        bool guided;
        string clueText;
        readonly Dictionary<string, Texture2D> pictures = new Dictionary<string, Texture2D>();

        /// <summary>True while a key word's activity runs on the die (the timer runs, as with the panels).</summary>
        bool DieGateOpen => dieGate != null;

        void OpenDieGame()
        {
            if (!DieMode) return;
            if (!dashboardScreen.IsOpen) OpenDashboardScreen();
            if (dieBoard == null)
            {
                var root = dashboardScreen.page.rootVisualElement;
                dieBoard = new DieGamePanel(root);
                dieBoard.FlipPressed += OnDieFlip;
                dieBoard.HintPressed += DieHint;
                dieBoard.GuidePressed += DieGuide;
                dieBoard.LaterPressed += DieLater;
                dieBoard.ReadPressed += OpenReader;
                dieBoard.FinishPressed += FinishAttempt;
                if (dashboardScreen.Host != null) dashboardScreen.Host.BringToFront();   // the reader still opens on top
            }
            if (dashboard.sentenceText != null) dashboard.sentenceText.enabled = false;   // the panel shows the sentence
            die.gameObject.SetActive(true);
            if (matStones != null) matStones.HideAll();
            tray.HideAll();
            LookAtDie(true);
            BeginSentence();
        }

        void CloseDieGame()
        {
            if (die == null) return;
            dieBoard?.Remove();
            dieBoard = null;
            if (dashboard != null && dashboard.sentenceText != null) dashboard.sentenceText.enabled = true;
            die.gameObject.SetActive(false);
            dieGate = null; step = DieStep.Off;
            LookAtDie(false);
        }

        /// <summary>While the die is played, the student and both helpers turn to watch it.</summary>
        void LookAtDie(bool on)
        {
            if (studentLook == null) studentLook = FindAnyObjectByType<StudentSpriteAnimator>();
            if (studentLook != null) studentLook.lookAt = on ? die.transform : null;
            if (encourager != null && encourager.actor != null)
            {
                if (on) { if (encourager.actor.faceTarget != die.transform) encouragerUsualTarget = encourager.actor.faceTarget; encourager.actor.faceTarget = die.transform; }
                else if (encouragerUsualTarget != null) encourager.actor.faceTarget = encouragerUsualTarget;
            }
            if (zombie != null) zombie.faceTarget = on ? die.transform : null;
        }

        /// <summary>Keep the panel in step every frame (Finish appears when everything is restored; hidden under the reader).</summary>
        void UpdateDiePanel()
        {
            if (dieBoard == null) return;
            dieBoard.SetVisible(!reader.IsOpen);
            dieBoard.SetFinish(IsLive && CanFinish);
        }

        /// <summary>A new sentence: nothing picked yet, and the first word's choices.</summary>
        void BeginSentence()
        {
            if (!DieMode || dieBoard == null || trail == null || session == null) return;
            dieGate = null; clueText = null;
            buildIndex = 0;
            step = DieStep.Sentence;
            die.SetTier(TierNow());
            die.Glow(-1);
            dieBoard.SetTitle($"Sentence {sentenceIndex + 1} of {trail.Sentences.Count}");
            dieBoard.SetClue("Read the sentence, then tap the word that comes next on the die.");
            dieBoard.SetLine(null, false);
            dieBoard.SetPictures(null);
            dieBoard.SetStreak(streak);
            Advance(roll: false);
        }

        int TierNow() => session == null ? 0 : session.SolvedGates;

        static string Clean(string word)
        {
            var sb = new StringBuilder();
            foreach (var c in word) if (char.IsLetterOrDigit(c) || c == '\'' || c == '-') sb.Append(c);
            return sb.Length > 0 ? sb.ToString() : word;
        }

        void ShowProgress() => dieBoard.SetSentence(StyledSentence(buildIndex));

        /// <summary>Move along the sentence to the next thing to do: a word to pick, a key word's activity, or the end.</summary>
        void Advance(bool roll)
        {
            bool revisit = builtSentences.Contains(sentenceIndex);
            while (buildIndex < words.Count)
            {
                var gate = GateInWord(buildIndex);
                if (gate == null)
                {
                    if (revisit) { buildIndex++; continue; }
                    ShowProgress();
                    PresentWord(words[buildIndex].text, roll);
                    return;
                }
                if (Solved(gate)) { buildIndex++; continue; }
                var next = session.ActiveGate ?? session.PeekNextGate();
                if (next == gate) { ShowProgress(); OpenDieGate(gate, roll); return; }
                buildIndex++;                                           // saved for Later: the sentence goes on
            }
            SentenceDone();
        }

        void PresentWord(string word, bool roll)
        {
            dieBoard.SetTitle($"Sentence {sentenceIndex + 1} of {trail.Sentences.Count}");
            dieBoard.SetLine(null, false);
            dieBoard.SetPictures(null);
            dieBoard.SetButtons(true, false, false, false, true);
            faceChoices.Clear(); faceKeys.Clear();
            var answerWord = Clean(word);
            var picks = new List<string> { answerWord };
            foreach (var d in Decoys(answerWord, 2)) picks.Add(d);
            Shuffle(picks);
            foreach (var p in picks) { faceChoices.Add(ReadingDie.Choice.Word(p)); faceKeys.Add(p); }
            if (roll) die.Roll(faceChoices); else die.Show(faceChoices);
        }

        /// <summary>Two decoys from elsewhere in the passage: real words, not the answer, of a similar length.</summary>
        List<string> Decoys(string answerWord, int n)
        {
            var all = new List<string>();
            foreach (var w in trail.PassageText.Split(new[] { ' ', '\n', '\t' }, System.StringSplitOptions.RemoveEmptyEntries))
            {
                var c = Clean(w);
                if (c.Length == 0 || string.Equals(c, answerWord, System.StringComparison.OrdinalIgnoreCase)) continue;
                if (all.Exists(x => string.Equals(x, c, System.StringComparison.OrdinalIgnoreCase))) continue;
                all.Add(c);
            }
            all.Sort((a, b) => Mathf.Abs(a.Length - answerWord.Length).CompareTo(Mathf.Abs(b.Length - answerWord.Length)));
            var near = all.GetRange(0, Mathf.Min(all.Count, 10));
            Shuffle(near);
            var result = near.GetRange(0, Mathf.Min(n, near.Count));
            string[] spare = { "the", "and", "blue", "jump", "river", "happy", "under" };
            for (int i = 0; result.Count < n && i < spare.Length; i++)
                if (!string.Equals(spare[i], answerWord, System.StringComparison.OrdinalIgnoreCase) && !result.Contains(spare[i])) result.Add(spare[i]);
            return result;
        }

        static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--) { int j = Random.Range(0, i + 1); (list[i], list[j]) = (list[j], list[i]); }
        }

        void SentenceDone()
        {
            step = DieStep.Done;
            builtSentences.Add(sentenceIndex);
            ShowProgress();
            dieBoard.SetClue("Sentence complete!");
            dieBoard.SetButtons(false, false, false, false, true);
            faceChoices.Clear(); faceKeys.Clear();
            die.Show(faceChoices);
            die.Celebrate();
            if (encourager != null) encourager.GoodPick();
            StartCoroutine(NextSentenceSoon());
        }

        IEnumerator NextSentenceSoon()
        {
            yield return new WaitForSeconds(1.4f);
            if (session == null || trail == null || dieBoard == null) yield break;
            var next = session.ActiveGate ?? session.PeekNextGate();
            if (sentenceIndex >= trail.Sentences.Count - 1)
            {
                if (next != null) { ShowSentence(next.SentenceIndex, announce: false); yield break; }   // back to a word saved for Later
                dieBoard.SetTitle("Passage complete!");
                dieBoard.SetClue(IsLive && CanFinish ? "Every word restored: tap Finish & submit." : "Every word restored!");
                if (encourager != null) encourager.Finished();
                yield break;
            }
            ShowSentence(sentenceIndex + 1, announce: false);
        }

        // ── picks ──────────────────────────────────────────────────────────

        /// <summary>A tap on the die (forwarded by ReadingHouseInput): which face's choice, −1 if none.</summary>
        public void OnDieTapped(int choice)
        {
            if (!DieMode || dieBoard == null || choice < 0 || choice >= faceKeys.Count || submitting) return;
            var pick = faceKeys[choice];
            if (step == DieStep.Sentence)
            {
                var want = Clean(words[buildIndex].text);
                if (string.Equals(pick, want, System.StringComparison.OrdinalIgnoreCase))
                {
                    Right();
                    buildIndex++;
                    Advance(roll: true);
                }
                else Wrong();
                return;
            }
            if (step == DieStep.Gate && dieGate != null) GatePick(pick);
        }

        void Right()
        {
            streak++;
            dieBoard.SetStreak(streak);
            die.SetSparkle(streak >= 10 ? 2 : streak >= 5 ? 1 : 0);
            dieBoard.Flash(true);
            if (encourager != null && streak > 0 && streak % 8 == 0) encourager.GoodPick();
        }

        void Wrong()
        {
            streak = 0;
            dieBoard.SetStreak(0);
            die.SetSparkle(0);
            die.Shake();
            dieBoard.Flash(false);
            if (encourager != null && Time.time >= nextNudge) { encourager.Nudge(); nextNudge = Time.time + 6f; }
        }

        /// <summary>Flip: roll the die to show other choices (the next pieces / pictures), without picking.</summary>
        public void OnDieFlip() => OnDieFlip(1);

        public void OnDieFlip(int direction)
        {
            if (!DieMode || dieBoard == null || die.Busy) return;
            if (step == DieStep.Gate && dieGate != null) { window += 3; PresentGateChoices(roll: true, sideways: true, direction); return; }
            if (step == DieStep.Sentence)
            {
                // the same three words, shuffled onto other faces
                var picks = new List<string>(faceKeys); Shuffle(picks);
                faceChoices.Clear(); faceKeys.Clear();
                foreach (var p in picks) { faceChoices.Add(ReadingDie.Choice.Word(p)); faceKeys.Add(p); }
                die.Roll(faceChoices, sideways: true, direction);
            }
        }

        // ── a key word on the die ───────────────────────────────────────────

        void OpenDieGate(GateData gate, bool roll)
        {
            var current = session.ActiveGate ?? session.ArriveAtGate();
            if (current != gate) { buildIndex++; Advance(roll); return; }
            dieGate = gate;
            token = gate.ChallengePayload as ContentToken;
            step = DieStep.Gate;
            wrongTries = 0; window = 0; locked = 0; clueText = null;
            guided = gate.HintsUsed >= 3;
            answer = new List<string>();
            if (token == null)
            {
                // A malformed key word never traps the student (the panels wave it through the same way).
                dieBoard.SetClue("This word is broken: waving you through.");
                SolveDieGate();
                return;
            }
            if (encourager != null) encourager.GoodPick();
            switch (gate.Type)
            {
                case GateType.Illustrate:
                    options = GateSolving.IllustrateOptions(token);
                    foreach (var url in options) LoadPicture(url);
                    dieBoard.SetTitle("Illustrate: " + token.text);
                    break;
                case GateType.Define:
                    correct = GateSolving.DefineCorrect(token); pool = GateSolving.DefinePool(token);
                    dieBoard.SetTitle("Define: " + token.text);
                    break;
                default:
                    correct = GateSolving.FillCorrect(token); pool = GateSolving.FillPool(token);
                    dieBoard.SetTitle("Fill in the hidden word");
                    break;
            }
            dieBoard.SetHintLabel(HintLabel(gate.HintsUsed));
            if (gate.HintsUsed == 1) ApplyDieHint(1);
            else if (gate.HintsUsed >= 2) ApplyDieHint(2);
            if (guided) EnterDieGuided();
            RefreshGateBoard();
            PresentGateChoices(roll, sideways: false, 1);
        }

        bool Illustrating => dieGate != null && dieGate.Type == GateType.Illustrate;
        bool Filling => dieGate != null && dieGate.Type != GateType.Illustrate && dieGate.Type != GateType.Define;

        static string HintLabel(int level) => level <= 0 ? "Hint (−40%)" : "Hint (−70%)";

        /// <summary>The display during a key word: its slots filling in and what is left to place, or the pictures.</summary>
        void RefreshGateBoard(bool pop = false)
        {
            if (Illustrating)
            {
                dieBoard.SetLine(null, false);
                var tex = new List<Texture2D>();
                int glow = -1;
                for (int i = 0; i < options.Count; i++)
                {
                    pictures.TryGetValue(options[i], out var t); tex.Add(t);
                    if (guided && options[i] == token.correctImage) glow = i;
                }
                dieBoard.SetPictures(tex, glow);
                if (string.IsNullOrEmpty(clueText)) clueText = "Which picture shows “" + token.text + "”? Flip the die to its number and tap it.";
                dieBoard.SetClue(clueText);
            }
            else
            {
                dieBoard.SetPictures(null);
                var line = new List<(string, DieGamePanel.Piece)>();
                for (int i = 0; i < correct.Count; i++)
                    line.Add(i < answer.Count ? ((Filling ? answer[i].ToUpperInvariant() : answer[i]), DieGamePanel.Piece.Word)
                                              : ((Filling ? "_" : "____"), DieGamePanel.Piece.Slot));
                dieBoard.SetLine(line, pop);
                if (string.IsNullOrEmpty(clueText))
                    clueText = Filling ? "Flip the die and tap the letters in order." : "Flip the die and tap the definition's words in order.";
                var left = pool.Count == 0 ? "" : "   <color=#FFD666>Still to place:</color> " + string.Join("  ·  ", Filling ? pool.ConvertAll(p => p.ToUpperInvariant()) : pool);
                dieBoard.SetClue(clueText + left);
            }
            bool canHint = dieGate.HintsUsed < 2 && !guided;
            bool canGuide = !guided && (dieGate.HintsUsed >= 2 || wrongTries >= 2);
            dieBoard.SetButtons(true, canHint, canGuide, true, true);
        }

        /// <summary>Put the next window of pieces / pictures on the die (guided: the glowing next piece among them).</summary>
        void PresentGateChoices(bool roll, bool sideways, int direction)
        {
            faceChoices.Clear(); faceKeys.Clear();
            int glow = -1;
            if (Illustrating)
            {
                var list = options;
                int count = Mathf.Min(3, list.Count);
                for (int k = 0; k < count; k++)
                {
                    int idx = (window + k) % list.Count;
                    AddPicture(idx);
                    if (guided && list[idx] == token.correctImage) glow = k;
                }
                if (guided && glow < 0 && list.Contains(token.correctImage))
                {
                    faceChoices.RemoveAt(0); faceKeys.RemoveAt(0);
                    AddPicture(list.IndexOf(token.correctImage));
                    glow = faceKeys.Count - 1;
                }
            }
            else
            {
                var distinct = new List<string>();
                for (int k = 0; k < pool.Count && distinct.Count < 3; k++)
                {
                    var p = pool[(window + k) % pool.Count];
                    if (!distinct.Contains(p)) distinct.Add(p);
                }
                if (guided && answer.Count < correct.Count)
                {
                    var want = correct[answer.Count];
                    int at = distinct.FindIndex(p => Same(p, want));
                    if (at < 0) { if (distinct.Count == 3) distinct[0] = want; else distinct.Add(want); at = distinct.FindIndex(p => Same(p, want)); }
                    glow = at;
                }
                foreach (var p in distinct) { faceChoices.Add(ReadingDie.Choice.Word(Filling ? p.ToUpperInvariant() : p)); faceKeys.Add(p); }
            }
            if (roll) die.Roll(faceChoices, sideways, direction); else die.Show(faceChoices);
            die.Glow(glow);
        }

        void AddPicture(int idx)
        {
            var url = options[idx];
            pictures.TryGetValue(url, out var tex);
            faceChoices.Add(tex != null ? ReadingDie.Choice.Picture(tex, idx + 1) : ReadingDie.Choice.Word((idx + 1).ToString()));
            faceKeys.Add(url);
        }

        bool Same(string a, string b) => Filling ? string.Equals(a, b, System.StringComparison.OrdinalIgnoreCase) : a == b;

        void GatePick(string pick)
        {
            if (Illustrating)
            {
                if (guided && pick != token.correctImage) return;
                if (GateSolving.CheckIllustrate(pick, token)) { Right(); SolveDieGate(); return; }
                wrongTries++;
                Wrong();
                RefreshGateBoard();
                return;
            }
            if (answer.Count >= correct.Count) return;
            var want = correct[answer.Count];
            if (!Same(pick, want))
            {
                if (guided) return;                                  // guided: only the glowing piece
                wrongTries++;
                Wrong();
                RefreshGateBoard();
                return;
            }
            Right();
            answer.Add(pick);
            int inPool = pool.FindIndex(p => Same(p, pick));
            if (inPool >= 0) pool.RemoveAt(inPool);
            if (answer.Count == correct.Count)
            {
                bool ok = Filling ? GateSolving.CheckFill(answer, correct) : GateSolving.CheckDefine(answer, correct);
                if (ok) { RefreshGateBoard(pop: true); SolveDieGate(); return; }
            }
            window = 0;
            RefreshGateBoard(pop: true);
            PresentGateChoices(roll: true, sideways: false, 1);
        }

        void SolveDieGate()
        {
            session.SolveActiveGate();
            Save();
            dieGate = null; clueText = null;
            buildIndex++;
            step = DieStep.Sentence;
            die.Glow(-1);
            die.SetTier(TierNow());
            die.Celebrate();
            dashboard.SetSentence(StyledSentence());                 // the restored word turns green
            dashboard.SetPoints(session.Points);
            if (chalkboard != null) chalkboard.Show(trail);
            OnWordSolved();
            dieBoard.SetPictures(null);
            dieBoard.SetLine(null, false);
            ShowProgress();
            dieBoard.SetTitle($"Sentence {sentenceIndex + 1} of {trail.Sentences.Count}");
            dieBoard.SetClue("Restored! Keep going: tap the word that comes next.");
            StartCoroutine(ContinueAfter(0.9f));
        }

        IEnumerator ContinueAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (step == DieStep.Sentence && dieBoard != null) Advance(roll: true);
        }

        void DieLater()
        {
            if (dieGate == null) return;
            session.DeferActiveGate();
            EndGateAsLater("Saved for Later: on to the next word.");
        }

        /// <summary>The key word waits in Later (the student chose it, or its time ran out): the sentence goes on.</summary>
        void EndGateAsLater(string message)
        {
            Save();
            dieGate = null; clueText = null;
            die.Glow(-1);
            buildIndex++;
            step = DieStep.Sentence;
            dieBoard.SetPictures(null);
            dieBoard.SetLine(null, false);
            Say(message);
            Advance(roll: true);
        }

        void DieHint()
        {
            if (dieGate == null || guided) return;
            int level = session.UseHint();
            Save();
            dieBoard.SetHintLabel(HintLabel(level));
            ApplyDieHint(level);
            RefreshGateBoard();
            PresentGateChoices(roll: true, sideways: true, 1);
        }

        /// <summary>The hint ladder (R-8), as on the Classroom's panels: a clue, then a head start.</summary>
        void ApplyDieHint(int level)
        {
            if (level == 1)
            {
                if (Illustrating)
                    clueText = "💡 " + (!string.IsNullOrWhiteSpace(token.definition) ? token.definition : "Think about what “" + token.text + "” means.");
                else
                    clueText = Filling
                        ? "💡 It has " + correct.Count + " letters and starts with “" + (correct.Count > 0 ? correct[0].ToUpperInvariant() : "?") + "”."
                        : "💡 It has " + correct.Count + " words and starts with “" + (correct.Count > 0 ? correct[0] : "?") + "”.";
            }
            else if (level >= 2)
            {
                if (Illustrating)
                {
                    foreach (var gone in GateSolving.IllustrateEliminations(options, token)) options.Remove(gone);
                    clueText = "💡 Two pictures are out.";
                }
                else
                {
                    int head = Filling ? GateSolving.FillRevealIndices(correct).Count : Mathf.Min(GateSolving.DefineLockedPieces, correct.Count);
                    for (int i = answer.Count; i < head; i++)
                    {
                        answer.Add(correct[i]);
                        int inPool = pool.FindIndex(p => Same(p, correct[i]));
                        if (inPool >= 0) pool.RemoveAt(inPool);
                    }
                    locked = head;
                    clueText = "💡 A head start: finish it off!";
                }
            }
        }

        void DieGuide()
        {
            if (dieGate == null || guided) return;
            session.RequestGuidedSolve();
            Save();
            EnterDieGuided();
            RefreshGateBoard();
            PresentGateChoices(roll: true, sideways: true, 1);
        }

        void EnterDieGuided()
        {
            guided = true;
            clueText = Illustrating ? "The glowing picture is the one: tap it."
                     : Filling ? "The word is “" + string.Join("", correct).ToUpperInvariant() + "”: tap the glowing letters in order."
                     : "“" + string.Join(" ", correct) + "”: tap the glowing words in order.";
        }

        /// <summary>The key word's timer ran out (TrekSession already moved it to Later).</summary>
        void DieGateTimedOut() => EndGateAsLater("No rush: that word will wait in Later.");

        // ── pictures for Illustrate ─────────────────────────────────────────

        void LoadPicture(string url)
        {
            if (string.IsNullOrEmpty(url) || pictures.ContainsKey(url)) return;
            pictures[url] = null;
            StartCoroutine(Fetch(url));
        }

        IEnumerator Fetch(string url)
        {
            using (var req = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url))
            {
                yield return req.SendWebRequest();
                if (req.result != UnityEngine.Networking.UnityWebRequest.Result.Success) yield break;
                pictures[url] = UnityEngine.Networking.DownloadHandlerTexture.GetContent(req);
            }
            if (Illustrating && dieBoard != null)
            {
                RefreshGateBoard();
                if (faceKeys.Contains(url)) PresentGateChoices(roll: false, sideways: false, 1);
            }
        }
    }
}
