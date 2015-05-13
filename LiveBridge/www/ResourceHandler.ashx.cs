using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace LiveBridge
{
    /// <summary>
    /// Summary description for ResourceHandler
    /// </summary>
    public class ResourceHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/javascript";

            try
            {
                string script = context.Session["script"].ToString();
                //context.Session.Remove("script");

                if (!string.IsNullOrEmpty(script))
                {
                    context.Response.Write(script);
                }
                else
                {
                    context.Response.Write(string.Empty);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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