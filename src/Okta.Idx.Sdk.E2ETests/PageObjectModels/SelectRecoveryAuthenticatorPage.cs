using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Drivers;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Linq;

namespace Okta.Idx.Sdk.E2ETests.PageObjectModels
{
    public class SelectRecoveryAuthenticatorPage : BasePage
    {
        public SelectRecoveryAuthenticatorPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration) 
            : base(webDriverDriver, testConfiguration) { }

        public override string RelativePageUri => "Manage/SelectRecoveryAuthenticator";
        public IWebElement SubmitButton => _webDriver.FindElement(By.Id("submitBtn"));
        public ReadOnlyCollection<IWebElement> AuthenticatorsList => _webDriver.FindElements(By.XPath("//label[@name=\"AuthenticatorName\"]"));
        public IWebElement FindAuthenticatorOption(string name) => AuthenticatorsList.FirstOrDefault(x => x.Text.Contains(name));
        
        public override void AssertPageOpenedAndValid()
        {
            base.AssertPageOpenedAndValid();
            Title.Should().StartWith("Reset password");
        }
    }
}

