using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Reflection;
using System.Linq;
using UnityEngine.Rendering.Universal.Internal;

namespace UnityEngine.Rendering.Universal
{
	public static class UniversalRenderPipelineExtension
	{

		static Dictionary<string, MemberInfo> _cachedDict = new Dictionary<string, MemberInfo>();


		static MemberInfo GetMember<T>(string key,BindingFlags flag)
		{
			MemberInfo memberInfo = null;
			string dictKey = $"{typeof(T).Name}.{key}";
			if (!_cachedDict.TryGetValue(dictKey, out memberInfo))
			{
				var aAssetType = typeof(T);
				var mems = flag == BindingFlags.Default ? aAssetType.GetMember(key) : aAssetType.GetMember(key, flag);
				if (mems != null && mems.Length > 0)
				{
					memberInfo = mems[0];
					_cachedDict.Add(dictKey, memberInfo);
				}			
			}
			return memberInfo;
		}

		public static IEnumerable<ScriptableRendererData> IterRenderDatas(this UniversalRenderPipelineAsset rpAsset)
		{
			var rCount = rpAsset.GetRenderDataCount();
			for (int i = 0; i < rCount; i++)
			{
				var renderData = rpAsset.GetRenderData<ScriptableRendererData>(i);
				if (renderData == null)
					break;
				yield return renderData;
			}
		}

		public static void CloneRendererDatas(this UniversalRenderPipelineAsset asset)
		{
			var array = asset.GetRendererDataList();
			if (array == null)
				return;
			for (int i = 0;i < array.Length;i++)
			{
				array[i] = Object.Instantiate(array[i]);
			}
		}

		public static UniversalRendererData GetRendererData(this Camera cam)
		{
			if (cam == null) return null;
			var rp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
			if (rp == null)
				return null;
			var rCount = rp.GetRenderDataCount();
			for (int i = 0; i < rCount; i++)
			{
				if (cam != null && rp.GetRenderer(i) != cam.GetUniversalAdditionalCameraData().scriptableRenderer)
					continue;
				var renderData = rp.GetRenderData<UniversalRendererData>(i);
				if (renderData == null) continue;
				return renderData;
			}
			return null;
		}

		public static UniversalRendererData GetRendererData(this UniversalAdditionalCameraData cam)
		{
			if (cam == null) return null;
			var rp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
			if (rp == null)
				return null;
			var rCount = rp.GetRenderDataCount();
			for (int i = 0; i < rCount; i++)
			{
				if (cam != null &&  cam.scriptableRenderer != rp.GetRenderer(i) )
					continue;
				var renderData = rp.GetRenderData<UniversalRendererData>(i);
				if (renderData == null) continue;
				return renderData;
			}
			return null;
		}

