// <copyright file="OktaVerifyEnrollOptions.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Okta.Idx.Sdk.Helpers;

namespace Okta.Idx.Sdk.OktaVerify
{
    /// <summary>
    /// A class that contains the options for enrollment in Okta Verify.
    /// </summary>
    public class OktaVerifyEnrollOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OktaVerifyEnrollOptions"/> class.
        /// </summary>
        public OktaVerifyEnrollOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OktaVerifyEnrollOptions"/> class.
        /// </summary>
        /// <param name="authenticationResponse">The authentication response.</param>
        /// <param name="idxResponse">The Idx response.</param>
        public OktaVerifyEnrollOptions(AuthenticationResponse authenticationResponse, IIdxResponse idxResponse)
        {
            AuthenticationResponse = authenticationResponse;
            IdxContext = authenticationResponse.IdxContext;
            StateHandle = idxResponse.StateHandle;
            EnrollPollRemediationOption = idxResponse.FindRemediationOption(RemediationType.EnrollPoll);
            SelectEnrollmentChannelRemediationOption = idxResponse.FindRemediationOption(RemediationType.SelectEnrollmentChannel);
            CurrentAuthenticator = idxResponse.GetProperty<IIdxResponse>("currentAuthenticator").GetProperty<OktaVerifyAuthenticatorValue>("value");
        }

        /// <summary>
        /// Gets or sets the authentication response.
        /// </summary>
        protected internal AuthenticationResponse AuthenticationResponse { get; set; }

        /// <summary>
        /// Gets the current authenticator.
        /// </summary>
        protected internal OktaVerifyAuthenticatorValue CurrentAuthenticator { get; }

        /// <summary>
        /// Gets or sets the Idx context.
        /// </summary>
        protected internal IIdxContext IdxContext { get; set; }

        /// <summary>
        /// Gets the enroll-poll remediation option.
        /// </summary>
        protected internal IRemediationOption EnrollPollRemediationOption { get; }

        /// <summary>
        /// Gets the select-enrollment-channel remediation option.
        /// </summary>
        protected internal IRemediationOption SelectEnrollmentChannelRemediationOption { get; internal set; }

        /// <summary>
        /// Gets the enrollment-channel-data remediation option.
        /// </summary>
        protected internal IRemediationOption EnrollmentChannelDataRemediationOption { get; private set; }

        /// <summary>
        /// Gets or sets the state handle.
        /// </summary>
        public string StateHandle { get; set; }

        /// <summary>
        /// Gets a value indicating how long to wait before the next call.
        /// </summary>
        public int? Refresh { get => EnrollPollRemediationOption?.Refresh; }

        /// <summary>
        /// Gets the Qr code.
        /// </summary>
        public string QrCode { get => CurrentAuthenticator?.ContextualData?.QrCode?.Href; }

        /// <summary>
        /// Gets the Id of Okta Verify authenticator.
        /// </summary>
        public string AuthenticatorId
        {
            get
            {
                return SelectEnrollmentChannelRemediationOption?
                  .GetArrayProperty<IIdxResponse>("value")[0]
                  .GetProperty<IIdxResponse>("value")
                  .GetProperty<IIdxResponse>("form")
                  .GetArrayProperty<IIdxResponse>("value")[0]
                  .GetProperty<string>("value");
            }
        }

        /// <summary>
        /// Sends an Okta Verify enrollment link to the specified email.
        /// </summary>
        /// <param name="email">The email</param>
        /// <returns>The authentication response.</returns>
        public async Task<AuthenticationResponse> SendLinkToEmailAsync(string email)
        {
            IdxRequestPayload idxRequestPayload = new IdxRequestPayload
            {
                StateHandle = StateHandle,
            };
            idxRequestPayload.SetProperty("email", email);

            return await SendEnrollmentChannelDataAsync(idxRequestPayload);
        }

        /// <summary>
        /// Sends an Okta Verify enrollment link via sms to the specified phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns>The authentication response.</returns>
        public async Task<AuthenticationResponse> SendLinkToPhoneNumberAsync(string phoneNumber)
        {
            IdxRequestPayload idxRequestPayload = new IdxRequestPayload
            {
                StateHandle = StateHandle,
            };
            idxRequestPayload.SetProperty("phoneNumber", phoneNumber);

            return await SendEnrollmentChannelDataAsync(idxRequestPayload);
        }

