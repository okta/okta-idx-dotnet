using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk.OktaVerify
{
    public class InvalidOktaVerifyEnrollmentChannelException : Exception
    {
        public InvalidOktaVerifyEnrollmentChannelException(string specified, params string[] validOptions)
        : base($"Invalid enrollment channel specified: '{specified}', valid options are ({string.Join(", ", validOptions)})")
        {
        }
    }
}
