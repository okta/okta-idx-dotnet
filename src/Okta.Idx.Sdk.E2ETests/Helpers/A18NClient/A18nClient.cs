using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Okta.Idx.Sdk.E2ETests.Helpers.A18NClient.Dto;

namespace Okta.Idx.Sdk.E2ETests.Helpers.A18NClient
{
    public class A18nClient : IA18nClient, IDisposable
    {
        private const string BaseUrl = "https://api.a18n.help";
        private const string RelativePart = "v1/profile";
        private bool _disposed = false;

        private HttpClient _client;
        private string _defaultProfileId;
        private string _createdProfileId;
        private bool _needDeleteProfile = false;

        public A18nClient(string apiKey, bool createNewDefaultProfile = false, string newProfileUniqueTag = default)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
            _client.DefaultRequestHeaders.Add("x-api-key", apiKey);

            if (createNewDefaultProfile)
            {
                if (!string.IsNullOrEmpty(newProfileUniqueTag))
                {
                    CleanUpOldProfiles(newProfileUniqueTag);
                }

                var newProfile = CreateProfileAsync(newProfileUniqueTag).Result;
                _createdProfileId = newProfile.ProfileId;
                SetDefaultProfileId(newProfile.ProfileId);
                _needDeleteProfile = true;
            }
        }

        public A18nClient(string apiKey, string profileId) : this(apiKey, false)
        {
            _defaultProfileId = profileId;
        }

        public void SetDefaultProfileId(string profileId)
        {
            _defaultProfileId = profileId;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        // POST https://api.a18n.help/v1/profile
        public async Task<A18nProfile> CreateProfileAsync(string profileTag = default, CancellationToken cancellationToken = default)
        {
            StringContent content = null;

            if (profileTag != default)
            {
                var requestBody = new
                {
                    displayName = profileTag
                };
                content = new StringContent(JsonHelper.Serialize(requestBody), Encoding.UTF8, "application/json");
            }
            return await PostAsync<A18nProfile>(default, content, cancellationToken);
        }

        // DELETE https://api.a18n.help/v1/profile/:profileId
        public async Task DeleteProfileAsync(string profileId = null, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            await DeleteAsync(effectiveProfileId, cancellationToken);
            if (_needDeleteProfile && effectiveProfileId == _createdProfileId)
            {
                _needDeleteProfile = false;
            }
        }

        // GET https://api.a18n.help/v1/profile
        public async Task<ProfileList> GetActiveProfilesAsync(CancellationToken cancellationToken = default)
        {
            return await GetAsync<ProfileList>(cancellationToken: cancellationToken);
        }

        //Read information about a specific profile
        //GET https://api.a18n.help/v1/profile/:profileId
        public async Task<A18nProfile> GetProfileAsync(string profileId = default, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            return await GetAsync<A18nProfile>(effectiveProfileId, cancellationToken);
        }

        // List of current emails
        //GET https://api.a18n.help/v1/profile/:profileId/email
        public async Task<EmailMessageList> GetAllProfileEmailsAsync(string profileId = default, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            return await GetAsync<EmailMessageList>($"{effectiveProfileId}/email", cancellationToken);
        }

        // Delete all emails
        // DELETE https://api.a18n.help/v1/profile/:profileId/email
        public async Task DeleteAllProfileEmailsAsync(string profileId = default, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            await DeleteAsync($"{effectiveProfileId}/email", cancellationToken);
        }

        // Load a specific email
        // GET https://api.a18n.help/v1/profile/:profileId/email/:messageId
        public async Task<EmailMessage> GetEmailMessageAsync(string messageId, string profileId = default, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            return await GetAsync<EmailMessage>($"{effectiveProfileId}/email/{messageId}", cancellationToken).ConfigureAwait(false);
        }

        // Delete a specific email
        // DELETE https://api.a18n.help/v1/profile/:profileId/email/:messageId
        public async Task DeleteEmailAsync(string messageId, string profileId = default, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            await DeleteAsync($"{effectiveProfileId}/email/{messageId}", cancellationToken);
        }

        // Get the plain-text email content
        // GET https://api.a18n.help/v1/profile/:profileId/email/:messageId/content
        public async Task<string> GetPlainMessageContentAsync(string messageId, string profileId = default, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            return await GetRawAsync($"{effectiveProfileId}/email/{messageId}/content", cancellationToken);
        }

        //Load the latest email message
        //GET https://api.a18n.help/v1/profile/:profileId/email/latest
        public async Task<EmailMessage> GetLatestEmailMessageAsync(string profileId = default, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            return await GetAsync<EmailMessage>($"{effectiveProfileId}/email/latest", cancellationToken).ConfigureAwait(false);
        }

        //Delete the latest email message
        //DELETE https://api.a18n.help/v1/profile/:profileId/email/latest
        public async Task DeleteLatestEmailAsync(string profileId = default, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            await DeleteAsync($"{effectiveProfileId}/email/latest", cancellationToken);
        }

        //Get the plain-text content for the latest email message
        //GET https://api.a18n.help/v1/profile/:profileId/email/latest/content
        public async Task<string> GetLastMessagePlainContentAsync(string profileId = default, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            return await GetRawAsync($"{effectiveProfileId}/email/latest/content", cancellationToken);
        }

        // List of current SMS messages
        // GET https://api.a18n.help/v1/profile/:profileId/sms
        public async Task<SmsMessageList> GetAllProfileSmsAsync(string profileId = default, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            return await GetAsync<SmsMessageList>($"{effectiveProfileId}/sms", cancellationToken);
        }

        // Delete all SMS messages
        // DELETE https://api.a18n.help/v1/profile/:profileId/sms
        public async Task DeleteAllProfileSmsAsync(string profileId = default, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            await DeleteAsync($"{effectiveProfileId}/sms", cancellationToken);
        }

        // Load a specific SMS message
        // GET https://api.a18n.help/v1/profile/:profileId/sms/:messageId
        public async Task<SmsMessage> GetSmsMessageAsync(string messageId, string profileId = default, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            return await GetAsync<SmsMessage>($"{effectiveProfileId}/sms/{messageId}", cancellationToken).ConfigureAwait(false);
        }

        // Delete a specific SMS message
        // DELETE https://api.a18n.help/v1/profile/:profileId/sms/:messageId
        public async Task DeleteSmsAsync(string messageId, string profileId = default, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            await DeleteAsync($"{effectiveProfileId}/sms/{messageId}", cancellationToken);
        }

        //Get the plain-text SMS message content
        //GET https://api.a18n.help/v1/profile/:profileId/sms/:messageId/content
        public async Task<string> GetPlainSmsContentAsync(string messageId, string profileId = default, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            return await GetRawAsync($"{effectiveProfileId}/sms/{messageId}/content", cancellationToken);
        }

        //Load the latest SMS message
        //GET https://api.a18n.help/v1/profile/:profileId/sms/latest
        public async Task<SmsMessage> GetLatestSmsAsync(string profileId = default, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            return await GetAsync<SmsMessage>($"{effectiveProfileId}/sms/latest", cancellationToken).ConfigureAwait(false);
        }

        //Delete the latest SMS message
        //DELETE https://api.a18n.help/v1/profile/:profileId/sms/latest
        public async Task DeleteLatestSmsAsync(string profileId = default, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            await DeleteAsync($"{effectiveProfileId}/sms/latest", cancellationToken);
        }

        // Get the plain-text content for the latest SMS message message
        // GET https://api.a18n.help/v1/profile/:profileId/sms/latest/content
        public async Task<string> GetLastSmsPlainContentAsync(string profileId = default, CancellationToken cancellationToken = default)
        {
            var effectiveProfileId = EffectiveProfileId(profileId);
            return await GetRawAsync($"{effectiveProfileId}/sms/latest/content", cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_needDeleteProfile)
                {
                    DeleteProfileAsync(profileId: _createdProfileId).Wait();
                }
                if (_client != null)
                    _client.Dispose();
            }
            _disposed = true;
        }

