using Bridge;
using Bridge.Html5;

namespace Mandelbrot.Canvas
{
    [ObjectLiteral]
    public class Complex
    {
        public double Re;
        public double Im;
    }

    [ObjectLiteral]
    public class Point
    {
        public int X;
        public int Y;
    }

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
}