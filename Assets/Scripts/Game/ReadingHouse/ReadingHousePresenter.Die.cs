using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using SReader.Domains.Assignments.Models;
using SReader.Game.Trek;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The die game: reading the passage at the dashboard, with one die on the floor for the key words.
    ///
    ///   • The dashboard's display (<see cref="DieGamePanel"/>) shows the WHOLE passage, its key words marked as in
    ///     the app's reading game: a fill-in word shows only some of its letters, a word to define is gold and
    ///     underlined, a word to illustrate is pink and underlined with a picture mark; restored words turn green.
    ///     The student reads, and taps a key word (it bursts with the Word Path keyword sparkle).
    ///   • The display wipes to that word's PLAY board: its sentence, the answer slots and the pieces, shuffled
    ///     (the definition's words, the word's letters) or the pictures, numbered. The die carries only those
    ///     pieces: it shuffles itself every couple of seconds (spinning, sparkles round it) and the student taps
    ///     it when it shows the right next piece, which flies into its slot. Hint, Guide me and Later as on the
    ///     Classroom's panels. Restored: the board celebrates and "Back to the passage" wipes back.
    ///   • Key words are restored in the passage's order (TrekSession decides which is next; it glows):
    ///     tapping another gets a friendly nudge. Every key word restored makes the die more precious (wood →
    ///     silver → gold → crystal → rainbow gem); a streak of right taps makes it sparkle; the candy board
    ///     behind the display lights its gumballs and throws confetti.
    /// Checking (GateSolving), points, hints, Later, the timer and saving all go through TrekSession exactly as
    /// before; the die, streak and confetti are presentation only.
    /// </summary>
    public sealed partial class ReadingHousePresenter
    {
        [Header("Die game (replaces the stones when set)")]
        public ReadingDie die;
        [Tooltip("Seconds between the die's self-shuffles while a key word is played.")]
        public float dieShuffleSeconds = 2.0f;

        DieGamePanel dieBoard;                                         // on the dashboard's display while reading
        bool DieMode => die != null && dashboardScreen != null && Application.isPlaying;

        readonly HashSet<int> builtSentences = new HashSet<int>();   // kept for the shared flow (unused by the die game)
        readonly List<ReadingDie.Choice> faceChoices = new List<ReadingDie.Choice>();
        readonly List<string> faceKeys = new List<string>();      // what the face showing means (piece / image url)
        int streak;
        float nextNudge;
        StudentSpriteAnimator studentLook;
        Transform encouragerUsualTarget;
        CandyBoard candy;

        // the key word being played on the die
        GateData dieGate;
        ContentToken token;
        List<string> correct, pool, answer, options;
        int locked, wrongTries, lastShown = -1;
        bool guided, solvedShowing, flying;                            // flying: a picked piece is on its way to its slot
        string clueText;
        Coroutine shuffler;
        readonly Dictionary<string, Texture2D> pictures = new Dictionary<string, Texture2D>();

        /// <summary>True while a key word is played on the die (its timer runs, as with the panels).</summary>
        bool DieGateOpen => dieGate != null && !solvedShowing;

        float KeyProgress() => session == null || session.TotalGates == 0 ? 0f : session.SolvedGates / (float)session.TotalGates;

        void OpenDieGame()
        {
            if (!DieMode) return;
            if (!dashboardScreen.IsOpen) OpenDashboardScreen();
            if (dieBoard == null)
            {
                var root = dashboardScreen.page.rootVisualElement;
                dieBoard = new DieGamePanel(root);
                dieBoard.KeyWordPressed += OnKeyWordPressed;
                dieBoard.HintPressed += DieHint;
                dieBoard.GuidePressed += DieGuide;
                dieBoard.LaterPressed += DieLater;
                dieBoard.BackPressed += BackToPassage;
                dieBoard.FinishPressed += FinishAttempt;
                if (dashboardScreen.Host != null) dashboardScreen.Host.BringToFront();   // the reader still opens on top
            }
            if (dashboard.sentenceText != null) dashboard.sentenceText.enabled = false;   // the panel shows the passage
            die.gameObject.SetActive(true);
            if (candy == null) candy = FindAnyObjectByType<CandyBoard>();
            if (matStones != null) matStones.HideAll();
            tray.HideAll();
            LookAtDie(true);
            ShowPassage();
        }

        void CloseDieGame()
        {
            if (die == null) return;
            StopShuffle();
            dieBoard?.Remove();
            dieBoard = null;
            if (dashboard != null && dashboard.sentenceText != null) dashboard.sentenceText.enabled = true;
            die.gameObject.SetActive(false);
            dieGate = null; solvedShowing = false;
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

        /// <summary>Every frame: Finish appears when everything is restored; the panel hides under the reader.</summary>
        void UpdateDiePanel()
        {
            if (dieBoard == null) return;
            if (dashboard.sentenceText != null && dashboard.sentenceText.enabled) dashboard.sentenceText.enabled = false;
            dieBoard.SetVisible(!reader.IsOpen);
            dieBoard.SetFinish(IsLive && CanFinish);
        }

        /// <summary>Called when the shared flow shows a sentence: in the die game the passage simply refreshes.</summary>
        void BeginSentence()
        {
            if (dieBoard != null && !dieBoard.Playing) ShowPassage();
        }

        // ── the passage ─────────────────────────────────────────────────────

        static string Clean(string word)
        {
            var sb = new StringBuilder();
            foreach (var c in word) if (char.IsLetterOrDigit(c) || c == '\'' || c == '-') sb.Append(c);
            return sb.Length > 0 ? sb.ToString() : word;
        }

        /// <summary>A fill-in word with only some letters showing (every third), so it reads as a word to complete.</summary>
        static string PartlyHidden(string word)
        {
            var sb = new StringBuilder();
            int letter = 0;
            foreach (var c in word)
            {
                if (!char.IsLetterOrDigit(c)) { sb.Append(c); continue; }
                sb.Append(letter % 3 == 0 ? c : '_');
                letter++;
            }
            return sb.ToString();
        }

        /// <summary>The whole passage, word by word, key words marked; the next one to restore glows.</summary>
        void ShowPassage()
        {
            if (dieBoard == null || trail == null || session == null) return;
            StopShuffle();
            dieGate = null; solvedShowing = false;
            var next = session.ActiveGate ?? session.PeekNextGate();
            var list = new List<DieGamePanel.Word>();
            string text = trail.PassageText;
            int i = 0;
            while (i < text.Length)
            {
                while (i < text.Length && char.IsWhiteSpace(text[i])) i++;
                int start = i;
                while (i < text.Length && !char.IsWhiteSpace(text[i])) i++;
                if (i <= start) continue;
                var w = text.Substring(start, i - start);
                int gi = -1;
                for (int g = 0; g < trail.Gates.Count; g++)
                    if (trail.Gates[g].PassageCharIndex >= start && trail.Gates[g].PassageCharIndex < i) { gi = g; break; }
                if (gi < 0) { list.Add(new DieGamePanel.Word { text = w, mark = DieGamePanel.Mark.Plain, key = -1 }); continue; }
                var gate = trail.Gates[gi];
                var mark = Solved(gate) ? DieGamePanel.Mark.Restored
                         : gate.Type == GateType.Illustrate ? DieGamePanel.Mark.Illustrate
                         : gate.Type == GateType.Define ? DieGamePanel.Mark.Define : DieGamePanel.Mark.Fill;
                list.Add(new DieGamePanel.Word
                {
                    text = mark == DieGamePanel.Mark.Fill ? PartlyHidden(w) : w,
                    mark = mark, key = gi, next = gate == next && !Solved(gate),
                });
            }
            dieBoard.SetPassage(list);
            dieBoard.SetPassageTitle(trail.Title, $"{session.SolvedGates} of {session.TotalGates} key words restored");
            dieBoard.ShowPassage();
            if (candy != null) candy.SetProgress(KeyProgress());
            // the die waits, showing a question mark
            faceChoices.Clear(); faceKeys.Clear();
            faceChoices.Add(ReadingDie.Choice.Word("?"));
            die.Glow(-1);
            die.Show(faceChoices);
        }

        /// <summary>A key word on the passage was tapped.</summary>
        void OnKeyWordPressed(int gateIndex)
        {
            if (session == null || trail == null || gateIndex < 0 || gateIndex >= trail.Gates.Count || submitting) return;
            var gate = trail.Gates[gateIndex];
            BurstAt(dieBoard.KeyElement(gateIndex));
            if (Solved(gate)) { Say($"“{gate.Word}” is already restored!"); return; }
            var next = session.ActiveGate ?? session.PeekNextGate();
            if (next != gate)
            {
                if (encourager != null && Time.time >= nextNudge) { encourager.Nudge(); nextNudge = Time.time + 4f; }
                Say("Let's restore the glowing word first!");
                return;
            }
            OpenPlay(gate);
        }

        /// <summary>The Word Path keyword sparkle, at a key word on the display (and round the die).</summary>
        void BurstAt(UnityEngine.UIElements.VisualElement element)
        {
            if (element != null && dashboardScreen != null && dashboardScreen.viewCamera != null)
            {
                var screen = dashboardScreen.ScreenPointOf(element);
                var ray = dashboardScreen.viewCamera.ScreenPointToRay(screen);
                var col = dashboardScreen.GetComponent<Collider>();
                if (col != null && col.Raycast(ray, out var hit, 100f)) WordStone.PlayKeywordBurst(hit.point - ray.direction * 0.05f);
            }
            if (die != null) WordStone.PlayKeywordBurst(die.transform.position + Vector3.up * 0.7f);
        }

        // ── the play board for one key word ─────────────────────────────────

        void OpenPlay(GateData gate)
        {
            var current = session.ActiveGate ?? session.ArriveAtGate();
            if (current != gate) { ShowPassage(); return; }
            dieGate = gate;
            solvedShowing = false; flying = false;
            token = gate.ChallengePayload as ContentToken;
            wrongTries = 0; locked = 0; clueText = null; lastShown = -1;
            guided = gate.HintsUsed >= 3;
            answer = new List<string>();
            sentenceIndex = gate.SentenceIndex;
            if (token == null)
            {
                // A malformed key word never traps the student (the panels wave it through the same way).
                Say("This word is broken: waving you through.");
                SolveDieGate();
                return;
            }
            if (encourager != null) encourager.GoodPick();
            string badge; Color badgeColour; string title;
            switch (gate.Type)
            {
                case GateType.Illustrate:
                    options = GateSolving.IllustrateOptions(token);
                    foreach (var url in options) LoadPicture(url);
                    badge = "ILLUSTRATE"; badgeColour = new Color32(255, 150, 190, 255); title = token.text;
                    break;
                case GateType.Define:
                    correct = GateSolving.DefineCorrect(token); pool = GateSolving.DefinePool(token);
                    badge = "DEFINE"; badgeColour = new Color32(255, 214, 102, 255); title = token.text;
                    break;
                default:
                    correct = GateSolving.FillCorrect(token); pool = GateSolving.FillPool(token);
                    badge = "FILL IN"; badgeColour = new Color32(140, 228, 196, 255); title = "The missing word";
                    break;
            }
            dieBoard.SetPlay(badge, badgeColour, title, PlaySentence(gate));
            dieBoard.SetHintLabel(HintLabel(gate.HintsUsed));
            if (gate.HintsUsed == 1) ApplyDieHint(1);
            else if (gate.HintsUsed >= 2) ApplyDieHint(2);
            if (guided) EnterDieGuided();
            RefreshPlay();
            dieBoard.SetStreak(streak);
            dieBoard.ShowPlay();
            StartShuffle();
        }

        /// <summary>The key word's sentence, the key word marked (a fill-in word blanked).</summary>
        string PlaySentence(GateData gate)
        {
            if (gate.SentenceIndex < 0 || gate.SentenceIndex >= trail.Sentences.Count) return "";
            var span = trail.Sentences[gate.SentenceIndex];
            var s = trail.PassageText.Substring(span.Start, span.Length);
            int at = gate.PassageCharIndex - span.Start;
            if (at < 0 || at >= s.Length) return s.Trim();
            int a = at, b = at;
            while (a > 0 && char.IsLetterOrDigit(s[a - 1])) a--;
            while (b < s.Length && (char.IsLetterOrDigit(s[b]) || s[b] == '-' || s[b] == '\'')) b++;
            var word = s.Substring(a, b - a);
            string marked = gate.Type == GateType.Fill
                ? "<color=#FFD666>" + new string('_', Mathf.Max(4, word.Length)) + "</color>"
                : (gate.Type == GateType.Illustrate ? "<color=#FF9BC0><u>" : "<color=#FFD666><u>") + word + "</u></color>";
            return (s.Substring(0, a) + marked + s.Substring(b)).Trim();
        }

        bool Illustrating => dieGate != null && dieGate.Type == GateType.Illustrate;
        bool Filling => dieGate != null && dieGate.Type != GateType.Illustrate && dieGate.Type != GateType.Define;

        static string HintLabel(int level) => level <= 0 ? "Hint (−40%)" : "Hint (−70%)";

        /// <summary>The play board: slots, the pieces still to place (or the pictures), the clue, the buttons.</summary>
        void RefreshPlay()
        {
            if (dieGate == null) return;
            if (Illustrating)
            {
                dieBoard.SetSlots(new List<string>(), 0, false);
                dieBoard.SetPool(null);
                var tex = new List<Texture2D>();
                int glow = -1;
                for (int i = 0; i < options.Count; i++)
                {
                    pictures.TryGetValue(options[i], out var t); tex.Add(t);
                    if (guided && options[i] == token.correctImage) glow = i;
                }
                dieBoard.SetPictures(tex, glow);
                if (string.IsNullOrEmpty(clueText)) clueText = "Which picture shows “" + token.text + "”? Tap the die when it shows its number.";
            }
            else
            {
                dieBoard.SetPictures(null);
                var shown = new List<string>();
                foreach (var p in answer) shown.Add(Filling ? p.ToUpperInvariant() : p);
                dieBoard.SetSlots(shown, correct.Count, Filling);
                dieBoard.SetPool(Filling ? pool.ConvertAll(p => p.ToUpperInvariant()) : pool);
                if (string.IsNullOrEmpty(clueText))
                    clueText = Filling ? "Tap the die when it shows the next letter of the word."
                                       : "Put the definition in order: tap the die when it shows the next word.";
            }
            dieBoard.SetClue(clueText);
            bool canHint = dieGate.HintsUsed < 2 && !guided;
            bool canGuide = !guided && (dieGate.HintsUsed >= 2 || wrongTries >= 2);
            dieBoard.SetButtons(canHint, canGuide, true);
        }

        // ── the die shuffles itself ─────────────────────────────────────────

        void StartShuffle()
        {
            StopShuffle();
            shuffler = StartCoroutine(Shuffle());
        }

        void StopShuffle()
        {
            if (shuffler != null) StopCoroutine(shuffler);
            shuffler = null;
        }

        IEnumerator Shuffle()
        {
            while (dieGate != null && !solvedShowing)
            {
                ShowNextOnDie();
                yield return new WaitForSeconds(dieShuffleSeconds);
            }
            shuffler = null;
        }

        /// <summary>The die spins to another piece of this key word only (guided: to the right one).</summary>
        void ShowNextOnDie()
        {
            faceChoices.Clear(); faceKeys.Clear();
            if (Illustrating)
            {
                if (options.Count == 0) return;
                int idx = guided ? Mathf.Max(0, options.IndexOf(token.correctImage)) : NextIndex(options.Count);
                pictures.TryGetValue(options[idx], out var tex);
                faceChoices.Add(tex != null ? ReadingDie.Choice.Picture(tex, idx + 1) : ReadingDie.Choice.Word((idx + 1).ToString()));
                faceKeys.Add(options[idx]);
                die.Glow(guided ? 0 : -1);
            }
            else
            {
                if (pool.Count == 0) return;
                int idx;
                if (guided && answer.Count < correct.Count)
                {
                    var want = correct[answer.Count];
                    idx = Mathf.Max(0, pool.FindIndex(p => Same(p, want)));
                }
                else idx = NextIndex(pool.Count);
                faceChoices.Add(ReadingDie.Choice.Word(Filling ? pool[idx].ToUpperInvariant() : pool[idx]));
                faceKeys.Add(pool[idx]);
                die.Glow(guided ? 0 : -1);
            }
            die.Spin(faceChoices);
        }

        /// <summary>A random piece, never the one just shown (when there is another).</summary>
        int NextIndex(int count)
        {
            if (count <= 1) { lastShown = 0; return 0; }
            int i;
            do i = Random.Range(0, count); while (i == lastShown);
            lastShown = i;
            return i;
        }

        bool Same(string a, string b) => Filling ? string.Equals(a, b, System.StringComparison.OrdinalIgnoreCase) : a == b;

        /// <summary>A tap on the die (forwarded by ReadingHouseInput): the piece it shows.</summary>
        public void OnDieTapped(int choice)
        {
            if (!DieMode || dieBoard == null || dieGate == null || solvedShowing || flying || choice < 0 || choice >= faceKeys.Count || submitting) return;
            GatePick(faceKeys[choice]);
        }

        /// <summary>Kept for the input's swipe: a swipe spins the die on at once.</summary>
        public void OnDieFlip(int direction = 1)
        {
            if (dieGate == null || solvedShowing || die.Busy) return;
            StartShuffle();
        }

        void Right()
        {
            streak++;
            dieBoard.SetStreak(streak);
            die.SetSparkle(streak >= 10 ? 2 : streak >= 5 ? 1 : 0);
            dieBoard.Flash(true);
            if (encourager != null && streak > 0 && streak % 6 == 0) encourager.GoodPick();
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

        void GatePick(string pick)
        {
            if (Illustrating)
            {
                if (guided && pick != token.correctImage) return;
                if (GateSolving.CheckIllustrate(pick, token)) { Right(); SolveDieGate(); return; }
                wrongTries++;
                Wrong();
                RefreshPlay();
                return;
            }
            if (answer.Count >= correct.Count) return;
            var want = correct[answer.Count];
            if (!Same(pick, want))
            {
                if (guided) return;                                  // guided: only the glowing piece
                wrongTries++;
                Wrong();
                RefreshPlay();
                return;
            }
            Right();
            int inPool = pool.FindIndex(p => Same(p, pick));
            int slot = answer.Count;
            StopShuffle();
            flying = true;                                           // one piece at a time: further taps wait for it to land
            dieBoard.FlyToSlot(inPool, slot, () =>
            {
                flying = false;
                if (dieGate == null) return;
                answer.Add(pick);
                if (inPool >= 0) pool.RemoveAt(inPool);
                if (answer.Count == correct.Count)
                {
                    bool ok = Filling ? GateSolving.CheckFill(answer, correct) : GateSolving.CheckDefine(answer, correct);
                    if (ok) { RefreshPlay(); SolveDieGate(); return; }
                }
                RefreshPlay();
                StartShuffle();
            });
        }

        void SolveDieGate()
        {
            StopShuffle();
            session.SolveActiveGate();
            Save();
            solvedShowing = true;
            die.Glow(-1);
            die.SetTier(TierNow());
            die.Celebrate();
            if (candy != null) { candy.SetProgress(KeyProgress()); candy.Celebrate(); }
            dashboard.SetPoints(session.Points);
            if (chalkboard != null) chalkboard.Show(trail);
            OnWordSolved();
            dieBoard.SetClue(token != null ? "<color=#78E68C>Restored!</color>  “" + token.text + "” is back in the passage." : "Restored!");
            dieBoard.SetButtons(false, false, false, "Back to the passage  ▸");
            if (session.CanComplete) Say("Every word restored: tap Finish & submit!");
        }

        int TierNow() => session == null ? 0 : session.SolvedGates;

        /// <summary>Back to the passage (after restoring the word, or to read on first: the word stays where it was).</summary>
        void BackToPassage()
        {
            StopShuffle();
            ShowPassage();
        }

        void DieLater()
        {
            if (dieGate == null || solvedShowing) return;
            session.DeferActiveGate();
            Save();
            Say("Saved for Later: on to the next word.");
            BackToPassage();
        }

        void DieHint()
        {
            if (dieGate == null || guided || solvedShowing) return;
            int level = session.UseHint();
            Save();
            dieBoard.SetHintLabel(HintLabel(level));
            ApplyDieHint(level);
            RefreshPlay();
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
            if (dieGate == null || guided || solvedShowing) return;
            session.RequestGuidedSolve();
            Save();
            EnterDieGuided();
            RefreshPlay();
            StartShuffle();
        }

        void EnterDieGuided()
        {
            guided = true;
            clueText = Illustrating ? "The glowing picture is the one: tap the die."
                     : Filling ? "The word is “" + string.Join("", correct).ToUpperInvariant() + "”: tap the die for each glowing letter."
                     : "“" + string.Join(" ", correct) + "”: tap the die for each glowing word.";
        }

        /// <summary>The key word's timer ran out (TrekSession already moved it to Later).</summary>
        void DieGateTimedOut()
        {
            Save();
            Say("No rush: that word will wait in Later.");
            BackToPassage();
        }

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
            if (Illustrating && dieBoard != null && !solvedShowing) RefreshPlay();
        }
    }
}
