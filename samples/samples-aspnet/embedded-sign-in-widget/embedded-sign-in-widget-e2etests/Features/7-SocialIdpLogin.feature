Feature: 7.1: Direct Auth with Self Hosted Sign In Widget Social Login with 1 Social IDP

  Background:
	Given a SPA, WEB APP or MOBILE Policy that defines Password as the only factor required for authentication
	And a configured IDP object for Google
	And an IDP routing rule defined to allow users in the Sample App to use the IDP
	And a user named Mary does not have an account in the org

  Scenario: 7.1.1: Mary Logs in with Social IDP
	Given Mary navigates to Login with Social IDP
	When she clicks the Login with Google button in the embedded Sign In Widget
	And logs in to Google
	Then she is redirected back to the Sample App
	And the Widget is reinitialized to re-enter the remediation flow to complete authentication
	And Mary is redirected back to the Root View