Shader "Unlit/MaskShader"
{
	Properties
	{
		_BackTex ("Texture", 2D) = "white" {}
		_MaskTex("Texture", 2D) = "white" {}
		/*_CenterTex ("Texture", 2D) = "white" {}
		_LeftTex("Texture", 2D) = "white" {}
		_RightTex("Texture", 2D) = "white" {}
		_TopTex("Texture", 2D) = "white" {}
		_BottomTex("Texture", 2D) = "white" {}*/
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
				float2 uvMask: TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _BackTex;
			sampler2D _MaskTex;
			float4 _BackTex_ST;
			float4 _MaskTex_ST;
			float4 _MaskTex_TexelSize;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _BackTex);
				o.uvMask = TRANSFORM_TEX(v.uv, _MaskTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 pixelColor = tex2D(_BackTex, i.uv);
				fixed4 maskColor = tex2D(_MaskTex, i.uvMask);

				if (maskColor.a == 0) discard;

				float2 offsets[8] = {
					float2(-1, 0),
					float2(0, -1),
					float2(0, 1),
					float2(1, 0),
					float2(-1, 1),
					float2(1, -1),
					float2(1, 1),
					float2(-1, -1)
				};
				
				for (int j = 0; j < 8; j++) {
					float2 p = i.uvMask + offsets[j] * _MaskTex_TexelSize;
					if (p.x > 1 || p.y > 1 || p.x < 0 || p.y < 0)
						return fixed4(0, 0, 0, 1);

					fixed4 sampledValue = tex2D(_MaskTex, p);
					if (sampledValue.a == 0)
						return fixed4(0, 0, 0, 1);
				}

				//fixed4 maskColor = (tex2D(_LeftTex, i.uv) + tex2D(_RightTex, i.uv) + tex2D(_TopTex, i.uv) + tex2D(_BottomTex, i.uv) + tex2D(_CenterTex, i.uv)) / 5;
				
				fixed4 col = pixelColor * maskColor;
				
				return col;
			}
			ENDCG
		}
	}
}
