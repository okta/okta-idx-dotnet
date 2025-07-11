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


        // The GET action is simplified to only display the view.
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        // The POST action now handles the multi-step logic.
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginAsync(LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserName))
            {
                ModelState.AddModelError(string.Empty, "Username is required.");
                return View("Login", model);
            }

            try
            {
                var authnOptions = new AuthenticationOptions { Username = model.UserName };
                var authnResponse = await _idxClient.AuthenticateAsync(authnOptions).ConfigureAwait(false);
                var idxContext = authnResponse.IdxContext;
                Session["idxContext"] = idxContext;

                var idpResponse = await _idxClient.GetIdentityProvidersAsync(idxContext);

                if (idpResponse.IdpOptions?.Count > 0)
                {
                    var idpRedirectUrl = idpResponse.IdpOptions.First().Href;
                    return Redirect(idpRedirectUrl);
                }

                switch (authnResponse.AuthenticationStatus)
                {
                    case AuthenticationStatus.AwaitingChallengeAuthenticatorSelection:
                        var passwordAuthenticator = authnResponse.Authenticators.FirstOrDefault(auth => auth.DisplayName == "Password");
                        if (passwordAuthenticator != null)
                        {
                            if (!string.IsNullOrEmpty(model.Password))
                            {
                                var challengeOptions = new AuthenticationOptions
                                {
                                    Username = model.UserName,
                                    Password = model.Password,
                                };
                                var challengeAuthnResponse = await _idxClient.AuthenticateAsync(challengeOptions).ConfigureAwait(false);
                                if (challengeAuthnResponse.AuthenticationStatus == AuthenticationStatus.Success)
                                {
                                    ClaimsIdentity successIdentity = await AuthenticationHelper.GetIdentityFromTokenResponseAsync(_idxClient.Configuration, challengeAuthnResponse.TokenInfo);
                                    _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe }, successIdentity);
                                    return RedirectToAction("Index", "Home");
                                }
                            }

                            model.ShouldRenderPasswordField = true;
                            return View("Login", model);
                        }
                        
                        Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(authnResponse.Authenticators);
                        Session["isChallengeFlow"] = true;
                        return RedirectToAction("SelectAuthenticator", "Manage");

                    case AuthenticationStatus.Success:
                        ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromTokenResponseAsync(_idxClient.Configuration, authnResponse.TokenInfo);
                        _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe }, identity);
                        return RedirectToAction("Index", "Home");

                    default:
                        ModelState.AddModelError(string.Empty, "An unexpected authentication status was received.");
                        return View("Login", model);
                }
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError(string.Empty, $"Error: {exception.Message}");
                return View("Login", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOut()
        {
            var accessToken = HttpContext.GetOwinContext().Authentication.User.Claims.FirstOrDefault(x => x.Type == "access_token");
            if (accessToken != null) await _idxClient.RevokeTokensAsync(TokenType.AccessToken, accessToken.Value);
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

        [AllowAnonymous]
        public ActionResult RecoverPasswordWithToken()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RecoverPasswordWithTokenAsync(RecoverPasswordWithTokenViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("RecoverPasswordWithToken", model);
            }

            var recoveryToken = await OktaSdkHelper.ForgotPasswordGenerateToken(model.UserName);
            if (recoveryToken == null)
            {
                ModelState.AddModelError(string.Empty, $"Unable to get recovery token. Check if the user name is spelled correctly.");
                return View("RecoverPasswordWithToken", model);
            }

            var changePasswordViewModel = new ChangePasswordWithRecoveryTokenViewModel
            {
                RecoveryToken = recoveryToken,
                UserName = model.UserName,
            };

            return View("~/Views/Manage/ChangePasswordWithRecoveryToken.cshtml", changePasswordViewModel);
        }
    }
}
