using UnityEngine.Rendering.Universal.Internal;

namespace UnityEngine.Rendering.Universal
{
    public partial class UniversalRenderPipeline
    {
	    private static void InitializeAdditionalCameraDataEx(UniversalAdditionalCameraData additionalCameraData, bool hasGammaUI,ref CameraData cameraData)
        {
			if (additionalCameraData == null)
				return;

			cameraData.gammmaUICamera = QualitySettings.activeColorSpace == ColorSpace.Linear 
				&& additionalCameraData.ColorSpaceUsage == ColorSpace.Gamma	&&
				cameraData.renderType == CameraRenderType.Overlay;

			cameraData.hasGammaUI = hasGammaUI && cameraData.renderType == CameraRenderType.Base;

			cameraData.splitResolution = cameraData.targetTexture == null && !cameraData.isSceneViewCamera && cameraData.renderScale < RenderTargetBufferSystem.overlayMinScale;
		}

    }
}
