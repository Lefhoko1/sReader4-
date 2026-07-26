using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// SReader Login Screen
/// Attach this script to an empty GameObject in your scene.
/// Requires: TextMeshPro package installed.
/// Usage: Attach to a GameObject, hit Play — the full login UI is built at runtime.
/// </summary>
public class SReaderLoginUI : MonoBehaviour
{
    // ── Colour palette ────────────────────────────────────────────────────────
    static readonly Color ColSkyTop      = HexColor("D6E8FF");
    static readonly Color ColSkyBottom   = HexColor("EEF4FF");
    static readonly Color ColPurpleDark  = HexColor("3D2EB8");
    static readonly Color ColPurpleMid   = HexColor("6C5CE7");
    static readonly Color ColPurpleLight = HexColor("8B7CF8");
    static readonly Color ColTeal        = HexColor("00B4A6");
    static readonly Color ColOrange      = HexColor("F5A623");
    static readonly Color ColTextDark    = HexColor("1A1A3E");
    static readonly Color ColTextGrey    = HexColor("8892A4");
    static readonly Color ColWhite       = HexColor("FFFFFF");
    static readonly Color ColCardBg      = HexColor("FFFFFF");
    static readonly Color ColInputBg     = HexColor("F4F6FF");
    static readonly Color ColInputBorder = HexColor("DDE2F0");
    static readonly Color ColBtnLogin    = HexColor("7B5CF5");   // gradient approximated

    // ── Role-selector state ───────────────────────────────────────────────────
    private int selectedRole = 0;   // 0=Student  1=Tutor  2=Guardian
    private GameObject[] rolePanels = new GameObject[3];
    private TextMeshProUGUI[] roleLabels = new TextMeshProUGUI[3];

    // ── Password toggle ───────────────────────────────────────────────────────
    private TMP_InputField passwordField;
    private bool passwordVisible = false;

    // ── Canvas reference ──────────────────────────────────────────────────────
    private Canvas rootCanvas;

    // ─────────────────────────────────────────────────────────────────────────
    void Start() => BuildUI();

