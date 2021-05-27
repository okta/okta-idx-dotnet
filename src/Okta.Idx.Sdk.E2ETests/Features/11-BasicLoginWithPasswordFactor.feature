Feature: 1.1 Basic Login with Password Factor
	As a user, Mary should be able to login into the app and access her profile

  Background:
	Given a SPA, WEB APP or MOBILE Sign On Policy that defines Password as required
	And the list of Authenticators contains Email and Password
	And a User named "Mary" exists, and this user has already setup email and password factors

  Scenario: 1.1.1 Mary logs in with a Password
	Given Mary navigates to the Basic Login View
	When she fills in her correct username
	And she fills in her correct password
	And she submits the Login form
    Then she is redirected to the Root View
#    And the access_token is stored in session
#    And the id_token is stored in session
#    And the refresh_token is stored in session
    And Mary should get logged-in
	
  Scenario: 1.1.2 Mary doesn't know her username
	Given Mary navigates to the Basic Login View
	When she fills in her incorrect username
	And she fills in her password
	And she submits the Login form
    Then she should see a message on the Login form "You do not have permission to perform the requested action."
	
  Scenario: 1.1.3: Mary doesn't know her password
	Given Mary navigates to the Basic Login View
	When she fills in her correct username
	And she fills in her incorrect password
	And she submits the Login form  with blank fields
    Then she should see the message "Authentication failed"
    
  Scenario: 1.1.8: Mary clicks on the "Forgot Password Link"
	Given Mary navigates to the Basic Login View
	When she clicks on the "Forgot Password Link"
	Then she is redirected to the Self Service Password Reset View

