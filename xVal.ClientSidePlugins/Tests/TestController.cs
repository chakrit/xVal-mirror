using System.Web.Mvc;

namespace xVal.ClientSidePlugins.MvcTestSite
{
    public class TestController : Controller
    {
        public ViewResult RenderSpecificView(string viewPath)
        {
            return View("~" + viewPath);
        }
    }
}