# Reading House — word labels, keyword cards and activity layout

Follow-up to the owner's screenshots on 23 October 2026. This request explicitly extends the earlier visual-only brief to restore the keyword card interaction and improve the dashboard activity layout.

[Open the before/after review](review.html). Final live captures: [../word-fix-final-play](../word-fix-final-play).

## Changes

- Stone words use their actual TMP measurements instead of a character-count estimate. Their labels are centred over each visible stone, kept upright toward the house camera and fitted between neighbours. A short lift along the viewing ray clears the stone surface while preserving the projected position and size. Navy ink on cream and dark brown on gold improve contrast. Layout values and stone positions are preserved.
- Reused the existing `KeywordCard` from the earlier Word Path scenes. Tapping a gold keyword now pops out a gold card with its word and a separate action. A Define token says **Define**; Fill and Illustrate tokens retain their authored actions. The real Titration definition card is captured in `07_define_card.png`.
- Choosing the action forwards to the existing gate checks, challenge and scoring hooks. A solved word stays solved, and upcoming words still obey the session order. Back, outside taps, sentence/page refresh, leaving the reading screen and teardown dismiss the card. One card can be open at a time; generated textures/materials are released on destruction.
- Activity panels have a compact title/instruction row and a fixed action footer. Only the challenge body scrolls when necessary. Letter preview no longer duplicates the selected-letter row, empty slots use underscores, and the scroll bar uses the dashboard palette.
- Shared `GatePanelHost` changes are presentation names and the empty-slot glyph. Its checking, hint, guided-solve, scoring and submission semantics were retained. Its pre-existing local-image hook changes remain unstaged.

## Verification

Unity compiled the scripts without C# errors. Eight live checks passed using the real saved session with `allowSampleContent=False`:

1. Every visible word label's anchor projects onto its stone centre (within one pixel).
2. A keyword tap shows the card before an activity opens.
3. Tapping the action opens the existing real activity and dismisses the card.
4. The action footer remains entirely inside the display.
5. A letter can be selected through the dashboard's screen-to-panel mapping.
6. The selected letter can be returned to the pool.
7. A real Define keyword produces the Define card; Back dismisses it.
8. Outside taps and sentence refresh dismiss the card; the score remains unchanged.

Eight 1520 × 720 screenshots were captured. No answer was completed or assignment submitted. See [verification.json](verification.json) and [live log](../word-fix-final-play/log.txt). Another 3,059 protected assets/scripts were checked against the original manifest and remain unchanged outside the explicitly allowed files/importers. The scene, camera poses, room layout, lighting, character art and original stone prefabs have no new changes in this follow-up.

The student still partially occludes the leftmost stone in the approved reading pose; moving the student or rug layout was outside this request's label/card changes.

## Performance and build

Single editor snapshots on the RTX laptop: reading 295 draws / 71 SetPass / 81,330 triangles / 5.20 ms; open keyword card 304 draws / 80 SetPass / 81,448 triangles / 5.65 ms; activity 153 draws / 34 SetPass / 49,116 triangles / 6.33 ms. These are individual frames, not a phone benchmark.

Android development build **Succeeded** in 15:04. Updated APK: `GeneratedAssets/ReadingHouseUnity/build/sReader_ReadingHouse_Test.apk`, 314,129,550 bytes (299.6 MiB). ZIP integrity check passed. [Build summary](android-build.txt), [build messages](android-messages.txt).

Unity reports the same 13 pre-existing errors in unrelated PavilionPort volume assets (YAML parse failures / missing PPtr IDs), plus 24 warnings. No new C# compilation errors. Those assets were not modified. Unity's reported build total of 2424.1 MB is not the APK's file size.

No ADB device is connected. Huawei Y5 frame rate, memory, thermal behaviour and on-device touch still require phone verification.

## Review of the code

[changes.patch](changes.patch) isolates this follow-up against the files as they were at its start. The four Reading House runtime files previously existed as untracked project files; this commit adds those complete files so the fix has tracked source and metadata. Other local work remains outside the commit. Nothing was pushed.
