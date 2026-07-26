using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.UI.Views.Home
{
    /// <summary>
    /// A design-system dropdown that replaces Unity's DropdownField: a dark,
    /// tappable field that opens a styled popup list. The popup scrolls by drag /
    /// wheel with NO visible scrollbars, and everything uses the app palette so
    /// the value and items are always legible (the native control rendered
    /// near-white text on its light popup). Exposes a string <see cref="value"/>
    /// just like DropdownField, so call sites read it unchanged.
    /// </summary>
    internal sealed class HomeDropdown : VisualElement
    {
        readonly List<string> choices;
        readonly Button field;
        readonly Label valueLabel;
        VisualElement blocker;

        string current;

        /// <summary>Invoked with the new value when the user picks an item.</summary>
        public Action<string> OnChanged;

        public string value
        {
            get => current;
            set
            {
                current = value;
                valueLabel.text = string.IsNullOrEmpty(value) ? "Select…" : value;
            }
        }

        public HomeDropdown(IEnumerable<string> options, string initial = null)
        {
            choices = new List<string>(options ?? Array.Empty<string>());
            current = initial ?? (choices.Count > 0 ? choices[0] : null);

            style.marginBottom = 6;

            field = new Button(Toggle);
            field.style.height = 42;
            field.style.flexDirection = FlexDirection.Row;
            field.style.alignItems = Align.Center;
            field.style.backgroundColor = HomeUI.Bg;
            HomeUI.Radius(field, 10);
            HomeUI.Border(field, HomeUI.Hairline, 1);
            field.style.paddingLeft = field.style.paddingRight = 10;
            field.style.marginLeft = field.style.marginRight = 0;

            valueLabel = new Label(string.IsNullOrEmpty(current) ? "Select…" : current);
            valueLabel.style.color = HomeUI.Text;
            valueLabel.style.fontSize = 15;
            valueLabel.style.flexGrow = 1;
            valueLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            field.Add(valueLabel);

            var arrow = new Label("▾");
            arrow.style.color = HomeUI.Muted;
            arrow.style.fontSize = 14;
            field.Add(arrow);

            Add(field);
        }

        void Toggle()
        {
            if (blocker != null) Close();
            else Open();
        }

        void Open()
        {
            // Topmost element so the popup floats above the page content.
            var root = (VisualElement)this;
            while (root.parent != null) root = root.parent;

            // Full-screen transparent catcher: a tap outside the list closes it.
            blocker = new VisualElement();
            blocker.style.position = Position.Absolute;
            blocker.style.left = 0;
            blocker.style.right = 0;
            blocker.style.top = 0;
            blocker.style.bottom = 0;
            blocker.RegisterCallback<PointerDownEvent>(_ => Close());

            var fb = field.worldBound;
            var popup = new VisualElement();
            popup.style.position = Position.Absolute;
            popup.style.left = fb.x;
            popup.style.top = fb.yMax + 2;
            popup.style.width = fb.width;
            popup.style.maxHeight = 260;
            popup.style.backgroundColor = HomeUI.Panel;
            HomeUI.Radius(popup, 12);
            HomeUI.Border(popup, HomeUI.Hairline, 1);
            popup.style.overflow = Overflow.Hidden;
            popup.style.paddingTop = popup.style.paddingBottom = 4;
            // Taps inside the list must not bubble out to the blocker.
            popup.RegisterCallback<PointerDownEvent>(e => e.StopPropagation());

            // Vertical-only scroll, scrollbars hidden (drag / wheel to scroll).
            var scroll = new ScrollView(ScrollViewMode.Vertical);
            scroll.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            scroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            scroll.style.maxHeight = 252;
            popup.Add(scroll);

            foreach (var choice in choices)
            {
                var captured = choice;
                var item = new Button(() => { value = captured; OnChanged?.Invoke(captured); Close(); }) { text = captured };
                item.style.height = 40;
                item.style.backgroundColor = Color.clear;
                item.style.color = current == captured ? HomeUI.Parchment : HomeUI.Text;
                item.style.fontSize = 15;
                item.style.unityTextAlign = TextAnchor.MiddleLeft;
                item.style.unityFontStyleAndWeight = current == captured ? FontStyle.Bold : FontStyle.Normal;
                HomeUI.Border(item, Color.clear, 0);
                item.style.marginTop = item.style.marginBottom = 0;
                item.style.paddingLeft = 12;
                scroll.Add(item);
            }

            blocker.Add(popup);
            root.Add(blocker);
        }

        void Close()
        {
            blocker?.RemoveFromHierarchy();
            blocker = null;
        }
    }
}
