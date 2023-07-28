using UnityEngine;
using UnityEngine.Rendering;

namespace Lesson_8
{
    [CreateAssetMenu(menuName = "Rendering/SpaceRunPipelineRenderAsset")]
    public class SpaceRunPipelineRenderAsset : RenderPipelineAsset
    {
        protected override RenderPipeline CreatePipeline()
        {
            return new SpaceRunPipelineRender();
        }
    }

}