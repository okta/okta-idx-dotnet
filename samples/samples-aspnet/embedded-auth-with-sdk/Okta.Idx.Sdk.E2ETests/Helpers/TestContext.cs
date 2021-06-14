using System;
using System.IO;
using System.Threading.Tasks;
using embedded_auth_with_sdk.E2ETests.Drivers;
using embedded_auth_with_sdk.E2ETests.Helpers.A18NClient;
using embedded_auth_with_sdk.E2ETests.Helpers.A18NClient.Dto;
using OpenQA.Selenium;

namespace embedded_auth_with_sdk.E2ETests.Helpers
{
    public class TestContext : ITestContext, IDisposable
    {
        private readonly IA18nClient _a18nClient;
        private readonly WebDriverDriver _webDriver;
        private readonly ITestConfiguration _configuration;
        private readonly A18nProfile _a18nProfile;
        private readonly IOktaSdkHelper _oktaHelper;

        private bool _disposed = false;
        private const int MaxAttempts = 45; // with delay 1000 ms between attempts, it's 45 seconds of total waiting time 
        private const string DefaultScreenshotFolder = "./screenshots";

        private readonly string[] messageCodeMarkers = new[]
        {
            "Can't use the link? Enter a code instead: <b>",
            "To verify manually, enter this code: <b>",
            "Your verification code is "
        };

        public UserProfile UserProfile { get; private set; }

        public TestContext(ITestConfiguration configuration, IA18nClient a18nClient, IOktaSdkHelper oktaHelper, WebDriverDriver webDriver)
        {
            _oktaHelper = oktaHelper;
            _a18nClient = a18nClient;
            _webDriver = webDriver;
            _configuration = configuration;

            _a18nProfile = Task.Run(() => _a18nClient.GetProfileAsync()).Result;
        }

        public async Task SetActivePasswordUserAsync(string firstName)
        {
            await CleanUpAsync();

            var oktaUser = await _oktaHelper.CreateActiveUser(_a18nProfile.EmailAddress, _a18nProfile.PhoneNumber, firstName, _configuration.UserPassword);
            
            UserProfile = new UserProfile()
            {
                FirstName = firstName,
                LastName = "Lastname",
                Email = oktaUser.Profile.Email,
                PhoneNumber = oktaUser.Profile.PrimaryPhone,
                Password = _configuration.UserPassword,
            };
        }

        public async Task SetUnenrolledUserAsync(string firstName)
        {
            await CleanUpAsync();            
            UserProfile = new UserProfile()
            {
                FirstName = firstName,
                LastName = "Lastname",
                Email = _a18nProfile.EmailAddress,
                PhoneNumber = _a18nProfile.PhoneNumber,
                Password = _configuration.UserPassword,
            };
        }

        public async Task SetActivePasswordAndEmailUserAsync(string firstName)
        {
            await CleanUpAsync();

            var oktaUser = await _oktaHelper.CreateActiveUser(_a18nProfile.EmailAddress, _a18nProfile.PhoneNumber, firstName, _configuration.UserPassword);
            await _oktaHelper.AddUserToGroup(oktaUser, _configuration.MfaRequiredGroup);

            UserProfile = new UserProfile()
            {
                FirstName = firstName,
                LastName = "Lastname",
                Email = oktaUser.Profile.Email,
                PhoneNumber = oktaUser.Profile.PrimaryPhone,
                Password = _configuration.UserPassword,
            };
        }

        public async Task SetActivePasswordAndSmsUserAsync(string firstName)
        {
            await CleanUpAsync();

            var oktaUser = await _oktaHelper.CreateActiveUser(_a18nProfile.EmailAddress, _a18nProfile.PhoneNumber, firstName, _configuration.UserPassword);
            await _oktaHelper.AddUserToGroup(oktaUser, _configuration.MfaRequiredGroup);
            await _oktaHelper.AddUserToGroup(oktaUser, _configuration.PhoneEnrollmentRequiredGroup);

            UserProfile = new UserProfile()
            {
                FirstName = firstName,
                LastName = "Lastname",
                Email = oktaUser.Profile.Email,
                PhoneNumber = oktaUser.Profile.PrimaryPhone,
                Password = _configuration.UserPassword,
            };
        }

        public async Task EnrollPhoneAuthenticator()
        {
            var factor = await _oktaHelper.EnrollPhoneFactor(_a18nProfile.EmailAddress, _a18nProfile.PhoneNumber);
            var passCode = await GetRecoveryCodeFromSms();

            await _oktaHelper.ActivateFactor(factor, _a18nProfile.EmailAddress, passCode);
        }

        public async Task<string> GetRecoveryCodeFromEmail()
        {            
            var code = await GetRecoveryCodeFromMessage(() => _a18nClient.GetLastMessagePlainContentAsync());
            await _a18nClient.DeleteAllProfileEmailsAsync();
            return code;
        }

        public async Task<string> GetRecoveryCodeFromSms()
        {
            var passCode = await GetRecoveryCodeFromMessage(()=>_a18nClient.GetLastSmsPlainContentAsync());
            await _a18nClient.DeleteAllProfileSmsAsync();
            return passCode;
        }

        public void TakeScreenshot(string name)
        {
            var screenshotDriver = (ITakesScreenshot)_webDriver.WebDriver;
            var saveFolder = _configuration.ScreenshotsFolder;
            if (string.IsNullOrEmpty(saveFolder))
            {
                saveFolder = DefaultScreenshotFolder;
            }

            Screenshot screenShot = screenshotDriver.GetScreenshot();
            var allowedName = name.Replace(':', '-')
                .Replace(' ', '-')
                .Replace('"', '-');
            var fileName = $"{saveFolder}/{allowedName}.png";
            Directory.CreateDirectory(saveFolder);
            screenShot.SaveAsFile(fileName, ScreenshotImageFormat.Png);
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
                CleanUpOktaUserAsync().Wait();
            }
            _disposed = true;
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
                catch (A18NClient.NotFoundException)
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
    }
}
