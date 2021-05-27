using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace A18NAdapter
{
    public class A18nAdapter : IDisposable
    {
        private const string BaseUrl = "https://api.a18n.help";
        private const string RelativePart = "v1/profile";

        private string _apiKey;
        private HttpClient _client;

        public A18nAdapter(string apiKey)
        {
            _apiKey = apiKey;
            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
            _client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        }

        public void Dispose()
        {
            if (_client != null)
                _client.Dispose();
        }

        //  POST https://api.a18n.help/v1/profile
        public async Task<A18nProfile> CreateProfileAsync(CancellationToken cancellationToken = default)
        {
            var response = await _client.PostAsync(GetRelativeUri(), null, cancellationToken).ConfigureAwait(false);
            return await GetObjectFromResponseAsync<A18nProfile>(response);
        }

        // GET https://api.a18n.help/v1/profile
        public async Task<ActiveProfiles> GetActiveProfilesAsync(CancellationToken cancellationToken = default)
        {
            var response = await _client.GetAsync(GetRelativeUri(), cancellationToken).ConfigureAwait(false);
            return await GetObjectFromResponseAsync<ActiveProfiles>(response);
        }

        //Read information about a specific profile
        //GET https://api.a18n.help/v1/profile/:profileId
        public async Task<A18nProfile> GetProfileAsync(string profileId, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetAsync(GetRelativeUri(profileId), cancellationToken).ConfigureAwait(false);
            return await GetObjectFromResponseAsync<A18nProfile>(response);
        }

        // List of current emails
        //GET https://api.a18n.help/v1/profile/:profileId/email

        public async Task<A18nProfile> GetAllEmailsAsync(string profileId, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetAsync(GetRelativeUri(profileId), cancellationToken).ConfigureAwait(false);
            return await GetObjectFromResponseAsync<A18nProfile>(response);
        }



        private async Task<T> GetObjectFromResponseAsync<T>(HttpResponseMessage responseMessage)
        {
            EnsureResponseStatusOK(responseMessage);
            var json = await responseMessage.Content.ReadAsStringAsync();
            return JsonHelper.Deserialize<T>(json);
        }

        private void EnsureResponseStatusOK(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Unexpected status code: {response.StatusCode}");
            }
        }

        private string GetRelativeUri(string resource = default)
        {
            if (string.IsNullOrEmpty(resource))
            {
                return RelativePart;
            }
            else
            {
                return $"{RelativePart}/{resource}";
            }
        }
    }
}
