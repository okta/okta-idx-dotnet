Feature: 8.1 Basic Login with Embedded Sign In Widget

  Background:
	Given a SPA, WEB APP or MOBILE Sign On Policy that defines Password as required
	And the list of Authenticators contains Email and Password
	And a User named "Mary" exists, and this user has already setup email and password factors

  Scenario: 8.1.1 Mary logs in with a Password
	Given Mary navigates to the Embedded Widget View
	When she fills in her correct username
	And she fills in her correct password
	And she submits the Login form
	Then she is redirected to the Root View
    And the access_token is shown and not empty
    And the id_token is shown and not empty
    And the refresh_token is shown and not empty