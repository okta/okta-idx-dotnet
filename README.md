[<img src="https://aws1.discourse-cdn.com/standard14/uploads/oktadev/original/1X/0c6402653dfb70edc661d4976a43a46f33e5e919.png" align="right" width="256px"/>](https://devforum.okta.com/)

[![Support](https://img.shields.io/badge/support-Developer%20Forum-blue.svg)][devforum]
[![API Reference](https://img.shields.io/badge/docs-reference-lightgrey.svg)][dotnetdocs]

# Okta .NET IDX SDK

This library is built for server-side projects in .NET to communicate with Okta as an OAuth
2.0 + OpenID Connect provider. It works with the [Okta's Identity Engine](https://developer.okta.com/docs/concepts/ie-intro/) to authenticate and register users.

To see this library working in a sample, check out our [ASP.NET Samples](samples/samples-aspnet). You can also check out our guides for step-by-step instructions:
 
* [Pre-requisites](https://developer.okta.com/docs/guides/oie-embedded-common-run-samples/aspnet/main/)
* [Build with the embedded SDK](https://developer.okta.com/docs/guides/oie-embedded-common-run-samples/aspnet/main/#run-the-embedded-sdk-sample-app)
* [Build with the embedded widget](https://developer.okta.com/docs/guides/oie-embedded-common-run-samples/aspnet/main/#run-the-embedded-widget-sample-app)

> :grey_exclamation: The use of this SDK requires usage of the Okta Identity Engine. This functionality is in general availability but is being gradually rolled out to customers. If you want to request to gain access to the Okta Identity Engine, please reach out to your account manager. If you do not have an account manager, please reach out to oie@okta.com for more information.

> This library is currently GA. See [release status](#release-status) for more information.

* [Release status](#release-status)
* [Need help?](#need-help)
* [Getting started](#getting-started)
* [Usage guide](#usage-guide)
* [Configuration reference](#configuration-reference)
* [Building the SDK](#building-the-sdk)
* [Contributing](#contributing)

## Release status

This library uses semantic versioning and follows Okta's [Library Version Policy][okta-library-versioning].

✔️ The current stable major version series is: 1.x

| Version | Status                             |
| ------- | ---------------------------------- |
| 1.0.0    | ✔️ Stable |

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
    Scopes = new List<string> { "openid", "profile", "offline_access" }
});
```

### Authenticate users

```csharp
var authnOptions = new AuthenticationOptions
                    {
                        Username = "username@mail.com",
                        Password = "superSecretPassword",
                    };

var authnResponse = await _idxClient.AuthenticateAsync(authnOptions).ConfigureAwait(false);

if (authn.AuthenticationStatus == AuthenticationStatus.Success)
{
    var accessToken = authnResponse.TokenInfo.AccessToken;
}
```

### Authentication Status

The `AuthenticationResponse` you get when using the `IdxClient` will indicate how to proceed to continue with the authentication flow. When using the `AuthenticateAsync` method you can get the following statuses:

#### Success

Type: `AuthenticationStatus.Success`

The user was successfully authenticated and you can retrieve the tokens from the response by calling `authnResponse.TokenInfo`.

#### Password Expired

Type: `AuthenticationStatus.PasswordExpired`

The user needs to change their password to continue with the authentication flow and retrieve tokens.

#### Awaiting for authenticator enrollment

Type: `AuthenticationStatus.AwaitingAuthenticatorEnrollment`

The user needs to enroll an authenticator to continue with the authentication flow and retrieve tokens. You can retrieve the authenticators information by calling `authnResponse.Authenticators`.

#### Awaiting for challenge authenticator selection

Type: `AwaitingChallengeAuthenticatorSelection`

The user needs to select and challenge an additional authenticator to continue with the authentication flow and retrieve tokens. You can retrieve the authenticators information by calling `authnResponse.Authenticators`.

There other statuses that you can get when calling other methods of the `IdxClient`:

#### Awaiting for Authenticator Verification

Type: `AwaitingAuthenticatorVerification`

The user has successfully selected an authenticator to challenge so they now need to verify the selected authenticator. For example, if the user selected phone, this status indicates that they have to provide they code they received to verify the authenticator.

#### Awaiting for Authenticator Enrollment Data

Type: `AwaitingAuthenticatorEnrollmentData`

The user needs to provide additional authenticator information. For example, when a user selects to enroll phone they will have to provide their phone number to complete the enrollment process. You can retrieve current authenticator information by calling  `authnResponse.CurrentAuthenticator`.

#### Awaiting for Challenge Authenticator Data

Type: `AwaitingChallengeAuthenticatorData`

The user needs to provide additional authenticator information. For example, when a user selects to challenge phone they will have to choose if they want to receive the code via voice or SMS. You can retrieve current authenticator enrollment information by calling `authnResponse.CurrentAuthenticatorEnrollment`.

#### Awaiting for Password Reset

Type: `AwaitingPasswordReset`

The user needs to reset their password to continue with the authentication flow and retrieve tokens.

### Revoke Tokens

```csharp
await _idxClient.RevokeTokensAsync(TokenType.AccessToken, accessToken);
```

### Register a user

```csharp
// UserProfile is a dynamic class that allows you set properties dinamically
var userProfile = new UserProfile();
userProfile.SetProperty("firstName", model.FirstName);
userProfile.SetProperty("lastName", model.LastName);
userProfile.SetProperty("email", model.Email);

var registerResponse = await _idxClient.RegisterAsync(userProfile);

if (registerResponse.AuthenticationStatus == AuthenticationStatus.Success)
{
    // Retrieve tokens
}

```

> Note: Check the response's [`AuthenticatonStatus` property](#Authentication-Status) to determine what the next step is.


### Recover Password

```csharp
var recoverPasswordOptions = new RecoverPasswordOptions { Username = model.UserName, };
var authnResponse = await _idxClient.RecoverPasswordAsync(recoverPasswordOptions);

```

> Note: Check the response's [`AuthenticatonStatus` property](#Authentication-Status) to determine what the next step is.

### Handling Errors

The SDK throws an `OktaException` everytime the server responds with an invalid status code, or there is an internal error. You can get more information by calling `exception.Message`.

`UnexpectedRemediationException` is an `OktaException` derived class that usually indicates inconsistencies in the configuration. It is recommended to verify your policy configuration when you face with this error.

`RedeemInteractionCodeException` is an `OktaException` derived class that indicates there was an error when redeeming the interaction code.

`TerminalStateException`  is an `OktaException` derived class that indicates that the user cannot continue the current flow, possibly due to an error or required additional actions outside of the authentication flow.

This exception object contains an array of messages that can be shown to the user as they are. Each message object in the array also contains a key property that can be used for internationalization.
Here is an example of accessing the exception data. All the properties can be null. Null checks are not shown in the example.

```csharp
try
{
    // Trying to sign-on with a non-existent user name.
    // The exact error depends on the Org settings and may differ.
}
catch (TerminalStateException exception)
{
    string combinedTextMessage = exception.Message; // "There is no account with the Username non-existentuser@somewhere.com."

    IList<IMessage> allMessages = exception.IdxMessages.Messages;
    IMessage firstMessage = allMessages.First();

    string firstMessageText = firstMessage.Text; // "There is no account with the Username non-existentuser@somewhere.com."
    IIdxI18n firstMessageI18nInfo = firstMessage.I18n;

    string firstMessageI18nKey = firstMessageI18nInfo.Key; // "idx.unknown.user"

    IList<string> firstMessageI18nParams = firstMessageI18nInfo.Params; // an empty list

    //.........................
}

```

For more usage examples check out our [ASP.NET Sample Application](samples/samples-aspnet).

## Configuration Reference

This library looks for configuration in the following sources:

1. As a YAML file in your home directory
2. As part of an appsettings.json file in your application’s working directory
3. As a YAML file in your application’s working directory
4. In environment variables
5. In the client constructor for the application  (see the example in [Getting started](#getting-started))

This is the order that the locations are searched for configuration information. If information is set in more than one location, it is overwritten so if, for example, the Issuer is set in both an okta.yaml file and in environment variables, the value in the environment variables takes precedence. If the Issuer was also set in the client constructor, that value would be used instead of either of the other two.

> Note: To avoid confusion as to which configuration values are used by the SDK, you should use only one configuration option in your solution.


### YAML configuration

The SDK will look for a YAML file named `okta.yaml` in:

- A folder called .okta whose parent is the current user's home directory
  - On Mac/Linux, this is `~/.okta/okta.yaml`
  - On Windows, this is `%userprofile%\.okta\okta.yaml`
- The application's current working directory
  - If you're using the recommended IISExpress debugger in Visual Studio to run your app, this is `${IIS Express install location}\IIS Express`, for example, `C:\Program Files\IIS Express\okta.yaml`.

The information in the YAML file should be formatted as follows:

```
okta:
  idx:
    issuer: "{YOUR_ISSUER}"
    clientId: "{YOUR_CLIENT_ID}"
    clientSecret: "{YOUR_CLIENT_SECRET}"
    scopes:
      - "openid"
      - "profile"
      - "offline_access"
    redirectUri: "{YOUR_REDIRECT_URI}"
```
### JSON Configuration
The SDK will look for a JSON file named `appsettings.json` in:

- The application’s current working directory
  - If you're using the recommended IISExpress debugger in Visual Studio to run your app, this is `${IIS Express install location}\IIS Express`, for example, `C:\Program Files\IIS Express\appsettings.json`.

The information in the JSON file should be formatted as follows:

```
{
    "okta": {
        "idx": {
            "issuer" : "{YOUR_ISSUER}",
            "clientId" : "{YOUR_CLIENT_ID}",
            "clientSecret": "{YOUR_CLIENT_SECRET}",
            "redirectUri": "{YOUR_REDIRECT_URI}",
            "scopes": [ "openid", "profile", "offline_access" ]
        }
    }
}
```

### Environment variables

The SDK will search your machine’s environment variables for any of the following keys:

* `OKTA_IDX_ISSUER`
* `OKTA_IDX_CLIENTID`
* `OKTA_IDX_CLIENTSECRET`
* `OKTA_IDX_REDIRECTURI`

Environment variables hold only single values so you need to create one for each scope you need to give to your application.

```
OKTA_IDX_SCOPES_0 = "openid"
OKTA_IDX_SCOPES_1 = "profile"
OKTA_IDX_SCOPES_2 = "offline_access"
```

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
