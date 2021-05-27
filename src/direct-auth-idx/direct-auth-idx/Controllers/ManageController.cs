namespace direct_auth_idx.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using direct_auth_idx.Models;

    using Microsoft.Owin.Security;

    using Okta.Idx.Sdk;
    using Okta.Sdk.Abstractions;

    public class ManageController : Controller
    {

        private readonly IAuthenticationManager _authenticationManager;

        private readonly IIdxClient _idxClient;
        public ManageController(IAuthenticationManager authenticationManager, IIdxClient idxClient)
        {
            _authenticationManager = authenticationManager;
            _idxClient = idxClient;
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
            
            try
            {
                var authnResponse = await _idxClient.ChangePasswordAsync(changePasswordOptions, (IIdxContext)Session["idxContext"]).ConfigureAwait(false);
                Session["idxContext"] = authnResponse.IdxContext;

                switch (authnResponse.AuthenticationStatus)
                {
                    case AuthenticationStatus.Success:
                        ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromAuthResponseAsync(_idxClient.Configuration, authnResponse);
                        _authenticationManager.SignIn(new AuthenticationProperties(), identity);
                        return RedirectToAction("Index", "Home");

                    case AuthenticationStatus.AwaitingAuthenticatorEnrollment:
                        TempData["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(authnResponse.Authenticators);
                        TempData["canSkip"] = authnResponse.CanSkip;
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
            
            var verifyAuthenticatorOptions = new VerifyAuthenticatorOptions
            {
                Code = code,
            };

            try
            {
                var authnResponse = await _idxClient.VerifyAuthenticatorAsync(verifyAuthenticatorOptions, (IIdxContext)Session["idxContext"]);
                Session["idxContext"] = authnResponse.IdxContext;

                switch (authnResponse.AuthenticationStatus)
                {
                    case AuthenticationStatus.AwaitingPasswordReset:
                        return RedirectToAction("ChangePassword", "Manage");

                    case AuthenticationStatus.AwaitingAuthenticatorEnrollment:
                        TempData["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(authnResponse.Authenticators);
                        TempData["canSkip"] = authnResponse.CanSkip;
                        return RedirectToAction("selectAuthenticator", "Manage");

                    case AuthenticationStatus.Success:
                        ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromAuthResponseAsync(_idxClient.Configuration, authnResponse);
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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnrollPhoneAuthenticatorAsync(EnrollPhoneViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EnrollPhoneAuthenticator", model);
            }

            try
            {

                var enrollPhoneAuthenticatorOptions = new EnrollPhoneAuthenticatorOptions
                {
                    AuthenticatorId = Session["phoneId"].ToString(),
                    PhoneNumber = model.PhoneNumber,
                    MethodType = model.MethodType,
                };

                var enrollResponse = await _idxClient.EnrollAuthenticatorAsync(enrollPhoneAuthenticatorOptions, (IIdxContext)Session["IdxContext"]);
                Session["IdxContext"] = enrollResponse.IdxContext;

                if (enrollResponse.AuthenticationStatus == AuthenticationStatus.AwaitingAuthenticatorVerification)
                {   
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
            var model = new EnrollPhoneViewModel
                            {
                                MethodTypes = (List<AuthenticatorMethodType>)TempData["methodTypes"] ?? new List<AuthenticatorMethodType>(),
                            };
            TempData["methodTypes"] = model.MethodTypes;
            return View(model);
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
                    MethodType = model.MethodType,
                };
                
                var challengeResponse = await _idxClient.ChallengeAuthenticatorAsync(challengeOptions, (IIdxContext)Session["IdxContext"]);

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
            var authenticators = (IList<AuthenticatorViewModel>)TempData["authenticators"];
            var viewModel = new SelectAuthenticatorViewModel
            {
                Authenticators = authenticators,
                PasswordId = authenticators.FirstOrDefault(x => x.Name.ToLower() == "password")?.AuthenticatorId,
                PhoneId = authenticators.FirstOrDefault(x => x.Name.ToLower() == "phone")?.AuthenticatorId,
                CanSkip = TempData["canSkip"] != null && (bool)TempData["canSkip"]
            };

            TempData["authenticators"] = viewModel.Authenticators;
            return View(viewModel);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SkipAuthenticatorSelectionAsync()
        {
            try
            {
                var skipSelectionResponse = await _idxClient.SkipAuthenticatorSelectionAsync((IIdxContext)Session["IdxContext"]);

                switch (skipSelectionResponse.AuthenticationStatus)
                {
                    case AuthenticationStatus.Success:
                        ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromAuthResponseAsync(_idxClient.Configuration, skipSelectionResponse);
                        _authenticationManager.SignIn(new AuthenticationProperties(), identity);
                        return RedirectToAction("Index", "Home");

                    case AuthenticationStatus.Terminal:
                        TempData["MessageToUser"] = skipSelectionResponse.MessageToUser;
                        return RedirectToAction("Login", "Account");
                }
                return RedirectToAction("Index", "Home");
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return RedirectToAction("SelectAuthenticator");
            }
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
                var isChallengeFlow = (bool?)Session["isChallengeFlow"] ?? false;
                Session["isPhoneSelected"] = model.IsPhoneSelected;
                Session["phoneId"] = model.PhoneId;
                var authenticators = (IList<AuthenticatorViewModel>)TempData["authenticators"];

                if (isChallengeFlow)
                {
                    AuthenticationResponse selectAuthenticatorResponse = null;

                    if (model.IsPhoneSelected)
                    {
                        var selectPhoneOptions = new SelectPhoneAuthenticatorOptions
                        {
                            EnrollmentId = authenticators
                                            .FirstOrDefault(x => x.AuthenticatorId == model.AuthenticatorId)?
                                            .EnrollmentId,
                            AuthenticatorId = model.AuthenticatorId
                        };

                        selectAuthenticatorResponse = await _idxClient.SelectChallengeAuthenticatorAsync(selectPhoneOptions, (IIdxContext)Session["IdxContext"]);
                    }
                    else
                    {
                        var selectAuthenticatorOptions = new SelectAuthenticatorOptions
                        {
                            AuthenticatorId = model.AuthenticatorId,
                        };

                        selectAuthenticatorResponse = await _idxClient.SelectChallengeAuthenticatorAsync(selectAuthenticatorOptions, (IIdxContext)Session["IdxContext"]);
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
                                MethodTypes = selectAuthenticatorResponse.CurrentAuthenticatorEnrollment.MethodTypes,
                            };

                            return RedirectToAction("SelectPhoneChallengeMethod", "Manage");
                        case AuthenticationStatus.AwaitingAuthenticatorVerification:
                            return RedirectToAction("VerifyAuthenticator", "Manage");
                        default:
                            return View("SelectAuthenticator", model);
                    }
                }
                else
                {
                    var enrollAuthenticatorOptions = new EnrollAuthenticatorOptions
                    {
                        AuthenticatorId = model.AuthenticatorId,
                    };

                    var enrollResponse = await _idxClient.EnrollAuthenticatorAsync(enrollAuthenticatorOptions, (IIdxContext)Session["IdxContext"]);

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
                            TempData["methodTypes"] = enrollResponse.CurrentAuthenticator.MethodTypes;
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

        public ActionResult SelectRecoveryAuthenticator()
        {
            var viewModel = new SelectRecoveryAuthenticatorViewModel
                                {
                                    Authenticators = (List<AuthenticatorViewModel>)TempData["authenticators"],
                                };
            TempData["authenticators"] = viewModel.Authenticators;
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectRecoveryAuthenticatorAsync(SelectRecoveryAuthenticatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SelectRecoveryAuthenticator", model);
            }

            try
            {
                var applyAuthenticatorResponse = await _idxClient.SelectRecoveryAuthenticatorAsync(
                                                     new SelectAuthenticatorOptions { AuthenticatorId = model.AuthenticatorId },
                                                     (IIdxContext)Session["IdxContext"]);

                Session["IdxContext"] = applyAuthenticatorResponse.IdxContext;
                Session["isPasswordSelected"] = false;

                if (applyAuthenticatorResponse.AuthenticationStatus == AuthenticationStatus.AwaitingAuthenticatorVerification)
                {
                    return RedirectToAction("VerifyAuthenticator", "Manage");
                }

                return View("SelectRecoveryAuthenticator", model);
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return View("SelectRecoveryAuthenticator", model);
            }
        }

    }
}