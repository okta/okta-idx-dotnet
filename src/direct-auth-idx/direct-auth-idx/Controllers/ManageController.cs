using Microsoft.Owin.Security;
using Okta.Idx.Sdk;
using Okta.Sdk.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using direct_auth_idx.Models;

namespace direct_auth_idx.Controllers
{
    public class ManageController : Controller
    {

        private readonly IAuthenticationManager _authenticationManager;

        public ManageController(IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

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
                return await VerifyAuthenticatorAsync(model.NewPassword, "ChangePassword", model);
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

                switch (authnResponse.AuthenticationStatus)
                {
                    case AuthenticationStatus.Success:
                        var userName = (string)Session["UserName"];
                        if (string.IsNullOrEmpty(userName))
                        {
                            return RedirectToAction("Login", "Account");
                        }
                        else
                        {
                            Session["UserName"] = null;
                            var identity = AuthenticationHelper.GetIdentityFromAuthResponse(userName, authnResponse);
                            _authenticationManager.SignIn(new AuthenticationProperties(), identity);
                            return RedirectToAction("Index", "Home");
                        }
            
                    case AuthenticationStatus.AwaitingAuthenticatorEnrollment:
                        Session["idxContext"] = authnResponse.IdxContext;
                        TempData["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(authnResponse.Authenticators);
                        return RedirectToAction("selectAuthenticator", "Manage");
                }
                return View("ChangePassword", model);
            }
            catch (OktaApiException exception)
            {
                ModelState.AddModelError("Oops! Something went wrong.", exception.ErrorSummary 
                                         ?? "Cannot change password. Check if the new password meets the requirements.");
                return View("ChangePassword", model);
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError("Oops! Something went wrong.", exception.Message);
                return View("ChangePassword", model);
            }
        }

        // GET
        public ActionResult VerifyAuthenticator()
        {
            return View();
        }


        private async Task<ActionResult> VerifyAuthenticatorAsync(string code, string view, BaseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(view, model);
            }

            var idxAuthClient = new IdxClient(null);

            var verifyAuthenticatorOptions = new VerifyAuthenticatorOptions
            {
                Code = code,
            };

