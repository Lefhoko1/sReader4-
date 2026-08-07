# PavilionPort — HDRP sample scene ported to URP

`SC_Pavilion.unity` is Unity's HDRP "SampleScene" pavilion, converted to run in this
project's URP pipeline. Nothing outside this folder was modified by the port.

The conversion was done by rewriting asset files directly on disk. **No editor
scripts were added to this project** — there is nothing here that can run, misfire,
or need maintaining later.

---

## Before it will look right — 3 manual steps

The port is complete, but three things can only be done in the Unity editor:

### 1. Add the Decal Renderer Feature (required for 80 decals)

The scene has 80 `DecalProjector` components, remapped to URP's own decal system.
URP only draws decals if the renderer has the feature enabled:

> `Assets/Settings/Renderer3D.asset` → Inspector → **Add Renderer Feature** → **Decal**

This was left as a manual step on purpose: hand-editing `Renderer3D.asset` risks
corrupting the renderer your actual game uses, and it's a single click here.

Without it the geometry still renders — you just lose the grime, puddles and wear.

### 2. Rebake the lighting

`LightingData.asset` and the lightmaps came across, but they were baked by HDRP
against HDRP's physical light units. Treat whatever you see on first open as
provisional and rebake:

> `Window → Rendering → Lighting → Generate Lighting`

Also rebake reflection probes — 19 native probes survived, but their HDRP capture
data did not.

### 3. Pick a camera

The scene had 12 native cameras driven by **Cinemachine 2.10**. This project runs
**Cinemachine 3.1.7**, which is a rewrite with incompatible types, so the 84
Cinemachine components were stripped rather than left as missing scripts. The plain
`Camera` objects are all still there — either drive one directly, wire up
Cinemachine 3 cameras, or point your existing `PathWalker` / `WalkCamera` at it.

---

## What transferred unchanged

GUIDs were preserved (`.meta` files copied alongside every asset), so all internal
references resolve exactly as they did in the source project.

| | |
|---|---|
| Scene hierarchy | 228 GameObjects, 320 transforms |
| Prefab instances | 100, from 55 prefabs |
| Meshes | 49 FBX |
| Textures | 122 |
| Reflection probes | 19 (native) |
| Volumes | 8 |

## What was converted

- **45 materials → URP.** 35 `HDRP/Lit` → `URP/Lit`, 5 `HDRP/Decal` → URP Decal
  shadergraph, 1 `LitTessellation` → `URP/Lit` (tessellation is not a mobile
  feature), 1 `HDRP/Unlit` → `URP/Unlit`, plus 3 imported HDRP package materials.
  HDRP's `_MaskMap` packing (R=metallic, G=AO, A=smoothness) maps onto URP with no
  rechannelling: the same texture feeds `_MetallicGlossMap` and `_OcclusionMap`.
- **81 decal projectors** remapped from HDRP's `DecalProjector` to URP's.
- **8 volume profiles** rebuilt as URP profiles, keeping their GUIDs so the scene's
  Volume components stay wired. `VolumeGlobal` carries Tonemapping (ACES), Bloom,
  ColorAdjustments and Vignette — **authored fresh for URP, not converted**. HDRP's
  values are in EV100/physical units and are meaningless in URP, so copying the
  numbers across would have looked wrong. The 7 room volumes are empty; tune them
  if you want per-room grading.
- **122 textures** given an Android import override: max size 1024, compression on.

## What was removed, and why

| Removed | Reason |
|---|---|
| 69 HDRP-only components | `HDAdditionalLightData` ×28, `HDAdditionalReflectionData` ×19, `HDAdditionalCameraData` ×14, `ReflectionProxyVolumeComponent` ×4, plus water/fog/sky singletons. No URP counterpart; the native `Light`, `Camera` and `ReflectionProbe` components underneath them all survived. |
| 84 Cinemachine 2.x components | Cinemachine 3.1.7 is installed here; the 2.x types don't exist. |
| 3 VFX Graph effects | Butterflies, falling leaves, floating dust. `com.unity.visualeffectgraph` is not installed in this project. |
| HDRP water system | No URP equivalent exists. |
| 337 tutorial files | `com.unity.learn.iet-framework` is not installed. |
| 5 HDRP pipeline assets | HDRP quality tiers, inert here. |
| 2 Timeline sequences | Demo camera flythroughs that drove the removed Cinemachine 2.x rig. |
| Diffusion profiles | HDRP subsurface scattering. |

## Known cosmetic gaps

Four references were already dangling **in the original HDRP project** and were
carried over or cleared, not broken by the port:

- `_ParallaxMap` on 5 materials pointed at height textures that don't exist in the
  source either — cleared (parallax is expensive on mobile regardless).
- One prefab-modification target and one animation motion in
  `StarterAssetsThirdPerson.controller` remain unresolvable. Harmless.

## Honest expectations

This will not look identical to the HDRP original. Gone with HDRP: volumetric fog,
screen-space reflections, the physically-based sky at that fidelity, and the water.
What you get is a well-built URP scene that should run on a phone — and, more
usefully, a working reference for what baked lighting plus a real prefab kit buys
you, which is the technique worth carrying into the library scenes.
