
Shader "Custom/ShadowMapSPCF" 
{
	Properties 
	{
		_DiffuseColor ("Diffuse Color", Color) = (1,1,1,1)
		_ShadowMap ("_ShadowMap", 2D) = "red"  {}
		_NearClip ("Near Clip", float)  = 0.3
		_FarClip ("Far Clip", float) = 1000
		_TexSize ("Tex Size", int) = 1024
		_Bias ("Bias", float) = 2.99
		_Blur("Blur", float ) = 5.0
	}
	SubShader 
	{
	    Tags { "RenderType"="Opaque" }
	    Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			uniform sampler2D _ShadowMap;
			uniform float4x4 _LightProjectionMat;
			uniform float4 _DiffuseColor;
			uniform float4 _LightPosition;
			uniform float _TexSize;
			uniform float _NearClip;
			uniform float _FarClip;
			uniform float _Bias;
			uniform float _Blur;

			struct vertInput 
			{
				float4 vertex :POSITION0;
				float3 normal : NORMAL0;
				float2 texcoord : TEXCOORD0;
			};

			struct vertOutput 
			{
				float4 color : COLOR0;
				float4 position :SV_POSITION;
				float4 texcoordScreen : TEXCOORD1;
			};

			vertOutput vert(vertInput v)
			{
				vertOutput output;

				float4 worldPosition = mul(unity_ObjectToWorld, v.vertex);

				float3 worldNormal = normalize(mul(float4(v.normal, 0), transpose(unity_WorldToObject)).xyz);

				float3 lightDir = normalize((_LightPosition - worldPosition).xyz);

				float diffuse = max(0, dot(worldNormal, lightDir)) * 0.1;

				output.color = (_DiffuseColor  * diffuse) + _DiffuseColor;

				output.position = UnityObjectToClipPos(v.vertex);

				output.texcoordScreen = mul( _LightProjectionMat ,worldPosition);

				return output;
			}
		
			float4 frag(vertOutput v) : COLOR 
			{
				v.texcoordScreen = v.texcoordScreen/ v.texcoordScreen.w;
				v.texcoordScreen.x = ( v.texcoordScreen.x/2) + 0.5f;
				v.texcoordScreen.y = ( v.texcoordScreen.y/2) + 0.5f;

				float depth = v.texcoordScreen.z;
				
				//linearize
				float sceneDepth = _NearClip * (depth + 1.0) / (_FarClip + _NearClip - depth * (_FarClip - _NearClip));

				float2 texelSize = float2(1/_TexSize, 1/_TexSize);
				float _bias = _Bias/_TexSize;

				//PCF
				float sample = (_Blur - 1.0f) / 2.0f;
				float shadowCoeff = 0;

				for(float y = -sample; y <= sample; y = y+1.0f)
				{
					for(float x = -sample; x <= sample; x = x+1.0f)
					{
					   float2 coordOffset = float2(x/ _TexSize,y/ _TexSize);
					   float2 sampleCoord = v.texcoordScreen.xy + coordOffset;
					   
					   //Bilinear Interpolation
					   float2 pixelPosition =sampleCoord/texelSize + float2(0.5, 0.5);
					   float2 fract = frac(pixelPosition);
					   float2 startTexel = ( pixelPosition - fract) *texelSize;

				       float dx = 1 /_TexSize;
					   float s0 = (tex2D(_ShadowMap, startTexel).x  < sceneDepth - _bias) ? 0.3 :1.0f;
				       float s1 = (tex2D(_ShadowMap, startTexel + float2(dx, 0.0f)).x < sceneDepth - _bias) ? 0.3 :1.0f;
					   float s2 = (tex2D(_ShadowMap, startTexel + float2 (0.0f, dx)).x  < sceneDepth - _bias) ? 0.3 :1.0f;
					   float s3 = (tex2D(_ShadowMap, startTexel + float2 (dx, dx)).x  < sceneDepth - _bias) ? 0.3 :1.0f;

					   shadowCoeff += lerp(lerp(s0, s1, fract.x), lerp(s2, s3, fract.x) , fract.y);
					}
				}
				shadowCoeff = shadowCoeff/ (_Blur * _Blur);
			   
				return shadowCoeff * v.color;
			}

		ENDCG
		}
	}
}
