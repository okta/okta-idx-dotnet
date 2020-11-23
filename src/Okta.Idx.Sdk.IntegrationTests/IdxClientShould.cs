using FluentAssertions;
using Okta.Idx.Sdk.Configuration;
using Okta.Sdk.Abstractions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Okta.Idx.Sdk.IntegrationTests
{
    public class IdxClientShould
    {
        [Fact]
        public async Task CallIntrospectEndpoint()
        {
            var stateHandle = "{stateHandlePlaceholder}";
            var client = TestIdxClient.Create(new IdxConfiguration() { Issuer = "https://idx-foo.com/oauth2/default/v1", ClientId = "foo" });

            var res = await ((IdxClient)client).InteractAsync();


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
            response.Remediation.RemediationOptions.FirstOrDefault().Href.Should().Be("https://idx-foo.com.oktapreview.com/idp/idx/identify");
            response.Remediation.RemediationOptions.FirstOrDefault().Method.Should().Be("POST");

            response.Remediation.RemediationOptions.FirstOrDefault().Form.Should().NotBeNullOrEmpty();

            var formItem = response.Remediation.RemediationOptions.FirstOrDefault().Form.FirstOrDefault(x => x.Name == "stateHandle");
            formItem.Should().NotBeNull();


            var resourceData = new IdxRequestPayload();
            resourceData.SetProperty("identifier", "{emailPlaceHolder}");
            resourceData.StateHandle = stateHandle;
            resourceData.SetProperty("rememberMe", false);

            var response2 =  await response.Remediation.RemediationOptions.FirstOrDefault().ProceedAsync(resourceData);

        }
    }
}
