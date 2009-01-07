using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace xVal.ClientSideTests
{
    public class TestController : Controller
    {
        public ViewResult RenderSpecificView(string viewPath)
        {
            return View("~/TestPages/" + viewPath + ".aspx");
        }
    }
}
