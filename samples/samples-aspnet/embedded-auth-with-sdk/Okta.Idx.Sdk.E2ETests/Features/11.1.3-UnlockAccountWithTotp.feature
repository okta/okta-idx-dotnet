Feature: 11.1.3 - Unlock Account with TOTP

Background: 
    Given a SPA, WEB APP or MOBILE Sign On Policy that defines Password as required
    And Password Policy is set to Lock out user after 1 unsuccessful attempt
    And the Password Policy Rule "Users can perform self-service" has "Unlock Account" checked
    And the Password Policy Rule "Users can initiate Recovery with" has "Phone" and "Email" checked
    And the Password Policy Rule "Additional Verification is" has "Not Required" checked
    And a User named "Mary" exists, and this user has already setup email and password factors
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
    And she selects Email to unlock her account
    #Then she should see a screen telling her to "Verify with your email"
    Then she sees a page to input her code
    #And she should see an input box for a code to enter from the email
    When She inputs the correct code from the Email
    #When she enters the OTP from her email in the original tab
    #And she submits the verification form
    Then she is redirected to the Basic Login View
    And she should see the terminal message "Account Successfully Unlocked!"
    #Then she should see a terminal page that says "Account Successfully Unlocked!"
    #And she should see a link on the page to go back to the Basic Login View
