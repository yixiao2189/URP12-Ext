namespace UnityEngine.Rendering.Universal
{
	public partial class UniversalAdditionalCameraData
	{
		[SerializeField] 
		private ColorSpace m_ColorSpaceUsage = ColorSpace.Linear;

		public ColorSpace ColorSpaceUsage
		{
			get => m_ColorSpaceUsage;
			set => m_ColorSpaceUsage = value;
		}
	}
}
