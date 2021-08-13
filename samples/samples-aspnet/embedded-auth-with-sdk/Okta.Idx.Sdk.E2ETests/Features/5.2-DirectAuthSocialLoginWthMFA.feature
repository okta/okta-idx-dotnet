Feature: 5.2: Direct Auth Social Login with MFA
  
  Background:
	Given a SPA, WEB APP or MOBILE Policy that defines Password as the only factor required for authentication
	And a configured IDP object for Okta
	And an IDP routing rule defined to allow users in the Sample App to use the IDP
    And the Application Sign on Policy is set to "Password + Another Factor"
	And a user named Mary does not have an account in the org

  Scenario: 5.2.1: Mary logs in with a social IDP and gets an error message
	Given Mary navigates to the Basic Login View
	When she clicks the Login with Okta button
	And logs in to Okta
#	And the remediation returns "MFA_REQUIRED"
	Then Mary should see an error message "Multifactor Authentication and Social Identity Providers is not currently supported, Authentication failed." 