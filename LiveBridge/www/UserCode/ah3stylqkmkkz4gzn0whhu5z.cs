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
        List<string> list = new List<string>();
        list.Add("Jim");
        list.Add("Daniil");
        var filter = from l in list
                     where l.Contains("i")
                     select l;
                     
        Console.WriteLine("Hello Bridge.NET"); 
        foreach (string s in filter)
        {
            Console.Log(s);
        }
    }
}}