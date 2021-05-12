using Newtonsoft.Json;
using Okta.Idx.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace direct_auth.Controllers
{
    public class HomeController : Controller
    {
        public const string IdxStateKey = "IdxStateHandle";

        private readonly IIdxClient idxClient;

        public HomeController()
        {
            this.idxClient = new IdxClient();
        }

        public ActionResult Index()
        {
            return View();
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

        public async Task<ActionResult> SocialSignIn()
        {
            var socialLoginSettings = await this.idxClient.StartSocialLoginAsync();
            Session[IdxStateKey] = socialLoginSettings.Context.State;
            string contextJson = JsonConvert.SerializeObject(socialLoginSettings.Context);
            Session[socialLoginSettings.Context.State] = contextJson;
            return View(socialLoginSettings);
        }
    }
}