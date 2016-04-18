using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestGeometryDrawWebApplication.Startup))]
namespace TestGeometryDrawWebApplication
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
