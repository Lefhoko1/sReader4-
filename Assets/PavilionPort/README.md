# PavilionPort — HDRP sample scene ported to URP

`SC_Pavilion.unity` is Unity's HDRP "SampleScene" pavilion, converted to run in this
project's URP pipeline. Nothing outside this folder was modified by the port.

The conversion was done by rewriting asset files directly on disk. **No editor
scripts were added to this project** — there is nothing here that can run, misfire,
or need maintaining later.

---

## Quick path: the editor menu

`Editor/PavilionPortSetup.cs` does the steps below from a menu:

> **Tools → Pavilion Port →**
> `0. Report Status` · `1. Set Up Scene` · `2. Add Decal Renderer Feature` ·
> `3a. Enable Adaptive Probe Volumes` · `3b. Mark Scene Static` · `4. Bake Lighting`

Run **0. Report Status** first — it only reads, and prints exactly what is still
missing. Then 1, 2, and one of 3a/3b, then 4.

Nothing in that script runs on its own: no `InitializeOnLoad`, no callbacks, no
asset postprocessors. It acts only when you click a menu item, every action is safe
to run twice, and deleting the file leaves no trace. The two steps that touch files
outside this folder (`2` and `3a`) ask for confirmation first and say what they will
affect.

The rest of this document explains what each step does and why, if you would rather
do it by hand or need to understand what the script changed.

---

## The steps, in this order

The port is complete, but four things can only be done in the Unity editor. **Do
them in this order** — the first two are why the scene will look broken on first
open, and no amount of relighting fixes them.

### 1. Give the scene an active camera

**All 12 cameras in this scene are inactive and untagged.** There is no
`MainCamera`. On first open you will see nothing in Game view — that is expected,
not a failed port.

The scene's cameras were driven by **Cinemachine 2.10**; this project runs
**Cinemachine 3.1.7**, an incompatible rewrite, so the 84 Cinemachine components
were stripped rather than left as missing scripts. The plain `Camera` objects all
survived.

> Pick one (e.g. `Screenshot Camera 1`) → tick its **enabled** checkbox in the
> Inspector header → set **Tag** to `MainCamera`.

Or point your existing `PathWalker` / `WalkCamera` rig at the scene instead.

### 2. Set that camera's renderer to Renderer3D  ← easiest step to miss

`Assets/Settings/UniversalRP.asset` has **`m_DefaultRendererIndex: 0`**, and index 0
is **`Renderer2D`**. `Renderer3D` is index 1.

So any camera that doesn't explicitly override its renderer draws through the **2D
renderer** — no 3D lighting, no shadows, no reflection probes, no decals. The scene
will look flat and wrong, and it will look that way no matter how many times you
rebake.

> Camera → Inspector → **Rendering** → **Renderer** → **Renderer3D**

Set this on every camera you actually use. **Do not** change
`m_DefaultRendererIndex` globally to "fix" this — the rest of this project's scenes
rely on Renderer2D being the default.

### 3. Add the Decal Renderer Feature (required for 80 decals)

The scene has 80 `DecalProjector` components remapped to URP's decal system. URP
only draws decals if the renderer has the feature:

> `Assets/Settings/Renderer3D.asset` → Inspector → **Add Renderer Feature** →
> **Decal**

Then set **Technique** to `Screen Space` for mobile — the default `Automatic` picks
DBuffer, which forces a depth prepass and costs bandwidth phones don't have. Drop
**Surface Data** to `Albedo Normal` (or just `Albedo`) for the same reason.

This was left manual on purpose: hand-authoring a renderer-feature sub-asset that
can't be verified without opening Unity risks corrupting the renderer your real
game uses. Without it the geometry still renders — you just lose grime, puddles and
wear.

### 4. Relight

Two things to know before you press Bake.

**This scene is Adaptive-Probe-Volume driven, not lightmap driven.** Of the objects
in the prefabs, 261 carry only the `ReflectionProbeStatic` flag and just a handful
have `ContributeGI`. Its indirect light came from APV, and the `ProbeVolume` /
`ProbeAdjustmentVolume` components survived the port.

**But APV is currently switched off in this project:** `UniversalRP.asset` has
`m_LightProbeSystem: 0` (Legacy light probes). Until that changes, the probe
volumes do nothing.

You have two routes:

- **Route A — enable APV** (faithful to the original, least marking work):
  `UniversalRP.asset` → **Lighting** → **Light Probe System** → *Adaptive Probe
  Volumes*, then `Window → Rendering → Lighting → Generate Lighting`.
  ⚠️ `UniversalRP.asset` is used by **all six quality levels**, so this is a
  project-wide change that will also affect how `SC_IslandVista` lights. Check that
  scene afterwards, or clone the URP asset for this scene only.

- **Route B — traditional lightmaps** (isolated, kinder to low-end phones): select
  the scene geometry, tick **Contribute GI** + **Static**, then Generate Lighting.
  More marking work, but nothing outside this folder changes.

Either way, also rebake the **19 reflection probes** — the native probes survived
but their HDRP capture data did not. Set them to *Baked* and they bake along with
Generate Lighting.

Finally, add `SC_Pavilion` to **File → Build Settings** if you want to run it on
device.

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
