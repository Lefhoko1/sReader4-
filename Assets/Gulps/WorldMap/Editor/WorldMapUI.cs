using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Builds the world-map UI overlay (HUD + nav bar) and world-space node labels.
//  Tools > World Map > Build UI Overlay
//  Tools > World Map > Build Node Labels
public static class WorldMapUI
{
    // palette
    static readonly Color BannerBlue = new Color(0.18f, 0.47f, 0.80f);
    static readonly Color BannerDark = new Color(0.10f, 0.28f, 0.52f);
    static readonly Color Gold       = new Color(0.97f, 0.80f, 0.28f);
    static readonly Color Gem        = new Color(0.35f, 0.82f, 0.95f);
    static readonly Color PanelDark  = new Color(0.09f, 0.15f, 0.27f, 0.82f);
    static readonly Color NavBg      = new Color(0.09f, 0.16f, 0.28f, 0.94f);
    static readonly Color Highlight  = new Color(0.28f, 0.55f, 0.88f);
    static readonly Color White      = Color.white;

    static Sprite Round  => AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
    static Sprite Circle => AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
    static Font   UiFont => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

    [MenuItem("Tools/World Map/Build UI Overlay")]
    static void BuildUI()
    {
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        var old = GameObject.Find("WorldMap_UI");
        if (old != null) Object.DestroyImmediate(old);

        var canvasGo = new GameObject("WorldMap_UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(720, 1520);
        scaler.matchWidthOrHeight = 0.5f;
        var root = canvasGo.GetComponent<RectTransform>();

        // ---------- Top HUD ----------
        // avatar
        var avatar = Img("Avatar", root, Circle, Gold);
        SetRect(avatar, new Vector2(0,1), new Vector2(0,1), new Vector2(0,1), new Vector2(24, -24), new Vector2(96, 96));
        var face = Img("Face", avatar, Circle, new Color(0.80f,0.66f,0.50f));
        SetRect(face, new Vector2(.5f,.5f), new Vector2(.5f,.5f), new Vector2(.5f,.5f), Vector2.zero, new Vector2(74,74));
        var lvl = Img("LevelBadge", avatar, Circle, BannerDark);
        SetRect(lvl, new Vector2(.5f,0), new Vector2(.5f,0), new Vector2(.5f,.5f), new Vector2(0, 4), new Vector2(40,40));
        Center(Label("Lvl", lvl, "23", 22, White, TextAnchor.MiddleCenter));

        // coins pill
        var coins = Img("Coins", root, Round, PanelDark);
        SetRect(coins, new Vector2(0,1), new Vector2(0,1), new Vector2(0,1), new Vector2(150, -34), new Vector2(150, 52));
        var coinIcon = Img("CoinIcon", coins, Circle, Gold);
        SetRect(coinIcon, new Vector2(0,.5f), new Vector2(0,.5f), new Vector2(0,.5f), new Vector2(6,0), new Vector2(40,40));
        var coinTxt = Label("CoinTxt", coins, "860", 28, White, TextAnchor.MiddleRight);
        SetRect(coinTxt.rectTransform, new Vector2(0,0), new Vector2(1,1), new Vector2(.5f,.5f), new Vector2(-12,0), Vector2.zero);

        // gems pill
        var gems = Img("Gems", root, Round, PanelDark);
        SetRect(gems, new Vector2(0,1), new Vector2(0,1), new Vector2(0,1), new Vector2(322, -34), new Vector2(140, 52));
        var gemIcon = Img("GemIcon", gems, Round, Gem);
        SetRect(gemIcon, new Vector2(0,.5f), new Vector2(0,.5f), new Vector2(0,.5f), new Vector2(14,0), new Vector2(30,30));
        gemIcon.transform.localRotation = Quaternion.Euler(0,0,45);   // diamond
        var gemTxt = Label("GemTxt", gems, "42", 28, White, TextAnchor.MiddleRight);
        SetRect(gemTxt.rectTransform, new Vector2(0,0), new Vector2(1,1), new Vector2(.5f,.5f), new Vector2(-12,0), Vector2.zero);

        // settings
        var settings = Img("Settings", root, Circle, PanelDark);
        SetRect(settings, new Vector2(1,1), new Vector2(1,1), new Vector2(1,1), new Vector2(-24,-24), new Vector2(76,76));
        Center(Label("Gear", settings, "⚙", 40, White, TextAnchor.MiddleCenter));

        // ---------- Title ----------
        var title = Label("Title", root, "Reading\nAdventure", 60, White, TextAnchor.UpperLeft);
        title.fontStyle = FontStyle.Bold;
        title.lineSpacing = 0.85f;
        SetRect(title.rectTransform, new Vector2(0,1), new Vector2(0,1), new Vector2(0,1), new Vector2(30,-150), new Vector2(420, 170));
        var chapter = Img("ChapterBanner", root, Round, BannerBlue);
        SetRect(chapter, new Vector2(0,1), new Vector2(0,1), new Vector2(0,1), new Vector2(30,-330), new Vector2(220, 54));
        Center(Label("ChapterTxt", chapter, "Chapter 1", 28, White, TextAnchor.MiddleCenter));

        // ---------- Library button (locked, above nav) ----------
        var lib = Img("Library", root, Circle, PanelDark);
        SetRect(lib, new Vector2(1,0), new Vector2(1,0), new Vector2(1,0), new Vector2(-70, 210), new Vector2(96,96));
        Center(Label("Lock", lib, "🔒", 30, White, TextAnchor.MiddleCenter));
        var libTxt = Label("LibTxt", lib, "Library", 22, White, TextAnchor.UpperCenter);
        SetRect(libTxt.rectTransform, new Vector2(.5f,0), new Vector2(.5f,0), new Vector2(.5f,1), new Vector2(0,-4), new Vector2(120,28));

        // ---------- Bottom nav bar ----------
        var nav = Img("NavBar", root, Round, NavBg);
        SetRect(nav, new Vector2(0,0), new Vector2(1,0), new Vector2(.5f,0), new Vector2(0, 0), new Vector2(0, 150));
        string[] items = { "Map", "Adventure", "Friends", "Bag", "Shop" };
        Color[] cols = { Highlight, new Color(.5f,.35f,.7f), new Color(.3f,.6f,.5f), new Color(.7f,.5f,.3f), new Color(.7f,.35f,.4f) };
        for (int i = 0; i < items.Length; i++)
        {
            float fx = (i + 0.5f) / items.Length;
            var item = Img("Nav_" + items[i], nav, null, new Color(0,0,0,0));
            SetRect(item, new Vector2(fx,0), new Vector2(fx,1), new Vector2(.5f,.5f), Vector2.zero, new Vector2(130, 0));
            var icon = Img("Icon", item, Round, cols[i]);
            SetRect(icon, new Vector2(.5f,1), new Vector2(.5f,1), new Vector2(.5f,1), new Vector2(0,-22), new Vector2(56,56));
            var t = Label("Txt", item, items[i], 22, i == 0 ? White : new Color(.8f,.85f,.9f), TextAnchor.LowerCenter);
            SetRect(t.rectTransform, new Vector2(0,0), new Vector2(1,0), new Vector2(.5f,0), new Vector2(0,18), new Vector2(0,30));
        }

        Selection.activeGameObject = canvasGo;
        Debug.Log("[WorldMap] UI overlay built. (Set the Game view to 720x1520 to see it laid out correctly.)");
    }

    [MenuItem("Tools/World Map/Build Node Badges")]
    static void BuildNodeBadges()
    {
        var canvasGo = GameObject.Find("WorldMap_UI");
        if (canvasGo == null) { Debug.LogError("[WorldMap] Build UI Overlay first (no WorldMap_UI canvas)."); return; }
        var camGo = GameObject.Find("WorldMapCamera");
        var cam = camGo != null ? camGo.GetComponent<Camera>() : null;

        var old = GameObject.Find("WorldMap_NodeBadges");
        if (old != null) Object.DestroyImmediate(old);
        var oldLabels = GameObject.Find("WorldMap_NodeLabels");   // remove the old floating TextMesh numbers
        if (oldLabels != null) Object.DestroyImmediate(oldLabels);
        var group = new GameObject("WorldMap_NodeBadges", typeof(RectTransform));
        group.transform.SetParent(canvasGo.transform, false);
        // put badges behind the HUD/nav but above the 3D
        group.transform.SetSiblingIndex(0);

        // world pos (from the path build), number, name, stars, ring color
        (Vector3 pos, string num, string name, string stars, Color ring)[] nodes =
        {
            (new Vector3( 6f, 3.3f, -9f), "2", "Meadow Point", "★★★",   new Color(0.30f,0.55f,0.85f)),
            (new Vector3(10f, 3.9f, -5f), "3", "Crystal Cove", "★★",    new Color(0.30f,0.55f,0.85f)),
            (new Vector3( 5f, 4.4f, -1f), "4", "Sunny Cliffs", "★★★★",  new Color(0.30f,0.55f,0.85f)),
            (new Vector3(10f, 6.3f,  4f), "5", "Brave Harbor", "★★★★★", new Color(0.45f,0.35f,0.70f)),
            (new Vector3( 6f, 6.8f,  9f), "",  "",             "",       new Color(0.30f,0.30f,0.36f)),
        };
        foreach (var nd in nodes)
        {
            var badge = new GameObject("Badge_" + (nd.num == "" ? "Locked" : nd.num), typeof(RectTransform));
            badge.transform.SetParent(group.transform, false);
            var wa = badge.AddComponent<WorldAnchoredUI>();
            wa.worldPosition = nd.pos; wa.targetCamera = cam;
            var brt = badge.GetComponent<RectTransform>();
            brt.sizeDelta = new Vector2(84, 84);

            var ring = Img("Ring", badge.transform, Circle, nd.ring);
            SetRect(ring, new Vector2(.5f,.5f), new Vector2(.5f,.5f), new Vector2(.5f,.5f), Vector2.zero, new Vector2(84,84));
            var inner = Img("Inner", ring, Circle, new Color(0.12f,0.20f,0.34f));
            SetRect(inner, new Vector2(.5f,.5f), new Vector2(.5f,.5f), new Vector2(.5f,.5f), Vector2.zero, new Vector2(64,64));

            if (nd.num != "")
                Center(Label("Num", inner, nd.num, 40, White, TextAnchor.MiddleCenter));
            else
                Center(Label("Lock", inner, "🔒", 34, White, TextAnchor.MiddleCenter));

            if (nd.stars != "")
            {
                var st = Label("Stars", badge.transform, nd.stars, 22, new Color(1f,0.82f,0.2f), TextAnchor.MiddleCenter);
                SetRect(st.rectTransform, new Vector2(.5f,0), new Vector2(.5f,0), new Vector2(.5f,1), new Vector2(0,-2), new Vector2(140,26));
            }
            if (nd.name != "")
            {
                var pill = Img("NamePill", badge.transform, Round, new Color(0.10f,0.16f,0.27f,0.9f));
                // pill to the LEFT of the badge (nodes are on the right of the frame -> avoids clipping)
                SetRect(pill, new Vector2(0,.5f), new Vector2(0,.5f), new Vector2(1,.5f), new Vector2(-8,0), new Vector2(170,44));
                Center(Label("Name", pill, nd.name, 24, White, TextAnchor.MiddleCenter));
            }
        }
        Debug.Log("[WorldMap] Node badges built (world-anchored to the path nodes).");
    }

    [MenuItem("Tools/World Map/Build Node Labels")]
    static void BuildNodeLabels()
    {
        var cam = GameObject.Find("WorldMapCamera");
        Quaternion face = cam != null ? cam.transform.rotation : Quaternion.identity;

        var old = GameObject.Find("WorldMap_NodeLabels");
        if (old != null) Object.DestroyImmediate(old);
        var grp = new GameObject("WorldMap_NodeLabels").transform;
        var root = GameObject.Find("WorldMap_Greybox");
        if (root != null) grp.SetParent(root.transform, false);

        // (x, z, number, stars)  — matches the path nodes
        (float x, float z, string n, string s)[] nodes =
        {
            (-4f, -7f, "2", "★★★"),
            ( 4f, -3f, "3", "★★"),
            (-3f,  1f, "4", "★★★★"),
            ( 3f,  4f, "5", "★★★★★"),
            (-1.5f, 6f, "", "🔒"),   // locked
        };
        foreach (var nd in nodes)
        {
            float y = SampleSurfaceY("Island_Land", nd.x, nd.z, 1.2f, 3f) + 1.9f;
            var pos = new Vector3(nd.x, y, nd.z);
            if (nd.n != "") MakeTextMesh(grp, "Num_" + nd.n, nd.n, pos, face, 0.30f, Color.white);
            if (nd.s != "") MakeTextMesh(grp, "Stars", nd.s, pos + new Vector3(0, -0.45f, 0), face, 0.14f, new Color(1f, 0.85f, 0.2f));
        }
        Debug.Log("[WorldMap] Node labels built (billboarded to the camera).");
    }

    // ---------- helpers ----------
    static RectTransform Img(string name, Transform parent, Sprite sprite, Color color)
    {
        var go = new GameObject(name, typeof(Image));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.sprite = sprite;
        img.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        img.color = color;
        if (sprite == null) img.raycastTarget = false;
        return go.GetComponent<RectTransform>();
    }

    static Text Label(string name, Transform parent, string text, int size, Color color, TextAnchor anchor)
    {
        var go = new GameObject(name, typeof(Text));
        go.transform.SetParent(parent, false);
        var t = go.GetComponent<Text>();
        t.text = text; t.font = UiFont; t.fontSize = size; t.color = color; t.alignment = anchor;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        return t;
    }

    static void Center(Text t)
    {
        SetRect(t.rectTransform, Vector2.zero, Vector2.one, new Vector2(.5f,.5f), Vector2.zero, Vector2.zero);
    }

    static void SetRect(RectTransform rt, Vector2 aMin, Vector2 aMax, Vector2 pivot, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = aMin; rt.anchorMax = aMax; rt.pivot = pivot;
        rt.anchoredPosition = pos; rt.sizeDelta = size;
    }

    static void MakeTextMesh(Transform parent, string name, string text, Vector3 pos, Quaternion face, float size, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, true);
        go.transform.position = pos;
        go.transform.rotation = face;
        var tm = go.AddComponent<TextMesh>();
        tm.text = text; tm.characterSize = size; tm.fontSize = 64; tm.color = color;
        tm.anchor = TextAnchor.MiddleCenter; tm.alignment = TextAlignment.Center;
        tm.font = UiFont;
        go.GetComponent<MeshRenderer>().sharedMaterial = UiFont.material;
    }

    static float SampleSurfaceY(string islandName, float x, float z, float radius, float fallback)
    {
        var go = GameObject.Find(islandName);
        var mf = go != null ? go.GetComponentInChildren<MeshFilter>() : null;
        if (mf == null || mf.sharedMesh == null) return fallback;
        var verts = mf.sharedMesh.vertices; var tf = mf.transform;
        float best = float.NegativeInfinity, r2 = radius * radius;
        foreach (var v in verts)
        {
            var w = tf.TransformPoint(v);
            float dx = w.x - x, dz = w.z - z;
            if (dx * dx + dz * dz <= r2 && w.y > best) best = w.y;
        }
        return float.IsNegativeInfinity(best) ? fallback : best;
    }
}
