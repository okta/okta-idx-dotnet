using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class InvalidEnrollmentChannelException : Exception
    {
        public InvalidEnrollmentChannelException(string specified, params string[] validOptions)
        : base($"Invalid enrollment channel specified: '{specified}', valid options are ({string.Join(", ", validOptions)})")
        {
        }
    }
}
