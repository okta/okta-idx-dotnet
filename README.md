[<img src="https://aws1.discourse-cdn.com/standard14/uploads/oktadev/original/1X/0c6402653dfb70edc661d4976a43a46f33e5e919.png" align="right" width="256px"/>](https://devforum.okta.com/)

[![Support](https://img.shields.io/badge/support-Developer%20Forum-blue.svg)][devforum]
[![API Reference](https://img.shields.io/badge/docs-reference-lightgrey.svg)][dotnetdocs]

# Okta .NET IDX SDK

This repository contains the Okta IDX SDK for .NET. This SDK can be used in your server-side code to interact with the Okta Identity Engine.

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

Once you initialize a `Client`, you can call methods to make requests to the Okta API.

### Create the Client

```csharp
var client = new IdxClient(new IdxConfiguration()
            {
                Issuer = "{YOUR_ISSUER}", // e.g. https://foo.okta.com/oauth2/default, https://foo.okta.com/oauth2/ausar5vgt5TSDsfcJ0h7
                ClientId = "{YOUR_CLIENT_ID}",
                ClientSecret = "{YOUR_CLIENT_SECRET}", //Required for confidential clients. 
                RedirectUri = "{YOUR_REDIRECT_URI}", // Must match the redirect uri in client app settings/console
                Scopes = OktaDefaults.Scopes,
            });
```
### Get Interaction Handle

```csharp
var interactResponse = await client.InteractAsync();
var interactHandle = interactResponse.InteractionHandle;
```

### Get State Handle

```csharp
// optional with interactionHandle or empty; if empty, a new interactionHandle will be obtained
var introspectResponse = await client.IntrospectAsync(interactResponse.InteractionHandle);
var stateHandle = introspectResponse.StateHandle;
```

### Get new tokens (access + id + refresh tokens) using interact code flow

In this example the sign-on policy has no authenticators required.

> Note: Steps to identify the user might change based on the Org configuration.

```csharp
// Create a new client passing the desired scopes
var client = new IdxClient(new IdxConfiguration()
            {
                Issuer = "{YOUR_ISSUER}", // e.g. https://foo.okta.com/oauth2/default, https://foo.okta.com/oauth2/ausar5vgt5TSDsfcJ0h7
                ClientId = "{YOUR_CLIENT_ID}",
                ClientSecret = "{YOUR_CLIENT_SECRET}", //Required for confidential clients. 
                RedirectUri = "{YOUR_REDIRECT_URI}", // Must match the redirect uri in client app settings/console
                Scopes = "openid profile offline_access",
            });

// Call Introspect - interactionHandle is optional; if it's not provided, a new interactionHandle will be obtained.
var introspectResponse = await client.IntrospectAsync();

// Identify with username 
var identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = introspectResponse.StateHandle;
            identifyRequest.SetProperty("identifier", " foo@example.com");

var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "identify")
                                                        .ProceedAsync(identifyRequest);

// Challenge with password
identifyRequest = new IdxRequestPayload()
            {
                StateHandle = identifyResponse.StateHandle,
            };

            identifyRequest.SetProperty("credentials", new
            {
                passcode = "foo",
            });


var challengeResponse = await identifyResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                            .ProceedAsync(identifyRequest);   

// Exchange tokens
var tokenResponse = await challengeResponse.SuccessWithInteractionCode.ExchangeCodeAsync();

```

### Cancel the OIE transaction and start a new one after that

In this example the Org is configured to require email as a second authenticator. After answering password challenge, a cancel request is send right before answering the email challenge.

```csharp
// Create a new client passing the desired scopes
var client = new IdxClient(new IdxConfiguration()
            {
                Issuer = "{YOUR_ISSUER}", // e.g. https://foo.okta.com/oauth2/default, https://foo.okta.com/oauth2/ausar5vgt5TSDsfcJ0h7
                ClientId = "{YOUR_CLIENT_ID}",
                ClientSecret = "{YOUR_CLIENT_SECRET}", //Required for confidential clients. 
                RedirectUri = "{YOUR_REDIRECT_URI}", // Must match the redirect uri in client app settings/console
                Scopes = "openid profile offline_access",
            });

// Call Introspect - interactionHandle is optional; if it's not provided, a new interactionHandle will be obtained.
var introspectResponse = await client.IntrospectAsync();

// Identify with username 
var identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = introspectResponse.StateHandle;
            identifyRequest.SetProperty("identifier", " foo@example.com");

var identifyResponse = await introspectResponse.Remediation.RemediationOptions
                                                        .FirstOrDefault(x => x.Name == "identify")
                                                        .ProceedAsync(identifyRequest);

// Challenge with password
identifyRequest = new IdxRequestPayload()
            {
                StateHandle = identifyResponse.StateHandle,
            };

            identifyRequest.SetProperty("credentials", new
            {
                passcode = "foo",
            });


var challengeResponse = await identifyResponse.Remediation.RemediationOptions
                                            .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                            .ProceedAsync(identifyRequest);   

// Before answering email challenge, cancel the transaction
await challengeResponse.CancelAsync();

// Get a new interaction code
interactResponse = await client.InteractAsync();

// From now on, you can use interactResponse.InteractionHandle to continue with a new flow.
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

var selectSecondAuthenticatorRequest = new IdentityEngineRequest()
{
    StateHandle = stateHandle,
};

selectSecondAuthenticatorRequest.SetProperty("authenticator", new
{
    id = "aut2ihzk1gHl7ynhd1d6",
    methodType = "email",
});

var response = await selectSecondAuthenticatorRemediationOption.ProceedAsync(selectSecondAuthenticatorRequest);
```

### Answer Authenticator Challenge

```csharp
var challengeSecondAuthenticatorRemediationOption = response.Remediation.RemediationOptions.FirstOrDefault(x => x.Name == "challenge-authenticator");


var code = "<CODE_RECEIVED_BY_EMAIL>"

var sendEmailCodeRequest = new IdentityEngineRequest()
{
    StateHandle = stateHandle,
};

sendEmailCodeRequest.SetProperty("credentials", new
{
    passcode = code,
});

var response = await challengeSecondAuthenticatorRemediationOption.ProceedAsync(sendEmailCodeRequest);
```

### Cancel Flow

```csharp
// invalidates the supplied stateHandle and obtains a fresh one
var idxResponse = await client.CancelAsync();
```

### Get Token with Interaction Code


```csharp
if (idxResponse.IsLoginSuccessful) {
    // exchange interaction code for token
    var tokenResponse = await challengeResponse.SuccessWithInteractionCode.ExchangeCodeAsync();
    var accessToken = tokenResponse.AccessToken;
    var idToken = tokenResponse.IdToken;
}
```

### Print Raw Response

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