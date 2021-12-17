using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk.UnitTests
{
    public class TestEnrollPollOptions : EnrollPollOptions
    {
        EnrollPollOptions wrapped;
        public TestEnrollPollOptions(EnrollPollOptions wrapped)
        {
            this.wrapped = wrapped;
            this.SelectEnrollmentChannelRemediationOption = wrapped.SelectEnrollmentChannelRemediationOption;
        }

        public new IList<RemediationOptionParameter> GetChannelOptions()
        {
            return base.GetChannelOptions();
        }
    }
}
