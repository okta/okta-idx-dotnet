using System;

namespace embedded_auth_with_sdk.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using embedded_auth_with_sdk.Models;

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
                        ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromTokenResponseAsync(_idxClient.Configuration, authnResponse.TokenInfo);
                        _authenticationManager.SignIn(new AuthenticationProperties(), identity);
                        return RedirectToAction("Index", "Home");

                    case AuthenticationStatus.AwaitingAuthenticatorEnrollment:
                        Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(authnResponse.Authenticators);
                        TempData["canSkip"] = authnResponse.CanSkip;
                        return RedirectToAction("SelectAuthenticator", "Manage");
                    case AuthenticationStatus.AwaitingChallengeAuthenticatorSelection:
                        Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(authnResponse.Authenticators);
                        Session["isChallengeFlow"] = true;
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

        public async Task<ActionResult> VerifyWebAuthnAuthenticator()
        {
            var isVerificationCompleted = false;
            Boolean.TryParse(Request.Params["verificationCompleted"], out isVerificationCompleted);

            if (isVerificationCompleted)
            {
                var authnResponse = (IAuthenticationResponse)Session["webAuthnResponse"];

                switch (authnResponse?.AuthenticationStatus)
                {
                    case AuthenticationStatus.Success:
                        ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromTokenResponseAsync(_idxClient.Configuration, authnResponse.TokenInfo);
                        _authenticationManager.SignIn(identity);
                        return RedirectToAction("Index", "Home");

                    case AuthenticationStatus.AwaitingAuthenticatorEnrollment:
                        Session["isChallengeFlow"] = false;
                        Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(authnResponse.Authenticators);
                        TempData["canSkip"] = authnResponse.CanSkip;
                        return RedirectToAction("SelectAuthenticator", "Manage");

                    default:
                        return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                var currentAuthenticator = (IAuthenticator)Session["currentWebAuthnAuthenticator"];

                var viewModel = new VerifyWebAuthnViewModel
                {
                    DisplayName = currentAuthenticator.Name,
                    UserId = currentAuthenticator.ContextualData.ActivationData.User.Id,
                    Username = currentAuthenticator.ContextualData.ActivationData.User.Name,
                    Challenge = currentAuthenticator.ContextualData.ActivationData.Challenge,
                };

                return View(viewModel);
            }
        }

        [HttpPost]
        public async Task<IAuthenticationResponse> VerifyWebAuthnAuthenticatorAsync(VerifyWebAuthnViewModel viewModel)
        {
            var authnResponse = await _idxClient.VerifyAuthenticatorAsync(new VerifyWebAuthnAuthenticatorOptions
            {
                Attestation = viewModel.Attestation,
                ClientData = viewModel.ClientData,
            }, (IIdxContext)Session["idxContext"]);

            Session["webAuthnResponse"] = authnResponse;

            return authnResponse;
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
                        Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(authnResponse.Authenticators);
                        TempData["canSkip"] = authnResponse.CanSkip;
                        Session["isChallengeFlow"] = false;
                        return RedirectToAction("SelectAuthenticator", "Manage");

                    case AuthenticationStatus.Success:
                        ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromTokenResponseAsync(_idxClient.Configuration, authnResponse.TokenInfo);
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
        public async Task<ActionResult> ResendAuthenticatorAsync()
        {
            try
            {
                var authnResponse = await _idxClient.ResendCodeAsync((IIdxContext)Session["idxContext"]);
                Session["idxContext"] = authnResponse.IdxContext;

                return View("VerifyAuthenticator");
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return View("VerifyAuthenticator");
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
                model.MethodTypes = (List<AuthenticatorMethodType>)Session["methodTypes"];
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
                model.MethodTypes = (List<AuthenticatorMethodType>)Session["methodTypes"];
                return View("EnrollPhoneAuthenticator", model);
            }
        }

        public ActionResult EnrollPhoneAuthenticator()
        {
            var methodTypes = (List<AuthenticatorMethodType>)Session["methodTypes"]
                              ?? new List<AuthenticatorMethodType>();
            var model = new EnrollPhoneViewModel
                            {
                                MethodTypes = methodTypes,
                                MethodType = methodTypes.FirstOrDefault(),
                            };

            return View(model);
        }

        public ActionResult SelectPhoneChallengeMethod(SelectAuthenticatorMethodViewModel model)
        {
           return View(model);
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
                return View("SelectPhoneChallengeMethod", model);
            }
        }
        
        public ActionResult SelectAuthenticator()
        {
            var authenticators = (IList<AuthenticatorViewModel>)Session["authenticators"] ?? new List<AuthenticatorViewModel>();
            
            var viewModel = new SelectAuthenticatorViewModel
                                {
                                    Authenticators = authenticators,
                                    AuthenticatorId = authenticators.FirstOrDefault()?.AuthenticatorId,
                                    PasswordId = authenticators.FirstOrDefault(x => x.Name.ToLower() == "password")?.AuthenticatorId,
                                    PhoneId = authenticators.FirstOrDefault(x => x.Name.ToLower() == "phone")?.AuthenticatorId,
                                    WebAuthnId = authenticators.FirstOrDefault(x => x.Name.ToLower() == "security key or biometric")?.AuthenticatorId,
                                    CanSkip = TempData["canSkip"] != null && (bool)TempData["canSkip"]
                                };

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
                        ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromTokenResponseAsync(_idxClient.Configuration, skipSelectionResponse.TokenInfo);
                        _authenticationManager.SignIn(new AuthenticationProperties(), identity);
                        return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Index", "Home");
            }
            catch (TerminalStateException exception)
            {
                TempData["TerminalStateMessage"] = exception.Message;
                return RedirectToAction("Login", "Account");
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
            var authenticators = (IList<AuthenticatorViewModel>)Session["authenticators"];

            if (!ModelState.IsValid)
            {
                model.Authenticators = authenticators;
                return View("SelectAuthenticator", model);
            }

            try
            {
                var isChallengeFlow = (bool?)Session["isChallengeFlow"] ?? false;
                Session["isPhoneSelected"] = model.IsPhoneSelected;
                Session["phoneId"] = model.PhoneId;
                Session["isWebAuthnSelected"] = model.IsWebAuthnSelected;

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
                        case AuthenticationStatus.AwaitingChallengeAuthenticatorData:
                            var methodViewModel = new SelectAuthenticatorMethodViewModel
                            {
                                Profile = selectAuthenticatorResponse.CurrentAuthenticatorEnrollment.Profile,
                                EnrollmentId = selectAuthenticatorResponse.CurrentAuthenticatorEnrollment.EnrollmentId,
                                AuthenticatorId = model.AuthenticatorId,
                                MethodTypes = selectAuthenticatorResponse.CurrentAuthenticatorEnrollment.MethodTypes,
                                MethodType = selectAuthenticatorResponse.CurrentAuthenticatorEnrollment.MethodTypes.FirstOrDefault(),
                            };
                            return View("SelectPhoneChallengeMethod", methodViewModel);
                        case AuthenticationStatus.AwaitingAuthenticatorVerification:
                            return RedirectToAction("VerifyAuthenticator", "Manage");
                        default:
                            return View("SelectAuthenticator", model);
                    }
                }
                else
                {
                    var enrollAuthenticatorOptions = new SelectEnrollAuthenticatorOptions
                    {
                        AuthenticatorId = model.AuthenticatorId,
                    };

                    var enrollResponse = await _idxClient.SelectEnrollAuthenticatorAsync(enrollAuthenticatorOptions, (IIdxContext)Session["IdxContext"]);

                    Session["IdxContext"] = enrollResponse.IdxContext;
                    Session["isPasswordSelected"] = model.IsPasswordSelected;

                    switch (enrollResponse?.AuthenticationStatus)
                    {
                        case AuthenticationStatus.AwaitingAuthenticatorVerification:
                            if (model.IsPasswordSelected)
                            {
                                return RedirectToAction("ChangePassword", "Manage");
                            }
                            else if (model.IsWebAuthnSelected)
                            {
                                Session["currentWebAuthnAuthenticator"] = enrollResponse.CurrentAuthenticator;
                                return RedirectToAction("VerifyWebAuthnAuthenticator", "Manage");
                            }

                            return RedirectToAction("VerifyAuthenticator", "Manage"); 

                        case AuthenticationStatus.AwaitingAuthenticatorEnrollmentData:
                            Session["methodTypes"] = enrollResponse.CurrentAuthenticator.MethodTypes;
                            return RedirectToAction("EnrollPhoneAuthenticator", "Manage");
                            

                        default:
                            return View("SelectAuthenticator", model);
                    }
                }
            }
            catch (OktaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return View("SelectAuthenticator", model);
            }
        }

        public ActionResult SelectRecoveryAuthenticator()
        {
            var viewModel = new SelectRecoveryAuthenticatorViewModel
                                {
                                    Authenticators = (List<AuthenticatorViewModel>)Session["authenticators"],
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