Feature: 11-BasicLoginWithPasswordFactor
	As a user, Mary should be able to login into the app and access her profile

Background:
    Given Mary navigates to the login page

Scenario: 1.1.1: Mary logs in with a Password
    When she enters valid credentials
    And she submits the login form
    Then Mary should get logged-in
