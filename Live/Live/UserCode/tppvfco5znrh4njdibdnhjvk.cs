using Bridge;
            using Bridge.Bootstrap3;
            using Bridge.Html5;
            using Bridge.jQuery2;
            using Bridge.Linq;
            using Bridge.WebGL;
            using System.Linq;
            using System.Text;
            using System.Collections.Generic;

            namespace LiveBridgeBuilder
            {public class App
{
    [Ready]
    public static void Main()
    {
        string[] names = { "Daniil", "Fabricio", "Geoffrey", "Leonid", "Ozgur" };
        IEnumerable query =
            from n in names
            where n.Contains("i")
            orderby n
            select n.ToUpperCase();
 
        foreach (string q in query)
        {
            Console.WriteLine(q);
        }
    }
}
}