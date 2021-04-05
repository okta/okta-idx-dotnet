using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(direct_auth_idx.Startup))]

namespace direct_auth_idx
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
