using System.Web.Mvc;
using System.Web.Routing;

namespace xVal.ClientSidePlugins.MvcTestSite
{
    public class GlobalApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteTable.Routes.MapRoute(null, "Test", new { controller = "Test", action = "RenderSpecificView" });
        }
    }
}