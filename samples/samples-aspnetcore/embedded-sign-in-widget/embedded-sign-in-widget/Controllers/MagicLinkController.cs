namespace embedded_sign_in_widget.Controllers
{
    using embedded_sign_in_widget;
    using embedded_sign_in_widget.Models;
    using Microsoft.AspNetCore.Mvc;
    using Okta.Idx.Sdk;
    using System.Threading.Tasks;

    public class MagicLinkController : Controller
    {
        public MagicLinkController()
        {
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

            SignInWidgetConfiguration siwConfig = await HttpContext.GetSignInWidgetConfigurationAsync();
            if (siwConfig != null)
            {
                HttpContext.Session.SetString("otp", otp);
                return View("SignInWidget", siwConfig);
            }

            // We will assume that we are in a different browser and advise the user to 
            // return to the original tab and enter the otp value there.
            return View(new MagicLinkCallbackModel { Message = $"Please enter the OTP '{otp}' in the original browser tab to finish the flow." });
        }
    }
}
