using Okta.Sdk.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk
{
    public class AuthenticatorEnrollment : Resource, IAuthenticatorEnrollment
    {
        public string Type => GetStringProperty("type");

        public IAuthenticatorEnrollmentValue Value => GetResourceProperty<AuthenticatorEnrollmentValue>("value");

    }

    
}
