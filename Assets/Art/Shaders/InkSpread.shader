// ===========================================================================
//  GreatLibrary/InkSpread — the "correct answer" restoration reveal (F5)
//  Radial gold-ink spread with an organic noisy edge and an HDR glow front
//  bright enough to trip URP Bloom (threshold ~1.1 per Bible Ch. 8.6).
//  Driven by FeedbackDirector via MaterialPropertyBlock:
//     _Progress  0 -> 1  over ~0.4 s   (reveal)
//     _Fade      1 -> 0  afterwards    (dissolve away)
//  SRP-batcher compatible (CBUFFER). Mobile-safe: one pass, no textures.
// ===========================================================================
Shader "GreatLibrary/InkSpread"
{
    Properties
    {
        _InkColor  ("Ink Color",  Color) = (0.83, 0.62, 0.25, 1)
        _EdgeColor ("Edge Glow (HDR)", Color) = (2.2, 1.6, 0.7, 1)
        _Progress  ("Progress", Range(0,1)) = 0
        _Fade      ("Fade",     Range(0,1)) = 1
        _EdgeWidth ("Edge Width", Range(0.01,0.5)) = 0.12
        _NoiseAmp  ("Edge Noise", Range(0,0.5)) = 0.18
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"
               "RenderPipeline"="UniversalPipeline" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "InkSpread"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _InkColor;
                half4 _EdgeColor;
                half  _Progress;
                half  _Fade;
                half  _EdgeWidth;
                half  _NoiseAmp;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            // Cheap value noise — organic ink edge without any texture fetch.
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }
            float vnoise(float2 p)
            {
                float2 i = floor(p), f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(hash21(i),               hash21(i + float2(1,0)), f.x),
                            lerp(hash21(i + float2(0,1)), hash21(i + float2(1,1)), f.x),
                            f.y);
            }

            half4 frag (Varyings i) : SV_Target
            {
                float2 c   = i.uv - 0.5;
                float  ang = atan2(c.y, c.x);
                // Organic radius: noise varies by angle + a little by radius
                float  n   = vnoise(float2(ang * 2.2, 7.31)) * 0.7
                           + vnoise(i.uv * 6.0) * 0.3;
                float  r   = length(c) * 2.0 + (n - 0.5) * _NoiseAmp;

                // Reveal front sweeping outward with _Progress (0..1.15 covers corners)
                float front = _Progress * 1.15;
                float ink   = smoothstep(front, front - 0.05, r);     // filled area
                float edge  = smoothstep(front, front - _EdgeWidth, r)
                            - smoothstep(front - _EdgeWidth,
                                         front - _EdgeWidth * 2.0, r); // glow band

                // Ink body has painterly density variation
                half  body  = ink * (0.75 + 0.25 * vnoise(i.uv * 14.0));

                half3 col   = _InkColor.rgb * body + _EdgeColor.rgb * edge;
                half  alpha = saturate(body * _InkColor.a + edge) * _Fade;

                // Soft circular vignette so the quad never shows square corners
                alpha *= smoothstep(1.05, 0.85, length(c) * 2.0);

                return half4(col, alpha);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
