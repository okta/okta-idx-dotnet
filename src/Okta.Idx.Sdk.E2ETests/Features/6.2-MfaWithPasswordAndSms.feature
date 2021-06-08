Feature: 6.2: Multi-Factor Authentication with Password and SMS

  Background:
	Given a SPA, WEB APP or MOBILE Policy that defines MFA with Password and SMS as required
	And an Authenticator Enrollment Policy that has PHONE as optional and EMAIL as required for the Everyone Group
	And a User named "Mary" created that HAS NOT yet enrolled in the SMS factor

  Scenario: 6.2.1: Enroll in SMS Factor prompt when authenticating
	Given Mary navigates to the Basic Login View
#added:
	And she has inserted her username

	And she has inserted her password
	And her password is correct
	When she clicks Login
	Then she is presented with a list of factors
#
	When She selects Phone from the list
	And She inputs a valid phone number
	And She selects "Receive a Code"
	Then the screen changes to receive an input for a code
	When She inputs the correct code from the SMS
	And She selects "Verify"
	Then she is redirected to the Root View
    And an application session is created
	
  Scenario: 6.2.2: 2FA Login with SMS
	Given Mary navigates to the Basic Login View
#added 
	And she has enrolled her phone already
#added:
	And she has inserted her username	
	And she has inserted her password
	And her password is correct
	When she clicks Login
	Then she is presented with an option to select Phone
	When she selects Phone
	Then she is presented with an option to select SMS to verify
	When She selects SMS from the list
	And She selects "Receive a Code"
	Then the screen changes to receive an input for a code
	When She inputs the correct code from the SMS
	And She selects "Verify"
	Then she is redirected to the Root View
    And an application session is created

  Scenario: 6.2.3: Enroll with Invalid Phone Number
	Given Mary navigates to the Basic Login View
#added:
	And she has inserted her username
	And she has inserted her password
	And her password is correct
	When she clicks Login
	Then she is presented with an option to select SMS to enroll
	When She selects Phone from the list
	And she inputs an invalid phone number
	#And She selects "Receive a Code"
	And submits the enrollment form
	Then she should see a message "Unable to initiate factor enrollment: Invalid Phone Number"

  Scenario: 6.2.4: Mary enters a wrong verification code on verify
	Given Mary navigates to the Basic Login View
#added 
	And she has enrolled her phone already
#added:
	And she has inserted her username	
	And she has inserted her password
	And her password is correct
	When she clicks Login
#
	Then she is presented with an option to select Phone
	When she selects Phone from the list
	And She selects "Receive a Code"
# two lines added
    Then she is presented with an option to select SMS to verify
	When She selects SMS from the list

	Then the screen changes to receive an input for a code
# was: When She inputs the incorrect code from the email
	When She inputs the incorrect code from the sms
	Then the sample show as error message "Invalid code. Try again." on the SMS Challenge page
	And she sees a field to re-enter another code
