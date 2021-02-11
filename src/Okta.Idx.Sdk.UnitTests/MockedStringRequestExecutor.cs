using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk.UnitTests
{
    public class MockedStringRequestExecutor : IRequestExecutor
    {
        private readonly string _returnThis;
        private readonly int _statusCode;

        public string ReceivedHref { get; set; }

        public string ReceivedBody { get; set; }

        public string OktaDomain => throw new NotImplementedException();

        public MockedStringRequestExecutor(string returnThis, int statusCode = 200)
        {
            _returnThis = returnThis;
            _statusCode = statusCode;
        }

        public Task<HttpResponse<string>> GetAsync(string href, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken cancellationToken)
        {
            ReceivedHref = href;
            return Task.FromResult(new HttpResponse<string>
            {
                StatusCode = _statusCode,
                Payload = _returnThis,
            });
        }

        public Task<HttpResponse<string>> PostAsync(string href, IEnumerable<KeyValuePair<string, string>> headers, string body, CancellationToken cancellationToken)
        {
            ReceivedHref = href;
            ReceivedBody = body;
            return Task.FromResult(new HttpResponse<string>
            {
                StatusCode = _statusCode,
                Payload = _returnThis,
            });
        }

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
