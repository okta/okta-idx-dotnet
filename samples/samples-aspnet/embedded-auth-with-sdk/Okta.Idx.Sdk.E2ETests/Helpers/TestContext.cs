using System;
using System.IO;
using System.Threading.Tasks;
using embedded_auth_with_sdk.E2ETests.Drivers;
using OpenQA.Selenium;

namespace embedded_auth_with_sdk.E2ETests.Helpers
{
    public class TestContext : ITestContext, IDisposable
    {
        private readonly WebDriverDriver _webDriver;
        private readonly ITestConfiguration _configuration;
        private readonly IOktaSdkHelper _oktaHelper;
        private readonly IA18nClientHelper _ia18nHelper;
        private bool _disposed = false;
        private const int MaxAttempts = 61; // seconds
        private const string DefaultScreenshotFolder = "./screenshots";
        private readonly string _passwordToUse;

        private readonly string[] messageCodeMarkers = new[]
        {
            "Can't use the link? Enter a code instead: <b>",
            "To verify manually, enter this code: <b>",
            "Your verification code is "
        };

        public UserProfile UserProfile { get; private set; }

        public TestContext(ITestConfiguration configuration, IA18nClientHelper ia18nHelper, IOktaSdkHelper oktaHelper, WebDriverDriver webDriver)
        {
            _oktaHelper = oktaHelper;
            _ia18nHelper = ia18nHelper;
            _webDriver = webDriver;
            _configuration = configuration;
            _passwordToUse = $"{configuration.UserPassword}{Guid.NewGuid()}";
        }

        public async Task SetActivePasswordUserAsync(string firstName)
        {
            await CleanUpAsync();
            var a18nprofile = _ia18nHelper.GetDefaultProfile();
            var oktaUser = await _oktaHelper.CreateActiveUser(a18nprofile.EmailAddress, a18nprofile.PhoneNumber, firstName, _passwordToUse);
            
            UserProfile = new UserProfile()
            {
                FirstName = firstName,
                LastName = "Lastname",
                Email = oktaUser.Profile.Email,
                PhoneNumber = oktaUser.Profile.PrimaryPhone,
                Password = _passwordToUse,
            };
        }

        public async Task SetUnenrolledUserAsync(string firstName)
        {
            await CleanUpAsync();
            var a18nProfile = _ia18nHelper.GetDefaultProfile();

            UserProfile = new UserProfile()
            {
                FirstName = firstName,
                LastName = "Lastname",
                Email = a18nProfile.EmailAddress,
                PhoneNumber = a18nProfile.PhoneNumber,
                Password = _passwordToUse,
            };
        }

        public async Task SetActivePasswordAndEmailUserAsync(string firstName)
        {
            await CleanUpAsync();
            var a18nProfile = _ia18nHelper.GetDefaultProfile();

            var oktaUser = await _oktaHelper.CreateActiveUser(a18nProfile.EmailAddress, a18nProfile.PhoneNumber, firstName, _passwordToUse);
            await _oktaHelper.AddUserToGroup(oktaUser, _configuration.MfaRequiredGroup);

            UserProfile = new UserProfile()
            {
                FirstName = firstName,
                LastName = "Lastname",
                Email = oktaUser.Profile.Email,
                PhoneNumber = oktaUser.Profile.PrimaryPhone,
                Password = _passwordToUse,
            };
        }

        public async Task SetActivePasswordAndSmsUserAsync(string firstName)
        {
            await CleanUpAsync();
            var a18nProfile = _ia18nHelper.GetDefaultProfile();

            var oktaUser = await _oktaHelper.CreateActiveUser(a18nProfile.EmailAddress, a18nProfile.PhoneNumber, firstName, _passwordToUse);
            await _oktaHelper.AddUserToGroup(oktaUser, _configuration.MfaRequiredGroup);
            await _oktaHelper.AddUserToGroup(oktaUser, _configuration.PhoneEnrollmentRequiredGroup);

            UserProfile = new UserProfile()
            {
                FirstName = firstName,
                LastName = "Lastname",
                Email = oktaUser.Profile.Email,
                PhoneNumber = oktaUser.Profile.PrimaryPhone,
                Password = _passwordToUse,
            };
        }

        public void SetUnenrolledUserWithFacebookAccount(string firstName)
        {
            UserProfile = new UserProfile()
            {
                FirstName = firstName,
                LastName = "Lastname",
                Email = _configuration.FacebookUserEmail,
                Password = _configuration.FacebookUserPassword,
            };
        }

        public async Task EnrollPhoneAuthenticator()
        {
            var factor = await _oktaHelper.EnrollPhoneFactor(UserProfile.Email, UserProfile.PhoneNumber);
            var passCode = await GetRecoveryCodeFromSms();

            await _oktaHelper.ActivateFactor(factor, UserProfile.Email, passCode);
        }

        public async Task<string> GetRecoveryCodeFromEmail()
        {
            var a18nClient = _ia18nHelper.GetClient();

            var passCode = await GetRecoveryCodeFromMessage(() => a18nClient.GetLastMessagePlainContentAsync());
            if (string.IsNullOrEmpty(passCode))
            {
                throw new Exception("Cannot get an email recovery code");
            }
            await a18nClient.DeleteAllProfileEmailsAsync();
            return passCode;
        }

        public async Task<string> GetRecoveryCodeFromSms()
        {
            var a18nClient = _ia18nHelper.GetClient();

            var passCode = await GetRecoveryCodeFromMessage(()=>a18nClient.GetLastSmsPlainContentAsync());
            if (string.IsNullOrEmpty(passCode))
            {
                throw new Exception("Cannot get an sms recovery code");
            }
            await a18nClient.DeleteAllProfileSmsAsync();
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

        private async Task CleanUpOktaUserAsync()
        {
            if (!string.IsNullOrEmpty(UserProfile?.Email))
            {
                await _oktaHelper.DeleteUserAsync(UserProfile.Email);
            }
        }

        private async Task CleanUpAsync()
        {
            _ia18nHelper.CleanUpProfile();
            await CleanUpOktaUserAsync();
        }

    }
}
