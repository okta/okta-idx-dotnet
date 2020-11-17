using FluentAssertions;
using Okta.IdentityEngine.Sdk.Configuration;
using Okta.Sdk.Abstractions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Okta.IdentityEngine.Sdk.IntegrationTests
{
    public class OktaIdentityEngineClientShould
    {
        [Fact]
        public async Task CallIntrospectEndpoint()
        {
            var stateHandle = "{stateHandlePlaceholder}";
            var client = TestIdentityEngineClient.Create(new OktaIdentityEngineConfiguration() { Issuer = "https://devex-idx-testing.oktapreview.com/oauth2/default" });

            var response = await client.StartAsync(stateHandle);

            response.StateHandle.Should().Be(stateHandle);
            response.Version.Should().NotBeNullOrEmpty();
            response.ExpiresAt.Should().NotBeNull();
            response.Intent.Should().Be("LOGIN");

            response.Remediation.Should().NotBeNull();
            response.Remediation.GetRaw().Should().NotBeNullOrEmpty();
            response.Remediation.Type.Should().Be("array");

            response.Remediation.RemediationOptions.Should().NotBeNullOrEmpty();
            response.Remediation.RemediationOptions.FirstOrDefault().Rel.Should().Contain("create-form");
            response.Remediation.RemediationOptions.FirstOrDefault().Name.Should().Be("identify");
            response.Remediation.RemediationOptions.FirstOrDefault().Href.Should().Be("https://devex-idx-testing.oktapreview.com/idp/idx/identify");
            response.Remediation.RemediationOptions.FirstOrDefault().Method.Should().Be("POST");

            response.Remediation.RemediationOptions.FirstOrDefault().Form.Should().NotBeNullOrEmpty();

            var formItem = response.Remediation.RemediationOptions.FirstOrDefault().Form.FirstOrDefault(x => x.Name == "stateHandle");
            formItem.Should().NotBeNull();


            var resourceData = new IdentityEngineRequest();
            resourceData.SetProperty("identifier", "{emailPlaceHolder}");
            resourceData.StateHandle = stateHandle;
            resourceData.SetProperty("rememberMe", false);

            var response2 =  await response.Remediation.RemediationOptions.FirstOrDefault().ProceedAsync(resourceData);

        }
    }
}
