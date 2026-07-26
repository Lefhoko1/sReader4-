using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SReader.Domains.Assignments.Models;
using SReader.UI.Views.Home;

namespace SReader.Game.Trek
{
    /// <summary>
    /// Hosts a gate's challenge as an in-world bottom sheet (the mockup's
    /// "panel slides up when Kai reaches a gate"). The three challenges are the
    /// EXISTING ones reskinned — Word Forge (fill), Scroll of Order (define),
    /// Picture Gate (illustrate) — with identical checking semantics via
    /// <see cref="GateSolving"/> and the R-8 hint ladder:
    ///   Hint 1 (−40%) → clue · Hint 2 (−70%) → reveal-half / lock-two /
    ///   eliminate-two · Guide me → step-through, 0 points, still solved.
    /// The panel renders and forwards input; all scoring lives in TrekSession
    /// (via the presenter-supplied hooks).
    /// </summary>
    internal sealed class GatePanelHost
    {
        /// <summary>Presenter-supplied verbs. The panel never touches the session directly.</summary>
        internal sealed class Hooks
        {
            public Func<int> UseHint;          // advance the ladder, returns new level (1..2)
            public Action RequestGuided;       // switch to guided solve (R-8.3)
            public Action Solved;              // challenge checked correct / guided finished
            public Action Later;               // defer to Cleanup Camp (R-7)
        }

        VisualElement overlay;
        public bool IsOpen => overlay != null;

        public void Open(VisualElement host, GateData gate, Hooks hooks)
        {
            Close();
            var token = gate.ChallengePayload as ContentToken;

            overlay = new VisualElement();
            overlay.style.position = Position.Absolute;
            overlay.style.left = 0; overlay.style.right = 0; overlay.style.top = 0; overlay.style.bottom = 0;
            overlay.style.justifyContent = Justify.FlexEnd;
            overlay.style.backgroundColor = new Color(0f, 0f, 0f, 0.25f);

            var sheet = TrekTheme.CardBox();
            sheet.style.marginLeft = sheet.style.marginRight = 10;
            sheet.style.marginBottom = 86;   // sits above the ribbon
            overlay.Add(sheet);

            if (token == null)
            {
                // Malformed payload — never trap the student (mirrors the
                // TrailBuilder fallback philosophy). Solve for zero.
                sheet.Add(TrekTheme.Title("This gate is broken — waving you through."));
                sheet.Add(TrekTheme.Primary("Continue", () => hooks.Solved()));
                host.Add(overlay);
                return;
            }

            switch (gate.Type)
            {
                case GateType.Fill:       BuildArrange(sheet, gate, token, hooks, fill: true);  break;
                case GateType.Define:     BuildArrange(sheet, gate, token, hooks, fill: false); break;
                case GateType.Illustrate: BuildIllustrate(sheet, gate, token, hooks);           break;
                default:                  BuildArrange(sheet, gate, token, hooks, fill: true);  break;
            }

            host.Add(overlay);
        }

        public void Close()
        {
            overlay?.RemoveFromHierarchy();
            overlay = null;
        }

        // ── Word Forge (fill) / Scroll of Order (define) — shared arrange core ──

