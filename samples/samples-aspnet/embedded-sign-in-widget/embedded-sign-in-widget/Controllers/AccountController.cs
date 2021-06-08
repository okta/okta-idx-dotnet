using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Okta.Idx.Sdk;

namespace embedded_sign_in_widget.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationManager _authenticationManager;

        private readonly IIdxClient _idxClient;

        public AccountController(IAuthenticationManager authenticationManager, IIdxClient idxClient)
        {
            _authenticationManager = authenticationManager;
            _idxClient = idxClient;
        }

        [HttpGet]
        public async Task<ActionResult> SignInWidget(string state = null)
        {
            SignInWidgetConfiguration signInWidgetConfiguration = await AuthenticationHelper.StartWidgetSignInAsync(HttpContext, _idxClient, state);
            if (string.IsNullOrEmpty(state))
            {
                // redirect back to current action with state to allow reload without starting a new idx interaction
                return Redirect($"/Account/SignInWidget?state={signInWidgetConfiguration.State}");
            }
            return View(signInWidgetConfiguration);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOut()
        {
            var accessToken = HttpContext.GetOwinContext().Authentication.User.Claims.FirstOrDefault(x => x.Type == "access_token");
            await _idxClient.RevokeTokensAsync(TokenType.AccessToken, accessToken.Value);
            _authenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}