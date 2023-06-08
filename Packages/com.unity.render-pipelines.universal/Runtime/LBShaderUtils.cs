using System;
using System.Linq;

namespace UnityEngine.Rendering.Universal
{
	public enum LBShaderPathID
	{
		Lit,
		LitInstancing,
		Unlit,

		Fresnel,
		Ghost,
		Flag,
		MatCap,
		Wave,
		ScreenFlow,
		Grass,
		Lightning,	
		Dissolve,
		Fur,
	}

	public class LBPassName
	{
		public const string OutlineStencil = "OutlineStencil";
		public const string OutlineFill = "OutlineFill";
		public const string ZWriteAlways = "ZWriteAlways";
	}

	public static class LBShaderUtils
	{
		public static readonly string[] s_ShaderPaths =
		{
			"LB/URP/Lit",
			"LB/URP/LitInstancing",
			"LB/URP/Unlit",

			"LB/URP/Fresnel",
			"LB/URP/Ghost",
			"LB/URP/Flag",
			"LB/URP/MatCap",
			"LB/URP/Wave",
			"LB/URP/ScreenFlow",
			"LB/URP/Grass",
			"LB/URP/Lightning",
			"LB/URP/Dissolve",
			"LB/URP/Fur"
		};

		public static string GetShaderPath(LBShaderPathID id)
		{
			int index = (int)id;
			int arrayLength = s_ShaderPaths.Length;
			if (arrayLength > 0 && index >= 0 && index < arrayLength)
				return s_ShaderPaths[index];

			Debug.LogError("Trying to access universal shader path out of bounds: (" + id + ": " + index + ")");
			return "";
		}

		public static ShaderPathID GetEnumFromPath(string path)
		{
			var index = Array.FindIndex(s_ShaderPaths, m => m == path);
			return (ShaderPathID)index;
		}

		public static void EnableOutline(Material material, bool enable)
		{
			material.SetShaderPassEnabled(LBPassName.OutlineStencil, enable);
			material.SetShaderPassEnabled(LBPassName.OutlineFill, enable);
		}

		public static void EnableZWriteAlwaysPass(Material material, bool enable)
		{
			material.SetShaderPassEnabled(LBPassName.ZWriteAlways, enable);
		}
	}
}
