﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace embedded_sign_in_widget
{
    [ExcludeFromCodeCoverage]
#pragma warning disable SA1601 // Partial elements should be documented
    public partial class Startup
#pragma warning restore SA1601 // Partial elements should be documented
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });
        }
    }
}
