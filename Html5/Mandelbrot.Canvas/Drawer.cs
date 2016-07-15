using Bridge;
using Bridge.Html5;

using System;
using System.Collections.Generic;

namespace Mandelbrot.Canvas
{


    class Drawer
    {
        private CanvasRenderingContext2D CanvasRenderingContext;
        private ImageData Image;
        private HTMLButtonElement DrawButton;
        private HTMLInputElement IterationCountElement;
        private HTMLInputElement RadiusElement;

        private HTMLInputElement ColorMapCheckbox;
        private HTMLInputElement ColorOffsetElement;
        private HTMLInputElement ColorScaleElement;

        private HTMLInputElement JuliaSetCheckbox;
        private HTMLInputElement JuliaReElement;
        private HTMLInputElement JuliaImElement;

        private HTMLInputElement XMinElement;
        private HTMLInputElement XMaxElement;
        private HTMLInputElement YMinElement;
        private HTMLInputElement YMaxElement;

        private int CurrentY;
        private Mandelbrot Calculator;
        private Options Settings;

        public Drawer(Options settings, Mandelbrot calculator)
        {
            this.Settings = settings;
            this.Calculator = calculator;

            // the actual canvas element
            var canvas = new HTMLCanvasElement();
            canvas.Width = 900;
            canvas.Height = 500;

            DrawButton = new HTMLButtonElement
            {
                InnerHTML = "Draw the Mandelbrot fractal",
                OnClick = (ev) =>
                {
                    StartDraw(canvas);
                }
            };

            DrawButton.SetAttribute("style", "font-size:18px;height: 60px;  width:95%; border: 2px solid black; cursor: pointer");

            // Iteration controls
            RadiusElement = GetInputNumberElement(null, this.Settings.MaxRadius, 3, 0.5);
            IterationCountElement = GetInputNumberElement(null, this.Settings.MaxIterations, 4, 100, 0, 100000);
            // Color controls
            ColorMapCheckbox = GetCheckboxElement(this.Settings.UseColorMap);
            ColorScaleElement = GetInputNumberElement(ColorMapCheckbox, this.Settings.ColorScale, 5, 1000);
            ColorOffsetElement = GetInputNumberElement(ColorMapCheckbox, this.Settings.ColorOffset, 4, 10);

            // Julia sets
            JuliaSetCheckbox = GetCheckboxElement(this.Settings.UseJuliaSet);
            JuliaImElement = GetInputNumberElement(JuliaSetCheckbox, this.Settings.JuliaSetParameter.Im, 5, 0.005, null);
            JuliaReElement = GetInputNumberElement(JuliaSetCheckbox, this.Settings.JuliaSetParameter.Re, 5,  0.005, null);

            // Viewport controls
            XMinElement = GetInputNumberElement(null, this.Settings.XMin, 5, 0.005, -5.0);
            XMaxElement = GetInputNumberElement(null, this.Settings.XMax, 5, 0.005, 0.0);
            YMinElement = GetInputNumberElement(null, this.Settings.YMin, 5, 0.005, -5.0);
            YMaxElement = GetInputNumberElement(null, this.Settings.YMax, 5, 0.005, 0.0);

            var paramsColumn = new HTMLTableDataCellElement();
            var canvasColumn = new HTMLTableDataCellElement();
            paramsColumn.SetAttribute("valign", "top");
            canvasColumn.SetAttribute("valign", "top");
            canvasColumn.AppendChild(canvas);

            var layoutRow = new HTMLTableRowElement();
            layoutRow.AppendChildren(paramsColumn, canvasColumn);

            var layout = new HTMLTableElement();

            var paramsTable = new HTMLTableElement();
            paramsTable.AppendChildren(
                Row(Label("XMin: "), XMinElement),
                Row(Label("XMax: "), XMaxElement),
                Row(Label("YMin: "), YMinElement),
                Row(Label("YMax: "), YMaxElement),
                Row(Label("Escape radius: "), RadiusElement),
                Row(Label("Iteration count: "), IterationCountElement),
                Row(Label("Use color map: "), ColorMapCheckbox),
                Row(Label("Color scale: "), ColorScaleElement),
                Row(Label("Color offset: "), ColorOffsetElement),
                Row(Label("Use Julia set: "), JuliaSetCheckbox),
                Row(Label("Im: "), JuliaImElement),
                Row(Label("Re: "), JuliaReElement),
                Row(new HTMLHRElement(), 2),
                Row(DrawButton, 2)
            );

            paramsColumn.AppendChild(paramsTable);

            layout.AppendChild(layoutRow);
            Document.Body.AppendChild(layout);
        }

        private HTMLLabelElement Label(string value)
        {
            var lbl = new HTMLLabelElement();
            lbl.InnerHTML = value;
            lbl.Style.Margin = "5px";
            lbl.Style.FontSize = "18px";
            return lbl;
        }
        private HTMLTableRowElement Row(params HTMLElement[] elements)
        {
            var row = new HTMLTableRowElement();
            foreach(var el in elements)
                row.AppendChild(el.WithinTableDataCell());
            return row;
        }

        private HTMLTableRowElement Row(HTMLElement el, int colspan)
        {
            var row = new HTMLTableRowElement();
            var td = new HTMLTableDataCellElement();
            td.SetAttribute("colspan", colspan.ToString());
            td.AppendChild(el);
            row.AppendChild(td);
            return row;
        }

