using System;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Demo.WebForms
{
    public class GetServerTime : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(DateTime.Now, new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd\\THH:mm:ss"
            }));
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