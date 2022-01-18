using Okta.Idx.Sdk;
using Okta.Idx.Sdk.OktaVerify;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace embedded_auth_with_sdk.Models
{
    public class OktaVerifySelectEnrollmentChannelModel
    {
        public OktaVerifySelectEnrollmentChannelModel()
        { 
        }

        public OktaVerifySelectEnrollmentChannelModel(OktaVerifyEnrollOptions enrollPollOptions)
        {
            this.OktaVerifyEnrollOptions = enrollPollOptions;
        }

        public OktaVerifySelectEnrollmentChannelModel(IAuthenticator authenticator)
        {
            this.AuthenticatorId = authenticator.Id;
        }

        protected OktaVerifyEnrollOptions OktaVerifyEnrollOptions { get; set; }

        [Required]
        [Display(Name = "enrollment channel")]
        public string SelectedChannel
        {
            get; 
            set;
        }

        [Required]
        public string AuthenticatorId { get; set; }

        public IList<OktaVerifySelectEnrollmentChannelOptionModel> EnrollmentChannelOptions 
        {
            get => new List<string>(new string[] {"email", "sms"})
                .Select(option => new OktaVerifySelectEnrollmentChannelOptionModel(option))
                .ToList();
        } 
    }
}
