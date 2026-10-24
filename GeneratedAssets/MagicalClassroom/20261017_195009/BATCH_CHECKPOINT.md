# Classroom batch checkpoint

Saved: 2026-10-18T06:55:28.367351+02:00

This batch reproduced the classroom flow in a fresh Play Mode session. All 21 interaction checks passed, including Fill, Define, Illustrate, Later, hint scoring, guided solve, full-passage reading and gold completion feedback. The earlier picture-test failure did not reproduce; gameplay code was not changed in this batch.

The Unity scene was saved, all three previews were refreshed, and Play Mode was stopped. The checkpoint directory contains a scene copy, the fresh test report, preview copies and a scene hash.

Scene: `Assets/Scenes/SC_LibraryRedesignPreview.unity`

Blender source: `GeneratedAssets/MagicalClassroom/20261017_195009/sReader_Classroom_v02.blend`

Next batch: produce the Android development APK and obtain a useful Frame Debugger capture. The previous capture returned zero events, so it is not treated as successful profiling evidence.

Still requires a connected phone: Huawei Y5 frame rate, memory and physical touch testing. Authenticated assignment resume/submission also needs end-to-end validation. No claim of device 30 FPS is made.

## APK batch — completed

The classroom is build index 0. Both Android build profiles use the global scene list. Other scenes remain after it. The completed development APK explicitly contains the classroom as its scene, confirmed in its globalgamemanagers data.

APK: `android/sReader_Classroom_Development.apk` (222.1 MiB). Build succeeded in 11.9 minutes. Android APK Signature Scheme v2 verification passed. ARMv7 and ARM64 libraries are present.

Unity's raw report records 13 error-level messages and 795 warnings despite a successful build. No C# compilation, shader compilation or exception failures were found in the reviewed build log; licensing diagnostics were present. Raw diagnostics remain in `reports/android_build.json`. The report's total bytes includes additional build outputs; the APK file itself is 232897081 bytes.

Verified artifact details: `reports/apk_verification.json`. No physical-phone install or performance claim is made.
