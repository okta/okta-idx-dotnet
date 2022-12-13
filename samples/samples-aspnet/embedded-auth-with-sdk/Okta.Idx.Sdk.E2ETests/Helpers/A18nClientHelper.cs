using System;
using System.Threading.Tasks;
using embedded_auth_with_sdk.E2ETests.Helpers.A18NClient;
using embedded_auth_with_sdk.E2ETests.Helpers.A18NClient.Dto;


namespace embedded_auth_with_sdk.E2ETests.Helpers
{
    public class A18nClientHelper : IA18nClientHelper, IDisposable
    {
        private A18nClient _client;
        private A18nProfile _profile;
        private ITestConfiguration _configuration;
        private bool _disposed;

        public A18nClientHelper(ITestConfiguration configuration)
        {
            _configuration = configuration;
        }

        public A18nClient GetClient()
        {
            if (_client == null)
            {
                if (string.IsNullOrWhiteSpace(_configuration.A18nProfileId))
                {
                    _client = new A18nClient(_configuration.A18nApiKey,
                        createNewDefaultProfile: true,
                        _configuration.A18nProfileTag);
                }
                else
                {
                    _client = new A18nClient(_configuration.A18nApiKey, _configuration.A18nProfileId);
                }
                CleanUpProfile();
            }
            return _client;
        }

        public A18nProfile GetDefaultProfile()
        {
            if (_profile == null)
            {             
                _profile = Task.Run(() => GetClient().GetProfileAsync()).Result;
            }
            return _profile;
        }

        public void CleanUpProfile()
        {
            if (_client != null)
            {
                Task.Run(async () =>
                {
                    await _client.DeleteAllProfileEmailsAsync();
                    await _client.DeleteAllProfileSmsAsync();
                }).Wait();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_client != null)
                {
                    _client.Dispose();
                }
            }
            _disposed = true;
        }
    }
}
