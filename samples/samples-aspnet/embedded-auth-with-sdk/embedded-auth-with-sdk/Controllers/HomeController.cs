using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace embedded_auth_with_sdk.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(HttpContext.GetOwinContext().Authentication.User.Claims);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}