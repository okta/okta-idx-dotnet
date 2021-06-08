using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace embedded_sign_in_widget_e2etests
{
    public interface ITestConfiguration
    {
        string UserPassword { get; set; }
        int IisPort { get; set; }
        string SiteUrl { get; set; }
    }
}
