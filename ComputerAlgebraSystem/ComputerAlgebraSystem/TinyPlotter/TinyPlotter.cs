using System;
using System.Collections.Generic;
using Bridge.Html5;

namespace ComputerAlgebraSystem
{
    public class TinyPlotter
    {
        public PlotSettings Settings;
        public HTMLCanvasElement Canvas;

        public TinyPlotter(PlotSettings settings)
        {
            Settings = settings;
            Canvas = new HTMLCanvasElement();
            Canvas.Height = Settings.Height;
            Canvas.Width = Settings.Width;
            Canvas.Style.Border = "1px solid black";
            var ctx = Canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            var image = ctx.CreateImageData(Canvas.Width, Canvas.Height);
            if (Settings.DrawXAxis)
                DrawXAxis(image);
            if (Settings.DrawXAxis)
                DrawYAxis(image);
            ctx.PutImageData(image, 0, 0);
        }

        public void Draw()
        {
            var ctx = Canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            var image = ctx.CreateImageData(Canvas.Width, Canvas.Height);

            if (Settings.DrawXAxis)
                DrawXAxis(image);

            if (Settings.DrawXAxis)
                DrawYAxis(image);

            foreach (var curve in Settings.Curves)
                DrawCurve(curve, image);

            ctx.PutImageData(image, 0, 0);
        }

        void DrawXAxis(ImageData image)
        {
            var xmin = Settings.Viewport.XMin;
            var xmax = Settings.Viewport.XMax;
            var ymin = Settings.Viewport.YMin;
            var ymax = Settings.Viewport.YMax;
            var step = Settings.StepX;

            for (double x = xmin; x <= xmax; x += 0.01)
            {
                var point = new Point { X = x, Y = 0 };
                var pointFromPlain = FromPointOnPlain(point, xmin, xmax, ymin, ymax, Canvas.Height, Canvas.Width);
                SetPixel(image, (int)pointFromPlain.X, (int)pointFromPlain.Y, new Color { Red = 0, Blue = 0, Green = 0 });
            }
        }

        void DrawYAxis(ImageData image)
        {
            var xmin = Settings.Viewport.XMin;
            var xmax = Settings.Viewport.XMax;
            var ymin = Settings.Viewport.YMin;
            var ymax = Settings.Viewport.YMax;
            var step = Settings.StepX;

            for (double y = ymin; y <= ymax; y += 0.01)
            {
                var point = new Point { X = 0, Y = y };
                var pointFromPlain = FromPointOnPlain(point, xmin, xmax, ymin, ymax, Canvas.Height, Canvas.Width);
                SetPixel(image, (int)pointFromPlain.X, (int)pointFromPlain.Y, new Color { Red = 0, Blue = 0, Green = 0 });
            }
        }
        void DrawCurve(Curve curve, ImageData image)
        {
            var xmin = Settings.Viewport.XMin;
            var xmax = Settings.Viewport.XMax;
            var ymin = Settings.Viewport.YMin;
            var ymax = Settings.Viewport.YMax;
            var step = Settings.StepX;

            for (double x = xmin; x <= xmax; x += step)
            {
                var y = curve.Map(x);
                if (y < ymin || y > ymax || double.IsNaN(y)) // off bounds or undefined
                    continue;

                var point = new Point { X = x, Y = y };
                var pointFromPlain = FromPointOnPlain(point, xmin, xmax, ymin, ymax, Canvas.Height, Canvas.Width);
                SetPixel(image, (int)pointFromPlain.X, (int)pointFromPlain.Y, curve.Color);
            }
        }


        double Rescale(double value, Interval realRange, Interval projection)
        {
            if (value > realRange.To || value < realRange.From)
            {
                throw new ArgumentException("value is not the real range");
            }

            var percentageOfProjection = (Math.Abs(projection.To - projection.From) * Math.Abs(value - realRange.From)) / Math.Abs(realRange.To - realRange.From);

            return percentageOfProjection + projection.From;
        }


        Point FromPointOnPlain(Point p, double xmin, double xmax, double ymin, double ymax, double height, double width)
        {
            return new Point
            {
                X = Rescale(p.X,
                         new Interval { From = xmin, To = xmax },
                         new Interval { From = 0, To = width }),


                Y = height - Rescale(p.Y,
                                new Interval { From = ymin, To = ymax },
                                new Interval { From = 0, To = height }),
            };
        }

        void SetPixel(ImageData img, int x, int y, Color color)
        {
            int index = (x + y * (int)img.Width) * 4;

            if (index > img.Data.Length - 4)
            {
                return;
            }

            img.Data[index] = color.Red;
            img.Data[index + 1] = color.Green;
            img.Data[index + 2] = color.Blue;

            img.Data[index + 3] = 255; // alpha
        }
    }
}