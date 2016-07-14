(function (globals) {
    "use strict";

    Bridge.define('Mandelbrot.Canvas.App', {
        statics: {
            config: {
                init: function () {
                    Bridge.ready(this.main);
                }
            },
            main: function () {
                var settings = new Mandelbrot.Canvas.Options();
    
                var calculator = new Mandelbrot.Canvas.Mandelbrot(settings);
    
                var drawer = new Mandelbrot.Canvas.Drawer(settings, calculator);
            }
        },
        $entryPoint: true
    });
    
    Bridge.define('Mandelbrot.Canvas.Drawer', {
        statics: {
            getInputValue$1: function (source, number) {
                var d = parseFloat(source.value);
                if (isNaN(d)) {
                    number.v = 0;
                    return false;
                }
    
                number.v = Bridge.Int.clip32(Bridge.Math.round(d, 0, 6));
                return true;
            },
            getInputValue: function (source, number) {
                number.v = parseFloat(source.value);
                if (isNaN(number.v)) {
                    return false;
                }
    
                return true;
            }
        },
        canvasRenderingContext: null,
        image: null,
        drawButton: null,
        iterationCountElement: null,
        radiusElement: null,
        colorMapCheckbox: null,
        colorOffsetElement: null,
        colorScaleElement: null,
        juliaSetCheckbox: null,
        juliaReElement: null,
        juliaImElement: null,
        currentY: 0,
        calculator: null,
        settings: null,
        constructor: function (settings, calculator) {
            this.$initialize();
            this.settings = settings;
            this.calculator = calculator;
    
            // the actual canvas element
            var canvas = document.createElement('canvas');
            canvas.width = 900;
            canvas.height = 500;
    
            this.drawButton = Bridge.merge(document.createElement('button'), {
                innerHTML: "Draw the Mandelbrot fractal",
                onclick: Bridge.fn.bind(this, function (ev) {
                    this.startDraw(canvas);
                })
            } );
    
            this.drawButton.setAttribute("style", "font-size:30px, height:80px; width:180px; border-radius:10px; border: 2px solid black; cursor: pointer");
    
            var parameterContainer = document.createElement('div');
    
            this.radiusElement = this.getInputNumberElement(parameterContainer, null, this.settings.maxRadius, 3, "Escape radius:", 0.5);
            this.iterationCountElement = this.getInputNumberElement(parameterContainer, null, this.settings.maxIterations, 4, "Iteration count:", 100, 0, 100000);
    
            parameterContainer.appendChild(document.createElement('br'));
    
            this.colorMapCheckbox = this.getCheckboxElement(parameterContainer, "Use Color map ", this.settings.useColorMap);
            this.colorScaleElement = this.getInputNumberElement(parameterContainer, this.colorMapCheckbox, this.settings.colorScale, 5, "Scale:", 1000);
            this.colorOffsetElement = this.getInputNumberElement(parameterContainer, this.colorMapCheckbox, this.settings.colorOffset, 4, "Offset:", 10);
    
            parameterContainer.appendChild(document.createElement('br'));
    
            this.juliaSetCheckbox = this.getCheckboxElement(parameterContainer, "Use Julia set ", this.settings.useJuliaSet);
            this.juliaImElement = this.getInputNumberElement(parameterContainer, this.juliaSetCheckbox, this.settings.juliaSetParameter.im, 5, "Im:", 0.005, null);
            this.juliaReElement = this.getInputNumberElement(parameterContainer, this.juliaSetCheckbox, this.settings.juliaSetParameter.re, 5, "Re:", 0.005, null);
    
            parameterContainer.appendChild(document.createElement('br'));
    
            parameterContainer.appendChild(this.drawButton);
    
            document.body.appendChild(parameterContainer);
    
            document.body.appendChild(document.createElement('hr'));
    
            document.body.appendChild(canvas);
        },
        getCheckboxElement: function (container, title, isChecked) {
            var checkbox = Bridge.merge(document.createElement('input'), {
                type: "checkbox",
                checked: isChecked
            } );
    
            // Extend the checkbox
            var elements = this.getElementListToEnable(checkbox);
    
            var enableInputs = function (ev) {
                var $t;
                $t = Bridge.getEnumerator(elements);
                while ($t.moveNext()) {
                    var control = $t.getCurrent();
                    control.disabled = !checkbox.checked;
                }
            };
            checkbox.onchange = enableInputs;
    
            var label = Bridge.merge(document.createElement('label'), {
                innerHTML: title
            } );
    
            label.style.margin = "5px";
            label.appendChild(checkbox);
    
            if (container != null) {
                container.appendChild(label);
            }
    
            return checkbox;
        },
        getElementListToEnable: function (element) {
            var ce = element.toEnable;
            var elements;
    
            if (ce == null || ((elements = Bridge.as(ce, System.Collections.Generic.List$1(HTMLInputElement)))) == null) {
                elements = new (System.Collections.Generic.List$1(HTMLInputElement))();
                element.toEnable = elements;
            }
    
            return elements;
        },
        getInputNumberElement: function (container, enableElement, initialValue, widthInDigits, title, step, min, max) {
            if (step === void 0) { step = 1.0; }
            if (min === void 0) { min = 0.0; }
            if (max === void 0) { max = null; }
            var label = document.createElement('label');
            label.innerHTML = title;
            label.style.margin = "5px";
    
            var element = Bridge.merge(document.createElement('input'), {
                type: "number",
                value: System.Double.format(initialValue, 'G'),
                onkeyup: Bridge.fn.bind(this, $_.Mandelbrot.Canvas.Drawer.f1)
            } );
    
            if (enableElement != null) {
                element.disabled = !enableElement.checked;
    
                var elements = this.getElementListToEnable(enableElement);
                elements.add(element);
            }
    
            if (System.Nullable.hasValue(step)) {
                element.step = System.Nullable.toString(step, function ($t) { return System.Double.format($t, 'G'); });
            }
    
            if (System.Nullable.hasValue(min)) {
                element.min = System.Nullable.toString(min, function ($t) { return System.Double.format($t, 'G'); });
            }
    
            if (System.Nullable.hasValue(max)) {
                element.max = System.Nullable.toString(max, function ($t) { return System.Double.format($t, 'G'); });
            }
    
            element.style.width = widthInDigits + "em";
            element.style.margin = "5px";
    
            label.appendChild(element);
    
            if (container != null) {
                container.appendChild(label);
            }
    
            return element;
        },
        getImageData: function (canvas) {
            var height = canvas.height;
            var width = canvas.width;
    
            if (this.canvasRenderingContext == null) {
                this.canvasRenderingContext = canvas.getContext("2d");
            }
    
            if (this.image == null) {
                // the data to manipulate
                this.image = this.canvasRenderingContext.createImageData(width, height);
            }
            else  {
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
        startDraw: function (canvas) {
            if (!Mandelbrot.Canvas.Drawer.getInputValue$1(this.iterationCountElement, Bridge.ref(this.settings, "maxIterations"))) {
                window.alert("Iteration count should be positive integer");
                return;
            }
    
            if (!Mandelbrot.Canvas.Drawer.getInputValue(this.radiusElement, Bridge.ref(this.settings, "maxRadius"))) {
                window.alert("Escape radius should be positive integer");
                return;
            }
    
            this.settings.useColorMap = this.colorMapCheckbox.checked;
            if (this.settings.useColorMap) {
                if (!Mandelbrot.Canvas.Drawer.getInputValue$1(this.colorScaleElement, Bridge.ref(this.settings, "colorScale"))) {
                    window.alert("Color scale should be positive integer");
                    return;
                }
    
                if (!Mandelbrot.Canvas.Drawer.getInputValue$1(this.colorOffsetElement, Bridge.ref(this.settings, "colorOffset"))) {
                    window.alert("Color offset should be positive integer");
                    return;
                }
            }
    
            this.settings.useJuliaSet = this.juliaSetCheckbox.checked;
            if (this.settings.useJuliaSet) {
                if (!Mandelbrot.Canvas.Drawer.getInputValue(this.juliaImElement, Bridge.ref(this.settings.juliaSetParameter, "im"))) {
                    window.alert("Julia Im should be a number");
                    return;
                }
    
                if (!Mandelbrot.Canvas.Drawer.getInputValue(this.juliaReElement, Bridge.ref(this.settings.juliaSetParameter, "re"))) {
                    window.alert("Julia Re should be a number");
                    return;
                }
            }
    
            this.getImageData(canvas);
    
            this.currentY = 0;
            this.drawPart();
        },
        drawPart: function () {
            var height = this.canvasRenderingContext.canvas.height;
            var width = this.canvasRenderingContext.canvas.width;
    
            if (this.currentY < height) {
                for (var x = 0; x < width; x = (x + 1) | 0) {
                    var color = this.calculator.calculatePoint(x, this.currentY, height, width);
    
                    Mandelbrot.Canvas.Extensions.setPixel(this.image, x, this.currentY, color);
                }
    
                this.canvasRenderingContext.putImageData(this.image, 0, 0);
                this.currentY = (this.currentY + 1) | 0;
            }
    
            if (this.currentY < height) {
                window.setTimeout(Bridge.fn.bind(this, this.drawPart), 0);
            }
        }
    });
    
    var $_ = {};
    
    Bridge.ns("Mandelbrot.Canvas.Drawer", $_);
    
    Bridge.apply($_.Mandelbrot.Canvas.Drawer, {
        f1: function (ev) {
            if (Bridge.is(ev, KeyboardEvent)) {
                var kev = ev;
    
                if (kev != null) {
                    if (Bridge.equals(kev.keyCode, 13)) {
                        ev.preventDefault();
                        this.drawButton.click();
                    }
                }
            }
        }
    });
    
    Bridge.define('Mandelbrot.Canvas.Extensions', {
        statics: {
            setPixel: function (img, x, y, color) {
                var index = ((((x + ((y * (img.width | 0)) | 0)) | 0)) * 4) | 0;
                img.data[index] = color.red;
                img.data[((index + 1) | 0)] = color.green;
                img.data[((index + 2) | 0)] = color.blue;
                img.data[((index + 3) | 0)] = 255; // alpha
            }
        }
    });
    
    Bridge.define('Mandelbrot.Canvas.Mandelbrot', {
        config: {
            properties: {
                Settings: null
            }
        },
        constructor: function (settings) {
            this.$initialize();
            this.setSettings(settings);
        },
        magnitude: function (z) {
            return Math.sqrt(z.re * z.re + z.im * z.im);
        },
        multiply: function (z, u) {
            return { re: z.re * u.re - z.im * u.im, im: z.re * u.im + u.re * z.im };
        },
        add: function (z, u) {
            return { re: z.re + u.re, im: z.im + u.im };
        },
        rescale: function (value, realRange, projection) {
            if (value > realRange.to || value < realRange.from) {
                throw new System.ArgumentException("value is not the real range");
            }
    
            var percentageOfProjection = (Math.abs(projection.to - projection.from) * Math.abs(value - realRange.from)) / Math.abs(realRange.to - realRange.from);
    
            return percentageOfProjection + projection.from;
        },
        fromPointOnCanvas: function (p, xmin, xmax, ymin, ymax, height, width) {
            return { re: this.rescale(p.x, { from: 0, to: width }, { from: xmin, to: xmax }), im: this.rescale(p.y, { from: 0, to: height }, { from: ymin, to: ymax }) };
        },
        iterationsUntilDivergence: function (z) {
            var constant = this.getSettings().useJuliaSet ? this.getSettings().juliaSetParameter : { re: z.re, im: z.im };
    
            var count = 0;
    
            while (true) {
                z = this.add(this.multiply(z, z), constant);
                count = (count + 1) | 0;
    
                if (this.magnitude(z) > this.getSettings().maxRadius || count >= this.getSettings().maxIterations) {
                    break;
                }
            }
    
    
            z = this.add(this.multiply(z, z), constant);
            count = (count + 1) | 0;
            z = this.add(this.multiply(z, z), constant);
            count = (count + 1) | 0;
    
    
            return count;
        },
        colorLookup: function (count, maxCount, z) {
            var bytes = System.Array.init(3, 0);
            var scale = (this.getSettings().colorScale) >>> 0;
            var shift = (this.getSettings().colorOffset) >>> 0;
    
            var log2 = Math.log(50);
    
            if (count < this.getSettings().maxIterations) {
                var modulus = Math.sqrt(z.re * z.re + z.im * z.im);
                var a = Math.log(modulus);
                if (isNaN(a)) {
                    a = 0;
                }
    
                var b = Math.log(a);
                if (isNaN(b)) {
                    b = 0;
                }
    
                var c = b / log2;
                var mu = count - c;
    
                var p = scale * mu / this.getSettings().maxIterations;
    
                var i;
                if (p > scale) {
                    i = scale;
                }
                else  {
                    if (p < 0) {
                        i = 0;
                    }
                    else  {
                        //Console.WriteLine("{0} {1} {2} {3} {4}",  modulus, a, b, c, p);
                        i = (System.Convert.toUInt32(p) + shift) >>> 0;
                    }
                }
    
                bytes[2] = ((i >>> 16)) & 255;
                bytes[1] = ((i >>> 8)) & 255;
                bytes[0] = i & 255;
            }
            else  {
                bytes[2] = 0;
                bytes[1] = 0;
                bytes[0] = 0;
            }
    
            var color = { red: bytes[2], green: bytes[1], blue: bytes[0] };
    
            return color;
        },
        calculatePoint: function (x, y, height, width) {
            // complex plain viewport
            var xmin = -1.5;
            var xmax = 1.0;
            var ymin = -2.0;
            var ymax = 2.0;
    
            var point = { x: x, y: y };
            var complex = this.fromPointOnCanvas(point, xmin, xmax, ymin, ymax, height, width);
            var iterations = this.iterationsUntilDivergence(complex);
    
            var color;
            if (this.getSettings().useColorMap) {
                color = this.colorLookup(iterations, this.getSettings().maxIterations, complex);
            }
            else  {
                color = { red: (iterations & 255), green: (iterations & 255), blue: (iterations & 255) };
            }
    
            return color;
        }
    });
    
    Bridge.define('Mandelbrot.Canvas.Options', {
        maxIterations: 1000,
        maxRadius: 10,
        useColorMap: true,
        colorOffset: 300,
        colorScale: 10000,
        useJuliaSet: false,
        juliaSetParameter: null,
        config: {
            init: function () {
                this.juliaSetParameter = { re: -0.735, im: 0.175 };
            }
        }
    });
    
    Bridge.init();
})(this);