        void BuildArrange(VisualElement sheet, GateData gate, ContentToken token, Hooks hooks, bool fill)
        {
            var correct = fill ? GateSolving.FillCorrect(token) : GateSolving.DefineCorrect(token);
            var pool = fill ? GateSolving.FillPool(token) : GateSolving.DefinePool(token);
            var answer = new List<string>();
            var locked = 0;                       // leading pieces placed by hint 2 / guided
            bool guided = gate.HintsUsed >= 3;
            int wrongTries = 0;

            sheet.Add(Kicker(fill ? "WORD FORGE" : "SCROLL OF ORDER"));
            sheet.Add(TrekTheme.Title(fill ? "Forge the word" : "Define: " + token.text, 18));
            sheet.Add(TrekTheme.Sub(fill ? "Tap the letters to spell the hidden word." : "Arrange the definition in the right order."));

            var preview = new Label();            // slot preview (fill only)
            if (fill)
            {
                preview.style.fontSize = 24;
                preview.style.unityFontStyleAndWeight = FontStyle.Bold;
                preview.style.color = TrekTheme.Ink;
                preview.style.unityTextAlign = TextAnchor.MiddleCenter;
                preview.style.marginTop = 4;
                sheet.Add(preview);
            }

            var clue = TrekTheme.Sub("");
            clue.style.display = DisplayStyle.None;
            sheet.Add(clue);

            var answerRow = WrapRow(); sheet.Add(answerRow);
            var divider = TrekTheme.Sub(fill ? "Letters" : "Pieces", 11);
            divider.style.marginTop = 6;
            sheet.Add(divider);
            var poolRow = WrapRow(); sheet.Add(poolRow);

            var status = TrekTheme.Sub("");
            status.style.display = DisplayStyle.None;
            sheet.Add(status);

            Button guide = null, hint = null;

            void UpdatePreview()
            {
                if (!fill) return;
                var slots = new List<string>(correct.Count);
                for (int i = 0; i < correct.Count; i++)
                    slots.Add(i < answer.Count ? answer[i].ToUpperInvariant() : "▢");
                preview.text = string.Join(" ", slots);
            }

            void Rebuild()
            {
                answerRow.Clear();
                poolRow.Clear();
                for (int i = 0; i < answer.Count; i++)
                {
                    int idx = i;
                    bool isLocked = idx < locked;
                    answerRow.Add(TrekTheme.Chip(answer[i], () =>
                    {
                        if (idx < locked) return;
                        pool.Add(answer[idx]);
                        answer.RemoveAt(idx);
                        Hide(status);
                        Rebuild();
                    }, filled: true, locked: isLocked));
                }
                for (int i = 0; i < pool.Count; i++)
                {
                    int idx = i;
                    var chip = TrekTheme.Chip(pool[i], () =>
                    {
                        if (guided && !IsNextGuidedPiece(pool[idx])) return;   // guided: only the glowing piece
                        answer.Add(pool[idx]);
                        pool.RemoveAt(idx);
                        Hide(status);
                        Rebuild();
                        if (guided && answer.Count == correct.Count) hooks.Solved();
                    }, filled: false);
                    if (guided && IsNextGuidedPiece(pool[idx]))
                    {
                        chip.style.backgroundColor = TrekTheme.Sun;           // the glowing next piece
                        TrekTheme.Border(chip, TrekTheme.Coral, 2);
                    }
                    poolRow.Add(chip);
                }
                UpdatePreview();
            }

            bool IsNextGuidedPiece(string piece)
            {
                if (answer.Count >= correct.Count) return false;
                var want = correct[answer.Count];
                return fill ? string.Equals(piece, want, StringComparison.OrdinalIgnoreCase) : piece == want;
            }

            void EnterGuided()
            {
                guided = true;
                hooks.RequestGuided();
                clue.text = fill
                    ? "The word is “" + string.Join("", correct).ToUpperInvariant() + "” — tap the glowing letters in order."
                    : "“" + string.Join(" ", correct) + "” — tap the glowing pieces in order.";
                clue.style.color = TrekTheme.Coral;
                clue.style.display = DisplayStyle.Flex;
                if (guide != null) guide.style.display = DisplayStyle.None;
                if (hint != null) hint.style.display = DisplayStyle.None;
                Rebuild();
            }

            void ApplyHint(int level)
            {
                if (level == 1)
                {
                    clue.text = fill
                        ? "💡 It has " + correct.Count + " letters and starts with “" + (correct.Count > 0 ? correct[0].ToUpperInvariant() : "?") + "”."
                        : "💡 It has " + correct.Count + " words and starts with “" + (correct.Count > 0 ? correct[0] : "?") + "”.";
                }
                else if (level == 2)
                {
                    if (fill)
                    {
                        // Reveal half the letters (R-8.2): pre-place them, locked.
                        foreach (var i in GateSolving.FillRevealIndices(correct))
                        {
                            if (answer.Count > i) continue;
                            var piece = correct[i];
                            int inPool = pool.FindIndex(p => string.Equals(p, piece, StringComparison.OrdinalIgnoreCase));
                            if (inPool >= 0) pool.RemoveAt(inPool);
                            answer.Insert(i, piece);
                        }
                        locked = (correct.Count + 1) / 2;
                    }
                    else
                    {
                        // Lock the first two pieces in place (R-8.2).
                        for (int i = 0; i < GateSolving.DefineLockedPieces && i < correct.Count; i++)
                        {
                            if (answer.Count > i && answer[i] == correct[i]) continue;
                            int inPool = pool.IndexOf(correct[i]);
                            if (inPool >= 0) pool.RemoveAt(inPool);
                            answer.Insert(Math.Min(i, answer.Count), correct[i]);
                        }
                        locked = Math.Min(GateSolving.DefineLockedPieces, correct.Count);
                    }
                    clue.text = "💡 A head start — finish it off!";
                    if (hint != null) hint.style.display = DisplayStyle.None;   // ladder ends; Guide me remains
                    if (guide != null) guide.style.display = DisplayStyle.Flex;
                }
                clue.style.color = TrekTheme.Muted;
                clue.style.display = DisplayStyle.Flex;
                Rebuild();
            }

            var actions = WrapRow();
            actions.style.marginTop = 10;

            actions.Add(TrekTheme.Primary("Check", () =>
            {
                bool ok = fill ? GateSolving.CheckFill(answer, correct) : GateSolving.CheckDefine(answer, correct);
                if (ok) { hooks.Solved(); return; }

                wrongTries++;
                status.text = GateSolving.SamePiecesWrongOrder(answer, correct, fill)
                    ? (fill ? "Right letters — check the order." : "Right words — check the order.")
                    : (fill ? "Not quite — keep spelling." : "Not quite — keep arranging.");
                status.style.color = TrekTheme.Coral;
                status.style.display = DisplayStyle.Flex;
                Shake(sheet);
                if (wrongTries >= 2 && guide != null) guide.style.display = DisplayStyle.Flex;
            }));

            hint = TrekTheme.Ghost(HintLabel(gate.HintsUsed), () =>
            {
                int level = hooks.UseHint();
                hint.text = HintLabel(level);
                ApplyHint(level);
            });
            if (gate.HintsUsed >= 2) hint.style.display = DisplayStyle.None;
            actions.Add(hint);

            guide = TrekTheme.Ghost("Guide me (0 ★)", EnterGuided);
            guide.style.display = gate.HintsUsed >= 2 ? DisplayStyle.Flex : DisplayStyle.None;
            actions.Add(guide);

            actions.Add(TrekTheme.Ghost("⏭ Later", () => hooks.Later()));
            sheet.Add(actions);

            if (guided) EnterGuided(); else Rebuild();
            if (gate.HintsUsed == 1) ApplyHint(1);
            else if (gate.HintsUsed == 2) ApplyHint(2);
        }

