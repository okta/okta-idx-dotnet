// <copyright file="MockedStringRequestExecutor.cs" company="Okta, Inc">
// Copyright (c) 2018 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.Sdk.Abstractions.UnitTests.Internal
{
    public class MockedStringRequestExecutor : IRequestExecutor
    {
        private readonly string _returnThis;
        private readonly int _statusCode;
        private readonly IEnumerable<KeyValuePair<string, IEnumerable<string>>> _headers;
        public string OktaDomain => throw new NotImplementedException();

        public MockedStringRequestExecutor(string returnThis, int statusCode = 200, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers = null)
        {
            _returnThis = returnThis;
            _statusCode = statusCode;
            _headers = headers;
        }

        public Task<HttpResponse<string>> GetAsync(string href, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponse<string>
            {
                StatusCode = _statusCode,
                Payload = _returnThis,
                Headers = _headers,
            });

        public Task<HttpResponse<string>> PostAsync(string href, IEnumerable<KeyValuePair<string, string>> headers, string body, CancellationToken cancellationToken)
        => Task.FromResult(new HttpResponse<string>
        {
            StatusCode = _statusCode,
            Payload = _returnThis,
            Headers = _headers,
        });

        public Task<HttpResponse<string>> PutAsync(string href, IEnumerable<KeyValuePair<string, string>> headers, string body, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponse<string>> DeleteAsync(string href, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
