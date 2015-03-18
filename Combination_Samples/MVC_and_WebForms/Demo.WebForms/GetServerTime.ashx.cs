using System;
using System.Web;

namespace Demo.WebForms
{
    public class GetServerTime : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(DateTime.Now));
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