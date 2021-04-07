using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using direct_auth_idx.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Okta.Idx.Sdk;
using Okta.Sdk.Abstractions;

namespace direct_auth_idx.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationManager _authenticationManager;

        public AccountController(IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        // GET: Account
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginAsync(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login");
            }

            // WIP
            var idxAuthClient = new IdxClient(null);


            var authnOptions = new Okta.Idx.Sdk.AuthenticationOptions()
            {
                Username = model.UserName,
                Password = model.Password,
            };

            try
            {
                var authnResponse = await idxAuthClient.AuthenticateAsync(authnOptions).ConfigureAwait(false);

                if (authnResponse != null && authnResponse.AuthenticationStatus == AuthenticationStatus.Success)
                {
                    var identity = new ClaimsIdentity(
                        new[] { new Claim(ClaimTypes.Name, model.UserName),
                                new Claim("access_token", authnResponse.TokenInfo.AccessToken),
                                new Claim("id_token", authnResponse.TokenInfo.IdToken),
                                new Claim("refresh_token", authnResponse.TokenInfo.RefreshToken ?? string.Empty),
                                },
                        DefaultAuthenticationTypes.ApplicationCookie);

                    _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe }, identity);

                    return RedirectToAction("Index", "Home");
                }
                else if (authnResponse != null && authnResponse.AuthenticationStatus == AuthenticationStatus.PasswordExpired)
                {
                    // Redirect to ChangePassword
                    Session["idxContext"] = authnResponse.IdxContext;

                    return RedirectToAction("ChangePassword", "Manage");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Invalid login attempt:");
                    return View("Login", model);
                }
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError(string.Empty, $"Invalid login attempt: {exception.Message}");
                return View("Login", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOff()
        {
            var client = new IdxClient(null);
            var accessToken = HttpContext.GetOwinContext().Authentication.User.Claims.FirstOrDefault(x => x.Type == "access_token");
            await client.RevokeTokensAsync(TokenType.AccessToken, accessToken.Value);
            _authenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

    }
}