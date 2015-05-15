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
                string hash = context.Request.UrlReferrer.Query.Split('=').LastOrDefault();
                if (!string.IsNullOrEmpty(hash))
                {
                    string script = context.Session[hash].ToString();

                    if (!string.IsNullOrEmpty(script))
                    {
                        context.Response.Write(script);
                    }
                    else
                    {
                        context.Response.Write(string.Empty);
                    }
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