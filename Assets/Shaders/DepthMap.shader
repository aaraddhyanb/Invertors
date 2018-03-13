
Shader "DepthMap"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
 
        Pass
        {
 
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
			//Unity's inbuilt depth texture
            sampler2D _CameraDepthTexture; 
 
            struct v2f
            {
                float4 position : SV_POSITION;
                float4 projectedPosition : TEXCOORD1;
            };
            
			v2f vert(appdata_base v)
            {
                v2f output;
                output.position = UnityObjectToClipPos(v.vertex);
                output.projectedPosition = ComputeScreenPos(output.position);
                return output;
            }
 
            float4 frag(v2f i) : COLOR
            {
                //Linear01Depth is a function used to restrict the value to [0, 1] 
				//In this case the depth value from the depth texture
                float depthValue = Linear01Depth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projectedPosition)).r);
                //return EncodeFloatRGBA(depthValue);
				return float4(depthValue, 0, 0, 0);
            }
 
            ENDCG
        }
	}
	 FallBack "VertexLit"
}