using System;
using System.Web.Mvc;

namespace Demo.MVC.Controllers
{
    public class DataController : Controller
    {
        public string GetServerTime()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(DateTime.Now);
        }

        public string LongRunningProcess()
        {
            // Mimic a long running process
            System.Threading.Thread.Sleep(5000);

            return Newtonsoft.Json.JsonConvert.SerializeObject("ok");
        }
    }
}
