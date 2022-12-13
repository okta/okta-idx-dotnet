Feature: 1.2 Login With Identifier First

This is basic login scenario that includes password optional

Background: 
	Given a Global Session Policy defines the Primary factor as Password / IDP / any factor allowed by app sign on rules
	And the Global Session Policy does NOT require a second factor
	And a SPA, WEB APP or MOBILE with an Authentication Policy that is defined as Any 1 factor
	And User Enumeration Prevention is set to ENABLED in Security > General
	And the list of Authenticators contains Email and Password is optional
	And a User named "Mary" exists, and this user has already setup email and password factors

 Scenario: 1.2.1: Mary logs in with Email with an OTP
	Given Mary navigates to the Basic Login View
	When she fills in her correct username
	And she clicks Login
	Then she is presented with an option to select Email to verify
	When She selects Email from the list
	And She selects "Receive a Code"
	Then the screen changes to receive an input for a code
	When She inputs the correct code from the Email
    And She selects "Verify"
    Then she is redirected to the Root View
    And she sees a table with her profile info
    And the cell for the value of email is shown and contains her email
    And the cell for the value of name is shown and contains her first name and last name