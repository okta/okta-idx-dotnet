using embedded_sign_in_widget_e2etests.Drivers;
using embedded_sign_in_widget_e2etests.PageObjectModels;
using FluentAssertions;
using OpenQA.Selenium;
using System;

namespace embedded_sign_in_widget_e2etests.PageObjectModel
{
    public class FacebookLoginPage : BasePage
        {
            public override string RelativePageUri => throw new NotImplementedException();
            public IWebElement UserNameInput => TryFindElement(By.Id("email"));
            public IWebElement PasswordInput => TryFindElement(By.Id("pass"));
            public IWebElement LoginButton => TryFindElement(By.Id("loginbutton"));

            public FacebookLoginPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration)
                : base(webDriverDriver, testConfiguration)
            { }

            public override void AssertPageOpenedAndValid()
            {
                Title.Should().StartWith("Log into Facebook");
                _webDriver.Url.Should().StartWith("https://facebook.com/");
            }
        }
    }
