using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Okta.Idx.Sdk.Configuration;

namespace Okta.Idx.Sdk.UnitTests
{
    public class IdxClientBuilderShould
    {
        [Fact]
        public async Task BuildClient()
        {
            IdxClient client = new IdxClientBuilder()
                .Build();

            client.Should().NotBeNull();
            client.PasswordWarnStateResolver.Should().NotBeNull();
        }

        [Fact]
        public async Task UseConfiguration()
        {
            string testIssuer = "http://fake.com";
            string testClientId = Guid.NewGuid().ToString();
            string testRedirectUri = "http://fake.com/callback";
            IdxClient client = new IdxClientBuilder()
                .UseConfiguration(new IdxConfiguration
                {
                    Issuer = testIssuer,
                    ClientId = testClientId,
                    RedirectUri = testRedirectUri
                })
                .Build();

            client.Should().NotBeNull();
            client.Configuration.Issuer.Should().Be(testIssuer);
            client.Configuration.ClientId.Should().Be(testClientId);
            client.Configuration.RedirectUri.Should().Be(testRedirectUri);
        }

        [Fact]
        public async Task UsePasswordWarnStateResolver()
        {
            IdxClient client = new IdxClientBuilder()
                .UsePasswordWarnStateResolver((idxResponse) => true)                
                .Build();

            client.Should().NotBeNull();
            client.PasswordWarnStateResolver.Should().NotBeNull();
            client.PasswordWarnStateResolver.GetType().Should().Be(typeof(CustomPasswordWarnStateResolver));
        }
    }
}
