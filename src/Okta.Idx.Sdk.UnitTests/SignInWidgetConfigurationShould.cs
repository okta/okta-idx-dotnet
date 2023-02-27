using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Okta.Idx.Sdk.UnitTests
{
    public class SignInWidgetConfigurationShould
    {
        [Fact]
        public void Deserialize()
        {
            var signInWidgetJson = @"{
    ""interactionHandle"": ""notreal"",
    ""version"": ""5.8.1"",
    ""baseUrl"": ""https://test.com"",
    ""clientId"": ""0oa738lmiljOyCtEn1d7"",
    ""redirectUri"": ""https://localhost:51936/interactioncode/callback"",
    ""authParams"": {
        ""issuer"": ""https://test.com"",
        ""scopes"": [
            ""openid"",
            ""profile"",
            ""email"",
            ""offline_access""
        ]
    },
    ""useInteractionCodeFlow"": true,
    ""state"": ""dyR1gauT_RxOP299Bn9q1A"",
    ""codeChallenge"": ""bqKBsjNt4TsV-p9NUcZvKpFN-g5VT_uMHKVy72d6fGI"",
    ""codeChallengeMethod"": ""S256""
}";
            var signInWidgetConfiguratin = JsonConvert.DeserializeObject<SignInWidgetConfiguration>(signInWidgetJson);
            signInWidgetConfiguratin.Should().NotBeNull();
        }
    }
}
