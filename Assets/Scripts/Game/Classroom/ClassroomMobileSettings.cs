using UnityEngine;
using UnityEngine.Rendering;
namespace SReader.Game.Classroom
{
    [ExecuteAlways]
    public sealed class ClassroomMobileSettings : MonoBehaviour
    {
        public RenderPipelineAsset pipeline;
        [HideInInspector] public RenderPipelineAsset previousPipeline;
        bool applied;
        void OnEnable() { Apply(); }
        public void Apply()
        {
            if (!pipeline || applied) return;
            if (QualitySettings.renderPipeline != pipeline) previousPipeline = QualitySettings.renderPipeline;
            QualitySettings.renderPipeline = pipeline; applied = true;
            if (Application.isPlaying) Application.targetFrameRate = 30;
        }
        void OnDisable()
        {
            if (!applied) return;
            if (QualitySettings.renderPipeline == pipeline) QualitySettings.renderPipeline = previousPipeline;
            applied = false;
        }
    }
}
