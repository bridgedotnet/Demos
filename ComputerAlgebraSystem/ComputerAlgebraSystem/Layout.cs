using System;
using Bridge;
using Bridge.Html5;

namespace ComputerAlgebraSystem
{
    public class Layout
    {
        private HTMLElement container;
        public Layout(PlotSettings settings)
        {
            var plotter = new TinyPlotter(settings);

            var exprInput = new HTMLInputElement { Type = InputType.Text };
            exprInput.Value = "sin(3x)";

            var evalInput = new HTMLInputElement { Type = InputType.Text };
            evalInput.Value = "plus(5^2, sin(div(pi, 2)))";

            var variableInput = new HTMLInputElement { Type = InputType.Text };
            variableInput.Value = "x";

            var deltaXInput = new HTMLInputElement { Type = InputType.Text };
            deltaXInput.Value = "0.005";

            var xminInput = new HTMLInputElement { Type = InputType.Text };
            xminInput.Value = settings.Viewport.XMin.ToString();

            var xmaxInput = new HTMLInputElement { Type = InputType.Text };
            xmaxInput.Value = settings.Viewport.XMax.ToString();

            var yminInput = new HTMLInputElement { Type = InputType.Text };
            yminInput.Value = settings.Viewport.YMin.ToString();

            var ymaxInput = new HTMLInputElement { Type = InputType.Text };
            ymaxInput.Value = settings.Viewport.YMax.ToString();

            var resultDiv = new HTMLDivElement();
            resultDiv.Style.FontSize = "18px";
            resultDiv.Style.MaxWidth = "300px";

            var btnPlot = new HTMLButtonElement
            {
                InnerHTML = "Plot with derivative",
                OnClick = ev =>
                {
                    Func<HTMLInputElement, bool> IsNaN = x => double.IsNaN(Script.ParseFloat(x.Value));

                    var isNotValid = exprInput.Value == ""
                                  || variableInput.Value == ""
                                  || IsNaN(deltaXInput)
                                  || IsNaN(xminInput)
                                  || IsNaN(xmaxInput)
                                  || IsNaN(yminInput)
                                  || IsNaN(ymaxInput);


                    if (isNotValid)
                    {
                        Write("<h1 style='color:red'>Input is not valid!</h1>", resultDiv);
                        return;
                    }

                    var result = Parser.TryParseInput(exprInput.Value);
                    if (result.WasSuccessful)
                    {
                        // set the settings
                        plotter.Settings.StepX = Script.ParseFloat(deltaXInput.Value);
                        plotter.Settings.Viewport.XMin = Script.ParseFloat(xminInput.Value);
                        plotter.Settings.Viewport.XMax = Script.ParseFloat(xmaxInput.Value);
                        plotter.Settings.Viewport.YMin = Script.ParseFloat(yminInput.Value);
                        plotter.Settings.Viewport.YMax = Script.ParseFloat(ymaxInput.Value);

                        resultDiv.InnerHTML = "";
                        var f = result.Value;
                        var df = Expr.Differentiate(f, variableInput.Value);

                        var fLambda = Expr.Lambdify(f, variableInput.Value);
                        var dfLambda = Expr.Lambdify(df, variableInput.Value);
                        var curveColor = RandomColor();

                        plotter.Settings.Curves.Clear();
                        plotter.Settings.Curves.Add(new Curve { Map = fLambda, Color = curveColor });
                        plotter.Settings.Curves.Add(new Curve { Map = dfLambda, Color = Grayscale(curveColor) });
                        plotter.Draw();

                        var rgbCurveColor = RGB(curveColor);
                        var rgbGrayColor = RGB(Grayscale(curveColor));

                        var msgParsed = "<strong style='color:" + rgbCurveColor + "'>" + f.ToString() + "</strong>";
                        var derivative = "<strong style='color:" + rgbGrayColor + "'>" + df.ToString() + "</strong>";
                        Write("<hr /> Parsed: <br />" + msgParsed + "<br /> Derivative: <br /> " + derivative + "<hr />", resultDiv);
                    }
                    else
                    {
                        var error = string.Join("<br />", result.Expectations);
                        Write("<h1 style='color:red'>" + error + "</h1>", resultDiv);
                    }

                }
            };


            var btnEvaluate = new HTMLButtonElement
            {
                InnerHTML = "Evaluate",
                OnClick = ev =>
                {
                    if (evalInput.Value == "")
                    {
                        Write("<h1 style='color:red'>Input is not valid!</h1>", resultDiv);
                        return;
                    }

                    var result = Parser.TryParseInput(evalInput.Value);
                    if (result.WasSuccessful)
                    {
                        resultDiv.InnerHTML = "";
                        var expression = result.Value;
                        var eval = Expr.Evaluate(expression);

                        Write("<h4 style='color:green'>" +
                                "Parsed: " + expression.ToString() + "<br />" +
                                "Answer: " + eval.ToString()
                            + "</h4>", resultDiv);
                    }
                    else
                    {
                        var error = string.Join("<br />", result.Expectations);
                        Write("<h1 style='color:red'>" + error + "</h1>", resultDiv);
                    }


                }
            };

            var slider = new HTMLInputElement { Type = InputType.Range };

            btnEvaluate.Style.Width = "90%";
            btnEvaluate.Style.Margin = "5px";
            btnEvaluate.Style.Height = "40px";
            btnPlot.Style.Margin = "5px";
            btnPlot.Style.Height = "40px";
            btnPlot.Style.Width = "90%";

            var layout = Table(
                    Row(Table(
                            Row(Label("Expression"), exprInput),
                            Row(Label("Variable"), variableInput),
                            Row(Label("XAxis step"), deltaXInput),
                            Row(Label("XMin"), xminInput),
                            Row(Label("XMax"), xmaxInput),
                            Row(Label("YMin"), yminInput),
                            Row(Label("YMax"), ymaxInput),
                            Row(btnPlot, 2),
                            Row(new HTMLHRElement(), 2),
                            Row(Label("Expression"), evalInput),
                            Row(btnEvaluate, 2),
                            Row(resultDiv, 2)),
                        Table(
                            Row(plotter.Canvas)))
                );

            this.container = layout;
        }

        public void AppendTo(HTMLElement el)
        {
            el.AppendChild(this.container);
        }

        private HTMLTableElement Table(params HTMLTableRowElement[] rows)
        {
            var table = new HTMLTableElement();
            table.AppendChildren(rows);
            return table;
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
            foreach (var el in elements)
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

        private void Write(string expr, HTMLDivElement el)
        {
            el.InnerHTML = expr;
        }

        private Color RandomColor()
        {
            var random = new Random();
            return new Color
            {
                Blue = (byte)random.Next(0, 255),
                Green = (byte)random.Next(0, 255),
                Red = (byte)random.Next(0, 50)
            };
        }

        private Color Grayscale(Color color)
        {
            byte avg = (byte)((color.Red + color.Green + color.Blue) / 3.0);
            return new Color
            {
                Blue = avg,
                Green = avg,
                Red = avg
            };
        }

        private string RGB(Color color)
        {
            return string.Format("rgba({0}, {1}, {2}, 255)", color.Red, color.Green, color.Blue);
        }
    }
}