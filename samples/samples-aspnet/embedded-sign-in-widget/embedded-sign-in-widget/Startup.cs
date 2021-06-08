using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(embedded_sign_in_widget.Startup))]

namespace embedded_sign_in_widget
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
