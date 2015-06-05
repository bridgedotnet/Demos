using System;
using System.Web;
using System.Web.SessionState;
using Newtonsoft.Json;

namespace Live
{
    /// <summary>
    /// Translates the C# code into JavaScript and returns json:
    ///     Success      : true or false
    ///     JsCode       : the generated JavaScript code
    ///     Hash         : the hash of JsCode
    ///     ErrorMessage : the error message if Success is false
    /// The C# code is passed as the value of request parameter "cs".
    /// The translation begins when request parameter "ajax" exists.
    /// </summary>
    public class TranslateHandler : IHttpHandler, IRequiresSessionState
    {
        private bool isDebugMode = false;
        private HttpContext context;

        public void ProcessRequest(HttpContext context)
        {
            this.context = context;

            this.context.Response.ContentType = "application/json";

            if (!string.IsNullOrEmpty(this.context.Request["ajax"]))
            {
                this.Translate(this.context.Request.Form["cs"]);
            }
        }


        protected void Translate(string csCode)
        {
            #if DEBUG
                this.isDebugMode = true;
            #endif

            string json = "{}";

            try
            {
                string bridgeStubLocation = (this.isDebugMode) ? this.context.Server.MapPath("~") + @"..\LiveApp\bin\Debug\LiveApp.dll" : this.context.Server.MapPath(@".\Bridge\Builder\LiveApp.dll");

                LiveTranslator translator =
                    new LiveTranslator(
                        this.context.Server.MapPath(@"."),
                        csCode,
                        false,
                        bridgeStubLocation,
                        HttpContext.Current);

                string jsCode = translator.Translate();

                string hash = jsCode.GetHashCode().ToString();

                json = JsonConvert.SerializeObject(new
                {
                    Success = true,
                    JsCode = jsCode,
                    Hash = hash
                });

                // store emitted javascript to session with its hash as key.                   
                this.context.Session[hash] = jsCode;
            }
            catch (Exception ex)
            {
                json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
            finally
            {
                this.context.Response.Write(json);
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