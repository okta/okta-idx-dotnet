using FluentAssertions;
using Okta.Idx.Sdk.Configuration;
using Okta.Sdk.Abstractions;
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
        public async Task CallSuccessfullyIntrospectWithRetrievedInteractEndpoint()
        {
            var client = TestIdxClient.Create();

            var interactResponse = await ((IdxClient)client).InteractAsync();


            var response = await client.IntrospectAsync(interactResponse.InteractionHandle);

            response.StateHandle.Should().NotBeNullOrEmpty();
            response.Version.Should().NotBeNullOrEmpty();
            response.ExpiresAt.Should().NotBeNull();
            response.Intent.Should().Be("LOGIN");

            response.Remediation.Should().NotBeNull();
            response.Remediation.GetRaw().Should().NotBeNullOrEmpty();
            response.Remediation.Type.Should().Be("array");

            response.Remediation.RemediationOptions.Should().NotBeNullOrEmpty();
            response.Remediation.RemediationOptions.FirstOrDefault().Rel.Should().Contain("create-form");
            response.Remediation.RemediationOptions.FirstOrDefault().Name.Should().Be("identify");
            response.Remediation.RemediationOptions.FirstOrDefault().Href.Should().NotBeNullOrEmpty();
            response.Remediation.RemediationOptions.FirstOrDefault().Method.Should().Be("POST");

            response.Remediation.RemediationOptions.FirstOrDefault().Form.Should().NotBeNullOrEmpty();

            var formItem = response.Remediation.RemediationOptions.FirstOrDefault().Form.FirstOrDefault(x => x.Name == "stateHandle");
            formItem.Should().NotBeNull();
        }

        [Fact]
        public async Task ThrowWhenRetrievingTokensAfterCancel()
        {
            var client = TestIdxClient.Create();

            var interactResponse = await client.InteractAsync();

            var introspectResponse = await client.IntrospectAsync(interactResponse.InteractionHandle);

            var identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = introspectResponse.StateHandle;
            identifyRequest.SetProperty("identifier", Environment.GetEnvironmentVariable("OKTA_IDX_USERNAME"));

            // Send username
            var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "identify")
                                                        .ProceedAsync(identifyRequest);

            identifyRequest = new IdxRequestPayload()
            {
                StateHandle = identifyResponse.StateHandle,
            };

            identifyRequest.SetProperty("credentials", new
            {
                passcode = Environment.GetEnvironmentVariable("OKTA_IDX_PASSWORD"),
            });


            var challengeResponse = await identifyResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                                        .ProceedAsync(identifyRequest);

            var cancelResponse =  await identifyResponse.CancelAsync();

            await Assert.ThrowsAsync<OktaApiException>(() => challengeResponse.SuccessWithInteractionCode.ExchangeCodeAsync());
        }


        [Fact]
        public async Task ExchangeTokens()
        {
            var client = TestIdxClient.Create();

            var interactResponse = await client.InteractAsync();

            var introspectResponse = await client.IntrospectAsync(interactResponse.InteractionHandle);

            var identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = introspectResponse.StateHandle;
            identifyRequest.SetProperty("identifier", Environment.GetEnvironmentVariable("OKTA_IDX_USERNAME"));
            
            // Send username
            var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "identify")
                                                        .ProceedAsync(identifyRequest);

            // Send password

            identifyRequest = new IdxRequestPayload()
            {
                StateHandle = identifyResponse.StateHandle,
            };

            identifyRequest.SetProperty("credentials", new
            {
                passcode = Environment.GetEnvironmentVariable("OKTA_IDX_PASSWORD"),
            });


            var challengeResponse = await identifyResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                                        .ProceedAsync(identifyRequest);



            var token = await challengeResponse.SuccessWithInteractionCode.ExchangeCodeAsync();

            token.AccessToken.Should().NotBeNullOrEmpty();
        }

        // This test requires to have email configured as second authenticator.
        [Fact]
        public async Task CancelFlowBeforeSendingEmailCode()
        {
            // Create an app with email required and set secrets via env var
            var client = TestIdxClient.Create();

            var interactResponse = await client.InteractAsync();

            var introspectResponse = await client.IntrospectAsync(interactResponse.InteractionHandle);

            var identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = introspectResponse.StateHandle;
            identifyRequest.SetProperty("identifier", Environment.GetEnvironmentVariable("OKTA_IDX_USERNAME"));

            // Send username
            var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "identify")
                                                        .ProceedAsync(identifyRequest);

            // Send password

            identifyRequest = new IdxRequestPayload()
            {
                StateHandle = identifyResponse.StateHandle,
            };

            identifyRequest.SetProperty("credentials", new
            {
                passcode = Environment.GetEnvironmentVariable("OKTA_IDX_PASSWORD"),
            });


            var challengeResponse = await identifyResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                                        .ProceedAsync(identifyRequest);




            var selectAuthenticatorRemediationOption = challengeResponse.Remediation.RemediationOptions
                                                                .FirstOrDefault(x => x.Name == "select-authenticator-authenticate");

            var emailId = selectAuthenticatorRemediationOption.GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "authenticator")
                                                        .Options
                                                        .FirstOrDefault(x => x.Label == "Email")
                                                        .GetProperty<FormValue>("value")
                                                        .Form
                                                        .GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "id")
                                                        .GetProperty<string>("value");
                                                              
                                                            


            var selectEmailProceedRequest = new IdxRequestPayload()
            {
                StateHandle = challengeResponse.StateHandle,
            };

            selectEmailProceedRequest.SetProperty("authenticator", new
            {
                id = $"{emailId}",
                methodType = "email",
            });



            var selectEmailResponse = await selectAuthenticatorRemediationOption.ProceedAsync(selectEmailProceedRequest);

            // Before answering email challenge (code), cancel the transaction
            await selectEmailResponse.CancelAsync();

            // Get a new interaction code
            interactResponse = await client.InteractAsync();

            interactResponse.InteractionHandle.Should().NotBeNullOrEmpty();
        }

    }
}
