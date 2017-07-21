/**
 * Bridge Html5 Touch Demo
 * @version 1.0.0.0
 * @author Object.NET, Inc.
 * @copyright Copyright 2008-2015 Object.NET, Inc.
 * @compiler Bridge.NET 16.0.0-beta5
 */
Bridge.assembly("TouchSample", function ($asm, globals) {
    "use strict";

    Bridge.define("TouchSample.TouchSample", {
        statics: {
            ctors: {
                init: function () {
                    Bridge.ready(this.Start);
                }
            },
            methods: {
                Start: function () {
                    new TouchSample.TouchSample().Initialize();
                }
            }
        },
        $entryPoint: true,
        fields: {
            canvas: null,
            context2D: null
        },
        props: {
            Canvas: {
                get: function () {
                    return this.canvas;
                },
                set: function (value) {
                    this.canvas = value;

                    if (this.canvas == null) {
                        this.context2D = null;
                    } else {
                        this.context2D = this.canvas.getContext("2d");
                    }
                }
            },
            Context2D: {
                get: function () {
                    return this.context2D;
                }
            },
            Log: null,
            OngoingTouches: null
        },
        methods: {
            Initialize: function () {
                this.OngoingTouches = new (System.Collections.Generic.List$1(TouchSample.TouchShadow))();

                this.Canvas = document.getElementById("canvas");

                this.Log = document.getElementById("log");

                this.Canvas.onclick = Bridge.fn.combine(this.Canvas.onclick, Bridge.fn.cacheBind(this, this.ClickHandler));
                this.Canvas.ontouchstart = Bridge.fn.combine(this.Canvas.ontouchstart, Bridge.fn.cacheBind(this, this.TouchStartHandler));
                this.Canvas.ontouchmove = Bridge.fn.combine(this.Canvas.ontouchmove, Bridge.fn.cacheBind(this, this.TouchMoveHandler));
                this.Canvas.ontouchend = Bridge.fn.combine(this.Canvas.ontouchend, Bridge.fn.cacheBind(this, this.TouchEndHandler));
                this.Canvas.ontouchcancel = Bridge.fn.combine(this.Canvas.ontouchcancel, Bridge.fn.cacheBind(this, this.TouchCancelHandler));

                this.LogMessage("Initialized.");
            },
            ClickHandler: function (e) {
                this.LogMessage("ClickHandler.");
            },
            TouchStartHandler: function (evt) {
                var ctx = this.Context2D;
                var touches = evt.touches;

                for (var i = 0; i < touches.length; i = (i + 1) | 0) {
                    this.LogMessage("touchstart:" + i + "...");

                    var t = touches[System.Array.index(i, touches)];

                    this.OngoingTouches.add(this.CopyTouch(t));

                    var color = this.ColorForTouch(t, t.identifier + " start idx " + (((this.OngoingTouches.Count - 1) | 0)) + " ");

                    ctx.beginPath();
                    ctx.arc(t.pageX, t.pageY, 4, 0, 6.2831853071795862, false); // a circle at the start

                    ctx.fillStyle = color;
                    ctx.fill();

                    this.LogMessage("touchstart:" + i + ".");
                }
            },
            TouchMoveHandler: function (evt) {
                evt.preventDefault();

                var ctx = this.Context2D;

                var touches = evt.changedTouches;

                for (var i = 0; i < touches.length; i = (i + 1) | 0) {
                    var t = touches[System.Array.index(i, touches)];

                    var idx = this.OngoingTouchIndexById(t.identifier);

                    if (idx >= 0) {
                        this.LogMessage("continuing touch " + idx);

                        var color = this.ColorForTouch(t, t.identifier + " move idx " + idx + " ");

                        ctx.beginPath();

                        var ogt = this.OngoingTouches.getItem(idx);

                        this.LogMessage("ctx.moveTo(" + ogt.PageX + ", " + ogt.PageY + ");");
                        ctx.moveTo(ogt.PageX, ogt.PageY);

                        this.LogMessage("ctx.lineTo(" + t.pageX + ", " + t.pageY + ");");
                        ctx.lineTo(t.pageX, t.pageY);
                        ctx.lineWidth = 4;

                        ctx.strokeStyle = color;
                        ctx.stroke();

                        this.OngoingTouches.setItem(idx, this.CopyTouch(t)); // swap in the new touch record

                        this.LogMessage("continued .");
                    } else {
                        this.LogMessage("can't figure out which touch to continue");
                    }
                }
            },
            TouchEndHandler: function (evt) {
                evt.preventDefault();
                this.LogMessage("touchend");

                var ctx = this.Context2D;
                var touches = evt.changedTouches;

                for (var i = 0; i < touches.length; i = (i + 1) | 0) {
                    var t = touches[System.Array.index(i, touches)];

                    var idx = this.OngoingTouchIndexById(t.identifier);

                    if (idx >= 0) {
                        var color = this.ColorForTouch(t, t.identifier + " end idx " + idx + " ");

                        ctx.lineWidth = 4;
                        ctx.fillStyle = color;
                        ctx.beginPath();

                        var ogt = this.OngoingTouches.getItem(idx);

                        ctx.moveTo(ogt.PageX, ogt.PageY);
                        ctx.lineTo(t.pageX, t.pageY);
                        ctx.fillRect(((t.pageX - 4) | 0), ((t.pageY - 4) | 0), 8, 8); // and a square at the end

                        this.OngoingTouches.removeAt(idx); // remove it; we're done
                    } else {
                        this.LogMessage("can't figure out which touch to end");
                    }
                }
            },
            TouchCancelHandler: function (evt) {
                evt.preventDefault();

                this.OngoingTouches.clear();
                this.LogMessage("touchcancel.");
            },
            LogMessage: function (message, newLine) {
                if (newLine === void 0) { newLine = true; }
                this.Log.innerHTML = System.String.concat(this.Log.innerHTML, (System.String.concat(message, (newLine ? "\n" : ""))));
            },
            CopyTouch: function (source) {
                var $t;
                return ($t = new TouchSample.TouchShadow(), $t.Identifier = source.identifier, $t.PageX = source.pageX, $t.PageY = source.pageY, $t);
            },
            ColorForTouch: function (touch, who) {
                var r = touch.identifier % 16;
                var g = Math.floor(((Bridge.Int.div(touch.identifier, 3)) | 0)) % 16;
                var b = Math.floor(((Bridge.Int.div(touch.identifier, 7)) | 0)) % 16;

                var rx = System.Int32.format(r, "X"); // make it a hex digit
                var gx = System.Double.format(g, "X"); // make it a hex digit
                var bx = System.Double.format(b, "X"); // make it a hex digit
                var color = System.String.concat("#", rx, gx, bx);

                this.LogMessage(System.String.concat(who, " color for touch with identifier ", touch.identifier, " = ", color));

                return color;
            },
            OngoingTouchIndexById: function (id) {
                for (var i = 0; i < this.OngoingTouches.Count; i = (i + 1) | 0) {
                    if (this.OngoingTouches.getItem(i).Identifier === id) {
                        return i;
                    }
                }

                return -1; // not found
            }
        }
    });

    Bridge.define("TouchSample.TouchShadow", {
        props: {
            Identifier: 0,
            PageX: 0,
            PageY: 0
        }
    });
});
