﻿Feature: 4.2 Self Service Registration Password Optional

User registeration scenario for password optional feature

Background: 
	Given a Profile Enrollment policy defined assigning new users to the Everyone Group and by collecting "First Name", "Last Name", and "Email", is allowed and assigned to a SPA, WEB APP or MOBILE application
	And "Required before access is granted" is selected for Email Verification under Profile Enrollment in Security > Profile Enrollment
	And configured Authenticators are Email (required) and Password (optional)
	And a user named "Mary"
	And Mary does not have an account in the org

  @ignore
  Scenario: 4.2.1 Mary signs up for an account with required Email factor, then skips optional Password
	Given Mary navigates to the Self Service Registration View
	When she fills out her First Name
	And she fills out her Last Name
	And she fills out her Email
	And she submits the registration form
	Then She sees a list of factors
	When she selects Email
	And she submits the select authenticator form
	Then she sees a page to input a code
	When she inputs the correct code from her email
	Then she sees the list of optional factors
	When she skips optional authenticators if prompted
	Then she is redirected to the Root View
	And she sees a table with her profile info
	And the access_token is shown and not empty
	And the id_token is shown and not empty
	And the cell for the value of email is shown and contains her email
	And the cell for the value of name is shown and contains her first name and last name
	And the preferred_username claim is shown and matches Mary's email

  @ignore	
  Scenario: 4.2.2 Mary signs up for an account with required Email factor, then sets up optional Password
	Given Mary navigates to the Self Service Registration View
	When she fills out her First Name
	And she fills out her Last Name
	And she fills out her Email
	And she submits the registration form
	Then She sees a list of factors
	When she selects Email
	And she submits the select authenticator form
	Then she sees a page to input a code
	When she inputs the correct code from her email
	Then she sees the list of optional factors
	When she chooses password factor option
	And she submits the select authenticator form
	Then she sees the set new password form
	When she fills out her Password
	And she confirms her Password
	And she submits the change password form
	Then she sees the list of optional factors
	When she selects Skip
	Then she is redirected to the Root View
	And she sees a table with her profile info
	And the access_token is shown and not empty
	And the id_token is shown and not empty
	And the cell for the value of email is shown and contains her email
	And the cell for the value of name is shown and contains her first name and last name
	And the preferred_username claim is shown and matches Mary's email 
 