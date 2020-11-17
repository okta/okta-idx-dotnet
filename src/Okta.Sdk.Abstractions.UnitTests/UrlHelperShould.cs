using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Okta.Sdk.Abstractions.UnitTests
{
    public class UrlHelperShould
    {
        [Theory]
        [InlineData("https://devex-testing.oktapreview.com/oauth2/default", "https://devex-testing.oktapreview.com")]
        [InlineData("https://devex-testing.okta.com/oauth2/default", "https://devex-testing.okta.com")]
        [InlineData("http://devex-testing.okta.com/oauth2/default", "http://devex-testing.okta.com")]
        public void GetOktaDomain(string issuer, string expectedOktaDomain)
        {
            UrlHelper.GetOktaDomain(issuer).Should().Be(expectedOktaDomain);
        }
    }
}
