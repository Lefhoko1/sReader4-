using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// A surface in the house (the chalkboard's slate, the dashboard's display) used as a screen for
    /// UI Toolkit pages. Example — the chalkboard: the app's real pages. The page (a UIDocument with its
    /// existing UXML and controller — StudentHomeReal + StudentHomeController, wired to the real
    /// ViewModels by the house's AppCompositionRoot) renders into a texture shown on the slate,
    /// and taps on the slate are turned into the page's own pointer positions — so the student
    /// uses Academies, Assignments, Tutors and Friends right on the chalkboard.
    ///
    /// Presentation only: nothing here loads data or decides anything; the controllers do.
    /// The page is only live while the board is open (one render target, no cost otherwise).
    /// </summary>
    [DefaultExecutionOrder(-300)]   // the page's panel must be pointed at the texture before it enables
    public class WorldUIScreen : MonoBehaviour
    {
        [Tooltip("The chalkboard's slate (its world bounds give the screen's size and facing).")]
        public Renderer slate;
        [Tooltip("The app page shown on the board (inactive until the board opens).")]
        public UIDocument page;
        public Camera viewCamera;
        [Tooltip("More app pages drawn on the board (e.g. Profile). The app's navigation shows and hides them; they sit above the home page.")]
        public UIDocument[] extraPages = new UIDocument[0];
        [Tooltip("Chalk text shown on the board while the screen is off.")]
        public TextMeshPro chalkText;
        [Tooltip("The room centre: the screen faces it.")]
        public Transform roomCentre;

        [Header("Resolution")]
        public int textureWidth = 1600;
        [Tooltip("Width the page is laid out at, in UI pixels (the board is a wide 'screen').")]
        public float layoutWidth = 860f;
        [Range(0.8f, 1f)] public float fill = 0.97f;
        [Tooltip("Draw only what the page draws (clear background): the surface underneath shows through.")]
        public bool transparent;

        /// <summary>True for the chalkboard: a tap on its surface opens the board.</summary>
        protected virtual bool MarkAsBoard => true;

        public bool IsOpen { get; private set; }
        public Vector3 Centre { get; private set; }
        public Vector3 Normal { get; private set; }
        public Vector2 Size { get; private set; }

        RenderTexture target;
        PanelSettings panel;
        MeshRenderer quad;
        MeshCollider surface;

        /// <summary>
        /// Where the screen is: by default from the slate (its thinnest horizontal axis is the wall normal, facing
        /// the room). Screens that are not on a wall (e.g. a board leaning on the floor) say so themselves.
        /// </summary>
        protected virtual void Measure(out Vector3 normal, out Vector2 size, out Vector3 centre)
        {
            var b = slate.bounds;
            bool thinX = b.extents.x < b.extents.z;
            Vector3 n = thinX ? Vector3.right : Vector3.forward;
            if (roomCentre != null && Vector3.Dot(roomCentre.position - b.center, n) < 0f) n = -n;
            normal = n;
            size = new Vector2(2f * (thinX ? b.extents.z : b.extents.x), 2f * b.extents.y) * fill;
            centre = b.center + n * ((thinX ? b.extents.x : b.extents.z) + 0.012f);
        }

        protected virtual void Awake()
        {
            Measure(out var n, out var size, out var centre);
            Normal = n; Size = size; Centre = centre;

            transform.SetPositionAndRotation(Centre, Quaternion.LookRotation(-n, Vector3.up));   // the quad's face looks into the room
            transform.localScale = new Vector3(Size.x, Size.y, 1f);

            var mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
            gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            quad = gameObject.AddComponent<MeshRenderer>();
            quad.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            quad.receiveShadows = false;
            surface = gameObject.AddComponent<MeshCollider>();
            surface.sharedMesh = mesh;
            if (MarkAsBoard) gameObject.AddComponent<AssignmentsBoardTarget>();   // a tap on the board opens it

            int h = Mathf.RoundToInt(textureWidth * Size.y / Size.x);
            target = new RenderTexture(textureWidth, h, 24, RenderTextureFormat.ARGB32) { name = "Chalkboard Screen", antiAliasing = 1 };
            target.Create();
            var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Texture"));
            mat.mainTexture = target;
            if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", target);
            if (transparent && mat.HasProperty("_Surface"))
            {
                // UI Toolkit writes premultiplied colour into the texture: blend it as such.
                mat.SetFloat("_Surface", 1f); mat.SetFloat("_Blend", 1f);
                mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetFloat("_ZWrite", 0f);
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            quad.sharedMaterial = mat;
            quad.enabled = false;

            // A private copy of the page's panel settings, drawing into the board's texture.
            panel = Instantiate(page.panelSettings);
            panel.name = "Chalkboard Panel";
            panel.targetTexture = target;
            panel.scaleMode = PanelScaleMode.ConstantPixelSize;
            panel.scale = textureWidth / layoutWidth;
            panel.clearColor = true;
            panel.colorClearValue = transparent ? new Color(0f, 0f, 0f, 0f) : new Color(18f / 255f, 20f / 255f, 25f / 255f, 1f);
            panel.SetScreenToPanelSpaceFunction(ScreenToBoard);
            page.panelSettings = panel;
            page.gameObject.SetActive(false);
            foreach (var extra in extraPages)
                if (extra != null) { extra.panelSettings = panel; extra.sortingOrder = page.sortingOrder + 1; }
        }

        void OnDestroy()
        {
            if (target != null) target.Release();
        }

        public virtual void Open()
        {
            if (IsOpen) return;
            IsOpen = true;
            page.gameObject.SetActive(true);
            quad.enabled = true;
            if (chalkText != null) chalkText.enabled = false;
        }

        public virtual void Close()
        {
            if (!IsOpen) return;
            IsOpen = false;
            page.gameObject.SetActive(false);
            quad.enabled = false;
            if (chalkText != null) chalkText.enabled = true;
        }

        /// <summary>Where an element of the page appears on screen (bottom-left origin), e.g. for automated taps.</summary>
        public Vector2 ScreenPointOf(VisualElement element)
        {
            var c = element.worldBound.center * panel.scale;                     // page units → texture pixels
            var local = new Vector3(c.x / target.width - 0.5f, 0.5f - c.y / target.height, 0f);
            return viewCamera.WorldToScreenPoint(transform.TransformPoint(local));
        }

        /// <summary>Screen → the page's pixel space, through the point tapped on the board (NaN = not on the board).</summary>
        /// <summary>Last pointer position handed to the board and where it landed (diagnostics).</summary>
        public Vector2 LastIn { get; private set; }
        public Vector2 LastOut { get; private set; }

        Vector2 ScreenToBoard(Vector2 screenPosition)
        {
            LastIn = screenPosition;
            var result = MapToBoard(screenPosition);
            LastOut = result;
            return result;
        }

        Vector2 MapToBoard(Vector2 screenPosition)
        {
            var off = new Vector2(float.NaN, float.NaN);
            if (!IsOpen || viewCamera == null) return off;
            screenPosition.y = Screen.height - screenPosition.y;          // UI Toolkit hands over top-left based coordinates
            if (!surface.Raycast(viewCamera.ScreenPointToRay(screenPosition), out var hit, 100f)) return off;
            var uv = hit.textureCoord;
            return new Vector2(uv.x * target.width, (1f - uv.y) * target.height);   // texture pixels; the panel applies its own scale
        }
    }
}
