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
            var signInWidgetConfiguration = JsonConvert.DeserializeObject<SignInWidgetConfiguration>(signInWidgetJson);
            signInWidgetConfiguration.Should().NotBeNull();
            signInWidgetConfiguration.InteractionHandle.Should().Be("notreal");
            signInWidgetConfiguration.Version.Should().Be("5.8.1");
            signInWidgetConfiguration.BaseUrl.Should().Be("https://test.com");
            signInWidgetConfiguration.ClientId.Should().Be("testclientid");
            signInWidgetConfiguration.RedirectUri.Should().Be("testredirecturi");
            signInWidgetConfiguration.AuthParams.Should().NotBeNull();
            signInWidgetConfiguration.AuthParams.Issuer.Should().Be("https://test.com");
            signInWidgetConfiguration.AuthParams.Scopes.Should().NotBeNull();
            List<string> scopes = new List<string>(signInWidgetConfiguration.AuthParams.Scopes);
            scopes.Contains("openid").Should().BeTrue();
            scopes.Contains("profile").Should().BeTrue();
            scopes.Contains("email").Should().BeTrue();
            scopes.Contains("offline_access").Should().BeTrue();
            signInWidgetConfiguration.UseInteractionCodeFlow.Should().BeTrue();
            signInWidgetConfiguration.State.Should().Be("teststate");
            signInWidgetConfiguration.CodeChallenge.Should().Be("testcodechallenge");
            signInWidgetConfiguration.CodeChallengeMethod.Should().Be("S256");
        }
    }
}
