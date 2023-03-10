# Changelog

## 2.2.9

### Bug Fixes

* Fix issue with authenticators response not getting webauthn authenticator enrollments name.

## 2.2.8

### Bug Fixes

* Fix issue with `AuthenticateAsync()` method failing when a Users Password is in Warn Before Expires State (#182).
* Marking `AuthenticationResponse.MessageToUser` as deprecated.


## 2.2.7

### Bug Fixes

* Fix issue with Phone factor when enrolled with multiple contact numbers (#169)
* Fix issue with Reset password workflow (#174)

## 2.2.6

### Bug Fixes

* Properly initialize PollResponse with json data received from server after polling related method calls.

## 2.2.5

### Features

* Add Okta Verify number challenge support.

## 2.2.4

### Bug Fixes

* Fix Okta Verify push notification not sent if "send push automatically" is unchecked.

## 2.2.3

### Features

* Added support for login_hint and ACR values.

## 2.2.2

### Bug Fixes

* Properly handle Okta Verify push rejection.

## 2.2.1

### Updates

* Send method type when selecting email as a challenge authenticator to avoid additional email selection.

## 2.2.0

### Features

* Add support for security question.
* Add support for sending device token, user agent and IP address headers in methods that bootstraps a new authentication flow. The device token via client configuration has been deprecated.

## 2.1.0

### Features

* Add support for optional Password. 

## 2.0.1

### Bug Fixes

* SSPR process fails for Embedded SDK sample.

## 2.0.0

### Features

* Add support for Google Authenticator TOTP.
* Add support for WebAuthn 
* Add support for Okta Verify
* Add support for Device Token configuration
* Add support for Recovery Token
* Add support for Activation Token

## 1.0.1

### Updates

* Remove FlexibleConfiguration dependency, use Microsoft Configuration instead.

## 1.0.0

* SDK is now GA.

## 0.1.0-beta06

### Features

* Add support for internationalization.
* Update SIW version to 5.8.1.

### Breaking changes

* Remove css-related `Class` property from `IMessage` interface and `Message` class.

### Bug Fixes

* Validate that URIs are valid. (#98)
* Reinit SIW on interaction required. (#103)

## 0.1.0-beta05

### Features

* Add support to resend code during MFA.

## 0.1.0-beta04

### Features

* Add new methods to represent different actions in the authentication flow.
* Add ASP.NET 4.8 samples showing how to use the SDK.

#### New Methods

* `Task<AuthenticationResponse> ChangePasswordAsync(ChangePasswordOptions changePasswordOptions, IIdxContext idxContext, CancellationToken cancellationToken = default);`

* `Task<TokenResponse> RedeemInteractionCodeAsync(IIdxContext idxContext, string interactionCode, CancellationToken cancellationToken = default);`

* `Task<AuthenticationResponse> AuthenticateAsync(AuthenticationOptions authenticationOptions, CancellationToken cancellationToken = default);`

* `Task<AuthenticationResponse> RecoverPasswordAsync(RecoverPasswordOptions recoverPasswordOptions, CancellationToken cancellationToken = default);`

* `Task<AuthenticationResponse> VerifyAuthenticatorAsync(VerifyAuthenticatorOptions verifyAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default);`

* `Task RevokeTokensAsync(TokenType tokenType, string token, CancellationToken cancellationToken = default);`

* `Task<AuthenticationResponse> RegisterAsync(UserProfile userProfile, CancellationToken cancellationToken = default);`

* `Task<AuthenticationResponse> SelectEnrollAuthenticatorAsync(SelectEnrollAuthenticatorOptions enrollAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default);`

* `Task<AuthenticationResponse> SelectRecoveryAuthenticatorAsync(SelectAuthenticatorOptions selectAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default);`

* `Task<AuthenticationResponse> EnrollAuthenticatorAsync(EnrollPhoneAuthenticatorOptions enrollAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default);`

* `Task<AuthenticationResponse> SelectChallengeAuthenticatorAsync(SelectAuthenticatorOptions selectAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default);`

* `Task<AuthenticationResponse> SelectChallengeAuthenticatorAsync(SelectPhoneAuthenticatorOptions selectAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default);`

* `Task<AuthenticationResponse> ChallengeAuthenticatorAsync(ChallengePhoneAuthenticatorOptions challengeAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default);`

* `Task<WidgetSignInResponse> StartWidgetSignInAsync(CancellationToken cancellationToken = default);`

* `Task<AuthenticationResponse> SkipAuthenticatorSelectionAsync(IIdxContext idxContext, CancellationToken cancellationToken = default);`

* `Task<IdentityProvidersResponse> GetIdentityProvidersAsync(string state = null, CancellationToken cancellationToken = default);`

* `Task<IdentityProvidersResponse> GetIdentityProvidersAsync(IIdxContext idxContext, CancellationToken cancellationToken = default);`

## v0.1.0-beta03

### Features

* Update `InteractAsync()` method to support passing an existing state
* Expose the `state` in the `IdxContext`

## v0.1.0-beta02

### Features

* Add support for password recovery
* Update the client to allow for continuing a flow from any point
* Move the creation of the PKCE codeVerifier into `InteractAsync()` instead of the client constructor

### Breaking changes

* `InteractAsync` now returns an `IdxContext`
* Remove accessor of the `IdxContext` from the client
* `ExchangeCodeAsync()` now requires an `IdxContext` to exchange tokens

### Additions

New models: `AuthenticatorEnrollment`, `AuthenticatorEnrollmentValue`, `Recover` and `AuthenticatorEnrollmentMethod`.

## v0.1.0-beta01

### Features

- Initial version with basic functionality:
* `InteractAsync()`
* `IntrospectAsync()`
* `CancelAsync()`
* `RemediationOption.ProceedAsync()`
* `IdxResponse.SuccessWithInteractionCode.ExchangeCode()`
