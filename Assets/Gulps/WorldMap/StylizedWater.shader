Shader "Custom/StylizedWater"
{
    Properties
    {
        _ShallowColor ("Shallow Color", Color) = (0.38, 0.74, 0.86, 1)
        _DeepColor    ("Deep Color",    Color) = (0.16, 0.49, 0.73, 1)
        _FoamColor    ("Foam Color",    Color) = (1, 1, 1, 1)
        _WaveSpeed    ("Wave Speed",    Float) = 0.35
        _WaveScale    ("Wave Scale",    Float) = 1.0
        _FadeRadius   ("Fade Radius",   Float) = 45.0
        _CapStrength  ("Cap Strength",  Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings   { float4 positionHCS : SV_POSITION; float3 positionWS : TEXCOORD0; };

            CBUFFER_START(UnityPerMaterial)
                float4 _ShallowColor;
                float4 _DeepColor;
                float4 _FoamColor;
                float  _WaveSpeed;
                float  _WaveScale;
                float  _FadeRadius;
                float  _CapStrength;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings o;
                o.positionWS  = TransformObjectToWorld(IN.positionOS.xyz);
                o.positionHCS = TransformWorldToHClip(o.positionWS);
                return o;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float t = _Time.y;
                float3 wp = IN.positionWS;

                float d = saturate(length(wp.xz) / max(_FadeRadius, 1.0));
                float3 col = lerp(_ShallowColor.rgb, _DeepColor.rgb, d);

                // sparse, soft wave crests
                float w1 = sin((wp.x * 0.22 + wp.z * 0.16) * _WaveScale + t * _WaveSpeed * 2.4);
                float w2 = sin((wp.x * 0.13 - wp.z * 0.26) * _WaveScale - t * _WaveSpeed * 1.7);
                float caps = smoothstep(0.78, 0.99, w1 * w2);
                caps *= saturate(1.0 - d * 0.7);            // fade in the distance (kills moiré)
                col = lerp(col, _FoamColor.rgb, caps * _CapStrength);

                return half4(col, 1.0);
            }
            ENDHLSL
        }
    }
}