        /// <summary>
        /// Sends the specified payload to the enrollment channel data endpoint.
        /// </summary>
        /// <param name="idxRequestPayload">The payload.</param>
        /// <returns>The authentication response.</returns>
        protected async Task<AuthenticationResponse> SendEnrollmentChannelDataAsync(IdxRequestPayload idxRequestPayload)
        {
            var sendEnrollmentDataResponse = await EnrollmentChannelDataRemediationOption.ProceedAsync(idxRequestPayload);

            var authenticationResponse = new AuthenticationResponse
            {
                IdxContext = IdxContext,
                AuthenticationStatus = AuthenticationResponse.AuthenticationStatus,
                CurrentAuthenticator = IdxResponseHelper.ConvertToAuthenticator(sendEnrollmentDataResponse.Authenticators.Value, sendEnrollmentDataResponse.CurrentAuthenticator.Value),
            };

            return authenticationResponse;
        }

        /// <summary>
        /// Selects the specified enrollment channel.
        /// </summary>
        /// <param name="enrollmentChannelName">The enrollment channel.</param>
        /// <returns>The authentication response.</returns>
        public async Task<AuthenticationResponse> SelectEnrollmentChannelAsync(string enrollmentChannelName)
        {
            IdxRequestPayload idxRequestPayload = new IdxRequestPayload
            {
                StateHandle = StateHandle,
            };
            idxRequestPayload.SetProperty("authenticator", new { channel = enrollmentChannelName, id = AuthenticatorId });
            idxRequestPayload.SetProperty("stateHandle", StateHandle);

            var selectEnrollmentChannelResponse = await SelectEnrollmentChannelRemediationOption.ProceedAsync(idxRequestPayload);

            var authenticationResponse = new AuthenticationResponse
            {
                IdxContext = IdxContext,
                AuthenticationStatus = AuthenticationResponse.AuthenticationStatus,
                CurrentAuthenticator = IdxResponseHelper.ConvertToAuthenticator(selectEnrollmentChannelResponse.Authenticators.Value, selectEnrollmentChannelResponse.CurrentAuthenticator.Value),
            };

            EnrollmentChannelDataRemediationOption = selectEnrollmentChannelResponse.FindRemediationOption(RemediationType.EnrollmentChannelData);

            return authenticationResponse;
        }

        /// <summary>
        /// Gets available enrollment channel options.
        /// </summary>
        /// <returns>Parameters for channel selection.</returns>
        public IList<OktaVerifyRemediationParameter> GetChannelOptions()
        {
            return SelectEnrollmentChannelRemediationOption?
                .GetArrayProperty<IIdxResponse>("value")[0]
                .GetProperty<IIdxResponse>("value")
                .GetProperty<IIdxResponse>("form")
                .GetArrayProperty<IIdxResponse>("value")[1]
                .GetArrayProperty<OktaVerifyRemediationParameter>("options");
        }

        /// <summary>
        /// Sends a request to the enrollment polling endpoint.
        /// </summary>
        /// <returns>The poll response.</returns>
        /// <exception cref="ArgumentException">If the poll option is not valid.</exception>
        public async Task<PollResponse> PollOnceAsync()
        {
            if (EnrollPollRemediationOption.Name != RemediationType.EnrollPoll)
            {
                throw new ArgumentException($"Expected remediation option of type '{RemediationType.EnrollPoll}', the specified remediation option is of type {EnrollPollRemediationOption.Name}.");
            }

            IdxRequestPayload requestPayload = new IdxRequestPayload
            {
                StateHandle = StateHandle,
            };

            var enrollResponse = await EnrollPollRemediationOption.ProceedAsync(requestPayload);
            bool continuePolling = enrollResponse.ContainsRemediationOption(RemediationType.EnrollPoll, out IRemediationOption enrollPollRemediationOption);

            return new PollResponse
            {
                Refresh = enrollPollRemediationOption?.Refresh,
                ContinuePolling = continuePolling,
            };
        }
    }
}
