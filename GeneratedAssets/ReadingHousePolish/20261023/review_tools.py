from pathlib import Path
import re, json, hashlib, csv, html, sys
ROOT=Path(__file__).resolve().parents[3]
OUT=ROOT/'GeneratedAssets/ReadingHousePolish/20261023'
def documents(path):
    text=path.read_text(encoding='utf-8-sig')
    matches=list(re.finditer(r'^--- !u!(\d+) &(-?\d+)(?: stripped)?\s*$',text,re.M))
    return {m[2]:(int(m[1]),text[m.end():matches[i+1].start() if i+1<len(matches) else len(text)].strip()) for i,m in enumerate(matches)}
def sha(path):return hashlib.sha256(path.read_bytes()).hexdigest().upper()
def verify():
    before=documents(OUT/'baseline.unity.txt');after=documents(ROOT/'Assets/Scenes/SC_ReadingHouse_Integration.unity')
    problems=[];transforms=0;controllers=0;overrides=0;visual_reference=0
    light_objects={re.search(r'm_GameObject: \{fileID: (\d+)\}',body)[1] for kind,body in before.values() if kind==108}
    for ident,(kind,body) in before.items():
        if ident not in after:problems.append('Removed existing document '+ident);continue
        new=after[ident][1]
        if kind==4 and 'stripped' not in body:
            owner=re.search(r'm_GameObject: \{fileID: (\d+)\}',body)
            if owner and owner[1] in light_objects:continue
            for prop in ['m_LocalRotation','m_LocalPosition','m_LocalScale','m_ConstrainProportionsScale','m_Father','m_LocalEulerAnglesHint']:
                old_match=re.search(r'^  '+prop+r': (.*)$',body,re.M);new_match=re.search(r'^  '+prop+r': (.*)$',new,re.M)
                if old_match and (not new_match or old_match[1]!=new_match[1]):problems.append('Transform changed '+ident+' '+prop)
            transforms+=1
        if kind==1001:
            def pose_overrides(b):
                result={}
                for part in re.split(r'^    - target: ',b,flags=re.M)[1:]:
                    prop=re.search(r'propertyPath: (.*)',part)
                    if prop and (prop[1].startswith(('m_LocalPosition','m_LocalRotation','m_LocalScale','m_LocalEulerAnglesHint')) or prop[1] in ['m_TransformParent','m_Name']):
                        result[(part.splitlines()[0],prop[1])]=re.search(r'      value: (.*)',part)[1]
                return result
            a,b=pose_overrides(body),pose_overrides(new)
            if a!=b:problems.append('Existing prefab pose/name override changed '+ident)
            overrides+=len(a)
        if kind==114:
            script=re.search(r'm_Script: \{fileID: 11500000, guid: ([a-f0-9]+)',body)
            if not script:continue
            if script[1]=='a79441f348de89743a2939f4d699eac1':continue
            # Only the URP house camera's visual settings may change in an existing behaviour.
            if 'm_RenderPostProcessing:' in body:continue
            if '  keyPrefab:' in body and '  edgeMargin:' in body:
                old_visual=re.sub(r'^  keyPrefab: .*$', '  keyPrefab: VISUAL_MATERIAL_VARIANT', body, flags=re.M)
                new_visual=re.sub(r'^  keyPrefab: .*$', '  keyPrefab: VISUAL_MATERIAL_VARIANT', new, flags=re.M)
                if old_visual!=new_visual:problems.append('MatStoneBoard layout/behaviour changed '+ident)
                if body!=new:visual_reference+=1
            elif body!=new:problems.append('Existing serialized behaviour changed '+ident+' script '+script[1])
            controllers+=1
    protected=json.loads((OUT/'protected-before.json').read_text(encoding='utf-8-sig'))
    allowed={'Assets/Art/ReadingHouse/Models/SM_ReadingHouse_Environment.fbx.meta','Assets/Art/ReadingHouse/Models/SM_ReadingHouse_Dashboard.fbx.meta'}
    allowed_changes=[]
    for item in protected:
        path=Path(item['Path']);rel=path.relative_to(ROOT).as_posix()
        if not path.exists() or sha(path)!=item['Hash']:
            if rel in allowed:
                saved=OUT/('Environment-before.meta.txt' if 'Environment' in rel else 'Dashboard-before.meta.txt')
                def clean_import(text):return re.sub(r'^    (generateSecondaryUV|secondaryUVMinLightmapResolution|secondaryUVMinObjectScale): .*$', '', text, flags=re.M)
                if clean_import(saved.read_text()) != clean_import(path.read_text()):problems.append('Unexpected importer change '+rel)
                allowed_changes.append(rel)
            else:problems.append('Protected file changed '+rel)
    import struct
    verified_shots=0
    required=['01_exterior','02_follow_inside','03_reading','04_activity','05_chalkboard']
    for stage in sorted(OUT.glob('*-play')):
        if not stage.is_dir():continue
        for name in required:
            picture=stage/(name+'.png')
            if not picture.exists():problems.append('Missing screenshot '+str(picture.relative_to(OUT)));continue
            with picture.open('rb') as stream:header=stream.read(24)
            size=struct.unpack('>II',header[16:24])
            if size!=(1520,720):problems.append('Wrong screenshot size '+str(picture.relative_to(OUT))+': '+str(size))
            verified_shots+=1
    result={'screenshots_checked':verified_shots,'passed':not problems,'existing_transforms_checked':transforms,'prefab_pose_values_checked':overrides,'existing_behaviours_checked':controllers,'visual_material_prefab_references':visual_reference,'protected_files_checked':len(protected),'allowed_uv_importer_changes':allowed_changes,'failures':problems}
    (OUT/'constraint-validation.json').write_text(json.dumps(result,indent=2)+'\n')
    print(json.dumps(result,indent=2))
    if problems:sys.exit(1)
