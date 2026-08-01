using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using SReader.Domains.Assignments.Models;
using SReader.Game.Trek;
using SReader.Game.Library.Feel;

namespace SReader.Game.Library.Reading
{
    /// <summary>
    /// GL-1 Reading Alcove presenter: the Bible's Book page projection + reading
    /// engine LOAD/READ (Ch. 8.2). Renders a sample book's passage on a world-space
    /// TMP "page", glows the <b>current</b> keyword Candle Gold (Word Kindling
    /// affordance), and on tap opens the matching ritual — the existing
    /// <see cref="GatePanelHost"/> (Ink Weaving / Definition Reforging / Vision
    /// Restoration) on a UIDocument overlay. Solve / hint / guided / Later all flow
    /// through <see cref="TrekSession"/>; a solve fires the Shared Restoration
    /// Grammar via <see cref="FeedbackDirector"/> on the word itself.
    ///
    /// A presenter (MVVM): it renders <see cref="TrailData"/> and forwards input;
    /// every rule (ordering, hint ladder, deferral, scoring) lives in the reused,
    /// tested <see cref="TrekSession"/>/<see cref="GateSolving"/>. Sequential order
    /// (only the current keyword is live) enforces R-4/R-5 context-before-puzzle.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ReadingPagePresenter : MonoBehaviour
    {
        [Header("Wiring")]
        [SerializeField] TextMeshPro pageText;
        [SerializeField] SReader.Game.Library.Feel.FeedbackDirector director;
        [SerializeField] Camera readingCamera;
        [Tooltip("UIDocument whose root hosts the ritual panels (any PanelSettings, ScreenSpaceOverlay).")]
        [SerializeField] UIDocument ritualOverlay;

        [Header("Content")]
        [Tooltip("A sample book under Resources/ (see Docs/GL0_BOOKDATA_CONTRACT.md).")]
        [SerializeField] string sampleBookResource = "Books/SampleBook_Nature_MeadowAtDawn";

        [Header("Palette (Bible Ch. 3.2)")]
        [SerializeField] Color bodyColor     = new Color(0.16f, 0.13f, 0.10f);   // ink on parchment
        [SerializeField] Color sealedColor   = new Color(0.55f, 0.45f, 0.28f);   // upcoming keyword, fog-touched
        [SerializeField] Color currentColor  = new Color(0.949f, 0.698f, 0.298f);// #F2B24C — live keyword
        [SerializeField] Color solvedColor   = new Color(1f, 0.86f, 0.55f);      // restored, brighter gold
        [SerializeField] Color deferredColor = new Color(0.357f, 0.247f, 0.659f);// #5B3FA8 arcane violet — Later

        TrailData trail;
        TrekSession session;
        readonly GatePanelHost panel = new GatePanelHost();

        Vector3[] gateWorldPos;
        List<int>[] gateCharPositions;   // TMP characterInfo indices per gate

        void Start()
        {
            if (director == null) director = FeedbackDirector.Instance;
            if (readingCamera == null) readingCamera = Camera.main;
            if (pageText == null) { Debug.LogError("[ReadingPage] No TMP page assigned."); enabled = false; return; }

            trail = BuildTrail();
            session = new TrekSession(trail);
            BuildPage();

            if (ritualOverlay != null && ritualOverlay.rootVisualElement != null)
                ritualOverlay.rootVisualElement.pickingMode = PickingMode.Ignore;   // let 3D taps through when no panel

            RefreshStates();
        }

        TrailData BuildTrail()
        {
            var asset = Resources.Load<TextAsset>(sampleBookResource);
            if (asset == null)
            {
                Debug.LogWarning($"[ReadingPage] Sample book not found: {sampleBookResource}. Reading-only page.");
                return TrailBuilder.BuildFromPlainText("sample", "Sample", "The page waits, quiet and unread.");
            }
            var content = AssignmentContentCodec.FromJson(asset.text);
            return TrailBuilder.Build("sample", "Sample Book", content);
        }

        // ── page render (LOAD) ────────────────────────────────────────────────

        void BuildPage()
        {
            pageText.text = trail.PassageText;
            pageText.color = bodyColor;
            pageText.ForceMeshUpdate();

            int gateCount = trail.Gates.Count;
            gateWorldPos = new Vector3[gateCount];
            gateCharPositions = new List<int>[gateCount];
            for (int g = 0; g < gateCount; g++) gateCharPositions[g] = new List<int>();

            var info = pageText.textInfo;
            for (int p = 0; p < info.characterCount; p++)
            {
                int g = FindGateAt(info.characterInfo[p].index);
                if (g >= 0) gateCharPositions[g].Add(p);
            }
            for (int g = 0; g < gateCount; g++) gateWorldPos[g] = WordCentre(g, info);
        }

        int FindGateAt(int srcIndex)
        {
            var gates = trail.Gates;
            for (int i = 0; i < gates.Count; i++)
            {
                int start = gates[i].PassageCharIndex;
                int end = start + (gates[i].Word != null ? gates[i].Word.Length : 0);
                if (srcIndex >= start && srcIndex < end) return i;
            }
            return -1;
        }

        Vector3 WordCentre(int g, TMP_TextInfo info)
        {
            Vector3 sum = Vector3.zero;
            int n = 0;
            foreach (int p in gateCharPositions[g])
            {
                var ci = info.characterInfo[p];
                if (!ci.isVisible) continue;
                sum += pageText.transform.TransformPoint((ci.bottomLeft + ci.topRight) * 0.5f);
                n++;
            }
            return n > 0 ? sum / n : pageText.transform.position;
        }

        // Recolour every keyword to reflect the session state (current / solved / deferred / upcoming).
        void RefreshStates()
        {
            if (trail == null || !trail.HasGates) return;
            int current = CurrentGateIndex();
            var info = pageText.textInfo;
            for (int g = 0; g < trail.Gates.Count; g++)
            {
                var state = trail.Gates[g].State;
                Color c = IsSolved(state) ? solvedColor
                        : state == GateState.Deferred ? deferredColor
                        : g == current ? currentColor
                        : sealedColor;
                SetWordColor(g, c, info);
            }
            pageText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }

        void SetWordColor(int g, Color32 color, TMP_TextInfo info)
        {
            foreach (int p in gateCharPositions[g])
            {
                var ci = info.characterInfo[p];
                if (!ci.isVisible) continue;
                var colors = info.meshInfo[ci.materialReferenceIndex].colors32;
                int vi = ci.vertexIndex;
                colors[vi + 0] = color; colors[vi + 1] = color;
                colors[vi + 2] = color; colors[vi + 3] = color;
            }
        }

        // ── input (READ → Word Kindling → ritual) ─────────────────────────────

        void Update()
        {
            if (session == null || panel.IsOpen) return;
            if (session.Phase == TrekPhase.Complete) return;
            if (!TryGetPointerDown(out var screenPos)) return;

            int charIndex = TMP_TextUtilities.FindIntersectingCharacter(pageText, screenPos, readingCamera, true);
            if (charIndex == -1) return;

            int g = FindGateAt(pageText.textInfo.characterInfo[charIndex].index);
            if (g < 0 || g != CurrentGateIndex()) return;   // R-5: only the current keyword is live

            var opened = session.ArriveAtGate();
            if (opened == null) { AfterAdvance(); return; }
            OpenRitual();
        }

        void OpenRitual()
        {
            var gate = session.ActiveGate;
            var host = ritualOverlay != null ? ritualOverlay.rootVisualElement : null;
            if (gate == null) return;
            if (host == null) { Debug.LogWarning("[ReadingPage] No ritual overlay — auto-solving to avoid a trap."); Solve(); return; }

            panel.Open(host, gate, new GatePanelHost.Hooks
            {
                UseHint = () => session.UseHint(),
                RequestGuided = () => session.RequestGuidedSolve(),
                Solved = () => { panel.Close(); Solve(); },
                Later = () => { panel.Close(); session.DeferActiveGate(); AfterAdvance(); },
            });
        }

        void Solve()
        {
            int idx = session.ActiveGateIndex;
            var gate = session.ActiveGate;
            Vector3 pos = idx >= 0 && gateWorldPos != null ? gateWorldPos[idx] : transform.position;

            session.SolveActiveGate();   // scoring + state live in the session
            if (gate != null) director?.PlayCorrect(new FeedbackContext(gate.GateId, pos, gate.Word));
            AfterAdvance();
        }

        void AfterAdvance()
        {
            RefreshStates();
            if (session.Phase == TrekPhase.Complete)
            {
                director?.PlayCompletion();
                Debug.Log("[ReadingPage] Page restored — every ritual complete (R-6).");
            }
        }

        int CurrentGateIndex()
        {
            var next = session != null ? session.PeekNextGate() : null;
            return next == null ? -1 : trail.Gates.IndexOf(next);
        }

        static bool IsSolved(GateState s) =>
            s == GateState.SolvedGold || s == GateState.SolvedSilver || s == GateState.SolvedBronze;

        static bool TryGetPointerDown(out Vector2 pos)
        {
            pos = default;
#if ENABLE_INPUT_SYSTEM
            var mouse = UnityEngine.InputSystem.Mouse.current;
            if (mouse != null && mouse.leftButton.wasPressedThisFrame) { pos = mouse.position.ReadValue(); return true; }
            var touch = UnityEngine.InputSystem.Touchscreen.current;
            if (touch != null && touch.primaryTouch.press.wasPressedThisFrame) { pos = touch.primaryTouch.position.ReadValue(); return true; }
            return false;
#else
            if (Input.GetMouseButtonDown(0)) { pos = Input.mousePosition; return true; }
            return false;
#endif
        }
    }
}
