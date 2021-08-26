using embedded_sign_in_widget_e2etests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace embedded_sign_in_widget_e2etests
{
    public abstract class BaseTestSteps
    {
        protected ITestContext _context;
        protected TestConfiguration testConfiguration;

        public BaseTestSteps(ITestContext context)
        {
            _context = context;
        }
    }
}
