using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using embedded_auth_with_sdk.E2ETests.Drivers;
using Okta.Sdk;
using Okta.Sdk.Configuration;
using OpenQA.Selenium;
using OtpNet;
using ZXing;
using ZXing.Windows.Compatibility;

namespace embedded_auth_with_sdk.E2ETests.Helpers
{
    public class TestContext : ITestContext, IDisposable
    {
        private readonly WebDriverDriver _webDriver;
        private readonly ITestConfiguration _configuration;
        private readonly IOktaSdkHelper _oktaHelper;
        private readonly IA18nClientHelper _ia18nHelper;
        private readonly string _passwordToUse;
        private bool _disposed = false;
        private const int MaxRetries = 3; // when resend option is available
        private const int OneAttemptTime = 30; // seconds
        private const string DefaultScreenshotFolder = "./screenshots";
        private bool _keepOktaUser = false;
        private string _lastPassCode;
        private string _totpSharedSecret;
        private Func<Task> OnDispose { get; set; } = () => Task.CompletedTask;

        private readonly string[] messageCodeMarkers = new[]
        {
            "Can't use the link? Enter a code instead: <b>",
            "To verify manually, enter this code: <b>",
            "Your verification code is "
        };

        public UserProfile UserProfile { get; private set; }
        public string TotpSharedSecret
        {
            get => _totpSharedSecret;
            set
            {
                _lastPassCode = string.Empty;
                _totpSharedSecret = value;
            }
        }

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

