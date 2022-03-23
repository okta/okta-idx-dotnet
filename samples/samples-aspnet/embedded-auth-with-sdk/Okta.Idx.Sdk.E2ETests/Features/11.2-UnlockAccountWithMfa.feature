Feature: 11.2.1 - Unlock Account with TOTP and SMS

Background: 
    Given a SPA, WEB APP or MOBILE Sign On Policy that defines Password as required
    And Password Policy is set to Lock out user after 1 unsuccessful attempt
    And the Password Policy Rule "Users can perform self-service" has "Unlock Account" checked
    And the Password Policy Rule "Users can initiate Recovery with" has "Phone" and "Email" checked
    And the Password Policy Rule "Additional Verification is" has "Not Required" checked
    And a User named "Mary" exists, and this user has already setup email, phone and password factors
    #And Mary has entered an incorrect password to trigger an account lockout


Scenario: Mary recovers from a locked account with Email OTP
	Given Mary navigates to the Basic Login View
    And she has inserted her username
	When she fills in her incorrect password
    And she submits the Login form
    Then she should see the message "Authentication failed"
    #When she sees a link to unlock her account 
    When she clicks the link to unlock her account
    Then she sees a page to input her user name and select Email, Phone, or Okta Verify to unlock her account
    When she inputs her email
    #And she selects Email to unlock her account
    And She selects Phone from the list
	Then she is presented with an option to select SMS to verify and unlock her account
	When She selects SMS from the list
	And She selects "Receive a Code"
	Then the screen changes to receive an input for a code
	When She inputs the correct code from the SMS
	And She selects "Verify"
    Then she is redirected to the Basic Login View
    And she should see the terminal message "Account Successfully Unlocked!"
    