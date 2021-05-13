using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(direct_auth.Startup))]

namespace direct_auth
{
    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security.Cookies;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            app.UseCookieAuthentication(new CookieAuthenticationOptions
                                            {
                                                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                                                //LoginPath = new PathString("/Account/Login"),
                                            });
        }
    }
}
