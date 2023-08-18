using UnityEngine.Rendering.Universal.Internal;

namespace UnityEngine.Rendering.Universal
{
    public partial class UniversalRenderPipeline
    {
	    private static void InitializeAdditionalCameraDataEx(UniversalAdditionalCameraData additionalCameraData, bool nextIsUI, bool nextIsGamma,ref CameraData cameraData)
        {
			if (additionalCameraData == null)
				return;

			cameraData.gammmaUICamera = QualitySettings.activeColorSpace == ColorSpace.Linear 
				&& additionalCameraData.ColorSpaceUsage == ColorSpace.Gamma	&&
				cameraData.renderType == CameraRenderType.Overlay;

			cameraData.isUICamera = cameraData.renderType == CameraRenderType.Overlay && additionalCameraData.ColorSpaceUsage != ColorSpace.Uninitialized ;
			cameraData.nextIsUI = nextIsUI;
			cameraData.nextIsGamma = nextIsGamma;
		

			cameraData.splitResolution = cameraData.targetTexture == null && !cameraData.isSceneViewCamera && cameraData.renderScale < RenderTargetBufferSystem.overlayMinScale;
			cameraData.nowSplit = cameraData.splitResolution && cameraData.nextIsUI || nextIsGamma;
		}

    }
}
