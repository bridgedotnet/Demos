namespace Mandelbrot.Canvas
{
    public class App
    {
        public static void Main()
        {
            var settings = new Options();

            var calculator = new Mandelbrot(settings);

            var drawer = new Drawer(settings, calculator);
        }
    }
}