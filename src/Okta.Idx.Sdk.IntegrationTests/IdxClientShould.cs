using FluentAssertions;
using Okta.Sdk.Abstractions;
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

            var idxContext = await ((IdxClient)client).InteractAsync();

            var response = await client.IntrospectAsync(idxContext.InteractionHandle);

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

            var idxContext = await client.InteractAsync();

            var introspectResponse = await client.IntrospectAsync(idxContext.InteractionHandle);

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

            await Assert.ThrowsAsync<OktaApiException>(() => challengeResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext));
        }


        [Fact]
        public async Task ExchangeTokens()
        {
            var client = TestIdxClient.Create();

            var idxContext = await client.InteractAsync();

            var introspectResponse = await client.IntrospectAsync(idxContext.InteractionHandle);

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



            var token = await challengeResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);

            token.AccessToken.Should().NotBeNullOrEmpty();
        }

        // This test requires to have email configured as second authenticator.
        // Test plan: #4
        [Fact]
        public async Task CancelFlowBeforeSendingEmailCode()
        {
            // Create an app with email required and set secrets via env var
            var client = TestIdxClient.Create();

            var idxContext = await client.InteractAsync();

            var introspectResponse = await client.IntrospectAsync(idxContext.InteractionHandle);

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
            idxContext = await client.InteractAsync();

            idxContext.InteractionHandle.Should().NotBeNullOrEmpty();
            idxContext.CodeChallenge.Should().NotBeNullOrEmpty();
            idxContext.CodeVerifier.Should().NotBeNullOrEmpty();
            idxContext.CodeChallengeMethod.Should().NotBeNullOrEmpty();
        }

        // This test requires a policy with security question as a required second factor.
        // Test plan: #2
        [Fact]
        public async Task EnrollSecurityQuestion()
        {
            // Create an app with email required and set secrets via env var
            var client = TestIdxClient.Create();

            var idxContext = await client.InteractAsync();

            var introspectResponse = await client.IntrospectAsync(idxContext.InteractionHandle);

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


            var tokenResponse = await skipResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);

            tokenResponse.AccessToken.Should().NotBeNullOrEmpty();
        }

        // This test requires a policy with email as a required second authenticator.
        // Test plan: #2
        [Fact]
        public async Task EnrollEmail()
        {
            // Create an app with email required and set secrets via env var
            var client = TestIdxClient.Create();

            var idxContext = await client.InteractAsync();

            var introspectResponse = await client.IntrospectAsync(idxContext.InteractionHandle);

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
            challengePasswordRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
            challengePasswordRequest.SetProperty("credentials", new
            {
                passcode = Environment.GetEnvironmentVariable("OKTA_IDX_PASSWORD"),
            });

            var challengePasswordRemediationOption = await selectPasswordAuthenticatorResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                                        .ProceedAsync(challengePasswordRequest);

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


            var tokenResponse = await challengeEmailResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);

            tokenResponse.AccessToken.Should().NotBeNullOrEmpty();
        }
        // This test requires a policy with phone, security question and email required as additional authenticators.
        // Test plan: #3
        [Fact]
        public async Task EnrollEmailPhoneSecurityQuestion()
        {
            // Create an app with email required and set secrets via env var
            var client = TestIdxClient.Create();

            var idxContext = await client.InteractAsync();

            var introspectResponse = await client.IntrospectAsync(idxContext.InteractionHandle);

            // TODO: Create user and assign user to application.

            var identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = introspectResponse.StateHandle;
            identifyRequest.SetProperty("identifier", "test-mfa@test.com");

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
            challengePasswordRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
            challengePasswordRequest.SetProperty("credentials", new
            {
                passcode = Environment.GetEnvironmentVariable("OKTA_IDX_PASSWORD"),
            });

            var challengePasswordRemediationOption = await selectPasswordAuthenticatorResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                                        .ProceedAsync(challengePasswordRequest);


            // EMAIL


            var selectEmailAuthenticatorRemediationOption = challengePasswordRemediationOption.Remediation.RemediationOptions
                                                                .FirstOrDefault(x => x.Name == "select-authenticator-authenticate");

            var emailId = selectEmailAuthenticatorRemediationOption.GetArrayProperty<FormValue>("value")
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


            var selectEmailAuthenticatorResponse = await selectEmailAuthenticatorRemediationOption.ProceedAsync(selectEmailAuthenticatorRequest);
            var challengeEmailRequest = new IdxRequestPayload();
            challengeEmailRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
            challengeEmailRequest.SetProperty("credentials", new
            {
                passcode = "xxxxx",
            });


            var challengeEmailResponse = await selectEmailAuthenticatorResponse.Remediation.RemediationOptions
                                                                .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                                                .ProceedAsync(challengeEmailRequest);
            // PHONE


            var selectPhoneAuthenticatorRemediationOption = challengeEmailResponse.Remediation.RemediationOptions
                                                                .FirstOrDefault(x => x.Name == "select-authenticator-enroll");

            var phoneId = selectPhoneAuthenticatorRemediationOption.GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "authenticator")
                                                        .Options
                                                        .FirstOrDefault(x => x.Label == "Phone")
                                                        .GetProperty<FormValue>("value")
                                                        .Form
                                                        .GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "id")
                                                        .GetProperty<string>("value");

            var selectPhoneAuthenticatorRequest = new IdxRequestPayload();
            selectPhoneAuthenticatorRequest.StateHandle = identifyResponse.StateHandle;
            selectPhoneAuthenticatorRequest.SetProperty("authenticator", new
            {
                id = phoneId,
                methodType = "sms",
                phoneNumber = "+00000000000",
            });


            var selectPhoneAuthenticatorResponse = await selectPhoneAuthenticatorRemediationOption.ProceedAsync(selectPhoneAuthenticatorRequest);
            var challengePhoneRequest = new IdxRequestPayload();
            challengePhoneRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
            challengePhoneRequest.SetProperty("credentials", new
            {
                passcode = "xxxxxx",
            });


            var challengePhoneResponse = await selectPhoneAuthenticatorResponse.Remediation.RemediationOptions
                                                                .FirstOrDefault(x => x.Name == "enroll-authenticator")
                                                                .ProceedAsync(challengePhoneRequest);




            // SECURITY QUESTION
            var selectSQAuthenticatorRemediationOption = challengePhoneResponse.Remediation.RemediationOptions
                                                               .FirstOrDefault(x => x.Name == "select-authenticator-enroll");


            var securityQuestionId = selectSQAuthenticatorRemediationOption.GetArrayProperty<FormValue>("value")
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


            var selectAuthenticatorEnrollResponse = await selectSQAuthenticatorRemediationOption.ProceedAsync(selectAuthenticatorEnrollRequest);




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
            enrollRequest.SetProperty("credentials", new
            {
                answer = "chicken",
                questionKey = seccurityQuestionValue,
            });

            var enrollResponse = await enrollAuthenticatorRemediationOption.ProceedAsync(enrollRequest);


            var skipRequest = new IdxRequestPayload();
            skipRequest.StateHandle = enrollResponse.StateHandle;

            // skip optional factor
            var skipResponse = await enrollResponse.Remediation.RemediationOptions
                                        .FirstOrDefault(x => x.Name == "skip")
                                        .ProceedAsync(skipRequest);


            var tokenResponse = await skipResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);

        }

        // This test requires a policy with phone as a required second authenticator.
        // Test plan: #2
        [Fact]
        public async Task ChallengePhone()
        {
            // Create an app with email required and set secrets via env var
            var client = TestIdxClient.Create();

            var idxContext = await client.InteractAsync();

            var introspectResponse = await client.IntrospectAsync(idxContext.InteractionHandle);

            // TODO: Create user and assign user to application.

            var identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = introspectResponse.StateHandle;
            identifyRequest.SetProperty("identifier", "test-login-phone@test.com");

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
            challengePasswordRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
            challengePasswordRequest.SetProperty("credentials", new
            {
                passcode = Environment.GetEnvironmentVariable("OKTA_IDX_PASSWORD"),
            });

            var challengePasswordRemediationOption = await selectPasswordAuthenticatorResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                                        .ProceedAsync(challengePasswordRequest);

            var selectAuthenticatorRemediationOption2 = identifyResponse.Remediation.RemediationOptions
                                                                .FirstOrDefault(x => x.Name == "select-authenticator-authenticate");

            var phoneId = selectAuthenticatorRemediationOption2.GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "authenticator")
                                                        .Options
                                                        .FirstOrDefault(x => x.Label == "Phone")
                                                        .GetProperty<FormValue>("value")
                                                        .Form
                                                        .GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "id")
                                                        .GetProperty<string>("value");

            var enrollmentId = selectAuthenticatorRemediationOption2.GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "authenticator")
                                                        .Options
                                                        .FirstOrDefault(x => x.Label == "Phone")
                                                        .GetProperty<FormValue>("value")
                                                        .Form
                                                        .GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "enrollmentId")
                                                        .GetProperty<string>("value");

            var selectPhoneAuthenticatorRequest = new IdxRequestPayload();
            selectPhoneAuthenticatorRequest.StateHandle = identifyResponse.StateHandle;
            selectPhoneAuthenticatorRequest.SetProperty("authenticator", new
            {
                enrollmentId = enrollmentId,
                id = phoneId,
                methodType = "sms",
            });


            var selectPhoneAuthenticatorResponse = await selectAuthenticatorRemediationOption2.ProceedAsync(selectPhoneAuthenticatorRequest);
            
            var challengePhoneRequest = new IdxRequestPayload();
            challengePhoneRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
            challengePhoneRequest.SetProperty("credentials", new
            {
                passcode = "000000",
            });


            var challengePhoneResponse = await selectPhoneAuthenticatorResponse.Remediation.RemediationOptions
                                                                .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                                                .ProceedAsync(challengePhoneRequest);


            var tokenResponse = await challengePhoneResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);

            tokenResponse.AccessToken.Should().NotBeNullOrEmpty();
        }

        // Test plan: #5
        [Fact]
        public async Task ChallengeFingerprint()
        {
            var client = TestIdxClient.Create();

            var idxContext = await client.InteractAsync();

            var introspectResponse = await client.IntrospectAsync(idxContext.InteractionHandle);

            var identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = introspectResponse.StateHandle;
            identifyRequest.SetProperty("identifier", "test-login-fingerprint@test.com");

            // Send username
            var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "identify")
                                                        .ProceedAsync(identifyRequest);


            var selectAuthenticatorRemediationOption1 = identifyResponse.Remediation.RemediationOptions
                                                                .FirstOrDefault(x => x.Name == "select-authenticator-authenticate");

            // Select password
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
            challengePasswordRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
            challengePasswordRequest.SetProperty("credentials", new
            {
                passcode = Environment.GetEnvironmentVariable("OKTA_IDX_PASSWORD"),
            });

            var challengePasswordRemediationOption = await selectPasswordAuthenticatorResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                                        .ProceedAsync(challengePasswordRequest);

            var selectAuthenticatorRemediationOption = challengePasswordRemediationOption.Remediation.RemediationOptions
                                                                .FirstOrDefault(x => x.Name == "select-authenticator-authenticate");

            var fingerprintId = selectAuthenticatorRemediationOption.GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "authenticator")
                                                        .Options
                                                        .FirstOrDefault(x => x.Label == "Windows Hello Hardware Authenticator")
                                                        .GetProperty<FormValue>("value")
                                                        .Form
                                                        .GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "id")
                                                        .GetProperty<string>("value");




            var selectFingerprintProceedRequest = new IdxRequestPayload();
            selectFingerprintProceedRequest.StateHandle = identifyResponse.StateHandle;
            selectFingerprintProceedRequest.SetProperty("authenticator", new
            {
                id = fingerprintId,
            });



            var selectFingerprintResponse = await selectAuthenticatorRemediationOption.ProceedAsync(selectFingerprintProceedRequest);


            var challengeFingerprintRequest = new IdxRequestPayload();
            challengeFingerprintRequest.StateHandle = selectFingerprintResponse.StateHandle;
            challengeFingerprintRequest.SetProperty("credentials", new
            {
                authenticatorData = "5vIi/yA...",
                clientData = "eyJjaGFsbGVuZ2UiO...",
                signatureData = "jaZSjGS6+jiVH...",
            });


            var challengeFingerprintResponse = await selectFingerprintResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(X => X.Name == "challenge-authenticator")
                                            .ProceedAsync(challengeFingerprintRequest);

            challengeFingerprintResponse.IsLoginSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task EnrollFingerprint()
        {
            var client = TestIdxClient.Create();

            var idxContext = await client.InteractAsync();

            var introspectResponse = await client.IntrospectAsync(idxContext.InteractionHandle);

            var identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = introspectResponse.StateHandle;
            identifyRequest.SetProperty("identifier", "test-login-fingerprint@test.com");

            // Send username
            var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "identify")
                                                        .ProceedAsync(identifyRequest);


            var selectAuthenticatorRemediationOption1 = identifyResponse.Remediation.RemediationOptions
                                                                .FirstOrDefault(x => x.Name == "select-authenticator-authenticate");

            // Select password
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
            challengePasswordRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
            challengePasswordRequest.SetProperty("credentials", new
            {
                passcode = Environment.GetEnvironmentVariable("OKTA_IDX_PASSWORD"),
            });

            var challengePasswordRemediationOption = await selectPasswordAuthenticatorResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                                        .ProceedAsync(challengePasswordRequest);

            var selectAuthenticatorRemediationOption = challengePasswordRemediationOption.Remediation.RemediationOptions
                                                                .FirstOrDefault(x => x.Name == "select-authenticator-enroll");

            var fingerprintId = selectAuthenticatorRemediationOption.GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "authenticator")
                                                        .Options
                                                        .FirstOrDefault(x => x.Label == "Security Key or Biometric")
                                                        .GetProperty<FormValue>("value")
                                                        .Form
                                                        .GetArrayProperty<FormValue>("value")
                                                        .FirstOrDefault(x => x.Name == "id")
                                                        .GetProperty<string>("value");




            var selectFingerprintProceedRequest = new IdxRequestPayload();
            selectFingerprintProceedRequest.StateHandle = identifyResponse.StateHandle;
            selectFingerprintProceedRequest.SetProperty("authenticator", new
            {
                id = fingerprintId,
            });



            var selectFingerprintResponse = await selectAuthenticatorRemediationOption.ProceedAsync(selectFingerprintProceedRequest);


            var challengeFingerprintRequest = new IdxRequestPayload();
            challengeFingerprintRequest.StateHandle = selectFingerprintResponse.StateHandle;
            challengeFingerprintRequest.SetProperty("credentials", new
            {
                authenticatorData = "5vIi/yA...",
                clientData = "eyJjaGFsbGVuZ2UiO...",
            });


            var challengeFingerprintResponse = await selectFingerprintResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(X => X.Name == "enroll-authenticator")
                                            .ProceedAsync(challengeFingerprintRequest);

            challengeFingerprintResponse.IsLoginSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task LoginSuccessfullyWithProgressiveProfilingFeature()
        {
            var client = TestIdxClient.Create();

            var idxContext = await client.InteractAsync();

            var introspectResponse = await client.IntrospectAsync(idxContext.InteractionHandle);

            var identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = introspectResponse.StateHandle;
            identifyRequest.SetProperty("identifier", "test-progressive-profiling@okta.com");

            // Send username
            var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "identify")
                                                        .ProceedAsync(identifyRequest);

            identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = identifyResponse.StateHandle;
            identifyRequest.SetProperty("credentials", new
            {
                passcode = Environment.GetEnvironmentVariable("OKTA_IDX_PASSWORD"),
            });


            var challengeResponse = await identifyResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                                        .ProceedAsync(identifyRequest);


            // TODO: Enable profiling attributes
            var enrollProfileRequest = new IdxRequestPayload();
            enrollProfileRequest.StateHandle = challengeResponse.StateHandle;
            enrollProfileRequest.SetProperty("userProfile", new
            {
                prop1 = "foo",
                prop2 = "bar",
            });

            var enrollProfileResponse = await challengeResponse.Remediation.RemediationOptions
                                                       .FirstOrDefault(x => x.Name == "enroll-profile")
                                                       .ProceedAsync(enrollProfileRequest);

            enrollProfileResponse.IsLoginSuccess.Should().BeTrue();
        }
    }
}
