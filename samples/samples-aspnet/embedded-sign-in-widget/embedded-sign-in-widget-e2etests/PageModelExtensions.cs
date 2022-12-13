using embedded_sign_in_widget_e2etests.PageObjectModels;
using OpenQA.Selenium;
using System;
using System.Threading;

namespace embedded_sign_in_widget_e2etests
{
    public static class PageModelExtensions
    {
        public static void TrySendKeys(this IWebElement element, string text)
        {
            int tryCount = 0;
            Exception thrown = null;
            int maxAttempts = 10;

            while (tryCount < maxAttempts)
            {
                try
                {
                    tryCount++;
                    element.SendKeys(text);
                    return;
                }
                catch (Exception ex)
                {
                    thrown = ex;
                }
                Thread.Sleep(1000);
            }

            throw thrown ?? new Exception($"Could not interact with the element after {maxAttempts} attempts");
        }

        public static void WaitForPageToOpen(this BasePage page)
        {
            int tryCount = 0;
            Exception thrown = null;
            int maxAttempts = 20;

            while (tryCount < maxAttempts)
            {
                try
                {
                    tryCount++;
                    page.AssertPageOpenedAndValid();
                    return;
                }
                catch (Exception ex)
                {
                    thrown = ex;
                }
                Thread.Sleep(1000);
            }

            throw thrown ?? new Exception($"Page is not opened after {maxAttempts} attempts");
        }

    }
}
