Shader "Unlit/UnlitOutLine"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_LineColor("Line Color", Color) = (1, 1, 1, 1)
		_LineWidth("Line Width", Range(0.001, 0.2)) = 0.01
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			//Line
			Pass
			{
			//Back Side
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _LineWidth;
			fixed4 _LineColor;

			v2f vert(appdata v)
			{
				//offset•ûŒü‚ÌŒvŽZ
				float3 normal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
				float2 offset = TransformViewToProjection(normal.xy);

				//OutLine‚Æ‚µ‚ÄƒŒƒ“ƒ_ƒŠƒ“ƒO‚³‚ê‚éƒ|ƒŠƒSƒ“
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex.xy = o.vertex.xy + offset * _LineWidth;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = _LineColor;
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}

			//Unlit
			Pass
			{
				//Surface
				Cull Back

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
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

				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
			}

		}
}
