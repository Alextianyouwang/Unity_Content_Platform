Shader "Custom/Additive_Unlit"
{
    Properties
    {
        _Albedo("Albedo", 2D) = "white"{}
            [HDR]_Tint("Tint",Color) = (0,1,0,1)
    }
    SubShader
    {
        Tags {"RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline"}
        Pass
        {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}
            Cull Off
            ZWrite Off
            Blend One One
            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag
      
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            TEXTURE2D(_Albedo); SAMPLER(sampler_Albedo);
            float4 _Tint;
            struct Input
            {
                float3 positionOS : POSITION;
                float4 uv : TEXCOORD0;
            };

            RWStructuredBuffer<float3> _InstancePosition;
            
            struct Interpolator
            {
                float4 positionCS : SV_POSITION;
                float4 uv : TEXCOORD0;
            };
            
            Interpolator vert(Input i, uint id: SV_InstanceID)
            {
                Interpolator o;
               o.positionCS = mul(UNITY_MATRIX_MVP, float4(i.positionOS, 1));
                o.uv = i.uv;
                return o;
            }
            
            half4 frag(Interpolator i) : SV_Target
            {
                half4 albedo = SAMPLE_TEXTURE2D(_Albedo, sampler_Albedo, i.uv.xy);
                return albedo * _Tint * i.uv.y;
            }

            ENDHLSL
        }
  
    }  
}
    

