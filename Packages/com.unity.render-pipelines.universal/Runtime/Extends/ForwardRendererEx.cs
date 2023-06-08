using UnityEngine.Rendering.Universal.Internal;

namespace UnityEngine.Rendering.Universal
{
    partial class UniversalRenderer
    {
        private DrawObjectsPassEx drawUIObjectPass;

        private BlitPassEx gammaPrePass, gammaPostPass;

        /// <summary>
        /// 
        /// Inject to end of UniversalRenderer's ctor
        /// </summary>
        /// <param name="data"></param>
        public void InitEx(UniversalRendererData data)
        {
            InitCameraGammaRendering(data);
        }

        public void SetupEx(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            SetupCameraGammaRendering(context, ref renderingData);
        }

        public void DisposeEx()
        {
            gammaPostPass.Cleanup();
        }
        
        public void InitCameraGammaRendering(UniversalRendererData data)
        {
            gammaPrePass = new BlitPassEx(nameof(gammaPrePass), RenderPassEvent.AfterRenderingSkybox + 1, m_BlitMaterial);
            
            gammaPostPass = new BlitPassEx(nameof(gammaPostPass), RenderPassEvent.AfterRendering + 20, m_BlitMaterial);
        }

        public void SetupCameraGammaRendering(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;

            var isUICamera = QualitySettings.activeColorSpace == ColorSpace.Linear &&
                             cameraData.exData.colorSpaceUsage == ColorSpace.Gamma &&
                             cameraData.renderType == CameraRenderType.Overlay;

            if (isUICamera)
            {
	            m_RenderTransparentForwardPass.ConfigureTarget(ShaderPropertyId._FULLSIZE_GAMMA_TEX);
                
	            //remove original blit pass
                DequeuePass(m_FinalBlitPass); // ui cammera use gammaPostPass

                gammaPrePass.SetupPrePass(cameraData.cameraTargetDescriptor, m_ActiveCameraColorAttachment);
                EnqueuePass(gammaPrePass);

                gammaPostPass.SetupPostPass(cameraData.cameraTargetDescriptor, m_ActiveCameraColorAttachment);
                EnqueuePass(gammaPostPass);
                
            }
        }

        public static bool IsApplyFinalPostProcessing(ref RenderingData renderingData, bool anyPostProcessing,
            bool lastCameraInTheStack)
        {
            return anyPostProcessing && lastCameraInTheStack &&
                   renderingData.cameraData.antialiasing == AntialiasingMode.FastApproximateAntialiasing ||
                   renderingData.cameraData.imageScalingMode == ImageScalingMode.Upscaling &&
                   renderingData.cameraData.upscalingFilter != ImageUpscalingFilter.Linear;
        }
    }
}
