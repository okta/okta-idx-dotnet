# Changelog

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

