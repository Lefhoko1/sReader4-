# Reading House visual polish — 23 October 2026

Branch: `polish/reading-house-lighting`. Scene edited in place: `Assets/Scenes/SC_ReadingHouse_Integration.unity`. No push.

[Open the five-view before/material/after comparison](review.html). All live images are **1520×720**. The saved sign-in session restored, and the dashboard/activity/chalkboard captures use real assignment/account content. No sample content was enabled, no answers were entered, and no assignment was submitted. Existing session save behaviour runs when a real reading session closes.

## Lighting and atmosphere

- Sun: realtime, intensity 1.05, warm #FFE2BE, no shadows → baked, intensity 1.1, #FFF0D4, rotation (42, −150, 0), baked soft shadows, strength 0.65. Only the light direction changed; house/camera/character poses are unchanged.
- Two wall lamps: realtime point, intensity 1.4, range 6, #FFB070 → baked point, intensity 2, range 4.5, #FFD69C, source radius 0.35 m.
- Four baked rectangular window lights: 0.9×1.8 m, intensity 4.5, #FFF1DD, range 7. Two broad baked sky-bounce sources: front 6×2.5 m / intensity 2.4 / #E6F1FF, ceiling 7×5 m / intensity 1.4 / #EDF2FF. Window glazing does not cast a solid shadow.
- Runtime lights: **3 → 0**. Runtime shadow-casting lights: **0 → 0**. Static light/shadow detail lives in the lightmap.
- Trilight ambient sky/equator/ground: #FFEAD2 / #F0C8A0 / #6E4C37 → #CDDDE7 / #B5BAB2 / #81725D; intensity 1. Reflection intensity 1 → 0.35.
- Camera clear colour #BFE6F7 → RGB (0.24, 0.55, 0.93), keeping a blue sky after Neutral tonemapping.
- 78 legacy light probes: 60 over the room/rug/chairs, 18 along the exterior approach. Three room heights: 0.52, 1.35, 2.4 m. Exterior heights: 0.52, 1.7 m.
- One baked 64-pixel ASTC 6×6 room cubemap (34 KB source asset), assigned as a static custom reflection, size 8.5×4.5×7.5 m, intensity 0.45, no realtime refresh or box projection. Characters and text are hidden during its bake. A dedicated camera explicitly uses the house renderer; the generated cubemap passes a pixel-variance check.
- Two soft warm contact-shadow quads under the seated characters. Reuse the existing student contact texture; no character frames, sprite sets, materials or animators changed.
- No particles, volumetric shafts, fog, SSAO, SSR, depth of field, or motion blur were added.

## Bake and renderer assets

- All new lighting, grade, renderer, bake outputs and material copies are in `Assets/Art/ReadingHouse/Lighting/`.
- Progressive **GPU** bake on NVIDIA RTX 3500 Ada Generation Laptop GPU; 10 texels/m, maximum atlas 1024, padding 3, non-directional lightmaps, 32 direct / 128 indirect / 64 environment samples, 3 bounces, probe sample multiplier 2, high quality compression. Baked AO: distance 0.35 m, indirect strength 0.65, direct strength 0.
- Bake result: **one 512×512 colour lightmap**, 78 baked probes. Android overrides: **ASTC 6×6**, 1024 maximum, quality 70. No shadowmask texture is needed because all lights are baked.
- 121 large surfaces receive lightmaps; 114 smaller surfaces receive probes. Small surfaces still contribute bounce/occlusion. Moving doors, the cut-away front wall, RT displays and sprites are excluded from static lightmapping.
- Enabled secondary UV generation only on the Environment and Dashboard FBX importers; minimum lightmap resolution 40 → 10. Original FBX binaries and all mesh transforms are untouched. The two original importer files are preserved beside this report.
- `Renderer_ReadingHouse_Mobile.asset` copies Renderer3D with post-process resources assigned, intermediate texture mode Auto, and no renderer features. The house camera uses new renderer index 2; original renderer index 1 is preserved for other scenes. Depth/opaque copies are off on this camera.
- `UniversalRP.asset`: new renderer slot 2 points to the house renderer, and the probe system changes from APV to legacy groups. The pre-existing shader-prefilter edit is preserved and excluded from the polish commit; HDR remains enabled as it was before this task. The other renderer and quality settings are unchanged. `URP_HuaweiY5.asset` is unchanged; it is not the active pipeline for this scene.

