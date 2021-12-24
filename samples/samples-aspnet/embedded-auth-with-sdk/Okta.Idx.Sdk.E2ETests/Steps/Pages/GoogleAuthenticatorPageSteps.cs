using embedded_auth_with_sdk.E2ETests.PageObjectModels;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.Steps.Pages
{
    [Binding]
    public class GoogleAuthenticatorPageSteps : BaseTestSteps
    {
        private GoogleAuthenticatorPage _googleAuthenticatorPageModel;
        private GoogleAuthenticatorAsyncPage _googleAuthenticatorAsyncPageModel;

        public GoogleAuthenticatorPageSteps(ITestContext context,
                GoogleAuthenticatorPage googleAuthenticatorPageModel,
                GoogleAuthenticatorAsyncPage googleAuthenticatorAsyncPageModel)
                : base(context)
        {
            _googleAuthenticatorPageModel = googleAuthenticatorPageModel;
            _googleAuthenticatorAsyncPageModel = googleAuthenticatorAsyncPageModel;
        }

        [When(@"She scans a QR Code")]
        public void WhenSheScansAQRCode()
        {
            var base64Image = _googleAuthenticatorPageModel.QrCodeImage.GetAttribute("src");
             _context.GetGoogleSharedSecretFromQrCodeImage(base64Image);
        }

        [Then(@"She enters the shared Secret Key into the Google Authenticator App")]
        [When(@"She enters the shared Secret Key into the Google Authenticator App")]
        public void ThenSheEntersTheSharedSecretKeyIntoTheGoogleAuthenticatorApp()
        {
            _context.TotpSharedSecret = _googleAuthenticatorPageModel.SharedSecretInput.GetProperty("value");
        }

        [When(@"She selects Next")]
        [When(@"She selects Next on the screen which is showing the QR code")] 
        [Then(@"She selects Next on the screen which is showing the QR code")]
        public void WhenSheSelectsNext()
        {
            _googleAuthenticatorPageModel.NextButton.Click();
        }

        [When(@"She selects Verify")]
        public void WhenSheSelectsVerify()
        {
            _googleAuthenticatorAsyncPageModel.SubmitButton.Click();
        }

        [When(@"She inputs the correct code from her Google Authenticator App")]
        public void WhenSheInputsTheCorrectCodeFromHerGoogleAuthenticatorApp()
        {
            var code = _context.GetTOTP();
            _googleAuthenticatorAsyncPageModel.PasscodeInput.SendKeys(code);
        }


        [Then(@"She sees a screen which shows a QR code and a shared secret key")]
        public void ThenSheSeesAScreenWhichShowsAQRCodeAndASharedSecretKey()
        {
            _googleAuthenticatorPageModel.AssertPageOpenedAndValid();
        }


        [Then(@"the screen changes to receive an input for a TOTP code")]
        public void ThenTheScreenChangesToReceiveAnInputForATOTPCode()
        {
            _googleAuthenticatorAsyncPageModel.AssertPageOpenedAndValid();
        }

        [Then(@"She selects ""(.*)"" on the screen which is showing the QR code")]
        public void ThenSheSelectsOnTheScreenWhichIsShowingTheQRCode(string p0)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
