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
        private Queue<RequestInfo> _requestInfoQueue;

        public MockedQueueRequestExecutor(Queue<MockResponse> responsesQueue)
        {
            this._responsesQueue = responsesQueue;
            this._requestInfoQueue = new Queue<RequestInfo>();
        }
        public Task<HttpResponse<string>> GetAsync(string href, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponse<string>> PostAsync(string href, IEnumerable<KeyValuePair<string, string>> headers, string body, CancellationToken cancellationToken)
        {
            var response = this._responsesQueue.Dequeue();
            _requestInfoQueue.Enqueue(new RequestInfo { Href = href, Payload = body });

            return Task.FromResult(new HttpResponse<string>
                                       {
                                           StatusCode = response.StatusCode,
                                           Payload = response.Response,
                                           Headers = new[] { new KeyValuePair<string, IEnumerable<string>>(RequestHeaders.ContentType, new[] { "application/ion+json" }) } 
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

        public Queue<RequestInfo>  RequestInfoQueue
        {
            get
            {
                return _requestInfoQueue;
            }
        }

        public class RequestInfo
        {
            public string Href { get; set; }

            public string Payload { get; set; }

        }
    }
}