## Global Volume

| Setting | Value |
| --- | --- |
| Tonemapping | Neutral |
| White balance | Temperature +4, tint +1 |
| Colour adjustments | Exposure +0.10 EV, contrast +5, saturation +5 |
| Bloom | Threshold 1.5, intensity 0.12, scatter 0.42, clamp 4; quarter resolution, 2 iterations, high quality filtering off |
| Vignette | Intensity 0.10, smoothness 0.40, colour #483B56 |
| Camera | Post enabled, HDR enabled; same projection/poses/follow settings |

## Materials

All changes are scene material overrides on copies; original materials are unchanged. The physical chalkboard slate remains distinct from its runtime unlit page surface. RT screen blending and premultiplication are unchanged.

| Source | Base colour before → after | Smoothness before → after | New metallic | New emission RGB |
| --- | --- | --- | --- | --- |
| SR_Oak | #825D3DFF → #AC8457 | 0.45 → 0.24 | 0 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_Walnut | #513728FF → #886346 | 0.45 → 0.23 | 0 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_REF_golden | #FFB51BFF → #DDB669 | 0.74 → 0.36 | 0.16 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_MAT_Housing_Cream | #FCF8EEFF → #E9DABC | 0.75 → 0.32 | 0 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_MAT_Trim_Gold | #FFBA1EFF → #E1B765 | 0.85 → 0.44 | 0.28 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_MAT_Chalkboard | embedded 2D shader → #294A48 | - → 0.1 | 0 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_Gold | #D6AE58FF → #CFA45D | 0.68 → 0.4 | 0.3 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_MAT_Plastic_Orange | #F07610FF → #CD914B | 0.78 → 0.36 | 0.2 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_MAT_Plastic_Purple | #683AD6FF → #70529F | 0.8 → 0.34 | 0.02 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_MAT_Plastic_Navy | #102C6CFF → #283F68 | 0.55 → 0.29 | 0.02 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_Slate | #232733FF → #48596F | 0.48 → 0.18 | 0 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_Carved_Limestone | #C0B49CFF → #D8C7A5 | 0.45 → 0.16 | 0 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_Amber_Glass | #F6E3B8FF → #FFE6B4 | 0.85 → 0.34 | 0.02 | RGBA(0.800, 0.709, 0.562, 1.000) |
| SR_Limestone | #888C8DFF → #C2B59D | 0.45 → 0.14 | 0 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_Moss | #3B5044FF → #587649 | 0.45 → 0.1 | 0 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_MAT_Ceiling_Warm | #FAE2BEFF → #F1DEBB | 0.2 → 0.12 | 0 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_Sage_Leaves | #536D54FF → #78935B | 0.45 → 0.1 | 0 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_Garden_Soil | #302E27FF → #736444 | 0.45 → 0.1 | 0 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_Slate_Light | embedded 2D shader → #7A91A2 | - → 0.17 | 0 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_REF_rugblue | #07559BFF → #2069A4 | 0.1 → 0.08 | 0 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_MAT_Wall_Interior_Warm | #F7CE9CFF → #EAD0AA | 0.25 → 0.16 | 0 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_REF_light | #FFF0ACFF → #FFE6AC | 0.7 → 0.28 | 0 | RGBA(2.000, 1.765, 1.271, 1.000) |
| SR_Floor | #604532FF → #AA805D | 0.45 → 0.2 | 0 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_MAT_Light_Warm | #FFE696FF → #FFE3A0 | 0.7 → 0.3 | 0 | RGBA(2.200, 1.933, 1.380, 1.000) |
| SR_MAT_Plastic_DeepBlue | #185CC8FF → #416F94 | 0.8 → 0.3 | 0.02 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_MAT_Plastic_PurpleHi | #8A5CF0FF → #9B7FC2 | 0.88 → 0.38 | 0.02 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_MAT_Plastic_Cyan | #46C8F0FF → #66BEB9 | 0.82 → 0.3 | 0.03 | RGBA(0.044, 0.096, 0.093, 1.000) |
| SR_MAT_Moss | #4F9A3AFF → #75A154 | 0.25 → 0.1 | 0 | RGBA(0.000, 0.000, 0.000, 1.000) |
| SR_REF_brown | #89451EFF → #98704F | 0.52 → 0.23 | 0 | RGBA(0.000, 0.000, 0.000, 1.000) |

