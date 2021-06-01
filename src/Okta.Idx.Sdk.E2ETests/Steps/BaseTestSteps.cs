using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Drivers;
using Okta.Idx.Sdk.E2ETests.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    public abstract class BaseTestSteps
    {
        protected TestUserProperties _testUser;
        protected ITestUserHelper _userHelper;

        public BaseTestSteps(ITestUserHelper userHelper)
        {
            _userHelper = userHelper;
        }
    }
}
