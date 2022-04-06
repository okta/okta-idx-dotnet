Feature: 10.1: TOTP Support (Google Authenticator) part 2

  Background:
    Given configured Authenticators are Password (required), and Google Authenticator (required)
    And a user named "Mary"
    And Mary does not have an account in the org

  @ignore
  Scenario: 10.1.4: Mary signs up for an account with Password, setups up required Google Authenticator by scanning a QR Code
    Given Mary navigates to the Self Service Registration View
    When she fills out her First Name
    And she fills out her Last Name
    And she fills out her Email
    And she submits the registration form
    Then she sees the Select Authenticator page with password as the only option
    When she chooses password factor option
    And she submits the select authenticator form
    Then she sees the set new password form
    When she fills out her Password
    And she confirms her Password
    And she submits the change password form
    Then she sees a list of required factors to setup

	When she selects Email
	Then she sees a page to input a code
	When she inputs the correct code from her email

    When She selects Google Authenticator from the list
    And She scans a QR Code
    And She selects Next
    Then the screen changes to receive an input for a TOTP code
    When She inputs the correct code from her Google Authenticator App
    And She selects Verify
    Then she sees the list of optional factors
    #When She selects Email from the list
    #Then she sees a page to input a code
    #When she inputs the correct code from her email
    #Then she sees the list of optional factors
    When she selects Skip
    Then she is redirected to the Root View
    And she sees a table with her profile info
    And the cell for the value of email is shown and contains her email
    And the cell for the value of name is shown and contains her first name and last name
  
  @ignore
  Scenario: 10.1.5: Mary signs up for an account with Password, setups up required Google Authenticator by entering a shared secret
    Given Mary navigates to the Self Service Registration View
    When she fills out her First Name
    And she fills out her Last Name
    And she fills out her Email
    And she submits the registration form
    Then she sees the Select Authenticator page with password as the only option
    When she chooses password factor option
    And she submits the select authenticator form
    Then she sees the set new password form
    When she fills out her Password
    And she confirms her Password
    And she submits the set new password form
    Then she sees a list of required factors to setup



	When she selects Email
	Then she sees a page to input a code
	When she inputs the correct code from her email



    When She selects Google Authenticator from the list
    And She enters the shared Secret Key into the Google Authenticator App
    And She selects Next on the screen which is showing the QR code
    Then the screen changes to receive an input for a TOTP code
    When She inputs the correct code from her Google Authenticator App
    And She selects Verify
    Then she sees the list of optional factors
    #When She selects Email from the list
    #Then she sees a page to input a code
    #When she inputs the correct code from her email
    #Then she sees the list of optional factors
    When she selects Skip
    Then she is redirected to the Root View
    And she sees a table with her profile info
    And the cell for the value of email is shown and contains her email
    And the cell for the value of name is shown and contains her first name and last name
