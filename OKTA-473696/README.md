# Dynamic Identity Engine View Rendering (OKTA-473696)
## DRAFT
This document is a work in progress.

## Abstract
This document describes **Okta Identity Engine** response structure and a strategy for rendering responses to accept input for the purpose of authentication. 

## Introducation
**Okta Identity Engine** hereinafter referred to as **OIE**, is an authentication API modeled as a [state machine](https://developer.mozilla.org/en-US/docs/Glossary/State_machine); this allows a consumer of the API to design a rendering loop, suitable to their application, that renders responses from **OIE** that is resilient to changes in [Sign-on policies](https://help.okta.com/en/prod/Content/Topics/Security/policies/policies-home.htm).  See [Parsing](#parsing) and [View Rendering](#view-rendering).

## Terminology
This document uses common industry terminology as well as **OIE** specific terminology unique to this document and related works.

### Ion Spec Terminology
This document makes use of terminology defined in the ion spec to reference components of an **OIE** response, see section [1.1 Terminology](https://ionspec.org/#_terminology) of the Ion spec.  Common terminology used in this document follow:

- *Member* - A JSON name/value pair as defined in [RFC 7159](https://datatracker.ietf.org/doc/html/rfc7159#section-4)
- *Root Object* - The single JSON object at the root of an Ion content structure.
- *Value Object* - A JSON object with a member named `value`.
- *Collection Object* - A *Value Object* where the value member is a JSON array.  If a JSON value is an element in a *Collection Object's* value array, it is said that the Collection Object *contains* that value.

### Mv* Pattern Terminology
This document uses terminology commonly associated with *Model-View-* patterns such as [MVC](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller) and [Mvvm](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel).

- Model - A domain model that defines the structure of data for an application.
- View - The structure layout and appearnce of what a user sees on the screen.  It displays a representation of the model.
- ViewModel - Loosely used in this document as the relationship between the model and associated actions that a user may take to affect it.

### Okta Identity Engine Terminology
This section describes **OIE** specific terminology.

- Remediation - A high level concept in **OIE**.  Remediation can be thought of as an action one may take to *"remedy or 'set right' the unauthenticated state of the user"*.
- Remediation collection - The `remediation` member of the root object of an **OIE** response; it is an ion [collection object](https://ionspec.org/#_terminology).  See also, [OIE Response](#oie-response).
- Remediation object - A [value object](https://ionspec.org/#_terminology) contained in the `remediation` collection member of an **OIE** response.  Remediation objects describe a call that may be made to invoke the associated remediation.  See also, [OIE Response](#oie-response).
- Remediation option - A JSON object provided as an argument to the invocation of a remediation.  The structure of a remediation option is described by the dependent remediation.
- Remediation form - A visual depiction of a *remediation object*, most commonly a rendered html form.
- Authenticator - A way to authenticate a user.
- Authenticator object - A [value object](https://ionspec.org/#_terminology) contained in the `authenticators` collection member of an **OIE** response.  Authenticator objects describe authenticators.  See also, [OIE Response](#oie-response).
- Authenticator enrollment - An authenticator that a user has registered or "enrolled" as a way to authenticate.
- Authenticator enrollment object - A [value object](https://ionspec.org/#_terminology) contained in the `authenticatorEnrollments` collection member of an **OIE** response.  Authenticator enrollment objects describe authenticator enrollments.  See also, [OIE Response](#oie-response).
- State handle - A value that a developer may use to reference a specific authentication session.

## OIE Response
An **OIE** response is the [root object](https://ionspec.org/#_terminology) and the members it contains.  The following provides examples of the structure of an **OIE** response.  For the purposes of authentication a developer is primarily intersted in the `remediation` member, see [Parsing](#parsing) and [View Rendering](#view-rendering).

### Root Object
```csharp
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
    "authenticators": {
        "type": "array",
        "value": [
            ... see Authenticator Object section
        ]
    },
    "authenticatorEnrollments": {
        "type": "array",
        "value": [
            .. see Authenticator Enrollments Object section
        ]
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

The root object may contain the following members:

- `remediation` - This member is an ion *collection object* containing *remediation objects*.  See also [Remediation Object](#remediation-object).
- `authenticators` - This member is an ion *collection object* containing *authenticator objects*.  See also [Authenticator Object](#authenticator-object).
- `authenticatorEnrollments` - 

### Remediation Object

### Authenticator Object

### Authenticator Enrollments

## Parsing

## View Rendering


