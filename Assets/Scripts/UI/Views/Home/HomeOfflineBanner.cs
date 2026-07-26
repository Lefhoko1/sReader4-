using SReader.Core.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.UI.Views.Home
{
    /// <summary>
    /// The thin status bar shared by every role home. It shows one of:
    ///  • red  — offline storage is unavailable on this device (no local copy at all);
    ///  • amber — you're offline and viewing saved (cached) data;
    ///  • hidden — online with working storage.
    /// Build it once, insert it at the top of the page, and call <see cref="Refresh"/>
    /// on a timer.
    /// </summary>
    internal static class HomeOfflineBanner
    {
        const string LabelName = "offline-banner-label";

        static Color Rgb(int r, int g, int b) => new Color(r / 255f, g / 255f, b / 255f);

        public static VisualElement Build()
        {
            var bar = new VisualElement { name = "offline-banner" };
            bar.style.display = DisplayStyle.None;
            bar.style.flexDirection = FlexDirection.Row;
            bar.style.justifyContent = Justify.Center;
            bar.style.alignItems = Align.Center;
            bar.style.paddingTop = bar.style.paddingBottom = 6;
            bar.style.paddingLeft = bar.style.paddingRight = 10;

            var label = new Label { name = LabelName };
            label.style.color = Rgb(242, 239, 230);
            label.style.fontSize = 13;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.whiteSpace = WhiteSpace.Normal;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            bar.Add(label);
            return bar;
        }

        public static void Refresh(VisualElement banner, IConnectivity connectivity, OfflineStatus status)
        {
            if (banner == null) return;
            var label = banner.Q<Label>(LabelName);

            bool online = connectivity == null || connectivity.IsOnline;
            bool storageReady = status == null || status.StorageReady;

            // Offline storage failed to start → no local copy exists; surface it
            // always (even online) so the user knows data needs a connection here.
            if (!storageReady)
            {
                var text = "⚠  Offline storage unavailable — data needs a connection";
                // Surface the real reason (e.g. the native-load error) for on-device diagnosis.
                var detail = status?.Detail;
                if (!string.IsNullOrEmpty(detail) && detail != "ready" && detail != "disabled")
                    text += "\n(" + detail + ")";
                if (label != null) label.text = text;
                banner.style.backgroundColor = Rgb(150, 54, 54); // red
                banner.style.display = DisplayStyle.Flex;
                return;
            }

            if (!online)
            {
                if (label != null) label.text = "⚠  Offline — showing saved data";
                banner.style.backgroundColor = Rgb(122, 88, 24); // amber
                banner.style.display = DisplayStyle.Flex;
                return;
            }

            banner.style.display = DisplayStyle.None;
        }
    }
}