        public async Task SetActiveUserRequiresTotpAsync(string firstName)
        {
            await CleanUpAsync();
            var a18nprofile = _ia18nHelper.GetDefaultProfile();
            var oktaUser = await _oktaHelper.CreateActiveUser(a18nprofile.EmailAddress, a18nprofile.PhoneNumber, firstName, _passwordToUse);
            await _oktaHelper.AddUserToGroup(oktaUser, "TOTP required");
         //   await _oktaHelper.AddUserToGroup(oktaUser, "MFA required");

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
        public async Task SetActiveUserWithOktaOidcIdpAccount(string firstName)
        {
            // Okta as OIDC IDP org
            var oktaClient = new OktaClient(
                new OktaClientConfiguration
                {
                    OktaDomain = _configuration.OktaOidcIdpDomain,
                    Token = _configuration.OktaOidcIdpToken,
                }); ;

            var oktaHelper = new OktaSdkHelper(oktaClient);

            await CleanUpAsync(oktaHelper);

            var a18nprofile = _ia18nHelper.GetDefaultProfile();
            var oktaUser = await oktaHelper.CreateActiveUser(a18nprofile.EmailAddress, a18nprofile.PhoneNumber, firstName, _passwordToUse);

            OnDispose = () => CleanUpOktaUserAsync(oktaHelper);

            UserProfile = new UserProfile()
            {
                FirstName = firstName,
                LastName = "Dotnet",
                Email = oktaUser.Profile.Email,
                PhoneNumber = oktaUser.Profile.PrimaryPhone,
                Password = _passwordToUse,
            };
        }

        public async Task EnrollPhoneAuthenticator()
        {
            var a18nClient = _ia18nHelper.GetClient();

            var factor = await _oktaHelper.EnrollPhoneFactor(UserProfile.Email, UserProfile.PhoneNumber);

            var passCode = await GetActivationCodeFromSms(() => _oktaHelper.ResendEnrollCode());
            await _oktaHelper.ActivateFactor(factor, UserProfile.Email, passCode);
        }

        public async Task<string> GetActivationCodeFromEmail(Action resendRequest = default)
        {
            var a18nClient = _ia18nHelper.GetClient();

            var passCode = await GetRecoveryCodeFromMessage(() => a18nClient.GetLastMessagePlainContentAsync(),
                                                                resendRequest);
            if (string.IsNullOrEmpty(passCode))
            {
                throw new Exception("Cannot get an email activation code");
            }
            await a18nClient.DeleteAllProfileEmailsAsync();
            return passCode;
        }

        public async Task<string> GetActivationCodeFromSms(Action resendRequest = default)
        {
            var a18nClient = _ia18nHelper.GetClient();

            var passCode = await GetRecoveryCodeFromMessage(() => a18nClient.GetLastSmsPlainContentAsync(), 
                                                            resendRequest);
            if (string.IsNullOrEmpty(passCode))
            {
                throw new Exception("Cannot get an sms activation code");
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

        public void SetMfaOktaSocialIdpUser()
        {
            UserProfile = new UserProfile()
            {
                Email = _configuration.OktaSocialIdpMfaUserEmail,
                Password = _configuration.OktaSocialIdpMfaUserPassword,
            };
            // This is a predetermined user and shouln't be removed.
            _keepOktaUser = true;
        }

        public string GetTOTP()
        {
            var totp = new Totp(Base32Encoding.ToBytes(TotpSharedSecret));
            var counter = 0;
            while (true)
            {
                var theCode = totp.ComputeTotp();
                if (!theCode.Equals(_lastPassCode))
                {
                    _lastPassCode = theCode;
                    return theCode;
                }
                else
                {
                    if (++counter > 10)
                    {
                        throw new Exception("Can't generate a new unique PassCode.");
                    }
                }
                System.Threading.Thread.Sleep(3000);
            }
        }

        public void GetGoogleSharedSecretFromQrCodeImage(string base64Image)
        {
            Result qrCodeContents = QrDecode(base64Image);

            var segments = qrCodeContents.Text.Split(new[] { '&', '?' });
            TotpSharedSecret = segments.FirstOrDefault(x => x.StartsWith("secret="))?.Split("=").LastOrDefault();
        }

        public async Task EnrollGoogleAuthenticator()
        {
            await _oktaHelper.AddGoogleAuthenticator(UserProfile.Email,
                                                    (sharedSecret) =>
                                                            {
                                                                TotpSharedSecret = sharedSecret;
                                                                return GetTOTP();
                                                            });
        }

        private static Result QrDecode(string base64Image)
        {
            var barcodeBitmap = GetBitmapFromBase64(base64Image);

            IBarcodeReader reader = new BarcodeReader();
            var qrCodeContents = reader.Decode(new BitmapLuminanceSource(barcodeBitmap));
            return qrCodeContents;
        }

        private static Bitmap GetBitmapFromBase64(string qrcodeContents)
        {
            var rawBase64 = qrcodeContents.Split("base64,").Last();
            byte[] bitmapData = Convert.FromBase64String(rawBase64);

            using var streamBitmap = new MemoryStream(bitmapData);
#pragma warning disable CA1416 // Validate platform compatibility
            var img = Image.FromStream(streamBitmap);
            return new Bitmap(img);
#pragma warning restore CA1416 // Validate platform compatibility
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
                OnDispose().Wait();
                CleanUpOktaUserAsync().Wait();
            }
            _disposed = true;
        }

        private async Task<string> GetRecoveryCodeFromMessage(Func<Task<string>> getMessageBodyFunc,
                                                            Action resendRequest = default)
        {
            int retry = 0;
            while (true)
            {
                for (int seconds = 0; seconds < OneAttemptTime; seconds++)
                {
                    await Task.Delay(1000);
                    try
                    {
                        var messageBody = await getMessageBodyFunc();
                        return ExtractRecoveryCodeFromMessage(messageBody);
                    }
                    catch (A18NClient.NotFoundException)
                    {
                        // expected exception when a mail box is empty
                    }
                }

                if (resendRequest == default || ++retry >= MaxRetries)
                {
                    break;
                }
                resendRequest();
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

        private async Task CleanUpOktaUserAsync(IOktaSdkHelper oktaHelper = null)
        {
            var oktaSdkHelper = oktaHelper ?? _oktaHelper;
            
            if (!string.IsNullOrEmpty(UserProfile?.Email) && !_keepOktaUser)
            {
                await oktaSdkHelper.DeleteUserAsync(UserProfile.Email);
            }
        }

        private async Task CleanUpAsync(OktaSdkHelper oktaHelper = null)
        {
            var oktaSdkHelper = oktaHelper ?? _oktaHelper;
            _ia18nHelper.CleanUpProfile();
            await CleanUpOktaUserAsync(oktaSdkHelper);
        }

    }
}
