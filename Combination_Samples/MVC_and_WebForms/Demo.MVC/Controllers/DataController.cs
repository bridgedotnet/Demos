using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Demo.MVC.Controllers
{
    public class DataController : Controller
    {
        public string GetServerTime()
        {
            return JsonConvert.SerializeObject(DateTime.Now, new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd\\THH:mm:ss"
            });
        }

        public string LongRunningProcess()
        {
            // Mimic a long running process
            System.Threading.Thread.Sleep(5000);

            return Newtonsoft.Json.JsonConvert.SerializeObject("ok");
        }
    }
}