            try
            {
                var authnResponse = await idxAuthClient.VerifyAuthenticatorAsync(verifyAuthenticatorOptions, (IIdxContext)Session["idxContext"]);

                switch (authnResponse.AuthenticationStatus)
                {
                    case AuthenticationStatus.AwaitingPasswordReset:
                        // TODO: Force authentication and redirect to home page
                        Session["idxContext"] = authnResponse.IdxContext;
                        return RedirectToAction("ChangePassword", "Manage");

                    case AuthenticationStatus.AwaitingAuthenticatorEnrollment:
                        Session["idxContext"] = authnResponse.IdxContext;
                        TempData["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(authnResponse.Authenticators);
                        return RedirectToAction("selectAuthenticator", "Manage");

                    case AuthenticationStatus.Success:
                        var userName = (string)Session["UserName"] ?? string.Empty;
                        var identity = AuthenticationHelper.GetIdentityFromAuthResponse(userName, authnResponse);
                        _authenticationManager.SignIn(new AuthenticationProperties(), identity);
                        return RedirectToAction("Index", "Home");
                }
                
                return View(view, model);
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return View(view, model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyAuthenticatorAsync(VerifyAuthenticatorViewModel model)
        {
            return await VerifyAuthenticatorAsync(model.Code, "VerifyAuthenticator", model);
        }

        public async Task<ActionResult> EnrollPhoneAuthenticatorAsync(EnrollPhoneViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EnrollPhoneAuthenticator", model);
            }

            try
            {
                // WIP
                var idxAuthClient = new IdxClient(null);

                var enrollPhoneAuthenticatorOptions = new EnrollPhoneAuthenticatorOptions
                {
                    AuthenticatorId = Session["phoneId"].ToString(),
                    PhoneNumber = model.PhoneNumber,
                    MethodType = AuthenticatorMethodType.Sms,
                };

                var enrollResponse = await idxAuthClient.EnrollAuthenticatorAsync(enrollPhoneAuthenticatorOptions, (IIdxContext)Session["IdxContext"]);

                if (enrollResponse.AuthenticationStatus == AuthenticationStatus.AwaitingAuthenticatorVerification)
                {
                    // TODO: clean session.
                    Session["IdxContext"] = enrollResponse.IdxContext;
                    
                    return RedirectToAction("VerifyAuthenticator", "Manage");
                }

                return View("EnrollPhoneAuthenticator", model);
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return RedirectToAction("EnrollPhoneAuthenticator", model);
            }
        }

        public ActionResult EnrollPhoneAuthenticator()
        {
            return View();
        }

        public ActionResult SelectPhoneChallengeMethod()
        {
           return View((SelectAuthenticatorMethodViewModel)TempData["selectMethodModel"]);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectPhoneChallengeMethodAsync(SelectAuthenticatorMethodViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SelectPhoneChallengeMethod", model);
            }

            try
            {
                var challengeOptions = new ChallengePhoneAuthenticatorOptions
                {
                    AuthenticatorId = model.AuthenticatorId,
                    EnrollmentId = model.EnrollmentId,
                    MethodType = AuthenticatorMethodType.Sms,
                };

                var idxAuthClient = new IdxClient(null);

                var challengeResponse = await idxAuthClient.ChallengeAuthenticatorAsync(challengeOptions, (IIdxContext)Session["IdxContext"]);

                switch (challengeResponse?.AuthenticationStatus)
                {
                    case AuthenticationStatus.AwaitingAuthenticatorVerification:
                        return RedirectToAction("VerifyAuthenticator", "Manage");
                    default:
                        return View("SelectPhoneChallengeMethod", model);
                }
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return RedirectToAction("SelectPhoneChallengeMethod", model);
            }
        }

        public ActionResult SelectAuthenticator()
        {
            var viewModel = new SelectAuthenticatorViewModel();
            viewModel.Authenticators = (IList<AuthenticatorViewModel>)TempData["authenticators"];
            viewModel.PasswordId = viewModel.Authenticators.FirstOrDefault(x => x.Name.ToLower() == "password")?.AuthenticatorId;
            viewModel.PhoneId = viewModel.Authenticators.FirstOrDefault(x => x.Name.ToLower() == "phone")?.AuthenticatorId;

            TempData["authenticators"] = viewModel.Authenticators;
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
                var isChallengeFlow = (bool?)Session["isChallengeFlow"] ?? false;
                Session["isPhoneSelected"] = model.IsPhoneSelected;
                Session["phoneId"] = model.PhoneId;
                var authenticators = (IList<AuthenticatorViewModel>)TempData["authenticators"];

                if (isChallengeFlow)
                {
                    AuthenticationResponse selectAuthenticatorResponse = null;

                    if (model.IsPhoneSelected)
                    {
                        var selectPhoneOptions = new SelectPhoneAuthenticatorOptions();
                        ((SelectPhoneAuthenticatorOptions)selectPhoneOptions).EnrollmentId = authenticators
                                                                    .FirstOrDefault(x => x.AuthenticatorId == model.AuthenticatorId)?
                                                                    .EnrollmentId;
                        selectPhoneOptions.AuthenticatorId = model.AuthenticatorId;

                        selectAuthenticatorResponse = await idxAuthClient.SelectChallengeAuthenticatorAsync(selectPhoneOptions, (IIdxContext)Session["IdxContext"]);
                    }
                    else
                    {
                        var selectAuthenticatorOptions = new SelectAuthenticatorOptions
                        {
                            AuthenticatorId = model.AuthenticatorId,
                        };

                        selectAuthenticatorResponse = await idxAuthClient.SelectChallengeAuthenticatorAsync(selectAuthenticatorOptions, (IIdxContext)Session["IdxContext"]);
                    }
                    
                    Session["IdxContext"] = selectAuthenticatorResponse.IdxContext;

                    switch (selectAuthenticatorResponse?.AuthenticationStatus)
                    {
                        // TODO: Review
                        //case AuthenticationStatus.AwaitingAuthenticatorEnrollmentData:
                        //    return RedirectToAction("SelectPhoneChallengeMethod", 
                        //        new SelectAuthenticatorMethodViewModel
                        //        {
                        //            Profile = selectAuthenticatorResponse.CurrentAuthenticatorEnrollment.Profile,
                        //            EnrollmentId = selectAuthenticatorResponse.CurrentAuthenticatorEnrollment.Id,
                        //            AuthenticatorId = model.AuthenticatorId,
                        //        });
                        case AuthenticationStatus.AwaitingChallengeAuthenticatorData:
                            TempData["selectMethodModel"] = new SelectAuthenticatorMethodViewModel
                            {
                                Profile = selectAuthenticatorResponse.CurrentAuthenticatorEnrollment.Profile,
                                EnrollmentId = selectAuthenticatorResponse.CurrentAuthenticatorEnrollment.EnrollmentId,
                                AuthenticatorId = model.AuthenticatorId,
                            };

                            return RedirectToAction("SelectPhoneChallengeMethod", "Manage");
                        case AuthenticationStatus.AwaitingAuthenticatorVerification:
                            return RedirectToAction("VerifyAuthenticator", "Manage");
                        default:
                            return View("SelectAuthenticator", model);
                    }
                    // TODO
                    return View("SelectAuthenticator", model);
                }
                else
                {
                    var enrollAuthenticatorOptions = new EnrollAuthenticatorOptions
                    {
                        AuthenticatorId = model.AuthenticatorId,
                    };

                    var enrollResponse = await idxAuthClient.EnrollAuthenticatorAsync(enrollAuthenticatorOptions, (IIdxContext)Session["IdxContext"]);

                    Session["IdxContext"] = enrollResponse.IdxContext;
                    Session["isPasswordSelected"] = model.IsPasswordSelected;

                    switch (enrollResponse?.AuthenticationStatus)
                    {
                        case AuthenticationStatus.AwaitingAuthenticatorVerification:
                            if (model.IsPasswordSelected)
                            {
                                return RedirectToAction("ChangePassword", "Manage");
                            }

                            return RedirectToAction("VerifyAuthenticator", "Manage");

                        case AuthenticationStatus.AwaitingAuthenticatorEnrollmentData:
                            return RedirectToAction("EnrollPhoneAuthenticator", "Manage");

                        default:
                            return View("SelectAuthenticator", model);
                    }
                }
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return RedirectToAction("SelectAuthenticator", model);
            }
        }

    }
}