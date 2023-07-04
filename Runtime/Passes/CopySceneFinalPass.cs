using System;

namespace UnityEngine.Rendering.Universal.Internal
{

	internal class CopySceneFinalPass : ScriptableRenderPass
    {
        private static readonly ProfilingSampler m_ProfilingScope = new ProfilingSampler("Copy Scene Final Pass");

        Material m_BlitMaterial;

		public CopySceneFinalPass(RenderPassEvent evt, Material blitMaterial)
        {
            m_BlitMaterial = blitMaterial;
            renderPassEvent = evt;
		}


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_BlitMaterial == null)
            {
                Debug.LogErrorFormat("Missing {0}. {1} render pass will not execute. Check for missing reference in the renderer resources.", m_BlitMaterial, GetType().Name);
                return;
            }

            ref CameraData cameraData = ref renderingData.cameraData;


			CommandBuffer cmd = CommandBufferPool.Get();


			var source = renderingData.cameraData.renderer.cameraColorTarget;

			using (new ProfilingScope(cmd, m_ProfilingScope))
            {
                GetActiveDebugHandler(renderingData)?.UpdateShaderGlobalPropertiesForFinalValidationPass(cmd, ref cameraData, true);

                if (cameraData.hasGammaUI)
                {
                    CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.LinearToSRGBConversion, true);
                    CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.SRGBToLinearConversion, false);
                }
                else
                {
                    CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.LinearToSRGBConversion,
                    cameraData.requireSrgbConversion);
                }

               
				if(cameraData.splitResolution)
					RenderTargetBufferSystem.ApplyScale(ref cameraData);
				RenderTargetIdentifier cameraTarget = cameraData.renderer.GetCameraColorFrontBuffer(cmd);


				cmd.SetGlobalTexture(ShaderPropertyId.sourceTex, source);


                cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget,
                    RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, // color
                    RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare); // depth
                cmd.Blit(cameraData.renderer.cameraColorTarget, cameraTarget, m_BlitMaterial);
                cameraData.renderer.ConfigureCameraTarget(cameraTarget, cameraTarget);

				cameraData.renderer.SwapColorBuffer(cmd);

                if (cameraData.hasGammaUI)
                {
                    CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.LinearToSRGBConversion, false);
                    CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.SRGBToLinearConversion, false);
                }
			}

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
