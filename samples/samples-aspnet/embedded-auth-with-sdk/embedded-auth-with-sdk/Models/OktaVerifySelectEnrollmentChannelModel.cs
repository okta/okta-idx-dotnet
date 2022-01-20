using Okta.Idx.Sdk;
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

        public OktaVerifySelectEnrollmentChannelModel(IAuthenticator authenticator)
        {
            this.AuthenticatorId = authenticator.Id;
        }

        [Required]
        [Display(Name = "enrollment channel")]
        public string SelectedChannel
        {
            get; 
            set;
        }

        [Required]
        public string AuthenticatorId { get; set; }
    }
}
