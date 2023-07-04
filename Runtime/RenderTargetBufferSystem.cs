using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine.Rendering.Universal.Internal
{
    //NOTE: This class is meant to be removed when RTHandles get implemented in urp
    internal sealed class RenderTargetBufferSystem
    {

        struct SwapBuffer
        {
            public RenderTargetHandle rt;
            public int name;
            public int msaa;
        }
        SwapBuffer m_A, m_B;
        static bool m_AisBackBuffer = true;

        public static RenderTextureDescriptor m_Desc;
        FilterMode m_FilterMode;
        bool m_AllowMSAA = true;
        bool m_RTisAllocated = false;

        SwapBuffer backBuffer { get { return m_AisBackBuffer ? m_A : m_B; } }
        SwapBuffer frontBuffer { get { return m_AisBackBuffer ? m_B : m_A; } }




		public const float overlayMinScale = 1f;
		static bool m_needUpdateA = false;
		static bool m_needUpdateB = false;
	

		public static void ApplyScale(ref CameraData cameraData)
		{
			int width = (int)(cameraData.pixelRect.width * Mathf.Max(cameraData.renderScale, overlayMinScale));
			int height = (int)(cameraData.pixelRect.height * Mathf.Max(cameraData.renderScale, overlayMinScale));
			if (width == m_Desc.width && m_Desc.height == height)
				return;
			m_Desc.width = width;
			m_Desc.height = height;
         
            m_needUpdateA = true;
			m_needUpdateB = true;
		}

		static bool needUpdateBack {
			get { return m_AisBackBuffer ? m_needUpdateA : m_needUpdateB; }
			set {
				if (m_AisBackBuffer)
					m_needUpdateA = value;
				else
					m_needUpdateB = value;
			}
		}

		static bool needUpdateFront { 
			get { return m_AisBackBuffer ? m_needUpdateB : m_needUpdateA; }
			set {
				if (m_AisBackBuffer)
					m_needUpdateB = value;
				else
					m_needUpdateA = value;
			}
		}



		public RenderTargetBufferSystem(string name)
        {
            m_A.name = Shader.PropertyToID(name + "A");
            m_B.name = Shader.PropertyToID(name + "B");
            m_A.rt.Init(name + "A");
            m_B.rt.Init(name + "B");
        }

        public RenderTargetHandle GetBackBuffer()
        {
            return backBuffer.rt;
        }

        public RenderTargetHandle GetBackBuffer(CommandBuffer cmd)
        {
            if (!m_RTisAllocated)
                Initialize(cmd);
			if (needUpdateBack)
			{
				cmd.ReleaseTemporaryRT(backBuffer.name);
				cmd.GetTemporaryRT(backBuffer.name, m_Desc, m_FilterMode);
				needUpdateBack = false;
			}
            return backBuffer.rt;
        }

        public RenderTargetHandle GetFrontBuffer(CommandBuffer cmd)
        {
            if (!m_RTisAllocated)
                Initialize(cmd);

            int pipelineMSAA = m_Desc.msaaSamples;
            int bufferMSAA = frontBuffer.msaa;

            if (m_AllowMSAA && bufferMSAA != pipelineMSAA)
            {
                //We don't want a depth buffer on B buffer
                var desc = m_Desc;
                if (m_AisBackBuffer)
                    desc.depthBufferBits = 0;

                cmd.ReleaseTemporaryRT(frontBuffer.name);
                cmd.GetTemporaryRT(frontBuffer.name, desc, m_FilterMode);

                if (m_AisBackBuffer)
                    m_B.msaa = desc.msaaSamples;
                else m_A.msaa = desc.msaaSamples;
            }
            else if (!m_AllowMSAA && bufferMSAA > 1)
            {
                //We don't want a depth buffer on B buffer
                var desc = m_Desc;
                desc.msaaSamples = 1;
                if (m_AisBackBuffer)
                    desc.depthBufferBits = 0;

                cmd.ReleaseTemporaryRT(frontBuffer.name);
                cmd.GetTemporaryRT(frontBuffer.name, desc, m_FilterMode);

                if (m_AisBackBuffer)
                    m_B.msaa = desc.msaaSamples;
                else m_A.msaa = desc.msaaSamples;
            }
            else if (needUpdateFront)
            {
                cmd.ReleaseTemporaryRT(frontBuffer.name);
                cmd.GetTemporaryRT(frontBuffer.name, m_Desc, m_FilterMode);
            }

            needUpdateFront = false;

            return frontBuffer.rt;
        }

        public void Swap()
        {
            m_AisBackBuffer = !m_AisBackBuffer;
        }

        void Initialize(CommandBuffer cmd)
        {
            m_A.msaa = m_Desc.msaaSamples;
            m_B.msaa = m_Desc.msaaSamples;

            cmd.GetTemporaryRT(m_A.name, m_Desc, m_FilterMode);
            var descB = m_Desc;
            //descB.depthBufferBits = 0;
            cmd.GetTemporaryRT(m_B.name, descB, m_FilterMode);

            m_RTisAllocated = true;

			m_needUpdateA = false;
			m_needUpdateB = false;
		}

        public void Clear(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(m_A.name);
            cmd.ReleaseTemporaryRT(m_B.name);

            m_AisBackBuffer = true;
            m_AllowMSAA = true;

			m_needUpdateA = false;
			m_needUpdateB = false;
		}

        public void SetCameraSettings(CommandBuffer cmd, RenderTextureDescriptor desc, FilterMode filterMode)
        {
            Clear(cmd); //SetCameraSettings is called when new stack starts rendering. Make sure the targets are updated to use the new descriptor.

            m_Desc = desc;
            m_FilterMode = filterMode;
            Initialize(cmd);
        }

        public RenderTargetHandle GetBufferA()
        {
            return m_A.rt;
        }

        public void EnableMSAA(bool enable)
        {
            m_AllowMSAA = enable;
        }
    }
}
