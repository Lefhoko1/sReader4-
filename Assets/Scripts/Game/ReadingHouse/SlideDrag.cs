using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// Grab-and-slide for pages on the house's screens: press anywhere on the element and drag it up or
    /// down; let go and it glides on with the flick's speed, slowing naturally, and eases back softly if
    /// it was pulled past an end. A short press still reaches the buttons under the finger (sliding only
    /// starts once the finger has really moved, then the press is taken from the button).
    ///
    ///   • <see cref="ForSheet"/>: the whole activity sheet slides inside its frame (nothing is clipped
    ///     into a little scrolling box).
    ///   • <see cref="ForScrollView"/>: a page's ScrollView follows a mouse / pen drag (touch already drags
    ///     ScrollViews natively).
    /// Presentation only.
    /// </summary>
    public sealed class SlideDrag : PointerManipulator
    {
        readonly Func<float> getOffset;
        readonly Action<float> setOffset;
        readonly Func<Vector2> range;          // (min, max) offset
        readonly bool mouseOnly;

        const float StartDistance = 10f;       // panel px before a press becomes a slide
        const float Friction = 3.2f;           // glide slows by e^-Friction per second
        const float EdgePull = 0.35f;          // past an end the page follows the finger at this rate
        const float SpringTime = 0.16f;

        int pointerId = -1;
        bool sliding;
        Vector2 pressAt;
        float startOffset, velocity, lastPointer, lastTime, springVelocity;
        IVisualElementScheduledItem glide;

        SlideDrag(Func<float> get, Action<float> set, Func<Vector2> range, bool mouseOnly)
        {
            getOffset = get; setOffset = set; this.range = range; this.mouseOnly = mouseOnly;
        }

        /// <summary>The whole <paramref name="sheet"/> slides vertically inside <paramref name="frame"/>.</summary>
        public static SlideDrag ForSheet(VisualElement sheet, VisualElement frame)
        {
            float offset = 0f;
            var drag = new SlideDrag(
                () => offset,
                v => { offset = v; sheet.style.translate = new Translate(0, v); },
                () =>
                {
                    // However the frame aligns it: a sheet taller than the frame may slide until either end shows.
                    const float pad = 12f;
                    float top = sheet.layout.y, bottom = top + sheet.layout.height, h = frame.layout.height;
                    if (float.IsNaN(top) || bottom - top <= h - 2f * pad) return Vector2.zero;
                    return new Vector2(h - pad - bottom, pad - top);
                },
                false) { target = sheet };
            return drag;
        }

        /// <summary>The ScrollView's content follows a mouse / pen drag.</summary>
        public static SlideDrag ForScrollView(ScrollView view)
        {
            return new SlideDrag(
                () => -view.scrollOffset.y,
                v => view.scrollOffset = new Vector2(view.scrollOffset.x, -v),
                () => new Vector2(-Mathf.Max(0f, view.contentContainer.layout.height - view.contentViewport.layout.height), 0f),
                true) { target = view };
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnDown, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerMoveEvent>(OnMove, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerUpEvent>(OnUp, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerCaptureOutEvent>(OnCaptureOut);
            glide = target.schedule.Execute(Glide).Every(16);
            glide.Pause();
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnDown, TrickleDown.TrickleDown);
            target.UnregisterCallback<PointerMoveEvent>(OnMove, TrickleDown.TrickleDown);
            target.UnregisterCallback<PointerUpEvent>(OnUp, TrickleDown.TrickleDown);
            target.UnregisterCallback<PointerCaptureOutEvent>(OnCaptureOut);
            glide?.Pause();
        }

        bool Accepts(IPointerEvent e) => !mouseOnly || e.pointerType == UnityEngine.UIElements.PointerType.mouse || e.pointerType == UnityEngine.UIElements.PointerType.pen;

        void OnDown(PointerDownEvent e)
        {
            if (pointerId >= 0 || !Accepts(e) || e.button > 0) return;
            pointerId = e.pointerId;
            pressAt = e.position;
            startOffset = getOffset();
            lastPointer = e.position.y; lastTime = Time.unscaledTime;
            velocity = 0f; sliding = false;
            glide.Pause();
        }

        void OnMove(PointerMoveEvent e)
        {
            if (e.pointerId != pointerId) return;
            if (!sliding)
            {
                if (Mathf.Abs(e.position.y - pressAt.y) < StartDistance) return;
                sliding = true;
                target.CapturePointer(pointerId);         // the button under the finger lets go: this is a slide
            }
            float raw = startOffset + (e.position.y - pressAt.y);
            setOffset(Soften(raw));
            float now = Time.unscaledTime, dt = Mathf.Max(0.001f, now - lastTime);
            velocity = Mathf.Lerp(velocity, (e.position.y - lastPointer) / dt, 0.5f);
            lastPointer = e.position.y; lastTime = now;
            e.StopPropagation();
        }

        void OnUp(PointerUpEvent e)
        {
            if (e.pointerId != pointerId) return;
            bool wasSliding = sliding;
            Release();
            if (wasSliding) e.StopPropagation();
        }

        void OnCaptureOut(PointerCaptureOutEvent e)
        {
            if (sliding) Release();
        }

        void Release()
        {
            if (target.HasPointerCapture(pointerId)) target.ReleasePointer(pointerId);
            pointerId = -1;
            if (Time.unscaledTime - lastTime > 0.08f) velocity = 0f;   // held still before letting go: no fling
            sliding = false;
            springVelocity = 0f;
            glide.Resume();
        }

        /// <summary>Past an end the page follows the finger less and less (a soft edge, not a wall).</summary>
        float Soften(float raw)
        {
            var r = range();
            if (raw > r.y) return r.y + (raw - r.y) * EdgePull;
            if (raw < r.x) return r.x + (raw - r.x) * EdgePull;
            return raw;
        }

        void Glide()
        {
            if (pointerId >= 0) return;
            float dt = 0.016f;
            var r = range();
            float o = getOffset();
            if (o > r.y || o < r.x)
            {
                float goal = o > r.y ? r.y : r.x;
                velocity = 0f;
                o = Mathf.SmoothDamp(o, goal, ref springVelocity, SpringTime, Mathf.Infinity, dt);
                if (Mathf.Abs(o - goal) < 0.5f) { o = goal; glide.Pause(); }
                setOffset(o);
                return;
            }
            if (Mathf.Abs(velocity) < 8f) { glide.Pause(); return; }
            o += velocity * dt;
            velocity *= Mathf.Exp(-Friction * dt);
            if (o > r.y || o < r.x) { o = Mathf.Clamp(o, r.x - 40f, r.y + 40f); springVelocity = 0f; }   // glided into an end: a small overshoot, then it settles back
            setOffset(o);
        }
    }
}
