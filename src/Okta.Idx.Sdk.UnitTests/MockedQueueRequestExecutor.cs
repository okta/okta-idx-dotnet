using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk.UnitTests
{
    using System.Threading;
    using System.Threading.Tasks;

    using Okta.Sdk.Abstractions;

    public class MockedQueueRequestExecutor : IRequestExecutor
    {
        private Queue<MockResponse> _responsesQueue;
        public MockedQueueRequestExecutor(Queue<MockResponse> responsesQueue)
        {
            this._responsesQueue = responsesQueue;
        }
        public Task<HttpResponse<string>> GetAsync(string href, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponse<string>> PostAsync(string href, IEnumerable<KeyValuePair<string, string>> headers, string body, CancellationToken cancellationToken)
        {
            var response = this._responsesQueue.Dequeue();

            return Task.FromResult(new HttpResponse<string>
                                       {
                                           StatusCode = response.StatusCode,
                                           Payload = response.Response,
                                           Headers = new[] { new KeyValuePair<string, IEnumerable<string>>("Content-Type", new[] { "application/ion+json" }) } 
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
