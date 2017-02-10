using Bridge.Html5;

namespace ComputerAlgebraSystem
{
    public static class Extensions
    {
        public static HTMLTableDataCellElement WithinTableDataCell(this HTMLElement el)
        {
            var td = new HTMLTableDataCellElement();
            td.SetAttribute("valign", "top");
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