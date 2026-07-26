// ===========================================================================
//  GreatLibrary/MoteAdditive — soft golden light motes (F5)
//  Additive, vertex-colored, procedural soft dot (no texture needed).
//  Used by the FeedbackDirector's pooled ParticleSystem. Mobile-safe.
// ===========================================================================
Shader "GreatLibrary/MoteAdditive"
{
    Properties
    {
        _Softness ("Dot Softness", Range(0.05, 1)) = 0.55
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"
               "RenderPipeline"="UniversalPipeline" "IgnoreProjector"="True" }
        Blend SrcAlpha One          // additive: motes only ever ADD light
        ZWrite Off
        Cull Off

        Pass
        {
            Name "Mote"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half _Softness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                half4  color      : COLOR;      // particle color (HDR-capable)
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                half4  color      : COLOR;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float d = length(i.uv - 0.5) * 2.0;
                // Soft round dot with a hot little core
                half dot  = smoothstep(1.0, 1.0 - _Softness, d);
                half core = smoothstep(0.35, 0.0, d) * 0.6;
                half a = (dot + core) * i.color.a;
                return half4(i.color.rgb * a, a);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
