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
	public partial class UniversalRenderer
	{
		public RenderPassEvent customCopyDepthEvent ;
		public bool customCopyDepth = false;
	}
}
