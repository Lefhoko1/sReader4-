# Great Library Unity preview

Open `Assets/Scenes/SC_LibraryRedesignPreview.unity`. The scene was created and visually verified in Unity 6000.4.9f1 with the existing URP 3D renderer (camera override index 1). It was left running in Play mode on the interior overview.

In Play mode, use the Interior, Exterior and Book buttons, or keys 1 / 2 / 3. Exterior enables the complete Roof group; the other views hide it. Editor view commands are under Tools > sReader > Library Preview.

The geometry-only reusable prefab is `Assets/Art/LibraryRedesign/Prefabs/P_GreatLibrary.prefab`. Mesh and material assets are stored under this folder. The scene adds lighting, camera controls, atmosphere, a floor collider and shell colliders. Page line art and stone glyphs remain decorative; this scene does not run reading activities or connect to a backend.

The import preserves 389 evaluated Blender objects, 73,904 triangles and 24 materials. Source geometry, camera poses and palette are in `GeneratedAssets/LibraryRedesign/20261017_184255/unity/library_meshes.json`. It uses native Unity Mesh assets; no new import package is required. The Blender original and redesigned .blend remain in the source folder.

Verification: Unity compiled the new scripts; native scene saved; all three camera captures contain visible geometry; camera projection and Roof activation checks passed in Edit mode and Play mode. `unity_status.json` records runtimeVerified=true and viewSwitchesVerified=true. Screenshots are in the source unity/screenshots folder, including `04_unity_live.png` with the on-screen controls.

The original SC_Pavilion scene was clean when switched, so no unsaved-scene backup was necessary. No existing scene, gameplay, scoring, Supabase, multiplayer, global render pipeline or build settings were changed. Preview additions are under the new LibraryRedesign assets, the dedicated preview component and its editor builder.

Remaining production work: bake lighting/lightmaps, profile static batching and draw calls on Android, and connect the existing reading presentation to the new props when requested. Unity realtime lighting approximates the Blender Cycles preview; this was not an Android build or device performance test.
