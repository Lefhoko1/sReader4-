// ===========================================================================
//  SeaWaterSetup — one-click animated sea for the island vista (Editor)
// ===========================================================================
//  Tools > Great Library > Island > 5. Build Sea Water
//
//  1. Bakes two SEAMLESS tiling wave maps into Assets/Art/Textures_Island:
//        T_Wave_Swell  — broad rolling swell
//        T_Wave_Chop   — fine surface chop (also drives glitter + foam noise)
//     RGB = normal, A = height. Generated with a periodic gradient noise, so
//     they tile perfectly with no visible repeat — no photo needed.
//  2. Points M_Sea at GreatLibrary/StylizedSea and tunes it to the vista's
//     palette and its fog/sun setup.
//  3. Finds the sea renderer in the open scene and, if its mesh is a flat
//     low-poly quad, swaps in a generated dense grid of the same size so the
//     vertex swell has something to displace.
//  4. Registers the project's runtime-found shaders in Always Included
//     Shaders, so Shader.Find still works in a device build.
//
//  Safe to re-run. Step 3 needs SC_IslandVista open; 1, 2 and 4 don't.
// ===========================================================================
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class SeaWaterSetup
{
    const string TEX_DIR  = "Assets/Art/Textures_Island";
    const string MAT_DIR  = "Assets/Art/Materials_Island";
    const string MESH_DIR = "Assets/Art/Meshes_Island";
    const string SEA_MAT  = MAT_DIR + "/M_Sea.mat";
    const string SWELL    = TEX_DIR + "/T_Wave_Swell.png";
    const string CHOP     = TEX_DIR + "/T_Wave_Chop.png";
    const string GRID     = MESH_DIR + "/SM_SeaGrid.asset";

    const string SEA_SHADER = "GreatLibrary/StylizedSea";

    // Shaders this project resolves at runtime with Shader.Find. Anything in
    // here must ship in the build or the effect silently falls back.
    static readonly string[] RUNTIME_SHADERS =
    {
        SEA_SHADER,
        "GreatLibrary/SparkleAdditive",
        "GreatLibrary/MoteAdditive",
        "GreatLibrary/InkSpread",
    };

    [MenuItem("Tools/Great Library/Island/5. Build Sea Water")]
    public static void BuildSeaWater()
    {
        EnsureDir(TEX_DIR); EnsureDir(MAT_DIR); EnsureDir(MESH_DIR);

        BakeWaveMaps();
        var mat = BuildSeaMaterial();
        int swapped = UpgradeSeaMeshInScene(mat);
        RegisterAlwaysIncludedShaders();

        AssetDatabase.SaveAssets();
        Debug.Log($"[Sea] Wave maps baked, M_Sea on {SEA_SHADER}, " +
                  $"{swapped} sea mesh(es) subdivided. " +
                  (swapped == 0
                      ? "No flat sea mesh found in the open scene — the shader "
                      + "still animates fully (the motion is in the normals); "
                      + "vertex swell stays off."
                      : "Vertex swell enabled."));
    }

    // ==================================================== 1. WAVE MAPS =====
    static void BakeWaveMaps()
    {
        // Broad swell: few, large, soft features.
        Bake(SWELL, 512, basePeriod: 4, octaves: 3, slope: 2.6f, seed: 1337,
             ridged: false);
        // Fine chop: many small crests. Ridged noise gives crisper crest lines,
        // which is what the glitter threshold latches onto.
        Bake(CHOP, 512, basePeriod: 10, octaves: 4, slope: 3.4f, seed: 9001,
             ridged: true);
    }

    static void Bake(string path, int size, int basePeriod, int octaves,
                     float slope, int seed, bool ridged)
    {
        // --- height field ------------------------------------------------
        var h = new float[size * size];
        float min = float.MaxValue, max = float.MinValue;
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float u = x / (float)size, v = y / (float)size;
            float n = Fbm(u, v, basePeriod, octaves, seed, ridged);
            h[y * size + x] = n;
            if (n < min) min = n;
            if (n > max) max = n;
        }
        float inv = 1f / Mathf.Max(max - min, 1e-5f);
        for (int i = 0; i < h.Length; i++) h[i] = (h[i] - min) * inv;

        // --- normals from central differences, wrapped -------------------
        var px = new Color32[size * size];
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float hl = h[y * size + Wrap(x - 1, size)];
            float hr = h[y * size + Wrap(x + 1, size)];
            float hd = h[Wrap(y - 1, size) * size + x];
            float hu = h[Wrap(y + 1, size) * size + x];

            var n = new Vector3((hl - hr) * slope, (hd - hu) * slope, 1f).normalized;
            px[y * size + x] = new Color32(
                (byte)Mathf.RoundToInt((n.x * 0.5f + 0.5f) * 255f),
                (byte)Mathf.RoundToInt((n.y * 0.5f + 0.5f) * 255f),
                (byte)Mathf.RoundToInt((n.z * 0.5f + 0.5f) * 255f),
                (byte)Mathf.RoundToInt(h[y * size + x] * 255f));   // height in A
        }

        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false, true);
        tex.SetPixels32(px);
        tex.Apply();
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        var imp = (TextureImporter)AssetImporter.GetAtPath(path);
        // Plain colour, NOT NormalMap: that keeps RGB unswizzled across
        // platforms, so the shader can decode it by hand and use A for height.
        imp.textureType         = TextureImporterType.Default;
        imp.sRGBTexture         = false;
        imp.alphaSource         = TextureImporterAlphaSource.FromInput;
        imp.alphaIsTransparency = false;
        imp.wrapMode            = TextureWrapMode.Repeat;
        imp.filterMode          = FilterMode.Bilinear;
        imp.mipmapEnabled       = true;
        imp.maxTextureSize      = size;
        imp.textureCompression  = TextureImporterCompression.CompressedHQ;
        imp.SaveAndReimport();
    }

    static int Wrap(int i, int n) => (i % n + n) % n;

    // --- periodic gradient (Perlin) noise: tiles exactly over [0,1) --------
    static float Fbm(float u, float v, int basePeriod, int octaves, int seed,
                     bool ridged)
    {
        float sum = 0f, amp = 1f, norm = 0f;
        int period = basePeriod;
        for (int o = 0; o < octaves; o++)
        {
            float n = Perlin(u * period, v * period, period, seed + o * 131);
            if (ridged) n = 1f - Mathf.Abs(n) * 2f;      // crest lines
            sum  += n * amp;
            norm += amp;
            amp  *= 0.5f;
            period *= 2;
        }
        return sum / Mathf.Max(norm, 1e-5f);
    }

    static float Perlin(float x, float y, int period, int seed)
    {
        int x0 = Mathf.FloorToInt(x), y0 = Mathf.FloorToInt(y);
        float fx = x - x0, fy = y - y0;
        int wx0 = Wrap(x0, period), wx1 = Wrap(x0 + 1, period);
        int wy0 = Wrap(y0, period), wy1 = Wrap(y0 + 1, period);

        float u = Fade(fx), v = Fade(fy);
        float n00 = Dot(wx0, wy0, seed, fx,        fy);
        float n10 = Dot(wx1, wy0, seed, fx - 1f,   fy);
        float n01 = Dot(wx0, wy1, seed, fx,        fy - 1f);
        float n11 = Dot(wx1, wy1, seed, fx - 1f,   fy - 1f);
        return Mathf.Lerp(Mathf.Lerp(n00, n10, u), Mathf.Lerp(n01, n11, u), v);
    }

    static float Fade(float t) => t * t * t * (t * (t * 6f - 15f) + 10f);

    static float Dot(int gx, int gy, int seed, float dx, float dy)
    {
        uint hsh = (uint)(gx * 73856093) ^ (uint)(gy * 19349663) ^ (uint)(seed * 83492791);
        hsh ^= hsh >> 13; hsh *= 1274126177u; hsh ^= hsh >> 16;
        float a = (hsh & 0xFFFFu) / 65535f * Mathf.PI * 2f;
        return Mathf.Cos(a) * dx + Mathf.Sin(a) * dy;
    }

    // ==================================================== 2. MATERIAL ======
    static Material BuildSeaMaterial()
    {
        var sh = Shader.Find(SEA_SHADER);
        if (sh == null)
        {
            Debug.LogError($"[Sea] {SEA_SHADER} not found. Is " +
                           "Assets/Art/Shaders/StylizedSea.shader imported " +
                           "and compiling?");
            return null;
        }

        var mat = AssetDatabase.LoadAssetAtPath<Material>(SEA_MAT);
        if (mat == null)
        {
            mat = new Material(sh) { name = "M_Sea" };
            AssetDatabase.CreateAsset(mat, SEA_MAT);
        }
        mat.shader = sh;

        mat.SetTexture("_NormalMap", AssetDatabase.LoadAssetAtPath<Texture2D>(SWELL));
        mat.SetTexture("_DetailMap", AssetDatabase.LoadAssetAtPath<Texture2D>(CHOP));

        // Tuned against the vista's sun (1.0, 0.93, 0.80 @ 1.35) and its
        // linear fog (0.66, 0.80, 0.92 from 18m to 85m).
        mat.SetColor("_ShallowColor", new Color(0.44f, 0.80f, 0.78f));
        mat.SetColor("_DeepColor",    new Color(0.05f, 0.24f, 0.38f));
        mat.SetColor("_HorizonColor", new Color(0.66f, 0.84f, 0.93f));
        mat.SetFloat("_DepthFade",    4.0f);
        mat.SetFloat("_AlphaShallow", 0.30f);
        mat.SetFloat("_AlphaDeep",    0.97f);

        mat.SetFloat("_SwellTiling",   0.035f);
        mat.SetFloat("_SwellStrength", 0.85f);
        mat.SetFloat("_SwellSpeed",    0.020f);
        mat.SetFloat("_ChopTiling",    0.150f);
        mat.SetFloat("_ChopStrength",  0.55f);
        mat.SetFloat("_ChopSpeed",     0.055f);
        mat.SetFloat("_DetailFade",    90f);

        mat.SetColor("_SunColor",       new Color(1f, 0.97f, 0.88f));
        mat.SetFloat("_SunSharpness",   420f);
        mat.SetFloat("_SunStrength",    2.2f);
        mat.SetFloat("_GlitterTiling",  0.30f);
        mat.SetFloat("_GlitterSharp",   500f);
        mat.SetFloat("_GlitterCover",   0.35f);
        mat.SetFloat("_GlitterStrength", 5.0f);
        mat.SetFloat("_GlitterRange",   70f);
        mat.SetFloat("_FresnelPower",   4.5f);
        mat.SetFloat("_FresnelStrength", 0.55f);

        mat.SetColor("_FoamColor",   new Color(1f, 1f, 1f, 0.90f));
        mat.SetFloat("_FoamDepth",    0.9f);
        mat.SetFloat("_FoamSoftness", 0.42f);
        mat.SetFloat("_FoamTiling",   0.35f);
        mat.SetFloat("_FoamSpeed",    0.10f);

        mat.SetFloat("_WaveHeight", 0f);        // raised by step 3 if we subdivide
        mat.SetFloat("_WaveLength", 9f);
        mat.SetFloat("_WaveSpeedV", 0.55f);

        mat.renderQueue = (int)RenderQueue.Transparent - 100;
        EditorUtility.SetDirty(mat);
        return mat;
    }

    // ============================================ 3. SUBDIVIDE THE SEA =====
    // The FBX sea is a flat quad. Vertex swell on 4 corners would just tilt the
    // whole plane, so we swap in a dense grid covering the same footprint.
    static int UpgradeSeaMeshInScene(Material seaMat)
    {
        if (seaMat == null) return 0;

        var targets = Object.FindObjectsByType<MeshRenderer>(
                          FindObjectsInactive.Include, FindObjectsSortMode.None)
                      .Where(IsSeaRenderer)
                      .ToList();

        if (targets.Count == 0) return 0;

        Mesh grid = null;
        int swapped = 0;
        foreach (var r in targets)
        {
            var mf = r.GetComponent<MeshFilter>();
            var src = mf != null ? mf.sharedMesh : null;

            r.sharedMaterials = r.sharedMaterials.Select(_ => seaMat).ToArray();
            r.shadowCastingMode = ShadowCastingMode.Off;   // water casting shadows looks wrong
            EditorUtility.SetDirty(r);

            if (src == null) continue;
            var b = src.bounds;
            bool flat  = b.size.y < Mathf.Max(b.size.x, b.size.z) * 0.02f;
            bool lowRes = src.vertexCount < 2000;
            if (!flat || !lowRes) continue;      // already dense, or not a plane

            if (grid == null)
            {
                grid = BuildGrid(b, 140);
                AssetDatabase.CreateAsset(grid, GRID);
            }
            Undo.RecordObject(mf, "Subdivide sea");
            mf.sharedMesh = grid;
            EditorUtility.SetDirty(mf);
            swapped++;
        }

        if (swapped > 0)
        {
            seaMat.SetFloat("_WaveHeight", 0.10f);
            EditorUtility.SetDirty(seaMat);
        }
        return swapped;
    }

    static bool IsSeaRenderer(MeshRenderer r)
    {
        if (Contains(r.gameObject.name)) return true;
        return r.sharedMaterials.Any(m => m != null && Contains(m.name));

        bool Contains(string s) =>
            s.IndexOf("sea",   System.StringComparison.OrdinalIgnoreCase) >= 0 ||
            s.IndexOf("water", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
            s.IndexOf("ocean", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    static Mesh BuildGrid(Bounds b, int n)
    {
        var verts = new Vector3[(n + 1) * (n + 1)];
        var uvs   = new Vector2[verts.Length];
        var norms = new Vector3[verts.Length];
        for (int z = 0; z <= n; z++)
        for (int x = 0; x <= n; x++)
        {
            float u = x / (float)n, v = z / (float)n;
            int i = z * (n + 1) + x;
            verts[i] = new Vector3(Mathf.Lerp(b.min.x, b.max.x, u),
                                   b.center.y,
                                   Mathf.Lerp(b.min.z, b.max.z, v));
            uvs[i]   = new Vector2(u, v);
            norms[i] = Vector3.up;
        }

        var tris = new int[n * n * 6];
        int k = 0;
        for (int z = 0; z < n; z++)
        for (int x = 0; x < n; x++)
        {
            int i = z * (n + 1) + x;
            tris[k++] = i;             tris[k++] = i + n + 1; tris[k++] = i + 1;
            tris[k++] = i + 1;         tris[k++] = i + n + 1; tris[k++] = i + n + 2;
        }

        var m = new Mesh { name = "SM_SeaGrid" };
        m.indexFormat = verts.Length > 65000
            ? UnityEngine.Rendering.IndexFormat.UInt32
            : UnityEngine.Rendering.IndexFormat.UInt16;
        m.vertices = verts; m.uv = uvs; m.normals = norms; m.triangles = tris;
        m.RecalculateBounds();
        return m;
    }

    // ================================== 4. ALWAYS INCLUDED SHADERS =========
    static void RegisterAlwaysIncludedShaders()
    {
        var gs = AssetDatabase.LoadAllAssetsAtPath(
                     "ProjectSettings/GraphicsSettings.asset").FirstOrDefault();
        if (gs == null) { Debug.LogWarning("[Sea] GraphicsSettings not readable."); return; }

        var so   = new SerializedObject(gs);
        var list = so.FindProperty("m_AlwaysIncludedShaders");
        if (list == null) { Debug.LogWarning("[Sea] No m_AlwaysIncludedShaders."); return; }

        var have = new HashSet<Shader>();
        for (int i = 0; i < list.arraySize; i++)
        {
            var s = list.GetArrayElementAtIndex(i).objectReferenceValue as Shader;
            if (s != null) have.Add(s);
        }

        var added = new List<string>();
        foreach (var name in RUNTIME_SHADERS)
        {
            var s = Shader.Find(name);
            if (s == null || have.Contains(s)) continue;
            list.InsertArrayElementAtIndex(list.arraySize);
            list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = s;
            have.Add(s);
            added.Add(name);
        }
        if (added.Count == 0) return;

        so.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
        Debug.Log("[Sea] Added to Always Included Shaders: " + string.Join(", ", added));
    }

    static void EnsureDir(string dir)
    {
        if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); AssetDatabase.Refresh(); }
    }
}
