// <copyright file="IdxResponseHelper.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Okta.Idx.Sdk.Helpers
{
    internal static class IdxResponseHelper
    {
        internal static IList<IAuthenticator> ConvertToAuthenticators(IList<IAuthenticatorValue> authenticators, IList<IAuthenticatorEnrollment> authenticatorEnrollments = null)
        {
            var authenticatorOptions = new List<IAuthenticator>();

            foreach (var authenticator in authenticators)
            {
                var enrollment = authenticatorEnrollments?.FirstOrDefault(x => x.Key == authenticator.Key);
                authenticatorOptions.Add(new Authenticator
                {
                    Id = authenticator.Id,
                    Name = authenticator.DisplayName,
                    MethodTypes = authenticator.Methods?.Select(x => x.Type).ToList(),
                    EnrollmentId = enrollment?.Id,
                    Profile = (enrollment != null) ? GetAuthenticatorProfile(enrollment) : string.Empty,
                });
            }

            return authenticatorOptions;
        }

        internal static IAuthenticator ConvertToAuthenticator(IList<IAuthenticatorValue> authenticators, IAuthenticatorEnrollment authenticatorEnrollment)
        => new Authenticator
        {
            Id = authenticators?.FirstOrDefault(x => x.Key == authenticatorEnrollment.Key)?.Id,
            Name = authenticatorEnrollment.DisplayName,
            MethodTypes = authenticatorEnrollment.Methods?.Select(x => x.Type).ToList(),
            EnrollmentId = authenticatorEnrollment.Id,
            Profile = GetAuthenticatorProfile(authenticatorEnrollment),
        };

        internal static string GetAuthenticatorProfile(IAuthenticatorEnrollment authenticatorEnrollment)
        {
            if (authenticatorEnrollment.Key == AuthenticatorType.Phone.ToIdxKeyString())
            {
                return authenticatorEnrollment.Profile?.GetProperty<string>("phoneNumber");
            }
            else if (authenticatorEnrollment.Key == AuthenticatorType.Email.ToIdxKeyString())
            {
                return authenticatorEnrollment.Profile?.GetProperty<string>("email");
            }

            return string.Empty;
        }
    }
}
