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
            this.EnrollPollOptions = enrollPollOptions;
        }

        protected OktaVerifyEnrollOptions EnrollPollOptions { get; set; }

        [Required]
        [Display(Name = "enrollment channel")]
        public string SelectedChannel
        {
            get; 
            set;
        }

        public IList<OktaVerifySelectEnrollmentChannelOptionModel> EnrollmentChannelOptions 
        {
            get => EnrollPollOptions?
                .GetChannelOptions()
                .Where(rop => rop.Value != OktaVerifyEnrollmentChannel.QrCode)
                .Select(optionParameter => new OktaVerifySelectEnrollmentChannelOptionModel(optionParameter))
                .ToList();
        } 
    }
}
