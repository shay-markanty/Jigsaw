Shader "Unlit/MaskShader"
{
	Properties
	{
		_BackTex ("Texture", 2D) = "white" {}
		_CenterTex ("Texture", 2D) = "white" {}
		_LeftTex("Texture", 2D) = "white" {}
		_RightTex("Texture", 2D) = "white" {}
		_TopTex("Texture", 2D) = "white" {}
		_BottomTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
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

			sampler2D _BackTex;
			sampler2D _CenterTex;
			sampler2D _LeftTex;
			sampler2D _RightTex;
			sampler2D _TopTex;
			sampler2D _BottomTex;
			float4 _BackTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _BackTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 pixelColor = tex2D(_BackTex, i.uv);
				fixed4 maskColor = max(tex2D(_LeftTex, i.uv), 
									max(tex2D(_RightTex, i.uv), 
									 max(tex2D(_TopTex, i.uv), 
									  max(tex2D(_BottomTex, i.uv), 
									   tex2D(_CenterTex, i.uv)))));

				// fixed4 maskColor = tex2D(_LeftTex, i.uv) + tex2D(_RightTex, i.uv) + tex2D(_TopTex, i.uv) + tex2D(_BottomTex, i.uv) + tex2D(_CenterTex, i.uv);

				if (maskColor.a == 0) discard;
				fixed4 col = pixelColor * maskColor;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
