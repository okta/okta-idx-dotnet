# Dynamic Identity Engine View Rendering (OKTA-473696)
## DRAFT
This document is a work in progress.

## Abstract
This document describes **Okta Identity Engine** response structure and a strategy for rendering responses to accept input for the purpose of authentication. 

## Introducation
**Okta Identity Engine** hereinafter referred to as **OIE**, is an authentication API modeled as a [state machine](https://developer.mozilla.org/en-US/docs/Glossary/State_machine); this allows a consumer of the API to design a rendering loop that renders responses from **OIE** that is resilient to changes in [Sign-on policies](https://help.okta.com/en/prod/Content/Topics/Security/policies/policies-home.htm).  See [Sdk Client](#sdk-client).

## Terminology
This document uses common industry terminology as well as **OIE** specific terminology unique to this document and related works.

### Ion Spec Terminology
This document makes use of terminology defined in the ion spec to reference components of an **OIE** response, see section [1.1 Terminology](https://ionspec.org/#_terminology) of the Ion spec.  Common terminology used in this document follows:

- *Member* - A JSON name/value pair as defined in [RFC 7159](https://datatracker.ietf.org/doc/html/rfc7159#section-4)
- *Root Object* - The single JSON object at the root of an Ion content structure.
- *Value Object* - A JSON object with a member named `value`.
- *Collection Object* - A *Value Object* where the value member is a JSON array.  If a JSON value is an element in a *Collection Object's* value array, it is said that the Collection Object *contains* that value.

### Mv* Pattern Terminology
This document makes use of terminology commonly associated with *Model-View-* patterns such as [MVC](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller) and [Mvvm](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel).

- Model - A domain model that defines the structure of data for an application.
- View - The structure layout and appearance of what a user sees on the screen.  It displays a representation of the model.
- ViewModel - Loosely defined in this document as the relationship between the model and associated actions that a user may take to affect it.

### Okta Identity Engine Terminology
This section describes **OIE** specific terminology.

- **Remediation** - A high level concept in **OIE**.  Remediation can be thought of as an action one may take to *"remedy or 'set right' the unauthenticated state of the user"*.
- **Remediation collection** - The `remediation` [member](#ion-spec-terminology) of the [root object](#ion-spec-terminology) of an **OIE** response; it is an ion [collection object](#ion-spec-terminology).  See also, [OIE Response](#oie-response).
- **Remediation invocation** - An HTTP call made to an endpoint associated with a remediation.  See also, [Remediation Object](#remediation-object).
- **Remediation object** - A [value object](#ion-spec-terminology) contained in the `remediation` collection member of an **OIE** response.  Remediation objects provide the necessary information to make a `remediation invocation`.  See also, [Remediation Object](#remediation-object).
- **Remediation parameter** - A JSON object expected as an argument to a `remediation invocation`.  The structure of a `remediation parameter` is described by the dependent `remediation object`.  See also, [Remediation Object](#remediation-object).
- **Remediation parameter property descriptor** - A JSON object that describes a property of a `remediation parameter`, it may or may not be a [value object](#ion-spec-terminology).  See also, [Remediation Parameter](#remediation-parameter).
- **Authenticator** - A way to authenticate a user.
- **Authenticator enrollment** - An `authenticator` that a user has registered or "enrolled" as a way to authenticate.
- **State handle** - A value that a developer may use to reference a specific authentication session.

## OIE Response
An **OIE** response is the [root object](#ion-spec-terminology) and the [members](#ion-spec-terminology) it contains.  The following shows the initial [members](#ion-spec-terminology) of an **OIE Response**.

### Root Object
```json
{
    "version": "1.0.0",
    "stateHandle": "028Iw6C9XIrN1Pez_NZ6ulTaQmgBVr6R7WFxWr6E6E",
    "expiresAt": "2022-03-03T15:31:22.000Z",
    "intent": "LOGIN",
    "remediation": {
        "type": "array",
        "value": [
            ... see Remediation Object section
        ]
    },
    "currentAuthenticator": {
        "type": "object",
        "value": {
            ... see Authenticator Object section
        }
    },
    "cancel": {
        "rel": [
            "create-form"
        ],
        "name": "cancel",
        "href": "https://dotnet-sdk-idx.okta.com/idp/idx/cancel",
        "method": "POST",
        "produces": "application/ion+json; okta-version=1.0.0",
        "value": [
            {
                "name": "stateHandle",
                "required": true,
                "value": "028Iw6C9XIrN1Pez_NZ6ulTaQmgBVr6R7WFxWr6E6E",
                "visible": false,
                "mutable": false
            }
        ],
        "accepts": "application/json; okta-version=1.0.0"        
    }
}
```
The members included in the OIE response [root object](#ion-spec-terminology) change as the authentication flow progresses.  The primary member of concern for the purposes of authentication is `remediation`.  See also, [Remdediation Object](#remediation-object) and [Sdk Object Model](#sdk-object-model).

The [root object](#ion-spec-terminology) may contain the following members at different points during the authentciation flow:

- **remediation** - The `remediation` member is an ion [collection object](#ion-spec-terminology) that contains `remediation objects`.  See also [Remediation Object](#remediation-object).
- **currentAuthenticator** - The `currentAuthenticator` member provides additional information for the authenticator in the current context of the authentication session.
- **authenticators** - The `authenticators` member is an ion [collection object](#ion-spec-terminology) that contains objects that describe []().
- **authenticatorEnrollments** - The `authenticatorEnrollments` member is an ion [collection object](#ion-spec-terminology) that contains objects that describe [authenticator enrollments](#okta-identity-engine-terminology).

### Remediation Object
A `remediation object` is an ion [collection object](#ion-spec-terminology) that contains [remediation parameter property descriptors](#okta-identity-engine-terminology).  
```json
{
    "rel": [
        "create-form"
    ],
    "name": "identify",
    "href": "https://dotnet-sdk-idx.okta.com/idp/idx/identify",
    "method": "POST",
    "produces": "application/ion+json; okta-version=1.0.0",
    "value": [
        {
            "name": "identifier",
            "label": "Username",
            "required": true
        },
        {
            "name": "credentials",
            "type": "object",
            "form": {
                "value": [
                    {
                        "name": "passcode",
                        "label": "Password",
                        "secret": true
                    }
                ]
            },
            "required": true            
        },
        {
            "name": "rememberMe",
            "type": "boolean",
            "label": "Remember this device"
        },
        {
            "name": "stateHandle",
            "required": true,
            "value": "02rEajQcd651m1YehmCEOjfBkcpOBcEIJ2PZO6sSqJ",
            "visible": false,
            "mutable": false
        }
    ],
    "accepts": "application/json; okta-version=1.0.0"
}
```

### Remediation Parameter
To invoke the associated [remediation invocation](#okta-identity-engine-terminology) the JSON argument provided in the invocation request body must have [members](#ion-spec-terminology) as described by the [remediation parameter property descriptors](#okta-identity-engine-terminology) contained in the `remediation object`.  The following JSON may be used as an argument to the `remediation object` [previously defined](#remediation-object).  See also, [Sdk Object Model](#sdk-object-model):
```json
{
    "identifier": "username@domain.com",
    "credentials": {
        "passcode": "Password1234"
    },
    "rememberMe": false,
    "stateHandle": "02rEajQcd651m1YehmCEOjfBkcpOBcEIJ2PZO6sSqJ"
}
```

## SDK Client
Because members included in the OIE response [root object](#ion-spec-terminology) change as the authentication flow progresses, it isn't possible to define a static [class](https://en.wikipedia.org/wiki/Class_(computer_programming)) that represents it.  In order to reference the members of the OIE response [root object](#ion-spec-terminology) an Ion API is recommended.

- parsing strategy - why?
    - dynamic root object

### Ion Object Model
    - ion object model to reference root members

### Sdk Object Model


### View Rendering


