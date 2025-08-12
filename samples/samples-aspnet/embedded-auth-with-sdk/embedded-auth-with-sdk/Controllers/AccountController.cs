using System;
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

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is HttpAntiForgeryException)
            {
                // Handle anti-forgery token mismatch
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new { action = "Login", controller = "Account" }));
                filterContext.ExceptionHandled = true;
                
                // Add a message to inform the user
                TempData["ErrorMessage"] = "Session expired. Please try again.";
            }
            base.OnException(filterContext);
        }

        // The GET action is simplified to only display the view.
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            
            // Clear any existing authentication state that might cause token conflicts
            if (Request.IsAuthenticated)
            {
                _authenticationManager.SignOut();
            }
            
            return View(new LoginViewModel());
        }

        // The POST action handles the identifier-first authentication flow
        [HttpPost]
        [AllowAnonymous]
        // [ValidateAntiForgeryToken] // Temporarily disabled due to authentication flow conflicts
        public async Task<ActionResult> LoginAsync(LoginViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserName))
                {
                    ModelState.AddModelError(string.Empty, "Username is required.");
                    return View("Login", model);
                }

                System.Diagnostics.Debug.WriteLine($"=== LOGIN ATTEMPT ===");
                System.Diagnostics.Debug.WriteLine($"Username: {model.UserName}");
                System.Diagnostics.Debug.WriteLine($"Time: {DateTime.Now}");
                System.Diagnostics.Debug.WriteLine($"User Domain: {(model.UserName.Contains("@") ? model.UserName.Split('@')[1] : "No domain found")}");

                // Step 1: Identifier-First Authentication
                // Only send the username/email first to allow Okta to check routing rules
                var identifyOptions = new AuthenticationOptions { Username = model.UserName };
                var identifyResponse = await _idxClient.AuthenticateAsync(identifyOptions).ConfigureAwait(false);
                
                // Store the IDX context for subsequent steps
                var idxContext = identifyResponse.IdxContext;
                Session["idxContext"] = idxContext;

                // Debug: Log the authentication status
                System.Diagnostics.Debug.WriteLine($"Initial Authentication Status: {identifyResponse.AuthenticationStatus}");
                System.Diagnostics.Debug.WriteLine($"IDX Context State: {idxContext?.State}");

                // Step 2: Check for Identity Provider Routing
                // This will return IdP options if the user's domain matches a routing rule
                System.Diagnostics.Debug.WriteLine("Calling GetIdentityProvidersAsync...");
                var idpResponse = await _idxClient.GetIdentityProvidersAsync(idxContext);

                System.Diagnostics.Debug.WriteLine($"IdP Options Count: {idpResponse.IdpOptions?.Count ?? 0}");
                
                if (idpResponse.IdpOptions?.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("=== EXTERNAL IDP ROUTING DETECTED ===");
                    foreach (var idpOption in idpResponse.IdpOptions)
                    {
                        System.Diagnostics.Debug.WriteLine($"IdP Option - Name: {idpOption.Name}, Href: {idpOption.Href}, Id: {idpOption.Id}");
                    }
                    
                    // User's email domain matches a routing rule - redirect to external IdP
                    var idpRedirectUrl = idpResponse.IdpOptions.First().Href;
                    
                    // Debug: Log the redirect
                    System.Diagnostics.Debug.WriteLine($"SUCCESS: Redirecting to external IdP: {idpRedirectUrl}");
                    System.Diagnostics.Debug.WriteLine($"Storing context with state: {idxContext.State}");
                    
                    // Store the state for callback handling
                    Session[idxContext.State] = idxContext;
                    
                    return Redirect(idpRedirectUrl);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("=== NO EXTERNAL IDP ROUTING ===");
                    System.Diagnostics.Debug.WriteLine("Possible reasons:");
                    System.Diagnostics.Debug.WriteLine("1. No routing rule configured for this domain");
                    System.Diagnostics.Debug.WriteLine("2. User domain doesn't match any routing rule");
                    System.Diagnostics.Debug.WriteLine("3. Routing rule exists but points to invalid/inactive IdP");
                    System.Diagnostics.Debug.WriteLine("4. User doesn't exist and JIT provisioning not enabled");
                }

                // Step 3: Handle Internal Authentication Flow
                switch (identifyResponse.AuthenticationStatus)
                {
                    case AuthenticationStatus.AwaitingChallengeAuthenticatorSelection:
                        // User needs to authenticate with password or other authenticators
                        System.Diagnostics.Debug.WriteLine("Status: AwaitingChallengeAuthenticatorSelection - proceeding with password auth");
                        return await HandlePasswordAuthentication(model, idxContext);

                    case AuthenticationStatus.AwaitingAuthenticatorSelection:
                        // Multiple authenticators available
                        System.Diagnostics.Debug.WriteLine("Status: AwaitingAuthenticatorSelection - redirecting to authenticator selection");
                        Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(identifyResponse.Authenticators);
                        Session["isChallengeFlow"] = true;
                        return RedirectToAction("SelectAuthenticator", "Manage");

                    case AuthenticationStatus.Success:
                        // Immediate success (rare case)
                        System.Diagnostics.Debug.WriteLine("Status: Success - immediate authentication success");
                        ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromTokenResponseAsync(_idxClient.Configuration, identifyResponse.TokenInfo);
                        _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe }, identity);
                        return RedirectToAction("Index", "Home");

                    default:
                        System.Diagnostics.Debug.WriteLine($"Unexpected Status: {identifyResponse.AuthenticationStatus}");
                        ModelState.AddModelError(string.Empty, $"Unexpected authentication status: {identifyResponse.AuthenticationStatus}");
                        return View("Login", model);
                }
            }
            catch (HttpAntiForgeryException)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: HttpAntiForgeryException occurred");
                TempData["ErrorMessage"] = "Security token expired. Please try logging in again.";
                return RedirectToAction("Login");
            }
            catch (Exception exception) when (exception.GetType().Name.Contains("UnexpectedRemediationException"))
            {
                // Handle the specific UnexpectedRemediationException
                System.Diagnostics.Debug.WriteLine($"=== CAUGHT UNEXPECTED REMEDIATION EXCEPTION ===");
                System.Diagnostics.Debug.WriteLine($"Exception: {exception.Message}");
                System.Diagnostics.Debug.WriteLine($"Full Exception: {exception}");
                
                // If we get an UnexpectedRemediationException with "launch-authenticator", 
                // it means Okta wants to redirect to external IdP but the flow wasn't handled properly
                if (exception.Message.Contains("launch-authenticator"))
                {
                    System.Diagnostics.Debug.WriteLine("ATTEMPTING: Recovery for launch-authenticator exception");
                    System.Diagnostics.Debug.WriteLine("This suggests routing rule exists but IdP configuration may be incomplete");
                    
                    // Try to get IdP options again and redirect
                    try
                    {
                        var idxContext = Session["idxContext"] as IIdxContext;
                        if (idxContext != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Attempting recovery with context state: {idxContext.State}");
                            var idpResponse = await _idxClient.GetIdentityProvidersAsync(idxContext);
                            System.Diagnostics.Debug.WriteLine($"Recovery attempt - IdP Options Count: {idpResponse.IdpOptions?.Count ?? 0}");
                            
                            if (idpResponse.IdpOptions?.Count > 0)
                            {
                                foreach (var idpOption in idpResponse.IdpOptions)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Recovery IdP Option - Name: {idpOption.Name}, Href: {idpOption.Href}");
                                }
                                
                                var idpRedirectUrl = idpResponse.IdpOptions.First().Href;
                                System.Diagnostics.Debug.WriteLine($"RECOVERY SUCCESS: Redirecting to: {idpRedirectUrl}");
                                Session[idxContext.State] = idxContext;
                                return Redirect(idpRedirectUrl);
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("RECOVERY FAILED: No IdP options available in recovery attempt");
                                System.Diagnostics.Debug.WriteLine("This indicates routing rule exists but IdP is not properly configured");
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("RECOVERY FAILED: No IDX context available");
                        }
                    }
                    catch (Exception innerEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"RECOVERY ERROR: {innerEx.Message}");
                        System.Diagnostics.Debug.WriteLine($"Recovery Exception Details: {innerEx}");
                    }
                }
                
                ModelState.AddModelError(string.Empty, "External Identity Provider routing detected but configuration incomplete. Please check your Okta Identity Provider setup.");
                return View("Login", model);
            }
            catch (OktaException exception)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: OktaException: {exception.Message}");
                ModelState.AddModelError(string.Empty, $"Error: {exception.Message}");
                return View("Login", model);
            }
            catch (Exception exception)
            {
                // Generic exception handler for any other unexpected issues
                System.Diagnostics.Debug.WriteLine($"ERROR: General Exception: {exception.Message}");
                System.Diagnostics.Debug.WriteLine($"Exception Details: {exception}");
                ModelState.AddModelError(string.Empty, $"An unexpected error occurred: {exception.Message}");
                return View("Login", model);
            }
        }

        private async Task<ActionResult> HandlePasswordAuthentication(LoginViewModel model, IIdxContext idxContext)
        {
            if (!string.IsNullOrEmpty(model.Password))
            {
                // User has provided password, attempt authentication
                try
                {
                    var challengeOptions = new AuthenticationOptions
                    {
                        Username = model.UserName,
                        Password = model.Password,
                    };
                    
                    var challengeResponse = await _idxClient.AuthenticateAsync(challengeOptions).ConfigureAwait(false);
                    Session["idxContext"] = challengeResponse.IdxContext;

                    switch (challengeResponse.AuthenticationStatus)
                    {
                        case AuthenticationStatus.Success:
                            ClaimsIdentity successIdentity = await AuthenticationHelper.GetIdentityFromTokenResponseAsync(_idxClient.Configuration, challengeResponse.TokenInfo);
                            _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe }, successIdentity);
                            return RedirectToAction("Index", "Home");

                        case AuthenticationStatus.AwaitingAuthenticatorEnrollment:
                            Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(challengeResponse.Authenticators);
                            TempData["canSkip"] = challengeResponse.CanSkip;
                            Session["isChallengeFlow"] = false;
                            return RedirectToAction("SelectAuthenticator", "Manage");

                        case AuthenticationStatus.AwaitingChallengeAuthenticatorSelection:
                            Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(challengeResponse.Authenticators);
                            Session["isChallengeFlow"] = true;
                            return RedirectToAction("SelectAuthenticator", "Manage");

                        default:
                            ModelState.AddModelError(string.Empty, "Invalid username or password.");
                            model.ShouldRenderPasswordField = true;
                            return View("Login", model);
                    }
                }
                catch (OktaException)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    model.ShouldRenderPasswordField = true;
                    return View("Login", model);
                }
            }
            else
            {
                // Show password field for user to enter password
                model.ShouldRenderPasswordField = true;
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