Exact old/new emissions and new asset paths: [materials.tsv](materials.tsv). Added contact material: transparent URP Unlit, warm brown RGB (0.32, 0.23, 0.18), opacity 0.64, depth writes/shadow casting off; two quads, four triangles total.

The key stone uses Lighting/P_KeyStone_Gold.prefab, a material-only copy of the original Key prefab: base #F3CA6C, smoothness 0.24, metallic 0.08, non-emissive surface; the original gold aura remains. Original surface colours were M_WordStone RGB (0.936, 0.916, 0.865) and M_KeyGold RGB (0.978, 0.865, 0.584). Geometry, WordStone fields, label colours, effects and the original prefab are preserved. Only the house board’s key-prefab material reference changes; all MatStoneBoard layout values are audited unchanged.

## Editor performance snapshots

Same live 1520×720 Game views; `UnityStats.drawCalls`, SetPass, triangles and frame delta recorded at each capture. These are individual editor samples on the RTX laptop, include world UI/particles, and vary with animation/UI activity. They are **not a Huawei benchmark or a 30/60 fps guarantee**. The existing in-game overlay incorrectly reports zero draw calls; the figures below use the editor counters.

| View | Draw calls before → after | SetPass before → after | Triangles before → after | Editor frame ms before → after |
| --- | --- | --- | --- | --- |
| Opening exterior | 367 → 383 | 35 → 59 | 100740 → 100778 | 5,46 → 6,21 |
| Walking inside + candy bubble | 188 → 202 | 22 → 48 | 63156 → 63187 | 5,57 → 6,31 |
| Reading / Word Path | 279 → 295 | 49 → 74 | 81338 → 81346 | 4,92 → 5,18 |
| Activity / TrayFocus | 140 → 153 | 18 → 38 | 49221 → 49236 | 5,73 → 5,27 |
| Chalkboard home | 42 → 53 | 13 → 21 | 8053 → 8972 | 4,65 → 4,54 |

Android: no ADB device was connected, so Huawei CPU/GPU frame time, thermal behaviour and the 60/30 fps target remain unverified.

Result Succeeded, 13 errors, 348 warnings, size 2395,4 MB, time 02:30, output C:/Users/11113930/sReader4/GeneratedAssets/ReadingHouseUnity/build/sReader_ReadingHouse_Test.apk

Actual APK file: 271.2 MiB (284335491 bytes). The totalSize reported above is Unity's build total, rather than the APK file length.

Unity returned Succeeded with 13 logged errors in existing PavilionPort SampleSceneAssets volume profiles (malformed YAML and broken PPtr references), plus the warning count above. Those files are unchanged. [Exact build diagnostics](android-messages.txt).

[Android test APK](../../ReadingHouseUnity/build/sReader_ReadingHouse_Test.apk).

## Validation and remaining suggestions

- Constraint audit: **PASS**; 3068 protected file hashes, 71 existing transforms, 45 prefab pose values, 129 serialized behaviours checked. The only permitted source-art changes are secondary-UV settings in the two static model importers.
- All 35 primary and intermediate live screenshots pass the 1520×720 dimension check.
- Live comparison includes the sentence directly on the illustration, navy/gold activity, stone words, chalkboard page and candy bubble. UI palettes, UXML/USS, controllers, layout values, input and data services are unchanged.
- The white banners visible in editor-only geometry previews are legacy serialized placeholders. Runtime correctly hides them and draws the sentence on the illustration. No white sentence box was reintroduced.
- Optional later UI review: slightly larger/darker sentence outline for busy illustrations and toned cream scrollbars inside activity pages. These are suggestions only because UI/WordStone behaviour and palettes were outside this task.
- The brief describes the Encourager on the right and Zombie on the left. The saved baseline shows the opposite when facing the dashboard; the approved seating transforms were preserved.
- The Huawei alternate pipeline has HDR off. If that alternate is adopted for the house later, assign the new renderer and verify HDR/bloom support and performance on the phone before switching.
- Re-run the same five views on the Huawei Y5 and record GPU/CPU frame timings after thermal warm-up. Lower bloom or render scale only if the measured device budget requires it.

All screenshots and logs stay local. No push or publication.
