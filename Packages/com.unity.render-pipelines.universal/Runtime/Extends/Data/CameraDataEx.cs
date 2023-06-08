namespace UnityEngine.Rendering.Universal
{
	public struct CameraDataEx
	{
		public ColorSpace colorSpaceUsage;

		public bool NeedLinearToSRGB()
		{
			return colorSpaceUsage == ColorSpace.Gamma
			       && QualitySettings.activeColorSpace == ColorSpace.Linear;
		}
	}
}
