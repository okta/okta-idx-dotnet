using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using direct_auth_idx.Models;
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

            Session["UserName"] = model.UserName;

            try
            {
                var authnResponse = await idxAuthClient.AuthenticateAsync(authnOptions).ConfigureAwait(false);

                switch (authnResponse?.AuthenticationStatus)
                {
                    case AuthenticationStatus.Success:
                            ClaimsIdentity identity = AuthenticationHelper.GetIdentityFromAuthResponse(model.UserName, authnResponse);
                            _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe }, identity);
                            return RedirectToAction("Index", "Home");

                    case AuthenticationStatus.PasswordExpired:
                        // Redirect to ChangePassword
                        Session["idxContext"] = authnResponse.IdxContext;
                        return RedirectToAction("ChangePassword", "Manage");

                    case AuthenticationStatus.AwaitingChallengeAuthenticatorSelection:
                        Session["idxContext"] = authnResponse.IdxContext;
                        TempData["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(
                                                                        authnResponse.AuthenticatorEnrollments,
                                                                        authnResponse.Authenticators);
                        Session["isChallengeFlow"] = true;
                        return RedirectToAction("selectAuthenticator", "Manage");

                    default:
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

        // GET: Account
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterAsync(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Register");
            }

            try
            {
                // WIP
                var idxAuthClient = new IdxClient(null);
                Session["UserName"] = model.Email;

                var userProfile = new UserProfile();
                userProfile.SetProperty("firstName", model.FirstName);
                userProfile.SetProperty("lastName", model.LastName);
                userProfile.SetProperty("email", model.Email);

                var registerResponse = await idxAuthClient.RegisterAsync(userProfile);

                if (registerResponse.AuthenticationStatus == AuthenticationStatus.AwaitingAuthenticatorEnrollment)
                {
                    Session["idxContext"] = registerResponse.IdxContext;
                    TempData["authenticators"] = registerResponse.Authenticators;
                    return RedirectToAction("selectAuthenticator", "Manage");
                }

                ModelState.AddModelError(string.Empty, $"Oops! Something went wrong.");
                return View("Register", model);
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError(string.Empty, $"Oops! Something went wrong: {exception.Message}");
                return View("Register", model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ForgotPassword", model);
            }

            var idxAuthClient = new IdxClient(null);

            var recoverPasswordOptions = new RecoverPasswordOptions
            {
                Username = model.UserName,
            };

            try
            {
                var authnResponse = await idxAuthClient.RecoverPasswordAsync(recoverPasswordOptions);

                if (authnResponse.AuthenticationStatus == AuthenticationStatus.AwaitingAuthenticatorSelection)
                {
                    Session["idxContext"] = authnResponse.IdxContext;
                    Session["UserName"] = model.UserName;
                    TempData["authenticators"] = authnResponse.Authenticators;
                    return RedirectToAction("SelectRecoveryAuthenticator", "Account");
                }

                if (authnResponse.AuthenticationStatus == AuthenticationStatus.AwaitingAuthenticatorVerification)
                {
                    Session["idxContext"] = authnResponse.IdxContext;
                    return RedirectToAction("VerifyAuthenticator", "Manage");
                }

                return View("ForgotPassword", model);
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return View("ForgotPassword", model);
            }
        }

        public ActionResult SelectRecoveryAuthenticator()
        {
            var authenticators = (IList<IAuthenticator>)TempData["authenticators"];
            var viewModel = new SelectRecoveryAuthenticatorViewModel
            {
                Authenticators = authenticators?
                                 .Select(x =>
                                            new AuthenticatorViewModel
                                            {
                                                AuthenticatorId = x.Id,
                                                Name = x.DisplayName
                                            })
                                .ToList() ?? new List<AuthenticatorViewModel>()
            };
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectRecoveryAuthenticatorAsync(SelectRecoveryAuthenticatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ForgotPassword", new ForgotPasswordViewModel());
            }

            try
            {
                var idxAuthClient = new IdxClient(null);
                var applyAuthenticatorResponse = await idxAuthClient.SelectRecoveryAuthenticatorAsync(
                                                                                    new SelectAuthenticatorOptions { AuthenticatorId = model.AuthenticatorId },
                                                                                    (IIdxContext)Session["IdxContext"]);

                Session["IdxContext"] = applyAuthenticatorResponse.IdxContext;
                Session["isPasswordSelected"] = false;

                if (applyAuthenticatorResponse.AuthenticationStatus == AuthenticationStatus.AwaitingAuthenticatorVerification)
                {
                    return RedirectToAction("VerifyAuthenticator", "Manage");
                }

                return View("SelectAuthenticator", model);
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return RedirectToAction("ForgotPassword", model);
            }
        }

    }
}