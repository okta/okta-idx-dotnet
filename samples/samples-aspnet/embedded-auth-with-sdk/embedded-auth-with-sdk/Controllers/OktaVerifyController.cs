

namespace embedded_auth_with_sdk.Controllers
{
    using embedded_auth_with_sdk.Models;
    using Microsoft.Owin.Security;
    using Okta.Idx.Sdk;
    using Okta.Idx.Sdk.OktaVerify;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
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
            var oktaVerifyAuthenticationOptions = (OktaVerifyAuthenticationOptions)Session[nameof(OktaVerifyAuthenticationOptions)];
            return View(new OktaVerifySelectAuthenticatorMethodModel(oktaVerifyAuthenticationOptions));
        }

        public ActionResult Enroll()
        {
            var oktaVerifyEnrollOptions = (OktaVerifyEnrollOptions)Session[nameof(OktaVerifyEnrollOptions)];
            return View("EnrollWithQrCode", new OktaVerifyEnrollPollModel(oktaVerifyEnrollOptions));
        }

        [HttpGet]
        public ActionResult SelectEnrollmentChannel()
        {
            var oktaVerifyEnrollOptions = (OktaVerifyEnrollOptions)Session[nameof(OktaVerifyEnrollOptions)];
            var selectEnrollmentChannelModel = new OktaVerifySelectEnrollmentChannelModel(oktaVerifyEnrollOptions);
            return View(selectEnrollmentChannelModel);
        }

        [HttpPost]
        public async Task<ActionResult> SelectEnrollmentChannel(OktaVerifySelectEnrollmentChannelModel model)
        {
            var oktaVerifyEnrollOptions = (OktaVerifyEnrollOptions)Session[nameof(OktaVerifyEnrollOptions)];
            if (!ModelState.IsValid)
            {
                var selectEnrollmentChannelViewModel = new OktaVerifySelectEnrollmentChannelModel(oktaVerifyEnrollOptions)
                {
                    SelectedChannel = model.SelectedChannel,
                };
                return View(selectEnrollmentChannelViewModel);
            }

            await oktaVerifyEnrollOptions.SelectEnrollmentChannelAsync(model.SelectedChannel);

            switch (model.SelectedChannel)
            {
                case "email":
                    return View("EnrollWithEmail");
                case "sms":
                    return View("EnrollWithPhoneNumber", new OktaVerifyEnrollWithPhoneNumberModel { CountryCode = "+1" });
            }

            throw new ArgumentException($"Unrecognized Okta Verify channel: {model.SelectedChannel}");
        }

        [HttpPost]
        public ActionResult EnrollWithEmail(OktaVerifyEnrollWithEmailModel emailModel)
        {
            var oktaVerifyEnrollOptions = (OktaVerifyEnrollOptions)Session[nameof(OktaVerifyEnrollOptions)];
            _ = oktaVerifyEnrollOptions.SendLinkToEmailAsync(emailModel.Email);
            var oktaVerifyEnrollModel = new OktaVerifyEnrollPollModel(oktaVerifyEnrollOptions)
            {
                Message = "An activation link was sent to your email, use it to complete Okta Verify enrollment."
            };

            return View("EnrollPoll", oktaVerifyEnrollModel);
        }

        [HttpPost]
        public ActionResult EnrollWithPhoneNumber(OktaVerifyEnrollWithPhoneNumberModel phoneNumberModel)
        {
            var oktaVerifyEnrollOptions = (OktaVerifyEnrollOptions)Session[nameof(OktaVerifyEnrollOptions)];
            _ = oktaVerifyEnrollOptions.SendLinkToPhoneNumberAsync($"{phoneNumberModel.CountryCode}{phoneNumberModel.PhoneNumber}");
            var oktaVerifyEnrollModel = new OktaVerifyEnrollPollModel(oktaVerifyEnrollOptions)
            {
                Message = "An activation link was sent to your phone via sms, use it to complete Okta Verify enrollment."
            };

            return View("EnrollPoll", oktaVerifyEnrollModel);
        }

        public async Task<ActionResult> EnrollPoll()
        {
            var oktaVerifyEnrollmentOptions = (OktaVerifyEnrollOptions)Session[nameof(OktaVerifyEnrollOptions)];

            var pollResponse = await oktaVerifyEnrollmentOptions.PollOnceAsync();
            if (!pollResponse.ContinuePolling)
            {
                pollResponse.Next = "/Account/Login";
            }

            return Json(pollResponse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SelectAuthenticatorMethod(string methodType)
        {
            var oktaVerifyAuthenticationOptions = (OktaVerifyAuthenticationOptions)Session[nameof(OktaVerifyAuthenticationOptions)];
            switch (methodType)
            {
                case "totp":
                    _ = await oktaVerifyAuthenticationOptions.SelectTotpMethodAsync();
                    return View("EnterCode");
                case "push":
                    _ = await oktaVerifyAuthenticationOptions.SelectPushMethodAsync();
                    return View("PushSent", new OktaVerifySelectAuthenticatorMethodModel(oktaVerifyAuthenticationOptions));
            }

            throw new ArgumentException($"Unrecognized Okta Verify authentication method: {methodType}");
        }

        [HttpPost]
        public async Task<ActionResult> EnterCode(OktaVerifyEnterCodeModel oktaVerifyEnterCodeModel)
        {
            var oktaVerifyAuthenticationOptions = (OktaVerifyAuthenticationOptions)Session[nameof(OktaVerifyAuthenticationOptions)];
            var authenticationResponse = await oktaVerifyAuthenticationOptions.EnterCodeAsync(oktaVerifyEnterCodeModel.Code);

            switch (authenticationResponse?.AuthenticationStatus)
            {
                case AuthenticationStatus.Success:
                    ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromTokenResponseAsync(_idxClient.Configuration, oktaVerifyAuthenticationOptions.TokenInfo);
                    _authenticationManager.SignIn(new AuthenticationProperties(), identity);
                    return RedirectToAction("Index", "Home");

                case AuthenticationStatus.PasswordExpired:
                    return RedirectToAction("ChangePassword", "Manage");

                case AuthenticationStatus.AwaitingChallengeAuthenticatorSelection:
                    if (authenticationResponse.IsOktaVerifyCurrentAuthenticator == true)
                    {
                        Session[nameof(OktaVerifyAuthenticationOptions)] = authenticationResponse.OktaVerifyAuthenticationOptions;
                        return RedirectToAction("SelectAuthenticatorMethod", "OktaVerify");
                    }
                    Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(authenticationResponse.Authenticators);
                    Session["isChallengeFlow"] = true;
                    return RedirectToAction("SelectAuthenticator", "Manage");
                case AuthenticationStatus.AwaitingAuthenticatorEnrollment:
                    Session["isChallengeFlow"] = false;
                    Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(authenticationResponse.Authenticators);
                    return RedirectToAction("SelectAuthenticator", "Manage");
                case AuthenticationStatus.Exception:
                    ModelState.AddModelError("Code", authenticationResponse.MessageToUser);
                    return View("EnterCode", oktaVerifyEnterCodeModel);
                default:
                    return View("Login");
            }            
        }

        public async Task<ActionResult> ChallengePoll()
        {
            var oktaVerifyAuthenticationOptions = (OktaVerifyAuthenticationOptions)Session[nameof(OktaVerifyAuthenticationOptions)];

            var pollResponse = await oktaVerifyAuthenticationOptions.PollOnceAsync();
            if (!pollResponse.ContinuePolling)
            {
                ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromTokenResponseAsync(_idxClient.Configuration, oktaVerifyAuthenticationOptions.TokenInfo);
                _authenticationManager.SignIn(new AuthenticationProperties(), identity);
                pollResponse.Next = "/Home/Index";
            }

            return Json(pollResponse, JsonRequestBehavior.AllowGet);
        }
    }
}
