/**
 * @version 1.0.0.0
 * @author Object.NET, Inc.
 * @copyright Copyright 2008-2016 Object.NET, Inc.
 * @compiler Bridge.NET 16.0.0-beta5
 */
Bridge.assembly("Mandelbrot.Canvas", function ($asm, globals) {
    "use strict";

    Bridge.define("Mandelbrot.Canvas.App", {
        main: function Main () {
            var settings = new Mandelbrot.Canvas.Options();

            var calculator = new Mandelbrot.Canvas.Mandelbrot(settings);

            var drawer = new Mandelbrot.Canvas.Drawer(settings, calculator);
        }
    });

    Bridge.define("Mandelbrot.Canvas.Drawer", {
        statics: {
            methods: {
                GetInputValue$1: function (source, number) {
                    var d = parseFloat(source.value);
                    if (isNaN(d)) {
                        number.v = 0;
                        return false;
                    }

                    number.v = Bridge.Int.clip32(Bridge.Math.round(d, 0, 6));
                    return true;
                },
                GetInputValue: function (source, number) {
                    number.v = parseFloat(source.value);
                    if (isNaN(number.v)) {
                        return false;
                    }

                    return true;
                }
            }
        },
        fields: {
            CanvasRenderingContext: null,
            Image: null,
            DrawButton: null,
            IterationCountElement: null,
            RadiusElement: null,
            ColorMapCheckbox: null,
            ColorOffsetElement: null,
            ColorScaleElement: null,
            JuliaSetCheckbox: null,
            JuliaReElement: null,
            JuliaImElement: null,
            XMinElement: null,
            XMaxElement: null,
            YMinElement: null,
            YMaxElement: null,
            CurrentY: 0,
            Calculator: null,
            Settings: null
        },
        ctors: {
            ctor: function (settings, calculator) {
                this.$initialize();                var $t;

                this.Settings = settings;
                this.Calculator = calculator;

                // the actual canvas element
                var canvas = document.createElement('canvas');
                canvas.width = 900;
                canvas.height = 500;

                this.DrawButton = ($t = document.createElement('button'), $t.innerHTML = "Draw the Mandelbrot fractal", $t.onclick = Bridge.fn.bind(this, function (ev) {
                    this.StartDraw(canvas);
                }), $t);

                this.DrawButton.setAttribute("style", "font-size:18px;height: 60px;  width:95%; border: 2px solid black; cursor: pointer");

                // Iteration controls
                this.RadiusElement = this.GetInputNumberElement(null, this.Settings.MaxRadius, 3, 0.5);
                this.IterationCountElement = this.GetInputNumberElement(null, this.Settings.MaxIterations, 4, 100, 0, 100000);
                // Color controls
                this.ColorMapCheckbox = this.GetCheckboxElement(this.Settings.UseColorMap);
                this.ColorScaleElement = this.GetInputNumberElement(this.ColorMapCheckbox, this.Settings.ColorScale, 5, 1000);
                this.ColorOffsetElement = this.GetInputNumberElement(this.ColorMapCheckbox, this.Settings.ColorOffset, 4, 10);

                // Julia sets
                this.JuliaSetCheckbox = this.GetCheckboxElement(this.Settings.UseJuliaSet);
                this.JuliaImElement = this.GetInputNumberElement(this.JuliaSetCheckbox, this.Settings.JuliaSetParameter.Im, 5, 0.005, null);
                this.JuliaReElement = this.GetInputNumberElement(this.JuliaSetCheckbox, this.Settings.JuliaSetParameter.Re, 5, 0.005, null);

                // Viewport controls
                this.XMinElement = this.GetInputNumberElement(null, this.Settings.XMin, 5, 0.005, -5.0);
                this.XMaxElement = this.GetInputNumberElement(null, this.Settings.XMax, 5, 0.005, 0.0);
                this.YMinElement = this.GetInputNumberElement(null, this.Settings.YMin, 5, 0.005, -5.0);
                this.YMaxElement = this.GetInputNumberElement(null, this.Settings.YMax, 5, 0.005, 0.0);

                var paramsColumn = document.createElement('td');
                var canvasColumn = document.createElement('td');
                paramsColumn.setAttribute("valign", "top");
                canvasColumn.setAttribute("valign", "top");
                canvasColumn.appendChild(canvas);

                var layoutRow = document.createElement('tr');
                Mandelbrot.Canvas.Extensions.AppendChildren(layoutRow, [paramsColumn, canvasColumn]);

                var layout = document.createElement('table');

                var paramsTable = document.createElement('table');
                Mandelbrot.Canvas.Extensions.AppendChildren(paramsTable, [this.Row$1([this.Label("XMin: "), this.XMinElement]), this.Row$1([this.Label("XMax: "), this.XMaxElement]), this.Row$1([this.Label("YMin: "), this.YMinElement]), this.Row$1([this.Label("YMax: "), this.YMaxElement]), this.Row$1([this.Label("Escape radius: "), this.RadiusElement]), this.Row$1([this.Label("Iteration count: "), this.IterationCountElement]), this.Row$1([this.Label("Use color map: "), this.ColorMapCheckbox]), this.Row$1([this.Label("Color scale: "), this.ColorScaleElement]), this.Row$1([this.Label("Color offset: "), this.ColorOffsetElement]), this.Row$1([this.Label("Use Julia set: "), this.JuliaSetCheckbox]), this.Row$1([this.Label("Im: "), this.JuliaImElement]), this.Row$1([this.Label("Re: "), this.JuliaReElement]), this.Row(document.createElement('hr'), 2), this.Row(this.DrawButton, 2)]);

                paramsColumn.appendChild(paramsTable);

                layout.appendChild(layoutRow);
                document.body.appendChild(layout);
        }
    },
    methods: {
        Label: function (value) {
            var lbl = document.createElement('label');
            lbl.innerHTML = value;
            lbl.style.margin = "5px";
            lbl.style.fontSize = "18px";
            return lbl;
        },
        Row$1: function (elements) {
            var $t;
            if (elements === void 0) { elements = []; }
            var row = document.createElement('tr');
            $t = Bridge.getEnumerator(elements);
            try {
                while ($t.moveNext()) {
                    var el = $t.Current;
                    row.appendChild(Mandelbrot.Canvas.Extensions.WithinTableDataCell(el));
                }
            } finally {
                if (Bridge.is($t, System.IDisposable)) {
                    $t.System$IDisposable$dispose();
                }
            }return row;
        },
        Row: function (el, colspan) {
            var row = document.createElement('tr');
            var td = document.createElement('td');
            td.setAttribute("colspan", colspan.toString());
            td.appendChild(el);
            row.appendChild(td);
            return row;
        },
        GetCheckboxElement: function (isChecked) {
            var $t;
            var checkbox = ($t = document.createElement('input'), $t.type = "checkbox", $t.checked = isChecked, $t);

            // Extend the checkbox
            var elements = this.GetElementListToEnable(checkbox);

            var enableInputs = function (ev) {
                var $t1;
                $t1 = Bridge.getEnumerator(elements);
                try {
                    while ($t1.moveNext()) {
                        var control = $t1.Current;
                        control.disabled = !checkbox.checked;
                    }
                } finally {
                    if (Bridge.is($t1, System.IDisposable)) {
                        $t1.System$IDisposable$dispose();
                    }
                }};
            checkbox.onchange = enableInputs;

            //var label = new HTMLLabelElement
            //{
            //    InnerHTML = title
            //};

            //label.Style.Margin = "5px";
            //label.AppendChild(checkbox);

            //if (container != null)
            //{
            //    container.AppendChild(label);
            //}

            return checkbox;
        },
        GetElementListToEnable: function (element) {
            var ce = element.toEnable;
            var elements;

            if (ce == null || ((elements = Bridge.as(ce, System.Collections.Generic.List$1(HTMLInputElement)))) == null) {
                elements = new (System.Collections.Generic.List$1(HTMLInputElement))();
                element.toEnable = elements;
            }

            return elements;
        },
        GetInputNumberElement: function (enableElement, initialValue, widthInDigits, step, min, max) {
            var $t;
            if (step === void 0) { step = 1.0; }
            if (min === void 0) { min = 0.0; }
            if (max === void 0) { max = null; }
            var element = ($t = document.createElement('input'), $t.type = "number", $t.value = System.Double.format(initialValue), $t.onkeyup = Bridge.fn.bind(this, $asm.$.Mandelbrot.Canvas.Drawer.f1), $t);

            if (enableElement != null) {
                element.disabled = !enableElement.checked;

                var elements = this.GetElementListToEnable(enableElement);
                elements.add(element);
            }

            if (System.Nullable.hasValue(step)) {
                element.step = System.Nullable.toString(step, function ($t) { return System.Double.format($t); });
            }

            if (System.Nullable.hasValue(min)) {
                element.min = System.Nullable.toString(min, function ($t) { return System.Double.format($t); });
            }

            if (System.Nullable.hasValue(max)) {
                element.max = System.Nullable.toString(max, function ($t) { return System.Double.format($t); });
            }

            element.style.width = widthInDigits + "em";
            element.style.margin = "5px";

            return element;
        },
        GetImageData: function (canvas) {
            var height = canvas.height;
            var width = canvas.width;

            if (this.CanvasRenderingContext == null) {
                this.CanvasRenderingContext = canvas.getContext("2d");
            }

            if (this.Image == null) {
                // the data to manipulate
                this.Image = this.CanvasRenderingContext.createImageData(width, height);
            } else {
                //var color = new Color() { Red = 0, Green = 0, Blue = 0 };

                //for (int y = 0; y < height; y++)
                //{
                //    for (int x = 0; x < width; x++)
                //    {
                //        Image.SetPixel(x, y, color);
                //    }
                //}
            }
        },
        StartDraw: function (canvas) {
            if (!Mandelbrot.Canvas.Drawer.GetInputValue$1(this.IterationCountElement, Bridge.ref(this.Settings, "MaxIterations"))) {
                window.alert("Iteration count should be positive integer");
                return;
            }

            if (!Mandelbrot.Canvas.Drawer.GetInputValue(this.RadiusElement, Bridge.ref(this.Settings, "MaxRadius"))) {
                window.alert("Escape radius should be positive integer");
                return;
            }

            if (!Mandelbrot.Canvas.Drawer.GetInputValue(this.XMinElement, Bridge.ref(this.Settings, "XMin"))) {
                window.alert("XMin should be a number");
                return;
            }

            if (!Mandelbrot.Canvas.Drawer.GetInputValue(this.XMaxElement, Bridge.ref(this.Settings, "XMax"))) {
                window.alert("XMax should be a number");
                return;
            }

            if (!Mandelbrot.Canvas.Drawer.GetInputValue(this.YMinElement, Bridge.ref(this.Settings, "YMin"))) {
                window.alert("YMin should be a number");
                return;
            }

            if (!Mandelbrot.Canvas.Drawer.GetInputValue(this.YMaxElement, Bridge.ref(this.Settings, "YMax"))) {
                window.alert("YMax should be a number");
                return;
            }


            this.Settings.UseColorMap = this.ColorMapCheckbox.checked;
            if (this.Settings.UseColorMap) {
                if (!Mandelbrot.Canvas.Drawer.GetInputValue$1(this.ColorScaleElement, Bridge.ref(this.Settings, "ColorScale"))) {
                    window.alert("Color scale should be positive integer");
                    return;
                }

                if (!Mandelbrot.Canvas.Drawer.GetInputValue$1(this.ColorOffsetElement, Bridge.ref(this.Settings, "ColorOffset"))) {
                    window.alert("Color offset should be positive integer");
                    return;
                }
            }

            this.Settings.UseJuliaSet = this.JuliaSetCheckbox.checked;
            if (this.Settings.UseJuliaSet) {
                if (!Mandelbrot.Canvas.Drawer.GetInputValue(this.JuliaImElement, Bridge.ref(this.Settings.JuliaSetParameter, "Im"))) {
                    window.alert("Julia Im should be a number");
                    return;
                }

                if (!Mandelbrot.Canvas.Drawer.GetInputValue(this.JuliaReElement, Bridge.ref(this.Settings.JuliaSetParameter, "Re"))) {
                    window.alert("Julia Re should be a number");
                    return;
                }
            }

            this.GetImageData(canvas);

            this.CurrentY = 0;
            this.DrawPart();
        },
        DrawPart: function () {
            var height = this.CanvasRenderingContext.canvas.height;
            var width = this.CanvasRenderingContext.canvas.width;

            if (this.CurrentY < height) {
                for (var x = 0; x < width; x = (x + 1) | 0) {
                    var color = this.Calculator.CalculatePoint(x, this.CurrentY, height, width);

                    Mandelbrot.Canvas.Extensions.SetPixel(this.Image, x, this.CurrentY, color);
                }

                this.CanvasRenderingContext.putImageData(this.Image, 0, 0);
                this.CurrentY = (this.CurrentY + 1) | 0;
            }

            if (this.CurrentY < height) {
                window.setTimeout(Bridge.fn.cacheBind(this, this.DrawPart), 0);
            }
        }
    }
    });

    Bridge.ns("Mandelbrot.Canvas.Drawer", $asm.$);

    Bridge.apply($asm.$.Mandelbrot.Canvas.Drawer, {
        f1: function (ev) {
            if (Bridge.is(ev, KeyboardEvent)) {
                var kev = ev;

                if (kev != null) {
                    if (Bridge.equals(kev.keyCode, Bridge.box(13, System.Int32))) {
                        ev.preventDefault();
                        this.DrawButton.click();
                    }
                }
            }
        }
    });

    Bridge.define("Mandelbrot.Canvas.Extensions", {
        statics: {
            methods: {
                SetPixel: function (img, x, y, color) {
                    var index = Bridge.Int.mul((((x + Bridge.Int.mul(y, (img.width | 0))) | 0)), 4);
                    img.data[System.Array.index(index, img.data)] = color.Red;
                    img.data[System.Array.index(((index + 1) | 0), img.data)] = color.Green;
                    img.data[System.Array.index(((index + 2) | 0), img.data)] = color.Blue;
                    img.data[System.Array.index(((index + 3) | 0), img.data)] = 255; // alpha
                },
                WithinTableDataCell: function (el) {
                    var td = document.createElement('td');
                    td.appendChild(el);
                    return td;
                },
                AppendChildren: function (parent, elems) {
                    var $t;
                    if (elems === void 0) { elems = []; }
                    $t = Bridge.getEnumerator(elems);
                    try {
                        while ($t.moveNext()) {
                            var element = $t.Current;
                            parent.appendChild(element);
                        }
                    } finally {
                        if (Bridge.is($t, System.IDisposable)) {
                            $t.System$IDisposable$dispose();
                        }
                    }}
            }
        }
    });

    Bridge.define("Mandelbrot.Canvas.Mandelbrot", {
        props: {
            Settings: null
        },
        ctors: {
            ctor: function (settings) {
                this.$initialize();
                this.Settings = settings;
            }
        },
        methods: {
            Magnitude: function (z) {
                return Math.sqrt(z.Re * z.Re + z.Im * z.Im);
            },
            Multiply: function (z, u) {
                return { Re: z.Re * u.Re - z.Im * u.Im, Im: z.Re * u.Im + u.Re * z.Im };
            },
            Add: function (z, u) {
                return { Re: z.Re + u.Re, Im: z.Im + u.Im };
            },
            Rescale: function (value, realRange, projection) {
                if (value > realRange.To || value < realRange.From) {
                    throw new System.ArgumentException("value is not the real range");
                }

                var percentageOfProjection = (Math.abs(projection.To - projection.From) * Math.abs(value - realRange.From)) / Math.abs(realRange.To - realRange.From);

                return percentageOfProjection + projection.From;
            },
            FromPointOnCanvas: function (p, xmin, xmax, ymin, ymax, height, width) {
                return { Re: this.Rescale(p.X, { From: 0, To: width }, { From: xmin, To: xmax }), Im: this.Rescale(p.Y, { From: 0, To: height }, { From: ymin, To: ymax }) };
            },
            IterationsUntilDivergence: function (z) {
                var constant = this.Settings.UseJuliaSet ? this.Settings.JuliaSetParameter : { Re: z.Re, Im: z.Im };

                var count = 0;

                while (true) {
                    z = this.Add(this.Multiply(z, z), constant);
                    count = (count + 1) | 0;

                    if (this.Magnitude(z) > this.Settings.MaxRadius || count >= this.Settings.MaxIterations) {
                        break;
                    }
                }


                z = this.Add(this.Multiply(z, z), constant);
                count = (count + 1) | 0;
                z = this.Add(this.Multiply(z, z), constant);
                count = (count + 1) | 0;


                return count;
            },
            ColorLookup: function (count, maxCount, z) {
                var bytes = System.Array.init(3, 0, System.Byte);
                var scale = (this.Settings.ColorScale) >>> 0;
                var shift = (this.Settings.ColorOffset) >>> 0;

                var log2 = Bridge.Math.log(50);

                if (count < this.Settings.MaxIterations) {
                    var modulus = Math.sqrt(z.Re * z.Re + z.Im * z.Im);
                    var a = Bridge.Math.log(modulus);
                    if (isNaN(a)) {
                        a = 0;
                    }

                    var b = Bridge.Math.log(a);
                    if (isNaN(b)) {
                        b = 0;
                    }

                    var c = b / log2;
                    var mu = count - c;

                    var p = scale * mu / this.Settings.MaxIterations;

                    var i;
                    if (p > scale) {
                        i = scale;
                    } else if (p < 0) {
                        i = 0;
                    } else {
                        //Console.WriteLine("{0} {1} {2} {3} {4}",  modulus, a, b, c, p);
                        i = (System.Convert.toUInt32(Bridge.box(p, System.Double, System.Double.format, System.Double.getHashCode)) + shift) >>> 0;
                    }

                    bytes[System.Array.index(2, bytes)] = (i >>> 16) & 255;
                    bytes[System.Array.index(1, bytes)] = (i >>> 8) & 255;
                    bytes[System.Array.index(0, bytes)] = i & 255;
                } else {
                    bytes[System.Array.index(2, bytes)] = 0;
                    bytes[System.Array.index(1, bytes)] = 0;
                    bytes[System.Array.index(0, bytes)] = 0;
                }

                var color = { Red: bytes[System.Array.index(2, bytes)], Green: bytes[System.Array.index(1, bytes)], Blue: bytes[System.Array.index(0, bytes)] };

                return color;
            },
            CalculatePoint: function (x, y, height, width) {
                // complex plain viewport
                var xmin = this.Settings.XMin;
                var xmax = this.Settings.XMax;
                var ymin = this.Settings.YMin;
                var ymax = this.Settings.YMax;

                var point = { X: x, Y: y };
                var complex = this.FromPointOnCanvas(point, xmin, xmax, ymin, ymax, height, width);
                var iterations = this.IterationsUntilDivergence(complex);

                var color;
                if (this.Settings.UseColorMap) {
                    color = this.ColorLookup(iterations, this.Settings.MaxIterations, complex);
                } else {
                    color = { Red: (iterations & 255), Green: (iterations & 255), Blue: (iterations & 255) };
                }

                return color;
            }
        }
    });

    Bridge.define("Mandelbrot.Canvas.Options", {
        fields: {
            XMin: 0,
            XMax: 0,
            YMin: 0,
            YMax: 0,
            MaxIterations: 0,
            MaxRadius: 0,
            UseColorMap: false,
            ColorOffset: 0,
            ColorScale: 0,
            UseJuliaSet: false,
            JuliaSetParameter: null
        },
        ctors: {
            init: function () {
                this.XMin = -2.0;
                this.XMax = 2.0;
                this.YMin = -2.0;
                this.YMax = 2.0;
                this.MaxIterations = 1000;
                this.MaxRadius = 10;
                this.UseColorMap = true;
                this.ColorOffset = 300;
                this.ColorScale = 10000;
                this.UseJuliaSet = false;
                this.JuliaSetParameter = { Re: -0.735, Im: 0.175 };
            }
        }
    });
});
