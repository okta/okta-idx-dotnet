# ASP.NET Core & Embedded-Widget Login Page Example

## Introduction
> :grey_exclamation: The use of this Sample uses an SDK that requires usage of the Okta Identity Engine. 
This functionality is in general availability but is being gradually rolled out to customers. If you want
to request to gain access to the Okta Identity Engine, please reach out to your account manager. If you 
do not have an account manager, please reach out to oie@okta.com for more information.

This Sample Application will show you the best practices for integrating Authentication by embedding the 
Sign In Widget into your application. The Sign In Widget is powered by [Okta's Identity Engine](https://
developer.okta.com/docs/concepts/ie-intro/) and will adjust your user experience based on policies. 
Once integrated, you will be able to utilize all the features of Okta's Sign In Widget in your application.

For information and guides on how to build your app with this sample, please take a look at the [{FRAMEWORK} 
guides for Embedded Sign In Widget Authentication](link to DevDoc SBS guide)

## Installation & Running The App

### Prerequisites

Before running this sample, you will need the following:

* An Okta Developer Account, you can sign up for one at https://developer.okta.com/signup/.
* An Okta Application, configured for Web mode. This is done from the Okta Developer Console; find instructions [here][OIDC Web Application Setup Instructions].  When following the wizard, use the default properties.  They are designed to work with our sample applications.

### Clone the repository

Clone this repo and add your Okta configuration by following the [IDX SDK Configuration Reference](../../../README.md#configuration-reference) 

#### Run the web application from Visual Studio

When you run this project in Visual Studio it starts the web application on port 44314 using HTTPS. 

### Configure your application in the Okta Developer Console

Go to your [Okta Developer Console] and update the following parameters in your Okta Web Application configuration:
* **Login redirect URI** - `https://localhost:44314/interactioncode/callback`
* **Logout redirect URI** - `https://localhost:44314/account/signout`

The sample application defines an `InteractionCodeController` which receives the `interaction_code` upon successful login; review the `RedeemInteractionCodeAndSignInAsync` method of the `InteractionCodeController` for example code illustrating how to exchange the interaction code for Okta tokens.

### Enable CORS (Cross-Origin Resource Sharing)

Your application must be configured to allow your application to make requests to the Okta API using the Okta session cookie. To enable CORS for your application do the following:

- In your [Okta Developer Console], go to **Security > API > Trusted Origins** 
- Add your web application’s base URL `https://localhost:44314/` as a **Trusted Origin**.

For step-by-step instructions, visit the Okta [Sign Users in to Your Web Application guide] which shows how to sign users in using Okta and, [Sign Users Out guide] which shows how to sign users out of your application and out of Okta.

### Run your application and sign in

Click the **Sign In** link on the Home page and you are directed to the sign-in page.

Sign in using the same account you created when you signed up for your Developer Org, or you can use a known username and password from your Okta Directory.

**Note:** If you are currently using your Developer Console, you already have a Single Sign-On (SSO) session for your Org.  You will be automatically signed into your application as the same user that is using the Developer Console.  You may want to use an incognito tab to test the flow from a blank slate.
