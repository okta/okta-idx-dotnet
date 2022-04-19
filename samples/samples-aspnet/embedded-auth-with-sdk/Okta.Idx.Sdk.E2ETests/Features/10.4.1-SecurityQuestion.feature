Feature: 10.4: Security Questions
  Background:
    Given a Profile Enrollment policy defined assigning new users to the Everyone Group and by collecting "First Name", "Last Name", and "Email", is allowed and assigned to a SPA, WEB APP or MOBILE application
    And "Required before access is granted" is selected for Email Verification under Profile Enrollment in Security > Profile Enrollment
    And configured Authenticators are Password (required), Email (required), and SMS (optional)
    And a user named "Mary"
    And Mary does not have an account in the org

  Scenario: 10.4.1: Mary signs up for an account and enrolls in Password and a predefined Security Question
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
    And she submits the registration form
    Then she sees a list of required factors to setup
    When she selects Email
    Then she sees a page to input a code
    When she inputs the correct code from her email
    Then she sees a list of factors to register
    When she selects Security Question
    Then she sees a screen to set up security question
    And she sees dropdown list of questions
    And she sees an input box to enter her answer
    When she enters Okta
    And submits the form
    And she skips optional authenticators if prompted
    Then she is redirected to the Root View
    And the access_token is shown and not empty
    And the id_token is shown and not empty
    And the preferred_username claim is shown and matches Mary's email
