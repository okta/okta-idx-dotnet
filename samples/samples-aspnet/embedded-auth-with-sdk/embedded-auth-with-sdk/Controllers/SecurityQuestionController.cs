namespace embedded_auth_with_sdk.Controllers
{
    using embedded_auth_with_sdk.Models;
    using Microsoft.Owin.Security;
    using Okta.Idx.Sdk;
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
            return View(new SecurityQuestionModel
            {
                QuestionKey = "custom",
                Questions = securityQuestionAuthenticator.ContextualData.Questions.ToList(),
            });
        }

        public async Task<ActionResult> Answer(SecurityQuestionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Select", model);
            }
            var idxContext = (IIdxContext)Session["idxContext"];
            var securityQuestionOptions = new SecurityQuestionAuthenticatorOptions
            {
                Answer = model.Answer,
                Question = model.Question,
                QuestionKey = model.QuestionKey,
            };

            var securityQuestionResponse =  await _idxClient.VerifyAuthenticatorAsync(securityQuestionOptions, idxContext);
            switch (securityQuestionResponse?.AuthenticationStatus)
            {
                case AuthenticationStatus.Success:
                    ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromTokenResponseAsync(_idxClient.Configuration, securityQuestionResponse.TokenInfo);
                    _authenticationManager.SignIn(new AuthenticationProperties(), identity);
                    return RedirectToAction("Index", "Home");
                case AuthenticationStatus.PasswordExpired:
                    return RedirectToAction("ChangePassword", "Manage");
                case AuthenticationStatus.AwaitingChallengeAuthenticatorSelection:
                    Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(securityQuestionResponse.Authenticators);
                    Session["isChallengeFlow"] = true;
                    return RedirectToAction("SelectAuthenticator", "Manage");
                case AuthenticationStatus.AwaitingAuthenticatorEnrollment:
                    Session["isChallengeFlow"] = false;
                    Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(securityQuestionResponse.Authenticators);
                    return RedirectToAction("SelectAuthenticator", "Manage");
                default:
                    return RedirectToAction("Login", "Account");
            }
        }
    }
}
