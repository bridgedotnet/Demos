using Bridge;
using Bridge.Html5;

using System;
using System.Collections.Generic;

namespace Mandelbrot.Canvas
{
    static class Extensions
    {
        // extension method
        public static void SetPixel(this ImageData img, int x, int y, Color color)
        {
            int index = (x + y * (int)img.Width) * 4;
            img.Data[index] = color.Red;
            img.Data[index + 1] = color.Green;
            img.Data[index + 2] = color.Blue;
            img.Data[index + 3] = 255; // alpha
        }
    }

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

            DrawButton.SetAttribute("style", "font-size:30px, height:80px; width:180px; border-radius:10px; border: 2px solid black; cursor: pointer");

            var parameterContainer = new HTMLDivElement();

            RadiusElement = GetInputNumberElement(parameterContainer, null, this.Settings.MaxRadius, 3, "Escape radius:", 0.5);
            IterationCountElement = GetInputNumberElement(parameterContainer, null, this.Settings.MaxIterations, 4, "Iteration count:", 100, 0, 100000);

            parameterContainer.AppendChild(new HTMLBRElement());

            ColorMapCheckbox = GetCheckboxElement(parameterContainer, "Use Color map ", this.Settings.UseColorMap);
            ColorScaleElement = GetInputNumberElement(parameterContainer, ColorMapCheckbox, this.Settings.ColorScale, 5, "Scale:", 1000);
            ColorOffsetElement = GetInputNumberElement(parameterContainer, ColorMapCheckbox, this.Settings.ColorOffset, 4, "Offset:", 10);

            parameterContainer.AppendChild(new HTMLBRElement());

            JuliaSetCheckbox = GetCheckboxElement(parameterContainer, "Use Julia set ", this.Settings.UseJuliaSet);
            JuliaImElement = GetInputNumberElement(parameterContainer, JuliaSetCheckbox, this.Settings.JuliaSetParameter.Im, 5, "Im:", 0.005, null);
            JuliaReElement = GetInputNumberElement(parameterContainer, JuliaSetCheckbox, this.Settings.JuliaSetParameter.Re, 5, "Re:", 0.005, null);

            parameterContainer.AppendChild(new HTMLBRElement());

            parameterContainer.AppendChild(DrawButton);

            Document.Body.AppendChild(parameterContainer);

            Document.Body.AppendChild(new HTMLHRElement());

            Document.Body.AppendChild(canvas);
        }


        private HTMLInputElement GetCheckboxElement(HTMLDivElement container, string title, bool isChecked)
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

            var label = new HTMLLabelElement
            {
                InnerHTML = title
            };

            label.Style.Margin = "5px";
            label.AppendChild(checkbox);

            if (container != null)
            {
                container.AppendChild(label);
            }

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

        private HTMLInputElement GetInputNumberElement(HTMLElement container, HTMLInputElement enableElement, double initialValue, int widthInDigits, string title, double? step = 1, double? min = 0, double? max = null)
        {
            var label = new HTMLLabelElement();
            label.InnerHTML = title;
            label.Style.Margin = "5px";

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

            label.AppendChild(element);

            if (container != null)
            {
                container.AppendChild(label);
            }

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