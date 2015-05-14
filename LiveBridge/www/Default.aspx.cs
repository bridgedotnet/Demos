using System;
using System.Web;
using Newtonsoft.Json;

namespace LiveBridge
{
    public partial class Default : System.Web.UI.Page
    {
        private bool isDebugMode = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (!string.IsNullOrEmpty(this.Request["ajax"]))
                {
                    this.Translate(this.Request.Form["cs"]);
                }
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
                string bridgeStubLocation = (this.isDebugMode) ? this.Server.MapPath("~") + @"..\BridgeStub\bin\Debug\BridgeStub.dll" : this.Server.MapPath(@".\BridgeTranslator\Stub\BridgeStub.dll");

                LiveTranslator translator =
                    new LiveTranslator(
                        this.Server.MapPath(@".\UserCode\"),
                        csCode,
                        false,
                        bridgeStubLocation,
                        HttpContext.Current);

                string jsCode = translator.Translate();

                json = JsonConvert.SerializeObject(new
                    {
                        Success = true,
                        JsCode = jsCode
                    });

                Session["script"] = jsCode;
            }
            catch (Exception ex)
            {
                json = JsonConvert.SerializeObject(new
                    {
                        Success = false,
                        ErrorMessage = ex.Message
                    });

                Session["script"] = string.Empty;
            }
            finally
            {
                this.Response.Clear();
                this.Response.ContentType = "application/json";
                this.Response.Write(json);
                this.Response.End();
            }
        }
    }
}