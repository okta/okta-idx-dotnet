using Okta.Idx.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace embedded_auth_with_sdk.Models
{
    public class OktaVerifyPollConfig
    {
        public OktaVerifyPollConfig(AuthenticationResponse enrollResponse, string pollEndpoint = "/Manage/Poll")
        {
            this.AuthenticationResponse = enrollResponse;
            this.RefreshInterval = enrollResponse.EnrollPollOptions?.Refresh;
            this.StateHandle = enrollResponse.EnrollPollOptions?.StateHandle;
            this.ViewModel = new EnrollOktaVerifyAuthenticatorViewModel
            {
                QrCodeHref = enrollResponse.CurrentAuthenticator.QrCode.Href,
                SelectedChannel = enrollResponse.CurrentAuthenticator.SelectedChannel,
                RefreshInterval = this.RefreshInterval,
                PollEndpoint = pollEndpoint,
            };            
        }

        protected AuthenticationResponse AuthenticationResponse { get; }

        public EnrollPollOptions EnrollPollOptions => AuthenticationResponse.EnrollPollOptions;
        public int? RefreshInterval { get; set; }
        public string StateHandle { get; set; }

        public EnrollOktaVerifyAuthenticatorViewModel ViewModel { get; set; }
    }
}