using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sharp.Startup))]
namespace Sharp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
