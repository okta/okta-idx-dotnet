namespace embedded_auth_with_sdk.Controllers
{
    using embedded_auth_with_sdk.Models;
    using Microsoft.Owin.Security;
    using Okta.Idx.Sdk;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class SecurityQuestionController : Controller
    {
        private readonly IAuthenticationManager _authenticationManager;

        private readonly IIdxClient _idxClient;

        public SecurityQuestionController(IAuthenticationManager authenticationManager, IIdxClient idxClient)
        {
            _authenticationManager = authenticationManager;
            _idxClient = idxClient;
        }

        public ActionResult EnrollSecurityQuestion()
        {
            var securityQuestionAuthenticator = (IAuthenticator)Session["securityQuestionAuthenticator"];
            return View(new EnrollSecurityQuestionModel
            {
                QuestionKeys = securityQuestionAuthenticator.ContextualData.QuestionKeys.ToList(),
                Questions = securityQuestionAuthenticator.ContextualData.Questions.ToList(),
            });
        }

        public async Task<ActionResult> EnrollSecurityQuestionAsync(EnrollSecurityQuestionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EnrollSecurityQuestion", model);
            }
            try
            {
                var idxContext = (IIdxContext)Session["idxContext"];
                var securityQuestionOptions = new EnrollSecurityQuestionAuthenticatorOptions
                {
                    Answer = model.Answer,
                    QuestionKey = model.QuestionKey,
                };

                var securityQuestionResponse = await _idxClient.EnrollAuthenticatorAsync(securityQuestionOptions, idxContext);
                TempData["canSkip"] = securityQuestionResponse.CanSkip;
                return await RedirectForStatusAsync(securityQuestionResponse);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Answer", $"An exception occurred: ({ex.Message})");
                return View("EnrollSecurityQuestion", model);
            }
        }

        [HttpGet]
        public ActionResult EnrollCustomSecurityQuestion()
        {
            return View(new EnrollCustomSecurityQuestionModel());
        }

        [HttpPost]
        public async Task<ActionResult> EnrollCustomSecurityQuestionAsync(EnrollCustomSecurityQuestionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EnrollCustomSecurityQuestion", model);
            }
            try
            {
                var idxContext = (IIdxContext)Session["idxContext"];
                var securityQuestionOptions = new EnrollSecurityQuestionAuthenticatorOptions
                {
                    Answer = model.Answer,
                    Question = model.Question,
                    QuestionKey = model.QuestionKey,
                };

                var securityQuestionEnrollResponse = await _idxClient.EnrollAuthenticatorAsync(securityQuestionOptions, idxContext);
                TempData["canSkip"] = securityQuestionEnrollResponse.CanSkip;
                return await RedirectForStatusAsync(securityQuestionEnrollResponse);
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("Answer", $"An exception occurred: ({ex.Message})");
                return View("CreateQuestion", model);
            }
        }

        [HttpGet]
        public ActionResult VerifyAuthenticator()
        {
            var model = (AnswerSecurityQuestionModel)Session[nameof(AnswerSecurityQuestionModel)];
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> VerifyAuthenticatorAsync(AnswerSecurityQuestionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("VerifyAuthenticator", model);
            }
            try
            {
                var idxContext = (IIdxContext)Session["idxContext"];
                var securityQuestionOptions = new VerifySecurityQuestionAuthenticatorOptions
                {
                    Answer = model.Answer,
                    QuestionKey = model.QuestionKey,
                };

                var securityQuestionAuthenticateResponse = await _idxClient.VerifyAuthenticatorAsync(securityQuestionOptions, idxContext);
                TempData["canSkip"] = securityQuestionAuthenticateResponse.CanSkip;
                return await RedirectForStatusAsync(securityQuestionAuthenticateResponse);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Answer", $"An exception occurred: ({ex.Message})");
                return View("VerifyAuthenticator", model);
            }
        }

        private async Task<RedirectToRouteResult> RedirectForStatusAsync(AuthenticationResponse authenticationResponse)
        {
            switch (authenticationResponse?.AuthenticationStatus)
            {
                case AuthenticationStatus.Success:
                    ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromTokenResponseAsync(_idxClient.Configuration, authenticationResponse.TokenInfo);
                    _authenticationManager.SignIn(new AuthenticationProperties(), identity);
                    return RedirectToAction("Index", "Home");
                case AuthenticationStatus.PasswordExpired:
                    return RedirectToAction("ChangePassword", "Manage");
                case AuthenticationStatus.AwaitingChallengeAuthenticatorSelection:
                    Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(authenticationResponse.Authenticators);
                    Session["isChallengeFlow"] = true;
                    return RedirectToAction("SelectAuthenticator", "Manage");
                case AuthenticationStatus.AwaitingAuthenticatorEnrollment:
                    Session["isChallengeFlow"] = false;
                    Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(authenticationResponse.Authenticators);
                    return RedirectToAction("SelectAuthenticator", "Manage");
                default:
                    return RedirectToAction("Login", "Account");
            }
        }
    }
}