    // ═════════════════════════════════════════════════════════════════════════
    //  ENTRY POINT
    // ═════════════════════════════════════════════════════════════════════════
    void BuildUI()
    {
        // ── Root Canvas ───────────────────────────────────────────────────────
        rootCanvas = CreateCanvas();

        // ── Sky gradient background ───────────────────────────────────────────
        var bg = CreateImage(rootCanvas.gameObject, "Background", ColSkyBottom);
        StretchFill(bg.GetComponent<RectTransform>());
        // Simple 2-tone gradient using a second overlay panel
        var bgTop = CreateImage(bg, "BgTopGrad", new Color(ColSkyTop.r, ColSkyTop.g, ColSkyTop.b, 0.6f));
        var bgTopRT = bgTop.GetComponent<RectTransform>();
        bgTopRT.anchorMin = new Vector2(0, 0.5f);
        bgTopRT.anchorMax = Vector2.one;
        bgTopRT.offsetMin = bgTopRT.offsetMax = Vector2.zero;

        // ── Scroll / content root ─────────────────────────────────────────────
        var content = CreateRectObject(bg, "ContentRoot");
        var contentRT = content.GetComponent<RectTransform>();
        StretchFill(contentRT);
        // Vertical layout
        var vl = content.AddComponent<VerticalLayoutGroup>();
        vl.childAlignment = TextAnchor.UpperCenter;
        vl.spacing = 0;
        vl.padding = new RectOffset(24, 24, 40, 40);
        vl.childControlWidth = true;
        vl.childControlHeight = false;
        vl.childForceExpandWidth = true;
        vl.childForceExpandHeight = false;

        // ── LOGO SECTION ──────────────────────────────────────────────────────
        BuildLogoSection(content);

        // ── "Welcome Back" heading ────────────────────────────────────────────
        AddSpacerElement(content, 16);
        var welcome = CreateTMP(content, "WelcomeTitle", "Welcome Back",
            36, FontStyles.Bold, ColTextDark, TextAlignmentOptions.Center);
        SetLayoutElement(welcome, minHeight: 46);

        var subtitle = CreateTMP(content, "Subtitle", "Log in to continue your learning journey",
            14, FontStyles.Normal, ColTextGrey, TextAlignmentOptions.Center);
        SetLayoutElement(subtitle, minHeight: 22);

        // Purple underline bar
        AddSpacerElement(content, 8);
        var bar = CreateImage(content, "Underbar", ColPurpleMid);
        var barRT = bar.GetComponent<RectTransform>();
        barRT.sizeDelta = new Vector2(40, 3);
        var barLE = bar.AddComponent<LayoutElement>();
        barLE.minHeight = 3;
        barLE.preferredHeight = 3;
        barLE.flexibleWidth = 0;
        // Centre horizontally inside the HLG
        var barHolder = CreateRectObject(content, "UnderbarHolder");
        SetLayoutElement(barHolder, minHeight: 3);
        var barHolderHL = barHolder.AddComponent<HorizontalLayoutGroup>();
        barHolderHL.childAlignment = TextAnchor.MiddleCenter;
        barHolderHL.childControlWidth = false;
        barHolderHL.childControlHeight = true;
        barHolderHL.childForceExpandWidth = false;
        barHolderHL.childForceExpandHeight = false;
        bar.transform.SetParent(barHolder.transform, false);

        // ── WHITE CARD ────────────────────────────────────────────────────────
        AddSpacerElement(content, 20);
        var card = BuildCard(content);
        SetLayoutElement(card, minHeight: 430);

        // ── BACK BUTTON ───────────────────────────────────────────────────────
        AddSpacerElement(content, 16);
        BuildBackButton(content);
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  LOGO SECTION
    // ═════════════════════════════════════════════════════════════════════════
    void BuildLogoSection(GameObject parent)
    {
        var logoRoot = CreateRectObject(parent, "LogoSection");
        SetLayoutElement(logoRoot, minHeight: 110);
        var vl = logoRoot.AddComponent<VerticalLayoutGroup>();
        vl.childAlignment = TextAnchor.UpperCenter;
        vl.spacing = 4;
        vl.childControlWidth = true;
        vl.childControlHeight = false;
        vl.childForceExpandWidth = true;
        vl.childForceExpandHeight = false;

        // Book icon placeholder (blue rounded rect)
        var bookHolder = CreateRectObject(logoRoot, "BookIconHolder");
        SetLayoutElement(bookHolder, minHeight: 50);
        var bookHolderHL = bookHolder.AddComponent<HorizontalLayoutGroup>();
        bookHolderHL.childAlignment = TextAnchor.MiddleCenter;
        bookHolderHL.childControlWidth = false;
        bookHolderHL.childControlHeight = true;
        bookHolderHL.childForceExpandWidth = false;
        bookHolderHL.childForceExpandHeight = false;

        var bookIcon = CreateImage(bookHolder, "BookIcon", ColPurpleDark);
        bookIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(52, 44);
        bookIcon.GetComponent<Image>().type = Image.Type.Sliced;
        AddRoundedCorners(bookIcon, 8);

        // "SReader" text rendered in two colours via rich text
        var logoText = CreateTMP(logoRoot, "LogoText",
            "<color=#F5A623>S</color><color=#3D2EB8>Reader</color>",
            40, FontStyles.Bold | FontStyles.Italic, ColWhite, TextAlignmentOptions.Center);
        logoText.enableVertexGradient = false;
        logoText.richText = true;
        SetLayoutElement(logoText, minHeight: 50);

        // "Read • Learn • Grow" purple banner
        var bannerHolder = CreateRectObject(logoRoot, "BannerHolder");
        SetLayoutElement(bannerHolder, minHeight: 26);
        var bHL = bannerHolder.AddComponent<HorizontalLayoutGroup>();
        bHL.childAlignment = TextAnchor.MiddleCenter;
        bHL.childControlWidth = false;
        bHL.childControlHeight = true;
        bHL.childForceExpandWidth = false;
        bHL.childForceExpandHeight = false;

        var banner = CreateRectObject(bannerHolder, "Banner");
        banner.GetComponent<RectTransform>().sizeDelta = new Vector2(190, 26);
        var bannerImg = banner.AddComponent<Image>();
        bannerImg.color = ColPurpleDark;
        AddRoundedCorners(banner, 13);

        var bannerTxt = CreateTMP(banner, "BannerTxt",
            "Read  •  Learn  •  Grow",
            11, FontStyles.Bold, ColWhite, TextAlignmentOptions.Center);
        bannerTxt.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        bannerTxt.GetComponent<RectTransform>().anchorMax = Vector2.one;
        bannerTxt.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        bannerTxt.GetComponent<RectTransform>().offsetMax = Vector2.zero;
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  WHITE CARD
    // ═════════════════════════════════════════════════════════════════════════
    GameObject BuildCard(GameObject parent)
    {
        var card = CreateImage(parent, "Card", ColCardBg);
        AddRoundedCorners(card, 24);
        var cardVL = card.AddComponent<VerticalLayoutGroup>();
        cardVL.padding = new RectOffset(20, 20, 28, 28);
        cardVL.spacing = 14;
        cardVL.childAlignment = TextAnchor.UpperCenter;
        cardVL.childControlWidth = true;
        cardVL.childControlHeight = false;
        cardVL.childForceExpandWidth = true;
        cardVL.childForceExpandHeight = false;

        // ── Email input ───────────────────────────────────────────────────────
        var emailRow = BuildInputField(card, "EmailInput", "Email address",
            isPassword: false, out _);
        SetLayoutElement(emailRow, minHeight: 54);

        // ── Password input ────────────────────────────────────────────────────
        var passRow = BuildInputField(card, "PasswordInput", "Password",
            isPassword: true, out passwordField);
        SetLayoutElement(passRow, minHeight: 54);

        // ── Forgot password ───────────────────────────────────────────────────
        var forgotHolder = CreateRectObject(card, "ForgotHolder");
        SetLayoutElement(forgotHolder, minHeight: 24);
        var forgotHL = forgotHolder.AddComponent<HorizontalLayoutGroup>();
        forgotHL.childAlignment = TextAnchor.MiddleRight;
        forgotHL.childForceExpandWidth = true;
        forgotHL.childForceExpandHeight = false;

        var forgotBtn = CreateTMP(forgotHolder, "ForgotBtn", "Forgot Password?",
            13, FontStyles.Normal, ColPurpleMid, TextAlignmentOptions.Right);
        SetLayoutElement(forgotBtn, minHeight: 24);
        var forgotBtnGO = forgotBtn.gameObject;
        var forgotBtnComp = forgotBtnGO.AddComponent<Button>();
        forgotBtnComp.targetGraphic = forgotBtnGO.AddComponent<Image>();
        forgotBtnComp.targetGraphic.color = new Color(0, 0, 0, 0);
        forgotBtnComp.onClick.AddListener(() => Debug.Log("Forgot Password tapped"));

        // ── "I am logging in as" ──────────────────────────────────────────────
        var roleLabel = CreateTMP(card, "RoleLabel", "I am logging in as",
            13, FontStyles.Bold, ColTextDark, TextAlignmentOptions.Left);
        SetLayoutElement(roleLabel, minHeight: 20);

        // ── Role selector row ─────────────────────────────────────────────────
        var roleRow = BuildRoleSelector(card);
        SetLayoutElement(roleRow, minHeight: 96);

        // ── Login button ──────────────────────────────────────────────────────
        var loginBtn = BuildLoginButton(card);
        SetLayoutElement(loginBtn, minHeight: 54);

        // ── OR divider ────────────────────────────────────────────────────────
        BuildOrDivider(card);

        // ── Sign Up row ───────────────────────────────────────────────────────
        BuildSignUpRow(card);

        return card;
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  INPUT FIELD
    // ═════════════════════════════════════════════════════════════════════════
    GameObject BuildInputField(GameObject parent, string name, string placeholder,
        bool isPassword, out TMP_InputField fieldOut)
    {
        var container = CreateImage(parent, name, ColInputBg);
        AddRoundedCorners(container, 14);
        var outline = container.AddComponent<Outline>();
        outline.effectColor = ColInputBorder;
        outline.effectDistance = new Vector2(1, -1);

        var hl = container.AddComponent<HorizontalLayoutGroup>();
        hl.padding = new RectOffset(14, 14, 0, 0);
        hl.spacing = 10;
        hl.childAlignment = TextAnchor.MiddleLeft;
        hl.childControlWidth = false;
        hl.childControlHeight = true;
        hl.childForceExpandWidth = false;
        hl.childForceExpandHeight = false;

        // Icon
        var icon = CreateImage(container, name + "_Icon", isPassword ? ColPurpleMid : ColPurpleMid);
        icon.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
        var iconLE = icon.AddComponent<LayoutElement>();
        iconLE.minWidth = 20;
        iconLE.preferredWidth = 20;
        iconLE.flexibleWidth = 0;
        // Draw a simple icon via text glyph
        var iconTxt = CreateTMP(icon, "IconGlyph",
            isPassword ? "🔒" : "✉",
            14, FontStyles.Normal, ColPurpleMid, TextAlignmentOptions.Center);
        iconTxt.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        iconTxt.GetComponent<RectTransform>().anchorMax = Vector2.one;
        iconTxt.GetComponent<RectTransform>().offsetMin = iconTxt.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        Object.Destroy(icon.GetComponent<Image>());

        // TMP_InputField
        var fieldGO = CreateRectObject(container, name + "_Field");
        var fieldRT = fieldGO.GetComponent<RectTransform>();
        fieldRT.sizeDelta = new Vector2(0, 54);
        var fieldLE = fieldGO.AddComponent<LayoutElement>();
        fieldLE.flexibleWidth = 1;
        fieldLE.minHeight = 54;
        fieldLE.preferredHeight = 54;

        // Text area child required by TMP_InputField
        var textAreaGO = CreateRectObject(fieldGO, "Text Area");
        var textAreaRT = textAreaGO.GetComponent<RectTransform>();
        textAreaRT.anchorMin = Vector2.zero;
        textAreaRT.anchorMax = Vector2.one;
        textAreaRT.offsetMin = new Vector2(0, 4);
        textAreaRT.offsetMax = new Vector2(-4, -4);
        var textAreaMask = textAreaGO.AddComponent<RectMask2D>();

        var textGO = CreateRectObject(textAreaGO, "Text");
        var textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = textRT.offsetMax = Vector2.zero;
        var textComp = textGO.AddComponent<TextMeshProUGUI>();
        textComp.fontSize = 14;
        textComp.color = ColTextDark;
        textComp.alignment = TextAlignmentOptions.MidlineLeft;

        var phGO = CreateRectObject(textAreaGO, "Placeholder");
        var phRT = phGO.GetComponent<RectTransform>();
        phRT.anchorMin = Vector2.zero;
        phRT.anchorMax = Vector2.one;
        phRT.offsetMin = phRT.offsetMax = Vector2.zero;
        var phComp = phGO.AddComponent<TextMeshProUGUI>();
        phComp.text = placeholder;
        phComp.fontSize = 14;
        phComp.color = ColTextGrey;
        phComp.alignment = TextAlignmentOptions.MidlineLeft;
        phComp.fontStyle = FontStyles.Italic;

        fieldOut = fieldGO.AddComponent<TMP_InputField>();
        fieldOut.textComponent = textComp;
        fieldOut.placeholder = phComp;
        fieldOut.textViewport = textAreaRT;
        if (isPassword)
        {
            fieldOut.contentType = TMP_InputField.ContentType.Password;
            fieldOut.inputType = TMP_InputField.InputType.Password;
        }
        fieldOut.fontAsset = textComp.font;

        // Eye toggle (password only)
        if (isPassword)
        {
            var eyeBtn = CreateRectObject(container, "EyeToggle");
            eyeBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(24, 24);
            var eyeLE = eyeBtn.AddComponent<LayoutElement>();
            eyeLE.minWidth = 24;
            eyeLE.preferredWidth = 24;
            eyeLE.flexibleWidth = 0;
            var eyeTxt = CreateTMP(eyeBtn, "EyeIcon", "👁",
                16, FontStyles.Normal, ColTextGrey, TextAlignmentOptions.Center);
            eyeTxt.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            eyeTxt.GetComponent<RectTransform>().anchorMax = Vector2.one;
            eyeTxt.GetComponent<RectTransform>().offsetMin = eyeTxt.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            var eyeImage = eyeBtn.AddComponent<Image>();
            eyeImage.color = new Color(0, 0, 0, 0);
            var eyeButton = eyeBtn.AddComponent<Button>();
            eyeButton.targetGraphic = eyeImage;
            var capturedField = fieldOut;
            eyeButton.onClick.AddListener(() => TogglePassword(capturedField));
        }

        return container;
    }

    void TogglePassword(TMP_InputField field)
    {
        passwordVisible = !passwordVisible;
        field.contentType = passwordVisible
            ? TMP_InputField.ContentType.Standard
            : TMP_InputField.ContentType.Password;
        field.ForceLabelUpdate();
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  ROLE SELECTOR
    // ═════════════════════════════════════════════════════════════════════════
    GameObject BuildRoleSelector(GameObject parent)
    {
        var row = CreateRectObject(parent, "RoleSelector");
        var hl = row.AddComponent<HorizontalLayoutGroup>();
        hl.spacing = 10;
        hl.childAlignment = TextAnchor.MiddleCenter;
        hl.childControlWidth = true;
        hl.childControlHeight = true;
        hl.childForceExpandWidth = true;
        hl.childForceExpandHeight = false;

        string[] labels = { "Student", "Tutor", "Guardian" };
        string[] emojis = { "🎓", "📋", "👥" };
        Color[] iconColors = { ColPurpleMid, ColPurpleMid, ColTeal };

        for (int i = 0; i < 3; i++)
        {
            int idx = i;
            var panel = CreateImage(row, "Role_" + labels[i], ColWhite);
            rolePanels[i] = panel;
            AddRoundedCorners(panel, 14);

            var panelVL = panel.AddComponent<VerticalLayoutGroup>();
            panelVL.padding = new RectOffset(6, 6, 12, 12);
            panelVL.spacing = 6;
            panelVL.childAlignment = TextAnchor.MiddleCenter;
            panelVL.childControlWidth = true;
            panelVL.childControlHeight = false;
            panelVL.childForceExpandWidth = true;
            panelVL.childForceExpandHeight = false;

            var panelLE = panel.AddComponent<LayoutElement>();
            panelLE.minHeight = 90;

            // Emoji icon
            var iconTxt = CreateTMP(panel, "Icon", emojis[i],
                26, FontStyles.Normal, iconColors[i], TextAlignmentOptions.Center);
            SetLayoutElement(iconTxt, minHeight: 36);

            // Label
            var lbl = CreateTMP(panel, "Label", labels[i],
                12, FontStyles.Bold, ColTextDark, TextAlignmentOptions.Center);
            roleLabels[i] = lbl;
            SetLayoutElement(lbl, minHeight: 18);

            // Button
            var btn = panel.AddComponent<Button>();
            btn.targetGraphic = panel.GetComponent<Image>();
            btn.transition = Selectable.Transition.None;
            btn.onClick.AddListener(() => SelectRole(idx));
        }

        SelectRole(0);   // default: Student selected
        return row;
    }

    void SelectRole(int index)
    {
        selectedRole = index;
        for (int i = 0; i < 3; i++)
        {
            var img = rolePanels[i].GetComponent<Image>();
            if (i == selectedRole)
            {
                img.color = new Color(ColPurpleLight.r, ColPurpleLight.g, ColPurpleLight.b, 0.12f);
                var ol = rolePanels[i].GetComponent<Outline>() ?? rolePanels[i].AddComponent<Outline>();
                ol.effectColor = ColPurpleMid;
                ol.effectDistance = new Vector2(1.5f, -1.5f);
                roleLabels[i].color = ColPurpleMid;
            }
            else
            {
                img.color = ColWhite;
                var ol = rolePanels[i].GetComponent<Outline>();
                if (ol != null) ol.effectColor = ColInputBorder;
                roleLabels[i].color = ColTextDark;
            }
        }
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  LOGIN BUTTON
    // ═════════════════════════════════════════════════════════════════════════
    GameObject BuildLoginButton(GameObject parent)
    {
        var btn = CreateImage(parent, "LoginButton", ColBtnLogin);
        AddRoundedCorners(btn, 27);

        var hl = btn.AddComponent<HorizontalLayoutGroup>();
        hl.childAlignment = TextAnchor.MiddleCenter;
        hl.spacing = 8;
        hl.childForceExpandWidth = false;
        hl.childForceExpandHeight = false;

        var arrowTxt = CreateTMP(btn, "Arrow", "→",
            20, FontStyles.Bold, ColWhite, TextAlignmentOptions.Center);
        SetLayoutElement(arrowTxt, minHeight: 30, minWidth: 24);

        var lblTxt = CreateTMP(btn, "LoginLabel", "Login",
            18, FontStyles.Bold, ColWhite, TextAlignmentOptions.Center);
        SetLayoutElement(lblTxt, minHeight: 30);

        var button = btn.AddComponent<Button>();
        button.targetGraphic = btn.GetComponent<Image>();
        button.onClick.AddListener(OnLoginClicked);

        // Hover colour
        ColorBlock cb = button.colors;
        cb.normalColor = ColBtnLogin;
        cb.highlightedColor = ColPurpleDark;
        cb.pressedColor = ColPurpleDark;
        button.colors = cb;

        return btn;
    }

    void OnLoginClicked()
    {
        string[] roles = { "Student", "Tutor", "Guardian" };
        Debug.Log($"[SReader] Login pressed — Role: {roles[selectedRole]}");
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  OR DIVIDER
    // ═════════════════════════════════════════════════════════════════════════
    void BuildOrDivider(GameObject parent)
    {
        var row = CreateRectObject(parent, "OrDivider");
        SetLayoutElement(row, minHeight: 28);
        var hl = row.AddComponent<HorizontalLayoutGroup>();
        hl.spacing = 10;
        hl.childAlignment = TextAnchor.MiddleCenter;
        hl.childForceExpandWidth = true;
        hl.childForceExpandHeight = false;

        var lineL = CreateImage(row, "LineL", ColInputBorder);
        lineL.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 1);
        var lineLL = lineL.AddComponent<LayoutElement>();
        lineLL.flexibleWidth = 1;
        lineLL.preferredHeight = 1;
        lineLL.minHeight = 1;

        var orTxt = CreateTMP(row, "OrText", "OR",
            12, FontStyles.Normal, ColTextGrey, TextAlignmentOptions.Center);
        SetLayoutElement(orTxt, minHeight: 20, minWidth: 30);

        var lineR = CreateImage(row, "LineR", ColInputBorder);
        lineR.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 1);
        var lineRL = lineR.AddComponent<LayoutElement>();
        lineRL.flexibleWidth = 1;
        lineRL.preferredHeight = 1;
        lineRL.minHeight = 1;
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  SIGN UP ROW
    // ═════════════════════════════════════════════════════════════════════════
    void BuildSignUpRow(GameObject parent)
    {
        var row = CreateImage(parent, "SignUpRow", new Color(ColInputBg.r, ColInputBg.g, ColInputBg.b, 0.5f));
        AddRoundedCorners(row, 14);
        SetLayoutElement(row, minHeight: 50);

        var hl = row.AddComponent<HorizontalLayoutGroup>();
        hl.childAlignment = TextAnchor.MiddleCenter;
        hl.spacing = 4;
        hl.childForceExpandWidth = false;
        hl.childForceExpandHeight = false;

        var labelTxt = CreateTMP(row, "NoAccountLabel", "Don't have an account?",
            13, FontStyles.Normal, ColTextGrey, TextAlignmentOptions.Center);
        SetLayoutElement(labelTxt, minHeight: 22);

        var signUpBtn = CreateTMP(row, "SignUpBtn", "Sign Up",
            13, FontStyles.Bold, ColPurpleMid, TextAlignmentOptions.Center);
        SetLayoutElement(signUpBtn, minHeight: 22);
        var btnImg = signUpBtn.gameObject.AddComponent<Image>();
        btnImg.color = new Color(0, 0, 0, 0);
        var btn = signUpBtn.gameObject.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        btn.onClick.AddListener(() => Debug.Log("[SReader] Sign Up tapped"));
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  BACK BUTTON
    // ═════════════════════════════════════════════════════════════════════════
    void BuildBackButton(GameObject parent)
    {
        var btn = CreateImage(parent, "BackButton", new Color(1, 1, 1, 0.85f));
        AddRoundedCorners(btn, 28);
        SetLayoutElement(btn, minHeight: 56);
        var outline = btn.AddComponent<Outline>();
        outline.effectColor = new Color(ColPurpleMid.r, ColPurpleMid.g, ColPurpleMid.b, 0.3f);
        outline.effectDistance = new Vector2(1, -1);

        var hl = btn.AddComponent<HorizontalLayoutGroup>();
        hl.childAlignment = TextAnchor.MiddleCenter;
        hl.spacing = 8;
        hl.childForceExpandWidth = false;
        hl.childForceExpandHeight = false;

        var arrowTxt = CreateTMP(btn, "BackArrow", "←",
            18, FontStyles.Normal, ColTextDark, TextAlignmentOptions.Center);
        SetLayoutElement(arrowTxt, minHeight: 28, minWidth: 20);

        var backTxt = CreateTMP(btn, "BackLabel", "Back",
            16, FontStyles.Normal, ColTextDark, TextAlignmentOptions.Center);
        SetLayoutElement(backTxt, minHeight: 28);

        var button = btn.AddComponent<Button>();
        button.targetGraphic = btn.GetComponent<Image>();
        button.onClick.AddListener(() => Debug.Log("[SReader] Back tapped"));
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  HELPERS
    // ═════════════════════════════════════════════════════════════════════════

    Canvas CreateCanvas()
    {
        var go = new GameObject("SReaderLoginCanvas");
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        var scaler = go.GetComponent<CanvasScaler>();
        scaler.referenceResolution = new Vector2(390, 844);  // iPhone 14 reference
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    GameObject CreateRectObject(GameObject parent, string name)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);
        return go;
    }

    GameObject CreateImage(GameObject parent, string name, Color color)
    {
        var go = CreateRectObject(parent, name);
        var img = go.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = true;
        return go;
    }

    TextMeshProUGUI CreateTMP(GameObject parent, string name, string text,
        float fontSize, FontStyles style, Color color, TextAlignmentOptions align)
    {
        var go = CreateRectObject(parent, name);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.color = color;
        tmp.alignment = align;
        tmp.richText = true;
        tmp.textWrappingMode = TextWrappingModes.Normal;
        return tmp;
    }

    void StretchFill(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    void SetLayoutElement(GameObject go, float minHeight = -1, float minWidth = -1,
        float preferredHeight = -1, float flexibleWidth = -1)
    {
        var le = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
        if (minHeight >= 0)       le.minHeight       = minHeight;
        if (minWidth >= 0)        le.minWidth        = minWidth;
        if (preferredHeight >= 0) le.preferredHeight = preferredHeight;
        if (flexibleWidth >= 0)   le.flexibleWidth   = flexibleWidth;
    }

    void SetLayoutElement(TextMeshProUGUI tmp, float minHeight = -1, float minWidth = -1)
        => SetLayoutElement(tmp.gameObject, minHeight, minWidth);

    void AddSpacerElement(GameObject parent, float height)
    {
        var spacer = CreateRectObject(parent, "Spacer_" + height);
        var le = spacer.AddComponent<LayoutElement>();
        le.minHeight = height;
        le.preferredHeight = height;
    }

    /// <summary>Approximate rounded corners by setting the sprite to Unity's built-in rounded rect.
    /// In a real project replace with a custom 9-slice sprite.</summary>
    void AddRoundedCorners(GameObject go, float radius)
    {
        var img = go.GetComponent<Image>();
        if (img == null) return;
        // Use Unity's default UI sprite (already round-cornered) —
        // for pixel-perfect rounding, assign a 9-sliced sprite here.
        img.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        img.type = Image.Type.Sliced;
    }

    static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