def gallery():
    stages=['before-play','01-palette-play','after-play']
    names=['01_exterior','02_follow_inside','03_reading','04_activity','05_chalkboard']
    labels=['Opening exterior','Walking inside','Reading at dashboard','Activity / TrayFocus','Chalkboard student home']
    parts=['<!doctype html><html><meta charset="utf-8"><meta name="viewport" content="width=device-width"><title>Reading House visual review</title><style>body{font:16px system-ui;background:#f5eddf;color:#25364e;margin:24px}h1{font-size:26px}section{margin:28px 0}article{display:grid;grid-template-columns:repeat(auto-fit,minmax(360px,1fr));gap:12px}figure{margin:0}img{width:100%;height:auto;border-radius:8px}figcaption{margin:6px 0}a{color:#315f79}p{max-width:1000px}</style><h1>Reading House — colour and lighting review</h1><p>1520 × 720. Live saved sign-in session and real assignment content. No answers submitted. Compare the baseline, material pass, and baked lighting/grade pass. Capture logs contain editor counters; they are not Huawei GPU timings.</p>']
    for name,label in zip(names,labels):
        parts.append('<section><h2>'+label+'</h2><article>')
        for stage in stages:
            file=OUT/stage/(name+'.png')
            if file.exists():parts.append(f'<figure><a href="{stage}/{name}.png"><img loading="lazy" src="{stage}/{name}.png"></a><figcaption>{html.escape({"before-play":"Before","01-palette-play":"Material pass","after-play":"After"}[stage])}</figcaption></figure>')
        parts.append('</article></section>')
    parts.append('</html>');(OUT/'review.html').write_text(''.join(parts),encoding='utf-8')
    print('Wrote '+str(OUT/'review.html'))

