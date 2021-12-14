using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class PollResponse
    {
        public int? Refresh { get; set; }

        public bool ContinuePolling { get; set; }

        public string Next { get; set; }
    }
}
