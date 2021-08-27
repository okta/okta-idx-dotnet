using Okta.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace embedded_sign_in_widget_e2etests.Helpers
{
    public class TestContext : ITestContext
    {
        private readonly ITestConfiguration _configuration;
        private readonly IOktaSdkHelper _oktaHelper;
        private bool _disposed = false;

        public TestContext(ITestConfiguration configuration, IOktaSdkHelper oktaHelper)
        {
            _oktaHelper = oktaHelper;
            _configuration = configuration;

            Configuration = ConfigBuilder.Configuration;
        }

        public ITestConfiguration Configuration 
        {
            get;
            set;
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

            _disposed = true;
        }

    }
}
