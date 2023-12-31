Shader "Unlit/UIShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline"  "RenderType"="Opaque" "Queue" = "Transparent"}
        LOD 100

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always

        Fog { Mode Off }
        Blend SrcAlpha  OneMinusSrcAlpha


        Pass
        {
            CGPROGRAM


            #pragma multi_compile_fragment _ _LINEAR_TO_SRGB_CONVERSION

            #pragma vertex vert
            #pragma fragment frag
        

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                #ifdef _LINEAR_TO_SRGB_CONVERSION
                    col.rgb = LinearToGammaSpace(col.rgb);
                #endif
            
                return col;
            }
            ENDCG
        }
    }
}