        // ── Picture Gate (illustrate) ─────────────────────────────────────────

        void BuildIllustrate(VisualElement sheet, GateData gate, ContentToken token, Hooks hooks)
        {
            bool guided = gate.HintsUsed >= 3;
            int wrongTries = 0;

            sheet.Add(Kicker("PICTURE GATE"));
            sheet.Add(TrekTheme.Title("Illustrate: " + token.text, 18));
            sheet.Add(TrekTheme.Sub("Pick the image that matches the word."));

            var clue = TrekTheme.Sub("");
            clue.style.display = DisplayStyle.None;
            sheet.Add(clue);

            var options = GateSolving.IllustrateOptions(token);
            var boxes = new Dictionary<string, VisualElement>();

            var grid = new VisualElement();
            grid.style.flexDirection = FlexDirection.Row;
            grid.style.flexWrap = Wrap.Wrap;
            grid.style.justifyContent = Justify.SpaceBetween;
            grid.style.marginTop = 8;
            sheet.Add(grid);

            var tileHeight = options.Count > 4 ? 76 : 100;
            Button guide = null, hint = null;

            void PaintGuided()
            {
                foreach (var kv in boxes)
                {
                    bool isCorrect = kv.Key == token.correctImage;
                    TrekTheme.Border(kv.Value, isCorrect ? TrekTheme.Sun : TrekTheme.Line, isCorrect ? 3 : 1);
                    kv.Value.style.opacity = isCorrect ? 1f : 0.35f;
                }
            }

            foreach (var url in options)
            {
                var captured = url;
                var box = new VisualElement();
                box.style.width = Length.Percent(48);
                box.style.height = tileHeight;
                box.style.marginBottom = 8;
                box.style.backgroundColor = TrekTheme.Paper;
                TrekTheme.Round(box, 10);
                TrekTheme.Border(box, TrekTheme.Line, 1);
                box.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
                HomeUI.LoadImageInto(box, captured);
                boxes[captured] = box;

                box.RegisterCallback<ClickEvent>(_ =>
                {
                    if (guided && captured != token.correctImage) return;
                    if (GateSolving.CheckIllustrate(captured, token)) { hooks.Solved(); return; }

                    wrongTries++;
                    TrekTheme.Border(box, TrekTheme.Coral, 2);
                    box.style.opacity = 0.5f;
                    Shake(sheet);
                    if (wrongTries >= 2 && guide != null) guide.style.display = DisplayStyle.Flex;
                });
                grid.Add(box);
            }

            void ApplyHint(int level)
            {
                if (level == 1)
                {
                    clue.text = !string.IsNullOrWhiteSpace(token.definition)
                        ? "💡 " + token.definition
                        : "💡 Think about what “" + token.text + "” means.";
                }
                else if (level == 2)
                {
                    // Eliminate two wrong images (R-8.2).
                    foreach (var gone in GateSolving.IllustrateEliminations(options, token))
                        if (boxes.TryGetValue(gone, out var b)) { b.style.opacity = 0.25f; b.SetEnabled(false); }
                    clue.text = "💡 Two of them are out.";
                    if (hint != null) hint.style.display = DisplayStyle.None;
                    if (guide != null) guide.style.display = DisplayStyle.Flex;
                }
                clue.style.color = TrekTheme.Muted;
                clue.style.display = DisplayStyle.Flex;
            }

            var actions = WrapRow();
            actions.style.marginTop = 10;

            hint = TrekTheme.Ghost(HintLabel(gate.HintsUsed), () =>
            {
                int level = hooks.UseHint();
                hint.text = HintLabel(level);
                ApplyHint(level);
            });
            if (gate.HintsUsed >= 2) hint.style.display = DisplayStyle.None;
            actions.Add(hint);

            guide = TrekTheme.Ghost("Guide me (0 ★)", () =>
            {
                guided = true;
                hooks.RequestGuided();
                clue.text = "The glowing picture is the one — tap it.";
                clue.style.color = TrekTheme.Coral;
                clue.style.display = DisplayStyle.Flex;
                PaintGuided();
                if (guide != null) guide.style.display = DisplayStyle.None;
                if (hint != null) hint.style.display = DisplayStyle.None;
            });
            guide.style.display = gate.HintsUsed >= 2 ? DisplayStyle.Flex : DisplayStyle.None;
            actions.Add(guide);

            actions.Add(TrekTheme.Ghost("⏭ Later", () => hooks.Later()));
            sheet.Add(actions);

            if (guided) PaintGuided();
            else if (gate.HintsUsed == 1) ApplyHint(1);
            else if (gate.HintsUsed == 2) ApplyHint(2);
        }

