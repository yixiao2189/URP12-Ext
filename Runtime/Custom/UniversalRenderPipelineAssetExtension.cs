using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Reflection;

namespace UnityEngine.Rendering.Universal
{
	public static class UniversalRenderPipelineAssetExtension
	{

		static Dictionary<string, MemberInfo> _cachedDict = new Dictionary<string, MemberInfo>();


		static MemberInfo GetMember(string key,BindingFlags flag)
		{
			MemberInfo memberInfo = null;
			if (!_cachedDict.TryGetValue(key, out memberInfo))
			{
				var aAssetType = typeof(UniversalRenderPipelineAsset);
				var mems = flag == BindingFlags.Default ? aAssetType.GetMember(key) : aAssetType.GetMember(key, flag);
				if (mems != null && mems.Length > 0)
				{
					memberInfo = mems[0];
					_cachedDict.Add(key, memberInfo);
				}			
			}
			return memberInfo;
		}


		public static int GetRenderDataCount(this UniversalRenderPipelineAsset asset)
		{
			var memberInfo = GetMember("m_RendererDataList",BindingFlags.NonPublic | BindingFlags.Instance);
			if (memberInfo == null)
				return 0;
			IList list = (memberInfo as FieldInfo).GetValue(asset) as IList;
			return list.Count;
		}

		public static T GetRenderData<T>(this UniversalRenderPipelineAsset asset, int index) where T : ScriptableRendererData
		{
			var memberInfo = GetMember("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);
			if (memberInfo == null)
				return default(T);
			IList list = (memberInfo as FieldInfo).GetValue(asset) as IList;
			return list[index] as T;
		}

		public static void SetShadowResolution(this UniversalRenderPipelineAsset asset,int resolution)
		{
			var memberInfo1 = GetMember("supportsMainLightShadows",BindingFlags.Default);
			if (memberInfo1 == null)
				return;
			(memberInfo1 as PropertyInfo).SetValue(asset,resolution > 0);


			var memberInfo2 = GetMember("mainLightShadowmapResolution", BindingFlags.Default);
			if (memberInfo2 == null)
				return;
			if (resolution > 0)
				(memberInfo2 as PropertyInfo).SetValue(asset, resolution);

			var memberInfo3 = GetMember("supportsAdditionalLightShadows", BindingFlags.Default);
			if (memberInfo3 == null)
				return;
			(memberInfo3 as PropertyInfo).SetValue(asset, false);

			asset.shadowCascadeCount = Mathf.Min(2, asset.shadowCascadeCount);
		}


		public static void SetRenderer(this UniversalAdditionalCameraData camData, string name,bool contain)
		{
			int index = 0;
			var rp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
			if (rp != null)
			{
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
			}

			camData.SetRenderer(index); 
		}
	}
}
