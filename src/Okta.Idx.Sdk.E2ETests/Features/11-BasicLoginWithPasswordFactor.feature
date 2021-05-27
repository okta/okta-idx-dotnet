Feature: 11-BasicLoginWithPasswordFactor
	As a user, Mary should be able to login into the app and access her profile

Background:
    Given Mary navigates to the Basic Login View

Scenario: 1.1.1: Mary logs in with a Password
    When she enters correct credentials
    And she submits the Login form
    Then Mary should get logged-in

Scenario: 1.1.2: Mary doesn't know her username
	Given Mary navigates to the Basic Login View
	When she fills in her incorrect username with password
	And she submits the Login form
	Then she should see a message on the Login form "You do not have permission to perform the requested action."
	
Scenario: 1.1.3: Mary doesn't know her password
	Given Mary navigates to the Basic Login View
	When she fills in her correct username and incorrect password
	And she submits the Login form  with blank fields
	Then she should see the message "Authentication failed"
   
Scenario: 1.1.8: Mary clicks on the "Forgot Password Link"
	Given Mary navigates to the Basic Login View
	When she clicks on the "Forgot Password Link"
	Then she is redirected to the Self Service Password Reset View
