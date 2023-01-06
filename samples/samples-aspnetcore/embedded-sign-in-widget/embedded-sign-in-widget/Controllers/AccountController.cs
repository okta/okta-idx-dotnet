using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Okta.Idx.Sdk;

namespace embedded_sign_in_widget.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdxClient _idxClient;

        public AccountController(IIdxClient idxClient)
        {
            _idxClient = idxClient;
        }

        [HttpGet]
        public async Task<ActionResult> SignInWidget(string? state = null)
        {
            if (string.IsNullOrEmpty(state))
            {
                state = HttpContext.Session.GetString(AuthenticationHelper.IdxStateKey);
            }
            SignInWidgetConfiguration signInWidgetConfiguration = await AuthenticationHelper.StartWidgetSignInAsync(HttpContext, _idxClient, state);
            return View(signInWidgetConfiguration);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignOut()
        {
            var accessToken = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "access_token");
            await _idxClient.RevokeTokensAsync(TokenType.AccessToken, accessToken.Value);
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}