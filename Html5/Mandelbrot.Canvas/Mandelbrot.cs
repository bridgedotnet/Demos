using System;

namespace Mandelbrot.Canvas
{
    class Mandelbrot
    {
        public Options Settings { get; set; }

        public Mandelbrot(Options settings)
        {
            this.Settings = settings;
        }

        // gives the absolute value of a complex number 
        double Magnitude(Complex z)
        {
            return Math.Sqrt(z.Re * z.Re + z.Im * z.Im);
        }

        // Multiplication of complex numbers
        Complex Multiply(Complex z, Complex u)
        {
            return new Complex
            {
                Re = z.Re * u.Re - z.Im * u.Im,
                Im = z.Re * u.Im + u.Re * z.Im
            };
        }

        // Addition of complex numbers
        Complex Add(Complex z, Complex u)
        {
            return new Complex
            {
                Re = z.Re + u.Re,
                Im = z.Im + u.Im
            };
        }

        // Interval transformation
        double Rescale(double value, Interval realRange, Interval projection)
        {
            if (value > realRange.To || value < realRange.From)
                throw new ArgumentException("value is not the real range");

            var percentageOfProjection = (Math.Abs(projection.To - projection.From) * Math.Abs(value - realRange.From)) / Math.Abs(realRange.To - realRange.From);

            return percentageOfProjection + projection.From;
        }

        Complex FromPointOnCanvas(Point p, double xmin, double xmax, double ymin, double ymax, int height, int width)
        {
            return new Complex
            {
                Re = Rescale(p.X,
                        new Interval { From = 0, To = width },
                        new Interval { From = xmin, To = xmax }),

                Im = Rescale(p.Y,
                        new Interval { From = 0, To = height },
                        new Interval { From = ymin, To = ymax }),
            };
        }

        int IterationsUntilDivergence(Complex z)
        {
            var constant = this.Settings.UseJuliaSet
                ? this.Settings.JuliaSetParameter
                : new Complex { Re = z.Re, Im = z.Im };

            var count = 0;

            while (true)
            {
                z = Add(Multiply(z, z), constant);
                count++;

                if (Magnitude(z) > this.Settings.MaxRadius || count >= this.Settings.MaxIterations)
                {
                    break;
                }
            }


            z = Add(Multiply(z, z), constant);
            count++;
            z = Add(Multiply(z, z), constant);
            count++;


            return count;
        }

        Color ColorLookup(int count, int maxCount, Complex z)
        {
            byte[] bytes = new byte[3];
            uint scale = (uint)this.Settings.ColorScale;
            uint shift = (uint)this.Settings.ColorOffset;

            var log2 = Math.Log(50);

            if (count < this.Settings.MaxIterations)
            {
                var modulus = Math.Sqrt(z.Re * z.Re + z.Im * z.Im);
                var a = Math.Log(modulus);
                if (double.IsNaN(a))
                {
                    a = 0;
                }

                var b = Math.Log(a);
                if (double.IsNaN(b))
                {
                    b = 0;
                }

                var c = b / log2;
                double mu = count - c;

                var p = scale * mu / this.Settings.MaxIterations;

                UInt32 i;
                if (p > scale)
                {
                    i = scale;
                }
                else if (p < 0)
                {
                    i = 0;
                }
                else
                {
                    //Console.WriteLine("{0} {1} {2} {3} {4}",  modulus, a, b, c, p);
                    i = Convert.ToUInt32(p) + shift;
                }

                bytes[2] = (byte)(i >> 16);
                bytes[1] = (byte)(i >> 8);
                bytes[0] = (byte)i;
            }
            else
            {
                bytes[2] = 0;
                bytes[1] = 0;
                bytes[0] = 0;
            }

            var color = new Color()
            {
                Red = bytes[2],
                Green = bytes[1],
                Blue = bytes[0]
            };

            return color;
        }

        public Color CalculatePoint(int x, int y, int height, int width)
        {
            // complex plain viewport
            var xmin = -1.5;
            var xmax = 1.0;
            var ymin = -2.0;
            var ymax = 2.0;

            var point = new Point { X = x, Y = y };
            var complex = FromPointOnCanvas(point, xmin, xmax, ymin, ymax, height, width);
            var iterations = IterationsUntilDivergence(complex);

            Color color;
            if (this.Settings.UseColorMap)
            {
                color = ColorLookup(iterations, this.Settings.MaxIterations, complex);
            }
            else
            {
                color = new Color
                {
                    Red = (byte)iterations,
                    Green = (byte)iterations,
                    Blue = (byte)iterations
                };
            }

            return color;
        }
    }
}