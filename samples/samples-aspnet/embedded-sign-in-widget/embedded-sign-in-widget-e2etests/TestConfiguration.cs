using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace embedded_sign_in_widget_e2etests
{
    public class TestConfiguration : ITestConfiguration
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public int IisPort { get; set; }
        public string SiteUrl { get; set; }
    }
}
