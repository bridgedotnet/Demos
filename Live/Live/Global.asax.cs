using System;

namespace Live
{
    public class Global : System.Web.HttpApplication
    {
        protected void Session_Start(object sender, EventArgs e)
        {
            Session["init"] = 0;
        }
    }
}