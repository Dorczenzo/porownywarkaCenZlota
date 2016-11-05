using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(cenyzlota.Startup))]
namespace cenyzlota
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
