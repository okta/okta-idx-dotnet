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

        public ActionResult ChooseQuestion()
        {
            var securityQuestionAuthenticator = (IAuthenticator)Session["securityQuestionAuthenticator"];
            return View(new SecurityQuestionModel
            {
                QuestionKeys = securityQuestionAuthenticator.ContextualData.QuestionKeys.ToList(),
                Questions = securityQuestionAuthenticator.ContextualData.Questions.ToList(),
            });
        }

        public ActionResult CreateQuestion()
        {
            var securityQuestionAuthenticator = (IAuthenticator)Session["securityQuestionAuthenticator"];
            return View(new CustomSecurityQuestionModel
            {
                QuestionKey = "custom",
            });
        }

        public async Task<ActionResult> AnswerQuestion(SecurityQuestionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Select", model);
            }
            try
            {
                var idxContext = (IIdxContext)Session["idxContext"];
                var securityQuestionOptions = new SecurityQuestionAuthenticatorOptions
                {
                    Answer = model.Answer,
                    QuestionKey = model.QuestionKey,
                };

                var securityQuestionResponse = await _idxClient.VerifyAuthenticatorAsync(securityQuestionOptions, idxContext);
                TempData["canSkip"] = securityQuestionResponse.CanSkip;
                return await RedirectForStatusAsync(securityQuestionResponse);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Answer", $"An exception occurred: ({ex.Message})");
                return View("ChooseQuestion", model);
            }
        }

        public async Task<ActionResult> AnswerCustomQuestion(CustomSecurityQuestionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Select", model);
            }
            try
            {

                var idxContext = (IIdxContext)Session["idxContext"];
                var securityQuestionOptions = new SecurityQuestionAuthenticatorOptions
                {
                    Answer = model.Answer,
                    Question = model.Question,
                    QuestionKey = model.QuestionKey,
                };

                var securityQuestionResponse = await _idxClient.VerifyAuthenticatorAsync(securityQuestionOptions, idxContext);
                TempData["canSkip"] = securityQuestionResponse.CanSkip;
                return await RedirectForStatusAsync(securityQuestionResponse);
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("Answer", $"An exception occurred: ({ex.Message})");
                return View("CreateQuestion", model);
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