		static ScriptableRendererData[] GetRendererDataList(this UniversalRenderPipelineAsset asset)
		{
			var memberInfo = GetMember<UniversalRenderPipelineAsset>("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);
			if (memberInfo == null)
				return null;
			return (memberInfo as FieldInfo).GetValue(asset) as ScriptableRendererData[];
		}

		static void SetRendererDataList(this UniversalRenderPipelineAsset asset, ScriptableRendererData[] array)
		{
			var memberInfo = GetMember<UniversalRenderPipelineAsset>("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);
			if (memberInfo == null)
				return ;
			(memberInfo as FieldInfo).SetValue(asset, array);
			asset.SetDirty();
		}

		public static int GetRenderDataCount(this UniversalRenderPipelineAsset asset)
		{
			var array = asset.GetRendererDataList();	
			return array == null ? 0 : array.Length;
		}

		public static T GetRenderData<T>(this UniversalRenderPipelineAsset asset, int index) where T : ScriptableRendererData
		{
			var array = asset.GetRendererDataList();
			if (array == null)
				return default(T);
			return array[index] as T;
		}

		public static void SetShadowResolution(this UniversalRenderPipelineAsset asset,int resolution)
		{
			var memberInfo1 = GetMember<UniversalRenderPipelineAsset>("supportsMainLightShadows",BindingFlags.Default);
			if (memberInfo1 == null)
				return;
			(memberInfo1 as PropertyInfo).SetValue(asset,resolution > 0);


			var memberInfo2 = GetMember<UniversalRenderPipelineAsset>("mainLightShadowmapResolution", BindingFlags.Default);
			if (memberInfo2 == null)
				return;
			if (resolution > 0)
				(memberInfo2 as PropertyInfo).SetValue(asset, resolution);

			var memberInfo3 = GetMember<UniversalRenderPipelineAsset>("supportsAdditionalLightShadows", BindingFlags.Default);
			if (memberInfo3 == null)
				return;
			(memberInfo3 as PropertyInfo).SetValue(asset, false);

			asset.shadowCascadeCount = Mathf.Min(2, asset.shadowCascadeCount);
		}

		public static void SetShadowEnable(this UniversalRenderPipelineAsset asset, bool enabled)
		{
			var memberInfo1 = GetMember<UniversalRenderPipelineAsset>("supportsMainLightShadows", BindingFlags.Default);
			if (memberInfo1 == null)
				return;
			(memberInfo1 as PropertyInfo).SetValue(asset, enabled);
		}

		public static void SetClonedRenderer(this UniversalAdditionalCameraData camData,string name)
		{
			if (string.IsNullOrEmpty(name))
				return;
			var rp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
			if (rp == null) return;
			camData.SetRenderer(name, true);
			var renderData = camData.GetRendererData();
			if (renderData == null) return;
			if (renderData.name == name) return;

	

			var newRenderData = Object.Instantiate(renderData);
			newRenderData.name = name;
			var array = rp.GetRendererDataList();
			System.Array.Resize(ref array,array.Length+1);
			array[array.Length-1] = newRenderData;
			rp.SetRendererDataList(array);
			camData.SetRenderer(array.Length-1);
		}


		public static void SetRenderer(this UniversalAdditionalCameraData camData, string name,bool contain)
		{
			int index = 0;
			var rp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
			if (rp == null) return;
			var rCount = rp.GetRenderDataCount();
			for (int i = 0; i < rCount; i++)
			{
				var renderData = rp.GetRenderData<ScriptableRendererData>(i);
				if (renderData == null) continue;
				if (contain && renderData.name.Contains(name) || !contain && renderData.name == name)
				{
					index = i;
					break;
				}
			}

			camData.SetRenderer(index); 
		}

		public static List<DrawObjectsPass> GetDrawObjectsPass(this UniversalRenderer renderer)
		{
			List<DrawObjectsPass> list = new List<DrawObjectsPass>();
			var memberInfo = GetMember<UniversalRenderer>("m_RenderOpaqueForwardPass", BindingFlags.NonPublic | BindingFlags.Instance);
			if (memberInfo != null)
			{	
				var opaquePass = (memberInfo as FieldInfo).GetValue(renderer) as DrawObjectsPass;
				if (opaquePass != null)
					list.Add(opaquePass);
			}
			var memberInfo2 = GetMember<UniversalRenderer>("m_RenderTransparentForwardPass", BindingFlags.NonPublic | BindingFlags.Instance);
			if (memberInfo2 != null)
			{

				var transPass = (memberInfo2 as FieldInfo).GetValue(renderer) as DrawObjectsPass;
				if (transPass != null)
					list.Add(transPass);
			}
			return list;
		}

		public static CopyDepthPass GetCopyDepthPass(this UniversalRenderer renderer)
		{
			var memberInfo = GetMember<UniversalRenderer>("m_CopyDepthPass", BindingFlags.NonPublic | BindingFlags.Instance);
			if (memberInfo == null)
				return null;
			var pass = (memberInfo as FieldInfo).GetValue(renderer) as CopyDepthPass;
			if (pass == null)
				return null;
			return pass;
		}
	}
}
