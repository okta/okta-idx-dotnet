Feature: 6.1: Multi-Factor Authentication with Password and Email

  Background:
	Given a SPA, WEB APP or MOBILE Policy that defines MFA with Password and Email as required
	And a User named "Mary" created in the admin interface with a Password only

  Scenario: 6.1.2: 2FA Login with Email
	Given Mary navigates to the Basic Login View
#
	And she has inserted her username
#

	And she has inserted her password

# ???
#	And her password is correct 
	When she clicks Login
	Then she is presented with an option to select Email to verify
	When She selects Email from the list

# not a real button
	And She selects "Receive a Code"
	Then the screen changes to receive an input for a code
	When She inputs the correct code from the Email
#	And She selects "Verify"
	Then she is redirected to the Root View
    And an application session is created

  Scenario: 6.1.3: Mary enters a wrong verification code
	Given Mary navigates to the Basic Login View
	When she fills in her username
	And she fills in her correct password
	And she clicks Login
	Then She sees a list of factors
	When She has selected Email from the list of factors
	And She inputs the incorrect code from the email
	Then the sample shows an error message "Invalid code. Try again." on the Sample App