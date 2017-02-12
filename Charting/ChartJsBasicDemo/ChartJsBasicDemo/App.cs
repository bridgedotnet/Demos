using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChartJS;
using Bridge.Html5;

namespace ChartJsBasicDemo
{ 
    public class App
    {
        static IEnumerable<int> Random(int count)
        {
            var random = new Random();
            for (int i = 1; i <= count; i++)
                yield return random.Next(1, 100);
        }


        static string RGB(byte red, byte green, byte blue, double opacity) => $"rgba({red}, {green}, {blue}, {opacity})";

        public static void Main()
        {
            var canvas = Document.GetElementById("chartCanvas") as HTMLCanvasElement;
            var width = Window.InnerWidth;

            var radarChart = new Chart(canvas, new ChartConfig
            {
                Type = ChartType.Radar,
                Data = new DataConfig
                {
                    Labels = new[] { "Eating", "Drinking", "Sleeping", "Designing", "Coding", "Cycling", "Running" },
                    Datasets = new Dataset[]
                    {
                        new RadarDataset
                        {
                            Label = "Today",
                            Data = new int[] { 65, 59, 90, 81, 56, 55, 40 },
                            PointRadius = 10,
                            PointHoverRadius = 10,
                            BackgroundColor = RGB(0, 100, 255, 0.5),
                            PointHoverBackgroundColor = "blue",

                        },
                        new RadarDataset
                        {
                            Label = "Yesterday",
                            Data = new int [] { 28, 48, 40, 19, 96, 27, 100 },
                            PointRadius = 10,
                            PointHoverRadius = 10,
                            BackgroundColor = RGB(0, 255, 100, 0.4),
                            PointHoverBackgroundColor = "green",
                        }
                    }
                },
                Options = new ChartOptions
                {
                    Responsive = true
                }
            });

            Window.SetInterval(() =>
            {
                var fstDataset = radarChart.Data.Datasets[0].As<RadarDataset>();
                fstDataset.Data = Random(7).ToArray();

                var sndDataset = radarChart.Data.Datasets[1].As<RadarDataset>();
                sndDataset.Data = Random(7).ToArray();

                radarChart.Update();

            }, 2000);
        }
    }
}
