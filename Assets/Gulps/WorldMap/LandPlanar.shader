Shader "Custom/LandPlanar"
{
    // Top-down (world-space XZ) planar projection of a flat 2D land illustration.
    // The source PNG is a hills-with-sky picture; we remap the sampled V into the
    // green band [_GreenMinV.._GreenMaxV] so the white/transparent sky is never
    // sampled ("crop sky"). Simple URP main-light + SH ambient so it sits in the lit diorama.
    Properties
    {
        _BaseMap    ("Land Texture", 2D)          = "white" {}
        _Tint       ("Tint",         Color)       = (1,1,1,1)
        _WorldScale ("World Scale (tiles/unit)", Float) = 0.03
        _GreenMinV  ("Green Band Min V", Range(0,1)) = 0.02
        _GreenMaxV  ("Green Band Max V", Range(0,1)) = 0.52
        _AmbientBoost ("Ambient Boost", Range(0,1)) = 0.30
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; };
            struct Varyings   { float4 positionHCS : SV_POSITION; float3 positionWS : TEXCOORD0; float3 normalWS : TEXCOORD1; };

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _Tint;
                float  _WorldScale;
                float  _GreenMinV;
                float  _GreenMaxV;
                float  _AmbientBoost;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings o;
                VertexPositionInputs p = GetVertexPositionInputs(IN.positionOS.xyz);
                o.positionWS  = p.positionWS;
                o.positionHCS = p.positionCS;
                o.normalWS    = TransformObjectToWorldNormal(IN.normalOS);
                return o;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Planar UV from world XZ; tile horizontally, keep V inside the green band.
                float2 uv;
                uv.x = frac(IN.positionWS.x * _WorldScale);
                uv.y = lerp(_GreenMinV, _GreenMaxV, frac(IN.positionWS.z * _WorldScale));

                half3 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv).rgb * _Tint.rgb;

                float3 n = normalize(IN.normalWS);
                Light main = GetMainLight();
                float ndotl = saturate(dot(n, main.direction)) * 0.5 + 0.5;   // half-lambert, soft
                half3 lighting = main.color.rgb * ndotl + SampleSH(n) + _AmbientBoost;

                return half4(albedo * lighting, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
