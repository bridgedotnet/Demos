using Bridge;
using Bridge.Html5;

namespace Mandelbrot.Canvas
{
    [ObjectLiteral]
    class Complex
    {
        public double Re;
        public double Im;
    }

    [ObjectLiteral]
    class Point
    {
        public int X;
        public int Y;
    }

    [ObjectLiteral]
    class Interval
    {
        public double From;
        public double To;
    }

    [ObjectLiteral]
    class Color
    {
        public byte Red;
        public byte Green;
        public byte Blue;
    }
}