using System;
using System.Collections.Generic;

using Bridge.Html5;


/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/Touch_events demo written in Bridge C#
/// </summary>
namespace TouchSample
{
    public class TouchSample
    {
        #region Html element on the page

        private HTMLCanvasElement canvas;
        public HTMLCanvasElement Canvas
        {
            get { return canvas; }
            set
            {
                canvas = value;

                if (canvas == null)
                {
                    context2D = null;
                }
                else
                {
                    context2D = (CanvasRenderingContext2D)canvas.GetContext("2d");
                }
            }
        }

        private CanvasRenderingContext2D context2D;
        public CanvasRenderingContext2D Context2D { get { return context2D; } }

        public HTMLPreElement Log { get; set; }

        #endregion Html element on the page

        public List<TouchShadow> OngoingTouches { get; set; }

        [Ready]
        public static void Start()
        {
            new TouchSample().Initialize();
        }

        public void Initialize()
        {
            this.OngoingTouches = new List<TouchShadow>();

            this.Canvas = Document.GetElementById<HTMLCanvasElement>("canvas");

            this.Log = Document.GetElementById<HTMLPreElement>("log");

            this.Canvas.OnClick += ClickHandler;
            this.Canvas.OnTouchStart += TouchStartHandler;
            this.Canvas.OnTouchMove  += TouchMoveHandler;
            this.Canvas.OnTouchEnd += TouchEndHandler;
            this.Canvas.OnTouchCancel += TouchCancelHandler;

            this.LogMessage("Initialized.");
        }

        #region Events

        private void ClickHandler(MouseEvent<HTMLCanvasElement> e)
        {
            this.LogMessage("ClickHandler.");
        }

        private void TouchStartHandler(TouchEvent<HTMLCanvasElement> evt)
        {
            var ctx = this.Context2D;
            var touches = evt.Touches;

            for (var i = 0; i < touches.Length; i++)
            {
                LogMessage("touchstart:" + i + "...");

                var t = touches[i];

                this.OngoingTouches.Add(this.CopyTouch(t));

                var color = this.ColorForTouch(t, t.Identifier + " start idx " + (OngoingTouches.Count - 1) + " ");

                ctx.BeginPath();
                ctx.Arc(t.PageX, t.PageY, 4, 0, 2 * Math.PI, false);  // a circle at the start

                ctx.FillStyle = color;
                ctx.Fill();

                this.LogMessage("touchstart:" + i + ".");
            }
        }

        private void TouchMoveHandler(TouchEvent<HTMLCanvasElement> evt)
        {
            evt.PreventDefault();

            var ctx = this.Context2D;

            var touches = evt.ChangedTouches;

            for (var i = 0; i < touches.Length; i++)
            {
                var t = touches[i];

                var idx = this.OngoingTouchIndexById(t.Identifier);

                if (idx >= 0)
                {
                    this.LogMessage("continuing touch " + idx);

                    var color = this.ColorForTouch(t, t.Identifier + " move idx " + idx + " ");

                    ctx.BeginPath();

                    var ogt = OngoingTouches[idx];

                    this.LogMessage("ctx.moveTo(" + ogt.PageX + ", " + ogt.PageY + ");");
                    ctx.MoveTo(ogt.PageX, ogt.PageY);

                    this.LogMessage("ctx.lineTo(" + t.PageX + ", " + t.PageY + ");");
                    ctx.LineTo(t.PageX, t.PageY);
                    ctx.LineWidth = 4;

                    ctx.StrokeStyle = color;
                    ctx.Stroke();

                    OngoingTouches[idx] = CopyTouch(t);  // swap in the new touch record

                    this.LogMessage("continued .");
                }
                else
                {
                    this.LogMessage("can't figure out which touch to continue");
                }
            }
        }

        private void TouchEndHandler(TouchEvent<HTMLCanvasElement> evt)
        {
            evt.PreventDefault();
            this.LogMessage("touchend");

            var ctx = this.Context2D;
            var touches = evt.ChangedTouches;

            for (var i = 0; i < touches.Length; i++)
            {
                var t = touches[i];

                var idx = OngoingTouchIndexById(t.Identifier);

                if (idx >= 0)
                {
                    var color = this.ColorForTouch(t, t.Identifier + " end idx " + idx + " ");

                    ctx.LineWidth = 4;
                    ctx.FillStyle = color;
                    ctx.BeginPath();

                    var ogt = OngoingTouches[idx];

                    ctx.MoveTo(ogt.PageX, ogt.PageY);
                    ctx.LineTo(t.PageX, t.PageY);
                    ctx.FillRect(t.PageX - 4, t.PageY - 4, 8, 8);  // and a square at the end

                    OngoingTouches.RemoveAt(idx);  // remove it; we're done
                }
                else
                {
                    this.LogMessage("can't figure out which touch to end");
                }
            }
        }

        private void TouchCancelHandler(TouchEvent<HTMLCanvasElement> evt)
        {
            evt.PreventDefault();

            OngoingTouches.Clear();
            LogMessage("touchcancel.");
        }

        #endregion Events

        #region Helpers

        private void LogMessage(string message, bool newLine = true)
        {
            this.Log.InnerHTML += message + (newLine ? "\n" : "");
        }

        private TouchShadow CopyTouch(Touch source)
        {
            return new TouchShadow() { Identifier = source.Identifier, PageX = source.PageX, PageY = source.PageY };
        }

        private string ColorForTouch(Touch touch, string who)
        {
            var r = touch.Identifier % 16;
            var g = Math.Floor((double)(touch.Identifier / 3)) % 16;
            var b = Math.Floor((double)(touch.Identifier / 7)) % 16;

            var rx = r.ToString("X"); // make it a hex digit
            var gx = g.ToString("X"); // make it a hex digit
            var bx = b.ToString("X"); // make it a hex digit
            var color = "#" + rx + gx + bx;

            this.LogMessage(who + " color for touch with identifier " + touch.Identifier + " = " + color);

            return color;
        }

        private int OngoingTouchIndexById(int id)
        {
            for (var i = 0; i < this.OngoingTouches.Count; i++)
            {
                if (this.OngoingTouches[i].Identifier == id)
                {
                    return i;
                }
            }

            return -1;  // not found
        }

        #endregion Helpers
    }

    public class TouchShadow
    {
        public int Identifier { get; set; }

        public int PageX { get; set; }

        public int PageY { get; set; }

    }

}
