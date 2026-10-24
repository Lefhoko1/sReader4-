Reference polish v02

Production: SR_Console, SR_Characters, SR_Decor. SR_StoneSource is hidden to avoid displaying the reusable template. All sample text, screen imagery, speech bubbles and word-stone examples are in SR_Demo; exclude it from exports. SR_DISPLAY_MAIN is clean and untextured.

Stone width: fixed hemisphere caps (radius X .18m) move to +/- centreWidth/2; SR_STONE_STRETCH_CENTRE scales only in X. Total width = centreWidth + .36m. Text fits one word and attaches at SR_STONE_TEXT_ANCHOR; Unity computes widths and wraps/paginates across the 6.1 x 1.35m region. Six material variants are supplied. Normal meshes use opaque single-colour materials.

Assignment and score displays are preview examples only. Unity should use existing assignment/scoring systems and TMP/world-space Canvas at named anchors. Portrait framing is a geometry review, not physical Huawei testing. No Unity code or existing exported models were changed. Character roots allow independent loading and later rigging; no skeleton/animations yet.

Exterior and roof are preserved but hidden for cutaway interior presentation. Original source and previous greybox are backed up. Re-enable original architecture render flags and Roof for exterior views.

Final source: sReader_ReadingHouse_Reference_FINAL.blend. Main review: 08_perspective_final.png. SR_PRODUCTION_EXPORT organises console, characters and new decor; original Architecture/Roof/Interior remain separate. Demonstration collections are excluded. Nine-word-plus example wraps to two rows using the same fixed-cap stone geometry. Exterior render temporarily restores the preserved facade and roof. Latest added asset triangles: console 11,444; characters about 90,120; new decor about 14,670; reusable stone source 876. These are measured scene counts, not Android performance results. Retopology/mesh combining and device profiling remain before a mobile production release.
