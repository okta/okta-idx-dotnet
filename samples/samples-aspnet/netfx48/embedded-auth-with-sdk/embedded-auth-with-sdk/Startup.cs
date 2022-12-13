using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(embedded_auth_with_sdk.Startup))]

namespace embedded_auth_with_sdk
{
#pragma warning disable SA1601 // Partial elements should be documented
    public partial class Startup
#pragma warning restore SA1601 // Partial elements should be documented
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
