namespace Mandelbrot.Canvas
{
    class Options
    {
        public double XMin = -2.0;
        public double XMax = 2.0;
        public double YMin = -2.0;
        public double YMax = 2.0;
        public int MaxIterations = 1000;
        public double MaxRadius = 10;
        public bool UseColorMap = true;
        public int ColorOffset = 300;
        public int ColorScale = 10000;
        public bool UseJuliaSet = false;
        public Complex JuliaSetParameter = new Complex { Re = -0.735, Im = 0.175 };
    }
}