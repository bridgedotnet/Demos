(function (globals) {
    "use strict";

    Bridge.define('TouchSample.TouchSample', {
        statics: {
            config: {
                init: function () {
                    Bridge.ready(this.start);
                }
            },
            start: function () {
                new TouchSample.TouchSample().initialize();
            }
        },
        canvas: null,
        context2D: null,
        config: {
            properties: {
                Log: null,
                OngoingTouches: null
            }
        },
        getCanvas: function () {
            return this.canvas;
        },
        setCanvas: function (value) {
            this.canvas = value;
    
            if (!Bridge.hasValue(this.canvas)) {
                this.context2D = null;
            }
            else  {
                this.context2D = this.canvas.getContext("2d");
            }
        },
        getContext2D: function () {
            return this.context2D;
        },
        initialize: function () {
            this.setOngoingTouches(new Bridge.List$1(TouchSample.TouchShadow)());
    
            this.setCanvas(document.getElementById("canvas"));
    
            this.setLog(document.getElementById("log"));
    
            this.getCanvas().onclick = Bridge.fn.combine(this.getCanvas().onclick, Bridge.fn.bind(this, this.clickHandler));
            this.getCanvas().ontouchstart = Bridge.fn.combine(this.getCanvas().ontouchstart, Bridge.fn.bind(this, this.touchStartHandler));
            this.getCanvas().ontouchmove = Bridge.fn.combine(this.getCanvas().ontouchmove, Bridge.fn.bind(this, this.touchMoveHandler));
            this.getCanvas().ontouchend = Bridge.fn.combine(this.getCanvas().ontouchend, Bridge.fn.bind(this, this.touchEndHandler));
            this.getCanvas().ontouchcancel = Bridge.fn.combine(this.getCanvas().ontouchcancel, Bridge.fn.bind(this, this.touchCancelHandler));
    
            this.logMessage("Initialized.");
        },
        clickHandler: function (e) {
            this.logMessage("ClickHandler.");
        },
        touchStartHandler: function (evt) {
            var ctx = this.getContext2D();
            var touches = evt.touches;
    
            for (var i = 0; i < touches.length; i++) {
                this.logMessage("touchstart:" + i + "...");
    
                var t = touches[i];
    
                this.getOngoingTouches().add(this.copyTouch(t));
    
                var color = this.colorForTouch(t, t.identifier + " start idx " + (this.getOngoingTouches().getCount() - 1) + " ");
    
                ctx.beginPath();
                ctx.arc(t.pageX, t.pageY, 4, 0, 2 * Math.PI, false); // a circle at the start
    
                ctx.fillStyle = color;
                ctx.fill();
    
                this.logMessage("touchstart:" + i + ".");
            }
        },
        touchMoveHandler: function (evt) {
            evt.preventDefault();
    
            var ctx = this.getContext2D();
    
            var touches = evt.changedTouches;
    
            for (var i = 0; i < touches.length; i++) {
                var t = touches[i];
    
                var idx = this.ongoingTouchIndexById(t.identifier);
    
                if (idx >= 0) {
                    this.logMessage("continuing touch " + idx);
    
                    var color = this.colorForTouch(t, t.identifier + " move idx " + idx + " ");
    
                    ctx.beginPath();
    
                    var ogt = this.getOngoingTouches().getItem(idx);
    
                    this.logMessage("ctx.moveTo(" + ogt.getPageX() + ", " + ogt.getPageY() + ");");
                    ctx.moveTo(ogt.getPageX(), ogt.getPageY());
    
                    this.logMessage("ctx.lineTo(" + t.pageX + ", " + t.pageY + ");");
                    ctx.lineTo(t.pageX, t.pageY);
                    ctx.lineWidth = 4;
    
                    ctx.strokeStyle = color;
                    ctx.stroke();
    
                    this.getOngoingTouches().setItem(idx, this.copyTouch(t)); // swap in the new touch record
    
                    this.logMessage("continued .");
                }
                else  {
                    this.logMessage("can't figure out which touch to continue");
                }
            }
        },
        touchEndHandler: function (evt) {
            evt.preventDefault();
            this.logMessage("touchend");
    
            var ctx = this.getContext2D();
            var touches = evt.changedTouches;
    
            for (var i = 0; i < touches.length; i++) {
                var t = touches[i];
    
                var idx = this.ongoingTouchIndexById(t.identifier);
    
                if (idx >= 0) {
                    var color = this.colorForTouch(t, t.identifier + " end idx " + idx + " ");
    
                    ctx.lineWidth = 4;
                    ctx.fillStyle = color;
                    ctx.beginPath();
    
                    var ogt = this.getOngoingTouches().getItem(idx);
    
                    ctx.moveTo(ogt.getPageX(), ogt.getPageY());
                    ctx.lineTo(t.pageX, t.pageY);
                    ctx.fillRect(t.pageX - 4, t.pageY - 4, 8, 8); // and a square at the end
    
                    this.getOngoingTouches().removeAt(idx); // remove it; we're done
                }
                else  {
                    this.logMessage("can't figure out which touch to end");
                }
            }
        },
        touchCancelHandler: function (evt) {
            evt.preventDefault();
    
            this.getOngoingTouches().clear();
            this.logMessage("touchcancel.");
        },
        logMessage: function (message, newLine) {
            if (newLine === void 0) { newLine = true; }
            this.getLog().innerHTML += message + (newLine ? "\n" : "");
        },
        copyTouch: function (source) {
            return Bridge.merge(new TouchSample.TouchShadow(), {
                setIdentifier: source.identifier,
                setPageX: source.pageX,
                setPageY: source.pageY
            } );
        },
        colorForTouch: function (touch, who) {
            var r = touch.identifier % 16;
            var g = Math.floor(Bridge.cast((Bridge.Int.div(touch.identifier, 3)), Number)) % 16;
            var b = Math.floor(Bridge.cast((Bridge.Int.div(touch.identifier, 7)), Number)) % 16;
    
            var rx = Bridge.Int.format(r, "X"); // make it a hex digit
            var gx = Bridge.Int.format(g, "X"); // make it a hex digit
            var bx = Bridge.Int.format(b, "X"); // make it a hex digit
            var color = "#" + rx + gx + bx;
    
            this.logMessage(who + " color for touch with identifier " + touch.identifier + " = " + color);
    
            return color;
        },
        ongoingTouchIndexById: function (id) {
            for (var i = 0; i < this.getOngoingTouches().getCount(); i++) {
                if (this.getOngoingTouches().getItem(i).getIdentifier() === id) {
                    return i;
                }
            }
    
            return -1; // not found
        }
    });
    
    Bridge.define('TouchSample.TouchShadow', {
        config: {
            properties: {
                Identifier: 0,
                PageX: 0,
                PageY: 0
            }
        }
    });
    
    Bridge.init();
})(this);
