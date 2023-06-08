namespace UnityEngine.Rendering.Universal.Internal
{
    public partial class PostProcessPass
    {
	    public void SetupFinalPass(in RenderTargetHandle source, bool useSwapBuffer,
            in RenderTextureDescriptor finalDesc = new RenderTextureDescriptor(), bool hasExternalPostPasses = true)
        {
            m_Source = source.id;
            m_Destination = RenderTargetHandle.CameraTarget;
            m_IsFinalPass = true;
            m_HasFinalPass = false;
            m_EnableSRGBConversionIfNeeded = true;
            m_Descriptor = finalDesc;
            m_UseSwapBuffer = useSwapBuffer;
            m_hasExternalPostPasses = hasExternalPostPasses;
        }
    }
}
