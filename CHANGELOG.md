# Changelog

## v0.1.0-beta01

### Features

- Initial version with basic functionality:
* `InteractAsync()`
* `IntrospectAsync()`
* `CancelAsync()`
* `RemediationOption.ProceedAsync()`
* `IdxResponse.SuccessWithInteractionCode.ExchangeCode()`

## v0.1.0-beta02

### Features

* Add support for password recovery.
* Update the client to allow for continuing a flow from any point
* Move the creation of the PKCE codeVerifier into `InteractAsync()` instead of the client constructor

### Breaking changes

* `InteractAsync` now returns an `IdxContext`
* Remove accessor of the `IdxContext` from the client
* `ExchangeCodeAsync()` now requires an `IdxContext` to exchange tokens

### Additions

New models: `AuthenticatorEnrollment`, `AuthenticatorEnrollmentValue`, `Recover` and `AuthenticatorEnrollmentMethod`.

