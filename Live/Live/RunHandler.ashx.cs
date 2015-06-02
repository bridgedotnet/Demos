using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace Live
{
    /// <summary>
    /// Returns the generated JavaScript code from session with the JavaScript code hash as key.
    /// The hash is passed as the value of query parameter "h". Returns empty string if no hash parameter or session is found.
    /// </summary>
    public class RunHandler : IHttpHandler, IRequiresSessionState
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