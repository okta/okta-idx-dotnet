using System;
using System.Threading.Tasks;
using Okta.Idx.Sdk.E2ETests.Helpers.A18NClient;
using Okta.Idx.Sdk.E2ETests.Helpers.A18NClient.Dto;

namespace Okta.Idx.Sdk.E2ETests.Helpers
{
    public class TestUserHelper : ITestUserHelper, IDisposable
    {
        private readonly IA18nClient _a18nClient;
        private readonly ITestConfig _configuration;
        private readonly A18nProfile _a18nProfile;
        private readonly IOktaSdkHelper _oktaHelper;
        private bool _disposed = false;
        private const int MaxAttempts = 30; // with delay 1000 ms between attempts, it's 30 seconds of total waiting time 

        private readonly string[] messageCodeMarkers = new[]
        {
            "Can't use the link? Enter a code instead: <b>",
            "To verify manually, enter this code: <b>",
            "Your verification code is "
        };


        public TestUserHelper(ITestConfig configuration, IA18nClient a18nClient, IOktaSdkHelper oktaHelper)
        {
            _oktaHelper = oktaHelper;
            _a18nClient = a18nClient;
            _configuration = configuration;

            _a18nProfile = _a18nClient.GetProfileAsync().Result;
        }

        public async Task<TestUserProperties> GetActivePasswordUserAsync(string firstName)
        {
            await CleanUpAsync();

            var oktaUser = await _oktaHelper.CreateActiveUserIdentifiedWithPasswordAsync(_a18nProfile.EmailAddress, firstName, _configuration.UserPassword);
            
            return new TestUserProperties()
            {
                FirstName = firstName,
                LastName = "Lastname",
                Email = oktaUser.Profile.Email,
                PhoneNumber = oktaUser.Profile.PrimaryPhone,
                Password = _configuration.UserPassword,
            };
        }

        public async Task<TestUserProperties> GetUnenrolledUserAsync(string firstName)
        {
            await CleanUpAsync();            
            return new TestUserProperties()
            {
                FirstName = firstName,
                LastName = "Lastname",
                Email = _a18nProfile.EmailAddress,
                PhoneNumber = _a18nProfile.PhoneNumber,
                Password = _configuration.UserPassword,
            };
        }

        public async Task<TestUserProperties> GetActivePasswordAndEmailUserAsync(string firstName)
        {
            await CleanUpAsync();

            var oktaUser = await _oktaHelper.CreateActiveUserAndAddToGroupAsync(_a18nProfile.EmailAddress, firstName, _configuration.UserPassword, _configuration.MfaRequiredGroupName);

            return new TestUserProperties()
            {
                FirstName = firstName,
                LastName = "Lastname",
                Email = oktaUser.Profile.Email,
                PhoneNumber = oktaUser.Profile.PrimaryPhone,
                Password = _configuration.UserPassword,
            };
        }

        public async Task<string> GetRecoveryCodeFromEmail()
        {            
            return await GetRecoveryCodeFromMessage(() => _a18nClient.GetLastMessagePlainContentAsync());
        }

        public async Task<string> GetRecoveryCodeFromSms()
        {
            return await GetRecoveryCodeFromMessage(()=>_a18nClient.GetLastSmsPlainContentAsync());
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private async Task<string> GetRecoveryCodeFromMessage(Func<Task<string>> getMessageBodyFunc)
        {
            for (int tries = 0; tries < MaxAttempts; tries++)
            {
                try
                {
                    var messageBody = await getMessageBodyFunc();

                    return ExtractRecoveryCodeFromMessage(messageBody);
                }
                catch (NotFoundException)
                {
                    // expected exception when a mail box is empty
                }
                await Task.Delay(1000);
            }

            return string.Empty;
        }


        private string ExtractRecoveryCodeFromMessage(string text)
        {
            var recoveryCode = string.Empty;
            if (!string.IsNullOrEmpty(text))
            {
                foreach (var marker in messageCodeMarkers)
                {
                    var pos = text.IndexOf(marker);
                    if (pos >= 0)
                    {
                        recoveryCode = text.Substring(pos + marker.Length, 6);
                    }
                }
            }
            return recoveryCode;
        }

        private async Task CleanUpA18ProfileAsync()
        {
            await _a18nClient.DeleteAllProfileEmailsAsync();
            await _a18nClient.DeleteAllProfileSmsAsync();
        }

        private async Task CleanUpOktaUserAsync()
        {
            await _oktaHelper.DeleteUserAsync(_a18nProfile.EmailAddress);
        }

        private async Task CleanUpAsync()
        {
            await CleanUpA18ProfileAsync();
            await CleanUpOktaUserAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                CleanUpOktaUserAsync().Wait();
            }
            _disposed = true;
        }
    }
}
