using System.Web.Mvc;

namespace direct_auth.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;

    using direct_auth.Models;

    using IdentityModel.Client;

    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security;

    using Okta.Idx.Sdk;
    using Okta.Sdk.Abstractions;

    using AuthenticationOptions = Okta.Idx.Sdk.AuthenticationOptions;

    public class AccountController : Controller
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IIdxClient _idxClient;

        public AccountController(IAuthenticationManager authenticationManager, IIdxClient idxClient)
        {
            _authenticationManager = authenticationManager;
            _idxClient = idxClient;
        }
        
        // GET: Account
        [AllowAnonymous]
        public ActionResult LoginWithPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginWithPasswordAsync(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("LoginWithPassword");
            }

            var authnOptions = new AuthenticationOptions()
                                   {
                                       Username = model.UserName,
                                       Password = model.Password,
                                   };

            try
            {
                var authnResponse = await _idxClient.AuthenticateAsync(authnOptions).ConfigureAwait(false);

                if (authnResponse.AuthenticationStatus == AuthenticationStatus.Success)
                {
                    Session["access_token"] = authnResponse.TokenInfo.AccessToken;
                    var claims = await GetClaimsFromUserInfoAsync(authnResponse.TokenInfo.AccessToken);
                    ClaimsIdentity identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                    _authenticationManager.SignIn(identity);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("Login was unsuccessful", $"Please, review your policies and make sure your org is configured for password only.");
                return View("LoginWithPassword", model);

            }
            catch (OktaException exception)
            {
                ModelState.AddModelError(string.Empty, $"Invalid login attempt: {exception.Message}");
                return View("LoginWithPassword", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOut()
        {
            var client = new IdxClient(null);
            //var accessToken = HttpContext.GetOwinContext().Authentication.User.Claims.FirstOrDefault(x => x.Type == "access_token");
            await client.RevokeTokensAsync(TokenType.AccessToken, Session["access_token"]?.ToString());
            _authenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }


        [Authorize]
        public ActionResult Profile()
        {
            return View(HttpContext.GetOwinContext().Authentication.User.Claims);
        }

        private async Task<IEnumerable<Claim>> GetClaimsFromUserInfoAsync(string accessToken)
        {
            Uri userInfoUri = new Uri(GetNormalizedUriString(this._idxClient.Configuration.Issuer, "v1/userinfo"));
            HttpClient httpClient = new HttpClient();
            var userInfoResponse = await httpClient.GetUserInfoAsync(new UserInfoRequest
                                                                          {
                                                                              Address = userInfoUri.ToString(),
                                                                              Token = accessToken,
                                                                          }).ConfigureAwait(false);

            return userInfoResponse.Claims;
        }

        /******************************************************************************************/
        // TODO: Reuse Bryan's helper
        private static string GetNormalizedUriString(string issuer, string resourceUri)
        {
            string normalized = issuer;
            if (IsRootOrgIssuer(issuer))
            {
                normalized = Path.Combine(normalized, "oauth2", resourceUri);
            }
            else
            {
                normalized = Path.Combine(normalized, resourceUri);
            }
            return normalized;
        }

        private static bool IsRootOrgIssuer(string issuerUri)
        {
            string path = new Uri(issuerUri).AbsolutePath;
            string[] splitUri = path.Split(new char[]{'/'}, StringSplitOptions.RemoveEmptyEntries);
            if (splitUri.Length >= 2 &&
                "oauth2".Equals(splitUri[0]) &&
                !string.IsNullOrEmpty(splitUri[1]))
            {
                return false;
            }

            return true;
        }
        /******************************************************************************************/
    }
}