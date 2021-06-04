using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using embedded_auth_with_sdk.Models;
using Microsoft.Owin.Security;
using Okta.Idx.Sdk;
using Okta.Sdk.Abstractions;

namespace embedded_auth_with_sdk.Controllers
{
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
        public async Task<ActionResult> Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            IdentityProvidersResponse identityProvidersResponse = await _idxClient.GetIdentityProvidersAsync();

            // save the context in session, keyed by state handle, so it can be retrieved on callback.  See InteractionCodeController.Callback
            Session[identityProvidersResponse.State] = identityProvidersResponse.Context;

            LoginViewModel loginViewModel = new LoginViewModel
            {
                IdpOptions = identityProvidersResponse.IdpOptions,  // You can keep IdpOptions unset (set to null) if you don't want or need social login buttons
            };

            return View(loginViewModel);
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

            var authnOptions = new AuthenticationOptions()
                                   {
                                       Username = model.UserName,
                                       Password = model.Password,
                                   };

            try
            {
                var authnResponse = await _idxClient.AuthenticateAsync(authnOptions).ConfigureAwait(false);
                Session["idxContext"] = authnResponse.IdxContext;

                switch (authnResponse?.AuthenticationStatus)
                {
                    case AuthenticationStatus.Success:
                            ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromTokenResponseAsync(_idxClient.Configuration, authnResponse.TokenInfo);
                            _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe }, identity);
                            return RedirectToAction("Index", "Home");

                    case AuthenticationStatus.PasswordExpired:
                        return RedirectToAction("ChangePassword", "Manage");

                    case AuthenticationStatus.AwaitingChallengeAuthenticatorSelection:
                        Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(authnResponse.Authenticators);
                        Session["isChallengeFlow"] = true;
                        return RedirectToAction("selectAuthenticator", "Manage");
                    case AuthenticationStatus.AwaitingAuthenticatorEnrollment:
                        Session["isChallengeFlow"] = false;
                        Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(authnResponse.Authenticators);
                        return RedirectToAction("SelectAuthenticator", "Manage");

                    default:
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
        public async Task<ActionResult> LogOut()
        {
            var accessToken = HttpContext.GetOwinContext().Authentication.User.Claims.FirstOrDefault(x => x.Type == "access_token");
            await _idxClient.RevokeTokensAsync(TokenType.AccessToken, accessToken.Value);
            _authenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
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
                var userProfile = new UserProfile();
                userProfile.SetProperty("firstName", model.FirstName);
                userProfile.SetProperty("lastName", model.LastName);
                userProfile.SetProperty("email", model.Email);

                var registerResponse = await _idxClient.RegisterAsync(userProfile);

                if (registerResponse.AuthenticationStatus == AuthenticationStatus.AwaitingAuthenticatorEnrollment)
                {
                    Session["idxContext"] = registerResponse.IdxContext;
                    Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(registerResponse.Authenticators);
                    return RedirectToAction("SelectAuthenticator", "Manage");
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
        
        [AllowAnonymous]
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

            var recoverPasswordOptions = new RecoverPasswordOptions { Username = model.UserName, };

            try
            {
                var authnResponse = await _idxClient.RecoverPasswordAsync(recoverPasswordOptions);
                Session["idxContext"] = authnResponse.IdxContext;

                if (authnResponse.AuthenticationStatus == AuthenticationStatus.AwaitingAuthenticatorSelection)
                {
                    Session["authenticators"] =
                        ViewModelHelper.ConvertToAuthenticatorViewModelList(authnResponse.Authenticators);
                    return RedirectToAction("SelectRecoveryAuthenticator", "Manage");
                }

                if (authnResponse.AuthenticationStatus == AuthenticationStatus.AwaitingAuthenticatorVerification)
                {
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
    }
}