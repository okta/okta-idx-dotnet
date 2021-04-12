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

            // Password was selected during registration.
            if ((bool?)Session["isPasswordSelected"] ?? false)
            {
                return await VerifyAuthenticatorAsync(new VerifyAuthenticatorViewModel
                {
                    Code = model.NewPassword,
                });
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
                else if (authnResponse.AuthenticationStatus == AuthenticationStatus.AwaitingAuthenticatorEnrollment)
                {
                    Session["idxContext"] = authnResponse.IdxContext;
                    TempData["authenticators"] = authnResponse.Authenticators;
                    return RedirectToAction("selectAuthenticator", "Manage");
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
                else if (authnResponse.AuthenticationStatus == AuthenticationStatus.AwaitingAuthenticatorEnrollment)
                {
                    Session["idxContext"] = authnResponse.IdxContext;
                    TempData["authenticators"] = authnResponse.Authenticators;
                    return RedirectToAction("selectAuthenticator", "Manage");
                }
                else if (authnResponse.AuthenticationStatus == AuthenticationStatus.Success)
                {
                    return RedirectToAction("Login", "Account");
                }

                return View("VerifyAuthenticator", model);
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return View("VerifyAuthenticator", model);
            }
        }

        public ActionResult SelectAuthenticator()
        {
            var authenticators = (IList<IAuthenticator>)TempData["authenticators"];

            var viewModel = new SelectAuthenticatorViewModel();
            viewModel.Authenticators = authenticators
                                        .Select(x =>
                                                    new AuthenticatorViewModel
                                                    {
                                                        Id = x.Id,
                                                        Name = x.DisplayName
                                                    })
                                        .ToList();

            //viewModel.Authenticators = new List<AuthenticatorViewModel> { new AuthenticatorViewModel { Id = "emailId", Name = "Email" }, new AuthenticatorViewModel { Id = "passId", Name = "Password" } };
            viewModel.PasswordId = viewModel.Authenticators.FirstOrDefault(x => x.Name.ToLower() == "password")?.Id;
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectAuthenticatorAsync(SelectAuthenticatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SelectAuthenticator", model);
            }

            try
            {
                // WIP
                var idxAuthClient = new IdxClient(null);

                var enrollAuthenticatorOptions = new EnrollAuthenticatorOptions
                {
                    AuthenticatorId = model.AuthenticatorId,
                };

               var enrollResponse = await idxAuthClient.EnrollAuthenticatorAsync(enrollAuthenticatorOptions, (IIdxContext)Session["IdxContext"]);

               if (enrollResponse.AuthenticationStatus == AuthenticationStatus.AwaitingAuthenticatorVerification)
                {
                    // TODO: clean session.
                    Session["IdxContext"] = enrollResponse.IdxContext;
                    Session["isPasswordSelected"] = model.IsPasswordSelected;

                    if (model.IsPasswordSelected)
                    {
                        ViewDate.PageTitle = "Enter your password.";
                        return RedirectToAction("ChangePassword", "Manage");
                    }

                    return RedirectToAction("VerifyAuthenticator", "Manage");
                }

                return View("SelectAuthenticator", model);
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return RedirectToAction("SelectAuthenticator", model);
            }
        }
    }
}