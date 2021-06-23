using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Okta.Idx.Sdk.UnitTests
{
    using Okta.Idx.Sdk.Configuration;
    using Okta.Idx.Sdk.Helpers;

    public class IdxConfigurationValidatorShould
    {
        [Fact]
        public void ThrowUriFormatExceptionOnInvalidRedirectUri()
        {
            IdxConfiguration idxConfiguration = new IdxConfiguration
            {
                Issuer = "test issuer",
                ClientId = "test client id",
                RedirectUri = "https://subdomain.domain.tld:8080:path/to/endpoint"
            };

            Assert.Throws<UriFormatException>(() => IdxConfigurationValidator.Validate(idxConfiguration));
        }

        [Fact]
        public void NotThrowUriFormatExceptionOnInvalidRedirectUri()
        {
            IdxConfiguration idxConfiguration = new IdxConfiguration
            {
                Issuer = "test issuer",
                ClientId = "test client id",
                RedirectUri = "https://subdomain.domain.tld:8080/path/to/endpoint"
            };

            IdxConfigurationValidator.Validate(idxConfiguration);
        }
    }
}
