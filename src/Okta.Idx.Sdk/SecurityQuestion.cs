using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class SecurityQuestion : Resource, ISecurityQuestion
    {
        public string QuestionKey => GetStringProperty("questionKey");

        public string Question => GetStringProperty("question");
    }
}
