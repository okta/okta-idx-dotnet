

using System.Web.UI.WebControls;

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

        //public ActionResult SelectAuthenticatorMethod()
        //{
        //    //var oktaVerifyAuthenticationOptions = (OktaVerifyAuthenticationOptions)Session[nameof(OktaVerifyAuthenticationOptions)];
        //    /*            var viewModel = new OktaVerifySelectAuthenticatorMethodModel(oktaVerifyAuthenticationOptions)
        //                {
        //                    AuthenticatorId = ((IAuthenticationResponse)Session["ovAuthnResponse"]).CurrentAuthenticator.Id,
        //                };*/

        //    var oktaAuthenticator = (IAuthenticator)Session["oktaVerifyAuthenticator"];
        //    return View(new OktaVerifySelectAuthenticatorMethodModel(oktaAuthenticator));
        //}

        public ActionResult SelectAuthenticatorMethod(OktaVerifySelectAuthenticatorMethodModel model)
        {
            return View(model);
        }

        public ActionResult Enroll()
        {
            //var oktaVerifyEnrollOptions = (OktaVerifyEnrollOptions)Session[nameof(OktaVerifyEnrollOptions)];
            var oktaVerifyAuthenticator = (IAuthenticator)Session["oktaVerifyAuthenticator"];
            return View("EnrollWithQrCode", new OktaVerifyEnrollPollModel(oktaVerifyAuthenticator.ContextualData.QrCode.Href));
        }

        [HttpGet]
        public ActionResult SelectEnrollmentChannel()
        {
            //var oktaVerifyEnrollOptions = (OktaVerifyEnrollOptions)Session[nameof(OktaVerifyEnrollOptions)];
            var oktaVerifyAuthenticator = (IAuthenticator)Session["oktaVerifyAuthenticator"];
            var selectEnrollmentChannelModel = new OktaVerifySelectEnrollmentChannelModel(oktaVerifyAuthenticator);
            return View(selectEnrollmentChannelModel);
        }

        [HttpPost]
        public async Task<ActionResult> SelectEnrollmentChannel(OktaVerifySelectEnrollmentChannelModel model)
        {
            var oktaVerifyEnrollOptions = (OktaVerifyEnrollOptions)Session[nameof(OktaVerifyEnrollOptions)];
            if (!ModelState.IsValid)
            {
                var selectEnrollmentChannelViewModel = new OktaVerifySelectEnrollmentChannelModel()// oktaVerifyEnrollOptions)
                {
                    SelectedChannel = model.SelectedChannel,
                };
                return View(selectEnrollmentChannelViewModel);
            }

            //_ = await oktaVerifyEnrollOptions.SelectEnrollmentChannelAsync(model.SelectedChannel);
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
            //var oktaVerifyEnrollOptions = (OktaVerifyEnrollOptions)Session[nameof(OktaVerifyEnrollOptions)];
            //_ = oktaVerifyEnrollOptions.SendLinkToEmailAsync(emailModel.Email);
            var idxContext = (IIdxContext)Session["idxContext"];
            var okaVerifyEnrollWithEmailOptions = new EnrollOktaVerifyAuthenticatorOptions
            {
                Channel = "email",
                Email = emailModel.Email,
            };
            _ = await _idxClient.EnrollAuthenticatorAsync(okaVerifyEnrollWithEmailOptions, idxContext);
            var oktaVerifyEnrollModel = new OktaVerifyEnrollPollModel()//oktaVerifyEnrollOptions)
            {
                Message = "An activation link was sent to your email, use it to complete Okta Verify enrollment."
            };

            return View("EnrollPoll", oktaVerifyEnrollModel);
        }

        [HttpPost]
        public async Task<ActionResult> EnrollWithPhoneNumber(OktaVerifyEnrollWithPhoneNumberModel phoneNumberModel)
        {
            //var oktaVerifyEnrollOptions = (OktaVerifyEnrollOptions)Session[nameof(OktaVerifyEnrollOptions)];
            //_ = oktaVerifyEnrollOptions.SendLinkToPhoneNumberAsync($"{phoneNumberModel.CountryCode}{phoneNumberModel.PhoneNumber}");
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
            var pollResponse = await _idxClient.PollEnroll(idxContext);
            //pollResponse.Next = "/Account/Login";
            dynamic pollViewModel = new
            {
                Refresh = pollResponse.Refresh,
                ContinuePolling = pollResponse.ContinuePolling
            };

            if (!pollResponse.ContinuePolling)
            {
                pollViewModel.Next = "/Account/Login";
            };
            
            return Json(pollViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SelectAuthenticatorMethodAsync(OktaVerifySelectAuthenticatorMethodModel model)
        {
            var oktaVerifyAuthenticationOptions = (OktaVerifyAuthenticationOptions)Session[nameof(OktaVerifyAuthenticationOptions)];

            var selectAuthenticatorOptions = new SelectOktaVerifyAuthenticatorOptions
            {
                AuthenticatorMethodType = model.MethodType,
                AuthenticatorId = model.AuthenticatorId,
            };
            
            try
            {

                var authnResponse = await _idxClient.SelectChallengeAuthenticatorAsync(selectAuthenticatorOptions,
                    (IIdxContext)Session["IdxContext"]);

                if (authnResponse.AuthenticationStatus == AuthenticationStatus.AwaitingAuthenticatorVerification)
                {
                    switch (model.MethodType)
                    {
                        // TODO: change it to enum
                        case "totp":
                            return View("EnterCode");
                        case "push":
                            return View("PushSent",
                                new OktaVerifySelectAuthenticatorMethodModel(oktaVerifyAuthenticationOptions));
                    }
                }
            }
            catch (Exception e)
            {
                // Add errors to model
            }

            //ModelState.AddModelError("", new ArgumentException($"Unrecognized Okta Verify authentication method: {methodType}"));
            return View(new OktaVerifySelectAuthenticatorMethodModel(oktaVerifyAuthenticationOptions));
        }

        [HttpPost]
        public async Task<ActionResult> EnterCode()
        {
            return View();
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
                    // TODO: Review this
                    //if (authenticationResponse.IsOktaVerifyCurrentAuthenticator == true)
                    //{
                    //    Session[nameof(OktaVerifyAuthenticationOptions)] = authenticationResponse.OktaVerifyAuthenticationOptions;
                    //    return RedirectToAction("SelectAuthenticatorMethod", "OktaVerify");
                    //}
                    return RedirectToAction("SelectAuthenticator", "Manage");
                case AuthenticationStatus.AwaitingAuthenticatorEnrollment:
                    Session["isChallengeFlow"] = false;
                    Session["authenticators"] = ViewModelHelper.ConvertToAuthenticatorViewModelList(authenticationResponse.Authenticators);
                    return RedirectToAction("SelectAuthenticator", "Manage");
                default:
                    return View("Login");
            }            
        }

        public async Task<ActionResult> ChallengePoll()
        {
            var oktaVerifyAuthenticationOptions = (OktaVerifyAuthenticationOptions)Session[nameof(OktaVerifyAuthenticationOptions)];

            var pollResponse = await _idxClient.PollAuthenticatorPushStatusAsync((IIdxContext)Session["idxContext"]);
            // oktaVerifyAuthenticationOptions.PollOnceAsync();
            dynamic pollViewModel = new
            {
                Refresh = pollResponse.Refresh,
                ContinuePolling = pollResponse.ContinuePolling,
            };

            if (!pollResponse.ContinuePolling)
            {
                ClaimsIdentity identity = await AuthenticationHelper.GetIdentityFromTokenResponseAsync(_idxClient.Configuration, pollResponse.TokenInfo);
                _authenticationManager.SignIn(new AuthenticationProperties(), identity);
                pollViewModel.Next = "/Home/Index";
            }

            return Json(pollViewModel, JsonRequestBehavior.AllowGet);
        }
    }
}
