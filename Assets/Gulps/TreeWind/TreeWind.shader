Shader "Custom/TreeWind"
{
    // Wind-animated foliage shader for URP.
    // Reads per-vertex wind weights baked in Blender:
    //   vertex color R = bend weight   (0 trunk base -> 1 canopy / branch tips)
    //   vertex color G = flutter weight (0 trunk, 1 leaves)
    //   vertex color B = phase offset   (random per leaf so they desync)
    Properties
    {
        _BaseColor      ("Base Color", Color) = (0.18, 0.35, 0.12, 1)
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2  // Back; set Off for leaf cards

        _WindDir        ("Wind Direction", Vector) = (1, 0, 0.3, 0)
        _WindStrength   ("Wind Strength", Float)   = 0.3
        _WindSpeed      ("Wind Speed", Float)      = 1.2
        _FlutterStrength("Flutter Strength", Float)= 0.08
        _FlutterSpeed   ("Flutter Speed", Float)   = 6.0
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" "Queue" = "Geometry" }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            half4  _BaseColor;
            float  _Cull;
            float4 _WindDir;
            float  _WindStrength;
            float  _WindSpeed;
            float  _FlutterStrength;
            float  _FlutterSpeed;
        CBUFFER_END

        struct Attributes
        {
            float4 positionOS : POSITION;
            float3 normalOS   : NORMAL;
            float4 color      : COLOR;   // baked wind weights
        };

        // Displace a vertex in object space using the baked wind weights.
        float3 ApplyWind(float3 positionOS, float3 normalOS, float4 vcol)
        {
            float bendW    = vcol.r;
            float flutterW = vcol.g;
            float phase    = vcol.b * 6.2831853;
            float t        = _Time.y;

            float3 wdir = normalize(_WindDir.xyz + float3(1e-5, 0, 0));

            // Coherent sway: a wave that travels up the tree (positionOS.y term),
            // amplitude scaled by the bend weight so the base stays rigid.
            float sway = sin(t * _WindSpeed + positionOS.y * 0.15 + phase);
            float3 bend = wdir * (sway * _WindStrength * bendW);

            // High-frequency leaf flutter along the normal, masked to leaves by G,
            // desynced per leaf by the phase.
            float fl = sin(t * _FlutterSpeed + phase * 3.0);
            float3 flutter = normalOS * (fl * _FlutterStrength * flutterW);

            return positionOS + bend + flutter;
        }
        ENDHLSL

        // ---------------------------------------------------------------- Forward lit
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float  fogCoord   : TEXCOORD2;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 posOS = ApplyWind(IN.positionOS.xyz, IN.normalOS, IN.color);
                VertexPositionInputs p = GetVertexPositionInputs(posOS);
                VertexNormalInputs   n = GetVertexNormalInputs(IN.normalOS);
                OUT.positionCS = p.positionCS;
                OUT.positionWS = p.positionWS;
                OUT.normalWS   = n.normalWS;
                OUT.fogCoord   = ComputeFogFactor(p.positionCS.z);
                return OUT;
            }

            half4 frag(Varyings IN, FRONT_FACE_TYPE facing : FRONT_FACE_SEMANTIC) : SV_Target
            {
                half3 albedo = _BaseColor.rgb;
                half3 N = normalize(IN.normalWS);
                N = IS_FRONT_VFACE(facing, N, -N);   // light back-facing leaf cards correctly

                float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                Light mainLight = GetMainLight(shadowCoord);

                half ndl = saturate(dot(N, mainLight.direction));
                half3 color = albedo * mainLight.color * (ndl * mainLight.shadowAttenuation);
                color += albedo * SampleSH(N);

            #ifdef _ADDITIONAL_LIGHTS
                uint count = GetAdditionalLightsCount();
                for (uint i = 0u; i < count; ++i)
                {
                    Light l = GetAdditionalLight(i, IN.positionWS);
                    half a = saturate(dot(N, l.direction)) * l.distanceAttenuation * l.shadowAttenuation;
                    color += albedo * l.color * a;
                }
            #endif

                color = MixFog(color, IN.fogCoord);
                return half4(color, 1.0);
            }
            ENDHLSL
        }

        // ---------------------------------------------------------------- Shadow caster
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On ZTest LEqual ColorMask 0
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex   shadowVert
            #pragma fragment shadowFrag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            float3 _LightDirection;

            struct ShadowVaryings { float4 positionCS : SV_POSITION; };

            ShadowVaryings shadowVert(Attributes IN)
            {
                ShadowVaryings OUT;
                float3 posOS = ApplyWind(IN.positionOS.xyz, IN.normalOS, IN.color);
                float3 posWS = TransformObjectToWorld(posOS);
                float3 nWS   = TransformObjectToWorldNormal(IN.normalOS);

                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(posWS, nWS, _LightDirection));
            #if UNITY_REVERSED_Z
                positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
            #else
                positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
            #endif
                OUT.positionCS = positionCS;
                return OUT;
            }

            half4 shadowFrag(ShadowVaryings IN) : SV_Target { return 0; }
            ENDHLSL
        }

        // ---------------------------------------------------------------- Depth only
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            ZWrite On ColorMask R
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex   depthVert
            #pragma fragment depthFrag

            struct DepthVaryings { float4 positionCS : SV_POSITION; };

            DepthVaryings depthVert(Attributes IN)
            {
                DepthVaryings OUT;
                float3 posOS = ApplyWind(IN.positionOS.xyz, IN.normalOS, IN.color);
                OUT.positionCS = TransformObjectToHClip(posOS);
                return OUT;
            }

            half4 depthFrag(DepthVaryings IN) : SV_Target { return 0; }
            ENDHLSL
        }
    }
    FallBack Off
}
