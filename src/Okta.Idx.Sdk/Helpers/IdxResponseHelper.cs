// <copyright file="IdxResponseHelper.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
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
                    CredentialId = string.Equals(authenticator.Key, AuthenticatorType.WebAuthn.ToString(), StringComparison.OrdinalIgnoreCase) ? enrollment?.CredentialId : null,
                });
            }

            return authenticatorOptions;
        }

        internal static IList<IAuthenticator> ConvertEnrollmentsToAuthenticators(IList<IAuthenticatorValue> authenticators, IList<IAuthenticatorEnrollment> authenticatorEnrollments)
        {
            var authenticatorOptions = new List<IAuthenticator>();

            if (authenticatorEnrollments != null)
            {
                foreach (var enrollment in authenticatorEnrollments)
                {
                    var authenticator = authenticators?.FirstOrDefault(x => x.Key == enrollment.Key);
                    authenticatorOptions.Add(new Authenticator
                    {
                        Id = authenticator?.Id,
                        Name = authenticator?.DisplayName,
                        MethodTypes = authenticator?.Methods?.Select(x => x.Type).ToList(),
                        EnrollmentId = enrollment?.Id,
                        Profile = (enrollment != null) ? GetAuthenticatorProfile(enrollment) : string.Empty,
                        CredentialId =
                            string.Equals(
                                authenticator?.Key,
                                AuthenticatorType.WebAuthn.ToString(),
                                StringComparison.OrdinalIgnoreCase)
                                ? enrollment?.CredentialId
                                : null,
                    });
                }
            }

            return authenticatorOptions;
        }

        internal static IAuthenticator ConvertToAuthenticator(IList<IAuthenticatorValue> authenticators, IAuthenticatorEnrollment authenticatorEnrollment)
        {
            return new Authenticator
            {
                Id = authenticators?.FirstOrDefault(x => x.Key == authenticatorEnrollment.Key)?.Id,
                Name = authenticatorEnrollment.DisplayName,
                MethodTypes = authenticatorEnrollment.Methods?.Select(x => x.Type).ToList(),
                EnrollmentId = authenticatorEnrollment.Id,
                Profile = GetAuthenticatorProfile(authenticatorEnrollment),
                ContextualData = authenticatorEnrollment.AuthenticatorContextualData,
            };
        }

        internal static IAuthenticator ConvertToAuthenticator(IIdxResponse idxResponse)
        {
            var authenticator = ConvertToAuthenticator(idxResponse.Authenticators.Value,
                idxResponse.CurrentAuthenticator.Value,
                idxResponse.AuthenticatorEnrollments.Value);

            // Okta Verify channel types
            authenticator.ChannelTypes = idxResponse.FindRemediationOption(RemediationType.SelectEnrollmentChannel)
                    .Form?
                    .FirstOrDefault(x => x.Name == "authenticator")?.GetProperty<IFormValue>("value")?
                    .Form?
                    .GetArrayProperty<IFormValue>("value")?
                    .FirstOrDefault(x => x.Name == "channel")?.Options
                    ?.Select(x => x.GetProperty<AuthenticatorChannelType>("value")).ToList();

            return authenticator;
        }

        internal static IAuthenticator ConvertToAuthenticator(
            IList<IAuthenticatorValue> authenticators,
            IAuthenticatorEnrollment authenticatorEnrollment,
            IList<IAuthenticatorEnrollment> authenticatorEnrollments)
        {
            var credentialId = string.Empty;

            // currentAuthenticatorEnrollment not present with webAuthn
            if (string.Equals(authenticatorEnrollment.Key, AuthenticatorType.WebAuthn.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                credentialId = authenticatorEnrollments?.FirstOrDefault(x =>
                    string.Equals(x.Key, AuthenticatorType.WebAuthn.ToString(), StringComparison.OrdinalIgnoreCase))?.CredentialId;
            }

            return new Authenticator
            {
                Id = authenticators?.FirstOrDefault(x => x.Key == authenticatorEnrollment?.Key)?.Id,
                Name = authenticatorEnrollment?.DisplayName,
                MethodTypes = authenticatorEnrollment?.Methods?.Select(x => x.Type).ToList(),
                EnrollmentId = authenticatorEnrollment?.Id,
                Profile = (authenticatorEnrollment != null) ? GetAuthenticatorProfile(authenticatorEnrollment) : string.Empty,
                ContextualData = authenticatorEnrollment?.AuthenticatorContextualData,
                CredentialId = credentialId,
            };
        }

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
