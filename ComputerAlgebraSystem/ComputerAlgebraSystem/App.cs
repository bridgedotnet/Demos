using Bridge.Html5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerAlgebraSystem
{
    public class App
    {
        public static void Main()
        {
            var settings = new PlotSettings
            {
                Curves = new List<Curve>(),
                Viewport = new Viewport
                {
                    XMin = -2 * Math.PI,
                    XMax = 2 * Math.PI,
                    YMin = -5,
                    YMax = 5
                },

                Height = 500,
                Width = 800,
                StepX = 0.01,
                DrawXAxis = true,
                DrawYAxis = true
            };


            var layout = new Layout(settings);

            layout.AppendTo(Document.Body);
        }
    }
}