        #region request executors
        private async Task<T> GetAsync<T>(string uri = default, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetAsync(GetRelativeUri(uri), cancellationToken).ConfigureAwait(false);
            return await GetObjectFromResponseAsync<T>(response);
        }

        private async Task<string> GetRawAsync(string uri = default, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetAsync(GetRelativeUri(uri), cancellationToken).ConfigureAwait(false);
            EnsureResponseStatus(response);

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<T> PostAsync<T>(string uri, HttpContent content, CancellationToken cancellationToken = default)
        {
            var response = await _client.PostAsync(GetRelativeUri(uri), content, cancellationToken).ConfigureAwait(false);
            return await GetObjectFromResponseAsync<T>(response);
        }

        private async Task DeleteAsync(string uri = default, CancellationToken cancellationToken = default)
        {
            var response = await _client.DeleteAsync(GetRelativeUri(uri), cancellationToken).ConfigureAwait(false);
            EnsureResponseStatus(response, HttpStatusCode.NoContent);
        }
        #endregion request executors

        #region utility functions
        private string EffectiveProfileId(string profileId) => profileId == default ? _defaultProfileId : profileId;

        private async Task<T> GetObjectFromResponseAsync<T>(HttpResponseMessage responseMessage)
        {
            EnsureResponseStatus(responseMessage);
            var json = await responseMessage.Content.ReadAsStringAsync();
            return JsonHelper.Deserialize<T>(json);
        }

        private void EnsureResponseStatus(HttpResponseMessage response, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            if (response.StatusCode != httpStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new NotFoundException();
                }
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

        private void CleanUpOldProfiles(string newProfileUniqueTag)
        {
            var activeProfiles = GetActiveProfilesAsync().Result;

            foreach (var profile in activeProfiles.Profiles.Where(p => p.DisplayName == newProfileUniqueTag))
            {
                DeleteProfileAsync(profile.ProfileId).Wait();
            }
        }

        #endregion utility functions
    }
}
