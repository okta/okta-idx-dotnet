using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// A component used to determine if the pipeline is in a password warn state.
    /// </summary>
    public interface IPasswordWarnStateResolver
    {
        /// <summary>
        /// Gets a value indicating whether the pipeline is in a password warn state.
        /// </summary>
        /// <param name="authenticationResponse">The response to anlyse.</param>
        /// <returns>bool.</returns>
        bool IsInPasswordWarnState(IIdxResponse authenticationResponse);
    }
}
