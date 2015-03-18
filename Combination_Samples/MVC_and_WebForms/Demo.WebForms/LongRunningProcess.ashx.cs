using System;
using System.Web;

namespace Demo.WebForms
{
    public class LongRunningProcess : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            // Mimic a long running process
            System.Threading.Thread.Sleep(5000);

            context.Response.ContentType = "text/plain";
            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject("ok"));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}