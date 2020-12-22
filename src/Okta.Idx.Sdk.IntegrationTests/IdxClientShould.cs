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
        // Test plan: #4
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

        // This test requires a policy with security question as a required second factor.
        // Test plan: #2
        [Fact]
        public async Task EnrollSecurityQuestion()
        {
            // Create an app with email required and set secrets via env var
            var client = TestIdxClient.Create();

            var interactResponse = await client.InteractAsync();

            var introspectResponse = await client.IntrospectAsync(interactResponse.InteractionHandle);

            // TODO: Create user and assign user to application.

            var identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = introspectResponse.StateHandle;
            identifyRequest.SetProperty("identifier", "test-enroll-idx@test.com");
            identifyRequest.SetProperty("credentials", new
            {
                passcode = Environment.GetEnvironmentVariable("OKTA_IDX_PASSWORD"),
            });

            // Send username and password
            var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "identify")
                                                        .ProceedAsync(identifyRequest);


            var selectAuthenticatorRemediationOption = identifyResponse.Remediation.RemediationOptions
                                                                .FirstOrDefault(x => x.Name == "select-authenticator-enroll");


            var securityQuestionId = selectAuthenticatorRemediationOption.GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "authenticator")
                                                        .Options
                                                        .FirstOrDefault(x => x.Label == "Security Question")
                                                        .GetProperty<FormValue>("value")
                                                        .Form
                                                        .GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "id")
                                                        .GetProperty<string>("value");


            // Send password
            var selectAuthenticatorEnrollRequest = new IdxRequestPayload()
            {
                StateHandle = identifyResponse.StateHandle,
            };

            selectAuthenticatorEnrollRequest.SetProperty("authenticator", new
            {
                id = securityQuestionId,
            });


            var selectAuthenticatorEnrollResponse = await identifyResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "select-authenticator-enroll")
                                            .ProceedAsync(selectAuthenticatorEnrollRequest);




            var enrollAuthenticatorRemediationOption = selectAuthenticatorEnrollResponse.Remediation.RemediationOptions
                                                                .FirstOrDefault(x => x.Name == "enroll-authenticator");

            var seccurityQuestionValue = enrollAuthenticatorRemediationOption.GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "credentials")
                                                        .Options
                                                        .FirstOrDefault(x => x.Label == "Choose a security question")
                                                        .GetProperty<FormValue>("value")
                                                        .Form
                                                        .GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "questionKey")
                                                        .Options
                                                        .FirstOrDefault(x => x.GetProperty<string>("value") == "disliked_food")
                                                        .GetProperty<string>("value");


            var enrollRequest = new IdxRequestPayload();
            enrollRequest.StateHandle = selectAuthenticatorEnrollResponse.StateHandle;
            enrollRequest.SetProperty("credentials", new {
                answer = "pasta",
                questionKey = seccurityQuestionValue,
            });

            var enrollResponse = await enrollAuthenticatorRemediationOption.ProceedAsync(enrollRequest);


            var skipRequest = new IdxRequestPayload();
            skipRequest.StateHandle = enrollResponse.StateHandle;

            // skip optional factor
            var skipResponse = await enrollResponse.Remediation.RemediationOptions
                                        .FirstOrDefault(x => x.Name == "skip")
                                        .ProceedAsync(skipRequest);


            var tokenResponse = await skipResponse.SuccessWithInteractionCode.ExchangeCodeAsync();

            tokenResponse.AccessToken.Should().NotBeNullOrEmpty();
        }

        // This test requires a policy with email as a required second authenticator.
        // Test plan: #2
        [Fact]
        public async Task EnrollEmail()
        {
            // Create an app with email required and set secrets via env var
            var client = TestIdxClient.Create();

            var interactResponse = await client.InteractAsync();

            var introspectResponse = await client.IntrospectAsync(interactResponse.InteractionHandle);

            // TODO: Create user and assign user to application.

            var identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = introspectResponse.StateHandle;
            identifyRequest.SetProperty("identifier", "test-login-email@test.com");
            
            // Send username
            var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "identify")
                                                        .ProceedAsync(identifyRequest);

            var selectAuthenticatorRemediationOption1 = identifyResponse.Remediation.RemediationOptions
                                                                .FirstOrDefault(x => x.Name == "select-authenticator-authenticate");

            // Select password first
            var passwordId = selectAuthenticatorRemediationOption1.GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "authenticator")
                                                        .Options
                                                        .FirstOrDefault(x => x.Label == "Password")
                                                        .GetProperty<FormValue>("value")
                                                        .Form
                                                        .GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "id")
                                                        .GetProperty<string>("value");

            var selectPasswordRequest = new IdxRequestPayload();
            selectPasswordRequest.StateHandle = identifyResponse.StateHandle;
            selectPasswordRequest.SetProperty("authenticator", new
            {
                id = passwordId
            });


            var selectPasswordAuthenticatorResponse = await selectAuthenticatorRemediationOption1.ProceedAsync(selectPasswordRequest);

            var challengePasswordRequest = new IdxRequestPayload();
            sendPasswordRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
            sendPasswordRequest.SetProperty("credentials", new
            {
                passcode = Environment.GetEnvironmentVariable("OKTA_IDX_PASSWORD"),
            });

            var challengePasswordRemediationOption = await selectPasswordAuthenticatorResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                                        .ProceedAsync(sendPasswordRequest);

            var selectAuthenticatorRemediationOption2 = identifyResponse.Remediation.RemediationOptions
                                                                .FirstOrDefault(x => x.Name == "select-authenticator-authenticate");

            var emailId = selectAuthenticatorRemediationOption2.GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "authenticator")
                                                        .Options
                                                        .FirstOrDefault(x => x.Label == "Email")
                                                        .GetProperty<FormValue>("value")
                                                        .Form
                                                        .GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "id")
                                                        .GetProperty<string>("value");


            // Send password
            var selectEmailAuthenticatorRequest = new IdxRequestPayload();
            selectEmailAuthenticatorRequest.StateHandle = identifyResponse.StateHandle;
            selectEmailAuthenticatorRequest.SetProperty("authenticator", new
            {
                id = emailId,
            });


            var selectEmailAuthenticatorResponse = await selectAuthenticatorRemediationOption2.ProceedAsync(selectEmailAuthenticatorRequest);

            var challengeEmailRequest = new IdxRequestPayload();
            challengeEmailRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
            challengeEmailRequest.SetProperty("credentials", new
            {
                passcode = "00000",
            });


            var challengeEmailResponse = await selectEmailAuthenticatorResponse.Remediation.RemediationOptions
                                                                .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                                                .ProceedAsync(challengeEmailRequest);


            var tokenResponse = await challengeEmailResponse.SuccessWithInteractionCode.ExchangeCodeAsync();

            tokenResponse.AccessToken.Should().NotBeNullOrEmpty();
        }

        // Test plan: #5
        [Fact]
        public async Task LoginSuccessfullyWithProgressiveProfilingFeature()
        {
            var client = TestIdxClient.Create();

            var interactResponse = await client.InteractAsync();

            var introspectResponse = await client.IntrospectAsync(interactResponse.InteractionHandle);

            var identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = introspectResponse.StateHandle;
            identifyRequest.SetProperty("identifier", "devex@okta.com");

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



            var enrollProfileRequest = new IdxRequestPayload()
            {
                StateHandle = challengeResponse.StateHandle,
            };

            // TODO: Enable profiling attributes
            enrollProfileRequest.SetProperty("userProfile", new
            {
                test = "foo",
            });

            var enrollProfileResponse = await challengeResponse.Remediation.RemediationOptions
                                                       .FirstOrDefault(x => x.Name == "enroll-profile")
                                                       .ProceedAsync(enrollProfileRequest);

            enrollProfileResponse.IsLoginSuccess.Should().BeTrue();
        }
    }
}
