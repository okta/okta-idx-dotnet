namespace embedded_auth_with_sdk.Controllers
{
    using embedded_auth_with_sdk.Models;
    using Microsoft.Owin.Security;
    using Okta.Idx.Sdk;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class OktaVerifyController : Controller
    {
        private readonly IAuthenticationManager _authenticationManager;

        private readonly IIdxClient _idxClient;

        public OktaVerifyController(IAuthenticationManager authenticationManager, IIdxClient idxClient)
        {
            _authenticationManager = authenticationManager;
            _idxClient = idxClient;
        }

        public ActionResult SelectAuthenticatorMethod()
        {
            var model = (OktaVerifySelectAuthenticatorMethodModel)Session[nameof(OktaVerifySelectAuthenticatorMethodModel)];
            if (!string.IsNullOrEmpty(model.Message))
            {
                ModelState.AddModelError("", model.Message);
            }
            return View(model);
        }

        public ActionResult Enroll()
        {
            var oktaVerifyAuthenticator = (IAuthenticator)Session["oktaVerifyAuthenticator"];
            return View("EnrollWithQrCode", new OktaVerifyEnrollPollModel(oktaVerifyAuthenticator.ContextualData.QrCode.Href));
        }

        [HttpGet]
        public ActionResult SelectEnrollmentChannel()
        {
            var oktaVerifyAuthenticator = (IAuthenticator)Session["oktaVerifyAuthenticator"];
            var selectEnrollmentChannelModel = new OktaVerifySelectEnrollmentChannelModel(oktaVerifyAuthenticator);
            return View(selectEnrollmentChannelModel);
        }

        [HttpPost]
        public async Task<ActionResult> SelectEnrollmentChannel(OktaVerifySelectEnrollmentChannelModel model)
        {
            var oktaVerifyAuthenticator = (IAuthenticator)Session["oktaVerifyAuthenticator"];
            if (!ModelState.IsValid)
            {
                var selectEnrollmentChannelViewModel = new OktaVerifySelectEnrollmentChannelModel(oktaVerifyAuthenticator)
                {
                    SelectedChannel = model.SelectedChannel,
                };
                return View(selectEnrollmentChannelViewModel);
            }

            var idxContext = (IIdxContext)Session["idxContext"];
            var enrollOktaVerifyOptions = new EnrollOktaVerifyAuthenticatorOptions
            {
                Channel = model.SelectedChannel,
                AuthenticatorId = model.AuthenticatorId
            };

            await _idxClient.SelectEnrollAuthenticatorAsync(enrollOktaVerifyOptions, idxContext);

            switch (model.SelectedChannel)
            {
                case "email":
                    return View("EnrollWithEmail");
                case "sms":
                    return View("EnrollWithPhoneNumber", new OktaVerifyEnrollWithPhoneNumberModel { CountryCode = "+1" });
            }

            ModelState.AddModelError("SelectedChannel", new ArgumentException($"Unrecognized Okta Verify channel: {model.SelectedChannel}"));
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EnrollWithEmail(OktaVerifyEnrollWithEmailModel emailModel)
        {
            var idxContext = (IIdxContext)Session["idxContext"];
            var okaVerifyEnrollWithEmailOptions = new EnrollOktaVerifyAuthenticatorOptions
            {
                Channel = "email",
                Email = emailModel.Email,
            };

            _ = await _idxClient.EnrollAuthenticatorAsync(okaVerifyEnrollWithEmailOptions, idxContext);
            var oktaVerifyEnrollModel = new OktaVerifyEnrollPollModel()
            {
                Message = "An activation link was sent to your email, use it to complete Okta Verify enrollment."
            };

            return View("EnrollPoll", oktaVerifyEnrollModel);
        }

        [HttpPost]
        public async Task<ActionResult> EnrollWithPhoneNumber(OktaVerifyEnrollWithPhoneNumberModel phoneNumberModel)
        {
            var idxContext = (IIdxContext)Session["idxContext"];

            var oktaVerifEnrollAuthenticatorOptions = new EnrollOktaVerifyAuthenticatorOptions()
            {
                Channel = "sms",
                PhoneNumber = phoneNumberModel.PhoneNumber,
            };

            _ = await _idxClient.EnrollAuthenticatorAsync(oktaVerifEnrollAuthenticatorOptions, idxContext);
            var oktaVerifyEnrollModel = new OktaVerifyEnrollPollModel()
            {
                Message = "An activation link was sent to your phone via sms, use it to complete Okta Verify enrollment."
            };

            return View("EnrollPoll", oktaVerifyEnrollModel);
        }

        public async Task<ActionResult> EnrollPoll()
        {
            var idxContext = (IIdxContext)Session["idxContext"];
            var pollResponse = await _idxClient.PollAuthenticatorEnrollmentStatusAsync(idxContext);
            TempData["canSkip"] = pollResponse.CanSkip;

            var pollViewModel = new OktaVerifyPollResponseModel
            {
                Refresh = pollResponse.Refresh,
                ContinuePolling = pollResponse.ContinuePolling,
                Next = "/Account/Login",
            };

            if (!pollResponse.ContinuePolling)
            {
                switch (pollResponse?.AuthenticationStatus)
                {
                    case AuthenticationStatus.Success:
                        ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromTokenResponseAsync(_idxClient.Configuration, pollResponse.TokenInfo);
                        _authenticationManager.SignIn(new AuthenticationProperties(), identity);
                        pollViewModel.Next = "/Home/Index";
                        break;
                    case AuthenticationStatus.PasswordExpired:
                        pollViewModel.Next = "/Manage/ChangePassword";
                        break;
                    case AuthenticationStatus.AwaitingChallengeAuthenticatorSelection:
                        Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(pollResponse.Authenticators);
                        Session["isChallengeFlow"] = true;
                        pollViewModel.Next = "/Manage/SelectAuthenticator";
                        break;
                    case AuthenticationStatus.AwaitingAuthenticatorEnrollment:
                        Session["isChallengeFlow"] = false;
                        Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(pollResponse.Authenticators);
                        pollViewModel.Next = "/Manage/SelectAuthenticator";
                        break;
                    default:
                        pollViewModel.Next = "/Account/Login";
                        break;
                }
            }
            
            return Json(pollViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SelectAuthenticatorMethodAsync(OktaVerifySelectAuthenticatorMethodModel model)
        {
            var selectAuthenticatorOptions = new SelectOktaVerifyAuthenticatorOptions
            {
                AuthenticatorMethodType = model.MethodType,
                AuthenticatorId = model.AuthenticatorId,
            };
            
            try
            {
                var authnResponse = await _idxClient.SelectChallengeAuthenticatorAsync(selectAuthenticatorOptions,
                    (IIdxContext)Session["IdxContext"]);

                if (authnResponse.AuthenticationStatus == AuthenticationStatus.AwaitingAuthenticatorVerification ||
                    authnResponse.AuthenticationStatus == AuthenticationStatus.AwaitingChallengePollResponse)
                {
                    switch (model.MethodType)
                    {
                        case "totp":
                            return View("EnterCode");
                        case "push":
                            return View("PushSent", model);
                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("MethodType", e);
                return View("SelectAuthenticatorMethod", model);
            }

            return View(new OktaVerifySelectAuthenticatorMethodModel());
        }
        
        [HttpPost]
        public async Task<ActionResult> EnterCodeAsync(OktaVerifyEnterCodeModel oktaVerifyEnterCodeModel)
        {
            var authenticationResponse = await _idxClient.VerifyAuthenticatorAsync(
                new OktaVerifyVerifyAuthenticatorOptions { TotpCode = oktaVerifyEnterCodeModel.Code },
                (IIdxContext)Session["idxContext"]);

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
                    return View("Login", "Account");
            }            
        }

        public async Task<ActionResult> ChallengePoll()
        {
            var pollResponse = await _idxClient.PollAuthenticatorPushStatusAsync((IIdxContext)Session["idxContext"]);
            var pollViewModel = new OktaVerifyPollResponseModel
            {
                Refresh = pollResponse.Refresh ?? 4000,
                ContinuePolling = pollResponse.ContinuePolling,
            };

            if (!pollResponse.ContinuePolling)
            {
                switch (pollResponse.AuthenticationStatus)
                {
                    case AuthenticationStatus.Success:
                        ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromTokenResponseAsync(_idxClient.Configuration, pollResponse.TokenInfo);
                        _authenticationManager.SignIn(new AuthenticationProperties(), identity);
                        pollViewModel.Next = "/Home/Index";
                        break;
                    case AuthenticationStatus.PushChallengeFailed:
                        var model = (OktaVerifySelectAuthenticatorMethodModel)Session[nameof(OktaVerifySelectAuthenticatorMethodModel)];
                        model.Message = "Authentication failed";
                        pollViewModel.Next = "/OktaVerify/SelectAuthenticatorMethod";
                        break;
                    default:
                        return View("Login", "Account");
                }
            }

            return Json(pollViewModel, JsonRequestBehavior.AllowGet);
        }
    }
}
