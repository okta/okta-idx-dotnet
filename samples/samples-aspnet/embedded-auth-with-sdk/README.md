# ASP.NET MVC & Embedded Authentication with the IDX SDK

## Introduction

> :grey_exclamation: The use of this Sample uses an SDK that requires usage of the Okta Identity Engine. 
This functionality is in general availability but is being gradually rolled out to customers. If you want
to request to gain access to the Okta Identity Engine, please reach out to your account manager. If you 
do not have an account manager, please reach out to oie@okta.com for more information.

This Sample Application will show you the best practices for integrating Authentication into your app
using [Okta's Identity Engine](https://developer.okta.com/docs/concepts/ie-intro/). Specifically, this 
application will cover some basic needed use cases to get you up and running quickly with Okta.
These Examples are:
1. Sign In
2. Sign Out
3. Sign Up
4. Sign In/Sign Up with Social Identity Providers
5. Sign In with Multifactor Authentication using Email or Phone

For information and guides on how to build your app with this sample, please take a look at the [.NET 
guides for Embedded Authentication](TBD)

## Prerequisites

Before running this sample, you will need the following:

* An Okta Developer Account, you can sign up for one at https://developer.okta.com/signup/.
* An Okta Application, configured for Web mode. This is done from the Okta Developer Console and you can find instructions [here][OIDC Web Application Setup Instructions].  When following the wizard, use the default properties.  They are designed to work with our sample applications.


## Installation & Running The App

Clone this repo and add your Okta configuration by following the [IDX SDK Configuration Reference](../../../README.md#configuration-reference) 

> Note: This application assumes you have your configuration in an okta.yaml file located in a .okta folder in the application or project's root directory. 
> The `IdxClient` , which is provided to the application via DI in the `App_Start > UnityConfig.cs` file, grabs the required configuration from the yaml file.

Now start your server and navigate to https://localhost:44314 in your browser.

If you see a home page that allows you to login, then things are working! 

You can login with the same account that you created when signing up for your Developer Org, or you can use a known username and password from your Okta Directory.

To see some examples for use cases using this sample application, please take a look at the [.NET guides
for Embedded Authentication](TBD)

[OIDC Web Application Setup Instructions]: https://developer.okta.com/authentication-guide/implementing-authentication/auth-code#1-setting-up-your-application
