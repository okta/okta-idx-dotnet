// <copyright file="UnexpectedRemediationException.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>
using System;
using System.Linq;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An exception to be thrown when (an unexpected) message to user has been received and further remediation is not possible.
    /// </summary>
    public class TerminalStateException : OktaException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalStateException"/> class.
        /// </summary>
        /// <param name="idxTerminalMessages">Terminal state messages to User</param>
        public TerminalStateException(IIdxMessages idxTerminalMessages)
            : base(GetCombinedMessages(idxTerminalMessages))
        {
        }

        private static string GetCombinedMessages(IIdxMessages terminalMessages)
        {
            return string.Join(Environment.NewLine, terminalMessages.Messages.Select(m => m.Text));
        }
    }
}