        private HTMLInputElement GetCheckboxElement(bool isChecked)
        {
            var checkbox = new HTMLInputElement
            {
                Type = InputType.Checkbox,
                Checked = isChecked,
            };

            // Extend the checkbox
            var elements = GetElementListToEnable(checkbox);

            Action<Event<HTMLInputElement>> enableInputs = (ev) =>
            {
                foreach (var control in elements)
                {
                    control.Disabled = !checkbox.Checked;
                }
            };
            checkbox.OnChange = enableInputs;

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
        }

        private List<HTMLInputElement> GetElementListToEnable(HTMLInputElement element)
        {
            var ce = element["toEnable"];
            List<HTMLInputElement> elements;

            if (ce == null || (elements = ce as List<HTMLInputElement>) == null)
            {
                elements = new List<HTMLInputElement>();
                element["toEnable"] = elements;
            }

            return elements;
        }

        private HTMLInputElement GetInputNumberElement(HTMLInputElement enableElement, double initialValue, int widthInDigits, double? step = 1, double? min = 0, double? max = null)
        {
            var element = new HTMLInputElement()
            {
                Type = InputType.Number,
                Value = initialValue.ToString(),
                OnKeyUp = (ev) =>
                {
                    if (ev.IsKeyboardEvent())
                    {
                        var kev = ev as KeyboardEvent<HTMLInputElement>;

                        if (kev != null)
                        {
                            if (object.Equals(kev["keyCode"], 13))
                            {
                                ev.PreventDefault();
                                DrawButton.Click();
                            }
                        }
                    }
                }
            };

            if (enableElement != null)
            {
                element.Disabled = !enableElement.Checked;

                var elements = GetElementListToEnable(enableElement);
                elements.Add(element);
            }

            if (step.HasValue)
            {
                element.Step = step.ToString();
            }

            if (min.HasValue)
            {
                element.Min = min.ToString();
            }

            if (max.HasValue)
            {
                element.Max = max.ToString();
            }

            element.Style.Width = widthInDigits + "em";
            element.Style.Margin = "5px";

            return element;
        }

        private void GetImageData(HTMLCanvasElement canvas)
        {
            int height = canvas.Height;
            int width = canvas.Width;

            if (CanvasRenderingContext == null)
            {
                CanvasRenderingContext = canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            }

            if (Image == null)
            {
                // the data to manipulate
                Image = CanvasRenderingContext.CreateImageData(width, height);
            }
            else
            {
                //var color = new Color() { Red = 0, Green = 0, Blue = 0 };

                //for (int y = 0; y < height; y++)
                //{
                //    for (int x = 0; x < width; x++)
                //    {
                //        Image.SetPixel(x, y, color);
                //    }
                //}
            }
        }

        private void StartDraw(HTMLCanvasElement canvas)
        {
            if (!GetInputValue(IterationCountElement, out this.Settings.MaxIterations))
            {
                Window.Alert("Iteration count should be positive integer");
                return;
            }

            if (!GetInputValue(RadiusElement, out this.Settings.MaxRadius))
            {
                Window.Alert("Escape radius should be positive integer");
                return;
            }

            if (!GetInputValue(XMinElement, out this.Settings.XMin))
            {
                Window.Alert("XMin should be a number");
                return;
            }

            if (!GetInputValue(XMaxElement, out this.Settings.XMax))
            {
                Window.Alert("XMax should be a number");
                return;
            }

            if (!GetInputValue(YMinElement, out this.Settings.YMin))
            {
                Window.Alert("YMin should be a number");
                return;
            }

            if (!GetInputValue(YMaxElement, out this.Settings.YMax))
            {
                Window.Alert("YMax should be a number");
                return;
            }


            this.Settings.UseColorMap = ColorMapCheckbox.Checked;
            if (this.Settings.UseColorMap)
            {
                if (!GetInputValue(ColorScaleElement, out this.Settings.ColorScale))
                {
                    Window.Alert("Color scale should be positive integer");
                    return;
                }

                if (!GetInputValue(ColorOffsetElement, out this.Settings.ColorOffset))
                {
                    Window.Alert("Color offset should be positive integer");
                    return;
                }
            }

            this.Settings.UseJuliaSet = JuliaSetCheckbox.Checked;
            if (this.Settings.UseJuliaSet)
            {
                if (!GetInputValue(JuliaImElement, out this.Settings.JuliaSetParameter.Im))
                {
                    Window.Alert("Julia Im should be a number");
                    return;
                }

                if (!GetInputValue(JuliaReElement, out this.Settings.JuliaSetParameter.Re))
                {
                    Window.Alert("Julia Re should be a number");
                    return;
                }
            }

            GetImageData(canvas);

            CurrentY = 0;
            DrawPart();
        }

        private void DrawPart()
        {
            int height = CanvasRenderingContext.Canvas.Height;
            int width = CanvasRenderingContext.Canvas.Width;

            if (CurrentY < height)
            {
                for (int x = 0; x < width; x++)
                {
                    var color = this.Calculator.CalculatePoint(x, CurrentY, height, width);

                    Image.SetPixel(x, CurrentY, color);
                }

                CanvasRenderingContext.PutImageData(Image, 0, 0);
                CurrentY++;
            }

            if (CurrentY < height)
            {
                Window.SetTimeout(DrawPart, 0);
            }
        }

        private static bool GetInputValue(HTMLInputElement source, out int number)
        {
            var d = Script.ParseFloat(source.Value);
            if (double.IsNaN(d))
            {
                number = 0;
                return false;
            }

            number = (int)Math.Round(d);
            return true;
        }

        private static bool GetInputValue(HTMLInputElement source, out double number)
        {
            number = Script.ParseFloat(source.Value);
            if (double.IsNaN(number))
            {
                return false;
            }

            return true;
        }
    }
}