using Bridge.Html5;

namespace Mandelbrot.Canvas
{
    public static class Extensions
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

        public static HTMLTableDataCellElement WithinTableDataCell(this HTMLElement el)
        {
            var td = new HTMLTableDataCellElement();
            td.AppendChild(el);
            return td;
        }

        public static void AppendChildren(this HTMLElement parent, params HTMLElement[] elems)
        {
            foreach (var element in elems)
                parent.AppendChild(element);
        }
    }
}