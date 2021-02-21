[<img src="https://aws1.discourse-cdn.com/standard14/uploads/oktadev/original/1X/0c6402653dfb70edc661d4976a43a46f33e5e919.png" align="right" width="256px"/>](https://devforum.okta.com/)

[![Support](https://img.shields.io/badge/support-Developer%20Forum-blue.svg)][devforum]
[![API Reference](https://img.shields.io/badge/docs-reference-lightgrey.svg)][dotnetdocs]

# Okta .NET IDX SDK

This repository contains the Okta IDX SDK for .NET. This SDK can be used in your server-side code to interact with the Okta Identity Engine.

> :grey_exclamation: The use of this SDK requires usage of the Okta Identity Engine. This functionality is in general availability but is being gradually rolled out to customers. If you want to request to gain access to the Okta Identity Engine, please reach out to your account manager. If you do not have an account manager, please reach out to oie@okta.com for more information.

> :warning: Beta alert! This library is in beta. See [release status](#release-status) for more information.

* [Release status](#release-status)
* [Need help?](#need-help)
* [Getting started](#getting-started)
* [Usage guide](#usage-guide)
* [Configuration reference](#configuration-reference)
* [Building the SDK](#building-the-sdk)
* [Contributing](#contributing)

## Release status

This library uses semantic versioning and follows Okta's [Library Version Policy][okta-library-versioning].

| Version | Status                             |
| ------- | ---------------------------------- |
| 0.1.0    | :warning: Beta |

The latest release can always be found on the [releases page][github-releases].

## Need help?
 
If you run into problems using the SDK, you can
 
* Ask questions on the [Okta Developer Forums][devforum]
* Post [issues][github-issues] here on GitHub (for code errors)

## Getting started

### Prerequisites

You will need:

* An Okta account, called an _organization_ (sign up for a free [developer organization](https://developer.okta.com/signup) if you need one)

## Usage guide

These examples will help you understand how to use this library.

Once you initialize a `Client`, you can call methods to make requests to the Okta API. Check out the [Configuration reference](#configuration-reference) section for more details.

### Create the Client

```csharp
var client = new IdxClient(new IdxConfiguration()
            {
                Issuer = "{YOUR_ISSUER}", // e.g. https://foo.okta.com/oauth2/default, https://foo.okta.com/oauth2/ausar5vgt5TSDsfcJ0h7
                ClientId = "{YOUR_CLIENT_ID}",
                ClientSecret = "{YOUR_CLIENT_SECRET}", //Required for confidential clients. 
                RedirectUri = "{YOUR_REDIRECT_URI}", // Must match the redirect uri in client app settings/console
                Scopes = "openid profile offline_access",
            });
```

### Get Interaction Handle and Code Verifier

```csharp
var idxContext = await client.InteractAsync();
var interactHandle = idxContext.InteractionHandle;
var codeVerifier = idxContext.CodeVerifier;
```

### Get State Handle

```csharp
var introspectResponse = await client.IntrospectAsync(idxContext);
var stateHandle = introspectResponse.StateHandle;
```

### Get New Tokens (access_token/id_token/refresh_token)

In this example the sign-on policy has no authenticators required.

> Note: Steps to identify the user might change based on your Org configuration.

```csharp
// Create a new client passing the desired scopes
var client = new IdxClient(new IdxConfiguration()
            {
                Issuer = "{YOUR_ISSUER}",                 // e.g. https://foo.okta.com/oauth2/default, https://foo.okta.com/oauth2/ausar5vgt5TSDsfcJ0h7
                ClientId = "{YOUR_CLIENT_ID}",
                ClientSecret = "{YOUR_CLIENT_SECRET}",    // Required for confidential clients. 
                RedirectUri = "{YOUR_REDIRECT_URI}",      // Must match the redirect uri in client app settings/console
                Scopes = "openid profile offline_access", // Optional. The default value is "openid profile".
            });

// Call Interact to obtain an IDX context
var idxContext = await client.InteractAsync();

// Continue with Introspect
var introspectResponse = await client.IntrospectAsync(idxContext);

// Identify with username 
var identifyRequest = new IdxRequestPayload();
identifyRequest.StateHandle = introspectResponse.StateHandle;
identifyRequest.SetProperty("identifier", " foo@example.com");

var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "identify")
                                                        .ProceedAsync(identifyRequest);

// Challenge with password
identifyRequest = new IdxRequestPayload();
identifyRequest.StateHandle = identifyResponse.StateHandle;            
identifyRequest.SetProperty("credentials", new
{
    passcode = "foo",
});


var challengeResponse = await identifyResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                            .ProceedAsync(identifyRequest);   

// Exchange tokens
var tokenResponse = await challengeResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);

```

### Cancel the OIE transaction and start a new one after that

In this example, the Org is configured to require an email as a second authenticator. After answering the password challenge, a cancel request is sent right before answering the email challenge.

> Note: Steps to identify the user might change based on your Org configuration.

```csharp
var client = new IdxClient();

// Call Interact to obtain an IDX context
var idxContext = await client.InteractAsync();

// Continue with Introspect
var introspectResponse = await client.IntrospectAsync(idxContext);

// Identify with username 
var identifyRequest = new IdxRequestPayload();
identifyRequest.StateHandle = introspectResponse.StateHandle;
identifyRequest.SetProperty("identifier", "foo@example.com");

var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "identify")
                                                        .ProceedAsync(identifyRequest);

// Challenge with password
identifyRequest = new IdxRequestPayload();
identifyRequest.StateHandle = identifyResponse.StateHandle;            
identifyRequest.SetProperty("credentials", new
{
    passcode = "foo",
});


var challengeResponse = await identifyResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                            .ProceedAsync(identifyRequest);   

// Before answering the email challenge, cancel the transaction
await challengeResponse.CancelAsync();

// Get a new IDX context
idxContext = await client.InteractAsync();

// From now on, you can use the new IDX context to continue with a new flow.
```                                            

### Reset password 

In this example, the Org is configured to require only password when logging into. After providing the username, select your current authenticator enrollment and proceed with the recovery process.

> Note: Steps to identify the user might change based on your Org configuration.

```csharp
var client = new IdxClient();

var idxContext = await client.InteractAsync();

var introspectResponse = await client.IntrospectAsync(idxContext);

var identifyRequest = new IdxRequestPayload();
identifyRequest.StateHandle = introspectResponse.StateHandle;
identifyRequest.SetProperty("identifier", "test-recover-password@test.com");

// Send username
var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "identify")
                                            .ProceedAsync(identifyRequest);

// Proceed with recovery
var recoveryRequest = new IdxRequestPayload();
recoveryRequest.StateHandle = identifyResponse.StateHandle;

var recoveryResponse = await identifyResponse.CurrentAuthenticatorEnrollment.Value.Recover.ProceedAsync(recoveryRequest);

// Select email
var selectEmailAuthenticatorRemediationOption = recoveryResponse.Remediation.RemediationOptions
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

// Send code
var selectEmailAuthenticatorRequest = new IdxRequestPayload();
selectEmailAuthenticatorRequest.StateHandle = identifyResponse.StateHandle;
selectEmailAuthenticatorRequest.SetProperty("authenticator", new
{
    id = emailId,
});


var selectEmailAuthenticatorResponse = await selectEmailAuthenticatorRemediationOption.ProceedAsync(selectEmailAuthenticatorRequest);

var challengeEmailRequest = new IdxRequestPayload();
challengeEmailRequest.StateHandle = selectEmailAuthenticatorResponse.StateHandle;
challengeEmailRequest.SetProperty("credentials", new
{
    passcode = "xxxxxx",
});


var challengeEmailResponse = await selectEmailAuthenticatorResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                                    .ProceedAsync(challengeEmailRequest);

// Send your security question value
var challengeSecurityQuestionRemediationOption = challengeEmailResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "challenge-authenticator");

var securityQuestionValue = challengeSecurityQuestionRemediationOption.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "credentials")
                                            .Form.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "questionKey")
                                            .GetProperty<string>("value");


var challengeSecurityQuestionRequest = new IdxRequestPayload();
challengeSecurityQuestionRequest.StateHandle = challengeEmailResponse.StateHandle;
challengeSecurityQuestionRequest.SetProperty("credentials", new
{
    answer = "chicken",
    questionKey = securityQuestionValue,
});

var challengeSecurityQuestionResponse = await challengeSecurityQuestionRemediationOption.ProceedAsync(challengeSecurityQuestionRequest);


// Reset password
var resetAuthenticatorRequest = new IdxRequestPayload();
resetAuthenticatorRequest.StateHandle = challengeSecurityQuestionResponse.StateHandle;
resetAuthenticatorRequest.SetProperty("credentials", new
{
    passcode = "yyyyyy",
});


var resetAuthenticatorResponse = await challengeSecurityQuestionResponse.Remediation.RemediationOptions
                                                .FirstOrDefault(x => x.Name == "reset-authenticator")
                                                .ProceedAsync(resetAuthenticatorRequest);
// Exchange tokens
if (resetAuthenticatorResponse.IsLoginSuccess)
{
    var tokenResponse = await resetAuthenticatorResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);
}
```

### Remediation/MFA scenarios with sign-on policy

#### Login using password + enroll security question authenticator

In this example, the Org is configured to require a security question as a second authenticator. After answering the password challenge, users have to select _security question_ and then select a question and enter an answer to finish the process.

> Note: Steps to identify the user might change based on your Org configuration.

```csharp
var client = new IdxClient();

// Call Interact to obtain an IDX context
var idxContext = await client.InteractAsync();

// Continue with Introspect
var introspectResponse = await client.IntrospectAsync(idxContext);

var identifyRequest = new IdxRequestPayload();
identifyRequest.StateHandle = introspectResponse.StateHandle;
identifyRequest.SetProperty("identifier", "foo@example.com");
identifyRequest.SetProperty("credentials", new
{
    passcode = "foo",
});

// Send username and password
var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "identify")
                                            .ProceedAsync(identifyRequest);


// Select `select-authenticator-enroll` remediation option
var selectAuthenticatorRemediationOption = identifyResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "select-authenticator-enroll");

// Get the authenticator ID
var securityQuestionId = selectAuthenticatorRemediationOption.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "authenticator")
                                            .Options
                                            .FirstOrDefault(x => x.Label == "Security Question")
                                            .GetProperty<FormValue>("value")
                                            .Form
                                            .GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "id")
                                            .GetProperty<string>("value");


// Proceed with security question
var selectAuthenticatorEnrollRequest = new IdxRequestPayload();
selectAuthenticatorEnrollRequest.StateHandle = identifyResponse.StateHandle;
selectAuthenticatorEnrollRequest.SetProperty("authenticator", new
{
    id = securityQuestionId,
});


var selectAuthenticatorEnrollResponse = await identifyResponse.Remediation.RemediationOptions
                                .FirstOrDefault(x => x.Name == "select-authenticator-enroll")
                                .ProceedAsync(selectAuthenticatorEnrollRequest);



// Select `enroll-authenticator` remediation option
var enrollAuthenticatorRemediationOption = selectAuthenticatorEnrollResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "enroll-authenticator");

// Select a question
var securityQuestionValue = enrollAuthenticatorRemediationOption.GetArrayProperty<FormValue>("value")
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

// Proceed with the answer
var enrollRequest = new IdxRequestPayload();
enrollRequest.StateHandle = selectAuthenticatorEnrollResponse.StateHandle;
enrollRequest.SetProperty("credentials", new {
    answer = "chicken",
    questionKey = securityQuestionValue,
});

var enrollResponse = await enrollAuthenticatorRemediationOption.ProceedAsync(enrollRequest);

// Skip other optional factors if applicable; otherwise exchange tokens `enrollResponse.SuccessWithInteractionCode.ExchangeCodeAsync()`
var skipRequest = new IdxRequestPayload();
skipRequest.StateHandle = enrollResponse.StateHandle;

var skipResponse = await enrollResponse.Remediation.RemediationOptions
                            .FirstOrDefault(x => x.Name == "skip")
                            .ProceedAsync(skipRequest);


var tokenResponse = await skipResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);
```

#### Login using password + email authenticator

In this example, the Org is configured to require an email as a second authenticator. After answering the password challenge, users have to select _email_ and enter the code to finish the process.

> Note: Steps to identify the user might change based on your Org configuration.

> Note: If users click a magic link instead of providing a code, they will be redirected to the login page with a valid session if applicable.

```csharp
var client = new IdxClient();

// Call Interact to obtain an IDX context
var idxContext = await client.InteractAsync();

// Continue with Introspect
var introspectResponse = await client.IntrospectAsync(idxContext);

var identifyRequest = new IdxRequestPayload();
identifyRequest.StateHandle = introspectResponse.StateHandle;
identifyRequest.SetProperty("identifier", "test-login-email@test.com");

// Send username
var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "identify")
                                            .ProceedAsync(identifyRequest);

// Select `select-authenticator-authenticate` remediation option
var selectAuthenticatorRemediationOption1 = identifyResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "select-authenticator-authenticate");

// Get password authenticator ID 
var passwordId = selectAuthenticatorRemediationOption1.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "authenticator")
                                            .Options
                                            .FirstOrDefault(x => x.Label == "Password")
                                            .GetProperty<FormValue>("value")
                                            .Form
                                            .GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "id")
                                            .GetProperty<string>("value");

// Proceed with password
var selectPasswordRequest = new IdxRequestPayload();
selectPasswordRequest.StateHandle = identifyResponse.StateHandle;
selectPasswordRequest.SetProperty("authenticator", new
{
    id = passwordId
});


var selectPasswordAuthenticatorResponse = await selectAuthenticatorRemediationOption1.ProceedAsync(selectPasswordRequest);

// Challenge password
var challengePasswordRequest = new IdxRequestPayload();
challengePasswordRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
challengePasswordRequest.SetProperty("credentials", new
{
    passcode = "foo",
});

// Select `challenge-authenticator` remediation and proceed
var challengePasswordRemediationOption = await selectPasswordAuthenticatorResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                            .ProceedAsync(challengePasswordRequest);

// Select email authenticator
var selectAuthenticatorRemediationOption2 = identifyResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "select-authenticator-authenticate");

// Get the email authenticator ID
var emailId = selectAuthenticatorRemediationOption2.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "authenticator")
                                            .Options
                                            .FirstOrDefault(x => x.Label == "Email")
                                            .GetProperty<FormValue>("value")
                                            .Form
                                            .GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "id")
                                            .GetProperty<string>("value");


// Proceed with email
var selectEmailAuthenticatorRequest = new IdxRequestPayload();
selectEmailAuthenticatorRequest.StateHandle = identifyResponse.StateHandle;
selectEmailAuthenticatorRequest.SetProperty("authenticator", new
{
    id = emailId,
});


var selectEmailAuthenticatorResponse = await selectAuthenticatorRemediationOption2.ProceedAsync(selectEmailAuthenticatorRequest);

// Challenge email
var challengeEmailRequest = new IdxRequestPayload();
challengeEmailRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
challengeEmailRequest.SetProperty("credentials", new
{
    passcode = "00000", // Set the code you received in your email
});


var challengeEmailResponse = await selectEmailAuthenticatorResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                                    .ProceedAsync(challengeEmailRequest);

if (challengeEmailResponse.IsLoginSuccess)
{
    var tokenResponse = await challengeEmailResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);
}
```

#### Login using password + web authenticator

In this example, the Org is configured with fingerprint as a second authenticator. After answering the password challenge, users have to provide their fingerprint to finish the process.

> Note: Steps to identify the user might change based on your Org configuration.

```csharp
var client = new IdxClient();

// Call Interact to obtain an IDX context
var idxContext = await client.InteractAsync();

// Continue with Introspect
var introspectResponse = await client.IntrospectAsync(idxContext);

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
    passcode = "foo",
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

// Exchange tokens
var tokenResponse = await challengeFingerprintResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);
```

#### Login using password + enroll phone authenticator (SMS/Voice)

In this example, the Org is configured with phone as a second authenticator. After answering the password challenge, users have to provide a phone number and then enter a code to finish the process.

> Note: Steps to identify the user might change based on your Org configuration.


```csharp
var client = new IdxClient();

// Call Interact to obtain an IDX context
var idxContext = await client.InteractAsync();

// Continue with Introspect
var introspectResponse = await client.IntrospectAsync(idxContext);

// Identify with username
var identifyRequest = new IdxRequestPayload();
identifyRequest.StateHandle = introspectResponse.StateHandle;
identifyRequest.SetProperty("identifier", "test-login-phone@test.com");

// Send username
var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "identify")
                                            .ProceedAsync(identifyRequest);

// Select `select-authenticator-authenticate` remediation option
var selectAuthenticatorRemediationOption1 = identifyResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "select-authenticator-authenticate");

// Select password ID first
var passwordId = selectAuthenticatorRemediationOption1.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "authenticator")
                                            .Options
                                            .FirstOrDefault(x => x.Label == "Password")
                                            .GetProperty<FormValue>("value")
                                            .Form
                                            .GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "id")
                                            .GetProperty<string>("value");

// Proceed with password
var selectPasswordRequest = new IdxRequestPayload();
selectPasswordRequest.StateHandle = identifyResponse.StateHandle;
selectPasswordRequest.SetProperty("authenticator", new
{
    id = passwordId
});


var selectPasswordAuthenticatorResponse = await selectAuthenticatorRemediationOption1.ProceedAsync(selectPasswordRequest);

// Send credentials
var challengePasswordRequest = new IdxRequestPayload();
challengePasswordRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
challengePasswordRequest.SetProperty("credentials", new
{
    passcode = "foo",
});

var challengePasswordRemediationOption = await selectPasswordAuthenticatorResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                            .ProceedAsync(challengePasswordRequest);

// Select `select-authenticator-enroll` remediation option
var selectAuthenticatorRemediationOption2 = identifyResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "select-authenticator-enroll");

// Get the phone authenticator ID
var phoneId = selectAuthenticatorRemediationOption2.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "authenticator")
                                            .Options
                                            .FirstOrDefault(x => x.Label == "Phone")
                                            .GetProperty<FormValue>("value")
                                            .Form
                                            .GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "id")
                                            .GetProperty<string>("value");

// Proceed with phone (SMS or Voice)
var selectPhoneAuthenticatorRequest = new IdxRequestPayload();
selectPhoneAuthenticatorRequest.StateHandle = identifyResponse.StateHandle;
selectPhoneAuthenticatorRequest.SetProperty("authenticator", new
{
    phoneNumber = "xxxxxx",
    id = phoneId,
    methodType = "sms" // You can set either `sms` or `voice`
});


var selectPhoneAuthenticatorResponse = await selectAuthenticatorRemediationOption2.ProceedAsync(selectPhoneAuthenticatorRequest);

// Send the code received via SMS or Voice
var challengePhoneRequest = new IdxRequestPayload();
challengePhoneRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
challengePhoneRequest.SetProperty("credentials", new
{
    passcode = "xxxxxx", // Set the code received via SMS or Voice
});


var challengePhoneResponse = await selectPhoneAuthenticatorResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "enroll-authenticator")
                                                    .ProceedAsync(challengePhoneRequest);

// Exchange tokens
var tokenResponse = await challengePhoneResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);

```

#### Login using password + enroll web authenticator (Fingerprint)

In this example, the Org is configured with web authenticator as a second authenticator. After answering the password challenge, users have to provide their fingerprint to finish the process.

> Note: Steps to identify the user might change based on your Org configuration.


```csharp
var client = new IdxClient();

// Call Interact to obtain an IDX context
var idxContext = await client.InteractAsync();

// Continue with Introspect
var introspectResponse = await client.IntrospectAsync(idxContext);

var identifyRequest = new IdxRequestPayload();
identifyRequest.StateHandle = introspectResponse.StateHandle;
identifyRequest.SetProperty("identifier", "test-enroll-fingerprint@test.com");

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

// Send password
var challengePasswordRequest = new IdxRequestPayload();
challengePasswordRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
challengePasswordRequest.SetProperty("credentials", new
{
    passcode = "foo",
});

var challengePasswordRemediationOption = await selectPasswordAuthenticatorResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                            .ProceedAsync(challengePasswordRequest);

// Select "select-authenticator-enroll" remediation option 
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


// Select the desired authenticator ID
var selectFingerprintProceedRequest = new IdxRequestPayload();
selectFingerprintProceedRequest.StateHandle = identifyResponse.StateHandle;
selectFingerprintProceedRequest.SetProperty("authenticator", new
{
    id = fingerprintId,
});

var selectFingerprintResponse = await selectAuthenticatorRemediationOption.ProceedAsync(selectFingerprintProceedRequest);

// Select "enroll-authenticator" and proceed with the `authenticationData` and `clientData`.
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

// Exchange tokens
var tokenResponse = await challengeFingerprintResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);
```

#### Login using password + email authenticator + enroll phone + enroll security question

In this example, the Org is configured to require email, phone and security question in addition to password. After answering the password challenge, users have to select _email_ and enter the code, enroll a phone and enter the code and finally enroll a security question to complete the process.

> Note: Steps to identify the user might change based on your Org configuration.

```csharp
var client = new IdxClient();

// Call Interact to obtain an IDX context
var idxContext = await client.InteractAsync();

// Continue with Introspect
var introspectResponse = await client.IntrospectAsync(idxContext);

var identifyRequest = new IdxRequestPayload();
identifyRequest.StateHandle = introspectResponse.StateHandle;
identifyRequest.SetProperty("identifier", "test-mfa@okta.com");

// Send username
var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "identify")
                                            .ProceedAsync(identifyRequest);

// Select `select-authenticator-authenticate` remediation option and select password
var selectAuthenticatorRemediationOption1 = identifyResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "select-authenticator-authenticate");

// Get password ID
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

// Send password
var challengePasswordRequest = new IdxRequestPayload();
challengePasswordRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
challengePasswordRequest.SetProperty("credentials", new
{
    passcode = "foo",
});

var challengePasswordRemediationOption = await selectPasswordAuthenticatorResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                            .ProceedAsync(challengePasswordRequest);


// EMAIL

// Select `select-authenticator-authenticate` remediation option
var selectEmailAuthenticatorRemediationOption = challengePasswordRemediationOption.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "select-authenticator-authenticate");

// Get email authenticator ID
var emailId = selectEmailAuthenticatorRemediationOption.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "authenticator")
                                            .Options
                                            .FirstOrDefault(x => x.Label == "Email")
                                            .GetProperty<FormValue>("value")
                                            .Form
                                            .GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "id")
                                            .GetProperty<string>("value");


var selectEmailAuthenticatorRequest = new IdxRequestPayload();
selectEmailAuthenticatorRequest.StateHandle = identifyResponse.StateHandle;
selectEmailAuthenticatorRequest.SetProperty("authenticator", new
{
    id = emailId,
});

// Proceed with email
var selectEmailAuthenticatorResponse = await selectEmailAuthenticatorRemediationOption.ProceedAsync(selectEmailAuthenticatorRequest);

var challengeEmailRequest = new IdxRequestPayload();
challengeEmailRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
challengeEmailRequest.SetProperty("credentials", new
{
    passcode = "xxxxxx",
});


var challengeEmailResponse = await selectEmailAuthenticatorResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                                    .ProceedAsync(challengeEmailRequest);
// PHONE

// Select `select-authenticator-enroll` remediation option
var selectPhoneAuthenticatorRemediationOption = challengeEmailResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "select-authenticator-enroll");
// Get phone authenticator ID
var phoneId = selectPhoneAuthenticatorRemediationOption.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "authenticator")
                                            .Options
                                            .FirstOrDefault(x => x.Label == "Phone")
                                            .GetProperty<FormValue>("value")
                                            .Form
                                            .GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "id")
                                            .GetProperty<string>("value");

// Proceed with phone
var selectPhoneAuthenticatorRequest = new IdxRequestPayload();
selectPhoneAuthenticatorRequest.StateHandle = identifyResponse.StateHandle;
selectPhoneAuthenticatorRequest.SetProperty("authenticator", new
{
    id = phoneId,
    methodType = "sms", // You can set either `sms` or `voice`
    phoneNumber = "xxxxxx",
});


var selectPhoneAuthenticatorResponse = await selectPhoneAuthenticatorRemediationOption.ProceedAsync(selectPhoneAuthenticatorRequest);

var challengePhoneRequest = new IdxRequestPayload();
challengePhoneRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
challengePhoneRequest.SetProperty("credentials", new
{
    passcode = code,
});


var challengePhoneResponse = await selectPhoneAuthenticatorResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "enroll-authenticator")
                                                    .ProceedAsync(challengePhoneRequest);




// SECURITY QUESTION
var selectSQAuthenticatorRemediationOption = challengePhoneResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "select-authenticator-enroll");

// Get security question authenticator ID
var securityQuestionId = selectSQAuthenticatorRemediationOption.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "authenticator")
                                            .Options
                                            .FirstOrDefault(x => x.Label == "Security Question")
                                            .GetProperty<FormValue>("value")
                                            .Form
                                            .GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "id")
                                            .GetProperty<string>("value");


// Proceed with Security Question
var selectAuthenticatorEnrollRequest = new IdxRequestPayload();
selectAuthenticatorEnrollRequest.StateHandle = challengePhoneResponse.StateHandle;
selectAuthenticatorEnrollRequest.SetProperty("authenticator", new
{
    id = securityQuestionId,
});


var selectAuthenticatorEnrollResponse = await selectSQAuthenticatorRemediationOption.ProceedAsync(selectAuthenticatorEnrollRequest);

// Select `enroll-authenticator` remediation option
var enrollAuthenticatorRemediationOption = selectAuthenticatorEnrollResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "enroll-authenticator");

// Get the desired security question
var securityQuestionValue = enrollAuthenticatorRemediationOption.GetArrayProperty<FormValue>("value")
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
    questionKey = securityQuestionValue,
});

var enrollResponse = await enrollAuthenticatorRemediationOption.ProceedAsync(enrollRequest);

// skip other optional factors if applicable; otherwise exchange tokens `enrollResponse.SuccessWithInteractionCode.ExchangeCodeAsync()`
var skipRequest = new IdxRequestPayload();
skipRequest.StateHandle = enrollResponse.StateHandle;


var skipResponse = await enrollResponse.Remediation.RemediationOptions
                            .FirstOrDefault(x => x.Name == "skip")
                            .ProceedAsync(skipRequest);


var tokenResponse = await skipResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);

```

#### Login using password + challenge phone authenticator (SMS/Voice)

```csharp
var client = new IdxClient();

// Call Interact to obtain an IDX context
var idxContext = await client.InteractAsync();

// Continue with Introspect
var introspectResponse = await client.IntrospectAsync(idxContext);

// Identify with username
var identifyRequest = new IdxRequestPayload();
identifyRequest.StateHandle = introspectResponse.StateHandle;
identifyRequest.SetProperty("identifier", "test-login-phone@test.com");

// Send username
var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "identify")
                                            .ProceedAsync(identifyRequest);

// Select `select-authenticator-authenticate` remediation option
var selectAuthenticatorRemediationOption1 = identifyResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "select-authenticator-authenticate");

// Select password ID first
var passwordId = selectAuthenticatorRemediationOption1.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "authenticator")
                                            .Options
                                            .FirstOrDefault(x => x.Label == "Password")
                                            .GetProperty<FormValue>("value")
                                            .Form
                                            .GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "id")
                                            .GetProperty<string>("value");

// Proceed with password
var selectPasswordRequest = new IdxRequestPayload();
selectPasswordRequest.StateHandle = identifyResponse.StateHandle;
selectPasswordRequest.SetProperty("authenticator", new
{
    id = passwordId
});


var selectPasswordAuthenticatorResponse = await selectAuthenticatorRemediationOption1.ProceedAsync(selectPasswordRequest);

// Send credentials
var challengePasswordRequest = new IdxRequestPayload();
challengePasswordRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
challengePasswordRequest.SetProperty("credentials", new
{
    passcode = "foo",
});

var challengePasswordRemediationOption = await selectPasswordAuthenticatorResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                            .ProceedAsync(challengePasswordRequest);

// Select `select-authenticator-authenticate` remediation option
var selectAuthenticatorRemediationOption2 = identifyResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "select-authenticator-authenticate");

// Get the phone authenticator ID
var phoneId = selectAuthenticatorRemediationOption2.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "authenticator")
                                            .Options
                                            .FirstOrDefault(x => x.Label == "Phone")
                                            .GetProperty<FormValue>("value")
                                            .Form
                                            .GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "id")
                                            .GetProperty<string>("value");

// Get the phone enrollment ID
var enrollmentId = selectAuthenticatorRemediationOption2.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "authenticator")
                                            .Options
                                            .FirstOrDefault(x => x.Label == "Phone")
                                            .GetProperty<FormValue>("value")
                                            .Form
                                            .GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "enrollmentId")
                                            .GetProperty<string>("value");

// Proceed with phone (SMS or Voice)
var selectPhoneAuthenticatorRequest = new IdxRequestPayload();
selectPhoneAuthenticatorRequest.StateHandle = identifyResponse.StateHandle;
selectPhoneAuthenticatorRequest.SetProperty("authenticator", new
{
    enrollmentId = enrollmentId,
    id = phoneId,
    methodType = "sms" // You can set either `sms` or `voice`
});


var selectPhoneAuthenticatorResponse = await selectAuthenticatorRemediationOption2.ProceedAsync(selectPhoneAuthenticatorRequest);

// Send the code received via SMS or Voice
var challengePhoneRequest = new IdxRequestPayload();
challengePhoneRequest.StateHandle = selectPasswordAuthenticatorResponse.StateHandle;
challengePhoneRequest.SetProperty("credentials", new
{
    passcode = "xxxxxx", // Set the code received via SMS or Voice
});


var challengePhoneResponse = await selectPhoneAuthenticatorResponse.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                                    .ProceedAsync(challengePhoneRequest);

// Exchange tokens
var tokenResponse = await challengePhoneResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);

```

#### User Enrollment - Registration and progressive profiling

In this example, the Org is configured to require additional attributes when users are registering. After answering the password challenge, users have to provide the required attributes to finish the process.

> Note: Steps to identify the user might change based on your Org configuration.

```csharp
var client = new IdxClient();
// Call Interact to obtain an IDX context
var idxContext = await client.InteractAsync();

// Continue with Introspect
var introspectResponse = await client.IntrospectAsync(idxContext);

// Identify with username
var identifyRequest = new IdxRequestPayload();
identifyRequest.StateHandle = introspectResponse.StateHandle;
identifyRequest.SetProperty("identifier", "test-progressive-profiling@okta.com");

var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "identify")
                                            .ProceedAsync(identifyRequest);

// Send credentials
identifyRequest = new IdxRequestPayload();
identifyRequest.StateHandle = identifyResponse.StateHandle;
identifyRequest.SetProperty("credentials", new
{
    passcode = "foo",
});


var challengeResponse = await identifyResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                            .ProceedAsync(identifyRequest);


// Send required attributes
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
```

#### Self-service signup flow

Following factors are enrolled during the signup: password, security question (disliked food) and e-mail. All the others factors are optional and skipped.

```csharp
var client = TestIdxClient.Create();
var idxContext = await client.InteractAsync();
var introspectResponse = await client.IntrospectAsync(idxContext);

var enrollRequest = new IdxRequestPayload
{
    StateHandle = introspectResponse.StateHandle
};

// choose enroll option
var enrollProfileResponse = await introspectResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "select-enroll-profile")
                                            .ProceedAsync(enrollRequest);
var enrollNewProfileRequest = new IdxRequestPayload();
enrollNewProfileRequest.SetProperty("userProfile", new
{
    lastName = "LastName",
    firstName = "FirstName",
    email = "mailbox@domain.com"
});

enrollNewProfileRequest.StateHandle = enrollProfileResponse.StateHandle;

var enrollNewProfileResponse = await enrollProfileResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "enroll-profile")
                                            .ProceedAsync(enrollNewProfileRequest);

// enroll a password factor
var selectAuthenticatorRemediationOption = enrollNewProfileResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "select-authenticator-enroll");

var passwordAuthenticatorId = selectAuthenticatorRemediationOption.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "authenticator")
                                            .Options
                                            .FirstOrDefault(x => x.Label == "Password")
                                            .GetProperty<FormValue>("value")
                                            .Form.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "id")
                                            .GetProperty<string>("value");

var selectPasswordAuthenticatorRequest = new IdxRequestPayload
{
    StateHandle = enrollNewProfileResponse.StateHandle
};
selectPasswordAuthenticatorRequest.SetProperty("authenticator", new
{
    id = passwordAuthenticatorId,
});

var selectPasswordAuthenticatorResponse = await selectAuthenticatorRemediationOption.ProceedAsync(selectPasswordAuthenticatorRequest);

var setPasswordOption = selectPasswordAuthenticatorResponse.Remediation.RemediationOptions
                                    .FirstOrDefault(x => x.Name == "enroll-authenticator");
var setPasswordRequest =  new IdxRequestPayload
{
    StateHandle = enrollNewProfileResponse.StateHandle
};

setPasswordRequest.SetProperty("credentials", new
{
    passcode= "N3wP@55w0rd!"
});

var setPasswordResponcse = await setPasswordOption.ProceedAsync(setPasswordRequest);

// enroll a security question factor
selectAuthenticatorRemediationOption = enrollNewProfileResponse.Remediation.RemediationOptions
                                .FirstOrDefault(x => x.Name == "select-authenticator-enroll");

var questionAuthenticatorId = selectAuthenticatorRemediationOption.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "authenticator")
                                            .Options
                                            .FirstOrDefault(x => x.Label == "Security Question")
                                            .GetProperty<FormValue>("value")
                                            .Form.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "id")
                                            .GetProperty<string>("value");

var selectSecurityQuestionAuthenticatorRequest = new IdxRequestPayload
{
    StateHandle = enrollNewProfileResponse.StateHandle
};
selectSecurityQuestionAuthenticatorRequest.SetProperty("authenticator", new
{
    id = questionAuthenticatorId,
});

var selectSecurityQuestionAuthenticatorResponse = await selectAuthenticatorRemediationOption.ProceedAsync(selectSecurityQuestionAuthenticatorRequest);

var setSecurityQuestionAuthenticatorOption = selectSecurityQuestionAuthenticatorResponse.Remediation.RemediationOptions
                                    .FirstOrDefault(x => x.Name == "enroll-authenticator");
var setSecurityQuestionRequest = new IdxRequestPayload
{
    StateHandle = selectSecurityQuestionAuthenticatorResponse.StateHandle
};
setSecurityQuestionRequest.SetProperty("credentials", new
{
    questionKey = "disliked_food",
    answer = "oatmeal"
});

var setSecurityQuestionRequestResponse = await setSecurityQuestionAuthenticatorOption.ProceedAsync(setSecurityQuestionRequest);


// enroll an e-mail factor

selectAuthenticatorRemediationOption = enrollNewProfileResponse.Remediation.RemediationOptions
                                .FirstOrDefault(x => x.Name == "select-authenticator-enroll");

var emailAuthenticatorId = selectAuthenticatorRemediationOption.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "authenticator")
                                            .Options
                                            .FirstOrDefault(x => x.Label == "Email")
                                            .GetProperty<FormValue>("value")
                                            .Form.GetArrayProperty<FormValue>("value")
                                            .FirstOrDefault(x => x.Name == "id")
                                            .GetProperty<string>("value");

var selectEmailAuthenticatorRequest = new IdxRequestPayload
{
    StateHandle = enrollNewProfileResponse.StateHandle
};
selectEmailAuthenticatorRequest.SetProperty("authenticator", new
{
    id = emailAuthenticatorId,
});

var selectEmailAuthenticatorResponse = await selectAuthenticatorRemediationOption.ProceedAsync(selectEmailAuthenticatorRequest);

var setEmailAuthenticatorOption = selectPasswordAuthenticatorResponse.Remediation.RemediationOptions
                                    .FirstOrDefault(x => x.Name == "enroll-authenticator");
var setEmailRequest = new IdxRequestPayload
{
    StateHandle = selectEmailAuthenticatorResponse.StateHandle
};

setEmailRequest.SetProperty("credentials", new
{
    passcode = "123456" // passcode from one-time verification email
});

var setEmailRequestResponse = await setSecurityQuestionAuthenticatorOption.ProceedAsync(setEmailRequest);

// skip other optional factors

var skipRequest = new IdxRequestPayload
{
    StateHandle = setEmailRequestResponse.StateHandle
};

var skipResponse = await setEmailRequestResponse.Remediation.RemediationOptions
                            .FirstOrDefault(x => x.Name == "skip")
                            .ProceedAsync(skipRequest);
```

### Check Remediation Options

```csharp
// check remediation options to continue the flow
var remediationOptions = identifyResponse.Remediation.RemediationOptions;
remediationOption = remediationOptions.FirstOrDefault();
var formValues = remediationOption.Form;
```

### Check Remediation Options and Select Authenticator

```csharp
var selectSecondAuthenticatorRemediationOption = response.Remediation.RemediationOptions
                                                    .FirstOrDefault(x => x.Name == "select-authenticator-authenticate");

var selectSecondAuthenticatorRequest = new IdentityEngineRequest();
selectSecondAuthenticatorRequest.StateHandle = stateHandle,
selectSecondAuthenticatorRequest.SetProperty("authenticator", new
{
    id = "aut2ihzk1gHl7ynhd1d6",
    methodType = "email",
});

var response = await selectSecondAuthenticatorRemediationOption.ProceedAsync(selectSecondAuthenticatorRequest);
```

### Cancel Flow

```csharp
// invalidates the supplied stateHandle and obtains a fresh one
var idxResponse = await client.CancelAsync();
```

### Get Tokens with Interaction Code

```csharp
if (idxResponse.IsLoginSuccessful) {
    var tokenResponse = await challengeResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);
    var accessToken = tokenResponse.AccessToken;
    var idToken = tokenResponse.IdToken;
}
```

### Print Raw Response

Use the `GetRaw` method to get the full JSON response.

```csharp
var rawResponse = idxResponse.GetRaw();
```

### Get a form value

Use `GetProperty<T>("value")` method to get an ION form value.

```csharp
// Value is string
var stringValue = response.GetProperty<string>("value");

// Value is a FormValue.
var formValues = authenticatorOptionsEmail.GetProperty<FormValue>("value");

```

## Configuration Reference
  
This library looks for configuration in the following sources:
 
1. An `okta.yaml` file in a `.okta` folder in the current user's home directory (`~/.okta/okta.yaml` or `%userprofile%\.okta\okta.yaml`)
2. An `okta.yaml` file in a `.okta` folder in the application or project's root directory
3. Environment variables
4. Configuration explicitly passed to the constructor (see the example in [Getting started](#getting-started))
 
Higher numbers win. In other words, configuration passed via the constructor will override configuration found in environment variables, which will override configuration in `okta.yaml` (if any), and so on.
 
### YAML configuration
 
The full YAML configuration looks like:
 
```yaml
okta:
  idx:
    issuer: "https://{yourOktaDomain}/oauth2/{authorizationServerId}" // e.g. https://foo.okta.com/oauth2/default, https://foo.okta.com/oauth2/ausar5vgt5TSDsfcJ0h7
    clientId: "{clientId}"
    clientSecret: "{clientSecret}" // Required for confidential clients
    scopes:
    - "{scope1}"
    - "{scope2}"
    redirectUri: "{redirectUri}"
```
 
### Environment variables
 
Each one of the configuration values above can be turned into an environment variable name with the `_` (underscore) character:

* `OKTA_IDX_ISSUER`
* `OKTA_IDX_CLIENTID`
* `OKTA_IDX_CLIENTSECRET`
* `OKTA_IDX_SCOPES`
* `OKTA_IDX_REDIRECTURI`


## Building the SDK

In most cases, you won't need to build the SDK from source. If you want to build it yourself just clone the repo and compile using Visual Studio.

## Contributing
 
We are happy to accept contributions and PRs! Please see the [contribution guide](CONTRIBUTING.md) to understand how to structure a contribution.

[devforum]: https://devforum.okta.com/
[dotnetdocs]: https://developer.okta.com/okta-idx-dotnet/latest/
[lang-landing]: https://developer.okta.com/code/dotnet/
[github-issues]: https://github.com/okta/okta-idx-dotnet/issues
[github-releases]: https://github.com/okta/okta-idx-dotnet/releases
[Rate Limiting at Okta]: https://developer.okta.com/docs/api/getting_started/rate-limits
[okta-library-versioning]: https://developer.okta.com/code/library-versions