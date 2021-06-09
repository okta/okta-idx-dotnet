Feature: 0.1: Root page for Direct Auth Demo Application

  Background:

  Scenario: 0.1.3: Mary logs out of the app
    Given Mary has an authenticated session
    And Mary navigates to the Root View
    When Mary clicks the logout button
    Then she is redirected back to the Root View
    And Mary sees login, registration buttons
    And she does not see claims from /userinfo
