using UnityEngine;
using System.Linq;
namespace UnityEditor.Rendering.Universal
{
	internal partial class UniversalRenderPipelineCameraEditor
	{
		private SerializedProperty colorSpaceUsage;

		private void DrawRenderSettingsEx()
		{
			EditorGUILayout.PropertyField(colorSpaceUsage);
		}

		private void InitEx(SerializedObject m_AdditionalCameraDataSO)
		{
			colorSpaceUsage = m_AdditionalCameraDataSO.FindProperty("m_ColorSpaceUsage");
		}


		public static void DrawRenderSettingsEx(UniversalRenderPipelineSerializedCamera cam, Editor owner)
		{
			var e = owner as UniversalRenderPipelineCameraEditor;
			if (e == null)
				return;

			var addiData = cam.camerasAdditionalData.FirstOrDefault();
			if (addiData == null || addiData.renderType != UnityEngine.Rendering.Universal.CameraRenderType.Overlay)
				return;
			e.DrawRenderSettingsEx();
		}


	}
}
