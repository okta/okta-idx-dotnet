using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class EnrollPollOptions
    {
        /// <summary>
        /// Gets or sets the enroll-poll remediation option.
        /// </summary>
        protected internal IRemediationOption EnrollPollRemediationOption { get; set; }

        /// <summary>
        /// Gets or sets the select-enrollment-channel remediation option.
        /// </summary>
        protected internal IRemediationOption SelectEnrollmentChannelRemediationOption { get; set; }

        public string StateHandle { get; set; }

        public int? Refresh { get => EnrollPollRemediationOption.Refresh; }

        public AuthenticationResponse SelectEnrollmentChannel(string enrollmentChannelName)
        {
            ValidateEnrollmentChannel(enrollmentChannelName);

            throw new NotImplementedException();
        }

        protected void ValidateEnrollmentChannel(string enrollmentChannelName)
        {
            IList<RemediationOptionParameter> channelOptions = GetChannelOptions();
            if (!channelOptions.Any(rop => rop.Value == enrollmentChannelName))
            {
                throw new InvalidEnrollmentChannelException(enrollmentChannelName, channelOptions.Select(co => co.Value).ToArray());
            }
        }

        protected IList<RemediationOptionParameter> GetChannelOptions()
        {
            return SelectEnrollmentChannelRemediationOption?
                .GetArrayProperty<IIdxResponse>("value")[0]
                .GetProperty<IIdxResponse>("value")
                .GetProperty<IIdxResponse>("form")
                .GetArrayProperty<IIdxResponse>("value")[1]
                .GetArrayProperty<RemediationOptionParameter>("options");
        }
    }
}
