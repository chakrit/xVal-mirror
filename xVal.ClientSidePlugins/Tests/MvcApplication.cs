using System.Web.Mvc;
using System.Web.Routing;

namespace xVal.ClientSidePlugins.Tests
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteTable.Routes.MapRoute(null, "Test", new { controller = "Test", action = "RenderSpecificView" });
        }

        protected void Application_BeginRequest()
        {
            if(Request.AppRelativeCurrentExecutionFilePath == "~/")
                Response.Redirect("~/Tests");
        }
    }
}