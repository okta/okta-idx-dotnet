using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk.UnitTests
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        Dictionary<string, HttpResponseMessage> testResponses;
        Dictionary<string, int> callCounts;
        public MockHttpMessageHandler()
        {
            this.testResponses = new Dictionary<string, HttpResponseMessage>();
            this.callCounts = new Dictionary<string, int>();
        }

        public MockHttpMessageHandler(string response, HttpStatusCode httpStatusCode = HttpStatusCode.OK) : this()
        {
            this.DefaultResponse = new HttpResponseMessage(httpStatusCode) { Content = new StringContent(response) };            
        }

        public HttpResponseMessage DefaultResponse { get; set; }

        public Dictionary<string, int> CallCounts { get => callCounts; }

        public void AddTestResponse(string uriAbsolutePath, string response, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            testResponses.Add(uriAbsolutePath, new HttpResponseMessage(httpStatusCode) { Content = new StringContent(response) });
            callCounts.Add(uriAbsolutePath, 0);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if(testResponses.ContainsKey(request.RequestUri.AbsolutePath))
            {
                ++callCounts[request.RequestUri.AbsolutePath];
                return Task.FromResult(this.testResponses[request.RequestUri.AbsolutePath]);
            }
            return Task.FromResult(this.DefaultResponse);
        }
    }
}
