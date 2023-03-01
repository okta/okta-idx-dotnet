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
    ""clientId"": ""testclientid"",
    ""redirectUri"": ""testredirecturi"",
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
    ""state"": ""teststate"",
    ""codeChallenge"": ""testcodechallenge"",
    ""codeChallengeMethod"": ""S256""
}";
            var signInWidgetConfiguratin = JsonConvert.DeserializeObject<SignInWidgetConfiguration>(signInWidgetJson);
            signInWidgetConfiguratin.Should().NotBeNull();
            signInWidgetConfiguratin.InteractionHandle.Should().Be("notreal");
            signInWidgetConfiguratin.Version.Should().Be("5.8.1");
            signInWidgetConfiguratin.BaseUrl.Should().Be("https://test.com");
            signInWidgetConfiguratin.ClientId.Should().Be("testclientid");
            signInWidgetConfiguratin.RedirectUri.Should().Be("testredirecturi");
            signInWidgetConfiguratin.AuthParams.Should().NotBeNull();
            signInWidgetConfiguratin.AuthParams.Issuer.Should().Be("https://test.com");
            signInWidgetConfiguratin.AuthParams.Scopes.Should().NotBeNull();
            List<string> scopes = new List<string>(signInWidgetConfiguratin.AuthParams.Scopes);
            scopes.Contains("openid").Should().BeTrue();
            scopes.Contains("profile").Should().BeTrue();
            scopes.Contains("email").Should().BeTrue();
            scopes.Contains("offline_access").Should().BeTrue();
            signInWidgetConfiguratin.UseInteractionCodeFlow.Should().BeTrue();
            signInWidgetConfiguratin.State.Should().Be("teststate");
            signInWidgetConfiguratin.CodeChallenge.Should().Be("testcodechallenge");
            signInWidgetConfiguratin.CodeChallengeMethod.Should().Be("S256");
        }
    }
}
