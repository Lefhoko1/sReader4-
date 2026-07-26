using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.UI.Views.Home
{
    /// <summary>
    /// Loads decorative game art (backdrops, sprite-sheet mascots) from
    /// <c>Assets/Resources/Art</c> at runtime and builds UI Toolkit elements for
    /// it. Kept separate from <see cref="HomeUI"/> (which owns the design-system
    /// widgets) so the "make it feel like a game" art is in one place and easy to
    /// swap. All loads are cached, and missing art degrades to nothing rather than
    /// throwing — the dashboard must still render if a file isn't imported yet.
    /// </summary>
    internal static class HomeArt
    {
        // Resources paths (no extension, forward slashes) — files copied into
        // Assets/Resources/Art so they load in a player build, not just the editor.
        public const string HomeBackground = "Art/home_bg";
        public const string MascotBoyFolder = "Art/MascotBoy";

        static readonly Dictionary<string, Texture2D> TexCache = new Dictionary<string, Texture2D>();
        static readonly Dictionary<string, Texture2D[]> FramesCache = new Dictionary<string, Texture2D[]>();

        /// <summary>A single texture from Resources, or null if it isn't there.</summary>
        public static Texture2D Texture(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath)) return null;
            if (TexCache.TryGetValue(resourcePath, out var cached)) return cached;
            var tex = Resources.Load<Texture2D>(resourcePath);
            TexCache[resourcePath] = tex;   // cache nulls too, so a miss is looked up once
            return tex;
        }

        /// <summary>
        /// All frames of a sprite-sheet folder, ordered by file name
        /// (sprite_00, sprite_01, …). Empty array if the folder is missing.
        /// </summary>
        public static Texture2D[] Frames(string folderResourcePath)
        {
            if (string.IsNullOrEmpty(folderResourcePath)) return System.Array.Empty<Texture2D>();
            if (FramesCache.TryGetValue(folderResourcePath, out var cached)) return cached;

            var frames = Resources.LoadAll<Texture2D>(folderResourcePath);
            if (frames == null) frames = System.Array.Empty<Texture2D>();
            System.Array.Sort(frames, (a, b) => string.CompareOrdinal(a.name, b.name));
            FramesCache[folderResourcePath] = frames;
            return frames;
        }

        /// <summary>
        /// A full-bleed background layer for a screen: the image scaled to cover,
        /// with a dark scrim on top so the design-system cards and text stay
        /// legible. Returns a single element you drop in behind the content (it
        /// ignores pointer input). <paramref name="scrim"/> 0 = image only,
        /// 1 = fully dark; tune to taste.
        /// </summary>
        public static VisualElement Backdrop(string resourcePath = HomeBackground, float scrim = 0.82f)
        {
            var layer = new VisualElement();
            layer.style.position = Position.Absolute;
            layer.style.left = 0; layer.style.right = 0; layer.style.top = 0; layer.style.bottom = 0;
            layer.pickingMode = PickingMode.Ignore;

            var tex = Texture(resourcePath);
            if (tex != null)
            {
                layer.style.backgroundImage = new StyleBackground(tex);
                HomeUI.CoverBackground(layer);
            }

            // Dark veil keeps the dark theme readable over a light backdrop.
            var veil = new VisualElement();
            veil.style.position = Position.Absolute;
            veil.style.left = 0; veil.style.right = 0; veil.style.top = 0; veil.style.bottom = 0;
            veil.pickingMode = PickingMode.Ignore;
            veil.style.backgroundColor = new Color(18 / 255f, 20 / 255f, 25 / 255f, Mathf.Clamp01(scrim));
            layer.Add(veil);
            return layer;
        }

        /// <summary>
        /// A looping sprite-sheet animation as a VisualElement. Cycles its
        /// background image through <paramref name="frames"/> at <paramref name="fps"/>
        /// using the panel scheduler (so it pauses when the element leaves the
        /// panel). The element ignores pointer input. Sized to the given height,
        /// width auto from aspect. Returns an empty element if no frames loaded.
        /// </summary>
        public static VisualElement AnimatedSprite(Texture2D[] frames, float height = 64f, float fps = 12f)
        {
            var el = new VisualElement();
            el.pickingMode = PickingMode.Ignore;
            el.style.height = height;
            HomeUI.ContainBackground(el);

            if (frames == null || frames.Length == 0) return el;

            // Width from the first frame's aspect ratio so the boy isn't stretched.
            var first = frames[0];
            if (first != null && first.height > 0)
                el.style.width = height * (first.width / (float)first.height);

            el.style.backgroundImage = new StyleBackground(frames[0]);

            // Only start ticking once attached to a panel, and stop on detach.
            IVisualElementScheduledItem ticker = null;
            int frame = 0;
            int periodMs = Mathf.Max(1, Mathf.RoundToInt(1000f / Mathf.Max(1f, fps)));

            el.RegisterCallback<AttachToPanelEvent>(_ =>
            {
                ticker = el.schedule.Execute(() =>
                {
                    frame = (frame + 1) % frames.Length;
                    var t = frames[frame];
                    if (t != null) el.style.backgroundImage = new StyleBackground(t);
                }).Every(periodMs);
            });
            el.RegisterCallback<DetachFromPanelEvent>(_ => ticker?.Pause());

            return el;
        }

        /// <summary>Convenience: the reading-boy mascot, ready to drop in.</summary>
        public static VisualElement MascotBoy(float height = 64f, float fps = 12f)
            => AnimatedSprite(Frames(MascotBoyFolder), height, fps);
    }
}
