using Okta.Sdk.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk
{
    public interface IRemediationOption : IResource
    {
        /// <summary>
        /// Gets the ION spec rel member based around the [form structure](https://ionspec.org/#form-structure) rules
        /// </summary>
        IList<string> Rel { get; }

        /// <summary>
        /// Gets the identifier for the remediation option
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the HTTP Method to use for this remediation option.
        /// </summary>
        string Method { get; }

        /// <summary>
        /// Gets the Href for the remediation option
        /// </summary>
        string Href { get; }

        /// <summary>
        /// Gets the Accepts Header for this remediation option.
        /// </summary>
        string Accepts { get; }

        /// <summary>
        /// Returns an object that is populated from the json path.  
        /// Example: `$.authenticatorEnrollments.value[0]` would relate to the jsonPath `OktaIdentityEngine->raw()->authenticatorEnrollments->value[0]`
        /// </summary>
        string RelatesTo { get; }

        /// <summary>
        /// Get all form values. This is generated from `$this->value`. Each item in `$this->value` MUST be mapped to a `FormValue` object.
        /// </summary>
        IList<IFormValue> Form { get; }

        /// <summary>
        /// Allows you to continue the remediation with this option.
        /// </summary>
        /// <param name="dataFromUi">The data returned from the enduser</param>
        /// <returns>An IdxResponse. </returns>
        Task<IIdxResponse> ProceedAsync(IdxRequestPayload dataFromUi, CancellationToken cancellationToken = default);
    }
}
