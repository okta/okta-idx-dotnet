using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace embedded_sign_in_widget_e2etests.Drivers
{
    public interface IWebServerDriver
    {
        string StartWebServer();

        void StopWebServer();

        string SiteUrl { get; }
    }
}
