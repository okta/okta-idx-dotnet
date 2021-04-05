using direct_auth_idx.Models;
using Okta.Idx.Sdk;
using Okta.Sdk.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace direct_auth_idx.Controllers
{
    public class ManageController : Controller
    {
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ChangePassword", model);
            }

            var changePasswordOptions = new ChangePasswordOptions()
            {
                NewPassword = model.NewPassword,
            };

            // WIP
            var idxAuthClient = new IdxClient(null);

            try
            {
                var authnResponse = await idxAuthClient.ChangePasswordAsync(changePasswordOptions, (IIdxContext)Session["idxContext"]).ConfigureAwait(false);

                if (authnResponse.AuthenticationStatus == AuthenticationStatus.Success)
                {
                    return RedirectToAction("Login", "Account");
                }

                return View("ChangePassword", model);
            }
            catch (OktaApiException exception)
            {
                ModelState.AddModelError("Oops! Something went wrong.", exception.ErrorSummary);
                return View("ChangePassword", model);
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError("Oops! Something went wrong.", exception.Message);
                return View("ChangePassword", model);
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

            // WIP
            var idxAuthClient = new IdxClient(null);

            var recoverPasswordOptions = new RecoverPasswordOptions
            {
                AuthenticatorType = model.AuthenticatorType,
                Username = model.UserName,
            };

            try
            {
                var authnResponse = await idxAuthClient.RecoverPasswordAsync(recoverPasswordOptions);
                 
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

        // GET
        public ActionResult VerifyAuthenticator()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyAuthenticatorAsync(VerifyAuthenticatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("VerifyAuthenticator", model);
            }

            // WIP
            var idxAuthClient = new IdxClient(null);

            var verifyAuthenticatorOptions = new VerifyAuthenticatorOptions
            {
               Code = model.Code,
            };

            try
            {
                var authnResponse = await idxAuthClient.VerifyAuthenticatorAsync(verifyAuthenticatorOptions, (IIdxContext)Session["idxContext"]);

                if (authnResponse.AuthenticationStatus == AuthenticationStatus.AwaitingPasswordReset)
                {
                    // TODO: Force authentication and redirect to home page
                    Session["idxContext"] = authnResponse.IdxContext;

                    return RedirectToAction("ChangePassword", "Manage");
                }

                return View("ChangePassword", model);
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return View("ChangePassword", model);
            }
        }
    }
}