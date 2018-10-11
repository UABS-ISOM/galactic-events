// Copyright Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "Galaxy/Stars"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_WSScale ("World Space Scale", Float) = 1
		_TxScale ("TX Scale", Vector) = (0,0,0,0)
		[HDR] _Color ("Color", Color) = (1,1,1,1)
		_TransitionAlpha("Transition Alpha", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100


		Pass
		{
			Blend One One
			ZWrite Off
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geo
			
			#include "UnityCG.cginc"
			#include "StarVertDescriptor.cginc"
			#include "StarPositionCompute.cginc"

			struct v2g
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float size : TEXCOORD1;
				float4 color : COLOR0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float _WSScale;
			float4 _TxScale;

			float4 _Color;
			float _TransitionAlpha;

			StructuredBuffer<StarVertDescriptor> _Stars;
			float4x4 _GalaxyWorld;

			v2g vert (uint id : SV_VertexID, uint inst : SV_InstanceID)
			{
				StarVertDescriptor star = _Stars[id];

				v2g o;

				o.vertex = mul(_GalaxyWorld, float4(ComputeStarPosition(star), 1));
				o.vertex = mul(UNITY_MATRIX_VP, o.vertex);
				o.color = float4(star.color, 1) * _TransitionAlpha * _Color;
				o.uv = star.uv + float2(0, .5);
				o.size = star.size * _WSScale;

				return o;
			}

			[maxvertexcount(4)]
			void geo(point v2g p[1], inout TriangleStream<v2f> triStream)
			{
				float4 mvPos = p[0].vertex;

				float4 up = float4(0, 1, 0, 0) * UNITY_MATRIX_P._22;
				float4 right = float4(1, 0, 0, 0) * UNITY_MATRIX_P._11;
				float halfS = p[0].size;

				float4 v[4];
				v[0] = mvPos - halfS * up;
				v[1] = mvPos + halfS * right;
				v[2] = mvPos - halfS * right;
				v[3] = mvPos + halfS * up;

				v2f pIn;
				pIn.color = p[0].color;

				pIn.vertex = v[0];
				pIn.uv = float2(1.0f, 0.0f) * 0.5 + p[0].uv;
				triStream.Append(pIn);

				pIn.vertex = v[1];
				pIn.uv = float2(1.0f, 1.0f) * 0.5 + p[0].uv;
				triStream.Append(pIn);

				pIn.vertex = v[2];
				pIn.uv = float2(0.0f, 0.0f) * 0.5 + p[0].uv;
				triStream.Append(pIn);

				pIn.vertex = v[3];
				pIn.uv = float2(0.0f, 1.0f) * 0.5 + p[0].uv;
				triStream.Append(pIn);

				triStream.RestartStrip();
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv).a * float4(i.color.xyz, 1.0);
				color.a = dot(color.xyz, 1);

				return color;
			}
			ENDCG
		}
	}
}