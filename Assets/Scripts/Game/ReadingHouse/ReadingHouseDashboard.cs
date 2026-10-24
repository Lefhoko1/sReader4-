using TMPro;
using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>Which physical dashboard control a collider belongs to.</summary>
    public enum DashboardControl { Menu, PlayWithOthers, Points, Assignment }

    /// <summary>
    /// Writes runtime values onto the imported dashboard: assignment title,
    /// "Sentence n of N", the progress fill, points, the current sentence on the
    /// display, the display illustration and the two characters' speech lines.
    /// Every value is pushed in by the presenter from existing runtime data;
    /// nothing here is authored content or a rule. All text objects sit on the
    /// named anchors exported from Blender (handoff v04).
    /// </summary>
    public sealed class ReadingHouseDashboard : MonoBehaviour
    {
        [Header("Header")]
        public TextMeshPro assignmentTitle;
        public TextMeshPro sentenceProgress;
        public Transform progressFill;          // SR_HEADER_PROGRESS_FILL: pivot at its left end

        [Header("Display")]
        public TextMeshPro sentenceText;
        public Renderer sentencePanel;
        public Renderer illustration;          // SR_DISPLAY_ILLUSTRATION

        [Header("Controls")]
        public TextMeshPro pointsLabel;
        public TextMeshPro pointsValue;
        public TextMeshPro menuLabel;
        public TextMeshPro multiplayerLabel;

        [Header("Characters")]
        public TextMeshPro encouragerLine;
        public TextMeshPro zombieLine;
        [Tooltip("Speech line roots (text + panel). Each has an overview and a gameplay position, because the gameplay camera crops the room's sides.")]
        public Transform encouragerSpeech, zombieSpeech;
        public Vector3 encouragerOverviewPos, encouragerGameplayPos, zombieOverviewPos, zombieGameplayPos;

        static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

        public void SetHeader(string title, int sentenceNumber, int sentenceCount)
        {
            assignmentTitle.text = title ?? "";
            sentenceProgress.text = sentenceCount > 0 ? $"Sentence {sentenceNumber} of {sentenceCount}" : "";
            SetProgress(sentenceCount > 0 ? (float)sentenceNumber / sentenceCount : 0f);
        }

        public void SetProgress(float fraction)
        {
            if (progressFill == null) return;
            var s = progressFill.localScale;
            s.x = Mathf.Clamp01(fraction);
            progressFill.localScale = s;
        }

        public void SetPoints(int points) => pointsValue.text = points.ToString();

        public void SetControlLabels(string points, string menu, string playWithOthers)
        {
            pointsLabel.text = points;
            menuLabel.text = menu;
            multiplayerLabel.text = playWithOthers;
        }

        public void SetSentence(string sentence) => sentenceText.text = sentence ?? "";

        [Header("Sentence look")]
        [Tooltip("No white banner behind the sentence: white letters with a navy outline and a soft shadow read straight on the picture.")]
        public bool sentenceOnPicture = true;
        public Color sentenceColor = Color.white;
        public Color sentenceOutline = new Color32(14, 28, 64, 255);
        [Range(0f, 0.5f)] public float sentenceOutlineWidth = 0.4f;
        [Range(0f, 0.5f)] public float sentenceBoldness = 0.12f;
        public Color sentenceShadow = new Color(0.03f, 0.06f, 0.15f, 0.75f);

        void Awake()
        {
            if (!sentenceOnPicture || sentenceText == null) return;
            if (sentencePanel != null) sentencePanel.enabled = false;
            sentenceText.color = sentenceColor;
            sentenceText.fontStyle |= FontStyles.Bold;
            var m = sentenceText.fontMaterial;                     // this text's own material instance
            m.SetFloat(ShaderUtilities.ID_FaceDilate, sentenceBoldness);
            m.SetFloat(ShaderUtilities.ID_OutlineWidth, sentenceOutlineWidth);
            m.SetColor(ShaderUtilities.ID_OutlineColor, sentenceOutline);
            m.EnableKeyword(ShaderUtilities.Keyword_Underlay);
            m.SetColor(ShaderUtilities.ID_UnderlayColor, sentenceShadow);
            m.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0.35f);
            m.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, -0.45f);
            m.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.35f);
            m.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0.45f);
            sentenceText.UpdateMeshPadding();
        }

        public void SetIllustration(Texture texture)
        {
            if (illustration == null || texture == null) return;
            var block = new MaterialPropertyBlock();
            illustration.GetPropertyBlock(block);
            block.SetTexture(BaseMap, texture);
            illustration.SetPropertyBlock(block);
        }

        public void SetDialogue(string encourager, string zombie)
        {
            if (encouragerLine != null) encouragerLine.text = encourager ?? "";
            if (zombieLine != null) zombieLine.text = zombie ?? "";
        }

        public void SetDialogueVisible(bool visible)
        {
            if (encouragerSpeech != null) encouragerSpeech.gameObject.SetActive(visible);
            if (zombieSpeech != null) zombieSpeech.gameObject.SetActive(visible);
        }

        /// <summary>Move the speech lines to where the current camera can see them.</summary>
        public void SetDialogueLayout(bool gameplay)
        {
            if (encouragerSpeech != null) encouragerSpeech.position = gameplay ? encouragerGameplayPos : encouragerOverviewPos;
            if (zombieSpeech != null) zombieSpeech.position = gameplay ? zombieGameplayPos : zombieOverviewPos;
        }
    }
}
