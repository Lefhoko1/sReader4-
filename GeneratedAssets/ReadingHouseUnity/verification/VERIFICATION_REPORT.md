# Reading House — Unity integration verification
Generated 2026-10-18 19:43. Editor checks only; device numbers come from the APK overlay.

## Stone generation (test passage via TrailBuilder.BuildFromPlainText)

| Sentence | Words | Rows | Scale | Labels match | Labels fit | Overlaps | Gameplay blocked | Tray-focus blocked |
|---|---|---|---|---|---|---|---|---|
| 1 | 5 | 1 | 1,00 | yes | yes | 0 | 0 | 0 |
| 2 | 11 | 2 | 1,00 | yes | yes | 0 | 0 | 0 |
| 3 | 20 | 2 | 0,82 | yes | yes | 0 | 0 | 0 |

## Dashboard controls in the gameplay view

- Menu: on screen yes, a tap at its centre reaches it yes
- Assignment: on screen yes, a tap at its centre reaches it yes
- Points: on screen yes, a tap at its centre reaches it yes
- PlayWithOthers: on screen yes, a tap at its centre reaches it yes

## Sample book (placeholder mode)

Title "Meadow at Dawn", 6 sentences, 4 key words (gold stones).

## Scene budget (editor counts)

- Environment: 36 781 triangles
- Dashboard: 33 817 triangles
- Characters: 31 489 triangles
- Dashboard Binder: 490 triangles
- Word Stone Tray: 6 196 triangles
- **Total (incl. the current sentence's stones): 108 773 triangles**
- Mesh renderers 339, sub-meshes 373 (= draw calls before batching), unique materials 54
- Textures 2 (11,5 MB in editor), meshes 295 (5,8 MB in editor)
- Environment and dashboard are batching-static; expected draw calls after static batching are close to the material count.

Frame rate is not measured in the editor. Run the development APK on the Huawei Y5; the overlay shows FPS, draw calls, SetPass calls, triangles and memory from the device.
