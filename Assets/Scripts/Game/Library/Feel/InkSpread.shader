// The gold-ink spread of the Shared Restoration Grammar (Bible Ch. 2.1 / 8.4):
// a radial reveal of Candle Gold with a bright leading wavefront, driven 0->1 by
// _Progress over ~0.4 s. Unlit + transparent so it overlays the page/word quad.
// Follows the project's hand-written URP unlit convention (see StylizedWater.shader):
// UniversalPipeline tag, Core.hlsl, CBUFFER_START(UnityPerMaterial), no LightMode tag.
Shader "Custom/InkSpread"
{
    Properties
    {
        _InkColor  ("Ink (Candle Gold)", Color) = (0.949, 0.698, 0.298, 1)   // #F2B24C
        _EdgeColor ("Leading Edge",      Color) = (1.0, 0.93, 0.66, 1)
        _Progress  ("Progress",          Range(0,1)) = 0
        _Alpha     ("Master Alpha",      Range(0,1)) = 1
        _EdgeWidth ("Edge Width",        Range(0.001, 0.5)) = 0.12
        _Softness  ("Softness",          Range(0.0001, 0.3)) = 0.05
        _Reach     ("Reach",             Float) = 0.75
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };

            CBUFFER_START(UnityPerMaterial)
                float4 _InkColor;
                float4 _EdgeColor;
                float  _Progress;
                float  _Alpha;
                float  _EdgeWidth;
                float  _Softness;
                float  _Reach;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                o.uv = IN.uv;
                return o;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float  d      = length(IN.uv - 0.5);       // distance from quad centre (0..~0.7)
                float  radius = _Progress * _Reach;

                // Candle Gold fills inside the growing radius.
                float fill = smoothstep(radius, radius - _Softness, d);
                // A bright wavefront ring rides the growing edge and fades as it settles.
                float ring = smoothstep(_EdgeWidth, 0.0, abs(d - radius)) * (1.0 - _Progress);

                float3 col = lerp(_InkColor.rgb, _EdgeColor.rgb, saturate(ring));
                float  a   = saturate(fill * _InkColor.a + ring) * _Alpha;
                return half4(col, a);
            }
            ENDHLSL
        }
    }
}
