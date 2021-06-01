﻿using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Drivers;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk.E2ETests.PageObjectModels
{
    public class ChangePasswordPage:BasePage
    {
        public ChangePasswordPage(WebDriverDriver webDriverDriver, IWebServerDriver _webServerDriver) : base(webDriverDriver, _webServerDriver)
        { }

        public override string RelativePageUri => "Manage/ChangePassword";
        public IWebElement NewPasswordInput => _webDriver.FindElement(By.Id("NewPassword"));
        public IWebElement ConfirmPasswordInput => _webDriver.FindElement(By.Id("ConfirmPassword"));
        public IWebElement SubmitButton => _webDriver.FindElement(By.Id("SubmitBtn"));

        public override void AssertPageOpenedAndValid()
        {
            base.AssertPageOpenedAndValid();
            Title.Should().Contain("password");
        }
    }
}
