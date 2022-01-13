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

        protected OktaVerifyEnrollOptions OktaVerifyEnrollOptions { get; set; }

        [Required]
        [Display(Name = "enrollment channel")]
        public string SelectedChannel
        {
            get; 
            set;
        }

        public IList<OktaVerifySelectEnrollmentChannelOptionModel> EnrollmentChannelOptions 
        {
            get => OktaVerifyEnrollOptions?
                .GetChannelOptions()
                .Where(option => option.Value != OktaVerifyEnrollmentChannel.QrCode)
                .Select(option => new OktaVerifySelectEnrollmentChannelOptionModel(option))
                .ToList();
        } 
    }
}
