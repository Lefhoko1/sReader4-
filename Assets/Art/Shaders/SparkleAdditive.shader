// ===========================================================================
//  GreatLibrary/SparkleAdditive — soft textured sparkles, rings and trails
//  Additive, vertex-colored, samples a sprite's alpha. The textured sibling of
//  GreatLibrary/MoteAdditive (which is procedural and ignores its texture).
//  Used by StoryBook's sentence FX. Mobile-safe.
//
//  BUILD NOTE: add "GreatLibrary/SparkleAdditive" to
//  Project Settings > Graphics > Always Included Shaders (Shader.Find is
//  stripped from device builds otherwise).
// ===========================================================================
Shader "GreatLibrary/SparkleAdditive"
{
    Properties
    {
        [MainTexture] _BaseMap   ("Sprite", 2D) = "white" {}
        [MainColor]   _BaseColor ("Tint", Color) = (1,1,1,1)
        _Intensity ("Glow Intensity", Range(0.25, 4)) = 1.35
        _EdgeFade  ("Quad Edge Fade", Range(0, 0.6)) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"
               "RenderPipeline"="UniversalPipeline" "IgnoreProjector"="True"
               "PreviewType"="Plane" }
        Blend SrcAlpha One          // additive: sparkles only ever ADD light
        ZWrite Off
        Cull Off

        Pass
        {
            Name "Sparkle"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4  _BaseColor;
                half   _Intensity;
                half   _EdgeFade;
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
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                o.color = v.color;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                half3 rgb = tex.rgb * i.color.rgb * _BaseColor.rgb;
                half  a   = tex.a  * i.color.a   * _BaseColor.a;

                // Never let the billboard's own corners show: fade the last
                // sliver of the quad out, whatever the sprite does.
                float d = length(i.uv - 0.5) * 2.0;
                a *= smoothstep(1.0, 1.0 - max(_EdgeFade, 1e-3), d);

                a *= _Intensity;
                return half4(rgb * a, saturate(a));
            }
            ENDHLSL
        }
    }
    FallBack Off
}
