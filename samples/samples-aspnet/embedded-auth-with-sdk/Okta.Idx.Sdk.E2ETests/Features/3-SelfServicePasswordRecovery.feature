Feature: 3.1: Direct Auth Password Recovery

  Background:
	Given an org with an ORG Policy that defines Authenticators with Password and Email as required
	And a user named "Mary"
	And Mary is a user with a verified email and a set password

  Scenario: 3.1.1 Mary resets her password
	Given Mary navigates to the Self Service Password Reset View
	When she inputs her correct Email
	And she submits the recovery form
###	
	Then she sees a page to select an authenticator
	When she chooses Email
	And she submits the select form
###	
	Then she sees a page to input her code
	When she fills in the correct code
	And she submits the verification form
	Then she sees a page to set her password
	When she fills a password that fits within the password policy
	And she confirms that password
	And she submits the change password form
	Then she is redirected to the Root Page is provided
	
  Scenario: 3.1.2 Mary tries to reset a password with the wrong email
	Given Mary navigates to the Self Service Password Reset View
#	When she selects "Forgot Password"
	Then she sees the Password Recovery Page
	When she inputs an Email that doesn't exist
	And she submits the recovery form
	Then she sees a message "There is no account with the Username {username}." 