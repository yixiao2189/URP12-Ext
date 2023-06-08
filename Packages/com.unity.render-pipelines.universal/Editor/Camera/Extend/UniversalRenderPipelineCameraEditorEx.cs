using UnityEngine;

namespace UnityEditor.Rendering.Universal
{
	internal partial class UniversalRenderPipelineCameraEditor
	{
		private SerializedProperty colorSpaceUsage;
		private SerializedProperty renderPostProcessing;

		private void DrawRenderSettingsEx()
		{
			EditorGUILayout.PropertyField(colorSpaceUsage);
		}

		private void InitEx(SerializedObject m_AdditionalCameraDataSO)
		{
			colorSpaceUsage = m_AdditionalCameraDataSO.FindProperty("m_ColorSpaceUsage");
			renderPostProcessing = m_AdditionalCameraDataSO.FindProperty("m_RenderPostProcessing");
		}

		private void DrawPostProcessingEx()
		{
			//if (renderPostProcessing.boolValue)
			//    EditorGUILayout.PropertyField(m_AdditionalCameraDataRenderAMDFSR, StylesEx.AMDFSR);
		}

		public static void DrawRenderSettingsEx(UniversalRenderPipelineSerializedCamera cam, Editor owner)
		{
			var e = owner as UniversalRenderPipelineCameraEditor;
			if (e == null)
				return;

			e.DrawRenderSettingsEx();
		}

		public static void DrawPostProcessingEx(Editor owner)
		{
			var e = owner as UniversalRenderPipelineCameraEditor;
			if (e == null)
				return;

			e.DrawPostProcessingEx();
		}
	}
}
