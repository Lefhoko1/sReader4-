using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.UI.Bindings
{
    /// <summary>
    /// Game-style horizontal pagination for UI Toolkit pages — one screen
    /// of content at a time, no scrolling. Expects these element names in
    /// the page UXML:
    ///     pager-pages   container whose children are the pages
    ///     pager-prev    "‹" button (hidden on the first page)
    ///     pager-next    "›" button (hidden on the last page)
    ///     pager-dots    row that receives one dot per page (built here)
    /// The UIDocument rebuilds its visual tree every time the page
    /// GameObject is re-activated, so create a fresh pager from OnEnable.
    /// </summary>
    public class UIPager
    {
        static readonly Color DotActive = new Color32(233, 223, 200, 255); // parchment
        static readonly Color DotIdle   = new Color32(60, 66, 82, 255);

        readonly VisualElement pages;
        readonly Button prev;
        readonly Button next;
        readonly VisualElement dots;
        readonly Func<int, bool> beforeNext;
        readonly Action<int> onPageChanged;
        int current;

        /// <param name="beforeNext">
        /// Called with the current page index when "›" is pressed; return
        /// false to block the move (e.g. a form step that failed validation).
        /// </param>
        public static UIPager Setup(VisualElement root, MonoBehaviour context,
                                    Func<int, bool> beforeNext = null,
                                    Action<int> onPageChanged = null)
        {
            var pages = root?.Q<VisualElement>("pager-pages");
            if (pages == null || pages.childCount == 0)
            {
                Debug.LogWarning($"[{context.name}] 'pager-pages' not found or has no pages — pagination disabled.", context);
                return null;
            }
            return new UIPager(root, pages, beforeNext, onPageChanged);
        }

        UIPager(VisualElement root, VisualElement pages,
                Func<int, bool> beforeNext, Action<int> onPageChanged)
        {
            this.pages = pages;
            this.beforeNext = beforeNext;
            this.onPageChanged = onPageChanged;

            prev = root.Q<Button>("pager-prev");
            next = root.Q<Button>("pager-next");
            dots = root.Q<VisualElement>("pager-dots");

            if (prev != null) prev.clicked += () => Show(current - 1);
            if (next != null) next.clicked += () =>
            {
                if (this.beforeNext == null || this.beforeNext(current))
                    Show(current + 1);
            };

            BuildDots();
            Show(0);
        }

        void BuildDots()
        {
            if (dots == null) return;
            dots.Clear();
            for (int i = 0; i < pages.childCount; i++)
            {
                var dot = new VisualElement();
                dot.style.width  = 8;
                dot.style.height = 8;
                dot.style.borderTopLeftRadius     = 4;
                dot.style.borderTopRightRadius    = 4;
                dot.style.borderBottomLeftRadius  = 4;
                dot.style.borderBottomRightRadius = 4;
                dot.style.marginLeft  = 5;
                dot.style.marginRight = 5;
                dots.Add(dot);
            }
        }

        public void Show(int index)
        {
            current = Mathf.Clamp(index, 0, pages.childCount - 1);

            for (int i = 0; i < pages.childCount; i++)
                pages[i].style.display = i == current ? DisplayStyle.Flex : DisplayStyle.None;

            if (dots != null)
                for (int i = 0; i < dots.childCount; i++)
                    dots[i].style.backgroundColor = i == current ? DotActive : DotIdle;

            // visibility (not display) so the nav row never shifts sideways.
            SetArrow(prev, current > 0);
            SetArrow(next, current < pages.childCount - 1);

            onPageChanged?.Invoke(current);
        }

        static void SetArrow(Button arrow, bool usable)
        {
            if (arrow == null) return;
            arrow.visible = usable;
            arrow.SetEnabled(usable);
        }
    }
}
