using Bridge;
using Bridge.Html5;
using System;

namespace Mandelbrot.Canvas
{
    [ObjectLiteral]
    class Complex
    {
        public double Re { get; set; }
        public double Im { get; set; }
    }

    [ObjectLiteral]
    class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    [ObjectLiteral]
    class Interval
    {
        public double From { get; set; }
        public double To { get; set; }
    }

    [ObjectLiteral]
    class Color
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
    }

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

    public class App
    {
        // gives the absolute value of a complex number 
        static double Magnitude(Complex z)
        {
            return Math.Sqrt(z.Re * z.Re + z.Im * z.Im);
        }
        // Multiplication of complex numbers
        static Complex Multiply(Complex z, Complex u)
        {
            return new Complex
            {
                Re = z.Re * u.Re - z.Im * u.Im,
                Im = z.Re * u.Im + u.Re * z.Im
            };
        }
        // Addition of complex numbers
        static Complex Add(Complex z, Complex u)
        {
            return new Complex
            {
                Re = z.Re + u.Re,
                Im = z.Im + u.Im
            };
        }


        // Interval transformation
        static double Rescale(double value, Interval realRange, Interval projection)
        {
            if (value > realRange.To || value < realRange.From)
                throw new ArgumentException("value is not the real range");

            var intervalRealDistance = Math.Abs(realRange.To - realRange.From);

            var intervalProjectionDistance = Math.Abs(projection.To - projection.From);

            var percentageOfRealDistance = Math.Abs(value - realRange.From) / intervalRealDistance;

            var percentageOfProjection = percentageOfRealDistance * intervalProjectionDistance;

            return percentageOfProjection + projection.From;
        }

        static Complex FromPointOnCanvas(Point p, double xmin, double xmax, double ymin, double ymax, double height, double width)
        {
            return new Complex
            {
                Re = Rescale(p.X,
                        new Interval { From = 0.0, To = width },
                        new Interval { From = xmin, To = xmax }),

                Im = Rescale(p.Y,
                        new Interval { From = 0.0, To = height },
                        new Interval { From = ymin, To = ymax }),
            };
        }

        static int IterationsUntilDivergence(Complex z)
        {
            var constant = new Complex { Re = z.Re, Im = z.Im };
            var count = 0;
            var maxIterations = 255;
            while (Magnitude(z) <= 2 && count < maxIterations)
            {
                z = Add(Multiply(z, z), constant);
                count = count + 1;
            }
            return count;
        }

        public static void Main()
        {
            // canvas size
            var height = 700;
            var width = 900;

            // complex plain viewport
            var xmin = -1.5;
            var xmax = 1.0;
            var ymin = -2.0;
            var ymax = 2.0;

            // the actual canvas element
            var canvas = new HTMLCanvasElement();
            canvas.Width = width;
            canvas.Height = (int)height;


            var div = new HTMLButtonElement
            {
                InnerHTML = "Draw the Mandelbrot fractal",
                OnClick = (ev) =>
                {
                    var ctx = canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
                    // the data to manipulate
                    var img = ctx.CreateImageData(width, (int)height);
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            var point = new Point { X = x, Y = y };
                            var complex = FromPointOnCanvas(point, xmin, xmax, ymin, ymax, height, width);
                            var iterations = IterationsUntilDivergence(complex);
                            // the use of an extension method
                            img.SetPixel(x, y, new Color
                            {
                                Red = (byte)iterations,
                                Green = (byte)iterations,
                                Blue = (byte)iterations
                            });
                        }
                    }
                    ctx.PutImageData(img, 0, 0);
                }
            };

            div.SetAttribute("style", "font-size:30px, height:80px; width:180px; border-radius:10px; border: 2px solid black; cursor: pointer");
            Document.Body.AppendChild(div);
            Document.Body.AppendChild(new HTMLHRElement());
            Document.Body.AppendChild(canvas);
        }
    }
}