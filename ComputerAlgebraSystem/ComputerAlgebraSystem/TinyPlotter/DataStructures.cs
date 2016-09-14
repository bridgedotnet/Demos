using Bridge;
using System;
using System.Collections.Generic;

namespace ComputerAlgebraSystem
{
    [ObjectLiteral]
    public class Interval
    {
        public double From;
        public double To;
    }

    [ObjectLiteral]
    public class Color
    {
        public byte Red;
        public byte Green;
        public byte Blue;
    }

    [ObjectLiteral]
    public class Point
    {
        public double X;
        public double Y;
    }

    [ObjectLiteral]
    public class Viewport
    {
        public double XMin { get; set; }
        public double XMax { get; set; }
        public double YMin { get; set; }
        public double YMax { get; set; }
    }

    [ObjectLiteral]
    public class Curve
    {
        public Func<double, double> Map { get; set; }
        public Color Color { get; set; }
    }

    [ObjectLiteral]
    public class PlotSettings
    {
        public List<Curve> Curves;
        public Viewport Viewport;
        public double StepX;
        public int Height;
        public int Width;
        public bool DrawXAxis;
        public bool DrawYAxis;
    }
}
