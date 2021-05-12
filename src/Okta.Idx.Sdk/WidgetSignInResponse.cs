using Newtonsoft.Json;
using Okta.Idx.Sdk.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class WidgetSignInResponse
    {
        public IIdxContext IdxContext { get; set; }

        public SignInWidgetConfiguration SignInWidgetConfiguration { get; set; }
    }
}