def report():
    labels={'01_exterior':'Opening exterior','02_follow_inside':'Walking inside + candy bubble','03_reading':'Reading / Word Path','04_activity':'Activity / TrayFocus','05_chalkboard':'Chalkboard home'}
    def perf(stage):
        text=(OUT/stage/'log.txt').read_text(encoding='utf-8-sig')
        return {m[1]:m.groups()[1:] for m in re.finditer(r'(\d\d_\w+) signedIn=True sampleAllowed=False batches/drawCalls=(\d+) setPass=(\d+) triangles=(\d+) editorFrame=([\d,\.]+)ms',text)}
    before=perf('before-play');after=perf('after-play')
    validation=json.loads((OUT/'constraint-validation.json').read_text())
    lines=['# Reading House visual polish — 23 October 2026','',
        'Branch: `polish/reading-house-lighting`. Scene edited in place: `Assets/Scenes/SC_ReadingHouse_Integration.unity`. No push.', '',
        '[Open the five-view before/material/after comparison](review.html). All live images are **1520×720**. The saved sign-in session restored, and the dashboard/activity/chalkboard captures use real assignment/account content. No sample content was enabled, no answers were entered, and no assignment was submitted. Existing session save behaviour runs when a real reading session closes.', '',
        '## Lighting and atmosphere','',
        '- Sun: realtime, intensity 1.05, warm #FFE2BE, no shadows → baked, intensity 1.1, #FFF0D4, rotation (42, −150, 0), baked soft shadows, strength 0.65. Only the light direction changed; house/camera/character poses are unchanged.',
        '- Two wall lamps: realtime point, intensity 1.4, range 6, #FFB070 → baked point, intensity 2, range 4.5, #FFD69C, source radius 0.35 m.',
        '- Four baked rectangular window lights: 0.9×1.8 m, intensity 4.5, #FFF1DD, range 7. Two broad baked sky-bounce sources: front 6×2.5 m / intensity 2.4 / #E6F1FF, ceiling 7×5 m / intensity 1.4 / #EDF2FF. Window glazing does not cast a solid shadow.',
        '- Runtime lights: **3 → 0**. Runtime shadow-casting lights: **0 → 0**. Static light/shadow detail lives in the lightmap.',
        '- Trilight ambient sky/equator/ground: #FFEAD2 / #F0C8A0 / #6E4C37 → #CDDDE7 / #B5BAB2 / #81725D; intensity 1. Reflection intensity 1 → 0.35.',
        '- Camera clear colour #BFE6F7 → RGB (0.24, 0.55, 0.93), keeping a blue sky after Neutral tonemapping.',
        '- 78 legacy light probes: 60 over the room/rug/chairs, 18 along the exterior approach. Three room heights: 0.52, 1.35, 2.4 m. Exterior heights: 0.52, 1.7 m.',
        '- One baked 64-pixel ASTC 6×6 room cubemap (34 KB source asset), assigned as a static custom reflection, size 8.5×4.5×7.5 m, intensity 0.45, no realtime refresh or box projection. Characters and text are hidden during its bake. A dedicated camera explicitly uses the house renderer; the generated cubemap passes a pixel-variance check.',
        '- Two soft warm contact-shadow quads under the seated characters. Reuse the existing student contact texture; no character frames, sprite sets, materials or animators changed.',
        '- No particles, volumetric shafts, fog, SSAO, SSR, depth of field, or motion blur were added.', '',
        '## Bake and renderer assets','',
        '- All new lighting, grade, renderer, bake outputs and material copies are in `Assets/Art/ReadingHouse/Lighting/`.',
        '- Progressive **GPU** bake on NVIDIA RTX 3500 Ada Generation Laptop GPU; 10 texels/m, maximum atlas 1024, padding 3, non-directional lightmaps, 32 direct / 128 indirect / 64 environment samples, 3 bounces, probe sample multiplier 2, high quality compression. Baked AO: distance 0.35 m, indirect strength 0.65, direct strength 0.',
        '- Bake result: **one 512×512 colour lightmap**, 78 baked probes. Android overrides: **ASTC 6×6**, 1024 maximum, quality 70. No shadowmask texture is needed because all lights are baked.',
        '- 121 large surfaces receive lightmaps; 114 smaller surfaces receive probes. Small surfaces still contribute bounce/occlusion. Moving doors, the cut-away front wall, RT displays and sprites are excluded from static lightmapping.',
        '- Enabled secondary UV generation only on the Environment and Dashboard FBX importers; minimum lightmap resolution 40 → 10. Original FBX binaries and all mesh transforms are untouched. The two original importer files are preserved beside this report.',
        '- `Renderer_ReadingHouse_Mobile.asset` copies Renderer3D with post-process resources assigned, intermediate texture mode Auto, and no renderer features. The house camera uses new renderer index 2; original renderer index 1 is preserved for other scenes. Depth/opaque copies are off on this camera.',
        '- `UniversalRP.asset`: new renderer slot 2 points to the house renderer, and the probe system changes from APV to legacy groups. The pre-existing shader-prefilter edit is preserved and excluded from the polish commit; HDR remains enabled as it was before this task. The other renderer and quality settings are unchanged. `URP_HuaweiY5.asset` is unchanged; it is not the active pipeline for this scene.', '',
        '## Global Volume','',
        '| Setting | Value |','| --- | --- |',
        '| Tonemapping | Neutral |','| White balance | Temperature +4, tint +1 |','| Colour adjustments | Exposure +0.10 EV, contrast +5, saturation +5 |',
        '| Bloom | Threshold 1.5, intensity 0.12, scatter 0.42, clamp 4; quarter resolution, 2 iterations, high quality filtering off |',
        '| Vignette | Intensity 0.10, smoothness 0.40, colour #483B56 |',
        '| Camera | Post enabled, HDR enabled; same projection/poses/follow settings |','',
        '## Materials','',
        'All changes are scene material overrides on copies; original materials are unchanged. The physical chalkboard slate remains distinct from its runtime unlit page surface. RT screen blending and premultiplication are unchanged.', '',
        '| Source | Base colour before → after | Smoothness before → after | New metallic | New emission RGB |',
        '| --- | --- | --- | --- | --- |']
    with (OUT/'materials.tsv').open(encoding='utf-8-sig',newline='') as f:
        for row in csv.DictReader(f,delimiter='\t'):
            lines.append(f"| {row['source']} | {row['old base']} → {row['new base']} | {row['old smoothness']} → {row['new smoothness']} | {row['new metallic']} | {row['new emission']} |")
    lines+=['','Exact old/new emissions and new asset paths: [materials.tsv](materials.tsv). Added contact material: transparent URP Unlit, warm brown RGB (0.32, 0.23, 0.18), opacity 0.64, depth writes/shadow casting off; two quads, four triangles total.','',
        'The key stone uses Lighting/P_KeyStone_Gold.prefab, a material-only copy of the original Key prefab: base #F3CA6C, smoothness 0.24, metallic 0.08, non-emissive surface; the original gold aura remains. Original surface colours were M_WordStone RGB (0.936, 0.916, 0.865) and M_KeyGold RGB (0.978, 0.865, 0.584). Geometry, WordStone fields, label colours, effects and the original prefab are preserved. Only the house board’s key-prefab material reference changes; all MatStoneBoard layout values are audited unchanged.', '', '## Editor performance snapshots','',
        'Same live 1520×720 Game views; `UnityStats.drawCalls`, SetPass, triangles and frame delta recorded at each capture. These are individual editor samples on the RTX laptop, include world UI/particles, and vary with animation/UI activity. They are **not a Huawei benchmark or a 30/60 fps guarantee**. The existing in-game overlay incorrectly reports zero draw calls; the figures below use the editor counters.', '',
        '| View | Draw calls before → after | SetPass before → after | Triangles before → after | Editor frame ms before → after |',
        '| --- | --- | --- | --- | --- |']
    for key,label in labels.items():
        a,b=before.get(key),after.get(key)
        if a and b:lines.append(f'| {label} | {a[0]} → {b[0]} | {a[1]} → {b[1]} | {a[2]} → {b[2]} | {a[3]} → {b[3]} |')
    lines+=['','Android: no ADB device was connected, so Huawei CPU/GPU frame time, thermal behaviour and the 60/30 fps target remain unverified.']
    build=OUT/'android-build.txt'
    if build.exists():lines+=['',build.read_text(encoding='utf-8-sig').strip(), '', 'Unity returned Succeeded with 13 logged errors in existing PavilionPort SampleSceneAssets volume profiles (malformed YAML and broken PPtr references), plus the warning count above. Those files are unchanged. [Exact build diagnostics](android-messages.txt).', '', '[Android test APK](../../ReadingHouseUnity/build/sReader_ReadingHouse_Test.apk).']
    lines+=['','## Validation and remaining suggestions','',
        f"- Constraint audit: **{'PASS' if validation['passed'] else 'FAIL'}**; {validation['protected_files_checked']} protected file hashes, {validation['existing_transforms_checked']} existing transforms, {validation['prefab_pose_values_checked']} prefab pose values, {validation['existing_behaviours_checked']} serialized behaviours checked. The only permitted source-art changes are secondary-UV settings in the two static model importers.",
        '- All 35 primary and intermediate live screenshots pass the 1520×720 dimension check.',
        '- Live comparison includes the sentence directly on the illustration, navy/gold activity, stone words, chalkboard page and candy bubble. UI palettes, UXML/USS, controllers, layout values, input and data services are unchanged.',
        '- The white banners visible in editor-only geometry previews are legacy serialized placeholders. Runtime correctly hides them and draws the sentence on the illustration. No white sentence box was reintroduced.',
        '- Optional later UI review: slightly larger/darker sentence outline for busy illustrations and toned cream scrollbars inside activity pages. These are suggestions only because UI/WordStone behaviour and palettes were outside this task.',
        '- The brief describes the Encourager on the right and Zombie on the left. The saved baseline shows the opposite when facing the dashboard; the approved seating transforms were preserved.',
        '- The Huawei alternate pipeline has HDR off. If that alternate is adopted for the house later, assign the new renderer and verify HDR/bloom support and performance on the phone before switching.',
        '- Re-run the same five views on the Huawei Y5 and record GPU/CPU frame timings after thermal warm-up. Lower bloom or render scale only if the measured device budget requires it.',
        '', 'All screenshots and logs stay local. No push or publication.']
    (OUT/'REPORT.md').write_text('\n'.join(lines)+'\n',encoding='utf-8')
    print('Wrote '+str(OUT/'REPORT.md'))






def pipeline_patch():
    import difflib
    original=(OUT/'UniversalRP-before.asset.txt').read_text(encoding='utf-8-sig').splitlines(keepends=True)
    current=(ROOT/'Assets/Settings/UniversalRP.asset').read_text(encoding='utf-8-sig').splitlines(keepends=True)
    patch=''.join(difflib.unified_diff(original,current,fromfile='a/Assets/Settings/UniversalRP.asset',tofile='b/Assets/Settings/UniversalRP.asset'))
    (OUT/'pipeline-only.patch').write_bytes(patch.encode('utf-8'))
    print(patch)

if __name__=='__main__':
    {'verify':verify,'gallery':gallery,'report':report,'pipeline_patch':pipeline_patch}[sys.argv[1]]()
