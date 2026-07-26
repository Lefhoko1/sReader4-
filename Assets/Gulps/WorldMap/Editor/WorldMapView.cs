using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// Sets the Game view to the device portrait resolution (720x1520) and focuses the
// fixed diorama camera, so the world-map greybox is framed exactly like the design.
//
// Run from the menu:  Tools > World Map > Setup Portrait View (720x1520)
public static class WorldMapView
{
    const int W = 720;
    const int H = 1520;
    const string SizeLabel = "WorldMap Portrait";

    // Diorama camera framing — tweak these, then re-run "Frame Camera" (no greybox rebuild needed).
    static readonly Vector3 CamPos    = new Vector3(2f, 34f, -18f);
    static readonly Vector3 CamTarget = new Vector3(3f, 7f, 6f);
    const float CamFov = 68f;

    [MenuItem("Tools/World Map/Setup Portrait View (720x1520)")]
    static void Setup()
    {
        // 1) Game view resolution -------------------------------------------------
        try
        {
            int idx = FindOrAddFixedSize(W, H, SizeLabel);
            if (idx >= 0 && SetGameViewSize(idx))
                Debug.Log($"[WorldMap] Game view set to {W}x{H} portrait.");
            else
                Debug.LogWarning($"[WorldMap] Added {W}x{H} to the Game view size list — " +
                                 "if it didn't auto-select, pick 'WorldMap Portrait' from the Game view aspect dropdown.");
        }
        catch (Exception e)
        {
            Debug.LogWarning("[WorldMap] Couldn't set the Game view size automatically (" + e.Message +
                             "). Add a Fixed Resolution 720x1520 manually in the Game view aspect dropdown.");
        }

        // 2) Focus the diorama camera --------------------------------------------
        var camGo = GameObject.Find("WorldMapCamera");
        if (camGo == null)
        {
            Debug.LogWarning("[WorldMap] 'WorldMapCamera' not found. Run Tools > World Map > Build Greybox first.");
            return;
        }

        FrameCameraInternal(camGo);

        Selection.activeGameObject = camGo;
        var sv = SceneView.lastActiveSceneView;
        if (sv != null) sv.AlignViewToObject(camGo.transform);

        // Make sure a Game view is open/visible.
        var gvType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        if (gvType != null) EditorWindow.GetWindow(gvType, false, "Game", false);

        Debug.Log("[WorldMap] Focused 'WorldMapCamera'. Press Play or view the Game tab to see the portrait framing.");
    }

    [MenuItem("Tools/World Map/Frame Camera")]
    static void FrameCamera()
    {
        var camGo = GameObject.Find("WorldMapCamera");
        if (camGo == null)
        {
            Debug.LogWarning("[WorldMap] 'WorldMapCamera' not found. Run Build Greybox first.");
            return;
        }
        FrameCameraInternal(camGo);
        Selection.activeGameObject = camGo;
        var sv = SceneView.lastActiveSceneView;
        if (sv != null) sv.AlignViewToObject(camGo.transform);
        Debug.Log($"[WorldMap] Camera framed at {CamPos} -> {CamTarget}, FOV {CamFov}.");
    }

    static void FrameCameraInternal(GameObject camGo)
    {
        var cam = camGo.GetComponent<Camera>();
        camGo.transform.position = CamPos;
        camGo.transform.LookAt(CamTarget);
        if (cam != null)
        {
            cam.fieldOfView = CamFov;
            cam.depth = 10;           // draw on top of any existing Main Camera in the Game view
            cam.farClipPlane = Mathf.Max(cam.farClipPlane, 300f);
        }
    }

    // ---- Reflection into UnityEditor's internal GameView size system ----
    static object SizesInstance()
    {
        var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        return singleton.GetProperty("instance").GetValue(null, null);
    }

    static object CurrentGroup()
    {
        var inst = SizesInstance();
        var sizesType = inst.GetType();
        var currentGroupType = sizesType.GetProperty("currentGroupType").GetValue(inst, null);
        var enumVal = Enum.ToObject(
            typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeGroupType"), currentGroupType);
        return sizesType.GetMethod("GetGroup").Invoke(inst, new object[] { enumVal });
    }

    static int FindOrAddFixedSize(int width, int height, string label)
    {
        var group = CurrentGroup();
        var gType = group.GetType();

        int count = (int)gType.GetMethod("GetBuiltinCount").Invoke(group, null)
                  + (int)gType.GetMethod("GetCustomCount").Invoke(group, null);

        var getSize = gType.GetMethod("GetGameViewSize");
        var arg = new object[1];
        for (int i = 0; i < count; i++)
        {
            arg[0] = i;
            var size = getSize.Invoke(group, arg);
            var st = size.GetType();
            int w = (int)st.GetProperty("width").GetValue(size, null);
            int h = (int)st.GetProperty("height").GetValue(size, null);
            if (w == width && h == height) return i;
        }

        // Not present — create a Fixed Resolution entry and add it.
        var gvSizeType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
        var gvSizeTypeEnum = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");
        var ctor = gvSizeType.GetConstructor(new[] { gvSizeTypeEnum, typeof(int), typeof(int), typeof(string) });
        var fixedRes = Enum.Parse(gvSizeTypeEnum, "FixedResolution");
        var newSize = ctor.Invoke(new[] { fixedRes, (object)width, height, label });
        gType.GetMethod("AddCustomSize").Invoke(group, new[] { newSize });

        // Re-scan for its index.
        count = (int)gType.GetMethod("GetBuiltinCount").Invoke(group, null)
              + (int)gType.GetMethod("GetCustomCount").Invoke(group, null);
        for (int i = 0; i < count; i++)
        {
            arg[0] = i;
            var size = getSize.Invoke(group, arg);
            var st = size.GetType();
            int w = (int)st.GetProperty("width").GetValue(size, null);
            int h = (int)st.GetProperty("height").GetValue(size, null);
            if (w == width && h == height) return i;
        }
        return -1;
    }

    static bool SetGameViewSize(int index)
    {
        var gvType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        if (gvType == null) return false;
        var win = EditorWindow.GetWindow(gvType, false, "Game", false);

        // Preferred: internal property 'selectedSizeIndex'.
        var prop = gvType.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (prop != null) { prop.SetValue(win, index, null); win.Repaint(); return true; }

        var field = gvType.GetField("m_SelectedSizeIndex", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field != null) { field.SetValue(win, index); win.Repaint(); return true; }

        var method = gvType.GetMethod("SizeSelectionCallback", BindingFlags.Instance | BindingFlags.NonPublic);
        if (method != null) { method.Invoke(win, new object[] { index, null }); win.Repaint(); return true; }

        return false;
    }
}
