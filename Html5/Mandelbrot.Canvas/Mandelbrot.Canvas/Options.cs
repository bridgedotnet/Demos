namespace Mandelbrot.Canvas
{
    class Options
    {
        public int MaxIterations = 1000;
        public double MaxRadius = 10;
        public bool UseColorMap = true;
        public int ColorOffset = 300;
        public int ColorScale = 10000;
        public bool UseJuliaSet = false;
        public Complex JuliaSetParameter = new Complex { Re = -0.735, Im = 0.175 };
    }
}