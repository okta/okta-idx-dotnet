Feature: Social Login with Okta Idp

 Background:
	Given a SPA, WEB APP or MOBILE Policy that defines Password as the only factor required for authentication
	And a configured IDP object for Okta Idp
	And an IDP routing rule defined to allow users in the Sample App to use the IDP
	And a user named Mary does not have an account in the org but has an Okta Idp account
  
  @ignore
  Scenario: Mary Logs in with Okta IDP
	Given Mary navigates to the Login View
	When she clicks the Login with Okta Idp button
	And logs into Okta Idp application
	Then she is redirected to the sign in widget view
    And she sees an option for email authenticator
