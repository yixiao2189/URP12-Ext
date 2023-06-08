namespace UnityEngine.Rendering.Universal
{
    public partial class UniversalRenderPipeline
    {
	    private static void InitialCameraDataEx(UniversalAdditionalCameraData additionalCameraData, ref CameraData cameraData)
        {
            InitialCameraDataFsr(additionalCameraData, ref cameraData);
        }

        private static void InitialCameraDataFsr(UniversalAdditionalCameraData additionalCameraData, ref CameraData cameraData)
        {
            if (additionalCameraData != null)
            {
                cameraData.exData.colorSpaceUsage = additionalCameraData.ColorSpaceUsage;
            }
        }
    }
}
