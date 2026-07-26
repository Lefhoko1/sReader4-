using System;
using System.Collections.Generic;

namespace SReader.UI.Navigation
{
    /// <summary>
    /// A lightweight back stack for in-content screen navigation. Each entry is a
    /// closure that re-renders one screen (it clears and rebuilds the content
    /// area). <see cref="Push"/> to go deeper; <see cref="Back"/> pops the current
    /// screen and re-renders the previous one — so "‹ Back" and the Android system
    /// back button return to exactly where the user was, including the sub-tab and
    /// data they had open, and can step back through the whole trail.
    ///
    /// Pure C# (no Unity dependency) so it can be unit-tested and reused by any
    /// view that renders one screen at a time into a container.
    /// </summary>
    public sealed class NavStack
    {
        readonly List<Action> entries = new List<Action>();

        /// <summary>Number of screens currently on the stack.</summary>
        public int Count => entries.Count;

        /// <summary>True when there is a previous screen to return to.</summary>
        public bool CanGoBack => entries.Count > 1;

        /// <summary>
        /// Clear the stack and make <paramref name="render"/> the root screen
        /// (the one a back press can never go below). Renders it immediately.
        /// </summary>
        public void Root(Action render)
        {
            entries.Clear();
            if (render == null) return;
            entries.Add(render);
            render();
        }

        /// <summary>
        /// Make <paramref name="render"/> the root <em>without</em> rendering it —
        /// the caller is expected to <see cref="Push"/> the visible screen next.
        /// Used to seed a back target (e.g. a list) when jumping straight to a
        /// deeper screen, so going back renders the list only when it's needed.
        /// </summary>
        public void RootSilent(Action render)
        {
            entries.Clear();
            if (render != null) entries.Add(render);
        }

        /// <summary>Go one screen deeper and render it.</summary>
        public void Push(Action render)
        {
            if (render == null) return;
            entries.Add(render);
            render();
        }

        /// <summary>
        /// Pop the current screen and re-render the previous one. Returns
        /// <c>false</c> when already at the root (nothing to go back to), so the
        /// caller can decide what a back press means at the top level (e.g. switch
        /// section, or confirm-to-exit).
        /// </summary>
        public bool Back()
        {
            if (entries.Count <= 1) return false;
            entries.RemoveAt(entries.Count - 1);
            entries[entries.Count - 1].Invoke();
            return true;
        }

        /// <summary>Forget all history (e.g. when the owning view is re-shown fresh).</summary>
        public void Clear() => entries.Clear();
    }
}
