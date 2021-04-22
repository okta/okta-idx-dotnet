﻿using Okta.Idx.Sdk;
using Okta.Sdk.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using direct_auth_idx.Models;
using Microsoft.Owin.Security;

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
                        TempData["authenticators"] = authnResponse.Authenticators;
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

        public ActionResult SelectAuthenticator()
        {
            var authenticators = (IList<IAuthenticator>)TempData["authenticators"];

            var viewModel = new SelectAuthenticatorViewModel();
            viewModel.Authenticators = authenticators?
                                        .Select(x =>
                                                    new AuthenticatorViewModel
                                                    {
                                                        Id = x.Id,
                                                        Name = x.DisplayName
                                                    })
                                        .ToList() ?? new List<AuthenticatorViewModel>();

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