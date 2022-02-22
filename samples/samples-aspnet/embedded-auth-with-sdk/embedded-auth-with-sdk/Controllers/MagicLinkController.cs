namespace embedded_auth_with_sdk.Controllers
{
    using embedded_auth_with_sdk.Models;
    using Microsoft.Owin.Security;
    using Okta.Idx.Sdk;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class MagicLinkController : Controller
    {
        private readonly IAuthenticationManager _authenticationManager;

        private readonly IIdxClient _idxClient;

        public MagicLinkController(IAuthenticationManager authenticationManager, IIdxClient idxClient)
        {
            _authenticationManager = authenticationManager;
            _idxClient = idxClient;
        }

        /// <summary>
        /// This method is called by a 302 browser redirect back to the application from Okta after a user clicks a
        /// "sign in" link sent to them by email.
        /// </summary>
        /// <param name="state">The state handle.</param>
        /// <param name="otp">The one time password.</param>
        /// <param name="error">The error if an error occurred.</param>
        /// <param name="error_description">The error description if an error occurred.</param>
        /// <returns></returns>
        public async Task<ActionResult> Callback(string state, string otp, string error = null, string error_description = null)
        {
            if (!string.IsNullOrEmpty(error) || !string.IsNullOrEmpty(error_description))
            {
                return View(new MagicLinkCallbackModel { Message = $"{error}: {error_description}" });
            }

            // This implementation stores the idx context in session, keyed
            // by state, see AccountController.LoginAsync. Alternatively, you
            // may choose to store the idx context in a database or a file.
            // The `state` value serves as your handle to recover the idx context
            // to continue the authentication flow.
            IIdxContext idxContext = Session[state] as IIdxContext;
            if (idxContext == null)
            {
                idxContext = Session["idxContext"] as IIdxContext;
            }

            if (idxContext != null)
            {
                var verifyAuthenticatorOptions = new VerifyAuthenticatorOptions
                {
                    Code = otp,
                };

                var authnResponse = await _idxClient.VerifyAuthenticatorAsync(verifyAuthenticatorOptions, idxContext);

                switch (authnResponse.AuthenticationStatus)
                {
                    case AuthenticationStatus.AwaitingPasswordReset:
                        return RedirectToAction("ChangePassword", "Manage");

                    case AuthenticationStatus.AwaitingAuthenticatorEnrollment:
                        Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(authnResponse.Authenticators);
                        TempData["canSkip"] = authnResponse.CanSkip;
                        Session["isChallengeFlow"] = false;
                        return RedirectToAction("SelectAuthenticator", "Manage");

                    case AuthenticationStatus.Success:
                        ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromTokenResponseAsync(_idxClient.Configuration, authnResponse.TokenInfo);
                        _authenticationManager.SignIn(new AuthenticationProperties(), identity);
                        return RedirectToAction("Index", "Home");
                }
            }

            // We were unable to retrieve the context using the state.
            // We will assume that this is because we are in a different browser and advise the user to 
            // return to the original tab and enter the otp value there.
            //
            // Worth noting that if the AuthenticationStatus is not one of the values specifically handled above we will also arrive here.
            return View(new MagicLinkCallbackModel { Message = $"Please enter the OTP '{otp}' in the original browser tab to finish the flow." });
        }
    }
}