        // ── helpers ──

        // The small all-caps gate-type banner ("WORD FORGE" / "SCROLL OF ORDER" / "PICTURE GATE").
        static Label Kicker(string text)
        {
            var l = new Label(text);
            l.style.fontSize = 11;
            l.style.letterSpacing = 2;
            l.style.color = TrekTheme.Violet;
            l.style.unityFontStyleAndWeight = FontStyle.Bold;
            l.style.marginBottom = 2;
            return l;
        }

        static string HintLabel(int level) => level <= 0 ? "Hint (−40%)" : "Hint (−70%)";

        static VisualElement WrapRow()
        {
            var r = new VisualElement();
            r.style.flexDirection = FlexDirection.Row;
            r.style.flexWrap = Wrap.Wrap;
            r.style.marginTop = 6;
            return r;
        }

        static void Hide(Label l) => l.style.display = DisplayStyle.None;

        // Calm error shake (the juicy version lands in Phase 5).
        static void Shake(VisualElement e)
        {
            var seq = new[] { 4f, -4f, 2f, -2f, 0f };
            for (int i = 0; i < seq.Length; i++)
            {
                float x = seq[i];
                e.schedule.Execute(() => e.style.translate = new Translate(x, 0f, 0f)).StartingIn(i * 40);
            }
        }
    }
}
