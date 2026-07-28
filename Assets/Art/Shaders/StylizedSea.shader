// ===========================================================================
//  GreatLibrary/StylizedSea — stylized-realistic sea for the island vista
// ===========================================================================
//  Real water BEHAVIOUR, hand-painted PALETTE. What makes it read as water:
//    • two scrolling normal maps (broad swell + fine chop) drive every
//      lighting term, so the whole surface is in motion
//    • depth-based colour and transparency from the scene depth texture:
//      you can see the sand through the shallows, deep water goes opaque
//    • animated shoreline foam where the seabed comes close to the surface
//    • a broad sun specular PLUS a thresholded high-frequency "glitter" term
//      — that second one is the sparkle
//    • fresnel toward the horizon colour, so the sea lifts into the sky
//    • optional gerstner-ish vertex swell (needs a subdivided plane; the
//      setup script can swap the FBX's flat sea quad for a dense grid)
//
//  UVs come from WORLD XZ, not the mesh's own UVs, so tiling is uniform and
//  correct whatever the island FBX was unwrapped like.
//
//  Requires: URP with Depth Texture ON (UniversalRP.asset already has it) and
//  a camera on the 3D renderer (SC_IslandVista's camera is on index 1).
//
//  BUILD NOTE: add "GreatLibrary/StylizedSea" to Project Settings > Graphics >
//  Always Included Shaders. The setup script does this for you.
// ===========================================================================
Shader "GreatLibrary/StylizedSea"
{
    Properties
    {
        [Header(Colour)]
        _ShallowColor ("Shallow", Color)   = (0.42, 0.79, 0.78, 1)
        _DeepColor    ("Deep", Color)      = (0.05, 0.24, 0.38, 1)
        _HorizonColor ("Horizon / Fresnel", Color) = (0.66, 0.84, 0.93, 1)
        _DepthFade    ("Shallow to Deep (m)", Float) = 4.0
        _AlphaShallow ("Shore Alpha", Range(0,1)) = 0.30
        _AlphaDeep    ("Deep Alpha", Range(0,1))  = 0.97

        [Header(Surface motion)]
        [NoScaleOffset] _NormalMap ("Swell Normals (RGB) Height (A)", 2D) = "bump" {}
        [NoScaleOffset] _DetailMap ("Chop Normals (RGB) Height (A)", 2D)  = "bump" {}
        _SwellTiling   ("Swell Tiling", Float) = 0.035
        _SwellStrength ("Swell Strength", Range(0,3)) = 0.85
        _SwellSpeed    ("Swell Speed", Float) = 0.020
        _ChopTiling    ("Chop Tiling", Float) = 0.150
        _ChopStrength  ("Chop Strength", Range(0,3)) = 0.55
        _ChopSpeed     ("Chop Speed", Float) = 0.055
        _DetailFade    ("Chop Fade Distance", Float) = 90.0

        [Header(Sun and sparkle)]
        _SunColor      ("Sun Colour", Color) = (1, 0.97, 0.88, 1)
        _SunSharpness  ("Sun Tightness", Range(8, 2048)) = 420
        _SunStrength   ("Sun Strength", Range(0, 8)) = 2.2
        // Tiling stays modest on purpose: over-tile this and mipmaps average
        // the normals flat at distance, which kills the sparkle entirely.
        _GlitterTiling ("Glitter Tiling", Float) = 0.30
        _GlitterSharp  ("Glitter Tightness", Range(8, 4096)) = 500
        _GlitterCover  ("Glitter Rarity", Range(0, 0.999)) = 0.35
        _GlitterStrength ("Glitter Strength", Range(0, 12)) = 5.0
        _GlitterRange  ("Glitter Distance", Float) = 70.0
        _FresnelPower  ("Fresnel Power", Range(0.5, 12)) = 4.5
        _FresnelStrength ("Fresnel Strength", Range(0,1)) = 0.55

        [Header(Shoreline foam)]
        _FoamColor    ("Foam", Color) = (1, 1, 1, 0.9)
        _FoamDepth    ("Foam Width (m)", Float) = 0.9
        _FoamSoftness ("Foam Softness", Range(0.01, 1)) = 0.42
        _FoamTiling   ("Foam Noise Tiling", Float) = 0.35
        _FoamSpeed    ("Foam Speed", Float) = 0.10

        [Header(Vertex swell   needs a subdivided plane)]
        _WaveHeight ("Wave Height", Float) = 0.0
        _WaveLength ("Wave Length", Float) = 9.0
        _WaveSpeedV ("Wave Speed", Float) = 0.55
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent-100"
               "RenderPipeline"="UniversalPipeline" "IgnoreProjector"="True" }

        Pass
        {
            Name "SeaForward"
            Tags { "LightMode"="UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On                       // nothing draws behind the sea
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
            TEXTURE2D(_DetailMap); SAMPLER(sampler_DetailMap);

            CBUFFER_START(UnityPerMaterial)
                half4  _ShallowColor, _DeepColor, _HorizonColor;
                float  _DepthFade;
                half   _AlphaShallow, _AlphaDeep;
                float  _SwellTiling, _SwellSpeed, _ChopTiling, _ChopSpeed, _DetailFade;
                half   _SwellStrength, _ChopStrength;
                half4  _SunColor;
                half   _SunSharpness, _SunStrength;
                float  _GlitterTiling, _GlitterRange;
                half   _GlitterSharp, _GlitterCover, _GlitterStrength;
                half   _FresnelPower, _FresnelStrength;
                half4  _FoamColor;
                float  _FoamDepth, _FoamTiling, _FoamSpeed;
                half   _FoamSoftness;
                float  _WaveHeight, _WaveLength, _WaveSpeedV;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float  viewDepth  : TEXCOORD2;   // eye-space depth of the surface
                float  fogFactor  : TEXCOORD3;
            };

            // Sum of two crossed sines — a cheap stand-in for a gerstner swell.
            // Silent no-op when _WaveHeight is 0 (the default: the FBX sea is a
            // flat quad, and displacing 4 corners would just tilt the plane).
            float SwellHeight(float3 wp)
            {
                if (_WaveHeight <= 0.0) return 0.0;
                float k = 6.2831853 / max(_WaveLength, 0.01);
                float t = _Time.y * _WaveSpeedV;
                float h  = sin(wp.x * k + t);
                       h += 0.7 * sin((wp.z * 0.83 + wp.x * 0.31) * k - t * 1.31);
                       h += 0.4 * sin((wp.x - wp.z) * k * 1.9 + t * 1.7);
                return h * _WaveHeight * 0.5;
            }

            Varyings vert (Attributes v)
            {
                Varyings o;
                float3 wp = TransformObjectToWorld(v.positionOS.xyz);
                wp.y += SwellHeight(wp);

                o.positionWS = wp;
                o.positionCS = TransformWorldToHClip(wp);
                o.normalWS   = TransformObjectToWorldNormal(v.normalOS);
                o.viewDepth  = -TransformWorldToView(wp).z;
                o.fogFactor  = ComputeFogFactor(o.positionCS.z);
                return o;
            }

            // Our wave maps are imported as plain colour (linear, no DXT5nm
            // swizzle), so decode by hand rather than via UnpackNormal.
            float4 SampleWave(TEXTURE2D_PARAM(tex, smp), float2 uv, float strength)
            {
                float4 t = SAMPLE_TEXTURE2D(tex, smp, uv);
                float3 n = t.xyz * 2.0 - 1.0;
                n.xy *= strength;
                return float4(normalize(n), t.a);      // .a carries the height
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float3 wp = IN.positionWS;
                float  t  = _Time.y;

                // ---- tangent frame on the water plane -----------------------
                float3 gN = normalize(IN.normalWS);
                float3 T  = normalize(cross(float3(0, 0, 1), gN) + float3(1e-4, 0, 0));
                float3 B  = cross(gN, T);

                float3 V    = normalize(GetWorldSpaceViewDir(wp));
                float  dist = distance(wp, GetCameraPositionWS());

                // ---- two scrolling wave layers ------------------------------
                // world XZ as UV: uniform tiling, independent of the FBX unwrap
                float2 uvS = wp.xz * _SwellTiling + t * _SwellSpeed * float2(1.0, 0.32);
                float2 uvC = wp.xz * _ChopTiling  + t * _ChopSpeed  * float2(-0.62, 0.81);

                float4 swell = SampleWave(TEXTURE2D_ARGS(_NormalMap, sampler_NormalMap),
                                          uvS, _SwellStrength);
                // fade the fine chop out with distance, or it aliases into moire
                half   chopFade = saturate(1.0 - dist / max(_DetailFade, 1.0));
                float4 chop  = SampleWave(TEXTURE2D_ARGS(_DetailMap, sampler_DetailMap),
                                          uvC, _ChopStrength * chopFade);

                float3 nTS = normalize(float3(swell.xy + chop.xy, 1.0));
                float3 N   = normalize(T * nTS.x + B * nTS.y + gN * nTS.z);

                // ---- how deep is the water under this pixel? ----------------
                float2 suv     = GetNormalizedScreenSpaceUV(IN.positionCS);
                float  rawD    = SampleSceneDepth(suv);
                float  sceneEye = LinearEyeDepth(rawD, _ZBufferParams);
                float  depth   = max(sceneEye - IN.viewDepth, 0.0);

                float  dFade = saturate(depth / max(_DepthFade, 0.001));
                half3  col   = lerp(_ShallowColor.rgb, _DeepColor.rgb, dFade);
                half   alpha = lerp(_AlphaShallow, _AlphaDeep,
                                    saturate(depth / max(_DepthFade * 0.6, 0.001)));

                // ---- light ---------------------------------------------------
                Light main = GetMainLight();
                float3 L = normalize(main.direction);
                float3 H = normalize(L + V);
                half   ndl = saturate(dot(N, L));

                col *= lerp(half3(0.78, 0.80, 0.84), half3(1.12, 1.10, 1.05), ndl);
                col *= lerp(half3(1, 1, 1), main.color, 0.55);

                // broad sun sheen
                half spec = pow(saturate(dot(N, H)), _SunSharpness) * _SunStrength;

                // ---- the sparkle --------------------------------------------
                // A third, much finer normal sample gets its own razor-sharp
                // specular, then a threshold keeps only the brightest hits —
                // so you get discrete points of light that wink in and out as
                // the wave field scrolls under them, not a uniform shimmer.
                float2 uvG = wp.xz * _GlitterTiling + t * float2(0.031, -0.024);
                float4 gsamp = SampleWave(TEXTURE2D_ARGS(_DetailMap, sampler_DetailMap),
                                          uvG, 1.0);
                float3 NG  = normalize(T * gsamp.x + B * gsamp.y + gN * gsamp.z);
                half   g   = pow(saturate(dot(NG, H)), _GlitterSharp);
                       g   = smoothstep(_GlitterCover, 1.0, g);
                       g  *= _GlitterStrength * saturate(1.0 - dist / max(_GlitterRange, 1.0));

                col += _SunColor.rgb * main.color * (spec + g) * ndl;

                // ---- fresnel toward the horizon ------------------------------
                half fres = pow(1.0 - saturate(dot(N, V)), _FresnelPower);
                col  = lerp(col, _HorizonColor.rgb, fres * _FresnelStrength);
                alpha = saturate(alpha + fres * 0.35);

                // ---- shoreline foam ------------------------------------------
                float2 uvF = wp.xz * _FoamTiling + t * _FoamSpeed * float2(0.28, 0.19);
                half   fnoise = SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap, uvF).a;
                half   edge = 1.0 - saturate(depth / max(_FoamDepth, 0.001));
                half   foam = smoothstep(1.0 - _FoamSoftness, 1.0,
                                         edge + (fnoise - 0.5) * 0.55);
                col  = lerp(col, _FoamColor.rgb, foam * _FoamColor.a);
                alpha = saturate(alpha + foam * _FoamColor.a);

                // ---- ambient lift + fog --------------------------------------
                col += SampleSH(N) * 0.10;
                col  = MixFog(col, IN.fogFactor);

                return half4(col, alpha);
            }
            ENDHLSL
        }
    }
    // Deliberately no fallback: a Lit fallback would hand this material a
    // DepthOnly/ShadowCaster pass, and magenta water is easier to diagnose
    // than water that silently stops reading the depth texture.
    FallBack Off
}
