using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class SecurityQuestionAuthenticatorOptions
    {
        public string Answer { get; set; }

        public string QuestionKey { get; set; }

        public string Question { get; set; }
    }
}
